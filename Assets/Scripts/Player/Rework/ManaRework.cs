using UnityEngine;
using System.Collections;

public class ManaRework : MonoBehaviour {

  private int manaUpdates = 0x0;
  
	private float maxMana;										// How much mana that the player has
	private float regenRate;										// The rate at which the player gains mana passively
	private float currentMana;									// How much mana the player has right now

  public int GetUpdates() {
    return manaUpdates;
  }
	
	public float GetPercentage() {
    manaUpdates &= ~Constants.RESOURCE_UPDATE_CURRENT;
		return currentMana/maxMana;
	}

	void Start () {
		currentMana = maxMana;
	}
	
	public void Update () {
		currentMana += regenRate * Time.deltaTime;
		if(currentMana > maxMana) {
			currentMana = maxMana;
		}
	}
	
	public void SetMana(float amount) {
		if(amount < 0) {
			amount *= -1;
		}

    currentMana += amount-maxMana;
		maxMana = amount;
    manaUpdates |= Constants.RESOURCE_UPDATE_MAXIMUM;
    manaUpdates |= Constants.RESOURCE_UPDATE_CURRENT;
	}
	
	public void SetCurrentMana(float amount) {
		if(amount < 0) {
			amount *= -1;
		}
		currentMana = amount;
		if(currentMana>maxMana) {
			currentMana = maxMana;
		}
    manaUpdates |= Constants.RESOURCE_UPDATE_CURRENT;
	}
	
	public void SetRegenRate(float amountPerSecond) {
    manaUpdates |= Constants.RESOURCE_UPDATE_REGEN_RATE;
		regenRate = amountPerSecond;
	}

	public float GetMaxMana() {
    manaUpdates &= ~Constants.RESOURCE_UPDATE_MAXIMUM;
		return maxMana;
	}
	
	public float GetCurrentMana() {
    manaUpdates &= ~Constants.RESOURCE_UPDATE_CURRENT;
		return currentMana;
	}
	
	public float GetRegenRate() {
    manaUpdates &= ~Constants.RESOURCE_UPDATE_REGEN_RATE;
		return regenRate;
	}
	
	public bool DrainMana(float amount) {
		if (amount <= currentMana) {
			currentMana -= amount;
      manaUpdates |= Constants.RESOURCE_UPDATE_CURRENT;
			return true;
		} else {
			return false;
		}
	}
	
	public void GrantMana(float amount) {
		currentMana = Mathf.Clamp(currentMana+amount, 0f, maxMana);
    manaUpdates |= Constants.RESOURCE_UPDATE_CURRENT;
	}
}