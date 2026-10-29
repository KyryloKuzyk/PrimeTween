#if PRIME_TWEEN_INSTALLED
using PrimeTween;
using UnityEngine;

namespace PrimeTweenDemo {
    public class Road : Animatable {
        [SerializeField] MeshRenderer roadModel;
        [SerializeField] AnimationCurve ease;
        float currentSpeed;

        void Awake() {
            roadModel.sharedMaterial = new Material(roadModel.sharedMaterial); // copy shared material
        }

        public override Sequence Animate(bool isAnimating) {
            var currentSpeedTween = Tween.Custom(this, currentSpeed, isAnimating ? 0.3f : 0, 1, (_this, val) => _this.currentSpeed = val);
            var sequence = Sequence.Create(currentSpeedTween);
            if (isAnimating) {
                sequence.Group(Tween.LocalPositionY(transform, 0, -0.5f, 0.7f, ease));
            }
            return sequence;
        }

        void Update() {
            roadModel.sharedMaterial.mainTextureOffset += new Vector2(-1f, 1f) * currentSpeed * Time.deltaTime;
        }
    }
}
#endif