
using UnityEngine;
using System.Collections;

public class SpellObjectSpawner : SpellSpecial {
  public GameObject toSpawn;
  public int count;
  public ObjectSpreadType objectSpread;
  public ObjectSpawnType objectSpawnRule;
  public bool surfaceFromUnderground;
  public float speed;
  public float multiSpawnStaggerTime;
  public float pivotOffset;

  GameObject moveTarget;
  Vector3 endPosition;

  void Update() {
    if (activated) {
      if (!runOnce || (runOnce && !hasRun)) {
        hasRun = true;
        Run();
      }
    }
  }

  void Run() {
    Vector3 spawnPoint = bundle.GetFarthestPointGrounded(true);
    Debug.Log(bundle != null);
    moveTarget = Network.Instantiate(toSpawn, spawnPoint, bundle.caster.transform.rotation, 0) as GameObject;
    if(surfaceFromUnderground) {
      endPosition = moveTarget.transform.position;
      float height = moveTarget.collider.bounds.extents.y + pivotOffset;
      moveTarget.transform.position = new Vector3(moveTarget.transform.position.x, moveTarget.transform.position.y - height, moveTarget.transform.position.z);
      StartCoroutine ("Move");
    }
  }
  
  IEnumerator Move() {
    Vector3 startPosition = moveTarget.transform.position;
    endPosition = new Vector3(startPosition.x,
                              startPosition.y + moveTarget.collider.bounds.size.y,
                              startPosition.z);
    float startTime = Time.time;
    float dur = .1f;
    //while (moveTarget.transform.position != endPosition) {
    while (Time.time - startTime < dur) {
      moveTarget.transform.position = Vector3.Lerp(startPosition, endPosition, (Time.time - startTime)/dur);
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
}