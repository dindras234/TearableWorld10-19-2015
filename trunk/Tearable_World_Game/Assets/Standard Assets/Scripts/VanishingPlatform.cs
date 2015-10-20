using UnityEngine;
using System.Collections;

public class VanishingPlatform : MonoBehaviour{
	public float	cycleTime, timer;
	// Use this for initialization
	void Start(){
		cycleTime = 5.0F;
		timer = 0.0F;
	}
	// Update is called once per frame
	void Update(){
		timer += Time.deltaTime;
		if(timer >= cycleTime){
			this.collider.enabled = !this.collider.enabled;
			this.renderer.enabled = !this.renderer.enabled;
			timer = 0.0F;
		}
	}
}
