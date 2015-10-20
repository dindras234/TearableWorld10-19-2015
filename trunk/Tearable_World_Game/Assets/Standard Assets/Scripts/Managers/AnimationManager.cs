using UnityEngine;
using System;
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
public class AnimationManager : MonoBehaviour
{
    #region Fields

	 
    /*
	 * 
	 * ANIMATIONS NEEDED:
	 * 
	 * 3 different idle animation
	 * walk
	 * jump
	 * fall
	 * landing
	 * walking against wall
	 * sliding
	 * walking up incline
	 * walking down an incline
	 * opening door
	 * death animation
	 * 	disintegration
	 * 
	*/

    /// <summary>
    /// All possible animations
    /// that the player can perform
    /// </summary>
    public enum AnimationType
    {
        STAND = 0,
        IDLE = 1,
        IDLEA = 2,
        IDLEB = 3,
        WALK = 4,
        WALKINTOWALL = 5,
        WALKUP = 6,
        WALKDOWN = 7,
        RUN = 8,
        JUMP = 9,
        FALL = 10,
        OPENDOOR = 11,
        DEATH = 12
    }

    /// <summary>
    /// Which direction to have the animation
    /// face. Sorta overkill but helps with readability
    /// </summary>
    public enum AnimationDirection
    {
        LEFT = 0,
        RIGHT = 1,
		NONE = 2
    }

    AnimationDirection currentDirection;


    public string level1Name, level2Name, level3Name, level4Name,  level5Name,  level6Name,  level7Name,  
					level8Name,  level9Name,  level10Name,  level11Name,  level12Name,  level13Name,  level14Name,  level15Name, 
					level16Name,  level17Name,  level18Name,  level19Name,  level20Name,  level21Name,  level22Name,  level23Name, 
					level24Name, level25Name,
					finalLevel1Name, finalLevel6Name, finalLevel7Name, finalLevel8Name, finalLevel9Name, loadingName;

    public string[] levelNames = new string[Enum.GetNames(typeof(GameStateManager.LevelScenes)).Length + 1];

    /// <summary>
    /// Individual frames for level 1 (tree blowing in wind)
    /// </summary>
    public Texture2D level1Frame1, level1Frame2, level1Frame3;
    private List<Texture2D> level1FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level BACKSIDE 1 (tree blowing in wind) -> J.C.
    /// </summary>
    public Texture2D BacksideLevel1Frame1, BacksideLevel1Frame2, BacksideLevel1Frame3;
    private List<Texture2D> BacksideLevel1FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 2 (added by J.C.)
    /// </summary>
    public Texture2D level2Frame1, level2Frame2, level2Frame3;
    private List<Texture2D> level2FrameList = new List<Texture2D>();

    /// <summary>
    /// Individual frames for level 3 (rain)
    /// </summary>
    public Texture2D level3Frame1, level3Frame2, level3Frame3;
    private List<Texture2D> level3FrameList = new List<Texture2D>();
	
	
    public Texture2D Backsidelevel3Frame1, Backsidelevel3Frame2, Backsidelevel3Frame3;
    private List<Texture2D> Backsidelevel3FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 4 (added by J.C.)
    /// </summary>
    public Texture2D level4Frame1, level4Frame2, level4Frame3;
    private List<Texture2D> level4FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 5 (added by J.C.)
    /// </summary>
    public Texture2D level5Frame1, level5Frame2, level5Frame3;
    private List<Texture2D> level5FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 5 (added by J.C.)
    /// </summary>
    public Texture2D Backsidelevel5Frame1, Backsidelevel5Frame2, Backsidelevel5Frame3;
    private List<Texture2D> Backsidelevel5FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 6 (added by J.C.)
    /// </summary>
    public Texture2D level6Frame1, level6Frame2, level6Frame3;
    private List<Texture2D> level6FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level BACKSIDE 6 -> J.C.
    /// </summary>
    public Texture2D BacksideLevel6Frame1, BacksideLevel6Frame2, BacksideLevel6Frame3;
    private List<Texture2D> BacksideLevel6FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 7 (added by J.C.)
    /// </summary>
    public Texture2D level7Frame1, level7Frame2, level7Frame3;
    private List<Texture2D> level7FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level BACKSIDE 7 -> J.C.
    /// </summary>
    public Texture2D BacksideLevel7Frame1, BacksideLevel7Frame2, BacksideLevel7Frame3;
    private List<Texture2D> BacksideLevel7FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 8 (added by J.C.)
    /// </summary>
    public Texture2D level8Frame1, level8Frame2, level8Frame3;
    private List<Texture2D> level8FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level BACKSIDE 7 -> J.C.
    /// </summary>
    public Texture2D BacksideLevel8Frame1, BacksideLevel8Frame2, BacksideLevel8Frame3;
    private List<Texture2D> BacksideLevel8FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 9 (added by J.C.)
    /// </summary>
    public Texture2D level9Frame1, level9Frame2, level9Frame3;
    private List<Texture2D> level9FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level BACKSIDE 7 -> J.C.
    /// </summary>
    public Texture2D BacksideLevel9Frame1, BacksideLevel9Frame2, BacksideLevel9Frame3;
    private List<Texture2D> BacksideLevel9FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 10 (added by J.C.)
    /// </summary>
    public Texture2D level10Frame1, level10Frame2, level10Frame3;
    private List<Texture2D> level10FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level BACKSIDE 10 -> J.C.
    /// </summary>
    public Texture2D BacksideLevel10Frame1, BacksideLevel10Frame2, BacksideLevel10Frame3;
    private List<Texture2D> BacksideLevel10FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 11 (added by J.C.)
    /// </summary>
    public Texture2D level11Frame1, level11Frame2, level11Frame3;
    private List<Texture2D> level11FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level BACKSIDE 7 -> J.C.
    /// </summary>
    public Texture2D BacksideLevel11Frame1, BacksideLevel11Frame2, BacksideLevel11Frame3;
    private List<Texture2D> BacksideLevel11FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 12 (added by J.C.)
    /// </summary>
    public Texture2D level12Frame1, level12Frame2, level12Frame3;
    private List<Texture2D> level12FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level BACKSIDE 12 -> T.R.
    /// </summary
    public Texture2D BacksideLevel12Frame1, BacksideLevel12Frame2, BacksideLevel12Frame3;
    private List<Texture2D> BacksideLevel12FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 13 (added by J.C.)
    /// </summary>
    public Texture2D level13Frame1, level13Frame2, level13Frame3;
    private List<Texture2D> level13FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level BACKSIDE 12 -> T.R.
    /// </summary
    public Texture2D BacksideLevel13Frame1, BacksideLevel13Frame2, BacksideLevel13Frame3;
    private List<Texture2D> BacksideLevel13FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 14 (added by J.C.)
    /// </summary>
    public Texture2D level14Frame1, level14Frame2, level14Frame3;
    private List<Texture2D> level14FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level BACKSIDE 7 -> J.C.
    /// </summary>
    public Texture2D BacksideLevel14Frame1, BacksideLevel14Frame2, BacksideLevel14Frame3;
    private List<Texture2D> BacksideLevel14FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 15 (added by J.C.)
    /// </summary>
    public Texture2D level15Frame1, level15Frame2, level15Frame3;
    private List<Texture2D> level15FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level BACKSIDE 15 -> J.C.
    /// </summary>
    public Texture2D BacksideLevel15Frame1, BacksideLevel15Frame2, BacksideLevel15Frame3;
    private List<Texture2D> BacksideLevel15FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 16 (added by J.C.)
    /// </summary>
    public Texture2D level16Frame1, level16Frame2, level16Frame3;
    private List<Texture2D> level16FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 16 (added by T.R.)
    /// </summary>
    public Texture2D BacksideLevel16Frame1, BacksideLevel16Frame2, BacksideLevel16Frame3;
    private List<Texture2D> BacksideLevel16FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 17 (added by J.C.)
    /// </summary>
    public Texture2D level17Frame1, level17Frame2, level17Frame3;
    private List<Texture2D> level17FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 17 (added by T.R.)
    /// </summary>
    public Texture2D BacksideLevel17Frame1, BacksideLevel17Frame2, BacksideLevel17Frame3;
    private List<Texture2D> BacksideLevel17FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 18 (added by J.C.)
    /// </summary>
    public Texture2D level18Frame1, level18Frame2, level18Frame3;
    private List<Texture2D> level18FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 18 (added by T.R.)
    /// </summary>
    public Texture2D BacksideLevel18Frame1, BacksideLevel18Frame2, BacksideLevel18Frame3;
    private List<Texture2D> BacksideLevel18FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 19 (added by J.C.)
    /// </summary>
    public Texture2D level19Frame1, level19Frame2, level19Frame3;
    private List<Texture2D> level19FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 20 (added by J.C.)
    /// </summary>
    public Texture2D level20Frame1, level20Frame2, level20Frame3;
    private List<Texture2D> level20FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 21 (added by J.C.)
    /// </summary>
    public Texture2D level21Frame1, level21Frame2, level21Frame3;
    private List<Texture2D> level21FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 22 (added by J.C.)
    /// </summary>
    public Texture2D level22Frame1, level22Frame2, level22Frame3;
    private List<Texture2D> level22FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 23 (added by J.C.)
    /// </summary>
    public Texture2D level23Frame1, level23Frame2, level23Frame3;
    private List<Texture2D> level23FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 24 (added by J.C.)
    /// </summary>
    public Texture2D level24Frame1, level24Frame2, level24Frame3;
    private List<Texture2D> level24FrameList = new List<Texture2D>();
	
