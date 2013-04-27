using UnityEngine;

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
    for(int i = 0; i<transform.root.GetChildCount(); ++i) {
      Transform child = transform.root.GetChild(i);
      ModifyArmor modArmor = child.GetComponent<ModifyArmor>();
      if (modArmor != this && modArmor.amount == this.amount) {
        RemoveArmorMod();
         sameModFound = true;
        break;
      }
    }

    if(!sameModFound) {
      ApplyArmorMod();
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
  }

  void DestroyAllArmorMods() {
    for (int i = 0; i < transform.root.GetChildCount(); ++i) {
      Transform child = transform.root.GetChild(i);
      ModifyArmor modArmor = child.GetComponent<ModifyArmor>();
      if (modArmor != this && modArmor.amount == this.amount) {
        child.parent = null;
        Destroy(child.gameObject);
      }
    }
    transform.parent = null;
    Destroy(gameObject);
  }
}