using UnityEngine;
using System.Collections;

public class HealthRework : MonoBehaviour {
	public float maxHealth;
	public float regenRate;
	public float currentHealth;
	public bool changedSinceLastFetch = false;
	
	public bool IsAlive {
		get { return currentHealth > 0f; }
	}
	
	public float GetPercentage() {
		changedSinceLastFetch = false;
		return currentHealth/maxHealth;
	}
	
	public bool HasUpdate {
		get { return changedSinceLastFetch; }
	}

  public int GetUpdates() {
    Debug.LogError("NOT YET IMPLEMENTED: Health.GetUpdates()");
    return -1;
  }
	
	public void SetHealth(float health) {
		changedSinceLastFetch = true;
		maxHealth = health;
		currentHealth = maxHealth;
	}
	
	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
	}
	
	public void ReceiveDamage(float amount) {
		changedSinceLastFetch = true;
		currentHealth = Mathf.Max(0f, currentHealth-amount);
		
		if(currentHealth == 0f) { OnDeath(); }
	}
	
	public void ReceiveHeal(float amount) {
		changedSinceLastFetch = true;
		currentHealth = Mathf.Min (maxHealth, currentHealth+amount);
	}
	
	void OnDeath() {
		Debug.Log ("Dead");
		
		// Temporary Killer
	}
	
}