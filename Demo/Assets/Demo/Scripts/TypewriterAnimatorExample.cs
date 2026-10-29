#if PRIME_TWEEN_INSTALLED
#if TEXT_MESH_PRO_INSTALLED
using TMPro;
#endif
using JetBrains.Annotations;
using PrimeTween;
using UnityEngine;

namespace PrimeTweenDemo {
    [PublicAPI]
    public class TypewriterAnimatorExample : MonoBehaviour {
        enum AnimationType { Simple, WithPunctuations, ByWords }
        [SerializeField] AnimationType animationType = AnimationType.WithPunctuations;
        [SerializeField] float charsPerSecond = 40f;
        [SerializeField] int pauseAfterPunctuation = 20;
        #if TEXT_MESH_PRO_INSTALLED
        TextMeshProUGUI text;

        void Awake() {
            text = gameObject.AddComponent<TextMeshProUGUI>();
            text.maxVisibleCharacters = 0;
            text.alignment = TextAlignmentOptions.TopLeft;
            text.fontSize = 12;
            text.color = Color.black * 0.8f;
            text.text = "This text is <color=orange>animated</color> with <b>zero allocations</b>, see <i>'TypewriterAnimatorExample'</i> script for more details.\n\n" +
                        "PrimeTween rocks!";
        }

        public Tween Animate() {
            if (!Application.isPlaying) {
                // 'text' is created in Awake(), so this animation can't be played in Edit mode
                PrimeTweenConfig.warnZeroDuration = false;
                var emptyTween = Tween.Delay(0f);
                PrimeTweenConfig.warnZeroDuration = true;
                return emptyTween;
            }
            switch (animationType) {
                case AnimationType.Simple:
                    return TypewriterAnimationSimple();
                case AnimationType.WithPunctuations:
                    return TypewriterAnimationWithPunctuations();
                case AnimationType.ByWords:
                    return TypewriterAnimationByWords();
                default:
                    throw new System.Exception();
            }
        }

        /// <summary>A simple typewriter animation that uses built-in <see cref="Tween.TextMaxVisibleCharacters()"/> animation method.</summary>
        public Tween TypewriterAnimationSimple() {
            text.ForceMeshUpdate();
            int characterCount = text.textInfo.characterCount;
            float duration = characterCount / charsPerSecond;
            return Tween.TextMaxVisibleCharacters(text, 0, characterCount, duration, Ease.Linear);
        }

        #region TypewriterAnimationWithPunctuations
        /// <summary>Typewriter animation which inserts pauses after punctuation marks.</summary>
        public Tween TypewriterAnimationWithPunctuations() {
            text.ForceMeshUpdate();
            RemapWithPunctuations(text, int.MaxValue, out int remappedCount, out _);
            float duration = remappedCount / charsPerSecond;
            return Tween.Custom(this, 0f, remappedCount, duration, (t, x) => t.UpdateMaxVisibleCharsWithPunctuation(x), Ease.Linear);
        }

        void UpdateMaxVisibleCharsWithPunctuation(float progress) {
            int remappedEndIndex = Mathf.RoundToInt(progress);
            RemapWithPunctuations(text, remappedEndIndex, out _, out int visibleCharsCount);
            if (text.maxVisibleCharacters != visibleCharsCount) {
                text.maxVisibleCharacters = visibleCharsCount;
                // play keyboard typing sound here if needed
            }
        }

        void RemapWithPunctuations([NotNull] TMP_Text text, int remappedEndIndex, out int remappedCount, out int visibleCharsCount) {
            remappedCount = 0;
            visibleCharsCount = 0;
            int count = text.textInfo.characterCount;
            var characterInfos = text.textInfo.characterInfo;
            for (int i = 0; i < count; i++) {
                if (remappedCount >= remappedEndIndex) {
                    break;
                }
                remappedCount++;
                visibleCharsCount++;
                if (IsPunctuationChar(characterInfos[i].character)) {
                    int nextIndex = i + 1;
                    if (nextIndex != count && !IsPunctuationChar(characterInfos[nextIndex].character)) {
                        // add pause after the last subsequent punctuation character
                        remappedCount += Mathf.Max(0, pauseAfterPunctuation);
                    }
                }
            }

            bool IsPunctuationChar(char c) {
                return ".,:;!?".IndexOf(c) != -1;
            }
        }
        #endregion

        #region TypewriterAnimationByWords
        /// <summary>Typewriter animation that shows text word by word.</summary>
        public Tween TypewriterAnimationByWords() {
            text.ForceMeshUpdate();
            RemapWords(text, int.MaxValue, out int numWords, out _);
            float duration = text.textInfo.characterCount / charsPerSecond;
            return Tween.Custom(this, 0f, numWords, duration, (t, x) => t.UpdateVisibleWords(x), Ease.Linear);
        }

        void UpdateVisibleWords(float progress) {
            int curWordIndex = Mathf.RoundToInt(progress);
            RemapWords(text, curWordIndex, out _, out int visibleCharsCount);
            if (text.maxVisibleCharacters != visibleCharsCount) {
                text.maxVisibleCharacters = visibleCharsCount;
                // play keyboard typing sound here if needed
            }
        }

        static void RemapWords([NotNull] TMP_Text text, int remappedEndIndex, out int remappedCount, out int visibleCharsCount) {
            visibleCharsCount = 0;
            int count = text.textInfo.characterCount;
            if (count == 0) {
                remappedCount = 0;
                return;
            }
            remappedCount = 1;
            var characterInfos = text.textInfo.characterInfo;
            for (int i = 0; i < count; i++) {
                if (remappedCount >= remappedEndIndex) {
                    return;
                }
                remappedCount++;
                if (IsWordSeparatorChar(characterInfos[i].character)) {
                    int nextIndex = i + 1;
                    if (nextIndex == count || !IsWordSeparatorChar(characterInfos[nextIndex].character)) {
                        remappedCount++;
                        visibleCharsCount = nextIndex;
                    }
                }
            }
            visibleCharsCount = count;

            bool IsWordSeparatorChar(char ch) {
                return " \n".IndexOf(ch) != -1;
            }
        }
        #endregion
        #else
        void Awake() {
            Debug.LogWarning("Please install TextMeshPro 'com.unity.textmeshpro' to enable TypewriterAnimatorExample.", this);
        }
        #endif
    }
}
#endif
