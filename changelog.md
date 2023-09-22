## [1.0.12] - 2022-09-22
### Added
- Experimental: add parametric Easing.Bounce(float strength) and Easing.BounceExact(float magnitude). BounceExact allows to specify the exact bounce amplitude in meters/degrees regardless of the total tween distance. This feature requires the PRIME_TWEEN_EXPERIMENTAL define.
- Add From(fromValue) method to adapter.
- Support parametric OutBack and OutElastic eases in the adapter.
### Changed
- Easing.Elastic: normalize the oscillation period by duration; this ensures the tween has the same period regardless of duration.

## [1.0.11] - 2022-09-21
### Added
- Tween.GlobalTimeScale(), Tween.TweenTimeScale(), and tween.timeScale are no longer experimental.
- Add tween.OnUpdate() to execute a custom callback when the animated value is updated.
- Experimental: add Easing.Overshoot(float strength) and Easing.Elastic(float strength, float normalizedPeriod) methods to customize Ease.OutBounce and Ease.OutElastic.
### Changed
- Tween methods now accept Easing struct instead of AnimationCurve. You can continue using AnimationCurve as before because it is implicitly converted to the Easing struct.
- Ease.OutElastic now works the same as the most popular js and C# tween libraries.
### Fixed
- Add PrimeTween.Demo namespace to Demo scripts.

## [1.0.10] - 2022-09-13
### Added
- Add Tween.XxxAtSpeed() methods to create animations based on speed instead of duration.
- Add Tween.GetTweensCount() method.
### Changed
- Improve performance.

## [1.0.9] - 2022-09-08
### Changed
- Tween.StopAll(null) and Tween.CompleteAll(null) now immediately clean the internal tweens list, so PrimeTweenConfig.SetTweensCapacity() can be called immediately after that.
### Fixed
- Fixed: Tween.ShakeLocalRotation() doesn't work correctly.

## [1.0.8] - 2022-09-07
### Added
- Tween.Delay(), tween.OnComplete(), and sequence.ChainDelay() methods now accept the 'warnIfTargetDestroyed' parameter to control whether PrimeTween should log the error about tween's target destruction. More info: https://github.com/KyryloKuzyk/PrimeTween/discussions/4
### Fixed
- Fix compilation error with PRIME_TWEEN_DOTWEEN_ADAPTER.

## [1.0.7] - 2022-09-01
### Added
- Add 'UI Toolkit' support (com.unity.modules.uielements). All animations from the [ITransitionAnimations](https://docs.unity3d.com/2023.2/Documentation/ScriptReference/UIElements.Experimental.ITransitionAnimations.html) interface are supported. 
- Experimental: add Tween.PositionOutBounce/LocalPositionOutBounce() methods. These methods add the ability to fine-tune the Ease.OutBounce by specifying the exact bounce amplitude, number of bounces, and bounce stiffness.
- Log error when properties of dead tweens and sequences are used.
### Changed
- Rename 'Tween.LocalScale()' methods to 'Tween.Scale()'.
- Rename 'IsAlive' to 'isAlive'.
- Rename 'IsPaused' to 'isPaused'.

## [1.0.5] - 2022-08-29
### Added
- Experimental: add tween.timeScale; Tween.TweenTimeScale(); Tween.GlobalTimeScale().
### Fixed
- Fixed: onComplete callback should not be called if Tween.OnComplete<T>(_target_, ...) has been destroyed.
- Fixed: 'additive' tweens don't work correctly.

## [1.0.4] - 2022-08-22
### Added
- Warn if PrimeTween's API is used in Edit mode (when the scene is not running).
- Experimental: additive tweens.
### Changed
- Move Tween.Custom() 'AnimationCurve' parameter after 'onValueChange' parameter to be consistent with other overloads.
- Move Tween.ShakeCustom/PunchCustom() 'startValue' parameter before 'ShakeSettings' parameter to be consistent with Tween.Custom() methods.
### Fixed
- Normalize Quaternion passed to animation methods.

## [1.0.3] - 2022-08-15
### Added
- Sequence nesting: sequences can be grouped/chained to other sequences.
- Add startValue/endValue overload to all animation methods.
- Add Tween.ShakeCamera().
- Measure memory allocations in the Demo scene.

## [1.0.2] - 2022-05-29
### Added
- All tweens can now be viewed in PrimeTweenManager's inspector.
### Changed
- Tweens are now frame perfect. That is, on a stable framerate, tweens take a deterministic number of frames.

## [1.0.1] - 2022-04-28
### Added
- Add a version of Tween.Custom() that doesn't require passing a tween's target. Please note that this version will most probably generate garbage because of closure allocation.
- Add Tween.EulerAngles/LocalEulerAngles methods to animate rotation beyond 180 degrees.
- Add Demo scene.
### Changed
- Remove Tween.RotationXYZ/LocalRotationXYZ methods because manipulating Euler angles may lead to unexpected results. More info: https://docs.unity3d.com/ScriptReference/Transform-eulerAngles.html
- Tween.Rotation/LocalRotation methods now convert the Vector3 parameter to Quaternion instead of treating it as Euler angles.
### Fixed
- Property drawers don't work correctly with copy-pasting and prefabs.
- Fixed: samples asmdef has dependency duplication that leads to compilation error.
- Fixed: custom curve should not be clamped at the start or the end of the animation.

## [1.0.0] - 2022-04-20
### Added
- Animate anything (UI, Transform, Material, etc.) with zero allocations.
- Group animations in Sequences with zero allocations.
- Shake anything.
- Everything is tweakable from the Inspector.
- Async/await support.
- Coroutines support.