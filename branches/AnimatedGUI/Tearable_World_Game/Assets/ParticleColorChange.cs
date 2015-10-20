using UnityEngine;
using System.Collections;

public class ParticleColorChange : MonoBehaviour 
{
	
	
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		gameObject.particleSystem.startColor = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255)) * Time.deltaTime;
	}
}
