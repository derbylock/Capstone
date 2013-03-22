using UnityEngine;

public class BitwiseTest : MonoBehaviour {
  void Start() {
    int setBit = 0x8;
    Debug.Log("SetBit: " + setBit);
    setBit |= 0x2;
    Debug.Log("SetBit OR: " + setBit);
    Debug.Log((setBit & 0x4) == 0x4);
    setBit &= ~0x8;
    Debug.Log("SetBit ~: " + setBit);
  }
}