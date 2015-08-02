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
		_HIT		,		// skill hit enemy
		_BEHIT		,		// 被
		_HITBACK	,

		_DODGE		,		// 迴避


	};


	public cHitResult( _TYPE type , int ident , int value1 =0,int value2=0,int value3=0,int value4=0 )
	{
		eHitType = type;
		Ident = ident; 
		Value1 = value1;
		Value2 = value2;
		Value3 = value3;
		Value3 = value4;
	}
//	public cHitResult( int nAtkIdent , int nDefIdent )
//	{
//		AtkIdent = nAtkIdent;
//		DefIdent = nDefIdent;
//	}

	public _TYPE eHitType{ set; get;}

	public int Ident{ set; get;}
	public int SkillID{ set; get;}
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
		//
		//Panel_unit uDefer = Panel_StageUI.Instance.GetUnitByIdent( nDeferID ); 

		switch (nPhase) {
		case 0:	// prepare for event check


			// open CMD UI for def player
			if ( Defer.eCampID  == _CAMP._PLAYER) {
				// set open CMD 
				// need set a range
				Panel_CMDUnitUI.OpenCMDUI( _CMD_TYPE._COUNTER , nDeferID  , nAtkerID );
				
			}
			else{
				// mob need a method to get skill
				eDefCmdID = _CMD_ID._COUNTER; // mob select counter this time
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
					
					// init fight attr each time
					Atker.SetFightAttr( nDeferID , nAtkerSkillID );
					Defer.SetFightAttr( nAtkerID , nDeferSkillID );
					
					
					// set batle state
					Atker.AddStates( _UNITSTATE._ATKER );
					
					//Defer.AddStates( _UNITSTATE._DEFER );
					
					if( IsDefMode() ){
						Defer.AddStates( _UNITSTATE._DEFMODE );
					}
					
					
					
					
					// atk start cast action
					uAction pCastingAction = ActionManager.Instance.CreateCastAction( nAtkerID, nAtkerSkillID  );
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
			uAction pCastingAction = ActionManager.Instance.CreateCastAction( nDeferID , nDeferSkillID  );
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
			uAction pAtkAction = ActionManager.Instance.CreateAttackAction(nAtkerID,nDeferID,nAtkerSkillID  );
			if( pAtkAction != null )
			{

				pAtkAction.AddHitResult(  CalAttackResult( nAtkerID , nDeferID , IsDefMode() ) ) ;
				pAtkAction.AddHitResult( CalSkillHitResult(  Atker , Defer  , nAtkerSkillID ) );


				int nTarX = this.nTarGridX;
				int nTarY = this.nTarGridY;
				if( Defer != null ){
					nTarX = Defer.n_X;
					nTarY = Defer.n_Y;
				}

				// get affectpool
				GetAffectPool( Atker , nDeferID , Atker.FightAttr.SkillID , nTarX , nTarY , ref AtkAffectPool);
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

					pAtkAction.AddHitResult(  CalAttackResult( nAtkerID , unit.n_Ident ) ) ;
					//if( nAtkerSkillID > 0 ){
					pAtkAction.AddHitResult( CalSkillHitResult(  Atker , unit  , nAtkerSkillID ) );
					//}
					//Atker.Buffs.OnHit( unit , ref pAtkAction.HitResult );
				}
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
				if( Defer.IsStates( _UNITSTATE._DEAD ) == false )
				{
					if( iVec2.Dist(Atker.n_X,Atker.n_Y,Defer.n_X,Defer.n_Y ) <= nRange ){
						bCanCounter = true;
					}
				}
			}
			// 被打死不能反擊
			if( Defer.IsStates(_UNITSTATE._DEAD)  ){
				bCanCounter = false;
			}

			nPhase++;
			break;
		case 6:
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
			if ( bCanCounter ) {

				uAction pCountAct = ActionManager.Instance.CreateAttackAction (nDeferID,nAtkerID, nDeferSkillID );

				if (pCountAct != null) {
					pCountAct.AddHitResult( CalAttackResult( nDeferID , nAtkerID , false ) ); // must not def at this time
					pCountAct.AddHitResult( CalSkillHitResult( Defer,  Atker , nDeferSkillID ) );
//					Defer.Buffs.OnHit( Atker , ref pCountAct.HitResult );
				}

				GetAffectPool( Defer , nAtkerID , Defer.FightAttr.SkillID , 0 , 0 , ref DefAffectPool );
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
					
					pCountAct.AddHitResult(  CalAttackResult( nDeferID , unit.n_Ident ) ) ;

					//if( nDeferSkillID > 0 ){
					pCountAct.AddHitResult( CalSkillHitResult( Defer,  unit , nDeferSkillID ) );
					//}
					//Defer.Buffs.OnHit( unit , ref pCountAct.HitResult );
				}

				//				Panel_unit unitDef = Panel_StageUI.Instance.GetUnitByIdent( nDeferID );
				//				if( unitDef != null )
				//				{
				//					unitDef.ActionAttack( nAtkerID );
				//				}
			}
			nPhase++;
			break;
		case 9: 	// fight end . show exp
			
			nPhase++;
			break;
		case 10:			// close all 
			nPhase++;
			// add Exp / Money  action
			if ( Atker.eCampID  == _CAMP._PLAYER) {
				if( Atker == null ){
					Debug.Log( "atker is dead");
				}

				int nExp=0;
				int nMoney=0;

				CalDropResult( Atker , Defer , ref nExp , ref nMoney );
				foreach( cUnitData unit in AtkAffectPool ){
					CalDropResult( Atker , unit , ref nExp ,ref nMoney );
				}
				// drop rate on final value
				float fmuldrop = 1.0f + Atker.GetMulDrop();
				nMoney = (int)(nMoney*fmuldrop);
				if( nMoney < 0) nMoney = 0;
				nExp = (int)(nExp*fmuldrop);
				if( nExp < 0) nExp = 0;

				nDropMoney += nMoney;
				nDropExpPool.Add( Atker.n_Ident , nExp );
				//ActionManager.Instance.CreateDropAction( Atker.n_Ident , nExp , nMoney );

			}

			if ( Defer.eCampID  == _CAMP._PLAYER) {
				int nExp=0;
				int nMoney=0;
				if( Defer == null ){
					Debug.Log( "def is dead");
				}


				CalDropResult( Defer, Atker , ref nExp ,ref  nMoney );
				foreach( cUnitData unit in DefAffectPool ){
					CalDropResult( Defer , unit , ref nExp ,ref nMoney );
				}
				float fmuldrop = 1.0f + Defer.GetMulDrop();
				nMoney = (int)(nMoney*fmuldrop);
				if( nMoney < 0) nMoney = 0;
				nExp = (int)(nExp*fmuldrop);
				if( nExp < 0) nExp = 0;

				nDropMoney += nMoney;
				nDropExpPool.Add( Defer.n_Ident , nExp );
				//ActionManager.Instance.CreateDropAction( Defer.n_Ident , nExp , nMoney );

			}

			// Fight Finish

			if( (Defer!=null) && (Defer!=Atker) ){
				Defer.FightEnd();
			}
			
			foreach( cUnitData unit in AtkAffectPool )
			{
				if( (unit!=Atker) && (unit!=Defer) )
					unit.FightEnd();				
			}
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
			Atker.SetFightAttr (nDeferID, nAtkerSkillID);
			Atker.AddStates (_UNITSTATE._ATKER);
			uAction pCastingAction = ActionManager.Instance.CreateCastAction (nAtkerID, nAtkerSkillID);// Casting
			// skill attr
			if (pCastingAction != null) {
	//			Atker.DoSkillCastEffect( ref pCastingAction.HitResult );
				//MyTool.DoSkillEffect( Atker , Atker.FightAttr.CastPool , Atker.FightAttr.Skill.s_CAST_TRIG ,  Atker.FightAttr.CastEffPool , ref pCastingAction.HitResult  );
			}
			
			nPhase++;
			break;
		case 1:			// hit
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
				GetAffectPool( Atker , nDeferID , Atker.FightAttr.SkillID , nTarX , nTarY , ref AtkAffectPool);
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
				if ( Atker.eCampID  == _CAMP._PLAYER) {
					if( Atker == null ){
						Debug.Log( "atker is dead");
					}
					
					if( bIsDamage ){
						int nExp=0;
						int nMoney=0;
						CalDropResult( Atker , Defer , ref nExp , ref nMoney );
						foreach( cUnitData unit in AtkAffectPool ){
							CalDropResult( Atker , unit , ref nExp ,ref nMoney );
						}
						// drop rate on final value
						float fmuldrop = 1.0f + Atker.GetMulDrop();
						nMoney = (int)(nMoney*fmuldrop);
						if( nMoney < 0) nMoney = 0;
						nExp = (int)(nExp*fmuldrop);
						if( nExp < 0) nExp = 0;
						
						nDropMoney += nMoney;
						nDropExpPool.Add( Atker.n_Ident , nExp );
						//ActionManager.Instance.CreateDropAction( Atker.n_Ident , nExp , nMoney );
					}
				}
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
			Atker.FightEnd( true );

			// cmd finish
			
			// action finish in atk action
			Clear ();
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

	public void ShowBattleFX( int nIdent , string fx )
	{
		Panel_unit unit = Panel_StageUI.Instance.GetUnitByIdent( nIdent ); 
		if (unit == null)
			return;

		//string path = "FX/Cartoon FX/" + fx;
		//GameObject go = ResourcesManager.CreatePrefabGameObj ( unit.gameObject , fx );
		GameObject go = GameSystem.PlayFX ( unit.gameObject , fx  );
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


			UILabel lbl = go.GetComponent< UILabel >();
			if( lbl )
			{
				if( nValue > 0 )
				{
					// heal 
					
					lbl.gradientTop = new Color( 0.0f, 1.0f , 0.0f );
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

	public void ShowAtkAssist( int nAtkIdent ,  int nDefIdent )
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
			if( CanPK( pAtker.eCampID , pair.Value.eCampID )== false )
			{
				  
				if( iVec2.Dist(  pDefer.n_X , pDefer.n_Y , pair.Value.n_X , pair.Value.n_Y   ) >1 )
				{
					continue;
				}

				// addto cc pool?
				ShowBattleMsg( pair.Key , "助攻");

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

			if( CanPK( pDefer.eCampID , pair.Value.eCampID )== false )
			{
				
				if( iVec2.Dist(  pDefer.n_X , pDefer.n_Y , pair.Value.n_X , pair.Value.n_Y   ) >1 )
				{
					continue;
				}
				
				// addto cc pool?
				if( bShow == true ){
					ShowBattleMsg( pair.Key , "協防");
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

	public void GetAffectPool( cUnitData Atker , int nDefer , int SkillID , int nTarX , int nTarY , ref List< cUnitData> pool )
	{
		// don't push defer to affect pool

		SKILL skl = ConstDataManager.Instance.GetRow< SKILL > ( SkillID );
		if (skl == null) {
			return;
		}
		if (skl.n_AREA == 0)
			return;

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
		if ((skl.n_TARGET == 1) || (skl.n_TARGET == 3)) {
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
					pool.Add( pUnit );
					continue;
				}


				if(  CanPK( Atker.eCampID , pUnit.eCampID ) == bCanPK ){
					pool.Add( pUnit );
				}
			}
		}

	}

	public void CalDropResult( cUnitData Atker , cUnitData Defer , ref int nExp , ref int nMoney )
	{
		if( Atker == null ) return ;
		int exp 	= 1; // base exp
		int money 	= 0;
		if( Defer != null ){
			int nDiffLv = Defer.n_Lv-Atker.n_Lv;
			if( nDiffLv > 3  ){
				exp +=3;
			}
			else if( nDiffLv > -3  ){
				exp +=1;
			}
			//
			// kill
			if( Defer.IsStates( _UNITSTATE._DEAD ) ){

				exp = (exp*4)+20;
				money  = 1000;

				// check drop item
				if( Defer.cCharData.n_ITEM_DROP  > 0 ){
					nDropItemPool.Add( Defer.cCharData.n_ITEM_DROP );
				}

			}
		}
		nExp 	+= exp ; 
		nMoney 	+= money;



	}

	// cal result of castout hit
	public List<cHitResult> CalSkillHitResult( cUnitData pAtker , cUnitData pDefer , int nSkillID  )
	{
		//cUnitData pAtker = GameDataManager.Instance.GetUnitDateByIdent( nAtker ); 	//Panel_StageUI.Instance.GetUnitByIdent( nAtker ); 
		if ( (pAtker == null) || (pDefer == null) )
			return null;
		List<cHitResult> resPool = new List<cHitResult> ();
		//resPool.Add ( new cHitResult( cHitResult._TYPE._HIT ,pAtker   ) ); // not a really hit
	//	SKILL Skill = pAtker.FightAttr.SkillData.skill;

	//	if (Skill.n_TARGET == 0 ) { // self cast a buff
			// hit result

//		MyTool.DoSkillEffect( pAtker , pAtker.FightAttr.HitPool , Skill.s_CAST_TRIG ,  pAtker.FightAttr.HitEffPool , ref resPool  );
		pAtker.DoHitEffect(pDefer , ref resPool );
		if (pDefer != null) {
			pDefer.DoBeHitEffect( pAtker , ref resPool );

		}
		//	MyScript.Instance.RunSkillEffect ( pAtker , null , pAtker.FightAttr.Skill.s_HIT_EFFECT , ref resPool ); // bad frame work


		//}
		return resPool;
	}

	public List<cHitResult> CalAttackResult( int nAtker , int nDefer , bool bDefMode = false )
	{
		cUnitData pAtker = GameDataManager.Instance.GetUnitDateByIdent( nAtker ); 	//Panel_StageUI.Instance.GetUnitByIdent( nAtker ); 
		cUnitData pDefer = GameDataManager.Instance.GetUnitDateByIdent( nDefer );	//Panel_StageUI.Instance.GetUnitByIdent( nDefer ); 
		if ( (pAtker == null) || (pDefer == null) )
			return null;

		pAtker.AddStates (_UNITSTATE._DAMAGE);
		if (bDefMode == true) {
			pDefer.AddStates( _UNITSTATE._DEFMODE );
		}
		// create result pool

		List<cHitResult> resPool = new List<cHitResult> ();
		resPool.Add ( new cHitResult( cHitResult._TYPE._HIT ,nAtker , nDefer  ) );

		if (pDefer.IsStates (_UNITSTATE._DODGE)) {
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
					if (pDefer.IsStates (_UNITSTATE._MERCY)) {
						nAtkHp = -(pAtker.n_HP + pAtker.n_DEF-1);
					}
					else {
						pDefer.AddStates (_UNITSTATE._KILL);
						pAtker.AddStates (_UNITSTATE._DEAD);
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
			if (pAtker.IsStates (_UNITSTATE._MERCY)) {
				// 手加減
				nDefHp = -(pDefer.n_HP+pDefer.n_DEF-1);
			}
			else {
				pAtker.AddStates( _UNITSTATE._KILL );
				pDefer.AddStates( _UNITSTATE._DEAD );  // dead
			}
		}

		resPool.Add ( new cHitResult( cHitResult._TYPE._HP ,nDefer , nDefHp  ) );
		resPool.Add ( new cHitResult( cHitResult._TYPE._CP ,nDefer , 1  ) ); // def add 1 cp
		// Skill Hit spec Effect
//		MyScript.Instance.RunSkillEffect ( pAtker , pDefer, pAtker.FightAttr.Skill.s_HIT_EFFECT , ref resPool ); // bad frame work
	//	pAtker.DoSkillHitEffect ( pDefer , ref resPool );
	//	MyTool.DoSkillEffect ( pAtker , pAtker.FightAttr.HitPool , pAtker.FightAttr.Skill.s_HIT_TRIG ,  pAtker.FightAttr.HitEffPool , ref resPool  );

		return resPool;
	}

	// Operation Token ID 
	//
	public bool CanPK( _CAMP  camp1 , _CAMP  camp2 ) 	
	{
		return MyTool.CanPK(camp1 , camp2   );
//		if (camp1 != camp2 ) {
//			if( camp1 == _CAMP._ENEMY || camp2 == _CAMP._ENEMY )
//			{
//				return true;
//			}
//		}
//		
//		return false;
	}



};

