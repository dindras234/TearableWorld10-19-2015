using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class EmitParticles : MonoBehaviour {
	
	public GameObject DrawParticle;
	
	public float MinDist = 0.01f;
	
	public int MaxTime = 0;
	
	private Dictionary<GameObject, int> decalObjects;
	
	private float lerpDistCheck = 0.001f;
	
	private GameObject prevDecal;
	
	// Use this for initialization
	private void Start () 
	{
		decalObjects = new Dictionary<GameObject, int>();
	}
	
	private float scaleFactor = 0.01f;
	
	// Update is called once per frame
	private void Update () 
	{
		if(!GameObject.FindGameObjectWithTag("AnimationManager").GetComponent<IntroSceneAnimationManager>().AnimationDone)
		{
			//Create new particle decal object
			GameObject newdecal = (GameObject)Instantiate(DrawParticle);
			newdecal.transform.position = transform.position;
			decalObjects.Add(newdecal, 0);
			
			/*
			if(prevDecal != null)
			{
				float dist = Vector2.Distance(new Vector2(newdecal.transform.position.x, newdecal.transform.position.y), 
					new Vector2(prevDecal.transform.position.x, prevDecal.transform.position.y));
				
				if(dist < 0) dist *= -1;
				
				if(dist > lerpDistCheck)
				{
					AddMissingDecal(dist, newdecal.transform.position, prevDecal.transform.position, newdecal);
				}
				else
				{
					prevDecal = newdecal;
				}
			}
			*/
			
			//prevDecal = newdecal;
			
			//Update the time off all decals within dictionary
			for(int itor = 0; itor < decalObjects.Keys.Count(); itor++)
			{
				++decalObjects[decalObjects.Keys.ElementAt(itor)];
				
				//decalObjects.Keys.ElementAt(itor).transform.localScale = new Vector3(decalObjects.Keys.ElementAt(itor).transform.localScale.x - scaleFactor, 
				//	decalObjects.Keys.ElementAt(itor).transform.localScale.y - scaleFactor, decalObjects.Keys.ElementAt(itor).transform.localScale.z);
				/*
				decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().material.color = 
					new Color(decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().material.color.r,
						decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().material.color.g,
						decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().material.color.b,
						decalObjects.Keys.ElementAt(itor).GetComponent<MeshRenderer>().material.color.a*(1/20));
				*/
				
				//remove objects iff they exceed max time
				if(decalObjects.Values.ElementAt(itor) >= MaxTime)
				{
					GameObject.Destroy(decalObjects.Keys.ElementAt(itor));
					decalObjects.Remove(decalObjects.Keys.ElementAt(itor));
				}
			}
		}
		else
		{
			for(int itor = 0; itor < decalObjects.Keys.Count(); itor++)
			{
				GameObject.Destroy(decalObjects.Keys.ElementAt(itor));
				decalObjects.Remove(decalObjects.Keys.ElementAt(itor));
			}
		}
	}
	
	private void AddMissingDecal(float dist, Vector3 currPos, Vector3 prevPos, GameObject nd)
	{
		float numNewObjects = dist/MinDist;
		
		GameObject newdecal = nd;
		for(float itor = 1/numNewObjects; itor <= numNewObjects; itor += 1/numNewObjects)
		{
			Debug.Log("TZESTING");
			Vector3 newPos = Vector3.Lerp(prevPos, currPos, itor);
			newdecal = (GameObject)Instantiate(DrawParticle);
			newdecal.transform.position = newPos;
			decalObjects.Add(newdecal, 0);
			//prevDecal = newdecal;
		}
		prevDecal = newdecal;
	}
}
