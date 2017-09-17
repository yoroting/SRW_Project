﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_unit : MonoBehaviour {

    public int m_nType;
    public int m_nIdent;
    public int m_nSKillID;

    public bool m_bEnable = true;

    public GameObject spr_Icon;
    public GameObject lbl_Skl; // 技能提示


    public GameObject lbl_Name;
    public GameObject lbl_Comm;

    public GameObject lbl_Ban;  // 禁止 屬性

    public GameObject spr_Aoe;

    // Cost
    public GameObject spr_MP;
    public GameObject spr_SP;
    public GameObject spr_CP;
    public GameObject spr_Atk;
    public GameObject spr_Pow;
    public GameObject spr_Def;

    public GameObject spr_Target;
    public GameObject spr_Range;
    public GameObject spr_CD;

    public GameObject lbl_MP_value;
    public GameObject lbl_SP_value;
    public GameObject lbl_CP_value;
    public GameObject lbl_Atk_value;
    public GameObject lbl_Pow_value;
    public GameObject lbl_Def_value;
    public GameObject lbl_Target_value;
    public GameObject lbl_Range_value;
    public GameObject lbl_CD_value;



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Initialized()
    {
        m_bEnable = true;
        m_nType = 0;
        m_nIdent =0;
        m_nSKillID=0;


        lbl_Ban.SetActive(false);
        spr_Aoe.SetActive(false);

        spr_MP.SetActive( false );
        spr_SP.SetActive(false);
        spr_CP.SetActive(false);

        spr_Atk.SetActive( false );
        spr_Pow.SetActive( false );
        spr_Def.SetActive( false );
        spr_Range.SetActive( false );
        spr_CD.SetActive(false);



    }
    public void SetUnitSkillData(int nIdent, int nSkillID)
    {
        cUnitData unit = GameDataManager.Instance.GetUnitDateByIdent(nIdent);
        SetUnitSkillData(unit , nSkillID);
    }

    public void SetUnitSkillData(cUnitData unit, int nSkillID )
    {
        Initialized();        
        cSkillData skl =  GameDataManager.Instance.GetSkillData(nSkillID);
       
        // 開始設定資料
        SKILL conSkl = skl.skill;
        if (conSkl == null) {
            return;
        }
        m_nSKillID = nSkillID;
        m_nIdent = unit.n_Ident;

        MyTool.SetLabelText( lbl_Name , MyTool.GetSkillName(nSkillID) );

        string strTip = MyTool.GetSkillTip(nSkillID);
        if (conSkl.n_HITBACK > 0  ) {
            strTip += string.Format( "，震退目標{0}格", conSkl.n_HITBACK );
        }
        else if(conSkl.n_HITBACK < 0) {
            strTip += string.Format("，拉近目標{0}格", Mathf.Abs(conSkl.n_HITBACK));
        }


        MyTool.SetLabelText( lbl_Comm, strTip );

        //target
        switch (conSkl.n_TARGET) {
            case 0: MyTool.SetLabelText(lbl_Target_value, "施展者"); break;
            case 1: MyTool.SetLabelText(lbl_Target_value, "敵單體"); break;
            case 2: MyTool.SetLabelText(lbl_Target_value, "友單體"); break;
            case 3: MyTool.SetLabelText(lbl_Target_value, "敵複數"); break;
            case 4: MyTool.SetLabelText(lbl_Target_value, "友複數"); break;
            case 5: MyTool.SetLabelText(lbl_Target_value, "敵複數"); break;
            case 6: MyTool.SetLabelText(lbl_Target_value, "友範圍"); break;
            case 7: MyTool.SetLabelText(lbl_Target_value, "敵範圍"); break;
            case 8: MyTool.SetLabelText(lbl_Target_value, "全範圍"); break;
            case 9: MyTool.SetLabelText(lbl_Target_value, "我全體"); break;
            case 10: MyTool.SetLabelText(lbl_Target_value, "敵全體"); break;
            case 11: MyTool.SetLabelText(lbl_Target_value, "全全體"); break;
        }

        // check AOE
        if (conSkl.n_AREA > 0) {
        //    spr_Aoe.SetActive( true );
        }

        // set cost
        spr_MP.SetActive(conSkl.n_MP != 0);
        MyTool.SetLabelInt(lbl_MP_value , conSkl.n_MP );

        spr_SP.SetActive(conSkl.n_SP != 0);
        MyTool.SetLabelInt(lbl_SP_value , conSkl.n_SP);

        spr_CP.SetActive(conSkl.n_CP != 0);
        MyTool.SetLabelInt(lbl_CP_value, conSkl.n_CP);


        // set range
        spr_Range.SetActive(true);
        if ((conSkl.n_RANGE == 0))
        {
            MyTool.SetLabelText(lbl_Range_value, "自身");
        }
        else if (-1 == conSkl.n_RANGE)
        {
            MyTool.SetLabelText(lbl_Range_value, "無限");
        }        
        else if ((conSkl.n_MINRANGE == conSkl.n_RANGE) || (conSkl.n_MINRANGE==0))
        {
            MyTool.SetLabelInt(lbl_Range_value, conSkl.n_RANGE);
        }
        else
        {
            MyTool.SetLabelText(lbl_Range_value, string.Format("{0}-{1} ", conSkl.n_MINRANGE, conSkl.n_RANGE));
        }

        // 變更 skill Ico
        SetSkillIcon(conSkl.n_TYPE);

        // 一般技能
        if (conSkl.n_SCHOOL > 0)
        {
            spr_Atk.SetActive( true );

            MyTool.SetLabelText(lbl_Atk_value , ((conSkl.f_ATK - 1.0f)*100.0f).ToString() + "％"  ); 
            spr_Pow.SetActive( true );
            MyTool.SetLabelText(lbl_Pow_value, ((conSkl.f_POW - 1.0f) * 100.0f).ToString() + "％");

            if (conSkl.f_DEF != 0 ) {
                spr_Def.SetActive(true);
                MyTool.SetLabelText(lbl_Def_value, ((conSkl.f_DEF) * 100.0f).ToString() + "％");
            }

            // set cd
            if (conSkl.n_CD > 0) {
                spr_CD.SetActive(true);
                MyTool.SetLabelInt(lbl_CD_value, conSkl.n_CD);
                if (unit != null) {
                    int cd = unit.CDs.GetCD(nSkillID);
                    if (cd != 0) {
                        MyTool.SetLabelText(lbl_CD_value, ( (conSkl.n_CD-cd).ToString() +"/"+ conSkl.n_CD.ToString())  );
                    }

                }
            }
            // BAN
            string strBan = "";
            if (skl.IsTag(_SKILLTAG._BANATK)) {
                strBan += "禁攻";
            }
            if (skl.IsTag(_SKILLTAG._BANDEF))
            {
                strBan += "禁反";
            }
            if (skl.IsTag(_SKILLTAG._NOMOVE))
            {
                strBan += "禁移";
            }
            if (strBan.Length > 0) {
                lbl_Ban.SetActive( true );
                MyTool.SetLabelText(lbl_Ban, strBan);
            }


        }
        else // 精神指令
        {
            lbl_Ban.SetActive(true);
            MyTool.SetLabelText(lbl_Ban, "精神指令");
            // 攻擊防禦不顯示
        //    spr_Atk.SetActive(false);
          //  spr_Pow.SetActive(false);
          //  spr_Range.SetActive(false);
         

        }
        


    }

    public void SetSkillIcon( int nType )
    {
        UISprite sp = spr_Icon.GetComponent<UISprite>();

        switch (nType)
        {
            case 0: // 特
                sp.color = Color.yellow;
                MyTool.SetLabelText(lbl_Skl, "特");
                break;
            case 1: // 攻
                sp.color = Color.red;
                MyTool.SetLabelText(lbl_Skl, "攻");
                break;
            case 2: // 反
                sp.color = Color.cyan;
                MyTool.SetLabelText(lbl_Skl, "反");
                break;
            case 3: // 輔
                sp.color = Color.green;
                MyTool.SetLabelText(lbl_Skl, "輔");
                break;
            case 4: // 增
                sp.color = Color.blue;
                MyTool.SetLabelText(lbl_Skl, "增");
                break;
            case 5: // 暗
                sp.color = Color.gray;
                MyTool.SetLabelText(lbl_Skl, "暗");
                break;
        }

    }

    public void SetEnable(bool enable)
    {
        m_bEnable = enable;
        Color c = Color.white; // 黑色
        if (m_bEnable == false)
        {
            c =  Color.gray;
        }

        //if (lbl_Name != null)
        {
            UISprite sp = this.gameObject.GetComponent<UISprite>();
            if (sp != null) {
                sp.color = c;
            }
           // MyTool.SetLabelColor(lbl_Name, c);
        }
    }

    public void CastSkill( )
    {
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
