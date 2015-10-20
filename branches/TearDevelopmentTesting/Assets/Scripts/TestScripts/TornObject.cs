/// <summary>
/// 
/// FILE: 
/// 	Tear Object
/// 
/// DESCRIPTION: 
/// 	This class is used to create a structure storing the data we need to opperate the tear mechanic
/// 	upon different objects simultaneously
/// 
/// AUTHOR: 
/// 	John Crocker - jrcrocke@ucsc.edu
/// 
/// DATE: 
/// 	2/16/2013 - ...
/// 
/// </summary>
using UnityEngine;
using System.Collections;

public class TornObject {
	
	/// <summary>
	/// The tearable game object.
	/// </summary>
	public GameObject TearableGameObject;
	
	/// <summary>
	/// The mesh represents the paper world's mesh being manipulated
	/// </summary>
	public Mesh mesh;
	
	/// <summary>
	/// This is completely determinant upon what plane mesh is being used (THIS IS CURRENTLY HARDCODED)
	/// TODO: READ IN MESH AND ASSIGN, DO NOT HARDCODE THIS VALUE :-(
	/// </summary>
	public float MESH_VERT_OFFSET;
	
	/// <summary>
	/// The largest ackground piece is the flag to represet whether or not this tornObject is the largest
	/// background piece of paper. This will be helpful in many ways, for example, restricting the 
	/// movement of torn pieces once the player is moving it post tear
	/// </summary>
	public bool LargestbackgroundPiece;
	
	/// <summary>
	/// TearObject Constructor
	/// </summary>
	public TornObject(GameObject TGO, Mesh m, float offset, bool largeFlag)
	{
		//Set all values passed
		TearableGameObject = TGO;
		mesh = m;
		MESH_VERT_OFFSET = offset;
		LargestbackgroundPiece = largeFlag;
	}
	
	
}
