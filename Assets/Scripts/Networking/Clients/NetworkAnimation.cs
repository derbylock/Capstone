using UnityEngine;
using System.Collections;

public class NetworkAnimation : MonoBehaviour {
	public AnimationData[] m_anims;
	public string m_defaultAnimation;
	
	/**************************************************************************
	 * Here we need to set the animation states of certain m_anims so that
	 * they exhibit the proper features. Here we do things like making sure 
	 * run m_anims loop and death m_anims never end.
	 *************************************************************************/
	void Start() {
		for(int i=0; i<m_anims.Length; i++) {
			animation[m_anims[i].animationName].wrapMode = m_anims[i].animationWrapping;
			if(m_anims[i].animationRootTransform) {
				animation[m_anims[i].animationName].AddMixingTransform(m_anims[i].animationRootTransform, m_anims[i].recursiveMixing);
			}
			animation[m_anims[i].animationName].layer = m_anims[i].animationLayer;
			animation[m_anims[i].animationName].weight = m_anims[i].animationWeight;
			animation[m_anims[i].animationName].speed = 2f;
		}
		
		StartAnimation(m_defaultAnimation);
	}
	
	/// <summary>
	/// Plays the given animation from the start of the clip.
	/// </summary>
	public void StartAnimation(string animationName) {
		//Debug.Log ("Starting animation: " + animationName);
		int indexOf = GetIndex (animationName);
		for(int i=0; i<m_anims[indexOf].mutExAnimations.Length; i++) {
			StopAnimation (m_anims[indexOf].mutExAnimations[i]);
		}
		
		animation[animationName].time = 0f;
		//animation[animationName].enabled = true;
		
		// TODO: alter AnimationData class to include the functionality below
		if(animationName == "attack") {
			animation.Play(animationName);
		} else {
			animation.CrossFade(animationName);
		}
	}
	
	[RPC]
	public void StartNetworkAnimation(string animationName, float timestamp) {
		if(!animation.IsPlaying(animationName)) {
			//Debug.Log (animationName + " is not playing, attempting play now");
			StartAnimation(animationName, (float)Network.time-timestamp);
		}
	}
	
	/// <summary>
	/// Plays the given animation starting at the time chosen.
	/// </summary>
	public void StartAnimation(string animationName, float startAt) {
		//Debug.Log ("Starting animation: " + animationName);
		int indexOf = GetIndex (animationName);
		for(int i=0; i<m_anims[indexOf].mutExAnimations.Length; i++) {
			StopAnimation (m_anims[indexOf].mutExAnimations[i]);
		}
		
		animation[animationName].time = startAt;
		//animation[animationName].enabled = true;
		
		// TODO: alter AnimationData class to include the functionality below
		if(animationName == "attack") {
			animation.Play(animationName);
		} else {
			animation.CrossFade(animationName);
		}
	}
	
	/// <summary>
	/// Stops the given animation.
	/// </summary>
	public void StopAnimation(string animationName) {
		animation.Stop(animationName);
		//animation[animationName].time=0f;
		//animation[animationName].enabled = false;
	}
	
	int GetIndex(string animation) {
		for(int i=0; i<m_anims.Length; i++) {
			if(m_anims[i].animationName == animation) {
				return i;
			}
		}
		Debug.LogError("Attempted to find the index of: " + animation + " in m_anims, but no animation was found.");
		return -1;
	}
}