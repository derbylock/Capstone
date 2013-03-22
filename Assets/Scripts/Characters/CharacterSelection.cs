using UnityEngine;
using System.Collections;

public class CharacterSelection : MonoBehaviour {
	private bool isSelecting;
	public string prefabFolder = "PlayableCharacters";
	private string defaultIcon = "PlayableCharacters\\Icons\\NoIcon"
	
	private Object[] characters;
	private Texture2D[] textures;
	
	public float iconSize = 32f;
	public float hIconBuffer = 8f;
	public float vIconBuffer = 16f;
	public Rect iconScreenAllotment = new Rect(0.05f, 0.05f, 0.9f, 0.5f);
	private int rows, cols;
	
	#region Character Spawning NYI
	// A list of points as screenspace coordinates which will be
	// used to determine the proper positioning of the characters.
	// This information is used to fill the dynamicSpawnPoints array.
	public Vector2[] characterSpawnPoints;
	// The actual worldspace coordinates of each character's spawn
	// during the character select sequence.
	private Vector3[] dynamicSpawnPoints; 
	#endregion
	
	void Start() {
		characters = Resources.LoadAll(prefabFolder, typeof(GameObject));
		textures = new Texture2D[characters.Length];

        iconScreenAllotment.x = Screen.width*iconScreenAllotment.x;
        iconScreenAllotment.y = Screen.height*iconScreenAllotment.y;
        iconScreenAllotment.width = Screen.width*iconScreenAllotment.width;
        iconScreenAllotment.height = Screen.height*iconScreenAllotment.height;
		
		// Get character icons for GUI
		for(int i=0; i<characters.Length; ++i) {
			CharacterData charData = ((GameObject)characters[i]).GetComponent<CharacterData>();
			textures[i] = charData.icon;
			if(textures[i] == null) {
				textures[i] = Resournces.Load(defaultIcon, typeof(Texture2D)) as Texture2D;
			}
		}

        // Determine rows and cols needed and whether we need a scroll area or not

	}

    int GetMaxFit(float totalSpace, float objSize, float bufferSize) {
        int max = Mathf.FloorToInt(totalSpace/objSize);

        return max;
    }
	
	void OnGui() {
		if(isSelecting) {
			
		}
	}
}