using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class WinController : MonoBehaviour {
	public string m_levelToLoad = "";
	private bool m_matchBegin = false;
	public GameObject m_playerManager;
	private PlayerDataManager m_manager;	// Cache of the manager script for quick access
	
	public GameObject guiObject;	// Show final results to the player
	
	// Use this for initialization
	void Start () {
		networkView.observed = this;
		m_manager = m_playerManager.GetComponent<PlayerDataManager>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Network.isServer) {
			if(m_matchBegin) {
				// Iterate through both teams, assuming that the team is dead
				int teamKilled = 3;
				for(int i=0; i<m_manager.m_teamOne.Length; i++) {
					if(m_manager.m_teamOne[i] && m_manager.m_teamOne[i].m_health.IsAlive) {
						teamKilled -= 1;
						break;
					}
				}
				for(int i=0; i<m_manager.m_teamTwo.Length; i++) {
					if(m_manager.m_teamTwo[i] && m_manager.m_teamTwo[i].m_health.IsAlive) {
						teamKilled -= 2;
						break;
					}
				}
				
				switch(teamKilled) {
				case 0:
					// Both teams are still alive
					break;
				case 1:
					// Team 1 has been killed
					Debug.Log ("MATCH END");
					Debug.Log ("Team 1 Lost");
					m_matchBegin = false;
					Debug.Log ("Lose Messages Being Sent Out");
	                PropagateResultMessages(m_manager.m_teamOne, -1);
					Debug.Log ("Win Messages Being Sent Out");
					PropagateResultMessages(m_manager.m_teamTwo, 1);
	                break;
	            case 2:
					// Team 2 has been killed
					Debug.Log ("MATCH END");
					m_matchBegin = false;
					Debug.Log ("Win Messages Being Sent Out");
					PropagateResultMessages(m_manager.m_teamOne, 1);
					Debug.Log ("Lose Messages Being Sent Out");
					PropagateResultMessages(m_manager.m_teamTwo, -1);
	                break;
	            case 3:
					// Both teams were killed
					Debug.Log ("MATCH END");
					m_matchBegin = false;
					Debug.Log ("Draw Messages Being Sent Out");
					PropagateResultMessages(m_manager.m_teamOne, 0);
					Debug.Log ("Draw Messages Being Sent Out");
					PropagateResultMessages(m_manager.m_teamTwo, 0);
	                break;
				}
			}
		}
	}
	
	public void BeginMatch() {
		m_matchBegin = true;
	}
	
	/// <summary>
	/// Sends out the result messages based on the teamResult.
	/// </summary>
	/// <param name="teamResult">
	/// Lose = -1;
	/// Tie = 0;
	/// Win = 1;
	/// </param>
	void PropagateResultMessages(PlayerContainer[] players, int teamResult) {
		switch(teamResult) {
		case -1:
			for(int i=0; i<players.Length; i++) {
				if(players[i] != null && players[i].m_player != Network.player) {
					Debug.Log ("Message sent to: " + players[i].m_player);
					networkView.RPC("InitLoseSequence", players[i].m_player);
				} else if (players[i] != null && players[i].m_player == Network.player) {
					Debug.Log ("Message sent to: server");
					StartCoroutine("ClosingSequence", "YOU LOSE!");
				}
			}
			/*foreach(PlayerContainer p in players) {
				networkView.RPC ("InitLoseSequence", p.m_player);
			}*/
			break;
		case 0:
			for(int i=0; i<players.Length; i++) {
				if(players[i] != null && players[i].m_player != Network.player) {
					Debug.Log ("Message sent to: " + players[i].m_player);
					networkView.RPC("InitTieSequence", players[i].m_player);
				} else if (players[i] != null && players[i].m_player == Network.player) {
					Debug.Log ("Message sent to: server");
					StartCoroutine("ClosingSequence", "DRAW!");
				}
			}
			/*foreach(PlayerContainer p in players) {
				networkView.RPC ("InitTieSequence", p.m_player);
			}*/
			break;
		case 1:
			for(int i=0; i<players.Length; i++) {
				if(players[i] != null && players[i].m_player != Network.player) {
					Debug.Log ("Message sent to: " + players[i].m_player);
					networkView.RPC("InitWinSequence", players[i].m_player);
				} else if (players[i] != null && players[i].m_player == Network.player) {
					Debug.Log ("Message sent to: server");
					StartCoroutine("ClosingSequence", "YOU WIN!");
				}
			}
			/*foreach(PlayerContainer p in players) {
				networkView.RPC ("InitWinSequence", p.m_player);
			}*/
			break;
		}
		
		Debug.Log ("Messages Sent");
	}
	
	[RPC]
	void InitWinSequence() {
		StartCoroutine("ClosingSequence", "YOU WIN!");
	}
	
	[RPC]
	void InitLoseSequence() {
		StartCoroutine("ClosingSequence", "YOU LOSE!");
	}
	
	[RPC]
	void InitTieSequence() {
		StartCoroutine("ClosingSequence", "DRAW!");
	}
	
	IEnumerator ClosingSequence(string msg) {
		guiObject.guiText.text = msg;
		yield return new WaitForSeconds(3);
		/*float i=1f;
		while(i>0) {
			guiObject.renderer.material.color = new Color(0,0,0,i);
			i -= Time.deltaTime;
			yield return new WaitForSeconds(Time.deltaTime);
		}*/
		guiObject.guiText.text = "";
		Network.Disconnect();
		Screen.showCursor = true;
		Screen.lockCursor = false;
		Application.LoadLevel(m_levelToLoad);
		//Application.LoadLevel("Match Finder");
	}
}