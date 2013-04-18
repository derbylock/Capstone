using UnityEngine;
using System.Collections;

public class NowYourePissingMeOff : MonoBehaviour {
  public Transform target;
  public GameObject marker;
  //private Transform;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    if (!target) {
      target = GameObject.Find("Follow Camera").transform;
    } else {
      if (Input.GetKeyDown(KeyCode.F)) {
        RaycastHit hit;
        if (Physics.SphereCast(target.position, 0.5f, target.TransformDirection(Vector3.forward), out hit)) {
          Instantiate(marker, hit.point, Quaternion.identity);
        }
      }
    }
	}
}