	/// <summary>
    /// Individual frames for level 25 (added by J.C.)
    /// </summary>
    public Texture2D level25Frame1, level25Frame2, level25Frame3;
    private List<Texture2D> level25FrameList = new List<Texture2D>();
	
	public Texture2D loadingFrame1, loadingFrame2, loadingFrame3;
	private List<Texture2D> loadingFrameList = new List<Texture2D>();

    public Texture2D finalLevel1Frame1, finalLevel6Frame1, finalLevel7Frame1, finalLevel8Frame1, finalLevel9Frame1;
    private List<Texture2D> finalLevel1FrameList = new List<Texture2D>();
    private List<Texture2D> finalLevel6FrameList = new List<Texture2D>();
    private List<Texture2D> finalLevel7FrameList = new List<Texture2D>();
    private List<Texture2D> finalLevel8FrameList = new List<Texture2D>();
    private List<Texture2D> finalLevel9FrameList = new List<Texture2D>();

    /// <summary>
    /// The second idle animation.
    /// </summary>
    Texture2D idleA1, idleA2, idleA3;
    Animation idleAnimationA;


    /// <summary>
    /// The third idle animation.
    /// </summary>
    Texture2D idleB1, idleB2, idleB3;
    Animation idleAnimationB;

    /// <summary>
    /// The land animation.
    /// </summary>
    Texture2D land1, land2, land3, land4, land5, land6, land7, land8;
    Animation landAnimation;

    /// <summary>
    /// The land animation for landing against a wall.
    /// </summary>
    Texture2D landWall1, landWall2, landWall3, landWall4, landWall5, landWall6, landWall7, landWall8,
        landWall9, landWall10, landWall11, landWall12;
    Animation landWallAnimation;

    /// <summary>
    /// The open door animation. Triggered when the player beats the level.
    /// </summary>
    Texture2D open1, open2, open3, open4, open5, open6, open7, open8;
    Animation openAnimation;


    /// <summary>
    /// The animation for sliding up a wall..
    /// </summary>
    Texture2D slideUp1, slideUp2, slideUp3, slideUp4, slideUp5, slideUp6, slideUp7, slideUp8, slideUp9, slideUp10, slideUp11, slideUp12;
    Animation slideUpAnimation;

    /// <summary>
    /// The animation for sliding down a wall.
    /// </summary>
    Texture2D slideDown1, slideDown2, slideDown3, slideDown4, slideDown5, slideDown6, slideDown7, slideDown8, slideDown9, slideDown10, slideDown11, slideDown12;
    Animation slideDownAnimation;

    /// <summary>
    /// The animation for death.
    /// </summary>

    public Texture2D death1, death2, death3, death4, death5, death6, death7, death8, death9, death10, death11, death12,
              death13, death14, death15, death16, death17, death18, death19, death20, death21, death22, death23;

    Animation deathAnimation;



    /// <summary>
    /// All walk frames, loaded through unity interface
    /// </summary>
    public Texture2D walk1, walk2, walk3, walk4, walk5, walk6, walk7, walk8;
    Animation walkAnimation;

    /// <summary>
    /// All of the jump frames loaded into the jumpAnimation.
    /// </summary>
    public Texture2D jump1, jump2, jump3, jump4, jump5,
                     jump6, jump7, jump8, jump9, jump10,
                     jump11, jump12, jump13, jump14, jump15,
                     jump16, jump17;

    Animation jumpAnimation;


    /// <summary>
    /// All idle frames, loaded through the unity interface
    /// </summary>
    public Texture2D idle1, idle2, idle3, idle4, idle5, idle6,
                     idle7, idle8, idle9, idle10, idle11, idle12,
                     idle13, idle14, idle15, idle16, idle17, idle18,
                     idle19, idle20, idle21;

    Animation idleAnimation;


    public Texture2D fall1, fall2, fall3, fall4, fall5,
                     fall6, fall7, fall8, fall9, fall10;

    Animation fallAnimation;


    /// <summary>
    /// All stand frames, loaded through the unity interface
    /// </summary>
    public Texture2D stand1, stand2, stand3, stand4, stand5,
                     stand6, stand7, stand8, stand9, stand10,
                     stand11, stand12, stand13, stand14, stand15, stand16;

    Animation standAnimation;

    /// <summary>
    /// The character controller.
    /// </summary>
    TWCharacterController characterController; //douglas - made public so it can be called from inputmangaer

    /// <summary>
    /// The game state manager reference.
    /// </summary>
    GameStateManager gameStateManagerRef;

    /// <summary>
    /// The input manager reference.
    /// </summary>
    InputManager inputManagerRef;

