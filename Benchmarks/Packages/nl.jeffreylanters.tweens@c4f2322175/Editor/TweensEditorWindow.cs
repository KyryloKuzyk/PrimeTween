#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Tweens.Core;

namespace Tweens.Editor {
  internal class TweensEditorWindow : EditorWindow, IHasCustomMenu {
    const string baseKey = "Tweens.Editor.TweensEditorWindow";
    const string showDirectionKey = baseKey + ".showDirection";
    const string showIsPausedKey = baseKey + ".showIsPaused";
    const string showPingPongIntervalKey = baseKey + ".showPingPongInterval";
    const string showRepeatIntervalKey = baseKey + ".showRepeatInterval";
    const string showLoopsKey = baseKey + ".showLoops";
    const string autoRepaintKey = baseKey + ".autoRepaint";
    Vector2 scrollPosition;
    string searchQuery;
    bool showProgress;
    bool showDirection;
    bool showIsPaused;
    bool showPingPongInterval;
    bool showRepeatInterval;
    bool showLoops;
    bool autoRepaint = true;

    [MenuItem("Window/Analysis/Tweens", false, 1000)]
    internal static void ShowWindow() {
      var window = GetWindow<TweensEditorWindow>("Tweens");
      window.titleContent = new GUIContent("Tweens", EditorGUIUtility.IconContent("d_PlayButton").image);
      window.showDirection = EditorPrefs.GetBool(showDirectionKey, false);
      window.showIsPaused = EditorPrefs.GetBool(showIsPausedKey, false);
      window.showPingPongInterval = EditorPrefs.GetBool(showPingPongIntervalKey, false);
      window.showRepeatInterval = EditorPrefs.GetBool(showRepeatIntervalKey, false);
      window.showLoops = EditorPrefs.GetBool(showLoopsKey, false);
      window.autoRepaint = EditorPrefs.GetBool(autoRepaintKey, true);
    }

    void IHasCustomMenu.AddItemsToMenu(GenericMenu menu) {
      menu.AddItem(new GUIContent("Auto repaint"), autoRepaint, () => EditorPrefs.SetBool(autoRepaintKey, autoRepaint = !autoRepaint));
      menu.AddItem(new GUIContent("Documentation"), false, () => Application.OpenURL("https://github.com/jeffreylanters/unity-tweens"));
    }

