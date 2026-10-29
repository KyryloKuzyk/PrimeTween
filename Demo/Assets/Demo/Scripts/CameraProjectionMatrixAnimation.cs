#if PRIME_TWEEN_INSTALLED
using PrimeTween;
using UnityEngine;

namespace PrimeTweenDemo {
    public class CameraProjectionMatrixAnimation : Clickable {
        [SerializeField] Camera mainCamera;
        float interpolationFactor;
        bool isOrthographic;
        Tween tween;

        public override void OnClick() => AnimateCameraProjection();

        public void AnimateCameraProjection() {
            isOrthographic = !isOrthographic;
            tween.Stop();
            tween = Tween.Custom(this, interpolationFactor, isOrthographic ? 1 : 0, 0.6f, ease: Ease.InOutSine, onValueChange: (target, t) => {
                    target.InterpolateProjectionMatrix(t);
                })
                .OnComplete(this, target => {
                    target.mainCamera.orthographic = target.isOrthographic;
                    target.mainCamera.ResetProjectionMatrix();
                });
        }

        void InterpolateProjectionMatrix(float _interpolationFactor) {
            interpolationFactor = _interpolationFactor;
            uint width = (uint)Screen.width;
            uint height = (uint)Screen.height;

            #if UNITY_EDITOR && UNITY_2022_2_OR_NEWER
                if (!Application.isPlaying) {
                    UnityEditor.PlayModeWindow.GetRenderingResolution(out width, out height);
                }
            #endif

            float aspect = (float)width / height;
            float orthographicSize = mainCamera.orthographicSize;
            var perspectiveMatrix = Matrix4x4.Perspective(mainCamera.fieldOfView, aspect, mainCamera.nearClipPlane, mainCamera.farClipPlane);
            var orthoMatrix = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, mainCamera.nearClipPlane, mainCamera.farClipPlane);
            Matrix4x4 projectionMatrix = default;
            for (int i = 0; i < 16; i++) {
                projectionMatrix[i] = Mathf.Lerp(perspectiveMatrix[i], orthoMatrix[i], _interpolationFactor);
            }
            mainCamera.projectionMatrix = projectionMatrix;

            #if UNITY_EDITOR
                if (!Application.isPlaying) {
                    UnityEditor.SceneView.RepaintAll();
                }
            #endif
        }

        public bool IsAnimating => tween.isAlive;
    }
}
#endif