using UnityEngine;
using System.Collections;

public class PlayerData : ScriptableObject {
  public NetworkPlayer player;
  public GameObject avatar;
  public Health health;
  public Mana mana;
  public int team;  // -1 for spectator mode

  public bool SameTeam (int team) { return team == this.team; }
  public bool Equals (NetworkPlayer player) { return player == this.player; }
}