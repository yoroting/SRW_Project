//using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Enum = System.Enum;
//using MyClassLibrary;
////using _SRW;
//using MYGRIDS;


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
using MYGRIDS;
using MyClassLibrary;

// All SRW enum list
/// <summary>預設存在的 Channel Type</summary>

// this is enum all battle behavior need to Performance
public enum _BATTLE
{
	_NONE	 = 0 ,			// no battle
	_ATTACK  	 ,			// attack / skill
	_CAST  	 	 ,			// cast a skill on pos
	_SCRIPT  	 ,			// play a  battle script
};


public class cHitResult		// 
{
	public enum _TYPE
	{
		_HP=0		,		// 增減 HP
		_MP	 		,		// 增減 MP
		_DEF		,		// 增減 DEF
		_SP			,		// 增減 SP
		_CP			,		// 增減 CP
        _TIRED      ,       // 增減 破綻
        _ACTTIME	,		// actime

		_ADDBUFF	,
		_DELBUFF	,
        _DELSTACK,          // del stack

        _CAST		,		// cast skill
		_CASTOUT	,		// cast out skill
		//_HIT		,		// skill hit enemy
		_BEHIT		,		// 被

		_HITBACK	,
        _CIRIT,             // 取消爆擊的設計。以維持平衡    
       // _BECIRIT		,
		_DODGE		,		// 迴避
		_MISS		, 		// fail
		_GUARD		,		// guard some 
        _IMMUNE     ,      //  immune
        //_BLOCK		
        _SHIELD     ,       // 護盾
       

    };


	public cHitResult( _TYPE type , int ident ,int value1 =0,int value2=0,int value3=0,int value4=0 , int value5 = 0)
	{
		eHitType = type;
		Ident = ident; 
	//	SkillID = skillid;
		Value1 = value1;
		Value2 = value2;
		Value3 = value3;
		Value4 = value4;
        Value5 = value5;
    }
//	public cHitResult( int nAtkIdent , int nDefIdent )
//	{
//		AtkIdent = nAtkIdent;
//		DefIdent = nDefIdent;
//	}

	public _TYPE eHitType{ set; get;}

	public int Ident{ set; get;}
//	public int SkillID{ set; get;}
	public int Value1{ set; get;}
	public int Value2{ set; get;}
	public int Value3{ set; get;}
	public int Value4{ set; get;}
    public int Value5 { set; get; }

};

public partial class BattleManager
{
	//public delegate AudioClip GetAudioClipDelegate (string audioPath);

	private bool hadInit;
	public bool HadInit{ get{ return hadInit; } }

    public int nEndPhase = -1; // 不可能到的數
    //	private Dictionary<int, AudioChannelBase> channels = new Dictionary<int, AudioChannelBase>();

    public void Initial(  ){
		hadInit = true;

		Clear ();
		// drop exp	

		nDropMoney = 0;
		nDropExpPool = new Dictionary< int , int >();
		nDropItemPool = new List<int>();
        //	bIsBattle = false;
        //this.GetAudioClipFunc = getAudioClipFunc;
        //	UnitPool = new Dictionary< int , UNIT_DATA >();
        //	CampPool = new Dictionary< _CAMP , cCamp >();
        nLastAtkerCharID = 0;
        nLastDeferCharID = 0;
        nLastAtkerSkillID = 0;
        nLastDeferSkillID = 0;

    }

