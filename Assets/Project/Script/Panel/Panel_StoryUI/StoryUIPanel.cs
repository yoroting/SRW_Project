using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;			// for parser string


public class StoryUIPanel : MonoBehaviour {

	public STORY_DATA	m_StoryData;		// 目前操作的 story data
	private int		m_nFlowIdx;				// 腳本演到哪段
	private int		m_nTargetIdx;			// 劇本目標哪一段
	private cTextArray m_cScript;			// 劇本 腳本集合

	// 
	private bool m_bIsEnd;					// 是否已經演完了

	private int m_nTextIdx;					// 故事文字目前在哪一行
	private List<string> m_cTextList;		// 故事內容集合
	private string m_PopText;               // 是否有要秀出的文字

	private Dictionary<int, GameObject> m_idToCharObj; // 管理 產生的角色物件 

	public GameObject BackGroundTex;            // 大地圖背景貼圖
	public GameObject PanelStoryText;           // 故事文字匡

	private int nTweenObjCount;
	// Declare a delegate type for processing a book:
	public  void OnTweenNotifyEnd( )
	{
		if( --nTweenObjCount < 0 )
		{
			nTweenObjCount = 0;
		}
	}

	void Awake(){
		Debug.Log("StoryUIPanel:awake");
		m_StoryData = new STORY_DATA();
		m_idToCharObj = new Dictionary<int, GameObject>();
		m_cTextList = new List<string> ();
		nTweenObjCount = 0;
		// load const stage data
		// 播放  mian BGM
		DataRow row = ConstDataManager.Instance.GetRow("STAGE_STORY", GameDataManager.Instance.m_nStageID );
		if( row != null )
		{
			if( m_StoryData.FillDatabyDataRow( row ) == false )
				return ;
			GameSystem.PlayBGM ( m_StoryData.n_SCENE_BGM );

			m_cScript = new cTextArray( );
			m_cScript.SetText( m_StoryData.s_CONTEXT );


			//		string strFile = row.Field< string >("s_FILENAME");
			//		if( !String.IsNullOrEmpty( strFile ))
			//		{
			//			string audioPath = ResourcesManager.GetAudioClipPath( AudioChannelType.BGM ,  strFile );
			//			AudioManager.Instance.Play( AudioChannelType.BGM ,  audioPath );

		//	GameObject char1obj = GameSystem.CreateCharacterGameObj( this.gameObject );
		//	if( char1obj != null )
		//	{
		//		Vector3 v = new Vector3( -50 , 50 ,0 );
		//		char1obj.transform.position = v;
		//	}


		}

		m_bIsEnd = false;
		m_nFlowIdx = 0;
		m_nTextIdx = 0;
		m_nTargetIdx = 0;//m_cScript.GetMaxCol()-1;
		ClearText();

		nTweenObjCount =0;
		// add on click event
		//this.GetComponent( );
		UIEventListener.Get(BackGroundTex).onClick += OnPanelButtonClick;

	}
	// Use this for initialization
	void Start () {
		Debug.Log("StoryUIPanel:start");

	}
	
	// Update is called once per frame
	void Update () {
		//	Debug.Log("StoryUIPanel:update");

		// block when animate
		if( nTweenObjCount>0 )
			return;

		if( string.IsNullOrEmpty( m_PopText ) == false )
		{
			if( PanelStoryText.activeSelf == false  )
			{
				PanelStoryText.SetActive( true );
				ClearText();
				TweenWidth t = TweenWidth.Begin<TweenWidth>(PanelStoryText , 0.5f );
				if( t != null )
				{
					t.from = 0;
					t.to = 700;
					t.SetOnFinished( OnTweenNotifyEnd );
					nTweenObjCount++;
				}
			}
			else
			{
				ProcessText( 1 );
			}
			return;  // Dont go next script
		}



		// move cur flow to target flow
		if( m_nFlowIdx < m_nTargetIdx )
		{
			if( m_cScript != null )
			{
				ParserScript( m_cScript.GetTextLine( m_nFlowIdx ) );

				// auto push 1 story text
				PopNextText();
			}

			++ m_nFlowIdx;
		}

	}


	// Base Panel click
	void OnPanelButtonClick(GameObject go)
	{
		Debug.Log("Back Panel click ");
		if( m_cScript == null  )
			return ;
		// check all tween obj complete
		if( nTweenObjCount>0 )
			return;

		//
		if( PopNextText() )
		{
			return ;
		}
	
		// go to next script
		if( m_nTargetIdx == m_nFlowIdx ){ // only set with curscript complete
			if( ++m_nTargetIdx >= m_cScript.GetMaxCol() )
			{
				m_nTargetIdx = m_cScript.GetMaxCol()-1; // change flow 

				EndtoNext();
			}
		}
	

	}

