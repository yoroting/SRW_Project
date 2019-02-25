using UnityEngine;
using System.Collections;

public class Panel_Tip : MonoBehaviour {

	public const string Name = "Panel_Tip";

	public enum _TIPTYPE{
		_NONE,
		_SYS,
		_UI,
		_SKILL,
		_BUFF,
		_ITEM

	};


	public static  _TIPTYPE nTipType = _TIPTYPE._NONE;

	public static int nTipID=0;

	public UISprite spBG;
	public GameObject lblTitle;
	public GameObject lblText;

    public UILabel lblSubText;

	// Use this for initialization
	void Start () {
		UIEventListener.Get(spBG.gameObject).onClick += OnClick; // 
	}

    private void OnEnable()
    {
        lblTitle.gameObject.SetActive(false);
        lblSubText.gameObject.SetActive( false ); // default is close
    }

    // Update is called once per frame
    void Update () {

        // 底圖 size 修正
        
        if (spBG != null ) {
            if ( lblTitle.activeSelf == false ) {
             //   spBG.topAnchor = lblText.transform.up;
            }
            //else {
            //    spBG.topAnchor.target = lblTitle.transform;
            //}
        }
        //UIAnchor anc = spBG.GetComponent<UIAnchor>();
        //if (anc != null)
        //{
        //    anc
        //}
        //if (lblSubText.gameObject.activeSelf ) {
        //    Vector3 vpos = new Vector3();
        //    vpos.x = (spBG.width - lblSubText.width - 20) / 2;
        //    vpos.y = lblTitle.transform.localPosition.y;
        //    lblSubText.gameObject.transform.localPosition = vpos;
        //}

    }

    static public void CloseUI(  )
	{
		nTipType = _TIPTYPE._NONE;
		nTipID = 0;
		if (PanelManager.Instance != null) {
			PanelManager.Instance.CloseUI (Name);
		}
       // GameSystem.PlaySound("Tap"); // will error in close app
    }

    public void SetContent(string str )
    {
        UILabel lbl = lblText.GetComponent<UILabel>();
        UIWidget wid = lblText.GetComponent<UIWidget>();
        lbl.overflowMethod = UILabel.Overflow.ResizeFreely;
        MyTool.SetLabelText(lblText, ""); // 清空來確保每次 文字都異動
        lbl.width = 480;

        MyTool.SetLabelText(lblText, str);
        //if (wid.width <= 240)
        //{
        //    //wid.width = 480;
        //    lbl.overflowMethod = UILabel.Overflow.ResizeHeight;
        //    lbl.width = 240;
        //}
        //else
        if (wid.width <= 480)
        {
            //wid.width = 480;

            lbl.overflowMethod = UILabel.Overflow.ResizeHeight;
            lbl.width = 480;
        }
        else if (wid.width > 900)
        {
            //wid.width = 480;

            lbl.overflowMethod = UILabel.Overflow.ResizeHeight;
            lbl.width = 900;
        }

        // 座標修正
        lbl.transform.localPosition = Vector3.zero;

        // 背景高度修正
        //spBG.height = lbl.height + 56;
        spBG.height = (int)(lbl.drawingDimensions.w-lbl.drawingDimensions.y) + 48;
    }

    public void SetTitle(string str)
    {  
        MyTool.SetLabelText(lblTitle, str);
        if (lblTitle.activeSelf == false)
        {
            lblTitle.SetActive(true);

            UILabel lbl = lblTitle.GetComponent<UILabel>();
            spBG.height += (lbl.fontSize+(int)lbl.floatSpacingY)*2;
            //// 背景高度
            //if (spBG != null)
            //{

            //    //    spBG.width = lbl.width + 100;

            //    // 是否有 title
            //    if (lblTitle.activeSelf == true)
            //    {
            //        spBG.height += (lbl.finalFontSize) * 2;
            //        UIWidget title_wid = lblTitle.GetComponent<UIWidget>();
            //        if (title_wid != null)
            //        {
            //            title_wid.height = lbl.finalFontSize;
            //        }
            //    }
            //}
        }
    }

    public void SetTime(int nTime)
    {
        if (nTime > 0)
        {
            lblSubText.gameObject.SetActive(true); // default is close
            //string sTime = string.Format("持續：{0}", nTime);
            lblSubText.text = string.Format("剩餘：{0}回合", nTime); ;

         

            //lblSubText.height = 30;
        }
    }


    static public void OpenUI( string str )
	{
        GameSystem.PlaySound("Tap");
        Panel_Tip pTip = MyTool.GetPanel< Panel_Tip > (PanelManager.Instance.OpenUI (Name));
		if (pTip) {
            pTip.lblTitle.SetActive(false);

            pTip.SetContent( str );
           // MyTool.SetLabelText ( pTip.lblText , str );

        }

	}
	
