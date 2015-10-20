using UnityEngine;
using System.Collections;

public class CollisionObjects : MonoBehaviour {
	
	// Use this for initialization
	public GameObject thePlayer;
	CollisionHandler.Hitbox top, bottom, Cplayer, platform1;
	
	const float MAX_VEL = 0.05F;
	bool isJumping = false;
	bool canJump = true;
	Texture2D MyTexture;
	
	void Start () {
		MyTexture = Resources.Load("Square") as Texture2D;
		
		isJumping = false;
		canJump = true;
		Bounds pbound = collider.bounds;
		platform1 = new CollisionHandler.Hitbox(CollisionHandler.Shape.Rectangle, new CollisionHandler.Rectangle(5, 5, 0.0001F, 0.000000000001F), Vector2.zero);
		top = new CollisionHandler.Hitbox(CollisionHandler.Shape.Rectangle, new CollisionHandler.Rectangle(0, 20, pbound.size.x/10, pbound.size.y/10), Vector2.zero);
		bottom = new CollisionHandler.Hitbox(CollisionHandler.Shape.Rectangle, new CollisionHandler.Rectangle(0, -6.1F, pbound.size.x/10, pbound.size.y/10), Vector2.zero);
		Cplayer = new CollisionHandler.Hitbox(CollisionHandler.Shape.Rectangle, new CollisionHandler.Rectangle(0, 3, 10, 10), Vector2.zero);
	}
	
	// Update is called once per frame
	void Update () {
		Physics.gravity = new Vector3(1f,-9.81f,0f);
		Debug.Log ("here");
		canJump = false;
		// caps the players falling velocity to the MAX_VEL
		if(Cplayer.vel.y > -MAX_VEL && !isJumping){
			// essentially gravity, is enacted as long as the player is not on the ground
			Cplayer.vel.y -=  0.002F;
		}
		// if the player is traveling at or below the MAX_VEL allow the player to keep accelerating
		if(Mathf.Abs(Cplayer.vel.x) <= MAX_VEL && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))){
			if(Input.GetKey(KeyCode.LeftArrow)){
				Cplayer.vel.x -= 0.002F;
			}
			
			else if(Input.GetKey(KeyCode.RightArrow)){
				Cplayer.vel.x += 0.002F;
			}
		}
		// if the player isnt holding either the left arrow or right arrow stop their x movement
		else {
			Cplayer.vel.x = 0;
		}
		
		// if the player is moving faster than the MAX_VEL then set the player to be moving at the max velocity
		if(Mathf.Abs(Cplayer.vel.x)> MAX_VEL) {
			// check if the vel is positive, if it is then keep them moving in the positive direction at MAX_VEL
			if(Cplayer.vel.x > 0){
				Cplayer.vel.x = MAX_VEL;
			}
			// if the vel is negative keep the player moving in the negative direction at -MAX_VEL
			else {Cplayer.vel.x = -MAX_VEL;}
		}
		
		// if the player hits the bottom platform stop their y movement
		if(CollisionHandler.Hitbox.collisionCheck(Cplayer, bottom)){
			Cplayer.vel.y = 0;
			canJump = true;
			
			Debug.Log("Player hit bottom platform");
		}
		
		// if the player hits the top platform stop their y movement
		if(CollisionHandler.Hitbox.collisionCheck(Cplayer, top) && Cplayer.vel.y > 0){
			Cplayer.vel.y = 0;
			isJumping = false;
			//Debug.Log("Player hit top platform");
		}
		
		if(Input.GetKeyDown(KeyCode.UpArrow) && !isJumping && canJump){
			isJumping = true;
		}
		if(isJumping){ Debug.Log("Jumping");}
		if(isJumping && Cplayer.vel.y < MAX_VEL*1.75F){
			Cplayer.vel.y += 0.02F;
		}
		else if( Cplayer.vel.y >= MAX_VEL*1.75F){isJumping = false;}
		
		//if(CollisionHandler.Hitbox.collisionCheck(Cplayer, platform1)){
		//	Cplayer.vel.y = 0;
		//}
		
		Cplayer.shift(Cplayer.vel);
		thePlayer.transform.position = new Vector3(Cplayer.hitbox.x, Cplayer.hitbox.y, 0);
		
		
		Graphics.DrawTexture(new Rect(Cplayer.hitbox.x, Cplayer.hitbox.y, Cplayer.hitbox.width, Cplayer.hitbox.height), MyTexture);
		Graphics.DrawTexture(new Rect(bottom.hitbox.x, bottom.hitbox.y, bottom.hitbox.width, bottom.hitbox.height), MyTexture);
	}
	

}
