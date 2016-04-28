using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
//using _SRW;
using MYGRIDS;
// All SRW enum list
/// <summary>預設存在的 Channel Type</summary>
public enum _ACTION
{
	_WAIT = 0,
	_MOVE,
	_ATK,
	_DEF,

	_CAST,		// cast a skill  

	_HIT,			// atk a pos or AOE


	_HITED,
	_ASSIST_ATK,
	_ASSIST_DEF,

	_DROP,		// Get Grop
	_LVUP,		// level up action
	_WEAKUP,	// round end . weak up	
	_ITEM,		// Get Item

	// MOB use cmd
	_CMD_ATK,
	_CMD_CAST,


};

public class uAction
{
	public _ACTION eAct { set; get;}
	public int nActIdent { set; get;}
	public int nTarIdent { set; get;}

	public int nTarGridX { set; get;}
	public int nTarGridY { set; get;}

	public int nSkillID { set; get;}

	public int nActVar1 { set; get;}
	public int nActVar2 { set; get;}

	public List<cHitResult> HitResult;

	public uAction(){
		HitResult = new List<cHitResult>();
	}

	public void AddHitResult( List<cHitResult> pool )
	{
		if( HitResult == null )
			HitResult = new List<cHitResult>();

		if (pool == null)
			return;

		foreach (cHitResult hit in pool) {
			HitResult.Add( hit );
		}
	}

	public void AddHitResult( cHitResult hit )
	{
		if( HitResult == null )
			HitResult = new List<cHitResult>();
		
		if (hit != null) {
			HitResult.Add( hit );
		}
	}
};


public partial class ActionManager
{
	//public delegate AudioClip GetAudioClipDelegate (string audioPath);

	private bool hadInit;
	public bool HadInit{ get{ return hadInit; } }

//	uAction					CurAction;
//	uAction					LastAction;
//	int 					nActFlow	=0;

	List< uAction > 		ActionPool;				// record all action to do 
//	private Dictionary<int, AudioChannelBase> channels = new Dictionary<int, AudioChannelBase>();

	public void Initial( ){
		hadInit = true;
		//this.GetAudioClipFunc = getAudioClipFunc;
		ActionPool = new List< uAction >();
	//	UnitPool = new Dictionary< int , UNIT_DATA >();
	//	CampPool = new Dictionary< _CAMP , cCamp >();
	}

	private static ActionManager instance;
	public static ActionManager Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new ActionManager();

				instance.Clear();
			}
			
			return instance;
		}
	}

	public void Clear()
	{
		if( ActionPool != null )
			ActionPool.Clear ();
	}

	public bool Run()
	{
		if (ActionPool != null && ActionPool.Count > 0) {
			uAction act = ActionPool[0];

			if( act != null )
			{
				switch ( act.eAct  )
				{
					// AI_CMD
					case _ACTION._CMD_ATK: // mob atk
					{
					 // wait battle action complete
						if( BattleManager.Instance.IsBattlePhase() == false )
						{
							BattleManager.Instance.Clear();
							// setup act mode
							BattleManager.Instance.PlayAttack(act.nActIdent ,act.nTarIdent , act.nSkillID );

							ActionPool.RemoveAt( 0 ); // remove when setup success
						}
					}
					break;
					case _ACTION._CMD_CAST: // mob cast
					{
					if( BattleManager.Instance.IsBattlePhase() == false )
					{
						BattleManager.Instance.Clear();
						// setup act mode
						BattleManager.Instance.PlayCast(act.nActIdent , act.nTarIdent , act.nTarGridX ,act.nTarGridY , act.nSkillID );
						
						ActionPool.RemoveAt( 0 ); // remove when setup success
					}
					}
					break;
					// normal action
					default:
					{	
						// Normal atk action
						Panel_unit unit = Panel_StageUI.Instance.GetUnitByIdent( act.nActIdent );
						if( (unit!= null) )
						{
                           if (unit.SetAction(act))
                           {
                                ActionPool.RemoveAt(0); // remove when setup success
                                return true;
                           }
						}
						else {
							ActionPool.RemoveAt( 0 ); // remove when unit is not exist
							return true;
						}
					}
					break;

				}
			}
			else{
				ActionPool.RemoveAt( 0 );
				Debug.LogError( "Actmanager run null act!!" );
			}
		}
		return false;
	}

	public bool HaveAction()
	{
		if (ActionPool.Count > 0) {
			if( Panel_StageUI.Instance == null ){
				Debug.LogError( "action manager dead lock with no Panel_StageUI");
			}
		}

		return (ActionPool.Count > 0);
	}

	public void ReleaseAction()
	{
		foreach( uAction act in ActionPool )
		{
            Debug.LogFormat(" ActID-{0} , ident-{1},v1-{2}, v2-{3}", act.eAct.ToString(), act.nActIdent, act.nActVar1, act.nActVar2);

		}


		ActionPool.Clear ();
	}
