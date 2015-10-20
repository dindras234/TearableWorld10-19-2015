
var sndManager : PlaySound;
var level : String;
/*
function Start () {
//	sndManager = GameObject.Find("player").GetComponent("PlaySound");
//	sndManager = obj.GetComponent("PlaySound");
	//	sndManager : PlaySound = myCollision.gameObject.GetComponent(PlaySound);
	//sndManager : PlaySound;
	sndManager = gameObject.GetComponent("PlaySound");
}*/
/*
function onAwake()
{
		//sndManager : PlaySound = myCollision.gameObject.GetComponent(PlaySound);
}
function Update () {

}*/
function Awake()
{
	sndManager = GameObject.Find("SoundBox").GetComponent(PlaySound);
}
function Update()
{
	if(Input.GetKeyDown(KeyCode.Y))
	{
		sndManager.PlayAudio("smile", "SFX");
		Debug.Log("y");
	}
	if(Input.GetKeyDown(KeyCode.P))
	{
		sndManager.StopSound("smile", "SFX");
	}
	if(Input.GetKeyDown(KeyCode.UpArrow))
		sndManager.SetSndPitch("smile", sndManager.GetSndPitch("smile", "SFX")+.01, "SFX");
	if(Input.GetKeyDown(KeyCode.DownArrow))
		sndManager.SetSndPitch("smile", sndManager.GetSndPitch("smile", "SFX")-0.01, "SFX");
}

function OnCollisionEnter (myCollision : Collision) {
	Debug.Log("hit");

	if(myCollision.gameObject.name == "player")
	{
		sndManager.PlayAudio("dogbark1", "SFX");
		Application.LoadLevel(level);

		//sndManager.PlayAudio(obj.GetComponent("Audio Source"), "SFX");
	}
	
}

