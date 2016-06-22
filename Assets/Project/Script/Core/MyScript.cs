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
	public static bool bParsing = false;
    public static int nCheckIdent= 0;   // 指定檢查 執行事件者

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
            else if (func.sFunc == "BUFF")
            {
                if ( ConditionUnitBuff( func.I(0),func.I(1), func.I(2) ) == false)
                {
                    return false;
                }
            }
            else if (func.sFunc == "AFTER")
            {
                if (ConditionAfter(func.I(0), func.I(1), func.I(2)) == false)
                {
                    return false;
                }
            }
            else if (func.sFunc == "DONE")
            {
                if (ConditionDone(func.I(0), func.I(1)) == false)
                {
                    return false;
                }
            }
            else if ((func.sFunc == "NODONE") || (func.sFunc == "NO"))  //檢查的事件沒有完成
            {
                if (ConditionNotDone(func.I(0)) == false)
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
                    if (func.I(0) > 2) {
                        Debug.LogErrorFormat("Script check error in {0} with param1 = {1}", func.sFunc , func.I(0) );
                    }

					if( ConditionUnitDead( func.I(0), func.I(1) ) == false )
					{
						return false;
					}				
				}
				else if( func.sFunc == "ALIVE" || func.sFunc == "NODEAD" )
				{
                    if (func.I(0) > 2)
                    {
                        Debug.LogErrorFormat("Script check error in {0} with param1 = {1}", func.sFunc, func.I(0));
                    }
                    if ( ConditionUnitAlive( (_CAMP)func.I(0), func.I(1) ) == false )
					{
						return false;
					}				
				}
				else if( func.sFunc == "ROUND" || func.sFunc == "R")
				{
					if( ConditionRound( func.I(0), func.I(1)) == false )
					{
						return false;
					}				
				}		
				
				else if( func.sFunc == "DIST"  )
				{
					if( ConditionDist( func.I(0),func.I(1) ,func.I(2), func.I(3)) == false )
					{
						return false;
					}	
				}

                else if (func.sFunc == "NODIST")
                {
                    if (ConditionNoDist(func.I(0), func.I(1), func.I(2), func.I(3)) == false)
                    {
                        return false;
                    }
                }

                else if ( func.sFunc == "RATE"  )
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
                else if (func.sFunc == "INPOS")
                {
                    if (ConditionInPos(func.I(0), func.I(1), func.I(2) ) == false)
                    {
                        return false;
                    }
                }
                else if (func.sFunc == "NOPOS")
                {
                    if (ConditionNoPos(func.I(0), func.I(1), func.I(2) ) == false)
                    {
                        return false;
                    }
                }
                else if (func.sFunc == "TRIG_CHAR")
                {
                    if (ConditionTrigChar(func.I(0)) == false)
                    {
                        return false;
                    }
                }
                else if (func.sFunc == "TRIG_CAMP")
                {
                    if (ConditionTrigCamp(func.I(0)) == false)
                    {
                        return false;
                    }
                }
                else if (func.sFunc == "COUNT")
                {
                    _CAMP campid = (_CAMP)func.I(0);
                    int count = GameDataManager.Instance.GetCampNum(campid);
                    int nNum = func.I(2);
                    if (MyScript.Instance.ConditionInt(count, func.S(1), func.I(2)) == false)
                    {
                        return false;       // always fail
                    }

                }
                else
                {
					Debug.LogError( string.Format( "Error-Can't find script cond func '{0}'" , func.sFunc ) );
				}
			}
		}
		return true;
	}

    public bool IsCheckIdent(int nIdent)
    {
        return (nCheckIdent == 0) || (nCheckIdent == nIdent);
    }

    // Check event
    public bool CheckEventCanRun(STAGE_EVENT evt, int checkIdent = 0 )
	{
		// don't check during cmding
		if (cCMD.Instance.eCMDSTATUS != _CMD_STATUS._NONE)
			return false;

        nCheckIdent = checkIdent; // check target

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
    public bool ConditionGO(  ) // always active
	{
		return true;
	}
    public bool ConditionCombat( int nChar1 , int nChar2  )
	{
		if( BattleManager.Instance.IsDamagePhase() ) // only check 有傷害的戰鬥才算
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
    public bool ConditionRate( int Rate )
	{
		int nRoll = Random.Range (0, 100);

		return ( Rate > nRoll );
	}

    public bool ConditionAllDead( int nCampID )
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

    public bool ConditionUnitDead( int nCampID ,int nCharID )
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
		if (nCharID == 0) {
			Debug.LogErrorFormat( "script check dead err in char{0}", nCharID );
			return false;
		}

		foreach( KeyValuePair< int , cUnitData > pair in GameDataManager.Instance.UnitPool ){
            if (pair.Value.n_CharID != nCharID)
                continue;
          

            if (nCampID == -1) {
                return false;
            }            

            if ( pair.Value.eCampID != (_CAMP)nCampID )
				continue;
			return false;
		}  
		return true;
	}


    public bool ConditionUnitAlive( _CAMP nCampID ,int nCharID )
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
			
            if (!IsCheckIdent(pair.Value.n_Ident))
                continue;
            return true;
		}  
		return false;
	}

    public bool ConditionRound( int nID , int nCamp =0)
	{
		//	if( GameDataManager.Instance.nRoundStatus != 0 )
		//		return false;
		if (GameDataManager.Instance.nRound >= nID) {
            if (nCamp == 0)  // camp = 0 代表 兩方陣營都可觸發
                return true;
            else
            {
                if ((nCamp == (int)GameDataManager.Instance.nActiveCamp))
                {
                    return true;
                }
            }
            
		}
		return false ;
	}
    public bool ConditionAfter( int nID , int nRound, int nCamp = 0 ) // -1 = any camp
	{
		if( GameDataManager.Instance.EvtDonePool.ContainsKey( nID )  ){
			int nCompleteRound = GameDataManager.Instance.EvtDonePool[ nID ];
            if (nCamp > 0)  // camp = 0 代表 兩方陣營都可觸發
            {
                if (nCamp != (int)GameDataManager.Instance.nActiveCamp) {
                    return false;
                }
            }
//            if (nCamp == (int)GameDataManager.Instance.nActiveCamp) // 這樣寫將導致怪物急殺 我方所觸發的事件不能觸發
  //          {
            return (GameDataManager.Instance.nRound >= (nCompleteRound + nRound));
    //        }
		}
		return false;
	}

    public bool ConditionDone( int nID , int nRound )
	{
		if( GameDataManager.Instance.EvtDonePool.ContainsKey( nID )  ){
			if( nRound == 0 )
				return true;
			//========================
			int nCompleteRound = GameDataManager.Instance.EvtDonePool[ nID ];			
			return ( nCompleteRound <= ( nRound ) );
		}
		return false;
	}

    public bool ConditionNotDone( int nID  )
	{
		if( !GameDataManager.Instance.EvtDonePool.ContainsKey (nID) ) {
			return true;
		}
		return false;
	}


    public bool ConditionDist( int nChar1 , int nChar2 , int nMax  , int nMin = 0)
	{
		//check range
		Panel_unit unit1 = Panel_StageUI.Instance.GetUnitByCharID (nChar1);
		//Panel_unit unit2 = Panel_StageUI.Instance.GetUnitByCharID (nChar2);
		if ((unit1 != null) ) {
            foreach (KeyValuePair<int, cUnitData> pair in GameDataManager.Instance.UnitPool)
            {
                if (pair.Value.n_CharID != nChar2)
                    continue;

                int nDist = iVec2.Dist(unit1.Loc.X, unit1.Loc.Y, pair.Value.n_X, pair.Value.n_Y);
                if (nDist <= nMax && nDist >= nMin)
                {
                    return true;
                }
            }

		}
		return false;
	}
    public bool ConditionNoDist(int nChar1, int nChar2, int nMax, int nMin = 0)
    {
        //check range
        Panel_unit unit1 = Panel_StageUI.Instance.GetUnitByCharID(nChar1);
        //Panel_unit unit2 = Panel_StageUI.Instance.GetUnitByCharID (nChar2);
        if ((unit1 != null))
        {
            foreach (KeyValuePair<int, cUnitData> pair in GameDataManager.Instance.UnitPool)
            {
                if (pair.Value.n_CharID != nChar2)
                    continue;

                int nDist = iVec2.Dist(unit1.Loc.X, unit1.Loc.Y, pair.Value.n_X, pair.Value.n_Y);
                if (nDist > nMax || nDist < nMin)
                {
                    return true;
                }
            }

        }
        return false;
    }
    public bool ConditionHp( int nChar1 , string op , float fValue )
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

    public bool ConditionUnitBuff(int nChar1, int buffid , int num )
    {
        foreach (KeyValuePair<int, cUnitData> pair in GameDataManager.Instance.UnitPool)
        {
            if (pair.Value.n_CharID != nChar1)
                continue;

            if (pair.Value.Buffs.HaveBuff(buffid, num)) {
                return true;
            }
        }
        return false;
    }

    public bool ConditionInRect( int nChar1 ,int x1 ,int y1 , int x2 , int y2  ) 
	{
        int sx = x1 < x2 ? x1 : x2;
        int sy = y1 < y2 ? y1 : y2;
        int ex = x1 > x2 ? x1 : x2;
        int ey = y1 > y2 ? y1 : y2;
        int w = x2 - x1;
        int h = y2 - y1;

        foreach (KeyValuePair<int, cUnitData> pair in GameDataManager.Instance.UnitPool)
        {          
            if (pair.Value.n_CharID != nChar1)
                continue;
            if (MyTool.CheckInRect(pair.Value.n_X, pair.Value.n_Y, sx, sy, w, h))
            {
                return true;
            }
        }

  //      cUnitData unit = GameDataManager.Instance.GetUnitDateByCharID ( nChar1 );
		//if (unit != null) {
		//	return MyTool.CheckInRect( unit.n_X , unit.n_Y , x1 , y1 , x2-x1 , y2-y1 );
		//}
		return false;
	}

    public bool ConditionNoRect( int nChar1 ,int x1 ,int y1 , int x2 , int y2  ) 
	{

        return ConditionInRect(nChar1 ,x1 , y1 , x2 , y2 ) == false ;
        //cUnitData unit = GameDataManager.Instance.GetUnitDateByCharID ( nChar1 );
        //if (unit != null) {
        //	return (MyTool.CheckInRect( unit.n_X , unit.n_Y , x1 , y1 , x2 , y2 )== false);
        //}
//        return false;
	}

    public bool ConditionInPos(int nChar1, int x1, int y1)
    {
        cUnitData unit = GameDataManager.Instance.GetUnitDateByCharID(nChar1);
        if (unit != null)
        {
            return ((unit.n_X== x1)&&(unit.n_Y==y1));
        }
        return false;
    }

    public bool ConditionNoPos(int nChar1, int x1, int y1)
    {
        return ConditionInPos(nChar1,x1,y1) == false;         
    }

    public bool ConditionTrigChar(int nChar1)
    {
        if (nCheckIdent == 0)
            return false;

        cUnitData unit = GameDataManager.Instance.GetUnitDateByIdent( nCheckIdent );
        if (unit != null) {
            return (unit.n_CharID == nChar1);
        }
        return false;
    }

    public bool ConditionTrigCamp(int nCampID )
    {
        if (nCheckIdent == 0)
            return false;

        cUnitData unit = GameDataManager.Instance.GetUnitDateByIdent(nCheckIdent);
        if (unit != null)
        {
            return (unit.eCampID == (_CAMP)nCampID );
        }
        return false;
    }

    

    public bool ConditionCount(int campid , string op , int nNum )
    {       
        int count = GameDataManager.Instance.GetCampNum((_CAMP)campid);     
        if (MyScript.Instance.ConditionInt(count, op , nNum) == true)
        {
            return true;       
        }
        return false;// always fail
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
		bParsing = true;
		List<cTextFunc> funcList =line.GetFuncList();
		foreach( cTextFunc func in funcList )
		{
            if (func.sFunc == "POPCHAR" || func.sFunc == "POPC")
            {
                StagePopUnitEvent evt = new StagePopUnitEvent();
                evt.eCamp = _CAMP._PLAYER;
                evt.nCharID = func.I(0);
                evt.nX = func.I(1);
                evt.nY = func.I(2);
                evt.nValue1 = func.I(3); // pop num
                evt.nRadius = func.I(4); // random range

                Panel_StageUI.Instance.OnStagePopUnitEvent(evt);
                //GameEventManager.DispatchEvent ( evt );
            }
            else if (func.sFunc == "POPMOB" || func.sFunc == "POPM")
            {
                StagePopUnitEvent evt = new StagePopUnitEvent();
                evt.eCamp = _CAMP._ENEMY;
                evt.nCharID = func.I(0);
                evt.nX = func.I(1);
                evt.nY = func.I(2);
                evt.nValue1 = func.I(3); // pop num
                evt.nRadius = func.I(4); // random range
                //test code 
                Panel_StageUI.Instance.OnStagePopUnitEvent(evt);
                //GameEventManager.DispatchEvent ( evt );
            }
            else if (func.sFunc == "POPFRIEND" || func.sFunc == "POPF")
            {
                StagePopUnitEvent evt = new StagePopUnitEvent();
                evt.eCamp = _CAMP._FRIEND;
                evt.nCharID = func.I(0);
                evt.nX = func.I(1);
                evt.nY = func.I(2);
                evt.nValue1 = func.I(3); // pop num
                evt.nRadius = func.I(4); // random range
                //test code 
                Panel_StageUI.Instance.OnStagePopUnitEvent(evt);
                //GameEventManager.DispatchEvent ( evt );
            }
            else if (func.sFunc == "POP")
            {
                StagePopUnitEvent evt = new StagePopUnitEvent();
                evt.eCamp = (_CAMP)func.I(0);
                evt.nCharID = func.I(1);
                evt.nX = func.I(2);
                evt.nY = func.I(3);
                evt.nValue1 = func.I(4); // pop num
                evt.nRadius = func.I(5); // random range
                Panel_StageUI.Instance.OnStagePopUnitEvent(evt);
                //GameEventManager.DispatchEvent ( evt );

            }
            else if (func.sFunc == "POPGROUP" || func.sFunc == "POPG")
            {
                StagePopGroupEvent evt = new StagePopGroupEvent();
                //evt.eCamp 	= (_CAMP)func.I( 0 );

                evt.nCharID = func.I(0);
                evt.nLeaderCharID = func.I(1);
                evt.stX = func.I(2);
                evt.stY = func.I(3);
                evt.edX = func.I(4);
                evt.edY = func.I(5);
                evt.nPopType = func.I(6); // pop num
                Panel_StageUI.Instance.OnStagePopGroupEvent(evt);
                //GameEventManager.DispatchEvent ( evt );

            }
            else if (func.sFunc == "SCREEN") // open  screen ui
            {
                Panel_Screen.Open(func.I(0));
            }
            else if (func.sFunc == "TALK") // open talkui
            {
#if UNITY_EDITOR
                //	return ;
#endif
                int nID = func.I(0);
                GameSystem.TalkEvent(nID);
            }
            else if (func.sFunc == "BGM")
            {
                int nID = func.I(0);
                // change bgm 
                GameSystem.PlayBGM(nID);
            }
            else if (func.sFunc == "P_BGM")
            {
                int nID = func.I(0);
                // change bgm 
                if (nID > 0)
                {
                    GameDataManager.Instance.nPlayerBGM = nID;
                }
            }
            else if (func.sFunc == "E_BGM")
            {
                int nID = func.I(0);
                // change bgm 
                if (nID > 0)
                {
                    GameDataManager.Instance.nEnemyBGM = nID;
                }
            }
            else if (func.sFunc == "F_BGM")
            {
                int nID = func.I(0);
                // change bgm 
                if (nID > 0)
                {
                    GameDataManager.Instance.nFriendBGM = nID;
                }
            }


            else if (func.sFunc == "BGMPHASE")
            {
                // 0-正常 , 1-勝利 , 2-緊張 , 3-悲壯 ,4-壓迫
                int nPhase = func.I(0);
                GameDataManager.Instance.SetBGMPhase(nPhase);
                // play stage bgm
                Panel_StageUI.Instance.OnStageBGMEvent(new StageBGMEvent());
            }
            else if (func.sFunc == "HELPBGM") // 支援登場
            {
                int nID = 130 + func.I(0);// from 130 - 139
                GameSystem.PlayBGM(nID);
            }
            else if (func.sFunc == "FORCEBGM") // 敵軍登場
            {
                int nID = 140 + func.I(0); // from 140-149
                GameSystem.PlayBGM(nID);
            }
            else if (func.sFunc == "BOSSBGM") // BOSS FIGHT BGM
            {
                int nID = 150 + func.I(0); // from 150-159
                GameSystem.PlayBGM(nID);
            }
            else if (func.sFunc == "CHARBGM") // 1i z;4
            {
                int nCharID = func.I(0);
                CHARS pData = ConstDataManager.Instance.GetRow<CHARS>(nCharID);
                if (pData != null)
                {
                    if (pData.n_BGM != 0)
                    {
                        GameSystem.PlayBGM(pData.n_BGM);
                    }
                }
            }
            else if (func.sFunc == "CHARFACE") //變更角色FACE
            {
                int nCharID = func.I(0); // old
                int nFaceID = func.I(1); // new 
                GameDataManager.Instance.SetCharFace(nCharID, nFaceID);

                TalkFaceEvent evt = new TalkFaceEvent();
                evt.nChar = func.I(0);
                evt.nFaceID = func.I(1);
                GameEventManager.DispatchEvent(evt);
            }

            else if (func.sFunc == "SAY")
            {
                TalkSayEvent evt = new TalkSayEvent();
                //evt.nType  = func.I(0);
                evt.nChar = func.I(0);
                evt.nSayID = func.I(1);
                evt.nReplaceID = func.I(2);
                evt.nReplaceType = func.I(3);
                //Say( func.I(0), func.I(1) );

                GameEventManager.DispatchEvent(evt);
            }
            else if (func.sFunc == "SETCHAR")
            {
                TalkSetCharEvent evt = new TalkSetCharEvent();
                evt.nType = func.I(0);
                evt.nChar = func.I(1);
                evt.nReplaceID = func.I(2);
                evt.nReplaceType = func.I(3);
                //Say( func.I(0), func.I(1) );
                GameEventManager.DispatchEvent(evt);
            }
            else if (func.sFunc == "SETALL")
            {
                // sayend auto
                TalkSayEndEvent evt = new TalkSayEndEvent();
                evt.nChar = 0;
                GameEventManager.DispatchEvent(evt);
                //set char 0
                TalkSetCharEvent evt1 = new TalkSetCharEvent();
                evt1.nType = 0;
                evt1.nChar = func.I(0);
                GameEventManager.DispatchEvent(evt1);
                //set char 1
                TalkSetCharEvent evt2 = new TalkSetCharEvent();
                evt2.nType = 1;
                evt2.nChar = func.I(1);
                GameEventManager.DispatchEvent(evt2);

            }
            else if (func.sFunc == "TALKDEAD")
            {
                TalkDeadEvent evt = new TalkDeadEvent();
                evt.nChar = func.I(0);
                evt.nSoundID = func.I(1);
                GameEventManager.DispatchEvent(evt);
                // del unit . if it on stage
                Panel_StageUI.Instance.OnStageUnitDeadEvent(func.I(0)); // del unit auto
            }
            else if (func.sFunc == "TALKSHAKE")
            {
                TalkShakeEvent evt = new TalkShakeEvent();
                evt.nChar = func.I(0);
                evt.nSoundID = func.I(1);
                GameEventManager.DispatchEvent(evt);
            }
            else if (func.sFunc == "BACKGROUND" || func.sFunc == "TALKBG")
            {
                TalkBackGroundEvent evt = new TalkBackGroundEvent();
                //evt.nType = func.I(0);
                evt.nBackGroundID = func.I(0);
                evt.nSoundID = func.I(1);
                evt.nType = func.I(2);
                GameEventManager.DispatchEvent(evt);
            }
            else if (func.sFunc == "SAYEND")
            {
                TalkSayEndEvent evt = new TalkSayEndEvent();
                //evt.nType = func.I(0);
                evt.nChar = func.I(0);
                GameEventManager.DispatchEvent(evt);
                //				CloseBox( func.I(0), func.I(1) );
            }
            // stage event
            else if (func.sFunc == "STAGEBGM")
            {
                Panel_StageUI.Instance.OnStageBGMEvent(new StageBGMEvent());
                //GameEventManager.DispatchEvent ( new StageBGMEvent()  );				
            }

            else if (func.sFunc == "ATTACK")  //  pop a group of mob
            {
                // this is bad idea

                StageBattleAttackEvent evt = new StageBattleAttackEvent();
                evt.nAtkCharID = func.I(0);
                evt.nDefCharID = func.I(1);
                evt.nAtkSkillID = func.I(2);
                evt.nNum = func.I(3);
                evt.nResult = func.I(4); // 0- normal , 1- dodge, 2-miss , 3-shield , 4 - GUARD
                evt.nVar1 = func.I(5);
                evt.nVar2 = func.I(6);

                Panel_StageUI.Instance.OnStageBattleAttackEvent(evt);
                //GameEventManager.DispatchEvent ( evt  );

            }
            //			else if( func.sFunc  == "CAST")  //  pop a group of mob
            //			{
            //				// this is bad idea
            //				
            //				StageBattleCastEvent evt = new StageBattleCastEvent();
            //				evt.nAtkCharID = func.I(0);
            //				evt.nDefCharID = func.I(1);
            //				evt.nAtkSkillID = func.I(2);
            //				Panel_StageUI.Instance.OnStageBattleCastEvent( evt  ); 
            //				//GameEventManager.DispatchEvent ( evt  );
            //				
            //			}

            else if (func.sFunc == "CAST")  //  pop a group of mob
            {
                // this is bad idea

                StageBattleCastEvent evt = new StageBattleCastEvent();
                evt.nAtkCharID = func.I(0);
                evt.nDefCharID = func.I(1);
                evt.nAtkSkillID = func.I(2);
                evt.nResult = func.I(3); // fight result , // 0- normal , 1- dodge, 2-miss , 3-shield,4-guard
                evt.nVar1 = func.I(4);
                evt.nVar2 = func.I(5);

                Panel_StageUI.Instance.OnStageBattleCastEvent(evt);
                //GameEventManager.DispatchEvent ( evt  );

            }

            else if (func.sFunc == "MOVETOUNIT")  //  pop a group of mob
            {
                StageMoveToUnitEvent evt = new StageMoveToUnitEvent();
                evt.nAtkCharID = func.I(0);
                evt.nDefCharID = func.I(1);
                //evt.nAtkSkillID = func.I(2);
                Panel_StageUI.Instance.OnStageMoveToUnitEvent(evt);
                //GameEventManager.DispatchEvent ( evt  );
            }
            else if (func.sFunc == "MOVE")  //  pop a group of mob
            {
                StageCharMoveEvent evt = new StageCharMoveEvent();
                evt.nIdent = 0;
                evt.nCharID = func.I(0);
                evt.nX = func.I(1);
                evt.nY = func.I(2);
                //evt.nAtkSkillID = func.I(2);
                Panel_StageUI.Instance.OnStageCharMoveEvent(evt);
                //GameEventManager.DispatchEvent ( evt  );

            }
            else if (func.sFunc == "SWAP")  //  swap 2 mob's pos
            {
                StageCharSwapEvent evt = new StageCharSwapEvent();
                evt.nCharID = func.I(0);
                evt.nCharID2 = func.I(1);
                //evt.nAtkSkillID = func.I(2);
                Panel_StageUI.Instance.OnStageCharSwapEvent(evt);
                //GameEventManager.DispatchEvent ( evt  );

            }

            else if (func.sFunc == "SOUND")  // PLAY SOUND
            {
                Panel_StageUI.Instance.OnStagePlaySound(func.S(0));
            }
            else if (func.sFunc == "FX")  // PLAY FX
            {
                Panel_StageUI.Instance.OnStagePlayFX(func.I(0), func.I(1));
            }
            else if (func.sFunc == "POSFX")  // PLAY POSFX
            {
                Panel_StageUI.Instance.OnStagePosPlayFX(func.I(0), func.I(1), func.I(2), func.I(3));
            }

            else if (func.sFunc == "ADDBUFF")  // Add buff
            {
                Panel_StageUI.Instance.OnStageAddBuff(func.I(0), func.I(1), func.I(2), 0);
            }
            else if (func.sFunc == "DELBUFF")  // Add buff
            {
                Panel_StageUI.Instance.OnStageAddBuff(func.I(0), func.I(1), func.I(2), 1);
            }
            else if (func.sFunc == "ADDCAMPBUFF")  // Add buff
            {
                Panel_StageUI.Instance.OnStageCampAddBuff(func.I(0), func.I(1), func.I(2), 0);
            }
            else if (func.sFunc == "DELCAMPBUFF")  // Add buff
            {
                Panel_StageUI.Instance.OnStageCampAddBuff(func.I(0), func.I(1), func.I(2), 1);
            }
            else if (func.sFunc == "ADDHP")  // Add HP
            {
                Panel_StageUI.Instance.OnStageAddUnitValue(func.I(0), 0, func.F(1));
            }
            else if (func.sFunc == "ADDDEF")  // Add DEF
            {
                Panel_StageUI.Instance.OnStageAddUnitValue(func.I(0), 1, func.F(1));
            }
            else if (func.sFunc == "ADDMP")  // Add HP
            {
                Panel_StageUI.Instance.OnStageAddUnitValue(func.I(0), 2, func.F(1));
            }
            else if (func.sFunc == "ADDSCHOOL")  // Add school
            {
                Panel_StageUI.Instance.OnStageAddSchool(func.I(0), func.I(1), func.I(2));
            }
            else if (func.sFunc == "EQUIPITEM")  // Add buff
            {
                Panel_StageUI.Instance.OnStageEquipItem(func.I(0), func.I(1));
            }
            else if (func.sFunc == "SETHP")  // Add HP
            {
                Panel_StageUI.Instance.OnStageSetUnitValue(func.I(0), 0, func.F(1), func.I(2));
            }

            else if (func.sFunc == "RELIVE")  // relive
            {
                Panel_StageUI.Instance.OnStageRelive(func.I(0));
            }
            else if (func.sFunc == "SETUNDEAD")
            {
                Panel_StageUI.Instance.OnStageSetUnDeadEvent(func.I(0), func.I(1));
            }
            else if (func.sFunc == "UNITDEAD")
            {
                //int nCharID = func.I( 0 );		
                Panel_StageUI.Instance.OnStageUnitDeadEvent(func.I(0), func.I(1));

                // dont close auto.
                //	TalkDeadEvent evt = new TalkDeadEvent();			
                //	evt.nChar  = nCharID;			
                //	GameEventManager.DispatchEvent ( evt  );

            }
            else if (func.sFunc == "DELUNIT")
            {
                if (nCheckIdent > 0)
                {
                    cUnitData data = GameDataManager.Instance.GetUnitDateByIdent(nCheckIdent);
                    if (data != null)
                    {
                        StageDelUnitByIdentEvent evt = new StageDelUnitByIdentEvent();
                        evt.nIdent = nCheckIdent;
                        Panel_StageUI.Instance.OnStageDelUnitByIdentEvent(evt);

                        if (data.n_CharID != 0)
                        {
                            TalkSayEndEvent tlkevt = new TalkSayEndEvent();
                            tlkevt.nChar = data.n_CharID;
                            GameEventManager.DispatchEvent(tlkevt);
                        }

                    }
                }
                else
                {

                    StageDelUnitEvent evt = new StageDelUnitEvent();
                    //evt.eCamp = (_CAMP)func.I( 0 );
                    evt.nCharID = func.I(0);
                    evt.nDelType = 1; // always is leave mode


                    Panel_StageUI.Instance.OnStageDelUnitEvent(evt);
                    //GameEventManager.DispatchEvent ( evt );

                    // say end  event
                    if (evt.nCharID != 0)
                    {
                        TalkSayEndEvent tlkevt = new TalkSayEndEvent();
                        tlkevt.nChar = evt.nCharID;
                        GameEventManager.DispatchEvent(tlkevt);
                    }
                }
            }


            else if (func.sFunc == "JOIN")
            {
                int nCharID = func.I(0);
                GameDataManager.Instance.EnableStorageUnit(nCharID, true);
                GameDataManager.Instance.EnableStageUnit(nCharID, true);
            }
            else if (func.sFunc == "LEAVE")
            {
                int nCharID = func.I(0);
                GameDataManager.Instance.EnableStorageUnit(nCharID, false);
                GameDataManager.Instance.EnableStageUnit(nCharID, false);
            }
            //不能刪除 等待執行 event. 會造成 讀檔上的麻煩
            //			else if( func.sFunc  == "DELEVENT") 
            //			{
            //				Panel_StageUI.Instance.OnStageDelEventEvent( func.I( 0 ) );			
            //			}
            else if (func.sFunc == "UNITCAMP") //改變單位 陣營
            {
                int nCharid = func.I(0);
                int nCampid = func.I(1);

                Panel_StageUI.Instance.OnStageUnitCampEvent(nCharid, (_CAMP)nCampid);
            }
            else if (func.sFunc == "DELCAMP")
            {
                int nCampid = func.I(0);
                Panel_StageUI.Instance.OnStageDelCamp(func.I(0));

            }
            else if (func.sFunc == "POPMARK") //stage地圖上顯示 mark
            {
                Panel_StageUI.Instance.OnStagePopMarkEvent(func.I(0), func.I(1), func.I(2), func.I(3));
            }
            else if (func.sFunc == "CAMERACENTER")
            {
                Panel_StageUI.Instance.OnStageCameraCenterEvent(func.I(0), func.I(1));
            }
            else if (func.sFunc == "SHAKECAMERA" || func.sFunc == "CAMERASHAKE")
            {
                //Panel_StageUI.Instance.OnStageCameraCenterEvent( func.I(0),func.I(1) );
                GameSystem.ShakeCamera(func.F(0));

                int nSoundID = func.I(1);
                if (nSoundID > 0) {
                    GameSystem.PlaySound(nSoundID); 
                }

            }
            else if (func.sFunc == "SAI") //設定索敵AI
            {
                GameDataManager.Instance.SetUnitSearchAI(func.I(0), (_AI_SEARCH)func.I(1), func.I(2), func.I(3));
            }
            else if (func.sFunc == "CAI") // 改變單位攻擊AI
            {
                GameDataManager.Instance.SetUnitComboAI(func.I(0), (_AI_COMBO)func.I(1));
            }
            else if (func.sFunc == "CAMPSAI") //設定 陣營      索敵AI
            {
                GameDataManager.Instance.SetCampSearchAI(func.I(0), (_AI_SEARCH)func.I(1), func.I(2), func.I(3));
            }
            else if (func.sFunc == "CAMPCAI") // 改變陣營單位攻擊AI
            {
                GameDataManager.Instance.SetCampComboAI(func.I(0), (_AI_COMBO)func.I(1));
            }
            else if (func.sFunc == "WIN")
            {
                PanelManager.Instance.OpenUI(Panel_Win.Name);
                //Panel_StageUI.Instance.bIsStageEnd = true;
            }
            else if (func.sFunc == "LOST")
            {
                PanelManager.Instance.OpenUI(Panel_Lost.Name);
                //Panel_StageUI.Instance.bIsStageEnd = true;
            }
            else if (func.sFunc == "ADDSTAR")  // add star
            {
                int nStar = func.I(0);
                //				if( nStar == 0 )
                //					nStar += 1 ;
                //				GameDataManager.Instance.nStars +=nStar;
                //				string sMsg = string.Format( "星星+ {0}" , nStar );
                //				BattleManager.Instance.ShowBattleMsg( null , sMsg );
                //ShowBattleMsg.
                Panel_StageUI.Instance.AddStar(nStar);

            }
            else if (func.sFunc == "REGBLOCK")  // REG BLOCK EVENT
            {
                GameDataManager.Instance.RegEvtBlock(func.I(0), func.I(1), func.I(2), func.I(3), func.I(4), func.S(5));
                Panel_StageUI.Instance.OnStagePopMarkEvent(func.I(0), func.I(1), func.I(2), func.I(3));
            }
            else if (func.sFunc == "DELBLOCK")  // un reg BLOCK EVENT
            {
                GameDataManager.Instance.DelEvtBlock(func.S(0));
                Panel_StageUI.Instance.ClearMarkCellEffect();
                //nel_StageUI.Instance.OnStagePopMarkEvent(func.I(0), func.I(1), func.I(2), func.I(3));
            }
            else if (func.sFunc == "DISPATCH")  // DISPATCH
            {
                Panel_Dispatch panel = MyTool.GetPanel<Panel_Dispatch>(PanelManager.Instance.OpenUI(Panel_Dispatch.Name));
                if (panel != null)
                {
                    panel.BornBlock(func.I(0), func.I(1), func.I(2), func.I(3));

                }
            }
            else
            {
                Debug.LogError(string.Format("Error-Can't find script func '{0}'", func.sFunc));
            }
		}
		bParsing = false;
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
				else if( func.sFunc  == "AURABUFF_E") 				{
					pool.Add( new AURABUFF_E( func.I(0) , func.I(1)) );
				}
				else if( func.sFunc  == "AURABUFF_I") 				{
					pool.Add( new AURABUFF_I( func.I(0) , func.I(1) ) );
				}


                else if (func.sFunc == "AURA_DELBUFF_I")
                {
                    pool.Add(new AURA_DELBUFF_I(func.I(0), func.I(1)));
                }
                else if (func.sFunc == "AURA_DELBUFF_E")
                {
                    pool.Add(new AURA_DELBUFF_E(func.I(0), func.I(1)));
                }
                else if (func.sFunc == "AURA_DELSTACK_I")
                {
                    pool.Add(new AURA_DELSTACK_I(func.I(0), func.I(1)));
                }
                else if (func.sFunc == "AURA_DELSTACK_E")
                {
                    pool.Add(new AURA_DELSTACK_E(func.I(0), func.I(1)));
                }
                else if (func.sFunc == "AURA_CP_I")
                {
                    pool.Add(new AURA_CP_I(func.I(0), func.I(1)));
                }
                else if (func.sFunc == "AURA_CP_E")
                {
                    pool.Add(new AURA_CP_E(func.I(0), func.I(1)));
                }


                else if (func.sFunc == "BATTLE_ARRAY")
                {
                    pool.Add(new BATTLE_ARRAY(func.I(0), func.I(1)));
                }
                //else if (func.sFunc == "SKILL_EFFECT")
                //{
                //    pool.Add(new SKILL_EFFECT(func.I(0)));
                //}
                // Hit effect
                else if (func.sFunc == "HITBUFF_I")
                {
                    pool.Add(new HITBUFF_I(func.I(0)));
                }
                else if (func.sFunc == "HITBUFF_E")
                {
                    pool.Add(new HITBUFF_E(func.I(0)));
                }
                else if (func.sFunc == "HITHP_I")
                {
                    pool.Add(new HITHP_I(func.F(0), func.I(1)));
                }
                else if (func.sFunc == "HITHP_E")
                {
                    pool.Add(new HITHP_E(func.F(0), func.I(1)));
                }
                else if (func.sFunc == "HITMP_I")
                {
                    pool.Add(new HITMP_I(func.F(0), func.I(1)));
                }
                else if (func.sFunc == "HITMP_E")
                {
                    pool.Add(new HITMP_E(func.F(0), func.I(1)));
                }
                else if (func.sFunc == "HITSP_I")
                {
                    pool.Add(new HITSP_I(func.F(0), func.I(1)));
                }
                else if (func.sFunc == "HITSP_E")
                {
                    pool.Add(new HITSP_E(func.F(0), func.I(1)));
                }
                else if (func.sFunc == "HITCP_I")
                {
                    pool.Add(new HITCP_I(func.I(0)));
                }
                else if (func.sFunc == "HITCP_E")
                {
                    pool.Add(new HITCP_E(func.I(0)));
                }
                // skill upgrade
                else if (func.sFunc == "UP_SKILL")
                {
                    pool.Add(new UP_SKILL(func.I(0), func.I(1)));
                }
                // char data modify
                else if (func.sFunc == "ADDHP_I")
                {
                    pool.Add(new ADDHP_I(func.F(0), func.I(1)));
                }
                else if (func.sFunc == "ADDHP_E")
                {
                    pool.Add(new ADDHP_E(func.F(0), func.I(1)));
                }
                else if (func.sFunc == "ADDMP_I")
                {
                    pool.Add(new ADDMP_I(func.F(0), func.I(1)));
                }
                else if (func.sFunc == "ADDMP_E")
                {
                    pool.Add(new ADDMP_E(func.F(0), func.I(1)));
                }
                else if (func.sFunc == "ADDSP_I")
                {
                    pool.Add(new ADDSP_I(func.I(0)));
                }
                else if (func.sFunc == "ADDCP_I")
                {
                    pool.Add(new ADDCP_I(func.I(0)));
                }
                else if (func.sFunc == "ADDSP_E")
                {
                    pool.Add(new ADDSP_E(func.I(0)));
                }
                else if (func.sFunc == "ADDCP_E")
                {
                    pool.Add(new ADDCP_I(func.I(0)));
                }
                else if (func.sFunc == "ADDACTTIME_I")
                {
                    pool.Add(new ADDACTTIME_I(func.I(0)));
                }
                else if (func.sFunc == "ADDACTTIME_E")
                {
                    pool.Add(new ADDACTTIME_E(func.I(0)));
                }
                // Attr
                else if (func.sFunc == "ADD_MAR")
                {
                    pool.Add(new ADD_MAR(func.F(0)));
                }
                else if (func.sFunc == "ADD_MAR_DIFF")
                {
                    pool.Add(new ADD_MAR_DIFF(func.F(0), func.I(1)));
                }
                else if (func.sFunc == "ADD_ATTACK_DIFF")
                {
                pool.Add(new ADD_ATTACK_DIFF(func.F(0), func.I(1)));
                }
                else if (func.sFunc == "ADD_ATTACK")
                {
                    pool.Add(new ADD_ATTACK(func.I(0)));
                }
                else if (func.sFunc == "ADD_MAXDEF")
                {
                    pool.Add(new ADD_MAXDEF(func.I(0)));
                }
                else if (func.sFunc == "ADD_DEF_I")
                {
                    pool.Add(new ADD_DEF_I(func.F(0) , func.I(1)));
                }
                else if (func.sFunc == "ADD_DEF_E")
                {
                    pool.Add(new ADD_DEF_E(func.F(0), func.I(1)));
                }
                else if (func.sFunc == "ADD_POWER")
                {
                    pool.Add(new ADD_POWER(func.I(0)));
                }
                else if (func.sFunc == "ADD_MAXHP")
                {
                    pool.Add(new ADD_MAXHP(func.I(0)));
                }
                else if (func.sFunc == "ADD_MAXMP")
                {
                    pool.Add(new ADD_MAXMP(func.I(0)));
                }
                else if (func.sFunc == "ADD_MAXSP")
                {
                    pool.Add(new ADD_MAXSP(func.I(0)));
                }
                else if (func.sFunc == "MUL_DRAINHP")
                {
                    pool.Add(new MUL_DRAINHP(func.F(0)));
                }
                else if (func.sFunc == "MUL_DRAINMP")
                {
                    pool.Add(new MUL_DRAINMP(func.F(0)));
                }
                else if (func.sFunc == "ADD_MOVE")
                {
                    pool.Add(new ADD_MOVE(func.I(0)));
                }
                else if (func.sFunc == "ADD_ARMOR")
                {
                    pool.Add(new ADD_ARMOR(func.F(0)));
                }
                else if (func.sFunc == "MUL_DROP")
                {
                    pool.Add(new MUL_DROP(func.F(0)));
                }
                else if (func.sFunc == "MUL_BRUST")
                {
                    pool.Add(new MUL_BRUST(func.F(0)));
                }
                else if (func.sFunc == "MUL_DAMAGE")
                {
                    pool.Add(new MUL_DAMAGE(func.F(0)));
                }
                else if (func.sFunc == "MUL_ATTACK")
                {
                    pool.Add(new MUL_ATTACK(func.F(0)));
                }
                else if (func.sFunc == "MUL_MAXDEF")
                {
                    pool.Add(new MUL_MAXDEF(func.F(0)));
                }
                else if (func.sFunc == "MUL_POWER")
                {
                    pool.Add(new MUL_POWER(func.F(0)));
                }
                else if (func.sFunc == "MUL_MAXHP")
                {
                    pool.Add(new MUL_MAXHP(func.F(0)));
                }
                else if (func.sFunc == "MUL_MAXMP")
                {
                    pool.Add(new MUL_MAXMP(func.F(0)));
                }
                else if (func.sFunc == "MUL_MAXSP")
                {
                    pool.Add(new MUL_MAXSP(func.F(0)));
                }
                else if (func.sFunc == "MUL_MPCOST")
                {
                    pool.Add(new MUL_MPCOST(func.F(0)));
                }
                // immune
                else if (func.sFunc == "IMMUNE")
                {
                    pool.Add(new IMMUNE(func.I(0)));
                }


                //=============== Tag 
                else if (func.sFunc == "TAG_UNDEAD")// 除非隊長死否則無限重生
                {  // 
                    pool.Add(new TAG_UNDEAD());
                }
                else if (func.sFunc == "TAG_CHARGE") // 突襲移動. no block
                {
                    pool.Add(new TAG_CHARGE());
                }
                else if (func.sFunc == "TAG_NODIE")
                {  // 不死身
                    pool.Add(new TAG_NODIE());
                }
                else if (func.sFunc == "TAG_SILENCE")
                {    // can't  skill
                    pool.Add(new TAG_SILENCE());
                }
                else if (func.sFunc == "TAG_PEACE")
                {  // 中立
                    pool.Add(new TAG_PEACE());
                }
                else if (func.sFunc == "TAG_TRIGGER")// 是機關。不執行AI
                {  // 中立
                    pool.Add(new TAG_TRIGGER());
                }
                else if (func.sFunc == "TAG_BLOCKITEM")// 不能變更裝備道具           
                {  //
                    pool.Add(new TAG_BLOCKITEM());
                }
                else if (func.sFunc == "TAG_STUN")// 暈眩 ，點穴                            
                { 
                    pool.Add(new TAG_STUN());
                }
                //=============== fight status
                else if (func.sFunc == "IS_HIT")
                { // 必HIT
                    pool.Add(new IS_HIT());
                    //pool.Add( new IS_UNIT_STATUS( _FIGHTSTATE._DODGE ) );
                }
                else if (func.sFunc == "IS_DODGE")
                { // 必閃
                    pool.Add(new IS_DODGE());
                    //pool.Add( new IS_UNIT_STATUS( _FIGHTSTATE._DODGE ) );
                }
                else if (func.sFunc == "IS_CIRIT")
                {  // 
                    pool.Add(new IS_CIRIT());
                }
                else if (func.sFunc == "IS_MERCY")
                {  //手加減
                    pool.Add(new IS_MERCY());
                }
                else if (func.sFunc == "IS_GUARD")
                { // 被防衛
                    pool.Add(new IS_GUARD());
                }
                else if (func.sFunc == "IS_THROUGH")
                { // 攻擊穿透
                    pool.Add(new IS_THROUGH());
                }
                else if (func.sFunc == "IS_MISS")
                { // 失誤
                    pool.Add(new IS_MISS());
                }
                else if (func.sFunc == "IS_COMBO")
                { // 失誤
                    pool.Add(new IS_COMBO());
                }
                else if (func.sFunc == "IS_BROKEN")
                { // 破防
                    pool.Add(new IS_BROKEN());
                }
                else if (func.sFunc == "IS_RETURN")
                { // 傷害轉彈
                    pool.Add(new IS_RETURN());
                }
                else if (func.sFunc == "IS_COPY")
                { // 複製對方出手招式
                    pool.Add(new IS_COPY());
                }
                else if (func.sFunc == "IS_TWICE")
                { // 打兩下
                    pool.Add(new IS_TWICE());
                }
                else if (func.sFunc == "IS_NODMG")//免疫傷害
                {
                    pool.Add(new IS_NODMG());
                }
                else if (func.sFunc == "IS_ANTIFLY")//免疫飛行道具
                {
                    pool.Add(new IS_ANTIFLY());
                }
                else if (func.sFunc == "IS_SHIELD")
                { // 真氣盾
                    pool.Add(new IS_SHIELD());
                }
                else {

                    Debug.LogFormat(" unknow effect {0}", func.sFunc);
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
