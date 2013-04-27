using UnityEngine;

public class TargettedSpell : Spell {
  public bool requirePlayerHit;

  public override bool WillCastSuccessfully(Vector3 castFrom, Vector3 castTo) {
    if (requirePlayerHit) {
      int mask = 1 << LayerMask.NameToLayer("Player");
      return (Vector3.Distance(castFrom, castTo) <= m_maxDistance &&
              Physics.Raycast(castFrom, (castTo - castFrom).normalized, 1000f, mask));
    }
    return Vector3.Distance(castFrom, castTo) <= m_maxDistance;
  }

  public override void Cast(Vector3 castFrom, Vector3 castTo) {
    base.Cast (castFrom, castTo);

    RaycastHit hit;

    if (Physics.Raycast(castFrom, (castTo - castFrom).normalized, out hit, m_maxDistance)) {
      if (hit.transform.root.tag == "Player") {
        bundle.hitTarget = hit.transform.root.gameObject;
      }

      SpellActivator activator = gameObject.GetComponent<SpellActivator>();
      if (activator) {
        activator.Activate(bundle);
      }
    }
  }
}