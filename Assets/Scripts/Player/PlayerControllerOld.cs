using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerOld
  : MonoBehaviour {
	public float speed;
	public KeyCode dashKey;
	public float dashSpeed;
	public float jumpHeight = 8.0f;
	public float dashMaxDuration;
	private float currentDashTime;
	
	public float gravity = 9.81f;
	public bool canControl = true;
	private bool isJumping = false;
	private bool isGrounded = false;
	public float maxIncline = 0.5f;
	private float airbornTime = 0.0f;

  private Vector3 jumpTrajectory;
  public float jumpControl;
	
	private string lastAnimation;
	
	CharacterController controller;
	Vector3 groundNormal;
	
	// Use this for initialization
	void Start () {
		currentDashTime = 0;
		controller = gameObject.GetComponent<CharacterController>();
		groundNormal = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 moveDirection = Vector3.zero;
		isGrounded = IsGroundedTest();
		
		if(isGrounded) {
			airbornTime = 0.0f;
			isJumping = false;
		} else {
			airbornTime += Time.deltaTime;
		}
		
		if(canControl) {
			moveDirection = new Vector3(Input.GetAxis ("Horizontal"), 0.0f, Input.GetAxis ("Vertical"));
			//Debug.Log ("MoveDirection Raw: " + moveDirection);
			moveDirection = transform.TransformDirection(moveDirection);
			//Debug.Log ("MoveDirection Worldspace: " + moveDirection);
			moveDirection = Vector3.Normalize(moveDirection);
			//Debug.Log ("MoveDirection Normalized: " + moveDirection);
			
			// If the player is grounded, we want them to move along the floor rather than straight
			// and being pushed up or walk into the air and have to be pushed down by gravity.
			if(isGrounded) {
				moveDirection = AdjustGroundVelocityToNormal(moveDirection, groundNormal);
			}
			//Debug.Log ("MoveDirection Adjusted: " + moveDirection);
			
			moveDirection = moveDirection * speed * Time.deltaTime;
			
			// Find out if the jump key has been pressed and if they are grounded allow them to jump
			if(isGrounded && Input.GetAxis("Jump") > 0.1f) {
				isJumping = true;

        // Store trajectory so that we can have persistent movement while jumping
        jumpTrajectory = moveDirection.normalized;
			}
		}
		
		// Handle how the player jumps
		if (isJumping) {
			moveDirection.y = jumpHeight * Time.deltaTime;
		}
		
		// Apply gravity
		moveDirection.y -= (gravity*airbornTime*airbornTime) * Time.deltaTime;
		if(!isJumping) {
			// Force the player down a bit faster so they can't pop back up onto a surface
			moveDirection.y -= gravity*Time.deltaTime;
		}
		
		// Play the animation over the network
		if(networkView.isMine) {
			Vector2 horizontalMovement = new Vector2(moveDirection.x, moveDirection.z);
			if(Vector2.SqrMagnitude(horizontalMovement) >= 0.00001f) {
				networkView.RPC ("StartNetworkAnimation", RPCMode.All, "run", (float)Network.time);
			} else {
				networkView.RPC ("StartNetworkAnimation", RPCMode.All, "idle", (float)Network.time);
			}
		}
		
		//Debug.Log ("MoveDirection Complete: " + moveDirection);
		
    controller.Move (moveDirection);
	}
	
	private Vector3 AdjustGroundVelocityToNormal (Vector3 hVelocity, Vector3 groundNormal) {
		Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
		return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit) {
		groundNormal = hit.normal;
	}
	
	bool IsGroundedTest() {
		if(!controller.isGrounded) {
			return false;
		}
		
		RaycastHit hit;
		if(Physics.Raycast(transform.position, Vector3.down, out hit)) { //, transform.lossyScale.y/2 + 0.2f)) {
			return (1 - Vector3.Dot(Vector3.up, hit.normal) <= maxIncline ? true : false);
		}
		
		return false;
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