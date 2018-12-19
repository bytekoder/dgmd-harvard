using UnityEngine;

public class CameraFollower : MonoBehaviour {

	public Transform target;
	public float smoothTime = 0.3F;
	private Vector3 velocity = Vector3.zero;
	public float camHeight = 0;
	public float minX = 7;
	public float maxX = 93;
	public float minY = 3;
	public float maxY = 97;
	
	void Update()
	{
		// Define a target position above and behind the target transform
		Vector3 targetPosition = target.TransformPoint(new Vector3(0, camHeight, -10));
		
		// Variable to store where the camera will be smoothly moving towards.
		Vector3 camPos = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

		transform.position = new Vector3 (Mathf.Clamp(camPos.x, minX, maxX), Mathf.Clamp(camPos.y, minY, maxY), camPos.z);
		//Transform the position using clamps so that the camera doesn't go off the map, and stops at certain points.
	}
}