    private bool deathAnimComplete = false;


    public float JUMP_FPS = 0.06f;
    public float TRANSITION_FPS = (1f / 100f);
    public float WALK_FPS = (1f / 10f);
    public float IDLE_FPS = 0.01f;
    public float LEVEL_1_FPS = 0.4f;
	
    public float LEVEL_2_FPS = 0.4f;
    public float LEVEL_3_FPS = 0.4f;

    /// <summary>
    /// Controls game width and height. Helpful because we are
    /// using tablet devices only.
    /// </summary>
    const int frameWidth = 500;
    const int frameHeight = 800;

    /// <summary>
    /// A list containing all of our animations.
    /// </summary>
    public List<Animation> allAnimations = new List<Animation>();

    /// <summary>
    /// The current animation.
    /// </summary>
    public Animation currentAnim;

    /// <summary>
    /// A reference to the current level's list of textures to be animated
    /// </summary>
    private List<Texture2D> currentLevelTextures = new List<Texture2D>();
	
	/// <summary>
    /// A reference to the current level's list of BACKSIDE textures to be animated -> J.C.
    /// </summary>
    private List<Texture2D> currentLevelBacksideTextures = new List<Texture2D>();


    private bool levelChange = false;

    /// <summary>
    /// Determines whether or not an animation should stop.
    /// </summary>
    bool stop = false;

    /// <summary>
    /// Controls the direction of an animation.
    /// Default direction of an animation is to the RIGHT.
    /// </summary>
    bool reversed = false;


    /// <summary>
    /// Controls which frame in an animation is being read.
    /// </summary>
    int currFrame = 0;

    /// <summary>
    /// The number of animations.
    /// </summary>
    int numAnimations = 0;

    /// <summary>
    /// Used in Animate() to give a new yield.
    /// </summary>
    private float timeToWait;

    private int index = 0;

    /// <summary>
    /// Determines whether or not an animation should be restarted.
    /// </summary>
    bool restartAnim = false;

    /// <summary>
    /// The type of the current animation.
    /// </summary>
    AnimationType currentAnimType;

    /// <summary>
    /// Used to switch orientation.
    /// </summary>
    bool switchOrientation = false;

    /// <summary>
    /// used to determine if level has
    /// sprites available for animations
    /// </summary>
    bool levelHasAnims = false;

    /// <summary>
    /// boolean that assists
    /// the intial start of animate
    /// co routine call back function
    /// </summary>
    bool firstInGameLoop = true;

    GameObject playerObject, paperObject, backsidePaperObject;
	
	/// <summary>
	/// The cut piece paper -> for updating animated backgrounds correctly
	/// </summary>
	private GameObject CutPiecePaper;
	
	/// <summary>
	/// The world piece paper -> for updating animated backgrounds correctly
	/// </summary>
	private GameObject WorldPiecePaper;
	
	/// <summary>
	/// The tear manager reference.
	/// </summary>
	private TearManager TearManagerRef;


    #endregion


    public bool DeathComplete()
    {
        return deathAnimComplete;
    }


