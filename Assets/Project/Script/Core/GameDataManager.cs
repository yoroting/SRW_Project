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



// no more use cCamp
//public class cCamp
//{
//	public _CAMP CampID { set; get; }
//	public List<int> memLst;
//
//	public cCamp()
//	{
//		memLst = new List<int>();
//	}
//}

public class cEvtBlock
{
    // use to triger event
    public int nID;
    public string sName;
    public int nType;
    public iRect rc;
    public int nEvtID;

    public cEvtBlock(int nStX , int nEdX , int nStY, int nEdY ) {
        rc = new iRect(nStX, nEdX, nStY, nEdY);
    }

};


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
//		CampPool = new Dictionary< _CAMP , cCamp >();
		EvtDonePool = new Dictionary< int , int > ();			// record event complete round 
//		ConCharPool = new Dictionary< int , CHARS >();
//		ConCharPool   = new Dictionary< int , CHARS >() ;
//		ConSchoolPool = new Dictionary< int , SCHOOL >() ;
//		ConSkillPool  = new Dictionary< int , SKILL >() ;
		ImportEventPool = new List<int>();   // 已完成的重要事件列表

		ItemPool  = new List<int>();//

		GroupPool = new Dictionary< int , int >();			//  <leader char id , leader char ident>
		SkillDataCachePool = new Dictionary< int , cSkillData >();


        EvtBlockPool = new List<cEvtBlock>();        // list of event block

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
		Debug.Log(" game data clear called");
	}

	public void ResetStage()
	{
		nMoney = 0;
		nRound = 0;
		nActiveCamp = _CAMP._PLAYER;
		UnitPool.Clear ();
		//CampPool.Clear ();
		EvtDonePool.Clear();
		GroupPool.Clear();

        EvtBlockPool.Clear();
        // special reset
        nSerialNO = 0;
	}

	public void EndStage()
	{
		// recycle drop
		BattleManager.Instance.RecycleDrop();

		// 把 unit pool 資料回存到 storage
		foreach( KeyValuePair< int , cUnitData > pair in UnitPool )
		{
			if( pair.Value.eCampID != _CAMP._PLAYER )
				continue;
			// 存到 storage pool 
			AddUnitToStorage( pair.Value ) ;
		}

		UnitPool.Clear ();

		// stop bgm
		GameSystem.PlayBGM( 0 );

	}



	// need this to update all data's attr
