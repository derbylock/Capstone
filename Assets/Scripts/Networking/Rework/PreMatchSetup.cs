using UnityEngine;
using System;
using System.Collections;

public class PreMatchSetup : MonoBehaviour {
  ReadyCheckMonitor rCheckMonitor;
  public MatchSetupState state;

  public void Begin() {
    //StartCoroutine("RunSetup");
  }

  IEnumerator RunSetup() {
    rCheckMonitor = new ReadyCheckMonitor();

    while (!rCheckMonitor.AllReady()) {
      yield return new WaitForSeconds(0.1f);


    }
  }

  [RPC]
  public void UserReady(NetworkPlayer player, bool ready) {
    rCheckMonitor.SetPlayerReadyStatus(player, ready);
  }

  [RPC]
  void SetState(string state) {
    this.state = (MatchSetupState)Enum.Parse(typeof(MatchSetupState), state);

    switch (this.state) {
      case MatchSetupState.SpawnPlayers:
        StartCoroutine("WaitForCharacterSpawn");
        break;
      case MatchSetupState.SetPlayerInPosition:
        StartCoroutine("WaitForPlayerPositionSet");
        break;
      default:
        break;
    }
  }

  IEnumerator WaitForCharacterSpawn() {
    GameObject myChar = null;
    do {
      yield return new WaitForSeconds(0.1f);

      GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
      foreach (GameObject player in players) {
        if (player.transform.root.networkView.owner == Network.player) {
          myChar = player;
        }
      }
    } while (myChar == null);

    ReplyReady();
  }

  IEnumerator WaitForPlayerPositionSet() {
    yield return null;
  }

  void ReplyReady() {
    if (Network.isServer) {
      UserReady(Network.player, true);
    } else {
      networkView.RPC("UserReady", RPCMode.Server, Network.player, true);
    }
  }
}