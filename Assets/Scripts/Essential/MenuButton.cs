using UnityEngine;
using System.Collections;

public class MenuButton : MonoBehaviour {
	
	public string sceneToLoad;
	public Color mouseoverColor = Color.red;
	
	public GameObject levelLoader;
	
	public bool quitButton = false;
	
	void Start() {
		
	}

	void OnMouseOver() {
		renderer.material.color = mouseoverColor;
		
		if(Input.GetMouseButtonDown(0)) {
			if(!quitButton) {
				LevelLoad loader = levelLoader.GetComponent(typeof(LevelLoad)) as LevelLoad;
				
				loader.levelToLoad = sceneToLoad;
				loader.LoadNewLevel();
			} else {
				Application.Quit();
			}
		}
	}
	
	void OnMouseExit() {
		renderer.material.color = Color.white;
	}
}