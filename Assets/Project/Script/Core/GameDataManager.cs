using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
//using _SRW;
using MYGRIDS;
// All SRW enum list
//namespace _SRW
//{
	public enum _CAMP
	{
		_PLAYER=0,
		_ENEMY =1,
		_FRIEND=2,
	}

	public enum _SKILL_TYPE
	{
		_LAST = 0,
		_ABILITY ,
		_SKILL 	 ,
	}

	public enum _PK_MODE
	{
		_PLAYER 	=0,
		_ENEMY	  ,
		_ALL	  ,
	}

	public enum _CMD_TYPE // what kind of cmd list
	{
		_SYS = 0,
		_CELL ,
		_ALLY 	,
		_ENEMY 	,
		_MENU 	 ,
		_WAITATK ,
		_WAITMOVE ,
		_COUNTER,		// _COUNTER for mob atk
		_MAX ,
	}

	public enum _CMD_STATUS // what kind of cmd list
	{
		_NONE=0,
		_WAIT_CMDID,
		_WAIT_TARGET,

	}

	public enum _CMD_TARGET  // target type
	{
		_ALL = 0,
		_POS ,
		_UNIT,
		_SELF,
	}

	public enum _CMD_ID  // list of cmd btn id
	{
		_NONE = 0,			// 
		_MOVE ,			// 
		_ATK ,			// 
		_DEF ,			// 	
		_SKILL ,			// 	
		_ABILITY ,			// 	
		_SCHOOL ,			// 	
		_ITEM ,			//  No use		
		_WAIT,			//
		_INFO ,			// 	
		_COUNTER,			// 反擊	
		_STAGEINFO ,			// 	
		_CANCEL ,			// 	
		_NEWGAME,
		_SAVE,
		_LOAD,
		_ROUNDEND,
		_GAMEEND,
		_OPTION,
		_SUICIDE,
		_CHEAT,
		_SYSCHEAT,
		_WIN,
		_LOST,
		_KILLALLE,			//全敵人死亡
	}


	public enum _SEARCH_AI  // 搜索AI
	{
		_HOLD 	= 0,		//原地不動 	
		_ACTIVE	= 1,		//主動出擊
		_PASSIVE= 2,		//被動出擊
		_FOLLOW = 3,		//跟隨
		_POSITION = 4,		//前往目的地
	}

	public enum _COMBO_AI  // 戰鬥AI
	{
		_NORMAL	= 0,		//正常攻擊
		_DEFENCE= 1,		//總是防守
		_AOE	= 2,		//地圖攻擊優先		
	}
//	public enum _ROUND_STATUS
//	{
//		_START =0,
//		_RUNNING , 
//		_END     ,
//
//	}
	public enum _SAVE_PHASE  // 紀錄階段
	{
		_STARTUP		= 0,		// in startup
		_STAGE			= 1,		// in stage
		_MAINTEN		= 2,		// in mainta
		
	}
//}//_SRW_CMD



//
public class cCamp
{
	public _CAMP CampID { set; get; }
	public List<int> memLst;

	public cCamp()
	{
		memLst = new List<int>();
	}
}
/// <summary>預設存在的 Channel Type</summary>

public partial class GameDataManager 
{
	//public delegate AudioClip GetAudioClipDelegate (string audioPath);

	private bool hadInit;
	public bool HadInit{ get{ return hadInit; } }



//	private Dictionary<int, AudioChannelBase> channels = new Dictionary<int, AudioChannelBase>();

