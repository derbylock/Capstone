using UnityEngine;
using System.Collections;

public class CharacterSelection : MonoBehaviour {
  public bool isWaiting = true;
  public bool isSelecting;
  public string prefabFolder = "PlayableCharacters";
  private string defaultIcon = "PlayableCharacters/Icons/NoIcon.png";
	
  private GameObject[] characters;
  private Texture2D[] textures;
  public Texture2D noIcon;
	
  public float iconSize = 32f;
  public float hIconBuffer = 8f;
  public float vIconBuffer = 16f;
  private float sideBuffer;       // Space between sides of the window and the icons
  public Rect iconScreenAllotment = new Rect(0.05f, 0.05f, 0.9f, 0.5f);
  private Rect charWindowSize;
  private int rows, cols;

  private bool useScrollArea;
  private Vector2 scrollPosition;
    
	#region Character Spawning NYI
	// A list of points as screenspace coordinates which will be
	// used to determine the proper positioning of the characters.
	// This information is used to fill the dynamicSpawnPoints array.
	public Vector2[] characterSpawnPoints;
	// The actual worldspace coordinates of each character's spawn
	// during the character select sequence.
	private Vector3[] dynamicSpawnPoints; 
	#endregion
  private GameObject localSelectedCharacter;
  private GameObject instancedCharacter;
  public Transform tempSpawnPoint;
	
	void Start() {
		Object[] temp = Resources.LoadAll(prefabFolder, typeof(GameObject));
		Debug.Log("Length of temp: " + temp.Length);
    characters = new GameObject[temp.Length];
    for(int i=0; i < temp.Length; ++i) {
      Debug.Log("Loading Character " + i);
	  Debug.Log("temp array @ " + i + " is " + (temp[i] == null ? "null" : "not null"));
      characters[i] = temp[i] as GameObject;
	  Debug.Log("character array write status: " + (characters[i] == null ? "failed" : "success"));
    }

		textures = new Texture2D[characters.Length];

    charWindowSize = new Rect(Screen.width * iconScreenAllotment.x,
                              Screen.height * iconScreenAllotment.y,
                              Screen.width * iconScreenAllotment.width,
                              Screen.height * iconScreenAllotment.height);
		
		// Get character icons for GUI
		for(int i=0; i<characters.Length; ++i) {
			CharacterData charData = characters[i].GetComponent<CharacterData>();
      Debug.Log(characters[i].name);
      Debug.Log("charData at " + i + " == null -> " + charData == null);
			textures[i] = (charData.icon != null ? charData.icon : null);
			if(textures[i] == null) {
				textures[i] = Resources.Load(defaultIcon, typeof(Texture2D)) as Texture2D;
			}
		}

    //noIcon = Resources.Load(defaultIcon, typeof(Texture)) as Texture;
    Debug.Log(noIcon);

    // Determine rows and cols needed and whether we need a scroll area or not
    cols = GetMaxFit(charWindowSize.width, iconSize, hIconBuffer);
    rows = Mathf.CeilToInt((float)characters.Length / cols);
    useScrollArea = rows * (iconSize + vIconBuffer) > charWindowSize.height;
    sideBuffer = (charWindowSize.width - (cols * iconSize + (cols - 1) * hIconBuffer)) / 2f;
	}

    int GetMaxFit(float totalSpace, float objSize, float bufferSize) {
      // There may be some scenarios where this function causes an icon that would
      // fit onto the next line, but it is a simple enough approximation.
      return Mathf.FloorToInt(totalSpace/(objSize+bufferSize));
    }
	
	void OnGUI() {
    if (!isWaiting) {
      if (isSelecting) {
        GUI.Window(Constants.WINID_CHAR_SELECT,
                    charWindowSize,
                    CharacterSelectWindow,
                    "Select A Character");
      } else {
        if (GUI.Button(new Rect(Screen.width / 2 - 75,
                                Screen.height / 6 * 5,
                                150, 30),
                      "Select Character")) {
          isSelecting = true;
          LobbyManager lobby = GameObject.FindObjectOfType(typeof(LobbyManager)) as LobbyManager;
          lobby.networkView.RPC("ReadyCheck", RPCMode.Server, Network.player, false);
        }
      }
    }
	}

  void CharacterSelectWindow(int winId) {
    GUILayout.Space(5f);

    if(useScrollArea) {
      scrollPosition = GUILayout.BeginScrollView(scrollPosition);
    }

    for (int i = 0; i < rows; ++i) {
      GUILayout.BeginHorizontal();

      for (int j = 0; j < cols && i * cols + j < characters.Length; ++j) {
        GUILayout.FlexibleSpace();

        CharacterData current = characters[i*cols+j].GetComponent<CharacterData>();
        GUIContent character = new GUIContent(/*current.name, */(current.icon ? current.icon  : noIcon));

        if (GUILayout.Button(character, GUILayout.Width(iconSize), GUILayout.Height(iconSize))) {
          Debug.Log("Selected: " + current.name);
          localSelectedCharacter = characters[i*cols+j];
          Network.Destroy (instancedCharacter);
          instancedCharacter = Network.Instantiate(characters[i*cols+j], tempSpawnPoint.position, tempSpawnPoint.rotation, 0) as GameObject;
          isSelecting = false;

          LobbyManager lobby = GameObject.FindObjectOfType(typeof(LobbyManager)) as LobbyManager;
          lobby.networkView.RPC("ReadyCheck", RPCMode.Server, Network.player, true);
          CharacterSpawnData data = GameObject.FindObjectOfType(typeof(CharacterSpawnData)) as CharacterSpawnData;
          data.SelectCharacter(current);
        }

        GUILayout.FlexibleSpace();
      }

      GUILayout.EndHorizontal();
    }

    if (useScrollArea) {
      GUILayout.EndScrollView();
    }
  }
}