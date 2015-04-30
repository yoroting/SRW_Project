using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;			// for parser string


public class StoryUIPanel : MonoBehaviour {

	public STORY_DATA	m_StoryData;		// 目前操作的 story data
	private int		m_nFlowIdx;				// 腳本演到哪段
	private int		m_nTargetIdx;			// 
	private cTextArray m_cScript;			// 

	private Dictionary<int, GameObject> m_idToCharObj; // 

	public GameObject BackGroundTex;

	void Awake(){
		Debug.Log("StoryUIPanel:awake");
		m_StoryData = new STORY_DATA();
		m_idToCharObj = new Dictionary<int, GameObject>();

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
			// 
			ParserScript( m_cScript.GetTextLine( m_nFlowIdx ) );

			m_nFlowIdx++;
		}
	}
	// Base Panel click
	void OnPanelButtonClick(GameObject go)
	{
		Debug.Log("Back Panel click ");
		if( ++m_nTargetIdx >= m_cScript.GetMaxCol() )
		{
			m_nTargetIdx = m_cScript.GetMaxCol()-1;
		}

	}


	public GameObject AddChar( int nCharId , int nPosX , int PosY )
	{
		// check or add
		GameObject obj = null ;

		if( m_idToCharObj.ContainsKey(nCharId) == false )
		{
			obj = GameSystem.CreateCharacterGameObj( this.gameObject );
			if( obj != null ){
				m_idToCharObj.Add( nCharId , obj );
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

	public void MoveChar( int nCharId , int nPosX , int PosY )
	{
		GameObject obj = m_idToCharObj[nCharId];
		if( obj == null )
			return;
		//GetComponent<TweenPosition>().Reset();
		//obj.GetComponent( );


	}
	public void DelChar( int nCharId , int nType)
	{
	}
	void ParserScript( cTextArray.CTextLine line )
	{

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
