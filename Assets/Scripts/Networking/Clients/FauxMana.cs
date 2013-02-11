using UnityEngine;
using System.Collections;

public class FauxMana : ScriptableObject {
	public float m_maxMana;
	public float m_currentMana;
	public float m_regenRate;
	
	public float Percentage {
		get {
			return m_currentMana/m_maxMana; }
	}
	
	public void Update() {
		//Debug.Log ("FauxManaUpdate");
		m_currentMana += m_regenRate*Time.deltaTime;
		if(m_currentMana > m_maxMana) {
			//Debug.Log ("Capping Mana");
			m_currentMana = m_maxMana;
		}
		if(m_currentMana < 0f) {
			m_currentMana = 0f;
		}
	}
}