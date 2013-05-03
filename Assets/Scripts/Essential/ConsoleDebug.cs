using UnityEngine;
using System.Collections.Generic;

public class ConsoleDebug : MonoBehaviour {
  public int messageQueueLength = 20;
  bool isDebugging;

  Queue<string> _messages;
  string[] messages;

  Vector2 scrollPosition = Vector2.zero;

  void Start() {
    _messages = new Queue<string>(messageQueueLength);
  }

  void Update() {
    if(Input.GetKey(KeyCode.LeftControl) &&
       Input.GetKey(KeyCode.LeftShift) &&
       Input.GetKeyDown(KeyCode.K)) {
      isDebugging = !isDebugging;
    }
  }

  public void DebugLine(string line) {
    if (_messages.Count == messageQueueLength) {
      _messages.Dequeue();
    }
    _messages.Enqueue(line);
    messages = _messages.ToArray();
  }

  void OnGUI() {
    if (isDebugging) {
      GUI.Window(Constants.WINID_DEV_DEBUG,
                 new Rect(0, Screen.height / 3 * 2, Screen.width / 2, Screen.height / 3),
                 DebugWindow,
                 "Debug");
    }
  }

  void DebugWindow(int winID) {
    GUIStyle style = new GUIStyle();
    style.wordWrap = true;
    style.normal.textColor = Color.red;

    scrollPosition = GUILayout.BeginScrollView(scrollPosition);
    foreach (string m in messages) {
      GUILayout.BeginHorizontal();
      GUILayout.Label(messages[0], style);
      GUILayout.EndHorizontal();
    }
    GUILayout.EndScrollView();
  }
}