using UnityEngine;
using System.Collections;

public class Mana : MonoBehaviour {
	private float maxMana;										// How much mana that the player has
	private float regenRate;										// The rate at which the player gains mana passively
	private float currentMana;									// How much mana the player has right now
	
	public Texture2D mBarBackTex;								// The background texture that will serve as a deco for the manabar
	public Texture2D nullTex;									// A texture that will be stretched to display the amount of mana
	public Color mBarColor = Color.blue;						// The color of the mana fill
	public Vector2 barRelPosition = new Vector2(0.5f, 0.05f);	// The position (as a percentage of the screen) which the bar will sit
	public Vector2 manaInset = new Vector2(8,8);				// The border (in pixels) that the mana bar's background texture has around the area which will be filled
	
	private Rect mBarBackPos;									// The position that the mana bar's background will sit
	private Rect mBarFillPos;									// The position that the mana bar's fill will sit (manipulated at runtime)
	
	public bool maxChangedSinceLastFetch = false;
	public bool currentChangedSinceLastFetch = false;
	public bool regenChangedSinceLastFetch = false;
	
	public bool HasMaxUpdate {
		get { return maxChangedSinceLastFetch; }
	}
	
	public bool HasCurrentUpdate {
		get { return currentChangedSinceLastFetch; }
	}
	
	public bool HasRegenUpdate {
		get { return regenChangedSinceLastFetch; }
	}
	
	public float Percentage {
		get { maxChangedSinceLastFetch = false;
			  return currentMana/maxMana; }
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
		maxMana = amount;
		currentMana = amount;
		maxChangedSinceLastFetch = true;
		currentChangedSinceLastFetch = true;
	}
	
	public void SetCurrentMana(float amount) {
		if(amount < 0) {
			amount *= -1;
		}
		currentMana = amount;
		if(currentMana>maxMana) {
			currentMana = maxMana;
		}
		currentChangedSinceLastFetch = true;
	}
	
	public void SetRegenRate(float amountPerSecond) {
		regenChangedSinceLastFetch = true;
		regenRate = amountPerSecond;
	}

	public float GetMaxMana() {
		maxChangedSinceLastFetch = false;
		return maxMana;
	}
	
	public float GetCurrentMana() {
		currentChangedSinceLastFetch = false;
		return currentMana;
	}
	
	public float GetRegenRate() {
		regenChangedSinceLastFetch = false;
		return regenRate;
	}
	
	public bool DrainMana(float amount) {
		if (amount <= currentMana) {
			currentMana -= amount;
			currentChangedSinceLastFetch = true;
			return true;
		} else {
			return false;
		}
	}
	
	public void GrantMana(float amount) {
		currentMana = (currentMana+amount > maxMana ? maxMana : currentMana+amount);
		currentChangedSinceLastFetch = true;
	}
	
	
	/*void OnGUI() {
		// Draw the back bar which will house the actual manabar
		if(networkView.isMine) {
			// Draw the background of the mana bar
			GUI.DrawTexture(mBarBackPos, mBarBackTex);
			
			// Set the fill width based on current mana
			mBarFillPos.width = Mathf.FloorToInt((currentMana/maxMana) * (mBarBackTex.width - manaInset.x));
			GUI.color = mBarColor;
			GUI.DrawTexture(mBarFillPos, nullTex);
		}
	}*/
}