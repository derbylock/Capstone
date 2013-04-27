using UnityEngine;
using System.Collections;

public class CharacterSpawnData : MonoBehaviour {
  CharacterData spawnData;
  string spawnName;
 
  void Awake() {
    DontDestroyOnLoad(gameObject);
  }
  public void SelectCharacter(CharacterData data) {
    spawnData = data;
    spawnName = data.transform.root.name;
  }
  
  public string GetSpawnName() {
    return spawnName;
  }
}