	static public Panel_Tip OpenUI( string strTitle , string strContext )
	{

        GameSystem.PlaySound("Tap");
        Panel_Tip pTip = MyTool.GetPanel< Panel_Tip > (PanelManager.Instance.OpenUI (Name));
		if (pTip) {
            pTip.SetContent(strContext);
            if (strTitle != "")
            {
                pTip.SetTitle(strTitle);                
            }
        }
        return pTip;

    }

    static public void OpenUITip(int nTipID)
    {
        string sTipSub = "";
        string sTip = "";

       // if (nTipString == "")
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


    static public void OpenBuffTip( int nBuffID  , int nTime )
	{
		if (nBuffID == nTipID  && nTipType == _TIPTYPE._BUFF ) {
			CloseUI();
			return ;
		}


		string nBuffName = MyTool.GetBuffName ( nBuffID );

        string sTip = MyTool.GetBuffTip( nBuffID );

        // get buff time
        BUFF buff = ConstDataManager.Instance.GetRow<BUFF>(nBuffID);
        if (buff != null)
        {
            if (buff.n_DURATION > 0)
            {
                sTip += string.Format(",持續{0}回合", buff.n_DURATION);
            }
        }

        // get content
        //DataRow row = ConstDataManager.Instance.GetRow( (int)ConstDataTables.BUFF_TIP , nBuffID );
        //if( row != null )
        //{				
        //	sTip = row.Field<string>( "s_TIP");		
        //}
        //      sTip = sTip.Replace( "\\n" , System.Environment.NewLine );


        if (Config.GOD) {

			sTip += "\n(ID:"+nBuffID.ToString() +")";
		}

		nTipType = _TIPTYPE._BUFF;
		nTipID = nBuffID ;

        Panel_Tip pTip =  Panel_Tip.OpenUI( nBuffName , sTip );
        if ( pTip != null ) {
            pTip.SetTime( nTime );
        }

	}
	static public void OpenSkillTip( int nSkillID )
	{
		//Panel_Tip.OpenUI( MyTool.GetSkillName( obj.nID )   ); 
		if (nSkillID == nTipID  && nTipType == _TIPTYPE._SKILL ) {
			CloseUI();
			return ;
		}

		string nSkillName = MyTool.GetSkillName( nSkillID );
        string sTip = MyTool.GetSkillTip(nSkillID);
  //      string sTip = "";
		//// get content
		//DataRow row = ConstDataManager.Instance.GetRow( (int)ConstDataTables.SKILL_TEXT , nSkillID );
		//if( row != null )
		//{				
		//	sTip = row.Field<string>( "s_TIP");		
		//}
  //      sTip = sTip.Replace("\\n", System.Environment.NewLine);
        if (Config.GOD) {
			
			sTip += "\n(ID:"+nSkillID.ToString() +")";
		}
		nTipType = _TIPTYPE._SKILL;
		nTipID   = nSkillID;

		Panel_Tip.OpenUI( nSkillName , sTip );
	}
	static public void OpenItemTip( int nItemID )
	{
        if (nItemID == 0)
            return;

		//Panel_Tip.OpenUI( MyTool.GetSkillName( obj.nID )   ); 
		if (nItemID == nTipID  && nTipType == _TIPTYPE._ITEM ) {
			CloseUI();
			return ;
		}
		
		string nItemName = MyTool.GetItemName ( nItemID ); 
		
		string sTip = MyTool.GetItemTip(nItemID);
        //// get content
        //DataRow row = ConstDataManager.Instance.GetRow( "ITEM_MISC_TIP" , nItemID );
        //if( row != null )
        //{				
        //	sTip = row.Field<string>( "s_COMMON");		
        //}
        //      sTip = sTip.Replace("\\n", System.Environment.NewLine);
        if (Config.GOD) {
			
			sTip += "\n(ID:"+nItemID.ToString() +")";
		}

		nTipType = _TIPTYPE._ITEM;
		nTipID   = nItemID;

		Panel_Tip.OpenUI( nItemName , sTip );
	}

    static public void OpenSchoolTip(int nSchoolID)
    {
        string nSchoolName = MyTool.GetSchoolName(nSchoolID);

        int nBuffID = 0;
        string sTip = "";
        SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(nSchoolID);   //GameDataManager.Instance.GetConstSchoolData ( nSchool );
        if (sch == null) {

            sTip = MyTool.GetBuffTip(sch.n_BUFF); // 武學buff
        }

        Panel_Tip.OpenUI(nSchoolName, sTip);
    }
    
    // close
    void OnClick( GameObject go )
	{
		CloseUI ();
		//PanelManager.Instance.CloseUI ( Name );
	}

}
