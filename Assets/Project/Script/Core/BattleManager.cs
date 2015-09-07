using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
//using _SRW;
using MYGRIDS;
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

		_ADDBUFF	,
		_DELBUFF	,

		_CAST		,		// cast skill
		_CASTOUT	,		// cast out skill
		//_HIT		,		// skill hit enemy
		_BEHIT		,		// 被

		_HITBACK	,

		_DODGE		,		// 迴避


	};


	public cHitResult( _TYPE type , int ident ,int value1 =0,int value2=0,int value3=0,int value4=0 )
	{
		eHitType = type;
		Ident = ident; 
	//	SkillID = skillid;
		Value1 = value1;
		Value2 = value2;
		Value3 = value3;
		Value4 = value4;
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


};

public partial class BattleManager
{
	//public delegate AudioClip GetAudioClipDelegate (string audioPath);

	private bool hadInit;
	public bool HadInit{ get{ return hadInit; } }



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

	public bool IsBattlePhase()
	{
		if (eBattleType != _BATTLE._NONE)
			return true;

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
		// 因為事件的觸發～ 可能讓 atker / defer 消失。需有配套
		if (Atker == null || Defer == null) {
			nPhase = 10; // 戰鬥被中斷，直接結束
			Debug.Log( " null unit when RunAttack" );
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
				int nSklId = MobAI.SelSkill( Defer , Atker ,true  );
				if( nSklId < 0 ){
					eDefCmdID = _CMD_ID._DEF;  //select defence
				}
				else{
					nDeferSkillID = nSklId;		// counter skill
				}
			}
			
			nPhase++;
			break;
		case 1:			// Casting 
			// have set a cmd and ui is closed

				if( PanelManager.Instance.CheckUIIsOpening( Panel_CMDUnitUI.Name ) == false )
				{
					// special process for def is player
					if ( Defer.eCampID  == _CAMP._PLAYER  ) {

						// check to reopen counter CMD UI	
						if( !IsCounterMode() && !IsDefMode() ){
							if( PanelManager.Instance.CheckUIIsOpening(  Panel_Skill.Name  )== false  ){ // don't pop if skill ui is opening
							Panel_CMDUnitUI.OpenCMDUI( _CMD_TYPE._COUNTER , nDeferID  , nAtkerID );
							}
							return; // break when counter don't start
						}
					}
					//ShowBattleMsg( nAtkerID , "attack" );
					// change defer skill if deence								 
					
					// init fight attr each time
					Atker.SetFightAttr( nDeferID , nAtkerSkillID );
					Defer.SetFightAttr( nAtkerID , nDeferSkillID );

					// set batle state
					Atker.AddStates( _FIGHTSTATE._ATKER );
					
					//Defer.AddStates( _FIGHTSTATE._DEFER );
					
					if( IsDefMode() ){
						Defer.AddStates( _FIGHTSTATE._DEFMODE );
						nDeferSkillID = Config.sysDefSkillID;
					}
					
					// atk start cast action
					uAction pCastingAction = ActionManager.Instance.CreateCastAction( nAtkerID, nAtkerSkillID ,nDeferID , nTarGridX , nTarGridY );
					// skill attr
					if( pCastingAction != null )
					{
						//					Atker.DoSkillCastEffect( ref pCastingAction.HitResult  );
						//					if(  Atker.FightAttr.Skill != null )
						//						MyTool.DoSkillEffect( Atker , Atker.FightAttr.HitPool , Atker.FightAttr.Skill.s_CAST_TRIG ,  Atker.FightAttr.HitEffPool , ref pCastingAction.HitResult  );
					}
					
					//				if ( Atker.FightAttr.Skill != null ) {
					//					//Atker.FightAttr.Skill = ConstDataManager.Instance.GetRow< SKILL > (nAtkerSkillID);
					//					if( MyScript.Instance.CheckSkillEffect( Atker , Atker.FightAttr.Skill.s_CAST_TRIG ) == true ) {
					//						
					//						MyScript.Instance.RunSkillEffect( Atker , Defer , Atker.FightAttr.Skill.s_CAST_EFFECT , ref pCastingAction.HitResult );
					//					}
					//				}
					
					
					
					
					nPhase++;
				}


			//}
			//else
	//	{
//				// check to reopen counter CMD UI
//				if( PanelManager.Instance.CheckUIIsOpening(  Panel_CMDUnitUI.Name )== false ){
//
//					if( PanelManager.Instance.CheckUIIsOpening(  Panel_Skill.Name  )== false  ){ // don't pop if skill ui is opening
//						Panel_CMDUnitUI.OpenCMDUI( _CMD_TYPE._COUNTER , nDeferID  );
//					}
//				}
		//	}




			break;
		case 2:			// def casting
			uAction pCastingAction = ActionManager.Instance.CreateCastAction( nDeferID , nDeferSkillID , nAtkerID  );
			if( pCastingAction != null )
			{
//				Defer.DoSkillCastEffect( ref pCastingAction.HitResult  );
//				if(  Defer.FightAttr.Skill != null )
//					MyTool.DoSkillEffect( Defer , Defer.FightAttr.HitPool , Defer.FightAttr.Skill.s_CAST_TRIG ,  Defer.FightAttr.HitEffPool , ref pCastingAction.HitResult  );
			}

			//ShowBattleMsg( nDeferID , "counter" );
			// show assist
			nPhase++;
			break;
		case 3:			// atack assist pre show 
			ShowAtkAssist( nAtkerID,nDeferID );
			nPhase++;
			break;
		case 4:			// def assist pre show 
			ShowDefAssist( nDeferID );
			nPhase++;
			break;
		case 5:			// atk -> def 
			Panel_StageUI.Instance.FadeOutAVGObj();
			//
			uAction pAtkAction = ActionManager.Instance.CreateAttackAction(nAtkerID,nDeferID,nAtkerSkillID  );
			if( pAtkAction != null )
			{
				pAtkAction.AddHitResult(  CalAttackResult( nAtkerID , nDeferID , IsDefMode() ) ) ;
				pAtkAction.AddHitResult( CalSkillHitResult(  Atker , Defer  , nAtkerSkillID ) );

	//			CalDropResult( Atker , Defer );
				//獎勵計算要提早
				//int nTarX = this.nTarGridX;
				//int nTarY = this.nTarGridY;

				// get affectpool
				if (Atker.IsStates( _FIGHTSTATE._THROUGH ) ) {
					GetThroughPool( Atker , Defer ,Atker.FightAttr.SkillID , 0 , 0 , ref AtkAffectPool );
				}

				GetAffectPool( Atker , Defer , Atker.FightAttr.SkillID , 0 , 0 , ref AtkAffectPool );
				foreach( cUnitData unit in AtkAffectPool )
				{
					//=====================
					// checked if this cUnitData can Atk
					// already check in get affect pool
					//if( CanPK( Atker.eCampID , unit.eCampID ) == false )
					//	continue;
					if( Atker != unit ){
						unit.SetFightAttr( Atker.n_Ident , 0 );
					}
					ShowDefAssist( unit.n_Ident , false );

					pAtkAction.AddHitResult(  CalAttackResult( nAtkerID , unit.n_Ident , true) ) ;
					//if( nAtkerSkillID > 0 ){
					pAtkAction.AddHitResult( CalSkillHitResult(  Atker , unit  , nAtkerSkillID ) );

//					CalDropResult( Atker , unit );
					//}
					//Atker.Buffs.OnHit( unit , ref pAtkAction.HitResult );
				}
				//=========================
			//	Debug.LogFormat( "atk charid{0}, skill{1} , aff{2}", Atker.n_CharID  , nAtkerSkillID ,  AtkAffectPool.Count );
			}
			//should cal atk hit result for performance
			//			Panel_unit unitAtk = Panel_StageUI.Instance.GetUnitByIdent( nAtkerID );
			//			if( unitAtk != null )
			//			{
			//				unitAtk.ActionAttack( nDeferID );
			//			}

			// check if can counter at this time
			if (IsDefMode() == false ){	
				// check range
				int nRange =1; // default range
				SKILL defskill = ConstDataManager.Instance.GetRow<SKILL>( nDeferSkillID );
				if(  defskill != null ){
					nRange = defskill.n_RANGE;
				}
				if( Defer.IsStates( _FIGHTSTATE._DEAD ) == false )
				{
					if( iVec2.Dist(Atker.n_X,Atker.n_Y,Defer.n_X,Defer.n_Y ) <= nRange ){
						bCanCounter = true;
					}
				}
			}
			// 被打死不能反擊
			if( Defer.IsStates(_FIGHTSTATE._DEAD)  ){
				bCanCounter = false;
			}

			nPhase++;
			break;
		case 6:
			Panel_StageUI.Instance.ClearAVGObj();
			if (bCanCounter){
				ShowAtkAssist( nDeferID , nAtkerID );
			}
			nPhase++;
			break;
		case 7:
			if (bCanCounter ){
				ShowDefAssist( nAtkerID );
			}
			nPhase++;
			break;

		case 8:			//  def -> atk
			Panel_StageUI.Instance.FadeOutAVGObj();
			if ( bCanCounter ) {

				uAction pCountAct = ActionManager.Instance.CreateAttackAction (nDeferID,nAtkerID, nDeferSkillID );

				if (pCountAct != null) {
					pCountAct.AddHitResult( CalAttackResult( nDeferID , nAtkerID , false ) ); // must not def at this time
					pCountAct.AddHitResult( CalSkillHitResult( Defer,  Atker , nDeferSkillID ) );
//					CalDropResult( Defer , Atker );
//					Defer.Buffs.OnHit( Atker , ref pCountAct.HitResult );
				}
				if (Defer.IsStates( _FIGHTSTATE._THROUGH ) ) {
					GetThroughPool( Defer , Atker ,Defer.FightAttr.SkillID , 0 , 0 , ref AtkAffectPool );
				}
				GetAffectPool( Defer , Atker , Defer.FightAttr.SkillID , 0 , 0 , ref DefAffectPool );
				foreach( cUnitData unit in DefAffectPool )
				{
					//=====================
					// checked if this cUnitData can Atk
					//if( CanPK( Defer.eCampID , unit.eCampID ) == false )
					//	continue;
					if( unit !=Defer){
						unit.SetFightAttr( Defer.n_Ident  , 0 );
					}

					ShowDefAssist( unit.n_Ident , false );
					
					pCountAct.AddHitResult(  CalAttackResult( nDeferID , unit.n_Ident , true ) ) ; // always def for aoe affect

					//if( nDeferSkillID > 0 ){
					pCountAct.AddHitResult( CalSkillHitResult( Defer,  unit , nDeferSkillID ) );

					//CalDropResult( Defer , unit );
					//}
					//Defer.Buffs.OnHit( unit , ref pCountAct.HitResult );
				}

				//====
				//Debug.LogFormat( "def charid{0}, skill{1} , aff{2}",Defer.n_CharID  , nDeferSkillID ,  DefAffectPool.Count );


			}
			nPhase++;
			break;
		case 9: 	// 結算獎勵
			
//			if ( Atker.eCampID  == _CAMP._PLAYER) {
//				if( Atker == null ){
//					Debug.Log( "atker is dead");
//				}
//				
//				int nExp=0;
//				int nMoney=0;
//				
//				CalDropResult( Atker , Defer , ref nExp , ref nMoney );
//				foreach( cUnitData unit in AtkAffectPool ){
//					CalDropResult( Atker , unit , ref nExp ,ref nMoney );
//				}
//				// drop rate on final value
//				float fmuldrop = 1.0f + Atker.GetMulDrop();
//				nMoney = (int)(nMoney*fmuldrop);
//				if( nMoney < 0) nMoney = 0;
//				nExp = (int)(nExp*fmuldrop);
//				if( nExp < 0) nExp = 0;
//				
//				nDropMoney += nMoney;
//				nDropExpPool.Add( Atker.n_Ident , nExp );
//				//ActionManager.Instance.CreateDropAction( Atker.n_Ident , nExp , nMoney );
//				
//			}
//			
//			
//			if ( Defer.eCampID  == _CAMP._PLAYER) {
//				int nExp=0;
//				int nMoney=0;
//				if( Defer == null ){
//					Debug.Log( "def is dead");
//				}
//				
//				
//				CalDropResult( Defer, Atker , ref nExp ,ref  nMoney );
//				foreach( cUnitData unit in DefAffectPool ){
//					CalDropResult( Defer , unit , ref nExp ,ref nMoney );
//				}
//				float fmuldrop = 1.0f + Defer.GetMulDrop();
//				nMoney = (int)(nMoney*fmuldrop);
//				if( nMoney < 0) nMoney = 0;
//				nExp = (int)(nExp*fmuldrop);
//				if( nExp < 0) nExp = 0;
//				
//				nDropMoney += nMoney;
//				nDropExpPool.Add( Defer.n_Ident , nExp );
//				//ActionManager.Instance.CreateDropAction( Defer.n_Ident , nExp , nMoney );
//				
//			}

			nPhase++;
			break;
		case 10:			// close all 
			nPhase++;
			// add Exp / Money  action

			// Fight Finish

			if( (Defer!=null) && (Defer!=Atker) ){
				Defer.FightEnd();
			}
			
			foreach( cUnitData unit in AtkAffectPool )
			{
				if( (unit!=Atker) && (unit!=Defer) )
					unit.FightEnd();				
			}
			// atker clear at last
			if( Atker != null )
				Atker.FightEnd( true );


			// cmd finish
			
			// action finish in atk action
			//		StageUnitActionFinishEvent cmd = new StageUnitActionFinishEvent ();
			//		cmd.nIdent = nAtkerID;
			//		GameEventManager.DispatchEvent ( cmd );
			
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
			uAction pAct = ActionManager.Instance.CreateHitAction ( nAtkerID, nTarGridX , nTarGridY , nAtkerSkillID );
			if( pAct != null )
			{
				//必須先取得影響人數
				int nTarX = this.nTarGridX;
				int nTarY = this.nTarGridY;
			
				

				// get affectpool
				bool bIsDamage =  MyTool.IsDamageSkill( nAtkerSkillID );
				//IsDamageSkill
				if( Defer != null )
				{
					if( bIsDamage ){
						Defer.SetFightAttr(  nAtkerID  , 0 );
						ShowDefAssist( Defer.n_Ident , false );
						pAct.AddHitResult( CalAttackResult( nAtkerID , Defer.n_Ident , true ) ); // always def at this time

					}
					pAct.AddHitResult(  CalSkillHitResult( Atker, Defer , nAtkerSkillID ) ) ;
					//Atker.Buffs.OnHit( Defer , ref pAct.HitResult );
				}

				// Affect pool
				if (Atker.IsStates( _FIGHTSTATE._THROUGH ) ) {
					if( bIsDamage ){
						GetThroughPool( Atker , Defer ,Atker.FightAttr.SkillID , 0 , 0 , ref AtkAffectPool );
					}
				}

				GetAffectPool( Atker , Defer , Atker.FightAttr.SkillID , nTarX , nTarY , ref AtkAffectPool );
				foreach( cUnitData unit in AtkAffectPool )
				{

					if( unit == Defer  )
						continue;
					//=====================
					// checked if this cUnitData can Atk
					//if( CanPK( Atker.eCampID , unit.eCampID ) == false )
					//	continue;
					if( bIsDamage ){
						unit.SetFightAttr(  nAtkerID  , 0 );
						ShowDefAssist( unit.n_Ident , false );
						pAct.AddHitResult( CalAttackResult( nAtkerID , unit.n_Ident , true ) );
					}
					pAct.AddHitResult(  CalSkillHitResult( Atker, unit , nAtkerSkillID ) ) ;
					//Atker.Buffs.OnHit( unit , ref pAct.HitResult );
				}

				//pAct.AddHitResult( CalSkillHitResult( nAtkerID, nTarGridX , nTarGridY , nAtkerSkillID ) );
//				if ( Atker.eCampID  == _CAMP._PLAYER) {
//					if( Atker == null ){
//						Debug.Log( "atker is dead");
//					}
//					
//					if( bIsDamage ){
//						int nExp=0;
//						int nMoney=0;
//						CalDropResult( Atker , Defer , ref nExp , ref nMoney );
//						foreach( cUnitData unit in AtkAffectPool ){
//							CalDropResult( Atker , unit , ref nExp ,ref nMoney );
//						}
//						// drop rate on final value
//						float fmuldrop = 1.0f + Atker.GetMulDrop();
//						nMoney = (int)(nMoney*fmuldrop);
//						if( nMoney < 0) nMoney = 0;
//						nExp = (int)(nExp*fmuldrop);
//						if( nExp < 0) nExp = 0;
//						
//						nDropMoney += nMoney;
//						nDropExpPool.Add( Atker.n_Ident , nExp );
//						//ActionManager.Instance.CreateDropAction( Atker.n_Ident , nExp , nMoney );
//					}
//				}
			}
			nPhase++;
			break;
		case 2:			
			nPhase++;
			break;
		case 3:			// close all 
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
			
			// action finish in atk action
			Clear ();
			Panel_StageUI.Instance.ClearAVGObj();
			break;
		}
	}
	public void RunScript()
	{

	}


	//drop after dead event complete
	public bool ProcessDrop()
	{
		if (nDropMoney == 0 && nDropExpPool.Count == 0) {
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
		}

		foreach( KeyValuePair<int , int> pair in nDropExpPool )
		{
			ActionManager.Instance.CreateDropAction( pair.Key , pair.Value , nDropMoney );
			nDropMoney = 0;
		}
	//	if (nDropMoney > 0) {
	//		ActionManager.Instance.CreateDropAction( 0 , 0 , nDropMoney );
	//	}
		nDropMoney = 0;
		if( nDropExpPool.Count > 0  ){
			nDropExpPool.Clear ();
			return true;
		}
		return false;
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

	}

	//===================================================
	public _BATTLE eBattleType { get; set; } 
