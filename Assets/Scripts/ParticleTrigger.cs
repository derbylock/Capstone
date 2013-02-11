using UnityEngine;
using System.Collections;

public class ParticleTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		particleSystem.Stop();
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			particleSystem.Play();
		}
	}
	
	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			particleSystem.Stop ();
		}
	}
}
