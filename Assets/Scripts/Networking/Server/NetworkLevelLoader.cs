using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class NetworkLevelLoader : MonoBehaviour {
	[RPC]
	public void LoadLevel(string levelName) {
		StartCoroutine("Load", levelName);
	}

  IEnumerator Load(string levelName) {
    // Disable any outgoing network traffic
    Network.SetSendingEnabled(0, false);
    Network.isMessageQueueRunning = false;

    Application.LoadLevel(levelName);
    yield return null;
    yield return null;

    // Enable network traffic sending
    Network.isMessageQueueRunning = true;
    Network.SetSendingEnabled(0, true);
  }
}
