using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class ShootProjectile : MonoBehaviour {
	
	/// <summary>
	/// The projectile original gameObject
	/// </summary>
	public GameObject Projectile;
	
	/// <summary>
	/// The projectile original gameObject
	/// </summary>
	public GameObject Clamp1;
	
	/// <summary>
	/// The projectile original gameObject
	/// </summary>
	public GameObject Clamp2;
	
	/// <summary>
	/// The projectile original gameObject
	/// </summary>
	public GameObject CompanyNamePaper;
	
	/// <summary>
	/// The projectile original gameObject
	/// </summary>
	public GameObject Part2;
	
	private float timer = 0;
	
	
	public float TimeToShoot = 3.5f;
	
	
	private float timer2NextScene = 0;
	
	
	public float TimeToStartNextScene = 0.25f;
	
	private bool haveShot = false;
	private bool haveStartedPart2 = false;
	
	
	private float timer2EndIntroScene = 0;
	
	
	public float TimeToForceEndIntro = 2000000;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start () 
	{
		EnableChildrenOfObject(Part2, false);
	}
	
	private void EnableChildrenOfObject(GameObject go, bool enabled)
	{
		for(int itor = 0; itor < go.transform.GetChildCount(); itor++)
		{
			go.transform.GetChild(itor).gameObject.SetActive(enabled);
			if(go.transform.GetChild(itor).gameObject.transform.GetChildCount() > 0)
			{
				EnableChildrenOfObject(go.transform.GetChild(itor).gameObject, enabled);
			}
		}
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update () 
	{
		timer += Time.deltaTime;
		
		if(timer >= TimeToShoot && !haveShot)
		{
			haveShot = true;
			CreateProjectile();
			GameObject.Destroy(Clamp1);
			GameObject.Destroy(Clamp2);
			//CompanyNamePaper.GetComponent<InteractiveCloth>().randomAcceleration = new Vector3(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 3), 0);
		}
		else if(haveShot && !haveStartedPart2)
		{
			timer2NextScene += Time.deltaTime;
			if(timer2NextScene > TimeToStartNextScene)
			{
				haveStartedPart2 = true;
				EnableChildrenOfObject(Part2, true);
				TimeToForceEndIntro = 1.75f;
				CompanyNamePaper.GetComponent<ClothRenderer>().enabled = false;
				CompanyNamePaper.GetComponent<InteractiveCloth>().enabled = false;
			}
		}
		
		
		if(haveShot && 
			haveStartedPart2 && 
			GameObject.FindGameObjectWithTag("AnimationManager").GetComponent<IntroSceneAnimationManager>().AnimationDone)
		{
			timer2EndIntroScene += Time.deltaTime;
			//UnityEngine.Debug.LogError("poop");
			
			if(timer2EndIntroScene > TimeToForceEndIntro)
			{
				Application.LoadLevel(Application.loadedLevel);
			}
		}
		
	}
	
	/// <summary>
	/// Creates the projectile.
	/// </summary>
	private void CreateProjectile()
	{
		GameObject newProjectile = (GameObject)Instantiate(Projectile);
		newProjectile.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.GetScreenWidth()/2, Camera.main.GetScreenHeight()/3,0));
		newProjectile.AddComponent("Projectile");
		newProjectile.transform.forward =  Camera.main.transform.forward;
	}
}
