#nullable enable

using System.Collections;
using UnityEngine;
using Pixelplacement;
using System;

public class Tetromino : MonoBehaviour
{
    public GameController? Controller;
    public GameObject? Body;
    private ContactFilter2D CollisionFilter;

    public void Start()
    {
        Controller = FindObjectOfType<GameController>();
        CollisionFilter = new ContactFilter2D();
        CollisionFilter.SetLayerMask(new LayerMask() { value = LayerMask.GetMask("Default") });
        FallTimer.Start();
    }

    // Delay from when the Tetromino touches the ground until the turn ends, even if the player keeps moving the block.
    public const float HardTerminateDelaySeconds = 3;

    // Tetromino normal fall rate.
    public const float FallRateSeconds = 1;

    // Tetromino accelerated fall rate (when pressing DOWN).
    public const float FastFallRateSeconds = .025f;

    // How often to consider prolonged presses of right and left as separate inputs.
    public const float InputRepeatRateSeconds = .1f;

    // X and Y size of each block.
    public const float BlockSize = 1;

    private bool _paused = false;
    private readonly Timer HardTerminateTimer = new Timer(HardTerminateDelaySeconds, false);
    private readonly Timer SoftTerminateTimer = new Timer(FallRateSeconds, false);
    private readonly Timer FallTimer = new Timer(FallRateSeconds, false);
    private readonly Timer RepeatedInputTimer = new Timer(InputRepeatRateSeconds);

