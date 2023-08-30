#if UI_ELEMENTS_MODULE_INSTALLED
using System;
using System.Collections;
using NUnit.Framework;
using PrimeTween;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using Assert = UnityEngine.Assertions.Assert;

public class PrimeTween_VS_UIToolkit {
    const int iterations = 10000;
    const int duration = 2;
    readonly VisualElement[] visualElements = new VisualElement[iterations];
    readonly WaitForSeconds wait = new WaitForSeconds(duration);
    int numUIToolkitTestsDone;

    [OneTimeSetUp] public void oneTimeSetup() {
        PrimeTweenConfig.SetTweensCapacity(iterations);
        initUIToolkitTests();
    }

    [UnitySetUp]
    public IEnumerator setUp() {
        if (!Application.isEditor) {
            GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
        }
        Tween.StopAll();
        yield return null;
        GC.Collect();
        Assert.IsTrue(numUIToolkitTestsDone <= 1, "Please run UIToolkit tests one by one because there is no way to reset UI Toolkit animation library without influencing test results.");
    }
    
    void initUIToolkitTests() {
        var panelSettings = Resources.Load<PanelSettings>("PrimeTween_VS_UIToolkit/TestPanelSettings");
        Assert.IsNotNull(panelSettings);
        var uiDocument = new GameObject("UIToolkit_test_object").AddComponent<UIDocument>();
        uiDocument.panelSettings = panelSettings;
        
        for (int i = 0; i < iterations; i++) {
            var visualElement = new VisualElement();
            visualElements[i] = visualElement;
            uiDocument.rootVisualElement.Add(visualElement);
        }
    }
    
    
    [Test] public void _0_PrimeTweenAssertionsDisabled() {
        #if !PRIME_TWEEN_DISABLE_ASSERTIONS
        Debug.LogError("Please disable PrimeTween asserts by adding the define: PRIME_TWEEN_DISABLE_ASSERTIONS. This will ensure you're measuring the release performance.");
        #endif
    }
    
    
    [UnityTest, Performance] public IEnumerator _01_VisualElement_Animation__UIToolkit() {
        startAnimationsUIToolkit();
        yield return measureAverageFrameTimes();
        numUIToolkitTestsDone++;
    }
    [UnityTest, Performance] public IEnumerator _01_VisualElement_Animation_PrimeTween() {
        startAnimationsPrimeTween();
        yield return measureAverageFrameTimes();
    }


    [UnityTest, Performance]
    public IEnumerator _02_Animation_GCAlloc__UIToolkit() {
        measureGCAlloc(startAnimationsUIToolkit);
        yield return wait;
        numUIToolkitTestsDone++;
    }
    [Test, Performance] public void _02_Animation_GCAlloc_PrimeTween() => measureGCAlloc(startAnimationsPrimeTween);


    [UnityTest, Performance]
    public IEnumerator _03_Animation_Start__UIToolkit() {
        yield return measureFrameTime(startAnimationsUIToolkit);
        yield return wait;
        numUIToolkitTestsDone++;
    }
    [UnityTest, Performance]
    public IEnumerator _03_Animation_Start_PrimeTween() { yield return measureFrameTime(startAnimationsPrimeTween); }
    
    
    void startAnimationsUIToolkit() {
        foreach (var visualElement in visualElements) {
            visualElement.experimental.animation.Position(Vector3.one, duration * 1000);
        }
    }
    void startAnimationsPrimeTween() {
        foreach (var visualElement in visualElements) {
            Tween.Position(visualElement.transform, Vector3.one, duration, Ease.Linear);
        }
    }
    
    static void measureGCAlloc(Action action) {
        GC.Collect();
        var allocatedMemoryBefore = GC.GetTotalMemory(true);
        action();
        var gcAllocPerIteration = (GC.GetTotalMemory(true) - allocatedMemoryBefore) / iterations;
        Measure.Custom(new SampleGroup("GCAlloc", SampleUnit.Byte), gcAllocPerIteration);
    }
    static IEnumerator measureFrameTime(Action action) {
        using (Measure.Frames().Scope()) {
            action();
            GC.Collect();
            yield return null;
        }
    }
    static IEnumerator measureAverageFrameTimes() {
        return Measure.Frames().MeasurementCount(50).Run();
    }
}
#endif