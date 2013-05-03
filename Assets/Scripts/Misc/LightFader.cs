using UnityEngine;
using System.Collections;

public class LightFader : MonoBehaviour {
  public float fadeTime;
  private float fadeStartTime;

  public void BeginFade() {
    networkView.RPC("Fade", RPCMode.All, (float)Network.time);
  }

  [RPC]
  IEnumerator Fade(float startTime) {
    fadeStartTime = startTime;

    float lightStrength = light.intensity;
    while (Network.time - fadeStartTime < fadeTime) {
      light.intensity = Mathf.Max(Mathf.Lerp(lightStrength, 0f, (float)(Network.time - fadeStartTime) / fadeTime), 0f);
      yield return new WaitForSeconds(Time.deltaTime);
    }

    Destroy(light);
  }
}