	public void Initial( int fileindex =0 ){
		hadInit = true;
		//this.GetAudioClipFunc = getAudioClipFunc;
		StoragePool = new Dictionary< int , cUnitData > ();		
		UnitPool = new Dictionary< int , cUnitData >();
		CampPool = new Dictionary< _CAMP , cCamp >();
		EvtDonePool = new Dictionary< int , int > ();			// record event complete round 
//		ConCharPool = new Dictionary< int , CHARS >();
//		ConCharPool   = new Dictionary< int , CHARS >() ;
//		ConSchoolPool = new Dictionary< int , SCHOOL >() ;
//		ConSkillPool  = new Dictionary< int , SKILL >() ;
		ImportEventPool = new List<int>();   // 已完成的重要事件列表

		ItemPool  = new List<int>();//

		GroupPool = new Dictionary< int , int >();			//  <leader char id , leader char ident>
		SkillDataCachePool = new Dictionary< int , cSkillData >();		

	}

	private static GameDataManager instance;
	public static GameDataManager Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new GameDataManager();
				instance.Clear();
			}
			
			return instance;
		}
	}

	public void Clear()
	{

	}

	public void ResetStage()
	{
		nMoney = 0;
		nRound = 0;
		nActiveCamp = _CAMP._PLAYER;
		UnitPool.Clear ();
		CampPool.Clear ();
		EvtDonePool.Clear();
		GroupPool.Clear();

		// special reset
		nSerialNO = 0;
	}

	public void EndStage()
	{
		// 把 unit pool 資料回存到 storage
		foreach( KeyValuePair< int , cUnitData > pair in UnitPool )
		{
			if( pair.Value.eCampID != _CAMP._PLAYER )
				continue;
			// 存到 storage pool 
			AddUnitToStorage( pair.Value ) ;
		}

		UnitPool.Clear ();
	}



	// need this to update all data's attr
	public void Update()
	{
		// update all unit's attr
		foreach (KeyValuePair< int , cUnitData > pair in UnitPool) {
			pair.Value.UpdateAttr();
		}
	}

	//===================================================
	public int nStoryID{ get; set; } 
	public int nStageID{ get; set; } 
	public int nTalkID{ get; set; } 
	public int nBattleID{ get; set; } 

	//陣營切換的背景音樂，隨關卡進展會改變
	public int nPlayerBGM{ get; set; }   //我方
	public int nEnemyBGM{ get; set; } 	 // 敵方
	public int nFriendBGM{ get; set; } 	// 友方

//	public int nOpCharIdent{ get; set; } 				//
//	public int nOpMobIdent{ get; set; } 				//
//	public int nOpFriendIdent{ get; set; } 				//
//
//	public int nOpCellX{ get; set; } 				//
//	public int nOpCellY{ get; set; } 				//
//
//	public int nInfoIdent{ get; set; }							//
//	public int nInfoCharID{ get; set; }							

	//
	public int nMoney{ get; set; } 
	public int nRound{ get; set; } 
	public int nStars{ get; set; } 
//	public _ROUND_STATUS nRoundStatus{ get; set; }    // 0- start ,1- running, 2- end

	public _SAVE_PHASE ePhase{ get; set; } 		// 目前進度狀態

	// Camp
	public _CAMP nActiveCamp{ get; set; }  // 


	// Camp
	public Dictionary< _CAMP , cCamp > CampPool;			// add Camp
	public cCamp GetCamp( _CAMP nCampID )
	{
		if( CampPool.ContainsKey( nCampID ) )
		{
			return CampPool[ nCampID ];
		}
		return null;
	}

	public void AddCampMember( _CAMP nCampID , int nMemIdent )
	{
		if( CampPool.ContainsKey( nCampID ) ){
			cCamp unit = CampPool[ nCampID ];
			if( unit != null ){
				if( unit.memLst.Contains( nMemIdent ) == false  )
				{
					unit.memLst.Add( nMemIdent );
				}
			}
		}
		else{
			cCamp unit = new cCamp();
			unit.CampID = nCampID;
			unit.memLst.Add( nMemIdent );
			CampPool.Add( nCampID , unit );
		}
		// find unit to set camp
		cUnitData data = GetUnitDateByIdent ( nMemIdent );
		if (data != null) {
			data.eCampID = nCampID;
		}


	}
	public void DelCampMember( _CAMP nCampID , int nMemIdent )
	{
		if( CampPool.ContainsKey( nCampID ) ){
			cCamp unit = CampPool[ nCampID ];
			if( unit != null )
			{
				unit.memLst.Remove( nMemIdent );
			}
		}
	}
	// switch to next Camp. return true if round change
	public bool NextCamp()
	{
		// weakup current camp first for remove unit mask
//		StageWeakUpCampEvent cmd = new StageWeakUpCampEvent ();
//		cmd.nCamp = nActiveCamp;
//		GameEventManager.DispatchEvent ( cmd );

		// 
		bool bRoundChange = false;
		if( nActiveCamp == _CAMP._PLAYER )
		{
			nActiveCamp = _CAMP._ENEMY;
//			nRoundStatus = _ROUND_STATUS._START;

			bRoundChange = false;
		}
		else if( nActiveCamp == _CAMP._ENEMY )
		{
			nActiveCamp = _CAMP._PLAYER; //
			nRound++;
	//		nRoundStatus = _ROUND_STATUS._START;

			bRoundChange =  true;
		}

		//weak up current for correct re def
		StageWeakUpCampEvent cmd = new StageWeakUpCampEvent ();
		cmd.nCamp = nActiveCamp;
		GameEventManager.DispatchEvent ( cmd );

		// open . round change panel ui
		PanelManager.Instance.OpenUI( Panel_Round.Name );


		return bRoundChange;	 
	}
	//Dictionary< int , Abil > ConAbilityPool;
