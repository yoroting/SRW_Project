/*
 *	Singleton
 *
 *	Any script inherits from this class become singleton
 *
 */

using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static bool isApplicationQuit = false;
	protected static T instance;

	/// <summary> Returns the instance of this singleton. </summary>
	public static T Instance
	{
		get
		{
			#if UNITY_EDITOR
			if (isApplicationQuit)
				return null;
			#endif
			
			if(instance == null)
			{
				instance = (T) FindObjectOfType(typeof(T));
				
				if (instance == null)
				{
					GameObject go = new GameObject(typeof(T).Name);
					instance = go.AddComponent<T>();
				}
			}
			
			return instance;
		}
	}

	void OnDestroy()
	{
		instance = null;
	}

	// ios does not dispatch
	void OnApplicationQuit()
	{
		isApplicationQuit = true;
	}	

}