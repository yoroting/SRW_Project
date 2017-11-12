using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item_param : MonoBehaviour {

    public _ePARAMIDX m_eParam_idx;
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

    public void SetValue(string sValue, int nType = 0)
    {
        m_lblvalue.text = sValue;
    }


    public void SetUnit(cUnitData unit, int nType = 0)
    {
        if (unit == null)
            return;

        switch (m_eParam_idx)
        {
            case _ePARAMIDX._NULL: break;
            case _ePARAMIDX._LV: { SetValue( unit.n_Lv , nType ); } break;  //1   等級
            case _ePARAMIDX._EXP:{ SetValue(unit.n_EXP, nType); }break; //2	經驗
            case _ePARAMIDX._MAR: { SetValue(unit.GetMar(), nType); } break;  //3	武功
            case _ePARAMIDX._HP: { SetValue(unit.n_HP, nType); } break;  //4	生命
            case _ePARAMIDX._MP: { SetValue(unit.n_MP, nType); } break;  //5   內力
            case _ePARAMIDX._SP: { SetValue(unit.n_SP, nType); } break; //6	精神
            case _ePARAMIDX._ATK: { SetValue(unit.GetAtk(), nType); } break;  //7	攻擊
            case _ePARAMIDX._DEF: { SetValue(unit.GetMaxDef(), nType); } break;  //8   防禦
            case _ePARAMIDX._POW: { SetValue(unit.GetPow(), nType); } break; //9	氣勁
            case _ePARAMIDX._MOV: { SetValue(unit.GetMov(), nType); } break;  //10	移動
            case _ePARAMIDX._BRU: { SetValue(string.Format("{0}％", (unit.GetMulBurst() - 1.0f) * 100.0f) , nType); } break;  //11	爆發
            case _ePARAMIDX._RED: { SetValue(string.Format("{0}％", 100.0f * (1.0f - unit.GetMulDamage())), nType); } break;  //12	減免
            case _ePARAMIDX._ARM: { SetValue(unit.GetArmor(), nType); } break; //13	護甲
            case _ePARAMIDX._TIRED:{ SetValue(unit.nTired, nType); }break;  //14	破綻
            case _ePARAMIDX._MAXHP:{ SetValue(unit.GetMaxHP(), nType); }break;
            case _ePARAMIDX._MAXMP:{ SetValue(unit.GetMaxMP(), nType); }break;
            case _ePARAMIDX._MAXSP:{ SetValue(unit.GetMaxSP(), nType); }break;
            case _ePARAMIDX._MAXDEF:{ SetValue(unit.GetMaxDef(), nType); }break;
            case _ePARAMIDX._CP:{ SetValue(unit.n_CP, nType); }break;
                
            default:
                Debug.LogErrorFormat("item_param error idx : charid={0} : {1} " , unit.n_CharID, m_eParam_idx.ToString() );
                break;    

        }

    }
}

