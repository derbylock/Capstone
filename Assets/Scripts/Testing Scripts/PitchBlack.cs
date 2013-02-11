using UnityEngine;
using System.Collections;

public class PitchBlack : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.R)) {
			/*foreach(Camera cam in Camera.allCameras) {
				cam.camera.enabled = !cam.camera.enabled;
			}*/
			for(int i=0; i<Camera.allCameras.Length; i++) {
				Destroy (Camera.allCameras[i].gameObject);
			}
		}
	}
}
