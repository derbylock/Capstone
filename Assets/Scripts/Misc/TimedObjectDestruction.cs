using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class TimedObjectDestruction : MonoBehaviour {
  public bool serverOnlyDestructor;
  public float destroyTime;
  private float instantiatedAt;

  void Start() {
    if (serverOnlyDestructor && !Network.isServer) {
      Destroy(this);
    }
    instantiatedAt = Time.time;
  }

  void Update() {
    if (Time.time - instantiatedAt > destroyTime) {
      Network.Destroy(transform.root.gameObject);
    }
  }
}