	// end to enter next stage
	public void EndtoNext()
	{
		if( m_bIsEnd == false )
		{
			m_bIsEnd = true;
		}

	}

	public GameObject AddChar( int nCharId , int nPosX , int PosY )
	{
		// check or add
		GameObject obj = null ;

		if( m_idToCharObj.ContainsKey(nCharId) == false )
		{
			obj = GameSystem.CreatePrefabGameObj( this.gameObject , "Panel/Panel_StoryUI/Panel_char" );
			if( obj == null )return null;
			
			DataRow row = ConstDataManager.Instance.GetRow("CHARS", nCharId );
			if( row != null )
			{	
				CHAR_DATA charData = new CHAR_DATA();
				charData.FillDatabyDataRow( row );
				// charge face text
				
				UITexture tex = obj.GetComponentInChildren<UITexture>();
			
				if( tex )
				{
					if(tex != null){
						//	DynamicAssetBundleLoader.LoadTexture(tex,DynamicAssetBundleLoader.SSAssetType.Card, "CARD_" + card.PicName);
						//string texpath = "char/" +charData.s_FILENAME +"_S";
						string url = "Assets/Art/char/" + charData.s_FILENAME +"_S.png";
						//Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
						Texture t= Resources.LoadAssetAtPath( url , typeof(Texture) ) as Texture; ;
						tex.mainTexture = t;
						//tex.mainTexture = Resources.Load( texpath) as Texture; 
						tex.MakePixelPerfect();
					}
				}
			}		
			NGUITools.SetActive( obj , true );

			m_idToCharObj.Add( nCharId , obj );

			TweenAlpha tObj = TweenAlpha.Begin( obj , 1.5f, 1.0f );
			if( tObj != null ){
				tObj.from = 0.0f;
				tObj.SetOnFinished( OnTweenNotifyEnd );
				nTweenObjCount++;
			}
		}
		else {
			m_idToCharObj.TryGetValue( nCharId , out obj  );

		}

		// pos
		if( obj != null )
		{
			Vector3 v = new Vector3( nPosX , PosY ,0 );
			obj.transform.localPosition = v;
			return obj;
		}
		return null;
	}

	public void MoveChar( int nCharId , int nPosX , int nPosY )
	{
		GameObject obj = m_idToCharObj[nCharId];
		if( obj == null )
			return;
		TweenPosition t = TweenPosition.Begin ( obj , 3.0f , new Vector3( nPosX , nPosY , obj.transform.localPosition.z) ); //直接移動
		if( t != null )
		{
		   t.SetOnFinished( OnTweenNotifyEnd);
		   nTweenObjCount++;
		}
		return;

//		TweenPosition tp = null;
//		tp = obj.GetComponent<TweenPosition>( );
//		if( tp == null )
//			tp = obj.AddComponent<TweenPosition>();
//		if( tp != null )
//		{
//			tp.from = obj.transform.localPosition;;
//			tp.to   = new Vector3( nPosX , nPosY , obj.transform.localPosition.z);		
			//obj.active = true;
//			tp.duration = 3.0f;
//			tp.Play(true);
//		}
	}

	public void DelChar( int nCharId , int nType)
	{
		GameObject obj = m_idToCharObj[nCharId];
		if( obj == null )
			return;
		m_idToCharObj.Remove (nCharId);
		TweenAlpha t = TweenAlpha.Begin(obj , 1.0f , 0.0f  );// close
		if( t != null)
		{
			t.SetOnFinished( OnTweenNotifyEnd);
			nTweenObjCount++;
		}
		//NGUITools.Destroy (obj);
	}

	public void DelAll( int nType)
	{
		foreach( KeyValuePair<int , GameObject > pair  in m_idToCharObj )
		{
			TweenAlpha.Begin( pair.Value , 1.0f , 0.0f  );// close
			//NGUITools.Destroy ( pair.Value );
		}
		m_idToCharObj.Clear();
	}



