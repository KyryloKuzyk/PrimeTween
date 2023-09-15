using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class LocalEulerAnglesYTween : Tween<Transform, float> {
    internal sealed override float Current(Transform component) {
      return component.localEulerAngles.y;
    }

    internal sealed override float Lerp(float from, float to, float time) {
      return Mathf.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(Transform component, float value) {
      var localEulerAngles = component.localEulerAngles;
      localEulerAngles.y = value;
      component.localEulerAngles = localEulerAngles;
    }
  }
}