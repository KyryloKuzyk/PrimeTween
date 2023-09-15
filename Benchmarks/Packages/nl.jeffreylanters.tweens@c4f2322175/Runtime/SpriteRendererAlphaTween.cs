using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class SpriteRendererAlphaTween : Tween<SpriteRenderer, float> {
    internal sealed override float Current(SpriteRenderer component) {
      return component.color.a;
    }

    internal sealed override float Lerp(float from, float to, float time) {
      return Mathf.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(SpriteRenderer component, float value) {
      var color = component.color;
      color.a = value;
      component.color = color;
    }
  }
}