#if MAGIC_TWEEN_INSTALLED
using System;
using System.Collections;
using MagicTween;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.TestTools;

public class MagicTweenTests {
    Transform transform;

    [OneTimeSetUp]
    public void oneTimeSetup() {
        transform = new GameObject().transform;
        if (!Application.isEditor) {
            GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
        }
    }

    [UnitySetUp]
    public IEnumerator setUp() {
        Tween.Clear();
        yield return null;
        GC.Collect();
        yield return null;
    }

    readonly Vector3 endValue = new Vector3(0,0,10);
    const float longDuration = 10f;
    [UnityTest, Performance] public IEnumerator _01_PositionAnimation_MagicTween() => DOTween_PrimeTweenTests.measureAverageFrameTimes(() => transform.TweenPosition(endValue, longDuration));
    readonly Quaternion targetRotation = Quaternion.Euler(0,0,10);
    [UnityTest, Performance] public IEnumerator _01_RotationAnimation_MagicTween() => DOTween_PrimeTweenTests.measureAverageFrameTimes(() => transform.TweenRotation(targetRotation, longDuration));
    const float shortDuration = 0.0001f;
    [UnityTest, Performance] public IEnumerator _07_Animation_Start_MagicTween() => DOTween_PrimeTweenTests.measureFrameTime(() => transform.TweenPosition(endValue, shortDuration));
    [Test, Performance] public void _05_Animation_GCAlloc_MagicTween() => DOTween_PrimeTweenTests.measureGCAlloc(() => transform.TweenPosition(endValue, shortDuration));
}
#endif