//	public CHARS GetConstCharData( int nCharId )
//	{
//		CHARS p = null;
//
//		p = ConstDataManager.Instance.GetRow<CHARS> (nCharId);
//		if (p == null) {
//			Debug.LogErrorFormat( "can't get char constdata {0}" , nCharId );
//			p = new CHARS(); // fill empty value
//		}
//
//		return p;
//	}
//
//	public SCHOOL GetConstSchoolData( int nSchoolId )
//	{
//		SCHOOL p = null;
//		p = ConstDataManager.Instance.GetRow<SCHOOL> (nSchoolId);
//		if (p == null) {
//			Debug.LogErrorFormat( "can't get school constdata {0}" , nSchoolId );
//			p = new SCHOOL(); // fill empty value
//		}
//
//		return p;		
//	}
//

//	public SKILL GetConstSkillData( int nSkillId )
//	{
//		SKILL p = null;
//
//		p = ConstDataManager.Instance.GetRow<SKILL> (nSkillId);
//		if (p == null) {
//			Debug.LogErrorFormat( "can't get SKILL constdata {0}" , nSkillId );
//			p = new SKILL(); // fill empty value
//		}
//		return p;		
//	}

	Dictionary< int , cSkillData > SkillDataCachePool;
	// cache to get skill data
	public cSkillData GetSkillData( int nSkillId )
	{
		cSkillData skilldata = null;
		if( SkillDataCachePool.TryGetValue(nSkillId , out skilldata )== true ){
			return skilldata;
		}

		SKILL p = ConstDataManager.Instance.GetRow<SKILL> (nSkillId);
		if (p == null ) {
			if( nSkillId!=0 ){
				Debug.LogErrorFormat( "can't get SKILL constdata {0}" , nSkillId );
			}
		}

		skilldata = new cSkillData( p );
		if (nSkillId == 0) {
			 // spec setup
			skilldata.AddTag( _SKILLTAG._DAMAGE );
		}

		SkillDataCachePool.Add( nSkillId , skilldata );
		return skilldata;		
	}
	// 目前的紀錄狀態
	//public PLAYER_DATA			cPlayerData;
//	public cSaveData				SaveData;		
	public List<int>	ImportEventPool;   // 已完成的重要事件列表
