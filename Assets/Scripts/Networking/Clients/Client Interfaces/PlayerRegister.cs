using UnityEngine;
using System.Collections;

public class PlayerRegister : MonoBehaviour {
	public const float DEFAULT_HEALTH = 1000f;
	
	public void Register() {
		if(Network.isClient) {
			networkView.RPC("RegisterMe", RPCMode.Server, Network.player);
		} else {
			ServerRegister();
		}
	}
	
	[RPC]
	void RegisterMe(NetworkPlayer player) {/*
		Health h = gameObject.GetComponent<Health>();
		h.SetHealth(DEFAULT_HEALTH);*/
		
		GameObject managerObject = GameObject.Find("Player Data Manager");
		PlayerDataManager manager = managerObject.GetComponent<PlayerDataManager>();
		
		//manager.AddPlayer(gameObject);
		manager.AddPlayer (player);
	}
	
	void ServerRegister() {/*
		Health h = gameObject.GetComponent<Health>();
		h.SetHealth(DEFAULT_HEALTH);
		*/
		GameObject managerObject = GameObject.Find("Player Data Manager");
		PlayerDataManager manager = managerObject.GetComponent<PlayerDataManager>();
		
		manager.AddPlayer(Network.player);	// We can use Network.player here because only the server can use this
	}
}
