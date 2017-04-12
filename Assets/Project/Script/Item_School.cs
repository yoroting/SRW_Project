using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_School : MonoBehaviour {

    public GameObject RankObj;
    public GameObject NameObj;    
    public GameObject LvObj;

    

    public bool bEnable = true;

    public int nSchID;
    public int nSchLv;
    public int nSchType;

    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetData( int SchID, int SchLV =0)
    {
        LvObj.SetActive((SchLV > 0)); // 有傳值要 顯示
        SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(SchID);   //GameDataManager.Instance.GetConstSchoolData ( nSchool );
        if (sch == null)
            return;
        nSchID = SchID;
        nSchLv = SchLV;
        nSchType = sch.n_TYPE;


        MyTool.SetLabelFloat(RankObj, sch.f_RANK );
        MyTool.SetLabelText(NameObj, MyTool.GetSchoolName( nSchID ) );


    }

    public void SetScrollView(GameObject go)
    {
        UIDragScrollView dsv = this.GetComponent<UIDragScrollView>();
        if (dsv != null)
        {
            dsv.scrollView = go.GetComponent<UIScrollView>();
        }

    }

   
}
