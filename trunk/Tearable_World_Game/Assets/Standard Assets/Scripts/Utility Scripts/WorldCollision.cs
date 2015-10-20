using UnityEngine;
using System.Collections;

public class WorldCollision : MonoBehaviour 
{

    public void Start() { }

    public void Update() { }

    /// <summary>
    /// Method that determines if a point is within
    /// the bounds of a game object
    /// </summary>
    /// <param name="meshObject"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool PointInsideObject(GameObject gameObject, Vector2 point)
    {
        //UnityEngine.Debug.Log("FINGER POS " + touchController.GetLastFingerPosition().ToString());
		
		
		///THE FOLLOWING TRY CATCH IS TO PREVENT NullReferenceException UnityEngine.Camera.ScreenToWorldPoint ERROR - J.C.
		try
		{
        	return (gameObject.collider.bounds.Contains(new Vector3(Camera.mainCamera.ScreenToWorldPoint(point).x,
                                                                     Camera.mainCamera.ScreenToWorldPoint(point).y,
                                                                     gameObject.collider.bounds.center.z)));
			
		}
		
		catch
		{
			return false;
		}
    }
	
	public bool PointInsideObjectButton(GameObject gameObject, Vector2 point)
	{
        //UnityEngine.Debug.Log("FINGER POS " + touchController.GetLastFingerPosition().ToString());
		
		
		///THE FOLLOWING TRY CATCH IS TO PREVENT NullReferenceException UnityEngine.Camera.ScreenToWorldPoint ERROR - J.C.
		try
		{
			Camera buttonCamera = GameObject.FindGameObjectWithTag("button").GetComponent<Camera>() as Camera;
        	return (gameObject.collider.bounds.Contains(new Vector3(buttonCamera.ScreenToWorldPoint(point).x,
                                                                     buttonCamera.ScreenToWorldPoint(point).y,
                                                                     gameObject.collider.bounds.center.z)));
			
		}
		
		catch
		{
			return false;
		}
	}

}
