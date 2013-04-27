using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Spell : MonoBehaviour {
	#region Effect Definition
	public enum EffectType {
		APPLY_BUFF,
		DIRECT_DAMAGE,
		HEAL,
		KNOCKBACK,
		SPLASH_DAMAGE,
	}
	public enum EffectTargets {
		SELF,
		ALLY,
		ENEMY,
		ALL,
	}
	
	[System.Serializable]
	public class Effect {
		public EffectType type;
		public EffectTargets targets;
	}
	#endregion
		
	public string m_name;					// Name of the spell
	public uint m_id;						// The ID of the spell
	public float m_manaCost;				// Mana cost to successfully cast
	public float m_directDamage;			// Total amount of direct damage done
	public float m_directHeal;				// Total amount of direct healing done
	public float m_knockbackForce;			// The force of the knockback
	public float m_maxDistance;				// The farthest that this spell will cast to
	public int m_Team = -1;					// Team of the caster
	public float m_splashRange;
	public float m_splashDamage;
	public Effect[] m_effects = new Effect[2];
	public int[] buffIDs = new int[3];
	private Buff[] m_buffs;
	
	// Flavor variables
	public bool handLatch;			// Does this spell latch onto the avatar's hand when first cast?
	public float latchDuration;		// The duration that the spell remains attached to the player's hand if it does so
	
	[HideInInspector]
	public Transform latchingJoint;	// The joint which this spell latches onto, set by the instantiator
	[HideInInspector]
	public Transform m_myCaster;
	private PlayerContainer[] enemies;
	private PlayerContainer[] allies;

  protected SpellBundle bundle;

  void Awake() {
    bundle = new SpellBundle();
    bundle.rootSpell = this;
  }

	void Start() {
		// Shut this script down if it's on a client
		if(!Network.isServer) {
			this.enabled = false;
		}
		
		/*// If this spell applies buffs, then find out which ones.
		bool appliesBuffs = false;
		//foreach(EffectType e in m_effects) {
			//if(e == EffectType.APPLY_BUFF) {
		for(int i=0; i<m_effects.Length; i++) {
			if(m_effects[i].type == EffectType.APPLY_BUFF) {
				appliesBuffs = true;
				break;
			}
		}
		
		// Load in buffs if this spell applies them
		if (appliesBuffs) {
			List<Buff> tempBuffs = new List<Buff>();
			foreach (int i in buffIDs) {
				Buff temp = Buff.GetBuff(i);
				if(temp) {
					tempBuffs.Add (temp);
				}
			}
			m_buffs = tempBuffs.ToArray();
		} else {
			m_buffs = null;
		}*/
	}
	
	void Update() {
		if(enemies != null && m_Team != -1) {
			GameObject m = GameObject.Find("Player Data Manager");
			PlayerDataManager manager = m.GetComponent<PlayerDataManager>();
			//allies = manager.GetMyTeam(m_Team);
			//enemies = manager.GetEnemyTeam(m_Team);
		}
	}
	
  // Deprecated
  // Reason: Unnecessary overload, never used
	public virtual void Cast() {
		// Intentionally Blank
	}
	
  // Deprecated
  // Reason: Unnecessary overload, there is no use for having multiple
  // calls for different types of spells; simply call Cast(from, to)
  // and ignore the from variable within the specific class method override.
	public virtual void Cast(Vector3 castTo) {
		// Intentionally Blank
	}
	
	public virtual void Cast(Vector3 castFrom, Vector3 castTo) {
		bundle.startPoint = castFrom;
    bundle.endPoint = castTo;
	}
	
  // Used to determine beforehand if a spell will actually be cast.
  // This is mainly important for targetted spells which have a set
  // maximum range. All spell types which do not require this should
  // not overwrite this function.
  // Current spells which do not need this functionality:
  //  - Projectile
  //  - Blast
  //  - Cone
  // The following spells do not need this implemented within their
  // respective scripts:
  //  - Area of Effect
  //    - Reason: Implemented in superclass (AreaSpell)
  //  - Rune
  //    - Reason: Implemented in superclass (AreaSpell)
	public virtual bool WillCastSuccessfully(Vector3 castFrom, Vector3 castTo) {
		return true;
	}
	
	#region Applying Effects
	protected void TriggerEffects(params GameObject[] targets) {
		for(int j=0; j < m_effects.Length; j++) {
			switch(m_effects[j].type) {
			case EffectType.APPLY_BUFF:
				ApplyBuffs(targets, m_effects[j].targets);
				Debug.LogError ("NYI: Apply Buff");
				break;
			case EffectType.DIRECT_DAMAGE:
				ApplyDamage(targets, m_effects[j].targets);
				break;
			case EffectType.HEAL:
				ApplyHeal(targets, m_effects[j].targets);
				break;
			case EffectType.KNOCKBACK:
				ApplyKnockback(targets, m_effects[j].targets);
				Debug.LogError ("NYI: Knockback needs to be implemented in PlayerController.");
				break;
			case EffectType.SPLASH_DAMAGE:
				ApplySplash(m_effects[j].targets);
				break;
			default:
				Debug.LogError ("Attempted to parse " + m_effects[j].type.ToString() + " but no method was found.");
				break;
			};
		}
	}
	
	
	/// <summary>
	/// Applies all of this spell's buffs to each target.
	/// </summary>
	/// <param name='targets'>
	/// Targets.
	/// </param>
	protected void ApplyBuffs(GameObject[] targets, EffectTargets validTargets) {
		//PlayerContainer[] validPlayers = GrabValidTargets(validTargets);
	}
	
	/// <summary>
	/// Deals damage to all targets.
	/// </summary>
	/// <param name='targets'>
	/// Targets.
	/// </param>
	protected void ApplyDamage(GameObject[] targets, EffectTargets validTargets) {
		//Debug.Log ("Dealing Damage");
		PlayerContainer[] validPlayers = GrabValidTargets(validTargets);
		if(validPlayers != null) {
			//Debug.Log ("Valid players exist");
			for(int i=0; i<targets.Length; i++) {	// Loop through all targets
				for(int j=0; j<validPlayers.Length; j++) {	// Loop through all enemies we have cached
					if(validPlayers[j] != null && targets[i] == validPlayers[j].m_avatar) {
						//Debug.Log("Found a target");
						validPlayers[j].m_health.ReceiveDamage(m_directDamage);
					}
				}
			}
		}
	}
	
	/// <summary>
	/// Applies a heal to all targets.
	/// </summary>
	/// <param name='targets'>
	/// Targets.
	/// </param>
	protected void ApplyHeal(GameObject[] targets, EffectTargets validTargets) {
		PlayerContainer[] validPlayers = GrabValidTargets(validTargets);
		for(int i=0; i<targets.Length; i++) {	// Loop through all targets
			for(int j=0; j<validPlayers.Length; i++) {	// Loop through all allies we have cached
				if(targets[i] == validPlayers[i].m_avatar) {
					enemies[i].m_health.ReceiveHeal(m_directHeal);
				}
			}
		}
	}
	
	/// <summary>
	/// Applies a knockback to all targets.
	/// </summary>
	/// <param name='targets'>
	/// Targets.
	/// </param>
	protected void ApplyKnockback(GameObject[] targets, EffectTargets validTargets) {
		//PlayerContainer[] validPlayers = GrabValidTargets(validTargets);
	}
	
	protected void ApplySplash(EffectTargets validTargets) {
		//Debug.Log ("Disabled Temporarily");
		/*PlayerContainer[] validPlayers = GrabValidTargets(validTargets);
		RaycastHit[] hits = Physics.SphereCastAll(transform.position, m_splashRange, transform.forward);
		
		for(int i=0; i<hits.Length ; i++) {
			if(hits[i].transform.tag == "Player") {
				for(int j=0; j<validPlayers.Length; j++) {
					if(hits[i].transform.gameObject == validPlayers[j].m_avatar) {
						validPlayers[j].m_health.ReceiveDamage(m_splashDamage);
					}
				}
			}
		}*/
	}
	#endregion
	
	PlayerContainer[] GrabValidTargets(EffectTargets targetType) {
		//Debug.Log ("Searching targets");
		GameObject manager = GameObject.Find ("Player Data Manager") as GameObject;
		PlayerDataManager pManager = manager.GetComponent<PlayerDataManager>();
		
		switch(targetType) {
		case EffectTargets.ALL:
			List<PlayerContainer> temp = new List<PlayerContainer>();
			temp.AddRange(allies);
			temp.AddRange(enemies);
			return temp.ToArray();
		case EffectTargets.ALLY:
			return allies;
		case EffectTargets.ENEMY:
			return pManager.GetEnemyTeam(m_Team);
		case EffectTargets.SELF:
			//Debug.LogError ("Self targetting NYI.");
			return null;
		default:
			return null;
		};
	}

  public void SetCaster(Transform caster) {
    m_myCaster = caster;
    bundle.caster = caster.gameObject;
  }
}