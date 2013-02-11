using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class SetPlayerStart : MonoBehaviour {
	private GameObject[] startpoints;
	
	private int teamNum;
	
	void Start() {
		startpoints = GameObject.FindGameObjectsWithTag("Player Start Position");
	}
	
	[RPC]
	public void ReadyPlayer() {
		LockPlayer();
		MovePlayerToTeamSpawn();
		OrientPlayerToOpponent();
	}
	
	[RPC]
	void LockPlayer() {
		PlayerController controller = gameObject.GetComponent<PlayerController>();
		controller.LockPlayer();
	}
	
	void MovePlayerToTeamSpawn() {
		Team team = transform.GetComponent<Team>();
		teamNum = team.m_teamNumber;
		GameObject startAt = GameObject.Find ("Start Point " + teamNum);
		transform.position = startAt.transform.position;
	}
	
	void OrientPlayerToOpponent() {
		// Start Point should be the opposite of the team number
		GameObject otherSpawn = GameObject.Find ("Start Point " + (teamNum==0 ? 1 : 0));
		Vector3 oppSpawnPosition = otherSpawn.transform.position;
		oppSpawnPosition.y = transform.position.y;	// Don't give them a weird orientation on uneven ground
		transform.LookAt(oppSpawnPosition);
	}
}
