using UnityEngine;
using Playcoo.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;



public class GameSystem : MonoBehaviour {

	// 目前在進行那個模式
//	public enum eStageStatus
//	{
//		STORY 	=0 , // 故事中
//		STAGE 	=1 , // 戰場上
//		PREPARE =2 , // 整備
//		TALK 	=3 , // 對話 
//		BATTLE	= 4, // 戰鬥
//	};

	// 操作 ststus
//	public enum eOPStatus
//	{
//		NONE = 0,
//		SEL_ALLY =1,
//		SEL_ENEMY=2,
//	};

	public static bool isApplicationQuit = false;
	
	public static string SystemLogFormat(string log){
		return "<b><color=orange>[sys]" + log + "</color></b>";
	}

	static bool isSystemAwaked = false;

	static public  int m_nCurBGMIdx { set; get; }

	static public bool bFXPlayMode{ set; get; }			// 特效 播放模式 0 - normal

	//===============================
	protected virtual void Awake () 
	{
		// for game sys run once only
		if( isSystemAwaked == true )
			return ;
		isSystemAwaked = true;

		DontDestroyOnLoad(this.gameObject);
		
		// Lock all other device orientation in the beginning
		Screen.orientation = ScreenOrientation.AutoRotation;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		
		//設定背景載入優先度
		Application.backgroundLoadingPriority = ThreadPriority.Normal;
		
		Debug.developerConsoleVisible = true;
		
		// set target frame rate
		#if DEBUG && UNITY_EDITOR
		Application.targetFrameRate = -1;
		#else
		Application.targetFrameRate = 35;
		#endif
		
		//初始化音效系統
		Debug.Log(SystemLogFormat("開始初始化 Manager: " + typeof(ResourcesManager).ToString()));
		AudioManager.Instance.Initial(ResourcesManager.LoadClip);
		
		//初始化 ConstData 系統
		Debug.Log(SystemLogFormat("開始初始化 Manager: " + typeof(ConstDataManager).ToString()));
		//ConstData只讀有註冊並設定lazyMode
		//		ConstDataManager.Instance.useUnregistedTables = false;
		ConstDataManager.Instance.isLazyMode = false;
		StartCoroutine(ConstDataManager.Instance.ReadDataStreaming("pcz/", Config.COMMON_DATA_NAMES));
	
		//初始化 PanelManager
		Debug.Log(SystemLogFormat("開始初始化 Manager: " + typeof(PanelManager).ToString()));
		PanelManager.Instance.Initial(
			"Panel_Blocker",
			new Dictionary<string, string[]>(){
				{ "5Main", new string[]{ "Panel_MainUI", /*"Panel_BattleList", */ } },
			}
		);
		// 資料管理器
		Debug.Log(SystemLogFormat("開始初始化 Manager: " + typeof(GameDataManager).ToString()));
		GameDataManager.Instance.Initial (0);

		Debug.Log(SystemLogFormat("開始初始化 Manager: " + typeof(GameScene).ToString()));
		GameScene.Instance.Initial ();

		Debug.Log(SystemLogFormat("開始初始化 Manager: " + typeof(cCMD).ToString()));
		cCMD.Instance.Initial ();

		Debug.Log(SystemLogFormat("開始初始化 Manager: " + typeof(BattleManager).ToString()));
		BattleManager.Instance.Initial ();

		Debug.Log(SystemLogFormat("開始初始化 Manager: " + typeof(ActionManager).ToString()));
		ActionManager.Instance.Initial ();

		Debug.Log(SystemLogFormat("開始初始化 Manager: " + typeof(GameScene).ToString()));
		GameScene.Instance.Initial ();

		GameDataManager.Instance.nStageID = 1;	// first stage default
		// Cal cache value
		UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);	
		if (mRoot != null) {
			MyTool.fScnRatio = (float)mRoot.activeHeight / Screen.height;
		}

#if DEBUG && UNITY_EDITOR
		GameDataManager.Instance.nStageID  =3;
#endif
		bFXPlayMode = true;
	}
	
	void Start()
	{
		if(Application.loadedLevel == 1)
		{
			ResourcesManager.LoadLevel("5Main");
		}
		//ResourcesManager.LoadLevel(Application.loadedLevel + 1);
	}
	
	void OnLevelWasLoaded(int levelIndex)
	{
		Debug.Log("OnLevelWasLoaded: " + Application.loadedLevelName);
	}
	
	// ios does not dispatch
	void OnApplicationQuit()
	{
		#if DEBUG
		Debug.Log("OnApplicationQuit");
		#endif
		isApplicationQuit = true;
	}

	// 目前操作
	public static void ShakeCamera( ){
		if (UICamera.currentCamera != null) {
			TweenShake tw = TweenShake.Begin(UICamera.currentCamera.gameObject, 0.2f , 100 );
		}
	}


	public static GameObject PlayFX( GameObject go , int nFxid )
	{
		if (go == null || nFxid ==0 )
			return null;

		if (bFXPlayMode == false)
			return null;

		FX fx =  ConstDataManager.Instance.GetRow<FX>( nFxid );
		if (fx != null) {
			// play wav sound
			if( fx.s_SOUND !="0" && fx.s_SOUND !="null" ){
				PlaySound( fx.s_SOUND );
			}
			// shake camera
			if( fx.n_SHAKECAMERA != 0 ){
				//UICamera.
				//Camera.mainCamera;
				ShakeCamera();

			}

			// FX obj
			GameObject obj = null ;
			// play on tile
			if( fx.n_TAG == 3 )
			{
				obj = PlayFX( Panel_StageUI.Instance.TilePlaneObj , fx.s_FILENAME );
				if( obj != null ){
					obj.transform.localPosition = go.transform.localPosition;
				}
			}
			else{
				obj = PlayFX( go , fx.s_FILENAME );
			}


			if( obj != null ){
				// rotate
				if( (fx.f_ROTX != 0.0f) || (fx.f_ROTY != 0.0f) ){
					obj.transform.localRotation = Quaternion.Euler( fx.f_ROTX , fx.f_ROTY , 0.0f );
				}
				// scale
				if( fx.f_SACLE > 0.0f ){
					ParticleSystem ps = obj.GetComponent<ParticleSystem>();
					if( ps != null ){						
							ps.startSize *=  fx.f_SACLE;						
					}
					ParticleSystem[] pss = obj.GetComponentsInChildren<ParticleSystem>();
					foreach( ParticleSystem ps2 in pss )
					{
						ps2.startSize *=  fx.f_SACLE;
					}
				}
				if( fx.f_OFFSETY != 0.0f ){
					Vector3 v = obj.transform.localPosition;
					v.y += fx.f_OFFSETY;
					obj.transform.localPosition = v;
				}


			}

			return obj;
		}
		return null;
	}


	public static GameObject PlayFX( GameObject go , string name , string sortLayer="FX")//
	{
		if (go == null)
			return null;

		if (bFXPlayMode == false)
			return null;
			 
		if (string.IsNullOrEmpty (name)) {
			return null;
		}

		string path = "FX/Cartoon FX/" + name;

		GameObject instance = ResourcesManager.CreatePrefabGameObj ( go ,path );
		if (instance == null) {
			Debug.LogErrorFormat( " PlayFX fail can't load file {0}",path );
			return null;
		}


		ParticleSystem ps =instance.GetComponent< ParticleSystem>();
		if (ps!= null) {
		}
		SetParticleRenderLayer ( instance ,sortLayer  );
//		ParticleSystemRenderer psr =instance.GetComponent< ParticleSystemRenderer>();
//		if (psr != null) {
//			psr.sortingLayerName =sortLayer;
//		}
//		// for child
//		ParticleSystemRenderer[] psrs = instance.GetComponentsInChildren<ParticleSystemRenderer>();
//		foreach (ParticleSystemRenderer psr2 in psrs) {
//			psr2.sortingLayerName = sortLayer;
//		}

		//AutoParticleQueue q = instance.AddComponent <AutoParticleQueue>();
		//q.se

		//check auto destory
		CFX_AutoDestructShuriken des = instance.GetComponent< CFX_AutoDestructShuriken > ();

		if( des == null  ){
			//Debug.LogErrorFormat( " particle won't auto destory at {0}",name );
			instance.AddComponent< CFX_AutoDestructShuriken >(); // auto destory
		}

		// MAKE SURE not double del
		killParticle kill = instance.GetComponent< killParticle > ();
		if (kill != null && (kill.enabled==true) ) {
			Debug.LogErrorFormat( " particle have killParticle component with enabled {0}",name );

			//kill.gameObject.SetActive( false );
			kill.enabled = false;
		}


		return instance;
	}
	public static void SetParticleRenderLayer( GameObject instance , string sortLayer="FX" )
	{
		if (instance == null)
			return;
		// change layer
		ParticleSystemRenderer psr =instance.GetComponent< ParticleSystemRenderer>();
		if (psr != null) {
			psr.sortingLayerName =sortLayer;


		}
		// for child
		ParticleSystemRenderer[] psrs = instance.GetComponentsInChildren<ParticleSystemRenderer>();
		foreach (ParticleSystemRenderer psr2 in psrs) {
			psr2.sortingLayerName = sortLayer;
		}
	}


	public static void PlaySound( string strFile )
	{
		string audioPath = ResourcesManager.GetAudioClipPath( AudioChannelType.SoundFX , strFile );
		AudioManager.Instance.Play( AudioChannelType.SoundFX ,  audioPath );
	}

	public static void PlayBGM( int nBGMIdx )
	{
		if( nBGMIdx <=0 )
			AudioManager.Instance.Stop(AudioChannelType.BGM);

		if (m_nCurBGMIdx == nBGMIdx)
			return;

		m_nCurBGMIdx = nBGMIdx;		// record current bgm
		if( m_nCurBGMIdx == 0 )
			return ;

		// 播放  mian BGM
		BGM bgm =  ConstDataManager.Instance.GetRow<BGM>( nBGMIdx  );
		if( bgm!= null )
		{
			string strFile = bgm.s_FILENAME ;
			if( !String.IsNullOrEmpty( strFile ))
			{
				string audioPath = ResourcesManager.GetAudioClipPath( AudioChannelType.BGM , strFile );
				AudioManager.Instance.Play( AudioChannelType.BGM ,  audioPath );				
			}
		}


//		DataRow row = ConstDataManager.Instance.GetRow("BGM", nBGMIdx );
//		if( row != null )
//		{
//			string strFile = row.Field< string >("s_FILENAME");
//			if( !String.IsNullOrEmpty( strFile ))
//			{
//				string audioPath = ResourcesManager.GetAudioClipPath( AudioChannelType.BGM ,  strFile );
//				AudioManager.Instance.Play( AudioChannelType.BGM ,  audioPath );				
//
//				m_nCurBGMIdx = nBGMIdx;		// record current bgm
//				return ;
//			}
//		}
	}

	public static Panel_CheckBox OpenCheckBox()
	{
		return MyTool.GetPanel<Panel_CheckBox>( PanelManager.Instance.OpenUI ( Panel_CheckBox.Name ) );
	}



	public static void TalkEvent( int nTalkID )
	{
		GameDataManager.Instance.nTalkID = 0; // set to 0 first to avoid panel_talk awake->enable set script

		// start talk UI
		Panel_Talk pTalk = MyTool.GetPanel<Panel_Talk>(  PanelManager.Instance.OpenUI( Panel_Talk.Name ) );
		if (pTalk != null) {
			pTalk.SetScript( nTalkID ); 
		}
		GameDataManager.Instance.nTalkID = nTalkID;

	}
//	public static GameObject CreatePrefabGameObj( GameObject parent , string sPrefabPath )
//	{
//		GameObject preObj = Resources.Load( sPrefabPath ) as GameObject;
//		if (preObj != null) {
//			return  NGUITools.AddChild ( parent, preObj);
//		}
//		return null;
//	}

	public static string GetTalkText(int nSayID )
	{
		DataRow row = ConstDataManager.Instance.GetRow ( "TALK_TEXT" , nSayID );
		if( row != null )
		{
			string s = row.Field<string>( "s_CONTENT");
			return s;
		}
		
		return null;	
	}



}
