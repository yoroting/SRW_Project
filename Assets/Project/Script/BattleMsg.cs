using UnityEngine;
using System.Collections;

public class BattleMsg : MonoBehaviour {
	static public int nMsgCount=0;
	// Use this for initialization
	void Start () {
		nMsgCount++;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnAlphaFinish()
	{
		nMsgCount--;
		NGUITools.Destroy ( this.gameObject );

	}

}
