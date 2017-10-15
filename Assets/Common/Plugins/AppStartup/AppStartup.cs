/*
 *	AppStartup
 *
 *	Load next level at the beginning of app cycle, prevent app from crashing due to long startup time
 *
 */

using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;

public class AppStartup : MonoBehaviour {
	
	// Load next level
	void Start () {       
        Application.LoadLevel( Application.loadedLevel + 1 );
	}
}
