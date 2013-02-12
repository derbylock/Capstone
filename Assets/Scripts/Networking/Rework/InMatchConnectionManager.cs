using UnityEngine;

public class InMatchConnectionManager : MonoBehaviour {
  public string matchMakingLevel = "Matchmaking";
  public string lobbyLevel = "Lobby";
  public bool disconnected;
  public bool opponentDisconnected;

  private Rect disconnectedWindowSize = new Rect();
  private Rect opponentDisconnectedWindowSize = new Rect();

  void OnDisconnectedFromServer() {
    Time.timeScale = 0f;  // Don't forget to undo this on leaving
    disconnected = true;
  }

  void OnGUI() {
    if(disconnected) {
      disconnectedWindowSize = GUILayout.Window(1024, disconnectedWindowSize, DisconnectedWindow, "Disconnected");
    }
    if(opponentDisconnected) {
      opponentDisconnectedWindowSize = GUILayout.Window(2048, opponentDisconnectedWindowSize, OpponentDisconnectedWindow, "Opponent Disconnected");
    }
  }

  void DisconnectedWindow(int windowID) {
    float windowWidth = Screen.width/8;
    float windowHeight = Screen.width/16;
    disconnectedWindowSize = new Rect(Screen.width / 2 - windowWidth / 2, Screen.height / 2 - windowHeight / 2, windowWidth, windowHeight);

    GUILayout.BeginHorizontal();
    GUILayout.Label("You have disconnected.");
    GUILayout.EndHorizontal();

    if(GUILayout.Button("Return to Menu")) {
      Application.LoadLevel(matchMakingLevel);
    }
  }

  void OpponentDisconnectedWindow(int windowID) {
    float windowWidth = Screen.width / 6;
    float windowHeight = Screen.width / 12;
    opponentDisconnectedWindowSize = new Rect(Screen.width / 2 - windowWidth / 2, Screen.height / 2 - windowHeight / 2, windowWidth, windowHeight);

    GUILayout.BeginHorizontal();
    GUILayout.Label("You have disconnected.");
    GUILayout.EndHorizontal();

    if (GUILayout.Button("Find a New Opponent")) {
      Application.LoadLevel(lobbyLevel);
    }

    if (GUILayout.Button("Return to Menu")) {
      Application.LoadLevel(matchMakingLevel);
    }
  }
}