//	public List<int> 	GetImportEvent(){  
//		if (ImportEventPool == null)
//			ImportEventPool = new List<int> ();
//		return ImportEventPool;
//	}					


	public List<int>			ItemPool;  //
	public void AddItemtoBag( int nItemID )
	{
		if (ItemPool == null) {
			ItemPool = new List<int> ();
		}
		ItemPool.Add (nItemID);
	}
	public void RemoveItemfromBag( int nItemID )
	{
		if (ItemPool == null) {
			ItemPool = new List<int> ();
		}
		ItemPool.Remove(nItemID);
	}

	// 昌庫腳色
	public Dictionary< int , cUnitData > StoragePool;		//以 unit data 結構存才能顯示 詳細資訊		

	// don't public this
	int nSerialNO;		// object serial NO
	public int GenerSerialNO( ){ return ++nSerialNO ; }
//	int GenerMobSerialNO( ){ return (++nSerialNO)*(-1) ; }

	// public  unit data
	public Dictionary< int , cUnitData > UnitPool;			// add event id 

	public cUnitData CreateChar( int nCharID )
	{
		cUnitData unit = new cUnitData();
		unit.n_Ident = GenerSerialNO( );
		unit.n_CharID = nCharID;

		CHARS cdata = ConstDataManager.Instance.GetRow< CHARS > (nCharID);
		if (cdata == null) {
			Debug.LogErrorFormat( "CreateChar with null data {0}" , nCharID );

		}
		unit.SetContData( cdata  );

		UnitPool.Add( unit.n_Ident , unit );
		return unit;
	}

	public bool AddCharToPool( cUnitData unit  )
	{
		// check game data to insert
		if( UnitPool.ContainsKey( unit.n_Ident ) == true ){
			//cUnitData org = UnitPool[ unit.n_Ident ];
			//UnitPool.Remove( unit.n_Ident );
			Debug.LogErrorFormat( "Err Add double unit ident{0},charid{1} to gamedata manager " , unit.n_Ident , unit.n_CharID );
		}
		UnitPool.Add( unit.n_Ident , unit );

		return true;
	}

	public cUnitData CreateCharbySaveData( cUnitSaveData save )
	{
		cUnitData unit = new cUnitData();
		unit.n_Ident = save.n_Ident;  //GenerSerialNO( );
		unit.n_CharID = save.n_CharID;

		if (unit.n_Ident > nSerialNO) {
			nSerialNO = unit.n_Ident;  // update serial NO if need 
		}

		
		CHARS cdata = ConstDataManager.Instance.GetRow< CHARS > (unit.n_CharID);
		if (cdata == null) {
			Debug.LogErrorFormat( "CreateCharbysave data with null data {0}" , unit.n_CharID );
			
		}
		unit.SetContData( cdata  );

		// 調整
		unit.eCampID = save.eCampID;
		unit.n_Lv	 = save.n_Lv;
		unit.n_EXP   = save.n_EXP;
		unit.n_HP  	 = save.n_HP;
		unit.n_MP	 = save.n_MP;
		unit.n_SP	 = save.n_SP;
		unit.n_DEF   = save.n_DEF;
		unit.nActionTime = save.nActionTime;

		unit.n_X	 = save.n_X;
		unit.n_Y	 = save.n_Y;

		unit.n_BornX = save.n_BornX;
		unit.n_BornY = save.n_BornY;
		unit.n_LeaderIdent	= save.n_LeaderIdent;

		unit.nActSch = save.nActSch;
		unit.Items 	 = save.Items;
		// school
		unit.SchoolPool = MyTool.ConvetToIntInt ( save.School );
		// buff
		unit.Buffs.ImportSavePool ( save.Buffs );


		unit.UpdateAllAttr ();

		return unit;
	}

	public cUnitData GetUnitDateByIdent( int nIdent )
	{
		if (nIdent == 0)			return null; // return direct to avoid no key err log

		cUnitData data;
		if( UnitPool.TryGetValue( nIdent , out data ) == true ){
			return  data;
		}
		else
		{
			Debug.LogErrorFormat( "gamedata manager.GetUnitDateByIdent with no key {0}" ,  nIdent  );
			return null;
		}

	}

	public cUnitData GetUnitDateByCharID( int nCharID )
	{
		if( nCharID == 0 )			return null;

		foreach (KeyValuePair< int ,cUnitData > pair in UnitPool) {
			if( pair.Value != null && pair.Value.n_CharID == nCharID )
			{
				return pair.Value;
			}
		}
		// event check will keep get unit by char
		//Debug.LogFormat( "gamedata manager.GetUnitDateByCharID with no char {0}" ,  nCharID  );
		return null;
	}
	public cUnitData GetUnitDateByPos( int nX , int nY )
	{
		foreach (KeyValuePair< int ,cUnitData > pair in UnitPool) {
			if( pair.Value != null )
			{
				if( (pair.Value.n_X == nX) && (pair.Value.n_Y == nY) ){
					return pair.Value;
				}
			}
		}
		return null;
	}
