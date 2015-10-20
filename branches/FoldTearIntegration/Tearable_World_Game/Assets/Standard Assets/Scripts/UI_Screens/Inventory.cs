/// <summary>
/// FILE:
///    Inventory
/// 
/// DESCRIPTION:
///    This class is used to represent the player's 
/// inventory. The inventory at the moment is simply
/// a list of all the TornObjects a player carries.
/// Once a tear is made, the TornObject should be
/// placed inside this inventory and displayed as
/// a part of the GUI. 
/// 
/// Inventory is represented by a list of GUIBoxes
/// that display a smaller version of the corresponding torn piece.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : TearableScreen {
	
	
	
	#region Fields
	
	
	public enum INVENTORY_PHASES
	{
		NEUTRAL = 0,
		PLACING = 1,
		ROTATING = 2,
	}
	/// <summary>
	/// The width of both left and right buttons. This needs to get scaled when rotated.
	/// </summary>
	private const int BUTTON_WIDTH = 50;
	
	
	/// <summary>
	/// The height of both left and right butons. This needs to get scaled when rotated.
	/// </summary>
	private const int BUTTON_HEIGHT = 50;
	
	/// <summary>
	/// This should be the inventory represented in the game space. 
	/// There needs to be something here that knows when a torn object
	/// is created and then puts said torn object inside this list.
	/// 
	/// TODO: Determine whether or not UNDO should exist.
	/// </summary>
	public List<TornObject> inventory = new List<TornObject>();
	
	
	/// <summary>
	/// Looks at which TornObject is the one that should be placed
	/// and rotated on the game space. 
	/// </summary>
	public TornObject activeTornObject;
	
	/// <summary>
	/// Honestly.. not too sure about how GUIButtons work right now.
	/// Both of these buttons need to be fleshed out and tied to the controller
	/// </summary>

	
	#endregion
	
	
	#region Methods
	
	/// <summary>
	/// OnGui()
	/// 
	/// This is just something that overrides the OnGui()
	/// method (to my knowledge).
	/// </summary>
	void OnGui()
	{
		//TODO: Check the given inventory slot and bring it
		//into the placing phase.
	}
	
	public void RotatePiece(GuiButton button, INVENTORY_PHASES phase)
	{
	}
	
	
	/// <summary>
	/// Displays the inventory.
	/// Simply read through the inventory and display the torn pieces in a smaller capacity
	/// between the left and right rotate buttons.
	/// Each TornObject is represented in the inventory through a GUIBox. 
	/// The GUIBox should display a miniature picture of the torn piece.
	/// </summary>
	public void DisplayInventory()
	{
		foreach(TornObject obj in inventory)
		{
			//TODO: Make a smaller version of the TornObject
			//then tie it to the given Inventory slot.
		}
	}
	
	
	/// <summary>
	/// Places the torn object.
	/// 
	/// To do this, touch a torn piece from the inventory.
	/// This should set the inventory state to "PLACING".
	/// After placing, inventory state should be set to "ROTATING".
	/// </summary>
	public void PlaceTornObject(TornObject pickedTornObject, TouchInfo tapInfo)
	{
		
		/*if(tapInfo.touchPos == pickedTornObject.TearableGameObject.rigidbody.position)
		{
			//holy shit I don't actually know what this does, good luck
		}*/
	    ///TODO: Learn how to get Touch Controller to give me a point
		///corresponding to a certain box in the inventory
		
	}
	#endregion
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void CreateGUIBox()
    {
        throw new System.NotImplementedException();
    }

    public override void CreateGUIButtons()
    {
        throw new System.NotImplementedException();
    }
}
