#if PRIME_TWEEN_INSTALLED && UNITY_UGUI_INSTALLED
using PrimeTween;
using UnityEngine;

namespace PrimeTweenDemo {
    public class Door : Animatable {
        [SerializeField] CameraController cameraController;
        [SerializeField] Transform animationAnchor;
        bool isClosed;

        public override void OnClick() {
            Animate(!isClosed);
        }

        public override Sequence Animate(bool _isClosed) {
            if (isClosed == _isClosed) {
                return Sequence.Create();
            }
            isClosed = _isClosed;
            var sequence = Sequence.Create();
            var rotationTween = Tween.LocalRotation(animationAnchor, _isClosed ? new Vector3(0, -90) : Vector3.zero, 0.7f, Ease.InOutElastic);
            sequence.Group(rotationTween);
            if (_isClosed) {
                sequence.Group(cameraController.Shake(0.5f));
            }
            return sequence;
        }
    }
}
#endif
