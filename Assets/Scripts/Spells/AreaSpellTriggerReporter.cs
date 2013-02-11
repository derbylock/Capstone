using UnityEngine;

public class AreaSpellTriggerReporter : MonoBehaviour {
	private AreaSpell m_rootSpell;
	
	void Start() {
		if(!Network.isServer) {
			Destroy(this);
		}
		
		m_rootSpell = transform.root.GetComponent<AreaSpell>();
		
		if(!m_rootSpell) {
			Debug.LogError(transform.root.name + " does not have an AreaSpell attached to it " +
						   "but an AreaSpellTriggerReporter is trying to access it.");
		}
	}
	
	/*****************************************************************
	 * IMPORTANT NOTE:
	 * OnTrigger functions always run, even when a script is disabled.
	 * In order to avoid additional overhead on the clients, who do
	 * not do any of the processing within these blocks, we need
	 * to simply remove the script rather than just disable it. This
	 * is not an issue within the confines of this game, as there is
	 * no intent or any currently known way to allow them to assume
	 * host control.
	 ****************************************************************/
	
	void OnTriggerEnter(Collider other) {
		if(other.tag == "Player") {
			m_rootSpell.AddTarget(other.gameObject);
		}
	}
	
	void OnTriggerExit(Collider other) {
		if(other.tag == "Player") {
			m_rootSpell.RemoveTarget(other.gameObject);
		}
	}
	
	void OnTriggerStay(Collider other) {
		Debug.Log("Inside Trigger");
		if(other.tag == "Player" && m_rootSpell.GetType() == typeof(AreaOfEffectSpell)) {
			Debug.Log ("Found target for AoE spell: " + other.transform.root.name);
			m_rootSpell.AddTarget(other.gameObject);
		}
	}
}