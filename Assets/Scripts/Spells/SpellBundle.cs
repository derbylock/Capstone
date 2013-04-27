using UnityEngine;

public class SpellBundle {
  public Spell rootSpell;
  public GameObject caster;
  public Vector3 startPoint;
  public Vector3 endPoint;
  public GameObject hitTarget;
  public GameObject[] allTargets;

  public Vector3 GetFarthestPoint() {
    int mask = 1 << LayerMask.NameToLayer("Player");
    mask |= 1 << LayerMask.NameToLayer("Ignore Raycast");
    mask = ~mask;

    Vector3 temp;
    Vector3 direction = (endPoint - startPoint).normalized;
    RaycastHit hit;
    if (Physics.Raycast(startPoint, direction, out hit, rootSpell.m_maxDistance, mask)) {
      temp = hit.point;
    } else {
      temp = startPoint + direction * rootSpell.m_maxDistance;
    }
    return temp;
  }

  // Same as GetFarthestPoint except that it will return 
  public Vector3 GetFarthestPointGrounded(bool collideWithPlayer, ref bool success) {
    int mask = 0;
    if (!collideWithPlayer) {
      mask |= 1 << LayerMask.NameToLayer("Player");
    }
    mask |= 1 << LayerMask.NameToLayer("Ignore Raycast");
    mask = ~mask;

    Vector3 temp;
    Vector3 direction = (endPoint - startPoint).normalized;
    RaycastHit hit;
    if (Physics.Raycast(startPoint, direction, out hit, rootSpell.m_maxDistance, mask)) {
      temp = hit.point;
    } else {
      temp = startPoint + direction * rootSpell.m_maxDistance;
    }

    if (Physics.Raycast(temp, Vector3.down, out hit, 10000f, mask)) {
      success = true;
      temp = hit.point;
    } else {
      success = false;
    }
    return temp;
  }
  
  public Vector3 GetFarthestPointGrounded(bool collideWithPlayer) {
    int mask = 0;
    if (!collideWithPlayer) {
      mask |= 1 << LayerMask.NameToLayer("Player");
    }
    mask |= 1 << LayerMask.NameToLayer("Ignore Raycast");
    mask = ~mask;

    Vector3 temp;
    RaycastHit hit;

    if (Vector3.Distance(startPoint, endPoint) > rootSpell.m_maxDistance) {
      Vector3 direction = (endPoint - startPoint).normalized;

      if (Physics.Raycast(startPoint, direction, out hit, rootSpell.m_maxDistance, mask)) {
        temp = hit.point;
      } else {
        temp = startPoint + direction * rootSpell.m_maxDistance;
      }
    } else {
      temp = endPoint;
    }

    if (Physics.Raycast(temp, Vector3.down, out hit, 10000f, mask)) {
      temp = hit.point;
    }
    Debug.Log("FarthestGroundedPoint: " + temp);
    return temp;
  }
}