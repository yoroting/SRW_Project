using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;			// for parser string

public class Panel_Talk : MonoBehaviour {
	public const string Name = "Panel_Talk";

	public GameObject Tex_BackGround;
	public GameObject TalkWindow_Up;
	public GameObject TalkWindow_Down;
	public GameObject Skip_Button;
	//public GameObject StartButton;
	private Dictionary<int, GameObject> m_idToObj; // 管理 產生的 Prefab 物件 

	STAGE_TALK m_cStageTalk;				// talk data class


	private int m_nTalkIdx;					// 文字目前在哪一行
	//private List<string> m_cTextList;		// 內容集合


	private int 	  m_nScriptIdx;			// cur script index
	private cTextArray m_cScript;			// 腳本集合

	// script pause;
	bool m_bClickScript;

	// tween check
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
		m_idToObj = new Dictionary<int, GameObject> ();
	//	m_cTextList = new List<string> ();
		m_cScript = new cTextArray ();

		nTweenObjCount = 0;

		UIEventListener.Get(this.gameObject).onClick += OnPanelClick; // for trig next line
		UIEventListener.Get(Skip_Button).onClick += OnSkipClick; // for trig next line


		// for fast debug 
		ConstDataManager.Instance.isLazyMode = false;
		StartCoroutine(ConstDataManager.Instance.ReadDataStreaming("pcz/", Config.COMMON_DATA_NAMES));

	}
	// Use this for initialization
	void Start () {
//		TalkWindow_Up.SetActive (false);
//		TalkWindow_Down.SetActive (false);
		SetScript ( GameDataManager.Instance.nTalkID ); 
	}
	
	// Update is called once per frame
	void Update () {
		if (IsAllEnd () == false)
			return;
		// prcess script
		if (m_bClickScript) {
			NextLine ();
		}
	}

	// Base Panel click
	void OnPanelClick(GameObject go)
	{
		if (IsAllEnd())
		{
			m_bClickScript = true; // go next script
		}
	}
	void OnSkipClick(GameObject go)
	{
		//if (IsAllEnd())
		{
			m_bClickScript = true; // go next script
		}
		EndTalk();
	}

	// close talk panel
	void EndTalk()
	{
		PanelManager.Instance.CloseUI( Panel_Talk.Name );

	}

	public GameObject SelTextBoxObjByType( int nType )
	{
		GameObject obj;
		if (m_idToObj.ContainsKey (nType) == false) {
			obj = ResourcesManager.CreatePrefabGameObj (this.gameObject, "Prefab/SRW_TEXTBOX");
			if( obj )
			{
				// insert to map
				NGUITools.SetActive( obj , true );
				
				m_idToObj.Add( nType , obj );

				// setup Type
				SRW_TextBox boxobj =  obj.GetComponent<SRW_TextBox>( );
				if( boxobj )
				{
					boxobj.ChangeLayout( nType );
				}
			}

		}
		else {
			m_idToObj.TryGetValue( nType , out obj  );
		}

//		if( obj != null )
//			return obj.GetComponent<SRW_TextBox>( );

//		if (nType == 0) {
//			return TalkWindow_Up.GetComponent<SRW_TextBox>( );
			//return TalkWindow_Up;
//		}
//		else if (nType == 1) {
//			return TalkWindow_Down.GetComponent<SRW_TextBox>( );
//		}

		return obj;
	}
