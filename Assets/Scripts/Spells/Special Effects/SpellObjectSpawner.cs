
using UnityEngine;
using System.Collections;

public class SpellObjectSpawner : SpellSpecial {
  public GameObject toSpawn;
  public bool alignToHitNormal = true;
  public int count;
  public ObjectSpreadType objectSpread;
  public ObjectSpawnType objectSpawnRule;
  public bool surfaceFromUnderground;
  public float duration = 1f;
  public float multiSpawnStaggerTime;
  public float pivotOffset;
  public float offset;

  GameObject moveTarget;
  Vector3 startPosition;
  Vector3 endPosition;
  Quaternion rotation;

  void Update() {
    if (activated) {
      if (!runOnce || (runOnce && !hasRun)) {
        hasRun = true;
        Run();
      }
    }
  }

  void Run() {
    Vector3 spawnPoint;
    Vector3 hitNormal = Vector3.zero;
    /*if(alignToHitNormal) {
      spawnPoint = bundle.GetFarthestPointGrounded(true, out hitNormal);
      spawnPoint.y += pivotOffset;
      Vector3 dir = AdjustForwardToNormal(bundle.caster.transform.forward, hitNormal);
      Debug.Log(dir);
      rotation = Quaternion.FromToRotation(Vector3.forward, dir);
      Debug.Log(rotation);
    } else {
      spawnPoint = bundle.GetFarthestPointGrounded(true);
      spawnPoint.y += pivotOffset;
      rotation = bundle.caster.transform.rotation;
    }*/

    spawnPoint = bundle.GetFarthestPointGrounded(true);
    spawnPoint.y += pivotOffset;
    rotation = bundle.caster.transform.rotation;

    /*if (alignToHitNormal) {
      Vector3 dir = AdjustForwardToNormal(bundle.caster.transform.forward, hitNormal);
      Debug.Log(dir);
      rotation = Quaternion.FromToRotation(bundle.caster.transform.forward, dir);
    } else {
      rotation = bundle.caster.transform.rotation;
    }*/

    //rotation = (alignToHitNormal ? Quaternion.FromToRotation(Vector3.up, hitNormal) : bundle.caster.transform.rotation);
    spawnPoint += rotation * new Vector3(0f, pivotOffset, 0f);
    moveTarget = Network.Instantiate(toSpawn, spawnPoint, rotation, 0) as GameObject;
    if(surfaceFromUnderground) {
      //endPosition = spawnPoint;
      //float height = moveTarget.collider.bounds.extents.y + pivotOffset;
      //moveTarget.transform.position = new Vector3(moveTarget.transform.position.x, moveTarget.transform.position.y - height, moveTarget.transform.position.z);
      StartCoroutine ("Move");
    }

    ApplyDamageOnTrigger[] activatorsA = moveTarget.GetComponents<ApplyDamageOnTrigger>();
    foreach (ApplyDamageOnTrigger a in activatorsA) {
      a.Activate(bundle);
    }
    ApplyDamageOnTrigger[] activatorsB = moveTarget.GetComponentsInChildren<ApplyDamageOnTrigger>();
    foreach (ApplyDamageOnTrigger b in activatorsB) {
      b.Activate(bundle);
    }
  }
  
  IEnumerator Move() {
    endPosition = moveTarget.transform.position;
    startPosition = endPosition;
    startPosition -= rotation * new Vector3(0f, offset, 0f);
    Debug.Log("Start: " + startPosition + " End: " + endPosition);
    moveTarget.transform.position = startPosition;
    float startTime = Time.time;
    //while (moveTarget.transform.position != endPosition) {
    while (Time.time - startTime < duration) {
      Debug.Log ("Moving Object");
      moveTarget.transform.position = Vector3.Lerp(startPosition, endPosition, (Time.time - startTime)/duration);
      yield return new WaitForSeconds(Time.deltaTime);
    }
    Network.Destroy(gameObject);
  }

  /*IEnumerator RunMultiple() {
    for(int i = 0; i < count; ++i) {
      Vector3 spawnPoint = SetSpawnLocation();
      yield return new WaitForSeconds(multiSpawnStaggerTime);
    }
  }

  void Spawn(int spawnNumber) {
    Vector3 spawnLocation;
    if (objectSpawnRule == ObjectSpawnType.RequireGroundStrict) {
      bool successful = false;
      spawnLocation = bundle.GetFarthestPointGrounded(true, ref successful);
      if (!successful) {
        RaycastHit hit;
        if(Physics.Raycast(bundle.startPoint, Vector3.down, out hit, 1000f, ~(1 << LayerMask.NameToLayer("Player")))) {
          
        }
        spawnLocation = bundle.startPoint
      }
    } else if (objectSpawnRule == ObjectSpawnType.RequireGroundFlexible) {
      
    } else {
      
    }
  }
  
  Vector3 SetSpawnLocation() {
    switch(objectSpawnRule) {
      case ObjectSpawnType.NoRequirement:
      case ObjectSpawnType.RequireGroundStrict:
      case ObjectSpawnType.RequireGroundFlexible:
    }
  }*/

  private Vector3 AdjustForwardToNormal(Vector3 forward, Vector3 groundNormal) {
    Vector3 sideways = Vector3.Cross(Vector3.up, forward);
    return Vector3.Cross(sideways, groundNormal).normalized * forward.magnitude;
  }
}