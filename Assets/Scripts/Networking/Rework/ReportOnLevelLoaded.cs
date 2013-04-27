using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReportOnLevelLoaded : MonoBehaviour {
  bool receivedConfirmation;
  
	void Start () {
    if(Network.isServer) {
      this.enabled = false;
    } else {
	    StartCoroutine("BeginReport");
    }
	}
	
  IEnumerator BeginReport() {
    while(!receivedConfirmation) {
      networkView.RPC("ReportIn", RPCMode.Server, Network.player);
      yield return new WaitForSeconds(0.3f);
    }
  }
  
  [RPC]
  public void ServerReceivedReport() {
    receivedConfirmation = true;
  }
}
