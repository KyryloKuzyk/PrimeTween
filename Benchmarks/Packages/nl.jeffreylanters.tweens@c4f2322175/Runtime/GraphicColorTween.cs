#if TWEENS_DEFINED_COM_UNITY_UGUI
using Tweens.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Tweens {
  public sealed class GraphicColorTween : Tween<Graphic, Color> {
    internal sealed override Color Current(Graphic component) {
      return component.color;
    }

    internal sealed override Color Lerp(Color from, Color to, float time) {
      return Color.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(Graphic component, Color value) {
      component.color = value;
    }
  }
}
#endif