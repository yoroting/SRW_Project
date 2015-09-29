using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
using MYGRIDS;
//using _SRW;
using MyClassLibrary;			// for parser string
// 
public class MyScript {

	private static MyScript instance;
	public static MyScript Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new MyScript();
			}
			
			return instance;
		}
	}

	//List<cHitResult> CacheHitResultPool;


	public bool CheckEventCondition( CTextLine line  )
	{
		if( line == null )
			return false;
		
		List<cTextFunc> funcList =line.GetFuncList();
		foreach( cTextFunc func in funcList )
		{
			if( func.sFunc == "COMBAT"  )
			{
				if( ConditionCombat( func.I(0),func.I(1)  ) == false )
				{
					return false;
				}		
			}
			else if( func.sFunc == "GO" )
			{
				if( ConditionGO( ) == false )
				{
					return false;
				}	
			}
			else if( func.sFunc == "HP"  )
			{
				if( ConditionHp( func.I(0), func.S(1)  ,func.F(2) ) == false )
				{
					return false;
				}	
			}
			// 以上 可以在 戰鬥中檢查
			else { 
				// 以下 不可在 戰鬥中檢查
				if (BattleManager.Instance.IsBattlePhase ())
					return false;// don't check in battle

				if( func.sFunc == "ALLDEAD" )
				{
					if( ConditionAllDead( func.I(0) ) == false )
					{
						return false;
					}				
				}
				else if( func.sFunc == "DEAD"  )
				{
					if( ConditionUnitDead( (_CAMP)func.I(0), func.I(1) ) == false )
					{
						return false;
					}				
				}
				else if( func.sFunc == "ROUND"  )
				{
					if( ConditionRound( func.I(0), func.I(1) ) == false )
					{
						return false;
					}				
				}		
				else if( func.sFunc == "AFTER"  )
				{
					if( ConditionAfter( func.I(0),func.I(1)  ) == false )
					{
						return false;
					}				
				}
				else if( (func.sFunc  == "NODONE") || (func.sFunc=="NO") )  //檢查的事件沒有完成
				{
					if( ConditionNotDone( func.I(0) ) == false )
					{
						return false;
					}	
				}
				else if( func.sFunc == "DIST"  )
				{
					if( ConditionDist( func.I(0),func.I(1) ,func.I(2) ) == false )
					{
						return false;
					}	
				}

				else if( func.sFunc == "RATE"  )
				{
					if( ConditionRate( func.I(0) ) == false )
					{
						return false;
					}	
				}
				else if( func.sFunc == "INRECT"  )
				{
					if( ConditionInRect( func.I(0),func.I(1),func.I(2),func.I(3),func.I(4) ) == false )
					{
						return false;
					}	
				}
				else if( func.sFunc == "NORECT"  )
				{
					if( ConditionNoRect( func.I(0),func.I(1),func.I(2),func.I(3),func.I(4) ) == false )
					{
						return false;
					}	
				}
				else{
					Debug.LogError( string.Format( "Error-Can't find script cond func '{0}'" , func.sFunc ) );
				}
			}
		}
		return true;
	}

	// Check event
	public bool CheckEventCanRun( STAGE_EVENT evt )
	{
		// don't check during cmding
		if (cCMD.Instance.eCMDSTATUS != _CMD_STATUS._NONE)
			return false;

		cTextArray sCond = new cTextArray( );
		sCond.SetText( evt.s_CONDITION );
		// check all line . if one line success . this event check return true
		
		int nCol = sCond.GetMaxCol();
		for( int i= 0 ; i <nCol ; i++ )
		{
			if( CheckEventCondition( sCond.GetTextLine( i ) ) )
			{
				return true;
			}
		}
		return false;
	}


	// Condition
	// condition check 
	bool ConditionGO(  ) // always active
	{
		return true;
	}
	bool ConditionCombat( int nChar1 , int nChar2  )
	{
		if( BattleManager.Instance.IsBattlePhase() )
		{
			Panel_unit atker = Panel_StageUI.Instance.GetUnitByIdent( BattleManager.Instance.nAtkerID );
			Panel_unit defer = Panel_StageUI.Instance.GetUnitByIdent( BattleManager.Instance.nDeferID );
			int atkerid = 0;
			int deferid = 0;
			if( atker!=null )
			{
				atkerid = atker.CharID;
			}
			if( defer!=null )
			{
				deferid = defer.CharID;
			}
			
			if( nChar1 != 0 )
			{
				if( (atkerid!=nChar1) && ( deferid!=nChar1) )
				{
					return false;
				}
			}
			if( nChar2 != 0 )
			{
				if( (atkerid!=nChar2) && ( deferid!=nChar2) )
				{
					return false;
				}
			}
			return true;
			
		}
		
		return false;
	}


	//====================================================
	bool ConditionRate( int Rate )
	{
		int nRoll = Random.Range (0, 100);

		return ( Rate > nRoll );
	}

	bool ConditionAllDead( int nCampID )
	{
		// assign id
//		cCamp unit = GameDataManager.Instance.GetCamp( (_CAMP)nCampID );
//		if( unit != null )
//		{
//			return (unit.memLst.Count<=0) ;
//		}
		int nCount = GameDataManager.Instance.GetCampNum ( (_CAMP)nCampID  );
		return (nCount <= 0);

//		return true;
	}
	
	bool ConditionUnitDead( _CAMP nCampID ,int nCharID )
	{
		// assign id
//		cCamp camp = GameDataManager.Instance.GetCamp( (_CAMP)nCampID );
//		if( camp != null )
//		{
//			foreach( int no in  camp.memLst )
//			{
//				Panel_unit unit =  Panel_StageUI.Instance.GetUnitByIdent( no ); //this.IdentToUnit[ no ];
//				if( unit != null )
//				{
//					if( unit.CharID ==  nCharID )
//					{
//						return false;
//					}
//				}
//			}
//		}
		foreach( KeyValuePair< int , cUnitData > pair in GameDataManager.Instance.UnitPool ){
			if( pair.Value.eCampID != nCampID )
				continue;
			if( pair.Value.n_CharID != nCharID )
				continue;
			
			return false;
		}  
		return true;
	}
	
	bool ConditionRound( int nID , int nCamp =0)
	{
		//	if( GameDataManager.Instance.nRoundStatus != 0 )
		//		return false;
		if (GameDataManager.Instance.nRound >= nID) {
			if( nCamp == (int)GameDataManager.Instance.nActiveCamp ){
				return true;
			}
		}
		return false ;
	}
	bool ConditionAfter( int nID , int nRound  )
	{
		if( GameDataManager.Instance.EvtDonePool.ContainsKey( nID )  ){
			int nCompleteRound = GameDataManager.Instance.EvtDonePool[ nID ];
			
			return ( GameDataManager.Instance.nRound >= ( nCompleteRound + nRound ) );
		}
		return false;
	}
	bool ConditionNotDone( int nID  )
	{
		if( !GameDataManager.Instance.EvtDonePool.ContainsKey (nID) ) {
			return true;
		}
		return false;
	}


	bool ConditionDist( int nChar1 , int nChar2 , int nDist )
	{
		//check range
		Panel_unit unit1 = Panel_StageUI.Instance.GetUnitByCharID (nChar1);
		Panel_unit unit2 = Panel_StageUI.Instance.GetUnitByCharID (nChar2);
		if ((unit1 != null) && (unit2 != null)) {
			int dist = unit1.Loc.Dist( unit2.Loc );
			if( dist <= nDist )
			{
				return true;
			}
		}
		return false;
	}

	bool ConditionHp( int nChar1 , string op , float fValue )
	{
		cUnitData unit = GameDataManager.Instance.GetUnitDateByCharID ( nChar1 );
		if (unit != null) {
			float fPer = unit.GetHpPercent();

			return ConditionFloat( fPer , op , fValue );

//			if( op == "<"){
//				return (fPer < fValue);
//			}
//			else if( op == "<="){
//				return (fPer <= fValue);
//			}
//			else if( op == "=="){
//				return (fPer == fValue);
//			}
//			else if( op == "!="){
//				return (fPer != fValue);
//			}
//			else if( op == ">"){
//				return (fPer > fValue);
//			}
//			else if( op == ">="){
//				return (fPer >= fValue);
//			}
		}
		return false;
	}

	bool ConditionInRect( int nChar1 ,int x1 ,int y1 , int x2 , int y2  ) 
	{
		cUnitData unit = GameDataManager.Instance.GetUnitDateByCharID ( nChar1 );
		if (unit != null) {
			return MyTool.CheckInRect( unit.n_X , unit.n_Y , x1 , y1 , x2-x1 , y2-y1 );
		}
		return false;
	}

	bool ConditionNoRect( int nChar1 ,int x1 ,int y1 , int x2 , int y2  ) 
	{
		cUnitData unit = GameDataManager.Instance.GetUnitDateByCharID ( nChar1 );
		if (unit != null) {
			return (MyTool.CheckInRect( unit.n_X , unit.n_Y , x1 , y1 , x2 , y2 )== false);
		}
		return false;
	}

	//============================================================
	public bool ConditionFloat( float fVal_I , string op , float fVal_E )
	{
		if( op == "<"){
			return (fVal_I < fVal_E);
		}
		else if( op == "<="){
			return (fVal_I <= fVal_E);
		}
		else if( op == "=="){
			return (fVal_I == fVal_E);
		}
		else if( op == "!="){
			return (fVal_I != fVal_E);
		}
		else if( op == ">"){
			return (fVal_I > fVal_E);
		}
		else if( op == ">="){
			return (fVal_I >= fVal_E);
		}
		return false;
	}

	public bool ConditionInt( float nInt_I , string op , float nInt_E )
	{
		if( op == "<"){
			return (nInt_I < nInt_E);
		}
		else if( op == "<="){
			return (nInt_I <= nInt_E);
		}
		else if( op == "=="){
			return (nInt_I == nInt_E);
		}
		else if( op == "!="){
			return (nInt_I != nInt_E);
		}
		else if( op == ">"){
			return (nInt_I > nInt_E);
		}
		else if( op == ">="){
			return (nInt_I >= nInt_E);
		}
		return false;
	}

	//------------------
	// Stage Run
	//-----------------
	public void ParserScript( CTextLine line   )
	{
		List<cTextFunc> funcList =line.GetFuncList();
		foreach( cTextFunc func in funcList )
		{
			if( func.sFunc == "POPCHAR" )
			{
				StagePopUnitEvent evt = new StagePopUnitEvent ();
				evt.eCamp   = _CAMP._PLAYER;
				evt.nCharID = func.I( 0 );
				evt.nX		= func.I( 1 );
				evt.nY		= func.I( 2 );
				evt.nValue1 = func.I( 3 ); // pop num
				Panel_StageUI.Instance.OnStagePopUnitEvent( evt ); 
				//GameEventManager.DispatchEvent ( evt );
			}
			else if( func.sFunc == "POPMOB" )
			{
				StagePopUnitEvent evt = new StagePopUnitEvent ();
				evt.eCamp   = _CAMP._ENEMY;
				evt.nCharID = func.I( 0 );
				evt.nX		= func.I( 1 );
				evt.nY		= func.I( 2 );
				evt.nValue1 = func.I( 3 ); // pop num
				//test code 
				Panel_StageUI.Instance.OnStagePopUnitEvent( evt ); 
				//GameEventManager.DispatchEvent ( evt );
			}
			else if( func.sFunc == "POP" )
			{
				StagePopUnitEvent evt = new StagePopUnitEvent ();
				evt.eCamp 	= (_CAMP)func.I( 0 );
				evt.nCharID = func.I( 1 );
				evt.nX		= func.I( 2 );
				evt.nY		= func.I( 3 );
				evt.nValue1 = func.I( 4 ); // pop num
				Panel_StageUI.Instance.OnStagePopUnitEvent( evt ); 
				//GameEventManager.DispatchEvent ( evt );

			}
			else if( func.sFunc == "POPGROUP" )
			{
				StagePopGroupEvent evt = new StagePopGroupEvent ();
				//evt.eCamp 	= (_CAMP)func.I( 0 );

				evt.nCharID = func.I( 0 );
				evt.nLeaderCharID = func.I( 1 );
				evt.stX		= func.I( 2 );
				evt.stY		= func.I( 3 );
				evt.edX		= func.I( 4 );
				evt.edY		= func.I( 5 );
				evt.nPopType = func.I( 6 ); // pop num
				Panel_StageUI.Instance.OnStagePopGroupEvent( evt ); 
				//GameEventManager.DispatchEvent ( evt );
				
			}
			else if( func.sFunc == "TALK"  ) // open talkui
			{
				#if UNITY_EDITOR
				//	return ;
				#endif
				int nID = func.I( 0 );
				GameSystem.TalkEvent( nID );
			}
			else if( func.sFunc == "BGM"  )
			{
				int nID = func.I( 0 );
				// change bgm 
				GameSystem.PlayBGM ( nID );
			}
			else if( func.sFunc == "P_BGM"  )
			{
				int nID = func.I( 0 );
				// change bgm 
				if( nID> 0 ){
					GameDataManager.Instance.nPlayerBGM = nID;
				}
			}
			else if( func.sFunc == "E_BGM"  )
			{
				int nID = func.I( 0 );
				// change bgm 
				if( nID> 0 ){
					GameDataManager.Instance.nEnemyBGM = nID;
				}
			}
			else if( func.sFunc == "F_BGM"  )
			{
				int nID = func.I( 0 );
				// change bgm 
				if( nID> 0 ){
					GameDataManager.Instance.nFriendBGM = nID;
				}
			}


			else if( func.sFunc == "BGMPHASE"  )
			{
				// 0-正常 , 1-勝利 , 2-緊張 , 3-悲壯 ,4-壓迫
				int nPhase = func.I( 0 );
				GameDataManager.Instance.SetBGMPhase( nPhase );
				// play stage bgm
				Panel_StageUI.Instance.OnStageBGMEvent( new StageBGMEvent()  ); 
			}
			else if( func.sFunc == "HELPBGM"  ) // 支援登場
			{
				int nID = 130+func.I( 0 );// from 130 - 139
				GameSystem.PlayBGM ( nID );
			}
			else if( func.sFunc == "FORCEBGM"  ) // 敵軍登場
			{
				int nID = 140 + func.I( 0 ); // from 140-149
				GameSystem.PlayBGM ( nID );
			}
			else if( func.sFunc == "BOSSBGM"  ) // BOSS FIGHT BGM
			{
				int nID = 150 + func.I( 0 ); // from 150-159
				GameSystem.PlayBGM ( nID );
			}

			else if( func.sFunc == "SAY" )
			{
				TalkSayEvent evt = new TalkSayEvent();
				//evt.nType  = func.I(0);
				evt.nChar  = func.I(0);
				evt.nSayID = func.I(1);

				//Say( func.I(0), func.I(1) );
				GameEventManager.DispatchEvent ( evt  );
			}
			else if( func.sFunc == "SETCHAR" )
			{
				TalkSetCharEvent evt = new TalkSetCharEvent();
				evt.nType  = func.I(0);
				evt.nChar  = func.I(1);
				
				//Say( func.I(0), func.I(1) );
				GameEventManager.DispatchEvent ( evt  );
			}		
			else if( func.sFunc == "TALKDEAD" )
			{
				TalkDeadEvent evt = new TalkDeadEvent();			
				evt.nChar  = func.I(0);			
				GameEventManager.DispatchEvent ( evt  );
				// del unit . if it on stage
				Panel_StageUI.Instance.OnStageUnitDeadEvent( func.I(0)); // del unit auto
			}		
			else if( func.sFunc == "TALKSHAKE" )
			{
				TalkShakeEvent evt = new TalkShakeEvent();			
				evt.nChar  = func.I(0);
				GameEventManager.DispatchEvent ( evt  );
			}		
			else if( func.sFunc == "BACKBROUND") 
			{
				TalkBackGroundEvent evt = new TalkBackGroundEvent();
				//evt.nType = func.I(0);
				evt.nBackGroundID = func.I(0);
				GameEventManager.DispatchEvent ( evt  );
			}
			else if( func.sFunc  == "SAYEND") 
			{
				TalkSayEndEvent evt = new TalkSayEndEvent();
				//evt.nType = func.I(0);
				evt.nChar = func.I(0);
				GameEventManager.DispatchEvent ( evt  );
//				CloseBox( func.I(0), func.I(1) );
			}
			// stage event
			else if( func.sFunc  == "STAGEBGM") 
			{
				Panel_StageUI.Instance.OnStageBGMEvent( new StageBGMEvent()  ); 
				//GameEventManager.DispatchEvent ( new StageBGMEvent()  );				
			}

			else if( func.sFunc  == "ATTACK")  //  pop a group of mob
			{
				// this is bad idea
			
					StageBattleAttackEvent evt = new StageBattleAttackEvent();
					evt.nAtkCharID = func.I(0);
					evt.nDefCharID = func.I(1);
					evt.nAtkSkillID = func.I(2);
					Panel_StageUI.Instance.OnStageBattleAttackEvent( evt  ); 
					//GameEventManager.DispatchEvent ( evt  );
			
			}
			else if( func.sFunc  == "MOVETOUNIT")  //  pop a group of mob
			{
				StageMoveToUnitEvent evt = new StageMoveToUnitEvent();
				evt.nAtkCharID = func.I(0);
				evt.nDefCharID = func.I(1);
				//evt.nAtkSkillID = func.I(2);
				Panel_StageUI.Instance.OnStageMoveToUnitEvent( evt  ); 
				//GameEventManager.DispatchEvent ( evt  );
			}
			else if( func.sFunc  == "MOVE")  //  pop a group of mob
			{
				StageCharMoveEvent evt = new StageCharMoveEvent();
				evt.nIdent =0;
				evt.nCharID = func.I(0);
				evt.nX = func.I(1);
				evt.nY = func.I(2);
				//evt.nAtkSkillID = func.I(2);
				Panel_StageUI.Instance.OnStageCharMoveEvent( evt  ); 
				//GameEventManager.DispatchEvent ( evt  );

			}
			else if( func.sFunc  == "FX")  // Add buff
			{
				Panel_StageUI.Instance.OnStagePlayFX( func.I(0) , func.I(1));
			}	
			else if( func.sFunc  == "ADDBUFF")  // Add buff
			{
				Panel_StageUI.Instance.OnStageAddBuff( func.I(0) , func.I(1) , func.I(2) );
			}	
			else if( func.sFunc  == "DELBUFF")  // Add buff
			{
				Panel_StageUI.Instance.OnStageAddBuff( func.I(0) , func.I(1) , func.I(2) );
			}
			else if( func.sFunc  == "ADDHP")  // Add HP
			{
				Panel_StageUI.Instance.OnStageAddUnitValue( func.I(0) , 0 , func.F(1) );
			}
			else if( func.sFunc  == "ADDDEF")  // Add DEF
			{
				Panel_StageUI.Instance.OnStageAddUnitValue( func.I(0) , 1 ,func.F(1) );
			}
			else if( func.sFunc  == "ADDMP")  // Add HP
			{
				Panel_StageUI.Instance.OnStageAddUnitValue( func.I(0) , 2 , func.F(1) );
			}
			else if( func.sFunc  == "UNITDEAD") 
			{			
				int nCharID = func.I( 0 );		
				Panel_StageUI.Instance.OnStageUnitDeadEvent( func.I(0));

				// dont close auto.
			//	TalkDeadEvent evt = new TalkDeadEvent();			
			//	evt.nChar  = nCharID;			
			//	GameEventManager.DispatchEvent ( evt  );

			}
			else if( func.sFunc  == "DELUNIT") 
			{			
				StageDelUnitEvent evt = new StageDelUnitEvent ();
				//evt.eCamp = (_CAMP)func.I( 0 );
				evt.nCharID = func.I( 0 );
				evt.nDelType = 1; // always is leave mode


				Panel_StageUI.Instance.OnStageDelUnitEvent( evt  ); 
				//GameEventManager.DispatchEvent ( evt );

				// say end  event
				if( evt.nCharID != 0 ){
					TalkSayEndEvent tlkevt = new TalkSayEndEvent();				
					tlkevt.nChar = evt.nCharID;
					GameEventManager.DispatchEvent ( tlkevt  );
				}

			}
			else if( func.sFunc  == "JOIN") 
			{		
				int nCharID = func.I( 0 );
				GameDataManager.Instance.EnableStorageUnit(nCharID, true );
			}
			else if( func.sFunc  == "LEAVE") 
			{			
				int nCharID = func.I( 0 );
				GameDataManager.Instance.EnableStorageUnit(nCharID, false );
			}
			//不能刪除 等待執行 event. 會造成 讀檔上的麻煩
//			else if( func.sFunc  == "DELEVENT") 
//			{
//				Panel_StageUI.Instance.OnStageDelEventEvent( func.I( 0 ) );			
//			}
			else if( func.sFunc  == "UNITCAMP") 
			{			
				int nCharid =  func.I(0);
				int nCampid = func.I(1);

				Panel_StageUI.Instance.OnStageUnitCampEvent( nCharid , (_CAMP)nCampid );
			}
			else if( func.sFunc  == "POPMARK") 
			{
				Panel_StageUI.Instance.OnStagePopMarkEvent( func.I(0),func.I(1),func.I(2),func.I(3) );
			}
			else if( func.sFunc  == "CAMERACENTER") 
			{
				Panel_StageUI.Instance.OnStageCameraCenterEvent( func.I(0),func.I(1) );
			}
			else if( func.sFunc  == "SAI") 
			{
				// 改變單位索迪AI
				GameDataManager.Instance.SetUnitSearchAI( func.I(0),(_AI_SEARCH)func.I(1),func.I(2),func.I(3) ); 
			}
			else if( func.sFunc  == "CAI") 
			{
				// 改變單位攻擊AI
				GameDataManager.Instance.SetUnitComboAI( func.I(0),(_AI_COMBO)func.I(1) ); 
			}
			else if( func.sFunc  == "WIN") 
			{
				PanelManager.Instance.OpenUI(  Panel_Win.Name );
				//Panel_StageUI.Instance.bIsStageEnd = true;
			}
			else if( func.sFunc  == "LOST") 
			{
				PanelManager.Instance.OpenUI(  Panel_Lost.Name );
				//Panel_StageUI.Instance.bIsStageEnd = true;
			}

			else 
			{
				Debug.LogError( string.Format( "Error-Can't find script func '{0}'" , func.sFunc ) );
			}
		}
	}

	public void ParserStoryScript( CTextLine line )
	{


	}


	// Check skill trig effect
