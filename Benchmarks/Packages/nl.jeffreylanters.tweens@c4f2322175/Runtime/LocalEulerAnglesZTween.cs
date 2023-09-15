using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class LocalEulerAnglesZTween : Tween<Transform, float> {
    internal sealed override float Current(Transform component) {
      return component.localEulerAngles.z;
    }

    internal sealed override float Lerp(float from, float to, float time) {
      return Mathf.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(Transform component, float value) {
      var localEulerAngles = component.localEulerAngles;
      localEulerAngles.z = value;
      component.localEulerAngles = localEulerAngles;
    }
  }
}