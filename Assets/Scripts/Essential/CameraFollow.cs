using UnityEngine;
using System.Collections;

public class CameraFollow :MonoBehaviour {
  public Transform target;
  public bool rotateTarget;

  public float distance;
  public Vector3 offset;  // Offset from the target's transform pivot point

  public float xSpeed;
  public float ySpeed;

  public float yMin;
  public float yMax;
  private float xScroll;
  private float yScroll;


  void Start() {
    xScroll = target.rotation.eulerAngles.x;
    yScroll = target.rotation.eulerAngles.y;
  }

  void Update() {
    if (target) {
      xScroll += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
      yScroll += Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

      yScroll = ClampCircular(yScroll, yMin, yMax);
    }
  }

  void LateUpdate() {
    Debug.Log("Working");
    if (target != null) {
      Debug.Log("Has Target");
      // Get our new Quaternion rotation
      Quaternion rotation = Quaternion.Euler(yScroll, xScroll, 0);

      transform.rotation = rotation;
      if (rotateTarget) {
        target.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
      }

      Vector3 targetPosition = target.position + offset; // Cached to avoid multiple calculations
      Vector3 expectedPosition = rotation * new Vector3(0f, 0f, -distance) + target.position;

      // Find anything blocking the way to the target
      RaycastHit[] hits = Physics.SphereCastAll(targetPosition,
                                                0.5f,
                                                expectedPosition - targetPosition,
                                                distance);
                                                //(1 << LayerMask.NameToLayer("Player")));

      if (hits != null) {
        float closestHit = distance;
        for (int i = 0; i < hits.Length; ++i) {
          if (hits[i].transform.root.tag != "Player" && hits[i].distance < closestHit) {
            closestHit = hits[i].distance;
          }
        }

        if (closestHit < distance) {
          expectedPosition = rotation * new Vector3(0f, 0f, -closestHit) + targetPosition;
        }
      }

      expectedPosition += transform.TransformDirection(offset);
      transform.position = expectedPosition;
    }
  }

  float ClampCircular(float angle, float min, float max) {
    // Keep between -/+ 360
    if (angle < -360) {
      angle += 360;
    } else if (angle > 360) {
      angle -= 360;
    }

    return Mathf.Clamp(angle, min, max);
  }
}
