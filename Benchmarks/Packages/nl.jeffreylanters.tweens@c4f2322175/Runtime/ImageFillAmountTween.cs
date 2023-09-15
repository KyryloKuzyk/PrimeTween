#if TWEENS_DEFINED_COM_UNITY_UGUI
using Tweens.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Tweens {
  public sealed class ImageFillAmountTween : Tween<Image, float> {
    internal sealed override float Current(Image component) {
      return component.fillAmount;
    }

    internal sealed override float Lerp(float from, float to, float time) {
      return Mathf.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(Image component, float value) {
      component.fillAmount = value;
    }
  }
}
#endif