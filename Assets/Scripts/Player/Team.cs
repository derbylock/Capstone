using UnityEngine;

[RequireComponent(typeof(NetworkView))]
public class Team : MonoBehaviour {
  public enum TargetType {
    Self,
    Ally,
    Enemy,
    All,
  };

	public int m_teamNumber = -1;
	
	[RPC]
	public void RecordTeamNumber(int teamNumber) {
		m_teamNumber = teamNumber;
	}

  public static bool IsValidTarget(TargetType targetType, GameObject target, GameObject me) {
    Team targetTeam = target.GetComponent<Team>();
    Team myTeam = me.GetComponent<Team>();

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    switch (targetType) {
      case TargetType.Self:
        if(target == me) {
          return true;
        }
        break;
      case TargetType.Ally:
        if(myTeam.m_teamNumber == targetTeam.m_teamNumber) {
          return true;
        }
        break;
      case TargetType.Enemy:
        if(myTeam.m_teamNumber != targetTeam.m_teamNumber) {
          return true;
        }
        break;
      case TargetType.All:
        return true;
      default:
        return false;
    };
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    return false;
  }
}