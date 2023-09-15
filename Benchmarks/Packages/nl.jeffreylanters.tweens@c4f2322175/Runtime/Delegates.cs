using UnityEngine;

namespace Tweens {
  internal delegate DataType LerpDelegate<DataType>(DataType from, DataType to, float time);
  internal delegate void ApplyDelegate<ComponentType, DataType>(ComponentType component, DataType value);
  internal delegate float EaseFunctionDelegate(float time);
  public delegate void OnUpdateDelegate<ComponentType, DataType>(TweenInstance<ComponentType, DataType> instance, DataType value) where ComponentType : Component;
  public delegate void OnAddDelegate<ComponentType, DataType>(TweenInstance<ComponentType, DataType> instance) where ComponentType : Component;
  public delegate void OnStartDelegate<ComponentType, DataType>(TweenInstance<ComponentType, DataType> instance) where ComponentType : Component;
  public delegate void OnEndDelegate<ComponentType, DataType>(TweenInstance<ComponentType, DataType> instance) where ComponentType : Component;
  public delegate void OnCancelDelegate<ComponentType, DataType>(TweenInstance<ComponentType, DataType> instance) where ComponentType : Component;
  public delegate void OnFinallyDelegate<ComponentType, DataType>(TweenInstance<ComponentType, DataType> instance) where ComponentType : Component;
}