    // Use this for initialization
    void Start()
    {
        // Ensures all necessary scripts are added for the MainObject
        gameStateManagerRef = gameObject.GetComponent<GameStateManager>();
        gameStateManagerRef.EnsureCoreScriptsAdded();
        inputManagerRef = gameObject.GetComponent<InputManager>();

        playerObject = GameObject.FindGameObjectWithTag("Player");
        paperObject = GameObject.FindGameObjectWithTag("background");
		backsidePaperObject = GameObject.FindGameObjectWithTag("Fold");

        // by default, player should be facing right
        // since we typically start from the left and head right
        currentDirection = AnimationDirection.RIGHT;

        /*  FILLING IN ALL OF THE ANIMATIONS WITH TEXTURE2D FRAMES */
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

        List<Texture2D> idleAnimAList = new List<Texture2D>();
        PopulateIdleAnimationA(idleAnimAList);
        idleAnimationA = new Animation(idleAnimAList, Animation.AnimationState.IDLEA);


        List<Texture2D> idleAnimBList = new List<Texture2D>();
        PopulateIdleAnimationB(idleAnimBList);
        idleAnimationB = new Animation(idleAnimBList, Animation.AnimationState.IDLEB);


        List<Texture2D> landWallList = new List<Texture2D>();
        PopulateLandWallAnimation(landWallList);
        landWallAnimation = new Animation(landWallList, Animation.AnimationState.WALKINTOWALL);

        List<Texture2D> slideUpList = new List<Texture2D>();
        PopulateSlideUpAnimation(slideUpList);
        slideUpAnimation = new Animation(slideUpList, Animation.AnimationState.WALKUP);

        List<Texture2D> slideDownList = new List<Texture2D>();
        PopulateSlideDownAnimation(slideDownList);
        slideDownAnimation = new Animation(slideDownList, Animation.AnimationState.WALKDOWN);

        List<Texture2D> deathList = new List<Texture2D>();
        PopulateDeathAnimation(deathList);
        deathAnimation = new Animation(deathList, Animation.AnimationState.DEATH);

        List<Texture2D> openList = new List<Texture2D>();
        PopulateOpenAnimation(openList);
        openAnimation = new Animation(openList, Animation.AnimationState.OPENDOOR);

        List<Texture2D> fallList = new List<Texture2D>();
        PopulateFallAnimation(fallList);
        fallAnimation = new Animation(fallList, Animation.AnimationState.FALL);

        level1FrameList.Add(level1Frame1);
        level1FrameList.Add(level1Frame2);
        level1FrameList.Add(level1Frame3);
		
		//Load backside textures -> J.C.
		BacksideLevel1FrameList.Add(BacksideLevel1Frame1);
		BacksideLevel1FrameList.Add(BacksideLevel1Frame2);
		BacksideLevel1FrameList.Add(BacksideLevel1Frame3);
		
		//Add to list storing level background animation -> J.C.
		level2FrameList.Add(level2Frame1);
        level2FrameList.Add(level2Frame2);
        level2FrameList.Add(level2Frame3);

        level3FrameList.Add(level3Frame1);
        level3FrameList.Add(level3Frame2);
        level3FrameList.Add(level3Frame3);
		
		Backsidelevel3FrameList.Add(Backsidelevel3Frame1);
		Backsidelevel3FrameList.Add(Backsidelevel3Frame2);
		Backsidelevel3FrameList.Add(Backsidelevel3Frame3);
		
		//Add to list storing level background animation -> J.C.
		level4FrameList.Add(level4Frame1);
        level4FrameList.Add(level4Frame2);
        level4FrameList.Add(level4Frame3);
		
		//Add to list storing level background animation -> J.C.
		level5FrameList.Add(level5Frame1);
        level5FrameList.Add(level5Frame2);
        level5FrameList.Add(level5Frame3);
		
		//Add to list storing level background animation -> J.C.
		Backsidelevel5FrameList.Add(Backsidelevel5Frame1);
        Backsidelevel5FrameList.Add(Backsidelevel5Frame2);
        Backsidelevel5FrameList.Add(Backsidelevel5Frame3);
		
		//Add to list storing level background animation -> J.C.
		level6FrameList.Add(level6Frame1);
        level6FrameList.Add(level6Frame2);
        level6FrameList.Add(level6Frame3);
		
		//Load backside textures -> J.C.
		BacksideLevel6FrameList.Add(BacksideLevel6Frame1);
		BacksideLevel6FrameList.Add(BacksideLevel6Frame2);
		BacksideLevel6FrameList.Add(BacksideLevel6Frame3);
		
		//Add to list storing level background animation -> J.C.
		level7FrameList.Add(level7Frame1);
        level7FrameList.Add(level7Frame2);
        level7FrameList.Add(level7Frame3);
		
		//Load backside textures -> J.C.
		BacksideLevel7FrameList.Add(BacksideLevel7Frame1);
		BacksideLevel7FrameList.Add(BacksideLevel7Frame2);
		BacksideLevel7FrameList.Add(BacksideLevel7Frame3);
		
		//Add to list storing level background animation -> J.C.
		level8FrameList.Add(level8Frame1);
        level8FrameList.Add(level8Frame2);
        level8FrameList.Add(level8Frame3);
		
		//Load backside textures -> J.C.
		BacksideLevel8FrameList.Add(BacksideLevel8Frame1);
		BacksideLevel8FrameList.Add(BacksideLevel8Frame2);
		BacksideLevel8FrameList.Add(BacksideLevel8Frame3);
		
		//Add to list storing level background animation -> J.C.
		level9FrameList.Add(level9Frame1);
        level9FrameList.Add(level9Frame2);
        level9FrameList.Add(level9Frame3);
		
		//Load backside textures -> J.C.
		BacksideLevel9FrameList.Add(BacksideLevel9Frame1);
		BacksideLevel9FrameList.Add(BacksideLevel9Frame2);
		BacksideLevel9FrameList.Add(BacksideLevel9Frame3);
		
		//Add to list storing level background animation -> J.C.
		level10FrameList.Add(level10Frame1);
        level10FrameList.Add(level10Frame2);
        level10FrameList.Add(level10Frame3);
		
		//Load backside textures -> T.R.
		BacksideLevel10FrameList.Add(BacksideLevel10Frame1);
		BacksideLevel10FrameList.Add(BacksideLevel10Frame2);
		BacksideLevel10FrameList.Add(BacksideLevel10Frame3);
		
		//Add to list storing level background animation -> J.C.
		level11FrameList.Add(level11Frame1);
        level11FrameList.Add(level11Frame2);
        level11FrameList.Add(level11Frame3);
		
		//Load backside textures -> T.R.
		BacksideLevel11FrameList.Add(BacksideLevel11Frame1);
		BacksideLevel11FrameList.Add(BacksideLevel11Frame2);
		BacksideLevel11FrameList.Add(BacksideLevel11Frame3);
		
		//Add to list storing level background animation -> J.C.
		level12FrameList.Add(level12Frame1);
        level12FrameList.Add(level12Frame2);
        level12FrameList.Add(level12Frame3);
		
		//Load backside textures -> T.R.
		BacksideLevel12FrameList.Add(BacksideLevel12Frame1);
		BacksideLevel12FrameList.Add(BacksideLevel12Frame2);
		BacksideLevel12FrameList.Add(BacksideLevel12Frame3);
		
		//Add to list storing level background animation -> J.C.
		level13FrameList.Add(level13Frame1);
        level13FrameList.Add(level13Frame2);
        level13FrameList.Add(level13Frame3);
		
		//Load backside textures -> T.R.
		BacksideLevel13FrameList.Add(BacksideLevel13Frame1);
		BacksideLevel13FrameList.Add(BacksideLevel13Frame2);
		BacksideLevel13FrameList.Add(BacksideLevel13Frame3);
		
		//Add to list storing level background animation -> J.C.
		level14FrameList.Add(level14Frame1);
        level14FrameList.Add(level14Frame2);
        level14FrameList.Add(level14Frame3);
		
		//Load backside textures -> T.R.
		BacksideLevel14FrameList.Add(BacksideLevel14Frame1);
		BacksideLevel14FrameList.Add(BacksideLevel14Frame2);
		BacksideLevel14FrameList.Add(BacksideLevel14Frame3);
		
		//Add to list storing level background animation -> J.C.
		level15FrameList.Add(level15Frame1);
        level15FrameList.Add(level15Frame2);
        level15FrameList.Add(level15Frame3);
		
		//Add to list storing level background animation -> J.C.
		BacksideLevel15FrameList.Add(BacksideLevel15Frame1);
        BacksideLevel15FrameList.Add(BacksideLevel15Frame2);
        BacksideLevel15FrameList.Add(BacksideLevel15Frame3);
		
		//Add to list storing level background animation -> J.C.
		level16FrameList.Add(level16Frame1);
        level16FrameList.Add(level16Frame2);
        level16FrameList.Add(level16Frame3);
		
		//Load backside textures -> T.R.
		BacksideLevel16FrameList.Add(BacksideLevel16Frame1);
		BacksideLevel16FrameList.Add(BacksideLevel16Frame2);
		BacksideLevel16FrameList.Add(BacksideLevel16Frame3);
		
		//Add to list storing level background animation -> J.C.
		level17FrameList.Add(level17Frame1);
        level17FrameList.Add(level17Frame2);
        level17FrameList.Add(level17Frame3);
		
		//Load backside textures -> T.R.
		BacksideLevel17FrameList.Add(BacksideLevel17Frame1);
		BacksideLevel17FrameList.Add(BacksideLevel17Frame2);
		BacksideLevel17FrameList.Add(BacksideLevel17Frame3);
		
		//Add to list storing level background animation -> J.C.
		level18FrameList.Add(level18Frame1);
        level18FrameList.Add(level18Frame2);
        level18FrameList.Add(level18Frame3);
		
		//Load backside textures -> T.R.
		BacksideLevel18FrameList.Add(BacksideLevel18Frame1);
		BacksideLevel18FrameList.Add(BacksideLevel18Frame2);
		BacksideLevel18FrameList.Add(BacksideLevel18Frame3);
		
		//Add to list storing level background animation -> J.C.
		level19FrameList.Add(level19Frame1);
        level19FrameList.Add(level19Frame2);
        level19FrameList.Add(level19Frame3);
		
		//Add to list storing level background animation -> J.C.
		level20FrameList.Add(level20Frame1);
        level20FrameList.Add(level20Frame2);
        level20FrameList.Add(level20Frame3);
		
		//Add to list storing level background animation -> J.C.
		level21FrameList.Add(level21Frame1);
        level21FrameList.Add(level21Frame2);
        level21FrameList.Add(level21Frame3);
		
		//Add to list storing level background animation -> J.C.
		level22FrameList.Add(level22Frame1);
        level22FrameList.Add(level22Frame2);
        level22FrameList.Add(level22Frame3);
		
		//Add to list storing level background animation -> J.C.
		level23FrameList.Add(level23Frame1);
        level23FrameList.Add(level23Frame2);
        level23FrameList.Add(level23Frame3);
		
		//Add to list storing level background animation -> J.C.
		level24FrameList.Add(level24Frame1);
        level24FrameList.Add(level24Frame2);
        level24FrameList.Add(level24Frame3);
		
		//Add to list storing level background animation -> J.C.
		level25FrameList.Add(level25Frame1);
        level25FrameList.Add(level25Frame2);
        level25FrameList.Add(level25Frame3);

        finalLevel1FrameList.Add(finalLevel1Frame1);
        finalLevel6FrameList.Add(finalLevel6Frame1);
        finalLevel7FrameList.Add(finalLevel7Frame1);
        finalLevel8FrameList.Add(finalLevel8Frame1);
        finalLevel9FrameList.Add(finalLevel9Frame1);
		
		loadingFrameList.Add(loadingFrame1);
		loadingFrameList.Add(loadingFrame2);
		loadingFrameList.Add(loadingFrame3);
        //douglas -is this coded needed at all? looks like its not used by anything
        allAnimations.Clear();
        allAnimations.Add(slideUpAnimation);
        allAnimations.Add(slideDownAnimation);
        allAnimations.Add(openAnimation);
        allAnimations.Add(deathAnimation);
        allAnimations.Add(landWallAnimation);
        allAnimations.Add(idleAnimationA);
        allAnimations.Add(idleAnimationB);
        allAnimations.Add(walkAnimation);
        allAnimations.Add(standAnimation);
        allAnimations.Add(idleAnimation);
        allAnimations.Add(jumpAnimation);
        currFrame = 0;
        timeToWait = 1 / TRANSITION_FPS;

        currentAnimType = AnimationType.STAND;
        currentAnim = standAnimation;
        firstInGameLoop = true;
        reversed = false;
		
		//The following is initializes the local tearManagerRef
        if (GameObject.FindGameObjectWithTag("TearManager"))
		    TearManagerRef = GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>();

    }

