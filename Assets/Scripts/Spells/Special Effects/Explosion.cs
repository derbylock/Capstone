using UnityEngine;
using System.Collections;

  public class Explosion : SpellSpecial {
  private const float SPHERECAST_MOVE_DISTANCE = 0.001f;

  public bool destroyOnExplode;
  public float[] explodeTimes;  // Grant flexibility by allowing them to set the number and time of explosions
  private int lastExplodeIndex = -1; // Used to determine if an explosion has already occurred too recently to happen again
  public float timeBetween = -1f;
  private float currentTime;
  public float radius;
  public float damage;
  public Team.TargetType targetType;

  void Start() {
    System.Array.Sort(explodeTimes);

    // Run this after
    if(isDormant) {
      this.enabled = false;
    }
  }

  void FixedUpdate() {
    currentTime += Time.fixedDeltaTime;

    if(timeBetween > 0f) {
      HandleSetTimer();
    } else {
      HandleCustomTimer();
    }
  }

  void HandleSetTimer() {
    if (lastExplodeIndex < currentTime / timeBetween) {
      ++lastExplodeIndex;
      HandleDestruction();
    }
  }

  void HandleCustomTimer() {
    if(explodeTimes[lastExplodeIndex+1] < currentTime) {
      ++lastExplodeIndex;
      HandleDestruction();
    }
  }

  protected override void Activate() {
    Explode();
    base.Activate();
  }

  void Explode() {
    RaycastHit[] hits;
    hits = Physics.SphereCastAll(transform.position, radius, Vector3.up, SPHERECAST_MOVE_DISTANCE, 1 << LayerMask.NameToLayer("Player"));

    if(hits != null) {
      Spell spellScript = gameObject.GetComponent<Spell>();
      for(int i=0; i<hits.Length; ++i) {
        bool valid = Team.IsValidTarget(targetType,
                           hits[i].transform.root.gameObject,
                           spellScript.m_myCaster.root.gameObject);
        
        if(valid) {
          Health targetHealth = hits[i].transform.root.gameObject.GetComponent<Health>();
          targetHealth.ReceiveDamage(damage);
        }
      }
    }
  }

  void HandleDestruction() {
    if(destroyOnExplode) {
      Network.Destroy(gameObject);
    }
  }
}