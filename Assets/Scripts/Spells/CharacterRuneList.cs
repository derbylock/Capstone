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
      int occupiedSlots = 0;
      for (int i = 0; i < activeRunes.Length; ++i) {
        if (activeRunes[i] != null) {
          ++occupiedSlots;
        }
      }

      if (occupiedSlots == activeRunes.Length) {
        Network.Destroy(activeRunes[0].gameObject);
        activeRunes[0] = null;
      }

      /*Transform[] totalRunes = Array.FindAll(activeRunes, null);
      if(totalRunes.Length == activeRunes.Length) {
        Network.Destroy(activeRunes[0].gameObject);
        activeRunes[0] = null;
      }*/
      
      Update(); // Push all runes forward on the queue
      for (int j = 0; j < activeRunes.Length; ++j) {
        if (activeRunes[j] == null) {
          activeRunes[j] = rune;
        }
      }
      
    }
    
    public bool Equals(Transform owner) {
      return this.owner = owner;
    }
  }