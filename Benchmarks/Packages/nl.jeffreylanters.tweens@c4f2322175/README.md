<div align="center">

![readme splash](https://raw.githubusercontent.com/jeffreylanters/unity-tweens/master/.github/WIKI/repository-readme-splash.png)

[![license](https://img.shields.io/badge/mit-license-red.svg?style=for-the-badge)](https://github.com/jeffreylanters/unity-tweens/blob/master/LICENSE.md)
[![openupm](https://img.shields.io/npm/v/nl.jeffreylanters.tweens?label=UPM&registry_uri=https://package.openupm.com&style=for-the-badge&color=232c37)](https://openupm.com/packages/nl.jeffreylanters.tweens/)
[![build](https://img.shields.io/badge/build-passing-brightgreen.svg?style=for-the-badge)](https://github.com/jeffreylanters/unity-tweens/actions)
[![deployment](https://img.shields.io/badge/state-success-brightgreen.svg?style=for-the-badge)](https://github.com/jeffreylanters/unity-tweens/deployments)
[![stars](https://img.shields.io/github/stars/jeffreylanters/unity-tweens.svg?style=for-the-badge&color=fe8523&label=stargazers)](https://github.com/jeffreylanters/unity-tweens/stargazers)
[![downloads](https://img.shields.io/badge/dynamic/json?color=40AA72&style=for-the-badge&label=downloads&query=%24.downloads&url=https%3A%2F%2Fpackage.openupm.com%2Fdownloads%2Fpoint%2Fall-time%2Fnl.jeffreylanters.tweens)](https://openupm.com/packages/nl.jeffreylanters.tweens/)
[![size](https://img.shields.io/github/languages/code-size/jeffreylanters/unity-tweens?style=for-the-badge)](https://github.com/jeffreylanters/unity-tweens/blob/master/Runtime)
[![sponsors](https://img.shields.io/github/sponsors/jeffreylanters?color=E12C9A&style=for-the-badge)](https://github.com/sponsors/jeffreylanters)
[![donate](https://img.shields.io/badge/donate-paypal-F23150?style=for-the-badge)](https://paypal.me/jeffreylanters)
[![awesome](https://img.shields.io/badge/listed-awesome-fc60a8.svg?style=for-the-badge)](https://github.com/jeffreylanters/awesome-unity-packages)

An extremely light weight, extendable and customisable tweening engine made for strictly typed script-based animations for user-interfaces and world-space objects optimised for all platforms.

[**Installation**](#installation) &middot;
[**Documentation**](#documentation) &middot;
[**License**](./LICENSE.md)

**Made with &hearts; by Jeffrey Lanters**

</div>

# Installation

### Using the Unity Package Manager

Install the latest stable release using the Unity Package Manager by adding the following line to your `manifest.json` file located within your project's Packages directory, or by adding the Git URL to the Package Manager Window inside of Unity.

```json
"nl.jeffreylanters.tweens": "git+https://github.com/jeffreylanters/unity-tweens"
```

### Using OpenUPM

The module is availble on the OpenUPM package registry, you can install the latest stable release using the OpenUPM Package manager's Command Line Tool using the following command.

```sh
openupm add nl.jeffreylanters.tweens
```

# Documentation

Tweens focuses on providing a simple and easy to use API for creating and managing tween animations. The module is designed to be as lightweight as possible, while still providing a wide range of features. The module is also designed to be as extendable as possible, allowing you to create your own custom Tweens and Tweens while keeping typings strict.

- [Getting Started](#getting-started) - Code examples on how to create your first Tween
- [Tween Types](#tween-types) - A list of all available Tween types
- [Tween Options](#tween-options) - A list of all available Tween options
- [Tween Instances](#tween-instances) - Methods available on Tween instances
- [Extensions](#extensions) - Extensions available by the Tween module
- [Advanced Examples](#advanced-examples) - Advanced examples on how to use the Tween module

Still using version 2 of Tweens? View the [documentation here](https://github.com/jeffreylanters/unity-tweens/tree/v2.1.0)!

## Getting Started

To get started, create a new instance of one of the many available Tween types and add it to a GameObject. The following example shows how to create a new PositionTween and add it to a GameObject in order to move a GameObject from its current position to a new position.

```cs
var tween = new PositionTween {
  to = new Vector3(10, 5, 20),
  duration = 5,
};
gameObject.AddTween(tween);
```

The PositionTween in this example represents a configuration rather than a running tween, so you can reuse it as many times as you need while it can also be altered during uses.

```cs
var tween = new PositionTween {
  to = new Vector3(10, 5, 20),
  duration = 5,
};
gameObject.AddTween(tween);
differentGameObject.AddTween(tween);
tween.to.x = 20;
otherGameObject.AddTween(tween);
```

When a Tween is added, an Instance will be returned. This is where the Tween will be running. The Instance can be used to control the Tween, for example to pause, resume or cancel the Tween.

```cs
var tween = new PositionTween { };
var instance = gameObject.AddTween(tween);
instance.Cancel();
```

Use the built-in Tween Inspector to analyze and debug Tweens. The Tween Inspector can be found in the Window menu under the Analysis category.

![Tween Inspector Window](https://raw.githubusercontent.com/jeffreylanters/unity-tweens/master/.github/WIKI/tween-inspector.png)

These are just some of the many options available to you, for more information on how to use this Tweens. Not only are there many different types of Tweens, but there are also many different options available to you. For more information on how to use this module, please refer to the rest of the documentation.

Happy Tweening!

## Tween Types

To start animating a value, you will need to create a new Tween. The following sections will list all available Tween types. When a Tween will animate a specific value within a Component, the Tween will get the required Component from the GameObject automatically. When the Component is not available, the Tween will be cancelled.

### Transform

The following Tween Types can be used to alter values of a Transform Component; `PositionTween`, `PositionXTween`, `PositionYTween`, `PositionZTween`, `LocalPositionTween`, `LocalPositionXTween`, `LocalPositionYTween`, `LocalPositionZTween`, `RotationTween`, `LocalRotationTween`, `EulerAnglesTween`, `EulerAnglesXTween`, `EulerAnglesYTween`, `EulerAnglesZTween`, `LocalEulerAnglesTween`, `LocalEulerAnglesXTween`, `LocalEulerAnglesYTween`, `LocalEulerAnglesZTween`, `LocalScaleTween`, `LocalScaleXTween`, `LocalScaleTweenY`, `LocalScaleTweenZ`.

### Rect Transform

The following Tween Types can be used to alter values of a Rect Transform Component; `AnchoredPositionTween`, `AnchoredPositionXTween`, `AnchoredPositionYTween`, `AnchorMinTween`, `AnchorMaxTween`.

### Sprite Renderer

The following Tween Types can be used to alter values of a Sprite Renderer Component; `SpriteRendererAlphaTween`, `SpriteRendererColorTween`.

### Image

The following Tween Types can only be used if the `requires com.unity.ugui` package is installed in your project, and can be used to alter values of an Image Component; `ImageFillAmountTween`.

### Graphic

The following Tween Types can only be used if the `requires com.unity.ugui` package is installed in your project, and can be used to alter values of a Graphic Component; `GraphicAlphaTween`, `GraphicColorTween`.

### Audio Source

The following Tween Types can be used to alter values of an Audio Source Component; `AudioSourceVolumeTween`, `AudioSourcePitchTween`, `AudioSourcePanTween`, `AudioSourcePriorityTween`, `AudioSourceReverbZoneMixTween`, `AudioSourceSpatialBlendTween`.

### Light

The following Tween Types can be used to alter values of a Light Component; `LightColorTween`, `LightIntensityTween`, `LightRangeTween`, `LightSpotAngleTween`.

### Generic

The following Tween Types can be used to alter values of any property; `FloatTween`, `Vector2Tween`, `Vector3Tween`, `Vector4Tween`, `ColorTween`, `QuaternionTween`, `RectTween`.

## Tween Options

While the Tween Type defines what the Tween will do, the Tween Options define how the Tween will do it. In the following sections, you will find a list of all available Tween Options.

### From

The from value defines the starting value of the Tween. When the from value is not set, the Tween will use the current value of the property.

```cs
DataType from;
```

```cs
var tween = new ExampleTween {
  from = new Vector3(10, 5, 20),
};
```

### To

The to value defines the end value of the Tween. When the to value is not set, the Tween will use the current value of the property.

```cs
DataType to;
```

```cs
var tween = new ExampleTween {
  to = new Vector3(10, 5, 20),
};
```

### Duration

The duration of the Tween in seconds defines how long the Tween will take to complete. When the duration is not set, the Tween will complete instantly.

```cs
float duration;
```

```cs
var tween = new ExampleTween {
  duration = 5,
};
```

### Delay

The delay of the Tween in seconds defines how long the Tween will wait before starting. To change the behaviour of how to the delay will affect the Tween before it starts, you can change the [Fill Mode](#fill-mode). When the delay is not set, the Tween will start instantly.

```cs
float delay;
```

```cs
var tween = new ExampleTween {
  delay = 5,
};
```

### Loops

The amount of times the Tween will loop defines how many times the Tween will repeat itself. When the Tween is using a [Ping Pong](#ping-pong) loop type, the Tween has to play both the forward and backward animation to count as one loop. When [Infinite](#infinite) is set, the Tween will loop forever and the loop count will be ignored. When the amount of loops is not set, the Tween will not loop.

```cs
int loops;
```

```cs
var tween = new ExampleTween {
  loops = 5,
};
```

### Infinite

The infinite option defines whether the Tween will loop forever. When the Tween is set to loop forever, the [Loops](#loops) option will be ignored. When the infinite option is not set, the Tween will not loop forever.

```cs
bool isInfinite;
```

```cs
var tween = new ExampleTween {
  isInfinite = true,
};
```

### Ping Pong

The ping pong option defines whether the Tween will play the animation backwards after the animation has finished. When the ping pong option is not set, the Tween will not play the animation backwards after the animation has finished.

```cs
bool usePingPong;
```

```cs
var tween = new ExampleTween {
  usePingPong = true,
};
```

### Ping Pong Interval

The ping pong interval defines how long the Tween will wait before playing the animation backwards after the animation has finished. When the ping pong interval is not set, the Tween will play the animation backwards instantly after the animation has finished.

```cs
float pingPongInterval;
```

```cs
var tween = new ExampleTween {
  pingPongInterval = 5,
};
```

### Repeat Interval

The repeat interval defines how long the Tween will wait before repeating itself. When the repeat interval is not set, the Tween will repeat itself instantly after the animation has finished.

```cs
float repeatInterval;
```

```cs
var tween = new ExampleTween {
  repeatInterval = 5,
};
```

### Offset

The offset defines on which time the Tween will start. When the offset is not set, the Tween will start at the beginning.

```cs
float offset;
```

```cs
var tween = new ExampleTween {
  offset = 5,
};
```

### Ease Type

The ease type defines how the Tween will animate. If an [Animation Curve](#animation-curve) is set, the Ease Type won't be used. When the ease type is not set, the Tween will animate linearly.

The following Ease Types can be applied; `Linear`, `SineIn`, `SineOut`, `SineInOut`, `QuadIn`, `QuadOut`, `QuadInOut`, `CubicIn`, `CubicOut`, `CubicInOut`, `QuartIn`, `QuartOut`, `QuartInOut`, `QuintIn`, `QuintOut`, `QuintInOut`, `ExpoIn`, `ExpoOut`, `ExpoInOut`, `CircIn`, `CircOut`, `CircInOut`, `BackIn`, `BackOut`, `BackInOut`, `ElasticIn`, `ElasticOut`, `ElasticInOut`, `BounceIn`, `BounceOut`, `BounceInOut`.

```cs
EaseType easeType;
```

```cs
var tween = new ExampleTween {
  easeType = EaseType.QuadInOut,
};
```

### Animation Curve

The animation curve defines how the Tween will animate. The animation curve can be used to create custom ease types. When the animation curve is not set, the Tween will animate according to the [Ease Type](#ease-type).

```cs
AnimationCurve animationCurve;
```

```cs
var tween = new ExampleTween {
  animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1),
};
```

### Use Unscaled Time

The use unscaled time option defines whether the Tween will use the unscaled time. When the use unscaled time option is not set, the Tween will use the scaled time.

```cs
bool useUnscaledTime;
```

```cs
var tween = new ExampleTween {
  useUnscaledTime = true,
};
```

### Fill Mode

The fill mode defines how the Tween will behave before the Tween has started and after the Tween has ended. When the fill mode is not set, the fill mode will be set to `Backward`.

- `None` - The animation will not be applied before the Tween has started, and will return to its original state after the Tween has ended.
- `Forward` - The animation will be applied before the Tween has started, but will return to its original state after the Tween has ended.
- `Backward` - The animation will not be applied before the Tween has started, but will remain in its final state after the Tween has ended.
- `Both` - The animation will be applied before the Tween has started, and will remain in its final state after the Tween has ended.

```cs
FillMode fillMode;
```

```cs
var tween = new ExampleTween {
  fillMode = FillMode.Both,
};
```

### On Add

The on add delegate will be invoked when the Tween has been added to a GameObject.

```cs
OnAddDelegate<ComponentType, DataType> onAdd;
```

```cs
var tween = new ExampleTween {
  onAdd = (instance) => {
    Debug.Log("Tween has been added");
  },
};
```

### On Start

The on start delegate will be invoked when the Tween has started.

```cs
OnStartDelegate<ComponentType, DataType> onStart;
```

```cs
var tween = new ExampleTween {
  onStart = (instance) => {
    Debug.Log("Tween has started");
  },
};
```

### On Update

The on update delegate will be invoked when the Tween has updated.

```cs
OnUpdateDelegate<ComponentType, DataType> onUpdate;
```

```cs
var tween = new ExampleTween {
  onUpdate = (instance, value) => {
    Debug.Log("Tween has updated");
  },
};
```

### on End

The on end delegate will be invoked when the Tween has ended.

```cs
OnEndDelegate<ComponentType, DataType> onEnd;
```

```cs
var tween = new ExampleTween {
  onEnd = (instance) => {
    Debug.Log("Tween has ended");
  },
};
```

### On Cancel

The on cancel delegate will be invoked when the Tween has been cancelled.

```cs
OnCancelDelegate<ComponentType, DataType> onCancel;
```

```cs
var tween = new ExampleTween {
  onCancel = (instance) => {
    Debug.Log("Tween has been cancelled");
  },
};
```

### On Finally

The on finally delegate will be invoked when the Tween has ended or has been cancelled.

```cs
OnFinallyDelegate<ComponentType, DataType> onFinally;
```

```cs
var tween = new ExampleTween {
  onFinally = (instance) => {
    Debug.Log("Tween has ended or has been cancelled");
  },
};
```

## Tween Instances

When a Tween is added to a GameObject, an Instance will be returned. This is where the Tween will be running. The Instance can be used to control the Tween, for example to pause, resume or cancel the Tween.

### Cancel

The cancel method will cancel the Tween. When the Tween is cancelled, the [On Cancel](#on-cancel) and [On Finally](#on-finally) delegates will be invoked.

```cs
void Cancel();
```

```cs
var tween = new ExampleTween { };
var instance = gameObject.AddTween(tween);
instance.Cancel();
```

### Is Paused

The is paused property will return whether the Tween is paused while also allowing you to pause the Tween.

```cs
bool isPaused;
```

```cs
var tween = new ExampleTween { };
var instance = gameObject.AddTween(tween);
instance.isPaused = true;
```

### Target

The target property defines the target GameObject on which the Tween is running.

```cs
readonly GameObject target;
```

```cs
var tween = new ExampleTween { };
var instance = gameObject.AddTween(tween);
Debug.Log(instance.target);
```

## Extensions

Tweens also provides extension methods that can be used to control the Tween module.

### Add Tween

The add tween method will add a new Tween to the target GameObject. When the Tween is added, an Instance will be returned. This is where the Tween will be running. The Instance can be used to control the Tween, for example to pause, resume or cancel the Tween.

```cs
TweenInstance<ComponentType, DataType> AddTween<ComponentType, DataType>(this GameObject target, Tween<ComponentType, DataType> tween) where ComponentType : Component;
```

```cs
var tween = new ExampleTween { };
var instance = gameObject.AddTween(tween);
```

### Cancel Tweens

The cancel tweens method will cancel all Tweens on the target GameObject. When a Tween is cancelled, the [On Cancel](#on-cancel) and [On Finally](#on-finally) delegates will be invoked. When the include children option is set, all Tweens on the children of the target GameObject will also be cancelled, otherwise only the Tweens on the target GameObject will be cancelled.

```cs
void CancelTweens(this GameObject target, bool includeChildren = false);
```

```cs
gameObject.CancelTweens();
```

## Advanced Examples

Besides the many different types of Tweens and Tween Options, Tweens also provides a wide range of features that can be used to create advanced animations. The following sections will show you how to implemented some of these features to create advanced animation logic.

### Tweening Custom Values

The following example shows how to create a custom Tween that can be used to animate a value of an enemy Component. The Tween will animate the value of the Component from the current value to the new value.

```cs
var enemy = GetComponent<Enemy>();
var tween = new FloatTween {
  from = enemy.health,
  to = 23,
  duration = 1,
  easeType = EaseType.SineOut,
  onUpdate = (_, value) => enemy.health = value,
};
enemy.gameObject.AddTween(tween);
```