//	public void Update()
//	{
//		// update all unit's attr
//		foreach (KeyValuePair< int , cUnitData > pair in UnitPool) {
//			pair.Value.UpdateAttr();
//		}
//	}

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
    
	public int GetCampNum( _CAMP eCampID )
	{
		int cout = 0;
		foreach( KeyValuePair< int , cUnitData > pair in UnitPool )
		{
			if( pair.Value == null  )
				continue;
			if( pair.Value.eCampID ==  eCampID ){
				cout++;
			}
		}
		return cout;
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

			// check for ally
			//cCamp camp = GameDataManager.Instance.GetCamp( _CAMP._FRIEND );
			int nCount = GetCampNum(_CAMP._FRIEND );
			if( nCount > 0 )
			{
				nActiveCamp = _CAMP._FRIEND;  
			}
			else {
				nActiveCamp = _CAMP._ENEMY; // set to enemy if no friend
			}

			bRoundChange = false;
		}
		else if( nActiveCamp == _CAMP._FRIEND )
		{
			
			// check for ally
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

    public void SetCharFace(int nCharID, int nFaceID)
    {
        foreach (KeyValuePair<int, cUnitData> pair in StoragePool)
        {
            if (pair.Value == null)
                continue;

            if (nCharID == pair.Value.n_CharID)
            {
                pair.Value.n_FaceID = nFaceID ;
                break;
            }
        }
        //================
        foreach (KeyValuePair<int, cUnitData> pair in UnitPool)
        {
            if (pair.Value == null)
                continue;

            if (nCharID == pair.Value.n_CharID)
            {
                pair.Value.n_FaceID = nFaceID;

                if ( Panel_StageUI.Instance != null ) {
                    Panel_StageUI.Instance.OnStageRefreshUnitFaceEvent(pair.Key);
                }

                // change face
                break;
            }
        }
        // talk change face

    }
    
    // talk / story ui
    public int GetUnitFaceID( int nCharID ) {
        int nFaceID = 0;
        // find in storage first
        foreach (KeyValuePair<int, cUnitData> pair in StoragePool )
        {
            if (pair.Value == null)
                continue;

            if (nCharID == pair.Value.n_CharID) {
                nFaceID = pair.Value.n_FaceID;
                break;
            }
        }

        // find in battle pool
        if (0 == nFaceID)
        {
            foreach (KeyValuePair<int, cUnitData> pair in UnitPool)
            {
                if (pair.Value == null)
                    continue;

                if (nCharID == pair.Value.n_CharID)
                {
                    nFaceID = pair.Value.n_FaceID;
                    break;
                }
            }
        }
        // use const data default
        CHARS data = ConstDataManager.Instance.GetRow<CHARS>(nCharID);
        if (data != null) {
            nFaceID = data.n_FACEID;
        }

        if (0 == nFaceID) {
            Debug.LogWarningFormat("no unit for GetUnitFaceID in {0} ", nCharID);
            nFaceID = nCharID; // use charid as default
        }
        return nFaceID; // use charid as default

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
    
    public List<cEvtBlock> EvtBlockPool;        // list of event block
    public cEvtBlock RegEvtBlock( int nStX , int nEdX , int nStY, int nEdY , int nEvtID , string sName="" )
    {
        cEvtBlock evt = new cEvtBlock(nStX, nEdX, nStY, nEdY );
        evt.nEvtID = nEvtID;
        evt.sName = sName;
        EvtBlockPool.Add(evt);
        return evt;
    }

    public bool DelEvtBlock( string sName = "")
    {
        foreach (cEvtBlock evt in EvtBlockPool)
        {
            if( evt.sName == sName)
            {
                EvtBlockPool.Remove( evt );
                return true;
            }

        }        
        return false;
    }

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
	public Dictionary< int , cUnitData > StoragePool;		//以 < charid , unit > unit data 結構存才能顯示 詳細資訊		

	// don't public this
	int nSerialNO;		// object serial NO
	public int GenerSerialNO( ){ return ++nSerialNO ; }
//	int GenerMobSerialNO( ){ return (++nSerialNO)*(-1) ; }

	// public  unit data
	public Dictionary< int , cUnitData > UnitPool;			// add  < ident , unit >  event id  

	public cUnitData CreateChar( int nCharID , _CAMP camp , int bornx , int borny , int nLv , int nLeaderId )
	{
		cUnitData data = new cUnitData();
		data.n_Ident = GenerSerialNO( );
		data.n_CharID = nCharID;

		CHARS cdata = ConstDataManager.Instance.GetRow< CHARS > (nCharID);
		if (cdata == null) {
			Debug.LogErrorFormat( "CreateChar with null data {0}" , nCharID );

		}
		data.n_Lv = nLv;
		data.SetContData( cdata  );
		data.eCampID = camp;

		data.n_X = data.n_BornX = bornx;
		data.n_Y = data.n_BornY = borny;
		data.n_LeaderIdent = nLeaderId;
		if (UnitPool.ContainsKey (data.n_Ident)) {
			Debug.LogErrorFormat( "Err double key when GDM CreateChar with ident{0},charid{1}" ,data.n_Ident ,nCharID  );

		}
		else{
			UnitPool.Add( data.n_Ident , data );
		}
		return data;
	}



	public cUnitData CreateCharbySaveData( cUnitSaveData save , bool bAddtoPool = false )
	{
		cUnitData unit  = new cUnitData();
		unit.n_Ident 	= save.n_Ident;  //GenerSerialNO( );
		unit.n_CharID 	= save.n_CharID;
		unit.bEnable 	= save.b_Enable;

		if (unit.n_Ident > nSerialNO) {
			nSerialNO = unit.n_Ident;  // update serial NO if need 
		}
		
		CHARS cdata = ConstDataManager.Instance.GetRow< CHARS > (unit.n_CharID);
		if (cdata == null) {
			Debug.LogErrorFormat( "CreateCharbysave data with null data {0}" , unit.n_CharID );
			
		}
		unit.SetContData( cdata  );

        // 調整
        unit.n_FaceID = save.n_FaceID;
      //  if (0 == unit.n_FaceID) {
      //      unit.n_FaceID = unit.n_CharID; // default value
      //  }

		unit.eCampID = save.eCampID;

		unit.n_Lv	 = save.n_Lv;
		unit.n_EXP   = save.n_EXP;
		unit.n_HP  	 = save.n_HP;
		unit.n_MP	 = save.n_MP;
		unit.n_SP	 = save.n_SP;
        unit.n_CP    = save.n_CP;
        unit.n_DEF   = save.n_DEF;
		unit.nActionTime = save.nActionTime;

		unit.n_X	 = save.n_X;
		unit.n_Y	 = save.n_Y;

		unit.n_BornX = save.n_BornX;
		unit.n_BornY = save.n_BornY;
		unit.n_LeaderIdent	= save.n_LeaderIdent;

		unit.Items 	 = save.Items;
		// school
		unit.SchoolPool = MyTool.ConvetToIntInt ( save.School );
		// buff
		unit.Buffs.ImportSavePool ( save.Buffs );

		//=== AI
		unit.eSearchAI = save.eSearchAI;
		unit.eComboAI = save.eComboAI;
		unit.n_AITarget = save.nAITarget;
		unit.n_AIX = save.nAIX;
		unit.n_AIY = save.nAIY;

		// reactive school for skill data. take care old school must const data default school
		foreach ( int nSchID in save.nActSch) {
			unit.ActiveSchool ( nSchID );					
		}
		//unit.nActSch = save.nActSch;
		//unit.ActiveSchool ( unit.GetExtSchID() );
		//unit.ActiveSchool ( unit.GetIntSchID() );


		unit.UpdateAllAttr ();
		unit.UpdateAttr ();

		if (bAddtoPool) {
			AddCharToPool( unit );
		} else { // add to storge
			AddCharToStorage( unit );
		}

		return unit;
	}

	public bool AddCharToPool( cUnitData unit )
	{
		// check game data to insert
		if (UnitPool.ContainsKey (unit.n_Ident) == true) {
			//if( unit.IsTag(_UNITTAG._UNDEAD) == false  ){
				Debug.LogErrorFormat ("Err Add double unit ident{0},charid{1} to gamedata manager char pool ", unit.n_Ident, unit.n_CharID);
			//}
			
		} else {
			UnitPool.Add( unit.n_Ident , unit );
		}		
		
		return true;
	}
	public bool AddCharToStorage( cUnitData unit )
	{

		// check game data to insert
		if (StoragePool.ContainsKey (unit.n_CharID) == true) {
			Debug.LogErrorFormat ("Err Add double unit ident{0},charid{1} to gamedata manager storage pool", unit.n_Ident, unit.n_CharID);
			
		} else {
			StoragePool.Add( unit.n_CharID , unit );
		}		
		
		return true;
	}
	// for stage pop unit
	public cUnitData StagePopUnit ( int nCharID, _CAMP eCamp,  int nBX, int nBY , int nLv = 1, int nLeaderID =0 )
	{
		if (eCamp == _CAMP._PLAYER) {
			cUnitData data = null;
			if (StoragePool.TryGetValue (nCharID , out data ) )
			{
				data.n_Ident = GenerSerialNO() ;  // reassign ident
				data.eCampID = eCamp;
				data.n_X = data.n_BornX = nBX;
				data.n_Y = data.n_BornY = nBY;
				data.n_LeaderIdent = nLeaderID;

                data.bEnable = true;            // pop unit is true as default

                // Add to char pool
                AddCharToPool( data );

				// remove from storage
				RemoveStorageUnit( nCharID );
				//StoragePool.Remove( nCharID );

				return  data;
			}
		} 

		return  CreateChar( nCharID , eCamp, nBX , nBY , nLv , nLeaderID ); // already add to pool

	}

	//========
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

	public int GetIdentByCharID( int nCharID )
	{
		if( nCharID == 0 )			return 0;
		
		foreach (KeyValuePair< int ,cUnitData > pair in UnitPool) {
			if( pair.Value != null && pair.Value.n_CharID == nCharID )
			{
				return pair.Value.n_Ident;
			}
		}
		// event check will keep get unit by char
		//Debug.LogFormat( "gamedata manager.GetUnitDateByCharID with no char {0}" ,  nCharID  );
		return 0;
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
	//	data.bEnable = true; // not always enable
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

	public void EnableStorageUnit( int nCharID , bool bEnable = false )
	{
		if (StoragePool.ContainsKey (nCharID) == true) {
			StoragePool[nCharID].bEnable = bEnable;
		}
	}
    public void EnableStageUnit(int nCharID, bool bEnable = false)
    {
        foreach (var pair in UnitPool)
        {
            if (pair.Value == null)
                continue;
            if (pair.Value.eCampID != _CAMP._PLAYER)
                continue;
            if (pair.Value.n_CharID != nCharID)
                continue;
            pair.Value.bEnable = bEnable;

        }
    }

    public void ClearStorageUnit(  )
	{
		if (StoragePool != null) {
			StoragePool.Clear();
		} 
	}

	public void ReLiveUndeadUnit( _CAMP camp ) // all  undead
	{
		foreach (KeyValuePair< int , cUnitData > pair in UnitPool) {
			if( camp != pair.Value.eCampID )
				continue;
			// relive
			cUnitData p = pair.Value;
			if( p.n_HP == 0 ){
				p.Relive();
				Panel_StageUI.Instance.CreateUnitByUnitData( p );
			}
		}

	}
	/// <summary>
	///  AI
	/// </summary>
	public void SetUnitSearchAI( int nCharID , _AI_SEARCH nSearchAI , int nArg1=0 , int nArg2 =0 )
	{
        foreach (var pair in UnitPool)
        {
            cUnitData unit = pair.Value; //  GetUnitDateByCharID(nCharID);
            if (unit != null)
            {
                if (unit.n_CharID != nCharID)
                    continue;
                //unit.n
                if (nSearchAI != _AI_SEARCH._NOCHANGE)
                {
                    unit.eSearchAI = nSearchAI;

                    if (nSearchAI == _AI_SEARCH._TARGET)
                    {
                        unit.n_AITarget = nArg1;
                    }
                    else if (nSearchAI == _AI_SEARCH._POSITION)
                    {
                        unit.n_AIX = nArg1;
                        unit.n_AIY = nArg2;
                    }
                }
            }
            else
            {
                Debug.LogErrorFormat(" Set SAI fail with {0} - {1} - {2}", nCharID, nSearchAI, nArg1);

            }
        }
	}
	public void SetUnitComboAI( int nCharID ,  _AI_COMBO nComboAI=_AI_COMBO._NOCHANGE )
	{
        foreach (KeyValuePair<int, cUnitData> pair in UnitPool)
        {
            cUnitData unit = pair.Value; //  GetUnitDateByCharID(nCharID);
            if (unit != null)
            {
                if (unit.n_CharID != nCharID)
                    continue;

                if (nComboAI != _AI_COMBO._NOCHANGE)
                {
                    unit.eComboAI = nComboAI;
                }
            }
            else
            {
                Debug.LogErrorFormat(" Set CAI fail with {0} - {1} ", nCharID, nComboAI);

            }
        }
	}
    public void SetCampSearchAI(int nCampID, _AI_SEARCH nSearchAI, int nArg1 = 0, int nArg2 = 0)
    {
        _CAMP camp = (_CAMP)nCampID;

        foreach (var pair in UnitPool)
        {
            cUnitData unit = pair.Value; //  GetUnitDateByCharID(nCharID);
            if (unit != null)
            {
                if (unit.eCampID != camp)
                    continue;
                //unit.n
                if (nSearchAI != _AI_SEARCH._NOCHANGE)
                {
                    unit.eSearchAI = nSearchAI;

                    if (nSearchAI == _AI_SEARCH._TARGET)
                    {
                        unit.n_AITarget = nArg1;
                    }
                    else if (nSearchAI == _AI_SEARCH._POSITION)
                    {
                        unit.n_AIX = nArg1;
                        unit.n_AIY = nArg2;
                    }
                }
            }
            else
            {
                Debug.LogErrorFormat(" Set camp SAI fail with {0} - {1} - {2}", nCampID, nSearchAI, nArg1);

            }
        }
    }
    public void SetCampComboAI(int nCampID, _AI_COMBO nComboAI = _AI_COMBO._NOCHANGE)
    {
        _CAMP camp = (_CAMP)nCampID;

        foreach (KeyValuePair<int, cUnitData> pair in UnitPool)
        {
            cUnitData unit = pair.Value; //  GetUnitDateByCharID(nCharID);
            if (unit != null)
            {
                if (unit.eCampID != camp)
                    continue;

                if (nComboAI != _AI_COMBO._NOCHANGE)
                {
                    unit.eComboAI = nComboAI;
                }
            }
            else
            {
                Debug.LogErrorFormat(" Set camp CAI fail with {0} - {1} ", nCampID, nComboAI);

            }
        }
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

    public List<cBlockSaveData> ExportBlockPool()
    {
        List<cBlockSaveData> pool = new List<cBlockSaveData>();
        foreach ( cEvtBlock b in EvtBlockPool )
        {
            cBlockSaveData s = new cBlockSaveData( );
            s.ID = b.nID;
            s.StX = b.rc.nStX;
            s.StY = b.rc.nStY;
            s.EdX = b.rc.nEdX;
            s.EdY = b.rc.nEdY;
            s.EvtID = b.nEvtID;
            s.Type = b.nType;
            s.sName = b.sName;
            pool.Add(s);
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
        if (pool == null)
        {
            return;
        }
        foreach ( cUnitSaveData save in pool ) {
			// add to char 
			cUnitData data = CreateCharbySaveData( save  , true );
			if( data != null ){
				//UnitPool.Add( data.n_Ident , data ); // add to unit pool first
			//	Panel_StageUI.Instance.CreateUnitByUnitData( data ) ; // need create panel_unit

			}
		}
	}

    public void ImportBlockPool(List<cBlockSaveData> pool)
    {
        // clear unit data
        EvtBlockPool.Clear();
        if (pool == null)
        {
            return;
        }

        foreach (cBlockSaveData save in pool)
        {
            // add to char 
            cEvtBlock evt = new cEvtBlock( save.StX , save.StY , save.EdX , save.EdY );
            evt.nID = save.ID;
            evt.nEvtID = save.EvtID;
            evt.nType = save.Type;
            evt.sName = save.sName;
            EvtBlockPool.Add( evt );
        }
    }

    public void SetBGMPhase( int nPhase )
	{
		// 0-正常 , 1-勝利 , 2-緊張 , 3-悲壯 ,4-壓迫
		if( nPhase < 0 || nPhase >9)
			return;

		nPlayerBGM = 100 + nPhase ; // from 100-109
		nEnemyBGM  = 110 + nPhase ; // from 110-119	
		nFriendBGM  = 120 + nPhase ; // from 120-129	
	}
}

