using UnityEngine;
using System.Collections;

public class AreaOfEffectSpell : AreaSpell {
	public float m_distanceAbove = 10f;
	public float m_duration;		// How long does this effect stay there?
	public int m_tickCount;			// How many times does the spell tick over its duration?
	private float m_tickSpacing;	// Calculated at runtime, the time between each tick
	//private float m_tickDamage;		// Calculated at runtime, the damage of each tick
	
	public GameObject m_mainEffect;	// The main effect of the spell
	public Vector3 m_mainEffectOrientation;	// Fix the orientation of the effect
	
	void Start() {
		if(!Network.isServer) {
			this.enabled = false;
		}
		
		// We add 1 to the calculation so that we can start the ticks
		// after the first space and end at the very end of the effect
		m_tickSpacing = m_duration / (m_tickCount + 1);
		
		//m_tickDamage = m_directDamage / m_tickCount;
		
		StartCoroutine("RunSpell");
	}
	
	public override void Cast (Vector3 castFrom, Vector3 castTo) {
		RaycastHit hit;
		
		Debug.Log ("Casting AoE Spell");
		Debug.Log (castFrom + " : " + castTo);
		if(Physics.Raycast(castFrom, Vector3.Normalize(castTo-castFrom), out hit, m_maxDistance)) {
			transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			transform.position = hit.point; // + transform.TransformDirection(Vector3.up) * 0.1f;
		} else {
			Network.Destroy(gameObject);
		}
		
		if(m_mainEffect) {
			Network.Instantiate(m_mainEffect,
								transform.position + (transform.up*m_distanceAbove),
								Quaternion.FromToRotation(m_mainEffectOrientation, transform.up), 0);
		}
		
		m_isInPlace = true;
	}
	
	public override void AddTarget (GameObject target) {
		if(Network.isServer) {
			Debug.Log ("Adding " + target.name + " to " + gameObject.name + " target list.");
			if(m_isInPlace && !m_targets.Contains(target)) {
				m_targets.Add(target);
			}
		}
	}
	
	IEnumerator RunSpell() {
		for(int i=0; i<m_tickCount; i++) {
			yield return new WaitForSeconds(m_tickSpacing);
			TriggerEffects(m_targets.ToArray());
		}
		
		Network.Destroy(gameObject);
	}
	
	void AddInitialTargets() {
		
	}
	
	/*void OnTriggerEnter(Collider other) {
		AddTarget(other.gameObject);
	}
	
	void OnTriggerExit(Collider other) {
		RemoveTarget(other.gameObject);
	}
	
	void OnTriggerStay(Collider other) {
		AddTarget(other.gameObject);
	}*/
}