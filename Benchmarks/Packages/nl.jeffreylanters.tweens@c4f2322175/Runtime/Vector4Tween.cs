using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class Vector4Tween : Tween<Transform, Vector4> {
    internal sealed override Vector4 Current(Transform component) {
      return Vector4.zero;
    }

    internal sealed override Vector4 Lerp(Vector4 from, Vector4 to, float time) {
      return Vector4.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(Transform component, Vector4 value) { }
  }
}