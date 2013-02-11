using UnityEngine;
using System.Collections;

public class SetColor : MonoBehaviour {
	public Color color = Color.white;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		renderer.material.color = color;
	}
}
