#if PRIME_TWEEN_INSTALLED
using PrimeTween;
using Tween = PrimeTween.Tween;
#endif
using UnityEngine;
using NUnit.Framework;
using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using Unity.PerformanceTesting;
using UnityEngine.Profiling;
using UnityEngine.Scripting;
using UnityEngine.TestTools;
using Ease = DG.Tweening.Ease;
using Assert = UnityEngine.Assertions.Assert;

public class DOTween_PrimeTweenTests {
    #if !PRIME_TWEEN_INSTALLED
    [Test]
    public void PrimeTweenIsInstalled() {
        Debug.LogError("Please install PrimeTween from Asset Store: https://assetstore.unity.com/packages/slug/252960");
    }
    #else
    internal const int warmups = 1;
    internal const int iterations = 100000;
    Transform transform;

    [OneTimeSetUp] public void oneTimeSetup() {
        if (!Application.isEditor) {
            GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
        }
        Application.targetFrameRate = SystemInfo.deviceType == DeviceType.Handheld ? 120 : -1;
        transform = new GameObject().transform;
        const int capacity = iterations + warmups;
        
        DOTween.SetTweensCapacity(capacity + 1, capacity + 1);
        DOTween.defaultEaseType = Ease.Linear;

        PrimeTweenConfig.defaultEase = PrimeTween.Ease.Linear;
        PrimeTweenConfig.SetTweensCapacity(capacity);
        PrimeTweenConfig.warnZeroDuration = false;
        PrimeTweenConfig.warnTweenOnDisabledTarget = false;
        PrimeTweenConfig.warnEndValueEqualsCurrent = false;
    }

    [UnityTearDown] public IEnumerator setUp() {
        DOTween.KillAll();
        Tween.StopAll();
        Assert.AreEqual(0, DOTween.TotalActiveSequences());
        Assert.AreEqual(0, DOTween.TotalActiveTweeners());
        Assert.AreEqual(0, DOTween.TotalActiveTweens());
        GC.Collect();
        for (int i = 0; i < 10; i++) {
            yield return null;
        }
    }

    [Test] public void _0_ProfilerDisabled() => Assert.IsFalse(Profiler.enabled, "Please disable Profiler because it influences test results.");
    [Test] public void _0_RunningOnDevice() => Assert.IsFalse(Application.isEditor, "Please run performance tests on a real device, not in Editor.");
    [Test] public void _0_PrimeTweenAssertionsDisabled() {
        #if !PRIME_TWEEN_DISABLE_ASSERTIONS
        Debug.LogError("Please disable PrimeTween asserts by adding the define: PRIME_TWEEN_DISABLE_ASSERTIONS. This will ensure you're measuring the release performance.");
        #endif
    }
    

    readonly Vector3 endValue = new Vector3(0,0,10);
    const float longDuration = 10f;
    [UnityTest, Performance] public IEnumerator _01_PositionAnimation_DOTween() => measureAverageFrameTimes(() => transform.DOMove(endValue, longDuration));
    [UnityTest, Performance] public IEnumerator _01_PositionAnimation_PrimeTween() => measureAverageFrameTimes(() => Tween.Position(transform, endValue, longDuration));
    [UnityTest, Performance] public IEnumerator _01_RotationAnimation_DOTween() => measureAverageFrameTimes(() => transform.DORotate(endValue, longDuration));
    [UnityTest, Performance] public IEnumerator _01_RotationAnimation_PrimeTween() => measureAverageFrameTimes(() => Tween.Rotation(transform, endValue, longDuration));


