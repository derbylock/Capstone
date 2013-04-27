using UnityEngine;
using System.Collections;

public class HealthRework : MonoBehaviour {
  private int healthUpdates = 0x0;

	public float maxHealth;
	public float regenRate;
	public float currentHealth;
	public bool changedSinceLastFetch = false;

  public Armor armor;
	
	public bool IsAlive {
		get { return currentHealth > 0f; }
	}
	
	public float GetPercentage() {
    healthUpdates &= ~Constants.RESOURCE_UPDATE_CURRENT;
		return currentHealth/maxHealth;
	}
	
  public int GetUpdates() {
    return healthUpdates;
  }
	
	public void SetHealth(float health) {
    healthUpdates |= Constants.RESOURCE_UPDATE_CURRENT;
    healthUpdates |= Constants.RESOURCE_UPDATE_MAXIMUM;

		maxHealth = health;
		currentHealth = maxHealth;
	}
	
	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
	}

	public void ReceiveDamage(float amount) {
    if (armor == null) {
      armor = gameObject.GetComponent<Armor>();
    }

    healthUpdates |= Constants.RESOURCE_UPDATE_CURRENT;

    float damageMod = 1 - armor.armor;
    amount *= damageMod;

		currentHealth = Mathf.Max(0f, currentHealth-amount);
		if(currentHealth == 0f) { OnDeath(); }
	}
	
	public void ReceiveHeal(float amount) {
    healthUpdates |= Constants.RESOURCE_UPDATE_CURRENT;

		currentHealth = Mathf.Min (maxHealth, currentHealth+amount);
	}
	
	void OnDeath() {
		Debug.Log ("Dead");
		
		// Temporary Killer
	}
	
}