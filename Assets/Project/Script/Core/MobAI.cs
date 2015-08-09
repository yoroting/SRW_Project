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
		// find a pos 
		Dictionary< Panel_unit , int > pool = Panel_StageUI.Instance.GetUnitDistPool (mob, true);

		var items = from pair in pool orderby pair.Value ascending select pair;
		//Dictionary< Panel_unit , int > items = from pair in pool orderby pair.Value ascending select pair;
		foreach (KeyValuePair<Panel_unit , int> pair in items) {
			Debug.LogFormat ("{0}: {1}", pair.Key, pair.Value);
			int nDist = pair.Value;
			cUnitData data = GameDataManager.Instance.GetUnitDateByIdent (pair.Key.Ident ()); 
			
			int nMove = data.GetMov (); // mob movement;
			// path find when dist > 1
			if (nDist > 1) {
				// throw event
				//	GameScene.Instance.Grids.ClearIgnorePool();
				
				//GameScene.Instance.Grids.AddIgnorePool(  GetPKPosPool(  true )  ); // need check camp
				List< iVec2 > nearList = pair.Key.Loc.AdjacentList (); // the 4 pos can't stand ally
				Dictionary< iVec2 , int > distpool = new Dictionary< iVec2 , int > ();
				foreach (iVec2 v in nearList) {
					if (Panel_StageUI.Instance.CheckIsEmptyPos (v) == true) {// 目標 周圍 不可以站人
						distpool.Add (v, v.Dist (mob.Loc));
					}
				}
				// start try each vaild pos
				nDist = pair.Value;
				iVec2 last = null;
				
				var itemsdist = from pair2 in distpool orderby pair2.Value ascending select pair2;
				foreach (KeyValuePair<iVec2 , int> pair2 in itemsdist) {
					nDist = pair.Value; // try other
					last = null;
					
					List< iVec2> path = Panel_StageUI.Instance.PathFinding (mob, mob.Loc, pair2.Key, 999); // get a vaild path to run
					
					// limit out side
					path = MyTool.CutList<iVec2> (path, nMove);
					
					// avoid stand on invalid pos
					while (path.Count > 0) {
						last = path [path.Count - 1];
						if (Panel_StageUI.Instance.CheckIsEmptyPos (last) == false) {
							path.RemoveAt (path.Count - 1); // then go again
							
						} else {
							// success
							mob.SetPath (path); 
							// check if last pos is attack able pos
							ActionManager.Instance.CreateMoveAction (ident, last.X, last.Y);	
							
							
							if (Config.GOD == true) {
								Panel_StageUI.Instance.CreatePathOverEffect (path); // draw path
							}
							
							
							break;
						}
					}
					if (last != null) {
						nDist = last.Dist (pair.Key.Loc);  // final dist
						break;
					} else {
						nDist = -1; // can't find
					}
				}
			}
		
			
			// send attack
			
			if (nDist >= 0 && nDist <= 1) {
				ActionManager.Instance.CreateAttackCMD (ident, pair.Key.Ident (), 0); // create Attack CMD . need battle manage to run
				return;
			} else {
				//  can't attavk . waiting only 
				ActionManager.Instance.CreateWaitingAction (ident);
				return ;
			}
			// for next target
		}
	}


	


	static int SelSkill( Panel_unit Unit )
	{

		return 0;
	}

}
