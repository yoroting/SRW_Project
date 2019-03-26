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

[Serializable][JsonName("cd")]
public class cCDSaveData
{

    [JsonName("id")]
    [DefaultValue(0)]
    public int nID;

    [JsonName("time")]
    [DefaultValue(0)]
    public int nTime;           //還有幾回合 


    public cCDSaveData() { }

    public cCDSaveData(int skillid , int time)
    {
        nID = skillid;
        nTime = time;
    }

}

[Serializable][JsonName("unit")]
public class cUnitSaveData{
	[JsonName("id")] [DefaultValue(0)]public int n_Ident;
	[JsonName("cid")] [DefaultValue(0)] public int n_CharID;
    [JsonName("fid")] [DefaultValue(0)] public int n_FaceID;
    [JsonName("enable")] [DefaultValue(false)] public bool b_Enable;
	[JsonName("camp")] [DefaultValue(_CAMP._PLAYER )] public _CAMP eCampID;
	[JsonName("lv")][DefaultValue(0)] 	public int n_Lv;
	[JsonName("exp")][DefaultValue(0)] 	public int n_EXP;
	[JsonName("x")][DefaultValue(0)]   	public int n_X;
	[JsonName("y")][DefaultValue(0)]	public int n_Y;
	
	[JsonName("hp")][DefaultValue(0)]	public int n_HP;
	[JsonName("mp")][DefaultValue(0)]	public int n_MP;
	[JsonName("sp")][DefaultValue(0)]	public int n_SP;
    [JsonName("cp")][DefaultValue(0)]   public int n_CP;
    [JsonName("def")][DefaultValue(0)]	public int n_DEF;

	[JsonName("action")][DefaultValue(0)]	public int nActionTime;
    [JsonName("tired")] [DefaultValue(0)]    public int nTired;
    [JsonName("leader")][DefaultValue(0)] public int n_LeaderIdent;	// follow leader
	[JsonName("bornx")][DefaultValue(0)]	public int n_BornX;			// born Pox
	[JsonName("borny")][DefaultValue(0)]	public int n_BornY;
	//====data pool
	[JsonName("actsch")]				public int [] nActSch;		// current use 
	[JsonName("items")]					public int [] Items;		// current items 
    [JsonName("drop")]                  public int nDropItemID;     // current items 

    [JsonName("enhance")] public Dictionary<string, int> Enhance;     // Enhance Data
                                                                      // buff pool
    [JsonName("school")]				public Dictionary< string , int > School;		// current school 
	[JsonName("buffs")]					public List< cBuffSaveData> Buffs;		// current buffs
    [JsonName("cds")]					public List< cCDSaveData> CDs;      // current cd
    // TAG 需要存下來。會浮動
    [JsonName("tag")]                   public List<_UNITTAG> Tags;      // tag

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
        n_FaceID = data.n_FaceID;
		b_Enable = data.bEnable;
		eCampID = data.eCampID;
		n_Lv = data.n_Lv;
		n_EXP = data.n_EXP;
		n_X = data.n_X;
		n_Y = data.n_Y;
		n_HP = data.n_HP;
		n_MP = data.n_MP;
		n_SP = data.n_SP;
        n_CP = data.n_CP;
        n_DEF = data.n_DEF;
		n_LeaderIdent = data.n_LeaderIdent;
		n_BornX = data.n_BornX;
		n_BornY = data.n_BornY;

		nActSch = data.nActSch;
		Items = data.Items;
        nDropItemID = data.n_DropItemID;

        nActionTime = data.nActionTime;
        nTired = data.nTired;


        Enhance = MyTool.ConvetToStringInt(data.EnhancePool);  // unit enhance pool
        School = MyTool.ConvetToStringInt ( data.SchoolPool );  // unit school pool

		Buffs = data.Buffs.ExportSavePool ();					// unit buff pool
        CDs = data.CDs.ExportSavePool();        
        Tags = data.Tags;



