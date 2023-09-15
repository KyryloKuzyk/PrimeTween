using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class LocalPositionTween : Tween<Transform, Vector3> {
    internal sealed override Vector3 Current(Transform component) {
      return component.localPosition;
    }

    internal sealed override Vector3 Lerp(Vector3 from, Vector3 to, float time) {
      return Vector3.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(Transform component, Vector3 value) {
      component.localPosition = value;
    }
  }
}