    float floatField;
    [UnityTest, Performance] public IEnumerator _02_CustomAnimation_DOTween() => measureAverageFrameTimes(() => DOVirtual.Float(0, 1, longDuration, val => floatField = val));
    [UnityTest, Performance] public IEnumerator _02_CustomAnimation_PrimeTween() => measureAverageFrameTimes(() => Tween.Custom(this, 0, 1, longDuration, (_this, val) => _this.floatField = val));

    
    readonly AnimationCurve animationCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [UnityTest, Performance] public IEnumerator _03_AnimationWithCustomEase_DOTween() => measureAverageFrameTimes(() => transform.DOMove(endValue, longDuration).SetEase(animationCurve));
    [UnityTest, Performance] public IEnumerator _03_AnimationWithCustomEase_PrimeTween() => measureAverageFrameTimes(() => Tween.Position(transform, endValue, longDuration, animationCurve));


    int numCallbackCalled;
    [UnityTest, Performance] public IEnumerator _04_Delay_DOTween() => measureAverageFrameTimes(() => DOVirtual.DelayedCall(longDuration, () => numCallbackCalled++));
    [UnityTest, Performance] public IEnumerator _04_Delay_PrimeTween() => measureAverageFrameTimes(() => Tween.Delay(this, longDuration, _this => _this.numCallbackCalled++));


