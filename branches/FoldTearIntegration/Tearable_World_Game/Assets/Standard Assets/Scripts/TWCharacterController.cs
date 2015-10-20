using UnityEngine;
using System.Collections;

public class TWCharacterController : MonoBehaviour
{
	
	private int startingTicks = 2;
    // horizontal movement variables.
    private float baseSpeed = 10F, speed, baseMaxSpeed = 4, maxSpeed = 4;
    //private Vector3		horizontalDirection = Vector3.zero;

    // jumping variables. 
    private float jumpSpeed = 175.0F, airControl = 0.5F;
    private Vector3 verticalDirection = Vector3.zero;
    public bool isGrounded = false, rising = false,
                        falling = false,
						fDown = false, gDown = false, tDown = false, 
						vDown = false;

    /// <summary>
    /// Made this one public so we could reference
    /// it in AnimationManager
    /// </summary>
    public bool platformCollision = false;

    // gravity variables
    public float forceOfGravity = 10.0F, fallSpeed = 2.0F;
    private Vector3 gravity = new Vector3(0, 10.0F, 0);
    Vector3 horizontalDirection = new Vector3(1, 1, 1);
	/// <summary>
	/// This variables checks how long character has been rising.
	/// </summary>
	private int beenRising = 0;
    private float debugTest = 0f;
    // storing the script that is attached to the collider on the bottom of the player. 
    PlayerHitBottom playerhitbottom;

    /// <summary>
    /// Input manager reference
    /// </summary>
    InputManager inputManagerRef;

    /// <summary>
    /// Gamestate Manager reference
    /// </summary>
    GameStateManager gameStateManagerRef;
	
	//using these for debugging
	public float yvel = 0;
	public float xvel = 0;
	//public float rotation = 0;

    // function to move the character horizontally. 
    void Movement()
    {
		//checks if rising is true, if it is adds to beenRising
		if(rising)
			beenRising++;
		//if beenrising is equal to 15 then it sets rising to false, and resets beenRising.
		if(beenRising == 15)
		{
			rising = false;
			beenRising = 0;
		}
		//if(platformCollision && rising && !falling) rising = false;
		// used for debugging
		yvel = this.rigidbody.velocity.y;
		xvel = this.rigidbody.velocity.x;
		//rotation = this.rigidbody.rotation.z;
		
		//Debug.Log(this.transform.rotation.z);
		// check if script attached to bottom of player is colliding with a platform
        platformCollision = playerhitbottom.getHitting();
		
		// if on ground i am grounded, not jumping and havent reached jumpClimax.
		if(platformCollision)// && !rising)
		{
			falling = false;
			isGrounded = true;
		}
		else{ isGrounded = false;}
		
		float rotZ = Mathf.Abs(this.transform.rotation.z);
		
		// attempt to fix the first movement glitch where velocity keeps increasing. This did not work!
/*		if(isGrounded && !rising){
			if(rotZ == Quaternion.Euler(0f,0f,0f).z || rotZ == Quaternion.Euler(0f, 0f, 180f).z){
				this.rigidbody.velocity = new Vector3(this.rigidbody.velocity.x, 0, 0);
			}
			else {
				this.rigidbody.velocity = new Vector3(0, this.rigidbody.velocity.y, 0);
			}
		}*/
		
		// this is determining if we are pressing the horizontal in the opposite direction of what we are moving.
		if(rotZ == Quaternion.Euler (0f, 0f, 0f).z){
			if(this.rigidbody.velocity.y <= 0 && !isGrounded && !falling)
			{
				//Debug.Log("from max height check");
				falling = true;
				rising = false;
				beenRising = 0;
				//Debug.Log("Now Falling");
			}
		}
		else if (rotZ == Quaternion.Euler (0f, 0f, 180f).z){
			if(this.rigidbody.velocity.y >= 0 && !isGrounded && !falling)
			{
				//Debug.Log("from max height check");
				falling = true;
				beenRising = 0;
				rising = false;
			}
		}
		// this is determining if we are pressing the horizontal in the opposite direction of what we are moving.
		else if(/*Mathf.Abs((this.rigidbody.rotation * Quaternion.Euler(0f, 0f, 90f)).z)*/rotZ == Quaternion.Euler(0f, 0f, 90f).z){
			if(this.rigidbody.velocity.x >= 0 && !isGrounded && !falling)
			{
				//Debug.Log("from max height check");
				falling = true;
				rising = false;
				beenRising = 0;
			}
		}
		else if(rotZ == Quaternion.Euler(0f, 0f, 270f).z && !falling){ 
			if(this.rigidbody.velocity.x <= 0 && !isGrounded)
			{
				//Debug.Log("from max height check");
				falling = true;
				beenRising = 0;
				rising = false;
			}
		}
		//if (isGrounded && !jumping) jumpClimax = false;
		
		speed = baseSpeed;
		
        // check for horizontal movement.
        horizontalMoveKeyboard();

        // check for jump
        verticalMove();
		
        // apply gravity
        applyGravity(); 

    }

