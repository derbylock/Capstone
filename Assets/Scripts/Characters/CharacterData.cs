using UnityEngine;
using System.Collections;

public class CharacterData : ScriptableObject {
	public string devName;		// Used for loading Resources
	public string name;			// Actual character name
	public string magicType;	// Type of magic they use
	public Texture2D icon;		// Icon to display
}