using UnityEngine;
using System.Collections;

public class drawTest : MonoBehaviour {
	
	public GameObject thePlayer;
	Texture2D MyTexture;
	CollisionHandler.Hitbox bottom;
	// Use this for initialization
	void Start () {
		bottom = new CollisionHandler.Hitbox(CollisionHandler.Shape.Rectangle, new CollisionHandler.Rectangle(0, 1, 1, 1), Vector2.zero);
		MyTexture = Resources.Load("Square") as Texture2D;
		Graphics.DrawTexture(new Rect(thePlayer.transform.position.x, thePlayer.transform.position.y, thePlayer.GetComponent<Collider>().bounds.size.x, thePlayer.GetComponent<Collider>().bounds.size.y), MyTexture);
	}
	
	// Update is called once per frame
	void update (){
	}
	void OnGUI () {
		GUI.DrawTexture(new Rect(thePlayer.transform.position.x, thePlayer.transform.position.y, thePlayer.GetComponent<Collider>().bounds.size.x, thePlayer.GetComponent<Collider>().bounds.size.y), MyTexture);
		GUI.DrawTexture(new Rect(bottom.hitbox.x, bottom.hitbox.y, bottom.hitbox.width, bottom.hitbox.height), MyTexture);
	}
}
