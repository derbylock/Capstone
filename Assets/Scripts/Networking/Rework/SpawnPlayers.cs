using UnityEngine;
using System.Collections;

public class SpawnPlayers : MonoBehaviour {
  ReadyCheckMonitor rCheckMonitor;
  bool started;

	public void Begin() {
    rCheckMonitor = new ReadyCheckMonitor();
    started = true;
    networkView.RPC("SpawnSelf", RPCMode.All);
  }

  void Update() {
    if (started && rCheckMonitor.AllReady()) {

    }
  }
  
  public void SpawnSelf() {
    CharacterSpawnData data = GameObject.FindObjectOfType(typeof(CharacterSpawnData)) as CharacterSpawnData;
    GameObject character = Resources.Load("PlayableCharacters\\" + data.GetSpawnName(), typeof(GameObject)) as GameObject;
    GameObject spawned = Network.Instantiate(character, transform.position, Quaternion.identity, 0) as GameObject;
    CharacterRegistration register = spawned.GetComponent<CharacterRegistration>();
    register.BeginRegister();
  }

  void ReportReady() {
    if (Network.isServer) {
      SetPlayerReady(Network.player);
    } else {
      networkView.RPC("SetPlayerReady", RPCMode.Server, Network.player);
    }
  }

  [RPC]
  void SetPlayerReady(NetworkPlayer player) {
    rCheckMonitor.SetPlayerReadyStatus(player, true);
  }
}