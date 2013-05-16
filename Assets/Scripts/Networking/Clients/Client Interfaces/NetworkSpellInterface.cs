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
  private RuneLimiter runeLimiter;

  private float lastCastTime = 0f;

  public KeyCode primaryAttack;
  public KeyCode secondaryAttack;
  public KeyCode specialAttack;
  public KeyCode fourthAttack;

  public GameObject myCamera;

  // This will be private later, once gets are implemented
  public GameObject[] spells;
  public Transform handJoint;

  private Animator animator;

  public int myTeam = -1;
  void Start() {
    if (!networkView.isMine) {
      this.enabled = false;
    }
    animator = transform.GetComponent<Animator>();
    myCamera = GameObject.FindGameObjectWithTag("Follow Camera");
  }

  void Update() {

    if (networkView.isMine) {
      if (Input.GetKeyDown(primaryAttack)) {
        if (Network.isServer) {
          ServerCastSpell(0);
        } else {
          networkView.RPC("CastSpell", RPCMode.Server, Network.player, 0, myTeam, GetCastEndpoint());
        }
        animator.SetBool("IsAttacking", true);
      } else if (Input.GetKeyDown(secondaryAttack)) {
        if (Network.isServer) {
          ServerCastSpell(1);
        } else {
          networkView.RPC("CastSpell", RPCMode.Server, Network.player, 1, myTeam, GetCastEndpoint());
        }
        animator.SetBool("IsAttacking", true);
      } else if (Input.GetKeyDown(specialAttack)) {
        if (Network.isServer) {
          ServerCastSpell(2);
        } else {
          networkView.RPC("CastSpell", RPCMode.Server, Network.player, 2, myTeam, GetCastEndpoint());
        }
        animator.SetBool("IsAttacking", true);
      } else if (Input.GetKeyDown(fourthAttack)) {
        if (Network.isServer) {
          ServerCastSpell(3);
        } else {
          networkView.RPC("CastSpell", RPCMode.Server, Network.player, 3, myTeam, GetCastEndpoint());
        }
        animator.SetBool("IsAttacking", true);
      } else {
        //animator.SetBool("IsAttacking", false);
      }
    }
  }

  [RPC]
  void CastSpell(NetworkPlayer player, int spellArrLoc, int team, Vector3 castTo) {
    Debug.Log("Player " + player + " is trying to cast spell #" + spellArrLoc);
    Debug.Log("Spell cast from scene object " + transform.name);
    // Don't even touch the function if they've already cast too recently.
    if (Time.time - lastCastTime < Constants.MIN_RECAST_TIME) {
      return;
    }

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

    // If we have the spell script and we can subtract the proper amount of
    // mana from this user's mana pool, then we can begin to cast the spell.
    // TODO: WRITE THAT CODE ^
    Mana mana = GetCasterMana(player);

    if (spellScript && spellScript.WillCastSuccessfully(spawnPosition, castTo) &&
       mana.DrainMana(spellScript.m_manaCost)) {
      // Run the attack animation over the network
      //networkView.RPC("StartNetworkAnimation", RPCMode.All, "attack", (float)Network.time);

      spellScript.m_Team = myTeam;

      if (spellScript.handLatch) {
        spellScript.latchingJoint = handJoint;
      }
      //spellScript.m_myCaster = transform.root;
      spellScript.SetCaster(transform.root);
      spellScript.Cast(spawnPosition, castTo);

      if (spellScript.GetType() == typeof(RuneSpell)) {
        RuneLimiter runeLimiter = GameObject.FindObjectOfType(typeof(RuneLimiter)) as RuneLimiter;
        runeLimiter.AddRune(transform.root, spellScript.transform);
      }

      // Set the time of the cast
      lastCastTime = Time.time;
    } else {
      Network.Destroy(spellScript.gameObject);
    }
  }

  void ServerCastSpell(int spell) {
    CastSpell(Network.player, spell, myTeam, GetCastEndpoint());
  }

  // Old version of this function, too much code to do nothing extra.
  /*void ServerCastSpell(int spellArrLoc) {
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
      networkView.RPC("StartNetworkAnimation", RPCMode.All, "attack", (float)Network.time);

      spellScript.m_Team = myTeam;
      if (spellScript.handLatch) {
        spellScript.latchingJoint = handJoint;
      }
      spellScript.m_myCaster = transform.root;
      if (spellScript.GetType().BaseType == typeof(AreaSpell)) {
        spellScript.Cast(spawnPosition, GetCastEndpoint());

        DebugLine newLine = new DebugLine();
        newLine.start = spawnPosition;
        newLine.end = GetCastEndpoint();
        lines.Add(newLine);
      } else {
        spellScript.Cast(GetCastEndpoint());
      }
    }
  }*/

  Vector3 GetCastEndpoint() {
    if (!myCamera) {
      myCamera = GameObject.FindGameObjectWithTag("Follow Camera");
    }

    RaycastHit hit;
    if (!myCamera) {
    }
    if (Physics.Raycast(/*(handJoint ? handJoint.position : transform.position)*/myCamera.transform.position,
               myCamera.transform.forward,
               out hit)) {
      return hit.point;
    } else {
      Debug.LogError("A spell was cast at a location which does not have an endpoint.");
      return new Vector3();
    }
  }

  Mana GetCasterMana(NetworkPlayer player) {
    GameObject managerObj = GameObject.Find("Player Data Manager");
    PlayerDataManager manager = managerObj.GetComponent<PlayerDataManager>();

    return manager.GetMana(player);
  }
}