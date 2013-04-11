using UnityEngine;
using System;

public class CharacterRuneList {
    public Transform owner;
    private Transform[] activeRunes;
    
    public CharacterRuneList(Transform character, int initWithSize) {
      owner = character;
      activeRunes = new Transform[initWithSize];
    }
    
    public void Update() {
      for(int i=0; i<activeRunes.Length; ++i) {
        if(activeRunes[i] == null) {
          for(int j=i; j<activeRunes.Length; ++j) {
            if(activeRunes[j] != null) {
              activeRunes[i] = activeRunes[j];
              activeRunes[j] = null;
            }
          }
        }
      }
    }
    
    public void AddRune(Transform rune) {
      int[] totalRunes = Array.FindAll(activeRunes, null);
      if(totalRunes.Length == activeRunes.Length) {
        Network.Destroy(activeRunes[0]);
        activeRunes[0] = null;
      }
      
      Update(); // Push all runes forward on the queue
      int placementIndex = Array.FindIndex(activeRunes, null);
      activeRunes[placementIndex] = rune;
    }
    
    public bool Equals(Transform owner) {
      return this.owner = owner;
    }
  }