using UnityEngine;
using System.Collections;

public class Item_SaveData : MonoBehaviour {	
	public int nID { set; get; }
    public GameObject NoObj;
  //  public GameObject LvObj;

    public GameObject NameObj;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void SetData( int nIdx )
	{
		nID = nIdx;

        string sid = nID ==0 ? "自動":string.Format( "{0}." , nID );
       // string slv = " - ";
       // string content = "- - - - - - - - -";
        //string content = "NoData";

        string content = cSaveData.LoadSaveSimpleInfo(nID);

        MyTool.SetLabelText(NoObj, sid);
       // MyTool.SetLabelText(LvObj, slv);
        MyTool.SetLabelText( NameObj , content );

	}
	
}
