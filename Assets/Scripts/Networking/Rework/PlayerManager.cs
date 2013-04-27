using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {
  public List<PlayerData> activePlayers;
  private GameObject valuePool;
  
  void Start() {
    if (Network.isServer) {
      valuePool = GameObject.Find("Player Values");
      activePlayers = new List<PlayerData>();
      activePlayers.Add(GeneratePlayerData(Network.player));  // Host user

      // All connected users
      for (int i = 0; i < Network.connections.Length; ++i) {
        if (Network.connections[i] != null) {
          activePlayers.Add(GeneratePlayerData(Network.connections[i]));
        }
      }
    } else {
      networkView.RPC("GetMyAvatar", RPCMode.Server, Network.player);
    }
  }

  void Update() {
    for (int i = 0; i < activePlayers.Count; ++i) {
      if (activePlayers[i].team != -1) {
        ParseAndDistributeUpdates(activePlayers[i]);
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

  [RPC]
  void GetMyAvatar(NetworkPlayer player) {
    PlayerData target = FindPlayerData(player);
    GameObject[] avatars = GameObject.FindGameObjectsWithTag("Player");
    for (int i = 0; i < avatars.Length; ++i) {
      if (avatars[i].networkView.owner == player) {
        target.avatar = avatars[i];
        target.health = target.avatar.AddComponent<HealthRework>();
        target.mana = target.avatar.AddComponent<ManaRework>();
      }
    }
  }
  
  public void SetAvatar(NetworkPlayer player, GameObject avatar) {
    PlayerData data = FindPlayerData(player);
    data.avatar = avatar;
    data.health = avatar.AddComponent<HealthRework>();
    data.health.SetHealth(Constants.RSC_HEALTH_DEFAULT);
    data.mana = avatar.AddComponent<ManaRework>();
    data.mana.SetMana(Constants.RSC_MANA_DEFAULT);
    data.mana.SetCurrentMana(Constants.RSC_MANA_DEFAULT);
    data.mana.SetRegenRate(Constants.RSC_MANA_REGEN_DEFAULT);
  }
  
  PlayerData FindPlayerData(NetworkPlayer player) {
    for (int i = 0; i < activePlayers.Count; ++i) {
      if (activePlayers[i].Equals(player)) {
        return activePlayers[i];
      }
    }
    return null;
  }

  PlayerData GeneratePlayerData(NetworkPlayer player) {
    PlayerData newPlayer = new PlayerData();
    newPlayer.player = player;
    newPlayer.team = -1;
    return newPlayer;
  }

  void ParseAndDistributeUpdates(PlayerData player) {
    // Mana Updates
    int updatesList = player.mana.GetUpdates();
    if ((updatesList & Constants.RESOURCE_UPDATE_CURRENT) == Constants.RESOURCE_UPDATE_CURRENT) {
      valuePool.networkView.RPC("UpdatePlayerCurrentMana", RPCMode.Others, player.team, player.mana.GetCurrentMana(), Network.time);
    }
    if ((updatesList & Constants.RESOURCE_UPDATE_CURRENT) == Constants.RESOURCE_UPDATE_MAXIMUM) {
      valuePool.networkView.RPC("UpdatePlayerMaxMana", RPCMode.Others, player.team, player.mana.GetMaxMana(), Network.time);
    }
    if ((updatesList & Constants.RESOURCE_UPDATE_CURRENT) == Constants.RESOURCE_UPDATE_REGEN_RATE) {
      valuePool.networkView.RPC("UpdatePlayerManaRegen", RPCMode.Others, player.team, player.mana.GetRegenRate(), Network.time);
    }

    // Health Updates
    updatesList = player.health.GetUpdates();
    if ((updatesList & Constants.RESOURCE_UPDATE_CURRENT) == Constants.RESOURCE_UPDATE_CURRENT) {
      valuePool.networkView.RPC("UpdatePlayerHealth", RPCMode.Others, player.team, player.health.GetPercentage(), Network.time);
    }
  }
}