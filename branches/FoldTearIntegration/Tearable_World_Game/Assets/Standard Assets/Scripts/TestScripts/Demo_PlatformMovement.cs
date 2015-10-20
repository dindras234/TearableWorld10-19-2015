/// <summary>
/// Demo_ platform movement- testing platform logic for sole purpose of DEMO end of Winter quarter
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Demo_PlatformMovement : MonoBehaviour {
	
	/// <summary>
	/// The current position of this object
	/// </summary>
	public Vector3 curPos;
	
	public float offset;
	//public Vector3 offset;
	
	public int Indexer = -1;
	
	/// <summary>
	/// The paper world gameobject being parent to for proper translation and rotation
	/// </summary>
	public GameObject PaperWorld;
	
	public float AngleOffset = 90f;
	
	private float closestXDist = 100000f;
	private float closestYDist = 100000f;
	
	public GameObject myPivotPoint;
	
	// Use this for initialization
	private void Start () 
	{
		float closestDist = 100000f;
		myPivotPoint = new GameObject("PlatformPivot");
		
		//Loop through the mesh of the paper object background
		for(int itor = 0; itor < PaperWorld.GetComponent<MeshFilter>().mesh.vertices.Length; itor++)
		{
			//Debug.LogError("Trying to clamp vertice = ");
			//Make sure we are only evaluating visible vertices in mesh
			if(PaperWorld.GetComponent<MeshFilter>().mesh.triangles.Contains(itor))
			{
				
				float curDist = Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(PaperWorld.transform.TransformPoint(PaperWorld.GetComponent<MeshFilter>().mesh.vertices[itor]).x, PaperWorld.transform.TransformPoint(PaperWorld.GetComponent<MeshFilter>().mesh.vertices[itor]).y));
				if(curDist < 0) curDist *= -1;
				//float curXDist = this.transform.position.x - PaperWorld.transform.TransformPoint(PaperWorld.GetComponent<MeshFilter>().mesh.vertices[itor]).x;
				//if(curXDist < 0) curXDist *= -1;
				//float curYDist = this.transform.position.y - PaperWorld.transform.TransformPoint(PaperWorld.GetComponent<MeshFilter>().mesh.vertices[itor]).y;
				//if(curYDist < 0) curYDist *= -1;
				
				//Debug.LogError("Trying to clamp vertice, dist = " + curDist.ToString()); 
				
				if(curDist < closestDist)//curXDist < closestXDist && curYDist < closestYDist)
				{
					closestDist = curDist;
					offset = curDist;
					//offset = new Vector3(curXDist, curYDist, 0);
					//closestXDist = this.transform.position.x - PaperWorld.transform.TransformPoint(PaperWorld.GetComponent<MeshFilter>().mesh.vertices[itor]).x;
					//closestYDist = this.transform.position.y - PaperWorld.transform.TransformPoint(PaperWorld.GetComponent<MeshFilter>().mesh.vertices[itor]).y;
					//if(closestXDist < 0) closestXDist *= -1;
					//if(closestYDist < 0) closestYDist *= -1;
					//Vector3 positionToFollow = PaperWorld.GetComponent<MeshFilter>().mesh.vertices[itor];
					Indexer = itor;
					//Debug.LogError("*******************Found clamp vertice = ");
					//break;
				}
			}
		}
		
		
		myPivotPoint.transform.position = PaperWorld.transform.TransformPoint(PaperWorld.GetComponent<MeshFilter>().mesh.vertices[Indexer]);
		this.transform.parent = myPivotPoint.transform;
	}
	
	// Update is called once per frame
	private void Update () 
	{
		myPivotPoint.transform.position = PaperWorld.transform.TransformPoint(PaperWorld.GetComponent<MeshFilter>().mesh.vertices[Indexer]);// + new Vector3(closestXDist, closestYDist, 0);
		
	}
}
