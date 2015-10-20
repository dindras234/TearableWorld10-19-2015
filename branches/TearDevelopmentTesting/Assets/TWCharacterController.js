#pragma strict
// horizontal movement variables.
public var Speed : float = 5;
private var MoveDirection : Vector3 = Vector3.zero;
private var HorizontalDirection : Vector3 = Vector3.zero;

// jumping variable. 
public var jumpSpeed : float = 5.0;
private var VerticalDirection : Vector3 = Vector3.zero;
private var isGrounded : boolean = true;
private var Jumping : boolean = false;
private var inAir : boolean = false;
public var airControl : float = 0.5;

// function to move the character horizontally. 
function Movement(){
	if(Jumping || inAir){
		isGrounded = false;
		Speed = 4;
	}
	else Speed = 5;
	
	if(Input.GetAxis("Horizontal")){
		HorizontalDirection = Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
		HorizontalDirection = transform.TransformDirection(HorizontalDirection) * Speed;
		rigidbody.velocity.x = (HorizontalDirection.x)*Time.timeScale;
	}
	
	if(Input.GetButtonDown("Jump") && isGrounded){
		Jumping = true;
		rigidbody.AddForce((transform.up) * jumpSpeed);
	}
	
}

function Start () {

}

function FixedUpdate () {
	if (Physics.Raycast(transform.position, -transform.up, this.transform.localScale.y/2 + .1)){
		isGrounded = true;
		Jumping = false;
		inAir = false;
	}
	else if(!inAir){
		inAir = true;
	}
	Movement();
}