using UnityEngine;
using System.Collections;
using System.Reflection;

public class Aura : SpellSpecial {
  public enum AuraType {
    AddEffect,
    DirectDamage,
    DamageOverTime,
    Heal,
    HealOverTime,
    ModifySpeed,
    DevTest,
  };

  public bool infiniteDuration;
  public float duration;
  private float currentTime;
  public float initialDelay;
  public AuraType auraType;
  public float effectAmount;
  public int tickCount;
  private int currentTick;
  private float tickSpacing;

  MethodInfo applyMethod; // Cache method we may use repeatedly

  void Start() {
    applyMethod = this.GetType().GetMethod("Apply" + auraType.ToString(), BindingFlags.NonPublic | BindingFlags.Instance);

    if (!infiniteDuration) {
      tickSpacing = (duration - initialDelay) * tickCount;
    }

    currentTime = 0.0f;
    currentTick = 0;

    // Apply the first effect if there is no initial delay
    if(initialDelay == 0f) {
      applyMethod.Invoke(this, null);
    }
  }
  
  void ApplyAddEffect() {
    SpellActivator activator = transform.GetComponent<SpellActivator>();
    activator.Activate(bundle);
  }

  void ApplyDirectDamage() {
    Health health = transform.root.gameObject.GetComponent<Health>();
    health.ReceiveDamage(effectAmount);
    ++currentTick;
  }

  void ApplyDamageOverTime() {
    ApplyDirectDamage();
  }

  void ApplyHeal() {
    Health health = transform.root.gameObject.GetComponent<Health>();
    health.ReceiveHeal(effectAmount);
    ++currentTick;
  }

  void ApplyHealOverTime() {
    ApplyHeal();
  }

  void ApplyModifySpeed() {
    throw new System.NotImplementedException();
  }

  void ApplyDevTest() {
    Debug.Log("DevTest Successfully Run.");
  }
}
