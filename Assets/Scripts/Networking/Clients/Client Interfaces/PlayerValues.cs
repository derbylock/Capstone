using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class PlayerValues : MonoBehaviour {
	public class HealthPercentage : ScriptableObject {
		public NetworkPlayer m_player;
		public float m_healthPercent;
	}
	
	private PlayerDataManager playerManager;
	public List<float> m_teamOneHealth;
	private List<NetworkPlayer> m_teamOnePlayers;
	public List<float> m_teamTwoHealth;
	private List<NetworkPlayer> m_teamTwoPlayers;
	
	public List<FauxMana> m_teamOneMana;
	public List<FauxMana> m_teamTwoMana;
	
	void Awake() {
		networkView.observed = this;
		m_teamOneHealth = new List<float>();
		m_teamOneMana = new List<FauxMana>();
		m_teamOnePlayers = new List<NetworkPlayer>();
		m_teamTwoHealth = new List<float>();
		m_teamTwoMana = new List<FauxMana>();
		m_teamTwoPlayers = new List<NetworkPlayer>();
	}
	
	void Update() {
		foreach(FauxMana mana in m_teamOneMana) { mana.Update(); }
		foreach(FauxMana mana in m_teamTwoMana) { mana.Update(); }
	}
	
	[RPC]
	public void AddPlayer(int team, NetworkPlayer player, float currentHpPercent, float maxMana, float regenRate) {
		if(team == 0) {
			m_teamOnePlayers.Add(player);
			m_teamOneHealth.Add(currentHpPercent);
			m_teamOneMana.Add(ScriptableObject.CreateInstance<FauxMana>());
			m_teamOneMana[m_teamOneMana.Count-1].m_maxMana = maxMana;
			m_teamOneMana[m_teamOneMana.Count-1].m_currentMana = maxMana;
			m_teamOneMana[m_teamOneMana.Count-1].m_regenRate = regenRate;
		}
		if(team == 1) {
			m_teamTwoPlayers.Add(player);
			m_teamTwoHealth.Add(currentHpPercent);
			m_teamTwoMana.Add(ScriptableObject.CreateInstance<FauxMana>());
			m_teamTwoMana[m_teamTwoMana.Count-1].m_maxMana = maxMana;
			m_teamTwoMana[m_teamTwoMana.Count-1].m_currentMana = maxMana;
			m_teamTwoMana[m_teamTwoMana.Count-1].m_regenRate = regenRate;
		}
	}
	
	[RPC]
	public void UpdatePlayerHealth(int team, NetworkPlayer player, float currentHpPercent) {
		if(team == 0) {
			int index = m_teamOnePlayers.IndexOf(player);
			m_teamOneHealth[index] = currentHpPercent;
		}
		if(team == 1) {
			int index = m_teamTwoPlayers.IndexOf(player);
			m_teamTwoHealth[index] = currentHpPercent;
		}
	}
	
	/// <summary>
	/// Updates the current mana of the given player. The function includes
	/// the math necessary to take any regeneration since the time of the 
	/// function call into account.
	/// </summary>
	[RPC]
	public void UpdatePlayerCurrentMana(int team, NetworkPlayer player, float currentMana, float timestamp) {
		if(team==0) {
			int index = m_teamOnePlayers.IndexOf(player);
			m_teamOneMana[index].m_currentMana = currentMana + m_teamOneMana[index].m_regenRate * (float)(Network.time-timestamp);
		}
		if(team==1) {
			int index = m_teamTwoPlayers.IndexOf(player);
			m_teamTwoMana[index].m_currentMana = currentMana + m_teamTwoMana[index].m_regenRate * (float)(Network.time-timestamp);
		}
	}
	
	/// <summary>
	/// Updates the maximum mana of the given player. The function includes
	/// the math necessary to take any regeneration since the time of the 
	/// function call into account.
	/// </summary>
	[RPC]
	public void UpdatePlayerMaxMana(int team, NetworkPlayer player, float maxMana, float timestamp) {
		if(team==0) {
			int index = m_teamOnePlayers.IndexOf(player);
			
			// Here we have an odd case, where we are increasing the amount
			// of mana that the player has. If they are full on mana, we can
			// only assume that they have been full and provide necessary
			// regeneration to them. We do not need to do this if the player
			// was not capped as they were regenerating mana normally for the
			// whole duration of this call.
			float regened = 0f;
			if(m_teamOneMana[index].m_maxMana < maxMana) {
				regened = m_teamOneMana[index].m_regenRate * (float)(Network.time-timestamp);
			}
			m_teamOneMana[index].m_maxMana = maxMana;
			m_teamOneMana[index].m_currentMana += regened;
		}
		if(team==1) {
			int index = m_teamTwoPlayers.IndexOf(player);
			
			float regened = 0f;
			if(m_teamTwoMana[index].m_maxMana < maxMana) {
				regened = m_teamTwoMana[index].m_regenRate * (float)(Network.time-timestamp);
			}
			m_teamTwoMana[index].m_maxMana = maxMana;
			m_teamTwoMana[index].m_currentMana += regened;
		}
	}
	
	/// <summary>
	/// Updates the mana regeneration rate of the given player. The function
	/// includes the math necessary to take any regeneration since the time 
	/// of the function call into account.
	/// </summary>
	[RPC]
	public void UpdatePlayerManaRegen(int team, NetworkPlayer player, float regenRate, float timestamp) {
		if(team==0) {
			int index = m_teamOnePlayers.IndexOf(player);
			m_teamOneMana[index].m_currentMana += (regenRate-m_teamOneMana[index].m_regenRate) * (float)(Network.time-timestamp);
			m_teamOneMana[index].m_regenRate = regenRate;
		}
		if(team==1) {
			int index = m_teamTwoPlayers.IndexOf(player);
			m_teamTwoMana[index].m_currentMana += (regenRate-m_teamTwoMana[index].m_regenRate) * (float)(Network.time-timestamp);
			m_teamTwoMana[index].m_regenRate = regenRate;
		}
	}
	
	public float[] GetMyTeamHP(NetworkPlayer me, out int team) {
		//if(team == 0) {
		for(int i=0; i<m_teamOnePlayers.Count; i++) {
			//Debug.Log ("Running through team");
			if(m_teamOnePlayers[i] == me) {
				team = 0;
				float[] temp = m_teamOneHealth.ToArray();
				temp[0] = m_teamOneHealth[i];
				temp[i] = m_teamOneHealth[0];
				return temp;
			}
		}
		//}
		//if(team == 1) {
		for(int i=0; i<m_teamTwoPlayers.Count; i++) {
			if(m_teamTwoPlayers[i] == me) {
				team = 1;
				float[] temp = m_teamTwoHealth.ToArray();
				temp[0] = m_teamTwoHealth[i];
				temp[i] = m_teamTwoHealth[0];
				return temp;
			}
		}
		//}
		team = -1;
		return null;
	}
	
	/// <summary>
	/// Gets the health of the opposing team. If the team number given is
	/// not 0 or 1, then the return value will be null, otherwise it will
	/// return the other team's array.
	/// </summary>
	public float[] GetOppTeamHP(int myTeam) {
		if(myTeam == 0) {
			return m_teamTwoHealth.ToArray();
		}
		if(myTeam == 1) {
			return m_teamOneHealth.ToArray();
		}
		return null;
	}
	
	public float[] GetMyTeamMP(NetworkPlayer me, out int team) {
		// Find out if the player is on the first team and store their index if they are.
		// The grab the array and sort it before returning it.
		int index = m_teamOnePlayers.IndexOf(me);
		if(index != -1) {
			float[] temp = new float[m_teamOnePlayers.Count];
			for(int i=0; i<m_teamOnePlayers.Count; i++) {
				temp[i] = m_teamOneMana[i].Percentage;
			}
			
			temp[0] = m_teamOneMana[index].Percentage;
			temp[index] = m_teamOneMana[0].Percentage;
			team = 0;
			return temp;
		}
		
		// If we've made it to this point, then the player SHOULD BE in this array. Get
		// their index, and if it's not there, we need to alert the game that there's a
		// problem. If it is there, then we need to get and sort the array before returning it.
		index = m_teamTwoPlayers.IndexOf(me);
		if(index != -1) {
			float[] temp = new float[m_teamTwoPlayers.Count];
			for(int i=0; i<m_teamTwoPlayers.Count; i++) {
				temp[i] = m_teamTwoMana[i].Percentage;
			}
			
			temp[0] = m_teamTwoMana[index].Percentage;
			temp[index] = m_teamTwoMana[0].Percentage;
			team = 1;
			return temp;
		}
		
		Debug.Log ("The player was not found on either team.");
		team = -1;
		return null;
	}
	
	/// <summary>
	/// Gets the health of the opposing team. If the team number given is
	/// not 0 or 1, then the return value will be null, otherwise it will
	/// return the other team's array.
	/// </summary>
	public float[] GetOppTeamMP(int myTeam) {
		if(myTeam == 0) {
			float[] temp = new float[m_teamTwoPlayers.Count];
			for(int i=0; i<m_teamTwoPlayers.Count; i++) {
				temp[i] = m_teamTwoMana[i].Percentage;
			}
			return temp;
		}
		if(myTeam == 1) {
			float[] temp = new float[m_teamOnePlayers.Count];
			for(int i=0; i<m_teamOnePlayers.Count; i++) {
				temp[i] = m_teamOneMana[i].Percentage;
			}
			return temp;
		}
		return null;
	}
}