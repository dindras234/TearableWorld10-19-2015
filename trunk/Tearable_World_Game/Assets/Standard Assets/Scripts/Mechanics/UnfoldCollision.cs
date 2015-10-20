using UnityEngine;
using System.Collections;

public class UnfoldCollision : MonoBehaviour 
{
	
	/// <summary>
	/// The over player.
	/// </summary>
	public bool overPlayer;
	
	/// <summary>
	/// The torn over player.
	/// </summary>
	public bool tornOverPlayer;
	bool unfoldoverplayer;
	private TearManager TM;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start () 
	{
		overPlayer = false;
		
		//initialize to false
		tornOverPlayer = false;
		TM = GameObject.FindGameObjectWithTag("TearManager").GetComponent<TearManager>();
	}
	public void restart(){
		overPlayer = false;
		tornOverPlayer = false;
	}
	/// <summary>
	/// Raises the trigger enter event.
	/// </summary>
	private void OnTriggerEnter(Collider collisionInfo)
	{
		if(collisionInfo.gameObject.name == "backside")
        {
			//Debug.Log("i entered the fucking player : " + this.name);
			overPlayer = true;
		}
		else if(collisionInfo.gameObject.name == "paper_CuttPieceOfPaper" && !tornOverPlayer)
		{
			//Debug.Log("OnTriggerEnter   paper_CuttPieceOfPaper");
			tornOverPlayer = true;
		}
		
	}
	//costing at .1 MS with physics.simulate and it should be unecesary
	//commenting it out for now, stuff appears to still work fine w/o it
//	/// <summary>
//	/// Raises the trigger stay event.
//	/// </summary>
//	private void OnTriggerStay(Collider collisionInfo){
//		// if the hit box is colliding with something set hittingbottom to true and store what we collided with.
//		if(collisionInfo.gameObject.name == "backside")
//        {
//			overPlayer = true;
//			//Debug.Log("im in the fucking player");
//		}
//		
//		if(collisionInfo.gameObject.name == "paper_CuttPieceOfPaper")
//		{
//			//Debug.Log("OnTriggerStay   paper_CuttPieceOfPaper");
//			tornOverPlayer = true;
//		}
//		
//	}
	
	/// <summary>
	/// Raises the trigger exit event.
	/// </summary>
	private void OnTriggerExit(Collider collisionInfo){
		if(collisionInfo.gameObject.name == "backside")
        {
			overPlayer = false;
		}
		
		if(collisionInfo.gameObject.name == "paper_CuttPieceOfPaper")
		{
			//Debug.Log("OnTriggerExit   paper_CuttPieceOfPaper");
			tornOverPlayer = false;
		}
	
	}
	
	/// <summary>
	/// Gets the over player.
	/// </summary>
	public bool getOverPlayer()
	{
		//Debug.Log("returning overPlayer: " + overPlayer.ToString());
		return overPlayer;
	}
	
	
	/// <summary>
	/// Gets the torn over player flag
	/// </summary>
	public bool GetTornOverPlayer()
	{
		return tornOverPlayer;
	}
	
	/// <summary>
	/// Gets the torn over player flag
	/// </summary>
	public void SetTornOverPlayer(bool newVal)
	{
		tornOverPlayer = newVal;
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update()
	{
		//Debug.Log("torn Piece colliding with player = " + tornOverPlayer.ToString());
		
		//Ensure that the tornPiece over player plag is only true when playre is not 
		//if(TM.PlayerMovingPlatformState && tornOverPlayer)
		//{
		//	tornOverPlayer = false;
		//}
	}


}
