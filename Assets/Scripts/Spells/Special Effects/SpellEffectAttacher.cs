using UnityEngine;

public class SpellEffectAttacher : SpellSpecial {
  public GameObject[] effects;

  public void Activate(params GameObject[] targets) {
    for (int i = 0; i < targets.Length; ++i) {
      for (int j = 0; j < effects.Length; ++j) {
        GameObject temp = Instantiate(effects[j]) as GameObject;
        temp.transform.parent = targets[i].transform;
        SpellSpecial special = temp.GetComponent<SpellSpecial>();
        special.Activate(bundle);
      }
    }
  }
}