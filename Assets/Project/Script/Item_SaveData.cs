using UnityEngine;
using System.Collections;

public class Item_SaveData : MonoBehaviour {	
	public int nID { set; get; }
    public GameObject NoObj;
    //  public GameObject LvObj;

    public GameObject ContentObj;
    public GameObject NameObj;
    public GameObject StatusObj;
    public GameObject TimeObj;
    public GameObject LvObj;

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

        MyTool.SetLabelText(NoObj, sid);
        // string slv = " - ";
        // string content = "- - - - - - - - -";
        //string content = "NoData";
        char[] split = { ';' };
        string content = cSaveData.LoadSaveSimpleInfo(nID);
        string[] strContents = content.Split(split);

        if (strContents.Length > 0)
        {
            MyTool.SetLabelText(ContentObj, strContents[0]);
        }        

        if (strContents.Length > 1)
        {
            MyTool.SetLabelText(NameObj, strContents[1]);
        }        

        if (strContents.Length > 2)
        {
            MyTool.SetLabelText(StatusObj, strContents[2]);
        }       

        if (strContents.Length > 3)
        {
            MyTool.SetLabelText(TimeObj, strContents[3]);
        }

        if (strContents.Length > 4)
        {
            MyTool.SetLabelText(LvObj,  strContents[4]);

        }
        // MyTool.SetLabelText(LvObj, slv);


    }
	
}
