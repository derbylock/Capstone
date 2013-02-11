using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	private bool refreshing;
	
	private int buttonX = 25;
	private int buttonY = 25;
	private int buttonHeight = 25;
	private int buttonWidth = 125;
	
	public string gameType = "PMAW_Capstone_Network";
	public string lobbyName = "Lobby";
	public string returnToLevel = "Test Level";
	
	private HostData[] hostData;
	
	void Update() {
		if(refreshing) {
			if(MasterServer.PollHostList().Length > 0) {
				refreshing = false;
				hostData = MasterServer.PollHostList();
				Debug.Log(hostData.Length);
			}
		}
	}
	
	
	void StartServer() {
		Network.InitializeServer(2, 25001, !Network.HavePublicAddress());
		//MasterServer.RegisterHost (gameType, "Network Test");
		MasterServer.RegisterHost(gameType, Network.player.ipAddress.ToString());
	}
	
	void RefreshHostList() {
		MasterServer.RequestHostList(gameType);
		refreshing = true;
	}
	
	// When we create a server, automatically load into the lobby level.
	void OnServerInitialized() {
		Debug.Log ("Server Created");
		Application.LoadLevel (lobbyName);
	}
	
	// When we join a server, automatically load into the lobby level.
	void OnConnectedToServer() {
		Debug.Log ("Joined server successfully");
	}
	
	void OnDisconnectedFromServer() {
		Application.LoadLevel(returnToLevel);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player) {
		if(Network.connections.Length == 0) {
			Application.LoadLevel (returnToLevel);
		}
	}
	
	void OnMasterServerEvent(MasterServerEvent evt) {
		if (evt == MasterServerEvent.RegistrationSucceeded) {
			Debug.Log("Server Registered");
		}
	}
	
	
	void OnGUI() {
		if(!Network.isClient && !Network.isServer) {
			if(GUI.Button (new Rect (buttonX, buttonY, buttonWidth, buttonHeight), "Start Server")) {
				StartServer();
			}
			
			if(GUI.Button (new Rect (buttonX, buttonY + buttonHeight * 1.2f, buttonWidth, buttonHeight), "Refresh Hosts")) {
				Debug.Log ("Refreshing");
				RefreshHostList();
			}
			
			if(hostData != null) {
				for(int i=0; i< hostData.Length; i++) {
					if(GUI.Button (new Rect(Screen.width/2.0f - buttonWidth/2.0f,
											50 + i*55,
											buttonWidth,
											buttonHeight),
								   hostData[i].gameName)) {
						Network.Connect (hostData[i]);
					}
				}
			}
		}
	}
}