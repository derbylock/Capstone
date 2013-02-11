using UnityEngine;
using System.Collections;

public class DrawForward : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawLine(transform.position, transform.position + transform.TransformDirection(Vector3.forward)*1.5f);
	}
}
