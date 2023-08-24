#if PRIME_TWEEN_INSTALLED
using PrimeTween;
using Tween = PrimeTween.Tween;
#endif
using UnityEngine;
using NUnit.Framework;
using System;
using System.Collections;
using DG.Tweening;
using Unity.PerformanceTesting;
using UnityEngine.Profiling;
using UnityEngine.Scripting;
using UnityEngine.TestTools;
using Ease = DG.Tweening.Ease;

public class BenchmarksVsDOTween {
    #if !PRIME_TWEEN_INSTALLED
    [Test]
    public void PrimeTweenIsNotInstalled() {
        Debug.LogError("Please install PrimeTween from Asset Store: https://assetstore.unity.com/packages/slug/252960");
    }
    #else
    const int warmups = 1;
    const int iterations = 100000;
    Transform transform;
    
    [OneTimeSetUp] public void oneTimeSetup() {
        Application.targetFrameRate = SystemInfo.deviceType == DeviceType.Handheld ? 120 : -1;
        transform = new GameObject().transform;
        const int capacity = iterations + warmups;
        
        DOTween.SetTweensCapacity(capacity + 1, capacity + 1); // classic off-by-one _bug in DOTween :)
        DOTween.defaultEaseType = Ease.Linear;

        PrimeTweenConfig.defaultEase = PrimeTween.Ease.Linear;
        PrimeTweenConfig.SetTweensCapacity(capacity + 100);
        PrimeTweenConfig.warnZeroDuration = false;
        PrimeTweenConfig.warnTweenOnDisabledTarget = false;
    }

    [UnitySetUp] public IEnumerator setUp() {
        if (!Application.isEditor) {
            GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
        }
        DOTween.KillAll();
        Tween.StopAll();
        GC.Collect();
        yield return null;
        Assert.AreEqual(0, DOTween.TotalActiveSequences());
        Assert.AreEqual(0, DOTween.TotalActiveTweeners());
        Assert.AreEqual(0, DOTween.TotalActiveTweens());
    }

    
    [Test] public void _0_ProfilerDisabled() => Assert.IsFalse(Profiler.enabled, "Please disable Profiler because it influences test results.");
    [Test] public void _0_RunningOnDevice() => Assert.IsFalse(Application.isEditor, "Please run the test on a real device, not in Editor.");
    [Test] public void _0_PrimeTweenAssertionsDisabled() {
        #if !PRIME_TWEEN_DISABLE_ASSERTIONS
        Debug.LogError("Please disable PrimeTween asserts by adding the define: PRIME_TWEEN_DISABLE_ASSERTIONS. This will ensure you're measuring the release performance.");
        #endif
    }

    readonly Vector3 endValue = Vector3.one;
    const float longDuration = 10f;
    [UnityTest, Performance] public IEnumerator _01_Animation_DOTween() {
        yield return measureFrameTimeAfter(() => transform.DOMove(endValue, longDuration));
    }
    [UnityTest, Performance] public IEnumerator _01_Animation_PrimeTween() {
        yield return measureFrameTimeAfter(() => Tween.Position(transform, endValue, longDuration));
    }
    
  
    float floatField;
    [UnityTest, Performance] public IEnumerator _02_CustomAnimation_DOTween() {
        yield return measureFrameTimeAfter(() => DOVirtual.Float(0, 1, longDuration, val => floatField = val));
    }
    [UnityTest, Performance] public IEnumerator _02_CustomAnimation_PrimeTween() {
        yield return measureFrameTimeAfter(() => Tween.Custom(this, 0, 1, longDuration, (_this, val) => _this.floatField = val));
    }

    
    readonly AnimationCurve animationCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [UnityTest, Performance] public IEnumerator _03_AnimationWithCustomEase_DOTween() {
        yield return measureFrameTimeAfter(() => transform.DOMove(endValue, longDuration).SetEase(animationCurve));
    }
    [UnityTest, Performance] public IEnumerator _03_AnimationWithCustomEase_PrimeTween() {
        yield return measureFrameTimeAfter(() => Tween.Position(transform, endValue, longDuration, animationCurve));
    }

    
    [UnityTest, Performance] public IEnumerator _04_Delay_DOTween() {
        yield return measureFrameTimeAfter(() => DOVirtual.DelayedCall(longDuration, () => numCallbackCalled++));
    }
    [UnityTest, Performance] public IEnumerator _04_Delay_PrimeTween() {
        yield return measureFrameTimeAfter(() => Tween.Delay(this, longDuration, _this => _this.numCallbackCalled++));
    }


