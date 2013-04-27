using UnityEngine;

public class SpellActivator : MonoBehaviour {
  public string[] activateOnTrigger;


  public void Activate(SpellBundle bundle) {
    SpellSpecial toActivate;
    for(int i=0; i<activateOnTrigger.Length; ++i) {
      toActivate = gameObject.GetComponent(activateOnTrigger[i]) as SpellSpecial;
      if(toActivate != null) {
        toActivate.Activate(bundle);
      } else {
        Debug.LogError("Missing Component: " + activateOnTrigger[i] + " On object: " + transform.root.name);
      }
    }
    
    Destroy(this);
  }

  /*public void Activate(params GameObject[] targets) {
    SpellSpecial toActivate;
    for(int i=0; i<activateOnTrigger.Length; ++i) {
      toActivate = gameObject.GetComponent(activateOnTrigger[i]) as SpellSpecial;
      if(toActivate != null) {
        toActivate.Activate(targets);
      } else {
        Debug.LogError("Missing Component: " + activateOnTrigger[i] + " On object: " + transform.root.name);
      }
    }
    
    Destroy(this);
  }*/
}