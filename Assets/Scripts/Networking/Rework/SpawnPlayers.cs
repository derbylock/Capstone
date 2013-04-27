using UnityEngine;
using System.Collections;

public class SpawnPlayers : MonoBehaviour {

	public void Begin() {
    networkView.RPC("SpawnSelf", RPCMode.All);
  }
  
  public void SpawnSelf() {
    CharacterSpawnData data = GameObject.FindObjectOfType(typeof(CharacterSpawnData)) as CharacterSpawnData;
    GameObject character = Resources.Load("PlayableCharacters\\" + data.GetSpawnName(), typeof(GameObject)) as GameObject;
    GameObject spawned = Network.Instantiate(character, transform.position, Quaternion.identity, 0) as GameObject;
    CharacterRegistration register = spawned.GetComponent<CharacterRegistration>();
    register.BeginRegister();
  }
}