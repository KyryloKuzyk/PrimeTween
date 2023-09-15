using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class ColorTween : Tween<Transform, Color> {
    internal sealed override Color Current(Transform component) {
      return Color.clear;
    }

    internal sealed override Color Lerp(Color from, Color to, float time) {
      return Color.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(Transform component, Color value) { }
  }
}