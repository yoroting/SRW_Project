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

	List<cHitResult> CacheHitResultPool;


	public bool CheckEventCondition( CTextLine line  )
	{
		if( line == null )
			return false;
		
		List<cTextFunc> funcList =line.GetFuncList();
		foreach( cTextFunc func in funcList )
		{
			if( func.sFunc == "GO" )
			{
				if( ConditionGO( ) == false )
				{
					return false;
				}	
			}
			else if( func.sFunc == "ALLDEAD" )
			{
				if( ConditionAllDead( func.I(0) ) == false )
				{
					return false;
				}				
			}
			else if( func.sFunc == "DEAD"  )
			{
				if( ConditionUnitDead( func.I(0), func.I(1) ) == false )
				{
					return false;
				}				
			}
			else if( func.sFunc == "ROUND"  )
			{
				if( ConditionRound( func.I(0) ) == false )
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
			else if( func.sFunc == "COMBAT"  )
			{
				if( ConditionCombat( func.I(0),func.I(1)  ) == false )
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
			else if( func.sFunc == "HP"  )
			{
				if( ConditionHp( func.I(0), func.S(1)  ,func.F(2) ) == false )
				{
					return false;
				}	
			}


			else{
				Debug.LogError( string.Format( "Error-Can't find script cond func '{0}'" , func.sFunc ) );
			}
		}
		return true;
	}

	// Check event
	bool CheckEventCanRun( STAGE_EVENT evt )
	{
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
	
	bool ConditionAllDead( int nCampID )
	{
		// assign id
		cCamp unit = GameDataManager.Instance.GetCamp( (_CAMP)nCampID );
		if( unit != null )
		{
			return (unit.memLst.Count<=0) ;
		}
		return true;
	}
	
	bool ConditionUnitDead( int nCampID ,int nCharID )
	{
		// assign id
		cCamp camp = GameDataManager.Instance.GetCamp( (_CAMP)nCampID );
		if( camp != null )
		{
			foreach( int no in  camp.memLst )
			{
				Panel_unit unit =  Panel_StageUI.Instance.GetUnitByIdent( no ); //this.IdentToUnit[ no ];
				if( unit != null )
				{
					if( unit.CharID ==  nCharID )
					{
						return false;
					}
				}
			}
		}
		return true;
	}
	
	bool ConditionRound( int nID )
	{
		//	if( GameDataManager.Instance.nRoundStatus != 0 )
		//		return false;
		
		return (GameDataManager.Instance.nRound >=nID ) ;
	}
	bool ConditionAfter( int nID , int nRound  )
	{
		if( GameDataManager.Instance.EvtDonePool.ContainsKey( nID )  ){
			int nCompleteRound = GameDataManager.Instance.EvtDonePool[ nID ];
			
			return ( GameDataManager.Instance.nRound >= ( nCompleteRound + nRound ) );
		}
		return false;
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
	bool ConditionDist( int nChar1 , int nChar2 , int nDist )
	{
		// don't check suring cmd
		if (cCMD.Instance.eCMDSTATUS != _CMD_STATUS._NONE)
			return false;

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
			float fPer = (float)unit.n_HP / (float)unit.GetMaxHP();

			if( op == "<"){
				return (fPer < fValue);
			}
			else if( op == "<="){
				return (fPer <= fValue);
			}
			else if( op == "=="){
				return (fPer == fValue);
			}
			else if( op == "!="){
				return (fPer != fValue);
			}
			else if( op == ">"){
				return (fPer > fValue);
			}
			else if( op == ">="){
				return (fPer >= fValue);
			}
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
	public void ParserScript( CTextLine line ,cUnitData data_I=null , cUnitData data_E=null )
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
				GameEventManager.DispatchEvent ( evt );
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

				GameEventManager.DispatchEvent ( evt );
			}
			else if( func.sFunc == "POP" )
			{
				StagePopUnitEvent evt = new StagePopUnitEvent ();
				evt.eCamp 	= (_CAMP)func.I( 0 );
				evt.nCharID = func.I( 1 );
				evt.nX		= func.I( 2 );
				evt.nY		= func.I( 3 );
				evt.nValue1 = func.I( 4 ); // pop num
				GameEventManager.DispatchEvent ( evt );

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
			else if( func.sFunc == "CHANGEBACK") 
			{
				
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
				GameEventManager.DispatchEvent ( new StageBGMEvent()  );				
			}

			else if( func.sFunc  == "POPGROUP")  //  pop a group of mob
			{
				
			}
			else if( func.sFunc  == "ATTACK")  //  pop a group of mob
			{

				StageBattleAttackEvent evt = new StageBattleAttackEvent();
				evt.nAtkCharID = func.I(0);
				evt.nDefCharID = func.I(1);
				evt.nAtkSkillID = func.I(2);
				GameEventManager.DispatchEvent ( evt  );
			}
			else if( func.sFunc  == "DELUNIT") 
			{			
				StageDelUnitEvent evt = new StageDelUnitEvent ();
				//evt.eCamp = (_CAMP)func.I( 0 );
				evt.nCharID = func.I( 0 );
				GameEventManager.DispatchEvent ( evt );
			}
			else if( func.sFunc  == "WIN") 
			{
				PanelManager.Instance.OpenUI(  Panel_Win.Name );
			}
			else if( func.sFunc  == "LOST") 
			{
				PanelManager.Instance.OpenUI(  Panel_Lost.Name );
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
				if( func.sFunc  == "ADDBUFF_E") 
				{
					int nBuffID = func.I(0);
					pool.Add( new ADDBUFF_E( nBuffID ) );
				//	CacheHitResultPool.Add( new cHitResult(cHitResult._TYPE._ADDBUFF , data_E.n_Ident , nBuffID )  );
					
				}
				else if( func.sFunc  == "ADDBUFF_I") 
				{
					int nBuffID = func.I(0);
					pool.Add( new ADDBUFF_I( nBuffID ) );
				//	CacheHitResultPool.Add( new cHitResult(cHitResult._TYPE._ADDBUFF , data_I.n_Ident , nBuffID )  );
					
				}
				// Attr
				else if( func.sFunc  == "ADD_MAR") 
				{
					float f = func.F(0);
					pool.Add( new ADD_MAR( f ) );
				}
				else if( func.sFunc  == "ADD_MAR_DIFF") 
				{
					float f = func.F(0);
					pool.Add( new ADD_MAR_DIFF( f ) );
				}

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
