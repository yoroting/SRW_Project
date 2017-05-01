using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMsg : MonoBehaviour {
    static public int nDropCount = 0;

    public GameObject sprMoney;
    public GameObject lblMoney;

    public GameObject lblItem;
    public GameObject lblItemValue;

    // Use this for initialization
    void Start () {
        nDropCount++;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetData(int nExp , int nMoney )
    {
        MyTool.SetLabelText(lblMoney , string.Format( "+{0}" , nMoney) );

        MyTool.SetLabelText(lblItemValue, string.Format("+{0}", nExp));

        //  經驗為負時，不顯示
        if( nExp < 0　)
        {
            lblItem.SetActive(false);
            lblItemValue.SetActive(false);
        }
    }

    public void SetStar(int nStar)
    {
        MyTool.SetLabelText(lblMoney, string.Format("+{0}", nStar));
        //Sprite 換成　星星
        UISprite sp = sprMoney.GetComponent<UISprite>();
        if (sp != null) {
            sp.spriteName = "icon_star";
        }
      
        lblItem.SetActive(false);
        lblItemValue.SetActive(false);
    }

    public void OnAlphaFinish()
    {
        nDropCount--;
        NGUITools.Destroy(this.gameObject);

    }
}
