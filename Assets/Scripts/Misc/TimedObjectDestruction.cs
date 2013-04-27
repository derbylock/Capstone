using UnityEngine;

public class TimedObjectDestruction : MonoBehaviour {
  public float destroyTime;
  private float instantiatedAt;

  void Start() {
    instantiatedAt = Time.time;
  }

  void Update() {
    if (Time.time - instantiatedAt > destroyTime) {
      Destroy(gameObject);
    }
  }
}