    void OnGUI() {
      var wasEnabled = GUI.enabled;
      GUI.enabled = Application.isPlaying;
      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      if (GUILayout.Button(new GUIContent("Cancel all", "Cancels all of the instantiated tweens."), EditorStyles.toolbarButton)) {
        TweenEngine.instances.ForEach(tweenInstance => tweenInstance.Cancel());
      }
      if (GUILayout.Button(new GUIContent("Pause all", "Pauses all of the instantiated tweens."), EditorStyles.toolbarButton)) {
        TweenEngine.instances.ForEach(tweenInstance => tweenInstance.isPaused = true);
      }
      if (GUILayout.Button(new GUIContent("Unpause all", "Unpauses all of the instantiated tweens."), EditorStyles.toolbarButton)) {
        TweenEngine.instances.ForEach(tweenInstance => tweenInstance.isPaused = false);
      }
      GUILayout.FlexibleSpace();
      GUI.enabled = true;
      searchQuery = EditorGUILayout.TextField(searchQuery, EditorStyles.toolbarSearchField);
      if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_SceneViewVisibility@2x").image, "Toggle the visibility of various columns."), EditorStyles.toolbarDropDown)) {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Show Progress"), showProgress, () => EditorPrefs.SetBool(baseKey + ".showProgress", showProgress = !showProgress));
        menu.AddItem(new GUIContent("Show Direction"), showDirection, () => EditorPrefs.SetBool(showDirectionKey, showDirection = !showDirection));
        menu.AddItem(new GUIContent("Show Paused"), showIsPaused, () => EditorPrefs.SetBool(showIsPausedKey, showIsPaused = !showIsPaused));
        menu.AddItem(new GUIContent("Show Ping Pong Interval"), showPingPongInterval, () => EditorPrefs.SetBool(showPingPongIntervalKey, showPingPongInterval = !showPingPongInterval));
        menu.AddItem(new GUIContent("Show Repeat Interval"), showRepeatInterval, () => EditorPrefs.SetBool(showRepeatIntervalKey, showRepeatInterval = !showRepeatInterval));
        menu.AddItem(new GUIContent("Show Loops"), showLoops, () => EditorPrefs.SetBool(showLoopsKey, showLoops = !showLoops));
        menu.ShowAsContext();
      }
      GUI.enabled = Application.isPlaying;
      EditorGUILayout.EndHorizontal();
      scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
      EditorGUILayout.BeginHorizontal();
      GUILayout.Space(10);
      EditorGUILayout.LabelField(new GUIContent("Target", "The target gameObject on which the tween is attached."), EditorStyles.miniBoldLabel);
      GUILayout.FlexibleSpace();
      if (showProgress) {
        EditorGUILayout.LabelField(new GUIContent("Progress", "The progress position of the tween."), EditorStyles.miniLabel, GUILayout.Width(50));
      }
      EditorGUILayout.LabelField(new GUIContent("Time", "The current playback position."), EditorStyles.miniLabel, GUILayout.Width(50));
      EditorGUILayout.LabelField(new GUIContent("Duration", "The duration of the Tween."), EditorStyles.miniLabel, GUILayout.Width(50));
      EditorGUILayout.LabelField(new GUIContent("Halt", "A timer indicating how long the tween is halted."), EditorStyles.miniLabel, GUILayout.Width(50));
      if (showLoops) {
        EditorGUILayout.LabelField(new GUIContent("Loops", "The number of loops the tween has to go though left."), EditorStyles.miniLabel, GUILayout.Width(50));
      }
      if (showDirection) {
        EditorGUILayout.LabelField(new GUIContent("Direction", "The playback direction of the tween."), EditorStyles.miniLabel, GUILayout.Width(50));
      }
      if (showIsPaused) {
        EditorGUILayout.LabelField(new GUIContent("Paused", "Whether the tween is paused."), EditorStyles.miniLabel, GUILayout.Width(50));
      }
      if (showPingPongInterval) {
        EditorGUILayout.LabelField(new GUIContent("PingPong", "The interval between PingPong playbacks."), EditorStyles.miniLabel, GUILayout.Width(50));
      }
      if (showRepeatInterval) {
        EditorGUILayout.LabelField(new GUIContent("Repeat", "The interval between playbacks."), EditorStyles.miniLabel, GUILayout.Width(50));
      }
      GUILayout.Space(10);
      EditorGUILayout.EndHorizontal();
      if (!Application.isPlaying || TweenEngine.instances.Count == 0) {
        GUILayout.Space(20);
        GUILayout.Label("No tweens running", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.EndScrollView();
        return;
      }
      for (var i = TweenEngine.instances.Count - 1; i >= 0; i -= 1) {
        var tweenInstance = TweenEngine.instances[i];
        if (tweenInstance.isDecommissioned || tweenInstance.target == null) {
          continue;
        }
        if (!string.IsNullOrEmpty(searchQuery) && !tweenInstance.target.name.Contains(searchQuery)) {
          continue;
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button($"{tweenInstance.target.name} ({tweenInstance.@tweenTypeName})", EditorStyles.linkLabel)) {
          if (Event.current.button == 1) {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Pause"), tweenInstance.isPaused, () => tweenInstance.isPaused = !tweenInstance.isPaused);
            menu.AddItem(new GUIContent("Cancel"), false, () => tweenInstance.Cancel());
            menu.ShowAsContext();
          }
          else {
            EditorGUIUtility.PingObject(tweenInstance.target);
          }
        }
        GUILayout.FlexibleSpace();
        if (showProgress) {
          EditorGUILayout.LabelField("Progress", GUILayout.Width(50));
          EditorGUI.ProgressBar(GUILayoutUtility.GetLastRect(), tweenInstance.time / tweenInstance.duration, $"{Mathf.Round(tweenInstance.time / tweenInstance.duration * 100)}%");
        }
        EditorGUILayout.LabelField($"{tweenInstance.time:0.00}s", GUILayout.Width(50));
        EditorGUILayout.LabelField($"{tweenInstance.duration:0.00}s", GUILayout.Width(50));
        EditorGUILayout.LabelField(tweenInstance.haltTime != null ? $"{tweenInstance.haltTime:0.00}s" : "-", GUILayout.Width(50));
        if (showLoops) {
          EditorGUILayout.LabelField(tweenInstance.loops != null ? $"{tweenInstance.loops}" : "-", GUILayout.Width(50));
        }
        if (showDirection) {
          EditorGUILayout.LabelField(tweenInstance.isForwards ? "Forwards" : "Backwards", GUILayout.Width(50));
        }
        if (showIsPaused) {
          EditorGUILayout.LabelField(tweenInstance.isPaused ? "Paused" : "Playing", GUILayout.Width(50));
        }
        if (showPingPongInterval) {
          EditorGUILayout.LabelField(tweenInstance.pingPongInterval != null ? $"{tweenInstance.pingPongInterval:0.00}s" : "-", GUILayout.Width(50));
        }
        if (showRepeatInterval) {
          EditorGUILayout.LabelField(tweenInstance.repeatInterval != null ? $"{tweenInstance.repeatInterval:0.00}s" : "-", GUILayout.Width(50));
        }
        GUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
      }
      EditorGUILayout.EndScrollView();
      GUI.enabled = wasEnabled;
    }

    void Update() {
      if (!autoRepaint || !EditorApplication.isPlaying || EditorApplication.isPaused) {
        return;
      }
      Repaint();
    }
  }
}
#endif