using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class LocalScaleZTween : Tween<Transform, float> {
    internal sealed override float Current(Transform component) {
      return component.localScale.z;
    }

    internal sealed override float Lerp(float from, float to, float time) {
      return Mathf.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(Transform component, float value) {
      var localScale = component.localScale;
      localScale.z = value;
      component.localScale = localScale;
    }
  }
}