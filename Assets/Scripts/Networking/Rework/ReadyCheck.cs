using UnityEngine;
using System.Collections;

public class ReadyCheck {
  public NetworkPlayer player;
  public bool status;

  public ReadyCheck(NetworkPlayer player) {
    this.player = player;
    this.status = false;
  }
}