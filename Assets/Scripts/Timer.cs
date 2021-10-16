using UnityEngine;

public class Timer
{
    public float LastTickTime { get; private set; }
    public float Frequency
    {
        get => _frequency;

        set
        {
            RemainingDuration -= _frequency - value;
            _frequency = value;
        }
    }
    public bool Running { get; private set; }
    public bool Paused { get; private set; }

    private float _referenceTime;
    private float? RemainingDuration;
    private float _frequency;


    public void Start()
    {
        Reset();
        Running = true;
    }

    public void Pause()
    {
        if (!Running || Paused)
        {
            return;
        }
        Paused = true;
        RemainingDuration -= Time.time - _referenceTime;
    }

    public void Resume()
    {
        if (!Running || !Paused)
        {
            return;
        }
        Paused = false;
        _referenceTime = Time.time;
    }

    public Timer(float frequency = 1f, bool start = true)
    {
        Frequency = frequency;
        Running = start;
    }

    public bool OnUpdate()
    {
        if (!Running || Paused)
        {
            return false;
        }
        if ((Time.time - _referenceTime) < RemainingDuration)
        {
            return false;
        }
        LastTickTime = Time.time;
        Reset();
        return true;
    }

    public void Reset(bool Stop = false)
    {
        _referenceTime = Time.time;
        RemainingDuration = Frequency;
        Running = !Stop;
    }
}
