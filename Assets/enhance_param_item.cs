using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enhance_param_item : MonoBehaviour {

    public _ePARAMIDX m_eParamIdx;
    public int m_nCost=0;
    public int m_nMaxLv=0;
    public int m_nCurLv=0;
    public int m_nOrgLv=0;
    public UISprite[] m_spStar ;
    public UILabel m_lblCost;

    cUnitData m_pLinkUnit;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (CheckCanEnhance() == false ) {
            // disable btn
        }
	}

    public void LinkUnit(cUnitData data)
    {
        m_nCost = 0;
        m_pLinkUnit = data;
        if (m_pLinkUnit != null)
        {
            m_nMaxLv = m_pLinkUnit.GetEnhanceLimit(m_eParamIdx);
            
            SetLv( m_pLinkUnit.GetEnhanceLv(m_eParamIdx) );// 更新lv 狀態
        }
        // 更新介面
        UpdateUI();
    }

    public void SetMaxLv( int nMaxLv )
    {
        m_nMaxLv = nMaxLv;
        int nCount=0;
        foreach (UISprite s in m_spStar) {
            s.gameObject.SetActive( (nCount < m_nMaxLv) || (m_nMaxLv==0));
            nCount++;
        }
    }

    public void ReSet()
    {
        m_nCost = 0;
        SetLv( m_nOrgLv );

        if (m_pLinkUnit != null)
        {
            m_pLinkUnit.SetEnhanceLv(m_eParamIdx, m_nOrgLv);
        }
    }

    public void SetLv( int nLv )
    {
        m_nCurLv = nLv;
        m_nOrgLv = m_nCurLv;

        m_nCost  = 0;
    }
    public void UpdateUI()
    {
        // update star
        int nCount = 0;
        foreach (UISprite s in m_spStar)
        {
            if (nCount < m_nCurLv)
            {
                s.color = Color.white;
            }
            else
            {
                s.color = Color.black;
            }
            nCount++;
        }
        // update cost
        m_lblCost.text = m_nCost.ToString();
    }
    public int CalCost( int nLv )
    {
        return (int)((nLv * Config.LevelUPMoney ) * 0.6f) ;
    }

    public bool CheckCanEnhance()
    {
        return ( (m_nMaxLv == 0) || (m_nMaxLv < m_nCurLv) );
    }


    public void OnEnhanceClick(GameObject go)
    {
        // 已達強化上限
        if (CheckCanEnhance() == false )
        {
            return;
        }


        m_nCurLv++;

        m_nCost += CalCost( m_nCurLv );
        // Set Cost
        UpdateUI();

        if (m_pLinkUnit != null) {
            m_pLinkUnit.SetEnhanceLv( m_eParamIdx , m_nCurLv);
        }
        GameSystem.PlaySound(170);
    }
}
