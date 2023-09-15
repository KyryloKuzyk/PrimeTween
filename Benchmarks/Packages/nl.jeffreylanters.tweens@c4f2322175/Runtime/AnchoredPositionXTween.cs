using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class AnchoredPositionXTween : Tween<RectTransform, float> {
    internal sealed override float Current(RectTransform component) {
      return component.anchoredPosition.x;
    }

    internal sealed override float Lerp(float from, float to, float time) {
      return Mathf.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(RectTransform component, float value) {
      var anchoredPosition = component.anchoredPosition;
      anchoredPosition.x = value;
      component.anchoredPosition = anchoredPosition;
    }
  }
}