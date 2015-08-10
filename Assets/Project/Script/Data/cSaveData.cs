using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using JsonFx.Json;
using System.ComponentModel;


[Serializable][JsonName("buff")]
public class cBuffSaveData{

	[JsonName("id")] [DefaultValue(0)]public int nID ;
	[JsonName("time")] [DefaultValue(0)]public int nTime ;			//還有幾回合 
	[JsonName("num")] [DefaultValue(0)]public int nNum ;			//疊幾層了	
	
	[JsonName("cast")] [DefaultValue(0)]public int nCastIdent ;		// record castident
	[JsonName("target")] [DefaultValue(0)]public int nTargetIdent ;		// record targetident
	[JsonName("skillid")] [DefaultValue(0)]public int nSkillID ;		// which skill cast this buff, for fast remove to ensure no bug
	public cBuffSaveData(){}

	public cBuffSaveData( cBuffData buff ){
		nID 	= buff.nID;
		nTime 	= buff.nTime;
		nNum 	= buff.nNum;
		nCastIdent   = buff.nCastIdent;
		nTargetIdent = buff.nTargetIdent;
		nSkillID = buff.nSkillID;
	}

}

[Serializable][JsonName("unit")]
public class cUnitSaveData{
	[JsonName("id")] [DefaultValue(0)]public int n_Ident;
	[JsonName("cid")] [DefaultValue(0)] public int n_CharID;

	[JsonName("camp")] [DefaultValue(_CAMP._PLAYER )] public _CAMP eCampID;
	[JsonName("lv")][DefaultValue(0)] 	public int n_Lv;
	[JsonName("exp")][DefaultValue(0)] 	public int n_EXP;
	[JsonName("x")][DefaultValue(0)]   	public int n_X;
	[JsonName("y")][DefaultValue(0)]	public int n_Y;
	
	[JsonName("hp")][DefaultValue(0)]	public int n_HP;
	[JsonName("mp")][DefaultValue(0)]	public int n_MP;
	[JsonName("sp")][DefaultValue(0)]	public int n_SP;
	[JsonName("def")][DefaultValue(0)]	public int n_DEF;

	[JsonName("action")][DefaultValue(0)]	public int nActionTime;
	[JsonName("leader")][DefaultValue(0)] public int n_LeaderIdent;	// follow leader
	[JsonName("bornx")][DefaultValue(0)]	public int n_BornX;			// born Pox
	[JsonName("borny")][DefaultValue(0)]	public int n_BornY;
	//====data pool
	[JsonName("actsch")]				public int [] nActSch;		// current use 
	[JsonName("items")]					public int [] Items;		// current items 
	// buff pool
	[JsonName("school")]				public Dictionary< string , int > School;		// current school 
	[JsonName("buffs")]					public List< cBuffSaveData> Buffs;		// current buffs
	//==== AI
	[JsonName("sai")][DefaultValue(_AI_SEARCH._NORMAL )]	public _AI_SEARCH eSearchAI=_AI_SEARCH._NORMAL ;		// current use 
	[JsonName("cai")][DefaultValue(_AI_COMBO._NORMAL )]	public _AI_COMBO  eComboAI=_AI_COMBO._NORMAL ;		// current use 
	[JsonName("aitar")][DefaultValue(0 )]	public 	int nAITarget = 0 ;		// current use 
	[JsonName("aix")][DefaultValue(0 )]		public 	int nAIX = 0 ;		// current use 
	[JsonName("aiy")][DefaultValue(0 )]		public 	int nAIY = 0 ;		// current use 

	public cUnitSaveData(){}

	public void SetData( cUnitData data ){
		n_Ident 	= data.n_Ident;
		n_CharID = data.n_CharID;
		eCampID = data.eCampID;
		n_Lv = data.n_Lv;
		n_EXP = data.n_EXP;
		n_X = data.n_X;
		n_Y = data.n_Y;
		n_HP = data.n_HP;
		n_MP = data.n_MP;
		n_SP = data.n_SP;
		n_DEF = data.n_DEF;
		n_LeaderIdent = data.n_LeaderIdent;
		n_BornX = data.n_BornX;
		n_BornY = data.n_BornY;

		nActSch = data.nActSch;
		Items = data.Items;

		nActionTime = data.nActionTime;

		School = MyTool.ConvetToStringInt ( data.SchoolPool );  // unit school pool
		Buffs = data.Buffs.ExportSavePool ();					// unit buff pool

		eSearchAI = data.eSearchAI;
		eComboAI = data.eComboAI;
		nAITarget = data.n_AITarget;
		nAIX = data.n_AIX;
		nAIY = data.n_AIY;
	}

}
// 不斷調整後，SAVEDATA 變成了一個工具用物件

