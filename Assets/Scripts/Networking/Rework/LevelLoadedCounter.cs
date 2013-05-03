using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[RequireComponent(typeof(NetworkView))]
public class LevelLoadedCounter : MonoBehaviour {
  private List<NetworkPlayer> loaded = new List<NetworkPlayer>();
  public string scriptToAlert;
  
  /*void Start() {
    if(!Network.isServer) {
      this.enabled = false;
    }
  }*/
  
	void Update () {
    Debug.Log("I'm running, I swear");
	  if(loaded.Count == Network.connections.Length) {
      Debug.Log("All players reported in");
      Type type = Type.GetType(scriptToAlert);
      MethodInfo method = type.GetMethod("Begin");
      
      UnityEngine.Object script = GameObject.FindObjectOfType(type);
      if(script != null) {
        method.Invoke(script, null);
      } else {
        Debug.LogError ("LevelLoadedCounter could not find script of type " + type +
                        "; please ensure it is properly entered and that the script is in the scene."); 
      }
    }
	}
  
  [RPC]
  public void ReportIn(NetworkPlayer player) {
    Debug.Log("Received Report");
    if(loaded.IndexOf(player) == -1) {
      loaded.Add (player);
      networkView.RPC ("ServerReceivedReport", player);
    }
  }
}