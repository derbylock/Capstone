using UnityEngine;
using System.Collections;

public class FollowScript : MonoBehaviour {
	public Transform target;
	public bool rotateTarget = false;
	
	public float distance = 10.0f;
	public Vector3 offset;
	public float clippingBuffer = 0.5f;
	
	// Used to determine whether the player can zoom and to what extents
	public bool useScrollWheel = false;
	public float minDistance = 10.0f;
	public float maxDistance = 20.0f;
	public float scrollSpeed = 4.0f;
	
	// The rates at which the angle can be changed on each axis
	public float xSpeed = 200.0f;
	public float ySpeed = 100.0f;
	
	// Minimum and maximum angles for the y-axis rotation
	public float yMinLimit = -20.0f;
	public float yMaxLimit = 80.0f;
	public bool invertY = false;
	
	private float x = 0.0f;
	private float y = 0.0f;
	private Vector3 position;

	// Use this for initialization
	void Start () {
		x = transform.eulerAngles.x;
		y = transform.eulerAngles.y;
		
		if (useScrollWheel && minDistance > maxDistance) {
			var temp = minDistance;
			minDistance = maxDistance;
			maxDistance = temp;
			
			Debug.LogWarning("minDistance and maxDistance values are backwards.");
		}
	}
	
	void LateUpdate () {
	    if (target) {
			
			// If we are pressing down LMB, get the changes in x and y values from the mouse.
	    	//if (Input.GetMouseButton(1)){
			    x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
				
				// Added support for inverted y-axis controls.
				if(invertY) {
					y += Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
				} else {
			    	y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
				}
				
				// Make sure that y is between the minimum and maximum y values. This prevents
				// the camera from flipping over and underneath the character.
				y = ClampAngle(y, yMinLimit, yMaxLimit);
			//}
			
			// Support for zooming in and out if needed.
			if (useScrollWheel) {
				distance += Input.GetAxis("Mouse ScrollWheel") * -scrollSpeed;
				distance = AdjustDistance (distance, minDistance, maxDistance);
			}
			
			// TODO: FIGURE THIS OUT
			// Set rotation based off mouse changes, using y to rotate around the x-axis and
			// x to spin the camera around the y-axis. Then use the rotation to find the offset
			// from the object we are rotating around.
		    Quaternion rotation = Quaternion.Euler(y, x, 0);
		    position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
			
			// Now we have to find a point that is not clipped by another object.
			// If we did adjust the distance at all, subtract an additional buffer to prevent clipping.
			Vector3 rayDirection = position - target.position;
			RaycastHit[] hits = Physics.RaycastAll (target.position, rayDirection, distance);
			
			float nearestHit = distance;
			foreach(RaycastHit hit in hits) {
				if(!hit.collider.isTrigger && hit.transform.root.tag != "Player") {
					//Debug.Log(hit.collider.gameObject.name);
					if(hit.distance < minDistance) {
						nearestHit = hit.distance;
					}
				}
			}
			
			if(nearestHit < distance) {
				nearestHit -= clippingBuffer;
				position = rotation * new Vector3(0.0f, 0.0f, -nearestHit) + target.position;
			}
			
			
		    
			// Apply transforms and rotations.
		    transform.rotation = rotation;
			
			// We have to apply our rotation first in order to prevent a lagging effect
			position += transform.TransformDirection(offset);
		    transform.position = position;
			
			
			if(rotateTarget) {
				target.rotation = Quaternion.Euler(new Vector3(0.0f, transform.rotation.eulerAngles.y, 0.0f));
			}
	    }
	}
	
	private float ClampAngle (float angle, float min, float max) {
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
	
	private float AdjustDistance (float current, float min, float max) {
		if (current < min)
			return min;
		else if (current > max)
			return max;
		else
			return current;
	}
}