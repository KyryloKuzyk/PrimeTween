using System.Collections.Generic;
using UnityEngine;

namespace Tweens.Core {
  static class TweenEngine {
    readonly internal static List<TweenInstance> instances = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    internal static void Initialize() {
      Object.DontDestroyOnLoad(new GameObject("TweenBehaviour").AddComponent<TweenBehaviour>());
    }

    class TweenBehaviour : MonoBehaviour {
      void LateUpdate() => Update();
      void OnDestroy() => instances.Clear();
    }

    internal static void Update() {
      for (var i = 0; i < instances.Count; i += 1) {
        var instance = instances[i];
        if (instance.isDecommissioned) {
          instances.Remove(instance);
          i -= 1;
          continue;
        }
        instance.Update();
      }
    }
  }
}