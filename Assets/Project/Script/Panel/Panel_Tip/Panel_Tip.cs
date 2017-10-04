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

	public GameObject spBG;
	public GameObject lblTitle;
	public GameObject lblText;

	// Use this for initialization
	void Start () {
		UIEventListener.Get(spBG).onClick += OnClick; // 
	}
	
	// Update is called once per frame
	void Update () {

	}

	static public void CloseUI(  )
	{
		nTipType = _TIPTYPE._NONE;
		nTipID = 0;
		if (PanelManager.Instance != null) {
			PanelManager.Instance.CloseUI (Name);
		}
	}

	static public void OpenUI( string str )
	{
		Panel_Tip pTip = MyTool.GetPanel< Panel_Tip > (PanelManager.Instance.OpenUI (Name));
		if (pTip) {

			MyTool.SetLabelText ( pTip.lblText , str );
		}

	}
	
	static public void OpenUI( string strTitle , string strContext )
	{
        

        Panel_Tip pTip = MyTool.GetPanel< Panel_Tip > (PanelManager.Instance.OpenUI (Name));
		if (pTip) {
            UILabel lbl = pTip.lblText.GetComponent<UILabel>();
            lbl.overflowMethod = UILabel.Overflow.ResizeFreely;

            MyTool.SetLabelText ( pTip.lblTitle , strTitle );
			MyTool.SetLabelText ( pTip.lblText , strContext );

            // 如果寬度太少，則放大            


            UIWidget wid = pTip.lblText.GetComponent<UIWidget>();
            

            if (wid.width < 480) {
                //wid.width = 480;
                
                lbl.overflowMethod = UILabel.Overflow.ResizeHeight;
                lbl.width = 480;
            }



        }
		
	}

	static public void OpenBuffTip( int nBuffID )
	{
		if (nBuffID == nTipID  && nTipType == _TIPTYPE._BUFF ) {
			CloseUI();
			return ;
		}


		string nBuffName = MyTool.GetBuffName ( nBuffID );
		
		string sTip = "";
		// get content
		DataRow row = ConstDataManager.Instance.GetRow( (int)ConstDataTables.BUFF_TIP , nBuffID );
		if( row != null )
		{				
			sTip = row.Field<string>( "s_TIP");		
		}
        sTip = sTip.Replace( "\\n" , System.Environment.NewLine );


        if (Config.GOD) {

			sTip += "\n(ID:"+nBuffID.ToString() +")";
		}

		nTipType = _TIPTYPE._BUFF;
		nTipID = nBuffID ;

		Panel_Tip.OpenUI( nBuffName , sTip );

	}
	static public void OpenSkillTip( int nSkillID )
	{
		//Panel_Tip.OpenUI( MyTool.GetSkillName( obj.nID )   ); 
		if (nSkillID == nTipID  && nTipType == _TIPTYPE._SKILL ) {
			CloseUI();
			return ;
		}

		string nSkillName = MyTool.GetSkillName( nSkillID );
		
		string sTip = "";
		// get content
		DataRow row = ConstDataManager.Instance.GetRow( (int)ConstDataTables.SKILL_TEXT , nSkillID );
		if( row != null )
		{				
			sTip = row.Field<string>( "s_TIP");		
		}
        sTip = sTip.Replace("\\n", System.Environment.NewLine);
        if (Config.GOD) {
			
			sTip += "\n(ID:"+nSkillID.ToString() +")";
		}
		nTipType = _TIPTYPE._SKILL;
		nTipID   = nSkillID;

		Panel_Tip.OpenUI( nSkillName , sTip );
	}
	static public void OpenItemTip( int nItemID )
	{
		//Panel_Tip.OpenUI( MyTool.GetSkillName( obj.nID )   ); 
		if (nItemID == nTipID  && nTipType == _TIPTYPE._ITEM ) {
			CloseUI();
			return ;
		}
		
		string nItemName = MyTool.GetItemName ( nItemID ); 
		
		string sTip = "";
		// get content
		DataRow row = ConstDataManager.Instance.GetRow( "ITEM_MISC_TIP" , nItemID );
		if( row != null )
		{				
			sTip = row.Field<string>( "s_COMMON");		
		}
        sTip = sTip.Replace("\\n", System.Environment.NewLine);
        if (Config.GOD) {
			
			sTip += "\n(ID:"+nItemID.ToString() +")";
		}

		nTipType = _TIPTYPE._ITEM;
		nTipID   = nItemID;

		Panel_Tip.OpenUI( nItemName , sTip );
	}
	// close
	void OnClick( GameObject go )
	{
		CloseUI ();
		//PanelManager.Instance.CloseUI ( Name );
	}

}