//	public cUnitData CreateMob( int nCharID )
//	{
//		cUnitData unit = new cUnitData();
//		unit.n_Ident = GenerMobSerialNO() ; 
//		unit.n_CharID = nCharID;		
//		UnitPool.Add( unit.n_Ident , unit );
//		return unit;
//	}

	public void DelUnit( int nIdent )
	{
		if( UnitPool.ContainsKey( nIdent)  )
			UnitPool.Remove( nIdent );

	}
	public void DelUnit( cUnitData unit )
	{
		if( unit != null )
			UnitPool.Remove( unit.n_Ident );
	}

	public void AddUnitToStorage( cUnitData data )
	{
		if (data == null || (data.eCampID != _CAMP._PLAYER ) )
			return ;
			 
		// 把 unit pool 資料回存到 storage
		if( StoragePool.ContainsKey( data.n_CharID ) == true )
		{
			StoragePool.Remove( data.n_CharID );
		}
		data.Relive ();
		StoragePool.Add( data.n_CharID , data );
	}



	public void BackUnitToStorage( int nIdent )
	{
		cUnitData data = null;
		if (UnitPool.TryGetValue(nIdent, out data )) {
			AddUnitToStorage( data );
			UnitPool.Remove( nIdent );
		}
	}

	public cUnitData GetStorageUnit( int nCharID )
	{
		if (StoragePool.ContainsKey (nCharID) == true) {
			return StoragePool[ nCharID ];
		}
		return null;
	}

	public bool RemoveStorageUnit( int nCharID )
	{
		if (StoragePool.ContainsKey (nCharID) == true) {
			StoragePool.Remove( nCharID );
			return true;
		}
		return false;
	}

	// Event Status
	public Dictionary< int , int > EvtDonePool;			// record event complete round 


	// public  Group
	public Dictionary< int , int > GroupPool;			//  <leader char id , leader char ident>
	public int GetGroupIDbyLeaderChar( int nCharID  ){
		int nIdent = 0;
		if (GroupPool.TryGetValue (nCharID, out nIdent) == false ) {
			return 0;
		}
		return nIdent;
	}
	public int CreateGroupWithLeaderChar( int nCharID  ){
		cUnitData unit = GetUnitDateByCharID ( nCharID );
		if (unit == null) {
			return 0;
		}
		if (GroupPool.ContainsKey (nCharID) == false) {
			GroupPool.Add( nCharID , unit.n_Ident );
		} else {
			GroupPool[nCharID ] =  unit.n_Ident;
		}
		return unit.n_Ident;
	}

	// Save to binary;
	public List< cUnitSaveData > ExportStoragePool()
	{
		List< cUnitSaveData > pool = new List< cUnitSaveData > ();
		foreach (KeyValuePair< int ,cUnitData > pair in  StoragePool ) {
			if( pair.Value != null )
			{
				cUnitSaveData savedata = new cUnitSaveData();
				savedata.SetData( pair.Value );
				pool.Add( savedata );
				
			}
		}		
		
		return pool;
	}

	public List< cUnitSaveData > ExportSavePool()
	{
		List< cUnitSaveData > pool = new List< cUnitSaveData > ();
		foreach (KeyValuePair< int ,cUnitData > pair in UnitPool) {
			if( pair.Value != null )
			{
				cUnitSaveData savedata = new cUnitSaveData();
				savedata.SetData( pair.Value );
				pool.Add( savedata );

			}
		}


		return pool;
	}

	// load from binary;
	public void ImportStoragePool( List< cUnitSaveData > pool)
	{
		// clear unit data
		//Panel_StageUI.Instance.in
		StoragePool.Clear ();
		
		foreach( cUnitSaveData save in pool ) {
			// add to char 
			cUnitData data = CreateCharbySaveData( save );
			if( data != null ){
				StoragePool.Add( save.n_CharID , data );
				//Panel_StageUI.Instance.CreateUnitByUnitData( data ) ;
				//CreateCharbySaveData( save )	
			}
		}
	}

	public void ImportSavePool( List< cUnitSaveData > pool)
	{
		// clear unit data
		//Panel_StageUI.Instance.in
		UnitPool.Clear ();

		foreach( cUnitSaveData save in pool ) {
			// add to char 
			cUnitData data = CreateCharbySaveData( save );
			if( data != null ){
				UnitPool.Add( data.n_Ident , data ); // add to unit pool first

				Panel_StageUI.Instance.CreateUnitByUnitData( data ) ;
				//CreateCharbySaveData( save )	
			}
		}
	}
};


