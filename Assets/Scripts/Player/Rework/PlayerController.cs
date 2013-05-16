using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkView))]
public class PlayerController : MonoBehaviour {
  public bool canControl = true;
  public float speed = 8f;

  private float airbornStartTime;
  public float gravity = 9.81f;
  public float jumpHeight = 10f;
  public float airDrag = 0.1f;
  private bool isJumping;
  private bool isGrounded;
  private bool wasGrounded;

  private Vector3 airbornTrajectory = Vector3.zero;
  public float airbornControlAmount;

  private CharacterController controller;
  private Vector3 groundNormal = Vector3.zero;

  private Animator animator;
  private AnimationController anim;


  void Start() {
    controller = gameObject.GetComponent<CharacterController>();
    animator = transform.GetComponent<Animator>();
    anim = transform.GetComponent<AnimationController>();
  }

  void Update() {
    if (!networkView.isMine || Time.timeScale == 0f) {
      return;
    }
    Vector3 inputTrajectory = Vector3.zero;

    //---------------------Input Management---------------------
    if (canControl) {
      inputTrajectory = new Vector3(Input.GetAxis("Horizontal"),
                                            0f,
                                            Input.GetAxis("Vertical"));
      Vector3 input = inputTrajectory.normalized;
      inputTrajectory = transform.TransformDirection(inputTrajectory);
      inputTrajectory.Normalize();

      //-------------------Animator Fun Hack-------------------
      //animator.SetFloat("Strafe", input.x);
      //animator.SetFloat("Straight", input.z);
      //animator.SetFloat("Magnitude", input.magnitude);
      anim.direction = input;
      //-------------------------------------------------------

      // Get grounded state and store airborn values if not
      isGrounded = controller.isGrounded;
      if (!isGrounded && wasGrounded) {
        airbornStartTime = Time.time;
        airbornTrajectory = inputTrajectory;
        //Debug.Log("In the wrong part of town");
      }

      // Handled grounded vs not grounded control handling
      if (isGrounded) {
        if (Input.GetAxis("Jump") > 0.1f) {
          isJumping = true;
          airbornTrajectory = inputTrajectory;
          airbornStartTime = Time.time;
        } else {
          isJumping = false;
          inputTrajectory = AdjustGroundVelocityToNormal(inputTrajectory, groundNormal);
        }
      } else {
        // Modify airborn trajectory but keep it within a distance of 1
        // This keeps the player from speeding up while jumping
        airbornTrajectory += inputTrajectory * airbornControlAmount * Time.deltaTime;
        airbornTrajectory = Vector3.ClampMagnitude(airbornTrajectory, 1f);
      }
    }

    //---------------------Movement Handling---------------------
    Vector3 movementDirection = inputTrajectory * speed;

    float airbornTime = Time.time - airbornStartTime;
    if (isJumping) {
      movementDirection = airbornTrajectory * speed;
      movementDirection.y = jumpHeight;
    }

    if (!isGrounded) {
      Vector3 drag = airbornTrajectory * airDrag;
      drag.y = 0f;
      movementDirection -= drag;
      //Debug.Log(drag);
      movementDirection.y -= gravity * airbornTime * airbornTime;
    } else if (isGrounded && !isJumping) {
      movementDirection.y -= gravity;
    }

    //animator.SetBool("IsJumping", isJumping);
    //animator.SetBool("IsFalling", !isGrounded);
    anim.isJumping = isJumping;
    anim.isAirborn = !isGrounded;
    

    movementDirection *= Time.deltaTime;
    controller.Move(movementDirection);
    wasGrounded = isGrounded;
  }

  private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal) {
    Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
    return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
  }

  void OnControllerColliderHit(ControllerColliderHit hit) {
    groundNormal = hit.normal;
  }

  [RPC]
  public void LockPlayer() {
    canControl = false;
  }

  [RPC]
  public void UnlockPlayer() {
    canControl = true;
  }
}