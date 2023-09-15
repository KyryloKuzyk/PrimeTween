using UnityEngine;

namespace Tweens.Core {
  public abstract class Tween {
    /// <summary>The duration of the Tween in seconds defines how long the Tween will take to complete. When the duration is not set, the Tween will complete instantly.</summary>
    public float duration;
    /// <summary>The delay of the Tween in seconds defines how long the Tween will wait before starting. To change the behaviour of how to the delay will affect the Tween before it starts, you can change the Fill Mode. When the delay is not set, the Tween will start instantly.</summary>
    public float? delay;
    /// <summary>The ping pong interval defines how long the Tween will wait before playing the animation backwards after the animation has finished. When the ping pong interval is not set, the Tween will play the animation backwards instantly after the animation has finished.</summary>
    public float? pingPongInterval;
    /// <summary>The repeat interval defines how long the Tween will wait before repeating itself. When the repeat interval is not set, the Tween will repeat itself instantly after the animation has finished.</summary>
    public float? repeatInterval;
    /// <summary>The use unscaled time option defines whether the Tween will use the unscaled time. When the use unscaled time option is not set, the Tween will use the scaled time.</summary>
    public bool useUnscaledTime;
    /// <summary>The ping pong option defines whether the Tween will play the animation backwards after the animation has finished. When the ping pong option is not set, the Tween will not play the animation backwards after the animation has finished.</summary>
    public bool usePingPong;
    /// <summary>The infinite option defines whether the Tween will loop forever. When the Tween is set to loop forever, the Loops option will be ignored. When the infinite option is not set, the Tween will not loop forever.</summary>
    public bool isInfinite;
    /// <summary>The amount of times the Tween will loop defines how many times the Tween will repeat itself. When the Tween is using a Ping Pong loop type, the Tween has to play both the forward and backward animation to count as one loop. When Infinite is set, the Tween will loop forever and the loop count will be ignored. When the amount of loops is not set, the Tween will not loop.</summary>
    public int? loops;
    /// <summary>The offset defines on which time the Tween will start. When the offset is not set, the Tween will start at the beginning.</summary>
    public float? offset;
    /// <summary>The ease type defines how the Tween will animate. If an Animation Curve is set, the Ease Type won't be used. When the ease type is not set, the Tween will animate linearly.</summary>
    public EaseType easeType;
    /// <summary>The fill mode defines how the Tween will behave before the Tween has started and after the Tween has ended. When the fill mode is not set, the fill mode will be set to Backward.</summary>
    public FillMode fillMode = FillMode.Backwards;
    /// <summary>The animation curve defines how the Tween will animate. The animation curve can be used to create custom ease types. When the animation curve is not set, the Tween will animate according to the Ease Type.</summary>
    public AnimationCurve animationCurve;
  }

  public abstract class Tween<ComponentType, DataType> : Tween where ComponentType : Component {
    /// <summary>The from value defines the starting value of the Tween. When the from value is not set, the Tween will use the current value of the property.</summary>
    public Nullable<DataType> from;
    /// <summary>The to value defines the end value of the Tween. When the to value is not set, the Tween will use the current value of the property.</summary>
    public Nullable<DataType> to;
    /// <summary>The on add delegate will be invoked when the Tween has been added to a GameObject.</summary>
    public OnAddDelegate<ComponentType, DataType> onAdd;
    /// <summary>The on start delegate will be invoked when the Tween has started.</summary>
    public OnStartDelegate<ComponentType, DataType> onStart;
    /// <summary>The on update delegate will be invoked when the Tween has updated.</summary>
    public OnUpdateDelegate<ComponentType, DataType> onUpdate;
    /// <summary>The on end delegate will be invoked when the Tween has ended.</summary>
    public OnEndDelegate<ComponentType, DataType> onEnd;
    /// <summary>The on cancel delegate will be invoked when the Tween has been cancelled.</summary>
    public OnCancelDelegate<ComponentType, DataType> onCancel;
    /// <summary>The on finally delegate will be invoked when the Tween has ended or has been cancelled.</summary>
    public OnFinallyDelegate<ComponentType, DataType> onFinally;

    internal abstract DataType Current(ComponentType component);
    internal abstract DataType Lerp(DataType from, DataType to, float time);
    internal abstract void Apply(ComponentType component, DataType value);
  }
}