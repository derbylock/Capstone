using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class ReportOnLevelLoaded : MonoBehaviour {
  bool receivedConfirmation = false;

  ConsoleDebug debug;
  
	void Start () {
    debug = GameObject.FindObjectOfType(typeof(ConsoleDebug)) as ConsoleDebug;
    if (Network.isClient) {
      StartCoroutine("BeginReport");
      debug.DebugLine(networkView.viewID.ToString());
    } else {
      StartCoroutine("BeginTest");
      debug.DebugLine(networkView.viewID.ToString());
    }
	}

  IEnumerator BeginReport() {
    debug.DebugLine(networkView.viewID.ToString());
    while(!receivedConfirmation) {
      Debug.Log("Reporting");
      networkView.RPC("ReportToServer", RPCMode.Server);
      yield return new WaitForSeconds(0.1f);
    }
  }

  IEnumerator BeginTest() {
    while (true) {
      networkView.RPC("ReportToClient", Network.connections[0]);
      yield return new WaitForSeconds(0.1f);
    }
  }

  [RPC]
  void ReportToClient() {
    Debug.Log("Report reached client.");
  }

  [RPC]
  void ReportToServer(NetworkMessageInfo info) {
    Debug.Log("Report Reached The Server");
    LevelLoadedCounter llc = gameObject.GetComponent<LevelLoadedCounter>();
    llc.ReportIn(info.sender);
  }
  
  [RPC]
  public void ServerReceivedReport() {
    receivedConfirmation = true;
  }
}