    public void OnEnable()
    {
        currentAnimType = AnimationType.STAND;
        currentAnim = standAnimation;
        firstInGameLoop = true;
    }

    private void InitGameRefs()
    {
        if (playerObject)
        {
            if (characterController == null)
                characterController = playerObject.GetComponent<TWCharacterController>();
        }

        else
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }

        if (characterController)
            if (!characterController.playerIsDead && deathAnimComplete)
                deathAnimComplete = false;


    }
    


    public void AssignLevelTextures(bool levelComplete)
    {
        if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level1Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level1FrameList;
				currentLevelBacksideTextures = BacksideLevel1FrameList;
			}
			
            else
			{
                currentLevelTextures = level2FrameList;//Modified by J.C.
				//currentLevelBacksideTextures = BacksideLevel2FrameList;
			}
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level2Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level2FrameList;
				currentLevelBacksideTextures = BacksideLevel1FrameList;
			}

            else
                currentLevelTextures = level3FrameList;
        }
		
        else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level3Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level3FrameList;
				currentLevelBacksideTextures = Backsidelevel3FrameList;
			}

            else
                currentLevelTextures = level4FrameList;//Modified by J.C.
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level4Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level4FrameList;
				currentLevelBacksideTextures = BacksideLevel1FrameList;
			}

            else
            {
                currentLevelTextures = level5FrameList;
				currentLevelBacksideTextures = Backsidelevel5FrameList;
			}
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level5Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level5FrameList;
				currentLevelBacksideTextures = Backsidelevel5FrameList;
			}

            else
           {
                currentLevelTextures = level6FrameList;
				currentLevelBacksideTextures = BacksideLevel6FrameList;
			}
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level6Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level6FrameList;
				currentLevelBacksideTextures = BacksideLevel6FrameList;
			}

            else
            {
                currentLevelTextures = level7FrameList;
				currentLevelBacksideTextures = BacksideLevel7FrameList;
			}
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level7Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level7FrameList;
				currentLevelBacksideTextures = BacksideLevel7FrameList;
			}

            else
            {
                currentLevelTextures = level8FrameList;
				currentLevelBacksideTextures = BacksideLevel8FrameList;
			}
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level8Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level8FrameList;
				currentLevelBacksideTextures = BacksideLevel8FrameList;
			}

            else
            {
                currentLevelTextures = level9FrameList;
				currentLevelBacksideTextures = BacksideLevel9FrameList;
			}
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level9Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level9FrameList;
				currentLevelBacksideTextures = BacksideLevel9FrameList;
			}

            else
                currentLevelTextures = level10FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level10Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level10FrameList;
				currentLevelBacksideTextures = BacksideLevel10FrameList;
			}

            else
                currentLevelTextures = level11FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level11Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level11FrameList;
				currentLevelBacksideTextures = BacksideLevel11FrameList;
			}

            else
                currentLevelTextures = level12FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level12Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level12FrameList;
				currentLevelBacksideTextures = BacksideLevel12FrameList;
			}
            else
                currentLevelTextures = level13FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level13Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level13FrameList;
				currentLevelBacksideTextures = BacksideLevel13FrameList;
			}

            else
                currentLevelTextures = level14FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level14Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level14FrameList;
				currentLevelBacksideTextures = BacksideLevel14FrameList;
			}

            else
			{
                currentLevelTextures = level15FrameList;
				currentLevelBacksideTextures = BacksideLevel15FrameList;
			}
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level15Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level15FrameList;
				currentLevelBacksideTextures = BacksideLevel15FrameList;
			}

            else
                currentLevelTextures = level16FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level16Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level16FrameList;
				currentLevelBacksideTextures = BacksideLevel16FrameList;
			}
				
            else
                currentLevelTextures = level17FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level17Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level16FrameList;
				currentLevelBacksideTextures = BacksideLevel16FrameList;
			}

            else
                currentLevelTextures = level17FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level18Name)
        {
            if (!levelComplete)
			{
                currentLevelTextures = level17FrameList;
				currentLevelBacksideTextures = BacksideLevel17FrameList;
			}

            else
                currentLevelTextures = level18FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level19Name)
        {
            if (!levelComplete)
                currentLevelTextures = level19FrameList;

            else
                currentLevelTextures = level20FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level20Name)
        {
            if (!levelComplete)
                currentLevelTextures = level20FrameList;

            else
                currentLevelTextures = level21FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level21Name)
        {
            if (!levelComplete)
                currentLevelTextures = level21FrameList;

            else
                currentLevelTextures = level22FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level22Name)
        {
            if (!levelComplete)
                currentLevelTextures = level22FrameList;

            else
                currentLevelTextures = level23FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level23Name)
        {
            if (!levelComplete)
                currentLevelTextures = level23FrameList;

            else
                currentLevelTextures = level24FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level24Name)
        {
            if (!levelComplete)
                currentLevelTextures = level24FrameList;

            else
                currentLevelTextures = level25FrameList;
        }
		
		//Added by J.C.
		else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == level25Name)
        {
            if (!levelComplete)
                currentLevelTextures = level25FrameList;

            else
                currentLevelTextures = finalLevel6FrameList;//TODO ADD POINTER TO LEVEL 26 -> J.C.
        }

        else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == finalLevel6Name)
        {
            if (!levelComplete)
                currentLevelTextures = finalLevel6FrameList;

            else
                currentLevelTextures = finalLevel7FrameList;
        }

        else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == finalLevel7Name)
        {
            if (!levelComplete)
                currentLevelTextures = finalLevel7FrameList;

            else
                currentLevelTextures = finalLevel8FrameList;
        }

        else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == finalLevel8Name)
        {
            if (!levelComplete)
                currentLevelTextures = finalLevel8FrameList;

            else
                currentLevelTextures = finalLevel9FrameList;
        }

        else if ("Assets/Scenes/" + Application.loadedLevelName + ".unity" == finalLevel9Name)
        {
            if (!levelComplete)
                currentLevelTextures = finalLevel9FrameList;

            else
                currentLevelTextures = finalLevel1FrameList;
        }
		else if("Assets/Scenes/" + Application.loadedLevelName + ".unity" == loadingName)
		{
				currentLevelTextures = loadingFrameList;
		}
        currLevelFrame = 0;
		currLevelBacksideFrame = 0;

        if (levelComplete)
            levelChange = true;
    }

    private void OnInit()
    {
        if (firstInGameLoop && playerObject)
        {
            paperObject = GameObject.FindGameObjectsWithTag("background")[0];
			backsidePaperObject = GameObject.FindGameObjectWithTag("Fold");

            if (levelChange && currLevelFrame < currentLevelTextures.Count())
            {
                //UnityEngine.Debug.Log("CHANGE");
                paperObject.renderer.sharedMaterial.mainTexture = currentLevelTextures[currLevelFrame];
            }
			
			if(levelChange && currLevelBacksideFrame < currentLevelBacksideTextures.Count())
			{
				//UnityEngine.Debug.Log("CHANGE BACKSIDE");
				backsidePaperObject.renderer.sharedMaterial.mainTexture = currentLevelBacksideTextures[currLevelBacksideFrame];
			}

            playerObject.transform.Find("Graphics").localScale =
                new Vector3(Mathf.Abs(playerObject.transform.Find("Graphics").localScale.x) * -1,
                      playerObject.transform.Find("Graphics").localScale.y,
                      playerObject.transform.Find("Graphics").localScale.z);
            inputManagerRef.currDirection = AnimationDirection.RIGHT;
            currentDirection = AnimationDirection.RIGHT;
            firstInGameLoop = false;
            ChooseAnimation(currentAnim);

            //UnityEngine.Debug.Log("name " + EditorApplication.currentScene);



            //if (levelHasAnims)
            {
                //UnityEngine.Debug.Log("NAME " + "Assets/Scenes/" + Application.loadedLevelName + ".unity");
                AssignLevelTextures(false);
				StopCoroutine("AnimateLevel");
                StartCoroutine("AnimateLevel",
					"Assets/Scenes/" + Application.loadedLevelName + ".unity");
            }
        }
    }

    public void Update()
    {
        if (gameStateManagerRef.inGame)
        {
            // Ensure all game objects and references are declared and not null
            InitGameRefs();
            // On first update iteration, ensure player animations are set correctly
            OnInit();
        }

        // UnityEngine.Debug.Log(GetAnimationDirection().ToString());

    }



    private void PopulateFallAnimation(List<Texture2D> fallAnimList)
    {
        fallAnimList.Add(fall1);
        fallAnimList.Add(fall2);
        fallAnimList.Add(fall3);
        fallAnimList.Add(fall4);
        fallAnimList.Add(fall5);
        fallAnimList.Add(fall6);
        fallAnimList.Add(fall7);
        fallAnimList.Add(fall8);
        fallAnimList.Add(fall9);
        fallAnimList.Add(fall10);
    }

    /// <summary>
    /// Popularps the open animation list.
    /// </summary>
    /// <param name='openAnimList'>
    /// Open animation list.
    /// </param>
    private void PopulateOpenAnimation(List<Texture2D> openAnimList)
    {
        openAnimList.Add(open1);
        openAnimList.Add(open2);
        openAnimList.Add(open3);
        openAnimList.Add(open4);
        openAnimList.Add(open5);
        openAnimList.Add(open6);
        openAnimList.Add(open7);
        openAnimList.Add(open8);
    }

    /// <summary>
    /// Populates the death animation.
    /// </summary>
    /// <param name='deathList'>
    /// Death list.
    /// </param>
    private void PopulateDeathAnimation(List<Texture2D> deathList)
    {
        deathList.Add(death1);
        deathList.Add(death2);
        deathList.Add(death3);
        deathList.Add(death4);
        deathList.Add(death5);
        deathList.Add(death6);
        deathList.Add(death7);
        deathList.Add(death8);
        deathList.Add(death9);
        deathList.Add(death10);
        deathList.Add(death11);
        deathList.Add(death12);
        deathList.Add(death13);
        deathList.Add(death14);
        deathList.Add(death15);
        deathList.Add(death16);
        deathList.Add(death17);
        deathList.Add(death18);
        deathList.Add(death19);
        deathList.Add(death20);
        deathList.Add(death21);
        deathList.Add(death22);
        deathList.Add(death23);
    }


    /// <summary>
    /// Populates the slide up animation.
    /// </summary>
    /// <param name='slideUpList'>
    /// Slide up list.
    /// </param>
    private void PopulateSlideUpAnimation(List<Texture2D> slideUpList)
    {
        slideUpList.Add(slideUp1);
        slideUpList.Add(slideUp2);
        slideUpList.Add(slideUp3);
        slideUpList.Add(slideUp4);
        slideUpList.Add(slideUp5);
        slideUpList.Add(slideUp6);
        slideUpList.Add(slideUp7);
        slideUpList.Add(slideUp8);
        slideUpList.Add(slideUp9);
        slideUpList.Add(slideUp10);
        slideUpList.Add(slideUp11);
        slideUpList.Add(slideUp12);
    }

    /// <summary>
    /// Populates the slide down animation.
    /// </summary>
    /// <param name='slideDownList'>
    /// Slide down list.
    /// </param>

    private void PopulateSlideDownAnimation(List<Texture2D> slideDownList)
    {
        slideDownList.Add(slideDown1);
        slideDownList.Add(slideDown2);
        slideDownList.Add(slideDown3);
        slideDownList.Add(slideDown4);
        slideDownList.Add(slideDown5);
        slideDownList.Add(slideDown6);
        slideDownList.Add(slideDown7);
        slideDownList.Add(slideDown8);
        slideDownList.Add(slideDown9);
        slideDownList.Add(slideDown10);
        slideDownList.Add(slideDown11);
        slideDownList.Add(slideDown12);

    }

    /// <summary>
    /// Populates the land wall animation.
    /// </summary>
    /// <param name='landWallList'>
    /// Land wall list.
    /// </param>

    private void PopulateLandWallAnimation(List<Texture2D> landWallList)
    {
        landWallList.Add(landWall1);
        landWallList.Add(landWall2);
        landWallList.Add(landWall3);
        landWallList.Add(landWall4);
        landWallList.Add(landWall5);
        landWallList.Add(landWall6);
        landWallList.Add(landWall7);
        landWallList.Add(landWall8);
        landWallList.Add(landWall9);
        landWallList.Add(landWall10);
        landWallList.Add(landWall11);
        landWallList.Add(landWall12);
    }

    /// <summary>
    /// Populates the idle animation A list.
    /// </summary>
    /// <param name='idleAnimAList'>
    /// Idle animation A list.
    /// </param>
    private void PopulateIdleAnimationA(List<Texture2D> idleAnimAList)
    {
        idleAnimAList.Add(idleA1);
        idleAnimAList.Add(idleA2);
        idleAnimAList.Add(idleA3);
    }

    /// <summary>
    /// Populars the idle animation B list.
    /// </summary>
    /// <param name='idleAnimBList'>
    /// Idle animation B list.
    /// </param>
    private void PopulateIdleAnimationB(List<Texture2D> idleAnimBList)
    {
        idleAnimBList.Add(idleB1);
        idleAnimBList.Add(idleB2);
        idleAnimBList.Add(idleB3);
    }
    /// <summary>
    /// Populates the input parameter with a list of the walk images 
    /// </summary>
    /// <param name="walkList"></param>
    private void PopulateJumpAnimations(List<Texture2D> jumpList)
    {
        jumpList.Add(jump1);
        jumpList.Add(jump2);
        jumpList.Add(jump3);
        jumpList.Add(jump4);
        jumpList.Add(jump5);
        jumpList.Add(jump6);
        jumpList.Add(jump7);
        jumpList.Add(jump8);
        jumpList.Add(jump9);
        jumpList.Add(jump10);
        jumpList.Add(jump11);
        jumpList.Add(jump12);
        jumpList.Add(jump13);
        jumpList.Add(jump14);
        jumpList.Add(jump15);
        jumpList.Add(jump16);
        jumpList.Add(jump17);
    }

    /// <summary>
    /// Populates the input parameter with a list of the walk images 
    /// </summary>
    /// <param name="walkList"></param>
    private void PopulateWalkAnimations(List<Texture2D> walkList)
    {
        walkList.Add(walk1);
        walkList.Add(walk2);
        walkList.Add(walk3);
        walkList.Add(walk4);
        walkList.Add(walk5);
        walkList.Add(walk6);
        walkList.Add(walk7);
        walkList.Add(walk8);
    }

    /// <summary>
    /// Populates the input paramter with a list of idle list
    /// </summary>
    /// <param name="idleList"></param>
    private void PopulateIdleAnimations(List<Texture2D> idleList)
    {
        idleList.Add(idle1);
        idleList.Add(idle2);
        idleList.Add(idle3);
        idleList.Add(idle4);
        idleList.Add(idle5);
        idleList.Add(idle6);
        idleList.Add(idle7);
        idleList.Add(idle8);
        idleList.Add(idle9);
        idleList.Add(idle10);
        idleList.Add(idle11);
        idleList.Add(idle12);
        idleList.Add(idle13);
        idleList.Add(idle14);
        idleList.Add(idle15);
        idleList.Add(idle16);
        idleList.Add(idle17);
        idleList.Add(idle18);
        idleList.Add(idle19);
        idleList.Add(idle20);
        idleList.Add(idle21);
    }

    /// <summary>
    /// Populates the input parameter with a list of standing animations
    /// </summary>
    /// <param name="standList"></param>
    private void PopulateStandAnimations(List<Texture2D> standList)
    {
        standList.Add(stand1);
        standList.Add(stand2);
        standList.Add(stand3);
        standList.Add(stand4);
        standList.Add(stand5);
        standList.Add(stand6);
        standList.Add(stand7);
        standList.Add(stand8);
        standList.Add(stand9);
        standList.Add(stand10);
        standList.Add(stand11);
        standList.Add(stand12);
        standList.Add(stand13);
        standList.Add(stand14);
        standList.Add(stand15);
        standList.Add(stand16);
    }

    /// <summary>
    /// Choose which animation you want to use.
    /// This is determined by the string name
    /// of the wanted Texture2D.
    /// </summary>
    private void ChooseAnimation(Animation currentAnimation)
    {
		StopCoroutine("Animate");
        StartCoroutine("Animate");
    }

    private void OnLastFrame()
    {
        // JUSTIN, NOTICE I REPLACED THE FOLLOWING WITH
        // 'currentAnimType' for 'AnimationType' and not
        // 'currentAnim' for 'Animation'
        if (currentAnimType.Equals(AnimationType.IDLE))
        {
            // Trigger Animation will set the currFrame to zero
            // so we don't have to do it here
            inputManagerRef.idlePlayerWatch.Reset();
            inputManagerRef.idlePlayerWatch.Start();
        }

        else if (currentAnimType.Equals(AnimationType.JUMP) && !characterController.getGrounded())
        {
            // freeze the frame, we don't actually have to do anything here
            // since currFrame is only incremented if not greater or equal
            // the current animation length
            currFrame--;
            //UnityEngine.Debug.Log("NO");
        }

        // JUSTIN, I ADDED THIS JUST BECAUSE WE HAVE NOTHING TO TRIGGER A STOP IN JUMP WHEN WERE SUPPOSED
        // TO DO WHEN WE HIT THE GROUND
        else if (currentAnimType.Equals(AnimationType.JUMP) && characterController.getGrounded())
        {
            TriggerAnimation(AnimationType.STAND);
            //UnityEngine.Debug.Log("NO2");
        }

        // Alert TWController that they can respawn the 
        // player now.  
        else if (currentAnimType.Equals(AnimationType.DEATH))
        {
            deathAnimComplete = true;
        }

        // else this is not a special animation
        // so we can just set back to zero for looping
        else
        {
            //UnityEngine.Debug.Log("NO3");
            currFrame = 0;
        }
    }

    private float DetermineLevelAnimSpeed(string currLevel)
    {
        if (currLevel == level1Name)
            return LEVEL_1_FPS;

        else
            return LEVEL_3_FPS;
    }


    private float DeterminePlayerAnimSpeed()
    {
        if (currentAnim.Equals(jumpAnimation) || currentAnim.Equals(fallAnimation) || currentAnim.Equals(deathAnimation))
        {
            return JUMP_FPS;
        }

        else if (currentAnim.Equals(walkAnimation) || currentAnim.Equals(standAnimation))
        {
            return WALK_FPS;
        }

        else if (currentAnim.Equals(idleAnimation) || currentAnim.Equals(fallAnimation))
        {
            return IDLE_FPS;
        }

        else
        {
            return TRANSITION_FPS;
        }
    }


    /// <summary>
    /// Peforms the actual iteration through the sprite sheet
    /// </summary>
    /// <param name="currentAnimation"></param>
    /// <returns></returns>
    private IEnumerator Animate()
    {
        //UnityEngine.Debug.Log("CURR FRAME " + currFrame.ToString());
        //Debug.Log (currentAnimType.ToString ());

        // make sure this is only called
        // when in game.  If we end up having
        // animations in the UI, then we need to change this
        if (gameStateManagerRef.inGame && inputManagerRef)
        {
            // IF THE CURRENT FRAME IS GREATER OR EQUAL
            // TO MAX NUMBER OF FRAMES FOR THIS ANIM
            // THEN WE EITHER SET IT TO ZERO (LOOPING)
            // OR FREEZE IT AT THE LAST FRAME (JUMP)
            // JUSTIN, NOTICE I ADDED WHILE WERE NOT CURRENTLY STANDING SINCE STAND ONLY HAS
            // ONE ANIMATION
            if (currFrame >= (currentAnim.GetLength() - 1)) // && !currentAnimType.Equals(AnimationType.STAND))
            {
                // Ensure last frame properties 
                // are met: 
                // jump stays on last frame until touching ground
                // death allows the player to respond, etc
                OnLastFrame();
            }

            // ELSE, WE HAVENT HIT THE LAST FRAME
            // SO CONTINUE TO INCREMENT THE NEXT
            // FRAME TO BE PLAYED
            // JUSTIN, NOTICE I ADDED IF NOT STAND ANIM
            // SINCE WALK ONLY HAS ONE ANIM AND DOESN'T NEED
            // TO CYCLE
            else //if (!currentAnimType.Equals(AnimationType.STAND))
            {
                //UnityEngine.Debug.Log("NO");
                currFrame++;
            }

//			Debug.Log(DeterminePlayerAnimSpeed());
            /***** THIS ALLOWS COROUTINE TO ONLY PLAY SO MANY FRAMES PER SECOND *****/
            yield return new WaitForSeconds(DeterminePlayerAnimSpeed());



            /* THIS ACTUALLY PLAYS THE CURRENT FRAME */
            if (playerObject)
            {
                playerObject.transform.Find("Graphics").renderer.sharedMaterial.mainTexture =
                        currentAnim.GetFrames()[currFrame];

                //if (inputManagerRef.PlayerSelected())
                //{
                //    GameObject.FindGameObjectWithTag("Player").transform.Find("Graphics").renderer.sharedMaterial.color = Color.blue;
                //}

                //else
                //{
                //    GameObject.FindGameObjectWithTag("Player").transform.Find("Graphics").renderer.sharedMaterial.color = Color.white;
                //}
            }

            /* THIS IS THE CALLBACK THAT LETS US PLAY ONLY SO MANY ANIMS PER FRAME */
			StopCoroutine("Animate");
            StartCoroutine("Animate");
        }
    }

    private int currLevelFrame = 0;
	private int currLevelBacksideFrame = 0;
	
    private IEnumerator AnimateLevel(string currLevel)
    {
        //UnityEngine.Debug.Log("NUM " + currLevelFrame);

        if (currLevelFrame >= currentLevelTextures.Count - 1)
            currLevelFrame = 0;

        else
            currLevelFrame++;
		
		if (currLevelBacksideFrame >= currentLevelBacksideTextures.Count - 1)
            currLevelBacksideFrame = 0;

        else
            currLevelBacksideFrame++;
        yield return new WaitForSeconds(DetermineLevelAnimSpeed(currLevel));
		
		if(TearManagerRef == null){ TearManagerRef = GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>(); }
		if(TearManagerRef != null && TearManagerRef.TearFinished){
			//ensure that the object references are intialized - > J.C.
			if(CutPiecePaper == null && TearManagerRef != null && TearManagerRef.MainWorldCutPaper != null) CutPiecePaper = TearManagerRef.MainWorldCutPaper;
			if(WorldPiecePaper == null && TearManagerRef != null && TearManagerRef.MainWorldPaper != null) WorldPiecePaper = TearManagerRef.MainWorldPaper;
			
			//Now update the torn pieces to animate level background art
			if(currentLevelTextures != null &&
				currentLevelTextures.Count() > 0 &&
				currLevelFrame < currentLevelTextures.Count() &&
				currentLevelTextures[currLevelFrame] != null)
			{
				CutPiecePaper.renderer.sharedMaterial.mainTexture = currentLevelTextures[currLevelFrame];
				WorldPiecePaper.renderer.sharedMaterial.mainTexture = currentLevelTextures[currLevelFrame];
			}
			
			//Update backside texture accordingly -> J.C.
			if(currLevelBacksideFrame < currentLevelBacksideTextures.Count())
			{
				backsidePaperObject.renderer.sharedMaterial.mainTexture = currentLevelBacksideTextures[currLevelBacksideFrame];
			}
		}
		else
      //  if (TearManagerRef != null && !TearManagerRef.TearFinished)
        {
			//The following check is for NON Animated levels -> J.C.
			if(currLevelFrame < currentLevelTextures.Count())
			{
				if(paperObject != null)
					paperObject.renderer.sharedMaterial.mainTexture = currentLevelTextures[currLevelFrame];
			}
			
			//Update backside texture accordingly -> J.C.
            if (backsidePaperObject)
            {
                if (currLevelBacksideFrame < currentLevelBacksideTextures.Count())
                {
                    backsidePaperObject.renderer.sharedMaterial.mainTexture = currentLevelBacksideTextures[currLevelBacksideFrame];
                }
            }
			
        }
		
		//else 
//		else
//		{
//			//The following should NOT be called -> J.C.
//			UnityEngine.Debug.Log("Poopy, ONLY IS THIS IS CALLED OUTSIDE LEVEL TRANSITION OR RESET --- there is an Animating Level Material swap error - JOHN C. FIX");
//		}
		StopCoroutine("AnimateLevel");
        StartCoroutine("AnimateLevel",currLevel);
    }


    /// <summary>
    /// Sets the current animation to be rendered
    /// </summary>
    public void TriggerAnimation(AnimationType type)
    {
        if (gameStateManagerRef)
        {
            // make sure this is only called when in game
            if (gameStateManagerRef.inGame)
            {
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
                        if (characterController != null)
                            if (!characterController.getRising())
                                currentAnim = standAnimation;
                        break;

                    case AnimationType.JUMP:
                        currentAnim = jumpAnimation;
                        break;

                    case AnimationType.FALL:
                        currentAnim = fallAnimation;
                        break;

                    case AnimationType.DEATH:
                        currentAnim = deathAnimation;
                        break;

                    default:
                        currentAnim = idleAnimation;
                        break;
                }

                //douglas - does this if statement do something?  i can't find restartAnim used by anything besdies this if statement
                if (!currentAnim.Equals(standAnimation) && !restartAnim)
                    restartAnim = true;


                currentAnimType = type;
            }
        }
    }

    public void SetDirection(AnimationDirection dir)
    {
        if (currentDirection != dir)
        {
            //douglas - Added this here
            currentDirection = dir; //no need to make it equal outside of if statement
            playerObject.transform.Find("Graphics").localScale =
              new Vector3(playerObject.transform.Find("Graphics").localScale.x * -1,
                          playerObject.transform.Find("Graphics").localScale.y,
                          playerObject.transform.Find("Graphics").localScale.z);
        }

    }

    public AnimationType GetCurrentAnimationType()
    {
        return currentAnimType;
    }

    public AnimationDirection GetAnimationDirection()
    {
        return currentDirection;
    }

    public int GetCurrentFrame()
    {
        return currFrame;
    }
}
