#if PRIME_TWEEN_INSTALLED
using PrimeTween;
using UnityEngine;

namespace PrimeTweenDemo {
    public abstract class Clickable : MonoBehaviour {
        public virtual void OnClick() {}
    }

    public abstract class Animatable : Clickable {
        public abstract Sequence Animate(bool toEndValue);
    }
}
#endif
