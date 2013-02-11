using UnityEngine;
using System.Collections;

public class DrawReticle : MonoBehaviour {

	public Texture2D reticle;
	public float size = 0.05f;
	
	void OnGUI() {
		if(reticle) {
			float x, y;
			
			int textureSize = (int)(Screen.height*size);
			
			// Get the position of the texture. In both x and y we find Screen.height*size
			// so that the texture will be a perfect square
			x = Screen.width/2.0f - textureSize/2.0f;
			y = Screen.height/2.0f - textureSize/2.0f;
			
			GUI.DrawTexture(new Rect(x, y, textureSize, textureSize), reticle);
		}
	}
}