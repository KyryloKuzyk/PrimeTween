using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class EulerAnglesXTween : Tween<Transform, float> {
    internal sealed override float Current(Transform component) {
      return component.eulerAngles.x;
    }

    internal sealed override float Lerp(float from, float to, float time) {
      return Mathf.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(Transform component, float value) {
      var eulerAngles = component.eulerAngles;
      eulerAngles.x = value;
      component.eulerAngles = eulerAngles;
    }
  }
}