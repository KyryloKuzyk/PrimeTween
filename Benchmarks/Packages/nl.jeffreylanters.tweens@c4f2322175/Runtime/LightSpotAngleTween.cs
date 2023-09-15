using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class LightSpotAngleTween : Tween<Light, float> {
    internal sealed override float Current(Light component) {
      return component.spotAngle;
    }

    internal sealed override float Lerp(float from, float to, float time) {
      return Mathf.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(Light component, float value) {
      component.spotAngle = value;
    }
  }
}