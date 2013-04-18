using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class SpawnPlayer : MonoBehaviour {
	private static int playerNum = 0;
	public GameObject playerObject;
	public GameObject playerCam;
	private GameObject me;
	
	//public const float DEFAULT_HEALTH = 1000f;
	//private bool spawned = false; 
	
	/*void Update() {
		if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
			Instantiate(playerObject);
		}
	}*/
	void OnServerInitialized() {
		SpawnMe();
		/*GameObject player = new GameObject();
		Initialize(ref player);
		Team team = player.GetComponent (typeof(Team)) as Team;
		team.teamNumber = 0;*/
	}
	
	void OnConnectedToServer() {
		SpawnMe();
		/*GameObject player = new GameObject();
		Initialize(ref player);
		Team team = player.GetComponent (typeof(Team)) as Team;
		team.teamNumber = 1;*/
	}
	
	[RPC]
	void UpdateViewID(NetworkViewID id) {
		transform.networkView.viewID = id;
	}
	
	void OnPlayerDisconnected(NetworkPlayer player) {
	    Debug.Log("Clean up after player " +  player);
	    Network.RemoveRPCs(player);
	    Network.DestroyPlayerObjects(player);
	}
	
	[RPC]
	void CreatePlayer(Vector3 location, int teamNumber) {
		// Create the object and tweak the various settings
		GameObject player = new GameObject();
		Initialize(ref player);
		Team team = player.GetComponent (typeof(Team)) as Team;
		team.m_teamNumber = teamNumber;
		
		// This should be taken care of by the fact that the call will be
		// made on the client/client server rather than the server.
		//player.networkView.owner = Network.player;
		
		// Set the location of the player
		player.transform.position = location;
	}
	
	void Initialize(ref GameObject player) {
		player = Network.Instantiate(playerObject, transform.position, Quaternion.identity, 0) as GameObject;
		GameObject camera = Instantiate(playerCam) as GameObject;
		FollowScript camFollow = camera.GetComponent(typeof(FollowScript)) as FollowScript;
    //CameraFollow camFollow = camera.GetComponent<CameraFollow>();
		Team team = player.GetComponent(typeof(Team)) as Team;
		
		if(camFollow) {
			//camFollow.target = player.transform;
      camFollow.AssignTarget(player.transform);
		} else {
			Debug.Log("Can't find FollowScript");
		}
		
		if(team) {
			//team.teamNumber = playerNum % 2;
			if(Network.isServer) { team.m_teamNumber = 0; }
			else { team.m_teamNumber = 1; }
			playerNum++;
			
			Debug.Log (team.m_teamNumber);
			Debug.Log (playerNum);
		}
	}
	
	//[RPC]
	void SpawnMe() {
		// Create the player and set up scripts to be viewed over the network
		GameObject player = Network.Instantiate(playerObject, transform.position, Quaternion.identity, 0) as GameObject;
		NetworkView[] views = player.GetComponents<NetworkView>();
		views[0].observed = player.transform;
		//views[1].observed = player.GetComponent<NetworkInterpolatedTransform>();
		//views[2].observed = player.GetComponent<NetworkSpellInterface>();
		
		// Add and set the player's health
		NetworkViewID viewID = Network.AllocateViewID();
		if(Network.isClient) {
			player.networkView.RPC ("AddServerComponent", RPCMode.Server, "Health");
		} else {
			player.networkView.RPC ("AddNetworkComponent", RPCMode.AllBuffered, "Health", viewID);
		}
		
		
		
		// Add the player's buff list
		viewID = Network.AllocateViewID();
		if(Network.isClient) {
			player.networkView.RPC ("AddServerComponent", RPCMode.Server, "BuffContainer");
		} else {
			player.networkView.RPC ("AddNetworkComponent", RPCMode.AllBuffered, "BuffContainer", viewID);
		}
		
		/*if(Network.isServer) {
			NetworkView netView = gameObject.AddComponent<NetworkView>();
			netView.viewID = viewID;
			netView.observed = gameObject.AddComponent<BuffContainer>();
		}*/
		
		// Set up the camera
		GameObject camera = Instantiate(playerCam) as GameObject;
		//FollowScript camFollow = camera.GetComponent<FollowScript>();
		//CameraFollow camFollow = camera.GetComponent<CameraFollow>();
		CameraController camFollow = camera.GetComponent<CameraController>();
		
		//camFollow.target = player.transform;
    camFollow.AssignTarget(player.transform);

		PlayerRegister registry = player.GetComponent<PlayerRegister>();
		registry.Register();
		
		// Send data to the player manager
		/*GameObject managerObj = GameObject.Find("Player Data Manager");
		PlayerDataManager manager = managerObj.GetComponent<PlayerDataManager>();
		manager.AddPlayer(player);*/
	}
	
	/*void OnGUI() {
		if((Network.isClient || Network.isServer) && !spawned) {
			if(GUI.Button (new Rect(Screen.width-180, Screen.height-50, 150, 30), "Spawn")) {
				spawned = true;
				//networkView.RPC("SpawnMe", RPCMode.Server);
				SpawnMe();
			}
		}
	}*/
}