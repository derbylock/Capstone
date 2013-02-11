using UnityEngine;
using System.Collections;

public class SlowMo : MonoBehaviour {
	public float maxSlowTime = 3f;
	private bool isWeaponChanging;
	
	void Update() {
	  if(!isWeaponChanging && Input.GetKeyDown(KeyCode.F)) { 
	    isWeaponChanging = true;
	    StartCoroutine("SlowTime");
	  }
	
	  if(Input.GetKey(KeyCode.F)) { isWeaponChanging = true; }
	  else { isWeaponChanging = false; }
	}
	
	
	
	
	IEnumerator SlowTime() {
	  float startTime = Time.realtimeSinceStartup;
	  Time.timeScale = 0.5f;
	
	  while(Time.realtimeSinceStartup-startTime < maxSlowTime) {
	    // Break prematurely if the player is done before their time is up
	    if(!isWeaponChanging) { break; }
	    yield return null;
	  }
	
	  Time.timeScale = 1f;
	}
}