//	public bool bIsBattle { get; set; } 
	public bool bPause { get; set; } 

	public _CMD_ID eAtkCmdID{ get; set; } 
	public _CMD_ID eDefCmdID{ get; set; } 

	public int nAtkerID{ get; set; } 
	public int nDeferID{ get; set; } 

//	public bool bDefMode{ get; set; } 

	public int nAtkerSkillID{ get; set;} 
	public int nDeferSkillID{ get; set;  } 

	//public bool bIsDefenceMode { get; set; } 		// 

	public int nTarGridX{ get; set; } 				//
	public int nTarGridY{ get; set; } 				//

	public int nBattleID{ get; set; } 

	public bool bCanCounter{ get; set; }     // def have counter action

	public int nDropMoney{ get; set; } 		// drop money
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



	public void PlayAttack (int nAtkIdent, int nDefIdent, int nSkillID)
	{
		nAtkerID = nAtkIdent;
		nDeferID = nDefIdent;
		nAtkerSkillID = nSkillID ;

		eBattleType = _BATTLE._ATTACK;
	}
	public void PlayCast (int nAtkIdent, int nTarIdent , int nGridX , int nGridY , int nSkillID)
	{
		nAtkerID = nAtkIdent;
		nDeferID = nTarIdent;
		nTarGridX = nGridX;
		nTarGridY = nGridY;

		nAtkerSkillID = nSkillID;
		
		eBattleType = _BATTLE._CAST ;
	}

	public void PlayBattleID (int nBattleID )
	{
		nBattleID = nBattleID;
	}

	// 針對 counter 的立即處理
	public void PlayCounterCast (int nCastIdent, int nTarIdent , int nSkillID )
	{
		cUnitData Caster = GameDataManager.Instance.GetUnitDateByIdent ( nCastIdent );
		cUnitData Target = GameDataManager.Instance.GetUnitDateByIdent ( nTarIdent );

		ActionManager.Instance.CreateCastAction( nCastIdent , nSkillID  , nTarIdent );
		// hit effect
		uAction pAct = ActionManager.Instance.CreateHitAction ( nCastIdent, 0 , 0 , nSkillID );
		if( pAct != null )
		{
			//必須先取得影響人數
			int nTarX = 0;
			int nTarY = 0;
		
			// 只有簡單給 buff 可以執行

			SKILL skl = ConstDataManager.Instance.GetRow<SKILL>( nSkillID );
			if( skl == null )
				return;

			pAct.AddHitResult(  CalSkillHitResult( Caster, Target , nSkillID ) ) ;

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
	

	public void ShowBattleMsg( int nIdent , string msg )
	{
		Panel_unit unit = Panel_StageUI.Instance.GetUnitByIdent( nIdent ); 
		ShowBattleMsg (unit , msg );

	}

	public void ShowBattleMsg( Panel_unit unit , string msg )
	{
		Vector3 v = new Vector3 (0, 0, 0);
		if (unit != null) {
			// show in screen center
			//v = unit.transform.position;
			v = unit.transform.parent.localPosition+unit.transform.localPosition;
		}

		GameObject go = ResourcesManager.CreatePrefabGameObj ( Panel_StageUI.Instance.MaskPanelObj , "prefab/BattleMsg" );
		if (go != null) {
			//go.transform.position = v;
			go.transform.localPosition = v;
			UILabel lbl = go.GetComponentInChildren<UILabel>();
			if( lbl != null )
			{
				lbl.text = msg;
			}
		}

	}

	public void ShowBattleFX( int nIdent , int nFxId )
	{
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
		//nMode : 0 - hp , 1- def , 2 - mp , 3 -sp
		Vector3 v = new Vector3 (0, 0, 0);
		if ( obj != null) {
			// show in screen center
//			Vector3 vp = obj.transform.parent.localPosition;
//			v = obj.transform.localPosition;
			v = obj.transform.parent.localPosition+obj.transform.localPosition;
			//v = obj.transform.position ;
		}
		//Panel_StageUI.Instance.TilePlaneObj.transform.localPosition;


		//GameObject go = ResourcesManager.CreatePrefabGameObj ( Panel_StageUI.Instance.MaskPanelObj , "Prefab/BattleValue" );
		GameObject go = Panel_StageUI.Instance.SpwanBattleValueObj ();

		//string path = "Prefab/BattleValue";
		//GameObject go = ResourcesManager.CreatePrefabGameObj ( obj , path );
		//GameObject go = GameSystem.PlayFX ( unit.gameObject , fx  );
		if (go != null) {
			//go.transform.position = v;
			go.transform.localPosition = v;
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
				if( nValue > 0 )
				{
					if( nMode == 1 ){
						lbl.gradientTop = new Color( 1.0f, 1.0f , 0.0f );
					}
					else {
						lbl.gradientTop = new Color( 0.0f, 1.0f , 0.0f );
					}
				}
				else{ 
					// damage
					lbl.gradientTop = new Color( 1.0f, 0.0f , 0.0f );
				}
				lbl.text = nValue.ToString();
			}
		}
	}

	public void ShowBattleResValue( GameObject obj , string sMsg , int nMode )
	{	
		Vector3 v = new Vector3 (0, 0, 0);
		if ( obj != null) {
			v = obj.transform.parent.localPosition+obj.transform.localPosition;
		}
		GameObject go = Panel_StageUI.Instance.SpwanBattleValueObj ();
		
		if (go != null) {
			//go.transform.position = v;
			go.transform.localPosition = v;

			
			
			UILabel lbl = go.GetComponent< UILabel >();
			if( lbl )
			{

			//	lbl.gradientTop = new Color( 1.0f, 0.0f , 0.0f );

				lbl.text = sMsg;
			}
		}
	}

	public void ShowAtkAssist( int nAtkIdent ,  int nDefIdent , bool bShow = true )
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
			if( pair.Value.IsDead())
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
	
	public void ShowDefAssist( int nDefIdent , bool bShow = true  )
	{
		cUnitData pDefer = GameDataManager.Instance.GetUnitDateByIdent ( nDefIdent );
		if (pDefer == null)
			return;

		pDefer.FightAttr.fDefAssist = 0.0f;

		foreach( KeyValuePair< int , cUnitData> pair in GameDataManager.Instance.UnitPool )
		{
			if( pair.Key == nDefIdent )
				continue;
			if( pair.Value.IsDead() )
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
				if( pool.IndexOf( pUnit )< 0 ){
					pool.Add( pUnit );
				}
			}
			}
		}
	}

	public void GetAffectPool( cUnitData Atker , cUnitData Defer , int SkillID , int nTarX , int nTarY , ref List< cUnitData> pool  )
	{
		// don't push defer to affect pool
		int nDefer = 0;
		SKILL skl = ConstDataManager.Instance.GetRow< SKILL > ( SkillID );
		if (skl == null) {
			return;
		}
		if (skl.n_AREA == 0)
			return;
		if( Defer != null ){
			nTarX = Defer.n_X;
			nTarY = Defer.n_Y;
			nDefer = Defer.n_Ident;
		}

		List < iVec2 > lst = MyTool.GetAOEPool ( nTarX ,nTarY,skl.n_AREA ,Atker.n_X , Atker.n_Y  );

		if( pool == null ){
			pool = new List< cUnitData > ();
		}

		//		0→對自己施展
		//		1→需要敵方目標
		//		2→需要友方目標
		//		3→MAP敵方
		//		4→MAP我方
		//      5-MAP-all 
		bool bCanPK = false;
		bool bAll = false;
		if ((skl.n_TARGET == 1) || (skl.n_TARGET == 3) || (skl.n_TARGET == 7)) {
			bCanPK = true;
		} else if( skl.n_TARGET == 5 ){
			bAll = true;
		}


		// check  if have affect
		foreach (iVec2 v in lst) {
			cUnitData pUnit = GameDataManager.Instance.GetUnitDateByPos( v.X , v.Y );
			if( pUnit != null ){
				// defer don't add . he have a spec process
				if( pUnit.n_Ident == nDefer )  
					continue;
				 
				if( bAll == true ){ // all is no need check
					if( pool.IndexOf( pUnit )< 0 ){
						pool.Add( pUnit );
					}
					continue;
				}

				if(  CanPK( Atker.eCampID , pUnit.eCampID ) == bCanPK ){
					if( pool.IndexOf( pUnit )< 0 ){
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
		if( Atker == null || Defer == null ) return ;
		if( Atker.eCampID != _CAMP._PLAYER  ) return;

		float fdroprate = 1.0f + Atker.GetMulDrop();

		//
		int exp 	= 10; // base exp
		int money 	= 0;
		if( Defer != null ){
			int nDiffLv = Defer.n_Lv-Atker.n_Lv;
			exp += (nDiffLv*2);
			exp = MyTool.ClampInt( exp , 1 , 20 );

			// kill
			if( Defer.IsStates( _FIGHTSTATE._DEAD ) ){

				exp = (exp*3) ;
				money  = 1000 ;

				// check drop item
				if( Defer.cCharData.n_ITEM_DROP  > 0 ){
					nDropItemPool.Add( Defer.cCharData.n_ITEM_DROP );
				}
			}
		}
		// mul drop
		money =  MyTool.ClampInt(  money , 0 ,   (int)(money*fdroprate * Defer.cCharData.f_DROP_EXP) );
		exp   =  MyTool.ClampInt(  exp , 0 ,   (int)(exp*fdroprate * Defer.cCharData.f_DROP_MONEY) );

		// Add to pool
		//nExp 	+= exp ; 
		//nMoney 	+= money;

		//====
		nDropMoney += money;
		if( nDropExpPool.ContainsKey( Atker.n_Ident )  ){
			nDropExpPool[Atker.n_Ident ] += exp;
		}
		else {
			nDropExpPool.Add( Atker.n_Ident , exp );
		}

	}


	public List<cHitResult> CalAttackResult( int nAtker , int nDefer , bool bDefMode = false )
	{
		cUnitData pAtker = GameDataManager.Instance.GetUnitDateByIdent( nAtker ); 	//Panel_StageUI.Instance.GetUnitByIdent( nAtker ); 
		cUnitData pDefer = GameDataManager.Instance.GetUnitDateByIdent( nDefer );	//Panel_StageUI.Instance.GetUnitByIdent( nDefer ); 
		if ( (pAtker == null) || (pDefer == null) )
			return null;

		//標示用，讓後面有判斷依據
		pAtker.AddStates (_FIGHTSTATE._DAMAGE);		// 使用傷害姓技能
		pDefer.AddStates (_FIGHTSTATE._BEDAMAGE);// 被使用傷害姓技能


		if (bDefMode == true) {
			pDefer.AddStates( _FIGHTSTATE._DEFMODE );
		}
		// create result pool

		List<cHitResult> resPool = new List<cHitResult> ();
	//	resPool.Add ( new cHitResult( cHitResult._TYPE._HIT ,nAtker , nDefer  ) );


		// 守方強制迴避
		if (pDefer.IsStates (_FIGHTSTATE._DODGE)) {
			resPool.Add (new cHitResult (cHitResult._TYPE._DODGE, nDefer, 0 ));	
			return resPool;
		}


		// buff effect
		float AtkMarPlus = pAtker.FightAttr.fAtkAssist;   // assist is base pils
		float DefMarPlus = pDefer.FightAttr.fDefAssist;

		int AtkPowPlus = 0;
		int DefPowPlus = 0;
		int AtkPlus = 0;
		int DedfPlus = 0;

		// default 倍率
	//	float fAtkMarFactor = 0.0f;
	//	float fDefMarFactor = 0.0f;
		float fAtkPowFactor = 1.0f;
		float fDefPowFactor = 1.0f;
		float fAtkFactor = 1.0f;
		float fDedFactor = 1.0f;

		//float fAtkBurst  = 1.0f + pAtker.GetMulBurst ();
		//float fDefDamage = 1.0f + pDefer.GetMulDamage ();
		resPool.Add ( new cHitResult( cHitResult._TYPE._BEHIT , nDefer , pAtker.FightAttr.SkillID ) );

		SKILL AtkerSkill = pAtker.FightAttr.SkillData.skill;
		SKILL DeferSkill = pDefer.FightAttr.SkillData.skill;

		// skill effect
		if ( AtkerSkill != null ) {
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

		float AtkMar =  pAtker.GetMar() + AtkMarPlus ;
		float DefMar =  pDefer.GetMar() + DefMarPlus ;

		// 1 mar = 0.5% hit rate
		float HitRate = ((AtkMar-DefMar) + Config.HIT) / 200.0f; // add base rate
		if( HitRate < 0.0f )
			HitRate = 0.0f;


		float AtkPow =  fAtkPowFactor*(pAtker.GetPow() + AtkPowPlus);
		float DefPow =  fDefPowFactor*(pDefer.GetPow() + DefPowPlus);

		int PowDmg = (int)(HitRate*(AtkPow-DefPow) ); // 

		if( PowDmg > 0 ){
			nDefHp -= (int)(PowDmg * pAtker.GetMulBurst() * pDefer.GetMulDamage() );
	
		}
		else if( PowDmg < 0 ){
			nAtkHp += (int)(PowDmg * pDefer.GetMulBurst() * pAtker.GetMulDamage() ); // it is neg value already
			if (nAtkHp != 0) {
				if (nAtkHp < 0 && ((pAtker.n_HP + pAtker.n_DEF) < Math.Abs (nAtkHp))) {
					if (pDefer.IsStates (_FIGHTSTATE._MERCY)) {
						nAtkHp = -(pAtker.n_HP + pAtker.n_DEF-1);
					}
					else {
						pDefer.AddStates (_FIGHTSTATE._KILL);
						pAtker.AddStates (_FIGHTSTATE._DEAD);
					}
				}
				resPool.Add (new cHitResult (cHitResult._TYPE._HP, nAtker, nAtkHp));
			}
		}

		// buff effect
		float Atk = (pAtker.GetAtk() + AtkPlus)* fAtkFactor;
		float DefAC = 0.0f; // armor

		float fAtkDmg 	= (HitRate*Atk) - DefAC  ; 

		fAtkDmg = (fAtkDmg<0)? 0: fAtkDmg;
		if( bDefMode )
		{
			fAtkDmg = (fAtkDmg*Config.DefReduce /100.0f);
		}

		// 加成
		fAtkDmg = fAtkDmg * pAtker.GetMulBurst () * pDefer.GetMulDamage ();

		//玩家秒殺模式- cheat
		if (Config.KILL_MODE ) {
			if( pAtker.eCampID == _CAMP._PLAYER ){
				fAtkDmg *= 100.0f;
			}
		}


		nDefHp -= (int)(fAtkDmg);

//		cHitResult res = new cHitResult (nAtker, nDefer );
//		res.AtkHp = nAtkHp;
//		res.DefHp = nDefHp;


		// normal attack

		if( nDefHp < 0 && ( (pDefer.n_HP+pDefer.n_DEF) < Math.Abs(nDefHp) ) ){
			if (pAtker.IsStates (_FIGHTSTATE._MERCY) || pDefer.IsTag( _UNITTAG._NODIE ) ) {		// 手加減 或 defer is 不死身
				nDefHp = -(pDefer.n_HP+pDefer.n_DEF-1);
			}
			else {
				pAtker.AddStates( _FIGHTSTATE._KILL );
				pDefer.AddStates( _FIGHTSTATE._DEAD );  // dead
			}
		}

		resPool.Add ( new cHitResult( cHitResult._TYPE._HP ,nDefer , nDefHp  ) );
		resPool.Add ( new cHitResult( cHitResult._TYPE._CP ,nDefer , 1  ) ); // def add 1 cp

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
				resPool.Add ( new cHitResult( cHitResult._TYPE._MP ,nAtker , nDrainMp  ) );
			}
		}

		//有傷害的才會造成掉落
		CalDropResult( pAtker , pDefer );

		return resPool;
	}
	// cal result of castout hit
	public List<cHitResult> CalSkillHitResult( cUnitData pAtker , cUnitData pDefer , int nSkillID  )
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
		if (pAtker.FightAttr.SkillID!= 0) {
			int nBack = pAtker.FightAttr.SkillData.skill.n_HITBACK;
			
			iVec2 vFinal = Panel_StageUI.Instance.SkillHitBack(pAtker.n_Ident , pDefer.n_Ident , nBack  );
			if( vFinal != null )
			{
				resPool.Add( new cHitResult( cHitResult._TYPE._HITBACK, pDefer.n_Ident , vFinal.X , vFinal.Y ) );
			}
		} 
		
		
		//}
		return resPool;
	}

	// Operation Token ID 
	//
	public bool CanPK( _CAMP  camp1 , _CAMP  camp2 ) 	
	{
		return MyTool.CanPK(camp1 , camp2   );
	}



};

