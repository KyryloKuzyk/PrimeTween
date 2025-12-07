using System;
using System.Collections;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.TestTools;

public class LeanTweenTests {
    static bool initializedOnce;
    Transform transform;
    [OneTimeSetUp] public void oneTimeSetup() {
        Assert.IsFalse(initializedOnce, "LeanTween doesn't support disabled Domain Reload. Please enabled Domain Reload or recompile scripts");
        initializedOnce = true;
        transform = new GameObject().transform;
        if (!Application.isEditor) {
            GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
        }

        var capacity = DOTween_PrimeTweenTests.iterations + 1;
        LeanTween.init(capacity, capacity);
    }

    [UnityTearDown] public IEnumerator setUp() {
        LeanTween.cancelAll();
        GC.Collect();
        yield return null;
    }
    
    readonly Vector3 endValue = new Vector3(0,0,10);
    const float longDuration = 10f;
    [UnityTest, Performance] public IEnumerator _01_PositionAnimation_LeanTween() => measureAverageFrameTimes(() => startPositionAnimation());
    LTDescr startPositionAnimation() => LeanTween.move(transform.gameObject, endValue, longDuration);
    [UnityTest, Performance] public IEnumerator _01_RotationAnimation_LeanTween() => measureAverageFrameTimes(() => LeanTween.rotate(transform.gameObject, endValue, longDuration));
    float floatField;
    [UnityTest, Performance] public IEnumerator _02_CustomAnimation_LeanTween() => measureAverageFrameTimes(() => LeanTween.value(0, 1, longDuration).setOnUpdate(val => floatField = val));
    readonly AnimationCurve animationCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [UnityTest, Performance] public IEnumerator _03_AnimationWithCustomEase_LeanTween() => measureAverageFrameTimes(() => startPositionAnimation().setEase(animationCurve));
    int numCallbackCalled;
    [UnityTest, Performance] public IEnumerator _04_Delay_LeanTween() => measureAverageFrameTimes(() => startDelay());
    void startDelay() => LeanTween.delayedCall(longDuration, _this => (_this as LeanTweenTests).numCallbackCalled++).setOnCompleteParam(this);

    const float shortDuration = 0.0001f;
    [Test, Performance] public void _05_Animation_GCAlloc_LeanTween() => DOTween_PrimeTweenTests.measureGCAlloc(() => startPositionAnimation());
    [Test, Performance] public void _06_Delay_GCAlloc_LeanTween() => measureGCAlloc(() => startDelay());

    [UnityTest, Performance] public IEnumerator _07_Animation_Start_LeanTween() => measureFrameTime(() => startPositionAnimation());
    [UnityTest, Performance] public IEnumerator _08_Animation_Start_AllParams_LeanTween() => measureFrameTime(() => {
        startPositionAnimation()
            .setFrom(Vector3.zero)
            .setEase(LeanTweenType.easeInOutBounce)
            .setLoopCount(2)
            .setLoopType(LeanTweenType.pingPong)
            .setDelay(shortDuration)
            .setIgnoreTimeScale(true);
    });
    [UnityTest, Performance] public IEnumerator _09_Delay_Start_LeanTween() => measureFrameTime(() => startDelay());

    const int warmups = DOTween_PrimeTweenTests.warmups;
    const int iterations = DOTween_PrimeTweenTests.iterations;
    const int sequenceIterations = iterations / 3 - warmups;
    [UnityTest, Performance] public IEnumerator _11_Sequence_LeanTween() => measureAverageFrameTimes(createSequenceLeanTween, sequenceIterations);
    [Test, Performance] public void _12_Sequence_GCAlloc_LeanTween() => measureGCAlloc(createSequenceLeanTween, sequenceIterations);
    [UnityTest, Performance] public IEnumerator _13_SequenceStart_LeanTween() => measureFrameTime(createSequenceLeanTween, sequenceIterations);
    void createSequenceLeanTween() =>
        LeanTween.sequence()
            .append(LeanTween.move(transform.gameObject, Vector3.zero, longDuration))
            .append(LeanTween.scale(transform.gameObject, Vector3.zero, longDuration))
            .append(LeanTween.rotate(transform.gameObject, Vector3.zero, longDuration));

    static IEnumerator measureAverageFrameTimes(Action action, int _iterations = iterations) => DOTween_PrimeTweenTests.measureAverageFrameTimes(action);
    static void measureGCAlloc(Action action, int _iterations = iterations) => DOTween_PrimeTweenTests.measureGCAlloc(action, _iterations);
    internal static IEnumerator measureFrameTime(Action action, int _iterations = iterations) => DOTween_PrimeTweenTests.measureFrameTime(action, _iterations);
}