using UnityEngine;
using System.Collections;

public class Item_Skill : MonoBehaviour {
	public GameObject NameObj;
	public GameObject RangeObj;
	public GameObject CostObj;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetItemData( string name , int nRange , int nCost )
	{
		MyTool.SetLabelText ( NameObj , name );
		MyTool.SetLabelInt ( RangeObj , nRange );
		MyTool.SetLabelInt ( CostObj , nCost );

	}
}