	private static BattleManager instance;
	public static BattleManager Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new BattleManager();
				instance.Clear();
			}
			
			return instance;
		}
	}

    public void Clear()
	{
       

        // 
        eBattleType = _BATTLE._NONE;
		eAtkCmdID = _CMD_ID._NONE;
		eDefCmdID = _CMD_ID._NONE;

		bPause = false;
      

        nAtkerID = 0;
		nDeferID = 0;


		nAtkerSkillID = 0;
		nDeferSkillID = 0;		
	
		nTarGridX = 0;
		nTarGridY = 0;

	
		nBattleID = 0;
		
		//===================================================
		nPhase = 0; // 
		if (AtkAffectPool == null) {
			AtkAffectPool = new List< cUnitData >();
		} else {
			AtkAffectPool.Clear ();
		}

		if (DefAffectPool == null) {
			DefAffectPool = new List< cUnitData >();
		} else {
			DefAffectPool.Clear ();
		}

		bCanCounter = false;
	//	AtkCCPool = null;
	//	DefCCPool = null;

	}
    public void RecordLastCombatCharID()
    {
        // record last atker
        cUnitData Atker = GameDataManager.Instance.GetUnitDateByIdent(nAtkerID);
        cUnitData Defer = GameDataManager.Instance.GetUnitDateByIdent(nDeferID);
        if (Atker != null)
        {
            nLastAtkerCharID = Atker.n_CharID;
        }
        if (Defer != null)
        {
            nLastDeferCharID = Defer.n_CharID;
        }

        nLastAtkerSkillID = nAtkerSkillID;
        nLastDeferSkillID = nDeferSkillID;
    }

	public bool IsBattlePhase()
	{
		if (eBattleType != _BATTLE._NONE)
			return true;

		return false;
	}


    public bool IsDamagePhase()
    {
        //       if (MyTool.IsDamageSkill(nAtkerSkillID)) // 裡面有對 0 檢查
        //           return true;

        if (eBattleType == _BATTLE._ATTACK)
        {
            return true;
        }
        else if (eBattleType == _BATTLE._CAST)
        {
            if (MyTool.IsDamageSkill(nAtkerSkillID))
                return true;
        }

        // 有實際造成傷害的戰鬥
        return false;
    }

    // only run in battle  
    public void Run()
	{
		if (eBattleType == _BATTLE._NONE)
			return;

		if (bPause)
			return;

		switch (eBattleType) {
		case _BATTLE._ATTACK:
			RunAttack();
			break;
		case _BATTLE._CAST:
			RunCast();
			break;

		// may be never use here
		case _BATTLE._SCRIPT:
			RunScript();
			break;

		default:
			break;
		}




		return ;
	}

	//必定造成傷害與 反擊/攻守/協防的攻擊流程
	public void RunAttack()
	{
		cUnitData Atker = GameDataManager.Instance.GetUnitDateByIdent ( nAtkerID );
		cUnitData Defer = GameDataManager.Instance.GetUnitDateByIdent ( nDeferID );
		// 因為事件的觸發～ 可能讓 atker / defer 消失。需有配套.. need avoid this case happen .no unit dead during battle trig event
//		if (Atker == null || Defer == null) {
//			nPhase = 10; // 戰鬥被中斷，直接結束
//			Debug.Log( " null unit when RunAttack" );
//		}
		//if( Atker == null ){
		//	Debug.LogErrorFormat( "RunAttack with null Atker at {0}", nAtkerID );
		//	Clear ();
		//	return ;
		//}
		//else if( Defer == null ){
		//	Debug.LogErrorFormat( "RunAttack with null Defer at {0}", nDeferID );
		//	Clear ();
		//	return;
		//}


        // 任一方被打死 戰鬥結束
        if ((Atker == null) || (Defer == null)  || Defer.IsStates(_FIGHTSTATE._DEAD) || Atker.IsStates(_FIGHTSTATE._DEAD))
        {           
            nPhase = nEndPhase;   //戰鬥結束           
        }
        //Panel_unit uDefer = Panel_StageUI.Instance.GetUnitByIdent( nDeferID ); 

        switch (nPhase) {
		case 0:	// prepare for event check
			Panel_StageUI.Instance.ClearAVGObj();
			// open CMD UI for def player
			if ( Defer.eCampID  == _CAMP._PLAYER) {
				// set open CMD 
				// need set a range
				Panel_CMDUnitUI.OpenCMDUI( _CMD_TYPE._COUNTER , nDeferID  , nAtkerID );
				
			}
			else{ 
				// mob need a method to get skill
				eDefCmdID = _CMD_ID._COUNTER; // mob select counter this time
				int nSklId = MobAI.SelCountSkill( Defer , Atker  );
				if( nSklId < 0 ){
					eDefCmdID = _CMD_ID._DEF;  //select defence
				}
				else{
					nDeferSkillID = nSklId;		// counter skill
				}
			}
			
			nPhase++;
			break;
		case 1:
                {   // Casting 
                    // have set a cmd and ui is closed

                    if (PanelManager.Instance.CheckUIIsOpening(Panel_CMDUnitUI.Name) == false)
                    {
                        // special process for def is player+
                        if (Defer.eCampID == _CAMP._PLAYER)
                        {

                            // check to reopen counter CMD UI	
                            if (!IsCounterMode() && !IsDefMode())
                            {
                                if (PanelManager.Instance.CheckUIIsOpening(Panel_Skill.Name) == false)
                                { // don't pop if skill ui is opening
                                    Panel_CMDUnitUI.OpenCMDUI(_CMD_TYPE._COUNTER, nDeferID, nAtkerID);
                                }
                                return; // break when counter don't start
                            }
                        }
                        //ShowBattleMsg( nAtkerID , "attack" );
                        // change defer skill if deence								 

                        // init fight attr each time
                        Atker.SetFightAttr(nDeferID, nAtkerSkillID);
                        Defer.SetFightAttr(nAtkerID, nDeferSkillID);

                        // set batle state
                        Atker.AddStates(_FIGHTSTATE._ATKER);

                        //Defer.AddStates( _FIGHTSTATE._DEFER );

                        if (IsDefMode())
                        {
                            Defer.AddStates(_FIGHTSTATE._DEFMODE);
                            nDeferSkillID = Config.sysDefSkillID;
                        }

                        // atk start cast action
                        uAction pCastingAction = ActionManager.Instance.CreateCastAction(nAtkerID, nAtkerSkillID, nDeferID, nTarGridX, nTarGridY);
                        // skill attr
                        if (pCastingAction != null)
                        {
                        }

                        nPhase++;
                    }
                }
                break;
		case 2:
                {
                    if (Defer.IsStun()) {
                        Defer.RemoveStates(_FIGHTSTATE._DEFMODE);
                        nPhase++;
                        break;
                    }
                    
                    // def casting
                    uAction pCastingAction = ActionManager.Instance.CreateCastAction(nDeferID, nDeferSkillID, nAtkerID);
                    if (pCastingAction != null)
                    {
                        //				Defer.DoSkillCastEffect( ref pCastingAction.HitResult  );
                        //				if(  Defer.FightAttr.Skill != null )
                        //					MyTool.DoSkillEffect( Defer , Defer.FightAttr.HitPool , Defer.FightAttr.Skill.s_CAST_TRIG ,  Defer.FightAttr.HitEffPool , ref pCastingAction.HitResult  );

                        if (Defer != null)
                        {
                            if (Defer.IsStates(_FIGHTSTATE._COPY))
                            {
                                // if target have skill copy target 's skill to cast
                                if (nAtkerSkillID > 0)
                                {
                                    nDeferSkillID = nAtkerSkillID;
                                    Defer.SetFightAttr(nAtkerID, nDeferSkillID);
                                    // cast sceond skill
                                    ActionManager.Instance.CreateCastAction(nDeferID, nDeferSkillID, nAtkerID);
                                }
                            }
                        }


                    }

                    //ShowBattleMsg( nDeferID , "counter" );
                    // show assist
                    nPhase++;
                }
			break;
		case 3:			// atack assist pre show 
			ShowAtkAssist( nAtkerID,nDeferID );
            ShowDefAssist(nDeferID);
                nPhase++;
			break;
		case 4:			// def assist pre show 
			
			nPhase++;
			break;
		case 5:
                {   // atk -> def 
                    Panel_StageUI.Instance.FadeOutAVGObj();
                    //
                    uAction pAtkAction = ActionManager.Instance.CreateAttackAction(nAtkerID, nDeferID, nAtkerSkillID);
                    if (pAtkAction != null)
                    {
                        GetAtkHitResult(nAtkerID, nDeferID, Atker, Defer, nAtkerSkillID, nTarGridX, nTarGridY, ref pAtkAction, ref AtkAffectPool);

                        //
                        //				pAtkAction.AddHitResult(  CalAttackResult( nAtkerID , nDeferID , IsDefMode() ) ) ;
                        //				pAtkAction.AddHitResult( CalSkillHitResult(  Atker , Defer  , nAtkerSkillID ) );
                        //
                        //				// get affectpool
                        //				if (Atker.IsStates( _FIGHTSTATE._THROUGH ) ) {
                        //					GetThroughPool( Atker , Defer ,Atker.FightAttr.SkillID , 0 , 0 , ref AtkAffectPool );
                        //				}
                        //
                        //				GetAffectPool( Atker , Defer , Atker.FightAttr.SkillID , 0 , 0 , ref AtkAffectPool );
                        //				foreach( cUnitData unit in AtkAffectPool )
                        //				{
                        //					//=====================
                        //					// checked if this cUnitData can Atk
                        //					// already check in get affect pool
                        //					if( Atker != unit ){
                        //						unit.SetFightAttr( Atker.n_Ident , 0 );
                        //					}
                        //					ShowDefAssist( unit.n_Ident , false );
                        //
                        //					pAtkAction.AddHitResult(  CalAttackResult( nAtkerID , unit.n_Ident , true) ) ;
                        //
                        //					pAtkAction.AddHitResult( CalSkillHitResult(  Atker , unit  , nAtkerSkillID ) );
                        //
                        //				}
                        //				//=========================
                        //				//	Debug.LogFormat( "atk charid{0}, skill{1} , aff{2}", Atker.n_CharID  , nAtkerSkillID ,  AtkAffectPool.Count );
                    }

                    // 打兩下
                    if (Atker.IsDead() == false && Atker.IsStates(_FIGHTSTATE._TWICE))
                    {

                        uAction pAtkTwice = ActionManager.Instance.CreateAttackAction(nAtkerID, nDeferID, nAtkerSkillID); // always normal atk
                        if (pAtkTwice != null)
                        {
                            //Atker.FightAttr.fBurstRate -= 0.5f;		// 第二下 傷害降低
                            GetAtkHitResult(nAtkerID, nDeferID, Atker, Defer, nAtkerSkillID, nTarGridX, nTarGridY, ref pAtkTwice, ref AtkAffectPool);
                        }
                    }


                    // check if can counter at this time
                    if (IsDefMode() == false)
                    {
                        // check range
                        int nRange = 1; // default range
                        SKILL defskill = ConstDataManager.Instance.GetRow<SKILL>(nDeferSkillID);
                        if (defskill != null)
                        {
                            nRange = defskill.n_RANGE;
                        }
                        if (Defer.IsStates(_FIGHTSTATE._DEAD) == false)
                        {
                            bool bIsDamage = MyTool.IsDamageSkill(nDeferSkillID); // check damage skill 
                            if (bIsDamage == true)
                            {
                                if (iVec2.Dist(Atker.n_X, Atker.n_Y, Defer.n_X, Defer.n_Y) <= nRange)
                                {
                                    bCanCounter = true;
                                }
                            }
                        }
                    }



                    // 被打死或被暈不能反擊
                    if (Defer.IsStates(_FIGHTSTATE._DEAD)|| Defer.IsStun() )
                    {
                        bCanCounter = false;
                    }

                    nPhase++;
                }
			break;
		case 6:
			Panel_StageUI.Instance.ClearAVGObj();
                
                //被暈不能反擊
                if ( Defer.IsStun())
                {
                    bCanCounter = false;                    
                }

                if (bCanCounter){
				    ShowAtkAssist( nDeferID , nAtkerID );
                    ShowDefAssist(nAtkerID);
                }
			nPhase++;
			break;
		case 7:
                // 被打死或被暈不能反擊
              
                if (bCanCounter ){
			//	ShowDefAssist( nAtkerID );
			}
			nPhase++;
			break;

		case 8:			//  def -> atk
			Panel_StageUI.Instance.FadeOutAVGObj();
			if ( bCanCounter ) {
				uAction pCountAct = ActionManager.Instance.CreateAttackAction (nDeferID,nAtkerID, nDeferSkillID );
				GetAtkHitResult( nDeferID , nAtkerID ,  Defer , Atker , nDeferSkillID ,  nTarGridX , nTarGridY , ref pCountAct , ref DefAffectPool );

                    //				bool bIsDamage =  MyTool.IsDamageSkill( nDeferSkillID );
                    //				if (pCountAct != null) {
                    //					if( bIsDamage ){
                    //						pCountAct.AddHitResult( CalAttackResult( nDeferID , nAtkerID , false ) ); // must not def at this time
                    //					}
                    //
                    //					pCountAct.AddHitResult( CalSkillHitResult( Defer,  Atker , nDeferSkillID ) );
                    //				}
                    //				if (Defer.IsStates( _FIGHTSTATE._THROUGH ) ) {
                    //					GetThroughPool( Defer , Atker ,Defer.FightAttr.SkillID , 0 , 0 , ref DefAffectPool );
                    //				}
                    //				GetAffectPool( Defer , Atker , Defer.FightAttr.SkillID , 0 , 0 , ref DefAffectPool );
                    //				foreach( cUnitData unit in DefAffectPool )
                    //				{
                    //					//=====================
                    //					// checked if this cUnitData can Atk
                    //					if( unit !=Defer){
                    //						unit.SetFightAttr( Defer.n_Ident  , 0 );
                    //					}
                    //
                    //					ShowDefAssist( unit.n_Ident , false );
                    //
                    //					if(bIsDamage){
                    //						pCountAct.AddHitResult(  CalAttackResult( nDeferID , unit.n_Ident , true ) ) ; // always def for aoe affect
                    //					}
                    //					pCountAct.AddHitResult( CalSkillHitResult( Defer,  unit , nDeferSkillID ) );
                    //				}

                    //====
                    //Debug.LogFormat( "def charid{0}, skill{1} , aff{2}",Defer.n_CharID  , nDeferSkillID ,  DefAffectPool.Count );

                    // 打兩下
                    if (Defer.IsDead() == false && (bCanCounter == true) && Defer.IsStates(_FIGHTSTATE._TWICE))
                    {
                        uAction pDefTwice = ActionManager.Instance.CreateAttackAction(nDeferID, nAtkerID, nDeferSkillID);// always normal atk
                        if (pDefTwice != null)
                        {
                            //	Defer.FightAttr.fBurstRate -= 0.5f;		// 第二下 傷害降低
                            GetAtkHitResult(nDeferID, nAtkerID, Defer, Atker, nDeferSkillID, nTarGridX, nTarGridY, ref pDefTwice, ref DefAffectPool);
                        }
                    }

                }
			nPhase++;
			break;
		case 9: 	{// atker twice atk
			//if( Atker.Dist( Defer  )<=2 ){ // need range 2
				//if( Atker.IsDead() == false && Atker.IsStates( _FIGHTSTATE._TWICE)  ){

				//	uAction pAtkTwice = ActionManager.Instance.CreateAttackAction(nAtkerID,nDeferID, nAtkerSkillID ); // always normal atk
				//	if( pAtkTwice != null ){		
				//		//Atker.FightAttr.fBurstRate -= 0.5f;		// 第二下 傷害降低
				//		GetAtkHitResult( nAtkerID , nDeferID , Atker , Defer ,  nAtkerSkillID , nTarGridX , nTarGridY , ref pAtkTwice , ref AtkAffectPool );
				//	}
				//}
				// check defer can twice
				//if( Defer.IsDead()==false && (bCanCounter==true) && Defer.IsStates( _FIGHTSTATE._TWICE)  ){
				//	uAction pDefTwice = ActionManager.Instance.CreateAttackAction(nDeferID ,nAtkerID, nDeferSkillID  );// always normal atk
				//	if( pDefTwice != null ){
				//	//	Defer.FightAttr.fBurstRate -= 0.5f;		// 第二下 傷害降低
				//		GetAtkHitResult( nDeferID , nAtkerID ,  Defer , Atker , nDeferSkillID  , nTarGridX , nTarGridY , ref pDefTwice , ref DefAffectPool );
				//	}
				//}
			//}
			nPhase++;
		}break;
		case 10:
			nPhase++;			// wait all action complete
			break;
        case 11:
                nPhase = nEndPhase;
                break;
        default:            // close all 

                // add Exp / Money  action

                // Fight Finish	
            if (AtkAffectPool != null) {
                    foreach (cUnitData unit in AtkAffectPool)
                    {
                        if ((unit != Atker) && (unit != Defer))
                            unit.FightEnd();
                    }
            }

                if (DefAffectPool != null)
                {
                    foreach (cUnitData unit in DefAffectPool)
                    {
                        if ((unit != Atker) && (unit != Defer))
                            unit.FightEnd();
                    }
                }

			if( (Defer!=null) && (Defer!=Atker) ){
				Defer.FightEnd();
			}

			// atker clear at last
			if( Atker != null ){
				Atker.FightEnd( true );
			}


                // cmd finish

                // action finish in atk action
                //		StageUnitActionFinishEvent cmd = new StageUnitActionFinishEvent ();
                //		cmd.nIdent = nAtkerID;
                //		GameEventManager.DispatchEvent ( cmd );
            RecordLastCombatCharID();
            // Do Counter
            Clear ();

			break;
		}
	}
	//地圖攻擊/ 指向Buff /自我施法
	public void RunCast()
	{
		cUnitData Atker = GameDataManager.Instance.GetUnitDateByIdent ( nAtkerID );
		cUnitData Defer = GameDataManager.Instance.GetUnitDateByIdent ( nDeferID );
		//Panel_unit uDefer = Panel_StageUI.Instance.GetUnitByIdent( nDeferID ); 
		if( Atker == null ){
			Debug.LogErrorFormat( "RunCast with null Atker at {0}", nAtkerID );
			Clear ();
			return;
		}


		switch (nPhase) {
		case 0:	// prepare for event check
			Panel_StageUI.Instance.ClearAVGObj();

			Atker.SetFightAttr (nDeferID, nAtkerSkillID);
			Atker.AddStates (_FIGHTSTATE._ATKER);


			uAction pCastingAction = ActionManager.Instance.CreateCastAction (nAtkerID, nAtkerSkillID,nDeferID , nTarGridX , nTarGridY );// Casting
			// skill attr
			if (pCastingAction != null) {
	//			Atker.DoSkillCastEffect( ref pCastingAction.HitResult );
				//MyTool.DoSkillEffect( Atker , Atker.FightAttr.CastPool , Atker.FightAttr.Skill.s_CAST_TRIG ,  Atker.FightAttr.CastEffPool , ref pCastingAction.HitResult  );
			}
			
			nPhase++;
			break;
		case 1:			// hit
			Panel_StageUI.Instance.FadeOutAVGObj();
			// hit effect
			uAction pAct = ActionManager.Instance.CreateHitAction ( nAtkerID, nAtkerSkillID, nDeferID, nTarGridX , nTarGridY   );
			if( pAct != null )
			{
				if( Defer != null ){
					if( Defer != Atker ){
						Defer.SetFightAttr(  nAtkerID  , 0 ); // no skill
						ShowDefAssist( Defer.n_Ident , false );
					}
				}

				GetAtkHitResult( nAtkerID , nDeferID , Atker , Defer , Atker.FightAttr.SkillID ,  nTarGridX , nTarGridY , ref pAct , ref AtkAffectPool );


			
			}
			nPhase++;
			break;
         case 2: 	{// atker twice atk
			//if( Atker.Dist( Defer  )<=2 ){ // need range 2
				if( Atker.IsDead() == false && Atker.IsStates( _FIGHTSTATE._TWICE)  && MyTool.IsFinishSkill(nAtkerSkillID) ){
                    // 必須是 終結技才會twice

					uAction pAtkTwice = ActionManager.Instance.CreateHitAction (nAtkerID, nAtkerSkillID, nDeferID, nTarGridX, nTarGridY); // always normal atk
					if( pAtkTwice != null ){
                            //Atker.FightAttr.fBurstRate -= 0.5f;		// 第二下 傷害降低
                            GetAtkHitResult(nAtkerID, nDeferID, Atker, Defer, nAtkerSkillID , nTarGridX, nTarGridY, ref pAtkTwice, ref AtkAffectPool);
					}
				}
			//}
			nPhase++;
		}break;
		case 3:			
			// wait all action complete
			//if( Panel_StageUI.Instance.IsAnyActionRunning() == false ){

				nPhase++;	
			//}

			break;
		case 4:			// close all 
			nPhase++;
			// cal cul drop

			// avoid a unit be clear twice
			if( (Defer!=null) && (Defer!=Atker) ){
				Defer.FightEnd();
			}

			foreach( cUnitData unit in AtkAffectPool )
			{
				if( (unit!=Atker) && (unit!=Defer) )
					unit.FightEnd();				
			}
			if( Atker != null ){
				Atker.FightEnd( true );
			}

            // cmd finish
            RecordLastCombatCharID();
           // action finish in atk action
            Clear ();
			Panel_StageUI.Instance.ClearAVGObj();
			break;
		}
	}

	public void RunScript()
	{

	}

	public void RollBaseCirit( cUnitData Atker, cUnitData Defer )
	{
		if (Atker == null || Defer == null) {
			return;
		}
		
		float fRate = Config.BaseCirit + ( ( Atker.GetMar()-Defer.GetMar() ) / Config.BaseDodge );
		float  fRoll = Random.Range (0.0f, 100.0f );	
		if( fRoll < fRate ){
			Atker.AddStates( _FIGHTSTATE._CIRIT );
		}


	}

	public void RollBaseDodge( cUnitData Atker, cUnitData Defer )
	{
		if (Atker == null || Defer == null) {
			return;
		}

		float fRate = Config.BaseDodge + ( (Defer.GetMar() - Atker.GetMar() ) / Config.BaseDodge );
		float  fRoll = Random.Range (0.0f, 100.0f );	
		if( Atker.IsStates( _FIGHTSTATE._HIT ) || fRoll < fRate ){
			Defer.AddStates( _FIGHTSTATE._DODGE );
		}

	}


	public void GetAtkHitResult( int atk , int def , cUnitData Atker, cUnitData Defer, int nSkillID  , int gridX ,int gridY ,ref uAction action , ref List< cUnitData > AffectPool )
	{
		if( (Atker == null) ||  (Atker.FightAttr == null) ){
			Debug.LogErrorFormat( "GetAtkHitResult with null atker {0}" , atk );
			return;
		}
		// check change target pos
		// get affectpool
		AffectPool.Clear();
//
//		// check if self cast
//		SKILL skill = ConstDataManager.Instance.GetRow<SKILL> ( Atker.FightAttr.SkillID );
//		switch( skill.n_TARGET ){
//			case 0:	//0→對自己施展
//			case 6:	//6→自我AOE我方
//			case 7:	//7→自我AOE敵方
//			case 8:	//8→自我AOEALL
//				gridX = Atker.n_X;
//				gridY = Atker.n_Y;
//				break;
//			case 1:	//→需要敵方目標
//			case 2:	//→需要友方目標
//				if( Defer != null ){
//					gridX = Defer.n_X;
//					gridY = Defer.n_Y;
//				}
//				break;
//	//		case 3:	//→MAP敵方
//	//		case 4: //→MAP我方
//	//		case 5:	//→MAPALL		
//				break;
//		}


		// check is damage 
		bool bIsDamage =  MyTool.IsDamageSkill( nSkillID );
		int nGuarderID   = 0;
		// single atk
		if( Defer != null ){
			//gridX = Defer.n_X; // 不要這邊指定
			//gridY = Defer.n_Y;
			int 	  nRealTarId   = def;
			cUnitData cRealTarData = Defer;

			if( bIsDamage ){

				// check if base cirit happen
				//RollBaseCirit ( Atker , Defer ); // cancel cirit to avoid mob be overkill
				
				// check if base dodge happen
				RollBaseDodge ( Atker , Defer );
				// 
				nGuarderID = Defer.Buffs.GetGuarder();
                //if( Defer.IsStates( _FIGHTSTATE._GUARD ) )
                if (nGuarderID != 0)
                {
                    // change atk target to guard
                    cUnitData Guarder = GameDataManager.Instance.GetUnitDateByIdent(nGuarderID);
                    if (Guarder != null)
                    {
                        Guarder.SetFightAttr(atk, 0); // maybe change to def skill

                        action.AddHitResult(new cHitResult(cHitResult._TYPE._GUARD, Guarder.n_Ident, def));

                        if (!AffectPool.Contains(Guarder))
                        {  // this is call ed clear affect. but cause be hit issue
                            AffectPool.Add(Guarder);
                        }

                        cRealTarData = Guarder;     // change defer
                        nRealTarId = Guarder.n_Ident;
                    }

                }
                else
                {

                    action.AddHitResult(CalAttackResult(atk, nRealTarId, IsDefMode()));
                }
			}

            if (nGuarderID != 0) {
                // guard will be cacul in affect pool phase
            }
            else
            {
                action.AddHitResult(CalSkillHitResult(Atker, cRealTarData, nSkillID));
            }
		}

		// aoe attack

		if (Atker.IsStates( _FIGHTSTATE._THROUGH ) ) {  // 避免自我施法時穿透問題
			GetThroughPool( Atker , Defer ,nSkillID , gridX , gridY , ref AffectPool );
		}
		
		GetAffectPool( Atker , Defer , nSkillID ,gridX , gridY , ref AffectPool );
		foreach( cUnitData unit in AffectPool )
		{
			//=====================
			// checked if this cUnitData can Atk
			// already check in get affect pool
			//if( CanPK( Atker.eCampID , unit.eCampID ) == false )
			//	continue;
			if( Defer == unit ){
				continue;
			}
			int 	  nRealTarId   = unit.n_Ident;
			cUnitData cRealTarData = unit;
   //         nGuarderID = 0;
            if ( bIsDamage ){
				// change atk target to guard
				// can't guard aoe affect

				//cUnitData Guarder = GameDataManager.Instance.GetUnitDateByIdent( nGuarderID );
    //            if (Guarder != null)
    //            {
    //                Guarder.SetFightAttr(atk, 0); // maybe change to def skill

    //                action.AddHitResult(new cHitResult(cHitResult._TYPE._GUARD, Guarder.n_Ident, def));

    //                if (!AffectPool.Contains(Guarder))
    //                {
    //                    AffectPool.Add(Guarder);
    //                }

    //                cRealTarData = Guarder;     // change defer
    //                nRealTarId = Guarder.n_Ident;
    //            }
    //            else
    //            {
                    unit.SetFightAttr(Atker.n_Ident, 0);
                    ShowDefAssist(unit.n_Ident, false);
                    action.AddHitResult(CalAttackResult(atk, nRealTarId, false, true));// no def at this time
    //            }
			}
            
            action.AddHitResult(CalSkillHitResult(Atker, cRealTarData, nSkillID));
            
		}
        // Link Skill
        ProcessLinkSkill(atk, def, Atker, Defer , nSkillID , gridX , gridY, ref action ,ref AffectPool);

    }

    public void ProcessLinkSkill(int atk, int def, cUnitData Atker, cUnitData Defer, int nSkillID, int gridX, int gridY, ref uAction action, ref List<cUnitData> AffectPool)
    {
        SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nSkillID);
        if (skl != null)
        {
            if (string.IsNullOrEmpty(skl.s_LINK_SKILL))
                return;
            if (skl.s_LINK_SKILL == "null" || skl.s_LINK_SKILL == "NULL")
                return;
           
                // cTextArray Script = new cTextArray();
                // 		Script.SetText (strEffect);
            string[] links = skl.s_LINK_SKILL.Split(";".ToCharArray());
            foreach (string s in links)
            {
                    int id ;
                    if (int.TryParse(s, out id))
                    {
                        if (id == 0 || (id == nSkillID))
                            continue;

                        uAction pAct = ActionManager.Instance.CreateHitAction(Atker.n_Ident, id , def, gridX, gridY);
                        if (pAct!= null)
                        {
                            List<cUnitData> pool = new List<cUnitData>();

                            Atker.DoCastEffect(id, Defer, ref pAct.HitResult); // cast effect

                            // def ?

                            GetAtkHitResult(atk, def, Atker, Defer, id, gridX, gridY, ref pAct, ref pool);

                            foreach(cUnitData data in pool)
                            {
                                if(AffectPool.Contains( data ) == false  )
                                {
                                    AffectPool.Add( data );
                                }
                            }
                        }
                     //   AddTag((_UNITTAG)tag);
                    }
            }
        }
    }


    //drop after dead event complete
 //   public bool HaveDrop()
	//{
	//	return ( nDropExpPool.Count > 0 ) ;
	//}

	public bool CheckDrop()
	{
        if (IsBattlePhase())
            return false; // 戰鬥中不處理

        
        if(BattleMsg.nMsgCount > 0 )
            return false; //訊息中不處理（含自己process 出來的掉落訊息


//        if (IsDroping() )
//        {
			// drop item
			if( nDropItemPool.Count > 0 )
			{
				int itemid = nDropItemPool[0];
				string str = "獲得 " + MyTool.GetItemName( itemid );
				nDropItemPool.RemoveAt(0);
				ShowBattleMsg( 0 , str );

				GameDataManager.Instance.AddItemtoBag( itemid );// Add to item pool
				return true ;
			}
        //		}

        if (nDropExpPool.Count > 0)
        {
            foreach ( KeyValuePair<int , int> pair in nDropExpPool )
		    {
                Panel_unit unit = Panel_StageUI.Instance.GetUnitByIdent(pair.Key);
                if (unit != null)
                {
                    unit.ActionDrop(pair.Value, nDropMoney); // 實際增加經驗值
                    nDropMoney = 0; // avoid double add money
                }
           
                //ActionDrop
                //	ActionManager.Instance.CreateDropAction( pair.Key , pair.Value , nDropMoney );
                //nDropMoney = 0;
            }

            nDropExpPool.Clear();
            return true;
        }

        // 如果沒有人獲得經驗，則只顯示增加金錢
        if (nDropMoney > 0) {
            ShowAddMoney(nDropMoney);

            //		ActionManager.Instance.CreateDropAction( 0 , 0 , nDropMoney );
            nDropMoney = 0;
        }
		return false;
	}

    public void ShowAddMoney( int nMoney )
    {
        string sMsg = string.Format(" Money + {0}",  nMoney);        

        GameDataManager.Instance.nMoney += nMoney;
        BattleManager.Instance.ShowDropMsg(sMsg);
    }

	// recycle all drop exp when stage end
	public void RecycleDrop()
	{
		// item
		foreach( int itemid in nDropItemPool )
		{			
			GameDataManager.Instance.AddItemtoBag( itemid );// Add to item pool	
		}
		nDropItemPool.Clear();
		// exp 
		foreach( KeyValuePair<int , int> pair in nDropExpPool )
		{
			cUnitData pUnitData = GameDataManager.Instance.GetUnitDateByIdent( pair.Key );
			if( pUnitData == null )
				continue;		// by pass and wait stage end to recycle

			pUnitData.AddExp( pair.Value );

		}
		nDropExpPool.Clear();
		// money
		GameDataManager.Instance.nMoney += nDropMoney;
		nDropMoney = 0;

        // 清除戰鬥狀態
        Clear();

    }

	//===================================================
	public _BATTLE eBattleType { get; set; } 
