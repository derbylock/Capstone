using UnityEngine;
using System.Collections;

public class PlayerContainer : ScriptableObject {
	public NetworkPlayer m_player;		// The NetworkPlayer associated with this player
	public GameObject m_avatar;			// The GameObject which represents this player's character
	public Health m_health;				// The Health of this player
	public Mana m_mana;					// The Mana of this player
	public BuffContainer m_buffs;		// The Buffs affecting this player
	public int m_team;
	
	public int AvatarID {
		get { return m_avatar.GetInstanceID(); }
	}
}
