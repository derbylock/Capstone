using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimationData {
	public string animationName;
	public WrapMode animationWrapping;
	public Transform animationRootTransform;
	public bool recursiveMixing;
	public float animationWeight;
	public int animationLayer;
	public string[] mutExAnimations;
}