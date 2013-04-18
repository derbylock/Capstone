using UnityEngine;

public class AlterPlayerPosition : SpellSpecial {
  public AnimationCurve movementCurve;
  
  void Update() {
    if(!activated) {
      return;
    }
    
    if(!hasRun) {
      hasRun = true;
      StartCoroutine(
    }
  }
  
  IEnumerator MovePlayer() {
    
  }
}