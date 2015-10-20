using UnityEngine;
using System.Collections;

/// <summary>
/// This class must be attached to the player
/// since it is the only thing consistent from
/// scene to scene
/// </summary>
public class SceneTransistion : MonoBehaviour 
{
    GameStateManager gameStateManagerRef;
    GameObject levelCompleteText;
    GameObject swipeToContinueText;

	// Use this for initialization
	void Start () 
    {
        gameStateManagerRef = GameObject.FindGameObjectWithTag("MainObject").GetComponent<GameStateManager>();

        GameObject levelCompleteGUI = GameObject.FindGameObjectWithTag("EndLevelObject");
        GameObject blackFadeElement = GameObject.FindGameObjectWithTag("BlackFade");
        levelCompleteText = GameObject.FindGameObjectWithTag("LevelCompleteText");
        swipeToContinueText = GameObject.FindGameObjectWithTag("SwipeToContinueText");

        if (levelCompleteGUI)
        {
            levelCompleteGUI.GetComponent<GUITexture>().enabled = false;
            levelCompleteGUI.GetComponent<Camera>().enabled = false;
            levelCompleteGUI.GetComponent<GUILayer>().enabled = false;
        }

        if (blackFadeElement)
        {
            blackFadeElement.GetComponent<GUITexture>().enabled = false;
            blackFadeElement.GetComponent<Camera>().enabled = false;
            blackFadeElement.GetComponent<GUILayer>().enabled = false;
        }

        if (levelCompleteText)
        {
            levelCompleteText.GetComponent<GUIText>().enabled = false;
        }

        if (swipeToContinueText)
        {
            swipeToContinueText.GetComponent<GUIText>().enabled = false;
        }
        
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (GameObject.FindGameObjectsWithTag("MainObject").Length > 1)
        { 
            GameObject[] list = GameObject.FindGameObjectsWithTag("MainObject");
            for (int i = 0; i < list.Length; ++i)
            {
                if (!list[i].GetComponent<GameStateManager>().objectSaved)
                    GameObject.Destroy(list[i]);

                else
                    gameStateManagerRef = list[i].GetComponent<GameStateManager>();
            }
        }

        if (gameStateManagerRef.GetScreenManager().GetCurrentScreenArea() != ScreenAreas.LevelComplete)
        {
            if (levelCompleteText)
            {
                levelCompleteText.GetComponent<GUIText>().enabled = false;
            }

            if (swipeToContinueText)
            {
                swipeToContinueText.GetComponent<GUIText>().enabled = false;
            }
        }
	}
}