//	public void Run()
//	{

//		return ;
//	}


	public uAction CreateAction( int nIdent , _ACTION act )
	{
		uAction p = new uAction ();
		p.eAct = act;
		p.nActIdent = nIdent;
		if( ActionPool == null ){
			ActionPool =  new List< uAction >();
		}
		ActionPool.Add (p); 
		return p;
	}
	//
	public uAction CreateAttackAction( int nAtkIdent , int nDefIdent , int nSkillID , int nVar1=0, int nVar2=0 )
	{
		uAction act = CreateAction (nAtkIdent, _ACTION._ATK);
		if( act != null )  {
			act.nTarIdent = nDefIdent;
			act.nSkillID = nSkillID;
			act.nActVar1 = nVar1;
			act.nActVar2 = nVar2;

			act.AddHitResult( new cHitResult( cHitResult._TYPE._CASTOUT , nAtkIdent ,nSkillID , nDefIdent , 0 , 0  ) );
			// normal attack gen 1 cp
			if( nSkillID ==0 ){
				act.AddHitResult( new cHitResult( cHitResult._TYPE._CP , nAtkIdent , 1 ) );
			}

		}
		return act;
	}
	public uAction CreateMoveAction( int nAtkIdent , int X , int Y , int nVar1=0, int nVar2=0 )
	{
		uAction act = CreateAction (nAtkIdent, _ACTION._MOVE);
		if( act != null )  {
			act.nTarGridX = X;
			act.nTarGridY = Y;
			act.nActVar1 = nVar1;
			act.nActVar2 = nVar2;
		}
		return act;
	}

	public uAction CreateWaitingAction( int nAtkIdent  )
	{
		uAction act = CreateAction (nAtkIdent, _ACTION._WAIT);
		if( act != null )  {

		}
		return  act;
	}

	public uAction CreateWeakUpAction( int nAtkIdent  )
	{
		uAction act = CreateAction (nAtkIdent, _ACTION._WEAKUP);
		if( act != null )  {

		}
		return  act;
	}

	public uAction CreateCastAction( int nAtkIdent , int nSkillID , int nTargetIdent , int nGridX=0, int nGridY=0  )
	{
		uAction act = CreateAction (nAtkIdent, _ACTION._CAST);
		if( act != null )  {
			act.nSkillID = nSkillID;
			act.HitResult.Add( new cHitResult( cHitResult._TYPE._CAST, nAtkIdent, nSkillID ) );

			cUnitData caster = GameDataManager.Instance.GetUnitDateByIdent( nAtkIdent );
			if( caster != null ){
                // 檢查 有無攻擊時取消的 buff
                if (caster.Buffs.BuffCheckCancel()) {
                    caster.SetUpdate(cAttrData._BUFF);
                }

                // 有 cost
                if ( nSkillID > 0 )
				{
					BattleManager.ConvertSkillTargetXY( caster , nSkillID , nTargetIdent , ref nGridX ,ref nGridY   );
					act.nTarGridX = nGridX;
					act.nTarGridY = nGridY;

					SKILL skill = ConstDataManager.Instance.GetRow<SKILL>(nSkillID); 
//					switch( skill.n_TARGET ){
//						case 0:	//0→對自己施展
//							act.nTarGridX = caster.n_X;
//							act.nTarGridY = caster.n_Y;
//						//	nTargetIdent = nAtkIdent; // will cause ( atker == defer )
//							break;
//						case 6:	//6→自我AOE我方
//						case 7:	//7→自我AOE敵方
//						case 8:	//8→自我AOEALL
//							act.nTarGridX = caster.n_X;
//							act.nTarGridY = caster.n_Y;
//							break;
//						case 1:	//→需要敵方目標
//						case 2:	//→需要友方目標
//						{
//							cUnitData Target = GameDataManager.Instance.GetUnitDateByIdent( nTargetIdent );
//							if( Target != null ){
//								act.nTarGridX = Target.n_X;
//								act.nTarGridY = Target.n_Y;
//							}else{
//								// bug
//								Debug.LogErrorFormat( "CreateCastAction on null target{0},skill{1},x{2},y{3} " ,nTargetIdent,nSkillID, nGridX , nGridY );	
//							}
//						}
//							break;
//						case 3:	//→MAP敵方
//						case 4: //→MAP我方
//						case 5:	//→MAPALL
//						{
//							act.nTarGridX = nGridX;
//							act.nTarGridY = nGridY;
//							//  mob counter 防呆
//							cUnitData Target = GameDataManager.Instance.GetUnitDateByIdent( nTargetIdent );
//							if( Target != null ){
//								act.nTarGridX = Target.n_X;
//								act.nTarGridY = Target.n_Y;
//							}
//						}
//						break;
//					}

					// avoid crash
					if( skill == null ){
						Debug.LogErrorFormat( "CreateCastAction with null skill cast{0},skill{1},x{2},y{3} " ,nAtkIdent,nSkillID, nGridX , nGridY );	
						return act;
					}


					// don't use fightattr . counter will  cast without fightattr
					if( skill.n_MP > 0 ){
						float frate =  caster.GetMulMpCost ();

						int cost = MyTool.GetEffectValue( skill.n_MP , frate , 0 );	
						caster.AddMp( -cost );

					}
					if( skill.n_SP > 0 ){
						caster.AddSp ( -skill.n_SP);
					}
					if( skill.n_CP > 0 ){
						caster.AddCp( -skill.n_CP );
					}
					cUnitData target = GameDataManager.Instance.GetUnitDateByIdent( nTargetIdent );
					caster.DoCastEffect( nSkillID  , target ,  ref act.HitResult  );

					// 直接回復防禦
					if( skill.f_DEF > 0.0f  ){
						float f = caster.GetMaxDef() * skill.f_DEF ;
						act.HitResult.Add( new cHitResult( cHitResult._TYPE._DEF ,nAtkIdent , (int)f    ) ); 
						//caster.AddDef( (int)f );
					}

                    // 攻擊技能，判斷 是否爆擊

                   
                }
			}
			else{ // bug
				Debug.LogErrorFormat( "CreateCastAction with null caster{0},skill{1},x{2},y{3} " ,nAtkIdent,nSkillID, nGridX , nGridY );	
			}
		}
		return  act;
	}

	public uAction CreateHitAction( int nAtkIdent , int X , int Y , int nSkillID )
	{
		uAction act = CreateAction (nAtkIdent, _ACTION._HIT);
		if( act != null )  {
			act.nSkillID = nSkillID;
			act.nTarGridX = X;
			act.nTarGridY = Y;		

			act.AddHitResult( new cHitResult( cHitResult._TYPE._CASTOUT , nAtkIdent ,nSkillID ,0, X , Y  ) );
		}
		return  act;
	}

	public uAction CreateDropAction( int nAtkIdent , int nExp , int nMoney  )
	{
		uAction act = CreateAction (nAtkIdent, _ACTION._DROP);
		if( act != null )  {
			act.nActVar1 = nExp;
			act.nActVar2 = nMoney;
		}
		return  act;
	}

	public uAction CreateLevleUpAction( int nAtkIdent , int nUpLv )
	{
		uAction act = CreateAction (nAtkIdent, _ACTION._LVUP);
		if( act != null )  {
			act.nActVar1 = nUpLv;
		}
		return  act;
	}

	public void ExecActionHitResult(  uAction action  )
	{
		if (action == null)
			return;

		ExecActionHitResult( action.HitResult  );
	}

	public void ExecActionHitResult(  List< cHitResult> resPool  , bool bSkipMode = false)
	{
		// some action play in hit task
		if (resPool != null) {
			foreach (cHitResult res in resPool) {
				if (res == null) 
					continue;
				Panel_unit pUnit = Panel_StageUI.Instance.GetUnitByIdent (res.Ident);
				if (pUnit) {
					switch( res.eHitType )
					{
						case cHitResult._TYPE._HITBACK: // HitBack
						{
							// don't
							if( bSkipMode ){
								pUnit.SetXY( res.Value1 , res.Value2 );
							}
							else{
								pUnit.HitBackTo( res.Value1 , res.Value2 );
							}
						
						}break;
						case cHitResult._TYPE._CASTOUT: // cast out
						{
							if( bSkipMode ){
								continue;
							}
							// Add FX effect 
							pUnit.ShowSkillCastOutFX( res.Value1 , res.Value2, res.Value3 , res.Value4   );
						//	BattleManager.Instance.ShowBattleFX( res.Ident , "CFXM4 Hit B (Orange, CFX Blend)"  );
						
						}break;		
						case cHitResult._TYPE._BEHIT: // be Hit fX
						{
							if( bSkipMode ){
								continue;
							}

//							int nhitFX = 203;// default  
	//						SKILL skl = ConstDataManager.Instance.GetRow<SKILL> (res.Value1);
		//					if( skl != null ){								
			//					nhitFX=skl.n_HIT_FX;  // skill data may cancel hit fx to 0
				//			}
                                //if( nhitFX == 0)
                                //	nhitFX = 203;
                             pUnit.PlayHitResult(res);
//                                pUnit.PlayFX(nhitFX );
//                            BattleManager.Instance.ShowBattleFX( res.Ident , nhitFX );
							// it should have fx
						
						}break;
                        case cHitResult._TYPE._SHIELD: // be Hit fX
                            {
                                if (bSkipMode)
                                {
                                    continue;
                                }

                                //							int nhitFX = 203;// default  
                                //						SKILL skl = ConstDataManager.Instance.GetRow<SKILL> (res.Value1);
                                //					if( skl != null ){								
                                //					nhitFX=skl.n_HIT_FX;  // skill data may cancel hit fx to 0
                                //			}
                                //if( nhitFX == 0)
                                //	nhitFX = 203;
                                pUnit.PlayHitResult(res);
                                //                                pUnit.PlayFX(nhitFX );
                                //                            BattleManager.Instance.ShowBattleFX( res.Ident , nhitFX );
                                // it should have fx

                            }
                            break;

                        case cHitResult._TYPE._CIRIT: // show dodge
						{
							if( bSkipMode ){
								continue;
							}	
							// it should have fx
							pUnit.SetCirit(); // be cirit 
							
						}break;	
						case cHitResult._TYPE._DODGE: // show dodge
						{
							if( bSkipMode ){continue;}
						// it should have fx
							pUnit.SetDodge();
						
						}break;	

						case cHitResult._TYPE._MISS: // show miss
						{
							if( bSkipMode ){continue;}
						// it should have fx
							pUnit.SetMiss();
						
						}break;	

						case cHitResult._TYPE._GUARD:
						{
							if( bSkipMode ){continue;}

							pUnit.SetGuardTo( res.Value1 );
						}
                            break;

                        case cHitResult._TYPE._IMMUNE: // show dodge
                            {
                                if (bSkipMode) { continue; }
                                // it should have fx
                                pUnit.SetImmune();

                            }
                        break;
					}
				}

			}
		}
	}


	public void ExecActionEndResult(  uAction action )
	{
		if (action == null)
			return;
		ExecActionEndResult( action.HitResult );
	}

	public void ExecActionEndResult(  List< cHitResult> resPool  , bool bSkipMode = false )
	{
		if( resPool != null ){
			foreach (cHitResult res in resPool) {
				if (res != null) {
					// show effect
					Panel_unit pUnit = Panel_StageUI.Instance.GetUnitByIdent (res.Ident);
					if (pUnit) {
                        switch (res.eHitType)
                        {
                            case cHitResult._TYPE._HP:
                            case cHitResult._TYPE._DEF:
                            case cHitResult._TYPE._MP:
                            case cHitResult._TYPE._SP:
                            case cHitResult._TYPE._CP:
                            case cHitResult._TYPE._ACTTIME:                            
                                pUnit.PlayHitResult(res);
                                break;

                            case cHitResult._TYPE._ADDBUFF:
                            case cHitResult._TYPE._DELBUFF:
                            case cHitResult._TYPE._DELSTACK:
                                pUnit.PlayHitResult(res , true );
                                break;
                        }
                        


                        //switch( res.eHitType )
                        //{
                        //case cHitResult._TYPE._HP:{

                        //	pUnit.ShowValueEffect( res.Value1 , 0 ); // HP
                        //	if( res.Value1 != 0 ) // maybe change data in  battle manage
                        //	{
                        //		pUnit.pUnitData.AddHp (res.Value1   );
                        //	}

                        //}break;
                        //case cHitResult._TYPE._DEF:{

                        //	pUnit.ShowValueEffect( res.Value1 , 1 ); // DEF
                        //	if( res.Value1 != 0 ) // maybe change data in  battle manage
                        //	{
                        //		pUnit.pUnitData.AddDef (res.Value1   );
                        //	}

                        //}break;
                        //case cHitResult._TYPE._MP:{
                        //	pUnit.ShowValueEffect( res.Value1 , 2 ); // MP
                        //	if( res.Value1 != 0 ) // maybe change data in  battle manage
                        //	{
                        //		pUnit.pUnitData.AddMp (res.Value1   );
                        //	}							
                        //}break;
                        //case cHitResult._TYPE._SP:{
                        //	pUnit.ShowValueEffect( res.Value1 , 3 ); // SP
                        //	if( res.Value1 != 0 ) // maybe change data in  battle manage
                        //	{
                        //		pUnit.pUnitData.AddSp (res.Value1   );
                        //	}							
                        //}break;
                        //case cHitResult._TYPE._CP:{
                        //	//pUnit.ShowValueEffect( res.Value1 , 0 ); // CP
                        //	if( res.Value1 != 0 ) // maybe change data in  battle manage
                        //	{
                        //		pUnit.pUnitData.AddCp (res.Value1   );
                        //	}							
                        //}break;
                        //case cHitResult._TYPE._ACTTIME:{
                        //	//pUnit.ShowValueEffect( res.Value1 , 0 ); // SP
                        //	if( res.Value1 != 0 ) // maybe change data in  battle manage
                        //	{
                        //		pUnit.pUnitData.AddActionTime (res.Value1   );
                        //	}							
                        //}break;

                        //case cHitResult._TYPE._ADDBUFF: // add buff
                        //{
                        //	if( res.Value1 != 0 ) // maybe change data in  battle manage
                        //	{
                        //		//cUnitData pData = GameDataManager.Instance.GetUnitDateByIdent (res.Ident);
                        //		//if (pData != null) {
                        //			pData.pUnit.Buffs.AddBuff( res.Value1 , res.Value2, res.Value3 ,res.Value4);
                        //		//}
                        //	}

                        //}break;
                        //case cHitResult._TYPE._DELBUFF: // remove buff
                        //{
                        //	if( res.Value1 != 0 ) // maybe change data in  battle manage
                        //	{
                        //		cUnitData pData = GameDataManager.Instance.GetUnitDateByIdent (res.Ident);
                        //		if (pData != null) {
                        //			pData.Buffs.DelBuff( res.Value1 );
                        //		}
                        //	}

                        //}break;
                        //}
                    }
                }
			}
		}
	}


	// ==== Mob AI use
	// CMD
	public uAction CreateAttackCMD( int nAtkIdent , int nDefIdent , int nSkillID , int nVar1=0, int nVar2=0 )
	{
		uAction act = CreateAction (nAtkIdent, _ACTION._CMD_ATK);
		if( act != null )  {
			act.nTarIdent = nDefIdent;
            if (nSkillID < 0)            {
                nSkillID = 0;
            }
            act.nSkillID = nSkillID;
			act.nActVar1 = nVar1;
			act.nActVar2 = nVar2;
		}
		return act;
	}
	public uAction CreateCastCMD( int nAtkIdent , int nX ,  int nY , int nSkillID , int nVar1=0, int nVar2=0 )
	{
		uAction act = CreateAction (nAtkIdent, _ACTION._CMD_CAST);
		if( act != null )  {
			act.nTarGridX = nX;
			act.nTarGridY = nY;
            //act.nTarIdent = nDefIdent;
            if (nSkillID < 0)            {
                nSkillID = 0;
            }
            act.nSkillID = nSkillID;
			act.nActVar1 = nVar1;
			act.nActVar2 = nVar2;
		}
		return act;
	}
};

