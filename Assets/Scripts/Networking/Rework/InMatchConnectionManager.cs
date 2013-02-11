using UnityEngine;

public class InMatchConnectionManager : MonoBehaviour {
  public string matchMakingLevel = "Matchmaking";
  public bool disconnected;

  private Rect disconnectedWindowSize = new Rect();

  void OnDisconnectedFromServer() {
    Time.timeScale = 0f;  // Don't forget to undo this on leaving
    disconnected = true;
  }

  void OnGUI() {
    if(disconnected) {
      disconnectedWindowSize = GUILayout.Window(1024, disconnectedWindowSize, DisconnectedWindow, "Disconnected");
    }
  }

  void DisconnectedWindow(int windowID) {
    float windowWidth = Screen.width/6;
    float temp = windowWidth*3/5;
    float windowHeight = Screen.width/10;
    disconnectedWindowSize = new Rect(Screen.width / 2 - windowWidth / 2, Screen.height / 2 - windowHeight / 2, windowWidth, windowHeight);

    GUILayout.BeginHorizontal();
    GUILayout.Label("You have disconnected.");
    GUILayout.EndHorizontal();

    if(GUILayout.Button("Return to Menu")) {
      Application.LoadLevel(matchMakingLevel);
    }
  }
}