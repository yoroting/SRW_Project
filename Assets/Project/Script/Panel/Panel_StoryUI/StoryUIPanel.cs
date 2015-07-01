using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;			// for parser string


public class StoryUIPanel : MonoBehaviour {

	public const string Name = "Panel_StoryUI";

	public STAGE_STORY	m_StoryData;		// 目前操作的 story data
	private int		m_nFlowIdx;				// 腳本演到哪段
	private int		m_nTargetIdx;			// 劇本目標哪一段
	private cTextArray m_cScript;			// 劇本 腳本集合

	// 
	private bool m_bIsEnd;					// 是否已經演完了

	private Dictionary<int, GameObject> m_idToCharObj; // 管理 產生的角色物件 

	public GameObject BackGroundTex;            // 大地圖背景貼圖
	public GameObject PanelStoryText;           // 故事文字匡
	public GameObject SkipButton;           	// 跳過

	bool 	bIsLoading;							// load story ui
	private int nTweenObjCount;
	// Declare a delegate type for processing a book:
	public  void OnTweenNotifyEnd( )
	{
		if( --nTweenObjCount < 0 )
		{
			nTweenObjCount = 0;
		}
	}


	// sys func
	void Awake(){
		Debug.Log("StoryUIPanel:awake");
		nTweenObjCount = 0;
		m_idToCharObj = new Dictionary<int, GameObject>();

		m_bIsEnd = false;
		m_nFlowIdx = 0;
		m_nTargetIdx = 0;//m_cScript.GetMaxCol()-1;
		ClearText();

		nTweenObjCount =0;
		// add on click event
		//this.GetComponent( );
		UIEventListener.Get(BackGroundTex).onClick += OnPanelButtonClick;
		UIEventListener.Get(SkipButton).onClick += OnSkipButtonClick;

		//
	//	GameEventManager.AddEventListener(  StoryStartStageEvent.Name , OnStoryStartStageEvent );


		//PanelManager.Instance.OpenUI( "Panel_Loading");
		//bIsLoading = true;

	}

//	IEnumerator StoryLoading()
//	{
//		// Custom Update Routine which repeats forever
//		do
//		{
//			// wait one frame and continue
//			yield return 1;
//			
//			if ( bIsLoading == false )
//			{
//				// end
//				PanelManager.Instance.CloseUI( "Panel_Loading");
//				yield break;
//			}			
//			
//		} while (true);
//	}

	// Use this for initialization
	void Start () {
		Debug.Log("StoryUIPanel:start");
		//StartCoroutine("StoryLoading");
		// load const stage data
		// 播放  mian BGM
		m_StoryData =ConstDataManager.Instance.GetRow< STAGE_STORY> ( GameDataManager.Instance.nStoryID );
		if( m_StoryData != null )
		{
			GameSystem.PlayBGM ( m_StoryData.n_BGM );
			
			m_cScript = new cTextArray( );
			m_cScript.SetText( m_StoryData.s_CONTEXT );			

		}

		// close prefab face		
		SRW_TextBox pBox = PanelStoryText.GetComponent<SRW_TextBox>();
		if (pBox != null) {
			pBox.ChangeFace( 0 );
		}

		bIsLoading = false;

		// end
		PanelManager.Instance.CloseUI( "Panel_Loading");
	}
	
	public bool IsAllEnd ()
	{
		if( nTweenObjCount>0 )
			return false ;

		SRW_TextBox pBox = PanelStoryText.GetComponent<SRW_TextBox>();
		if (pBox != null) {
			if( pBox.IsEnd() == false ){
				return false;
			}
		}

		return true;
	}

	// Update is called once per frame
	void Update () {
		//	Debug.Log("StoryUIPanel:update");

		// block when animate

		if ( IsAllEnd () == false)
			return; 

		// skip script when end
		if (m_bIsEnd)
			return;

		// move cur flow to target flow
		if( m_nFlowIdx < m_nTargetIdx )
		{
			if( m_cScript != null )
			{
				ParserScript( m_cScript.GetTextLine( m_nFlowIdx ) );
			}

			++ m_nFlowIdx;
		}
	}

	void OnDestroy()
	{
	//	GameEventManager.RemoveEventListener(  StoryStartStageEvent.Name , OnStoryStartStageEvent );
	}

	// Base Panel click
	void OnPanelButtonClick(GameObject go)
	{
		Debug.Log("Back Panel click ");
		if( m_cScript == null  )
			return ;
		// check all tween obj complete
		if ( IsAllEnd () == false)
			return; 
	
		// go to next script
		if( m_nTargetIdx == m_nFlowIdx ){ // only set with curscript complete
			if( ++m_nTargetIdx >= m_cScript.GetMaxCol() )
			{
				m_nTargetIdx = m_cScript.GetMaxCol()-1; // change flow 
				End();
			}
		}
	}

	// on skip click
	void OnSkipButtonClick(GameObject go)
	{
		Debug.Log("Skip Button click ");
	//	if ( IsAllEnd () == false)
	//		return; 
		// Go To End
		End();

	}

