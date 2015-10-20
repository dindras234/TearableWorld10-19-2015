/// <summary>
/// 
/// FILE: 
/// 	Tear Cover Up
/// 
/// DESCRIPTION: 
/// 	This file is used to mask collision with platforms when
/// 	player has finalized rotation/translation of
/// 	torn piece after successful tear
/// 
/// AUTHOR: 
/// 	John Crocker - jrcrocke@ucsc.edu
/// 
/// DATE: 
/// 	5/11/2013 - ...
/// 
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


public class TearCoverUp : MonoBehaviour 
{
	
	/// <summary>
	/// The previous platform states of scene
	/// </summary>
	public Dictionary<GameObject, int[]> PreviousPlatformStates;
	
	/// <summary>
	/// The potential objects to mask after player moves torn piece
	/// after successful tear
	/// </summary>
	private List<GameObject> potentialObjectsToMask;

	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start () 
	{
		//Initialize storge for return to previous state
		PreviousPlatformStates = new Dictionary<GameObject, int[]>();
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update ()
	{
		if(!GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().PlayerMovingPlatformState &&
			!GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TornPieceCurrentlyMaskingCollision &&
			(PreviousPlatformStates.Count() > 0 || fullyMaskedObjects.Count() > 0))
		{
			GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TornPieceCurrentlyMaskingCollision = true;
		}
		else if((!GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().PlayerMovingPlatformState &&
			GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TornPieceCurrentlyMaskingCollision &&
			PreviousPlatformStates.Count() == 0 && 
			fullyMaskedObjects.Count() == 0)
			||
			(GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().PlayerMovingPlatformState&&
			GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TornPieceCurrentlyMaskingCollision))
		{
			GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>().TornPieceCurrentlyMaskingCollision = false;
		}
	}
	
	/// <summary>
	/// Checks to mask platforms after player has finalized movement of torn
	/// piece
	/// </summary>
	public void CheckToMaskPlatforms(GameObject tornWorldPiece){
		if(tornWorldPiece == null){ return; }
		//Debug.Log("CheckToMaskPlatforms");// with depth.parent = "  + tornWorldPiece.transform.parent.transform.position.z + " and depth = " + tornWorldPiece.transform.position.z );
		
		//Check to init
		if(potentialObjectsToMask == null){
			potentialObjectsToMask = new List<GameObject>();
		}
		
		if(potentialObjectsToMask.Count() > 0){
			potentialObjectsToMask.Clear();
		}
		if(PreviousPlatformStates == null){
			PreviousPlatformStates = new Dictionary<GameObject, int[]>();
		}
		if(PreviousPlatformStates.Count() > 0){
			PreviousPlatformStates.Clear();
		}
		
		//make sure we reset the container in case the player
		//has already set down the piece of paper once
		//if(potentialObjectsToMask.Count() > 0)
		//{
		//	potentialObjectsToMask.Clear();
		//}
		
		//Find all Potential objects in scene that need to be masked
		//when the player sets down a torn piece after successfull tear
		//or re-tranaslation of pre-existing form piece
		GameObject[] tearPlatforms = GameObject.FindGameObjectsWithTag("Platform");
		GameObject[] backsidePlatforms = GameObject.FindGameObjectsWithTag("FoldPlatform");

		
		//We check if the objects are within the tornWorldPiece mesh bounds
		//to then see if they need to be masked or not
		for(int itor = 0; itor < tearPlatforms.Length; itor++){
			if(tearPlatforms[itor].transform.parent != null 
				&& tearPlatforms[itor].GetComponent<MeshCollider>()){
				//UnityEngine.Debug.Log("testing tearable platform");
				//ensure we are not perform this masking logic for platforms ON torn piece
				if(tearPlatforms[itor].transform.parent != tornWorldPiece.transform
					&& !GetComponent<TearManager>().objectsBelongingToCutPiece.Contains(tearPlatforms[itor])
					&& tearPlatforms[itor].GetComponent<MeshCollider>().enabled
					&& tearPlatforms[itor].GetComponent<MeshFilter>().mesh.triangles.Length != 0){
					//UnityEngine.Debug.Log("tearable platform ADDED to potentialObjectsToMask");
					//CheckIfInBounds(tearPlatforms[itor], tornWorldPiece);
					potentialObjectsToMask.Add(tearPlatforms[itor]);
				}
			}
		}
		
		for(int itor = 0; itor < backsidePlatforms.Length; itor++){
			//ensure we are not perform this masking logic for platforms ON torn piece
			if(backsidePlatforms[itor].transform.parent != tornWorldPiece.transform
					&& !GetComponent<TearManager>().objectsBelongingToCutPiece.Contains(backsidePlatforms[itor])
					&& backsidePlatforms[itor].GetComponent<MeshCollider>().enabled
					&& backsidePlatforms[itor].GetComponent<MeshFilter>().mesh.triangles.Length != 0){
				//CheckIfInBounds(tearPlatforms[itor], tornWorldPiece);
				potentialObjectsToMask.Add(backsidePlatforms[itor]);
			}
		}
		//for(int itor = 0; itor < backsidePlatforms.Length; itor++)
		//{
		//	CheckIfInBoundsBackSide(backsidePlatforms[itor], tornWorldPiece);
		//}
		
		//Debug.LogError("Testing potentialObjectsToMask with count = " + potentialObjectsToMask.Count().ToString());
		// Now we need to iterate through mesh
		// faces of each object contained in potentialObjectsToMask
		// to determine if any masking is needed, at this point,
		// we know that the objects within potentialObjectsToMask are
		// close enough to potentially needing to be masked
		foreach(GameObject go in potentialObjectsToMask){
			go.GetComponent<MeshCollider>().enabled = false;
			//SetFoldBoarderCollision(false, "foldborder", 0);
			//SetFoldBoarderCollision(false, "tearablePlatformTrigger", 1);
			
			
			//Debug.LogError("Testing potentialObjectsToMask");
			MaskPlatformObject(go, tornWorldPiece);
			
			
			//go.GetComponent<MeshCollider>().enabled = true;
			//SetFoldBoarderCollision(true, "foldborder", 0);
			//SetFoldBoarderCollision(true, "tearablePlatformTrigger", 1);
		}
	}
	
	/// <summary>
	/// Sets the fold boarder collision enabled for all objects
	/// </summary>
	private void SetFoldBoarderCollision(bool enabled, string objects, int colliderType)
	{
		GameObject[] foldBorder = GameObject.FindGameObjectsWithTag(objects);
		
		for(int itor = 0; itor < foldBorder.Length; itor++)
		{
			if(colliderType == 0)
				foldBorder[itor].GetComponent<MeshCollider>().enabled = enabled;
			else
				foldBorder[itor].GetComponent<BoxCollider>().enabled = enabled;
		}
	}
	
	/// <summary>
	/// Returns the scene to the state of the previous platform (before collision
	/// was modified by movement of torn piece)
	/// </summary>
	public void ReturnToPreviousPlatformState()
	{
		//Debug.Log("ReturnToPreviousPlatformState");
		
		
		if(PreviousPlatformStates.Count() > 0)
		{
			foreach(GameObject go in PreviousPlatformStates.Keys)
			{	
				if(!go.GetComponent<MeshCollider>())go.AddComponent<MeshCollider>();
			}
			
			//Reset dictionary storage for next iteration
			PreviousPlatformStates.Clear ();
		}
		
		
		/*
		if(PreviousPlatformStates.Count() > 0)
		{
			foreach(GameObject go in PreviousPlatformStates.Keys)
			{
				//if(go != GetComponent<TearManager>().MainStartingWorldPaper 
				//	&& go != GetComponent<TearManager>().MainWorldCutPaper 
				//	&& go != GetComponent<TearManager>().MainWorldPaper )
				{
					//UnityEngine.Debug.Log("ReturnToPreviousPlatformState + " + go.name);
					go.GetComponent<MeshFilter>().mesh.triangles = PreviousPlatformStates[go];
					
					go.GetComponent<MeshFilter>().mesh.RecalculateBounds();
					go.GetComponent<MeshFilter>().mesh.RecalculateNormals();
					
					go.GetComponent<MeshCollider>().sharedMesh = go.GetComponent<MeshFilter>().mesh;
					
				}
			}
			
			//Reset dictionary storage for next iteration
			PreviousPlatformStates.Clear ();
		}
		*/
		
		
		/*
		if(fullyMaskedObjects.Count() > 0)
		{
			foreach(GameObject go in fullyMaskedObjects)
			{
				go.GetComponent<MeshRenderer>().enabled = true;
				go.GetComponent<MeshCollider>().enabled = true;
			}
		}
		fullyMaskedObjects.Clear();
		*/
	}
	
	/// <summary>
	/// The fully masked objects during a fold
	/// </summary>
	private List<GameObject> fullyMaskedObjects = new List<GameObject>();
	
	/// <summary>
	/// Checks if go is in the tornWorldPiece mesh collider bounds
	/// </summary>
	private void CheckIfInBounds(GameObject go, GameObject tornWorldPiece)
	{
		bool testDist = false;
		
		for(int itor = 0; itor < tornWorldPiece.GetComponent<MeshFilter>().mesh.triangles.Count(); itor++)
		{
			Vector2 dist1 = new Vector2(tornWorldPiece.transform.TransformPoint(tornWorldPiece.GetComponent<MeshFilter>().mesh.vertices[tornWorldPiece.GetComponent<MeshFilter>().mesh.triangles[itor]]).x, 
										tornWorldPiece.transform.TransformPoint(tornWorldPiece.GetComponent<MeshFilter>().mesh.vertices[tornWorldPiece.GetComponent<MeshFilter>().mesh.triangles[itor]]).y);
			Vector2 dist2 = new Vector2(go.transform.position.x, go.transform.position.y);
			float currDist = Vector3.Distance(dist1, dist2);
			if(currDist < 0) currDist *= -1;
			if(currDist <= tornWorldPiece.GetComponent<TearPaper>().MESH_VERT_OFFSET)
			{
				potentialObjectsToMask.Add(go);
				return;
			}
			
		}
	}
	
	/// <summary>
	/// Checks if go is in the tornWorldPiece mesh collider bounds
	/// </summary>
	private void CheckIfInBoundsBackSide(GameObject go, GameObject tornWorldPiece)
	{
		bool testDist = false;
		
		for(int itor = 0; itor < tornWorldPiece.GetComponent<MeshFilter>().mesh.vertices.Count(); itor++)
		{
			if(tornWorldPiece.GetComponent<MeshFilter>().mesh.triangles.Contains(itor))
			{
				
				Vector2 dist1 = new Vector2(tornWorldPiece.transform.TransformPoint(tornWorldPiece.GetComponent<MeshFilter>().mesh.vertices[itor]).x, 
											tornWorldPiece.transform.TransformPoint(tornWorldPiece.GetComponent<MeshFilter>().mesh.vertices[itor]).y);
				Vector2 dist2 = new Vector2(go.transform.position.x, go.transform.position.y);
				float currDist = Vector3.Distance(dist1, dist2);
				if(currDist < 0) currDist *= -1;
				
				Vector3 worldSixe = go.transform.TransformPoint(go.GetComponent<MeshCollider>().bounds.extents);
				float length = worldSixe.x;
				
				if(worldSixe.y > length)
				{
					length = worldSixe.y;
				}
				
				if(currDist <= length)
				{
					potentialObjectsToMask.Add(go);
					UnityEngine.Debug.LogError("Testing CheckIfInBoundsBackSide");
					return;
				}
				
			}
		}
	}
	
	/// <summary>
	/// potentialObjectsToMask call this function to mask the platform object
	/// that potentially need masking based off their porximity to settled
	/// torn piece
	/// </summary>
	private void MaskPlatformObject(GameObject go, GameObject tornWorldPiece)
	{	
		float zDepth = go.transform.position.z - 10.0f;
		
		//for(int itor= 0; itor + 2 < go.GetComponent<MeshFilter>().sharedMesh.vertices.Length; itor++)
		{
			RaycastHit hit;
			Vector3 rayPos = go.collider.bounds.center;
			rayPos.z = zDepth;
			//gameObject.collider.
			if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 25) 
				&& hit.collider.gameObject.name == "paper_CuttPieceOfPaper"
				)
			{
				//UnityEngine.Debug.Log("enabled = true");
				//go.GetComponent<MeshCollider>().enabled = false;
				Destroy(go.GetComponent<MeshCollider>());
				PreviousPlatformStates.Add(go, new int[0]);
				return;
			}
			else
			{
				//UnityEngine.Debug.Log("enabled = false");
			}
		}
		go.GetComponent<MeshCollider>().enabled = true;
		return;
		
