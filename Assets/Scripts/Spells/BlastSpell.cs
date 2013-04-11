using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlastSpell : Spell {
	//private const float SPHERE_MOVE_DISTANCE = 0.0001F;
	
	public float m_radius;
	public GameObject m_activationEffect;
	private List<GameObject> m_targets = new List<GameObject>();
	
	public override void Cast (Vector3 castFrom, Vector3 castTo) {
		Network.Instantiate(m_activationEffect, m_myCaster.position, m_myCaster.rotation, 0);
		
		FindTargets();
		
		if(m_targets.Count > 0) {
			TriggerEffects(m_targets.ToArray());
		}
		
		Network.Destroy(gameObject);
	}
	
	void FindTargets() {
		//RaycastHit[] hits = Physics.SphereCastAll(m_myCaster.position, m_radius, Vector3.up, SPHERE_MOVE_DISTANCE);
    Collider[] hits = Physics.OverlapSphere(transform.position, m_radius);

		if(hits != null && hits.Length > 0) {
			
			for(int i=0; i<hits.Length; i++) {
				if(hits[i].transform.root != m_myCaster) {
					m_targets.Add(hits[i].transform.root.gameObject);
				}
			}
		}
	}
}