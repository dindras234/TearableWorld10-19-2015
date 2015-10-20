using UnityEngine;
using System.Collections;

public class ArmMovement : MonoBehaviour {
	
	public float Movement = 0.3f;
	
	public float Speed = 0.1f;
	
	private float MaxPos;
	
	private float MinPos;
	
	private bool movingUp = true;
	
	// Use this for initialization
	void Start () 
	{
		MaxPos = transform.position.y + Movement;
		MinPos = transform.position.y - Movement;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		if(!GameObject.FindGameObjectWithTag("AnimationManager").GetComponent<IntroSceneAnimationManager>().AnimationDone)
		{
			if(movingUp)
			{
				if(transform.position.y > MaxPos)
				{
					movingUp = false;
				}
				transform.position = new Vector3(transform.position.x, transform.position.y + Speed, transform.position.z);
			}
			else
			{
				if(transform.position.y < MinPos)
				{
					movingUp = true;
				}
				transform.position = new Vector3(transform.position.x, transform.position.y - Speed, transform.position.z);
			}
		}
	}
}
