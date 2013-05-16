using UnityEngine;

public class SpellObjectForcePush : SpellSpecial {

  void Update() {
    if (activated && !hasRun) {
      hasRun = true;
      Run();
    }
  }

  void Run() {
    if (bundle.hitTarget) {
      SpellObjectMover mover = bundle.hitTarget.GetComponent<SpellObjectMover>();

      if (mover != null) {
        mover.Activate(bundle);
        Network.Destroy(gameObject);
      }
    }
  }

  void OnTriggerEnter(Collider other) {
    if (activated) {
      Transform root = other.transform.root;

      SpellObjectMover mover = root.GetComponent<SpellObjectMover>();

      if (mover != null) {
        mover.Activate(bundle);
        Network.Destroy(gameObject);
      }
    }
  }
}