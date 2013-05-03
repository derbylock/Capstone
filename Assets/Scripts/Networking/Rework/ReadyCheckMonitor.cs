using UnityEngine;
using System.Collections.Generic;

public class ReadyCheckMonitor {
  private List<ReadyCheck> playerRCs = new List<ReadyCheck>();

  public ReadyCheckMonitor() {
    InitWithPlayers();
  }

  public void InitWithPlayers() {
    if (Network.isServer) {
      playerRCs.Add(new ReadyCheck(Network.player));
      for (int i = 0; i < Network.connections.Length; ++i) {
        playerRCs.Add(new ReadyCheck(Network.connections[i]));
      }
    }
  }

  public void SetPlayerReadyStatus(NetworkPlayer player, bool status) {
    ReadyCheck rCheck = playerRCs.Find(x => x.player == player);
    rCheck.status = status;
  }

  public bool AllReady() {
    for (int i = 0; i < playerRCs.Count; ++i) {
      if (playerRCs[i].status == false) {
        return false;
      }
    }

    return true;
  }
}