    public void Update()
    {
        // Move left
        bool RepeatedInputTimerTick = RepeatedInputTimer.OnUpdate();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2.left);
            RepeatedInputTimer.Reset();
        }
        else if (RepeatedInputTimerTick && Input.GetKey(KeyCode.LeftArrow))
        {
            Move(Vector2.left);
        }

        // Move right
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2.right);
            RepeatedInputTimer.Reset();
        }
        else if (RepeatedInputTimerTick && Input.GetKey(KeyCode.RightArrow))
        {
            Move(Vector2.right);
        }

        // Downward acceleration
        if (Input.GetKey(KeyCode.DownArrow))
        {
            FallTimer.Frequency = FastFallRateSeconds;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            FallTimer.Frequency = FallRateSeconds;
        }

        // Rotate
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(RotateIfPossible());
        }

        // Fall
        bool FallTimerTick = FallTimer.OnUpdate();
        if (FallTimerTick)
        {
            Move(Vector2.down);
        }

        // Soft terminate - terminate if the Tetromino is touching something below and hasn't been moved for a certain
        // period. This allows players to still move the Tetromino after it has been touched down, but if they don't
        // want to - terminates the Tetromino.
        bool SoftTerminateTimerTick = SoftTerminateTimer.OnUpdate();
        if (SoftTerminateTimerTick)
        {
            Terminate();
        }

        // Hard terminate - terminate if the block has been touching something below for a certain period, regardless
        // of movement. This prevents players from moving the piece endlessly left and right across the floor.
        bool HardTerminateTimerTick = HardTerminateTimer.OnUpdate();
        if (HardTerminateTimerTick)
        {
            Terminate();
        }
    }

    // Check if the collider is not part of this Tetromino, meaning it is either a wall or an already laid-down Block.
    bool IsExternalCollider(Collider2D collider)
    {
        var potentialBlock = collider;
        var potentialBody = potentialBlock.transform.parent?.gameObject;
        var potentialTetromino = potentialBody?.transform?.parent?.gameObject;
        return potentialTetromino != gameObject;
    }

    private readonly RaycastHit2D[] _hits = new RaycastHit2D[3];

    bool CanMove(Vector2 direction)
    {
        foreach (var collider in Body!.GetComponentsInChildren<BoxCollider2D>())
        {
            int collisions = collider.Raycast(direction, CollisionFilter, _hits, BlockSize);
            for (int i = 0; i < collisions; ++i)
            {
                if (IsExternalCollider(_hits[i].collider))
                {
                    return false;
                }
            }
        }
        return true;
    }


    float DefaultSmoothMovementDuration()
    {
        return FallTimer.Frequency / 7;
    }


    bool Move(Vector2 direction)
    {
        if (!CanMove(direction))
        {
            return false;
        }
        StartCoroutine(SmoothMovement(new Vector3(direction.x, direction.y), then: OnMove));
        return true;
    }

    public class PausedScope : System.IDisposable
    {
        public Tetromino _tetromino;

        public PausedScope(Tetromino tetromino)
        {
            _tetromino = tetromino;
            _tetromino._paused = true;
            _tetromino.HardTerminateTimer.Pause();
            _tetromino.SoftTerminateTimer.Pause();
            _tetromino.FallTimer.Pause();
            _tetromino.RepeatedInputTimer.Pause();
        }


        public void Dispose()
        {
            _tetromino._paused = false;
            _tetromino.HardTerminateTimer.Resume();
            _tetromino.SoftTerminateTimer.Resume();
            _tetromino.FallTimer.Resume();
            _tetromino.RepeatedInputTimer.Resume();
        }
    }

    IEnumerator SmoothMovement(Vector3 offset, float? rotateAngle = null, float? duration = null, Action? then = null)
    {
        while (_paused)
        {
            yield return new WaitForFixedUpdate();
        }
        using (new PausedScope(this)) {
            duration ??= DefaultSmoothMovementDuration();
            var positionBefore = transform.position;
            var targetPosition = positionBefore + offset;
            var rotationBefore = Body!.transform.localRotation;
            yield return Utils.Animate(
                duration.Value,
                Tween.EaseOutBack,
                t => transform.position = Vector3.Lerp(positionBefore, targetPosition, t),
                t => {
                    if (rotateAngle.HasValue)
                    {
                        Body!.transform.localRotation = rotationBefore * Quaternion.Euler(0, 0, t * rotateAngle.Value);
                    }
                }
            );
            then?.Invoke();
        }
        yield break;
    }

    IEnumerator OnMoveImpl()
    {
        yield return new WaitForFixedUpdate();
        if (CanMove(Vector2.down))
        {
            HardTerminateTimer.Reset(stop: true);
            SoftTerminateTimer.Reset(stop: true);
            yield break;
        }
        // Hard terminate timer is only reset if it stops touching down (terminate even if player keeps moving).
        if (!HardTerminateTimer.Running)
        {
            HardTerminateTimer.Start();
        }
        // Soft timer is reset whenever the player moves (will only tick if player decides to stop moving).
        SoftTerminateTimer.Start();
    }

    void OnMove()
    {
        StartCoroutine(OnMoveImpl());
    }

    // Terminate this tetromino, letting the GameController break it down and spawn another one.
    void Terminate()
    {
        Controller!.OnTetrominoTermination(this);
    }

    void Rotate(Vector3 displacement)
    {
        StartCoroutine(SmoothMovement(displacement, rotateAngle: 90, then: OnMove));
    }

    private readonly Vector3[] _rotationDisplacements = new Vector3[] {
        new Vector3( 0, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 2, 0),
        new Vector3( 1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(2, 0, 0),
        new Vector3(-2, 0, 0),
        new Vector3(3, 0, 0),
        new Vector3(-3, 0, 0)
    };

    private readonly Collider2D[] _colliders = new Collider2D[5];

    IEnumerator RotateIfPossible()
    {
        while (_paused)
        {
            yield return new WaitForFixedUpdate();
        }
        using (new PausedScope(this))
        {
            // Check if we can rotate by copying a transparent version of the body, rotating it and trying to find a short
            // displacement that makes it not overlap any other block or wall.
            var rotationChecker = Instantiate(Body, this.transform);
            foreach (SpriteRenderer sprite in rotationChecker!.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.enabled = false;
            }
            try
            {
                rotationChecker.transform.Rotate(0, 0, 90);
                foreach (Vector3 displacement in _rotationDisplacements)
                {
                    if (displacement.x != 0 || displacement.y != 0)
                    {
                        rotationChecker.transform.localPosition = displacement;
                    }
                    yield return new WaitForFixedUpdate();
                    bool overlap = false;
                    foreach (var collider in rotationChecker.GetComponentsInChildren<BoxCollider2D>())
                    {
                        int colliderCount = collider.OverlapCollider(CollisionFilter, _colliders);

                        for (int j = 0; j < colliderCount; ++j)
                        {
                            if (IsExternalCollider(_colliders[j]))
                            {
                                overlap = true;
                                break;
                            }
                        }
                    }
                    if (!overlap)
                    {
                        Rotate(displacement);
                        break;
                    }
                }
            }
            finally
            {
                DestroyImmediate(rotationChecker);
            }
        }
    }
}