    const float shortDuration = 0.0001f;
    [Test, Performance] public void _05_Animation_GCAlloc_DOTween() => measureGCAlloc(() => transform.DOMove(endValue, shortDuration));
    [Test, Performance] public void _14_Animation_GCAlloc_DOTween_Recycle_ClearRef() => Animation_GCAlloc_DOTween_Recycle_internal(() => {
        Tweener testTweenReference = transform.DOMove(endValue, shortDuration).OnKill(() => {
            // Clear the tween reference because it will be reused for another tween, leading to unpredictable and hard-to-detect bugs
            // This operation doesn't do anything useful under the test, but shows the unavoidable delegate allocation
            // https://dotween.demigiant.com/documentation.php?api=DOTween.Init
            testTweenReference = null;
        });
    });
    [Test, Performance] public void _14_Animation_GCAlloc_DOTween_Recycle() => Animation_GCAlloc_DOTween_Recycle_internal(() => transform.DOMove(endValue, shortDuration));
    void Animation_GCAlloc_DOTween_Recycle_internal(Action action) {
        var settings = Resources.Load<DOTweenSettings>(nameof(DOTweenSettings));
        Assert.IsNotNull(settings);
        settings.defaultRecyclable = true;
        DOTween.defaultRecyclable = true;
        
        // Create tweens and recycle them
        for (int i = 0; i < iterations; i++) {
            transform.DOMove(endValue, shortDuration);
        }
        DOTween.KillAll();
        GC.Collect();
        // Do the actual measurement
        measureGCAlloc(action);
        
        settings.defaultRecyclable = false;
        DOTween.defaultRecyclable = false;
    }
    [Test, Performance] public void _05_Animation_GCAlloc_PrimeTween() => measureGCAlloc(() => Tween.Position(transform, endValue, shortDuration), expected: 0);
    [Test, Performance] public void _06_Delay_GCAlloc_DOTween() => measureGCAlloc(() => DOVirtual.DelayedCall(shortDuration, () => numCallbackCalled++));
    [Test, Performance] public void _06_Delay_GCAlloc_PrimeTween() => measureGCAlloc(() => Tween.Delay(this, shortDuration, _this => _this.numCallbackCalled++), expected: 0);
    
    
    [UnityTest, Performance] public IEnumerator _07_Animation_Start_DOTween() => measureFrameTime(() => transform.DOMove(endValue, shortDuration));
    [UnityTest, Performance] public IEnumerator _07_Animation_Start_PrimeTween() => measureFrameTime(() => Tween.Position(transform, endValue, shortDuration));
    
    
    [UnityTest, Performance] public IEnumerator _08_Animation_Start_AllParams_DOTween() => 
         measureFrameTime(() => {
            transform.DOMove(endValue, shortDuration)
                .From(Vector3.zero)
                .SetEase(Ease.InOutBounce)
                .SetLoops(2, LoopType.Yoyo)
                .SetDelay(shortDuration)
                .SetUpdate(true);
        });
    [UnityTest, Performance] public IEnumerator _08_Animation_Start_AllParams_PrimeTween() =>
        measureFrameTime(() => {
            Tween.Position(transform, Vector3.zero, Vector3.one, shortDuration, PrimeTween.Ease.InOutBounce, 2, CycleMode.Yoyo, shortDuration, 0, true);
        });

    
    [UnityTest, Performance] public IEnumerator _09_Delay_Start_DOTween() => measureFrameTime(() => DOVirtual.DelayedCall(longDuration, () => numCallbackCalled++));
    [UnityTest, Performance] public IEnumerator _09_Delay_Start_PrimeTween() => measureFrameTime(() => Tween.Delay(this, longDuration, _this => _this.numCallbackCalled++));

    
    const int sequenceIterations = iterations / 4 - warmups;
    [UnityTest, Performance] public IEnumerator _11_Sequence_DOTween() => measureAverageFrameTimes(createSequenceDOTween, sequenceIterations);
    [UnityTest, Performance] public IEnumerator _11_Sequence_PrimeTween() => measureAverageFrameTimes(createSequencePrimeTween, sequenceIterations);
    [Test, Performance] public void _12_Sequence_GCAlloc_DOTween() => measureGCAlloc(createSequenceDOTween, sequenceIterations);
    [Test, Performance] public void _12_Sequence_GCAlloc_PrimeTween() => measureGCAlloc(createSequencePrimeTween, sequenceIterations, expected: 0);
    [UnityTest, Performance] public IEnumerator _13_SequenceStart_DOTween() => measureFrameTime(createSequenceDOTween, sequenceIterations);
    [UnityTest, Performance] public IEnumerator _13_SequenceStart_PrimeTween() => measureFrameTime(createSequencePrimeTween, sequenceIterations);
    void createSequenceDOTween() =>
        DOTween.Sequence()
            .Append(transform.DOMove(Vector3.zero, longDuration))
            .Append(transform.DOScale(Vector3.zero, longDuration))
            .Append(transform.DORotate(Vector3.zero, longDuration));
    void createSequencePrimeTween() =>
        Tween.Position(transform, Vector3.zero, longDuration)
            //.Chain(Tween.Scale(transform, Vector3.zero, longDuration))
            .Chain(Tween.Rotation(transform, Vector3.zero, longDuration));

    
    /// More iterations produce higher measurement accuracy. 
    internal static void measureGCAlloc(Action action, int _iterations = iterations, long? expected = null) {
        for (int i = 0; i < warmups; i++) {
            action();
        }
        GC.Collect();
        var allocatedMemoryBefore = GC.GetTotalMemory(true);
        for (int i = 0; i < _iterations; i++) {
            action();
        }
        var gcAllocPerIteration = (GC.GetTotalMemory(true) - allocatedMemoryBefore) / _iterations;
        Measure.Custom(new SampleGroup("GCAlloc", SampleUnit.Byte), gcAllocPerIteration);
        if (expected.HasValue) {
            Assert.AreEqual(expected.Value, gcAllocPerIteration);
        }
    }

    internal static IEnumerator measureFrameTime(Action action, int _iterations = iterations) {
        for (int i = 0; i < warmups; i++) {
            action();
        }
        using (Measure.Frames().Scope()) {
            for (int i = 0; i < _iterations; i++) {
                action();
            }
            GC.Collect();
            yield return null;
        }
    }

    internal static IEnumerator measureAverageFrameTimes(Action action, int _iterations = iterations) {
        for (int i = 0; i < warmups; i++) {
            action();
        }
        yield return null;
        for (int i = 0; i < _iterations; i++) {
            action();
        }
        GC.Collect();
        yield return null;
        yield return Measure.Frames().MeasurementCount(200).Run();
    }
    #endif // PRIME_TWEEN_INSTALLED
}