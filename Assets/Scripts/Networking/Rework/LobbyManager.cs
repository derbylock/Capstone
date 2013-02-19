using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class LobbyManager : MonoBehaviour {
  public string matchLevelName;

  private const int TEAM_NEUTRAL = 0;
  private const int TEAM_ONE = 1;
  private const int TEAM_TWO = 2;
  private int playersOnTeamOne = 0;
  private int playersOnTeamTwo = 0;
  public int playersReady = 0;

  public class NetworkPlayerBundle {
    public NetworkPlayer player;
    public int team;
    public NetworkPlayerBundle(NetworkPlayer player, int team) {
      this.player = player;
      this.team = team;
    }
    public bool Equals(NetworkPlayer player) {
      return this.player == player;
    }
  };

  public List<NetworkPlayerBundle> playerTeams = new List<NetworkPlayerBundle>();

  void Start() {
    if(Network.isServer) {
      playerTeams.Add(new NetworkPlayerBundle(Network.player, TEAM_NEUTRAL));
    } else {
      networkView.RPC("RequestRoster", RPCMode.Server, Network.player);
    }
  }

  void Update() {
    if(Network.isServer) {
      if(playersReady == Network.connections.Length+1) {
        NetworkLevelLoader loader = GameObject.FindObjectOfType(typeof(NetworkLevelLoader)) as NetworkLevelLoader;
        loader.networkView.RPC("LoadLevel", RPCMode.All, matchLevelName);

        if (InputReader.GetKeysDownOr(KeyCode.Alpha0, KeyCode.Keypad0)) {
          UpdateServerWithMove(Network.player, TEAM_NEUTRAL);
        } else if (InputReader.GetKeysDownOr(KeyCode.Alpha1, KeyCode.Keypad1)) {
          UpdateServerWithMove(Network.player, TEAM_ONE);
        } else if (InputReader.GetKeysDownOr(KeyCode.Alpha2, KeyCode.Keypad2)) {
          UpdateServerWithMove(Network.player, TEAM_TWO);
        }
      }
    }

    // Handle input (even for the server)
    if(InputReader.GetKeysDownOr(KeyCode.Alpha0, KeyCode.Keypad0)) {
      networkView.RPC("UpdateServerWithMove", RPCMode.Server, Network.player, TEAM_NEUTRAL);
    } else if(InputReader.GetKeysDownOr(KeyCode.Alpha1, KeyCode.Keypad1)) {
      networkView.RPC("UpdateServerWithMove", RPCMode.Server, Network.player, TEAM_ONE);
    } else if (InputReader.GetKeysDownOr(KeyCode.Alpha2, KeyCode.Keypad2)) {
      networkView.RPC("UpdateServerWithMove", RPCMode.Server, Network.player, TEAM_TWO);
    }

    if(InputReader.GetKeysDownOr(KeyCode.Return)) {
      if (Network.isServer) {
        
      } else {

      }
    }
  }

  void OnPlayerConnected(NetworkPlayer player) {
    playerTeams.Add(new NetworkPlayerBundle(player, TEAM_NEUTRAL));
    for(int i=0; i<playerTeams.Count; ++i) {
      networkView.RPC("UpdateClient", player, playerTeams[i].player, playerTeams[i].team);
    }
  }

  [RPC]
  void UpdateServerWithMove(NetworkPlayer mover, int team) {
    for(int i=0; i<playerTeams.Count; ++i) {
      if(playerTeams[i].Equals(mover)) {
        if(team == TEAM_NEUTRAL) {
          IncrementTeam(playerTeams[i].team, -1);
          playerTeams[i].team = team;
        } else if(IsTeamJoinable(team)) {
          IncrementTeam(playerTeams[i].team, -1);
          playerTeams[i].team = team;
          IncrementTeam(team, 1);
        }
        networkView.RPC("UpdateClient", RPCMode.Others, mover, team);
      }
    }
  }

  [RPC]
  void UpdateClient(NetworkPlayer player, int team) {
    for(int i=0; i<playerTeams.Count; ++i) {
      if(playerTeams[i].Equals(player)) {
        playerTeams[i].team = team;
        return;
      }
    }
    playerTeams.Add(new NetworkPlayerBundle(player, team));
  }

  [RPC]
  void RequestRoster(NetworkPlayer me) {
    for(int i=0; i<playerTeams.Count; ++i) {
      networkView.RPC("UpdateClient", me, playerTeams[i].player, playerTeams[i].team);
    }
  }

  [RPC]
  void ReadyCheck(NetworkPlayer me, bool ready) {
    
  }

  bool IsTeamJoinable(int team) {
    if(team == TEAM_ONE && playersOnTeamOne <= playersOnTeamTwo) {
      return true;
    }
    if (team == TEAM_TWO && playersOnTeamTwo <= playersOnTeamOne) {
      return true;
    }
    return false;
  }

  void IncrementTeam(int team, int amount) {
    if(team == TEAM_ONE) {
      playersOnTeamOne += amount;
    }
    if (team == TEAM_TWO) {
      playersOnTeamTwo += amount;
    }
  }

  // OnGUI is only for testing purposes
  void OnGUI() {
    for(int i=0; i<playerTeams.Count; ++i) {
      GUILayout.BeginHorizontal();
      GUILayout.Label("Player: " + playerTeams[i].player);
      GUILayout.Label("Team: " +  playerTeams[i].team);
      GUILayout.EndHorizontal();
    }
  }
}