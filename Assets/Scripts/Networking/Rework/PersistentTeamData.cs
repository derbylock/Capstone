using UnityEngine;
using System.Collections;

public class PersistentTeamData : MonoBehaviour {
  public NetworkPlayer[] teamOne;
  public NetworkPlayer[] teamTwo;

	// Use this for initialization
	void Start () {
    DontDestroyOnLoad(gameObject);
	}

  public void FillTeams(NetworkPlayer[] t1, NetworkPlayer[] t2) {
    teamOne = t1;
    teamTwo = t2;
  }
}
