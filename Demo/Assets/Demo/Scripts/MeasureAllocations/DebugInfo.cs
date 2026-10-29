#if PRIME_TWEEN_INSTALLED && UNITY_UGUI_INSTALLED
#if UNITY_EDITOR && UNITY_2019_1_OR_NEWER
using UnityEngine.Profiling;
#endif
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace PrimeTweenDemo {
    /// <summary>
    /// PrimeTween uses static delegates (lambdas with no external captures) to play animations.
    /// The first time a particular animation is played, C# runtime caches the delegate for this animation, and the GC.Alloc is shown in Profiler.
    /// Such allocations are not technically 'garbage' because they are not subject to garbage collection.
    /// All subsequent calls will use the cached delegate and will never allocate again.
    ///
    /// To replicate '0B' heap allocations shown in the promo video:
    ///     Disable the 'Project Settings/Editor/Enter Play Mode Settings/Reload Domain' setting.
    ///     Enable Profiler with Deep Profile.
    ///     Run the Demo and play all animations at least once. This will cache the aforementioned static delegates.
    ///     Restart the Demo scene and observe that PrimeTween doesn't allocate heap memory after static delegates warm-up.
    /// </summary>
    public class DebugInfo : MonoBehaviour {
        #pragma warning disable 0414
        [SerializeField] MeasureMemoryAllocations measureMemoryAllocations;
        [SerializeField] Text tweensCountText;
        [SerializeField] Text gcAllocText;
        #pragma warning restore 0414
        #if UNITY_EDITOR && UNITY_2019_1_OR_NEWER
        long curTweensCount = -1;
        int? curGCAlloc;

        void Start() {
            gcAllocText.text = string.Empty;
            if (shouldDisable()) {
                gameObject.SetActive(false);
            }
            if (Profiler.enabled && !UnityEditorInternal.ProfilerDriver.deepProfiling) {
                Debug.LogWarning("Please enable 'Deep Profile' for more accurate memory allocation measurements.");
            }
        }

        static bool shouldDisable() {
            if (!Application.isEditor) {
                return true;
            }
            if (UnityEditor.EditorApplication.isPaused) {
                return false; // Profiler.enabled returns false if scene is started paused in Unity 2021.3.26
            }
            return !Profiler.enabled;
        }

        void Update() {
            var newTweensCount = PrimeTweenManager.Instance.lastId;
            if (curTweensCount != newTweensCount) {
                curTweensCount = newTweensCount;
                tweensCountText.text = $"Animations: {newTweensCount}";
            }
            var newGCAlloc = measureMemoryAllocations.gcAllocTotal;
            if (newGCAlloc.HasValue && curGCAlloc != newGCAlloc.Value) {
                curGCAlloc = newGCAlloc.Value;
                gcAllocText.text = $"Heap allocations: {UnityEditor.EditorUtility.FormatBytes(newGCAlloc.Value)}";
            }
        }
        #endif
    }
}
#endif
