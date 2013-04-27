using UnityEngine;

public class RandomLevelPicker : MonoBehaviour {
  public static string RandomizedLevel() {
    int level = Random.Range(0, Constants.PLAYABLE_LEVELS.Length);
    return Constants.PLAYABLE_LEVELS[level];
  }
}