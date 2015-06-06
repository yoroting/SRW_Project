using UnityEngine;
using System.Collections;

public class BattleValue : MonoBehaviour {

	static public int nValueCount=0;
	// Use this for initialization
	void Start () {
		nValueCount++;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void OnAlphaFinish()
	{
		nValueCount--;
		NGUITools.Destroy ( this.gameObject );
		
	}
}
