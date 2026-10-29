#if PRIME_TWEEN_INSTALLED
using PrimeTween;
using UnityEngine;

namespace PrimeTweenDemo {
    public class SwipeTutorial : MonoBehaviour {
        Tween tween;

        void OnEnable() {
            #if !UNITY_2019_1_OR_NEWER || UNITY_UGUI_INSTALLED
            tween = Tween.Alpha(GetComponent<UnityEngine.UI.Text>(), 1, 0, 1, Ease.InOutSine, -1, CycleMode.Yoyo);
            #else
            Debug.LogError("Please install the package and re-open the Demo scene: 'Package Manager/Packages/Unity Registry/Unity UI' (com.unity.ugui).");
            #endif
        }

        public void Hide() {
            if (tween.isAlive) {
                // Stop cycling the animation when it reaches the 'endValue' (0)
                tween.SetRemainingCycles(true);
            }
        }
    }
}
#endif
