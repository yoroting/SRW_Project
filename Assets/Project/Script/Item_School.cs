using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_School : MonoBehaviour {

    public GameObject RankObj;
    public GameObject NameObj;    
    public GameObject LvObj;
    public CMD_BTN chbtn;


    public int m_nMode = 0; // 0 - 察看 , 1 - 切換 , 2- 強化

    public bool bEnable = true;
    
    public int nSchID;
    public int nSchLv;
    public int nSchType;

    cUnitData m_pUnitdata=null;
    // Use this for initialization
    void Start () {
        UIEventListener.Get(this.gameObject).onClick = OnSchoolClick; // for trig next line
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ReSize()
    {

        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }

    public void SetMode( int nMode )
    {
        m_nMode = nMode;
    }

    public void SetData(cUnitData pUnit, int SchID, int SchLV =0 )
    {
        m_pUnitdata = pUnit;
        nSchID = SchID;
        nSchLv = SchLV;              

        nSchType = -1;
        LvObj.SetActive((SchLV > 0)); // 有傳值要 顯示
        MyTool.SetLabelText(NameObj, MyTool.GetSchoolName(nSchID));
        MyTool.SetLabelText(LvObj, "Lv "+ nSchLv.ToString() );

        SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(SchID);   //GameDataManager.Instance.GetConstSchoolData ( nSchool );
        if (sch == null)
            return;

        nSchType = sch.n_TYPE;
        MyTool.SetLabelFloat(RankObj, sch.f_RANK );
        
    }

    public void SetScrollView(GameObject go)
    {
        UIDragScrollView dsv = this.GetComponent<UIDragScrollView>();
        if (dsv != null)
        {
            dsv.scrollView = go.GetComponent<UIScrollView>();
        }

    }

    public void OnSchoolClick(GameObject go)
    {
        int schid = nSchID;
        if (m_nMode != 0 ) { // 如果是 檢視 以外 都是可以切換的
            schid = 0;
        }

        Panel_SchoolList.Open(m_nMode, m_pUnitdata , schid, nSchLv , nSchType); 
    }

    // on change
    public void OnSKillClick(GameObject go)
    {        
        Panel_Skill.OpenSchoolUI(m_pUnitdata, _SKILL_TYPE._SCHOOL, nSchID);
    }
}
