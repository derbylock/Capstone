using UnityEngine;
using System;
using System.Collections.Generic;

public class RuneLimiter : MonoBehaviour {
  public const int perCharLimit = 3;
  private List<CharacterRuneList> runeOwners = new List<CharacterRuneList>();
  
  public void AddRune(Transform owner, Transform rune) {
    int index = runeOwners.FindIndex(x => x.Equals(owner));
    if(index == -1) {
      CharacterRuneList temp = new CharacterRuneList(owner, perCharLimit);
      temp.AddRune(rune);
      runeOwners.Add(temp);
    } else {
      runeOwners[index].AddRune(rune);
    }
  }
}