        eSearchAI = data.eSearchAI;
		eComboAI = data.eComboAI;
		nAITarget = data.n_AITarget;
		nAIX = data.n_AIX;
		nAIY = data.n_AIY;
	}

    public cUnitData CreateUnitData()
    {
        cUnitData unit = new cUnitData();
        unit.n_Ident = n_Ident;  //GenerSerialNO( );
        unit.n_CharID = n_CharID;
        unit.bEnable = b_Enable;        

        CHARS cdata = ConstDataManager.Instance.GetRow<CHARS>(unit.n_CharID);
        if (cdata == null)
        {
            Debug.LogErrorFormat("CreateUnitData data with null data {0}", unit.n_CharID);

        }
        unit.SetContData(cdata);

        // 調整
        unit.n_FaceID = n_FaceID;
        //  if (0 == unit.n_FaceID) {
        //      unit.n_FaceID = unit.n_CharID; // default value
        //  }

        unit.eCampID = eCampID;

        unit.n_Lv = n_Lv;
        unit.n_EXP = n_EXP;
        unit.n_HP = n_HP;
        unit.n_MP = n_MP;
        unit.n_SP = n_SP;
        unit.n_CP = n_CP;
        unit.n_DEF = n_DEF;
        unit.nActionTime = nActionTime;
        unit.nTired = nTired;
        unit.n_X = n_X;
        unit.n_Y = n_Y;

        unit.n_BornX = n_BornX;
        unit.n_BornY = n_BornY;
        unit.n_LeaderIdent = n_LeaderIdent;

        unit.Items = Items;
        unit.n_DropItemID = nDropItemID;

        // enhance
        if (Enhance != null)
        {
            unit.EnhancePool = MyTool.ConvetToIntInt(Enhance);
        }
        // school
        unit.SchoolPool = MyTool.ConvetToIntInt(School);
        // buff
        unit.Buffs.ImportSavePool(Buffs);
        unit.CDs.ImportSavePool(CDs);

        // special tag
        if (Tags != null)
        {
            unit.Tags = Tags;
        }

        //=== AI
        unit.eSearchAI = eSearchAI;
        unit.eComboAI = eComboAI;
        unit.n_AITarget = nAITarget;
        unit.n_AIX = nAIX;
        unit.n_AIY = nAIY;

        // reactive school for skill data. take care old school must const data default school
        foreach (int nSchID in nActSch)
        {
            unit.ActiveSchool(nSchID);
        }
        //unit.nActSch = save.nActSch;
        //unit.ActiveSchool ( unit.GetExtSchID() );
        //unit.ActiveSchool ( unit.GetIntSchID() );

        unit.UpdateAllAttr(); // 設定 更新旗標
        // 此時做實際更新運算，在 buff 時會有問題，要等全部 pop 好再整體更新
        //	unit.UpdateAttr (); // sometime will cause fail when other ident not ready

        return unit;
    }

}

[Serializable][JsonName("block")]
public class cBlockSaveData
{
    [JsonName("id")]    [DefaultValue(0)]    public int ID;
    [JsonName("sx")]    [DefaultValue(0)]    public int StX;
    [JsonName("ex")]    [DefaultValue(0)]    public int EdX;
    [JsonName("sy")]    [DefaultValue(0)]    public int StY;
    [JsonName("ey")]    [DefaultValue(0)]    public int EdY;
    [JsonName("evtid")]    [DefaultValue(0)]    public int EvtID;
    [JsonName("type")]    [DefaultValue(0)]    public int Type;
    [JsonName("name")]    [DefaultValue("")]    public string sName;