		/*
		List<int> goodIndecies = new List<int>();
		float zDepth = go.transform.position.z - 5.0f;
		//go.GetComponent<MeshCollider>().enabled = false;
		
		for(int itor= 0; itor + 2 < go.GetComponent<MeshFilter>().sharedMesh.triangles.Length; itor += 3)
		{
			int numVertsInTriangle = 0;
			RaycastHit hit;
			Vector3 rayPos = go.transform.TransformPoint(go.GetComponent<MeshFilter>().mesh.vertices[go.GetComponent<MeshFilter>().mesh.triangles[itor]]);
			rayPos.z = zDepth;
	
			if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
				&& hit.collider.gameObject.name == "paper_CuttPieceOfPaper"
				)
			{
				++numVertsInTriangle;
			}
			rayPos = go.transform.TransformPoint(go.GetComponent<MeshFilter>().mesh.vertices[go.GetComponent<MeshFilter>().mesh.triangles[itor + 1]]);
			rayPos.z = zDepth;
	
			if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
				&& hit.collider.gameObject.name == "paper_CuttPieceOfPaper")
			{
				++numVertsInTriangle;
			}
			rayPos = go.transform.TransformPoint(go.GetComponent<MeshFilter>().mesh.vertices[go.GetComponent<MeshFilter>().mesh.triangles[itor + 2]]);
			rayPos.z = zDepth;

			if(Physics.Raycast(rayPos, Camera.main.transform.forward, out hit, 20) 
				&& hit.collider.gameObject.name == "paper_CuttPieceOfPaper"
				)
			{
				++numVertsInTriangle;
			}
			
			//If more than one vert in the tirangle face have been covered, we mask collision
			if(numVertsInTriangle < 1)
			{
				goodIndecies.Add(go.GetComponent<MeshFilter>().mesh.triangles[itor]);
				goodIndecies.Add(go.GetComponent<MeshFilter>().mesh.triangles[itor + 1]);
				goodIndecies.Add(go.GetComponent<MeshFilter>().mesh.triangles[itor + 2]);
			}
		}
		
		int[] newTriagles = new int[0];
		if(goodIndecies.Count() != 0)// && goodIndecies.Count() != go.GetComponent<MeshFilter>().mesh.triangles.Length)
		{
			newTriagles = new int[goodIndecies.Count()];
			//UnityEngine.Debug.Log("JnewTriagles " + goodIndecies.Count().ToString());
			for(int jtor = 0; jtor < newTriagles.Length; jtor++)
			{
				newTriagles[jtor] = goodIndecies.ElementAt(jtor);
			}
		}
		
		
		
		int[] old = go.GetComponent<MeshFilter>().mesh.triangles;
		
		
		
		//UnityEngine.Debug.Log("Just about to add " + go.name + " to the list of PreviousPlatforms with Triangle.Length = " + old.Length.ToString());
		
		
		//go.GetComponent<MeshCollider>().enabled = true;
		
		//Store previous state
		PreviousPlatformStates.Add(go, old);
		
		//Assign new faces
		go.GetComponent<MeshFilter>().mesh.triangles = newTriagles;
		go.GetComponent<MeshCollider>().sharedMesh = go.GetComponent<MeshFilter>().mesh;
		*/
		
	
		
