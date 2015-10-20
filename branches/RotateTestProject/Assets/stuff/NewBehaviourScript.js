var myLevel : String;

function OnCollisionEnter (myCollision : Collision) {
	Debug.Log("HIT");
	if(myCollision.gameObject.name == "player")
	{
		Application.LoadLevel(myLevel);
	}
}