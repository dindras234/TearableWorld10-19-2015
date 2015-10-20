using UnityEngine;
using System.Collections;

public class VolumeSlider : MonoBehaviour{

	public float hSliderValue = 0.0F;

	public void Start(){}

	public void Update(){}

	void OnGUI(){
		hSliderValue = GUI.HorizontalSlider(new Rect(25, 25, 100, 30), hSliderValue, 0.0F, 1.0F);
	} 
}
