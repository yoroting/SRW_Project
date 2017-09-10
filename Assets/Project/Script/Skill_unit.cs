using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_unit : MonoBehaviour {

    int m_nType;
    int m_nIdent;
    int m_nSKillID;
    public GameObject spr_Icon;

    public GameObject lbl_Name;
    public GameObject lbl_Comm;

    public GameObject lbl_Ban;  // 禁止 屬性
    // Cost
    public GameObject spr_MP;
    public GameObject spr_SP;
    public GameObject spr_CP;
    public GameObject spr_Atk;
    public GameObject spr_Pow;
    public GameObject spr_Range;
    public GameObject spr_CD;

    public GameObject lbl_MP_value;
    public GameObject lbl_SP_value;
    public GameObject lbl_CP_value;
    public GameObject lbl_Atk_value;
    public GameObject lbl_Pow_value;
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
        lbl_Ban.SetActive(false);

        spr_MP.SetActive( false );
        spr_SP.SetActive(false);
        spr_CP.SetActive(false);

        spr_Atk.SetActive( false );
        spr_Pow.SetActive( false );
        spr_Range.SetActive( false );
        spr_CD.SetActive(false);



    }

    public void SetUnitSkillData( int nIdent , int nSkillID )
    {
        Initialized();

        cSkillData skl =  GameDataManager.Instance.GetSkillData(nSkillID);
        cUnitData unit = GameDataManager.Instance.GetUnitDateByIdent( nIdent );
        // 開始設定資料
        SKILL conSkl = skl.skill;
        if (conSkl == null) {
            return;
        }
        MyTool.SetLabelText( lbl_Name , MyTool.GetSkillName(nSkillID) );
        MyTool.SetLabelText( lbl_Comm, MyTool.GetSkillTip(nSkillID) );
        
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
        else if ((conSkl.n_MINRANGE == conSkl.n_RANGE))
        {
            MyTool.SetLabelInt(lbl_Range_value, conSkl.n_RANGE);
        }
        else
        {
            MyTool.SetLabelText(lbl_Range_value, string.Format("{0}-{1} ", conSkl.n_MINRANGE, conSkl.n_RANGE));
        }

        // 一般技能
        if (conSkl.n_SCHOOL > 0)
        {
            spr_Atk.SetActive( true );

            MyTool.SetLabelText(lbl_Atk_value , ((conSkl.f_ATK - 1.0f)*100.0f).ToString()  ); 

            spr_Pow.SetActive( true );

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
        }
        


    }

    public void SetType( int nType )
    {


    }

    public void CastSkill( )
    {
    }

}