		/*
		//check to see if any faces need masking
		if(goodIndecies.Count() != 0)// && goodIndecies.Count() != go.GetComponent<MeshFilter>().mesh.triangles.Length)
		{
			int[] newTriagles = new int[goodIndecies.Count()];
			
			for(int jtor = 0; jtor < newTriagles.Length; jtor++)
			{
				newTriagles[jtor] = goodIndecies.ElementAt(jtor);
			}
			
			//Store previous state
			PreviousPlatformStates.Add(go, go.GetComponent<MeshFilter>().mesh.triangles);
			
			//Assign new faces
			go.GetComponent<MeshFilter>().mesh.triangles = newTriagles;
			go.GetComponent<MeshCollider>().sharedMesh = go.GetComponent<MeshFilter>().mesh;
			
		}
		else 
		{
			UnityEngine.Debug.Log("Testing fully masked");
			
			PreviousPlatformStates.Add(go, go.GetComponent<MeshFilter>().mesh.triangles);
			
			//Assign new faces
			go.GetComponent<MeshFilter>().mesh.triangles = new int[0];
			go.GetComponent<MeshCollider>().sharedMesh = go.GetComponent<MeshFilter>().mesh;
			
			
			//go.GetComponent<MeshRenderer>().enabled = false;
			//go.GetComponent<MeshCollider>().enabled = false;
			//fullyMaskedObjects.Add(go);
		}
		*/
		
	}
}
