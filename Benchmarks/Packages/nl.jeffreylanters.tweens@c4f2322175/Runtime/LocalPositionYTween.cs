using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class LocalPositionYTween : Tween<Transform, float> {
    internal sealed override float Current(Transform component) {
      return component.localPosition.y;
    }

    internal sealed override float Lerp(float from, float to, float time) {
      return Mathf.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(Transform component, float value) {
      var localPosition = component.localPosition;
      localPosition.y = value;
      component.localPosition = localPosition;
    }
  }
}