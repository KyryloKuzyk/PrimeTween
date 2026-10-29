#if PRIME_TWEEN_INSTALLED && UNITY_UGUI_INSTALLED
using PrimeTween;
using UnityEngine;

namespace PrimeTweenDemo {
    public class SlidingDoor : Animatable {
        [SerializeField] Demo demo;
        [SerializeField] Transform animationAnchor;
        [SerializeField] Vector3 openedPos, midPos, closedPos;
        bool isClosed;
        Sequence sequence;

        public override void OnClick() {
            if (!demo.animateAllSequence.isAlive) {
                Animate(!isClosed);
            }
        }

        public override Sequence Animate(bool _isClosed) {
            if (isClosed == _isClosed) {
                return Sequence.Create();
            }
            isClosed = _isClosed;
            if (sequence.isAlive) {
                sequence.Stop();
            }
            var tweenSettings = new TweenSettings(0.4f, Ease.OutBack, endDelay: 0.1f);
            sequence = Tween.LocalPosition(animationAnchor, new TweenSettings<Vector3>(midPos, tweenSettings))
                .Chain(Tween.LocalPosition(animationAnchor, new TweenSettings<Vector3>(_isClosed ? closedPos : openedPos, tweenSettings)));
            return sequence;
        }
    }
}
#endif
