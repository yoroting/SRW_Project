using UnityEngine;
using System.Collections;

public class Item_SaveData : MonoBehaviour {
	public GameObject NameObj;
	public int nID { set; get; }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void SetData( int nIdx )
	{
		nID = nIdx;
		name = cSaveData.GetKey( nID );
		//string content = "NoData";

		string content = cSaveData.LoadSimpleInfo (nID);
        if (string.IsNullOrEmpty(content) ) {
            content = "- - -";
        }


		MyTool.SetLabelText( NameObj , content );


	}
	
}
