using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MYGRIDS;
using MyClassLibrary;

public class MobAI  {




	public static void Run( Panel_unit mob )
	{
		int ident = mob.Ident ();
		cUnitData mobdata = GameDataManager.Instance.GetUnitDateByIdent ( ident );
		int nSkillID = -1;		// 0 - no attack
		// select a skill
		if( mobdata.eComboAI == _AI_COMBO._NORMAL ){
			nSkillID = SelSkill( mob.pUnitData );
		}

		int nMove = mobdata.GetMov ()  ; 
		switch( mobdata.eSearchAI )
		{
			case _AI_SEARCH._NORMAL:{ //主動攻擊 - 優先打血少
				_AI_NormalAttack( mob , nSkillID ,  nMove ) ;
			}break;
			case _AI_SEARCH._PASSIVE:{ //被動攻擊 - 有人在範圍內 優先打近的
				_AI_PassiveAttack( mob , nSkillID ,  nMove ) ;
			}break;
			case _AI_SEARCH._DEFENCE:{ //堅守原地
				_AI_Defence( mob , nSkillID ,  0 ) ;
			}break;
			case _AI_SEARCH._TARGET:{ //前往指定
				_AI_TargetAttack( mob , nSkillID ,  nMove ) ;
			}break;	
			case _AI_SEARCH._POSITION:{ //不在目標地點前往目標
				_AI_PositionAttack( mob , nSkillID ,  nMove ) ;
			}break;	
			default:
				ActionManager.Instance.CreateWaitingAction (ident);
			break;
		}
		return ;

		// old method .give up 
//		// find a pos 
//		Dictionary< Panel_unit , int > pool = Panel_StageUI.Instance.GetUnitDistPool (mob, true);
//
//		var items = from pair in pool orderby pair.Value ascending select pair;
//		//Dictionary< Panel_unit , int > items = from pair in pool orderby pair.Value ascending select pair;
//		foreach (KeyValuePair<Panel_unit , int> pair in items) {
//			Debug.LogFormat ("{0}: {1}", pair.Key, pair.Value);
//			int nDist = pair.Value;
//			cUnitData data = GameDataManager.Instance.GetUnitDateByIdent (pair.Key.Ident ()); 
//			
//
//			// path find when dist > 1
//			if (nDist > 1) {
//				// throw event
//				//	GameScene.Instance.Grids.ClearIgnorePool();
//				
//				//GameScene.Instance.Grids.AddIgnorePool(  GetPKPosPool(  true )  ); // need check camp
//				List< iVec2 > nearList = pair.Key.Loc.AdjacentList (); // the 4 pos can't stand ally
//				Dictionary< iVec2 , int > distpool = new Dictionary< iVec2 , int > ();
//				foreach (iVec2 v in nearList) {
//					if (Panel_StageUI.Instance.CheckIsEmptyPos (v) == true) {// 目標 周圍 不可以站人
//						distpool.Add (v, v.Dist (mob.Loc));
//					}
//				}
//				// start try each vaild pos
//				nDist = pair.Value;
//				iVec2 last = null;
//				
//				var itemsdist = from pair2 in distpool orderby pair2.Value ascending select pair2;
//				foreach (KeyValuePair<iVec2 , int> pair2 in itemsdist) {
//					nDist = pair.Value; // try other
//					last = null;
//					
//					List< iVec2> path = Panel_StageUI.Instance.PathFinding (mob, mob.Loc, pair2.Key, 999); // get a vaild path to run
//					
//					// limit out side
//					path = MyTool.CutList<iVec2> (path, nMove);
//					
//					// avoid stand on invalid pos
//					while (path.Count > 0) {
//						last = path [path.Count - 1];
//						if (Panel_StageUI.Instance.CheckIsEmptyPos (last) == false) {
//							path.RemoveAt (path.Count - 1); // then go again
//							
//						} else {
//							// success
//							mob.SetPath (path); 
//							// check if last pos is attack able pos
//							ActionManager.Instance.CreateMoveAction (ident, last.X, last.Y);	
//							
//							
//							if (Config.GOD == true) {
//								Panel_StageUI.Instance.CreatePathOverEffect (path); // draw path
//							}
//							
//							
//							break;
//						}
//					}
//					if (last != null) {
//						nDist = last.Dist (pair.Key.Loc);  // final dist
//						break;
//					} else {
//						nDist = -1; // can't find
//					}
//				}
//			}
//		
//			
//			// send attack
//			
//			if (nDist >= 0 && nDist <= 1) {
//				ActionManager.Instance.CreateAttackCMD (ident, pair.Key.Ident (), 0); // create Attack CMD . need battle manage to run
//				return;
//			} else {
//				//  can't attavk . waiting only 
//				ActionManager.Instance.CreateWaitingAction (ident);
//				return ;
//			}
//			// for next target
//		}
	}


