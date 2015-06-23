using UnityEngine;
using System.Collections;

public class BuffIcon : MonoBehaviour {

//	public GameObject IconObj;
	
	public GameObject NumObj;
	
	public int 	nBiffID;
	
	
	// Use this for initialization
	void Start () {
		UIEventListener.Get(this.gameObject).onClick += OnBuffClick; // for trig next line
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SetBuffData ( int nID , int nNum =1) {
		nBiffID = nID;
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
	}
	
	
	void OnBuffClick( GameObject go )
	{
		// show Tip
		DataRow row = ConstDataManager.Instance.GetRow( (int)ConstDataTables.BUFF_TIP , nBiffID );
		if( row != null )
		{				
			string sTip = row.Field<string>( "s_TIP");

			// set tip 
			Panel_Tip.OpenUI( sTip );
			
		}
	}

}
