using UnityEngine;

public class CameraController : MonoBehaviour {
  public float hAngle;
  public float hSpeed = 200;
  public float vAngle;
  public float vSpeed = 300f;
  public float vMinAngle = -80f;
  public float vMaxAngle = 80f;
  public float sensitivity = 0.3f;
  
  float maxCameraDistance = 1f;
  
  public float damping = 5f;
  public float smoothSpeed = 10f;
  
  public Transform player;
  public Transform aimTarget;
  public bool rotateTarget;
  
  public Vector3 pivotOffset;   // Offset from the target's pivot point
  public Vector3 cameraOffset;  // Offset from pivot offset
  public Vector3 nearestOffset; // Offset for when the camera is nearest to the player
  public float maxDistance;
  
  private Vector3 smoothedPlayerPosition;
  
  private int collisionMask;
  
  void Start() {
    collisionMask = 1 << LayerMask.NameToLayer("Player");
    collisionMask |= 1 << LayerMask.NameToLayer("Ignore Raycast");
    collisionMask = ~collisionMask;
    
    GameObject aim = new GameObject();
    aimTarget = aim.transform;
  }
  
  void LateUpdate() {
    if(Time.deltaTime == 0f || Time.timeScale == 0f || player == null) {
      return;
    }
    
    float previousAimDistance = (aimTarget.position - transform.position).magnitude;
    
    hAngle += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * hSpeed * sensitivity * Time.deltaTime;
    vAngle += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * vSpeed * sensitivity * Time.deltaTime;
    vAngle = Mathf.Clamp(vAngle, vMinAngle, vMaxAngle);
    
    Quaternion cameraRotation = Quaternion.Euler(-vAngle, hAngle, 0f); // Needed later as well
    transform.rotation = cameraRotation;
    
    Quaternion horizontalRotation = Quaternion.Euler(0f, hAngle, 0f);
    
    smoothedPlayerPosition = Vector3.Lerp(smoothedPlayerPosition, player.position, smoothSpeed * Time.deltaTime);
    smoothedPlayerPosition.x = player.position.x;
    smoothedPlayerPosition.z = player.position.z;
    
    // Use flat rotation for the pivot offset because we are already rotated
    // in the proper spot along the other axes (sp?).
    Vector3 farPosition = smoothedPlayerPosition + cameraRotation * cameraOffset + horizontalRotation * pivotOffset;
    Vector3 nearPosition = player.position + cameraRotation * nearestOffset + horizontalRotation * pivotOffset;
    float pointDistance = Vector3.Distance(farPosition, nearPosition);
    Vector3 nearToFar = (farPosition - nearPosition) / pointDistance;
    
    maxCameraDistance = Mathf.Lerp(maxCameraDistance, pointDistance, damping * Time.deltaTime);
    
    RaycastHit hit;
    if(Physics.Raycast(nearPosition, nearToFar, out hit, maxCameraDistance + 0.5f, collisionMask)) {
      maxCameraDistance = hit.distance - 0.5f;
    }
    
    transform.position = nearPosition + nearToFar * maxCameraDistance;
    
    float aimTargetDistance;
    if(Physics.Raycast(transform.position, transform.forward, out hit, 100f, collisionMask)) {
      aimTargetDistance = hit.distance + 0.05f;
    } else {
      aimTargetDistance = Mathf.Max(5, previousAimDistance);
    }
    
    aimTarget.position = transform.position + transform.forward * aimTargetDistance;
		
    if(rotateTarget) {
      player.root.rotation = Quaternion.Euler(new Vector3(0.0f, transform.rotation.eulerAngles.y, 0.0f));
    }
  }
  
  public void AssignTarget(Transform target) {
    this.player = target;
    smoothedPlayerPosition = target.position;
  }
}