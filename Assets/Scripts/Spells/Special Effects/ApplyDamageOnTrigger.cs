using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApplyDamageOnTrigger : SpellSpecial {
  public float damage;
  public float triggerDelay;
  public float triggerDuration;

  private List<GameObject> playersHit = new List<GameObject>();

  void Awake() {
    collider.enabled = false;
  }

  public void Activate(SpellBundle bundle) {
    this.bundle = bundle;
    StartCoroutine("ColliderActivator");
  }

  IEnumerator ColliderActivator() {
    yield return new WaitForSeconds(triggerDelay);
    Debug.Log("Starting trigger");
    collider.enabled = true;
    yield return new WaitForSeconds(triggerDuration);
    collider.enabled = false;
    Debug.Log("Ending trigger");
  }

  void OnTriggerEnter(Collider other) {
    GameObject root = other.transform.root.gameObject;
    if (root.tag == "Player" && playersHit.IndexOf(root) == -1) {
      Team team = root.GetComponent<Team>();
      if (team.m_teamNumber != bundle.team) {
        playersHit.Add(root);
        Health playerHealth = root.GetComponent<Health>();
        playerHealth.ReceiveDamage(damage);
      }
    }
  }
}