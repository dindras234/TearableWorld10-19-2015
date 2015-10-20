using UnityEngine;
using System.Collections;

public class PlayMovie : MonoBehaviour {

	public float MovieTimeLength;
	
	private float movieTimer;
	
	private GameStateManager localGameStateManagerRef;
	
	private GameObject MainObject;
	
	public bool IntroScene;
	
	public bool CreditsScene;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start () 
	{
		
	//	else
	//	{
	//		Destroy(Camera.main.GetComponent<AudioSource>());

			//if(IntroScene) Handheld.PlayFullScreenMovie ("Intro_Animation.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
			//if(CreditsScene) Handheld.PlayFullScreenMovie ("CreditsScreen_Animation_1.4.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);

					//if(IntroScene) Handheld.PlayFullScreenMovie ("Intro_Animation.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
			//if(CreditsScene) Handheld.PlayFullScreenMovie ("CreditsScreen_Animation_1.4.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);

		//if(GameObject.FindGameObjectWithTag("MainObject") != null 
		//	&& !GameObject.FindGameObjectWithTag("MainObject").GetComponent<GameStateManager>().OnMobileDevice())
		{
		//	renderer.material.mainTexture = movie;	
		//	movie.Play ();
		}
		//if(GameObject.FindGameObjectWithTag("MainObject").GetComponent<GameStateManager>().OnMobileDevice())
		
		
		//if(GameObject.FindGameObjectWithTag("MainObject").GetComponent<GameStateManager>().OnMobileDevice())
		//	StartCoroutine(CoroutinePlayMovie());
		
		if(GameObject.FindGameObjectWithTag("MainObject").GetComponent<GameStateManager>().OnMobileDevice())
		{
			if(IntroScene)
			{
				string dpath = "file://" + Application.streamingAssetsPath + "/Intro_Animation.mp4";
				//UnityEngine.Debug.Log("CoroutinePlayMovie path = " + dpath);
				//iPhoneUtils.PlayMovie(dpath, Color.black, iPhoneMovieControlMode.CancelOnTouch);//, Color.black, FullScreenMovieControlMode.CancelOnInput);
				
				
				//iPhoneUtils.PlayMovie("Intro_Animation.mp4",Color.black, iPhoneMovieControlMode.CancelOnTouch, iPhoneMovieScalingMode.AspectFit);
			//	Handheld.PlayFullScreenMovie(dpath, Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
	
			}
			if(CreditsScene)
			{
				string dpath = "file://" + Application.streamingAssetsPath +  "/CreditsScreen_Animation_1.4.mp4";
				//UnityEngine.Debug.Log("CoroutinePlayMovie path = " + dpath);
				//iPhoneUtils.PlayMovie(dpath, Color.black, iPhoneMovieControlMode.CancelOnTouch);//, Color.black, FullScreenMovieControlMode.CancelOnInput);
				
				
				//iPhoneUtils.PlayMovie("CreditsScreen_Animation_1.4.mp4",Color.black, iPhoneMovieControlMode.CancelOnTouch, iPhoneMovieScalingMode.AspectFit);
				
			//	Handheld.PlayFullScreenMovie(dpath, Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
			}
		}
	//	}
	}
	bool checker = false;
	
	
	/// <summary>
	/// Coroutines the play movie.
	/// </summary>
	/// <returns>
	/// The play movie.
	/// </returns>
	protected IEnumerator CoroutinePlayMovie() 
	{ 
		
		
		UnityEngine.Debug.Log("CoroutinePlayMovie");
		if(IntroScene)
			{
				string dpath = "file://" + Application.streamingAssetsPath + "/Intro_Animation.mp4";
				//UnityEngine.Debug.Log("CoroutinePlayMovie path = " + dpath);
				//iPhoneUtils.PlayMovie(dpath, Color.black, iPhoneMovieControlMode.CancelOnTouch);//, Color.black, FullScreenMovieControlMode.CancelOnInput);
				
				
				//iPhoneUtils.PlayMovie("Intro_Animation.mp4",Color.black, iPhoneMovieControlMode.CancelOnTouch, iPhoneMovieScalingMode.AspectFit);
				
			//	Handheld.PlayFullScreenMovie(dpath, Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
	
			}
			if(CreditsScene)
			{
				string dpath = "file://" + Application.streamingAssetsPath +  "/CreditsScreen_Animation_1.4.mp4";
				//UnityEngine.Debug.Log("CoroutinePlayMovie path = " + dpath);
				//iPhoneUtils.PlayMovie(dpath, Color.black, iPhoneMovieControlMode.CancelOnTouch);//, Color.black, FullScreenMovieControlMode.CancelOnInput);
				
				
				//iPhoneUtils.PlayMovie("CreditsScreen_Animation_1.4.mp4",Color.black, iPhoneMovieControlMode.CancelOnTouch, iPhoneMovieScalingMode.AspectFit);
				
			//	Handheld.PlayFullScreenMovie(dpath, Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
			}
	
		yield return new WaitForSeconds(MovieTimeLength); //Allow time for Unity to pause execution while the movie plays. 
	
		//GameObject.FindGameObjectWithTag("MainObject").GetComponent<GameStateManager>().ForceOnlyOneIntro = true;
		localGameStateManagerRef.EnterMainMenu();
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update () 
	{
		
		if(GameObject.FindGameObjectWithTag("MainObject") != null )
		{
			movieTimer += Time.deltaTime;
		}
		
		if(movieTimer >= MovieTimeLength
			&& GameObject.FindGameObjectWithTag("MainObject") !=  null)
		{
			UnityEngine.Debug.Log(" currently movieTimer is UP");
			checker = true;
			MainObject = GameObject.FindGameObjectWithTag("MainObject");	
			MainObject.GetComponent<AudioSource>().enabled = true;
			localGameStateManagerRef = MainObject.GetComponent<GameStateManager>();
			localGameStateManagerRef.EnterMainMenu();
			GameObject.FindGameObjectWithTag("MainObject").GetComponent<GameStateManager>().ForceOnlyOneIntro = true;
			
		}
		else
		{
			try
			{
				MainObject = GameObject.FindGameObjectWithTag("MainObject");
				
				MainObject.GetComponent<AudioSource>().enabled = false;
				
				localGameStateManagerRef = MainObject.GetComponent<GameStateManager>();
			}
			catch
			{
				
			}
		}
		
		
		
		
	}
}
