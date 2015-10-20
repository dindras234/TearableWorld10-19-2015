using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


/// <summary>
/// This is the animation manager for Tearable World.
/// At the moment, there needs to be a standard
/// established for what and how many animations
/// any given object should have in the game.
/// This bottlenecks performance to a point where
/// a fixed number of animations for each character
/// will mean that there are some instances
/// of an object not using certain animations.
/// 
/// Right now, there should be a list of Texture2D's.
/// Each Texture2D is a separate sprite sheet.
/// There will be a method to animate through a sprite sheet
/// until something tells it to switch to another Texture2D.
/// </summary>
public class AnimationManager : MonoBehaviour{
	#region Fields 

	/// <summary>
	/// All possible animations
	/// that the player can perform
	/// </summary>
	public enum AnimationType{
		STAND = 0,
		IDLE = 1,
		WALK = 2,
		RUN = 3,
		JUMP = 4
	}

	/// <summary>
	/// Which direction to have the animation
	/// face. Sorta overkill but helps with readability
	/// </summary>
	public enum AnimationDirection{
		LEFT = 0,
		RIGHT = 1,
		NONE = 2
	}

	AnimationDirection currentDirection;

	/// <summary>
	/// All walk frames, loaded through unity interface
	/// </summary>
	public Texture2D walk1, walk2, walk3, walk4, walk5, walk6, walk7;
	Animation walkAnimation;

	public Texture2D jump1, jump2, jump3, jump4, jump5;
	Animation jumpAnimation;

	/// <summary>
	/// All idle frames, loaded through the unity interface
	/// </summary>
	public Texture2D idle1, idle2, idle3, idle4, idle5, idle6;
	Animation idleAnimation;

	public GameObject playerObject;
	private GameObject player;
	private Material material;

	/// <summary>
	/// All stand frames, loaded through the unity interface
	/// </summary>
	public Texture2D stand;
	Animation standAnimation;

	TWCharacterController characterController;
	GameStateManager gameStateManagerRef;
    InputManager inputManagerRef;

	const int MAX_FPS = 10;
	const int frameWidth = 500;
	const int frameHeight = 800;
	public List<Animation> allAnimations = new List<Animation>();
	public Animation currentAnim;
	public bool stop = false;
	public bool reversed = false;
	public bool loop = false;
	private int FPS = MAX_FPS;
	private int currFrame = 0;
	public int numAnimations = 0;
	private float timeToWait;
	private int index = 0;
	public int currAnimTime = 0;

	//Each animation contains 8 frames. Can tweak for more frames.
	//public int columns = currentAnim.width/frameWidth;
	//public int rows = currentAnim.height/frameHeight;

	#endregion
	// Use this for initialization
	void Start()
    {
		gameStateManagerRef = gameObject.GetComponent<GameStateManager>();
        inputManagerRef = gameObject.GetComponent<InputManager>();

		// by default, player should be facing right
		// since we typically start from the left and head right
		currentDirection = AnimationDirection.RIGHT;

		List<Texture2D> walkList = new List<Texture2D>();
		PopulateWalkAnimations(walkList);
		walkAnimation = new Animation(walkList, Animation.AnimationState.WALK);

		List<Texture2D> idleList = new List<Texture2D>();
		PopulateIdleAnimations(idleList);
		idleAnimation = new Animation(idleList, Animation.AnimationState.IDLE);

		List<Texture2D> standList = new List<Texture2D>();
		PopulateStandAnimations(standList);
		standAnimation = new Animation(standList, Animation.AnimationState.STAND);

		List<Texture2D> jumpList = new List<Texture2D>();
		PopulateJumpAnimations(jumpList);
		jumpAnimation = new Animation(jumpList, Animation.AnimationState.JUMP);

		allAnimations.Add(walkAnimation);
		allAnimations.Add(standAnimation);
		allAnimations.Add(idleAnimation);

		currFrame = 0;
		timeToWait = 1 / FPS;

		currentAnim = standAnimation;
	}

	bool ifInGame = true;

	public void Update()
    {
		if(gameStateManagerRef.inGame)
        {
			if(GameObject.FindGameObjectWithTag("Player"))
				if(characterController == null)
					characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<TWCharacterController>();

			if(ifInGame)
            {
				ifInGame = false;
				chooseAnimation(currentAnim);
			}
		}
	}

	/// <summary>
	/// Populates the input parameter with a list of the walk images 
	/// </summary>
	/// <param name="walkList"></param>
	private void PopulateJumpAnimations(List<Texture2D> jumpList){
		jumpList.Add(jump1);
		jumpList.Add(jump2);
		jumpList.Add(jump3);
		jumpList.Add(jump4);
		jumpList.Add(jump5);
	}

