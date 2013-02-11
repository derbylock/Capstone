using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The NetworkSpellInterface includes both clientside and serverside logic for
/// the casting of spells through the network. It includes the following services:
///   Client:
///		-An RPC function which tells the server what spell the player wishes to use
///   Server:
/// 	-Check for availability of mana and other requirement completion for a caster
/// 	 who has issued an attack
/// 	-Gathering of proper spells 
/// 	-A function 
/// </summary>
public class NetworkSpellInterface : MonoBehaviour {
	public struct DebugLine {
		public Vector3 start;
		public Vector3 end;
	}; 
	private List<DebugLine> lines = new List<DebugLine>();
	
	public KeyCode primaryAttack;
	public KeyCode secondaryAttack;
	public KeyCode specialAttack;
  public KeyCode fourthAttack;
	
	public GameObject myCamera;
	
	// This will be private later, once gets are implemented
	public GameObject[] spells;
	public Transform handJoint;
	
	private Vector3 debugLineStart;
	private Vector3 debugLineEnd;
	public int myTeam = -1;
	void Start() {
		if(!networkView.isMine) {
			this.enabled = false;
		}
		myCamera = GameObject.FindGameObjectWithTag("Follow Camera");
	}
	
	void Update() {
		foreach(DebugLine line in lines) {
			Debug.DrawLine(line.start, line.end, Color.red);
		}
		
		if(networkView.isMine) {
			if(Input.GetKeyDown(primaryAttack)) {
				if(Network.isServer) {
					ServerCastSpell(0);
				} else {
					networkView.RPC ("CastSpell", RPCMode.Server, Network.player, 0, myTeam, GetCastEndpoint());
				}
			} else if(Input.GetKeyDown(secondaryAttack)) {
				if(Network.isServer) {
					ServerCastSpell(1);
				} else {
					networkView.RPC ("CastSpell", RPCMode.Server, Network.player, 1, myTeam, GetCastEndpoint());
				}
			} else if(Input.GetKeyDown(specialAttack)) {
				if(Network.isServer) {
					ServerCastSpell(2);
				} else {
					networkView.RPC ("CastSpell", RPCMode.Server, Network.player, 2, myTeam, GetCastEndpoint());
				}
      } else if (Input.GetKeyDown(fourthAttack)) {
        if (Network.isServer) {
          ServerCastSpell(3);
        } else {
          networkView.RPC("CastSpell", RPCMode.Server, Network.player, 3, myTeam, GetCastEndpoint());
        }
      }
		}
	}
	
	[RPC]
	void CastSpell(NetworkPlayer player, int spellArrLoc, int team, Vector3 castTo) {
		Debug.Log("Casting");
		Vector3 spawnPosition;
		Quaternion spawnRotation;
		
		if(handJoint) {
			spawnPosition = handJoint.position;
			spawnRotation = handJoint.rotation;
		} else {
			spawnPosition = transform.position;
			spawnRotation = transform.rotation;
		}
		
		GameObject spellInstance = Network.Instantiate(spells[spellArrLoc], spawnPosition, spawnRotation, 0) as GameObject;
		Spell spellScript = spellInstance.GetComponent<Spell>();
		
		// If we have the spell script and we can subtract the proper amount of
		// mana from this user's mana pool, then we can begin to cast the spell.
		// TODO: WRITE THAT CODE ^
		Mana mana = GetCasterMana(player);
		Debug.Log ("Player " + player + " mana found: " + (mana != null));
		if(spellScript && mana.DrainMana(spellScript.m_manaCost)) {
			// Run the attack animation over the network
			networkView.RPC ("StartNetworkAnimation", RPCMode.All, "attack", (float)Network.time);
			
			spellScript.m_Team = myTeam;
            
			if(spellScript.handLatch) {
				spellScript.latchingJoint = handJoint;
			}
			spellScript.m_myCaster = transform.root;
			
			if(spellScript.GetType().BaseType == typeof(AreaSpell)) {
				spellScript.Cast(spawnPosition, castTo);
				
				DebugLine newLine = new DebugLine();
				newLine.start = spawnPosition;
				newLine.end = castTo;
				lines.Add(newLine);
			} else {
				spellScript.Cast(castTo);
			}
		}
	}
	
	/// <summary>
	/// Allows a server player to cast a spell.
	/// </summary>
    void ServerCastSpell(int spellArrLoc)
    {
        Debug.Log("Casting");
        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (handJoint) {
            spawnPosition = handJoint.position;
            spawnRotation = handJoint.rotation;
        } else {
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
        }

        GameObject spellInstance = Network.Instantiate(spells[spellArrLoc], spawnPosition, spawnRotation, 0) as GameObject;
        Spell spellScript = spellInstance.GetComponent<Spell>();
        Mana mana = GetCasterMana(Network.player);
        if (spellScript && mana.DrainMana(spellScript.m_manaCost)) {
			// Run the attack animation over the network
			networkView.RPC ("StartNetworkAnimation", RPCMode.All, "attack", (float)Network.time);
			
            spellScript.m_Team = myTeam;
            if (spellScript.handLatch) {
                spellScript.latchingJoint = handJoint;
            }
            spellScript.m_myCaster = transform.root;
			if(spellScript.GetType().BaseType == typeof(AreaSpell)) {
				spellScript.Cast(spawnPosition, GetCastEndpoint());
				
				DebugLine newLine = new DebugLine();
				newLine.start = spawnPosition;
				newLine.end = GetCastEndpoint();
				lines.Add(newLine);
			} else {
            	spellScript.Cast(GetCastEndpoint());
			}
        }
    }
	
	Vector3 GetCastEndpoint() {
		if(!myCamera) {
			myCamera = GameObject.FindGameObjectWithTag("Follow Camera");
		}
		
		RaycastHit hit;
		if(!myCamera) {
			Debug.Log ("Missing camera");
		}
		if(Physics.Raycast(/*(handJoint ? handJoint.position : transform.position)*/myCamera.transform.position,
						   myCamera.transform.forward,
						   out hit)) {
			return hit.point;
		} else {
			Debug.LogError("A spell was cast at a location which does not have an endpoint.");
			debugLineStart = transform.position;
			debugLineEnd = myCamera.transform.forward;
			return new Vector3();
		}
	}

    Mana GetCasterMana(NetworkPlayer player)
    {
        GameObject managerObj = GameObject.Find("Player Data Manager");
        PlayerDataManager manager = managerObj.GetComponent<PlayerDataManager>();

        return manager.GetMana(player);
    }
}