using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enhance_school_item : MonoBehaviour {

    public _eSCHIDX m_eSchIdx;


    public UILabel lblStar;
    public UILabel lblName;
    public UILabel lblLv;
    public UILabel lblCost;
    public UIButton btnAdd;


    public int m_nSchoolID = 0;
    public int m_nCost = 0;
    public int m_nMaxLv = 0;
    public int m_nCurLv = 0;
    public int m_nOrgLv = 0;

    cUnitData m_pLinkUnit;

    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
        if (CheckCanEnhance() == false)
        {
            // disable btn
            btnAdd.isEnabled = false;
        }
        else {
            btnAdd.isEnabled = true;
        }
    }

    void OnEnable()
    {
        ReSize();
    }


    public void ReSize()
    {

        transform.localScale = Vector3.one;

        transform.localRotation = Quaternion.identity;
    }
    public void ReLoad()
    {
        lblName.text = MyTool.GetSchoolName(m_nSchoolID);
        lblLv.text = m_nCurLv.ToString();
        lblCost.text = m_nCost.ToString();
        SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(m_nSchoolID);
        if (sch != null)
        {
            lblStar.text = sch.f_RANK.ToString();
        }
    }
     public void ReSet()
    {
        m_nCost = 0;
        SetData(m_nSchoolID , m_nOrgLv );

        if (m_pLinkUnit != null)
        {
            // m_pLinkUnit.SetEnhanceLv(m_eParamIdx, m_nOrgLv);
            m_pLinkUnit.LearnSchool(m_nSchoolID, m_nOrgLv);
            m_pLinkUnit.ActiveSchool(m_nSchoolID );
        }
    }

    public void SetData(int schoolid, int lv)
    {
        m_nCost = 0;
        m_nSchoolID = schoolid;
        m_nOrgLv = m_nCurLv = lv;
        SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(m_nSchoolID);
        if (sch != null)
        {
            m_nMaxLv = sch.n_MAXLV;
        }
     //   ReLoad();
    }

    public void LinkUnit(cUnitData data)
    {
        m_nCost = 0;
        m_pLinkUnit = data;
        if (m_pLinkUnit != null)
        {
            // m_nMaxLv = m_pLinkUnit.GetEnhanceLimit(m_eParamIdx);
            int id= m_pLinkUnit.GetSchIDbyType( (int)m_eSchIdx);
            int lv = m_pLinkUnit.GetSchoolLv(id);

            SetData(id, lv);
           // SetLv(m_pLinkUnit.GetEnhanceLv(m_eParamIdx));// 更新lv 狀態

        }
        // 更新介面
        ReLoad();
    }
  

    public int CalCost(int nLv)
    {
        if (nLv <= 1)return 0;
        SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(m_nSchoolID);
        if (sch == null) return 0;

        return (int)(((nLv-1) * Config.LevelUPMoney) * 2.0f * sch.f_RANK );
    }

    public bool CheckCanEnhance()
    {
        if ((m_nMaxLv > 0) && (m_nMaxLv <= m_nCurLv))
        {
            return false;
        }

        // 被角色誤性卡住
        if (m_pLinkUnit != null) {
            if ( m_pLinkUnit.cCharData.n_RANK != 0 && m_pLinkUnit.cCharData.n_RANK<= m_nCurLv)
            {
                return false;
            }
        }

        // 確認關卡修練上限
        STAGE_DATA stage = ConstDataManager.Instance.GetRow<STAGE_DATA>(GameDataManager.Instance.nStageID);
        if (stage != null)
        {
            if (stage.n_ENHANCE_LIMIT > 0 && m_nCurLv >= stage.n_ENHANCE_LIMIT)
            {
                return false;
            }
        }
        // 
        return true;
    }

    public void OnEnhanceClick(GameObject go)
    {
        // 已達強化上限
        if (CheckCanEnhance() == false)
        {
            return;
        }
        //// 確認關卡修練上限
        //STAGE_DATA stage = ConstDataManager.Instance.GetRow<STAGE_DATA>(GameDataManager.Instance.nStageID);
        //if( stage != null ) {
        //    if (stage.n_ENHANCE_LIMIT > 0 && m_nCurLv >= stage.n_ENHANCE_LIMIT ) {                
        //        Panel_Tip.OpenUI( "強化上限未開放" );
        //        GameSystem.BtnSound(2); // error
        //        return;
        //    }
        //}

        m_nCurLv++;

        m_nCost += CalCost(m_nCurLv);
       
        ReLoad();

        if (m_pLinkUnit != null)
        {
            m_pLinkUnit.LearnSchool(m_nSchoolID , m_nCurLv );
            m_pLinkUnit.ActiveSchool(m_nSchoolID);
            // m_pLinkUnit.SetEnhanceLv(m_eParamIdx, m_nCurLv);
        }
        GameSystem.PlaySound(170);
    }
}
