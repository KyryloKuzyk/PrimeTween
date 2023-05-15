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
That's it! Simply type '**Tween.**' and let your IDE show all supported properties that can be animated with PrimeTween. Out of the box, PrimeTween can animate almost everything: UI, material properties, camera properties, sound, transform, and what not. Didn't find what you're looking for? No problem, use [**Tween.Custom()**](#custom-tweens) to animate **anything**.


### Callbacks
Use **OnComplete()** to execute custom code on tween's completion.
<pre>
// Destroy the GameObject when animation completes
Tween.LocalScale(transform, endValue: 0, duration: 1).<b>OnComplete</b>(() => Destroy(gameObject));

// Call SomeMethod() when animation completes
Tween.Position(transform, new Vector3(10, 0), duration: 1).<b>OnComplete</b>(() => {
    Debug.Log("Animation completed");
    SomeMethod();
});
</pre>

>"But wait! There is a **memory allocation** in the example above" you would say. And you would be right: calling Destroy() or SomeMethod() captures 'this' reference in a closure and allocates heap memory. See how to address this in the [zero allocations](#zero-allocations-with-delegates) section.

### Delays
Creating delays is by far the most useful feature in game development. Delays in PrimeTween behave like normal tweens and can be used with sequences, coroutines, and async/await methods. All while being completely [allocation-free](#zero-allocations-with-delegates).
<pre>
<b>Tween.Delay</b>(duration: 1f, () => Debug.Log("Delay completed"));
</pre>

Sequencing tweens
---
### Sequence
There are several sequencing methods in PrimeTween. Let's start with the most common one: grouping tweens in **Sequences**.

**Sequence** is an ordered group of tweens and callbacks. Tweens in a sequence can run in **parallel** to one another with **Group()** and **sequentially** with **Chain()**. Overlapping can be achieved by adding '**startDelay**' to a tween.

<pre>
<b>Sequence.Create</b>()
    .<b>Group</b>(Tween.PositionX(transform, endValue: 10f, duration: 1.5f))
    .<b>Group</b>(Tween.LocalScale(transform, endValue: 2f, duration: 0.5f, startDelay: 1)) // position and localScale tweens will run in parallel because they are 'grouped'
    .<b>Chain</b>(Tween.Rotation(transform, endValue: new Vector3(0f, 0f, 45f), duration: 1f)) // rotation tween is 'chained' so it will start when both previous tweens are finished (after 1.5 seconds) 
    .<b>ChainCallback</b>(() => Debug.Log("Sequence completed"));
</pre>
> Sequences can be controlled the same way as tweens, see [controlling tweens](#controlling-tweens) section.
### Coroutines
Another sequencing method is waiting for tweens and sequences in **coroutines**.
<pre>
IEnumerator Coroutine() {
    Tween.PositionX(transform, endValue: 10f, duration: 1.5f);
    yield return Tween.LocalScale(transform, endValue: 2f, duration: 0.5f, startDelay: 1).<b>ToYieldInstruction()</b>;
    yield return Tween.Rotation(transform, endValue: new Vector3(0f, 0f, 45f), duration: 1f).<b>ToYieldInstruction()</b>;
    Debug.Log("Sequence completed");
}
</pre>
### Async/await
And the last method is awaiting tweens and sequences using the **async/await** pattern. All three sequencing methods produce the same result, so choose one that best suits your use case.
<pre>
<b>async</b> void AsyncMethod() {
    Tween.PositionX(transform, endValue: 10f, duration: 1.5f);
    <b>await</b> Tween.LocalScale(transform, endValue: 2f, duration: 0.5f, startDelay: 1);
    <b>await</b> Tween.Rotation(transform, endValue: new Vector3(0f, 0f, 45f), duration: 1f);
    Debug.Log("Sequence completed");
}
</pre>
> While PrimeTween never allocates memory, the async/await feature in C# is allocating. Consider using [UniTask](https://github.com/Cysharp/UniTask) to address this language limitation.

Inspector integration
---
Inspector integration is the cornerstone of PrimeTween's design that lets you tweak all animation properties from the Inspector without changing the code. It **saves time** and gives **creative freedom**. All animation settings can be **serialized** in MonoBehaviour or ScriptableObject and passed to the corresponding Tween methods.
<pre>
// Tweak all animation properties from the Inspector: startValue, endValue, duration, ease type (or custom curve), etc.
// Then pass the settings to the animation method 
[SerializeField] <b>TweenSettings.Float yPositionTweenSettings</b>;
Tween.PositionY(transform, <b>yPositionTweenSettings</b>);

[SerializeField] <b>TweenSettings.Vector3 rotationTweenSettings</b>;
Tween.Rotation(transform, <b>rotationTweenSettings</b>);

[SerializeField] <b>TweenSettings.Vector3 eulerAnglesTweenSettings</b>;
Tween.EulerAngles(transform, <b>eulerAnglesTweenSettings</b>);

[SerializeField] <b>ShakeSettings cameraShakeSettings</b>;
Tween.ShakeLocalPosition(Camera.main.transform, <b>cameraShakeSettings</b>);
</pre>

<img height="250" src="Documentation/inspector_integration.jpg" alt="100">

> The notable thing about setting up animation properties in the Inspector is that you can quickly switch to a custom **animation curve** without touching the code.

Controlling tweens
---
All static Tween methods return a **Tween** struct. While the '**tween.IsAlive**' you can control it and access its properties such as duration, elapsedTime, progress, interpolationFactor, etc.

After completion, the tween becomes 'dead' and can't be reused. This ensures that completed tweens don't eat computing resources and prevents the common performance pitfalls encountered in other tween libraries.

<pre>
Tween tween = Tween.LocalPositionX(transform, endValue: 1.5f, duration: 1);
// ... 
if (tween.<b>IsAlive</b>) {
    // 'IsAlive' means the tween was created and not completed (or manually stopped) yet.
    // While the tween 'IsAlive' you can access its properties such as duration, elapsedTime, progress, interpolationFactor, etc.
    Debug.Log($"Animation is still running, elapsed time: {tween.<b>elapsedTime</b>}.");
}

// Pause the tween
tween.<b>IsPaused</b> = true;

// Interrupt the tween, leaving the animated value at the current state
tween.<b>Stop()</b>;

// Instantly complete the running tween and set the animated value to the endValue
tween.<b>Complete()</b>;
</pre>

As you can see, there is no way to change the direction of the currently running tween, it can only be **stopped** and **completed**. But how to play an animation **forward** and **backward**, for example to show or hide a window? Easy! Just start a new Tween in the desired direction.

In the next example, the window may change its state while the previous animation is still running. Generally, there is no need to stop the previously running tween in such cases. The new tween will start from the current position and  **overwrite** all previously running tweens on this target.

```csharp
[SerializeField] RectTransform window;

public void SetWindowOpened(bool isOpened) {
    Tween.UIAnchoredPositionY(window, endValue: isOpened ? 0 : -500, duration: 0.5f);
}
```

And to utilize the full power of PrimeTween, all window animation settings can come from the Inspector. Notice how the **isOpened** parameter is passed to the **WithDirection(bool toEndValue)** method. This helper method selects the target position based on the isOpened parameter. Nice and simple!

<pre>
[SerializeField] RectTransform window;
[SerializeField] TweenSettings.Float windowAnimationSettings;

public void SetWindowOpened(bool isOpened) {
    Tween.UIAnchoredPositionY(window, windowAnimationSettings.<b>WithDirection</b>(toEndValue: <b>isOpened</b>));
}
</pre>

Custom tweens
---
Use **Tween.Custom()** to animate literary anything. The supported types for custom tweens are float, Color, Vector2/3/4, Quaternion, and Rect.
<pre>float floatField;
Color colorField;

// Animate 'floatField' from 0 to 10 in 1 second
<b>Tween.Custom</b>(startValue: 0, endValue: 10, duration: 1, onValueChange: newVal => <b>floatField</b> = newVal);

// Animate 'colorField' from white to black in 1 second
<b>Tween.Custom</b>(startValue: Color.white, endValue: Color.black, duration: 1, onValueChange: newVal => <b>colorField</b> = newVal);
</pre>


As you may expect, custom tweens work with [inspector integration](#inspector-integration) the same way as regular tweens do.
<pre>
[SerializeField] TweenSettings.Float <b>tweenSettings</b>;
float floatField;

Tween.Custom(<b>tweenSettings</b>, onValueChange: newVal => floatField = newVal);
</pre>

Zero allocations with delegates
---
C# delegates is a powerful language feature essential for game development. It gives us the ability to receive callbacks and pass methods to other methods. But when delegates are used in hot code paths carelessly, they can create [performance issues](https://www.jacksondunstan.com/articles/3765).

Let's review the code from earlier. If SomeMethod() is an instance method, then calling it from the callback will implicitly capture **'this'** reference, allocating heap memory.
<pre>
Tween.Position(transform, new Vector3(10, 0), duration: 1)
    .OnComplete(() => <b>SomeMethod()</b>); // memory allocation because '<b>this</b>' reference is implicitly captured
</pre>

Here is how to fix the above code to be non-allocating. Notice how the '**target**' parameter is used instead of calling SomeMethod() directly.
<pre>
Tween.Position(transform, new Vector3(10, 0), duration: 1)
    .OnComplete(target: <b>this</b>, target => <b>target</b>.SomeMethod());
</pre>
That's it! The same non-allocating approach can be used in all places where PrimeTween uses delegates.
<pre>
Tween.Delay(target: <b>this</b>, duration: 1f, target => <b>target</b>.SomeMethod());
Tween.Custom(target: <b>this</b>, startValue: 0, endValue: 10, duration: 1, onValueChange: (target, newVal) => <b>target</b>.floatField = newVal);

var shakeSettings = new ShakeSettings(frequency: 10, strength: Vector3.one, duration: 1);
Tween.ShakeCustom(target: <b>this</b>, shakeSettings, startValue: vector3Field, onValueChange: (target, newVal) => <b>target</b>.vector3Field = newVal);
</pre>

Debugging tweens
---
To debug tweens select the **PrimeTweenManager** object under the DontDestroyOnLoad foldout in the scene Hierarchy. Here you can inspect all currently running tweens and their properties.

<img height="300" src="Documentation/debug_tweens.jpg" alt="100">

Also, the Inspector shows the '**Max alive tweens**' for the current session. Use this number to estimate the maximum number of tweens required for your game and pass it to the **PrimeTweenConfig.SetTweensCapacity(int capacity)** method at the launch of your game. This will ensure PrimeTween doesn't allocate any additional memory at runtime.