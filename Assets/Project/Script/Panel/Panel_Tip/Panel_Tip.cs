using UnityEngine;
using System.Collections;

public class Panel_Tip : MonoBehaviour {

	public const string Name = "Panel_Tip";
	public int nTipType;

	public int nTipID;

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
		PanelManager.Instance.CloseUI (Name);
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
			MyTool.SetLabelText ( pTip.lblTitle , strTitle );
			MyTool.SetLabelText ( pTip.lblText , strContext );
		}
		
	}

	static public void OpenBuffTip( int nBuffID )
	{
		string nBuffName = MyTool.GetBuffName ( nBuffID );
		
		string sTip = "";
		// get content
		DataRow row = ConstDataManager.Instance.GetRow( (int)ConstDataTables.BUFF_TIP , nBuffID );
		if( row != null )
		{				
			sTip = row.Field<string>( "s_TIP");		
		}
		Panel_Tip.OpenUI( nBuffName , sTip );

	}
	static public void OpenSkillTip( int nSkillID )
	{
		//Panel_Tip.OpenUI( MyTool.GetSkillName( obj.nID )   ); 


		string nSkillName = MyTool.GetSkillName( nSkillID );
		
		string sTip = "";
		// get content
		DataRow row = ConstDataManager.Instance.GetRow( (int)ConstDataTables.SKILL_TEXT , nSkillID );
		if( row != null )
		{				
			sTip = row.Field<string>( "s_TIP");		
		}
		Panel_Tip.OpenUI( nSkillName , sTip );
	}
	static public void OpenItemTip( int nItemID )
	{
		//Panel_Tip.OpenUI( MyTool.GetSkillName( obj.nID )   ); 
		
		
		string nItemName = MyTool.GetItemName ( nItemID ); 
		
		string sTip = "";
		// get content
		DataRow row = ConstDataManager.Instance.GetRow( "ITEM_MISC_TIP" , nItemID );
		if( row != null )
		{				
			sTip = row.Field<string>( "s_COMMON");		
		}
		Panel_Tip.OpenUI( nItemName , sTip );
	}
	// close
	void OnClick( GameObject go )
	{

		PanelManager.Instance.CloseUI ( Name );
	}

}
