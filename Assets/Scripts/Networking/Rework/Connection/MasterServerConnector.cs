using UnityEngine;
using System.Collections;

public class MasterServerConnector : MonoBehaviour {
  public string gameType = "PMAW_Capstone_Network";
  public string gameName = "Set Match Name...";
  private string comment = "";
  public int serverPort = 25001;
  public bool forceNat = false;
  private bool isCreatingMatch;

  public string lobbySceneName;

  private Rect hostWindowRect;
  private bool showHostWindow;
  private Rect browserWindowRect;
  private bool showBrowserWindow;
  private Vector2 scrollPosition;

  private ConnectionTesterStatus connectionTestResult = ConnectionTesterStatus.Undetermined;
  private string testMessage = "Undetermined NAT capabilities";
  private bool doneTesting = false;
  private bool probingPublicIP = false;
  private bool filterNATHosts = false;
  private bool useNat = false;
  private float timer = 0.0f;

  void Awake() {
    //DontDestroyOnLoad(transform.gameObject);
    hostWindowRect = new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 4, Screen.height / 4);
    browserWindowRect = new Rect(Screen.width / 3, Screen.height / 3, Screen.width / 3, Screen.height / 3);
  }

  void Update() {
    if (!doneTesting) {
      TestConnection();
    }
  }

  void OnGUI() {
    if (!Network.isClient && !Network.isServer) {
      if (GUILayout.Button("Create Match")) {
        showHostWindow = true;
        if (showBrowserWindow) {
          showBrowserWindow = false;
        }
      }
      if (GUILayout.Button("Browse Matches")) {
        MasterServer.RequestHostList(gameType);
        showBrowserWindow = true;
        if (showHostWindow) {
          showHostWindow = false;
        }
      }

      if (showHostWindow) {
        hostWindowRect = GUILayout.Window(2, hostWindowRect, MakeHostWindow, "Host a Match");
      }
      if (showBrowserWindow) {
        browserWindowRect = GUILayout.Window(1, browserWindowRect, MakeClientWindow, "Server List");
      }
    }
  }

  void TestConnection() {
    // Start/Poll the connection test, report the results in a label and react to the results accordingly
    connectionTestResult = Network.TestConnection();
    switch (connectionTestResult) {
      case ConnectionTesterStatus.Error:
        testMessage = "Problem determining NAT capabilities";
        doneTesting = true;
        break;

      case ConnectionTesterStatus.Undetermined:
        testMessage = "Undetermined NAT capabilities";
        doneTesting = false;
        break;

      case ConnectionTesterStatus.PublicIPIsConnectable:
        testMessage = "Directly connectable public IP address.";
        useNat = false;
        doneTesting = true;
        break;

      // This case is a bit special as we now need to check if we can 
      // circumvent the blocking by using NAT punchthrough
      case ConnectionTesterStatus.PublicIPPortBlocked:
        testMessage = "Non-connectble public IP address (port " + serverPort + " blocked), running a server is impossible.";
        useNat = false;
        // If no NAT punchthrough test has been performed on this public IP, force a test
        if (!probingPublicIP) {
          Debug.Log("Testing if firewall can be circumvented");
          connectionTestResult = Network.TestConnectionNAT();
          probingPublicIP = true;
          timer = Time.time + 10;
        }
          // NAT punchthrough test was performed but we still get blocked
        else if (Time.time > timer) {
          probingPublicIP = false; 		// reset
          useNat = true;
          doneTesting = true;
        }
        break;
      case ConnectionTesterStatus.PublicIPNoServerStarted:
        testMessage = "Public IP address but server not initialized, it must be started to check server accessibility. Restart connection test when ready.";
        break;

      case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
        Debug.Log("LimitedNATPunchthroughPortRestricted");
        testMessage = "Limited NAT punchthrough capabilities. Cannot connect to all types of NAT servers.";
        useNat = true;
        doneTesting = true;
        break;

      case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
        Debug.Log("LimitedNATPunchthroughSymmetric");
        testMessage = "Limited NAT punchthrough capabilities. Cannot connect to all types of NAT servers. Running a server is ill adviced as not everyone can connect.";
        useNat = true;
        doneTesting = true;
        break;

      case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
      case ConnectionTesterStatus.NATpunchthroughFullCone:
        Debug.Log("NATpunchthroughAddressRestrictedCone || NATpunchthroughFullCone");
        testMessage = "NAT punchthrough capable. Can connect to all servers and receive connections from all clients. Enabling NAT punchthrough functionality.";
        useNat = true;
        doneTesting = true;
        break;

      default:
        testMessage = "Error in test routine, got " + connectionTestResult;
        break;
    }

    //Debug.Log(testMessage);
  }

  void MakeHostWindow(int windowID) {
    hostWindowRect = new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 4, Screen.height / 4);
    GUILayout.Space(5);

    GUILayout.BeginHorizontal();
    GUILayout.Label(new GUIContent("Game Name", "This is how your game will be identified to others."));
    gameName = GUILayout.TextField(gameName);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.Label("Port");
    serverPort = System.Convert.ToInt32(GUILayout.TextField(serverPort.ToString()));
    GUILayout.EndHorizontal();
    if (!isCreatingMatch && GUILayout.Button("Create Match")) {
      CreateMatch();
    }
  }

  void MakeClientWindow(int windowID) {
    HostData[] data = MasterServer.PollHostList();

    GUILayout.Space(5);
    scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.Height(Screen.height / 2f));
    foreach (HostData host in data) {
      //if(!(filterNATHosts && host.useNat)) {
      string connections = host.connectedPlayers + "/" + host.playerLimit;
      GUILayout.BeginHorizontal();
      GUILayout.Label(host.gameName);
      //GUILayout.Space(5);
      GUILayout.Label(connections);
      //GUILayout.Space(5);
      GUILayout.FlexibleSpace();
      if (GUILayout.Button("Connect")) {
        Network.Connect(host);
      }
      GUILayout.EndHorizontal();
      //}
    }
    GUILayout.EndScrollView();
  }

  void CreateMatch() {
    isCreatingMatch = true;
    Network.InitializeServer(1, serverPort, !Network.HavePublicAddress());
    MasterServer.RegisterHost(gameType, gameName);
    Application.LoadLevel(lobbySceneName);
  }

  void OnConnectedToServer() {
    Application.LoadLevel(lobbySceneName);
  }
}