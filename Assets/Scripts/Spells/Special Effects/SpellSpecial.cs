using UnityEngine;
using System.Reflection;

public class SpellSpecial : MonoBehaviour {
  public bool isDormant;
  public string[] triggerOthers;  // List of other effects to trigger on activation (each should be isDormant)
  private bool activated = false;

  protected virtual void Activate() {
    if(!activated) {
      activated = true;
      for (int i = 0; i < triggerOthers.Length; ++i) {
        SpellSpecial script = gameObject.GetComponent(triggerOthers[i]) as SpellSpecial;
        script.enabled = true;
      }
    }
  }
}
