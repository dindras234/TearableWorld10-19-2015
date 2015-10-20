
using UnityEngine;
using System.Collections;

public class TestController : MonoBehaviour {

public Vector3 Gravity = new Vector3(0f, 21f, 0f);	 //downward force
public int gravityDirection = 0; //0 is regular gravity, 1 is gravity along x, 2 is - gravity, 3 is gravity along -x
public float TerminalVelocity = 20f;	//max downward speed
public float JumpSpeed = 6f;
public float MoveSpeed = 10f;

public Vector3 MoveVector {get; set;}
public float VerticalVelocity {get; set;}

public CharacterController CharacterController;

// Use this for initialization
void Awake () {
	CharacterController = gameObject.GetComponent("CharacterController") as CharacterController;
//characterController = GameObject.GetComponent(“CharacterController”) as CharacterController;
}

// Update is called once per frame
void Update () {
checkGravity();
checkMovement();
HandleActionInput();
processMovement();
}

void checkGravity()
{
	if(Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
	{
		Gravity = new Vector3(0f, 21f, 0f);
		gravityDirection = 0;
		transform.rotation = Quaternion.Euler(0, 0, 0);
	}
	else if(Input.deviceOrientation == DeviceOrientation.Portrait)
	{
		Gravity = new Vector3(-21f, 0f, 0f);
		gravityDirection = 1;
		transform.rotation = Quaternion.Euler(0, 0, 90);
	}
	else if(Input.deviceOrientation == DeviceOrientation.LandscapeRight)
	{
		Gravity = new Vector3(0f, -21f, 0f);
		gravityDirection = 2;
		transform.rotation = Quaternion.Euler(0, 0, 180);
	}
	else if(Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
	{
		Gravity = new Vector3(21f, 0f, 0f);
		gravityDirection = 3;
		transform.rotation = Quaternion.Euler(0, 0, 270);
	}

}
void checkMovement(){
//move l/r
var deadZone = 0.1f;
		if(gravityDirection == 0)
			VerticalVelocity = MoveVector.y;
		else if(gravityDirection == 1)
			VerticalVelocity = MoveVector.x;
		else if(gravityDirection == 2)
			VerticalVelocity = -MoveVector.y;
		else if(gravityDirection == 3)
			VerticalVelocity = -MoveVector.x;
		MoveVector = Vector3.zero;
		if(gravityDirection == 0){
			if(Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < -deadZone){
				MoveVector += new Vector3(Input.GetAxis("Horizontal"),0,0);
			}
		}
		else if(gravityDirection == 1)
			{
			if(Input.GetAxis("Horizontal") > deadZone || Input.GetAxis("Horizontal") < -deadZone){
				MoveVector += new Vector3(0,Input.GetAxis("Horizontal"),0);				
			}
		}
		else if(gravityDirection == 2){
			if(-Input.GetAxis("Horizontal") > deadZone || -Input.GetAxis("Horizontal") < -deadZone){
				MoveVector += new Vector3(Input.GetAxis("Horizontal"),0,0);
			}
		}
		else if(gravityDirection == 3)
			{
			if(-Input.GetAxis("Horizontal") > deadZone || -Input.GetAxis("Horizontal") < -deadZone){
				MoveVector += new Vector3(0,Input.GetAxis("Horizontal"),0);				
			}
		}

//jump

}

void HandleActionInput(){
if(Input.GetButton("Jump")){
jump();
}
}

void processMovement(){
//transform moveVector into world-space relative to character rotation
//MoveVector = transform.TransformDirection(MoveVector);

//normalize moveVector if magnitude > 1
if(MoveVector.magnitude > 1){
MoveVector = Vector3.Normalize(MoveVector);
}

//multiply moveVector by moveSpeed
MoveVector *= MoveSpeed;

//reapply vertical velocity to moveVector.y
		if(gravityDirection == 0)
			MoveVector = new Vector3(MoveVector.x, VerticalVelocity, MoveVector.z);
		else if(gravityDirection == 1)
			MoveVector = new Vector3(VerticalVelocity, MoveVector.y, MoveVector.z);
		else if(gravityDirection == 2)
			MoveVector = new Vector3(MoveVector.x, -VerticalVelocity, MoveVector.z);
		else if(gravityDirection == 3)
			MoveVector = new Vector3(-VerticalVelocity, MoveVector.y, MoveVector.z);



//apply gravity
applyGravity();

//move character in world-space
CharacterController.Move(MoveVector * Time.deltaTime);
}

void applyGravity(){
		float tempX = MoveVector.x;
		float tempY = MoveVector.y;
if(MoveVector.y > -TerminalVelocity)
		{
			MoveVector = new Vector3((MoveVector.x - Gravity.x * Time.deltaTime), (MoveVector.y - Gravity.y * Time.deltaTime), MoveVector.z);
	//MoveVector = new Vector3((MoveVector.x – Gravity.x * Time.deltaTime), (MoveVector.y – Gravity * Time.deltaTime), MoveVector.z);
}
		if(gravityDirection == 0){
			if(CharacterController.isGrounded && tempY < -1){
				MoveVector = new Vector3(tempX, (-1), MoveVector.z);
			}
		}
		else if(gravityDirection == 1){
			if(CharacterController.isGrounded && tempX < -1){
				MoveVector = new Vector3((-1), tempY, MoveVector.z);
			}
		}
		else if(gravityDirection == 2){
			if(CharacterController.isGrounded && -tempY < -1){
				MoveVector = new Vector3(-tempX, (-1), MoveVector.z);
			}
		}
		else if(gravityDirection == 3){
			if(CharacterController.isGrounded && -tempX < -1){
				MoveVector = new Vector3((-1), -tempY, MoveVector.z);
			}
		}

}

public void jump(){
//if(CharacterController.isGrounded){
VerticalVelocity = JumpSpeed;
//}
}
}