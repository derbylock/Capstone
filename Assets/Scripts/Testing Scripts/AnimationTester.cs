using UnityEngine;
using System.Collections;

public class AnimationTester : MonoBehaviour {
	NetworkAnimation animator;

	// Use this for initialization
	void Start () {
		animator = transform.GetComponent<NetworkAnimation>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Keypad1)) {
			animator.StartAnimation("idle");
		} else if(Input.GetKeyDown(KeyCode.Keypad2)) {
			animator.StartAnimation("run");
		} else if(Input.GetKeyDown(KeyCode.Keypad3)) {
			animator.StartAnimation("attack");
		} else if(Input.GetKeyDown(KeyCode.Keypad4)) {
			animator.StartAnimation("die");
		}
		
	}
}
