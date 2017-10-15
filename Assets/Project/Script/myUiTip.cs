using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myUiTip : MonoBehaviour
{
    public int nTipID=0;
    public string nTipString="";

    // Use this for initialization
    void Start()
    {
        UIEventListener.Get(this.gameObject).onClick = OnTipClick; // 
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTipClick(GameObject go)
    {
        string sTipSub = "";
        string sTip = nTipString;

        if (nTipString == "") {
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