	// Game event 
//	public void OnStoryStartStageEvent(GameEvent evt)
//	{
//		Debug.Log ("OnStoryStartStageEvent");
//		// setup global stage =1;
//		//string str = evt.ToString ();
//		//int stageid = int.Parse (str);
//		StoryStartStageEvent Evt = evt as StoryStartStageEvent;
//		if (Evt == null)
//			return;
//
//		GameDataManager.Instance.nStageID = Evt.StageID;
//		GameObject obj = PanelManager.Instance.OpenUI( Panel_StageUI.Name );//"Panel_StageUI"
//		// 回到第0 關
//
//		//GameDataManager.Instance.nStageID = Config.StartStage;  //設定為 第一關
//		// open story panel 
//
//	//		if (obj != null) {
//			PanelManager.Instance.CloseUI (Name);
//	//		}
//	}

	IEnumerator EnterStage( int nStageID )
	{
		GameDataManager.Instance.nStageID = nStageID;

		PanelManager.Instance.OpenUI( "Panel_Loading");
		yield return false;

		PanelManager.Instance.OpenUI( Panel_StageUI.Name );//"Panel_StageUI"
		yield return false;

		PanelManager.Instance.DestoryUI (Name ); // destory this ui will broken this Coroutine soon
		yield return true;
	}

	// end to enter next stage
	public void End()
	{
		if (m_bIsEnd == true)
			return; // avoid sdouble execute

		if( m_bIsEnd == false )
		{
			m_bIsEnd = true;
		}

		Debug.LogFormat("StoryEnd go StartStage{0}" , m_StoryData.n_NEXT_STAGE );


		// start loading ui
		// need a cartan


		//GameDataManager.Instance.nStageID =  m_StoryData.n_NEXT_STAGE;

		StartCoroutine ( EnterStage( m_StoryData.n_NEXT_STAGE ) );

		// go to talk or stage
//		StoryStartStageEvent evt = new StoryStartStageEvent ();
//		evt.StageID = m_StoryData.n_NEXT_STAGE;
//		GameEventManager.DispatchEvent ( evt );
		// GameEventManager.DispatchEventByName( StoryStartStageEvent.Name  , evt );  


		//PanelManager.Instance.DestoryUI (Name );
	}

	public GameObject AddChar( int nCharId , int nPosX , int PosY )
	{
		// check or add
		GameObject obj = null ;

		if( m_idToCharObj.ContainsKey(nCharId) == false )
		{
			obj = ResourcesManager.CreatePrefabGameObj( this.gameObject , "Panel/Panel_StoryUI/Panel_char" );
			if( obj == null )return null;
			CHARS charData = ConstDataManager.Instance.GetRow<CHARS>( nCharId );
			if( charData != null)
			{	
				// charge face text				
				UITexture tex = obj.GetComponentInChildren<UITexture>();
			
				if( tex )
				{
					if(tex != null){
						//	DynamicAssetBundleLoader.LoadTexture(tex,DynamicAssetBundleLoader.SSAssetType.Card, "CARD_" + card.PicName);
						//string texpath = "char/" +charData.s_FILENAME +"_S";
						string url = "Art/char/" + charData.s_FILENAME +"_S";
						//Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
						//Texture t= Resources.Load( url , typeof(Texture) ) as Texture; ;
						tex.mainTexture = Resources.Load( url , typeof(Texture) ) as Texture; ;
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
		PanelStoryText.SetActive( true );
		//ProcessText( 0 ); 

		DataRow row = ConstDataManager.Instance.GetRow("STORY_TEXT", StoryID );
		if( row != null )
		{	

			string content = row.Field<string>( "s_CONTENT");
			SRW_TextBox pBox = PanelStoryText.GetComponent<SRW_TextBox>();
			if( pBox )
			{
				pBox.AddText( content );

			}

//			cTextArray cTxt = new cTextArray( "\n".ToCharArray() , "".ToCharArray() );
//			cTxt.SetText( content );
//			for( int i = 0 ; i < cTxt.GetMaxCol() ; i++ )
//			{
//				cTextArray.CTextLine line = cTxt.GetTextLine( i );
//				foreach( string s in line.m_kTextPool )
//				{
//					if( s.IndexOf("//") >= 0 ) // is common
//						break; // giveup all of after
//					m_cTextList.Add( s );
//				}
//			}
		}
	}
	void ClearText()
	{
		SRW_TextBox pBox = PanelStoryText.GetComponent<SRW_TextBox>();
		if( pBox )
		{
			pBox.ClearText();
			
		}	
	}

	// this should be menthod  of text panel
	// Sys func to parser one line script

	void ParserScript( CTextLine line )
	{
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
				int nBgm = int.Parse( sp1 );
				GameSystem.PlayBGM( nBgm ) ;
				
			}
			else if( s == "END" )
			{
				// no need param
				End();
				return;
			}
		}
	}


}
