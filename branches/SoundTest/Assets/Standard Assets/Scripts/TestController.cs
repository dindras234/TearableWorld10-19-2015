using UnityEngine;
using System.Collections;
 
[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]
 
public class TestController : MonoBehaviour {
 
	public float speed = 10.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	private bool grounded = false;
	private float gravityDirection = 0; //0 = landscape left, 1 = portrait, 2 = landscape right, 3 = portraitupsidedown

 
 
 
	void Awake () {
	    rigidbody.freezeRotation = true;
	    rigidbody.useGravity = false;
	}
 
	void FixedUpdate () {
	if(Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
	{
		gravityDirection = 0;
		transform.rotation = Quaternion.Euler(0f, 0f, 0f);
	}
	else if(Input.deviceOrientation == DeviceOrientation.Portrait)
	{
		gravityDirection = 1;
		transform.rotation = Quaternion.Euler(0f, 0f, 90f);
	}
	else if(Input.deviceOrientation == DeviceOrientation.LandscapeRight)
	{
		gravityDirection = 2;
		transform.rotation = Quaternion.Euler(0f, 0f, 180f);
	}
	else if(Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
	{
		gravityDirection = 3;
		transform.rotation = Quaternion.Euler(0f, 0f, 270f);
	}
		
	    if (grounded) {
			Vector3 velocity = Vector3.zero;
			Vector3 velocityChange = Vector3.zero;
			Vector3 targetVelocity = Vector3.zero;
	        // Calculate how fast we should be moving
			if(gravityDirection == 0 || gravityDirection == 2){
	        	targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
			
	     	   targetVelocity = transform.TransformDirection(targetVelocity);
	        	targetVelocity *= speed;
 				//Debug.Log ("t1: " + targetVelocity.x + " : " + targetVelocity.y);
	        // Apply a force that attempts to reach our target velocity
	        	velocity = rigidbody.velocity;
	        	velocityChange = (targetVelocity - velocity);
	        	velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
	        	velocityChange.z = 0;
	        	velocityChange.y = 0;
	        	rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
			}
			else if(gravityDirection == 1 || gravityDirection == 3)
			{
	        	targetVelocity = new Vector3(0, Input.GetAxis("Horizontal") , 0);

			
	     	   targetVelocity = transform.TransformDirection(targetVelocity);
	        	//targetVelocity *= speed;
 				//Debug.Log ("t1: " + targetVelocity.x + " : " + targetVelocity.y);
	        // Apply a force that attempts to reach our target velocity
	        	velocity = rigidbody.velocity;
	        	velocityChange = (targetVelocity - velocity);
	        	velocityChange.y = -Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
	        	velocityChange.z = 0;
	        	velocityChange.x = 0;
		        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

			}
 
	        // Jump
	        if (canJump && Input.GetButton("Jump")) {
				if(gravityDirection == 0)
	            	rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), 0);
				else if(gravityDirection == 1)
	            	rigidbody.velocity = new Vector3(-CalculateJumpVerticalSpeed(), velocity.y, 0);
				else if(gravityDirection == 2)
	            	rigidbody.velocity = new Vector3(velocity.x, -CalculateJumpVerticalSpeed(), 0);
				else if(gravityDirection == 3)
	            	rigidbody.velocity = new Vector3(CalculateJumpVerticalSpeed(), velocity.y, 0);

	        }
	    }
		
	    // We apply gravity manually for more tuning control
	    if(gravityDirection == 0)
			rigidbody.AddForce(new Vector3(0f, -gravity * rigidbody.mass, 0f));
	    else if(gravityDirection == 1)
			rigidbody.AddForce(new Vector3(gravity * rigidbody.mass, 0f , 0f));
	    else if(gravityDirection == 2)
			rigidbody.AddForce(new Vector3(0f, gravity * rigidbody.mass, 0f));
	    else if(gravityDirection == 3)
			rigidbody.AddForce(new Vector3(-gravity * rigidbody.mass, 0f, 0f));
 
	    grounded = false;
	}
 
	void OnCollisionStay () {
	    grounded = true;    
	}
 
	float CalculateJumpVerticalSpeed () {
	    // From the jump height and gravity we deduce the upwards speed 
	    // for the character to reach at the apex.
	    return Mathf.Sqrt(2 * jumpHeight * gravity);
	}
}