//	public bool bIsBattle { get; set; } 
	public bool bPause { get; set; } 

	public _CMD_ID eAtkCmdID{ get; set; } 
	public _CMD_ID eDefCmdID{ get; set; } 

	public int nAtkerID{ get; set; } 
	public int nDeferID{ get; set; }

    public int nAtkerSkillID { get; set; }
    public int nDeferSkillID { get; set; }


    public int nLastAtkerCharID { get; set; }
    public int nLastDeferCharID { get; set; }

    //	public bool bDefMode{ get; set; } 

    public int nLastAtkerSkillID{ get; set;} 
	public int nLastDeferSkillID{ get; set;  } 

	//public bool bIsDefenceMode { get; set; } 		// 

	public int nTarGridX{ get; set; } 				//
	public int nTarGridY{ get; set; } 				//

	public int nBattleID{ get; set; } 

	public bool bCanCounter{ get; set; }     // def have counter action

	public int nDropMoney{ get; set; }      // drop money
    public bool HaveDrop()
    {
        if (nDropExpPool.Count > 0 || nDropItemPool.Count > 0)
        {
            return true;
        }
        return false;
    }
    public bool IsDroping() {   // 正在撥 掉落事件            
        if (nDropExpPool.Count > 0 || nDropItemPool.Count > 0 ) {
            return true;
        }
        return false;
    }
    

	public Dictionary< int , int > nDropExpPool;
	public List<int>			nDropItemPool;
	//===================================================
