using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class PlayerDataManager : MonoBehaviour {
	public const float DEFAULT_HEALTH = 1000f;
	public const float DEFAULT_MANA = 500f;
	public const float DEFAULT_MANA_REGEN = 100f;
	private List<PlayerContainer> players;		// Our all-inclusive data storage modules
	private int[] teamOne;
	private int[] teamTwo;
	
	// Rework variables
	public const int MAX_TEAM_SIZE = 4;
	public PlayerContainer[] m_teamOne;
	public PlayerContainer[] m_teamTwo;
	public PlayerContainer[] m_teamNeutral;
	private int playerCount;
	
	// Health value variables
	GameObject m_playerValuePool;
	
	void Start() {
		networkView.observed = this;
		if(Network.isClient) {
			Debug.Log ("What in the Sam-hell...");
		}
		
		if(Network.isServer) {
			m_teamOne = new PlayerContainer[MAX_TEAM_SIZE];
			m_teamTwo = new PlayerContainer[MAX_TEAM_SIZE];
			//Debug.Log (m_teamOne.Length + m_teamTwo.Length);
			players = new List<PlayerContainer>();
			m_teamNeutral = new PlayerContainer[MAX_TEAM_SIZE*2];
		}
		
		m_playerValuePool = GameObject.Find ("Player Values");
	}
	
	void Update() {
		UpdateMana(m_teamOne);
		UpdateMana(m_teamTwo);
	}
	
	void LateUpdate() {
		if(Network.isServer) {
			for(int i=0; i<m_teamOne.Length; i++) {
				if(m_teamOne[i] != null && m_teamOne[i].m_health.HasUpdate) {
					m_playerValuePool.networkView.RPC ("UpdatePlayerHealth", RPCMode.All, 0, m_teamOne[i].m_player, m_teamOne[i].m_health.Percentage);
				}
			}
			for(int i=0; i<m_teamTwo.Length; i++) {
				if(m_teamTwo[i] != null && m_teamTwo[i].m_health.HasUpdate) {
					m_playerValuePool.networkView.RPC ("UpdatePlayerHealth", RPCMode.All, 1, m_teamTwo[i].m_player, m_teamTwo[i].m_health.Percentage);
				}
			}
			
			
			
			// Handle changes in mana
			for(int i=0; i<m_teamOne.Length; i++) {
				if(m_teamOne[i] != null) {
					if(m_teamOne[i].m_mana.HasMaxUpdate) {
						m_playerValuePool.networkView.RPC("UpdatePlayerMaxMana", RPCMode.All, 0, m_teamOne[i].m_player, m_teamOne[i].m_mana.GetMaxMana(), (float)Network.time);
					}
					if(m_teamOne[i].m_mana.HasCurrentUpdate) {
						m_playerValuePool.networkView.RPC("UpdatePlayerCurrentMana", RPCMode.All, 0, m_teamOne[i].m_player, m_teamOne[i].m_mana.GetCurrentMana(), (float)Network.time);
					}
					if(m_teamOne[i].m_mana.HasRegenUpdate) {
						m_playerValuePool.networkView.RPC("UpdatePlayerManaRegen", RPCMode.All, 0, m_teamOne[i].m_player, m_teamOne[i].m_mana.GetRegenRate(), (float)Network.time);
					}
				}
			}
			for(int i=0; i<m_teamTwo.Length; i++) {
				if(m_teamTwo[i] != null) {
					if(m_teamTwo[i].m_mana.HasMaxUpdate) {
						m_playerValuePool.networkView.RPC("UpdatePlayerMaxMana", RPCMode.All, 1, m_teamTwo[i].m_player, m_teamTwo[i].m_mana.GetMaxMana(), (float)Network.time);
					}
					if(m_teamTwo[i].m_mana.HasCurrentUpdate) {
						m_playerValuePool.networkView.RPC("UpdatePlayerCurrentMana", RPCMode.All, 1, m_teamTwo[i].m_player, m_teamTwo[i].m_mana.GetCurrentMana(), (float)Network.time);
					}
					if(m_teamTwo[i].m_mana.HasRegenUpdate) {
						m_playerValuePool.networkView.RPC("UpdatePlayerManaRegen", RPCMode.All, 1, m_teamTwo[i].m_player, m_teamTwo[i].m_mana.GetRegenRate(), (float)Network.time);
					}
				}
			}
		}
	}
	
	public void UpdateMana(PlayerContainer[] team) {
		for(int i=0; i<team.Length; i++) {
			if(team[i] != null) {
				team[i].m_mana.Update();
			}
		}
	}

	/// <summary>
	/// Returns the health of the character controlled by the selected NetworkPlayer.
	/// </summary>
	public Health GetHealth (NetworkPlayer target) {
		for(int i=0; i<players.Count; i++) {
			if(players[i].m_player == target) {
				return players[i].m_health;
			}
		}
		return null;
	}

    public Mana GetMana(NetworkPlayer player) {
        for (int i = 0; i < m_teamOne.Length; i++) {
            if (m_teamOne[i] != null && m_teamOne[i].m_player == player) {
                return m_teamOne[i].m_mana;
            }
        }
        for (int i = 0; i < m_teamTwo.Length; i++) {
            if (m_teamTwo[i] != null && m_teamTwo[i].m_player == player) {
                return m_teamTwo[i].m_mana;
            }
        }

        return null;
    }
	
	/*/// <summary>
	/// Returns the buff list of the character controlled by the selected NetworkPlayer.
	/// </summary>
	public BuffContainer GetBuffs (NetworkPlayer target) {
		for(int i=0; i<players.Count; i++) {
			if(players[i].m_player == target) {
				return players[i].m_buffs;
			}
		}
		return null;
	}
	/// <summary>
	/// Sets the player avatar for the given NetworkPlayer.
	/// </summary>
	public void SetPlayerAvatar(NetworkPlayer target, GameObject avatar) {
		for(int i=0; i<players.Count; i++) {
			if(players[i].m_player == target) {
				players[i].m_avatar = avatar;
			}
		}
	}*/
	
	/*public int GetPlayerTeam(NetworkPlayer target) {
		for(int i=0;i<m_teamOne.Length; i++) {
			if(m_teamOne[i].m_player == target) {
				return m_teamOne[i].m_team;
			}
		}
		for(int i=0;i<m_teamTwo.Length; i++) {
			if(m_teamTwo[i].m_player == target) {
				return m_teamTwo[i].m_team;
			}
		}
		return -1;
	}*/
	
	/*[RPC]
	public void DamageTarget(NetworkPlayer target, float amount) {
		bool applied = false;
		for(int i=0; i<m_teamOne.Length; i++) {
			if(m_teamOne[i] && m_teamOne[i].m_player == target) {
				m_teamOne[i].m_health.ReceiveDamage(amount);
				applied = true;
				break;
			}
		}
		if(!applied) {
			for(int i=0; i<m_teamTwo.Length; i++) {
				if(m_teamTwo[i] && m_teamTwo[i].m_player == target) {
					m_teamTwo[i].m_health.ReceiveDamage(amount);
					break;
				}
			}
		}
	}*/
	
	public PlayerContainer[] GetMyTeam(int myTeam) {
		if(myTeam == 0) {
			return m_teamOne;
		} else if (myTeam == 1) {
			return m_teamTwo;
		}
		return null;
	}
	
	public PlayerContainer[] GetEnemyTeam(int myTeam) {
		if(myTeam == 0) {
			return m_teamTwo;
		} else if (myTeam == 1) {
			return m_teamOne;
		}
		return null;
	}
	
	/// <summary>
	/// Fill the player list with all of the players currently in the game.
	/// This function is to be called after players have moved past the lobby.
	/// Typically, the best object to call this function would be the scene manager
	/// as it has access to data about which players are actively player and which
	/// are spectators.
	/// </summary>
	public void Initialize(params NetworkPlayer[] players) {
		if(Network.isServer) {
			// Create all player containers
			for(int i=0; i<players.Length; i++) {
				PlayerContainer newPlayer = new PlayerContainer();
				newPlayer.m_player = players[i];
				newPlayer.m_avatar = null;
				newPlayer.m_buffs = new BuffContainer();
				newPlayer.m_health = new Health();
				newPlayer.m_health.SetHealth(DEFAULT_HEALTH);
				newPlayer.m_mana = new Mana();
				newPlayer.m_mana.SetMana(DEFAULT_MANA);
				newPlayer.m_mana.SetRegenRate(DEFAULT_MANA_REGEN);
				newPlayer.m_team = i%2;
				
				this.players.Add(newPlayer);
			}
			
			// Decide teams at this time, this is only a temporary solution
			teamOne = new int[players.Length/2 + (players.Length%2==1 ? 1 : 0)];
			teamTwo = new int[players.Length/2];
				
			for(int i=0; i<players.Length; i++) {
				int j=0;
				if(i%2 == 0) {
					teamOne[j] = i;
				} else {
					teamTwo[j] = i;
					j++;
				}
			}
		}
	}
	
	//public void AddPlayer(GameObject player) {
	public void AddPlayer(NetworkPlayer player) {
		PlayerContainer newPlayer = ScriptableObject.CreateInstance<PlayerContainer>();
		newPlayer.m_player = player;
		
		// Find the player's avatar and assign it if found
		GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject p in playerObjects) {
			if(p.networkView.owner == player) {
				newPlayer.m_avatar = p;
				break;
			}
		}
		
		newPlayer.m_buffs = newPlayer.m_avatar.GetComponent<BuffContainer>(); //(newPlayer.m_avatar.gameObject.AddComponent<BuffContainer>()) as BuffContainer; // new BuffContainer(); //newPlayer.m_avatar.GetComponent<BuffContainer>();	// Old way
		newPlayer.m_health = newPlayer.m_avatar.GetComponent<Health>();	 //(newPlayer.m_avatar.gameObject.AddComponent<Health>()) as Health; //new Health(); //newPlayer.m_avatar.GetComponent<Health>();			// Old way
		newPlayer.m_health.SetHealth(DEFAULT_HEALTH);
		Debug.Log ("Player health set: " + newPlayer.m_health.maxHealth);
		Debug.Log ("New player IP: " + newPlayer.m_player.ipAddress);
		Debug.Log ("New player model exists: " + newPlayer.m_avatar);
		
		newPlayer.m_mana = new Mana();
		newPlayer.m_mana.SetMana(DEFAULT_MANA);
		newPlayer.m_mana.SetRegenRate(DEFAULT_MANA_REGEN);
		
		// Temporarily decide teams based on join order
		if(m_teamOne == null || m_teamOne.Length<=0) {
			InitializeTeams();
		}
		
		// Assign team and report it to the player
		NetworkSpellInterface spells = newPlayer.m_avatar.GetComponent<NetworkSpellInterface>();
		if(playerCount%2 == 0) {
			m_teamOne[playerCount/2] = newPlayer;
			spells.myTeam = 0;
			m_playerValuePool.networkView.RPC("AddPlayer", RPCMode.AllBuffered, 0, player, newPlayer.m_health.Percentage, DEFAULT_MANA, DEFAULT_MANA_REGEN);
			
			// Set the team of the player so that we have that data available for
			// the spells to make quick decisions for valid targets
			newPlayer.m_avatar.networkView.RPC("RecordTeamNumber", RPCMode.AllBuffered, 0);
		} else {
			m_teamTwo[playerCount/2] = newPlayer;
			spells.myTeam = 1;
			m_playerValuePool.networkView.RPC("AddPlayer", RPCMode.AllBuffered, 1, player, newPlayer.m_health.Percentage, DEFAULT_MANA, DEFAULT_MANA_REGEN);
			
			// Set the team of the player so that we have that data available for
			// the spells to make quick decisions for valid targets
			newPlayer.m_avatar.networkView.RPC("RecordTeamNumber", RPCMode.AllBuffered, 1);
		}
		
		playerCount++;
	}
	
	void InitializeTeams() {
		m_teamOne = new PlayerContainer[MAX_TEAM_SIZE];
		m_teamTwo = new PlayerContainer[MAX_TEAM_SIZE];
		//Debug.Log (m_teamOne.Length + m_teamTwo.Length);
		players = new List<PlayerContainer>();
		m_teamNeutral = new PlayerContainer[MAX_TEAM_SIZE*2];
	}
	
	/*public void InitializeTeamHealth() {
		List<NetworkPlayer> temp = new List<NetworkPlayer>();
		List<float> tempPercent = new List<float>();
		
		// Start with team 1
		for(int i=0; i<m_teamOne.Length; i++) {
			if(m_teamOne[i] != null) {
				temp.Add(m_teamOne[i].m_player);
				tempPercent.Add(m_teamOne[i].m_health.Percentage);
			}
		}
		m_playerValuePool.networkView.RPC ("SetTeam", RPCMode.AllBuffered, 0, temp.ToArray());
		m_playerValuePool.networkView.RPC ("UpdateTeam", RPCMode.AllBuffered, 0, tempPercent.ToArray());
		
		// Clear the lists
		temp.Clear();
		temp.TrimExcess();
		tempPercent.Clear();
		tempPercent.TrimExcess();
		
		// Now handle team 2
		for(int i=0; i<m_teamTwo.Length; i++) {
			if(m_teamTwo[i] != null) {
				temp.Add(m_teamTwo[i].m_player);
				tempPercent.Add(m_teamTwo[i].m_health.Percentage);
			}
		}
		m_playerValuePool.networkView.RPC ("SetTeam", RPCMode.AllBuffered, 1, temp.ToArray());
		m_playerValuePool.networkView.RPC ("UpdateTeam", RPCMode.AllBuffered, 1, tempPercent.ToArray());
	}*/
}