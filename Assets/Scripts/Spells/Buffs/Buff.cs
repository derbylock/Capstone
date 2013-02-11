using UnityEngine;
using System.Collections;
using System.Xml;

public class Buff : MonoBehaviour {
	public const string DEFAULT_FILEPATH = "";
	/****************************Necessary values for initialization****************************
	  * Name				NAME OF THE BUFF
	  * ID					NUMERIC VALUE OF THE BUFF
	  * Buff/Debuff			TAG WHICH CAN BE USED FOR VARIOUS EFFECTS
	  * Effect				WHAT THE BUFF DOES
	  * Effect Amount		CAN BE DAMAGE, PERCENTAGE, ETC
	  * Duration			HOW LONG DOES THE EFFECT LAST
	  * Tick Speed/Count	HOW FREQUENTLY DOES AN "OVER TIME" EFFECT ACTIVATE
	  * Effect Delay		HOW LONG TO WAIT BEFORE ACTIVATING
	  *****************************************************************************************/
	
	/***********************************File Format for Buffs***********************************
	 * #<ID>~<NAME>~<BUFF/DEBUFF>
	 * <EFFECT>~<EFFECT AMOUNT>
	 * <DURATION>~<TICK SPEED>~<EFFECT DELAY>
	 * 
	 * #001 Test Buff Buff
	 * Damage 100
	 * 5 0 5
	 ******************************************************************************************/
	
	public enum BuffEffect {
		Movement,
		DamageOverTime,
	}
	
	#region Variables
	private string _name;
	private int _id;
	private bool _isBuff;
	private BuffEffect _effect;
	private float _effectValue;
	private float _duration;
	private float _tickTime;
	private float _effectDelay;
	#endregion
	
	#region Accessors
	public string Name {
		get { return _name; }
		set { _name = value; }
	}
	public int ID {
		get { return _id; }
		set { _id = value; }
	}
	public bool IsBuff {
		get { return _isBuff; }
		set { _isBuff = value; }
	}
	public float EffectValue {
		get { return _effectValue; }
		set { _effectValue = value; }
	}
	public float Duration {
		get { return _duration; }
		set { _duration = value; }
	}
	public float TickSpeed {
		get { return _tickTime; }
		set { _tickTime = value; }
	}
	public float EffectDelay {
		get { return _effectDelay; }
		set { _effectDelay = value; }
	}
	#endregion
	
	/*public Buff(BuffEffect effect, float damage, bool isDot, float delay, float tickTime, float duration) {
		
	}*/
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public static Buff GetBuff(int id) {
		return GetBuff(DEFAULT_FILEPATH, id);
	}

	public static Buff GetBuff(string filepath, int id) {
		bool buffFound = false;
		string[] split = new string[1];
		System.IO.StreamReader reader = new System.IO.StreamReader(filepath);
		
		while(!buffFound && !reader.EndOfStream) {
			int parsedResult;
			
			// Start by reading the current line and determining if it is the 
			// start of a new buff description. If it is, then check to see if
			// it is the buff we're looking for.
			string currentLine = reader.ReadLine();
			if (currentLine[0] == '#') {
				currentLine = currentLine.Trim('#');
				split = currentLine.Split('~');
				// If we find the right buff, exit the loop and begin editing values
				if(System.Int32.TryParse(split[0], out parsedResult) && parsedResult == id) {
					buffFound = true;
				}
			}
		}
		
		if(buffFound) {
			// Custom assert (Debug.Assert does not work within Unity code)
			if(split.Length != 3) {
				Debug.LogError ("Buff #" + id + " found, first line not properly formatted.");
				return null;
			}
			
			Buff buff = new Buff();
			buff._id = id;
			buff._name = split[1];
			buff._isBuff = (string.Compare(split[2], "BUFF", true) == 0 ? true : false);	// If the strings are the same (return 0), it's a buff
			split = reader.ReadLine().Split('~');
			
			if(split.Length != 2) {
				Debug.LogError ("Buff #" + id + " found, second line not properly formatted.");
				return null;
			}
			
			buff._effect = (BuffEffect)System.Enum.Parse(typeof(BuffEffect), split[0]);
			buff._effectValue = System.Int32.Parse(split[1]);
			split = reader.ReadLine().Split('~');
			
			if(split.Length != 3) {
				Debug.LogError ("Buff #" + id + " found, third line not properly formatted.");
				return null;
			}
			
			buff._duration = System.Int32.Parse(split[0]);
			buff._tickTime = System.Int32.Parse(split[1]);
			buff._effectDelay = System.Int32.Parse(split[2]);
			
			return buff;
		} else {
			Debug.LogError ("No buff was found with the ID: " + id);
			return null;
		}
	}
}