using UnityEngine;
#if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.UI;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif

namespace PrimeTweenDemo {
    public class InputController : MonoBehaviour {
        void Awake() {
            if (isNewInputSystemEnabled && !isLegacyInputManagerEnabled) {
                gameObject.SetActive(false);
                #if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM && UNITY_UGUI_INSTALLED
                var inputModule = gameObject.AddComponent<InputSystemUIInputModule>();
                inputModule.pointerBehavior = UIPointerBehavior.AllPointersAsIs;
                EnhancedTouchSupport.Enable();
                #endif
                gameObject.SetActive(true);
            } else {
                #if UNITY_UGUI_INSTALLED
                gameObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                #endif
            }
        }

        static bool isNewInputSystemEnabled {
            get {
                #if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
                return true;
                #else
                return false;
                #endif
            }
        }

        static bool isLegacyInputManagerEnabled {
            get {
                #if ENABLE_LEGACY_INPUT_MANAGER
                return true;
                #else
                return false;
                #endif
            }
        }

        public static bool touchSupported {
            get {
                #if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
                if (isNewInputSystemEnabled) {
                    return Touchscreen.current != null;
                }
                #endif
                return Input.touchSupported;
            }
        }

        public static bool GetDown() {
            #if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
            if (Mouse.current != null) {
                return Mouse.current.leftButton.wasPressedThisFrame;
            }
            if (isNewInputSystemEnabled) {
                return Touch.activeTouches.Count > 0 && Touch.activeTouches[0].phase == TouchPhase.Began;
            }
            #endif
            return Input.GetMouseButtonDown(0);
        }

        public static bool Get() {
            #if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
            if (isNewInputSystemEnabled) {
                if (Mouse.current != null) {
                    return Mouse.current.leftButton.isPressed;
                }
                if (Touch.activeTouches.Count == 0) {
                    return false;
                }
                var phase = Touch.activeTouches[0].phase;
                return phase == TouchPhase.Stationary || phase == TouchPhase.Moved;
            }
            #endif
            return Input.GetMouseButtonDown(0);
        }

        public static bool GetUp() {
            #if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
            if (isNewInputSystemEnabled) {
                if (Mouse.current != null) {
                    return Mouse.current.leftButton.wasReleasedThisFrame;
                }
                return Touch.activeTouches.Count > 0 && Touch.activeTouches[0].phase == TouchPhase.Ended;
            }
            #endif
            return Input.GetMouseButtonUp(0);
        }

        public static Vector2 screenPosition {
            get {
                #if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
                if (isNewInputSystemEnabled) {
                    if (Mouse.current != null) {
                        return Mouse.current.position.ReadValue();
                    }
                    var activeTouches = Touch.activeTouches;
                    return activeTouches.Count > 0 ? activeTouches[0].screenPosition : Vector2.zero;
                }
                #endif
                return Input.mousePosition;
            }
        }
    }
}
