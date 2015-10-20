using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class EndLevelAnimate : MonoBehaviour {
	
	private List<Texture2D> frames = new List<Texture2D>();
	int curr = 0;
	
	#region Fields
	public Texture2D frame01;
	public Texture2D frame02;
	public Texture2D frame03;
	public Texture2D frame04;
	public Texture2D frame05;
	public Texture2D frame06;
	public Texture2D frame07;
	public Texture2D frame08;
	public Texture2D frame09;
	public Texture2D frame10;
	public Texture2D frame11;
	public Texture2D frame12;
	public Texture2D frame13;
	public Texture2D frame14;
	public Texture2D frame15;
	public Texture2D frame16;
	public Texture2D frame17;
	public Texture2D frame18;
	public Texture2D frame19;
	public Texture2D frame20;
	public Texture2D frame21;
	public Texture2D frame22;
	public Texture2D frame23;
	public Texture2D frame24;
	public Texture2D frame25;
	public Texture2D frame26;
	public Texture2D frame27;
	public Texture2D frame28;
	public Texture2D frame29;
	public Texture2D frame30;
	public Texture2D frame31;
	public Texture2D frame32;
	public Texture2D frame33;
	public Texture2D frame34;
	public Texture2D frame35;
	public Texture2D frame36;
	public Texture2D frame37;
	public Texture2D frame38;
	public Texture2D frame39;
	public Texture2D frame40;
	public Texture2D frame41;
	public Texture2D frame42;
	public Texture2D frame43;
	public Texture2D frame44;
	public Texture2D frame45;
	public Texture2D frame46;
	public Texture2D frame47;
	public Texture2D frame48;
	public Texture2D frame49;
	public Texture2D frame50;
	public Texture2D frame51;
	public Texture2D frame52;
	public Texture2D frame53;
	public Texture2D frame54;
	public Texture2D frame55;
	public Texture2D frame56;
	public Texture2D frame57;
	public Texture2D frame58;
	public Texture2D frame59;
	public Texture2D frame60;
	public Texture2D frame61;
	public Texture2D frame62;
	public Texture2D frame63;
	public Texture2D frame64;
	public Texture2D frame65;
	public Texture2D frame66;
	public Texture2D frame67;
	public Texture2D frame68;
	public Texture2D frame69;
	public Texture2D frame70;
	public Texture2D frame71;
	public Texture2D frame72;
	public Texture2D frame73;
	public Texture2D frame74;
	public Texture2D frame75;
	public Texture2D frame76;
	public Texture2D frame77;
	public Texture2D frame78;
	public Texture2D frame79;
	public Texture2D frame80;
	public Texture2D frame81;
	public Texture2D frame82;
	public Texture2D frame83;
	public Texture2D frame84;
	public Texture2D frame85;
	public Texture2D frame86;

	#endregion
	
	// Use this for initialization
	void Start () {
	frames.Add(frame01);
	frames.Add(frame02);
	frames.Add(frame03);
	frames.Add(frame04);
	frames.Add(frame05);
	frames.Add(frame06);
	frames.Add(frame07);
	frames.Add(frame08);
	frames.Add(frame09);
	frames.Add(frame10);
	frames.Add(frame11);
	frames.Add(frame12);
	frames.Add(frame13);
	frames.Add(frame14);
	frames.Add(frame15);
	 frames.Add(frame16);
	 frames.Add(frame17);
	 frames.Add(frame18);
	 frames.Add(frame19);
	 frames.Add(frame20);
	 frames.Add(frame21);
	 frames.Add(frame22);
	 frames.Add(frame23);
	 frames.Add(frame24);
	 frames.Add(frame25);
	 frames.Add(frame26);
	 frames.Add(frame27);
	 frames.Add(frame28);
	 frames.Add(frame29);
	 frames.Add(frame30);
	 frames.Add(frame31);
	 frames.Add(frame32);
	 frames.Add(frame33);
	 frames.Add(frame34);
	 frames.Add(frame35);
	 frames.Add(frame36);
	 frames.Add(frame37);
	 frames.Add(frame38);
	 frames.Add(frame39);
	 frames.Add(frame40);
	 frames.Add(frame41);
	 frames.Add(frame42);
	 frames.Add(frame43);
	 frames.Add(frame44);
	 frames.Add(frame45);
	 frames.Add(frame46);
	 frames.Add(frame47);
	 frames.Add(frame48);
	 frames.Add(frame49);
	 frames.Add(frame50);
	 frames.Add(frame51);
	 frames.Add(frame52);
	 frames.Add(frame53);
	 frames.Add(frame54);
	 frames.Add(frame55);
	 frames.Add(frame56);
	 frames.Add(frame57);
	 frames.Add(frame58);
	 frames.Add(frame59);
	 frames.Add(frame60);
	 frames.Add(frame61);
	 frames.Add(frame62);
	 frames.Add(frame63);
	 frames.Add(frame64);
	 frames.Add(frame65);
	 frames.Add(frame66);
	 frames.Add(frame67);
	 frames.Add(frame68);
	 frames.Add(frame69);
	 frames.Add(frame70);
	 frames.Add(frame71);
	 frames.Add(frame72);
	 frames.Add(frame73);
	 frames.Add(frame74);
	 frames.Add(frame75);
	 frames.Add(frame76);
	 frames.Add(frame77);
	 frames.Add(frame78);
	 frames.Add(frame79);
	 frames.Add(frame80);
	 frames.Add(frame81);
	 frames.Add(frame82);
	 frames.Add(frame83);
	 frames.Add(frame84);
	 frames.Add(frame85);
	 frames.Add(frame86);
	}
	public void SetZero()
	{
		Debug.Log("HERE");
		curr = 0;
	}
	// Update is called once per frame
	void Update () {
		if (curr < 170) { gameObject.guiTexture.texture = frames[curr/2]; }
		else { gameObject.guiTexture.texture = frames[85]; }
		curr = (curr+1)%300;
	}
}
