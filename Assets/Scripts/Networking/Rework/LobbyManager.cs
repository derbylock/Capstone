using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class LobbyManager : MonoBehaviour {
  public string matchLevelName;

  private int playersOnTeamOne = 0;
  private int playersOnTeamTwo = 0;
  public int playersReady = 0;
  public bool localReady;
  public bool localCanReady;

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
      playerTeams.Add(new NetworkPlayerBundle(Network.player, Constants.TEAM_NEUTRAL));
    } else {
      networkView.RPC("RequestRoster", RPCMode.Server, Network.player);
    }
  }

  void Update() {
    if(Network.isServer) {
//      Debug.Log(playersReady);
      if(playersReady == Network.connections.Length+1) {
        NetworkLevelLoader loader = GameObject.FindObjectOfType(typeof(NetworkLevelLoader)) as NetworkLevelLoader;
        loader.networkView.RPC("LoadLevel", RPCMode.All, matchLevelName);

        //if (InputReader.GetKeysDownOr(KeyCode.Alpha0, KeyCode.Keypad0)) {
        if(Input.GetKeyDown(KeyCode.Alpha0)) {
          UpdateServerWithMove(Network.player, Constants.TEAM_NEUTRAL);
        } else if(Input.GetKeyDown(KeyCode.Alpha1)) { //if (InputReader.GetKeysDownOr(KeyCode.Alpha1, KeyCode.Keypad1)) {
          UpdateServerWithMove(Network.player, Constants.TEAM_ONE);
        } else if(Input.GetKeyDown(KeyCode.Alpha2)) { //if (InputReader.GetKeysDownOr(KeyCode.Alpha2, KeyCode.Keypad2)) {
          UpdateServerWithMove(Network.player, Constants.TEAM_TWO);
        }
      }
      
      if(playersReady == Network.connections.Length+1) {
        NetworkLevelLoader loader = GameObject.FindObjectOfType(typeof(NetworkLevelLoader)) as NetworkLevelLoader;
        loader.LoadLevel(RandomLevelPicker.RandomizedLevel());
      }
    }

    // Handle input (even for the server)
    if(InputReader.GetKeysDownOr(KeyCode.Alpha0, KeyCode.Keypad0)) {
      networkView.RPC("UpdateServerWithMove", RPCMode.Server, Network.player, Constants.TEAM_NEUTRAL);
      Debug.Log("Sending Update");
    } else if(InputReader.GetKeysDownOr(KeyCode.Alpha1, KeyCode.Keypad1)) {
      networkView.RPC("UpdateServerWithMove", RPCMode.Server, Network.player, Constants.TEAM_ONE);
      Debug.Log("Sending Update");
    } else if (InputReader.GetKeysDownOr(KeyCode.Alpha2, KeyCode.Keypad2)) {
      networkView.RPC("UpdateServerWithMove", RPCMode.Server, Network.player, Constants.TEAM_TWO);
      Debug.Log("Sending Update");
    }

    if(localCanReady && InputReader.GetKeysDownOr(KeyCode.Return, KeyCode.KeypadEnter)) {
      /*if (Network.isServer) {
        ReadyCheck(Network.player, true);
      } else {
        networkView.RPC("ReadyCheck", RPCMode.Server, Network.player, true);
      }*/
      localReady = true;
      CharacterSelection select = GameObject.FindObjectOfType(typeof(CharacterSelection)) as CharacterSelection;
      select.isWaiting = false;
    }
    
    if (InputReader.GetKeysDownOr(KeyCode.Backspace, KeyCode.Delete)) {
      /*if (Network.isServer) {
        ReadyCheck(Network.player, false);
      } else {
        networkView.RPC("ReadyCheck", RPCMode.Server, Network.player, false);
      }*/
      localReady = false;
      CharacterSelection select = GameObject.FindObjectOfType(typeof(CharacterSelection)) as CharacterSelection;
      select.isWaiting = true;
    }
  }

  void OnPlayerConnected(NetworkPlayer player) {
    playerTeams.Add(new NetworkPlayerBundle(player, Constants.TEAM_NEUTRAL));
    networkView.RPC("UpdateClient", RPCMode.Others, player, Constants.TEAM_NEUTRAL, false);
  }

  [RPC]
  void UpdateServerWithMove(NetworkPlayer mover, int team) {
    Debug.Log("Received Move Request");
    NetworkPlayerBundle bundle = GetPlayerBundle(mover);
    if (!bundle.ready) {
      Debug.Log("Player Not Ready (GOOD)");
      if (team == Constants.TEAM_NEUTRAL) {
        Debug.Log("Setting to neutral team");
        IncrementTeam(bundle.team, -1);
        bundle.team = team;
      } else if (IsTeamJoinable(team)) {
        Debug.Log("Team is joinable...");
        IncrementTeam(bundle.team, -1);
        bundle.team = team;
        IncrementTeam(team, 1);
      }
      networkView.RPC("UpdateClient", RPCMode.Others, bundle.player, bundle.team, bundle.ready);

      if (CanReady(mover)) {
        Debug.Log("Can Ready is true...");
        Debug.Log("Mover is server? ");
        if (mover == Network.player) {
          SetCanReady(true);
        } else {
          Debug.Log("Sending SetCanReady to Client");
          networkView.RPC("SetCanReady", mover, true);
        }
      } else {
        Debug.Log("Sending Bad Result");
        if (Network.isServer) {
          Debug.Log("Sending CanReady to the server....wat");
          SetCanReady(false);
        } else {
          networkView.RPC("SetCanReady", mover, false);
        }
      }
    } else {
      Debug.Log("Bundle somehow ready....");
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
  }

  [RPC]
  void RequestRoster(NetworkPlayer requester) {
    for(int i=0; i<playerTeams.Count; ++i) {
      networkView.RPC("UpdateClient", requester, playerTeams[i].player, playerTeams[i].team, playerTeams[i].ready);
    }
  }

  bool CanReady(NetworkPlayer player) {
    NetworkPlayerBundle bundle = GetPlayerBundle(player);
    return bundle.team != Constants.TEAM_NEUTRAL;
  }

  [RPC]
  void SetCanReady(bool canReady) {
    localCanReady = canReady;
  }

  [RPC]
  void ReadyCheck(NetworkPlayer me, bool ready) {
    Debug.Log("Received Ready Check from player " + me);
    bool changeMade = false;
    NetworkPlayerBundle bundle = GetPlayerBundle(me);
    if (ready && (bundle.team == Constants.TEAM_ONE || bundle.team == Constants.TEAM_TWO)) {
      changeMade = true;
      bundle.ready = true;
      ++playersReady;
    }
    if(!ready) {
      changeMade = true;
      bundle.ready = false;
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
    if(team == Constants.TEAM_ONE && playersOnTeamOne <= playersOnTeamTwo) {
      Debug.Log(playersOnTeamOne + " <= " + playersOnTeamTwo);
      return true;
    }
    if (team == Constants.TEAM_TWO && playersOnTeamTwo <= playersOnTeamOne) {
      Debug.Log(playersOnTeamTwo + " <= " + playersOnTeamOne);
      return true;
    }
    return false;
  }

  void IncrementTeam(int team, int amount) {
    if(team == Constants.TEAM_ONE) {
      playersOnTeamOne += amount;
    }
    if (team == Constants.TEAM_TWO) {
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