using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RuneSpell : AreaSpell {
	public float m_triggerDelay = 0f;
	public Texture2D m_overlayTexture;
	public GameObject m_triggerFx;
	public Vector3 m_triggerEffectDirection;
	
	void Start() {
		if(!Network.isServer) {
			this.enabled = false;
		}
		
		// We will only ever have 1 child, and that is the projector
		// Set the texture of the projector to whatever texture we want
		if(m_overlayTexture) {
			transform.FindChild("Blob Light Projector").renderer.material.mainTexture = m_overlayTexture;
		}
	}
	
	// Test the directionality of the particle effect
	void Update() {
		
		/*if(Input.GetKeyDown(KeyCode.Alpha1)) {
			Instantiate(m_triggerFx, transform.position, Quaternion.FromToRotation(m_triggerEffectDirection, transform.up));
		}*/
	}
	
	public override void Cast (Vector3 castFrom, Vector3 castTo) {
		RaycastHit hit;
		
		Debug.Log ("Casting Rune Spell");
		Debug.Log (castFrom + " : " + castTo);
		if(Physics.Raycast(castFrom, Vector3.Normalize(castTo-castFrom), out hit, m_maxDistance)) {
			transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			transform.position = hit.point; // + transform.TransformDirection(Vector3.up) * 0.1f;
		}
		
		m_isInPlace = true;
	}
	
	protected override IEnumerator WaitDelay() {
		Debug.Log("Running RuneSpell WaitDelay");
		yield return new WaitForSeconds(m_triggerDelay);
		Debug.Log ("Done waiting, instantiating effects");
		
		if(m_triggerFx) {
			Network.Instantiate(m_triggerFx, transform.position, Quaternion.FromToRotation(m_triggerEffectDirection, transform.up), 0);
		}
		Debug.Log ("Triggering spell effects");
		TriggerEffects(m_targets.ToArray());
		Debug.Log ("Destroying Rune");
		Network.Destroy(transform.root.gameObject);
	}
	
	// This only works if the collider is on the same transform as this script
	// Use AddTarget otherwise
	void OnTriggerEnter(Collider other) {
		if(Network.isServer) {
			if(other.tag == "Player") {
				AddTarget(other.gameObject);
			}
		}
		Debug.Log ("Still works with child objects.");
		
		Network.Destroy (transform.root.gameObject);
	}
	
	// This only works if the collider is on the same transform as this script
	// Use RemoveTarget otherwise
	void OnTriggerExit(Collider other) {
		if(other.tag == "Player") {
			RemoveTarget(other.gameObject);
		}
	}
}