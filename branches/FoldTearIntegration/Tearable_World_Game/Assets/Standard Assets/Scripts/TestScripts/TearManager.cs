/// <summary>
/// 
/// FILE: 
/// 	Tear Mechanic Manager
/// 
/// DESCRIPTION: 
/// 	This file is used for managing mesh filters and collision during a tear
/// 
/// AUTHOR: 
/// 	John Crocker - jrcrocke@ucsc.edu
/// 
/// DATE: 
/// 	4/12/2013 - ...
/// 
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class TearManager : MonoBehaviour {
	
	/// <summary>
	/// The rotation audio for when the player is rotationg a torn piece.
	/// </summary>
	public AudioClip RotationAudio;
	
	/// <summary>
	/// The platforms array stores all tearalbe platforms
	/// </summary>
	public List<GameObject> Platforms;
	
	/// <summary>
	/// The torn platforms represents the 
	/// </summary>
	public List<GameObject> TornPlatforms;
	
	/// <summary>
	/// The main starting world paper used to check UI bounds for
	/// creating tearing 'effect'
	/// </summary>
	public GameObject MainStartingWorldPaper;
	
	/// <summary>
	/// The decal object for tearing visual effect
	/// </summary>
	public GameObject DecalObject;
	
	/// <summary>
	/// The tear finished flags the moment a tear is done, triggering
	/// parenting so that translations and roations of torn 
	/// platforms correlated to the world peice they ly on top of
	/// </summary>
	public bool TearFinished = false;
	
	/// <summary>
	/// The main world cut paper.
	/// </summary>
	public GameObject MainWorldCutPaper;
	
	/// <summary>
	/// The main world cut paper.
	/// </summary>
	public Vector3 MainWorldCutPaperCenterPos;
	
	/// <summary>
	/// The main world paper (non cut portion after user tear)
	/// </summary>
	public GameObject MainWorldPaper;
	
	/// <summary>
	/// The decal life.
	/// </summary>
	public float DecalLife = 20f;
	
	/// <summary>
	/// The decal shrink speed.
	/// </summary>
	public float DecalShrinkSpeed = 0.002f;
	
	/// <summary>
	/// The bad tear flag thown during bad input
	/// </summary>
	public bool BadTear = false;
	
	/// <summary>
	/// The rotation speed of a torn piece
	/// </summary>
	public int RotationSpeed = 2;
	
	/// <summary>
	/// The center positions of every object n eeding rotation
	/// </summary>
	private Dictionary<GameObject, Vector3> centerPositions;
	
	/// <summary>
	/// The dist check from each tearable object's center to the torn world object,
	/// used for correctly assigning parent child relationships
	/// </summary>
	private Dictionary<GameObject, int> distCheck;
	
	/// <summary>
	/// The objects belonging to cut piece (platforms being parented
	/// for one to one translations and rotations
	/// </summary>
	public List<GameObject> objectsBelongingToCutPiece;
	
	/// <summary>
	/// This flags when parent child relations need to be set
	/// </summary>
	private bool needToSetParentChildRelations = false;
	
	/// <summary>
	/// The player completed tear falgs the moment they can start rotating and 
	/// translating the torn piece
	/// </summary>
	private bool playerCompletedTear = false;
	
	/// <summary>
	/// The player moving platform flag represents the state when the
	/// player is moving and rotating the torn piece
	/// </summary>
	public bool PlayerMovingPlatformState = false;
	
	/// <summary>
	/// The main cut piece rotation object used for rotating about user input correctly
	/// </summary>
	private GameObject MainCutPieceRotationObject;
	
	/// <summary>
	/// The decal objects stores all existing decal objects
	/// </summary>
	private Dictionary<GameObject, int> decalObjects;
	
	/// <summary>
	/// The tear piece colliding with player flag to prevent user movement
	/// </summary>
	private bool TearPieceCollidingWithPlayer = false;
	
	/// <summary>
	/// The previous player input position, used to stop the torn
	/// piece from covering player
	/// </summary>
	private Vector3 prevPlayerInputPos;
	
	/// <summary>
	/// The have torn once.
	/// </summary>
	private bool haveTornOnce = false;
	
	public GameObject FoldObject;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start () 
	{
		//Initialize the tornPlatform list which will be modified
		//during runtime when player triggeres tearing mechanic
		TornPlatforms = new List<GameObject>();
		
		//Initialize the centerPosition dictionary
		centerPositions = new Dictionary<GameObject, Vector3>();
		
		//Initialize the list storing the tearable objects which need
		//to be parented to the world cut piece
		objectsBelongingToCutPiece = new List<GameObject>();
		
		//Initialize decalObject dictionary storeing every decal object for proper updating and cleanup
		decalObjects = new Dictionary<GameObject, int>();
	}
	
	/// <summary>
	/// The currently tearing flag to draw decal objects for visual tearing effect
	/// </summary>
	public bool PlayerCurrentlyTearing = false;
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update () 
	{
		
		if(Input.GetMouseButtonUp(0) && httingFoldedArea)
		{
			MainStartingWorldPaper.GetComponent<TearPaper>().ForceStopTear();
			needToSetParentChildRelations = false;
			playerCompletedTear = false;
			PlayerCurrentlyTearing = false;
			centerPositions = new Dictionary<GameObject, Vector3>();
			//decalObjects = new Dictionary<GameObject, int>();
			//BadTear = false;
			//badTearTimer = 0;
			httingFoldedArea = false;
			TearFinished = false;
			haveTornOnce = false;
		}
		
		//Check, and only perform once
		if(TearFinished && !haveTornOnce)
		{
			GroupTornObjectsFindCenter();
		}
		
		//Wait for the flag to trigger parent child relation establishment
		if(needToSetParentChildRelations)
		{
			ParentObjectsToWorldCutPiece();
			
			DisableTearScripts();
		}
		
		//Translate torn objects with user input
		if(playerCompletedTear)
		{
			TranslateRotateTornObjects();
		}
		else if(PlayerCurrentlyTearing)
		{
			//Draw decal mouse input effects
			CreateTearingEffect();
		}
		
		//Update decal Objects
		if(decalObjects.Keys.Count() > 0) 
		{
			UpdateDecalObjects();
		}
		else if(BadTear)
		{
			//If decalObjects is empty, then make sure to reset Badtear iff needed
			BadTear = false;
			badTearTimer = 0;
		}
	}
	
	/// <summary>
	/// Disables the tear script from each tearable object
	/// </summary>
	private void DisableTearScripts()
	{
		foreach(GameObject go in Platforms)
		{
			go.GetComponent<TearPaper>().enabled = false;	
		}
		MainWorldCutPaper.GetComponent<TearPaper>().enabled = false;
		MainWorldPaper.GetComponent<TearPaper>().enabled = false;
	}
	
	/// <summary>
	/// Translates and rotates the torn objects.
	/// </summary>
	private void TranslateRotateTornObjects()
	{
		Vector3 newPosForTear = Vector3.zero;
		if(PlayerMovingPlatformState && PlayerInputInBounds())
		{
			//Update the player position if they are not currently colliding with the player
			if(!TearPieceCollidingWithPlayer)
			{
				newPosForTear = Camera.main.ScreenToWorldPoint(new 
					Vector3(Input.mousePosition.x, Input.mousePosition.y, 
					Camera.main.WorldToScreenPoint(MainWorldCutPaper.transform.position).z));
				newPosForTear.z = -2;
				prevPlayerInputPos = MainCutPieceRotationObject.transform.position;
				MainCutPieceRotationObject.transform.position = newPosForTear;
			}
			
			//determine if the player is moving torn piece over player
			if(PlayerCollidingWithTornPieceCheck())
			{
				TearPieceCollidingWithPlayer = true;
				MainCutPieceRotationObject.transform.position = prevPlayerInputPos;
				if(Camera.main.audio.isPlaying)Camera.main.audio.Pause();
			}
			
			//Check for good input after recieving bad user input
			if(TearPieceCollidingWithPlayer)
			{
				newPosForTear = Camera.main.ScreenToWorldPoint(new 
					Vector3(Input.mousePosition.x, Input.mousePosition.y, 
					Camera.main.WorldToScreenPoint(MainWorldCutPaper.transform.position).z));
				newPosForTear.z = -2;
				prevPlayerInputPos = MainCutPieceRotationObject.transform.position;
				MainCutPieceRotationObject.transform.position = newPosForTear;
				//determine if the player is moving torn piece over player
				if(!PlayerCollidingWithTornPieceCheck())
				{
					TearPieceCollidingWithPlayer = false;
				}
				MainCutPieceRotationObject.transform.position = prevPlayerInputPos;
			}
		}
		
		
		//Transition between moving and not moving the torn piece of 
		//paper by switching playerMovingPlatform on and off
		if(Input.GetKeyUp("b"))
		{
			//Rotate between being true and not true
			if(PlayerMovingPlatformState)
			{
				MainCutPieceRotationObject.transform.position = new Vector3(MainCutPieceRotationObject.transform.position.x, 
					MainCutPieceRotationObject.transform.position.y, MainWorldPaper.transform.position.z - 0.001f);
				
				PlayerMovingPlatformState = false;
			}
			else
			{
				PlayerMovingPlatformState = true;
			}
			
			if(Camera.main.audio.isPlaying)Camera.main.audio.Pause();
			
			//This is called when the torn piece moves onto player because the user was moving torn
			//piece extremely fast
			if(PlayerCollidingWithTornPieceCheck())
			{
				while(PlayerCollidingWithTornPieceCheck())
				{
					MainCutPieceRotationObject.transform.position += new Vector3(MainCutPieceRotationObject.transform.position.x + 0.1f, 
						MainCutPieceRotationObject.transform.position.y + 0.1f, MainCutPieceRotationObject.transform.position.z);
				}
			}
		}
		
		if(PlayerMovingPlatformState && PlayerInputInBounds() && !TearPieceCollidingWithPlayer)
		{
			/*
			Vector3 newPosForTear = Camera.main.ScreenToWorldPoint(new 
				Vector3(Input.mousePosition.x, Input.mousePosition.y, 
				Camera.main.WorldToScreenPoint(MainWorldCutPaper.transform.position).z));
			newPosForTear.z = 2;
			MainCutPieceRotationObject.transform.position = newPosForTear;
			*/
			
			if(Input.GetMouseButton(1))
			{
				MainCutPieceRotationObject.transform.RotateAround(newPosForTear, new Vector3(0, 0, 1), RotationSpeed);
				
				if(Camera.main.audio.clip != RotationAudio)
				{
					Camera.main.audio.clip = RotationAudio;
				}
				
				//Play rotation audio
				if(!Camera.main.audio.isPlaying)Camera.main.audio.Play();
			}
			else
			{
				//Stop rotation audio
				if(Camera.main.audio.isPlaying)Camera.main.audio.Pause();
			}
			
		}
		else if(!PlayerInputInBounds() && Camera.main.audio.isPlaying)
		{
			Camera.main.audio.Pause();
		}
	}
	
	/// <summary>
	/// Parents the objects to world cut piece.
	/// </summary>
	private void ParentObjectsToWorldCutPiece()
	{
		foreach(GameObject go in objectsBelongingToCutPiece)
		{
			go.transform.parent = MainWorldCutPaper.transform;
		}
		
		
		/*
		//*********TESTING*********
		foreach(GameObject go in objectsBelongingToCutPiece)
		{
			go.transform.RotateAround(centerPositions[go], new Vector3(0, 0, 1), 1);
		}
		*/
		
		//Ensure this logic is only hit once
		needToSetParentChildRelations = false;
		
		//Flag playerCompletedTear so that they can move and rotate tonr piece
		playerCompletedTear = true;
		
		//Flag the movement the player is in the movement state of the torn piece
		PlayerMovingPlatformState = true;
	}
	
	/// <summary>
	/// Groups the torn objects after tear complete, only called once
	/// </summary>
	private void GroupTornObjectsFindCenter()
	{
		//Find centerposition of main world cutt piece
		Vector3 newCenterPos = Vector3.zero;
		int numberOfVertices = 0;
		//Find center of every platform - those torn and not torn - only care about vertices
		//currently visible in tirangles storage
		for(int ktor =0; ktor < MainWorldCutPaper.GetComponent<MeshFilter>().mesh.triangles.Count(); ktor++)
		{
			newCenterPos += MainWorldCutPaper.transform.TransformPoint(MainWorldCutPaper.GetComponent<MeshFilter>().mesh.vertices[MainWorldCutPaper.GetComponent<MeshFilter>().mesh.triangles[ktor]]);
			numberOfVertices++;
		}
		//Divide to find average
		newCenterPos /= numberOfVertices;
		MainWorldCutPaperCenterPos = newCenterPos;
		
		MainCutPieceRotationObject = new GameObject();
		MainCutPieceRotationObject.transform.position = MainWorldCutPaperCenterPos;
		MainWorldCutPaper.transform.parent = MainCutPieceRotationObject.transform;
		
		//Debug.Log("center " + MainWorldCutPaperCenterPos.ToString());
		
		RemoveOldPlatforms();
		
		//Create dictionary centerPositions
		foreach(GameObject go in Platforms)
		{
			centerPositions.Add(go, new Vector3(0, 0, 0));
		}
		foreach(GameObject go in TornPlatforms)
		{
			centerPositions.Add(go, new Vector3(0, 0, 0));
		}
		
		//Debug.LogError("********testing tear manager triggered******** length of centerPositions = " + centerPositions.Keys.Count().ToString());
		
		FindCenterOfObject();
		
		DetermineIfPlatformOnCutPiece();
		
		//Ensure this bool logic is only triggered once each time set to true outside
		// from an instance of TearPaper
		//TearFinished = false;
		haveTornOnce = true;
		
		//Testing center of object
		needToSetParentChildRelations = true;
	}
	
	/// <summary>
	/// Determines if platform on cut world piece.
	/// </summary>
	private void DetermineIfPlatformOnCutPiece()
	{
		//Raycast from each center position to determine whether it belongs 
		//on cutt piece or not
		for(int itor = 0; itor < centerPositions.Keys.Count(); itor++)
		{
			
			RaycastHit hit = new RaycastHit();
			Vector3 direction = Camera.main.transform.forward;
			Vector3 pos = new Vector3(centerPositions[centerPositions.Keys.ElementAt(itor)].x, centerPositions[centerPositions.Keys.ElementAt(itor)].y, centerPositions.Keys.ElementAt(itor).transform.position.z + 0.1f);
			if (Physics.Raycast (pos, direction, out hit, 10)) 
			{
				//Debug.Log(centerPositions.Keys.ElementAt(itor).gameObject.name.ToString() + " is hitting object with name = " + hit.collider.gameObject.name.ToString());
				
				//Check if the object is hitting cutt piece
            	if(hit.collider.gameObject.name == "paper_CuttPieceOfPaper" && hit.collider.gameObject != centerPositions.Keys.ElementAt(itor))
				{
       				objectsBelongingToCutPiece.Add(centerPositions.Keys.ElementAt(itor));
  				}
       		}
       		
			
			//Vector3 tempPos = new Vector3(centerPositions[centerPositions.Keys.ElementAt(itor)].x, centerPositions[centerPositions.Keys.ElementAt(itor)].y, MainWorldCutPaper.transform.position.z);
			//if(MainWorldCutPaper.GetComponent<MeshCollider>().bounds.Contains(tempPos))
			//{
			//	objectsBelongingToCutPiece.Add(centerPositions.Keys.ElementAt(itor));
			//}
			
		}
	}
	
	/// <summary>
	/// Finds the center of all interactable game objects
	/// </summary>
	private void FindCenterOfObject()
	{
		//Now loop through each object, find the center and store into centerPositions
		for(int itor = 0; itor < centerPositions.Keys.Count(); itor++)
		{
			//Create position to represent the center of the current object
			Vector3 currObjectCenter = new Vector3(0, 0, 0);
			
			int frequency = 0;
			
			//Find center of every platform - those torn and not torn - only care about vertices
			//currently visible in tirangles storage
			for(int ktor =0; ktor < centerPositions.Keys.ElementAt(itor).GetComponent<MeshFilter>().mesh.triangles.Count(); ktor++)
			{
				currObjectCenter += centerPositions.Keys.ElementAt(itor).transform.TransformPoint(centerPositions.Keys.ElementAt(itor).GetComponent<MeshFilter>().mesh.vertices[centerPositions.Keys.ElementAt(itor).GetComponent<MeshFilter>().mesh.triangles[ktor]]);
				frequency++;
			}
			//Divide to find average
			currObjectCenter /= frequency;
			
			//Store center Position
			centerPositions[centerPositions.Keys.ElementAt(itor)] = currObjectCenter;
			
			//Debug.Log(centerPositions.Keys.ElementAt(itor).name + " has a center = " + currObjectCenter.ToString());
		}
	}
	
	/// <summary>
	/// Removes the old platforms from tearable game level scene
	/// </summary>
	private void RemoveOldPlatforms()
	{
		//Create structure to destroy excess objects not being used
		Dictionary<GameObject, bool> oldTornPlatforms = new Dictionary<GameObject, bool>();
		
		//remove excess objects
		foreach(GameObject go in Platforms)
		{
			if(!go.GetComponent<MeshRenderer>().enabled)
			{
				oldTornPlatforms.Add(go, true);
			}
		}
		//Now, oldTornPlatforms contains all old platforms needing to be erased
		for(int itor = 0; itor < oldTornPlatforms.Keys.Count(); itor++)
		{
			if(oldTornPlatforms[oldTornPlatforms.Keys.ElementAt(itor)])
			{
				//Debug.LogError("Removing Test, object = " + oldTornPlatforms.Keys.ElementAt(itor).name.ToString());
				GameObject.Destroy(oldTornPlatforms.Keys.ElementAt(itor));
				Platforms.Remove(oldTornPlatforms.Keys.ElementAt(itor));
			}
		}
		oldTornPlatforms = null;
		
	}
	
	/// <summary>
	/// Players the colliding with torn piece check.
	/// </summary>
	/// <returns>
	/// The colliding with torn piece check.
	/// </returns>
	private bool PlayerCollidingWithTornPieceCheck()
	{
		for(int itor = 0; itor < MainWorldCutPaper.GetComponent<MeshFilter>().mesh.triangles.Count(); itor++)
		{
			Vector3 vertWorldPos = MainWorldCutPaper.transform.TransformPoint(
				new Vector3(MainWorldCutPaper.GetComponent<MeshFilter>().mesh.vertices[MainWorldCutPaper.GetComponent<MeshFilter>().mesh.triangles[itor]].x,
				MainWorldCutPaper.GetComponent<MeshFilter>().mesh.vertices[MainWorldCutPaper.GetComponent<MeshFilter>().mesh.triangles[itor]].y,
				MainWorldCutPaper.GetComponent<MeshFilter>().mesh.vertices[MainWorldCutPaper.GetComponent<MeshFilter>().mesh.triangles[itor]].z));
			
			vertWorldPos.z = GameObject.FindGameObjectWithTag("Player").transform.position.z;
			if(GameObject.FindGameObjectWithTag("Player").GetComponent<SphereCollider>().bounds.Contains(vertWorldPos))
			{
				return true;
			}
			/*
			for(int jtor = 0; jtor < objectsBelongingToCutPiece.Count(); jtor++)
			{
				float widthOffset = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>().bounds.size.x;
				float heightOffset = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>().bounds.size.y;
				
				Vector3 testPos1 = GameObject.FindGameObjectWithTag("Player").transform.position;
				testPos1.x += (float)(widthOffset/2);
				testPos1.y += (float)(heightOffset/2);
				
				Vector3 testPos2 = GameObject.FindGameObjectWithTag("Player").transform.position;
				testPos2.x += (float)(widthOffset/2);
				testPos2.y -= (float)(heightOffset/2);
				
				Vector3 testPos3 = GameObject.FindGameObjectWithTag("Player").transform.position;
				testPos3.x -= (float)(widthOffset/2);
				testPos3.y += (float)(heightOffset/2);
				
				Vector3 testPos4 = GameObject.FindGameObjectWithTag("Player").transform.position;
				testPos4.x -= (float)(widthOffset/2);
				testPos4.y -= (float)(heightOffset/2);
				
				//Debug.Log("pooop");
				//vertWorldPos.z = objectsBelongingToCutPiece[jtor].transform.position.z;
				
				testPos1.z = objectsBelongingToCutPiece[jtor].transform.position.z;
				testPos2.z = objectsBelongingToCutPiece[jtor].transform.position.z;
				testPos3.z = objectsBelongingToCutPiece[jtor].transform.position.z;
				testPos4.z = objectsBelongingToCutPiece[jtor].transform.position.z;
				
				if(objectsBelongingToCutPiece[jtor].GetComponent<MeshCollider>().bounds.Contains(testPos1) || 
					objectsBelongingToCutPiece[jtor].GetComponent<MeshCollider>().bounds.Contains(testPos2) ||
					objectsBelongingToCutPiece[jtor].GetComponent<MeshCollider>().bounds.Contains(testPos3) ||
					objectsBelongingToCutPiece[jtor].GetComponent<MeshCollider>().bounds.Contains(testPos4))
				{
					Debug.LogError("pooop");
					return true;
				}
			}
			*/
		}
		
		return false;
	}
	
	/// <summary>
	/// Creates the visual tearing effect when player tears
	/// </summary>
	private void CreateTearingEffect()
	{
		if(Input.GetMouseButton(0) && !TearFinished)
		{
			//Play tear audio
			//if(!Camera.main.audio.isPlaying && PlayerCurrentlyTearing)Camera.main.audio.Play();
			
			Vector2 world2DPos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
			Vector3 worldPos = new Vector3(world2DPos.x, world2DPos.y, MainStartingWorldPaper.transform.position.z);
			if(MainStartingWorldPaper.GetComponent<MeshCollider>().bounds.Contains(worldPos))
			{
				//Play tear audio
				if(!Camera.main.audio.isPlaying && PlayerCurrentlyTearing)Camera.main.audio.Play();
				
				CreateNewDecal(worldPos);
			}
			else
			{
				//Stop tear audio
				if(Camera.main.audio.isPlaying)Camera.main.audio.Pause();
			}
		}
		else
		{
			//Stop tear audio
			if(Camera.main.audio.isPlaying)Camera.main.audio.Pause();
		}
	}
	
	/// <summary>
	/// Creates the new decal GameObject
	/// </summary>
	private void CreateNewDecal(Vector3 pos)
	{
		//Create new particle decal object
		GameObject newdecal = (GameObject)Instantiate(DecalObject);
		newdecal.transform.position = new Vector3(pos.x, pos.y, -1);
		newdecal.transform.RotateAround(new Vector3(0, 1, 0), 180);
		decalObjects.Add(newdecal, 0);
	}
	
	private int badTearTimer = 0;
	private int displayBadTearLine = 90;
	bool httingFoldedArea = false;
	
	/// <summary>
	/// Updates the decal objects.
	/// </summary>
	private void UpdateDecalObjects()
	{
		if(BadTear)
		{
			++badTearTimer;
		}
		
		for(int itor = 0; itor < decalObjects.Keys.Count(); itor++)
		{
			//increment the time each decal is alive
			++decalObjects[decalObjects.Keys.ElementAt(itor)];
			
			
			//Check for decal Object colliding with folded region to prevent
			//tearing about folded areas of world paper
			
			if(!httingFoldedArea)
			{
				Vector3 testPos = new Vector3(decalObjects.Keys.ElementAt(itor).transform.position.x, decalObjects.Keys.ElementAt(itor).transform.position.y, 
					FoldObject.transform.position.z);
				if(FoldObject.GetComponent<MeshCollider>().bounds.Contains(testPos))
				{
					//bool test = true;
					//while(test)
					{
						//Debug.Log("PLAYER HAS JUST TRIED TO TEAR THROUGH FOLDED AREA");
						//if(Input.GetMouseButtonUp(0)) test = false;
					}
					//MainStartingWorldPaper.GetComponent<TearPaper>().ForceStopTear();
					httingFoldedArea = true;
					//return;
				}
			}
			
			/*
			if(decalObjects[decalObjects.Keys.ElementAt(itor)] > DecalLife)
			{
				//GameObject.Destroy(decalObjects.Keys.ElementAt(itor));
				//decalObjects.Remove(decalObjects.Keys.ElementAt(itor));
				decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = false;
			}
			else
			{
				//Here we update the size of each new Decal Object
				decalObjects.Keys.ElementAt(itor).transform.localScale = 
					new Vector3(decalObjects.Keys.ElementAt(itor).transform.localScale.x - DecalShrinkSpeed,
						decalObjects.Keys.ElementAt(itor).transform.localScale.y - DecalShrinkSpeed,
						decalObjects.Keys.ElementAt(itor).transform.localScale.z - DecalShrinkSpeed);
				
			}
			*/
			
			if(BadTear)
			{
				decalObjects.Keys.ElementAt(itor).transform.localScale = new Vector3(0.1f, 0.1f, 1.0f);
				decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().material.color = Color.red;
				
				if(!decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled)
				{
					decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = true;
					decalObjects[decalObjects.Keys.ElementAt(itor)] = 0;
				}
				
				//PULSATE BAD TEARLINE
				if(badTearTimer >= 0 && badTearTimer < 30)
				{
					decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = true;
				}
				if(badTearTimer >= 30 && badTearTimer < 40)
				{
					decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = false;
				}
				if(badTearTimer >= 40 && badTearTimer < 70)
				{
					decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = true;
				}
				if(badTearTimer >= 70 || badTearTimer > displayBadTearLine)
				{
					decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = false;
					
					if(badTearTimer > displayBadTearLine)
					{
						GameObject.Destroy(decalObjects.Keys.ElementAt(itor));
						decalObjects.Remove(decalObjects.Keys.ElementAt(itor));
					}
				}
				
				/*
				if(badTearTimer > displayBadTearLine)
				{
					GameObject.Destroy(decalObjects.Keys.ElementAt(itor));
					decalObjects.Remove(decalObjects.Keys.ElementAt(itor));
				}
				*/
			}
			else
			{
				if(decalObjects[decalObjects.Keys.ElementAt(itor)] > DecalLife)
				{
					//GameObject.Destroy(decalObjects.Keys.ElementAt(itor));
					//decalObjects.Remove(decalObjects.Keys.ElementAt(itor));
					decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().enabled = false;
				}
				else
				{
					//Here we update the size of each new Decal Object
					decalObjects.Keys.ElementAt(itor).transform.localScale = 
						new Vector3(decalObjects.Keys.ElementAt(itor).transform.localScale.x - DecalShrinkSpeed,
							decalObjects.Keys.ElementAt(itor).transform.localScale.y - DecalShrinkSpeed,
							decalObjects.Keys.ElementAt(itor).transform.localScale.z - DecalShrinkSpeed);
					
				}
			}
			
			if(TearFinished && !BadTear)
			{
				GameObject.Destroy(decalObjects.Keys.ElementAt(itor));
				decalObjects.Remove(decalObjects.Keys.ElementAt(itor));
			}
		}
	}
	
	/// <summary>
	/// Players the input in bounds.
	/// </summary>
	/// <returns>
	/// The input in bounds.
	/// </returns>
	private bool PlayerInputInBounds()
	{
		Vector2 world2DPos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
		Vector3 worldPos = new Vector3(world2DPos.x, world2DPos.y, MainWorldPaper.transform.position.z);
		if(MainWorldPaper.GetComponent<MeshCollider>().bounds.Contains(worldPos))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
