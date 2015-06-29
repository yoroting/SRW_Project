using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using _SRW;

public class ColorButton : MonoBehaviour {

	public _CMD_ID 	 CMD_ID;
	public List<int>		 nArg;

	void Awake()
	{
		nArg = new List<int>();
	}
	// Use this for initialization
	void Start () {
		UIWidget wi = this.GetComponent<UIWidget >();
		if( wi != null ){
			wi.width = 150;  // size fix to correct wh
			wi.height = 40;			
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	// get param at array
	public int At( int nIdx )
	{
		if( (nArg== null)|| (nIdx >=nArg.Count) )
			return 0;
		return nArg[ nIdx ];
	}

	
	void OnDestroy()
	{

	}


}
