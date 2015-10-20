using UnityEngine;
using System.Collections;

public class SpinningPlatform : MonoBehaviour{
	public float	angle;
	// Use this for initialization
	void Start(){
		
	}
	// Update is called once per frame
	void Update(){
		angle = this.transform.rotation.z;
		angle += 1;
		if(angle > 180){ angle -= 180; }
		else if(angle < 0){ angle += 360; }
		
		angle -= this.transform.rotation.z;
		this.transform.Rotate(0, 0, angle);
	}
}
