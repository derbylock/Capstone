using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthGUI : MonoBehaviour {
	private bool initialized = false;
	private PlayerValues m_healthPool;
	private int m_myTeam;
	
	private Health[] m_allyHp;
	private int m_myIndex;
	private Health[] m_enemyHp;
	
	// Drawing variables
	public Texture2D m_nullTex;				// Texture used to draw the bars
	public Color m_barColor;				// Color of the bars
	public int m_guiDepth;				// How far forward or backward is this within the GUI
	public float m_bufferScale;				// Size of the buffer between the bars (as a percentage of the screen)
	public float m_separatorScale;			// The size of the space between the topmost and second bar
	public Vector2 m_topPlayerScale;		// The scale of the top-most player's health bar on both teams, this will usually be the user
	public Vector2 m_additionalPlayerScale;	// The scale of all other players' health bar underneath the top player
	
	// Float arrays holding health values as percentages
	// We use these to lighten the load on OnGUI by given it strict values
	// instead of forcing it to recalculate it each time it runs.
	private float[] m_allyPercentages;
	private float[] m_enemyPercentages;
	
	
	void LateUpdate() {
		if(initialized) {
			m_allyPercentages = m_healthPool.GetMyTeamHP(Network.player, out m_myTeam);
			m_enemyPercentages = m_healthPool.GetOppTeamHP(m_myTeam);
		}
	}
	
	public void Initialize() {
		GameObject gTeam = GameObject.FindGameObjectWithTag("Player Team");
		Team team = gTeam.GetComponent<Team>();
		m_myTeam = team.m_teamNumber;
		
		// Cache the global storage for health data
		GameObject healthValues = GameObject.Find ("Player Values");
		m_healthPool = healthValues.GetComponent<PlayerValues>();
		
		m_allyPercentages = m_healthPool.GetMyTeamHP(Network.player, out m_myTeam);
		m_enemyPercentages = m_healthPool.GetOppTeamHP(m_myTeam);
		
		initialized = true;
		
		
		
		/*// Get the PlayerDataManager script so that we can get its data on players
		GameObject playerManager = GameObject.Find("Player Data Manager");
		PlayerDataManager manager = playerManager.GetComponent<PlayerDataManager>();
		
		// Get the player's team number
		int myTeam = -1;
		myTeam = manager.GetTeam(Network.player);
		Debug.Log("Player has a team: " + (myTeam != -1));
		
		// Get the ally team's Health information		
		PlayerContainer[] temp = manager.GetMyTeam(myTeam);
		if(temp != null) {
			Debug.Log ("Temp is not null, player has a team");
			List<Health> tempHp = new List<Health>();
			for(int i=0; i<temp.Length; i++) {
				if(temp[i] != null) {
					// Force this player into the first position of the array and everyone else
					// in the order in which they are were added in the PlayerDataManager
					if(temp[i].m_player == Network.player) {
						Debug.Log ("Added");
						tempHp.Insert(0, temp[i].m_health);
					} else {
						Debug.Log ("Added");
						tempHp.Add(temp[i].m_health);
					}
				}
			}
			m_allyHp = tempHp.ToArray();
			m_allyPercentages = new float[m_allyHp.Length];
			tempHp.Clear();
			tempHp.TrimExcess();
		} else {
			Debug.Log("Bad team number given, no ally team data returned.");
		}
		
		
		// Get the other team's Health information
		/*temp = manager.GetEnemyTeam(myTeam);
		if(temp[0] != null) {
			List<Health> tempHp = new List<Health>();
			for(int i=0; i<temp.Length; i++) {
				if(temp[i] != null) {
					Debug.Log ("Added");
					tempHp.Add(temp[i].m_health);
				}
			}
			m_enemyHp = tempHp.ToArray();
			m_enemyPercentages = new float[m_enemyHp.Length];
		} else {
			Debug.Log("No enemy team data returned.");
		}
		
		initialized = true;
		
		// Calculate everyone's health
		CalculateHealthPercentages();*/
	}
	
	private void CalculateHealthPercentages() {
		for(int i=0; i<m_allyHp.Length; i++) {
			m_allyPercentages[i] = m_allyHp[i].currentHealth / m_allyHp[i].maxHealth;
		}
		
		/*for(int i=0; i<m_enemyHp.Length; i++) {
			m_enemyPercentages[i] = m_enemyHp[i].currentHealth / m_enemyHp[i].maxHealth;
		}*/
	}
	
	void OnGUI() {
		if(initialized) {
			// Set the GUI depth
			GUI.depth = m_guiDepth;
			
			// Cache some values so that we can use them repeatedly
			float hBufferSize = Screen.width*m_bufferScale;
			float vBufferSize = Screen.height*m_bufferScale;
			float tBarVertSize = m_topPlayerScale.y*Screen.height;
			Vector2 aBarSize = new Vector2(Screen.width*m_additionalPlayerScale.x,
										   Screen.height*m_additionalPlayerScale.y);
			float separatorSize = Screen.height*m_separatorScale;
			
			string playerTeam = "";
			GUI.color = m_barColor;
			if(m_allyPercentages != null && m_allyPercentages.Length >= 1) {
				playerTeam = playerTeam + m_allyPercentages[0] + " ";
				// Draw the player's team first, starting with the player
				GUI.DrawTexture(new Rect(hBufferSize, vBufferSize,
										 m_topPlayerScale.x*Screen.width * m_allyPercentages[0],
										 tBarVertSize),
								m_nullTex);
				
				// TESTING
				//Debug.Log (new Rect(hBufferSize, vBufferSize,
				//						 m_topPlayerScale.x*Screen.width * m_allyPercentages[0],
				//						 tBarVertSize));
				
				if(m_allyPercentages.Length > 1) {
					// This is the Rect that will be manipulated for all additional bars
					// Having this allows us to set all of the variables and then only change
					// the vertical offset and the bar width each time we create a new bar
					Rect additionalBar = new Rect(hBufferSize, vBufferSize+tBarVertSize+separatorSize, 0, aBarSize.y);
					
					// Intentionally start at i=1 because we already drew the player
					for(int i=1; i<m_allyPercentages.Length; i++) {
						playerTeam = playerTeam + m_allyPercentages[0] + " ";
						
						additionalBar.width = aBarSize.x * m_allyPercentages[i];
						GUI.DrawTexture (additionalBar, m_nullTex);
						
						// Tack on the added space that was just consumed
						additionalBar.y += aBarSize.y+vBufferSize;
					}
				}
			} else {
				//Debug.Log ("No allies found, including player.");
			}
			
			// Draw the enemy's team, starting with the first player
			if(m_enemyPercentages != null && m_enemyPercentages.Length >= 1) {
				GUI.DrawTexture(new Rect((Screen.width-hBufferSize)-(m_topPlayerScale.x*Screen.width * m_enemyPercentages[0]),
										 vBufferSize,
										 m_topPlayerScale.x*Screen.width * m_enemyPercentages[0],
										 tBarVertSize),
								m_nullTex);
				
				if(m_enemyPercentages.Length > 1) {
					// Here we have a slightly different case, but the essence of it is the same.
					// Instead of only 2 variables that have to change, we need to change 3 of them.
					Rect additionalBar = new Rect(0, vBufferSize+tBarVertSize+separatorSize,
												  0, aBarSize.y);
					
					// Intentionally start at i=1 because we already drew the player
					for(int i=1; i<m_enemyPercentages.Length; i++) {
						additionalBar.x = (Screen.width-hBufferSize) - (aBarSize.x * m_enemyPercentages[i]);
						additionalBar.width = aBarSize.x * m_enemyPercentages[i];
						GUI.DrawTexture (additionalBar, m_nullTex);
						
						// Tack on the added space that was just consumed
						additionalBar.y += aBarSize.y+vBufferSize;
					}
				}
			} else {
				Debug.Log ("No enemies found.");
			}
			
			GUI.color = Color.white;
		}
	}
}