using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;			// for parser string


public class StoryUIPanel : BasicPanel {

	public STORY_DATA	m_StoryData;		// 目前操作的 story data
	private int		m_nFlowIdx;				// 腳本演到哪段
	private int		m_nTargetIdx;			// 
	private cTextArray m_cScript;			// 

	// 
	private int m_nTextIdx;					//  
	private List<string> m_cTextList;		// 

	private Dictionary<int, GameObject> m_idToCharObj; // 

	public GameObject BackGroundTex;
	public GameObject PanelStoryText;

	void Awake(){
		Debug.Log("StoryUIPanel:awake");
		m_StoryData = new STORY_DATA();
		m_idToCharObj = new Dictionary<int, GameObject>();
		m_cTextList = new List<string> ();

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
		m_nFlowIdx = 0;
		m_nTextIdx = 0;
		m_nTargetIdx = 0;//m_cScript.GetMaxCol()-1;
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
		if( m_nFlowIdx < m_nTargetIdx )
		{
			if( m_cScript != null )// 
				ParserScript( m_cScript.GetTextLine( m_nFlowIdx ) );

			m_nFlowIdx++;
		}
	}
	// Base Panel click
	void OnPanelButtonClick(GameObject go)
	{
		Debug.Log("Back Panel click ");
		if( m_cScript == null || m_cTextList == null )
			return ;
		//
		if( m_nTextIdx < (m_cTextList.Count-1) )
		{
			m_nTextIdx++; // change text 
			return ;
		}

		if( ++m_nTargetIdx >= m_cScript.GetMaxCol() )
		{
			m_nTargetIdx = m_cScript.GetMaxCol()-1; // change flow 
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
		TweenPosition.Begin ( obj , 3.0f , new Vector3( nPosX , nPosY , obj.transform.localPosition.z) ); //直接移動
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
		NGUITools.Destroy (obj);


	}
	void ParserScript( cTextArray.CTextLine line )
	{
		m_cTextList.Clear();
		m_nTextIdx = 0 ; // change text 

		for( int i = 0 ; i < line.GetRowNum() ; i++ )
		{
			string s = line.GetString( i ).ToUpper();
			if( s == "ADDCHAR" )
			{
				string sp1 = line.GetString( ++i );	if( sp1 == null ) return; //  null = error
				string sp2 = line.GetString( ++i ); if( sp2 == null ) return;
				string sp3 = line.GetString( ++i ); if( sp3 == null ) return;

				//
					
				int nCharid = int.Parse( sp1 ); // MyClassLibrary.IntParseFast( sp1 ); // more slow
				int nPosX = int.Parse( sp2 );
				int nPosY = int.Parse( sp3 );

				AddChar( nCharid , nPosX , nPosY );

			}
			else if( s == "MOVECHAR" )
			{
				string sp1 = line.GetString( ++i );	if( sp1 == null ) return; //  null = error
				string sp2 = line.GetString( ++i ); if( sp2 == null ) return;
				string sp3 = line.GetString( ++i ); if( sp3 == null ) return;

				int nCharid = int.Parse( sp1 ); // MyClassLibrary.IntParseFast( sp1 ); // more slow
				int nPosX = int.Parse( sp2 );
				int nPosY = int.Parse( sp3 );

				MoveChar( nCharid , nPosX , nPosY );
			}
			else if( s == "DELCHAR" )
			{
				string sp1 = line.GetString( ++i );	if( sp1 == null ) return; //  null = error
				string sp2 = line.GetString( ++i ); if( sp2 == null ) return;
				int nCharid = int.Parse( sp1 ); // MyClassLibrary.IntParseFast( sp1 ); // more slow
				int nType = int.Parse( sp2 );
				DelChar( nCharid , nType );
			}
			else if( s == "TEXT" )
			{
				string sp1 = line.GetString( ++i );	if( sp1 == null ) return; //  null = error
				m_cTextList.Add( sp1 );

			}
			else if( s == "BGM" )
			{
				string sp1 = line.GetString( ++i );	if( sp1 == null ) return; //  null = error
				
			}
			else if( s == "END" )
			{
				
				
			}
		}
	}
}
