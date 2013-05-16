using UnityEngine;

public class NetworkSerializationTest : MonoBehaviour {
  public float myLocalValue;
  private float myLastValue;

  void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
    if (stream.isWriting) {
      Debug.Log("Stream writing.");
      stream.Serialize(ref myLocalValue);
    } else {
      Debug.Log("Stream reading?");
      stream.Serialize(ref myLocalValue);
      Debug.Log(myLocalValue);
    }
  }

  void Start() {
    if (Network.isServer) {
      myLocalValue = 5;
    } else {
      myLocalValue = -1;
    }
  }

  void Update() {
    if (Network.isServer) {
      myLocalValue = Random.value * 10000000f;
    }

    if (myLocalValue != myLastValue) {
      Debug.Log("Value has changed");
    }

    myLastValue = myLocalValue;
  }
}