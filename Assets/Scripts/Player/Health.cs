using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	public float maxHealth;
	public float regenRate;
	public float currentHealth;

  public bool changedSinceLastFetch = false;

	#region Screwy GUI Code NEEDS CLEANUP
	/*//public Texture2D mBarBackTex;								// The background texture that will serve as a deco for the manabar
	public Texture2D nullTex;									// A texture that will be stretched to display the amount of mana
	public Color hBarColor = Color.red;							// The color of the mana fill
	public Vector2 barRelPosition = new Vector2(0.5f, 0.05f);	// The position (as a percentage of the screen) which the bar will sit
	public Vector2 barFullSize = new Vector2(500, 40);
	//public Vector2 manaInset = new Vector2(8,8);				// The border (in pixels) that the mana bar's background texture has around the area which will be filled
	
	//private Rect mBarBackPos;									// The position that the mana bar's background will sit
	private Rect hBarFillPos;									// The position that the mana bar's fill will sit (manipulated at runtime)
	
	
	
	/*public Texture2D healthBarFullTex;
	public Rect healthBar = new Rect(0.5f, 0.05f, 500f, 50f); //Size and position of the bar, position is relative to screen size whereas size is absolute
	public Texture2D healthBarCapTex;
	
	public Vector2 hBarCapSize;
	
	public Color healthBarFull = Color.blue;
	public Color healthBarEmpty = new Color(0, 0, 0, 0.5f);*/
	#endregion
	
	public bool IsAlive {
		get { return currentHealth > 0f; }
	}
	
	public float Percentage {
		get { changedSinceLastFetch = false;
			  return currentHealth/maxHealth; }
	}
	
	public bool HasUpdate {
		get { return changedSinceLastFetch; }
	}
	
	public void SetHealth(float health) {
		changedSinceLastFetch = true;
		maxHealth = health;
		currentHealth = maxHealth;
	}
	
	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
		/*nullTex = Resources.Load("null") as Texture2D;

		hBarFillPos = new Rect(Mathf.FloorToInt (Screen.width*barRelPosition.x - barFullSize.x/2),
							   Mathf.FloorToInt (Screen.width*barRelPosition.y - barFullSize.y/2),
							   0, barFullSize.y);*/
	}
	
	public void ReceiveDamage(float amount) {
		changedSinceLastFetch = true;

    Armor armor = gameObject.GetComponent<Armor>();

    amount = amount * (1 - armor.armor);
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