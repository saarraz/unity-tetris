#nullable enable

using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using Pixelplacement;

public static class Utils
{
    public static IEnumerator WhilePlaying(this Animation animation)
    {
        do
        {
            yield return null;
        } while (animation.isPlaying);
    }
    public static IEnumerator PlayUntilEvent(this Animation animation, AnimationEvent animationEvent)
    {
        yield return new WaitForSeconds(animationEvent.time);
    }


    public static IEnumerator PlayUntilEnd(this Animation animation, string animationName)
    {
        animation.PlayQueued(animationName);
        yield return animation.WhilePlaying();
    }

    public static IEnumerator PlayUntilEvent(this Animation animation, string animationName, AnimationEvent animationEvent)
    {
        animation.PlayQueued(animationName);
        yield return animation.PlayUntilEvent(animationEvent);
    }

    public static IEnumerator Animate(float duration, params Action<float>[] renderFrames)
    {
        yield return Animate(duration, null, null, renderFrames);
    }

    public static IEnumerator Animate(float duration, AnimationCurve? curve, params Action<float>[] renderFrames)
    {
        yield return Animate(duration, curve, null, renderFrames);
    }

    public static IEnumerator Animate(float duration, Func<bool> shouldCancel, params Action<float>[] renderFrames)
    {
        yield return Animate(duration, null, shouldCancel, renderFrames);
    }

    public static IEnumerator Animate(float duration, AnimationCurve? curve, Func<bool>? shouldCancel, params Action<float>[] renderFrames)
    {
        var startTime = Time.fixedTime;
        for (var elapsed = 0f; elapsed <= duration; elapsed = Math.Min(duration, Time.fixedTime - startTime))
        {
            var progress = shouldCancel?.Invoke() == true ? 1f : elapsed / duration;
            if (curve != null)
            {
                progress = curve.Evaluate(progress);
            }
            foreach (var renderFrame in renderFrames)
            {
                renderFrame(progress);
            }
            yield return new WaitForFixedUpdate();
            if (progress == 1)
            {
                break;
            }
        }
    }

    public class OutStruct<T> where T : struct
    {
        private T? _value;
        public T Value
        {
            get
            {
                Debug.Assert(_value.HasValue);
                return _value!.Value;
            }
            set
            {
                _value = value;
            }
        }
    }

    public class OutClass<T> where T : class
    {
        private T? _value;
        public T Value
        {
            get
            {
                Debug.Assert(_value != null);
                return _value!;
            }
            set
            {
                _value = value;
            }
        }
    }
}