using UnityEngine;

public class InputReader {
  public static bool GetKeysDownOr(params KeyCode[] keys) {
    for(int i=0; i<keys.Length; ++i) {
      if(keys[i] != null && Input.GetKeyDown(keys[i])) {
        return true;
      }
    }
    return false;
  }

  public static bool GetKeysDownOr(params string[] keys) {
    for (int i = 0; i < keys.Length; ++i) {
      if (keys[i] != null && Input.GetKeyDown(keys[i])) {
        return true;
      }
    }
    return false;
  }

  public static bool GetKeysDownAnd(params KeyCode[] keys) {
    for (int i = 0; i < keys.Length; ++i) {
      if (keys[i] != null && !Input.GetKeyDown(keys[i])) {
        return false;
      }
    }
    return true;
  }

  public static bool GetKeysDownAnd(params string[] keys) {
    for (int i = 0; i < keys.Length; ++i) {
      if (keys[i] != null && !Input.GetKeyDown(keys[i])) {
        return false;
      }
    }
    return true;
  }

  public static Vector2 GetInputVector2(string x, string y) {
    return new Vector2(Input.GetAxis(x), Input.GetAxis(y));
  }
}
