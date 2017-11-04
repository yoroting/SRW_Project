using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_school_detail : MonoBehaviour {
    public int m_nSchId;
    public int m_nSchLv;
    public int m_nSchType;

    public UILabel m_lbl_star;
    public UILabel m_lbl_name;
    public UILabel m_lbl_lv;

    public item_param m_Mar;
    public item_param m_Hp;
    public item_param m_Mp;
    public item_param m_Atk;
    public item_param m_Def;
    public item_param m_Pow;

    public UILabel m_lbl_content;

    public GameObject m_Checked;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnEnable()
    {
        ReSize();
        if(m_Checked!=null)
            m_Checked.SetActive(false);

        m_nSchId =0;
        m_nSchLv=0;
        m_nSchType=0;
        m_lbl_content.text = "";
        
    }

    public void ReSize()
    {
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }
    public void SetData(int SchID, int SchLV = 0)
    {
        m_nSchId = SchID;
        m_nSchLv = SchLV ;

        m_lbl_lv.gameObject.SetActive((SchLV > 0)); // 有傳值要 顯示
        SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(SchID);   //GameDataManager.Instance.GetConstSchoolData ( nSchool );
        if (sch == null)
            return;
        m_nSchType = sch.n_TYPE;

        m_lbl_star.text = sch.f_RANK.ToString();
        m_lbl_name.text = MyTool.GetSchoolName( SchID );
        m_lbl_lv.text = "Lv " + SchLV.ToString();

        //  有說明
        if (sch.n_BUFF > 0)
        {
            m_lbl_content.text = MyTool.GetBuffTip(sch.n_BUFF);
         //   m_lbl_content.gameObject.SetActive(true);
        }


        cAttrData attr = new cAttrData();
        attr.Reset();

        cUnitData.CalSchoolAttr(attr , SchID, SchLV );
        // 計算出 加成 參數
        m_Mar.SetValue(attr.f_MAR , 1);
        m_Hp.SetValue(attr.n_HP, 1 );
        m_Mp.SetValue(attr.n_MP, 1);
        m_Atk.SetValue(attr.n_ATK, 1);
        m_Def.SetValue(attr.n_DEF, 1);
        m_Pow.SetValue(attr.n_POW, 1);
        //nSchID = SchID;
        //nSchLv = SchLV;
        //nSchType = sch.n_TYPE;


        //MyTool.SetLabelFloat(RankObj, sch.f_RANK);
        //MyTool.SetLabelText(NameObj, MyTool.GetSchoolName(nSchID));
        //MyTool.SetLabelInt(LvObj, nSchLv);
        //LvObj.SetActive(SchLV > 0);

    }

    public void SetScrollView(GameObject go)
    {
        UIDragScrollView dsv = this.GetComponent<UIDragScrollView>();
        if (dsv != null)
        {
            dsv.scrollView = go.GetComponent<UIScrollView>();
        }

    }

    public void SetChecked( bool check )
    {
        m_Checked.SetActive( check );
    }
}