public class cCMD{

	private bool hadInit = false;
	public bool HadInit{ get{ return hadInit; } }

	public List<_CMD_ID>[] CmdlistArray ;

	public MyGrids Grids;				// main grids . only one
	
	
	//	private Dictionary<int, AudioChannelBase> channels = new Dictionary<int, AudioChannelBase>();
	
	public void Initial( ){
		if (hadInit)return;
		
		hadInit = true;

		CmdlistArray = new List<_CMD_ID>[ (int)_CMD_TYPE._MAX ];

		int idx = 0;
		// create cmd list
		idx = (int)_CMD_TYPE._SYS;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add ( _CMD_ID._NEWGAME ); 
		CmdlistArray [idx].Add ( _CMD_ID._LOAD );
		CmdlistArray [idx].Add ( _CMD_ID._OPTION );

		// cell
		idx = (int)_CMD_TYPE._CELL;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add ( _CMD_ID._STAGEINFO ); 
		CmdlistArray [idx].Add ( _CMD_ID._SAVE ); 
		CmdlistArray [idx].Add ( _CMD_ID._LOAD ); 
		CmdlistArray [idx].Add ( _CMD_ID._ROUNDEND ); 
		CmdlistArray [idx].Add ( _CMD_ID._GAMEEND ); 
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		if (Config.GOD == true) {
			CmdlistArray [idx].Add (_CMD_ID._SYSCHEAT);
			CmdlistArray [idx].Add (_CMD_ID._WIN);
			CmdlistArray [idx].Add (_CMD_ID._LOST);
			CmdlistArray [idx].Add (_CMD_ID._KILLALLE);

		}


		// ally
		idx = (int)_CMD_TYPE._ALLY;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add ( _CMD_ID._INFO ); 
//		CmdlistArray [idx].Add ( _CMD_ID._MOVE ); 
		CmdlistArray [idx].Add ( _CMD_ID._ATK ); 
		CmdlistArray [idx].Add ( _CMD_ID._SKILL ); 
		CmdlistArray [idx].Add ( _CMD_ID._ABILITY ); 
	//	CmdlistArray [idx].Add ( _CMD_ID._DEF ); 
		CmdlistArray [idx].Add ( _CMD_ID._SCHOOL ); 
		CmdlistArray [idx].Add ( _CMD_ID._WAIT ); 
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		CmdlistArray [idx].Add ( _CMD_ID._SUICIDE );
		CmdlistArray [idx].Add ( _CMD_ID._CHEAT );

		// enemy
		idx = (int)_CMD_TYPE._ENEMY;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add (  _CMD_ID._INFO  );
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL );
		CmdlistArray [idx].Add ( _CMD_ID._SUICIDE );
		CmdlistArray [idx].Add ( _CMD_ID._CHEAT );


