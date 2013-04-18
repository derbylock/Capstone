using UnityEngine;
using System.Reflection;

public class SpellSpecial : MonoBehaviour {
  public bool isDormant;
  public string[] triggerOthers;  // List of other effects to trigger on activation (each should be isDormant)
  protected bool activated = false;
  public bool runOnce;
  protected bool hasRun;

  public void Activate() {
    activated = true;
  }
}
