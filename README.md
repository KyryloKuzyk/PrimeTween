
PrimeTween
===

PrimeTween is a high-performance, **allocation-free** animation library for Unity. **Animate anything** with just one line of code, tweak all animation properties directly from the Inspector, and create complex animation sequences. No runtime memory allocations, ever.

[**Performance Comparison with DOTween**](https://github.com/KyryloKuzyk/PrimeTween/discussions/8)

**[Asset Store](https://assetstore.unity.com/packages/slug/252960)** | **[Forum](https://forum.unity.com/threads/1479609/)** | **[FAQ](https://github.com/KyryloKuzyk/PrimeTween/discussions)** | **[YouTube](https://www.youtube.com/watch?v=MuMKwxOzc3M)**

Table of Contents
---
- [Getting started](#getting-started)
  * [Animations](#animations)
  * [Shakes](#shakes)
  * [Callbacks](#callbacks)
  * [Delays](#delays)
  * [Cycles](#cycles)
- [Sequencing tweens](#sequencing-tweens)
  + [Sequence](#sequence)
  + [Coroutines](#coroutines)
  + [Async/await](#asyncawait)
- [Inspector integration](#inspector-integration)
- [Controlling tweens](#controlling-tweens)
- [Custom tweens](#custom-tweens)
- [Zero allocations with delegates](#zero-allocations-with-delegates)
- [Debugging tweens](#debugging-tweens)
- [Migrating from DOTween to PrimeTween](#migrating-from-dotween-to-primetween)
  + [Performance comparison](#performance-comparison)
  + [DOTween adapter](#dotween-adapter)
  + [Tween.PlayForward/PlayBackwards](#tweenplayforwardplaybackwardsrestart)
- [Support](#support)

Getting started
---

### Installation
Import PrimeTween from [Asset Store](https://assetstore.unity.com/packages/slug/252960).

### Animations
Without further ado, let's jump straight to the code!
```csharp
using PrimeTween;

// Animate 'transform.position.y' from the current value to 10 in 1 second using the Ease.InOutSine
Tween.PositionY(transform, endValue: 10, duration: 1, ease: Ease.InOutSine);

// Rotate 'transform' from the current rotation to (0, 90, 0) in 1 second
Tween.Rotation(transform, endValue: Quaternion.Euler(0, 90, 0), duration: 1);

// Rotate 'transform' around the y-axis by 360 degrees in 1 second 
Tween.EulerAngles(transform, startValue: Vector3.zero, endValue: new Vector3(0, 360), duration: 1);
```
That's it!

Simply type **`Tween.`** and let your IDE suggest all supported animations. Out of the box, PrimeTween can animate almost everything: UI, UI Toolkit, Materials, Camera properties, Transforms, AudioSource, and whatnot.

Didn't find what you're looking for? No problem, use [`Tween.Custom()`](#custom-tweens) to animate **anything**.
> To view PrimeTween's XML documentation in your IDE, enable this setting: _'Unity Preferences/External Tools/Generate .csproj files for/**Local tarbal**'_ and press the _'Regenerate project files'_ button.

### Shakes
```csharp
// Shake the camera with medium strength (0.5f) 
Tween.ShakeCamera(camera, strengthFactor: 0.5f);

// Shake the camera with heavy strength (1.0f) for a duration of 0.5f seconds and a frequency of 10 shakes per second 
Tween.ShakeCamera(camera, strengthFactor: 1.0f, duration: 0.5f, frequency: 10);

// Shake the y-axis position with an amplitude of 1 unit 
Tween.ShakeLocalPosition(transform, strength: new Vector3(0, 1), duration: 1, frequency: 10);

// Shake the z-axis rotation with an amplitude of 15 degrees
Tween.ShakeLocalRotation(transform, strength: new Vector3(0, 0, 15), duration: 1, frequency: 10);

// Punch localPosition in the direction of 'punchDir'
var punchDir = transform.up;
Tween.PunchLocalPosition(transform, strength: punchDir, duration: 0.5f, frequency: 10);
```

### Callbacks
Use **`.OnComplete()`** to execute custom code on tween's completion.
```csharp
// Call SomeMethod() when the animation completes
Tween.Position(transform, endValue: new Vector3(10, 0), duration: 1)
    .OnComplete(() => SomeMethod());
    
// After the animation completes, wait for 0.5 seconds, then destroy the GameObject
Tween.Scale(transform, endValue: 0, duration: 1, endDelay: 0.5f)
    .OnComplete(() => Destroy(gameObject));
```

>"But wait! There is a **memory allocation** in the example above" you would say. And you would be right: calling `SomeMethod()` or `Destroy()` captures `this` reference in a closure and allocates heap memory. See how to address this in the [zero allocations](#zero-allocations-with-delegates) section.

### Delays
Creating delays is by far the most useful feature in game development. Delays in PrimeTween behave like normal tweens and can be used with sequences, coroutines, and async/await methods. All while being completely [allocation-free](#zero-allocations-with-delegates).
```csharp
Tween.Delay(duration: 1f, () => Debug.Log("Delay completed"));
```

### Cycles
Animations can be repeated with the help of cycles. To apply cycles to an animation, pass the `int cycles` and `CycleMode cycleMode` parameters to a `Tween.` method. Setting cycles to -1 will repeat the tween indefinitely.
```csharp
Tween.PositionY(transform, endValue: 10, duration: 0.5f, cycles: 2, cycleMode: CycleMode.Yoyo);
```

#### enum CycleMode
- **Restart** (default): restarts the tween from the beginning.
- **Yoyo**: animates forth and back, like a yoyo. Easing is normal on the backward cycle.
- **Incremental**: at the end of a cycle increments `startValue` and `endValue`, like this: `(startValue = endValue, endValue += deltaValue)`. For example, if a tween moves position.x from 0 to 1, then after the first cycle, the tween will move the position.x from 1 to 2, and so on.
- **Rewind**: rewinds the tween as if time was reversed. Easing is reversed on the backward cycle.
> Sequences don't support CycleMode and can't be played backward.

#### void SetCycles(int cycles)
Sets the number of remaining cycles to a tween or sequence.  
This method modifies the cyclesTotal so that the tween will complete after the number of cycles.

#### void SetCycles(bool stopAtEndValue)
Stops the tween when it reaches 'startValue' or 'endValue' for the next time.  
For example, if you have an infinite tween (cycles == -1) with CycleMode.Yoyo/Rewind, and you wish to stop it when it reaches the 'endValue' (odd cycle), then set stopAtEndValue to true.  
To stop the animation at the 'startValue' (even cycle), set stopAtEndValue to false.

Sequencing tweens
---
#### Sequence
There are several sequencing methods in PrimeTween. Let's start with the most common one: grouping tweens in **Sequences**.

**Sequence** is an ordered group of tweens and callbacks. Tweens in a sequence can run in **parallel** to one another with **`.Group()`** and **sequentially** with **`.Chain()`**. Overlapping can be achieved by adding **`startDelay`** to a tween.

Sequences can be controlled the same way as individual tweens, see [controlling tweens](#controlling-tweens) section.

```csharp
Sequence.Create()
    // PositionX and Scale tweens are 'grouped', so they will run in parallel
    .Group(Tween.PositionX(transform, endValue: 10f, duration: 1.5f))
    .Group(Tween.Scale(transform, endValue: 2f, duration: 0.5f, startDelay: 1))
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
    yield return Tween.Scale(transform, 2f, 0.5f, startDelay: 1).ToYieldInstruction();
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
    await Tween.Scale(transform, endValue: 2f, duration: 0.5f, startDelay: 1);
    await Tween.Rotation(transform, endValue: new Vector3(0f, 0f, 45f), duration: 1f);
    // Non-allocating alternative to 'await Task.Delay(1000)' that doesn't use 'System.Threading'. Tweens and sequences can be awaited on all platforms, even on WebGL
    await Tween.Delay(1); 
    Debug.Log("Sequence completed");
}
```

> While PrimeTween never allocates memory at runtime, the async/await feature in C# is allocating: awaiting an async method allocates a small amount of garbage. Consider using [UniTask](https://github.com/Cysharp/UniTask) to address this language limitation.

Inspector integration
---
Inspector integration is the cornerstone of PrimeTween's design. It lets you tweak all animation properties from the Inspector without changing the code. It **saves time** and gives **creative freedom**. All animation settings can be **serialized** in MonoBehaviour or ScriptableObject and passed to the corresponding Tween methods.
```csharp
// Tweak all animation properties from the Inspector:
//     startValue, endValue, duration, ease (or custom ease curve), etc.
[SerializeField] TweenSettings<float> yPositionTweenSettings;
// Then pass tween settings to the animation method
Tween.PositionY(transform, yPositionTweenSettings);

[SerializeField] TweenSettings<Vector3> rotationTweenSettings;
Tween.Rotation(transform, rotationTweenSettings);

[SerializeField] TweenSettings<Vector3> eulerAnglesTweenSettings;
Tween.EulerAngles(transform, eulerAnglesTweenSettings);

[SerializeField] ShakeSettings cameraShakeSettings;
Tween.ShakeLocalPosition(Camera.main.transform, cameraShakeSettings);
```
The neat thing about setting up animation properties in the Inspector is that you can any time switch to a custom **animation curve** without touching the code.

<img width="100%" src="Documentation/inspector_integration.jpg">

Controlling tweens
---
All static **`Tween.`** methods return a **`Tween`** struct. While the **`tween.isAlive`** you can control it and access its properties such as duration, elapsedTime, progress, interpolationFactor, etc.

After completion, the tween becomes 'dead' and can't be reused. This ensures that completed tweens don't eat computing resources and prevents the common performance pitfalls encountered in other tween libraries.
```csharp
Tween tween = Tween.LocalPositionX(transform, endValue: 1.5f, duration: 1f);
// ...
 
if (tween.isAlive) {
    // '.isAlive' means the tween was created and not completed (or manually stopped) yet.
    // While the tween '.isAlive' you can access its properties such as duration,
    //     elapsedTime, progress, interpolationFactor, etc.
    Debug.Log($"Animation is still running, elapsed time: {tween.elapsedTime}.");
}

// Pause the tween
tween.isPaused = true;

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
In this example, the `SetWindowOpened()` can be called again while the previous animation is still running. Generally, there is no need to stop the previously running tween in such cases. The new tween will seamlessly start from the current position and **overwrite** all previously running tweens on the `window`. Several duplicated tweens are fine, but if your code can potentially create duplicated tweens every frame, then consider stopping the previous tween.

And to utilize the full power of PrimeTween, all window animation settings can come from the Inspector. Notice how the **`isOpened`** parameter is passed to the **`WithDirection(bool toEndValue)`** method. This helper method selects the target position based on the `isOpened` parameter. Nice and simple!
```csharp
[SerializeField] RectTransform window;
[SerializeField] TweenSettings<float> windowAnimationSettings;

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
[SerializeField] TweenSettings<float> tweenSettings;
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
Tween.ShakeCustom(this, startValue: vector3Field, shakeSettings, (target, val) => target.vector3Field = val);
```

Debugging tweens
---
To debug tweens select the **PrimeTweenManager** object under the DontDestroyOnLoad foldout in the scene Hierarchy. Here you can inspect all currently running tweens and their properties.

If the tween's `target` is `UnityEngine.Object`, you can quickly show it in the Hierarchy by clicking on the `Unity Target` field. This is a good reason to supply the target even when it's optional, like in the case of `Tween.Delay()` and `Tween.Custom()`.

<img src="Documentation/debug_tweens.jpg" width="80%">

Also, the Inspector shows the '**Max alive tweens**' for the current session. Use this number to estimate the maximum number of tweens required for your game and pass it to the `PrimeTweenConfig.SetTweensCapacity(int capacity)` method at the launch of your game. This will ensure PrimeTween doesn't allocate any additional memory at runtime.

Migrating from DOTween to PrimeTween
---
PrimeTween and DOTween don't conflict with each other and can be used in one project. You can check out all PrimeTween's performance benefits in your current DOTween project without breaking anything.

What are the reasons to try PrimeTween?
* See [performance](https://github.com/KyryloKuzyk/PrimeTween/discussions/8) comparison.
* **Seamless installation** that never produces compilation errors regardless of what other packages or plugins your project already uses.
* PrimeTween is extremely easy to learn. It has only 7 top-level concepts, and you can learn the API even without the documentation by simply typing `PrimeTween` and pressing `.`
* PrimeTween is extensively covered by more than **150 automatic tests** that ensure that every bit works perfectly.
* With PrimeTween it's safe to [destroy objects](https://github.com/KyryloKuzyk/PrimeTween/discussions/4) with running animations.
* PrimeTween is truly multiplatform with no exceptions. Awaiting tweens in async methods works even on WebGL!

#### Performance comparison

Please visit the full performance comparison [article](https://github.com/KyryloKuzyk/PrimeTween/discussions/8).

#### DOTween adapter

PrimeTween comes with a built-in migration adapter that can help you migrate even big projects relatively quickly. The adapter can also be used if you're missing extension methods you've gotten used to.

Adapter is an **optional** feature designed to speed up PrimeTween's adoption. The migrated code may still be allocating because of the [delegate allocations](#zero-allocations-with-delegates).
> Please **back up** your project before proceeding. You should **test** the migrated code thoroughly before releasing it to production.


First, to enable the adapter, add the **`PRIME_TWEEN_DOTWEEN_ADAPTER`** define to the `ProjectSettings/Player/Script Compilation` and press Apply.

<img src="Documentation/adapter_define.png" width="60%">

The migration process may vary from project to project. In many cases, simply replacing `using DG.Tweening;` with `using PrimeTween;` is enough to switch a script from DOTween to PrimeTween. See how easy was to migrate the [MotionDesignFES](https://github.com/KirillKuzyk/MotionDesignFES-PrimeTween/commit/628cb17d027e9648add45e0b2d9b431983a1bde6) project with dozens of complex animations.

Open a script that uses DOTween, change `using DG.Tweening;` to `using PrimeTween;` and the adapter will handle the majority of cases **automatically**.
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

// Tween tween;
// Tweener tween;
// TweenerCore tween;
Tween tween;

// if (tween != null) {
//     tween.Kill(complete: true);
//     tween = null;
// }
tween.Complete(); // null checking and setting tween to null is not needed
```

#### Tween.PlayForward/PlayBackwards/Restart

PrimeTween offers a different approach to animating things forward and backward that is simpler and has greater performance.

Let's consider the common DOTween usage pattern: creating a tween once, then calling PlayForward() and PlayBackwards() when needed.
<details>
<summary>DOTweenWindow.cs (click to expand)</summary>

```csharp
public class DOTweenWindow : MonoBehaviour {
    // Disable auto-kill and store tween reference to reuse the tween later.
    // Disabling auto-kill wastes resources: even when the tween is not running, it still receives an update every frame and consumes memory.
    Tween tween;

    void Awake() {
        tween = transform.DOLocalMove(Vector3.zero, 1)
            .ChangeStartValue(new Vector3(0, -500))
            .SetEase(Ease.InOutSine)
            .SetAutoKill(false)
            // Option 1: link the tween to this GameObject, so the tween is killed when the GameObject is destroyed
            .SetLink(gameObject)
            // Paused tweens still receive updates every frame and consume resources
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
        // Option 2: kill the tween before destroying an object.
        tween.Kill();
        
        // Option 3: enable 'Safe Mode' and don't use SetLink() and don't kill the tween in OnDestroy(). 
        // BUT:
        // - 'Safe Mode' will silence other potential errors or exceptions
        // - 'Safe Mode' doesn't work on WebGL and with 'Fast and no exceptions' on iOS
    }
}
```
</details>

PrimeTween offers a much more elegant way of doing the same that comes with much better performance. Destroying the window while the animation is playing is perfectly fine:
```csharp
public class PrimeTweenWindow : MonoBehaviour {
    public void SetWindowOpened(bool isOpened) {
        Tween.LocalPosition(transform, isOpened ? Vector3.zero : new Vector3(0, -500), 1, Ease.InOutSine);
    }
}
```

#### Unsupported APIs
There are a few other things PrimeTween currently **doesn't support**.
```csharp
// Alternative available
sequence.Insert(atPosition: 1.5f, transform.DOMoveX(0, 1));  -->  sequence.Group(Tween.PositionX(transform, 0, 1, startDelay: 1.5f)); (at the beginning of a Sequence)
sequence.InsertCallback(atPosition: 1f, callback: delegate { });  -->  sequence.Group(Tween.Delay(duration: 1, onComplete: delegate { })); (at the beginning of a Sequence)
transform.DOJump() // https://forum.unity.com/threads/1479609/#post-9226566
    
// Not supported, but technically possible
sequence.OnComplete() // alternative: if a sequence has one loop, use ChainCallback() instead
sequence.SetEase() // alternative: apply eases to individual tweens in a sequence
sequence.SetUpdate(bool isIndependentUpdate) // alternative: apply 'useUnscaledTime' parameter to all tweens in a sequence
transform.DOPath()

// Not supported because sequences and tweens are non-reusable in PrimeTween
tween/sequence.PlayForward/PlayBackwards/Rewind/Restart() // alternative: start a new tween/sequence in the desired direction
sequence.OnStart() // alternative: execute the code before starting a sequence
tween.OnStart() // alternative: execute the code before starting a tween
```

Adding all the above features to PrimeTween is technically possible in one or another way, but I decided to gather feedback from users first to see if they really need it. Please drop me a note if your project needs any of these and describe your use case.

Support
---
Join the discussion on [Unity Forum](https://forum.unity.com/threads/1479609/).  
Please submit bug reports [here](https://github.com/KyryloKuzyk/PrimeTween/issues).  
Submit your questions and feature requests [here](https://github.com/KyryloKuzyk/PrimeTween/discussions).  
If you want to contact me privately, please drop me an email: kuzykkirill@gmail.com