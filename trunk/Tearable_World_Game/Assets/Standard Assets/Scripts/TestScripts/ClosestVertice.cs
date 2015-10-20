/*
 * 
 * Author : John Crocker
 * 
 * DESCRIPTION : This script is used to flag whether or not 
 * a given decal object hits a folded region of the paper to 
 * prevent tearing folded areas
 * 
 */
using UnityEngine;
using System.Collections;

public class ClosestVertice : MonoBehaviour 
{
	/// <summary>
	/// The mesh offset is used to
	/// represent the global vertice distance
	/// within the paper's mesh
	/// </summary>
	public float MeshOffset;
	
	/// <summary>
	/// The cut main world piece.
	/// </summary>
	public GameObject CutPiece;

	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start () 
	{
	
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update () 
	{
		
	}
	
	/// <summary>
	/// Used to return true when the player tries to tear through
	/// a folded region of the paper
	/// </summary>
	/// <returns>
	/// The cut paper bounds.
	/// </returns>
	public bool WithinCutPaperBounds(Vector3 pos)
	{
		int freqCheck = 0;
		for(int itor = 0; itor < CutPiece.GetComponent<MeshFilter>().mesh.triangles.Length; itor++)
		{
			Vector3 vertWorldPos = transform.TransformPoint(CutPiece.GetComponent<MeshFilter>().mesh.vertices[GetComponent<MeshFilter>().mesh.triangles[itor]]);
			float dist = Vector2.Distance(new Vector2(vertWorldPos.x, vertWorldPos.y), new Vector2(pos.x, pos.y));
			if(dist <= MeshOffset)
			{
				++freqCheck;
			}
			
			if(freqCheck > 3)
			{
				return true;
			}
		}
		
		return false;
	}
}
