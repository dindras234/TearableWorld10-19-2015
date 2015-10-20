/*
 * 
 * This script is used to move object from left to from from camera perpsective
 * this is used to tigger triangles being visible vs not visible
 * 
 */
using UnityEngine;
using System.Collections;

public class VisibleCue : MonoBehaviour 
{
	
	public float Speed = 0.08f;
	
	// Use this for initialization
	private void Start () 
	{
	
	}
	
	// Update is called once per frame
	private void Update () 
	{
		if(!GameObject.FindGameObjectWithTag("AnimationManager").GetComponent<IntroSceneAnimationManager>().AnimationDone)
		{
			transform.position = new Vector3(transform.position.x - Speed, transform.position.y, transform.position.z);
		}
	}
}
