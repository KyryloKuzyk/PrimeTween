using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class AnchoredPositionTween : Tween<RectTransform, Vector2> {
    internal sealed override Vector2 Current(RectTransform component) {
      return component.anchoredPosition;
    }

    internal sealed override Vector2 Lerp(Vector2 from, Vector2 to, float time) {
      return Vector2.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(RectTransform component, Vector2 value) {
      component.anchoredPosition = value;
    }
  }
}