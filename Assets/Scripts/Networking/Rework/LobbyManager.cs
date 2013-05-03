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

  /*void Awake() {
    DontDestroyOnLoad(gameObject);
  }*/

  void Start() {
    if(Network.isServer) {
      playerTeams.Add(new NetworkPlayerBundle(Network.player, Constants.TEAM_NEUTRAL));
    } else {
      networkView.RPC("RequestRoster", RPCMode.Server, Network.player);
    }
  }

  void Update() {
    if (Network.isServer) {
      Debug.Log(playersReady + "/" + (Network.connections.Length+1));
      if (playersReady == Network.connections.Length + 1) {
        MigrateToMatch();
        /*NetworkLevelLoader loader = GameObject.FindObjectOfType(typeof(NetworkLevelLoader)) as NetworkLevelLoader;
        loader.networkView.RPC("LoadLevel", RPCMode.All, RandomLevelPicker.RandomizedLevel());//matchLevelName);*/
      }

      if (Input.GetKeyDown(KeyCode.Alpha0)) {
        UpdateServerWithMove(Network.player, Constants.TEAM_NEUTRAL);
      } else if (Input.GetKeyDown(KeyCode.Alpha1)) {
        UpdateServerWithMove(Network.player, Constants.TEAM_ONE);
      } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
        UpdateServerWithMove(Network.player, Constants.TEAM_TWO);
      }
    } else {
      // Handle input (even for the server)
      if (InputReader.GetKeysDownOr(KeyCode.Alpha0, KeyCode.Keypad0)) {
        networkView.RPC("UpdateServerWithMove", RPCMode.Server, Network.player, Constants.TEAM_NEUTRAL);
        Debug.Log("Sending Update");
      } else if (InputReader.GetKeysDownOr(KeyCode.Alpha1, KeyCode.Keypad1)) {
        networkView.RPC("UpdateServerWithMove", RPCMode.Server, Network.player, Constants.TEAM_ONE);
        Debug.Log("Sending Update");
      } else if (InputReader.GetKeysDownOr(KeyCode.Alpha2, KeyCode.Keypad2)) {
        networkView.RPC("UpdateServerWithMove", RPCMode.Server, Network.player, Constants.TEAM_TWO);
        Debug.Log("Sending Update");
      }
    }

    if(localCanReady && InputReader.GetKeysDownOr(KeyCode.Return, KeyCode.KeypadEnter)) {
      localReady = true;
      CharacterSelection select = GameObject.FindObjectOfType(typeof(CharacterSelection)) as CharacterSelection;
      select.isWaiting = false;
    }
    
    if (InputReader.GetKeysDownOr(KeyCode.Backspace, KeyCode.Delete)) {
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
    NetworkPlayerBundle bundle = GetPlayerBundle(mover);
    if (!bundle.ready) {
      if (team == Constants.TEAM_NEUTRAL) {
        IncrementTeam(bundle.team, -1);
        bundle.team = team;
      } else if (IsTeamJoinable(team)) {
        IncrementTeam(bundle.team, -1);
        bundle.team = team;
        IncrementTeam(team, 1);
      }
      networkView.RPC("UpdateClient", RPCMode.Others, bundle.player, bundle.team, bundle.ready);

      if (CanReady(mover)) {
        if (mover == Network.player) {
          SetCanReady(true);
        } else {
          networkView.RPC("SetCanReady", mover, true);
        }
      } else {
        if (Network.isServer) {
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
  public void ReadyCheck(NetworkPlayer me, bool ready) {
    Debug.Log("Received Ready Check from player " + me);
    bool changeMade = false;
    NetworkPlayerBundle bundle = GetPlayerBundle(me);
    if (!bundle.ready) {
      if (ready && (bundle.team == Constants.TEAM_ONE || bundle.team == Constants.TEAM_TWO)) {
        changeMade = true;
        bundle.ready = true;
        ++playersReady;
      }
    }
    if (bundle.ready) {
      if (!ready) {
        changeMade = true;
        bundle.ready = false;
        --playersReady;
      }
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
      Debug.Log("Choosing team 0");
      Debug.Log("Team 0: " + playersOnTeamOne + " Team 1: " + playersOnTeamTwo);
      return true;
    }
    if (team == Constants.TEAM_TWO && playersOnTeamTwo <= playersOnTeamOne) {
      Debug.Log("Choosing team 1");
      Debug.Log("Team 0: " + playersOnTeamOne + " Team 1: " + playersOnTeamTwo);
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

  void MigrateToMatch() {
    // Export teams to the Persistent Team Data so they can be rebuilt in the match scene.
    PersistentTeamData teamData = GameObject.FindObjectOfType(typeof(PersistentTeamData)) as PersistentTeamData;
    List<NetworkPlayer> t1 = new List<NetworkPlayer>();
    List<NetworkPlayer> t2 = new List<NetworkPlayer>();
    for (int i = 0; i < playerTeams.Count; ++i) {
      if (playerTeams[i].team == Constants.TEAM_ONE) {
        t1.Add(playerTeams[i].player);
      }
      if (playerTeams[i].team == Constants.TEAM_TWO) {
        t2.Add(playerTeams[i].player);
      }
    }
    teamData.FillTeams(t1.ToArray(), t2.ToArray());

    // Load a random map
    NetworkLevelLoader loader = GameObject.FindObjectOfType(typeof(NetworkLevelLoader)) as NetworkLevelLoader;
    loader.networkView.RPC("LoadLevel", RPCMode.All, RandomLevelPicker.RandomizedLevel());
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