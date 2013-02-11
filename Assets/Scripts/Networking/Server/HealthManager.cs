using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class HealthManager : MonoBehaviour {
	public struct PlayerHealth {
		private NetworkPlayer player;
		private Health pHealth;
		
		public NetworkPlayer Player {
			get { return player; }
			set { player = value; }
		}
		
		public Health Health {
			get { return pHealth; }
			set { pHealth = value; }
		}
	}
	
	public bool playerServer;
	public int numPlayers;
	private PlayerHealth[] players;


	void Start () {
		networkView.observed = this;
		
		players = new PlayerHealth[numPlayers];
		if(playerServer) {
			players[0].Player = Network.player;
			for(int i=0; i<numPlayers-1; i++) { players[i+1].Player = Network.connections[i]; }
		}
		else {
			for(int i=0; i<numPlayers; i++) { players[i].Player = Network.connections[i]; }
		}
	}
	
	/// <summary>
	/// Takes the local client's Network information and returns the Health script
	/// data associated with that player's character.
	/// </summary>
	/// <returns>
	/// The Health of the local client.
	/// </returns>
	/// <param name='me'>
	/// The NetworkPlayer which represents the local client.
	/// </param>
	public Health GetMyHealth(NetworkPlayer me) {
		for(int i=0; i<players.Length; i++) {
			if(players[i].Player == me) { return players[i].Health; }
		}
		Debug.LogError("No Health script exists for this player.");
		return null;
	}
}