    // handle the vertical jump
    void verticalMove()
    {
		
        //checks to see if the player pressed the jump button and the player is on a platform, ie touching the ground.
        if (Input.GetButton("Jump") && isGrounded && !rising)
        {
			//Debug.Log("rising: " + rising);
            Jump();
        }
    }
	
	int count = 0;
    public void Jump()
    {
		if(isGrounded && !rising)
		{
			Vector3 force = rigidbody.transform.up * jumpSpeed * this.transform.localScale.y;
	        // add the jump force that propells the player upward. 
	        rigidbody.AddForce(force);
			rising = true;
			Debug.Log("now rising");
		}
    }

    void horizontalMoveKeyboard()
    {
        float horizontal = 0;
        if (Application.platform.Equals(RuntimePlatform.Android) || gameStateManagerRef.unityRemote)
        {
            horizontal = inputManagerRef.GetHorizontalTouchMovement();
            //Debug.Log("HORIZ " + horizontal);
        }

        else
        {
            horizontal = Input.GetAxisRaw("Horizontal");
        }

        horizontalMove(horizontal);
        // if the  left | A and right or D keys are being pressed then we move left or right appropriately. 
        /*if(Input.GetAxis("Horizontal") != 0){
            // get the horizontal axis relative to the player.  
            horizontalDirection = this.rigidbody.transform.right;
            // move in the correct direction a certain speed.
            horizontalDirection = horizontalDirection * Input.GetAxisRaw("Horizontal") * speed;
            // if we are rotated handle the movement direction correctly. 
            if(this.transform.rotation.z == 0 || this.transform.rotation.z == 1)
                rigidbody.velocity = new Vector3(horizontalDirection.x, rigidbody.velocity.y, rigidbody.velocity.z);
            else 	
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, horizontalDirection.y, rigidbody.velocity.z);
        }*/
    }

