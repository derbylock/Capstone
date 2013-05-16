using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class StalkerController : MonoBehaviour {
	private const float MAX_TRANSITION_DISTANCE = 20f;

	public enum StalkerState {
		Wisp,
		WallCrawler
	};

	public class WispBindings {
		public KeyCode Forward = KeyCode.W;
		public KeyCode Back = KeyCode.S;
		public KeyCode StrafeLeft = KeyCode.A;
		public KeyCode StrafeRight = KeyCode.D;
		public KeyCode Ascend = KeyCode.E;
		public KeyCode Descend = KeyCode.Q;
	}
	public GameObject crawler;

	WispBindings wispBindings = new WispBindings();
	private StalkerState stalkerState;
	private Transform myCamera;
	public float wispSpeed;
	public float crawlerSpeed;
	public float boostMultiplier;
	public KeyCode boostKey = KeyCode.LeftShift;

	public KeyCode swapModeKey = KeyCode.Mouse1;
	private bool isTransitioning;
	public float modeSwapCooldown;
	private float lastModeSwapTime;
	public float swapTransitionSpeed;

	public float lungeSpeed;
	public float lungeDistance;
	private float lungeTime;
	private float currentLungeTime;
	public float lungeCooldown;
	private float lastLungeTime;

	private bool canControl;
	

	// Use this for initialization
	void Start() {
		myCamera = GameObject.FindGameObjectWithTag("Follow Camera").transform;
		stalkerState = StalkerState.Wisp;
	}

	// Update is called once per frame
	void Update() {
		if (!isTransitioning) {
			switch (stalkerState) {
				case StalkerState.Wisp:
					WispState();
					break;
				case StalkerState.WallCrawler:
					CrawlerState();
					break;
				default:
					break;
			};

			if (Time.time > lastModeSwapTime + modeSwapCooldown && Input.GetKeyDown(swapModeKey)) {
				isTransitioning = true;
				StartCoroutine(SwapModes());
			}
		}
	}

	private void WispState() {
		Vector3 moveDirection = new Vector3();
		moveDirection.x = (Input.GetKey(wispBindings.StrafeLeft) ? -1 : 0) + (Input.GetKey(wispBindings.StrafeRight) ? 1 : 0);
		moveDirection.y = (Input.GetKey(wispBindings.Descend) ? -1 : 0) + (Input.GetKey(wispBindings.Ascend) ? 1 : 0);
		moveDirection.z = (Input.GetKey(wispBindings.Back) ? -1 : 0) + (Input.GetKey(wispBindings.Forward) ? 1 : 0);
		moveDirection = myCamera.TransformDirection(moveDirection).normalized;
		transform.position += moveDirection * wispSpeed * (Input.GetKey(boostKey) ? boostMultiplier : 1) * Time.deltaTime;
	}

	private void CrawlerState() {
		throw new System.NotImplementedException();
	}

	IEnumerator SwapModes() {
		Debug.Log("Swapping");
		//Raycast
		RaycastHit hit;
		if (Physics.Raycast(transform.position, myCamera.forward, out hit, MAX_TRANSITION_DISTANCE, 1<<LayerMask.NameToLayer("Terrain"))) {
			// Instantiate just for testing
			Instantiate(crawler, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
		}
		//Lerp to position
		isTransitioning = false;
		yield return null;
	}

	private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal) {
		Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
		return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
	}

	/*bool IsGroundedTest() {
		if (!controller.isGrounded) {
			return false;
		}

		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit)) {
			return (1 - Vector3.Dot(Vector3.up, hit.normal) <= maxIncline ? true : false);
		}

		return false;
	}*/

	/*void LateUpdate() {
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1];
		int particleCount = particleSystem.GetParticles(particles);
		Debug.Log(particles.Length);
	}*/

	public void LockPlayer() {
		canControl = false;
	}

	public void UnlockPlayer() {
		canControl = true;
	}
}