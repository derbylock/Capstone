using UnityEngine;
using System.Collections;

public class CoroutineTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
    StartCoroutine("Testing");
    Destroy(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  IEnumerator Testing() {
    while (true) {
      Debug.Log("Still running");
      yield return new WaitForSeconds(1);
    }
  }
}
