using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {
  public List<PlayerData> activePlayers;
  
  void Start() {
    if(Network.isServer) {
      activePlayers = new List<PlayerData>();
      activePlayers.Add(GeneratePlayerData(Network.player));  // Host user

      // All connected users
      for (int i = 0; i < Network.connections.Length; ++i) {
        if (Network.connections[i] != null) {
          activePlayers.Add(GeneratePlayerData(Network.connections[i]));
        }
      }
    }
  }

  void OnPlayerConnected(NetworkPlayer player) {
    activePlayers.Add(GeneratePlayerData(player));
  }

  void OnPlayerDisconnected(NetworkPlayer player) {
    for (int i = 0; i < activePlayers.Count; ++i) {
      if (activePlayers[i].Equals(player)) {
        activePlayers.RemoveAt(i);
        break;
      }
    }
  }

  PlayerData GeneratePlayerData(NetworkPlayer player) {
    PlayerData newPlayer = new PlayerData();
    newPlayer.player = player;
    newPlayer.team = -1;
    return newPlayer;
  }
}