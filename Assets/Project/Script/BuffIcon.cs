using UnityEngine;
using System.Collections;

public class BuffIcon : MonoBehaviour {

//	public GameObject IconObj;
	
	public GameObject NumObj;
	public GameObject NameObj;
    public GameObject TimeObj;

    public int 	nBuffID;
    public int nBuffTime;
    public int nBuffNum;
    myUiTip m_Tip;
    void Awake()
    {
        m_Tip = this.gameObject.GetComponent<myUiTip>();
        if (m_Tip == null)
        {
            m_Tip = this.gameObject.AddComponent<myUiTip>();
        }
    }

    // Use this for initialization
    void Start () {
        //	UIEventListener.Get(this.gameObject).onClick = OnBuffClick; // for trig next line

       
    }
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SetBuffData ( int nID, int nTime , int nNum   ) {
		nBuffID = nID;
        nBuffTime = nTime;
        nBuffNum = nNum;

        // Set Icon Pic
        SKILL skl = ConstDataManager.Instance.GetRow< SKILL >( nID ); 
		if(skl != null  ){
			// change sprite
			UISprite spr = GetComponent<UISprite>(); 
			if( spr != null ){
				//spr.spriteName = skl.s_ICON ;
			}
		}
        // time
        if (nTime <= 0 )
        {
            TimeObj.SetActive(false);
        }
        else
        {
            TimeObj.SetActive(true);
            MyTool.SetLabelInt(TimeObj, nTime);
        }


        // Buff number
        if ( nNum <= 1 ){
			NumObj.SetActive( false );
		}
		else{
			NumObj.SetActive( true );
			MyTool.SetLabelInt( NumObj , nNum );
		}




		MyTool.SetLabelText (NameObj, MyTool.GetBuffName (nBuffID));

        m_Tip.SetTip(nBuffID , myUiTip._TIP_TYPE._BUFF , nTime );


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
		Panel_Tip.OpenBuffTip ( nBuffID , nBuffTime );
	}

}
