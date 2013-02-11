using UnityEngine;
using System.Collections;

public class NetworkObject : MonoBehaviour {

	// Use this for initialization
	/*void Start() {
		if (networkView.isMine) {
			NetworkInterpolatedTransform netTransform = gameObject.GetComponent(typeof(NetworkInterpolatedTransform)) as NetworkInterpolatedTransform;
			netTransform.enabled = false;
		} else {
			name += "Remote";
			//GetComponent(ThirdPersonController).enabled = false;
			//GetComponent(ThirdPersonSimpleAnimation).enabled = false;
			NetworkInterpolatedTransform netTransform = gameObject.GetComponent(typeof(NetworkInterpolatedTransform)) as NetworkInterpolatedTransform;
			netTransform.enabled = true;
		}
	}*/
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnNetworkInstantiate (NetworkMessageInfo msg) {
		if (networkView.isMine) {
			NetworkInterpolatedTransform netTransform = gameObject.GetComponent(typeof(NetworkInterpolatedTransform)) as NetworkInterpolatedTransform;
			netTransform.enabled = false;
		} else {
			name += "Remote";
			//GetComponent(ThirdPersonController).enabled = false;
			//GetComponent(ThirdPersonSimpleAnimation).enabled = false;
			NetworkInterpolatedTransform netTransform = gameObject.GetComponent(typeof(NetworkInterpolatedTransform)) as NetworkInterpolatedTransform;
			netTransform.enabled = true;
		}
	}
}