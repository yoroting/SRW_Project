using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myUiTip : MonoBehaviour
{
    public enum _TIP_TYPE
    {
        _UI =0,
        _BUFF,
        _ITEM,
        _SKILL,
        _SCHOOL,
        _STRING,
    }

    public _TIP_TYPE m_nTipType;
    public int nTipID=0;
    public string nTipString="";
    public int nTipVar1 = 0;
    // Use this for initialization
    void Start()
    {
        UIEventListener.Get(this.gameObject).onPress += OnTipPress;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetTip( int id , _TIP_TYPE type , int nVar1=0 )
    {
        nTipID = id;
        m_nTipType = type;
        nTipVar1 = nVar1;
    }

    void OnTipPress(GameObject go, bool pressed)
    {   
        if (pressed == false ) {
            // CLOSE TIP
            Panel_Tip.CloseUI();
            return;
        }
        // pop tip
        ShowTip();


    }


    public void ShowTip()
    {
        if (nTipID == 0 && nTipString == "") {
            return; // no tip
        }


        switch (m_nTipType ) {
            case _TIP_TYPE._UI: { ShowUITip(); } break;
            case _TIP_TYPE._BUFF: { Panel_Tip.OpenBuffTip(nTipID, nTipVar1);  } break;
            case _TIP_TYPE._ITEM: { Panel_Tip.OpenItemTip(nTipID); } break;
            case _TIP_TYPE._SKILL: { Panel_Tip.OpenSkillTip(nTipID); } break;
            case _TIP_TYPE._SCHOOL: { Panel_Tip.OpenSchoolTip(nTipID); } break;
            case _TIP_TYPE._STRING: {
                    if (string.IsNullOrEmpty(nTipString) == false)
                    {
                        Panel_Tip.OpenUI(nTipString);
                    }
                } break;
        }
    }

    void ShowUITip()
    {
        string sTipSub = "";
        string sTip = nTipString;

        if (nTipString == "")
        {
            // 透過 ID 去 查表          
            // get content
            DataRow row = ConstDataManager.Instance.GetRow((int)ConstDataTables.TIP_MESSAGE, nTipID);
            if (row != null)
            {
                sTip = row.Field<string>("s_TIP_WORDS");
                sTipSub = row.Field<string>("s_TIP_SUBJECT");
            }
            sTip = sTip.Replace("\\n", System.Environment.NewLine);
            if (Config.GOD)
            {
                sTip += "\n(ID:" + nTipID.ToString() + ")";
            }
        }

        // 有tip 則 秀出
        if (string.IsNullOrEmpty(sTip) == false)
        {
            Panel_Tip.OpenUI(sTip);
        }
    }

   

}
