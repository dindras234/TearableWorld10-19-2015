/*
	SetRenderQueue.cs
 
	Sets the RenderQueue of an object's materials on Awake. This will instance
	the materials, so the script won't interfere with other renderers that
	reference the same materials.
*/
using UnityEngine;
using System.Collections;
 

public class SetRenderQueue : MonoBehaviour {
 
	//[SerializeField]
	public int[] m_queues = new int[]{3000};
 
	public int OriginalDepth = 0;
	
	public void Awake() {
		if(renderer != null)
		{
			Set(m_queues);
			
			OriginalDepth = m_queues[0];
		}
	}
	
	/// <summary>
	/// Gets the queue.
	/// </summary>
	public int[] GetQueue()
	{
		return m_queues;
	}
	
	/// <summary>
	/// Sets the queue.
	/// </summary>
	public void SetQueue(int[] newQ)
	{
		m_queues = newQ;
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	public void Update()
	{
	}
	
	/// <summary>
	/// Sets the queue.
	/// </summary>
	public void SetFirstQueue(int newVal)
	{
		m_queues[0] = newVal;
		Set(m_queues);
	}
	
	/// <summary>
	/// Sets the queue.
	/// </summary>
	public int GetFirstQueue()
	{
		return m_queues[0];
	}
	
	
	private void Set(int[] newQ)
	{
		Material[] materials = renderer.materials;
		for (int i = 0; i < materials.Length && i < newQ.Length; ++i) 
		{
			materials[i].renderQueue = newQ[i];
		}
	}
}