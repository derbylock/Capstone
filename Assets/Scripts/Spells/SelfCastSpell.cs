using UnityEngine;

public class SelfCastSpell : Spell {

  public override void Cast(Vector3 castFrom, Vector3 castTo) {
    base.Cast (castFrom, castTo);
    ApplyEffects();
  }

  public void ApplyEffects() {
    SpellActivator activator = gameObject.GetComponent<SpellActivator>();

    activator.Activate(bundle);
  }
}