using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConeSpell : Spell {
	private const float SPHERE_MOVE_DISTANCE = 0.0001F;
	public float m_radius;
	public GameObject m_activationEffect;
	
	public float m_coneAngle;
	private List<GameObject> m_targets = new List<GameObject>();
	
	public override void Cast (Vector3 castTo) {
		if(m_activationEffect) {
			Network.Instantiate(m_activationEffect, m_myCaster.position, m_myCaster.rotation, 0);
		}
		
		FindTargets();
		
		if(m_targets.Count > 0) {
			TriggerEffects(m_targets.ToArray());
		}
		
		Network.Destroy(gameObject);
	}
	
	void FindTargets() {
		RaycastHit[] hits = Physics.SphereCastAll(m_myCaster.position, m_radius, Vector3.up, SPHERE_MOVE_DISTANCE);
		
		if(hits != null && hits.Length > 0) {
			
			for(int i=0; i<hits.Length; i++) {
				if(hits[i].transform.root != m_myCaster && IsInAngle(hits[i].transform)) {
					Debug.Log ("Hit Dat -> " + hits[i].transform.root.name);
					m_targets.Add(hits[i].transform.root.gameObject);
				}
			}
		}
	}
	
	bool IsInAngle(Transform target) {
		Vector3 vectorTo = target.position - transform.position;
		if (Vector3.Dot(m_myCaster.forward, vectorTo) > m_coneAngle) {
			return true;
		}
		return false;
	}
}
