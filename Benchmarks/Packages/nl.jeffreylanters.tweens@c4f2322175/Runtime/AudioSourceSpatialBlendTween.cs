/*using Tweens.Core;
using UnityEngine;

namespace Tweens {
  public sealed class AudioSourceSpatialBlendTween : Tween<AudioSource, float> {
    internal sealed override float Current(AudioSource component) {
      return component.spatialBlend;
    }

    internal sealed override float Lerp(float from, float to, float time) {
      return Mathf.LerpUnclamped(from, to, time);
    }

    internal sealed override void Apply(AudioSource component, float value) {
      component.spatialBlend = value;
    }
  }
}*/