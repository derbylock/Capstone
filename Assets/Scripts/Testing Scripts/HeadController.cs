using UnityEngine;
using System.Collections;

public class HeadController : MonoBehaviour {
	public string headTransformPath;
	private Transform head;
	
	public bool invertY = false;
	
	public float yMin;
	public float yMax;
	private float yCurrent;
	
	public float xMin;
	public float xMax;
	private float xCurrent;
	
	public float rotationSpeed = 3.0f;
	
	private float x = 0.0f;
	private float y = 0.0f;
	
	// Use this for initialization
	void Start () {
		head = transform.Find(headTransformPath);
		Debug.Log (head == null);
		
		x = head.transform.localEulerAngles.x;
		y = head.transform.localEulerAngles.y;
	}
	
	// Update is called once per frame
	void Update () {
		animation.CrossFade("walk");
		
		// Get any changes in the mouse movement
		if(Input.GetMouseButton(1)) {
			x += Input.GetAxis ("Mouse X") * -rotationSpeed;
			
			// Invertable y-axis
			if(!invertY) {
				y += Input.GetAxis ("Mouse Y") * rotationSpeed;
			} else {
				y -= Input.GetAxis ("Mouse Y") * rotationSpeed;
			}
		}
		
		x = ClampAngle(x, xMin, xMax);
		y = ClampAngle(y, yMin, yMax);
		
		Quaternion rotation = Quaternion.Euler(x, 0.0f, y);
		
		head.localRotation = rotation;
	}
	
	float ClampAngle(float angle, float min, float max) {
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		
		return Mathf.Clamp(angle, min, max);
	}
}
