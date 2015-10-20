using UnityEngine;
using System.Collections;

/// <summary>
/// Script attached to player object
/// for collecting game objects in 
/// each of the levels.
/// </summary>
public class collectable : MonoBehaviour 
{
	public int worth;
    private GameStateManager gameStateManagerRef;

    public void Start()
    {
        //Grab mainObject prefab to access managers effectively
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

        gameStateManagerRef = mainObject.GetComponent<GameStateManager>();

    }
	
	void OnTriggerEnter (Collider other)
    {

        if (other.gameObject.tag.Equals("EndGoal"))
        {
            gameStateManagerRef.GetScreenManager().DisplayScreen(ScreenAreas.LevelComplete);
        }

        // Commenting this out for now, for being a 
        // a global script, "Collectable" tag should be
        // the tag for all collectable objects other
        // than the end goal, which I'm re-tagging
        // as "EndGoal".  Uncomment this when adding
        // new collectable items in the game that will
        // all have the same behavior.
        //if(other.gameObject.tag ==  "Collectable")
        //{

        //    if (!other.name.Equals("LevelGoal"))
        //    {
        //        Destroy(other.gameObject);
        //    }
        //    else
        //    {
        //        // Transition to next level

        //        GameStateManager.gameStateManagerRef.EnterGameState(Application.loadedLevel + 1);
        //    }
        //}
	}
}