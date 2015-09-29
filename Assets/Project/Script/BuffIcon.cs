using UnityEngine;
using System.Collections;

public class BuffIcon : MonoBehaviour {

//	public GameObject IconObj;
	
	public GameObject NumObj;
	public GameObject NameObj;
	
	public int 	nBuffID;
	
	
	// Use this for initialization
	void Start () {
		UIEventListener.Get(this.gameObject).onClick += OnBuffClick; // for trig next line
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SetBuffData ( int nID , int nNum =1) {
		nBuffID = nID;
		// Set Icon Pic
		SKILL skl = ConstDataManager.Instance.GetRow< SKILL >( nID ); 
		if(skl != null  ){
			// change sprite
			UISprite spr = GetComponent<UISprite>(); 
			if( spr != null ){
				//spr.spriteName = skl.s_ICON ;
			}
		}
		
		// Buff number
		if( nNum <= 1 ){
			NumObj.SetActive( false );
			
		}
		else{
			NumObj.SetActive( true );
			MyTool.SetLabelInt( NumObj , nNum );
		}

		MyTool.SetLabelText (NameObj, MyTool.GetBuffName (nBuffID));

	}
	
	
	void OnBuffClick( GameObject go )
	{
		// show Tip
//		string nBuffName = MyTool.GetBuffName ( nBiffID );
//
//		string sTip = "";
//		// get content
//		DataRow row = ConstDataManager.Instance.GetRow( (int)ConstDataTables.BUFF_TIP , nBiffID );
//		if( row != null )
//		{				
//			sTip = row.Field<string>( "s_TIP");		
//		}
		Panel_Tip.OpenBuffTip ( nBuffID );
	}

}
