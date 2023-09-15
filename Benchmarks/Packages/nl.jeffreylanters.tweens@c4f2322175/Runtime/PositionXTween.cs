using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class PositionXTween : Tween<Transform, float> {
    internal sealed override float Current(Transform component) {
      return component.position.x;
    }

    internal sealed override float Lerp(float from, float to, float time) {
      return Mathf.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(Transform component, float value) {
      var position = component.position;
      position.x = value;
      component.position = position;
    }
  }
}