	public static void RunCounter( Panel_unit mob , Panel_unit atker , int nAtkSkillID )
	{

	}


	//==================================

	static  void _AI_NormalAttack( Panel_unit mob , int nSkillID , int nMove )
	{
		if (_AI_LowstAttack (mob, nSkillID, nMove)) {
			return ;
		}

		// 找距離近的攻擊
		if (_AI_NearestAttack (mob, nSkillID, nMove))  {
			return ;
		}

		ActionManager.Instance.CreateWaitingAction (mob.Ident ());
		// all target faild.. waiting

	}
	// tool func hp lowest
	static  bool _AI_LowstAttack( Panel_unit mob , int nSkillID , int nMove , int nLimit = 0 )
	{
		int ident = mob.Ident ();
		int nSkillRange = MyTool.GetSkillRange ( nSkillID );
		Dictionary< Panel_unit , int > pool = Panel_StageUI.Instance.GetUnitHpPool(mob, true , (nMove+nSkillRange) );
		// Sort HP
		var items = from pair in pool orderby pair.Value ascending select pair;
		// 範圍內 低血量
		foreach (KeyValuePair<Panel_unit , int> pair in items) {
			// try path to target
			bool bCanAtk = false;
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
						if( last.Dist( pair.Key.Loc ) > nSkillRange ){
							continue; // 太遠了~放棄不打
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
		Dictionary< Panel_unit , int > pool = Panel_StageUI.Instance.GetUnitDistPool(mob, true);
		int ident = mob.Ident ();
		
		// get skill range
		int nSkillRange = MyTool.GetSkillRange ( nSkillID );
		
		// Sort dist
		var items = from pair in pool orderby pair.Value ascending select pair;
		// 
		foreach (KeyValuePair<Panel_unit , int> pair in items) {
			// try path to target
			bool bCanAtk = false;
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
						if( last.Dist( pair.Key.Loc ) > nSkillRange ){
							continue ; // too far

						}

						// send a move event					
						mob.SetPath (path); 
						// check if last pos is attack able pos
						ActionManager.Instance.CreateMoveAction (ident, last.X, last.Y);	
						
						if (Config.GOD == true) {
							Panel_StageUI.Instance.CreatePathOverEffect (path); // draw path
						}
						// check range can atk

						bCanAtk = true;

					}

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
			// for next target
		}
		// all target failed find again for nexreat target
		foreach (KeyValuePair<Panel_unit , int> pair in items) {
			// try path to target
			bool bCanAtk = false;
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
						if( last.Dist( pair.Key.Loc ) > nSkillRange ){
							// too far .. no attack
							
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

				ActionManager.Instance.CreateWaitingAction (mob.Ident ());
				return true;
			}
			// for next target
		}


		// all target faild.. waiting
		return false;
	}



	static  void _AI_PassiveAttack( Panel_unit mob , int nSkillID , int nMove )
	{
		Dictionary< Panel_unit , int > pool = Panel_StageUI.Instance.GetUnitDistPool(mob, true);
		int ident = mob.Ident ();
		
		// get skill range
		int nSkillRange = MyTool.GetSkillRange ( nSkillID );

		// Sort dist
		var items = from pair in pool orderby pair.Value ascending select pair;
		
		foreach (KeyValuePair<Panel_unit , int> pair in items) {
			// try path to target
			bool bCanAtk = false;
			int nDist = pair.Value; // value is dist
			if( nDist > nSkillRange  ) // pathfind if need
			{
				// 距離太遠，怎樣都不可能到的 先放棄
				if( (nSkillRange+nMove) < nDist )
					continue;

				List< iVec2> path = FindPathToTarget( mob , pair.Key , nMove , nSkillRange );
				if( path == null || (path.Count ==0) )
				{
					continue; // can't find path try next target
				}
				else
				{
					iVec2 last = path[ path.Count -1 ];
					if( last != null  ){
						if( last.Dist( pair.Key.Loc ) > nSkillRange ){
							continue; // 太遠了~放棄不打
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
			else{
				bCanAtk = true ; // atk directly
				
			}
			
			if( bCanAtk )
			{
				ActionManager.Instance.CreateAttackCMD (ident, pair.Key.Ident (), nSkillID ); // create Attack CMD . need battle manage to run
				
				return ;
			}
			// for next target
			
		}

		// all target faild.. waiting
		ActionManager.Instance.CreateWaitingAction (ident);
	}

	static void _AI_Defence(Panel_unit mob , int nSkillID , int nMove ) 
	{
		int ident = mob.Ident();
		ActionManager.Instance.CreateWaitingAction (ident); // always wait in pos
	}

	static void _AI_TargetAttack(Panel_unit mob , int nSkillID , int nMove ) 
	{
		int ident = mob.Ident();
		cUnitData data = GameDataManager.Instance.GetUnitDateByIdent ( ident ); 
		Panel_unit target =Panel_StageUI.Instance.GetUnitByCharID( data.n_AITarget );
		//cUnitData tar = GameDataManager.Instance.GetUnitDateByIdent ( data.n_AITarget ); 
		if( target == null) {
			// 目標死亡，切回正常模式
			_AI_NormalAttack( mob , nSkillID ,nMove  );
			return ;
		}
		//=========================
		bool bCanAtk = false;
		int nSkillRange = MyTool.GetSkillRange ( nSkillID );

		int nDist = mob.Loc.Dist( target.Loc )   ; // value is dist
		if( nDist > nSkillRange  ) // pathfind if need
		{
			List<iVec2> path = FindPathToTarget( mob , target , nMove , nSkillRange );
			if( path == null || (path.Count ==0) )
			{
				//continue; // can't find path try next target
			}
			else
			{
				iVec2 last = path[ path.Count -1 ];
				if( last != null  ){
					// send a move event					
					mob.SetPath (path); 
					// check if last pos is attack able pos
					ActionManager.Instance.CreateMoveAction (ident, last.X, last.Y);	
					
					if (Config.GOD == true) {
						Panel_StageUI.Instance.CreatePathOverEffect (path); // draw path
					}

					if( last.Dist( target.Loc ) > nSkillRange ){
						// too far
					}
					else {
						bCanAtk = true;
					}
				}			
			}

		}
		else{
			bCanAtk = true ; // atk directly
		}
		if( bCanAtk )
		{
			ActionManager.Instance.CreateAttackCMD (ident, target.Ident() , nSkillID ); // create Attack CMD . need battle manage to run
			return;
		}

		ActionManager.Instance.CreateWaitingAction (ident);
	}

	static void _AI_PositionAttack(Panel_unit mob , int nSkillID , int nMove ) 
	{
		int ident = mob.Ident();
		cUnitData data = GameDataManager.Instance.GetUnitDateByIdent ( ident ); 
		// move to pos
		iVec2 pos = new iVec2 ( data.n_AIX , data.n_AIY  );

		List< iVec2> path = Panel_StageUI.Instance.PathFinding ( mob, mob.Loc, pos , 999); // get a vaild path to run
		if( path == null || (path.Count ==0) )
		{
			//continue; // can't find path try next target
		}
		else
		{
			iVec2 last = path[ path.Count -1 ];
			if( last != null  ){
				// send a move event					
				mob.SetPath (path); 
				// check if last pos is attack able pos
				ActionManager.Instance.CreateMoveAction (ident, last.X, last.Y);	
				
				if (Config.GOD == true) {
					Panel_StageUI.Instance.CreatePathOverEffect (path); // draw path
				}
				return;
			}
		}
		// find target to atk



		ActionManager.Instance.CreateWaitingAction (ident);
	}


	public static List<iVec2> FindPathToTarget( Panel_unit Mob , Panel_unit Target  , int nMove , int nRange =1 )
	{
		if( Mob == null || Target == null ){
			return null;
		}
		int ident = Mob.Ident();

		List< iVec2 > nearList = Target.Loc.AdjacentList ( nRange ); // the 4 pos can't stand ally
		Dictionary< iVec2 , int > distpool = new Dictionary< iVec2 , int > ();
		foreach (iVec2 v in nearList) {
			if (Panel_StageUI.Instance.CheckIsEmptyPos (v) == true) {// 目標 pos 不可以站人
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
					return path; 
				}
			}
		}
		// all path failed. return null
		return null;

	}


	static public int SelSkill( cUnitData pMob , cUnitData pTarget = null , bool bCounterMode = false  )
	{
		//return 11702; // debug
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
		if( pTarget != null ){
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
		Dictionary< int , int > sklPool  = new Dictionary< int , int >();
		// normal attack

		//sklPool.Add( 0 , (pData.GetMaxCP() - pData.n_CP) +1 ); // 普攻永遠有機會 ,CP 越高，普攻機會越低

		foreach(  int  nID in pData.SkillPool ){
				int nSkillID = nID;
				nSkillID = pData.Buffs.GetUpgradeSkill( nSkillID ); // Get upgrade skill
				if( nSkillID ==0 )
					continue;

				SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nSkillID);				
				if( skl.n_SCHOOL == 0 )	// == 0 is ability
					continue;				
				if( skl.n_PASSIVE == 1 )
					continue;

				// check cp && MP
				if( (pData.n_CP < skl.n_CP) ){
					continue;
				}
				if( (pData.n_MP < skl.n_MP) ){
				//	continue;
				}


				// 防招只在破防時使用
				if( skl.f_DEF > 0.0f && pData.n_DEF>0)
					continue;

				//牽制招不對小怪物用
				cSkillData skldata = GameDataManager.Instance.GetSkillData( nSkillID );
				if( skldata.IsTag( _SKILLTAG._TIEUP ) )
				{
					if( pTarget == null )
						continue;
					if( (pMob.GetMar() - pTarget.GetMar()) < 20.0f )
						continue;
				}


				// check range when counter mode
				if( bCounterMode){
					//// 點地技能不能放	
					if( skl.n_RANGE < nDist )
						continue;
				}
				// 

				int widget = skl.n_CP; // cp is base widget

				// 	MODIFY widget 
				if( skl.n_AREA != 0 ){
					widget +=3; // aoe first
				}

				sklPool.Add( nSkillID , widget );
				//sklLst.Add(  skl );
		}
		// 
		if (nDist > 1) {
			int a =0;
		}

		if( (sklPool.Count > 0) )
		{
			// random a skill
			int id = MyTool.RollWidgetPool( sklPool );
			return id;
		}
		else{
			if( bCounterMode ){
				if( nDist > 1 )
					return -1;
			}
		}

		return 0; // normal attack
	}

}
