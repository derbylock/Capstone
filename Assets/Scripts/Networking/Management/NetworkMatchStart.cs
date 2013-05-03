using UnityEngine;
using System.Collections;

public class NetworkMatchStart : MonoBehaviour {
	public GUIText countdown;
	public int countdownDuration = 5;
	private bool matchFull = false;
	private bool sequenceComplete = false;

  private bool started;

	// Use this for initialization
	void Start () {
		countdown.guiText.enabled = false;
	}

  public void Begin() {
    started = true;
  }
	
	// Update is called once per frame
	void Update () {
		//if(!matchFull) { Debug.Log ("Still Waiting"); }
		if(Network.isServer) {
			if(!matchFull && Network.connections.Length >= 1) {
      //if(started) {
				Debug.Log ("Working");
				matchFull = true;
				
				//GameObject playerManager = GameObject.Find ("Player Data Manager");
				//PlayerDataManager manager = playerManager.GetComponent<PlayerDataManager>();
				//manager.InitializeTeamHealth();
				
				StartCoroutine("SetPlayersReady");
				
				networkView.RPC("BeginStartSequence", RPCMode.All);
			}
			
			if(sequenceComplete) {
				GameObject[] playersUnlock = GameObject.FindGameObjectsWithTag("Player");
				for(int i=0; i<playersUnlock.Length; i++) {
					playersUnlock[i].transform.root.networkView.RPC("UnlockPlayer", RPCMode.All);
				}
				
				WinController controller = gameObject.GetComponent<WinController>();
				controller.BeginMatch();
				sequenceComplete = false;
			}
		}
	}
	
	[RPC]
	void BeginStartSequence() {
		Screen.showCursor = false;
		Screen.lockCursor = true;
		StartCoroutine("MatchStart");
	}
	
	IEnumerator MatchStart() {
		GameObject healthGUI = GameObject.Find("Stat GUI");
		HealthGUI hGui = healthGUI.GetComponent<HealthGUI>();
		ManaGUI mGui = healthGUI.GetComponent<ManaGUI>();
		Debug.Log ("Initialize");
		hGui.Initialize();
		mGui.Initialize();
		
		float startTime = Time.time;
		countdown.guiText.enabled = true;
		
		while(Time.time - startTime < countdownDuration-1) {
			countdown.text = Mathf.FloorToInt(countdownDuration - (Time.time - startTime)).ToString();
			yield return new WaitForSeconds(Time.deltaTime);
		}
		
		// Get this player and unlock him
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			if(player.networkView.isMine) {
				PlayerController controller = player.GetComponent<PlayerController>();
				controller.UnlockPlayer();
				break;
			}
		}
		
		countdown.text = "GO!";
		
		yield return new WaitForSeconds(1);
		sequenceComplete = true;
		Destroy (countdown);
	}
	
	IEnumerator SetPlayersReady() {
		GameObject[] players;
		do {
			players = GameObject.FindGameObjectsWithTag("Player");
			yield return new WaitForSeconds(0.1f);
		} while(players.Length < 2);
		
		for(int i=0; i<players.Length; i++) {
			players[i].transform.root.networkView.RPC("ReadyPlayer", RPCMode.All);
		}
	}
}