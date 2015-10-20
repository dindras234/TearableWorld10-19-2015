
var sndManager : PlaySound;
/*
function Start () {
//	sndManager = GameObject.Find("player").GetComponent("PlaySound");
//	sndManager = obj.GetComponent("PlaySound");
	//	sndManager : PlaySound = myCollision.gameObject.GetComponent(PlaySound);
	//sndManager : PlaySound;
	sndManager = GameObject.Find("player").GetComponent("PlaySound");
}

function onAwake()
{
		//sndManager : PlaySound = myCollision.gameObject.GetComponent(PlaySound);
}
function Update () {

}*/

function Update()
{
	if(Input.GetKeyDown(KeyCode.Y)){
		gameObject.GetComponent(AudioSource).
		sndManager.PlayAudio(gameObject.GetComponent(AudioSource), "SFX");
			Debug.Log("y");

		}
}
function OnCollisionEnter (myCollision : Collision) {
	Debug.Log("hit");

	if(myCollision.gameObject.name == "player")
	{
		sndManager.PlayAudio(gameObject.GetComponent(AudioSource), "SFX");
		//sndManager.PlayAudio(obj.GetComponent("Audio Source"), "SFX");
	}
}

