using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
  public float force;
  public float duration;
  public float changeShakeDirection;
	
	// Update is called once per frame
	void Update () {
    if (Input.GetKeyDown(KeyCode.F)) {
      StartCoroutine("Shake");
    }
	}

  IEnumerator Shake() {
    Vector3 start = transform.localPosition;
    float startTime = Time.time;
    while (Time.time - startTime < duration) {
      Vector3 moveTowards = start + Random.insideUnitSphere * force;
      transform.localPosition = moveTowards;
      yield return new WaitForSeconds(changeShakeDirection);
    }
    transform.localPosition = start;
  }
}