	/// <summary>
	/// Populates the input parameter with a list of the walk images 
	/// </summary>
	/// <param name="walkList"></param>
	private void PopulateWalkAnimations(List<Texture2D> walkList){
		walkList.Add(walk1);
		walkList.Add(walk2);
		walkList.Add(walk3);
		walkList.Add(walk4);
		walkList.Add(walk5);
		walkList.Add(walk6);
		walkList.Add(walk7);
	}

	/// <summary>
	/// Populates the input paramter with a list of idle list
	/// </summary>
	/// <param name="idleList"></param>
	private void PopulateIdleAnimations(List<Texture2D> idleList){
		idleList.Add(idle1);
		idleList.Add(idle2);
		idleList.Add(idle3);
		idleList.Add(idle4);
		idleList.Add(idle5);
		idleList.Add(idle6);
	}

	/// <summary>
	/// Populates the input parameter with a list of standing animations
	/// </summary>
	/// <param name="standList"></param>
	private void PopulateStandAnimations(List<Texture2D> standList){
		standList.Add(stand);
	}

	/// <summary>
	/// Choose which animation you want to use.
	/// This is determined by the string name
	/// of the wanted Texture2D.
	/// </summary>
	void chooseAnimation(Animation currentAnimation){
		 StartCoroutine(Animate());
	}
	
	/// <summary>
	/// Peforms the actual iteration through the sprite sheet
	/// </summary>
	/// <param name="currentAnimation"></param>
	/// <returns></returns>
	private IEnumerator Animate()
    {
		
		//Debug.Log ("Current Animation Frame: " + currAnimTime);
		if(currFrame >= currentAnim.GetLength())
        {
			
			if(!loop)
            {
				//Debug.Log ("!loop");
				stop = true;
			}																
			else if(currentAnim.Equals(jumpAnimation) && characterController.platformCollision){
				// keep curr frame at what it is
				currFrame--;
			}
			else
			{
				//Debug.Log ("currFrame = 0");
				currFrame = 0;
			}	
		}

        if (currentAnimType.Equals(AnimationType.STAND) && (currAnimTime > Random.Range(25, 75)))
        {
            //Debug.Log ("beep");
            TriggerAnimation(AnimationType.IDLE);
        }
		
		
		if(currentAnim.Equals(jumpAnimation)){
			yield return new WaitForSeconds(1f / (float) FPS/2);
		}
		else
        {
			yield return new WaitForSeconds(3f/ (float)FPS);
		}
		
		if(switchOrientation)
        {
			GameObject.FindGameObjectWithTag("Player").transform.Find("Graphics").localScale =
					new Vector3(GameObject.FindGameObjectWithTag("Player").transform.Find("Graphics").localScale.x * -1,
								GameObject.FindGameObjectWithTag("Player").transform.Find("Graphics").localScale.y,
								GameObject.FindGameObjectWithTag("Player").transform.Find("Graphics").localScale.z);
			switchOrientation = false;
		}

		if(GameObject.FindGameObjectWithTag("Player"))
        {
			GameObject.FindGameObjectWithTag("Player").transform.Find("Graphics").renderer.sharedMaterial.mainTexture =
					currentAnim.GetFrames()[currFrame];

            if (inputManagerRef.PlayerSelected())
            {
                GameObject.FindGameObjectWithTag("Player").transform.Find("Graphics").renderer.sharedMaterial.color = Color.blue;
            }

            else
            {
                GameObject.FindGameObjectWithTag("Player").transform.Find("Graphics").renderer.sharedMaterial.color = Color.white;
            }
		}
		currFrame++;
		currAnimTime++;
		if(!stop)
        {
			//Debug.Log (currentAnimType.ToString ());
			StartCoroutine(Animate());
		}
	}
	
	AnimationType currentAnimType;

	/// <summary>
	/// Sets the current animation to be rendered
	/// </summary>
	public void TriggerAnimation(AnimationType type)
    {
        if (inputManagerRef.tearScript)
            if (inputManagerRef.tearScript.GetMovingPiece() || inputManagerRef.tearScript.GetRotatingPiece() || inputManagerRef.tearScript.cuttInProgress)
                return;
        
        currAnimTime = 0;
        currFrame = 0;
        switch (type)
        {
            case AnimationType.IDLE:
                currentAnim = idleAnimation;
                break;

            case AnimationType.WALK:
                currentAnim = walkAnimation;
                break;

            case AnimationType.STAND:
                currentAnim = standAnimation;
                break;

            case AnimationType.JUMP:
                //UnityEngine.Debug.Log("JUMP");
                currentAnim = jumpAnimation;
                break;

            default:
                currentAnim = idleAnimation;
                break;
        }
        currentAnimType = type;
	}

	bool switchOrientation = false;
	public void SetDirection(AnimationDirection dir)
    {
		if(currentDirection != dir)
        {
			switchOrientation = true;
		}
		currentDirection = dir;
	}

	public AnimationType GetCurrentAnimationType()
    {
		return currentAnimType;
	}

	public AnimationDirection GetAnimationDirection()
    {
		return currentDirection;
	}
	
}
