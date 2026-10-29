#if PRIME_TWEEN_INSTALLED && UNITY_UGUI_INSTALLED
using PrimeTween;
using UnityEngine;

namespace PrimeTweenDemo {
    public class Wheels : Animatable {
        [SerializeField] Demo demo;
        [SerializeField] Transform[] wheels;
        bool isAnimating;
        Sequence sequence;

        public override void OnClick() {
            demo.AnimateAll(!isAnimating);
        }

        public override Sequence Animate(bool _isAnimating) {
            isAnimating = _isAnimating;
            // Spinning wheels is an infinite animation, and we should not return it as result of this method.
            // Instead, we should wrap the SpinWheelsInfinitely() call inside the empty Sequence. This way, the SpinWheelsInfinitely() call can be grouped and chained with other tweens and sequences.
            return Sequence.Create().ChainCallback(this, target => target.SpinWheelsInfinitely());
        }

        void SpinWheelsInfinitely() {
            if (isAnimating) {
                sequence.Complete();
                sequence = Sequence.Create(-1);
                foreach (var wheel in wheels) {
                    sequence.Group(Tween.LocalEulerAngles(wheel, Vector3.zero, new Vector3(360, 0), 1, Ease.Linear));
                }
            } else {
                if (sequence.isAlive) {
                    sequence.SetRemainingCycles(0);
                }
            }
        }
    }
}
#endif
