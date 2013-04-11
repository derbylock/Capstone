using UnityEngine;
using System.Collections;

public class DevTools: MonoBehaviour {
	
	// Update is called once per frame
  void Update () {
    if (Input.GetKeyDown(KeyCode.U)) {
      Screen.showCursor = !Screen.showCursor;
      Screen.lockCursor = !Screen.lockCursor;
    }
    if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) {
      Application.LoadLevel(0);
    }
  }
}
