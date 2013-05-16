using UnityEngine;

public class AnimationController : MonoBehaviour {
  public Vector3 direction;
  public bool isJumping;
  public bool isAirborn;
  public bool isAttacking;

  public Animator animator;

  void Start() {
    animator = gameObject.GetComponent<Animator>();
  }

  void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
    if (stream.isWriting && networkView.owner == Network.player) {
      stream.Serialize(ref direction);
      stream.Serialize(ref isJumping);
      stream.Serialize(ref isAirborn);
      stream.Serialize(ref isAttacking);
    } else {
      stream.Serialize(ref direction);
      stream.Serialize(ref isJumping);
      stream.Serialize(ref isAirborn);
      stream.Serialize(ref isAttacking);
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