using UnityEngine;
using System.Collections;

public class DevTools: MonoBehaviour {
  public bool sensitivityWindow;
  CameraController camControl;
	
	// Update is called once per frame
  void Update () {
    if (Input.GetKeyDown(KeyCode.U)) {
      Screen.showCursor = !Screen.showCursor;
      Screen.lockCursor = !Screen.lockCursor;
    }
    if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) {
      Network.Disconnect();
      MasterServer.UnregisterHost();
      Application.LoadLevel(0);
    }

    if (Input.GetKeyDown(KeyCode.T)) {
      sensitivityWindow = !sensitivityWindow;
    }
  }

  void OnGUI() {
    if (sensitivityWindow) {
      Debug.Log("Drawing Window");
      GUI.Window(Constants.WINID_DEV_MOUSE_SENS,
                  new Rect(0, 0, 400, 60),
                  MouseSensitivity,
                  "Mouse Sensitivity");
    }
  }

  void MouseSensitivity(int winID) {
    if (!camControl) {
      camControl = GameObject.FindObjectOfType(typeof(CameraController)) as CameraController;
    }
    GUILayout.BeginHorizontal();
    GUILayout.Label("Sensitivity");
    camControl.sensitivity = GUILayout.HorizontalSlider(camControl.sensitivity, 0.01f, 1f);
    GUILayout.EndHorizontal();
  }
}