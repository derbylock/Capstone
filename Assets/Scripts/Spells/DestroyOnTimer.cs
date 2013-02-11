using UnityEngine;

public class DestroyOnTimer : MonoBehaviour {
  public float destroyAfter;
  private float currentTime;

  void Start() {
    currentTime = 0f;
  }

  void FixedUpdate() {
    currentTime += Time.fixedDeltaTime;
    if(currentTime > destroyAfter) {
      Destroy(gameObject);
    }
  }
}