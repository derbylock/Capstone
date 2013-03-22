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
    public bool ready;
    public NetworkPlayerBundle(NetworkPlayer player, int team) {
      this.player = player;
      this.team = team;
      this.ready = false;
    }
    public bool Equals(NetworkPlayer player) {
      return this.player == player;
    }
  };

  public List<NetworkPlayerBundle> playerTeams = new List<NetworkPlayerBundle>();

  void Awake() {
    DontDestroyOnLoad(gameObject);
  }

  void Start() {
    if(Network.isServer) {
      playerTeams.Add(new NetworkPlayerBundle(Network.player, TEAM_NEUTRAL));
    } else {
      networkView.RPC("RequestRoster", RPCMode.Server, Network.player);
    }
  }

  void Update() {
    if(Network.isServer) {
      Debug.Log(playersReady);
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

    //
    if(InputReader.GetKeysDownOr(KeyCode.Return, KeyCode.KeypadEnter)) {
      Debug.Log("Doinit");
      if (Network.isServer) {
        ReadyCheck(Network.player, true);
      } else {
        networkView.RPC("ReadyCheck", RPCMode.Server, Network.player, true);
      }
    }
    
    if (InputReader.GetKeysDownOr(KeyCode.Backspace, KeyCode.Delete)) {
      if (Network.isServer) {
        ReadyCheck(Network.player, false);
      } else {
        networkView.RPC("ReadyCheck", RPCMode.Server, Network.player, false);
      }
    }
  }

  void OnPlayerConnected(NetworkPlayer player) {
    playerTeams.Add(new NetworkPlayerBundle(player, TEAM_NEUTRAL));
    networkView.RPC("UpdateClient", RPCMode.Others, player, TEAM_NEUTRAL, false);
    /*for(int i=0; i<playerTeams.Count; ++i) {
      networkView.RPC("UpdateClient", RPCMode.Others, playerTeams[i].player, playerTeams[i].team, playerTeams[i].ready);
    }*/
  }

  [RPC]
  void UpdateServerWithMove(NetworkPlayer mover, int team) {
    NetworkPlayerBundle bundle = GetPlayerBundle(mover);
    if (!bundle.ready) {
      if (team == TEAM_NEUTRAL) {
        IncrementTeam(bundle.team, -1);
        bundle.team = team;
      } else if (IsTeamJoinable(team)) {
        IncrementTeam(bundle.team, -1);
        bundle.team = team;
        IncrementTeam(team, 1);
      }
      networkView.RPC("UpdateClient", RPCMode.Others, bundle.player, bundle.team, bundle.ready);
    }
  }

  [RPC]
  void UpdateClient(NetworkPlayer player, int team, bool ready) {
    NetworkPlayerBundle bundle = GetPlayerBundle(player);
    if (bundle != null) {
      bundle.team = team;
      bundle.ready = ready;
    } else {
      playerTeams.Add(new NetworkPlayerBundle(player, team));
    }
    /*for(int i=0; i<playerTeams.Count; ++i) {
      if(playerTeams[i].Equals(player)) {
        playerTeams[i].team = team;
        return;
      }
    }
    playerTeams.Add(new NetworkPlayerBundle(player, team));*/
  }

  [RPC]
  void RequestRoster(NetworkPlayer requester) {
    for(int i=0; i<playerTeams.Count; ++i) {
      networkView.RPC("UpdateClient", requester, playerTeams[i].player, playerTeams[i].team, playerTeams[i].ready);
    }
  }

  [RPC]
  void ReadyCheck(NetworkPlayer me, bool ready) {
    bool changeMade = false;
    NetworkPlayerBundle bundle = GetPlayerBundle(me);
    if (ready && (bundle.team == TEAM_ONE || bundle.team == TEAM_TWO)) {
      changeMade = true;
      bundle.ready = ready;
      ++playersReady;
    } else {
      changeMade = true;
      bundle.ready = ready;
      --playersReady;
    }

    if (changeMade) {
      networkView.RPC("UpdateClient", RPCMode.Others, bundle.player, bundle.team, bundle.ready);
    }
  }

  NetworkPlayerBundle GetPlayerBundle(NetworkPlayer player) {
    for (int i = 0; i < playerTeams.Count; ++i) {
      if (playerTeams[i].Equals(player)) {
        return playerTeams[i];
      }
    }
    return null;
  }

  bool IsTeamJoinable(int team) {
    if(team == TEAM_ONE && playersOnTeamOne <= playersOnTeamTwo) {
      Debug.Log(playersOnTeamOne + " <= " + playersOnTeamTwo);
      return true;
    }
    if (team == TEAM_TWO && playersOnTeamTwo <= playersOnTeamOne) {
      Debug.Log(playersOnTeamTwo + " <= " + playersOnTeamOne);
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
      GUILayout.Label("Ready: " + (playerTeams[i].ready ? "Locked In" : "Pending"));
      GUILayout.EndHorizontal();
    }
  }
}