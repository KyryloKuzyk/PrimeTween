#if UI_ELEMENTS_MODULE_INSTALLED
using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using PrimeTween;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using Assert = UnityEngine.Assertions.Assert;
using Easing = UnityEngine.UIElements.Experimental.Easing;

public class UIToolkit_PrimeTweenTests {
    const int iterations = 100000;
    const int duration = 1;
    readonly VisualElement visualElement = new VisualElement();
    readonly WaitForSeconds wait = new WaitForSeconds(duration);
    readonly MethodInfo cancelAllAnimationsMethod = typeof(VisualElement).GetMethod("UnityEngine.UIElements.IStylePropertyAnimations.CancelAllAnimations", BindingFlags.NonPublic | BindingFlags.Instance);

    [OneTimeSetUp] public void oneTimeSetup() {
        PrimeTweenConfig.SetTweensCapacity(iterations);
        if (!Application.isEditor) {
            GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
        }
        initUIToolkitTests();
    }
    
    [UnitySetUp]
    public IEnumerator setUp() {
        Tween.StopAll();
        if (cancelAllAnimationsMethod != null) {
            cancelAllAnimationsMethod.Invoke(visualElement, null);
        } else {
            Debug.LogWarning("CancelAllAnimations() method is not found on VisualElement. Please run UIToolkit tests one by one independently of other tests to measure performance correctly.");
        }
        yield return wait;
        GC.Collect();
    }
    
    void initUIToolkitTests() {
        var panelSettings = Resources.Load<PanelSettings>("UIToolkit_PrimeTweenTests/TestPanelSettings");
        Assert.IsNotNull(panelSettings);
        var uiDocument = new GameObject("UIToolkit_test_object").AddComponent<UIDocument>();
        uiDocument.panelSettings = panelSettings;
        uiDocument.rootVisualElement.Add(visualElement);
    }
    
    
    [Test] public void _0_PrimeTweenAssertionsDisabled() {
        #if !PRIME_TWEEN_DISABLE_ASSERTIONS
        Debug.LogError("Please disable PrimeTween asserts by adding the define: PRIME_TWEEN_DISABLE_ASSERTIONS. This will ensure you're measuring the release performance.");
        #endif
    }
    
    
    [UnityTest, Performance] public IEnumerator _01_Animation__UIToolkit() {
        yield return measureAverageFrameTimes(startAnimationUIToolkit);
        yield return wait;
    }
    [UnityTest, Performance] public IEnumerator _01_Animation_PrimeTween() {
        yield return measureAverageFrameTimes(startAnimationPrimeTween);
    }
    
    
    [UnityTest, Performance] public IEnumerator _02_AnimationLinearEase__UIToolkit() {
        yield return measureAverageFrameTimes(() => {
            visualElement.experimental.animation.Position(Vector3.one, duration * 1000).autoRecycle = true;
        });
        yield return wait;
    }
    [UnityTest, Performance] public IEnumerator _02_AnimationLinearEase_PrimeTween() {
        yield return measureAverageFrameTimes(() => {
            Tween.Position(visualElement, Vector3.one, duration, Ease.Linear);
        });
    }


    [UnityTest, Performance] public IEnumerator _03_Animation_GCAlloc__UIToolkit() {
        measureGCAlloc(startAnimationUIToolkit);
        yield return wait;
    }
    [Test, Performance] public void _03_Animation_GCAlloc_PrimeTween() => measureGCAlloc(startAnimationPrimeTween);


    [UnityTest, Performance] public IEnumerator _04_Animation_Start__UIToolkit() {
        yield return measureFrameTime(startAnimationUIToolkit);
        yield return wait;
    }
    [UnityTest, Performance] public IEnumerator _04_Animation_Start_PrimeTween() { yield return measureFrameTime(startAnimationPrimeTween); }

    
    readonly Func<float, float> uiToolkitInOutSineEase = Easing.InOutSine;
    void startAnimationUIToolkit() {
        visualElement.experimental.animation.Position(Vector3.one, duration * 1000).Ease(uiToolkitInOutSineEase).autoRecycle = true;
    }
    void startAnimationPrimeTween() {
        Tween.Position(visualElement, Vector3.one, duration, Ease.InOutSine);
    }

    static void measureGCAlloc(Action action) {
        GC.Collect();
        var allocatedMemoryBefore = GC.GetTotalMemory(true);
        for (int i = 0; i < iterations; i++) {
            action();
        }
        var gcAllocPerIteration = (GC.GetTotalMemory(true) - allocatedMemoryBefore) / iterations;
        Measure.Custom(new SampleGroup("GCAlloc", SampleUnit.Byte), gcAllocPerIteration);
    }
    static IEnumerator measureFrameTime(Action action) {
        using (Measure.Frames().Scope()) {
            for (int i = 0; i < iterations; i++) {
                action();
            }
            GC.Collect();
            yield return null;
        }
    }
    static IEnumerator measureAverageFrameTimes(Action action) {
        for (int i = 0; i < iterations; i++) {
            action();
        }
        GC.Collect();
        return Measure.Frames().MeasurementCount(50).Run();
    }
}
#endif