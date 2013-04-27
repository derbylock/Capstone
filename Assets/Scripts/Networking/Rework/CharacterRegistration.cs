using UnityEngine;
using System.Collections;

public class CharacterRegistration : MonoBehaviour {
  public void BeginRegister() {
    if(Network.isServer) {
      Register(Network.player);
    } else {
     networkView.RPC ("Register", RPCMode.Server, Network.player); 
    }
  }
  
  [RPC]
  public void Register(NetworkPlayer player) {
    PlayerManager manager = GameObject.FindObjectOfType(typeof(PlayerManager)) as PlayerManager;
    manager.SetAvatar(player, transform.root.gameObject);
  }
}