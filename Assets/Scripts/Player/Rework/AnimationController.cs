using UnityEngine;

public class AnimationController : MonoBehaviour {
  public Vector3 direction;
  public bool isJumping;
  public bool isAirborn;
  public bool isAttacking;

  public Animator animator;

  void Awake() {

  }

  void Start() {
    animator = gameObject.GetComponent<Animator>();
  }

  void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
    Vector3 dir = Vector3.zero;
    bool jump = false;
    bool air = false;
    bool attack = false;
    if (stream.isWriting && networkView.owner == Network.player) {
      /*dir = direction;
      jump = isJumping;
      air = isAirborn;
      attack = isAttacking;
      stream.Serialize(ref dir);
      stream.Serialize(ref jump);
      stream.Serialize(ref air);
      stream.Serialize(ref attack);*/
      stream.Serialize(ref direction);
      stream.Serialize(ref isJumping);
      stream.Serialize(ref isAirborn);
      stream.Serialize(ref isAttacking);
    } else {
      stream.Serialize(ref direction);
      stream.Serialize(ref isJumping);
      stream.Serialize(ref isAirborn);
      stream.Serialize(ref isAttacking);
      /*stream.Serialize(ref dir);
      stream.Serialize(ref jump);
      stream.Serialize(ref air);
      stream.Serialize(ref attack);
      direction = dir;
      isJumping = jump;
      isAirborn = air;
      isAttacking = attack;*/
    }
  }

  void Update() {
    animator.SetFloat("Strafe", direction.x);
    animator.SetFloat("Straight", direction.z);
    animator.SetFloat("Magnitude", direction.magnitude);
    animator.SetBool("IsJumping", isJumping);
    animator.SetBool("IsFalling", isAirborn);
    animator.SetBool("IsAttacking", isAttacking);
  }
}