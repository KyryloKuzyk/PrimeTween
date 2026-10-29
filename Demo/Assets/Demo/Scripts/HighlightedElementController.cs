#if PRIME_TWEEN_INSTALLED
using JetBrains.Annotations;
using PrimeTween;
using UnityEngine;

namespace PrimeTweenDemo {
    public class HighlightedElementController : MonoBehaviour {
        [SerializeField] Camera mainCamera;
        [SerializeField] CameraProjectionMatrixAnimation cameraProjectionMatrixAnimation;
        [CanBeNull] public HighlightableElement current { get; private set; }

        #if UNITY_2019_1_OR_NEWER && !PHYSICS_MODULE_INSTALLED
        void Awake() {
            Debug.LogError("Please install the package needed for Physics.Raycast(): 'Package Manager/Packages/Built-in/Physics' (com.unity.modules.physics).");
        }
        #endif

        void Update() {
            if (cameraProjectionMatrixAnimation.IsAnimating) {
                return;
            }
            if (Application.isMobilePlatform && InputController.touchSupported && !InputController.Get()) {
                SetCurrentHighlighted(null);
                return;
            }
            var screenPosition = InputController.screenPosition;
            if (!new Rect(0f, 0f, Screen.width, Screen.height).Contains(screenPosition)) {
                return;
            }
            var ray = mainCamera.ScreenPointToRay(screenPosition);
            var highlightableElement = RaycastHighlightableElement(ray);
            SetCurrentHighlighted(highlightableElement);

            if (current != null && InputController.GetDown()) {
                current.GetComponent<Animatable>().OnClick();
            }
        }

        [CanBeNull]
        static HighlightableElement RaycastHighlightableElement(Ray ray) {
            #if !UNITY_2019_1_OR_NEWER || PHYSICS_MODULE_INSTALLED
                // If you're seeing a compilation error on the next line, please install the package needed for Physics.Raycast(): 'Package Manager/Packages/Built-in/Physics' (com.unity.modules.physics).
                return Physics.Raycast(ray, out var hit) ? hit.collider.GetComponentInParent<HighlightableElement>() : null;
            #else
                return null;
            #endif
        }

        void SetCurrentHighlighted([CanBeNull] HighlightableElement newHighlighted) {
            if (newHighlighted != current) {
                if (current != null) {
                    AnimateHighlightedElement(current, false);
                }
                current = newHighlighted;
                if (newHighlighted != null) {
                    AnimateHighlightedElement(newHighlighted, true);
                }
            }
        }

        static readonly int emissionColorPropId = Shader.PropertyToID("_EmissionColor");

        static void AnimateHighlightedElement([NotNull] HighlightableElement highlightable, bool isHighlighted) {
            Tween.LocalPositionZ(highlightable.highlightAnchor, isHighlighted ? 0.08f : 0, 0.3f);
            foreach (var model in highlightable.models) {
                Tween.MaterialColor(model.sharedMaterial, emissionColorPropId, isHighlighted ? Color.white * 0.25f : Color.black, 0.2f, Ease.OutQuad);
            }
        }
    }
}
#endif
