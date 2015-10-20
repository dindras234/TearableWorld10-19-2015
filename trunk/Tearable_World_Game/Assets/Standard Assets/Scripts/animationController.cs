using UnityEngine;
using System.Collections;

/// <summary>
/// animationController.cs
/// 
/// Written by Justin Telmo. Largely influenced from a Youtube tutorial.
/// </summary>
public class animationController : MonoBehaviour 
{
	
	#region Fields
	public float FPS;
	private float timeToWait;
	public Texture[] frames;
	public bool loop;
	private int currFrame;
	#endregion
	
	#region Methods
	// Use this for initialization
	void Start () 
	{
		currFrame = 0;
		timeToWait = 1/FPS;
		StartCoroutine (Animate ());
	}
	
	/// <summary>
	/// Animate this instance.
	/// </summary>
	IEnumerator Animate()
	{
		bool stop = false;
		
		if (currFrame >= frames.Length)
		{
			if(!loop)
			{
				stop = true;
			}
			else currFrame = 0;
		}
		yield return new WaitForSeconds(timeToWait);
		
		renderer.material.mainTexture = frames[currFrame];
		currFrame++;
		
		if(!stop)
		{
			StartCoroutine (Animate ());
		}
	}
	#endregion
}