		// 
		idx = (int)_CMD_TYPE._MENU;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		// wait sel a target to atk
		idx = (int)_CMD_TYPE._WAITATK;
		CmdlistArray [idx] = new List<_CMD_ID> ();
//		CmdlistArray [idx].Add ( _CMD_ID._ATK ); 
		if (Config.GOD == true) {
			CmdlistArray [idx].Add ( _CMD_ID._ABILITY );  // god for debug
		}
		CmdlistArray [idx].Add ( _CMD_ID._SKILL ); 
		CmdlistArray [idx].Add ( _CMD_ID._WAIT ); 
		CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		// wait sel a pos
		idx = (int)_CMD_TYPE._WAITMOVE;
		CmdlistArray [idx] = new List<_CMD_ID> ();
		CmdlistArray [idx].Add ( _CMD_ID._WAIT ); 
		//CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		// counter 
		idx = (int)_CMD_TYPE._COUNTER;
		CmdlistArray [idx] = new List<_CMD_ID> (); // 反及
		CmdlistArray [idx].Add ( _CMD_ID._COUNTER ); 
		CmdlistArray [idx].Add ( _CMD_ID._ABILITY ); 
		CmdlistArray [idx].Add ( _CMD_ID._SKILL ); 
		CmdlistArray [idx].Add ( _CMD_ID._DEF ); 
		//CmdlistArray [idx].Add ( _CMD_ID._CANCEL ); 

		// initial cmd data
		Clear ();
	}

	private static cCMD instance;
	public static cCMD Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new cCMD();
				instance.Clear();
			}
			
			return instance;
		}
	}

	public int nCmderIdent;			// Operatr char ident
	public _CMD_TYPE	eCMDTYPE;		// Cmd type
	public _CMD_STATUS  eCMDSTATUS;		// current cmd status
	public _CMD_TARGET 	eCMDTARGET;		// cmd status
	public _CMD_ID 		eCMDID;			// current cmd ID

//	public _CMD_ID 		eLastCMDID;		// Last cmd ID
	public _CMD_TYPE	eLASTCMDTYPE;		// LAST Cmd type

	public _CMD_TYPE	eNEXTCMDTYPE;		// NEXT Cmd type
	public int nTarIdent;
	
	public int nOrgGridX;
	public int nOrgGridY;
	
	public int nTarGridX;
	public int nTarGridY;
	
	public int nSkillID;
	public int nAbilityID;

	public int nAOEID;				// if have aoe
	public int nItemID;

	public _CMD_TARGET 	eCMDAOETARGET;		// record Aoe target type


	public void Clear()
	{
		nCmderIdent = 0;
		eCMDTYPE = _CMD_TYPE._SYS;
		eCMDSTATUS = _CMD_STATUS._NONE;
		eCMDTARGET  = _CMD_TARGET._ALL;

		eCMDID 	  = _CMD_ID._NONE;
	//	eLastCMDID = _CMD_ID._NONE;

		eLASTCMDTYPE = _CMD_TYPE._SYS;;		// LAST Cmd type
		eNEXTCMDTYPE = _CMD_TYPE._SYS;

		nTarIdent = 0;		
		nOrgGridX = 0;
		nOrgGridY = 0;
		
		nTarGridX = 0;
		nTarGridY = 0;
		
		nSkillID = 0;
		nAbilityID = 0;
		nItemID = 0;
		
		nAOEID = 0;
		eCMDAOETARGET = _CMD_TARGET._ALL;
	}

	public List<_CMD_ID> GetCmdList( _CMD_TYPE eType)
	{
		int itype = (int)eType;
		if (itype < 0 || itype >= CmdlistArray.Length) {
			return null;
		}
		return CmdlistArray [itype];
	}
}

