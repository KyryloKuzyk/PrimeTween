#if PRIME_TWEEN_INSTALLED
using PrimeTween;
using UnityEngine;

namespace PrimeTweenDemo {
    public class SqueezeAnimation : Clickable {
        [SerializeField] Transform target;
        Tween tween;

        public override void OnClick() => PlayAnimation();

        public void PlayAnimation() {
            if (!tween.isAlive) {
                tween = Tween.Scale(target, new Vector3(1.15f, 0.9f, 1.15f), 0.2f, Ease.OutSine, 2, CycleMode.Yoyo);
            }
        }
    }
}
#endif
