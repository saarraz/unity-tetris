using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public GameController Controller;
    public GameObject Body;
    private ContactFilter2D CollisionFilter;

    private void Start()
    {
        Controller = FindObjectOfType<GameController>();
        CollisionFilter = new ContactFilter2D();
        CollisionFilter.SetLayerMask(new LayerMask() { value = LayerMask.GetMask("Default") });
    }

    private const float TerminateDelaySeconds = 3;
    private const float FallRateSeconds = 1;
    private const float FastFallRateSeconds = .1f;
    private const float InputRepeatRateSeconds = FastFallRateSeconds;
    public const float BlockSize = 1;

    private float LastMoveTime = 0;
    private Timer HardTerminateTimer = new Timer(TerminateDelaySeconds, false);
    private Timer SoftTerminateTimer = new Timer(FallRateSeconds, false);
    private Timer FallTimer = new Timer(FallRateSeconds);
    private Timer MoveInputTimer = new Timer(InputRepeatRateSeconds);

    private RaycastHit2D[] hits = new RaycastHit2D[3];


    bool isExternalCollider(Collider2D collider)
    {
        var potentialBlock = collider;
        var potentialBody = collider.transform.parent?.gameObject;
        var potentialTetromino = potentialBody?.transform?.parent?.gameObject;
        return potentialTetromino != gameObject;
    }

    bool CanMove(Vector2 direction)
    {
        foreach (var collider in Body.GetComponentsInChildren<BoxCollider2D>())
        {
            int collisions = collider.Raycast(direction, CollisionFilter, hits, BlockSize);
            for (int i = 0; i < collisions; ++i)
            {
                if (isExternalCollider(hits[i].collider))
                {
                    return false;
                }
            }
        }
        return true;
    }

    IEnumerator OnMoveImpl()
    {
        yield return new WaitForFixedUpdate();
        if (CanMove(Vector2.down))
        {
            HardTerminateTimer.Reset(true);
            SoftTerminateTimer.Reset(true);
            yield break;
        }
        if (!HardTerminateTimer.Running)
        {
            HardTerminateTimer.Start();
        }
        SoftTerminateTimer.Start();
    }

    void OnMove()
    {
        StartCoroutine(OnMoveImpl());
    }

    bool Move(Vector2 direction)
    {
        if (!CanMove(direction))
        {
            return false;
        }
        transform.position += new Vector3(direction.x, direction.y);
        OnMove();
        return true;
    }

    void Terminate()
    {
        Controller.OnTetrominoTermination(this);
    }

    private Collider2D[] colliders = new Collider2D[5];

    void Rotate(Vector3 displacement)
    {
        Body.transform.Rotate(0, 0, 90);
        transform.position += displacement;
        OnMove();
    }

    Vector3[] RotationDisplacements = new Vector3[] {
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

    IEnumerator RotateIfPossible()
    {
        HardTerminateTimer.Pause();
        SoftTerminateTimer.Pause();
        FallTimer.Pause();
        MoveInputTimer.Pause();

        var rotationChecker = Instantiate(Body, this.transform);
        foreach (SpriteRenderer sprite in rotationChecker.GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.enabled = false;
        }
        try
        {
            rotationChecker.transform.Rotate(0, 0, 90);
            foreach (Vector3 displacement in RotationDisplacements)
            {
                if (displacement.x != 0 || displacement.y != 0)
                {
                    rotationChecker.transform.localPosition = displacement;
                }
                yield return new WaitForFixedUpdate();
                bool overlap = false;
                foreach (var collider in rotationChecker.GetComponentsInChildren<BoxCollider2D>())
                {
                    int colliderCount = collider.OverlapCollider(CollisionFilter, colliders);

                    for (int j = 0; j < colliderCount; ++j)
                    {
                        if (isExternalCollider(colliders[j]))
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
            HardTerminateTimer.Resume();
            SoftTerminateTimer.Resume();
            FallTimer.Resume();
            MoveInputTimer.Resume();
        }
    }

    void Update()
    {
        // Move left/right
        bool MoveInputTimerTick = MoveInputTimer.OnUpdate();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2.left);
            MoveInputTimer.Reset();
        }
        else if (MoveInputTimerTick && Input.GetKey(KeyCode.LeftArrow))
        {
            Move(Vector2.left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2.right);
            MoveInputTimer.Reset();
        }
        else if (MoveInputTimerTick && Input.GetKey(KeyCode.RightArrow))
        {
            Move(Vector2.right);
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

        bool SoftTerminateTimerTick = SoftTerminateTimer.OnUpdate();
        // Check if any movement happened since last soft timer tick - if not, terminate now.
        if (SoftTerminateTimerTick)
        {
            Terminate();
        }

        bool HardTerminateTimerTick = HardTerminateTimer.OnUpdate();
        // When the hard timer ticks - terminate the piece even if the player is still moving it.
        if (HardTerminateTimerTick)
        {
            Terminate();
        }

        // Downward acceleration
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            FallTimer.Frequency = FastFallRateSeconds;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            FallTimer.Frequency = FallRateSeconds;
        }
    }
}
