#if PRIME_TWEEN_INSTALLED
using PrimeTween;
using UnityEngine;

namespace PrimeTweenDemo {
    public class DirectionalLightController : MonoBehaviour {
        [SerializeField] Light directionalLight;
        [SerializeField] Camera mainCamera;
        [SerializeField] Color startColor;
        [SerializeField] Color endColor;
        float angleX;
        float angleY;

        void OnEnable() {
            // This overload is simpler but allocates a small amount of garbage because 'this' reference is captured in a closure.
            // It's ok to use it once in a while, but for hot code paths consider using the overload that accepts 'target' as the first parameter.
            var xRotationSettings = new TweenSettings<float>(45, 10, 10, Ease.Linear, -1, CycleMode.Yoyo);
            Tween.Custom(xRotationSettings, newX => angleX = newX);

            // This overload is more verbose but doesn't allocate garbage.
            var yRotationSettings = new TweenSettings<float>(45, 405, 20, Ease.Linear, -1);
            Tween.Custom(this, yRotationSettings, (target, newY) => target.angleY = newY);

            var colorSettings = new TweenSettings<Color>(startColor, endColor, 10, Ease.InCirc, -1, CycleMode.Rewind);
            Tween.LightColor(directionalLight, colorSettings);
            Tween.CameraBackgroundColor(mainCamera, colorSettings);
            Tween.Custom(colorSettings, color => RenderSettings.fogColor = color);
        }

        void Update() {
            transform.localEulerAngles = new Vector3(angleX, angleY);
        }
    }
}
#endif