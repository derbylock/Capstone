using Unityengine;

public class SpellActivator : MonoBehaviour {
  public void Activate() {
    for (int i = 0; i < triggerOthers.Length; ++i) {
      SpellSpecial script = gameObject.GetComponent(triggerOthers[i]) as SpellSpecial;
      script.Activate();
    }
  }
}