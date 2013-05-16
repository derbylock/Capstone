using UnityEngine;
using System.Reflection;

public class SpellSpecial : MonoBehaviour {
  public int spellID;
  public bool isDormant;
  public string[] triggerOthers;  // List of other effects to trigger on activation (each should be isDormant)
  protected bool activated = false;
  public bool runOnce;
  protected bool hasRun;

  protected SpellBundle bundle;

  public void Activate(SpellBundle bundle) {
    activated = true;
    this.bundle = bundle;
  }

  //private GameObject[] targets;

  /*public void Activate(params GameObject[] targets) {
    activated = true;
    this.targets = targets;
  }*/
}
