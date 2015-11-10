﻿using UnityEngine;
using System.Collections;

public class Item_Skill : MonoBehaviour {
	public GameObject NameObj;
	public GameObject RangeObj;
	public GameObject CostObj;
    public GameObject DmgObj;
    public GameObject PowObj;

    public bool bEnable = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetItemData( string name , int nMinRange , int nRange , int nCost )
	{
		MyTool.SetLabelText ( NameObj , name );

		if( (nMinRange == 0) || (nMinRange==nRange) ){

			MyTool.SetLabelInt ( RangeObj , nRange  );
		}
		else{
			MyTool.SetLabelText ( RangeObj , string.Format("{0}-{1} ", nMinRange ,nRange  ) );
		}

		MyTool.SetLabelInt ( CostObj , nCost );

        //
        DmgObj.SetActive( false );
        PowObj.SetActive(false);
    }

    public void SetItemDmgData(float fDmg, float fPow)
    {        
        MyTool.SetLabelText(DmgObj, string.Format("{0}%", (int)(fDmg * 100.0f)));
        MyTool.SetLabelText(PowObj, string.Format("{0}%", (int)(fPow * 100.0f)));
        DmgObj.SetActive(true);
        PowObj.SetActive(true);
    }

    public void SetScrollView( GameObject go )
	{
		UIDragScrollView dsv = this.GetComponent<UIDragScrollView> ();
		if (dsv != null) {
			dsv.scrollView = go.GetComponent< UIScrollView >();
		}


	}
	public void SetEnable( bool enable )
	{
		bEnable = enable;
		Color c = new Color (1.0f, 1.0f, 1.0f);
		if (bEnable == false) {
			c.g = 0.0f; c.b = 0.0f; 
		}
		
		if (NameObj != null) {
			MyTool.SetLabelColor ( CostObj , c );
		}
	}
}

