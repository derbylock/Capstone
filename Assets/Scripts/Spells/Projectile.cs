using UnityEngine;
using System.Collections;

public class Projectile : Spell {
	public float speed;
	public float maxDistance;
	private float currentDistance;
	
	private Vector3 direction;
	
	// Collision and Activation variables
	public GameObject collisionEffect;
	public GameObject dissipationEffect;
	private bool activated = false;
	
	void Start() {
		if(!Network.isServer) {
			this.enabled = false;
		}
	}
	
	public override void Cast(Vector3 castTo) {
		// Get direction vector
		direction = Vector3.Normalize(castTo-transform.position);
		// Begin moving
		StartCoroutine("Move");
		Debug.Log("Casting Projectile");
	}
	
	IEnumerator Move() {
		currentDistance = 0;
		while (!activated && currentDistance < maxDistance) {
			// TODO: Apply logic for aim assist
			
			float travelDistance = speed*Time.deltaTime;
			
			RaycastHit hit;
			if(Physics.Raycast(transform.position, direction, out hit, travelDistance)) {
				if(hit.transform.root != m_myCaster) {
					Activate(hit.transform.gameObject);
					break;
				}
			}
			transform.position += direction*travelDistance;
			
			yield return new WaitForSeconds(Time.deltaTime);
		}
		
		// We have travelled the farthest possible distance, now dispose of the object
		if(dissipationEffect) {
			Network.Instantiate(dissipationEffect, transform.position, transform.rotation, 0);
		}
	}
	
	void Activate(GameObject hitObject) {
		if(!activated) {
			activated = true;
			gameObject.renderer.enabled = false;
			
			if(collisionEffect) {
				Network.Instantiate(collisionEffect, transform.position, transform.rotation, 0);
			}
			
			if(hitObject && hitObject.tag == "Player") {
				TriggerEffects(hitObject);
			} else {
				TriggerEffects();
			}
			
			StartCoroutine("DestroyOnComplete");
			//BeginDestruction();
		}
	}
	
	void BeginDestruction() {
		// TODO: Make this happen
	}
	
	IEnumerator DestroyOnComplete() {
		if(networkView.isMine) {
			if(particleSystem) {
				particleSystem.enableEmission = false;
				yield return new WaitForSeconds(particleSystem.startLifetime);
				Network.Destroy(transform.root.gameObject);
			} else {
				Network.Destroy(transform.root.gameObject);
			}
		}
	}
	
	void OnTriggerEnter(Collider other) {
		if(Network.isServer) {
			if(other.transform.root != m_myCaster) {
				Debug.Log (m_myCaster.name + " and "  + other.transform.root.name);
				Activate(other.gameObject);
			}
		}
		// Turn off the renderer and ensure this does not trigger again
		/*gameObject.renderer.enabled = false;
		activated = true;
		
		if(collisionEffect) {
			Network.Instantiate(collisionEffect, transform.position, transform.rotation, 0);
		}
		
		if(!activated && other.tag == "Player") {
			TriggerEffects(other.gameObject);
		}*/
	}
}