[Serializable][JsonName("save")]
public class cSaveData{
	[JsonName("ver")] [DefaultValue(1)] public int nVersion=1;
	[JsonName("idx")] [DefaultValue(0)] public int n_IDX;
	[JsonName("story")] [DefaultValue(0)] public int n_StoryID;
	[JsonName("stage")] [DefaultValue(0)] public int n_StageID;

	[JsonName("active")] [DefaultValue( _CAMP._PLAYER )] public _CAMP e_Camp ;
	[JsonName("round")] [DefaultValue(0)] public int n_Round;
	[JsonName("money")] [DefaultValue(0)] public int n_Money;
	[JsonName("stars")] [DefaultValue(0)] public int n_Stars;			//熟練度

	[JsonName("phase")] [DefaultValue(_SAVE_PHASE._MAINTEN)] public _SAVE_PHASE ePhase = _SAVE_PHASE._MAINTEN;			//  0 - 整備 , 1-戰場上 , 2- sys
//	 string sFileName;

//	 public cSaveData( int nIdx )
//	 {
//		sFileName = "savedata" + nIdx.ToString() +".dat";
//
//		CharPool = new Dictionary< int ,cUnitBaseData > ();
//		ItemPool = new List<int> ();
//	 }
	[JsonName("pbgm")] [DefaultValue(0)] public int nPlayerBGM;
	[JsonName("ebgm")] [DefaultValue(0)] public int nEnemyBGM;
	[JsonName("fbgm")] [DefaultValue(0)] public int nFriendBGM;


	[JsonName("spool")] public List< cUnitSaveData > 			StoragePool;		// 倉庫腳色
	[JsonName("cpool")] public List< cUnitSaveData > 			CharPool;
	[JsonName("ipool")] public List<int>						ItemPool;
	[JsonName("importpool")] public List<int>					ImportEventPool;   // 已完成的重要事件列表

	// stage special info
	[JsonName("evtdonepool")] public Dictionary<string,int>		EvtDonePool;   // 已完成的事件列表

//	[JsonName("evtcheckpool")] public List<int>					EvtCheckPool;   // event can run
//	[JsonName("evtwaitpool")] public List<int>					EvtWaitingPool;   // 已完成的重要事件列表

	[JsonName("grouppool")] public Dictionary< string , int >		GroupPool;   //  group event pool

			
	static bool 	bIsLoading;													// don't public to avoid recprd this
	public static bool		IsLoading(){ return bIsLoading;	 }
	public static void		SetLoading( bool b){  bIsLoading = b; }


	// write data to save
	public void SetData( int nIdx , _SAVE_PHASE phase )
	{
		n_IDX = nIdx;

		//把所有要記錄的都寫在這
		n_StoryID = GameDataManager.Instance.nStoryID;
		n_StageID = GameDataManager.Instance.nStageID;
		n_Round = GameDataManager.Instance.nRound;
		e_Camp = GameDataManager.Instance.nActiveCamp;
		n_Money = GameDataManager.Instance.nMoney;
		n_Stars = GameDataManager.Instance.nStars;

		nPlayerBGM = GameDataManager.Instance.nPlayerBGM;   //我方
		nEnemyBGM  = GameDataManager.Instance.nEnemyBGM;	 // 敵方
		nFriendBGM = GameDataManager.Instance.nFriendBGM;	// 友方


		ItemPool = GameDataManager.Instance.ItemPool;			// item list
		ImportEventPool = GameDataManager.Instance.ImportEventPool;
		StoragePool = GameDataManager.Instance.ExportStoragePool();

		ePhase = phase;

		// save during mainta
		if (ePhase == _SAVE_PHASE._MAINTEN ) {

		}
		// save during stage
		else if (ePhase == _SAVE_PHASE._STAGE ) {
			// event done pool
		//	EvtDonePool = GameDataManager.Instance.EvtDonePool;
			EvtDonePool = MyTool.ConvetToStringInt( GameDataManager.Instance.EvtDonePool );
//			EvtDonePool = new Dictionary<string,int>();
//			foreach( KeyValuePair< int , int > pair in GameDataManager.Instance.EvtDonePool)
//			{
//				EvtDonePool.Add( pair.Key.ToString() , pair.Value );
//			}
			//EvtCheckPool = Panel_StageUI.Instance.evt
			// group pool
			GroupPool   = MyTool.ConvetToStringInt( GameDataManager.Instance.GroupPool );
			// unit pool
			CharPool = GameDataManager.Instance.ExportSavePool();
		}
	}

