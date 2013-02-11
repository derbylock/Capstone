using UnityEngine;
using System.Collections;

public class LevelLoad : MonoBehaviour {
	public Texture2D blackScreen;
	public float fadeTime;
	private bool runOnce;
	
	public string levelToLoad;
	

	// Use this for initialization
	void Start () {
		// Make sure this stays alive when a new level is loaded
		DontDestroyOnLoad(gameObject);
		
		//Set up the texture so that the
		guiTexture.enabled = false;
		guiTexture.pixelInset = new Rect(-Screen.width/2, -Screen.height/2, Screen.width, Screen.height);
		
		runOnce = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.KeypadEnter)) {
			guiTexture.enabled = true;
			Application.LoadLevel(levelToLoad);
		}
		
		if(runOnce == false && !Application.isLoadingLevel && Application.loadedLevelName == levelToLoad) {
			runOnce = true;
			guiTexture.texture = blackScreen;
			StartCoroutine("FadeIn");
		}
	}
	
	public void LoadNewLevel() {
		guiTexture.enabled = true;
		Application.LoadLevel(levelToLoad);
		
		if(runOnce == false && !Application.isLoadingLevel && Application.loadedLevelName == levelToLoad) {
			runOnce = true;
			guiTexture.texture = blackScreen;
			StartCoroutine("FadeIn");
		}
	}
	
	IEnumerator FadeIn() {
		float i = 1.0f;
		
		while(i != 0.0f) {
			//i -= Time.deltaTime/fadeTime;
			//if(i<0.0f) { i = 0.0f; }
			i = Mathf.Max(i-(Time.deltaTime), 0.0f);
			//Debug.Log (i);
			
			guiTexture.color = Color.Lerp(new Color(1f, 1f, 1f, 0f), Color.white, i);
			
			yield return new WaitForSeconds(Time.deltaTime);
		}
		
		Destroy(guiTexture);
		Destroy(gameObject);
	}
	
	void OnGUI() {
		/*if(Application.isLoadingLevel) {
			texture.enabled = true;
			loadText.enabled = true;
		} else {
			texture.enabled = false;
			loadText.enabled = false;
		}*/
	}
}