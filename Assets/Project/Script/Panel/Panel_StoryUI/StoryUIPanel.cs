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

	private Dictionary<int, Panel_char> m_idToCharObj; // 管理 產生的角色物件 

	public GameObject BackGroundTex;            // 大地圖背景貼圖
	public GameObject PanelStoryText;           // 故事文字匡
	public GameObject SkipButton;           	// 跳過
	public GameObject GridObj;           		// grid obj

	public SRW_TextBox  TalkWindow;           // 故事文字匡


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
		m_idToCharObj = new Dictionary<int, Panel_char>();

		TalkWindow = PanelStoryText.GetComponent<SRW_TextBox>();


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
#if DEBUG && UNITY_EDITOR
	//	GameDataManager.Instance.nStoryID =5;
#endif 

		//PanelManager.Instance.OpenUI( "Panel_Loading");
		//bIsLoading = true;

		if (GridObj != null) {
			if( Config.DRAWGRID == true )
			{
#if DEBUG && UNITY_EDITOR
				for( int i = -10 ; i < 10 ; i++ ){
					for( int j = -10 ; j < 10 ; j++ ){
						string skey = string.Format( "{0},{1}" , i , j );
						Vector3 pos = new Vector3( i * 64 , j *64 , 0.0f ) ;
						GameObject obj  = GridObj.Spawn( BackGroundTex.transform , pos );
						if( obj != null ){
							obj.SetActive( true );
							UILabel lbl = obj.GetComponentInChildren< UILabel >(); 
							if( lbl != null ){
								lbl.text = skey;
							}
							obj.transform.localRotation = new Quaternion(); 
							obj.transform.localScale = new Vector3( 1.0f, 1.0f ,1.0f);
						}
					}
				}
#endif //
			}
			GridObj.SetActive( false );
		}
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
            PanelStoryText.SetActive(false);
        }

		bIsLoading = false;

		// end
		PanelManager.Instance.CloseUI( "Panel_Loading");
	}

    public bool IsAllEnd()
    {
        if (nTweenObjCount > 0)
            return false;

        foreach (KeyValuePair<int, Panel_char> pair in m_idToCharObj) {
            if (pair.Value.IsIdle() == false) {
                return false;
            }
        }

        if (PanelStoryText.activeSelf == true) { 
            SRW_TextBox pBox = PanelStoryText.GetComponent<SRW_TextBox>();
            if (pBox != null) {
                if (pBox.IsEnd() == false) {
                    return false;
                }
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
		if (IsAllEnd () == false) {
			TalkWindow.OnTextBoxClick( TalkWindow.gameObject  );
			return; 
		}
	
		// go to next script
		if( m_nTargetIdx == m_nFlowIdx ){ // only set with curscript complete
            ++m_nTargetIdx;
            if (m_nTargetIdx > m_cScript.GetMaxCol()) // end check
            {
                m_nTargetIdx = m_cScript.GetMaxCol() - 1; // change flow 
                End();
            }
            else {
                
            }
//            else {
//
  //          }

		}
	}

	// on skip click
	void OnSkipButtonClick(GameObject go)
	{
		Debug.Log("Skip Button click ");
        //	if ( IsAllEnd () == false)
        //		return; 
        // Go To End

        GameSystem.BtnSound(1);
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

		GameObject obj = PanelManager.Instance.OpenUI( "Panel_Loading");
		if (obj != null) {
			Panel_Loading ploading = MyTool.GetPanel<Panel_Loading>( obj );
			if( ploading != null )	{
				ploading.ShowStoryName();
			}	
		}

		
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

	public Panel_char AddChar( int nCharId , float fPosX , float fPosY )
	{
        PanelStoryText.SetActive(false);
        // check or add
        //GameObject obj = null ;
        Panel_char pChar = null;
        if ( m_idToCharObj.ContainsKey(nCharId) == false )
		{
            GameObject obj = ResourcesManager.CreatePrefabGameObj( this.gameObject , "Panel/Panel_StoryUI/Panel_char" );
			if( obj == null )return null;
			//CHARS charData = ConstDataManager.Instance.GetRow<CHARS>( nCharId );
			//if( charData != null)
			//{	
			//	// charge face text				
			//	UITexture tex = obj.GetComponentInChildren<UITexture>();
			
			//	if( tex )
			//	{
			//		if(tex != null){
			//			//	DynamicAssetBundleLoader.LoadTexture(tex,DynamicAssetBundleLoader.SSAssetType.Card, "CARD_" + card.PicName);
			//			//string texpath = "char/" +charData.s_FILENAME +"_S";
			//			string url = "Art/char/" + charData.s_FILENAME +"_S";
			//			//Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
			//			//Texture t= Resources.Load( url , typeof(Texture) ) as Texture; ;
			//			tex.mainTexture = Resources.Load( url , typeof(Texture) ) as Texture; ;
			//			//tex.mainTexture = Resources.Load( texpath) as Texture; 
			//			//tex.MakePixelPerfect();
			//		}
			//	}
			//}		
			NGUITools.SetActive( obj , true );			

            pChar = obj.GetComponent<Panel_char>();
            if (pChar != null) {
                pChar.SetFace( nCharId );


                pChar.StartAlpha( 0.0f , 1.0f );

                m_idToCharObj.Add(nCharId, pChar );
            }

   //         TweenAlpha tObj = TweenAlpha.Begin( obj , 1.5f, 1.0f );
			//if( tObj != null ){
			//	tObj.from = 0.0f;
			//	tObj.SetOnFinished( OnTweenNotifyEnd );
			//	nTweenObjCount++;
			//}
		}
		else {
			m_idToCharObj.TryGetValue( nCharId , out pChar);

		}

		// pos
		if(pChar != null )
		{
			Vector3 v = new Vector3( fPosX , fPosY ,0.0f );
            pChar.transform.localPosition = v;
			return pChar;
		}
		return null;
	}

	public void MoveChar( int nCharId , float fPosX , float fPosY )
	{
        PanelStoryText.SetActive(false);
        Panel_char obj ;
		if (m_idToCharObj.TryGetValue (nCharId, out obj) == false) {
			Debug.LogErrorFormat( "store script : MoveChar({0}) to ({1},{2})is null " , nCharId ,fPosX , fPosY );
			return;
		}
        obj.Moveto(fPosX , fPosY );
  //      TweenPosition t = TweenPosition.Begin ( obj , 2.0f , new Vector3( fPosX , fPosY , obj.transform.localPosition.z) ); //直接移動
		//if( t != null )
		//{
		//   t.SetStartToCurrentValue();
		//   t.SetOnFinished( OnTweenNotifyEnd);
		//   nTweenObjCount++;
		//}
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
        PanelStoryText.SetActive(false);
        //      pChar obj = m_idToCharObj[nCharId];
        //if( obj == null )
        //	return;
        Panel_char pChar;

        if (m_idToCharObj.TryGetValue(nCharId, out pChar) == false) {
            return;
        }

        pChar.StartAlpha(1.0f, 0.0f);

  //      TweenAlpha t = TweenAlpha.Begin(pChar, 1.0f , 0.0f  );// close
		//if( t != null)
		//{
		//	t.SetOnFinished( OnTweenNotifyEnd);
		//	nTweenObjCount++;
		//}

        m_idToCharObj.Remove(nCharId);
        //NGUITools.Destroy (obj);
    }

	public void DelAll( int nType)
	{
        PanelStoryText.SetActive(false);
        foreach ( KeyValuePair<int , Panel_char > pair  in m_idToCharObj )
		{
            pair.Value.StartAlpha(1.0f, 0.0f);
            //TweenAlpha.Begin( pair.Value , 1.0f , 0.0f  );// close
            //NGUITools.Destroy ( pair.Value );
        }
		m_idToCharObj.Clear();
	}

	public void GrayChar(  int nCharId  )
	{
        PanelStoryText.SetActive(false );

        if (nCharId == 0) {
			foreach( KeyValuePair<int , Panel_char> pair  in m_idToCharObj )
			{
				//Panel_char p = pair.Value.GetComponent< Panel_char > ();
				if (pair.Value != null )
				{
                    pair.Value.StartGray();

				}
			}
			// gray all
			return ;
		}


        Panel_char obj = m_idToCharObj[nCharId];
		if( obj == null )
			return;

        //Panel_char p2 = obj.GetComponent< Panel_char > ();
        //if( p2 != null )
        //{
         obj.StartGray();			
		//}

	}


	public void AddStoryText( int StoryID )
	{
		PanelStoryText.SetActive( true );
		//ProcessText( 0 ); 

		DataRow row = ConstDataManager.Instance.GetRow("STORY_TEXT", StoryID );
		if( row != null )
		{	

			string content = row.Field<string>( "s_CONTENT");
			// replace   heroine name
			string sText = content.Replace ( "$F" , Config.PlayerFirst ); // replace player name
					sText = sText.Replace ( "$N" , Config.PlayerName ); // replace player name

			TalkWindow.ClearText();
			TalkWindow.AddText( sText , 3  ); // always mode 3
//			SRW_TextBox pBox = PanelStoryText.GetComponent<SRW_TextBox>();
//			if( pBox )
//			{
//				pBox.ClearText();
//
//				pBox.AddText( sText );
//
//			}

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
		TalkWindow.ClearText ();
//		SRW_TextBox pBox = PanelStoryText.GetComponent<SRW_TextBox>();
//		if( pBox )
//		{
//			pBox.ClearText();
//			
//		}	
	}

	// this should be menthod  of text panel
	// Sys func to parser one line script
	void ParserScript( CTextLine line )
	{
		List<cTextFunc> funcList =line.GetFuncList();
		foreach( cTextFunc func in funcList )
		{
            if (func.sFunc == "ADDCHAR" || func.sFunc == "ADDC")
            {
                AddChar(func.I(0), func.F(1) * Config.BigMapTileW, func.F(2) * Config.BigMapTileH);
            }
            else if (func.sFunc == "MOVECHAR" || func.sFunc == "MOVEC")
            {
                MoveChar(func.I(0), func.F(1) * Config.BigMapTileW, func.F(2) * Config.BigMapTileH);
            }
            else if (func.sFunc == "DELCHAR" || func.sFunc == "DELC")
            {
                DelChar(func.I(0), func.I(1));
            }
            else if (func.sFunc == "DELALL")
            {
                DelAll(func.I(0));
            }
            else if (func.sFunc == "GRAYALL")
            {
                GrayChar(0);
            }
            else if (func.sFunc == "DEADCHAR")
            {
                GrayChar( func.I(0) );
            }
            else if (func.sFunc == "TEXT")
            {
                AddStoryText(func.I(0));
            }
            else if (func.sFunc == "BGM")
            {
                GameSystem.PlayBGM(func.I(0));
            }
            else if (func.sFunc == "END")
            {
                // no need param
                End();
                return;
            }
		}
	}

//	void ParserScript( CTextLine line )
//	{
//		List<cTextFunc> funcList =line.GetFuncList();
//		foreach( cTextFunc func in funcList )
//		//for( int i = 0 ; i < line.GetRowNum() ; i++ )
//		{
//			//string s = line.GetString( i ).ToUpper();
//			if( func.sFunc == "ADDCHAR" )
//			//if( s == "ADDCHAR" )
//			{
//			//	string sp = line.GetString( ++i );	if( sp == null ) return; //  null = error
//			//	List<string> lst = cTextArray.GetParamLst( sp );
//
//				// default value			
//			//	int[] array1 = new int[5];
//			//	int j = 0 ; 
//
//			//	foreach( string s2 in lst )
//			//	{
//			//		array1[j++] = int.Parse( s2.Trim( ) );
//
//			//	}
//
//			//	AddChar( array1[0] , array1[1] , array1[2] );
//				AddChar( func.I[0] , func.F[1] , func.F[2] );
//
//			}
//			else if( s == "MOVECHAR" )
//			{
//				string sp = line.GetString( ++i );	if( sp == null ) return; //  null = error
//				List<string> lst = cTextArray.GetParamLst( sp );
//				
//				// default value			
//				int[] array1 = new int[5];
//				int j = 0 ; 
//				
//				foreach( string s2 in lst )
//				{
//					array1[j++] = int.Parse( s2.Trim( ) );
//					
//				}
//				
//				MoveChar( array1[0] , array1[1] , array1[2] );
//			}
//			else if( s == "DELCHAR" )
//			{
//				string sp = line.GetString( ++i );	if( sp == null ) return; //  null = error
//				List<string> lst = cTextArray.GetParamLst( sp );
//				
//				// default value			
//				int[] array1 = new int[5];
//				int j = 0 ; 
//				
//				foreach( string s2 in lst )
//				{
//					array1[j++] = int.Parse( s2.Trim( ) );
//					
//				}
//				
//				DelChar( array1[0] , array1[1] );
//			}
//			else if( s == "DELALL" )
//			{
//				string sp = line.GetString( ++i );	if( sp == null ) return; //  null = error
//				int nType = 0;
//				nType = int.Parse( sp.Trim() );
//				//if( lst.Count > 0 )
//				//	nID = int.Parse( lst[0] );
//				
//				DelAll( nType  );
//				//DelChar( nCharid , nType );
//			}
//			else if( s == "TEXT" )
//			{
//				string sp= line.GetString( ++i );	if( sp == null ) return; //  null = error 
//				//List<string> lst = cTextArray.GetParamLst( sp );
//				
//				// only 1 par default value
//				int nID = 0;
//				nID = int.Parse( sp.Trim() );
//				//if( lst.Count > 0 )
//				//	nID = int.Parse( lst[0] );
//			
//				AddStoryText( nID  );
//			}
//			else if( s == "BGM" )
//			{
//				string sp1 = line.GetString( ++i );	if( sp1 == null ) return; //  null = error
//				int nBgm = int.Parse( sp1 );
//				GameSystem.PlayBGM( nBgm ) ;
//				
//			}
//			else if( s == "END" )
//			{
//				// no need param
//				End();
//				return;
//			}
//		}
//	}


}
