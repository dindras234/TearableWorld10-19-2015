using UnityEngine;
using System.Collections;

public class TWCharacterController : MonoBehaviour{
	// horizontal movement variables.
	public float		speed = 5;
	private Vector3		horizontalDirection = Vector3.zero;
	
	// jumping variable. 
	public float		jumpSpeed = 500.0F, airControl = 0.5F;
	private Vector3		verticalDirection = Vector3.zero;
	private bool		isGrounded = true, jumping = false, inAir = false,
						jumpClimax = false, platformCollision = false;
	
	// gravity variables
	public float		forceOfGravity = 10.0F, fallSpeed = 2.0F;
	private Vector3		gravity = new Vector3(0, 10.0F, 0);
	
	// function to move the character horizontally. 
	void Movement(){
		// if the player is jumping or is falling in the air set is grounded to false and reduce their horizontal speed.
		if(jumping || inAir){
			isGrounded = false;
			speed = 4;
		}
		// if they arent in the air or jumping set speed back to normal.
		else{ speed = 5; }
		
		// check for horizontal movement.
		HorizontalMove();
		
		// check for jump
		VerticalMove();
		
		// apply gravity
		if(!platformCollision){ applyGravity(); }
	}
	
	// handle the vertical jump
	void VerticalMove(){
		// just setting the 
		if(platformCollision && !inAir){
			//rigidbody.velocity.y = 0;
		}
		
		if(Input.GetButtonDown("Jump") && platformCollision){
			rigidbody.position += rigidbody.transform.up;
			jumping = true;
			platformCollision = false;
			rigidbody.AddForce((rigidbody.transform.up) * jumpSpeed);
		}
	}
	
	void HorizontalMove(){
		if(Input.GetAxis("Horizontal") != 0){
			horizontalDirection = this.rigidbody.transform.right;//Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
			horizontalDirection = horizontalDirection * Input.GetAxisRaw("Horizontal") * speed;
			//Debug.Log(this.transform.rotation.z);
			//horizontalDirection = transform.TransformDirection(horizontalDirection) * speed;
			if(this.transform.rotation.z == 0 || this.transform.rotation.z == 1)
				rigidbody.velocity = new Vector3(horizontalDirection.x, rigidbody.velocity.y, rigidbody.velocity.z);
			else 
				rigidbody.velocity = new Vector3(rigidbody.velocity.x, horizontalDirection.y, rigidbody.velocity.z);
		}
	}
	
	void applyGravity (){
		gravity = rigidbody.transform.up * forceOfGravity;
		if(jumpClimax){
			gravity += (rigidbody.transform.up * fallSpeed);
		}
		rigidbody.velocity -= gravity * Time.deltaTime;
	}
	
	void OnTriggerStay(Collider collisionInfo){
		//Debug.Log("colliding");
		if(collisionInfo.gameObject.tag == "Platform"){
			if((rigidbody.position.y - collisionInfo.transform.position.y) > 0){
				rigidbody.position = new Vector3(rigidbody.position.x,
						collisionInfo.transform.position.y + collisionInfo.bounds.size.y/2 + rigidbody.collider.bounds.size.y/2,
						rigidbody.position.z);
				rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
			}
			else if((rigidbody.position.y - collisionInfo.transform.position.y) < 0){
				rigidbody.position = new Vector3(rigidbody.position.x,
						collisionInfo.transform.position.y - collisionInfo.bounds.size.y/2 - rigidbody.collider.bounds.size.y/2,
						rigidbody.position.z);
				rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
			}
			else if((rigidbody.position.x - collisionInfo.transform.position.x) > 0){
				rigidbody.position = new Vector3(
						collisionInfo.transform.position.y + collisionInfo.bounds.size.x/2 + rigidbody.collider.bounds.size.x/2,
						rigidbody.position.y,
						rigidbody.position.z);
				rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, rigidbody.velocity.z);
			}
			else if((rigidbody.position.x - collisionInfo.transform.position.x) < 0){
				rigidbody.position = new Vector3(
						collisionInfo.transform.position.y - collisionInfo.bounds.size.x/2 - rigidbody.collider.bounds.size.x/2,
						rigidbody.position.y,
						rigidbody.position.z);
				rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, rigidbody.velocity.z);
			}
			platformCollision = true;
		}
	}
	
	void OnTriggerExit(Collider collisionInfo){
		if(collisionInfo.gameObject.tag == "Platform"){
			platformCollision = false;
		}
	}
	
	void Start(){
	
	}
	
	void FixedUpdate(){
		if(platformCollision){
			isGrounded = true;
			jumping = false;
			inAir = false;
			jumpClimax = false;
		}
		else if(!inAir){
			inAir = true;
		}
		if (rigidbody.velocity.y <= 0.0 && jumping){
			jumping = false;
			jumpClimax = true;
		}
		
		Movement();
	}
	
}