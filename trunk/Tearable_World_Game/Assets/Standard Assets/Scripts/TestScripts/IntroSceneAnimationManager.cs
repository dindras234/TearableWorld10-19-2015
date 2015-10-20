using UnityEngine;
using System.Collections;

public class IntroSceneAnimationManager : MonoBehaviour {
	
	public GameObject Hand;
	
	public GameObject Flag;
	
	public bool AnimationDone = false;
	
	// Use this for initialization
	private void Start ()
	{
		AnimationDone = false;
	}
	
	// Update is called once per frame
	private void Update () 
	{
		//if user presses r, restart scene
		if(Input.GetKeyUp("r"))
		{
			Application.LoadLevel(Application.loadedLevel);
		}
		
		if(Hand.transform.position.x < Flag.transform.position.x)
		{
			AnimationDone = true;
		}
	}
}
