using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkInterpolatedTransform))]
public class SpellObjectMover : SpellSpecial {
  public bool destroyOnEnemyCollision;
  public bool destroyOnAllyCollision;
  public float damageOnCollision;
  public float speed;
  
  public GameObject collisionEffect;
  
  void Update() {
    if(activated && !hasRun) {
      hasRun = true;

      TimedObjectDestruction destructor = transform.GetComponent<TimedObjectDestruction>();
      if (destructor != null) {
        Destroy(destructor);
      }

      StartCoroutine("Move");
    }
  }
  
  IEnumerator Move() {
    collider.isTrigger = true;
    Vector3 direction = transform.forward * speed;
    while(true) {
      transform.position += direction * Time.deltaTime;
      yield return new WaitForSeconds(Time.deltaTime);
    }
  }
  
  void OnTriggerEnter(Collider other) {
    Transform hitTarget = other.transform.root;
    bool destroy = false;
    if(hitTarget.tag == "Player") {
      Team targetTeam = hitTarget.GetComponent<Team>();
      if(bundle.team != targetTeam.m_teamNumber) {
        HealthRework targetHealth = hitTarget.GetComponent<HealthRework>();
        targetHealth.ReceiveDamage(damageOnCollision);
        if(destroyOnEnemyCollision) {
          destroy = true;
        }
      } else if(destroyOnAllyCollision) {
        destroy = true;
      }
    } else {
      destroy = true;
    }
    
    if(destroy) {
      if(collisionEffect != null) {
        Network.Instantiate(collisionEffect, transform.position, transform.rotation, 0);
      }
      Network.Destroy(gameObject);
    }
  }
}