	public void AddStoryText( int StoryID )
	{
		ProcessText( 0 ); 

		DataRow row = ConstDataManager.Instance.GetRow("CONTENT_STORY", StoryID );
		if( row != null )
		{	

			string content = row.Field<string>( "s_CONTENT");
			cTextArray cTxt = new cTextArray( "\n".ToCharArray() , "".ToCharArray() );
			cTxt.SetText( content );
			for( int i = 0 ; i < cTxt.GetMaxCol() ; i++ )
			{
				cTextArray.CTextLine line = cTxt.GetTextLine( i );
				foreach( string s in line.m_kTextPool )
				{
					if( s.IndexOf("//") >= 0 ) // is common
						break; // giveup all of after
					m_cTextList.Add( s );
				}
			}
		}
	}
	void ClearText()
	{
		UILabel lbl = PanelStoryText.GetComponentInChildren <UILabel>();
		if( lbl ){
			lbl.text = "";
		}
	}

	bool PopNextText()
	{
		if( m_nTextIdx < (m_cTextList.Count) )
		{
			ClearText();
			
			m_PopText =  m_cTextList[ m_nTextIdx ];  // this should set to panel text
			m_nTextIdx++; // change text 
			return true;
		}
		return false;
	}

	// this should be menthod  of text panel
	void ProcessText( int nByte=0 )
	{
		if( string.IsNullOrEmpty(m_PopText) )
			return;

		string strSub;
		string strTmp;

		if( nByte > 0 )
		{
			strSub = m_PopText.Substring( 0 , nByte) ;

			int nLen = m_PopText.Length-nByte ;
			if( nLen > 0 )
				strTmp = m_PopText.Substring(  nByte ,m_PopText.Length-nByte ) ;
			else
				strTmp = "";
		}
		else{
			strSub = m_PopText;
			strTmp = "";

		}
		UILabel lbl = PanelStoryText.GetComponentInChildren <UILabel>();
		if( lbl ){
			lbl.text += strSub;
		}
		m_PopText = strTmp;

	}


	// Sys func to parser one line script

	void ParserScript( cTextArray.CTextLine line )
	{
		m_cTextList.Clear();
		m_nTextIdx = 0 ; // change text 

		for( int i = 0 ; i < line.GetRowNum() ; i++ )
		{
			string s = line.GetString( i ).ToUpper();
			if( s == "ADDCHAR" )
			{
				string sp = line.GetString( ++i );	if( sp == null ) return; //  null = error
				List<string> lst = cTextArray.GetParamLst( sp );

				// default value			
				int[] array1 = new int[5];
				int j = 0 ; 

				foreach( string s2 in lst )
				{
					array1[j++] = int.Parse( s2.Trim( ) );

				}

				AddChar( array1[0] , array1[1] , array1[2] );

			}
			else if( s == "MOVECHAR" )
			{
				string sp = line.GetString( ++i );	if( sp == null ) return; //  null = error
				List<string> lst = cTextArray.GetParamLst( sp );
				
				// default value			
				int[] array1 = new int[5];
				int j = 0 ; 
				
				foreach( string s2 in lst )
				{
					array1[j++] = int.Parse( s2.Trim( ) );
					
				}
				
				MoveChar( array1[0] , array1[1] , array1[2] );
			}
			else if( s == "DELCHAR" )
			{
				string sp = line.GetString( ++i );	if( sp == null ) return; //  null = error
				List<string> lst = cTextArray.GetParamLst( sp );
				
				// default value			
				int[] array1 = new int[5];
				int j = 0 ; 
				
				foreach( string s2 in lst )
				{
					array1[j++] = int.Parse( s2.Trim( ) );
					
				}
				
				DelChar( array1[0] , array1[1] );
			}
			else if( s == "DELALL" )
			{
				string sp = line.GetString( ++i );	if( sp == null ) return; //  null = error
				int nType = 0;
				nType = int.Parse( sp.Trim() );
				//if( lst.Count > 0 )
				//	nID = int.Parse( lst[0] );
				
				DelAll( nType  );
				//DelChar( nCharid , nType );
			}
			else if( s == "TEXT" )
			{
				string sp= line.GetString( ++i );	if( sp == null ) return; //  null = error 
				//List<string> lst = cTextArray.GetParamLst( sp );
				
				// only 1 par default value
				int nID = 0;
				nID = int.Parse( sp.Trim() );
				//if( lst.Count > 0 )
				//	nID = int.Parse( lst[0] );
			
				AddStoryText( nID  );
			}
			else if( s == "BGM" )
			{
				string sp1 = line.GetString( ++i );	if( sp1 == null ) return; //  null = error
				
			}
			else if( s == "END" )
			{
				// no need param
				EndtoNext();
				return;
			}
		}
	}
}
