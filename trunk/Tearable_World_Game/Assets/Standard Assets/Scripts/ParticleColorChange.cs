using UnityEngine;
using System.Collections;

public class ParticleColorChange : MonoBehaviour 
{
	
	
	
	// Use this for initialization
	void Start () 
	{
//		gameObject.particleSystem.startColor = new Color(Random.Range(200, 255), Random.Range(200, 255), Random.Range(200, 255), 255);
	}
	
	// Update is called once per frame
	void Update () 
	{
		gameObject.particleSystem.startColor = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255), 255f) * Time.deltaTime;
		//gameObject.particleSystem.startColor.a = 255;
	}
}
