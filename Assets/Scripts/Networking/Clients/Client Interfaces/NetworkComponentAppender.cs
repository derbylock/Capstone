using UnityEngine;
using System;
using System.Collections.Generic;

public class NetworkComponentAppender : MonoBehaviour {
	[RPC]
	void AddNetworkComponent(string typeName, NetworkViewID viewID) {
		NetworkView netView = gameObject.AddComponent<NetworkView>();
		netView.viewID = viewID;
		netView.observed = gameObject.AddComponent(typeName);
	}
	
	[RPC]
	void AddServerComponent(string typeName) {
		NetworkView netView = gameObject.AddComponent<NetworkView>();
		NetworkViewID viewID = Network.AllocateViewID();
		netView.viewID = viewID;
		netView.observed = gameObject.AddComponent(typeName);
		
		networkView.RPC("AddNetworkComponent", RPCMode.OthersBuffered, typeName, viewID);
	}
}