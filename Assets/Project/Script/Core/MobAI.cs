﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MYGRIDS;
using MyClassLibrary;

public class MobAI  {    

    public static List<SKILL> tmpSklList  =  new List<SKILL>();
    static Dictionary<iVec2, int> distpool = new Dictionary<iVec2, int>();
    static Dictionary<int, int> sklPool = new Dictionary<int, int>();    

    public static void Run( Panel_unit mob )
	{
        long tick = System.DateTime.Now.Ticks;

        int ident = mob.Ident ();
		cUnitData mobdata = GameDataManager.Instance.GetUnitDateByIdent ( ident );


        if (mobdata.IsStun()) {
            // 行動力歸零
            mobdata.nActionTime = 0;
            return;
        }
		//int nSkillID = -1;		// -1 - no attack
		//// select a skill
		//if( mobdata.eComboAI == _AI_COMBO._NORMAL ){
  //          nSkillID = 0; //  SelSkill( mob.pUnitData );
		//}

		int nMove = mobdata.GetMov ()  ; 
		switch( mobdata.eSearchAI )
		{
			case _AI_SEARCH._NORMAL:{ //主動攻擊 - 優先打血少
				_AI_NormalAttack( mob  ,  nMove ) ;
			}break;
			case _AI_SEARCH._PASSIVE:{ //被動攻擊 - 有人在範圍內 優先打近的
				_AI_PassiveAttack( mob  ,  nMove ) ;
			}break;
			case _AI_SEARCH._DEFENCE:{ //堅守原地
                    if (mobdata.eComboAI == _AI_COMBO._DEFENCE)
                    {
                        _AI_Defence( mob  ,  0 ) ;
                    }                    
                    else
                    {                                       
                        _AI_PassiveAttack(mob, 0); // 還是會攻擊。但是不移動
                    }
            }
                break;
			case _AI_SEARCH._TARGET:{ //前往指定目標
				_AI_TargetAttack( mob  ,  nMove ) ;
			}break;	
			case _AI_SEARCH._POSITION:{ //不在目標地點前往目標
				_AI_PositionAttack( mob,  nMove ) ;
			}break;	
			default:
                _AI_MakeWaitCmd(mob);
                //ActionManager.Instance.CreateWaitingAction (ident);
                break;
		}

        // record ai time
        if (Debug.isDebugBuild == true)
        {
            long during = System.DateTime.Now.Ticks - tick;
            Debug.LogFormat("unit({0}-{1}) MobAI spend ticket:{2}ms ", mob.pUnitData.n_CharID, mob.pUnitData.n_Ident, during / 10000);
        }

        return;


	}


	public static void RunCounter( Panel_unit mob , Panel_unit atker , int nAtkSkillID )
	{

	}

    // Widget func .. find a skill to atk a target , -1 == can't atk
    // return : true - can attack , false - cant attack
    public static bool _FindToAttackTarget( Panel_unit mob, Panel_unit taget, int nMove , out int nSKillID , out List<iVec2> pathList , bool bCounterMode = false , bool ative = false)
    {
        nSKillID = -1;
        pathList = null;

        if (taget == null) return false;
        if (taget.pUnitData == null || taget.pUnitData.IsTag(_UNITTAG._PEACE) ||  taget.pUnitData.IsTag(_UNITTAG._HIDE))
        {
            return false; 
        }
        // Find a skill to atk. the       
        if (bCounterMode == true)
        {
            nMove = 0;
        }
         
        int nDist = mob.Loc.Dist(taget.Loc);
        if (CreateSkilTmpList(mob.pUnitData, taget.pUnitData , nDist ,  nMove , bCounterMode)) // create skill pool to atl
        {
            foreach (SKILL skl in tmpSklList)
            {
                int nMinRange = skl.n_MINRANGE;
                int nSkillRange = skl.n_RANGE;          // -1 代表 無距離限制
                bool bAll = (skl.n_TARGET == 9|| skl.n_TARGET == 10|| skl.n_TARGET == 11);
                if ( bAll || (nSkillRange== -1) || (  (nDist <= nSkillRange) && (nDist >= nMinRange) ) ) // 可以直接攻擊
                {
                    nSKillID = skl.n_ID;
                    return true; // tmpSklList
                }

                // 在最短距離內，需要跑遠一點再攻擊
                if (nDist < nMinRange)
                {
                    // 不處理本問題
                }             


                // 被動時，增加距離檢查. 減少 a*次數
                if (ative == false)
                {
                    if (nDist > nMove + nSkillRange )
                    {
                        return false;
                    }
                }

                // 在技能範圍外，需要收尋一個位置
                List<iVec2> path = FindPathToTarget(mob, taget, nMove, nSkillRange); // need a AI to move
                if ((path != null) && (path.Count > 0))
                {
                   
                   

                    // 研原路徑找到一個 可以攻擊的位置 
                    iVec2 last = path[path.Count - 1];
                    if (last != null)
                    {
                        int nDist2 = last.Dist(taget.Loc);
                        if ( ative || ((nDist2 <= nSkillRange) && (nDist2 >= nMinRange)) ) // （active 必須通過，不然永遠不會追擊） 可以攻擊
                        //if ( ((nDist2 <= nSkillRange) && (nDist2 >= nMinRange))) // 可以攻擊
                        {
                            pathList = path;
                            nSKillID = skl.n_ID; // assign skill id
                            return true; // tmpSklList
                        }
                    }
                }// if ((path != null) && (path.Count > 0))
            }
        }
        else {//無技可用，只能普攻
            // no skill , use melee , find pos to atk
            if (nDist <= 1) {
                nSKillID = 0;
                return true; // tmpSklList
            }

            // 被動時，增加距離檢查. 減少 a*次數
            if (ative == false)
            {
                if (nDist > nMove + 1)
                {
                    return false;
                }
            }


            List<iVec2> path = FindPathToTarget(mob, taget , nMove, 1 ); // melee is 1
            if ( (path != null) && (path.Count > 0) )
            {                // out put to make cmd
                
                iVec2 last = path[path.Count - 1];
                if (last != null)
                {
                    int nDist2 = last.Dist(taget.Loc);
                    if ( ative ||  (nDist2 <= 1) ) // 可以攻擊
                    {
                        pathList = path;
                        nSKillID = 0;
                        return true; // tmpSklList
                    }
                    return false; // // can't atk directly
                }
            }
        }
        return false; 
    }	

    //
    static void _AI_MakeCmd(Panel_unit mob, Panel_unit Tar , int nSkillID , ref List<iVec2> path )
    {      

        int nMinRange = 0;
        int nSkillRange = 0;
        int nDist = 0;
        MyTool.GetSkillRange(nSkillID, out nSkillRange, out nMinRange);
        // should set path here
        if (path != null && path.Count > 0)
        {
            // send move
            iVec2 last = path[path.Count - 1];
            if (last != null)
            {
                mob.SetPath(path);
                //// check if last pos is attack able pos
                ActionManager.Instance.CreateMoveAction(mob.Ident(), last.X, last.Y);
                // check can atk
                nDist = last.Dist(Tar.Loc);
            }
        }
        else
        {
            nDist = mob.Loc.Dist(Tar.Loc); // 沒有路徑，則取兩生物之距離
        }

        if ( mob.pUnitData.eComboAI == _AI_COMBO._DEFENCE) {
            //  ActionManager.Instance.CreateWaitingAction(mob.Ident());
            _AI_MakeWaitCmd(mob, Tar);
            return;
        }

        // 如果是攻擊性技能，判斷 敵我關係
        if (mob.CanPK(Tar) == false) {
            if (MyTool.IsDamageSkill(nSkillID) == true)
            {
                _AI_MakeWaitCmd(mob, Tar);
                //ActionManager.Instance.CreateWaitingAction(mob.Ident()); // 不能攻擊我方
                return;
            }
        }

        int nTarget = 0;
        // check is cast cmd
        SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nSkillID);
        if (skl != null)
        {
            // self , 6→自我AOE我方7→自我AOE敵方8→自我AOEALL
            if (skl.n_TARGET == 0 || skl.n_TARGET == 6 || skl.n_TARGET == 7 || skl.n_TARGET == 8)
            {
                int nTarX = 0;
                int nTarY = 0;
                //
                BattleManager.ConvertSkillTargetXY(mob.pUnitData, nSkillID, Tar.Ident(), ref nTarX, ref nTarY);
                // check can atk or not 
                ActionManager.Instance.CreateCastCMD(mob.Ident(), nTarX, nTarY, nSkillID); // create Attack CMD . need battle manage to run				

                return;
            }
            else if (skl.n_TARGET == 9 || skl.n_TARGET == 10 || skl.n_TARGET == 11)
            {
                ActionManager.Instance.CreateAttackCMD(mob.Ident(), Tar.Ident(), nSkillID); // create Attack CMD . need battle manage to run
                return;
            }
        }
        // 基本上到這邊，應該都已經通過檢查了。（ 還是需要距離檢查，主動怪需要靠這邊決定是否攻擊）


