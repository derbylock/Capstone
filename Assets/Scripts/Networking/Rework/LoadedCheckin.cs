using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class LoadedCheckin : MonoBehaviour {
  public bool receivedConfirmation = false;
  ConsoleDebug debug;

  void Start() {
    debug = GameObject.FindObjectOfType(typeof(ConsoleDebug)) as ConsoleDebug;

    if (Network.isClient) {
      StartCoroutine("BeginReport");
    }
  }

  IEnumerator BeginReport() {
    while (!receivedConfirmation) {
      Debug.Log("Reporting");
      networkView.RPC("ReportToServer", RPCMode.Server);
      yield return new WaitForSeconds(0.1f);
    }
  }

  /*IEnumerator BeginTest() {
    while (true) {
      networkView.RPC("ReportToClient", Network.connections[0]);
      yield return new WaitForSeconds(0.1f);
    }
  }*/

  [RPC]
  void ReportToClient() {
    Debug.Log("Report reached client.");
  }

  [RPC]
  void ReportToServer(NetworkMessageInfo info) {
    Debug.Log("Report Reached The Server");
    networkView.RPC("ServerReceivedReport", info.sender);
    LevelLoadedCounter llc = gameObject.GetComponent<LevelLoadedCounter>();
    llc.ReportIn(info.sender);
  }

  [RPC]
  public void ServerReceivedReport() {
    receivedConfirmation = true;
  }
}