    public cBlockSaveData() { }
    public void SetData(cEvtBlock data)
    {
        ID = data.nID;
        StX = data.rc.nStX;
        StY = data.rc.nEdX;
        EdX = data.rc.nStY;
        EdY = data.rc.nEdY;
        EvtID = data.nEvtID;
        Type = data.nType;
        sName = data.sName;
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
    [JsonName("DeadCount")] [DefaultValue(0)] public int n_DeadCount;			//撤退數
    
    [JsonName("stagephase")] [DefaultValue(0)] public int n_StagePhase;			//關卡階段
    

    [JsonName("emoney")]    [DefaultValue(0)]    public int n_EarnMoney;
    [JsonName("smoney")]    [DefaultValue(0)]    public int n_SpendMoney;

    [JsonName("phase")] [DefaultValue(_SAVE_PHASE._MAINTEN)] public _SAVE_PHASE ePhase = _SAVE_PHASE._MAINTEN;			//  0 - 整備 , 1-戰場上 , 2- sys

	[JsonName("pfirst")]public string sPlayerFirst ;			//玩家姓
	[JsonName("pname")] public string sPlayerName ;			//玩家名

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
	[JsonName("cpool")] public List< cUnitSaveData > 			CharPool;           // 戰場上角色
	[JsonName("ipool")] public List<int>						ItemPool;
	[JsonName("importpool")] public List<int>					ImportEventPool;   // 已完成的重要事件列表


    [JsonName("backstr")]    public string sBackJson;         //資料備份 （戰敗還原用 ）

    // stage special info
    [JsonName("evtdonepool")] public Dictionary<string,int>		EvtDonePool;   // 已完成的事件列表
    [JsonName("flagpool")]    public Dictionary<string, int> FlagPool;   // 特殊紀錄 旗標

    //	[JsonName("evtcheckpool")] public List<int>					EvtCheckPool;   // event can run
    //	[JsonName("evtwaitpool")] public List<int>					EvtWaitingPool;   // 已完成的重要事件列表

    [JsonName("grouppool")] public Dictionary< string , int >		GroupPool;   //  group event pool
    [JsonName("blockpool")] public List<cBlockSaveData>              EvtBlockPool;   //  list of event block
		
	static bool 	bIsLoading;													// don't public to avoid recprd this
	public static bool		IsLoading(){ return bIsLoading;	 }
	public static void		SetLoading( bool b){  bIsLoading = b; }


	// write data to save
	public void SetData( int nIdx , _SAVE_PHASE phase )
	{
		n_IDX = nIdx;

		//把所有要記錄的都寫在這
		sPlayerFirst = Config.PlayerFirst;
		sPlayerName = Config.PlayerName;

		n_StoryID = GameDataManager.Instance.nStoryID;
		n_StageID = GameDataManager.Instance.nStageID;
		n_Round = GameDataManager.Instance.nRound;
		e_Camp = GameDataManager.Instance.nActiveCamp;
		n_Money = GameDataManager.Instance.nMoney;
		n_Stars = GameDataManager.Instance.nStars;
        n_DeadCount = GameDataManager.Instance.n_DeadCount;
        n_StagePhase = GameDataManager.Instance.n_StagePhase;

        nPlayerBGM = GameDataManager.Instance.nPlayerBGM;   //我方
		nEnemyBGM  = GameDataManager.Instance.nEnemyBGM;	 // 敵方
		nFriendBGM = GameDataManager.Instance.nFriendBGM;	// 友方


		ItemPool = GameDataManager.Instance.ItemPool;			// item list
		ImportEventPool = GameDataManager.Instance.ImportEventPool;
		StoragePool = GameDataManager.Instance.ExportStoragePool();
        FlagPool = GameDataManager.Instance.FlagPool; // maybe it is null

        sBackJson = GameDataManager.Instance.sBackJson;

        //       [JsonName("bpool")]
        //public List<cUnitSaveData> CharBackPool;           // 備份 倉庫角色，戰敗時還原用
        //[JsonName("bipool")]
        //public List<int> ItemBackPool;                    // 備份 道具，戰敗時還原用

        // 經濟
        n_EarnMoney = GameDataManager.Instance.nEarnMoney;
        n_SpendMoney = GameDataManager.Instance.nSpendMoney;

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
            // flag pool
          
           //EvtCheckPool = Panel_StageUI.Instance.evt
           // group pool
            GroupPool   = MyTool.ConvetToStringInt( GameDataManager.Instance.GroupPool );
			// unit pool
			CharPool = GameDataManager.Instance.ExportSavePool();
            // block pool
            EvtBlockPool = GameDataManager.Instance.ExportBlockPool();
           
        }
	}

