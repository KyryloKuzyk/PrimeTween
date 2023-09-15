using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class EulerAnglesZTween : Tween<Transform, float> {
    internal sealed override float Current(Transform component) {
      return component.eulerAngles.z;
    }

    internal sealed override float Lerp(float from, float to, float time) {
      return Mathf.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(Transform component, float value) {
      var eulerAngles = component.eulerAngles;
      eulerAngles.z = value;
      component.eulerAngles = eulerAngles;
    }
  }
}