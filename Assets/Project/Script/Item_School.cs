using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_School : MonoBehaviour {

    public GameObject RankObj;
    public GameObject NameObj;    
    public GameObject LvObj;
    public GameObject ChangeObj;

    public GameObject InfoObj;
    public GameObject SkillObj;
    //public CMD_BTN chbtn;


    public int m_nMode = 0; // 0 - 察看 , 1 - 切換 , 2- 強化

    public bool bEnable = true;
    
    public int nSchID;
    public int nSchLv;
    public int nSchType;

    myUiTip m_Tip;
    cUnitData m_pUnitdata=null;
    // Use this for initialization
    void Start () {
        //  UIEventListener.Get(this.gameObject).onClick = OnSKillClick; // for trig next line

        UIEventListener.Get(this.gameObject).onClick = OnSchoolClick; // for trig next line

        if (InfoObj != null) {            
            m_Tip = InfoObj.GetComponent<myUiTip>();
            if (m_Tip == null)
            {
                m_Tip = InfoObj.AddComponent<myUiTip>();
            }
        }

        //
      

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

        // 除了整備畫面外，不顯示
        if (nMode == 1)
        {
            if (m_pUnitdata.GetSchoolNum(nSchType) > 1)
            {
                ChangeObj.SetActive(true);
                InfoObj.SetActive(false);
            }
            else
            {
                ChangeObj.SetActive(false);
                InfoObj.SetActive(true);
            }
        }
        else
        {// stage
            ChangeObj.SetActive(false);
            InfoObj.SetActive(true);
        }

        // 改變，永不顯示 info,skill
        InfoObj.SetActive(false);
        SkillObj.SetActive(false); // 永遠只顯示  skill
        // 改變作法
        //ChangeObj.SetActive(false);
        //InfoObj.SetActive(false); // 永遠只顯示 info -> skill
        //SkillObj.SetActive(false); // 永遠只顯示  skill
        // 開發版
        if (Config.GOD) {
         //   ChangeObj.SetActive(true);
        }

    }

    public void SetData(cUnitData pUnit, int SchID, int SchLV =0 )
    {
        m_pUnitdata = pUnit;
        nSchID = SchID;
        nSchLv = SchLV;              

        nSchType = -1;
        LvObj.SetActive((SchLV > 0)); // 有傳值要 顯示
        MyTool.SetLabelText(NameObj, MyTool.GetSchoolName(nSchID));
        MyTool.SetLabelText(LvObj, "等級 "+ nSchLv.ToString() );

        SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(SchID);   //GameDataManager.Instance.GetConstSchoolData ( nSchool );
        if (sch == null)
            return;

        nSchType = sch.n_TYPE;
        MyTool.SetLabelFloat(RankObj, sch.f_RANK );

        // 掛上武學能力
        //UIButton btn = InfoObj.GetComponent<UIButton>();
        //if (btn != null)
        //{
        //    btn.isEnabled = (sch.n_BUFF > 0);
        //}

        //if (m_Tip != null)
        //{
        //    m_Tip.SetTip(sch.n_BUFF, myUiTip._TIP_TYPE._BUFF);
        //}

        // 判斷可否使用 換武學
        //UIButton chbtn = ChangeObj.GetComponent<UIButton>();
        //chbtn.isEnabled = (m_pUnitdata.GetSchoolNum(nSchType) > 1);

        // 判斷可否顯示 技能扭
      //  SkillObj.SetActive( MyTool.GetSkillNumBySchool(SchID) > 0); // 永遠只顯示  skill
        

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
   
        GameSystem.BtnSound();
        Panel_SchoolList.Open(0, m_pUnitdata , nSchID, nSchLv , nSchType); // single school
    }

    public void OnSchoolListClick(GameObject go)
    {   
        GameSystem.BtnSound();
        Panel_SchoolList.Open(1, m_pUnitdata, 0, nSchLv, nSchType);  //  school list
    }

    // on change
    public void OnSKillClick(GameObject go)
    {
        GameSystem.BtnSound();
        Panel_Skill.OpenSchoolUI(m_pUnitdata, _SKILL_TYPE._SCHOOL, nSchID);
    }
}

