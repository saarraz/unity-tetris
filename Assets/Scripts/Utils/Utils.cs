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
        yield return Animate(duration, Tween.EaseLinear, renderFrames);
    }

    public static IEnumerator Animate(float duration, AnimationCurve? curve, params Action<float>[] renderFrames)
    {
        var startTime = Time.fixedTime;
        for (var elapsed = 0f; elapsed <= duration; elapsed = Math.Min(duration, Time.fixedTime - startTime))
        {
            var progress = elapsed / duration;
            if (curve != null)
            {
                progress = curve.Evaluate(progress);
            }
            foreach (var renderFrame in renderFrames)
            {
                renderFrame(progress);
            }
            yield return new WaitForFixedUpdate();
            if (elapsed == duration)
            {
                break;
            }
        }
    }
}