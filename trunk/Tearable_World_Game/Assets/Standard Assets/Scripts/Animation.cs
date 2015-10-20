using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

	
public class Animation
{
    /// <summary>
    /// All possible animations 
    /// that can be played for
    /// the player object
    /// </summary>
    public enum AnimationState
    { 
		STAND = 0,
		IDLE = 1,
		IDLEA = 2,
		IDLEB = 3,
		WALK = 4,
		WALKINTOWALL = 5,
		WALKUP = 6,
		WALKDOWN = 7,
		RUN = 8,
		JUMP = 9,
		FALL = 10,
		OPENDOOR = 11,
		DEATH = 12
    }
		/// <summary>
	/// All possible animations
	/// that the player can perform
	/// </summary>
    List<Texture2D> frames;
    AnimationState state;
	
	
	public Animation (List<Texture2D> frames, AnimationState state)
	{
		this.frames = frames;
        this.state = state;
	}

    public int GetLength()
    {
        return frames.Count;
    }

    public List<Texture2D> GetFrames()
	{
		return frames;
	}
	
	public AnimationState GetState()
	{
        return state;
	}
}


	