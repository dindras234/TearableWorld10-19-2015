using UnityEngine;

/// <summary>
/// FILE: PlayerCollisions.cs
/// 
/// DESCRIPTION:
///     This class simply determines when a player has collided with a platform
///     and triggers the WALKINTOWALL animation accordingly. Can be used
///     to extend functionaltiy for AnimationManager.
///     
/// AUTHORS: TearableWorld Team (Crumpled Up Engineers)
///     Justin Telmo, Dominic Arcamone...
///     
/// Last Revision: 5/5/2013
/// </summary>

public class PlayerCollisions : MonoBehaviour
{
    GameStateManager gameStateManagerRef;
	TWCharacterController controllerRef;
	InputManager inputManagerRef;
	AnimationManager animationManagerRef;
    WorldCollision worldCollisionRef;
    GameObject mainObject;
    GameObject paperObject;
	
	public void Start()
	{
        //Grab mainObject prefab to access managers effectively
        mainObject = GameObject.FindGameObjectsWithTag("MainObject")[0];
        if (GameObject.FindGameObjectsWithTag("MainObject").Length > 1)
        {
            GameObject[] mainObjectList = GameObject.FindGameObjectsWithTag("MainObject");
            for (int i = 0; i < mainObjectList.Length; ++i)
            {
                if (mainObjectList[i].GetComponent<GameStateManager>().objectSaved)
                    mainObject = mainObjectList[i];
            }
        }

        // Notice, these are attached to the MainObject
        gameStateManagerRef = mainObject.GetComponent<GameStateManager>();
        animationManagerRef = gameStateManagerRef.GetAnimationManager();
        inputManagerRef = gameStateManagerRef.GetInputManager();
        worldCollisionRef = gameStateManagerRef.GetWorldCollision();

        // This script is attached to the player, so we use 'gameObject' here
        controllerRef = gameObject.GetComponent<TWCharacterController>();

        paperObject = GameObject.FindGameObjectWithTag("background");
	}


  
	
	public void Update()
	{
        if (!paperObject)
        {
            paperObject = GameObject.FindGameObjectWithTag("background");
        }

        if (!inputManagerRef)
        {
            inputManagerRef = gameStateManagerRef.GetInputManager();
        }
	}
	
    /// <summary>
    /// Checks for collision between attached object and other Collider.
    /// For PlayerCollisions.cs context, it checks for collisions with platforms.
    /// </summary>
    /// <param name="other"></param>
	void OnTriggerEnter(Collider other)
    {
		//If you hit the wall while walking
        if (other.tag.Equals("Platform") && controllerRef.getGrounded())
        {
			//douglas -commented out because otherwise it doesn't let game run
            //gameObject.transform.localRotation;
            //Set InputManager's hasHorizontalCollision bool to true, trigger animation
            //UnityEngine.Debug.Log("I'm colliding horizontally!");
        }

        //UnityEngine.Debug.Log("COLLISION");
        if (inputManagerRef)
        {
            inputManagerRef.hasHorizontalCollision = true;
            //inputManagerRef.initPlayerNonGroundedZRot = gameObject.transform.rotation.z;
            inputManagerRef.initPlayerNonGroundedPos = gameObject.transform.position;
        }


		//Checks for which direction the player is sliding in.
		//Should never have both isSlidingUp and isSlidingDown 
		//both equal to true. - J.T.
        //Debug.Log(other.transform.up.ToString());


        //if(other.transform.up.z > )
        //{
        //    inputManagerRef.isSlidingUp = true;
        //}
        //else if (other.transform.up.y >= -0.2 && other.transform.up.y <= 0.2)
        //{
        //    inputManagerRef.isSlidingUp = false;
        //    inputManagerRef.isSlidingDown = false;
        //}
        //else inputManagerRef.isSlidingDown = true;
    }
}
