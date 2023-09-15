using UnityEngine;
using Tweens.Core;

namespace Tweens {
  public static class TweenExtension {
    public static TweenInstance<ComponentType, DataType> AddTween<ComponentType, DataType>(this GameObject target, Tween<ComponentType, DataType> tween) where ComponentType : Component {
      var instance = new TweenInstance<ComponentType, DataType>(target, tween);
      TweenEngine.instances.Add(instance);
      return instance;
    }

    public static TweenInstance[] AddTween<ComponentType1, DataType1, ComponentType2, DataType2>(this GameObject target, Tween<ComponentType1, DataType1> tween1, Tween<ComponentType2, DataType2> tween2) where ComponentType1 : Component where ComponentType2 : Component {
      var instances = new TweenInstance[] {
        new TweenInstance<ComponentType1, DataType1>(target, tween1),
        new TweenInstance<ComponentType2, DataType2>(target, tween2),
      };
      TweenEngine.instances.AddRange(instances);
      return instances;
    }

    public static TweenInstance[] AddTween<ComponentType1, DataType1, ComponentType2, DataType2, ComponentType3, DataType3>(this GameObject target, Tween<ComponentType1, DataType1> tween1, Tween<ComponentType2, DataType2> tween2, Tween<ComponentType3, DataType3> tween3) where ComponentType1 : Component where ComponentType2 : Component where ComponentType3 : Component {
      var instances = new TweenInstance[] {
        new TweenInstance<ComponentType1, DataType1>(target, tween1),
        new TweenInstance<ComponentType2, DataType2>(target, tween2),
        new TweenInstance<ComponentType3, DataType3>(target, tween3),
      };
      TweenEngine.instances.AddRange(instances);
      return instances;
    }

    public static GameObject CancelTweens(this GameObject target, bool includeChildren = false) {
      var instances = TweenEngine.instances.FindAll(instance => {
        if (instance.target == target) {
          return true;
        }
        if (includeChildren) {
          return instance.target.transform.IsChildOf(target.transform);
        }
        return false;
      });
      foreach (var instance in instances) {
        instance.Cancel();
      }
      return target;
    }
  }
}