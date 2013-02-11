using UnityEngine;
using System.Collections;

public class MousePosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI () {
		Debug.Log (Event.current.mousePosition);
	}
}
