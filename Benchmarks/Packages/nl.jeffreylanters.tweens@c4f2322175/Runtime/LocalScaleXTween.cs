using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class LocalScaleXTween : Tween<Transform, float> {
    internal sealed override float Current(Transform component) {
      return component.localScale.x;
    }

    internal sealed override float Lerp(float from, float to, float time) {
      return Mathf.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(Transform component, float value) {
      var localScale = component.localScale;
      localScale.x = value;
      component.localScale = localScale;
    }
  }
}