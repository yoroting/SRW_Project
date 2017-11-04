using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item_param : MonoBehaviour {

    public int m_nType;
    public int m_nVar1;
    public UILabel m_lblname;
    public UILabel m_lblvalue;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetName( string sName , int nValue=0)
    {
        m_lblname.text = sName;
        m_lblvalue.text = nValue.ToString();

    }

    public void SetValue( int nValue , int nType=0 )
    {
        string sValue = nValue.ToString();
        // 要顯示 正負號
        if (nType == 1) {
            if (nValue > 0) {
                sValue = "+" + sValue;
            }
            //else if (nValue < 0)
            //{
            //    sValue = "-" + sValue;
            //}
        }

        m_lblvalue.text = sValue;
    }
    public void SetValue(float fValue, int nType = 0)
    {
        string sValue = fValue.ToString();
        // 要顯示 正負號
        if (nType == 1)
        {
            if (fValue > 0)
            {
                sValue = "+" + sValue;
            }
            //else if (nValue < 0)
            //{
            //    sValue = "-" + sValue;
            //}
        }

        m_lblvalue.text = sValue;
    }
}

