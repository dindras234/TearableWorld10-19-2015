/// <summary>
/// Platform depth script used as an attachment to every platform object, ensure it never leaves
/// depth 0 which the player resides on
/// </summary>
using UnityEngine;
using System.Collections;

public class PlatformDepth : MonoBehaviour 
{
	/// <summary>
	/// The tear manager local ref
	/// </summary>
	private TearManager tearManager;
	
	/// <summary>
	/// The zero offset to prevent update when unity 
	/// adds random float to component of position
	/// </summary>
	private float zeroOffset = 0.001f;
	
	/*
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start () 
	{
		tearManager = GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>();

	}
	*/
	
	/*
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update () 
	{
		//FOR NOW NOT IN USE ->J.C.
		return;
		
		if(tearManager.PlayerMovingPlatformState 
			&& transform.TransformPoint(transform.position).z != tearManager.MainWorldCutPaper.transform.position.z - 0.1f 
			//&& !WithinRange(transform.TransformPoint(transform.position).z, tearManager.MainWorldCutPaper.transform.position.z - 0.1f, zeroOffset)
			&& tearManager.objectsBelongingToCutPiece.Contains(gameObject))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, tearManager.MainWorldCutPaper.transform.position.z - 0.1f);
		}
		//Force z-depth to be 0 for platforms
		else if((
			transform.TransformPoint(transform.position).z != 0
			//!WithinRange(transform.TransformPoint(transform.position).z, 0, zeroOffset)
			&& !tearManager.PlayerMovingPlatformState) 
			||
			(
			transform.TransformPoint(transform.position).z != 0
			//!WithinRange(transform.TransformPoint(transform.position).z, 0, zeroOffset)
			
			&& tearManager.PlayerMovingPlatformState
			&& !tearManager.objectsBelongingToCutPiece.Contains(gameObject)))
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
		}
	
		
		if(
			gameObject.transform.rotation.x != 0 
			|| 
			gameObject.transform.rotation.y != 0)
		{
			gameObject.transform.rotation = new Quaternion(0,0,gameObject.transform.rotation.z, gameObject.transform.rotation.w);
		}
	}
	*/
	
	/// <summary>
	/// Withins the range.
	/// </summary>
	/// <returns>
	private bool WithinRange(float component, float goal, float range)
	{
		if(component < goal + range && component > goal - range) return true;
		else return false;
	}
	
}
