using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SpawnPointDelegator : MonoBehaviour {
  // This class is used to get around the fact that NetworkPlayer is not nullable.
  private class MyNetworkPlayer {
    public NetworkPlayer player;
  }

  public Vector3[] spawnPoints;
  private MyNetworkPlayer[] occupierArray;
  private Vector3 mySpawn;

  List<PlayerData> players;

  // Use this for initialization
  void Start() {
    if (Network.isServer) {
      PlayerManager manager = GameObject.FindObjectOfType(typeof(PlayerManager)) as PlayerManager;
      players = manager.activePlayers;

      occupierArray = new MyNetworkPlayer[spawnPoints.Length];

      // Convenience variables for iteration
      int teamOneIndex = 0;
      int teamTwoIndex = occupierArray.Length / 2;
      for (int i = 0; i < players.Count; ++i) {
        MyNetworkPlayer temp = new MyNetworkPlayer();
        temp.player = players[i].player;
        if (players[i].team == Constants.TEAM_ONE) {
          occupierArray[teamOneIndex] = temp;
          ++teamOneIndex;
        } else {
          occupierArray[teamTwoIndex] = temp;
          ++teamTwoIndex;
        }
      }

      StartCoroutine("ReorderSpawns");
    }
  }

  void Update() {
    if (mySpawn == null) {
      networkView.RPC("RequestSpawnPoint", RPCMode.Server, Network.player);
    }
  }

  [RPC]
  void RequestSpawnPoint(NetworkPlayer toSpawn) {
    Vector3 spawnLocation = Vector3.zero;
    //int index = players.FindIndex(x => x.player == toSpawn);

    int index = Array.IndexOf(occupierArray, toSpawn);
    if (index != -1) {
      spawnLocation = spawnPoints[index];
    }

    /*if (index != -1) {
      int team = players[index].team;
      int teamPosition = 0;
      if (team != -1) {
        for (int i = 0; i < index; ++i) {
          if (players[i].team == team) {
            ++teamPosition;
          }
        }
      }
    } else {
      Debug.Log("Player " + toSpawn.ToString() + " not found in players. (SpawnPointDelegator)");
    }*/

    if (toSpawn == Network.player) { // This code is only run on the server
      SetSpawnPoint(spawnLocation);
    } else {
      networkView.RPC("SetSpawnPoint", toSpawn, spawnLocation);
    }
  }

  [RPC]
  void SetSpawnPoint(Vector3 spawnLocation) {
    mySpawn = spawnLocation;
  }

  IEnumerator ReorderSpawns() {
    while (true) {
      int halfwayPoint = occupierArray.Length / 2;
      int lowerBounds, upperBounds;
      // Modify player positions as necessary do not update players at this point.
      for (int i = 0; i < players.Count; ++i) {
        int playerIndex = Array.FindIndex(occupierArray, x => x.player == players[i].player);

        if (players[i].team == Constants.TEAM_NEUTRAL) {
          if (playerIndex != -1) {
            occupierArray[playerIndex] = null;
          }
        } else {
          if (playerIndex != -1) {
            // lowerBounds and upperBounds define the area which a given team is placed
            // within the occupierArray, since it is shared by both teams.
            lowerBounds = halfwayPoint * players[i].team;
            upperBounds = lowerBounds + halfwayPoint;
            if (!(playerIndex >= lowerBounds && playerIndex < upperBounds)) {
              occupierArray[playerIndex] = null;
              for (int j = lowerBounds; j < upperBounds; ++j) {
                if (occupierArray[j] == null) {
                  MyNetworkPlayer temp = new MyNetworkPlayer();
                  temp.player = players[i].player;
                  occupierArray[j] = temp;

                  if (players[i].player == Network.player) {
                    SetSpawnPoint(spawnPoints[j]);
                  } else {
                    networkView.RPC("SetSpawnPoint", players[i].player, spawnPoints[j]);
                  }
                  break;
                }
              }
            }
            // TODO: Implement logic to ensure that the player is on the right team and move them if not.
          }
        }
      }

      yield return new WaitForSeconds(0.5f);

      // Iterate through each half of the list and push players as far up the array
      // as they can go. If a move does occur, inform the player so they can shift.
      lowerBounds = 0;
      upperBounds = halfwayPoint;
      for (int i = 0; i < upperBounds; ++i) {
        if(occupierArray[i] == null) {
          for (int j = i; j < upperBounds; ++j) {
            if (occupierArray[j] != null) {
              occupierArray[i] = occupierArray[j];
              occupierArray[j] = null;

              if (players[i].player == Network.player) {
                SetSpawnPoint(spawnPoints[i]);
              } else {
                networkView.RPC("SetSpawnPoint", occupierArray[i].player, spawnPoints[i]);
              }
            }
          }
        }
      }

      lowerBounds = upperBounds;
      upperBounds += halfwayPoint;
      for (int i = 0; i < upperBounds; ++i) {
        if(occupierArray[i] == null) {
          for (int j = i; j < upperBounds; ++j) {
            if (occupierArray[j] != null) {
              occupierArray[i] = occupierArray[j];
              occupierArray[j] = null;

              if (players[i].player == Network.player) {
                SetSpawnPoint(spawnPoints[i]);
              } else {
                networkView.RPC("SetSpawnPoint", occupierArray[i].player, spawnPoints[i]);
              }
            }
          }
        }
      }

      // Wait
      yield return new WaitForSeconds(0.5f);
    }
    yield return null;
  }
}