using UnityEngine;
#if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;
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
            if (isNewInputSystemEnabled) {
                var touches = activeTouches;
                if (touches.Count > 0) {
                    return touches[0].phase == TouchPhase.Began;
                }
                return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
            }
            #endif
            return Input.GetMouseButtonDown(0);
        }

        /// On touchscreens, a second finger acts as a right click at the first finger's position.
        public static bool GetRightDown() {
            #if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
            if (isNewInputSystemEnabled) {
                var touches = activeTouches;
                if (touches.Count > 1) {
                    for (int i = 0; i < touches.Count; i++) {
                        if (touches[i].phase == TouchPhase.Began) {
                            return true;
                        }
                    }
                    return false;
                }
                return Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
            }
            #endif
            if (Input.touchCount > 1) {
                for (int i = 0; i < Input.touchCount; i++) {
                    if (Input.GetTouch(i).phase == UnityEngine.TouchPhase.Began) {
                        return true;
                    }
                }
                return false;
            }
            return Input.GetMouseButtonDown(1);
        }

        #if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
        // enabled lazily, not on startup: this demo code can be compiled into a user's game, which shouldn't get EnhancedTouch enabled globally
        static ReadOnlyArray<Touch> activeTouches {
            get {
                if (!EnhancedTouchSupport.enabled) {
                    EnhancedTouchSupport.Enable();
                }
                return Touch.activeTouches;
            }
        }
        #endif

        public static bool Get() {
            #if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
            if (isNewInputSystemEnabled) {
                var touches = activeTouches;
                if (touches.Count > 0) {
                    return touches[0].isInProgress;
                }
                return Mouse.current != null && Mouse.current.leftButton.isPressed;
            }
            #endif
            return Input.GetMouseButton(0);
        }

        public static bool GetUp() {
            #if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
            if (isNewInputSystemEnabled) {
                var touches = activeTouches;
                if (touches.Count > 0) {
                    return touches[0].phase == TouchPhase.Ended;
                }
                return Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;
            }
            #endif
            return Input.GetMouseButtonUp(0);
        }

        public static Vector2 screenPosition {
            get {
                #if INPUT_SYSTEM_INSTALLED && ENABLE_INPUT_SYSTEM
                if (isNewInputSystemEnabled) {
                    var touches = activeTouches;
                    if (touches.Count > 0) {
                        return touches[0].screenPosition;
                    }
                    return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
                }
                #endif
                return Input.mousePosition;
            }
        }
    }
}
