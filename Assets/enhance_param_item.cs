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
    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetMaxLv( int nMaxLv )
    {
        m_nMaxLv = nMaxLv;
        int nCount=0;
        foreach (UISprite s in m_spStar) {
            s.gameObject.SetActive( nCount < m_nMaxLv);
            nCount++;
        }
    }

    public void ReSet()
    {
        m_nCost = 0;
        SetLv( m_nOrgLv );
    }

    public void SetLv( int nLv )
    {
        m_nCurLv = nLv;
        m_nOrgLv = m_nCurLv;
    }
    public void UpdateStar()
    {
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
    }
    public int CalCost( int nLv )
    {
        return (nLv * 1000);
    }

    public void OnEnhanceClick(GameObject go)
    {
        m_nCurLv++;

        m_nCost += CalCost( m_nCurLv );
        // Set Cost
        UpdateStar();

        m_lblCost.text = m_nCost.ToString();
    }
}