        //if (((nDist <= nSkillRange) && (nDist >= nMinRange)))
        //{
        //    ActionManager.Instance.CreateAttackCMD(mob.Ident(), Tar.Ident(), nSkillID); 			
        //}
     
       
        // check send atk cmd ... // create Attack CMD . need battle manage to run	
        if (((nDist <= nSkillRange) && (nDist >= nMinRange))  ) // 可以攻擊
//            if (((nDist <= nSkillRange) && (nDist >= nMinRange)) || (skl.n_TARGET == 9 || skl.n_TARGET == 10 || skl.n_TARGET == 11)) // 可以攻擊
        {
            // check can atk or not 
            ActionManager.Instance.CreateAttackCMD(mob.Ident(), Tar.Ident(), nSkillID); // create Attack CMD . need battle manage to run				
                                                                                        //  return true;
        }
        else
        {
            //將來修正， 如果還能移動，則降低移動力，重新收尋
            _AI_MakeWaitCmd(mob, Tar);
            //ActionManager.Instance.CreateWaitingAction(mob.Ident());
        }
    }

    static void _AI_MakeWaitCmd(Panel_unit mob, Panel_unit Tar=null)
    {
        // check if wait skill
        int Dist = 0;
        int nTarX = 0;
        int nTarY = 0;
        cUnitData target = null;
        if (Tar != null)
        {
            target = Tar.pUnitData;
            nTarX = mob.Loc.X;
            nTarY = mob.Loc.Y;
            Dist = mob.Dist(Tar);
        }

        if (CreateWaitTmpList(mob.pUnitData, target, Dist))
        {
            // 
            foreach (SKILL skl in tmpSklList)
            {
                ActionManager.Instance.CreateCastCMD(mob.Ident(), nTarX, nTarY, skl.n_ID); // create Attack CMD . need battle manage to run				
                return;
            }
        }

        //_AI_MakeWaitCmd(mob, Tar);
        ActionManager.Instance.CreateWaitingAction(mob.Ident());
    }

    // tool func hp lowest ver2
    static bool _AI_LowstAttack2(Panel_unit mob, int nMove , bool ative = false )
    {

        //int ident = mob.Ident();
        int nMaxRange = _AI_GetMaxSkillRange( mob.pUnitData);

        Panel_StageUI.Instance.GetUnitHpPool(mob, true, (nMove + nMaxRange)); // all unit in 
        Dictionary<Panel_unit, int> pool = Panel_StageUI.Instance.m_tmpHpPool  ; // all unit in 
        var items = from pair in pool orderby pair.Value ascending select pair;             // Sort HP
        // 範圍內 低血量
        foreach (KeyValuePair<Panel_unit, int> pair in items)
        {
            int nSkillID ;
            List<iVec2> path ;            ;
            if (_FindToAttackTarget(mob, pair.Key, nMove , out nSkillID , out path, false , ative )) // 確定可以打到
            {              
                if (nSkillID == -1 ) {
                    // 沒有技能可以攻擊 。在 此模式會放棄找下一個
                    continue;

                }


                // 先判斷是否要 上 buff
                if (_AI_CastBuff(mob, nSkillID, nMove ))
                {   
                        return true; // 能施展下一個技能的話 先中斷並 重新收尋
                }

                // 判斷是否有被敵軍正面阻擋

                _AI_MakeCmd(mob, pair.Key, nSkillID, ref path );
                // wait 
                return true;
            }
            else {
                // try next unit
            }
        }
        return false;
    }

    static bool _AI_NearestAttack2(Panel_unit mob, int nMove, bool ative = false) // 是否 主動攻擊
    {
        Panel_StageUI.Instance.GetUnitDistPool(mob, true);
        Dictionary<Panel_unit, int> pool = Panel_StageUI.Instance.m_tmpDistPool;
      //  int ident = mob.Ident();
        // Sort dist
        var items = from pair in pool orderby pair.Value ascending select pair;
        foreach (KeyValuePair<Panel_unit, int> pair in items)
        {
            int nSkillID = 0;
            List<iVec2> path = null;
            if(  _FindToAttackTarget(mob, pair.Key, nMove, out nSkillID, out path, false, ative) )         
            {
                if (nSkillID == -1)
                {
                    // 沒有技能可以攻擊，在此模式 一樣會前進
                }

                    // 先判斷是否要 上 buff
                if (_AI_CastBuff(mob, nSkillID, 0))
                {
                    return true; // 有 buff 先中斷並 重新收尋
                }

                _AI_MakeCmd(mob, pair.Key, nSkillID, ref path);
                // wait 
                return true;
            }
            else // 不能 主動攻擊， 先看看 能否找到下一個
            {
              
                // next target
            }
        }

        // 決定是否需要主動移動
        if (ative) { // 主動攻擊
            foreach (KeyValuePair<Panel_unit, int> pair in items)
            {
                // 避開 錯誤目標
                if (pair.Key.pUnitData.IsTag(_UNITTAG._PEACE) || pair.Key.pUnitData.IsTag(_UNITTAG._HIDE ) )
                {
                    continue; // 不往中立單位移動
                }

                int nSkillID = 0;
                List<iVec2> path = null;
                if (_FindToAttackTarget(mob, pair.Key, nMove, out nSkillID, out path, false , true )) // 由於有些機關是中立單位，不會被攻擊。所以不能只找第一個資料                
                {

                    // 先判斷是否要 上 buff
                    if (_AI_CastBuff(mob, nSkillID, 0))
                    {
                        return true; // 有 buff 先中斷並 重新收尋
                    }

                    _AI_MakeCmd(mob, pair.Key, nSkillID, ref path); // 往最近的移動 + 攻擊
                    return true;
                }
                else {
                    // 移動過去
                    if (path != null && path.Count > 0 )
                    {
                        _AI_MakeCmd(mob, pair.Key, 0, ref path); // 往最近的移動
                        return true;
                    }
                }
            }
        }


        return false;
    }




    static  bool _AI_LowstAttack( Panel_unit mob , int nSkillID , int nMove , int nLimit = 0 )
	{
		int ident = mob.Ident ();
		int nMinRange ;
		int nSkillRange ;
        
        MyTool.GetSkillRange ( nSkillID , out nSkillRange , out nMinRange);
        Panel_StageUI.Instance.GetUnitHpPool(mob, true, (nMove + nSkillRange));
        Dictionary<Panel_unit, int> pool = Panel_StageUI.Instance.m_tmpHpPool;
        // Sort HP
        var items = from pair in pool orderby pair.Value ascending select pair;
		// 範圍內 低血量
		foreach (KeyValuePair<Panel_unit , int> pair in items) {
			// try path to target
			bool bCanAtk = false;
            
            if (pair.Key.pUnitData== null || pair.Key.pUnitData.IsTag( _UNITTAG._PEACE) || pair.Key.pUnitData.IsTag(_UNITTAG._HIDE) ) {
                continue;
            }

			int nDist = mob.Loc.Dist( pair.Key.Loc );
			if( nDist > nSkillRange  ) // pathfind if need
			{
				List< iVec2> path = FindPathToTarget( mob , pair.Key , nMove , nSkillRange );
				if( path == null || (path.Count ==0) )
				{
					continue;
				}
				else 
				{
					iVec2 last = path[ path.Count -1 ];
					if( last != null  ){
						int nDist2 = last.Dist( pair.Key.Loc );
						if( (nDist2 > nSkillRange) || (nDist2 < nMinRange) ){// 檢查 too far/too near 要不要換skill打
							SKILL newSkill = FindSkillByDist( mob , pair.Key.pUnitData , nDist2);
							if( newSkill == null ){
								continue; // 太遠了~放棄不打
							}
							else{
								// change skill
								nSkillID = newSkill.n_ID ;
							}
						}
						
						// send a move event					
						mob.SetPath (path); 
						// check if last pos is attack able pos
						ActionManager.Instance.CreateMoveAction (ident, last.X, last.Y);	
						
						if (Config.GOD == true) {
							Panel_StageUI.Instance.CreatePathOverEffect (path); // draw path
						}
						
						bCanAtk = true;
					}
					
				}

			}
			else if( nDist < nMinRange ){ // check if need change skill
				SKILL newSkill = FindSkillByDist( mob , pair.Key.pUnitData , nDist);
				if( newSkill != null ){	// change skill
					nSkillID = newSkill.n_ID ;
					bCanAtk = true;
				}else if( nDist <= 1 ){
					nSkillID = 0;
					bCanAtk = true;
				}
			}
			else{
				bCanAtk = true ; // atk directly
				
			}
			
			if( bCanAtk )
			{
				ActionManager.Instance.CreateAttackCMD (ident, pair.Key.Ident (), nSkillID ); // create Attack CMD . need battle manage to run				
				return true;
			}
			// next target
			
		}

		return false;
	}

	static  bool _AI_NearestAttack( Panel_unit mob , int nSkillID , int nMove  , bool bHold = false )
	{
        Panel_StageUI.Instance.GetUnitDistPool(mob, true);
        Dictionary<Panel_unit, int> pool = Panel_StageUI.Instance.m_tmpDistPool;

        int ident = mob.Ident ();
		
		// get skill range
		int nMinRange ;
		int nSkillRange ;
		MyTool.GetSkillRange ( nSkillID , out nSkillRange , out nMinRange);
		
		// Sort dist
		var items = from pair in pool orderby pair.Value ascending select pair;
		// 
		foreach (KeyValuePair<Panel_unit , int> pair in items) {
			// try path to target
			bool bCanAtk = false;
            
            if (pair.Key.pUnitData == null || pair.Key.pUnitData.IsTag(_UNITTAG._PEACE) || pair.Key.pUnitData.IsTag(_UNITTAG._HIDE) )
            {
                continue;
            }

            int nDist = pair.Value; // value is dist
			if( nDist > nSkillRange  ) // pathfind if need
			{
				List< iVec2> path = FindPathToTarget( mob , pair.Key , nMove , nSkillRange );
				if( path == null || (path.Count ==0) )
				{
					continue; // can't find path try next target
				}
				else
				{
					iVec2 last = path[ path.Count -1 ];
					if( last != null  ){
						int nDist2 = last.Dist( pair.Key.Loc );
						if( (nDist2 > nSkillRange) || (nDist2 < nMinRange) ){
							// 檢查要不要換skill打
							SKILL newSkill = FindSkillByDist( mob , pair.Key.pUnitData , nDist2);
							if( newSkill == null ){
								//	continue; // can't find skill. use original skill
							}
							else{
								// change skill to atk
								nSkillID = newSkill.n_ID ;
								bCanAtk = true;		// can atk

								// send a move event					
								mob.SetPath (path); 
								// check if last pos is attack able pos
								ActionManager.Instance.CreateMoveAction (ident, last.X, last.Y);	
								
								if (Config.GOD == true) {
									Panel_StageUI.Instance.CreatePathOverEffect (path); // draw path
								}
							}
						}
						// check range can atk

					}

				}

			}
			else if( nDist < nMinRange ){ // check if need change skill
				SKILL newSkill = FindSkillByDist( mob , pair.Key.pUnitData, nDist );
				if( newSkill != null ){	// change skill
					nSkillID = newSkill.n_ID ;
				}else if( nDist <= 1 ){
					nSkillID = 0;
				}
				bCanAtk = true;
			}
			else{
				bCanAtk = true ; // atk directly
				
			}
			//check can atk or wait
			if( bCanAtk )
			{
				ActionManager.Instance.CreateAttackCMD (ident, pair.Key.Ident (), nSkillID ); // create Attack CMD . need battle manage to run
				
				return true;
			}
			// for next target
		}
		// all target failed find again for nexreat target
		foreach (KeyValuePair<Panel_unit , int> pair in items) {
			// try path to target
			bool bCanAtk = false;
            if (pair.Key.pUnitData == null || pair.Key.pUnitData.IsTag(_UNITTAG._PEACE) || pair.Key.pUnitData.IsTag(_UNITTAG._HIDE) )
            {
                continue;
            }

            int nDist = pair.Value; // value is dist
			if( nDist > nSkillRange  ) // pathfind if need
			{
				List< iVec2> path = FindPathToTarget( mob , pair.Key , nMove );
				if( path == null || (path.Count ==0) )
				{
					continue; // can't find path try next target
				}
				else
				{
					iVec2 last = path[ path.Count -1 ];
					if( last != null  ){
						int nDist2 = last.Dist( pair.Key.Loc );
						if( (nDist2 > nSkillRange) || (nDist2 < nMinRange) ){
							// too far .. no attack
							// 檢查要不要換skill打
							SKILL newSkill = FindSkillByDist( mob , pair.Key.pUnitData , nDist2 );
							if( newSkill == null ){
								//continue; //  don't continue . set cmd for this loop
							}
							else{
								// change skill
								nSkillID = newSkill.n_ID ;
								bCanAtk = true;
							}
						}
						else {
							bCanAtk = true;
						}
						// send a move event					
						mob.SetPath (path); 
						// check if last pos is attack able pos
						ActionManager.Instance.CreateMoveAction (ident, last.X, last.Y);	
						
						if (Config.GOD == true) {
							Panel_StageUI.Instance.CreatePathOverEffect (path); // draw path
						}
						// check range can atk

					}
					
				}
				
			}
			else if( nDist < nMinRange ){ // check if need change skill
				SKILL newSkill = FindSkillByDist( mob , pair.Key.pUnitData, nDist );
				if( newSkill != null ){	// change skill
					nSkillID = newSkill.n_ID ;
					bCanAtk = true;
				}else if( nDist <= 1 ){
					nSkillID = 0;
					bCanAtk = true;
				}
			}
			else{
				bCanAtk = true ; // atk directly
				
			}
			//check can atk or wait
			if( bCanAtk )
			{
				ActionManager.Instance.CreateAttackCMD (ident, pair.Key.Ident (), nSkillID ); // create Attack CMD . need battle manage to run
				
				return true;
			}
			else{ // imort for break loop

                _AI_MakeWaitCmd(mob);
                //ActionManager.Instance.CreateWaitingAction (mob.Ident ());
                return true;
			}
			// for next target
		}


		// all target faild.. waiting
		return false;
	}


    //==================================
    static bool _AI_CastBuff(Panel_unit mob, int nSkillID, int nMove)
    {
        // 決定要不要施展 buff 技能
        if (CreateBuffTmpList(mob.pUnitData,null , 0, false )) // create skill pool to atl
        {
            // 
            foreach (SKILL skl in tmpSklList)
            {
                ActionManager.Instance.CreateCastCMD(mob.Ident(), 0, 0, skl.n_ID); // create Attack CMD . need battle manage to run				
                return true;              
            }
        }

        return false;
    }


    static void _AI_NormalAttack(Panel_unit mob, int nMove)
    {
       
        // 範圍內 優先找 血少
        if (_AI_LowstAttack2(mob, nMove, false))
        {
            return;
        }
       

        // 1. 找範圍內 可攻擊目標 , 2. 主動尋找 最近目標
        if (_AI_NearestAttack2(mob, nMove, true))
        {
            return;
        }

        //都不行則待機
        _AI_MakeWaitCmd(mob);
        //ActionManager.Instance.CreateWaitingAction(mob.Ident());
        // all target faild.. waiting

    }

    static  void _AI_PassiveAttack( Panel_unit mob , int nMove )
	{
        if (_AI_LowstAttack2(mob, nMove, false ))
        {
            return;
        }
        // 找距離近的攻擊
        if (_AI_NearestAttack2(mob, nMove, false )) // 不主動出擊
        {
            return;
        }


        //都不行則待機
        _AI_MakeWaitCmd(mob);
        //ActionManager.Instance.CreateWaitingAction(mob.Ident());


        return;

		//Dictionary< Panel_unit , int > pool = Panel_StageUI.Instance.GetUnitDistPool(mob, true);
		//int ident = mob.Ident ();
		
		//// get skill range
		//int nMinRange =0;
		//int nSkillRange =0;
		//MyTool.GetSkillRange ( nSkillID , out nSkillRange , out nMinRange);
		//// Sort dist
		//var items = from pair in pool orderby pair.Value ascending select pair;
		
		//foreach (KeyValuePair<Panel_unit , int> pair in items) {
		//	// try path to target
		//	bool bCanAtk = false;
  //          if (pair.Key.pUnitData == null || pair.Key.pUnitData.IsTag(_UNITTAG._PEACE))
  //          {
  //              continue;
  //          }
  //          int nDist = pair.Value; // value is dist
		//	if( nDist > nSkillRange  ) // pathfind if need
		//	{
		//		// 距離太遠，怎樣都不可能到的 先放棄
		//		if( (nSkillRange+nMove) < nDist ){
		//			// 檢查要不要換skill打
		//			SKILL newSkill = FindSkillByDist( mob , (nDist-nMove) );
		//			if( newSkill == null ){
		//				continue; // 太遠了~放棄不打
		//			}
		//			else{
		//				// change skill
		//				nSkillID 	= newSkill.n_ID ;
		//				nSkillRange = newSkill.n_RANGE;
		//				//bCanAtk = true;
		//			}
		//		}

		//		List< iVec2> path = FindPathToTarget( mob , pair.Key , nMove , nSkillRange );
		//		if( path == null || (path.Count ==0) )
		//		{
		//			continue; // can't find path try next target
		//		}
		//		else
		//		{
		//			iVec2 last = path[ path.Count -1 ];
		//			if( last != null  ){
		//				int nDist2 = last.Dist( pair.Key.Loc );

		//				if( nDist2 > nSkillRange ){
		//					SKILL newSkill = FindSkillByDist( mob , (nDist-nMove) );
		//					if( newSkill == null ){
		//						continue; // 太遠了~放棄不打
		//					}
		//					else{
		//						// change skill
		//						nSkillID 	= newSkill.n_ID ;

		//					}
		//				}

		//				// send a move event					
		//				mob.SetPath (path); 
		//				// check if last pos is attack able pos
		//				ActionManager.Instance.CreateMoveAction (ident, last.X, last.Y);	
						
		//				if (Config.GOD == true) {
		//					Panel_StageUI.Instance.CreatePathOverEffect (path); // draw path
		//				}
		//				bCanAtk = true;
		//			}

		//		}

		//	}
		//	else if( nDist < nMinRange ){ // check if need change skill
		//		SKILL newSkill = FindSkillByDist( mob ,nDist );
		//		if( newSkill != null ){	// change skill
		//			nSkillID = newSkill.n_ID ;
		//			bCanAtk = true;
		//		}else if( nDist <= 1 ){
		//			nSkillID = 0;
		//			bCanAtk = true;
		//		}
		//	}
		//	else{
		//		bCanAtk = true ; // atk directly				
		//	}
			
		//	if( bCanAtk )
		//	{
		//		ActionManager.Instance.CreateAttackCMD (ident, pair.Key.Ident (), nSkillID ); // create Attack CMD . need battle manage to run
				
		//		return ;
		//	}
		//	// for next target			
		//}

		//// all target faild.. waiting
		//ActionManager.Instance.CreateWaitingAction (ident);
	}

	static void _AI_Defence(Panel_unit mob  , int nMove ) 
	{
		int ident = mob.Ident();
        _AI_MakeWaitCmd(mob);
        //ActionManager.Instance.CreateWaitingAction (ident); // always wait in pos
    }

    // 攻擊指定目標
	static void _AI_TargetAttack(Panel_unit mob  , int nMove ) 
	{
		int ident = mob.Ident();
		cUnitData data = GameDataManager.Instance.GetUnitDateByIdent ( ident ); 
		Panel_unit target =Panel_StageUI.Instance.GetUnitByCharID( data.n_AITarget );
		//cUnitData tar = GameDataManager.Instance.GetUnitDateByIdent ( data.n_AITarget ); 
		if( target == null) {
			// 目標死亡，切回正常模式
			_AI_NormalAttack( mob ,nMove  );
			return ;
		}
        //如果目標隱藏，改用正常AI
        if (target.IsHide())
        {
            _AI_NormalAttack(mob, nMove);
            return;
        }

        int nSkillID=0;
        //int nSkillID;
        List<iVec2> path; ;
        if (_FindToAttackTarget(mob, target, nMove, out nSkillID, out path, false, true))
        {
            // 先判斷是否要 上 buff
            if (_AI_CastBuff(mob, nSkillID, nMove))
            {
                return ; // 能施展下一個技能的話 先中斷並 重新收尋
            }

            _AI_MakeCmd(mob, target, nSkillID, ref path);
        }
        else {
            //不能攻擊指定目標，則隨意攻擊
            //  _AI_NormalAttack(mob, nSkillID, nMove);
            // 在指定地點 看看有沒有人可以攻擊
            iVec2 last = mob.Loc;
            Panel_unit tmpTar = null;
            int tmpSkill = 0;

            if (path!= null && path.Count > 0)
            {
                last = path[path.Count - 1];
                if (_AI_FindTargetAttackbyPos(mob, last, ref tmpSkill, ref tmpTar))
                {
                    target = tmpTar; // 變更目標
                    nSkillID = tmpSkill;
                }

                _AI_MakeCmd(mob, target, nSkillID, ref path); // 往最近的移動
                return;
            }
            //
            _AI_NormalAttack(mob, nMove);

            //  ActionManager.Instance.CreateWaitingAction(ident); // 與其待機，不如正常攻擊
        }
     

        return;
     
	}

    // only move
	static void _AI_PositionAttack(Panel_unit mob  , int nMove ) 
	{
		int ident = mob.Ident();
		cUnitData data = GameDataManager.Instance.GetUnitDateByIdent ( ident );
        // move to pos
        //iVec2 pos = new iVec2 ( data.n_AIX , data.n_AIY  );        
        //List< iVec2> path = Panel_StageUI.Instance.PathFinding ( mob, mob.Loc, pos , 999); // get a vaild path to run

        // range
        bool bFind = false;
        iVec2 vTar = new iVec2(data.n_AIX , data.n_AIY );
//        vTar.X = data.n_AIX;
//        vTar.Y = data.n_AIY;
        List<iVec2> path = null;
        iVec2 last = null;
        if (Panel_StageUI.Instance.CheckIsEmptyPos(vTar) == true)
        {
            // 正常可以移動
             path = FindPathToPos(mob, vTar.X , vTar.Y, nMove, 999);
            if (path != null && (path.Count > 0))
            {
                last = path[path.Count - 1];
                if (last != null)
                {
                  
                    bFind = true;

                }
            }

        }
        else { // 目標上面有障礙，找周圍點
            List<iVec2> nearList = vTar.AdjacentList(3); // the 4 pos can't stand ally
            foreach (iVec2 v in nearList)
            {
                path = null;  // clear data
                if (Panel_StageUI.Instance.CheckIsEmptyPos(v) == true)
                {
                     path = FindPathToPos(mob, v.X, v.Y, nMove, 999);
                    if (path != null && (path.Count > 0))
                    {
                        last = path[path.Count - 1];
                        if (last != null)
                        {
                            bFind = true;
                            break;
                        }
                    }
                }
            }
        }
       
        if (bFind)
        {
            if (last != null)
            {
                // send a move event					
                mob.SetPath(path);
                // check if last pos is attack able pos
                ActionManager.Instance.CreateMoveAction(ident, last.X, last.Y);
            }

            if (Config.GOD == true)
            {
                Panel_StageUI.Instance.CreatePathOverEffect(path); // draw path
            }
        }
        _AI_MakeWaitCmd(mob);       
    }

    static bool _AI_FindTargetAttackbyPos(Panel_unit mob, iVec2 pos , ref int nSkillID  , ref Panel_unit Tar )
    {

        iVec2 back = mob.Loc;
        int nMaxRange = _AI_GetMaxSkillRange(mob.pUnitData);

        mob.Loc = pos;
        Panel_StageUI.Instance.GetUnitHpPool(mob, true, nMaxRange); // all unit in                                                                                                            
        Dictionary<Panel_unit, int> pool = Panel_StageUI.Instance.m_tmpHpPool;
        var items = from pair in pool orderby pair.Value ascending select pair;             // Sort HP
        // 範圍內 低血量
        foreach (KeyValuePair<Panel_unit, int> pair in items)
        {
            Panel_unit target = pair.Key;
            int nDist = pos.Dist(target.Loc);

            if (CreateSkilTmpList(mob.pUnitData, target.pUnitData, nDist, 0,  false)) // create skill pool to atk
            {
                foreach (SKILL skl in tmpSklList)
                {
                    int nMinRange = skl.n_MINRANGE;
                    int nSkillRange = skl.n_RANGE;

                    if ((nDist <= nSkillRange) && (nDist >= nMinRange)) // 可以直接攻擊
                    {
                        mob.Loc = back;
                        nSkillID = skl.n_ID;
                        Tar = target;
                        return true; // tmpSklList
                    }
                }
            }

        }
        mob.Loc = back;
        return false;
    }

    // tool finc
    public static List<iVec2> FindPathToTarget( Panel_unit Mob , Panel_unit Target  , int nMove , int nRange =1  )
	{
		if( Mob == null || Target == null || nMove ==0 )
        {
			return null;
		}

        long tick = System.DateTime.Now.Ticks;
        //int ident = Mob.Ident();

        List< iVec2 > nearList = Target.Loc.AdjacentList ( nRange ); // the 4 pos can't stand ally
        distpool.Clear();
		foreach (iVec2 v in nearList) {
			if (Panel_StageUI.Instance.CheckIsEmptyPos (v) == true) {// 目標 pos 不可以站人 也不可以是ZOC
				int d =v.Dist (Mob.Loc);
				distpool.Add (v, d );
			}
		}
       

        // dist sort
        // try each path until can atk
        var itemsdist = from pair2 in distpool orderby pair2.Value ascending select pair2;
		foreach (KeyValuePair<iVec2 , int> pair2 in itemsdist) {

		//	nDist = pair2.Value; // try other
			iVec2 last = null;
			
			List< iVec2> path = Panel_StageUI.Instance.PathFinding ( Mob, Mob.Loc, pair2.Key, 999); // get a vaild path to run
			
			// 由於底層搜尋 有快 A-str 兩種>
			int nLimit = nMove;
			if( path != null && path.Count > 0 ){
				iVec2 loc = path[0];
				if( Mob.Loc.Collision( loc ) ){
					nLimit++;
				}
				else {
					nLimit= nMove;  // this will happen?
				}
			}

			path = MyTool.CutList<iVec2> (path, nLimit);// mob movement; .. A-star 運算時自己原座標也算一個點所以這邊加給他
			
			// avoid stand on invalid pos
			while (path.Count > 0) {
				last = path [path.Count - 1];
				if (Panel_StageUI.Instance.CheckIsEmptyPos (last) == false) {
					path.RemoveAt (path.Count - 1); // then go again
					
				} 
				else if( path.Count <= 0 )
				{
					continue; // try next pos
				}
				else {
                    // get a sortest path 
                    if (Debug.isDebugBuild == true)
                    {
                        long during = System.DateTime.Now.Ticks - tick;
                        Debug.LogFormat("Mob{0}-{1} FindPathToTarget OK {2} phase try each unit can atk spend {3}ms ", Mob.CharID, Mob.Ident(), Target.CharID, during / 10000);
                        tick = System.DateTime.Now.Ticks;
                    }

                    return path; 
				}
			}
		}
        if (Debug.isDebugBuild == true)
        {
            long during = System.DateTime.Now.Ticks - tick;
            Debug.LogFormat("Mob{0}-{1} FindPathToTarget FAIL {2} phase try each unit can atk spend {3}ms ", Mob.CharID, Mob.Ident() , Target.CharID, during / 10000   );
            tick = System.DateTime.Now.Ticks;
        }
        // all path failed. return null
        //try method to find path move near to target
        // find target to atk
        // move == 0 時將造成這一段 太多的無意義收尋。所以 應該在最上方 move==0 return

        // 太多的loop 容易有問題
     


        int c = 0;
        for (int i = 2; i < 3; i++) // range
        {
            // pool.Clear();
            List<iVec2> pool = Panel_StageUI.Instance.Grids.GetRangePool(Target.Loc, i, i - 1);
            if (pool == null)
                continue;
            distpool.Clear();
            foreach (iVec2 pos in pool)
            {
                if (Panel_StageUI.Instance.CheckIsEmptyPos(pos) == false)
                {
                    continue;
                }
                distpool.Add(pos, pos.Dist(Mob.Loc));
            }
            // for each
            var dist2 = from pair2 in distpool orderby pair2.Value ascending select pair2;
            foreach (KeyValuePair<iVec2, int> pair2 in dist2)
            {
               // { 
                iVec2 last = null;
                List<iVec2>  path = Panel_StageUI.Instance.PathFinding(Mob , Mob.Loc, pair2.Key, 999); // get a vaild path to run
                c++;                                                                          //path = FindPathToPos(mob, pos, nMove, nSkillRange);
                int nLimit = nMove;
                if (path != null && (path.Count > 0)) // 這邊有路徑就算數了
                {
                    //底層 A*問題，避免起點也記入
                   // if (path != null && path.Count > 0)
                   // {
                   // 
                        iVec2 loc = path[0];
                        if (Mob.Loc.Collision(loc))
                        {
                            nLimit++;
                        }
                        else
                        {
                            nLimit = nMove;  // this will happen?
                        }
                   // }

                    path = MyTool.CutList<iVec2>(path, nLimit);// mob movement; .. A-star 運算時自己原座標也算一個點所以這邊加給他

                    // avoid stand on invalid pos
                    while (path.Count > 0)
                    {
                        last = path[path.Count - 1];
                        if (Panel_StageUI.Instance.CheckIsEmptyPos(last) == false)
                        {
                            path.RemoveAt(path.Count - 1); // then go again
                        }
                        else if (path.Count <= 0)
                        {
                            continue; // try next pos
                        }
                        else {
                            if (Debug.isDebugBuild == true)
                            {
                                float during = System.DateTime.Now.Ticks - tick;
                                Debug.LogFormat("Mob{0}-{1} FindPathToTarget  OK {2} phase find a path to atk unit can atk spend {3} ms , {4} times", Mob.CharID, Mob.Ident() , Target.CharID, during / 10000, c);
                                tick = System.DateTime.Now.Ticks;
                            }


                            return path;
                        }
                    }

                //  int num = path.Count;
                //  Debug.LogFormat("Mob{0} FindPathToTarget{1} POS({2}, {3}) with num {4}", Mob.CharID, Target.CharID , pos.X , pos.Y  , num);

                }
            }           
        }

        // last find to target

        if (Debug.isDebugBuild == true)
        {
            float during = System.DateTime.Now.Ticks - tick;
            Debug.LogFormat("Mob{0}-{1} FindPathToTarget FAIL {2} phase find a path to atk unit can atk spend {3} ms , {4} times", Mob.CharID, Mob.Ident(), Target.CharID, during / 10000, c);
            tick = System.DateTime.Now.Ticks;
        }
        return null;

	}


    public static List<iVec2> FindPathToPos(Panel_unit Mob, int nTarX , int nTarY , int nMove, int nRange = 1)
    {
        if (Mob == null || nMove == 0)
        {
            return null;
        }

        long tick = System.DateTime.Now.Ticks;
        //int ident = Mob.Ident();

        iVec2 vTar = new iVec2(nTarX , nTarY);
        distpool.Clear();        
        if (Panel_StageUI.Instance.CheckIsEmptyPos(vTar) == true)
        {
            distpool.Add(vTar, 0);
        }


        List<iVec2> nearList = vTar.AdjacentList(nRange); // the 4 pos can't stand ally
        foreach (iVec2 v in nearList)
        {
            if (Panel_StageUI.Instance.CheckIsEmptyPos(v) == true)
            {// 目標 pos 不可以站人 也不可以是ZOC
                int d = v.Dist(Mob.Loc);
                distpool.Add(v, d);
            }
        }


        // dist sort
        // try each path until can atk
        var itemsdist = from pair2 in distpool orderby pair2.Value ascending select pair2;
        foreach (KeyValuePair<iVec2, int> pair2 in itemsdist)
        {

            //	nDist = pair2.Value; // try other
            iVec2 last = null;

            List<iVec2> path = Panel_StageUI.Instance.PathFinding(Mob, Mob.Loc, pair2.Key, 999); // get a vaild path to run

            // 由於底層搜尋 有快 A-str 兩種>
            int nLimit = nMove;
            if (path != null && path.Count > 0)
            {
                iVec2 loc = path[0];
                if (Mob.Loc.Collision(loc))
                {
                    nLimit++;
                }
                else
                {
                    nLimit = nMove;  // this will happen?
                }
            }

            path = MyTool.CutList<iVec2>(path, nLimit);// mob movement; .. A-star 運算時自己原座標也算一個點所以這邊加給他

            // avoid stand on invalid pos
            while (path.Count > 0)
            {
                last = path[path.Count - 1];
                if (Panel_StageUI.Instance.CheckIsEmptyPos(last) == false)
                {
                    path.RemoveAt(path.Count - 1); // then go again

                }
                else if (path.Count <= 0)
                {
                    continue; // try next pos
                }
                else
                {
                    // get a sortest path 
                    if (Debug.isDebugBuild == true)
                    {
                        long during = System.DateTime.Now.Ticks - tick;
                        Debug.LogFormat("Mob{0}-{1} FindPathToPos OK ({2},{3}) phase try each unit can atk spend {4}ms ", Mob.CharID, Mob.Ident(), nTarX , nTarY, during / 10000);
                        tick = System.DateTime.Now.Ticks;
                    }

                    return path;
                }
            }
        }
        if (Debug.isDebugBuild == true)
        {
            long during = System.DateTime.Now.Ticks - tick;
            Debug.LogFormat("Mob{0}-{1} FindPathToPos FAIL ({2},{3}) phase try each unit can atk spend {4}ms ", Mob.CharID, Mob.Ident(), nTarX, nTarY, during / 10000);
            tick = System.DateTime.Now.Ticks;
        }
        // all path failed. return null
        //try method to find path move near to target
        // find target to atk
        // move == 0 時將造成這一段 太多的無意義收尋。所以 應該在最上方 move==0 return

        // 太多的loop 容易有問題
        int c = 0;
        for (int i = 2; i < 3; i++) // range
        {
            // pool.Clear();
            List<iVec2> pool = Panel_StageUI.Instance.Grids.GetRangePool( vTar, i, i - 1);
            if (pool == null)
                continue;
            foreach (iVec2 pos in pool)
            {
                iVec2 last = null;
                if (Panel_StageUI.Instance.CheckIsEmptyPos(pos) == false)
                {
                    continue;
                }

                List<iVec2> path = Panel_StageUI.Instance.PathFinding(Mob, Mob.Loc, pos, 999); // get a vaild path to run
                c++;                                                                          //path = FindPathToPos(mob, pos, nMove, nSkillRange);
                int nLimit = nMove;
                if (path != null && (path.Count > 0)) // 這邊有路徑就算數了
                {
                    //底層 A*問題，避免起點也記入
                    // if (path != null && path.Count > 0)
                    // {
                    // 
                    iVec2 loc = path[0];
                    if (Mob.Loc.Collision(loc))
                    {
                        nLimit++;
                    }
                    else
                    {
                        nLimit = nMove;  // this will happen?
                    }
                    // }

                    path = MyTool.CutList<iVec2>(path, nLimit);// mob movement; .. A-star 運算時自己原座標也算一個點所以這邊加給他

                    // avoid stand on invalid pos
                    while (path.Count > 0)
                    {
                        last = path[path.Count - 1];
                        if (Panel_StageUI.Instance.CheckIsEmptyPos(last) == false)
                        {
                            path.RemoveAt(path.Count - 1); // then go again
                        }
                        else if (path.Count <= 0)
                        {
                            continue; // try next pos
                        }
                        else
                        {
                            if (Debug.isDebugBuild == true)
                            {
                                float during = System.DateTime.Now.Ticks - tick;
                                Debug.LogFormat("Mob{0}-{1} FindPathToPos  OK ({2},{3}) phase find a path to atk unit can atk spend {4} ms , {5} times", Mob.CharID, Mob.Ident(), nTarX, nTarY, during / 10000, c);
                                tick = System.DateTime.Now.Ticks;
                            }


                            return path;
                        }
                    }

                    //  int num = path.Count;
                    //  Debug.LogFormat("Mob{0} FindPathToTarget{1} POS({2}, {3}) with num {4}", Mob.CharID, Target.CharID , pos.X , pos.Y  , num);

                }
            }
        }
        if (Debug.isDebugBuild == true)
        {
            float during = System.DateTime.Now.Ticks - tick;
            Debug.LogFormat("Mob{0}-{1} FindPathToPos FAIL phase find a path to ({2},{3}) unit can atk spend {4} ms , {5} times", Mob.CharID, Mob.Ident(), nTarX , nTarY, during / 10000, c);
            tick = System.DateTime.Now.Ticks;
        }
        return null;

    }
    static public int SelCountSkill(cUnitData pMob, cUnitData pTarget = null)
    {
        cUnitData pData = pMob;
        if (pData == null)
        {
            return 0;  // no attack
        }
        // defence ai always defence
        if (pData.eComboAI == _AI_COMBO._DEFENCE)
        {
            return -1;
        }
        // 被暈眩的不可以反擊
        if (pData.IsStun() ) {
            return -1;
        }

        int nDist = 0;
        int nTarX = pMob.n_X;
        int nTarY = pMob.n_Y;
        if (pTarget != null)
        {
            nTarX = pTarget.n_X;
            nTarY = pTarget.n_Y;
            nDist = iVec2.Dist(pMob.n_X, pMob.n_Y, pTarget.n_X, pTarget.n_Y);
        }
        if (CreateSkilTmpList(pMob, pTarget, nDist, 0, true))
        {
            // roll a skill?
            foreach (SKILL skl in tmpSklList)
            {
                int nMinRange = skl.n_MINRANGE;
                int nSkillRange = skl.n_RANGE;

                if ( (skl.n_TARGET==0) ||( (nDist <= nSkillRange) && (nDist >= nMinRange)) ) // 可以直接攻擊
                {
                    return skl.n_ID;
                }
            }


        }
        else {
            if( nDist <= 1) // use melee
            {
                return 0;
            }

        }
        return -1; // defence

    }

    static public int SelSkill( cUnitData pMob , cUnitData pTarget = null , bool bCounterMode = false  )// -1 is no attack
	{
	//	return 11701; // debug
		//nDeferSkillID = 11704;  //天羅地網
		cUnitData pData = pMob;
		if( pData == null ){
			return 0;  // no attack
		}
		// defence ai always defence
		if( pData.eComboAI == _AI_COMBO._DEFENCE ){
			return -1;
		}

		int nDist = 0;
		int nTarX =pMob.n_X;
		int nTarY =pMob.n_Y;
		if( pTarget != null ){
			nTarX = pTarget.n_X;
			nTarY = pTarget.n_Y;
			nDist = iVec2.Dist(pMob.n_X , pMob.n_Y ,pTarget.n_X , pTarget.n_Y );
		}

		// 普攻永遠有機會 ,CP 越高，普攻機會越低
		if( nDist <= 1 ){
			int r = Random.Range( 0, pData.GetMaxCP() );
			if( r > pData.n_CP ){
				return 0;				// select normal attack
			}
		}
        // 全部權重
        //int nTotalWidget = 0;
        // list skill 
        //List< SKILL> sklLst = new List< SKILL>();	
        //Dictionary< int , int > sklPool  = new Dictionary< int , int >();

        sklPool.Clear();
		// normal attack

		//sklPool.Add( 0 , (pData.GetMaxCP() - pData.n_CP) +1 ); // 普攻永遠有機會 ,CP 越高，普攻機會越低

		// always use most widget skill
		int nWidget = 0;
		int nFinalSkillID = 0;


		foreach(  int  nID in pData.SkillPool ){
				int nSkillID = nID;
				nSkillID = pData.Buffs.GetUpgradeSkill( nSkillID ); // Get upgrade skill
				if( nSkillID ==0 )
					continue;

				if( CheckSkillCanCast( pMob , pTarget , nSkillID, nDist , bCounterMode ) == false ){	
					continue;
				}

				SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nSkillID);				
//				if( skl.n_SCHOOL == 0 )	// == 0 is ability
//					continue;				
//				if( skl.n_PASSIVE == 1 )
//					continue;
//
//				// check cp && MP
//				if( (pData.n_CP < skl.n_CP) ){
//					continue;
//				}
//				if( (pData.n_MP < skl.n_MP) ){
//					if( Config.FREE_MP == false ){
//						continue;
//					}
//				}			
//
//
//				// 防招只在破防時使用
//				if( skl.f_DEF > 0.0f && pData.n_DEF>0)
//					continue;
//
//				//牽制招不對小怪物用
//				cSkillData skldata = GameDataManager.Instance.GetSkillData( nSkillID );
//				if( skldata.IsTag( _SKILLTAG._TIEUP ) )
//				{
//					if( pTarget == null )
//						continue;
//					if( (pMob.GetMar() - pTarget.GetMar()) > 20.0f )
//						continue;
//				}


//				// check range when counter mode
//				if( bCounterMode){
//					// range cheeck
//					int nRange ;
//					int nMinRange;
//					MyTool.GetSkillRange( nSkillID , out nRange , out nMinRange );
//				
//					if( nMinRange > nDist )
//						continue; // too near can't cast
//
//					// counter can't move
//					if( nRange < nDist )
//						continue;
//
//					// 點地技能不能放	
//					//if (skldata.IsTag (_SKILLTAG._BANDEF )) {
//					//	continue;
//					//}
//
//					if( (skl.n_TARGET==3) || (skl.n_TARGET==4) || (skl.n_TARGET==5) ){
//					//			case 6:	//6→自我AOE我方
//					//			case 7:	//7→自我AOE敵方
//					//			case 8:	//8→自我AOEALL
//					//	//		case 3:	//→MAP敵方
//					//	//		case 4: //→MAP我方
//					//	//		case 5:	//→MAPALL							
//						continue;
//					}
//				}
//				else{// normal atk
//					//if (skldata.IsTag (_SKILLTAG._BANATK )) {
//					//	continue;
//					//}
//				}
//				// 

				int tmpWidget = (skl.n_CP + skl.n_LEVEL_LEARN) ; // cp is base widget

				// 	MODIFY widget 
//				if( skl.n_AREA != 0 ){
//					List< cUnitData > AffectPool = new List< cUnitData >(); //影響人數
//
//					BattleManager.GetAffectPool( pMob , pTarget, skl.n_ID, nTarX ,nTarY , ref  AffectPool  );
//					widget += (4*AffectPool.Count); //  fail method . the AI need a new solution 影響人越多，權重越大
//
//					AffectPool.Clear();
//				}
				if( tmpWidget > nWidget )
				{
					nWidget = tmpWidget;
					nFinalSkillID = nSkillID;
				}


				//sklPool.Add( nSkillID , widget );
				//sklLst.Add(  skl );
		}

		// 
		if( (nFinalSkillID > 0) )
		{
			// random a skill
			//int id = MyTool.RollWidgetPool( sklPool );
			return nFinalSkillID;
		}
		else{ // if chose normal attack final
			if( bCounterMode ){
				if( nDist > 1 ){
					return -1;
				}
			}
		}

		return 0; // normal attack
	}


    static public bool CreateSkilTmpList( cUnitData pData , cUnitData pTarget,  int nDist, int nMove ,  bool bCounterMode = false )
    {
        if (pData == null)
            return false;     
        
        int nRealDist = nDist- nMove; // 實際距離
        if (nRealDist < 1)
            nRealDist = 1;

        tmpSklList.Clear();
        foreach (int nID in pData.SkillPool)
        {
            int nSkillID = nID;
            nSkillID = pData.Buffs.GetUpgradeSkill(nSkillID); // Get upgrade skill
            if (nSkillID == 0)
            {
                continue;
            }

            // 不可移動攻擊技能。不能考慮移動力
            if (MyTool.IsSkillTag(nSkillID, _SKILLTAG._NOMOVE))
            {
                if (CheckSkillCanCast(pData, pTarget, nSkillID, nDist, bCounterMode) == false)
                {
                    continue;
                }
            }
            else
            {
                // 一般技能
                if (CheckSkillCanCast(pData, pTarget, nSkillID, nRealDist, bCounterMode) == false)
                {
                    continue;
                }
            }
            



            SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nSkillID);
            // 必須不是 精神 技能
            if (skl.n_SCHOOL == 0)
                continue;
            // 必須是終結技能
            if (skl.n_FINISH == 0)
                continue;
            tmpSklList.Add(skl);
            
        }
        // revert for high skill use fast
        tmpSklList.Reverse();

        return (tmpSklList.Count > 0 );
    }

	static public SKILL FindSkillByDist( Panel_unit mob , cUnitData pTarget  , int nDist  , bool bCounterMode = false )
	{	
		cUnitData pData = mob.pUnitData;
		if( pData == null )
			return null;


		foreach(  int  nID in pData.SkillPool ){
			int nSkillID = nID;
			nSkillID = pData.Buffs.GetUpgradeSkill( nSkillID ); // Get upgrade skill
			if( nSkillID ==0 ){
				continue;
			}

			if( CheckSkillCanCast( pData , pTarget,  nSkillID , nDist , bCounterMode ) == false  ){
				continue;
			}

			SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nSkillID);
			return skl ;
		}
		return null;
	}

    static public bool CreateBuffTmpList(cUnitData pData, cUnitData pTarget, int nDist, bool bCounterMode = false)
    {
        if (pData == null)
            return false;

        int nRealDist = nDist;
        if (nRealDist < 1)
            nRealDist = 1;

        tmpSklList.Clear();
        foreach (int nID in pData.SkillPool)
        {
            int nSkillID = nID;
            nSkillID = pData.Buffs.GetUpgradeSkill(nSkillID); // Get upgrade skill
            if (nSkillID == 0)
            {
                continue;
            }

            if (CheckSkillCanCast(pData, pTarget, nSkillID, nRealDist, bCounterMode) == false)
            {
                continue;
            }

            SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nSkillID);

            // 必須不是 精神 技能
            if (skl.n_SCHOOL == 0)
                continue;
            // 不能是終結技能
            if (skl.n_FINISH == 1)
                continue;

            tmpSklList.Add(skl);

        }
        // revert for high skill use fast
        tmpSklList.Reverse();

        return (tmpSklList.Count > 0);
    }

    //建立 待機技能
    static public bool CreateWaitTmpList(cUnitData pData, cUnitData pTarget, int nDist, bool bCounterMode = false)
    {
        if (pData == null)
            return false;

        int nRealDist = nDist;
        if (nRealDist < 1)
            nRealDist = 1;

        tmpSklList.Clear();
        foreach (int nID in pData.SkillPool)
        {
            int nSkillID = nID;
            nSkillID = pData.Buffs.GetUpgradeSkill(nSkillID); // Get upgrade skill
            if (nSkillID == 0)
            {
                continue;
            }

            if (CheckSkillCanCast(pData, pTarget, nSkillID, nRealDist, bCounterMode) == false)
            {
                continue;
            }

            SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nSkillID);

            // 必須不是 精神 技能
            if (skl.n_SCHOOL == 0)
                continue;
            cSkillData skldata = GameDataManager.Instance.GetSkillData(nSkillID);
            if ( skldata.IsTag(_SKILLTAG._WAITING) )
            {
                tmpSklList.Add(skl);
            }

        }
        // revert for high skill use fast
        tmpSklList.Reverse();

        return (tmpSklList.Count > 0);
    }

    // tool func to check skill can use
    static public bool CheckSkillCanCast(cUnitData pData, cUnitData pTarget, int nSkillID, int nDist, bool bCounterMode = false) {
        if (pData == null) {
            return false;  // no attack
        }

        SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nSkillID);
        if (skl.n_SCHOOL == 0)  // == 0 is ability，怪物 AI 不會放 精神指令
            return false;

        if (pData.CheckSkillCanUse(skl) == false)
            return false;


        if (skl.n_PASSIVE == 1)
            return false;

        // check cp && MP
        //if ((pData.n_CP < skl.n_CP)) {
        //    return false;
        //}
        //if ((pData.n_MP < skl.n_MP)) {
        //    if (Config.FREE_MP == false) {
        //        return false;
        //    }
        //}
        // 防招只在破防時使用
    //    if (skl.f_DEF > 0.0f && pData.n_DEF > 0)
    //        return false;

        //牽制招不對小怪物用
        cSkillData skldata = GameDataManager.Instance.GetSkillData(nSkillID);
        if (skldata.IsTag(_SKILLTAG._TIEUP))
        {
            if (pTarget != null) {
                if ((pData.GetMar() - pTarget.GetMar()) > 20.0f) {
                    return false;
                }
            }
        }

        if (pData.eComboAI == _AI_COMBO._DEFENCE) {
            if (skldata.IsTag(_SKILLTAG._DAMAGE))
            {
                return false;       // defence no use dmg skill
            }
        }

        // 射程判斷
        if (skl.n_TARGET != 0) // 除了自我施法外，要判斷距離
        {
            if (skl.n_TARGET == 9 || skl.n_TARGET == 10 || skl.n_TARGET == 11)
            {
                // 全畫面攻擊
            }
            else    // 正常距離判斷
            {
                int nRange;
                int nMinRange;
                MyTool.GetSkillRange(nSkillID, out nRange, out nMinRange);

                if (nMinRange > nDist)
                    return false; // too near can't cast

                // counter can't move
                if (nRange >= 0 && nRange < nDist)
                    return false;
            }
        }


        // counter mode
        if ( bCounterMode){
			if (skldata.IsTag (_SKILLTAG._BANDEF )) { // 反擊禁用
				return false;
			}

			//反擊禁用點地
			if( (skl.n_TARGET==3) || (skl.n_TARGET==4) || (skl.n_TARGET==5) ){
				//			case 6:	//6→自我AOE我方
				//			case 7:	//7→自我AOE敵方
				//			case 8:	//8→自我AOEALL
				//	//		case 3:	//→MAP敵方
				//	//		case 4: //→MAP我方
				//	//		case 5:	//→MAPALL							
				return false;
			}

		}
		else{
			if (skldata.IsTag (_SKILLTAG._BANATK )) {
				return false;
			}
		}
		// range check
		//if( ( skl.n_RANGE>= 0 && skl.n_RANGE < nDist) || ( skl.n_MINRANGE > nDist ) ){
		//	return false;
		//}

        // 敵我判斷
        if (pTarget != null)
        {
            if (MyTool.CanPK(pData.eCampID, pTarget.eCampID))
            {
                if (MyTool.GetSkillPKmode(skl) == _PK_MODE._PLAYER)
                {
                    return false;
                }
            }
            else
            {
                if (MyTool.GetSkillPKmode(skl) == _PK_MODE._ENEMY)
                {
                    return false;
                }
            }
        }
        return true;
	}

    static public int _AI_GetMaxSkillRange(cUnitData pData )
    {        
        if (pData == null)
            return 0;

        int nRange = 0;
        foreach (int nID in pData.SkillPool)
        {
            int nSkillID = nID;
            nSkillID = pData.Buffs.GetUpgradeSkill(nSkillID); // Get upgrade skill
            if (nSkillID == 0)
            {
                continue;
            }
            // 必須檢查能否施展技能
            

            SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nSkillID);

            // 看 MP 能不能施展
            if (pData.CheckSkillCanUse(skl) == false)
                continue;

            //if (bMove) {
            //    if (MyTool.IsSkillTag(nSkillID, _SKILLTAG._NOMOVE))
            //    {
            //        continue; // 不能移動攻擊的技能 計算
            //    }
            //}

            if (skl.n_RANGE > nRange) {
                nRange = skl.n_RANGE;
            }            
        }
        return nRange;
    }

}