    const float shortDuration = 0.0001f;
    [Test, Performance] public void _05_Animation_GC_DOTween() => measureMethod(() => transform.DOMove(endValue, shortDuration));
    [Test, Performance] public void _05_Animation_GC_PrimeTween() => measureMethod(() => Tween.Position(transform, endValue, shortDuration));
    [Test, Performance] public void _06_Delay_GC_DOTween() => measureMethod(() => DOVirtual.DelayedCall(shortDuration, () => numCallbackCalled++));
    [Test, Performance] public void _06_Delay_GC_PrimeTween() => measureMethod(() => Tween.Delay(this, shortDuration, _this => _this.numCallbackCalled++));
    static void measureMethod(Action action) {
        for (int i = 0; i < warmups; i++) {
            action();
        }
        Measure.Method(action)
            .MeasurementCount(1)
            .IterationsPerMeasurement(iterations)
            .GC() // wierd units, seems li
            .Run();
    }

    
    [UnityTest, Performance] public IEnumerator _07_Animation_Start_DOTween() {
        yield return measureFrameTime(() => _ = transform.DOMove(endValue, shortDuration));
    }
    [UnityTest, Performance] public IEnumerator _07_Animation_Start_PrimeTween() {
        yield return measureFrameTime(() => {
            Tween.Position(transform, endValue, shortDuration);
        });
    }
    
    [UnityTest, Performance] public IEnumerator _08_Animation_Start_AllParams_DOTween() {
        yield return measureFrameTime(() => {
            transform.DOMove(endValue, shortDuration)
                .From(Vector3.zero)
                .SetEase(Ease.InOutBounce)
                .SetLoops(2, LoopType.Yoyo)
                .SetDelay(shortDuration)
                .SetUpdate(true);
        });
    }
    [UnityTest, Performance] public IEnumerator _08_Animation_Start_AllParams_PrimeTween() {
        yield return measureFrameTime(() => {
            Tween.Position(transform, Vector3.zero, Vector3.one, shortDuration, PrimeTween.Ease.InOutBounce, 2, CycleMode.Yoyo, shortDuration, 0, true);
        });
    }

    
    [UnityTest, Performance] public IEnumerator _09_Delay_Start_DOTween() {
        yield return measureFrameTime(() => DOVirtual.DelayedCall(longDuration, () => numCallbackCalled++));
    }
    [UnityTest, Performance] public IEnumerator _09_Delay_Start_PrimeTween() {
        yield return measureFrameTime(() => Tween.Delay(this, longDuration, _this => _this.numCallbackCalled++));
    }

    
    const float delayStartEndDuration = 0.1f;
    int numCallbackCalled;
    const int delayStartEndCount = iterations;
    /// DOTween measures time incorrectly with more than 5000 delays (Mac M1 IL2CPP; Mac M1 Editor) 
    [UnityTest, Performance] public IEnumerator _10_Delay_StartEnd_DOTween_BUGGED() {
        using (Measure.Frames().Scope()) {
            numCallbackCalled = 0;
            TweenCallback tweenCallback = () => numCallbackCalled++;
            for (int i = 0; i < delayStartEndCount; i++) {
                DOVirtual.DelayedCall(delayStartEndDuration, tweenCallback);
            }
            GC.Collect();
            while (numCallbackCalled != delayStartEndCount) {
                yield return null;
            }
            GC.Collect();
            yield return null;
        }
    }
    [UnityTest, Performance] public IEnumerator _10_Delay_StartEnd_PrimeTween() {
        using (Measure.Frames().Scope()) {
            numCallbackCalled = 0;
            Action<BenchmarksVsDOTween> onComplete = _this => _this.numCallbackCalled++;
            for (int i = 0; i < delayStartEndCount; i++) {
                Tween.Delay(this, delayStartEndDuration, onComplete);
            }
            GC.Collect();
            while (numCallbackCalled != delayStartEndCount) {
                yield return null;
            }
            GC.Collect();
            yield return null;
        }
    }

    
    static IEnumerator measureFrameTime(Action action) {
        for (int i = 0; i < warmups; i++) {
            action();
        }
        using (Measure.Frames().Scope()) {
            for (int i = 0; i < iterations; i++) {
                action();
            }
            GC.Collect();
            for (int i = 0; i < 1; i++) {
                yield return null;
            }
        }
    }
    static IEnumerator measureFrameTimeAfter(Action action) {
        for (int i = 0; i < warmups; i++) {
            action();
        }
        yield return null;
        for (int i = 0; i < iterations; i++) {
            action();
        }
        GC.Collect();
        yield return Measure.Frames().MeasurementCount(50).Run();
    }
    #endif // PRIME_TWEEN_INSTALLED
}
