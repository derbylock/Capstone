using UnityEngine;
using System.Collections;

public class AlterPlayerPosition : SpellSpecial {
  public AnimationCurve x;
  public AnimationCurve y;
  public AnimationCurve z;
  public float duration;
  public bool ensureGrounded;
  
  void Update() {
    if(!activated) {
      return;
    }
    
    if(!hasRun) {
      hasRun = true;
      Spell rootSpell = gameObject.GetComponent<Spell>();
      networkView.RPC("RunMovePlayer", rootSpell.m_myCaster.networkView.owner);
      //StartCoroutine("MovePlayer");
    }
  }

  [RPC]
  void RunMovePlayer() {
    StartCoroutine("MovePlayer");
  }
  
  IEnumerator MovePlayer() {
    Spell rootSpell = gameObject.GetComponent<Spell>();
    
    Transform player = rootSpell.m_myCaster.transform;

    // Change animation curves relative to the player's current position
    for (int i = 0; i < x.keys.Length; ++i) {
      x.keys[i].value += player.position.x;
    }
    for (int i = 0; i < y.keys.Length; ++i) {
      y.keys[i].value += player.position.y;
    }
    for (int i = 0; i < z.keys.Length; ++i) {
      z.keys[i].value += player.position.z;
    }

    // Create the clip
    AnimationClip clip = new AnimationClip();
    clip.ClearCurves();
    clip.SetCurve("", typeof(Transform), "localPosition.x", x);
    clip.SetCurve("", typeof(Transform), "localPosition.y", y);
    clip.SetCurve("", typeof(Transform), "localPosition.z", z);


    PlayerController controller = player.GetComponent<PlayerController>();
    player.networkView.RPC("LockPlayer", player.networkView.owner);

    float startTime = Time.time;
    while(Time.time < startTime + duration) {
    }
    
    return null;
  }
}