	//將遊戲還原到 紀錄的狀態
	public void RestoreData( _SAVE_PHASE phase )
	{
		// clear data

//		GameDataManager.Instance.SaveData = this; // for startcoror


		GameDataManager.Instance.StoragePool.Clear();
		GameDataManager.Instance.UnitPool.Clear();


		// reset data
		GameDataManager.Instance.nStoryID = n_StoryID;
		GameDataManager.Instance.nStageID = n_StageID;
		GameDataManager.Instance.nRound   = n_Round;
		GameDataManager.Instance.nActiveCamp = e_Camp ;
		GameDataManager.Instance.nMoney = n_Money;
		GameDataManager.Instance.nStars = n_Stars;

		// need set after stage load
//		if( nPlayerBGM > 0 )
//			GameDataManager.Instance.nPlayerBGM = nPlayerBGM ;   //我方
//		if( nEnemyBGM > 0 )
//			GameDataManager.Instance.nEnemyBGM  = nEnemyBGM;	 // 敵方
//		if( nFriendBGM > 0 )
//			GameDataManager.Instance.nFriendBGM = nFriendBGM;	// 友方


		GameDataManager.Instance.ItemPool = ItemPool ;			// item list
		GameDataManager.Instance.ImportEventPool = ImportEventPool;
		GameDataManager.Instance.ImportStoragePool( StoragePool );
		// 由phase 決定目前該切到哪個場僅. this should need a 

		// restore mainta

		//StartCoroutine(  cSaveData.SaveLoading( this  ) ); // need a mono behacior

		if (phase == _SAVE_PHASE._MAINTEN) {
			Panel_Mainten panel = MyTool.GetPanel< Panel_Mainten >( PanelManager.Instance.JustGetUI( Panel_Mainten.Name )  );
			if( panel != null ){
				panel.LoadSaveGame( this );
			}
		}
		// restore to stage
		else if (phase == _SAVE_PHASE._STAGE) {
			if( Panel_StageUI.Instance )
				Panel_StageUI.Instance.LoadSaveGame( this );
//			if( this.ePhase ==  _SAVE_PHASE._MAINTEN )
//			{
//				// free stage 
//
//				// open main tenUI
//
//			}
//			else if( this.ePhase ==  _SAVE_PHASE._STAGE )
//			{
//
//				 // need coror
//				Panel_StageUI.Instance.RestoreBySaveData();
//
//			}
		}
		else if(phase == _SAVE_PHASE._STARTUP )
		{
			MainUIPanel panel = MyTool.GetPanel< MainUIPanel >( PanelManager.Instance.JustGetUI( MainUIPanel.Name ) );
			if( panel != null ){
				panel.LoadSaveGame( this );
			}
		}

	}

	static public string GetKey( int Idx )
	{
		return "save" + Idx.ToString() ;
	}

	static public bool Load( int Idx , _SAVE_PHASE phase )
	{
		if (IsLoading ())
			return false;
		SetLoading (true);

		string sKeyName = GetKey( Idx );
		string sJson = PlayerPrefs.GetString ( sKeyName , "" );
		if (string.IsNullOrEmpty (sJson))
			return false;
		// ---- DESERIALIZATION ----
		
		JsonReaderSettings readerSettings = new JsonReaderSettings();
		readerSettings.TypeHintName = "__type";
		
		JsonReader reader = new JsonReader(sJson, readerSettings);
		
		cSaveData save = (cSaveData)reader.Deserialize ( typeof(cSaveData) );
		save.RestoreData ( phase );
		//parameters = (Dictionary<string, object>)reader.Deserialize();
		return true;
	}

	static public bool Save( int nID ,  _SAVE_PHASE phase  )
	{
		cSaveData save = new cSaveData ();
		save.SetData ( nID , phase );

		string sKeyName = GetKey( nID );
		// ---- SERIALIZATION ----
		
		JsonWriterSettings writerSettings = new JsonWriterSettings();
		writerSettings.TypeHintName = "__type";
		
		StringBuilder json = new StringBuilder();
		JsonWriter writer = new JsonWriter(json, writerSettings);
		writer.Write(save);

		PlayerPrefs.SetString (sKeyName , json.ToString() );
		PlayerPrefs.Save ();
		return true;
	}


	static public string LoadSimpleInfo( int Idx )
	{
		string sKeyName = GetKey( Idx );
		string sJson = PlayerPrefs.GetString ( sKeyName , "" );
		if (string.IsNullOrEmpty (sJson))
			return "NoData";
		// ---- DESERIALIZATION ----
		
		JsonReaderSettings readerSettings = new JsonReaderSettings();
		readerSettings.TypeHintName = "__type";
		
		JsonReader reader = new JsonReader(sJson, readerSettings);
		
		cSaveData save = (cSaveData)reader.Deserialize ( typeof(cSaveData) );
		string sInfo = string.Format ( "Stage {0} " , save.n_StageID );
		return sInfo;

	}

}