//	public bool CheckSkillCond(  string  strCond , cUnitData unit_i  , cUnitData unit_e )
//	{
//		if ( unit_i == null )
//			return false;
//
//		//cUnitData unit_e = GameDataManager.Instance.GetUnitDateByIdent ( unit.FightAttr.TarIdent );
//
//		cTextArray sCond = new cTextArray( );
//		sCond.SetText( strCond );
//		// check all line . if one line success . this event check return true
//		
//		int nCol = sCond.GetMaxCol();
//		for( int i= 0 ; i <nCol ; i++ )
//		{
//			if( CheckFightEventCondition( sCond.GetTextLine( i ) , unit_i , unit_e  ) )
//			{
//				return true;
//			}
//		}
//		return false;
//	}

//	public void RunSkillEffect( cUnitData atker  , cUnitData defer  , string strEffect , ref List<cHitResult>  pool )
//	{
//		if (atker == null )
//			return;
//		
//		//cUnitData unit_e = GameDataManager.Instance.GetUnitDateByIdent ( unit.FightAttr.TarIdent );
//		cTextArray Script = new cTextArray ();
//		Script.SetText (strEffect);
//
//		CacheHitResultPool = pool;
//
//		CTextLine line = Script.GetTextLine( 0 );
//		if( line != null )
//		{
//			
//			ParserScript( line , atker , defer );
//		}
//
//		CacheHitResultPool = null;
//
//	}

