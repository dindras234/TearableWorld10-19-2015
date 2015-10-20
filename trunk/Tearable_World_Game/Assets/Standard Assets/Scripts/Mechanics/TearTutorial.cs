using UnityEngine;
using System.Collections;

public class TearTutorial : MonoBehaviour 
{
	/// <summary>
	/// The tutorial page 1
	/// </summary>
	public Material TutorialPage1;
	
	/// <summary>
	/// The tutorial page 2
	/// </summary>
	public Material TutorialPage2;
	
	/// <summary>
	/// The tutorial page 3
	/// </summary>
	public Material TutorialPage3;
	
	/// <summary>
	/// The tutorial page 4
	/// </summary>
	public Material TutorialPage4;
	
	/// <summary>
	/// The tutorial page 5
	/// </summary>
	public Material TutorialPage5;
	
	/// <summary>
	/// The main world paper.
	/// </summary>
	public GameObject MainWorldPaper;
	
	/// <summary>
	/// The tutorial finished.
	/// </summary>
	private bool tutorialFinished;
	
	/// <summary>
	/// The current page number.
	/// </summary>
	private int currentPageNumber = 1;
	
	/// <summary>
	/// total number of pages in the tear tutorial
	/// </summary>
	public int TOTAL_NUM_TUTORIAL_PAGES = 5;
	
	/// <summary>
	/// The local tear manager reference.
	/// </summary>
	private TearManager tearManagerRef;
	
	/// <summary>
	/// The restart object.
	/// </summary>
	public GameObject RestartObjectRef;
	
	/// <summary>
	/// The player.
	/// </summary>
	public GameObject PlayerRef;
	
	/// <summary>
	/// The game state manager reference.
	/// </summary>
	private GameStateManager gameStateManagerRef;
	
	/// <summary>
	/// The block input.
	/// </summary>
	private bool BlockInput = false;
	
	/// <summary>
	/// The blocking timer.
	/// </summary>
	private float BlockingTimer = 0;
	
	/// <summary>
	/// The time to be blocked.
	/// </summary>
	private float TimeToBeBlocked = 120;
	
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start () 
	{
		//Start tutorial
		tutorialFinished = false;
		
		//Make sure the tearPaper script is disabled during tutorial
		MainWorldPaper.GetComponent<TearPaper>().enabled = false;
		
		//initialize the local reference to the tear manager
		tearManagerRef = GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>();
		
		
		GameObject mainObject = GameObject.FindGameObjectsWithTag("MainObject")[0];

        if (GameObject.FindGameObjectsWithTag("MainObject").Length > 1)
        {
            GameObject[] mainObjectList = GameObject.FindGameObjectsWithTag("MainObject");
            for (int i = 0; i < mainObjectList.Length; ++i)
            {
                if (mainObjectList[i].GetComponent<GameStateManager>().objectSaved)
                    mainObject = mainObjectList[i];
            }
        }

        // Ensures all necessary scripts are added for the MainObject
        gameStateManagerRef = mainObject.GetComponent<GameStateManager>();
		if(gameStateManagerRef.GetStatisticManager() != null)
		{
			if(Application.loadedLevel < gameStateManagerRef.GetStatisticManager().LevelScoresDict.Count + 1) 
				tutorialFinished = true;
		}
	}
	

	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update () 
	{
		if(!gameStateManagerRef.inGame || gameStateManagerRef.isPaused) BlockInput = true;
		
		//Check to trigger tutorial finished
		if(currentPageNumber > TOTAL_NUM_TUTORIAL_PAGES && !tutorialFinished) 
		{
			tutorialFinished = true;
		}
		else
		{
			if(RestartObjectRef.activeSelf)
			{
				RestartObjectRef.SetActive(false);
				PlayerRef.GetComponent<TWCharacterController>().enabled = false;
			}
		}
		
		//Check to finish tutorial
		if(tutorialFinished && !MainWorldPaper.GetComponent<TearPaper>().enabled)
		{
			//Turne tear paper script back on for tearing paper
			MainWorldPaper.GetComponent<TearPaper>().enabled = true;
			
			RestartObjectRef.SetActive(true);
			PlayerRef.GetComponent<TWCharacterController>().enabled = true;
			
			//Make sure the tutorial object is properly hidden
			GetComponent<MeshRenderer>().enabled = false;
			gameObject.SetActive(false);
			
			
		}
		
		//increment current Page number when input detected to turn page of tutorial
		if(gameStateManagerRef.inGame && !gameStateManagerRef.isPaused
			&& 
			tearManagerRef.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_RELEASED)
			
			//(
			//tearManagerRef.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_PRESSED) 
				//|| tearManagerRef.GetInputManager().GetcurrPressState().Equals(InputManager.PressState.JUST_PRESSED)
			//)
			&& !BlockInput
			)
		{
			++currentPageNumber;
		}
		
		else if(BlockInput)
		{
			BlockingTimer += Time.deltaTime * 100;
			
			if(BlockingTimer > TimeToBeBlocked)
			{
				BlockInput = false;
				BlockingTimer = 0;
			}
		}

		
		//CHECK TO SWAP PAGE MATERIAL
		if(currentPageNumber == 1 && gameObject.renderer.material != TutorialPage1)
		{
			gameObject.renderer.material = TutorialPage1;
		}
		else if(currentPageNumber == 2 && gameObject.renderer.material != TutorialPage2)
		{
			gameObject.renderer.material = TutorialPage2;
		}
		else if(currentPageNumber == 3 && gameObject.renderer.material != TutorialPage3)
		{
			gameObject.renderer.material = TutorialPage3;
		}
		else if(currentPageNumber == 4 && gameObject.renderer.material != TutorialPage4)
		{
			gameObject.renderer.material = TutorialPage4;
		}
		else if(currentPageNumber == 5 && gameObject.renderer.material != TutorialPage5)
		{
			gameObject.renderer.material = TutorialPage5;
		}
		
	}
}