	//將遊戲還原到 紀錄的狀態
	public void RestoreData( _SAVE_PHASE phase )
	{
		// clear data
		GameSystem.PlayBGM( 0 ); // stop bgm
		GameSystem.bFXPlayMode = false;

//		GameDataManager.Instance.SaveData = this; // for startcoror
		//把所有要記錄的都寫在這

		Config.PlayerFirst = sPlayerFirst ;
		Config.PlayerName = sPlayerName;
		if (string.IsNullOrEmpty (Config.PlayerFirst))
			Config.PlayerFirst = Config.DefaultPlayerFirst;
		if (string.IsNullOrEmpty (Config.PlayerName))
			Config.PlayerName = Config.DefaultPlayerName;


		GameDataManager.Instance.StoragePool.Clear();
		GameDataManager.Instance.UnitPool.Clear();


		// reset data
		GameDataManager.Instance.nStoryID = n_StoryID;
		GameDataManager.Instance.nStageID = n_StageID;
		GameDataManager.Instance.nMoney = n_Money;
		GameDataManager.Instance.nStars = n_Stars;
        GameDataManager.Instance.n_DeadCount = n_DeadCount;
        GameDataManager.Instance.n_StagePhase = n_StagePhase;
        // stage data  set in stage load
        //		GameDataManager.Instance.nRound   = n_Round;
        //		GameDataManager.Instance.nActiveCamp = e_Camp ;

        // need set after stage load
        //		if( nPlayerBGM > 0 )
        //			GameDataManager.Instance.nPlayerBGM = nPlayerBGM ;   //我方
        //		if( nEnemyBGM > 0 )
        //			GameDataManager.Instance.nEnemyBGM  = nEnemyBGM;	 // 敵方
        //		if( nFriendBGM > 0 )
        //			GameDataManager.Instance.nFriendBGM = nFriendBGM;	// 友方


        GameDataManager.Instance.ItemPool = ItemPool.ToList<int>()  ;			// item list
		GameDataManager.Instance.ImportEventPool = ImportEventPool;
		GameDataManager.Instance.ImportStoragePool( StoragePool );
        
        GameDataManager.Instance.FlagPool = FlagPool.ToDictionary(k => k.Key, k => k.Value);   //  clone pool to avoid smart pool fail
        // 由phase 決定目前該切到哪個場僅. this should need a 
        if (GameDataManager.Instance.FlagPool == null)
        {
            GameDataManager.Instance.FlagPool = new Dictionary<string, int>();
        }

        GameDataManager.Instance.sBackJson = sBackJson;


        GameDataManager.Instance.nEarnMoney = n_EarnMoney;
        GameDataManager.Instance.nSpendMoney = n_SpendMoney;
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
			//MainUIPanel panel = MyTool.GetPanel< MainUIPanel >( PanelManager.Instance.JustGetUI( MainUIPanel.Name ) );
			MainUIPanel panel = MyTool.GetPanel< MainUIPanel >( PanelManager.Instance.OpenUI( MainUIPanel.Name ) );
			if( panel != null ){
				panel.LoadSaveGame( this );
			}
		}

	}

	static public string GetKey( int Idx )
	{
		return "save" + Idx.ToString() ;
	}

    static public string GetSaveFileName(int Idx)
    {
        return Application.persistentDataPath + "/" + GetKey(Idx ) + ".sav";
    }

    static public string GetSaveFileContent(int nStoryID )
    {
        string nstoryname = MyTool.GetStoryName( nStoryID );
       
        string name = Config.PlayerFirst + Config.PlayerName;
        string status = "";
        string slv = "";
        cUnitData pUnit = GameDataManager.Instance.GetUnitDateByCharID(1);
        if (pUnit == null) {
            pUnit = GameDataManager.Instance.GetStorageUnit( 1 );
        }

        // 整備
        if (GameDataManager.Instance.ePhase == _SAVE_PHASE._MAINTEN)
        {
            status = "整備";
        }
        else 
        { // 關卡中
            status = "回合 " + GameDataManager.Instance.nRound;
        }
        // 時間
        string stime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm");

        // 主角等級
        if ( pUnit != null ) {
            slv = "Lv. " + pUnit.n_Lv.ToString() ;
        }

        // 組合
        string  content = String.Format("{0};{1};{2};{3};{4};", nstoryname , name , status , stime , slv );

        return content;
    }

    static public bool Load( int nID, _SAVE_PHASE phase )
	{
		if (IsLoading ())
			return false;
        //string sKeyName = GetKey( Idx );        
        //        string sBackKeyName = sKeyName + "_bak";
        string sFileName = GetSaveFileName(nID);
        if (System.IO.File.Exists(sFileName) == false)            
        {
            return false;
        }


        FileStream fs = new FileStream(sFileName , FileMode.Open);
        StreamReader sw = new StreamReader(fs);
        // 讀簡易資訊
        string sSimple = sw.ReadLine(); 
        // 讀完整資訊
        // sw.Write(json.ToString());
        string sJson = sw.ReadToEnd();
        sw.Close();
        //      string sJson = PlayerPrefs.GetString ( sKeyName , "" );
        if (string.IsNullOrEmpty(sJson))
        {
            return false;
        }
        //      string sBackJson = PlayerPrefs.GetString(sBackKeyName, "");


        SetLoading (true);
		// ---- DESERIALIZATION ----
		
		JsonReaderSettings readerSettings = new JsonReaderSettings();
		readerSettings.TypeHintName = "__type";
		
		JsonReader reader = new JsonReader(sJson, readerSettings);
		
		cSaveData save = (cSaveData)reader.Deserialize ( typeof(cSaveData) );
     //   save.sBackJson = sBackJson; // 接回去

        save.RestoreData ( phase );
		//parameters = (Dictionary<string, object>)reader.Deserialize();
		return true;
	}

	static public bool Save( int nID ,  _SAVE_PHASE phase  )
	{
		cSaveData save = new cSaveData ();
		save.SetData ( nID , phase );
        string sBackJson = save.sBackJson;
//        save.sBackJson = "";

        string sKeyName = GetKey( nID );
//        string sBackKeyName = sKeyName + "_bak";

        // ---- SERIALIZATION ----

        JsonWriterSettings writerSettings = new JsonWriterSettings();
		writerSettings.TypeHintName = "__type";
		
		StringBuilder json = new StringBuilder();
		JsonWriter writer = new JsonWriter(json, writerSettings);
		writer.Write(save);
        //    PlayerPrefs.SetString (sKeyName , json.ToString() ); // 分段存
        //    PlayerPrefs.SetString(sBackKeyName, sBackJson );
        //   PlayerPrefs.Save ();

        int nSize = json.ToString().Length;
//        int nBackSize = sBackJson.Length;

        //換 fileIO 方法才能永遠不擔心 windows size
        
        //FileStream fs = File.Create(Application.persistentDataPath + "/"+ sKeyName+".sav");
        FileStream fs = new FileStream(GetSaveFileName(nID), FileMode.OpenOrCreate);
        StreamWriter sw = new StreamWriter(fs);

        // 寫 簡易資訊
        sw.WriteLine( GetSaveFileContent(GameDataManager.Instance.nStoryID)  );
        // 寫 完整資訊
        sw.Write( json.ToString() );
        sw.Close();

        return true;
	}


	static public string LoadSaveSimpleInfo( int nID)
	{
        //string sKeyName = GetKey( nID );
        //string sJson = PlayerPrefs.GetString ( sKeyName , "" );

        string sFileName = GetSaveFileName(nID);
        string sSimple = "- - - - - - - - - -";
        if (System.IO.File.Exists(sFileName) == true)
        {

            FileStream fs = new FileStream(sFileName, FileMode.Open);
            StreamReader sw = new StreamReader(fs);

            string data = sw.ReadLine();
            if (data != "")
            {
                sSimple = data;
            }
            // sw.Write(json.ToString());
            //        string sJson = sw.ReadToEnd();
            sw.Close();
        }


        

        return sSimple;
//        if (string.IsNullOrEmpty (sJson))
//			return null;
// ---- DESERIALIZATION ----

        //		JsonReaderSettings readerSettings = new JsonReaderSettings();
        //readerSettings.TypeHintName = "__type";

        //		JsonReader reader = new JsonReader(sJson, readerSettings);

        //  return "s";
        //        cSaveData save = (cSaveData)reader.Deserialize ( typeof(cSaveData) );
        //        return save;
        //		string sInfo = string.Format ( "STORY {0} " , save.n_StoryID );
        //		return sInfo;

    }

}