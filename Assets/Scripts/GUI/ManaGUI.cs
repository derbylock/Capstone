using UnityEngine;
using System.Collections;

public class ManaGUI : MonoBehaviour {
	private bool initialized = false;
	private PlayerValues m_manaPool;
	private int m_myTeam;
	
	private Mana[] m_allyMp;
	private int m_myIndex;
	private Mana[] m_enemyMp;
	
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
			m_allyPercentages = m_manaPool.GetMyTeamMP(Network.player, out m_myTeam);
			m_enemyPercentages = m_manaPool.GetOppTeamMP(m_myTeam);
		}
	}
	
	public void Initialize() {
		GameObject gTeam = GameObject.FindGameObjectWithTag("Player Team");
		Team team = gTeam.GetComponent<Team>();
		m_myTeam = team.m_teamNumber;
		
		// Cache the global storage for health data
		GameObject healthValues = GameObject.Find ("Player Values");
		m_manaPool = healthValues.GetComponent<PlayerValues>();
		
		m_allyPercentages = m_manaPool.GetMyTeamMP(Network.player, out m_myTeam);
		m_enemyPercentages = m_manaPool.GetOppTeamMP(m_myTeam);
		
		initialized = true;
	}
	
	void OnGUI() {
		if(initialized) {
			//Debug.Log("Mana GUI Code");
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