//	SKILL	AtkerSkill = null;
//	SKILL	DeferSkill = null;
	private int nPhase = 0; // 
	// AOE
	List< cUnitData > AtkAffectPool = null; //攻方影響人數
	List< cUnitData > DefAffectPool = null; //守方反擊影響人數


	// CC link
//	List< cUnitData > AtkCCPool = null;
//	List< cUnitData > DefCCPool = null;

	//======================================================
//	public void SetAtkSkill( int skillid )
//	{
//		nAtkerSkillID = 0;
//		AtkerSkill = null;
//		if (skillid == 0) {
//			return ;
//		}
//
//		AtkerSkill = ConstDataManager.Instance.GetRow< SKILL > ( skillid ); 
//		if (AtkerSkill != null) {
//			nAtkerSkillID = skillid;
//		}
//
//	}
//	public void SetDefSkill( int skillid )
//	{
//		nDeferSkillID = 0;
//		DeferSkill = null;
//		if (skillid == 0) {
//			return ;
//		}
//		
//		DeferSkill = ConstDataManager.Instance.GetRow< SKILL > ( skillid ); 
//		if (DeferSkill != null) {
//			nDeferSkillID = skillid;
//		}		
//	}

	public void PlayCharFightBGMbyIdent( int nIdent )
	{
		if( nIdent == 0 )
			return;
        if (MyTool.IsDamageSkill(nAtkerSkillID) == false) // 干擾技能 不播放音樂
            return;

        cUnitData pData = GameDataManager.Instance.GetUnitDateByIdent( nIdent );
		if( pData == null ){
			return ;
		}
		if( pData.eCampID != _CAMP._ENEMY ){
			return;
		}

		PlayCharFightBGM( pData.n_CharID ); 
	}

    public void PlayCharFightBGM( int nCharID )
	{
		if( nCharID == 0 )
			return;

        if (MyTool.IsDamageSkill(nAtkerSkillID) == false)
            return;

		CHARS pData = ConstDataManager.Instance.GetRow< CHARS >( nCharID ); 
		if( pData == null ){
			return ;
		}
		if( pData.n_FIGHTBGM > 0  ){
			GameSystem.PlayBGM( pData.n_FIGHTBGM );
		}
	}

	public void PlayAttack (int nAtkIdent, int nDefIdent, int nSkillID)
	{
		nAtkerID = nAtkIdent;
		nDeferID = nDefIdent;
		nAtkerSkillID = nSkillID ;
		eBattleType = _BATTLE._ATTACK;

        PlayCharFightBGMbyIdent(nAtkIdent);
        PlayCharFightBGMbyIdent(nDefIdent);


    }
    public void PlayCast (int nAtkIdent, int nDefIdent , int nGridX , int nGridY , int nSkillID)
	{
		nAtkerID = nAtkIdent;
		nDeferID = nDefIdent;
		nTarGridX = nGridX;
		nTarGridY = nGridY;

		nAtkerSkillID = nSkillID;
		eBattleType = _BATTLE._CAST ;
        
        PlayCharFightBGMbyIdent(nAtkIdent);
        PlayCharFightBGMbyIdent(nDefIdent);

    }
    // no used
    public void PlayBattleID (int battleid )
	{
		nBattleID = battleid;
	}

	// 針對 counter 的立即處理
	public void PlayCounterCast (int nCastIdent, int nTarIdent , int nSkillID )
	{
		cUnitData Caster = GameDataManager.Instance.GetUnitDateByIdent ( nCastIdent );
		cUnitData Target = GameDataManager.Instance.GetUnitDateByIdent ( nTarIdent );

		ActionManager.Instance.CreateCastAction( nCastIdent , nSkillID  , nTarIdent );
		// hit effect
		uAction pAct = ActionManager.Instance.CreateHitAction ( nCastIdent, nSkillID , nTarIdent, 0, 0  );
		if( pAct != null )
		{
			//必須先取得影響人數
			int nTarX = 0;
			int nTarY = 0;
		
			// 只有簡單給 buff 可以執行

			SKILL skl = ConstDataManager.Instance.GetRow<SKILL>( nSkillID );
			if( skl == null )
				return;

			 GetAtkHitResult( nCastIdent , nTarIdent , Caster , Target ,nSkillID ,  nTarX , nTarY , ref pAct , ref AtkAffectPool );


			//pAct.AddHitResult(  CalSkillHitResult( Caster, Target , nSkillID ) ) ;

		}


//		nAtkerID = nAtkIdent;
//		nDeferID = nTarIdent;
//		nTarGridX = nGridX;
//		nTarGridY = nGridY;
//		
//		nAtkerSkillID = nSkillID;
//		
//		eBattleType = _BATTLE._CAST ;
	}

    //	public void Play()
    //	{
    //		bPause = false;
    //	}

    //	public void ShowSysMsg( string msg )
    //	{
    //		GameObject go = ResourcesManager.CreatePrefabGameObj ( Panel_StageUI.Instance.MaskPanelObj , "prefab/BattleMsg" );
    //		if (go != null) {
    //			go.transform.position = v;
    //			UILabel lbl = go.GetComponentInChildren<UILabel>();
    //			if( lbl != null )
    //			{
    //				lbl.text = msg;
    //			}
    //		}
    //	}

  

    public void ShowDropMsg( string msg)
    {
        Vector3 v = Vector3.zero;
        GameObject go = ResourcesManager.CreatePrefabGameObj(Panel_StageUI.Instance.MaskPanelObj, "prefab/BattleMsg");        
        //GameObject go = ResourcesManager.CreatePrefabGameObj ( Panel_StageUI.Instance.MaskPanelObj , "prefab/BattleMsg" );

        if (go != null)
        {
           // bIsDroping = true;
            go.transform.position = v;
            //go.transform.localPosition = v;
            UILabel lbl = go.GetComponentInChildren<UILabel>();
            if (lbl != null)
            {
                lbl.text = msg;
            }
           
        }

    }

    public void ShowBattleMsg( int nIdent , string msg )
	{
		Panel_unit unit = Panel_StageUI.Instance.GetUnitByIdent( nIdent ); 
		ShowBattleMsg (unit , msg );

	}

	public void ShowBattleMsg( Panel_unit unit , string msg )
	{
        GameObject go = null;
        Vector3 v = Vector3.zero;
        if (unit != null)
        {
            go = ResourcesManager.CreatePrefabGameObj(unit.gameObject, "prefab/BattleMsg");
            // show in screen center
            v = unit.transform.position;
            //v = unit.transform.parent.localPosition+unit.transform.localPosition;
        }
        else {
            //
            go = ResourcesManager.CreatePrefabGameObj(Panel_StageUI.Instance.MaskPanelObj, "prefab/BattleMsg");
        }
        //GameObject go = ResourcesManager.CreatePrefabGameObj ( Panel_StageUI.Instance.MaskPanelObj , "prefab/BattleMsg" );
        
        if (go != null) {
			go.transform.position = v;
			//go.transform.localPosition = v;
			UILabel lbl = go.GetComponentInChildren<UILabel>();
			if( lbl != null )
			{
				lbl.text = msg;
			}
		}

	}
    public void ShowCastMsg(int nIdent, string msg)
    {
        Panel_unit unit = Panel_StageUI.Instance.GetUnitByIdent(nIdent);
        ShowCastMsg(unit, msg);

    }
    public void ShowCastMsg(Panel_unit unit, string msg)
    {
        GameObject go = null;
        Vector3 v = Vector3.zero;
        if (unit != null)
        {
            go = ResourcesManager.CreatePrefabGameObj(unit.gameObject, "prefab/CastMsg");
            // show in screen center
            v = unit.transform.position;
            //v = unit.transform.parent.localPosition+unit.transform.localPosition;
        }
        else
        {
            //
            go = ResourcesManager.CreatePrefabGameObj(Panel_StageUI.Instance.MaskPanelObj, "prefab/CastMsg");
        }
        //GameObject go = ResourcesManager.CreatePrefabGameObj ( Panel_StageUI.Instance.MaskPanelObj , "prefab/BattleMsg" );

        if (go != null)
        {
            MyTool.SetDepth( go , 20 , unit.gameObject );
            //UIWidget widget = go.GetComponent<UIWidget>();
            //widget.depth = 20;

            go.transform.position = v;
            //go.transform.localPosition = v;
            UILabel lbl = go.GetComponentInChildren<UILabel>();
            if (lbl != null)
            {
                lbl.text = msg;
            }
        }

    }

    public void ShowBattleFX( int nIdent , int nFxId )
	{
		if( nIdent==0 || nFxId==0 )
			return;

		Panel_unit unit = Panel_StageUI.Instance.GetUnitByIdent( nIdent ); 
		if (unit == null)
			return;

		//string path = "FX/Cartoon FX/" + fx;
		//GameObject go = ResourcesManager.CreatePrefabGameObj ( unit.gameObject , fx );
		GameObject go = GameSystem.PlayFX ( unit.gameObject , nFxId  );
		if (go != null) {

		}
	}

	public void ShowBattleResValue( int nIdent , int nValue , int nMode)
	{
		Panel_unit unit = Panel_StageUI.Instance.GetUnitByIdent( nIdent ); 
		if (unit == null)
			return;
		ShowBattleResValue ( unit.gameObject , nValue , nMode );
	}
	public void ShowBattleResValue( GameObject obj , int nValue , int nMode )
	{	
		//nMode : 0 - hp , 1- def , 2 - mp , 3 -sp , 4 
		Vector3 v = Vector3.zero;
		if ( obj != null) {
			// show in screen center
//			Vector3 vp = obj.transform.parent.localPosition;
//			v = obj.transform.localPosition;
//			v = obj.transform.parent.localPosition+obj.transform.localPosition;

			//v = obj.transform.position ;
		}
		//Panel_StageUI.Instance.TilePlaneObj.transform.localPosition;
		// modify local pos
		switch (nMode) {
//		case 0:  // hp 
//			break;
		case 1:  // def
//			v.y += 50;
			break;
		case 2:  // mp
//			v.y -= 50;
			break;
        case 3:  // sp
            break;

        }
			

		//GameObject go = ResourcesManager.CreatePrefabGameObj ( Panel_StageUI.Instance.MaskPanelObj , "Prefab/BattleValue" );
		GameObject go = Panel_StageUI.Instance.SpwanBattleValueObj (Panel_StageUI.Instance.MaskPanelObj , obj.transform.localPosition ); // this will affect initial pos in enable
		if (go != null) {
            //go.transform.position = obj.transform.position ;
            //go.transform.localPosition = v;
            //Tween Y 將造成 成像上的偏移錯誤。徹底解決前～不開放
            // tween Y here for correct pos value
            //			TweenY twnY = TweenY.Begin<TweenY>( go , 0.5f ); 
            //			if( twnY != null ){
            //				twnY.SetStartToCurrentValue();
            //				twnY.to = v.y +300;
            //				twnY.Play();
            //
            //			}

            //nMode : 0 - hp , 1- def , 2 - mp , 3 -sp
          
            UILabel lbl = go.GetComponent< UILabel >();
			if( lbl )
			{
                lbl.text = "";
                switch ( nMode ){
				case 0:  // hp 
					if( nValue > 0 ){								
						lbl.gradientTop = new Color( 0.0f, 1.0f , 0.0f );	// green
					}
					else{
						lbl.gradientTop = new Color( 1.0f, 0.0f , 0.0f );	// red
					}
					break;

				case 1:  // def 

                     //   lbl.text = "防禦";
                        if ( nValue > 0 ){
							lbl.gradientTop = new Color( 1.0f, 1.0f , 0.0f );  // yellow
						}
						else{
							lbl.gradientTop = new Color( 0.0f, 1.0f , 0.0f );	// red
						}
					break;
					case 2:  // mp 
                     //   lbl.text = "內力";
                        if ( nValue > 0 ){
						lbl.gradientTop = new Color( 0.0f, 0.0f , 1.0f );  // blue 
						}
						else{
						lbl.gradientTop = new Color( 1.0f, 0.0f , 1.0f );	// purple
						}
					break;
					case 3:  // sp 
					if( nValue > 0 ){
						lbl.gradientTop = new Color( 1.0f, 1.0f , 1.0f );  // white
					}
					else{
						lbl.gradientTop = new Color( 0.0f, 0.0f , 0.0f );	// black
					}
					break;
				default:
					if( nValue > 0 ){								
						lbl.gradientTop = new Color( 0.0f, 1.0f , 0.0f );	// green
					}
					else{
						lbl.gradientTop = new Color( 1.0f, 0.0f , 0.0f );	// red
					}
					break;
				}
//
//				if( nValue > 0 )
//				{
//					if( nMode == 1 ){
//						lbl.gradientTop = new Color( 1.0f, 1.0f , 0.0f );
//					}
//					else {
//						lbl.gradientTop = new Color( 0.0f, 1.0f , 0.0f );
//					}
//				}
//				else{ 
//					// damage
//					lbl.gradientTop = new Color( 1.0f, 0.0f , 0.0f );
//				}
//
//

				lbl.text += nValue.ToString();
			}
		}
	}

	public void ShowBattleResValue( GameObject obj , string sMsg , int nColorMode )
	{	
		Vector3 v = new Vector3 (0, 0, 0);
//		if ( obj != null) {
	//		v = obj.transform.parent.localPosition+obj.transform.localPosition;
		//}
		GameObject go = Panel_StageUI.Instance.SpwanBattleValueObj (Panel_StageUI.Instance.MaskPanelObj ,  v ); // show on mask 
		
		if (go != null) {
            //go.transform.position = v;
            //go.transform.localPosition = v;			
            go.transform.position = obj.transform.position;

            UILabel lbl = go.GetComponent< UILabel >();
			if( lbl )
			{

			//	lbl.gradientTop = new Color( 1.0f, 0.0f , 0.0f );

				lbl.text = sMsg;
				switch( nColorMode ){
				case 1:			// light blue
					lbl.gradientTop = new Color( 0.0f, 1.0f , 1.0f );  // light blue 
					break;
				default:		// red
					lbl.gradientTop = new Color( 1.0f, 0.0f , 0.0f );	// red
					break;
				}

			}
		}
	}

	public void ShowAtkAssist( int nAtkIdent ,  int nDefIdent , bool bShow = false )
	{
		cUnitData pAtker = GameDataManager.Instance.GetUnitDateByIdent ( nAtkIdent );
		if ( pAtker == null)
			return;

		cUnitData pDefer = GameDataManager.Instance.GetUnitDateByIdent ( nDefIdent );
		if (pDefer == null)
			return;

		pAtker.FightAttr.fAtkAssist = 0.0f;

		// check if near atker 
		foreach( KeyValuePair< int , cUnitData> pair in GameDataManager.Instance.UnitPool )
		{
			if( pair.Key == nAtkIdent )
				continue;
			if( pair.Value.IsDead() || pair.Value.IsTriggr() )
				continue;

			if( CanPK( pAtker.eCampID , pair.Value.eCampID )== false )
			{
				  
				if( iVec2.Dist(  pDefer.n_X , pDefer.n_Y , pair.Value.n_X , pair.Value.n_Y   ) >1 )
				{
					continue;
				}

				// addto cc pool?
				if(bShow== true ){
					ShowBattleMsg( pair.Key , "助攻");
					// AVG obj
					Panel_StageUI.Instance.AddAVGObj( pair.Key  , true , true );
				}
				// add to pool regedit to unit data?
				pAtker.FightAttr.fAtkAssist += Config.AssistRate;
			}
		}

	}
	
	public void ShowDefAssist( int nDefIdent , bool bShow = false  )
	{
		cUnitData pDefer = GameDataManager.Instance.GetUnitDateByIdent ( nDefIdent );
		if (pDefer == null)
			return;

		pDefer.FightAttr.fDefAssist = 0.0f;

		foreach( KeyValuePair< int , cUnitData> pair in GameDataManager.Instance.UnitPool )
		{
			if( pair.Key == nDefIdent )
				continue;
			if( pair.Value.IsDead() || pair.Value.IsTriggr() )
				continue;
			if( CanPK( pDefer.eCampID , pair.Value.eCampID )== false )
			{
				
				if( iVec2.Dist(  pDefer.n_X , pDefer.n_Y , pair.Value.n_X , pair.Value.n_Y   ) >1 )
				{
					continue;
				}
				
				// addto cc pool?
				if( bShow == true ){
					ShowBattleMsg( pair.Key , "協防");
					// AVG obj
					Panel_StageUI.Instance.AddAVGObj( pair.Key , true , false );
				}
				//
				pDefer.FightAttr.fDefAssist += Config.AssistRate;


			}
		}
	}

	// 防禦方選反擊
	public bool IsCounterMode()
	{
		return ( eDefCmdID == _CMD_ID._COUNTER );
	}
	// 防禦方選防守
	public bool IsDefMode()
	{
		return ( eDefCmdID == _CMD_ID._DEF );
	}

	public void GetThroughPool( cUnitData Atker , cUnitData Defer , int SkillID , int nTarX , int nTarY  , ref List< cUnitData> pool, int nLen=1  )
	{
        if (Atker == Defer)
        {
            return;
        }
        //====
        int nDefer = 0;
		if( Defer != null ){
			nTarX = Defer.n_X;
			nTarY = Defer.n_Y;
			nDefer = Defer.n_Ident;
		}

		if( pool == null ){
			pool = new List< cUnitData > ();
		}

		// Get through effect

		_DIR dir = iVec2.Get8Dir( Atker.n_X ,Atker.n_Y , nTarX , nTarY );
		iVec2 v = new iVec2( nTarX , nTarY );
		for( int i = 0 ; i < nLen ; i++ ){
			v = iVec2.Move8Dir(  v.X , v.Y , dir );
			cUnitData pUnit = GameDataManager.Instance.GetUnitDateByPos( v.X  ,v.Y );
			if( pUnit != null ){
					// defer don't add . he have a spec process
			if( pUnit.n_Ident == nDefer )  
					continue;
				
			if(  CanPK( Atker.eCampID , pUnit.eCampID ) == true ){
				if( pool.Contains( pUnit ) == false ){
					pool.Add( pUnit );
				}
			}
			}
		}
	}

	static public void GetAffectPool( cUnitData Atker , cUnitData Defer , int SkillID , int nTarX , int nTarY , ref List< cUnitData> pool  )
	{
		// don't push defer to affect pool
		int nDefer = 0;
		SKILL skl = ConstDataManager.Instance.GetRow< SKILL > ( SkillID );
		if (skl == null) {
			return;
		}

		if( Defer != null ){
			nDefer = Defer.n_Ident;
		}
		
		//		0→對自己施展
		//		1→需要敵方目標
		//		2→需要友方目標
		//		3→MAP敵方
		//		4→MAP我方
		//      5-MAP-all 
		//		6→自我AOE我方
		//		7→自我AOE敵方
		//		8→自我AOEALL
		//      9→我方全員
		//     10→敵方全員
		//     11-all unit  
		bool bCanPK = false;
		bool bAll = false;

		if( MyTool.GetSkillPKmode( skl ) == _PK_MODE._ENEMY  )
		{
			bCanPK = true;
		}
		else if( MyTool.GetSkillPKmode( skl ) == _PK_MODE._ALL  )
		{
			bAll = true;
		}
		else{
			// help skill
		}
//		if ((skl.n_TARGET == 1) || (skl.n_TARGET == 3) || (skl.n_TARGET == 7) || (skl.n_TARGET == 10) ) {
//			bCanPK = true;
//		} else if( (skl.n_TARGET == 5)||(skl.n_TARGET == 8) ){
//			bAll = true;
//		}
		// fix  pos

		ConvertSkillTargetXY( Atker ,SkillID , nDefer , ref nTarX , ref nTarY );
//		if( (skl.n_TARGET==0) || (skl.n_TARGET==6) || (skl.n_TARGET==7) || (skl.n_TARGET==8) ){ // self cast
//			nTarX = Atker.n_X;
//			nTarY = Atker.n_Y;
//		}
//		else if( (skl.n_TARGET==1) || (skl.n_TARGET==2) ) // target
//		{
//			if( Defer != null ){
//				nTarX = Defer.n_X;
//				nTarY = Defer.n_Y;
//			}
//		}

		// start push to pool
		if( pool == null ){
			pool = new List< cUnitData > ();
		}

		//==============
		//  get all
		if( (skl.n_TARGET == 9) || (skl.n_TARGET == 10) )
		{
			foreach( KeyValuePair<int , cUnitData > pair in GameDataManager.Instance.UnitPool )
			{
				cUnitData pUnit = pair.Value;
				if( pUnit.n_Ident == nDefer )  
					continue;				
				if( pUnit.IsDead() )
					continue;
                if (pUnit.IsTag( _UNITTAG._PEACE ) ) {
                    continue;
                }

				if( bAll == true ){ // all is no need check
					if( pUnit == Atker )  
						continue;				

					if( pool.Contains( pUnit ) == false ){
						pool.Add( pUnit );
					}				
				}
				else if(  MyTool.CanPK( Atker.eCampID , pUnit.eCampID ) == bCanPK ){
					if( pool.Contains( pUnit ) == false ){
						pool.Add( pUnit );
					}
				}
			}
			return ;
		}


		// get AOE affect
		if (skl.n_AREA == 0)
			return;

		List < iVec2 > lst = MyTool.GetAOEPool ( nTarX ,nTarY,skl.n_AREA ,Atker.n_X , Atker.n_Y  );

	
		// check  if have affect
		foreach (iVec2 v in lst) {
			cUnitData pUnit = GameDataManager.Instance.GetUnitDateByPos( v.X , v.Y );
			if( pUnit != null ){
				// defer don't add . he have a spec process
				if( pUnit.n_Ident == nDefer )  
					continue;

				if( pUnit.IsDead() )
					continue;
                if (pUnit.IsTag(_UNITTAG._PEACE))
                {
                    continue;
                }
                if ( bAll == true ){ // all is no need check
					if( pUnit == Atker )  
						continue;				
					if( pool.Contains( pUnit ) == false ){
						pool.Add( pUnit );
					}
				}
				else if(  MyTool.CanPK( Atker.eCampID , pUnit.eCampID ) == bCanPK ){
					if( pool.Contains( pUnit ) == false ){
						pool.Add( pUnit );
					}
				}
			}
		}


		//=============================================================
//		if( pool.Count == 0 ){
//			Debug.Log( " GetAffectPool with 0 ");
//		}

	}

	public void CalDropResult( cUnitData Atker , cUnitData Defer )
	{
		//return;

		if( Atker == null || Defer == null ) return ;
		if( Atker.eCampID != _CAMP._PLAYER  ) return;

		float fdroprate =  Atker.GetMulDrop();

		//
		int exp 	= 5; // base exp
		int money 	= 0;
		if( Defer != null ){
			int nDiffLv = Defer.n_Lv-Atker.n_Lv;
			exp += (nDiffLv);
			exp = MyTool.ClampInt( exp , 1 , 10 );

			// kill
			if( Defer.IsStates( _FIGHTSTATE._DEAD ) ){

				exp = (exp*3) ;
                money = Config.BaseMobMoney;
                // 怪物 武功等級影響掉落資金
                
                int nIntLV = Defer.GetIntSchLv();
                int nExtLV = Defer.GetExtSchLv();
                //int nMaxLv = nIntLV > nExtLV ? nIntLV : nExtLV;
                //if (nMaxLv < 1)                    nMaxLv = 1;
                // float fMoneyRatio = Mathf.Pow(1.5f, (nMaxLv-1));
                //float fMoneyRatio = 1.0f + ((nIntLV + nExtLV) * 0.1f); // 不受內外功等級影響
                float fMoneyRatio = 1.0f;
                money = (int)(money * fMoneyRatio);

                // check drop item
                if ( Defer.cCharData.n_ITEM_DROP  > 0 ){
					nDropItemPool.Add( Defer.cCharData.n_ITEM_DROP );
				}
			}
		}
        // mul drop
        money = (int)(money * fdroprate * Defer.cCharData.f_DROP_MONEY);
        exp = (int)(exp * fdroprate * Defer.cCharData.f_DROP_EXP);
        money =  MyTool.ClampInt(  money , 0 , money);
		exp   =  MyTool.ClampInt(  exp , 0 , exp );

		// Add to pool
		//nExp 	+= exp ; 
		//nMoney 	+= money;

		//====
		nDropMoney += money;

        // 賺錢
        GameDataManager.Instance.nEarnMoney += money;

        if ( nDropExpPool.ContainsKey( Atker.n_Ident )  ){
			nDropExpPool[Atker.n_Ident ] += exp;
		}
		else {
			nDropExpPool.Add( Atker.n_Ident , exp );
		}

	}


    static public List<cHitResult> CalAttackResult(int nAtker, int nDefer, bool bDefMode = false, bool bAffect = false)
    {
        cUnitData pAtker = GameDataManager.Instance.GetUnitDateByIdent(nAtker);     //Panel_StageUI.Instance.GetUnitByIdent( nAtker ); 
        cUnitData pDefer = GameDataManager.Instance.GetUnitDateByIdent(nDefer); //Panel_StageUI.Instance.GetUnitByIdent( nDefer ); 
        if ((pAtker == null) || (pDefer == null))
            return null;

        List<cHitResult> resPool = new List<cHitResult>();
        //標示用，讓後面有判斷依據
        pAtker.AddStates(_FIGHTSTATE._DAMAGE);      // 使用傷害姓技能
        pDefer.AddStates(_FIGHTSTATE._BEDAMAGE);// 被使用傷害姓技能


        // check if base cirit happen
        //		RollBaseCirit ( pAtker , pDefer );

        // check if base dodge happen
        //		RollBaseDodge ( pAtker , pDefer );

        if (bDefMode == true) {
            pDefer.AddStates(_FIGHTSTATE._DEFMODE);
            resPool.Add(new cHitResult(cHitResult._TYPE._CP, nDefer, 1)); // defence add 1 cp
        }
        // create result pool


        //	resPool.Add ( new cHitResult( cHitResult._TYPE._HIT ,nAtker , nDefer  ) );

        // Atk must hit

        // 守方強制迴避
        if (pDefer.IsStates(_FIGHTSTATE._DODGE)) {
            resPool.Add(new cHitResult(cHitResult._TYPE._DODGE, nDefer, 0));
            return resPool;
        }
        // atk方強制 Fail
        if (pAtker.IsStates(_FIGHTSTATE._MISS)) {
            resPool.Add(new cHitResult(cHitResult._TYPE._MISS, nAtker, 0));
            return resPool;
        }
        // 守方免疫傷害
        if (pDefer.IsStates(_FIGHTSTATE._NODMG))
        {
            resPool.Add(new cHitResult(cHitResult._TYPE._IMMUNE, nDefer, 0)); // no dmg
            return resPool;
        }

        //
        SKILL AtkerSkill = pAtker.FightAttr.SkillData.skill;

        // 守方免疫飛行道具
        if (pDefer.IsStates(_FIGHTSTATE._ANTIFLY))
        {
            if ((pAtker.FightAttr.SkillData != null) && (pAtker.FightAttr.SkillData.IsTag(_SKILLTAG._FLY))) {
                resPool.Add(new cHitResult(cHitResult._TYPE._IMMUNE, nDefer, 0)); // no dmg
                return resPool;
            }
        }


        // buff effect
        float AtkMarPlus = pAtker.FightAttr.fAtkAssist;   // assist is base pils
        float DefMarPlus = pDefer.FightAttr.fDefAssist;

        int AtkPowPlus = 0;
        int DefPowPlus = 0;
        int AtkPlus = 0;
        //		int DedfPlus = 0;

        // default 倍率
        //	float fAtkMarFactor = 0.0f;
        //	float fDefMarFactor = 0.0f;
        float fAtkPowFactor = 1.0f;
        float fDefPowFactor = 1.0f;
        float fAtkFactor = 1.0f;

        //		float fDefFactor = 1.0f;

        //float fAtkBurst  = 1.0f + pAtker.GetMulBurst ();
        //float fDefDamage = 1.0f + pDefer.GetMulDamage ();
        //resPool.Add ( new cHitResult( cHitResult._TYPE._BEHIT , nDefer , pAtker.FightAttr.SkillID ) );


        //		SKILL DeferSkill = pDefer.FightAttr.SkillData.skill;

        // skill effect
        if (AtkerSkill != null) {
            fAtkFactor = AtkerSkill.f_ATK;
            fAtkPowFactor = AtkerSkill.f_POW;
            // how to trig condition?
        }
        //		if (DeferSkill != null) {
        //			fDedFactor 	  = DeferSkill.f_ATK;
        //			fDefPowFactor = DeferSkill.f_POW;
        //		}

        // Buff condition Effect
        //		pAtker.UpdateBuffConditionEffect ( pDefer ); // update buff eff
        //		pDefer.UpdateBuffConditionEffect ( pAtker );

        // dmg record
        int nAtkHp = 0;
        int nDefHp = 0;

        float AtkMar = pAtker.GetMar() + AtkMarPlus;
        float DefMar = pDefer.GetMar() + DefMarPlus;
        // 1 mar = 0.5% hit rate
        float HitRate = ((AtkMar - DefMar + Config.HIT) / 100.0f); // add base rate
        if (HitRate < 0.0f)
            HitRate = 0.0f;


        float AtkPow = fAtkPowFactor * (pAtker.GetPow() + AtkPowPlus);
        float DefPow = fDefPowFactor * (pDefer.GetPow() + DefPowPlus);
        float fAtkBrust = pAtker.GetMulBurst();  //攻方爆發
        float fDefReduce = pDefer.GetMulDamage();  //守方減免
        int PowDmg = (int)(HitRate * (AtkPow - DefPow)); // 

        if (PowDmg > 0) {
            nDefHp -= (int)(PowDmg * fAtkBrust * fDefReduce);
        }
        else if (PowDmg < 0) { // 氣勁傷害反彈
            if (pAtker.FightAttr.SkillData.IsTag(_SKILLTAG._FLY) || bAffect)
            {
                //暗器不造成反彈，AOE坡及的也不造成反彈
            }
            else {
                nAtkHp += (int)(PowDmg * pDefer.GetMulBurst() * pAtker.GetMulDamage()); // it is neg value already
                if (nAtkHp < 0) {
                    if (((pAtker.n_HP + pAtker.n_DEF) < Mathf.Abs(nAtkHp))) {
                        if (pDefer.IsStates(_FIGHTSTATE._MERCY)) {
                            nAtkHp = -(pAtker.n_HP + pAtker.n_DEF - 1);
                        }
                        else {
                            pDefer.AddStates(_FIGHTSTATE._KILL);
                            pAtker.AddStates(_FIGHTSTATE._DEAD);
                        }
                    }
                    resPool.Add(new cHitResult(cHitResult._TYPE._HP, nAtker, nAtkHp));
                }
            }
        }

        // buff effect
        float Atk = (pAtker.GetAtk() + AtkPlus) * fAtkFactor;


        float fAtkDmg = (HitRate * Atk) * fAtkBrust * fDefReduce;

        // 計算物理護甲減傷

        fAtkDmg = (fAtkDmg < 0) ? 0 : fAtkDmg;
        // 防禦..
        if (bDefMode  )
        {
            fAtkDmg = (fAtkDmg * Config.DefReduce / 100.0f);
        }

        // cirit happpen       
        if (pAtker.IsStates(_FIGHTSTATE._CIRIT)) {

            fAtkDmg *= Config.CiritRatio;
            resPool.Add(new cHitResult(cHitResult._TYPE._CIRIT, nAtker, 0));
        }


        //守方反彈傷害 1/2
        if (pDefer.IsStates(_FIGHTSTATE._RETURN)) {
            // 非 肉搏 不反彈
            if (pAtker.FightAttr.SkillData.IsTag(_SKILLTAG._FLY) || bAffect)
            {
                //暗器不造成反彈，AOE坡及的也不造成反彈
            }
            else
            {
                nAtkHp = (int)(-0.5f * fAtkDmg);
                if (nAtkHp < 0)
                {
                    if (((pAtker.n_HP + pAtker.n_DEF) < Mathf.Abs(nAtkHp)))
                    {
                        if (pDefer.IsStates(_FIGHTSTATE._MERCY))
                        {
                            nAtkHp = -(pAtker.n_HP + pAtker.n_DEF - 1);
                        }
                        else
                        {
                            pDefer.AddStates(_FIGHTSTATE._KILL);
                            pAtker.AddStates(_FIGHTSTATE._DEAD);
                        }
                    }
                    resPool.Add(new cHitResult(cHitResult._TYPE._HP, nAtker, nAtkHp));
                }
            }
        }

        if (pAtker.IsStates(_FIGHTSTATE._BROKEN) == false)// 攻方沒有破甲效果
        {
            float DefAC = pDefer.GetArmor();

            //fAtkDmg *= ((100.0f - DefAC) / 100.0f);            
            fAtkDmg -= DefAC;
            if (fAtkDmg < 0.0f)
            {
                fAtkDmg = 0.0f;
                // 完全格檔
                pDefer.AddStates(_FIGHTSTATE._BLOCK);
            }
        }

        //玩家秒殺模式- cheat
        if (Config.KILL_MODE) {
            if (pAtker.eCampID == _CAMP._PLAYER) {
                fAtkDmg *= 100.0f;
            }
        }


        nDefHp -= (int)(fAtkDmg);

        //		cHitResult res = new cHitResult (nAtker, nDefer );
        //		res.AtkHp = nAtkHp;
        //		res.DefHp = nDefHp;


        // normal attack

        if (nDefHp < 0 && ((pDefer.n_HP + pDefer.n_DEF) < Mathf.Abs(nDefHp)))
        {
            if (pAtker.IsStates(_FIGHTSTATE._MERCY) || pDefer.IsTag(_UNITTAG._NODIE))
            {       // 手加減 或 defer is 不死身
                nDefHp = -(pDefer.n_HP + pDefer.n_DEF - 1);
            }
            else
            {
                pAtker.AddStates(_FIGHTSTATE._KILL);
                pDefer.AddStates(_FIGHTSTATE._DEAD);  // dead
            }
        }
        // check parry / block
        //  if (pDefer.IsStates(_FIGHTSTATE._BLOCK))// 格檔
        if (PowDmg > 0 )
        {
            if( PowDmg + fAtkDmg < pDefer.n_DEF )
                pDefer.AddStates(_FIGHTSTATE._PARRY); // 還在防禦值內，算格檔
        }
        else 
        {
            if (fAtkDmg < pDefer.n_DEF)
                pDefer.AddStates(_FIGHTSTATE._PARRY); // 還在防禦值內，算格檔
        }


            resPool.Add ( new cHitResult( cHitResult._TYPE._HP ,nDefer , nDefHp  ) );
	//	resPool.Add ( new cHitResult( cHitResult._TYPE._CP ,nDefer , 1  ) ); // def add 1 cp

		// drain hp / mp
		float fDrainHpRate = pAtker.GetDrainHP();
		float fDrainMpRate = pAtker.GetDrainMP();
		if( (nDefHp < 0) &&  fDrainHpRate > 0.0f ){
			int nDrainHP = -(int)(nDefHp*fDrainHpRate);
			if( nDrainHP != 0 ){
				resPool.Add ( new cHitResult( cHitResult._TYPE._HP ,nAtker , nDrainHP  ) );
			}
		}

		if( fDrainMpRate>0.0f  )
		{
			int nDrainMp = (int)(pDefer.GetMaxMP()*fDrainMpRate);
			if( nDrainMp != 0 ){
				resPool.Add ( new cHitResult( cHitResult._TYPE._MP ,nDefer , -nDrainMp  ) );  	// drain mp
				resPool.Add ( new cHitResult( cHitResult._TYPE._MP ,nAtker , nDrainMp  ) );		// restory mp
			}
		}

		//有傷害的才會造成掉落
		if( BattleManager.Instance != null ){
			BattleManager.Instance.CalDropResult( pAtker , pDefer );

            // 彈死人的要計算掉落
            if (pAtker.IsStates(_FIGHTSTATE._DEAD) ) {
                BattleManager.Instance.CalDropResult(pDefer,pAtker );
            }
		}

		return resPool;
	}
	// cal result of castout hit
	static public List<cHitResult> CalSkillHitResult( cUnitData pAtker , cUnitData pDefer , int nSkillID  )
	{
		//cUnitData pAtker = GameDataManager.Instance.GetUnitDateByIdent( nAtker ); 	//Panel_StageUI.Instance.GetUnitByIdent( nAtker ); 
		if ( (pAtker == null) || (pDefer == null) )
			return null;

        

        // 守方強制迴避- Yoro : 需過濾非攻擊性技能
        bool bIsDamage =  MyTool.IsDamageSkill( nSkillID );
		if( bIsDamage ){
			if (pDefer.IsStates (_FIGHTSTATE._DODGE)) {		
				return null;
			}
		}

		List<cHitResult> resPool = new List<cHitResult> ();
		if (pDefer != null) {
          


            int nRes = 0;

            if (pDefer.IsStates(_FIGHTSTATE._BLOCK))// 格檔
            {
                nRes = (int)_FIGHTSTATE._BLOCK; 
            }
            else if (pDefer.IsStates(_FIGHTSTATE._PARRY))// 招架
            {
                nRes = (int)_FIGHTSTATE._PARRY;
            }            
			resPool.Add (new cHitResult (cHitResult._TYPE._BEHIT, pDefer.n_Ident, nSkillID , nRes )); // for play fx
		}


        // 機關 單位不處理戰鬥效果
        if (pDefer.IsTag(_UNITTAG._TRIGGER))
        {
            return resPool;
        }

        //		MyTool.DoSkillEffect( pAtker , pAtker.FightAttr.HitPool , Skill.s_CAST_TRIG ,  pAtker.FightAttr.HitEffPool , ref resPool  );
        pAtker.DoHitEffect( nSkillID, pDefer , ref resPool );
		if (pDefer != null) {
			pDefer.DoBeHitEffect( pAtker , ref resPool );
			
		}
		//	MyScript.Instance.RunSkillEffect ( pAtker , null , pAtker.FightAttr.Skill.s_HIT_EFFECT , ref resPool ); // bad frame work

		if (pDefer.IsStates (_FIGHTSTATE._DODGE)) {		
			return resPool;
		}
		//--- hit back 
		if ( nSkillID != 0 ) {
			SKILL skl = ConstDataManager.Instance.GetRow<SKILL> ( nSkillID );
			if( skl != null ){
				int nBack = skl.n_HITBACK;
			
				iVec2 vFinal = Panel_StageUI.Instance.SkillHitBack(pAtker.n_Ident , pDefer.n_Ident , nBack  );
				if( vFinal != null )
				{
					resPool.Add( new cHitResult( cHitResult._TYPE._HITBACK, pDefer.n_Ident , vFinal.X , vFinal.Y ) );
				}
			}
		} 
		
		
		//}
		return resPool;
	}

	// Operation Token ID 
	//
	static public bool CanPK( cUnitData  unit1 , cUnitData  unit2 ) 	
	{
		return MyTool.CanPK( unit1.eCampID , unit2.eCampID   );
	}

	static public bool CanPK( _CAMP  camp1 , _CAMP  camp2 ) 	
	{
		return MyTool.CanPK(camp1 , camp2   );
	}

	// 取得技能師法座標的工具 func
	static public void ConvertSkillTargetXY( cUnitData caster , int nSkillID , int nTargetID , ref int nTarX , ref int nTarY )
	{
		if( 0 == nSkillID )
			return ;
		SKILL skill = ConstDataManager.Instance.GetRow<SKILL>(nSkillID); 
		if( skill == null )
			return ;



		switch( skill.n_TARGET ){
			case 0:	//0→對自己施展
				{	
				nTarX = caster.n_X;
				nTarY = caster.n_Y;
				//	nTargetIdent = nAtkIdent; // will cause ( atker == defer )
				}				break;
			case 6:	//6→自我AOE我方
			case 7:	//7→自我AOE敵方
			case 8:	//8→自我AOEALL
				{
				
				nTarX = caster.n_X;
				nTarY = caster.n_Y;
				}				break;
			case 1:	//→需要敵方目標
			case 2:	//→需要友方目標
			{
				cUnitData Target = GameDataManager.Instance.GetUnitDateByIdent( nTargetID );
				if( Target != null ){
					nTarX = Target.n_X;
					nTarY = Target.n_Y;
				}else{
					// bug
				Debug.LogErrorFormat( "ConvertSkillTargetXY on null target{0},skill{1},x{2},y{3} " ,nTargetID,nSkillID, nTarX , nTarY );	
				}
			}
				break;
			case 3:	//→MAP敵方
			case 4: //→MAP我方
			case 5:	//→MAPALL
			{
				//  mob counter 防呆
				cUnitData Target = GameDataManager.Instance.GetUnitDateByIdent( nTargetID );
				if( Target != null ){
					nTarX = Target.n_X;
					nTarY = Target.n_Y;
				}else{
					// bug
					if( nTargetID > 0 ){
						Debug.LogErrorFormat( "ConvertSkillTargetXY on null target{0},skill{1},x{2},y{3} " ,nTargetID,nSkillID, nTarX , nTarY );	
					}
				}
			}
				break;
		}
		
	}

};

