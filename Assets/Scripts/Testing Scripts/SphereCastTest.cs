using UnityEngine;
using System.Collections;

public class SphereCastTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Q)) {
			RaycastHit[] hits = Physics.SphereCastAll(transform.position, 5f, transform.forward, 0.001f);
			Debug.Log ("Testing Spherecast.");
			if(hits != null && hits.Length != 0) {
				foreach (RaycastHit hit in hits) {
					Debug.Log("Hit " + hit.transform.root.name);
				}
			}
		}
	}
}
