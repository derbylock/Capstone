using UnityEngine;
using System.Collections;

public class ModifyArmor : SpellSpecial {
  public float amount;
  public float duration;

  void Update() {
    if(activated) {
      if (!runOnce || (runOnce && !hasRun)) {
        hasRun = true;
        Run();
      }
    }
  }

  void Run() {
    bool sameModFound = false;
    for(int i = 0; i<bundle.hitTarget.transform.GetChildCount() /*transform.root.GetChildCount()*/; ++i) {
      Transform child = bundle.hitTarget.transform.GetChild(i);
      ModifyArmor modArmor = child.GetComponent<ModifyArmor>();
      if (modArmor != null && modArmor != this && modArmor.spellID == this.spellID) {
        RemoveArmorMod();
         sameModFound = true;
        break;
      }
    }

    if(!sameModFound) {
      transform.parent = bundle.hitTarget.transform;
      transform.localPosition = Vector3.zero;
      transform.localRotation = Quaternion.identity;

      ApplyArmorMod();

      StartCoroutine("BeginDestructTimer");
    } else {
      DestroyAllArmorMods();
    }
  }

  void ApplyArmorMod() {
    Armor armor = transform.root.GetComponent<Armor>();

    if (armor != null) {
      armor.armor += amount;
    }
  }

  void RemoveArmorMod() {
    Armor armor = transform.root.GetComponent<Armor>();

    if (armor != null) {
      armor.armor -= amount;
    }

    this.transform.parent = null;
    Destroy(gameObject);
  }

  IEnumerator BeginDestructTimer() {
    yield return new WaitForSeconds(duration);
    RemoveArmorMod();
  }

  void DestroyAllArmorMods() {
    for (int i = 0; i < bundle.hitTarget.transform.GetChildCount(); ++i) {
      Transform child = bundle.hitTarget.transform.GetChild(i);
      ModifyArmor modArmor = child.GetComponent<ModifyArmor>();
      if (modArmor != null && modArmor != this && modArmor.spellID == this.spellID) {
        child.parent = null;
        Destroy(child.gameObject);
      }
    }
    transform.parent = null;
    Destroy(gameObject);
  }
}