    // Moves the player horizontally
    public void horizontalMove(float horizontal)
    {
		if(this.rigidbody.isKinematic == false){
		    //Debug.Log(this.transform.rotation.z);
			bool positive;
			float rotZ = Mathf.Abs(this.transform.rotation.z);
			// this is determining if we are pressing the horizontal in the opposite direction of what we are moving.
			if(rotZ == Quaternion.Euler (0f, 0f, 0f).z){
				positive =  this.rigidbody.velocity.x*horizontal >= 0;
			}
			else if (rotZ == Quaternion.Euler (0f, 0f, 180f).z){
				positive = this.rigidbody.velocity.x*horizontal*(-1) >= 0;
			}
			// this is determining if we are pressing the horizontal in the opposite direction of what we are moving.
			else {
				//Debug.Log( transform.rotation.z + " i am on the right wall");
				positive = this.rigidbody.velocity.y*horizontal*this.transform.right.y >=0;
				
			}
			
		    if (rotZ == Quaternion.Euler (0f, 0f, 0f).z || rotZ == Quaternion.Euler (0f, 0f, 180f).z)
		    {
				// if we are trying to go in the opposite direction that that which we are moving set the velocity to zero and start moving in the direction we want to go.
				if(!positive){
					this.rigidbody.velocity = new Vector3(0, this.rigidbody.velocity.y, 0);
				}
				
		        if (horizontal == 0)
		        {
		            Vector3 newVec = new Vector3(rigidbody.velocity.x * slowDownRate * transform.localScale.x, rigidbody.velocity.y, 0);
		            rigidbody.velocity = newVec;
		        }
		
		        else
		        {
		            rigidbody.AddForce(this.transform.right.x * horizontal * speed * transform.localScale.x, 0, 0);
					if(rigidbody.velocity.x > maxSpeed) rigidbody.velocity = new Vector3(maxSpeed, rigidbody.velocity.y, 0);
					else if(rigidbody.velocity.x < -maxSpeed) rigidbody.velocity = new Vector3(-maxSpeed, rigidbody.velocity.y, 0);
		        }
		    }
		    else
		    {	
				// if we are trying to go in the opposite direction than that which we are moving set the velocity to zero and start moving in the direction we want to go.
				if(!positive){
					this.rigidbody.velocity = new Vector3(this.rigidbody.velocity.x, 0, 0);
				}
				
		        if (horizontal == 0)
		        {
		            Vector3 newVec = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y * slowDownRate * transform.localScale.x, 0);
		            rigidbody.velocity = newVec;
		        }
		
		        else
		        {
		            rigidbody.AddForce(0, this.transform.right.y * horizontal * speed * transform.localScale.x, 0);
					if(rigidbody.velocity.y > maxSpeed) rigidbody.velocity = new Vector3(rigidbody.velocity.x, maxSpeed, 0);
					else if(rigidbody.velocity.y < -maxSpeed) rigidbody.velocity = new Vector3(rigidbody.velocity.x, -maxSpeed, 0);
		        }
		    }
		}

    }

    float slowDownRate = 0.89f;

    // Apply Gravity 
    void applyGravity()
    {
        // Apply gravity in the down direction relative to the player.
        gravity = rigidbody.transform.up * forceOfGravity;
        // if we are jumping and begin falling change the force of gravity to be stronger so that we fall faster. 
        if (falling)
        {
            gravity += (rigidbody.transform.up * fallSpeed);
        }
        rigidbody.velocity -= gravity * Time.deltaTime * this.transform.localScale.y;
    }

	// Check the screen orientation and rotate the character accordingly
	void checkOrientation()
	{
		if(Input.GetKeyDown(KeyCode.F))
			fDown = true;
		else if(Input.GetKeyDown(KeyCode.G))
			gDown = true;
		else if(Input.GetKeyDown(KeyCode.V))
			vDown = true;
		else if(Input.GetKeyDown(KeyCode.T))
			tDown = true;

		if(Input.GetKeyUp(KeyCode.F) && fDown)
		{
			fDown = false;
			this.transform.rotation = this.transform.rotation * Quaternion.Euler (0f, 0f, 90f);
		}
		else if(Input.GetKeyUp(KeyCode.G) && gDown)
		{
			gDown = false;
			this.transform.rotation = this.transform.rotation * Quaternion.Euler (0f, 0f, -90f);
		}
		else if (Input.deviceOrientation == DeviceOrientation.Portrait
			&& (this.transform.rotation == Quaternion.Euler (0f, 0f, 0f) || 
			this.transform.rotation == Quaternion.Euler (0f, 0f, 180f)))
		{
			this.transform.rotation = Quaternion.Euler (0f, 0f, 90f);
		}
		else if (Input.deviceOrientation == DeviceOrientation.LandscapeRight
			&& (this.transform.rotation == Quaternion.Euler (0f, 0f, 90f) || 
			this.transform.rotation == Quaternion.Euler (0f, 0f, 270f)))
		{
			this.transform.rotation = Quaternion.Euler (0f, 0f, 180f);
		}
		else if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown
			&& (this.transform.rotation == Quaternion.Euler (0f, 0f, 180f) || 
			this.transform.rotation == Quaternion.Euler (0f, 0f, 0f)))
		{
			this.transform.rotation = Quaternion.Euler (0f, 0f, 270f);
		}
		else if(Input.deviceOrientation == DeviceOrientation.LandscapeLeft
			&& (this.transform.rotation == Quaternion.Euler (0f, 0f, 90f) || 
			this.transform.rotation == Quaternion.Euler (0f, 0f, 270f)))
		{
			this.transform.rotation = Quaternion.Euler (0f, 0f, 0f);
		}
	}
	
    // gets the script attached to the bottom of the player. 
    void Start()
    {
		// attempt to fix the slowing jump when next to wall. 
		this.rigidbody.collider.material.dynamicFriction = 0;
        this.rigidbody.collider.material.dynamicFriction2 = 0;
        this.rigidbody.collider.material.staticFriction = 0;
        this.rigidbody.collider.material.staticFriction2 = 0;
		this.collider.material.dynamicFriction = 0;
		this.collider.material.dynamicFriction2 = 0;
		this.collider.material.staticFriction = 0;
		this.collider.material.staticFriction2 = 0;
		
		
        gameStateManagerRef = GameObject.FindGameObjectWithTag("MainObject").GetComponent<GameStateManager>();
        inputManagerRef = GameObject.FindGameObjectWithTag("MainObject").GetComponent<InputManager>();
		maxSpeed = baseMaxSpeed*this.transform.localScale.x;
		playerhitbottom = this.GetComponentInChildren<PlayerHitBottom>();
        //inputManagerRef = GameObject.FindGameObjectWithTag("MainObject").GetComponent("InputManager") as InputManager;
		
		horizontalMove(1);
    }

    void Update()
    {
		if(startingTicks>0){
			horizontalMove(1);
			startingTicks--;
		}
		// attempt to fix the slowing jump when next to wall. 
		this.rigidbody.collider.material.dynamicFriction = 0;
        this.rigidbody.collider.material.dynamicFriction2 = 0;
        this.rigidbody.collider.material.staticFriction = 0;
        this.rigidbody.collider.material.staticFriction2 = 0;
		this.collider.material.dynamicFriction = 0;
		this.collider.material.dynamicFriction2 = 0;
		this.collider.material.staticFriction = 0;
		this.collider.material.staticFriction2 = 0;
		this.collider.material.frictionCombine = PhysicMaterialCombine.Minimum;
		
		// Check orientation
		checkOrientation();

        // call movement functions.
       // if (!inputManagerRef.GetPieceMoving())
        Movement();
        //else
        //    UnityEngine.Debug.Log("MOVING PIECE");
    }
}