## [1.3.1] - 2025-04-27
### Added
- Add Edit mode support, so animations can be played in Editor without entering the Play mode. https://github.com/KyryloKuzyk/PrimeTween/discussions/62
- PrimeTween can now be installed via Unity Package Manager.
- Demo project: add 'Play Animation' button to Inspector to preview animations in Edit mode without entering Play mode.
- Add the experimental `ResetAfterComplete()` method to reset animations to the initial value before completion. https://github.com/KyryloKuzyk/PrimeTween/discussions/153
- Add the experimental `PrimeTweenConfig.ManualInitialize()` to initialize PrimeTween before `RuntimeInitializeLoadType.BeforeSceneLoad`. https://github.com/KyryloKuzyk/PrimeTween/issues/150
- Add `Tween.VisualElementOpacity()` method to animate the `VisualElement.style.opacity` property. Also, extend `Tween.Color/Alpha` methods to also work with VisualElement.
### Changed
- Passing `null` to `OnComplete()` is now allowed and no longer results in an error. https://github.com/KyryloKuzyk/PrimeTween/discussions/164
### Fixed
- Fixed: PrimeTween doesn't work after scripts are recompiled while playing in Editor when the 'Recompile And Continue Playing' setting is enabled.

## [1.3.0] - 2025-04-04
### Added
- Add support for updating animations in LateUpdate with the help of the new 'UpdateType updateType' parameter. The available options are Update, LateUpdate, and FixedUpdate. https://github.com/KyryloKuzyk/PrimeTween/issues/138
- Add 'Vector3' overloads to RotationAtSpeed() and LocalRotationAtSpeed() methods.
- Add 'TextFontSize()' to animate 'TextMeshPro.fontSize' property. https://github.com/KyryloKuzyk/PrimeTween/discussions/129
- Add a message to the exception thrown when an invalid ease is provided to `StandardEasing.Evaluate()`. https://github.com/KyryloKuzyk/PrimeTween/issues/151
### Changed
- Change 'bool useFixedUpdate' to 'UpdateType updateType'. The new version comes with an automatic script updater. Please back up your project before updating.
- Make the exception stack trace clickable when custom tween throws an exception. https://github.com/KyryloKuzyk/PrimeTween/issues/119
- Prevent allocations in development builds when  PRIME_TWEEN_SAFETY_CHECKS is enabled. 

## [1.2.2] - 2024-12-05
### Fixed
- Fixed: TextMeshPro animations are not available in Unity 2023. Bug report: https://discussions.unity.com/t/primetween-high-performance-animations-and-sequences/926420/365
- Fixed: Demo project produces compilation errors if Ugui package is not installed.

## [1.2.1] - 2024-11-28
### Fixed
- Fixed: when a new tween is created at the last frame of `ToYieldInstruction()`, coroutine will wait for the wrong tween. Bug report: https://github.com/KyryloKuzyk/PrimeTween/issues/126

## [1.2.0] - 2024-10-03
### Fixed
- Fixed: sequence.Group() doesn't work correctly after ChainCallback() and InsertCallback(). More info: https://github.com/KyryloKuzyk/PrimeTween/discussions/112

## [1.1.22] - 2024-09-14
### Added
- Add `Easing.Evaluate(float interpolationFactor, Ease ease)` method that gives the ability to evaluate standard eases manually. Previously this method was experimental.
### Fixed
- Fixed: destroying target of a paused tween doesn't release the tween. Bug report: https://github.com/KyryloKuzyk/PrimeTween/discussions/4#discussioncomment-10555636
- Fixed: UPM examples are not compatible with Input System (New).

## [1.1.21] - 2024-08-31
### Added
- Implement GetHashCode() and IEquatable for Tween and Sequence.

