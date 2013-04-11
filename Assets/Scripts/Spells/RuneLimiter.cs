using UnityEngine;
using System;
using System.Collections.Generic;

public class RuneLimiter : MonoBehaviour {
  public const int perCharLimit = 3;
  private List<CharacterRuneList> runeOwners;

  void Start {
    runeOwners = new List<CharacterRuneList>();
  }
  
  public void AddRune(Transform owner, Transform rune) {
    int index = runeOwners.FindIndexOf(x => x.equals(owner));
    if(index == -1) {
      CharacterRuneList temp = new CharacterRuneList(owner, perCharacterLimit);
      temp.AddRune(rune);
      runeOwners.Add(temp);
    } else {
      runeOwners[index].AddRune(rune);
    }
  }
}