//	public void SetTextBoxActive( int nType , bool bActive )
//	{
//		if (nType == 0) {
//			TalkWindow_Up.SetActive( bActive );
//		}
//		else if (nType == 1) {
//			TalkWindow_Down.SetActive( bActive );
//		}
//	}

	public void SetScript( int nScriptID )
	{
		m_nScriptIdx = 0; // current execute script

		 m_cStageTalk = ConstDataManager.Instance.GetRow<STAGE_TALK> ( nScriptID );
		if (m_cStageTalk == null)
			return;

		// change Back Tex
		if (m_cStageTalk.n_SCENE_ID > 0) {
			// load tex of sceneID

		}

		// change BGM
		if (m_cStageTalk.n_TALK_BGM > 0) {
			GameSystem.PlayBGM( m_cStageTalk.n_TALK_BGM );
		}


		m_cScript.SetText ( m_cStageTalk.s_CONTEXT );
		// for test
		//m_cScript.SetText( "SETCHAR(0,2);SAY(0,1)\nSETCHAR(1,1);SAY(1,2)\nSAY(0,3)\nSAY(1,4)\nSAY(0,5)\nCLOSE(0,0)\nSAY(1,6)\nSETCHAR(0,20);SAY(0,7)\nSAY(1,8)\nSAY(1,9)");
		//m_cScript.SetText( "SETCHAR(1,1);SAY(1,9)");
		//m_cScript.SetText( "SAY(0,3)");
		//m_cScript.SetText( "SAY(1,9)\nCLOSE(1,0)");
		// need get script for const data
		NextLine();
	}

	// script go next line
	public void NextLine()
	{
		if (m_nScriptIdx >= m_cScript.GetMaxCol ())
		{
			EndTalk();
			return;
		}

		ParserScript ( m_cScript.GetTextLine( m_nScriptIdx++ )  );
		m_bClickScript = false;
	}


	void ParserScript( CTextLine line )
	{
		//m_cTextList.Clear();
		//m_nTextIdx = 0 ; // change text 
		
		for (int i = 0; i < line.GetRowNum(); i++) {
			string s = line.GetString (i).ToUpper ();
			if (s == "SAY") {
				string sp = line.GetString (++i); // get parameter
				if (sp == null)
					return; //  null = error
				List<string> lst = cTextArray.GetParamLst (sp); // 
				
				// default value			
				int[] array1 = new int[2]{ 0, 0};
				int j = 0 ; 
				foreach (string s2 in lst) {
					array1 [j++] = int.Parse (s2.Trim ());
					
				}
				
				// AddChar (array1 [0], array1 [1], array1 [2]);
				Say( array1 [0], array1 [1] );
				
			}
			else if (s == "SETCHAR") {
				string sp = line.GetString (++i); // get parameter
				if (sp == null)
					return; //  null = error
				List<string> lst = cTextArray.GetParamLst (sp); // 
				
				// default value			
				int[] array1 = new int[2]{ 0, 0 };
				int j = 0 ; 
				foreach (string s2 in lst) {
					array1 [j++] = int.Parse (s2.Trim ());
					
				}

				SetChar( array1 [0], array1 [1] );
			}
			else if (s == "SHOCK") {
				string sp = line.GetString (++i); // get parameter
				if (sp == null)
					return; //  null = error
				List<string> lst = cTextArray.GetParamLst (sp); // 
				
				// default value			
				int[] array1 = new int[2]{ 0, 0 };
				int j = 0 ; 
				foreach (string s2 in lst) {
					array1 [j++] = int.Parse (s2.Trim ());
					
				}
				
				SetShock( array1 [0], array1 [1] );

			}
			else if( s == "BACK") 
			{

			}
			else if( s == "CLOSE") 
			{
				string sp = line.GetString (++i); // get parameter
				if (sp == null)
					return; //  null = error
				List<string> lst = cTextArray.GetParamLst (sp); // 
				// default value			
				int[] array1 = new int[2]{ 0, 0 };
				int j = 0 ; 
				foreach (string s2 in lst) {
					array1 [j++] = int.Parse (s2.Trim ());
					
				}
				
				CloseBox( array1 [0], array1 [1] );

			}
		}


	}

	public bool IsAllEnd()
	{
		// check both box is end
		foreach( KeyValuePair<int , GameObject > pair in m_idToObj )
		{
			GameObject obj = pair.Value;
			if (obj != null) {
				SRW_TextBox pBox = obj.GetComponent<SRW_TextBox>();
				if( pBox && pBox.IsEnd() == false )
				{
					return false;
				}
			}
		}

		// cehck all tween is end
//		SRW_TextBox obj1 = SelTextBoxObjByType (1) ;
//		if (obj1 != null) {
//			if( obj1.IsEnd() == false )
//				return false;
//		}

		return true;
	}

	public void Say( int nType , int nSayTextID )
	{
	//	SetTextBoxActive ( nType , true ); // need active first to awake() to do some thing

		GameObject obj = SelTextBoxObjByType (nType) ;
		if (obj == null)
			return;

		// get Text form talk_text
		string s = GameSystem.GetTalkText ( nSayTextID );
		SRW_TextBox pBox = obj.GetComponent<SRW_TextBox>();
		if (pBox) {
			pBox.AddText (s);
		}
	}

	public void SetChar( int nType , int nCharID )
	{
		//SetTextBoxActive ( nType , true ); // need active first to awake() to do some thing
		
		//SRW_TextBox obj = SelTextBoxObjByType (nType) ;
		GameObject obj = SelTextBoxObjByType (nType) ;
		if (obj) {
			// get Text form talk_text
			SRW_TextBox pBox = obj.GetComponent<SRW_TextBox> ();
			if (pBox) {
				pBox.ChangeFace (nCharID);
			}
		}
		
	}
	public void SetShock( int nType , int nShockType )
	{
		
		
	}
	public void CloseBox( int nType , int nCloseType )
	{
	//	SRW_TextBox obj = SelTextBoxObjByType (nType);
	//	SetTextBoxActive ( nType , false );
		GameObject obj = SelTextBoxObjByType (nType) ;
		if (obj) {
			NGUITools.Destroy( obj );
			m_idToObj.Remove( nType );
			// get Text form talk_text
			//SRW_TextBox pBox = obj.GetComponent<SRW_TextBox> ();
			//if (pBox) {
				//pBox.ChangeFace (nCharID);

			//}
		}
	}
	// widget func


}
