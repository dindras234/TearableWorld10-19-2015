/*
 * 
 * This script is used to move object from left to from from camera perpsective
 * this is used to tigger triangles being visible vs not visible
 * 
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class ScribleVisibility : MonoBehaviour 
{
	
	private int[] originalTiangles;
	
	// Use this for initialization
	private void Start () 
	{
		originalTiangles = GetComponent<MeshFilter>().mesh.triangles;
		GetComponent<MeshFilter>().mesh.triangles = new int[0];
		
		//Debug.Log("originalTiangles.Count = " + originalTiangles.Count().ToString());
	}
	
	// Update is called once per frame
	private void Update () 
	{
		
		if(!GameObject.FindGameObjectWithTag("AnimationManager").GetComponent<IntroSceneAnimationManager>().AnimationDone)
		{
			int[] newTriangles;
			List<int> newTri = new List<int>();
			bool test = false;
			
			//loop through original mesh, and iff the VisibleMarker's x - pos is less
			//than all three triangle vertices, the triangle then gets added back to
			//meshFilter to become visible once again
			for(int itor = 0; itor < originalTiangles.Count(); itor += 3)
			{
				//Debug.LogError("pos = " + GameObject.FindGameObjectWithTag("VisibleMarker").transform.position.x.ToString());
				//Debug.LogError("vertX = " + transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[originalTiangles[itor]]).x.ToString());
				if(transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[originalTiangles[itor]]).x > GameObject.FindGameObjectWithTag("VisibleMarker").transform.position.x &&
					transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[originalTiangles[itor + 1]]).x > GameObject.FindGameObjectWithTag("VisibleMarker").transform.position.x &&
					transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[originalTiangles[itor + 2]]).x > GameObject.FindGameObjectWithTag("VisibleMarker").transform.position.x)
				{
					//Debug.LogError(" test1 ");
					newTri.Add(originalTiangles[itor]);
					newTri.Add(originalTiangles[itor + 1]);
					newTri.Add(originalTiangles[itor + 2]);
					test = true;
				}
			}
			
			if(test)
			{
				newTriangles = new int[newTri.Count()];
				
				for(int jtor = 0; jtor < newTri.Count(); jtor++)
				{
					//Debug.LogError(" test22 ");
					newTriangles[jtor] = newTri.ElementAt(jtor);
				}
				
				GetComponent<MeshFilter>().mesh.triangles = newTriangles;
			}
		}
	}
}