## [1.1.20] - 2024-08-12
### Changed
- Change internal tween from 'int' to 'long', which reduces the likelihood of id collisions in apps that run for months or years. Feature request: https://discussions.unity.com/t/926420/291
### Added
- Add 'setImmediately' and 'isRelative' parameters to From() method in [Adapter](https://github.com/KyryloKuzyk/PrimeTween#dotween-adapter). Feature request: https://discussions.unity.com/t/926420/299
### Fixed
- Fixed: Demo scene doesn't work with Input System (New). Bug report: https://github.com/KyryloKuzyk/PrimeTween/issues/85
- Fixed (minor): calling Sequence.Complete() when the sequence is in Stater.After doesn't complete the sequence immediately.

## [1.1.19] - 2024-05-29
### Fixed
- Fixed: Tween.TextMaxVisibleCharacters() is not available in Unity 6 because Text Mesh Pro was merged with UGUI.

## [1.1.18] - 2024-05-11
### Fixed
- Fixed: nested sequence updates its children even when it doesn't update self. This resulted in multiple chained jumps to override the first jump. Bug report: https://forum.unity.com/threads/primetween-high-performance-animations-and-sequences.1479609/page-6#post-9802575 

## [1.1.17] - 2024-04-17
### Fixed
- Fixed: Tween.TextMaxVisibleCharacters() throws assertion exception.

## [1.1.16] - 2024-04-16
### Fixed
- Fixed: interpolation factor can be calculated incorrectly when cyclic tween is not started yet. Bug report: https://github.com/KyryloKuzyk/PrimeTween/issues/63
- Fixed: don't log an error when the child animation setting is the same as the parent Sequence's setting. Bug report: https://github.com/KyryloKuzyk/PrimeTween/issues/66
- Fixed: enabling PRIME_TWEEN_SAFETY_CHECKS in IL2CPP builds throws exception. Bug report: https://github.com/KyryloKuzyk/PrimeTween/issues/65
- Fixed: adding experimental define produces compilation error.

## [1.1.15] - 2024-04-07
### Fixed
- Fixed: Tween.GetTweenCount(target) doesn't work for tweens inside sequences. Bug report: https://github.com/KyryloKuzyk/PrimeTween/issues/60
- Fixed: ShakeSettings.strength property incorrectly displayed in narrow Inspector.
- Fixed: sometimes automatic installation fails and users have to press the 'PrimeTweenInstaller/Install' button to fix it.
- Fixed: review request should appear only after the user presses the Update button in Package Manager. Bug report: https://github.com/KyryloKuzyk/PrimeTween/issues/58

## [1.1.14] - 2024-03-16
### Added
- Add Tween.VisualElementColor/VisualElementBackgroundColor() to animate colors of UIToolkit components.
- Experimental: add 'double' numeric type support for Tween.Custom(). This feature requires the PRIME_TWEEN_EXPERIMENTAL define in 'Scripting Define Symbols'.
### Changed
- Improve documentation.

## [1.1.13] - 2024-02-23
### Added
- Add sequence.Pause/Play() to adapter.
### Changed
- If sequence child pauses the parent sequence, stop updating Sequence immediately. Other tweens after the current one will not receive an update.

## [1.1.12] - 2024-02-14
### Added
- Add experimental Easing.Evaluate(float, Ease).
- Add DOVirtual.EasedValue() to adapter.
- Add custom Ease.OutBounce strength support to adapter.
### Changed
- The 'warnEndValueEqualsCurrent' setting is now applied on a per-tween basis. Feature request: https://github.com/KyryloKuzyk/PrimeTween/discussions/48

## [1.1.11] - 2024-01-18
### Fixed
- Fixed: calling PrimeTweenConfig.SetTweensCapacity() before PrimeTweenManager is created throws an exception in Editor.
### Changed
- Optimize duplicated shakes on the same target.

## [1.1.10] - 2024-01-15
### Added
- Add sequence.Insert() and sequence.InsertCallback() APIs. Feature request: https://github.com/KyryloKuzyk/PrimeTween/discussions/33
- Add Asset Store review request when updating PrimeTween to a new version.
### Fixed
- Fixed: completing an infinite tween (cycles == -1) evaluates the interpolation factor to -1 and produces wrong animated value. Bug report: https://github.com/KyryloKuzyk/PrimeTween/issues/38
### Changed
- sequence.Complete() now respects the 'cycles' and 'cycleMode' and works similarly to tween.Complete(). Previuosly, sequence.Complete() completed only the current cycle. 
### Removed
- Remove Tween.PositionOutBounce() experimental API. Use Easing.BounceExact(float amplitude) instead.

## [1.1.9] - 2024-01-09
### Added
- [Adapter](https://github.com/KyryloKuzyk/PrimeTween#dotween-adapter): add 'loopType' parameter to sequence.SetLoops(); add sequence.SetEase(); add SetLink().
### Fixed
- Fixed: sequence.Group() after seqence.Chain() produces a wrong sequence duration (too short) if a grouped tween is longer than the previously chained tween.
- Fixed: sequence.Group(sequence) groups with the first tween in sequence, but should group with the previous tween (or sequence) instead. Now it works the same way as sequence.Group(tween).
- Fixed: sequence.OnComplete() should be called when all sequence children are completed. Previously it was called before the last children update.
- Fixed: setting 'elapsedTime' or 'progress' recursively from Tween.Custom() or tween.OnUpdate() leads to a crash because of stack overflow.
### Changed
- Remove the 'useUnscaledTime' parameter from sequence.ChainDelay(). The parent Sequence controls the 'useUnscaledTime' of all children tweens and sequences, so this parameter is not needed.

## [1.1.8] - 2024-01-05
### Added:
- Add more detailed warnings and errors.
### Fixed
- Fixed: Demo.cs causes compilation error, preventing PrimeTween to install correctly.

## [1.1.7] - 2024-01-02
### Fixed
- Fixed: ReusableTween.updateSequence() throws AssertionException when a sequence is running during scene change in the Editor. Bug report: https://github.com/KyryloKuzyk/PrimeTween/issues/32

## [1.1.6] - 2023-12-29
### Added:
- PrimeTween can now print tween's creation stack trace when an error occurs to make the debugging process a lot easier. This feature requires the 'PRIME_TWEEN_SAFETY_CHECKS' define.
- Add TypewriterAnimatorExample.TypewriterAnimationByWords() to the Demo scene to show an example of how to animate text word-by-word.
### Fixed
- Fixed: shake doesn't restore the position to the initial value if shake's period is less than duration.

## [1.1.5] - 2023-12-14
### Added:
- Add Tween.TextMaxVisibleCharacters(TMP_Text target, ...) method to support simple typewriter animation. Please see the 'Demo.TypewriterAnimatorExample.cs' for a more complex example of text animation that makes pauses after punctuations. 
### Fixed
- Fixed: don't allow to set invalid elapsedTimeTotal and progressTotal values to infinite tweens.

## [1.1.4] - 2023-12-10
### Added:
- Tween/Sequence.timeScale can now be negative to support backward movement (rewind).
- Add 'useFixedUpdate' parameter to update a tween or sequence in the FixedUpdate().
- Release Parametric Easing to production (previously this feature was experimental): https://github.com/KyryloKuzyk/PrimeTween#parametric-easing
### Changed
- The default 'easeBetweenShakes' is now Ease.OutQuad instead of Ease.OutSine.
- The 'useUnscaledTime' parameter now doesn't prevent the abrupt delta time changes when unpausing the Editor to be consistent with the official Unity's behavior.
### Fixed
- Fixed: shakes log the 'warnEndValueEqualsCurrent' warning if object's local position/rotation is Vector3.zero/Quaternion.identity.

## [1.1.3] - 2023-12-06
### Added:
- Add Sequence.OnComplete() API.
### Fixed
- Fixed: nesting multiple empty Sequences may lead to a StackOverflow exception in some cases.
- Fixed: enabling/disabling UI.Slider in Demo scene constantly allocates GC.  

## [1.1.2] - 2023-12-03
### Added:
- Add 'PrimeTweenConfig.warnEndValueEqualsCurrent' setting to warn if the 'endValue' equals to the current animated value.
### Fixed
- Fixed: PrimeTween may log warnings in Editor when exiting Play Mode. 

## [1.1.1] - 2023-11-30
### Added:
- Sequences now support CycleMode.Yoyo and CycleMode.Rewind with the help of Sequence.Create(numCycles, CycleMode **cycleMode**).
- Sequences now support easing that can be applied to the whole Sequence with the help of Sequence.Create(..., Ease **sequenceEase**).
- 'elapsedTime', 'elapsedTimeTotal', 'progress', and 'progressTotal' properties now have setters, so it's possible to manually set the elapsed time of tweens and sequences. Please see the Demo scene for usage example.
- Parent Sequence now controls the isPaused, timeScale, and useUnscaledTime of all its children tweens and sequences.
- Add a warning when tween.SetRemainingCycles() is called on Tween.Delay(). More info: https://discussions.unity.com/t/926420/101.
### Changed
- It's no longer allowed to Stop()/Complete() a tween inside a Sequence. Please Stop()/Complete() the parent Sequence instead.
- It's no longer allowed to await or use the '.ToYieldInstruction()' on tween inside a Sequence. Please use the parent Sequence instead.
- It's no longer allowed to add tweens to a started Sequence.
- It's now allowed to call Tween.StopAll(), Tween.CompleteAll() and Tween.SetPausedAll() from onValueChange, OnUpdate() and OnComplete().
- SetCycles() was renamed to SetRemainingCycles(). To set Sequence cycles, use Sequence.Create(cycles: numCycles).
- Remove 'minExpected' and 'numMaxExpected' parameters from Tween.StopAll/CompleteAll/SetPausedAll() methods.
### Fixed
- Fixed: Tween.GetTweensCount() may return the incorrect result if called from the OnComplete() callback.
- Fixed: MeasureMemoryAllocations.cs example script doesn't ignore its own allocations.

## [1.0.17] - 2023-11-12
### Fixed
- Fixed: the Demo scene doesn't compile if PrimeTween is not installed.

## [1.0.16] - 2023-11-04
### Fixed
- Fixed: Quaternion tweens don't work correctly with CycleMode.Incremental. Bug report: https://github.com/KyryloKuzyk/PrimeTween/issues/19

## [1.0.15] - 2023-10-14
### Fixed
- Fixed: the first time Sequence is created, it may play incorrectly in some cases.

## [1.0.14] - 2023-10-02
### Added
- Add methods to animate RectTransform.offsetMin/offsetMax.
- Add sequence.timeScale to set the timeScale for all tweens in a Sequence.
- Add Tween.TweenTimeScale(Sequence) method to animate sequence.timeScale over time.
### Fixed
- Fixed: Unity's Time.unscaledDeltaTime reports the wrong value after unpausing a scene in the Editor.

## [1.0.13] - 2023-09-24
### Fixed
- Fixed: passing a null UnityEngine.Object to 'Tween.' methods causes null ref exception in PrimeTweenManager.Update(). Bug report: https://github.com/KyryloKuzyk/PrimeTween/issues/13

## [1.0.12] - 2023-09-22
### Added
- Experimental: add parametric Easing.Bounce(float strength) and Easing.BounceExact(float magnitude). BounceExact allows to specify the exact bounce amplitude in meters/degrees regardless of the total tween distance. This feature requires the PRIME_TWEEN_EXPERIMENTAL define.
- Add From(fromValue) method to adapter.
- Support parametric OutBack and OutElastic eases in the adapter.
### Changed
- Easing.Elastic: normalize the oscillation period by duration; this ensures the tween has the same period regardless of duration.

## [1.0.11] - 2023-09-21
### Added
- Tween.GlobalTimeScale(), Tween.TweenTimeScale(), and tween.timeScale are no longer experimental.
- Add tween.OnUpdate() to execute a custom callback when the animated value is updated.
- Experimental: add Easing.Overshoot(float strength) and Easing.Elastic(float strength, float normalizedPeriod) methods to customize Ease.OutBounce and Ease.OutElastic.
### Changed
- Tween methods now accept Easing struct instead of AnimationCurve. You can continue using AnimationCurve as before because it is implicitly converted to the Easing struct.
- Ease.OutElastic now works the same as the most popular js and C# tween libraries.
### Fixed
- Add PrimeTween.Demo namespace to Demo scripts.

## [1.0.10] - 2023-09-13
### Added
- Add Tween.XxxAtSpeed() methods to create animations based on speed instead of duration.
- Add Tween.GetTweensCount() method.
### Changed
- Improve performance.

## [1.0.9] - 2023-09-08
### Changed
- Tween.StopAll(null) and Tween.CompleteAll(null) now immediately clean the internal tweens list, so PrimeTweenConfig.SetTweensCapacity() can be called immediately after that.
### Fixed
- Fixed: Tween.ShakeLocalRotation() doesn't work correctly.

## [1.0.8] - 2023-09-07
### Added
- Tween.Delay(), tween.OnComplete(), and sequence.ChainDelay() methods now accept the 'warnIfTargetDestroyed' parameter to control whether PrimeTween should log the error about tween's target destruction. More info: https://github.com/KyryloKuzyk/PrimeTween/discussions/4
### Fixed
- Fix compilation error with PRIME_TWEEN_DOTWEEN_ADAPTER.

## [1.0.7] - 2023-09-01
### Added
- Add 'UI Toolkit' support (com.unity.modules.uielements). All animations from the [ITransitionAnimations](https://docs.unity3d.com/2023.2/Documentation/ScriptReference/UIElements.Experimental.ITransitionAnimations.html) interface are supported. 
- Experimental: add Tween.PositionOutBounce/LocalPositionOutBounce() methods. These methods add the ability to fine-tune the Ease.OutBounce by specifying the exact bounce amplitude, number of bounces, and bounce stiffness.
- Log error when properties of dead tweens and sequences are used.
### Changed
- Rename 'Tween.LocalScale()' methods to 'Tween.Scale()'.
- Rename 'IsAlive' to 'isAlive'.
- Rename 'IsPaused' to 'isPaused'.

## [1.0.5] - 2023-08-29
### Added
- Experimental: add tween.timeScale; Tween.TweenTimeScale(); Tween.GlobalTimeScale().
### Fixed
- Fixed: onComplete callback should not be called if Tween.OnComplete<T>(_target_, ...) has been destroyed.
- Fixed: 'additive' tweens don't work correctly.

## [1.0.4] - 2023-08-22
### Added
- Warn if PrimeTween's API is used in Edit mode (when the scene is not running).
- Experimental: additive tweens.
### Changed
- Move Tween.Custom() 'AnimationCurve' parameter after 'onValueChange' parameter to be consistent with other overloads.
- Move Tween.ShakeCustom/PunchCustom() 'startValue' parameter before 'ShakeSettings' parameter to be consistent with Tween.Custom() methods.
### Fixed
- Normalize Quaternion passed to animation methods.

## [1.0.3] - 2023-08-15
### Added
- Sequence nesting: sequences can be grouped/chained to other sequences.
- Add startValue/endValue overload to all animation methods.
- Add Tween.ShakeCamera().
- Measure memory allocations in the Demo scene.

## [1.0.2] - 2023-05-29
### Added
- All tweens can now be viewed in PrimeTweenManager's inspector.
### Changed
- Tweens are now frame perfect. That is, on a stable framerate, tweens take a deterministic number of frames.

## [1.0.1] - 2023-04-28
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

## [1.0.0] - 2023-04-20
### Added
- Animate anything (UI, Transform, Material, etc.) with zero allocations.
- Group animations in Sequences with zero allocations.
- Shake anything.
- Everything is tweakable from the Inspector.
- Async/await support.
- Coroutines support.
