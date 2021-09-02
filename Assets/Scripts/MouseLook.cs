using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX;
	public float sensitivityY;

	public float minimumX;
	public float maximumX;

	public float minimumY;
	public float maximumY;

	float rotationY = 0F;

	bool debugLog = false;

	void Start ()
	{
		// Confine the cursor to the screen
		Cursor.lockState = CursorLockMode.Confined;
		// Lock the cursor to the screen
		Cursor.lockState = CursorLockMode.Locked;

		if (sensitivityX <= 0)
        {
			sensitivityX = 1.0f;
			if (debugLog)
				Debug.Log("SensitivityX not set on " + name + " defaulting to " + sensitivityX);
        }

		if (sensitivityY <= 0)
        {
			sensitivityY = 1.0f;
			if (debugLog)
				Debug.Log("SensitivityY not set on " + name + " defaulting to " + sensitivityY);
        }

		if (minimumX <= 0)
        {
			minimumX = -360.0f;
			if (debugLog)
				Debug.Log("MinimumX not set on " + name + " defaulting to " + minimumX);
		}

		if (maximumX <= 0)
        {
			maximumX = 360.0f;
			if (debugLog)
				Debug.Log("MaximumX not set on " + name + " defaulting to " + maximumX);
		}

		if (minimumY <= 0)
        {
			minimumY = -60.0f;
			if (debugLog)
				Debug.Log("MinimumY not set on " + name + " defaulting to " + minimumY);
		}

		if (maximumY <= 0)
        {
			maximumY = 60.0f;
			if (debugLog)
				Debug.Log("MaximumY not set on " + name + " defaulting to " + maximumY);
        }

		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	}

	void Update ()
	{
		if (axes == RotationAxes.MouseXAndY)
		{
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX)
		{
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}
	}	
}