//	public bool CheckFightEventCondition( CTextLine line, cUnitData data_I , cUnitData data_E  )
//	{
//		if( line == null || data_I == null )
//			return false;
//		
//		List<cTextFunc> funcList =line.GetFuncList();
//		foreach( cTextFunc func in funcList )
//		{
//			if( func.sFunc == "GO" )
//			{
//				return  true;
//			}
//			
//			 if( func.sFunc == "HP_I"  )
//			{
//				
//				return false;
//			}
//			else if( func.sFunc == "HP_E"  )
//			{
//				
//				return false;
//			}
//			else if( func.sFunc == "MAR_I"  )
//			{
//				float f1 = data_I.GetMar();
//				float f2 = 0.0f;
//				if( func.S( 1 ) == "E" )
//				{
//					if( data_E != null ){
//						f2 = data_E.GetMar();
//					}else{
//						return false; // no enemy is false
//					}
//				}
//				else{
//					f2 = func.F( 1 );
//				}
//
//				if( ConditionFloat( f1 , func.S(0) ,f2  ) == false  ){
//					return false;
//				}
//			}
//			else if( func.sFunc == "MAR_E"  )
//			{
//				
//				return false;
//			}
//			else if( func.sFunc == "BUFF_I"  )
//			{
//				
//				return false;
//			}
//			else if( func.sFunc == "BUFF_E"  )
//			{
//				
//				return false;
//			}
//			else if( func.sFunc == "SCHOOL_I"  )
//			{
//				
//				return false;
//			}
//			else if( func.sFunc == "SCHOOL_E"  )
//			{
//				
//				return false;
//			}
//			else if( func.sFunc == "SKILL_I"  )
//			{
//				
//				return false;
//			}
//			else if( func.sFunc == "SKILL_E"  )
//			{
//				
//				return false;
//			}
//			else if( func.sFunc == "RANGE_E"  )
//			{
//				
//				return false;
//			}
//			else{
//				Debug.LogError( string.Format( "Error-Can't find script cond func '{0}'" , func.sFunc ) );
//			}
//		}
//		return true;
//	}

	public List< cEffect > CreateEffectPool( string str )
	{
		List< cEffect > pool = new List< cEffect > ();

		cTextArray sEff = new cTextArray( );
		sEff.SetText( str );
		// check all line . if one line success . this event check return true

	
		int nCol = sEff.GetMaxCol();
		for( int i= 0 ; i <nCol ; i++ )
		{
			CTextLine line = sEff.GetTextLine( i );
			List<cTextFunc> funcList = line.GetFuncList();
			foreach( cTextFunc func in funcList )
			{
				// Fight Effect
				if( func.sFunc  == "ADDBUFF_E") 				{
					pool.Add( new ADDBUFF_E( func.I(0) ) );
				}
				else if( func.sFunc  == "ADDBUFF_I") 				{
					pool.Add( new ADDBUFF_I( func.I(0) ) );
				}
				// Hit effect
				if( func.sFunc  == "HITBUFF_I") 
				{
					pool.Add( new HITBUFF_I( func.I(0) ) );
				}
				else if( func.sFunc  == "HITBUFF_E") 
				{
					pool.Add( new HITBUFF_E( func.I(0) ) );
				}
				else if( func.sFunc  == "HITHP_I") {
					pool.Add( new HITHP_I( func.F(0), func.I(1) ) );
				}
				else if( func.sFunc  == "HITHP_E") {
					pool.Add( new HITHP_E( func.F(0), func.I(1) ) );
				}
				else if( func.sFunc  == "HITMP_I") {
					pool.Add( new HITMP_I( func.F(0), func.I(1) ) );
				}
				else if( func.sFunc  == "HITMP_E") {
					pool.Add( new HITMP_E( func.F(0), func.I(1) ) );
				}
				else if( func.sFunc  == "HITSP_I") {
					pool.Add( new HITSP_I( func.I(0)) );
				}
				else if( func.sFunc  == "HITSP_E") {
					pool.Add( new HITSP_E( func.I(0) ) );
				}
				else if( func.sFunc  == "HITCP_I") {
					pool.Add( new HITCP_I( func.I(0) ) );
				}
				else if( func.sFunc  == "HITCP_E") {
					pool.Add( new HITCP_E( func.I(0) ) );
				}



				// skill upgrade
				else if( func.sFunc  == "UP_SKILL") 
				{
					pool.Add( new UP_SKILL( func.I(0) , func.I(1) ) );
				}

				// char data modify
				else if( func.sFunc  == "ADDHP_I") {
					pool.Add( new ADDHP_I( func.F(0), func.I(1) ) );
				}
				else if( func.sFunc  == "ADDHP_E") {
					pool.Add( new ADDHP_E( func.F(0), func.I(1) ) );
				}
				else if( func.sFunc  == "ADDMP_I") {
					pool.Add( new ADDMP_I( func.F(0), func.I(1) ) );
				}
				else if( func.sFunc  == "ADDMP_E") {
					pool.Add( new ADDMP_E( func.F(0), func.I(1) ) );
				}
				else if( func.sFunc  == "ADDSP_I") {
					pool.Add( new ADDSP_I( func.I(0)) );
				}
				else if( func.sFunc  == "ADDCP_I") {
					pool.Add( new ADDCP_I( func.I(0) ) );
				}
				else if( func.sFunc  == "ADDSP_E") {
					pool.Add( new ADDSP_E( func.I(0) ) );
				}
				else if( func.sFunc  == "ADDCP_E") {
					pool.Add( new ADDCP_I( func.I(0) ) );
				}
				else if( func.sFunc  == "ADDACTTIME_I") {
					pool.Add( new ADDACTTIME_I( func.I(0) ) );
				}
				else if( func.sFunc  == "ADDACTTIME_E") {
					pool.Add( new ADDACTTIME_E( func.I(0) ) );
				}

				// Attr
				else if( func.sFunc  == "ADD_MAR") {
					pool.Add( new ADD_MAR( func.F(0) ) );
				}
				else if( func.sFunc  == "ADD_MAR_DIFF") {
					pool.Add( new ADD_MAR_DIFF( func.F(0) , func.I(1) ) );
				}
				else if( func.sFunc  == "ADD_ATTACK_DIFF") {
					pool.Add( new ADD_ATTACK_DIFF( func.F(0), func.I(1) ) );
				}
				else if( func.sFunc  == "ADD_ATTACK") {
					pool.Add( new ADD_ATTACK( func.I(0) ) );
				}
				else if( func.sFunc  == "ADD_MAXDEF") {
					pool.Add( new ADD_MAXDEF( func.I(0) ) );
				}
				else if( func.sFunc  == "ADD_DEF_I") {
					pool.Add( new ADD_DEF_I( func.I(0) ) );
				}
				else if( func.sFunc  == "ADD_POWER") {
					pool.Add( new ADD_POWER( func.I(0) ) );
				}
				else if( func.sFunc  == "ADD_MAXHP") {
					pool.Add( new ADD_MAXHP( func.I(0) ) );
				}
				else if( func.sFunc  == "ADD_MAXMP") {
					pool.Add( new ADD_MAXMP( func.I(0) ) );
				}
				else if( func.sFunc  == "ADD_MAXSP") {
					pool.Add( new ADD_MAXSP( func.I(0) ) );
				}
				else if( func.sFunc  == "MUL_DRAINHP") {
					pool.Add( new MUL_DRAINHP( func.F(0) ) );
				}
				else if( func.sFunc  == "MUL_DRAINMP") {
					pool.Add( new MUL_DRAINMP( func.F(0) ) );
				}
				else if( func.sFunc  == "ADD_MOVE") 
				{
					pool.Add( new ADD_MOVE( func.I(0) ) );
				}
				else if( func.sFunc  == "MUL_DROP") {
					pool.Add( new MUL_DROP( func.F(0) ) );
				}
				else if( func.sFunc  == "MUL_BRUST") {
					pool.Add( new MUL_BRUST( func.F(0) ) );
				}
				else if( func.sFunc  == "MUL_DAMAGE") {
					pool.Add( new MUL_DAMAGE( func.F(0) ) );
				}
				else if( func.sFunc  == "MUL_ATTACK") {
					pool.Add( new MUL_ATTACK( func.F(0) ) );
				}
				else if( func.sFunc  == "MUL_DEF") {
					pool.Add( new MUL_DEF( func.F(0) ) );
				}
				else if( func.sFunc  == "MUL_POWER"){
					pool.Add( new MUL_POWER( func.F(0) ) );
				}
				else if( func.sFunc  == "MUL_MAXHP") {
					pool.Add( new MUL_MAXHP( func.F(0) ) );
				}
				else if( func.sFunc  == "MUL_MAXMP") {
					pool.Add( new MUL_MAXMP( func.F(0) ) );
				}
				else if( func.sFunc  == "MUL_MAXSP") {
					pool.Add( new MUL_MAXSP( func.F(0) ) );
				}
				
				else if( func.sFunc  == "MUL_MPCOST") {
					pool.Add( new MUL_MPCOST( func.F(0) ) );
				}
				// immune
				else if( func.sFunc  == "IMMUNE") {
					pool.Add( new IMMUNE( func.I(0) ) );
				}

				//=============== Tag 
				else if( func.sFunc  == "TAG_CHARGE") {  // 
					pool.Add( new TAG_CHARGE( ) );
				}
				else if( func.sFunc  == "TAG_NODIE") {  // 不死身
					pool.Add( new TAG_NODIE( ) );
				}
				else if( func.sFunc  == "TAG_SILENCE") {  // 被封言
					pool.Add( new TAG_SILENCE( ) );
				}
				//=============== fight status
				else if( func.sFunc  == "IS_DODGE") { // 必閃
					pool.Add( new IS_DODGE( ) );
					//pool.Add( new IS_UNIT_STATUS( _FIGHTSTATE._DODGE ) );
				}
				else if( func.sFunc  == "IS_CIRIT") {  // 
					pool.Add( new IS_CIRIT( ) );
				}
				else if( func.sFunc  == "IS_MERCY") {  //手加減
					pool.Add( new IS_MERCY( ) );
				}
				else if( func.sFunc  == "IS_GUARD") { // 被防衛
					pool.Add( new IS_GUARD( ) );
				}
				else if( func.sFunc  == "IS_THROUGH") { // 攻擊穿透
					pool.Add( new IS_THROUGH( ) );
				}
				else if( func.sFunc  == "IS_MISS") { // 失誤
					pool.Add( new IS_MISS( ) );
				}
				else if( func.sFunc  == "IS_COMBO") { // 失誤
					pool.Add( new IS_COMBO( ) );
				}
				else if( func.sFunc  == "IS_BROKEN") { // 破防
					pool.Add( new IS_BROKEN( ) );
				}
				else if( func.sFunc  == "IS_RETURN") { // 傷害轉彈
					pool.Add( new IS_RETURN( ) );
				}
				else if( func.sFunc  == "IS_COPY") { // 
					pool.Add( new IS_COPY( ) );
				}
				else if( func.sFunc  == "IS_TWICE") { // 
					pool.Add( new IS_TWICE( ) );
				}


				//===

			
			}

		}

		return pool;
	}

	public cEffectCondition CreateEffectCondition( string str )
	{
		cEffectCondition pCon = new cEffectCondition ();
		cTextArray sCond = new cTextArray( );
		sCond.SetText( str );
		int nCol = sCond.GetMaxCol();
		for (int i= 0; i <nCol; i++) {
			CTextLine line = sCond.GetTextLine (i);
			//List<cTextFunc> funcList = line.GetFuncList ();
			pCon.Add( line );
		}
		return pCon;

	}
}
