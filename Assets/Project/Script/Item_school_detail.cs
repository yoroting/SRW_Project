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


    public UILabel m_lbl_WeaponType; // 武器類型
  //  public UISprite m_spr_WeaponType; // 武器類型
    // 基礎
    public item_param m_Mar;
    public item_param m_Hp;
    public item_param m_Mp;
    public item_param m_Atk;
    public item_param m_Def;
    public item_param m_Pow;

    // 差異
    //public item_param m_DiffMar;
    //public item_param m_DiffHp;
    //public item_param m_DiffMp;
    //public item_param m_DiffAtk;
    //public item_param m_DiffDef;
    //public item_param m_DiffPow;



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

    public void SetData(int SchID, int SchLV = 0 , cUnitData pUnitData=null )
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

        // 武器類型
        switch (sch.n_RELATIVE)
        {
            case 0: m_lbl_WeaponType.text = "內"; break;
            case 1: m_lbl_WeaponType.text = "劍"; break;
            case 2: m_lbl_WeaponType.text = "刀"; break;
            case 3: m_lbl_WeaponType.text = "槍"; break;
            case 4: m_lbl_WeaponType.text = "掌"; break;
            case 5: m_lbl_WeaponType.text = "拳"; break;
            case 6: m_lbl_WeaponType.text = "棍"; break;
            case 7: m_lbl_WeaponType.text = "暗"; break;
            case 8: m_lbl_WeaponType.text = "毒"; break;
            case 9: m_lbl_WeaponType.text = "兵"; break;
            case 10: m_lbl_WeaponType.text = "弓"; break;
            case 11: m_lbl_WeaponType.text = "扇"; break;
            default: m_lbl_WeaponType.text = "無";  break;
        }
    
//    

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
        if (pUnitData != null)
        {
            int eq_schid = pUnitData.GetSchIDbyType(m_nSchType);
            // 判斷是否為 裝備中武學
            if (m_nSchId == eq_schid) // 本次的內外功類型
            {
                SetChecked(true);
            }
            else
            {
                SetChecked(false);
            }
            // 顯示差異
            //cAttrData diff = new cAttrData();
            //diff.Reset();
            //if (diff != null)
            //{               
            //    cUnitData.CalSchoolAttr(diff, eq_schid, pUnitData.GetSchoolLv(eq_schid) );

            //    //m_DiffMar.ShowDiffValue(attr.f_MAR - diff.f_MAR);
            //    //m_DiffHp.ShowDiffValue(attr.n_HP - diff.n_HP);
            //    //m_DiffMp.ShowDiffValue(attr.n_MP - diff.n_MP);
            //    //m_DiffAtk.ShowDiffValue(attr.n_ATK - diff.n_ATK);
            //    //m_DiffDef.ShowDiffValue(attr.n_DEF - diff.n_DEF);
            //    //m_DiffPow.ShowDiffValue(attr.n_POW - diff.n_POW);

            //}
        }
             

    }

    public void ShowDiff( cUnitData pUnitData)
    {
        if (pUnitData == null)
            return;
        // 顯示各數據
    //     public int m_nSchId;
    //public int m_nSchLv;
    //public int m_nSchType;


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

