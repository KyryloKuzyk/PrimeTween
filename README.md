PrimeTween
===

PrimeTween is a high-performance, **allocation-free** animation library for Unity. **Animate anything** with just one line of code, tweak all animation properties directly from the Inspector, and create complex animation sequences. No runtime memory allocations, ever.

Table of Contents
---
- [Getting started](#getting-started)
  * [Animations](#animations)
  * [Callbacks](#callbacks)
  * [Delays](#delays)
- [Sequencing tweens](#sequencing-tweens)
  + [Sequence](#sequence)
  + [Coroutines](#coroutines)
  + [Async/await](#async-await)
- [Inspector integration](#inspector-integration)
- [Controlling tweens](#controlling-tweens)
- [Custom tweens](#custom-tweens)
- [Zero allocations with delegates](#zero-allocations-with-delegates)
- [Debugging tweens](#debugging-tweens)
- [Migrating from DOTween to PrimeTween](#migrating-from-dotween-to-primetween)
  + [Performance comparison](#performance-comparison)
  + [DOTween adapter](#dotween-adapter)
  + [Tween.PlayForward/PlayBackwards](#tweenplayforwardplaybackwardsrestart)

Getting started
---

### Installation
Import PrimeTween from [Asset Store](https://assetstore.unity.com/packages/slug/252960).
> Unity 2018: selected the `Assets/Plugins/PrimeTween/PrimeTweenInstaller` and press the Install button. To run the Demo, add the `PRIME_TWEEN_INSTALLED` define to the `Project Settings/Player/Scripting Define Symbols`

### Animations
Without further ado, let's jump straight to the code!
```csharp
// Animate 'transform.position.y' from the current value to 10 in 1 second using the Ease.InOutSine
Tween.PositionY(transform, endValue: 10, duration: 1, ease: Ease.InOutSine);

// Rotate 'transform' from the current rotation to (0, 90, 0) in 1 second
Tween.Rotation(transform, endValue: Quaternion.Euler(0, 90, 0), duration: 1);

// Rotate 'transform' around the y-axis by 360 degrees in 1 second 
Tween.EulerAngles(transform, startValue: Vector3.zero, endValue: new Vector3(0, 360), duration: 1);

// Shake camera with a frequency of 10 shakes per second, magnitude of 0.1 meters on the y-axis, for the duration of 1 second
Tween.ShakeLocalPosition(Camera.main.transform, frequency: 10, strength: new Vector3(0, 0.1f), duration: 1);
```
That's it!

Simply type **`Tween.`** and let your IDE suggest all supported animations. Out of the box, PrimeTween can animate almost everything: UI, materials, camera properties, transforms, audio, and whatnot.

Didn't find what you're looking for? No problem, use [`Tween.Custom()`](#custom-tweens) to animate **anything**.

### Callbacks
Use **`.OnComplete()`** to execute custom code on tween's completion.
```csharp
// Call SomeMethod() when the animation completes
Tween.Position(transform, endValue: new Vector3(10, 0), duration: 1)
    .OnComplete(() => SomeMethod());
    
// When the animation completes, wait for 0.5 seconds, then destroy the GameObject
Tween.LocalScale(transform, endValue: 0, duration: 1, endDelay: 0.5f)
    .OnComplete(() => Destroy(gameObject));
```

>"But wait! There is a **memory allocation** in the example above" you would say. And you would be right: calling `SomeMethod()` or `Destroy()` captures `this` reference in a closure and allocates heap memory. See how to address this in the [zero allocations](#zero-allocations-with-delegates) section.

### Delays
Creating delays is by far the most useful feature in game development. Delays in PrimeTween behave like normal tweens and can be used with sequences, coroutines, and async/await methods. All while being completely [allocation-free](#zero-allocations-with-delegates).

```csharp
Tween.Delay(duration: 1f, () => Debug.Log("Delay completed"));
```

Sequencing tweens
---
#### Sequence
There are several sequencing methods in PrimeTween. Let's start with the most common one: grouping tweens in **Sequences**.

**Sequence** is an ordered group of tweens and callbacks. Tweens in a sequence can run in **parallel** to one another with **`.Group()`** and **sequentially** with **`.Chain()`**. Overlapping can be achieved by adding **`startDelay`** to a tween.

Sequences can be controlled the same way as individual tweens, see [controlling tweens](#controlling-tweens) section.

```csharp
Sequence.Create()
    // PositionX and LocalScale tweens are 'grouped', so they will run in parallel
    .Group(Tween.PositionX(transform, endValue: 10f, duration: 1.5f))
    .Group(Tween.LocalScale(transform, endValue: 2f, duration: 0.5f, startDelay: 1))
    // Rotation tween is 'chained' so it will start when both previous tweens are finished (after 1.5 seconds)
    .Chain(Tween.Rotation(transform, endValue: new Vector3(0f, 0f, 45f), duration: 1f)) 
    .ChainDelay(1)
    .ChainCallback(() => Debug.Log("Sequence completed"));
```

#### Coroutines
Another sequencing method is waiting for tweens and sequences in **coroutines** by calling **`.ToYieldInstruction()`**.
```csharp
IEnumerator Coroutine() {
    Tween.PositionX(transform, endValue: 10f, duration: 1.5f);
    yield return Tween.LocalScale(transform, 2f, 0.5f, startDelay: 1).ToYieldInstruction();
    yield return Tween.Rotation(transform, new Vector3(0f, 0f, 45f), 1f).ToYieldInstruction();
    // Non-allocating alternative to 'yield return new WaitForSeconds(1f)'
    yield return Tween.Delay(1).ToYieldInstruction(); 
    Debug.Log("Sequence completed");
}
```

#### Async/await
And the last method is awaiting tweens and sequences using the **async/await** pattern. Async/await is a great tool to prevent the callback hell in your code. PrimeTween doesn't use threads, so tweens can be awaited on all platforms, even on WebGL.
```csharp
async void AsyncMethod() {
    Tween.PositionX(transform, endValue: 10f, duration: 1.5f);
    await Tween.LocalScale(transform, endValue: 2f, duration: 0.5f, startDelay: 1);
    await Tween.Rotation(transform, endValue: new Vector3(0f, 0f, 45f), duration: 1f);
    await Tween.Delay(1); 
    Debug.Log("Sequence completed");
}
```

> While PrimeTween never allocates memory at runtime, the async/await feature in C# is allocating: awaiting an async method allocates a small amount of garbage comparable to starting a coroutine. Consider using [UniTask](https://github.com/Cysharp/UniTask) to address this language limitation.

Inspector integration
---
Inspector integration is the cornerstone of PrimeTween's design. It lets you tweak all animation properties from the Inspector without changing the code. It **saves time** and gives **creative freedom**. All animation settings can be **serialized** in MonoBehaviour or ScriptableObject and passed to the corresponding Tween methods.
```csharp
// Tweak all animation properties from the Inspector:
//     startValue, endValue, duration, ease (or custom ease curve), etc.
[SerializeField] TweenSettings.Float yPositionTweenSettings;
// Then pass tween settings to the animation method
Tween.PositionY(transform, yPositionTweenSettings);

[SerializeField] TweenSettings.Vector3 rotationTweenSettings;
Tween.Rotation(transform, rotationTweenSettings);

[SerializeField] TweenSettings.Vector3 eulerAnglesTweenSettings;
Tween.EulerAngles(transform, eulerAnglesTweenSettings);

[SerializeField] ShakeSettings cameraShakeSettings;
Tween.ShakeLocalPosition(Camera.main.transform, cameraShakeSettings);
```
The neat thing about setting up animation properties in the Inspector is that you can any time switch to a custom **animation curve** without touching the code.

<img width="100%" src="Documentation/inspector_integration.jpg">

Controlling tweens
---
All static **`Tween.`** methods return a **`Tween`** struct. While the **`tween.IsAlive`** you can control it and access its properties such as duration, elapsedTime, progress, interpolationFactor, etc.

After completion, the tween becomes 'dead' and can't be reused. This ensures that completed tweens don't eat computing resources and prevents the common performance pitfalls encountered in other tween libraries.
```csharp
Tween tween = Tween.LocalPositionX(transform, endValue: 1.5f, duration: 1f);
// ...
 
if (tween.IsAlive) {
    // 'IsAlive' means the tween was created and not completed (or manually stopped) yet.
    // While the tween 'IsAlive' you can access its properties such as duration,
    //     elapsedTime, progress, interpolationFactor, etc.
    Debug.Log($"Animation is still running, elapsed time: {tween.elapsedTime}.");
}

// Pause the tween
tween.IsPaused = true;

// Interrupt the tween, leaving the animated value at the current value
tween.Stop();

// Instantly complete the running tween and set the animated value to the endValue
tween.Complete();
```

As you can see, there is no way to change the direction of the currently running tween, it can only be **stopped** and **completed**. But how to play an animation **forward** and **backward**, for example, to show or hide a window? Easy! Just start a new Tween in the desired direction.
```csharp
[SerializeField] RectTransform window;

public void SetWindowOpened(bool isOpened) {
    Tween.UIAnchoredPositionY(window, endValue: isOpened ? 0 : -500, duration: 0.5f);
}
```
In this example, the `SetWindowOpened()` can be called again while the previous animation is still running. Generally, there is no need to stop the previously running tween in such cases. The new tween will seamlessly start from the current position and **overwrite** all previously running tweens on the `window`. Several duplicated tweens are fine, but if your code can potentially start the same tween every frame, then consider stopping the previous tween.

And to utilize the full power of PrimeTween, all window animation settings can come from the Inspector. Notice how the **`isOpened`** parameter is passed to the **`WithDirection(bool toEndValue)`** method. This helper method selects the target position based on the `isOpened` parameter. Nice and simple!
```csharp
[SerializeField] RectTransform window;
[SerializeField] TweenSettings.Float windowAnimationSettings;

public void SetWindowOpened(bool isOpened) {
    Tween.UIAnchoredPositionY(window, windowAnimationSettings.WithDirection(toEndValue: isOpened));
}
```

Custom tweens
---
Use **`Tween.Custom()`** to animate literary anything. The supported types for custom tweens are `float, Color, Vector2/3/4, Quaternion, and Rect`.
```csharp
float floatField;
Color colorField;

// Animate 'floatField' from 0 to 10 in 1 second
Tween.Custom(0, 10, duration: 1, onValueChange: newVal => floatField = newVal);

// Animate 'colorField' from white to black in 1 second
Tween.Custom(Color.white, Color.black, duration: 1, onValueChange: newVal => colorField = newVal);
```

As you may expect, custom tweens work with [inspector integration](#inspector-integration) the same way as regular tweens do.
```csharp
[SerializeField] TweenSettings.Float tweenSettings;
float floatField;

Tween.Custom(tweenSettings, onValueChange: newVal => floatField = newVal);    
```

Zero allocations with delegates
---
C# delegates is a powerful language feature essential for game development. It gives us the ability to receive callbacks and pass methods to other methods. But when delegates are used in hot code paths carelessly, they can create [performance issues](https://www.jacksondunstan.com/articles/3765).

Let's review the code from earlier. If SomeMethod() is an instance method, then calling it from the callback will implicitly capture **`this`** reference, allocating heap memory.
```csharp
Tween.Position(transform, new Vector3(10, 0), duration: 1)
    .OnComplete(() => SomeMethod()); // delegate allocation!
```

Here is how to fix the above code to be non-allocating. Notice how **`this`** reference is passed to the `OnComplete` method, then the **`target`** parameter is used instead of calling `SomeMethod()` directly.
```csharp
Tween.Position(transform, new Vector3(10, 0), duration: 1)
    .OnComplete(target: this, target => target.SomeMethod()); // no allocation
```

That's it! The same non-allocating approach can be used in all places where PrimeTween uses delegates.
```csharp
Tween.Delay(this, duration: 1f, target => target.SomeMethod());

Tween.Custom(this, 0, 10, duration: 1, (target, newVal) => target.floatField = newVal);

var shakeSettings = new ShakeSettings(frequency: 10, strength: Vector3.one, duration: 1);
Tween.ShakeCustom(this, shakeSettings, vector3Field, (target, val) => target.vector3Field = val);
```

Debugging tweens
---
To debug tweens select the **PrimeTweenManager** object under the DontDestroyOnLoad foldout in the scene Hierarchy. Here you can inspect all currently running tweens and their properties.

If the tween's `target` is `UnityEngine.Object`, you can quickly show it in the Hierarchy by clicking on the `Unity Target` field. This is a good reason to supply the target even when it's optional, like in the case of `Tween.Delay()` and `Tween.Custom()`.

<img src="Documentation/debug_tweens.jpg" width="80%">

Also, the Inspector shows the '**Max alive tweens**' for the current session. Use this number to estimate the maximum number of tweens required for your game and pass it to the `PrimeTweenConfig.SetTweensCapacity(int capacity)` method at the launch of your game. This will ensure PrimeTween doesn't allocate any additional memory at runtime.

Migrating from DOTween to PrimeTween
---
DOTween made a long way in its development and brought a lot of value to the game industry. But for almost 10 years it accumulated a lot of [technical debt](https://github.com/Demigiant/dotween/issues) and inconsistencies.

DOTween gives a huge heads-up for game jams and early prototypes, but when a project becomes bigger, its quirks become to pile up. And while with years it was getting better with memory allocations, it still allocates memory on every animation start and puts a lot of pressure on the garbage collector.

PrimeTween is **simple**, **consistent**, covered by **tests**, and behaves exactly like you would expect. High performance and zero allocations out of the box.

PrimeTween and DOTween don't conflict with each other and can be used in one project. So you can try PrimeTween in your existing project without breaking anything.

#### Performance comparison

In real-world scenarios, PrimeTween is about **10-30%** faster than DOTween in terms of average runtime performance. But this is not the most important performance indicator most of the time.

Where PrimeTween really shines is in the absence of **memory allocations**. PrimeTween never allocates heap memory and never produces garbage collection spikes. Create millions of animations, delays, and sequences with **0KB of GC** and without hiccups!

Another important performance factor is **frame pacing**. To create a smooth experience for a user, frames in your game should be evenly spaced in time. A one-frame hiccup is still noticeable to the eye even when the profiler tells that the game runs at an average of 60 FPS. In this image, you can see that PrimeTween evenly distributes the load maintaining smooth frame times, while DOTween produces a lot of fluctuations. The testing scenario is identical in both cases.

![frame_pacing.png](Documentation%2Fframe_pacing.png)

#### DOTween adapter

PrimeTween comes with a built-in migration adapter that can help you migrate even big projects relatively quickly.

Adapter is an **optional** feature designed to speed up PrimeTween's adoption. The migrated code may still be allocating because of the [delegate allocations](#zero-allocations-with-delegates).
> Please **back up** your project before proceeding. You should **test** the migrated code thoroughly before releasing it to production.


First, to enable the adapter, add the **`PRIME_TWEEN_DOTWEEN_ADAPTER`** define to the `ProjectSettings/Player/Script Compilation` and press Apply.

<img src="Documentation/adapter_define.png" width="60%">

The migration process may vary from project to project. In many cases, simply replacing `using DG.Tweening;` with the `using PrimeTween;` is enough to switch a script from DOTween to PrimeTween. See how easy was to migrate the [MotionDesignFES](https://github.com/KirillKuzyk/MotionDesignFES-PrimeTween/commit/628cb17d027e9648add45e0b2d9b431983a1bde6) project with dozens of complex animations.

The adapter covers the majority of cases simply by replacing the `using DG.Tweening;` with the `using PrimeTween;` statement. Here is a non-exhaustive list of what the adapter does **automatically**.
```csharp
DOTween API on the left     -->  PrimeTween API on the right

// All animations are supported, here are only a few of them as an example
transform.DOMove()          -->  Tween.Position(transform)
uiImage.DOFade()            -->  Tween.Alpha(uiImage)
material.DOColor()          -->  Tween.Color(material)
transform.DOShakePosition() -->  Tween.ShakeLocalPosition(transform)

DOVirtual.DelayedCall()     -->  Tween.Delay()
DOTween.To()                -->  Tween.Custom()
DOVirtual.Vector3()         -->  Tween.Custom()

DOTween.Sequence()          -->  Sequence.Create()
sequence.Join()             -->  sequence.Group()
sequence.Append()           -->  sequence.Chain()

// The 'Kill()' naming may be misleading even for experienced developers.
// Does it kill the GameObject? Does it kill the MonoBehaviour? Does it kill other animations running on the same target?
// PrimeTween gives confidence in what the code actually does.
DOTween.Kill(target, true)  -->  Tween.CompleteAll(onTarget: target)
DOTween.Kill(target, false) -->  Tween.StopAll(onTarget: target)
tween.Kill(true)            -->  tween.Complete()
tween.Kill(false)           -->  tween.Stop()

yield return tween.WaitForCompletion()    -->  yield return tween.ToYieldInstruction()
yield return sequence.WaitForCompletion() -->  yield return sequence.ToYieldInstruction()

// PrimeTween doesn't use threads, so you can write async methods even on WebGL
await tween.AsyncWaitForCompletion()      -->  await tween
await sequence.AsyncWaitForCompletion()   -->  await sequence 
```

Although, here are the most common places that require a **manual fix** to the existing code base.
```csharp
// using DG.Tweening;
using PrimeTween;

// Tweener tween;
// TweenerCore tween;
// ABSSequentiable tween;
Tween tween; // just Tween ;)

// if (tween != null && tween.IsPlaying()) {}
if (tween.IsAlive) {} // null check is not needed because Tween in PrimeTween is a struct

// if (tween != null && tween.IsActive()) {
//     tween.Kill(complete: true);
//     tween = null;
// }
tween.Complete(); // null check and setting tween to null is not needed 

// DOTween.SetTweensCapacity(tweenersCapacity: 200, sequencesCapacity: 50);
PrimeTweenConfig.SetTweensCapacity(capacity: 250); // sequences in PrimeTween use the same pool as regular tweens
```

#### Tween.PlayForward/PlayBackwards/Restart
PrimeTween's main design goals are **performance** and **reliability**. So there are a few things that don't have an exact mapping when migrating from DOTween.

PrimeTween offers a different approach to animating things forward and backward that is simpler and has greater performance.

Let's consider the common DOTween usage pattern: creating a tween once, then calling PlayForward() and PlayBackwards() when needed.
<details>
<summary><b>DOTweenWindow.cs</b> (click to expand)</summary>

```csharp
public class DOTweenWindow : MonoBehaviour {
    // Bad: disable auto-kill and store tween reference to reuse the tween later.
    // Disabling auto-kill wastes resources: even when the tween is not running, it still receives an update every frame and consumes memory.
    Tween tween;

    void Awake() {
        tween = transform.DOLocalMove(Vector3.zero, 1)
            .ChangeStartValue(new Vector3(0, -500))
            .SetEase(Ease.InOutSine)
            .SetAutoKill(false)
            // Bad: don't forget to link the tween to this GameObject, so the tween is killed when the GameObject is destroyed
            .SetLink(gameObject)
            // Bad: paused tweens still receive updates every frame and consume resources
            .Pause();
    }

    public void SetWindowOpened(bool isOpened) {
        if (isOpened) {
            tween.PlayForward();
        } else {
            tween.PlayBackwards();
        }
    }
    
    void OnDestroy() {
        // Bad: don't forget to kill the tween before destroying an object.
        // I bet the majority of developers don't even know about the SetLink() API and kill tweens in OnDestroy()
        // 'Safe Mode' is NOT SAFE, and you should never rely on it because it silently swallows potential errors.
        //     Also, 'Safe Mode' doesn't work on WebGL and with 'Fast and no exceptions' on iOS.
        tween.Kill();
    }
}
```
</details>

PrimeTween offers a much more elegant way of doing the same that comes with much better performance. Destroying the window while the animation is playing is perfectly fine.
```csharp
public class PrimeTweenWindow : MonoBehaviour {
    public void SetWindowOpened(bool isOpened) {
        Tween.LocalPosition(transform, isOpened ? Vector3.zero : new Vector3(0, -500), 1, Ease.InOutSine);
    }
}
```

And with PrimeTween's [inspector integration](#inspector-integration), all animation properties can be tweaked from the Inspector, eliminating hard-coded magic values.
```csharp
public class PrimeTweenWindowWithInspectorIntegration : MonoBehaviour {
    // Tweak all animation properties from the Inspector
    [SerializeField] TweenSettings.Vector3 windowAnimationSettings;

    public void SetWindowOpened(bool isOpened) {
        // Use windowAnimationSettings.WithDirection() to animate the window to a closed or opened position
        Tween.LocalPosition(transform, windowAnimationSettings.WithDirection(isOpened));
    }
}
```

#### Unsupported APIs
There are a few other things PrimeTween currently **doesn't support**.
```csharp
// Not supported, but technically possible
sequence.OnComplete() // alternative: if a sequence has one loop, use ChainCallback() instead
transform.DOPath()

// Not supported because sequences and tweens are non-reusable in PrimeTween
sequence.OnStart() // alternative: execute the code before creating a sequence
tween.OnStart() // alternative: execute the code before starting a tween
sequence.PlayForward/PlayBackwards/Restart() // LoopType.Yoyo is also not supported
```

Adding all the above features to PrimeTween is technically possible in one or another way, but I decided to gather feedback from users first to see if they really need it. Please drop me a note if your project needs any of these and describe your use case.