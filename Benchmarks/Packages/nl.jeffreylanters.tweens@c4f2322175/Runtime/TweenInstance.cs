using UnityEngine;
using Tweens.Core;

namespace Tweens {
  public abstract class TweenInstance {
    readonly internal float duration;
    readonly internal float? pingPongInterval;
    readonly internal float? repeatInterval;
    readonly internal bool useUnscaledTime;
    readonly internal bool usePingPong;
    readonly internal FillMode fillMode = FillMode.Backwards;
    internal float? haltTime;
    internal int? loops;
    internal bool isDecommissioned;
    internal float time;
    internal bool isForwards = true;
    internal bool didReachEnd;
    internal EaseFunctionDelegate easeFunction;
    /// <summary>The target property defines the target GameObject on which the Tween is running.</summary>
    readonly public GameObject target;
    /// <summary>The is paused property will return whether the Tween is paused while also allowing you to pause the Tween.</summary>
    public bool isPaused;
#if UNITY_EDITOR
    readonly internal string @tweenTypeName;
#endif

    internal abstract void Update();
    /// <summary>The cancel method will cancel the Tween. When the Tween is cancelled, the OnCancel and OnFinally delegates will be invoked.</summary>
    public abstract void Cancel();

    internal TweenInstance(GameObject target, Tween tween) {
      this.target = target;
      duration = tween.duration > 0 ? tween.duration : 0.001f;
      pingPongInterval = tween.pingPongInterval;
      repeatInterval = tween.repeatInterval;
      useUnscaledTime = tween.useUnscaledTime;
      usePingPong = tween.usePingPong;
      fillMode = tween.fillMode;
      time = tween.offset ?? 0;
#if UNITY_EDITOR
      @tweenTypeName = tween.GetType().Name;
#endif
    }
  }

  public class TweenInstance<ComponentType, DataType> : TweenInstance where ComponentType : Component {
    readonly ComponentType component;
    readonly OnAddDelegate<ComponentType, DataType> onAdd;
    OnStartDelegate<ComponentType, DataType> onStart;
    readonly OnUpdateDelegate<ComponentType, DataType> onUpdate;
    readonly OnEndDelegate<ComponentType, DataType> onEnd;
    internal OnCancelDelegate<ComponentType, DataType> onCancel;
    internal OnFinallyDelegate<ComponentType, DataType> onFinally;
    readonly ApplyDelegate<ComponentType, DataType> apply;
    readonly LerpDelegate<DataType> lerp;
    readonly DataType initial;
    readonly DataType from;
    readonly DataType to;

    internal TweenInstance(GameObject target, Tween<ComponentType, DataType> tween) : base(target, tween) {
      component = target.GetComponent<ComponentType>();
      initial = tween.Current(component);
      from = tween.from != null ? tween.from : tween.Current(component);
      to = tween.to != null ? tween.to : tween.Current(component);
      onStart = tween.onStart;
      onAdd = tween.onAdd;
      onEnd = tween.onEnd;
      onCancel = tween.onCancel;
      onFinally = tween.onFinally;
      haltTime = tween.delay;
      loops = tween.isInfinite ? -1 : tween.loops;
      onUpdate = tween.onUpdate;
      apply = tween.Apply;
      lerp = tween.Lerp;
      easeFunction = tween.animationCurve != null ? new AnimationCurve(tween.animationCurve.keys).Evaluate : EaseFunctions.GetFunction(tween.easeType);
      onAdd?.Invoke(this);
      if (haltTime > 0 && (fillMode == FillMode.Both || fillMode == FillMode.Forwards)) {
        apply(component, from);
        onUpdate?.Invoke(this, from);
      }
    }

    internal sealed override void Update() {
      if (component == null) {
        Cancel();
        return;
      }
      if (isPaused) {
        return;
      }
      var deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
      if (haltTime.HasValue) {
        haltTime -= deltaTime;
        if (haltTime <= 0) {
          haltTime = null;
        }
        else {
          return;
        }
      }
      if (onStart != null) {
        onStart.Invoke(this);
        onStart = null;
      }
      var timeStep = deltaTime / duration;
      time += isForwards ? timeStep : -timeStep;
      if (time >= duration) {
        time = duration;
        if (usePingPong) {
          isForwards = false;
          haltTime = pingPongInterval ?? haltTime;
        }
        else {
          didReachEnd = true;
          haltTime = repeatInterval ?? haltTime;
        }
      }
      else if (usePingPong && time < 0) {
        time = 0;
        isForwards = true;
        didReachEnd = true;
        haltTime = repeatInterval ?? haltTime;
      }
      var easedTime = easeFunction(time / duration);
      var value = lerp(from, to, easedTime);
      apply(component, value);
      onUpdate?.Invoke(this, value);
      if (didReachEnd) {
        if (loops > 1 || loops == -1) {
          didReachEnd = false;
          time = 0;
          if (loops > 1) {
            loops -= 1;
          }
        }
        else {
          if (fillMode == FillMode.Forwards || fillMode == FillMode.None) {
            apply(component, initial);
            onUpdate?.Invoke(this, initial);
          }
          onEnd?.Invoke(this);
          onFinally?.Invoke(this);
          isDecommissioned = true;
        }
      }
    }

    /// <summary>The cancel method will cancel the Tween. When the Tween is cancelled, the OnCancel and OnFinally delegates will be invoked.</summary>
    public sealed override void Cancel() {
      onCancel?.Invoke(this);
      onFinally?.Invoke(this);
      isDecommissioned = true;
    }
  }
}