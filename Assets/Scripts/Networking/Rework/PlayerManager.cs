using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {
  public PlayerData[] activePlayers;
  
  void Start() {
    if(Network.isServer) {
      
    }
  }
}
