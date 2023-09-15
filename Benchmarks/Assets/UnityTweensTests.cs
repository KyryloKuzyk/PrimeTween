using System;
using System.Collections;
using NUnit.Framework;
using Tweens;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.TestTools;

public class UnityTweensTests {
    Transform transform;
    [OneTimeSetUp] public void oneTimeSetup() {
        transform = new GameObject().transform;
        if (!Application.isEditor) {
            GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
        }
    }

    [UnityTearDown] public IEnumerator setUp() {
        transform.gameObject.CancelTweens();
        GC.Collect();
        yield return null;
    }
    
    readonly Vector3 endValue = new Vector3(0,0,10);
    const float longDuration = 10f;
    [UnityTest, Performance] public IEnumerator _01_PositionAnimation_UnityTweens() => measureAverageFrameTimes(() => startPositionAnimation());
    TweenInstance<Transform, Vector3> startPositionAnimation() => transform.gameObject.AddTween(new PositionTween() { to = endValue, duration = longDuration });

    readonly Quaternion targetRotation = Quaternion.Euler(0,0,10);
    [UnityTest, Performance] public IEnumerator _01_RotationAnimation_UnityTweens() => measureAverageFrameTimes(() => transform.gameObject.AddTween(new RotationTween(){to = targetRotation, duration = longDuration}));
    float floatField;
    [UnityTest, Performance] public IEnumerator _02_CustomAnimation_UnityTweens() 
        => measureAverageFrameTimes(() => transform.gameObject.AddTween(new FloatTween(){from = 0, to = 1, duration = longDuration, onUpdate = (_, val) => floatField = val}));
    readonly AnimationCurve animationCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [UnityTest, Performance] public IEnumerator _03_AnimationWithCustomEase_UnityTweens() => measureAverageFrameTimes(() 
        => transform.gameObject.AddTween(new PositionTween() { to = endValue, duration = longDuration, animationCurve = animationCurve }));
    int numCallbackCalled;
    [UnityTest, Performance] public IEnumerator _04_Delay_UnityTweens() => measureAverageFrameTimes(() => startDelay());
    void startDelay() => transform.gameObject.AddTween(new FloatTween(){delay = longDuration, onEnd = delegate { numCallbackCalled++; }});

    const float shortDuration = 0.0001f;
    [Test, Performance] public void _05_Animation_GCAlloc_UnityTweens() => DOTween_PrimeTweenTests.measureGCAlloc(() => startPositionAnimation());
    [Test, Performance] public void _06_Delay_GCAlloc_UnityTweens() => measureGCAlloc(() => startDelay());

    [UnityTest, Performance] public IEnumerator _07_Animation_Start_UnityTweens() => measureFrameTime(() => startPositionAnimation());
    [UnityTest, Performance] public IEnumerator _08_Animation_Start_AllParams_UnityTweens() => measureFrameTime(() => {
        transform.gameObject.AddTween(new PositionTween() {
            to = endValue,
            duration = longDuration,
            from = Vector3.zero,
            easeType = EaseType.BounceInOut,
            loops = 2,
            usePingPong = true,
            delay = shortDuration,
            useUnscaledTime = true
        });
    });
    [UnityTest, Performance] public IEnumerator _09_Delay_Start_UnityTweens() => measureFrameTime(() => startDelay());


    const int warmups = DOTween_PrimeTweenTests.warmups;
    const int iterations = DOTween_PrimeTweenTests.iterations;
    const int sequenceIterations = iterations / 3 - warmups; 
    static IEnumerator measureAverageFrameTimes(Action action, int _iterations = iterations) => DOTween_PrimeTweenTests.measureAverageFrameTimes(action, _iterations);
    static void measureGCAlloc(Action action, int _iterations = iterations) => DOTween_PrimeTweenTests.measureGCAlloc(action, _iterations);
    internal static IEnumerator measureFrameTime(Action action, int _iterations = iterations) => DOTween_PrimeTweenTests.measureFrameTime(action, _iterations);
}