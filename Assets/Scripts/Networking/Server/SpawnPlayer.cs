// CreatePlayer and UpdateViewID were both commented out, if problems occur,
// check to make sure they are not the problem first.

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class SpawnPlayer : MonoBehaviour {
	private static int playerNum = 0;
	public GameObject playerObject;
  public GameObject[] playableCharacters;
	public GameObject playerCam;
	private GameObject me;
  
  private bool isSelecting;

	void OnServerInitialized() {
    isSelecting = true;
		//SpawnMe();
	}
	
	void OnConnectedToServer() {
		isSelecting = true;
    //SpawnMe();
	}

	void OnPlayerDisconnected(NetworkPlayer player) {
	    Debug.Log("Clean up after player " +  player);
	    Network.RemoveRPCs(player);
	    Network.DestroyPlayerObjects(player);
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
		//NetworkView[] views = player.GetComponents<NetworkView>();
		//views[0].observed = player.transform;
		//views[1].observed = player.GetComponent<NetworkInterpolatedTransform>();
		//views[2].observed = player.GetComponent<NetworkSpellInterface>();

    NetworkSpellInterface inter = player.GetComponent<NetworkSpellInterface>();
    inter.enabled = false;

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
		
		// Set up the camera
		GameObject camera = Instantiate(playerCam) as GameObject;
		CameraController camFollow = camera.GetComponent<CameraController>();
		
		//camFollow.target = player.transform;
    camFollow.AssignTarget(player.transform);

		PlayerRegister registry = player.GetComponent<PlayerRegister>();
		registry.Register();
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
  
  void OnGUI() {
    if(isSelecting) {
      float buttonSize = 128f;
      float buffer = 10;
      float xStartingPoint = (Screen.width / 2) - ((playableCharacters.Length * (buttonSize + buffer)) / 2) - (buffer / 2);
      for(int i = 0; i < playableCharacters.Length; ++i) {
        CharacterData data = playableCharacters[i].GetComponent<CharacterData>();
        Rect buttonRect = new Rect( xStartingPoint + (buttonSize + buffer) * i,
                                    Screen.height / 2 - buttonSize / 2,
                                    buttonSize, buttonSize);
        if(data != null) {
          if(GUI.Button(buttonRect, data.icon)) { // Obviously wrong but I don't know the call by heart.
            playerObject = playableCharacters[i];
            isSelecting = false;
            SpawnMe();
            break;
          }
        }
      }
    }
  }
}