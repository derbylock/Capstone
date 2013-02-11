using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaSpell : Spell {
	protected bool m_isInPlace = false;
	protected List<GameObject> m_targets = new List<GameObject>();
	
	public virtual void AddTarget(GameObject target) {
		if(m_isInPlace) {
			Debug.Log ("Adding target: " + target.name);
			m_targets.Add(target);
			if(FindAppropriateTarget()) {
				Debug.Log ("Found Appropriate Target");
				StartCoroutine("WaitDelay");
			}
		}
	}
	
	public void RemoveTarget(GameObject target) {
		m_targets.Remove(target);
	}
	
	bool FindAppropriateTarget() {
		for(int i=0; i<m_targets.Count; i++) {
			Team targetTeam = m_targets[i].GetComponent<Team>();
			bool sameTeam = (targetTeam.m_teamNumber == m_Team ? true : false);
			for(int j=0; j<m_effects.Length; j++) {
				switch(m_effects[i].targets) {
				case EffectTargets.ALL:
					return true;
				case EffectTargets.ALLY:
					if(sameTeam) {
						return true;
					}
					break;
				case EffectTargets.ENEMY:
					if(!sameTeam) {
						return true;
					}
					break;
				case EffectTargets.SELF:
					if(m_targets[i].transform == m_myCaster) {
						return true;
					}
					break;
				};
			}
		}
		return false;
	}
	
	protected virtual IEnumerator WaitDelay() {
		// Because it is an IEnumerator and not abstract, it needs
		// to return something, this will be overriden always though.
		Debug.Log ("The wrong WaitDelay function is running.");
		return null;
	}
}