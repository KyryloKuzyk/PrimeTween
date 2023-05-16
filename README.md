# PrimeTween
PrimeTween is a high-performance, **allocation-free** animation library for Unity. **Animate anything** with just one line of code, tweak all animation properties directly from the Inspector, and create complex animation sequences. No runtime memory allocations, ever.

Getting started
---
Without further ado, let's jump straight to the code!
```csharp
// Animate 'transform.position.y' from the current value to 5 in 1 second using the Ease.InOutSine
Tween.PositionY(transform, endValue: 10, duration: 1, ease: Ease.InOutSine);

// Rotate 'transform' from the current rotation to (0, 90, 0) in 1 second
Tween.Rotation(transform, endValue: Quaternion.Euler(0, 90, 0), duration: 1);

// Rotate 'transform' around y axis by 360 degrees in 1 second 
Tween.EulerAngles(transform, startValue: Vector3.zero, endValue: new Vector3(0, 360), duration: 1);

// Shake camera with a frequency of 10 shakes per second, magnitude of 0.1 meters on the y axis, for the duration of 1 second
Tween.ShakeLocalPosition(Camera.main.transform, frequency: 10, strength: new Vector3(0, 0.1f), duration: 1);
```
That's it!

Simply type **`Tween.`** and let your IDE show all supported properties that can be animated with PrimeTween. Out of the box, PrimeTween can animate almost everything: UI, material properties, camera properties, sound, transform, and what not. 

Didn't find what you're looking for? No problem, use [`Tween.Custom()`](#custom-tweens) to animate **anything**.

### Callbacks
Use **`.OnComplete()`** to execute custom code on tween's completion.
```csharp
// Call SomeMethod() when the animation completes
Tween.Position(transform, new Vector3(10, 0), duration: 1)
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
### Sequence
There are several sequencing methods in PrimeTween. Let's start with the most common one: grouping tweens in **Sequences**.

**Sequence** is an ordered group of tweens and callbacks. Tweens in a sequence can run in **parallel** to one another with **`.Group()`** and **sequentially** with **`.Chain()`**. Overlapping can be achieved by adding **`startDelay`** to a tween.
```csharp
Sequence.Create()
    // PositionX and LocalScale tweens are 'grouped', so they will run in parallel
    .Group(Tween.PositionX(transform, endValue: 10f, duration: 1.5f))
    .Group(Tween.LocalScale(transform, endValue: 2f, duration: 0.5f, startDelay: 1))
    // Rotation tween is 'chained' so it will start when both previous tweens are finished (after 1.5 seconds)
    .Chain(Tween.Rotation(transform, endValue: new Vector3(0f, 0f, 45f), duration: 1f)) 
    .ChainCallback(() => Debug.Log("Sequence completed"));
```
> Sequences can be controlled the same way as tweens, see [controlling tweens](#controlling-tweens) section.
### Coroutines
Another sequencing method is waiting for tweens and sequences in **coroutines** by calling **`.ToYieldInstruction()`**.
```csharp
IEnumerator Coroutine() {
    Tween.PositionX(transform, endValue: 10f, duration: 1.5f);
    yield return Tween.LocalScale(transform, 2f, 0.5f, startDelay: 1).ToYieldInstruction();
    yield return Tween.Rotation(transform, new Vector3(0f, 0f, 45f), 1f).ToYieldInstruction();
    Debug.Log("Sequence completed");
}
```

### Async/await
And the last method is awaiting tweens and sequences using the **async/await** pattern. All three sequencing methods produce the same result, so choose one that best suits your use case.
```csharp
async void AsyncMethod() {
    Tween.PositionX(transform, endValue: 10f, duration: 1.5f);
    await Tween.LocalScale(transform, endValue: 2f, duration: 0.5f, startDelay: 1);
    await Tween.Rotation(transform, endValue: new Vector3(0f, 0f, 45f), duration: 1f);
    Debug.Log("Sequence completed");
}
```

> While PrimeTween never allocates memory, the async/await feature in C# is allocating. Consider using [UniTask](https://github.com/Cysharp/UniTask) to address this language limitation.

Inspector integration
---
Inspector integration is the cornerstone of PrimeTween's design that lets you tweak all animation properties from the Inspector without changing the code. It **saves time** and gives **creative freedom**. All animation settings can be **serialized** in MonoBehaviour or ScriptableObject and passed to the corresponding Tween methods.
```csharp
// Tweak all animation properties from the Inspector:
//     startValue, endValue, duration, ease (or custom ease curve), etc.
// Then pass tween settings to the animation method
[SerializeField] TweenSettings.Float yPositionTweenSettings;
Tween.PositionY(transform, yPositionTweenSettings);

[SerializeField] TweenSettings.Vector3 rotationTweenSettings;
Tween.Rotation(transform, rotationTweenSettings);

[SerializeField] TweenSettings.Vector3 eulerAnglesTweenSettings;
Tween.EulerAngles(transform, eulerAnglesTweenSettings);

[SerializeField] ShakeSettings cameraShakeSettings;
Tween.ShakeLocalPosition(Camera.main.transform, cameraShakeSettings);
```
The noteworthy thing about setting up animation properties in the Inspector is that you can any time switch to a custom **animation curve** without touching the code.

<img height="250" src="Documentation/inspector_integration.jpg" alt="100">


Controlling tweens
---
All static Tween methods return a **`Tween`** struct. While the **`tween.IsAlive`** you can control it and access its properties such as duration, elapsedTime, progress, interpolationFactor, etc.

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

As you can see, there is no way to change the direction of the currently running tween, it can only be **stopped** and **completed**. But how to play an animation **forward** and **backward**, for example to show or hide a window? Easy! Just start a new Tween in the desired direction.

In the next example, the window may change its state while the previous animation is still running. Generally, there is no need to stop the previously running tween in such cases. The new tween will start from the current position and  **overwrite** all previously running tweens on this target.
```csharp
[SerializeField] RectTransform window;

public void SetWindowOpened(bool isOpened) {
    Tween.UIAnchoredPositionY(window, endValue: isOpened ? 0 : -500, duration: 0.5f);
}
```

And to utilize the full power of PrimeTween, all window animation settings can come from the Inspector. Notice how the **`isOpened`** parameter is passed to the **`WithDirection(bool toEndValue)`** method. This helper method selects the target position based on the isOpened parameter. Nice and simple!
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
    .OnComplete(() => SomeMethod()); // delegate allocation
```

Here is how to fix the above code to be non-allocating. Notice how **`this`** reference is passed to the method, then the **`target`** parameter is used instead of calling `SomeMethod()` directly.
```csharp
Tween.Position(transform, new Vector3(10, 0), duration: 1)
    .OnComplete(target: this, target => target.SomeMethod());
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

<img height="300" src="Documentation/debug_tweens.jpg" alt="100">

Also, the Inspector shows the '**Max alive tweens**' for the current session. Use this number to estimate the maximum number of tweens required for your game and pass it to the **`PrimeTweenConfig.SetTweensCapacity(int capacity)`** method at the launch of your game. This will ensure PrimeTween doesn't allocate any additional memory at runtime.