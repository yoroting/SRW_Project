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

		_ADDBUFF	,
		_DELBUFF	,

		_CAST		,		// cast skill
		_HIT		,		// skill hit enemy
		_BEHIT		,		// 被
		_HITBACK	,
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
		AtkAffectPool = null;
		DefAffectPool = null;
		AtkCCPool = null;
		DefCCPool = null;

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
				Panel_CMDUnitUI.OpenCMDUI( _CMD_TYPE._COUNTER , nDeferID  );
				
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
								Panel_CMDUnitUI.OpenCMDUI( _CMD_TYPE._COUNTER , nDeferID  );
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
			nPhase++;
			break;
		case 4:			// def assist pre show 
			nPhase++;
			break;
		case 5:			// atk -> def 
			uAction pAtkAction = ActionManager.Instance.CreateAttackAction(nAtkerID,nDeferID,nAtkerSkillID  );
			if( pAtkAction != null )
			{

				pAtkAction.AddHitResult(  CalAttackResult( nAtkerID , nDeferID ) ) ;

				int nTarX = this.nTarGridX;
				int nTarY = this.nTarGridY;
				if( Defer != null ){
					nTarX = Defer.n_X;
					nTarY = Defer.n_Y;

				}


				// get affectpool
				AtkAffectPool = GetAffectPool( Atker , nDeferID , Atker.FightAttr.SkillID , nTarX , nTarY );
				foreach( cUnitData unit in AtkAffectPool )
				{
					//=====================
					// checked if this cUnitData can Atk
					if( CanPK( Atker.eCampID , unit.eCampID ) == false )
						continue;

					unit.SetFightAttr(  unit.n_Ident  , 0 );

					pAtkAction.AddHitResult(  CalAttackResult( nAtkerID , unit.n_Ident ) ) ;
				}
			}
			//should cal atk hit result for performance
			
			
			//			Panel_unit unitAtk = Panel_StageUI.Instance.GetUnitByIdent( nAtkerID );
			//			if( unitAtk != null )
			//			{
			//				unitAtk.ActionAttack( nDeferID );
			//			}
			
			nPhase++;
			break;
		case 6:			//  def -> atk
			if (IsDefMode() == false ) {

				uAction pCountAct = ActionManager.Instance.CreateAttackAction (nDeferID,nAtkerID, nDeferSkillID );

				if (pCountAct != null) {

					pCountAct.AddHitResult( CalAttackResult( nDeferID , nAtkerID , true ) );		
				}
				
				//				Panel_unit unitDef = Panel_StageUI.Instance.GetUnitByIdent( nDeferID );
				//				if( unitDef != null )
				//				{
				//					unitDef.ActionAttack( nAtkerID );
				//				}
			}
			nPhase++;
			break;
		case 7: 	// fight end . show exp
			
			nPhase++;
			break;
		case 8:			// close all 
			nPhase++;

			Atker.FightEnd();
			if( Defer!= null  ){
				Defer.FightEnd();
			}
			foreach( cUnitData unit in AtkAffectPool )
			{
				unit.FightEnd();

			}
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
	public void RunCast()
	{
		cUnitData Atker = GameDataManager.Instance.GetUnitDateByIdent ( nAtkerID );
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
		case 1:			// Castout
			// hit effect
			uAction pAct = ActionManager.Instance.CreateHitAction ( nAtkerID, nTarGridX , nTarGridY , nAtkerSkillID );
			if( pAct != null )
			{
				pAct.AddHitResult( CalSkillHitResult( nAtkerID, nTarGridX , nTarGridY , nAtkerSkillID ) );

			}
			nPhase++;
			break;
		case 2:			// hit
			nPhase++;
			break;
		case 3:			// close all 
			nPhase++;
			Atker.FightEnd();


			// cmd finish
			
			// action finish in atk action
			Clear ();
			break;
		}
	}
	public void RunScript()
	{

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

	//===================================================
//	SKILL	AtkerSkill = null;
//	SKILL	DeferSkill = null;
	private int nPhase = 0; // 
	// AOE
	List< cUnitData > AtkAffectPool = null;
	List< cUnitData > DefAffectPool = null;

	// CC link
	List< cUnitData > AtkCCPool = null;
	List< cUnitData > DefCCPool = null;

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
	public void PlayCast (int nAtkIdent, int nGridX , int nGridY , int nSkillID)
	{
		nAtkerID = nAtkIdent;
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
			v = unit.transform.position;
		}

		GameObject go = ResourcesManager.CreatePrefabGameObj ( Panel_StageUI.Instance.MaskPanelObj , "prefab/BattleMsg" );
		if (go != null) {
			go.transform.position = v;
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

			v = obj.transform.position ;
		}
		
		GameObject go = ResourcesManager.CreatePrefabGameObj ( Panel_StageUI.Instance.MaskPanelObj , "Prefab/BattleValue" );
		//string path = "Prefab/BattleValue";
		//GameObject go = ResourcesManager.CreatePrefabGameObj ( obj , path );
		//GameObject go = GameSystem.PlayFX ( unit.gameObject , fx  );
		if (go != null) {
			go.transform.position = v;
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

	public void ShowAtkAssist( int nAtkIdent ,  int nDefIdent )
	{
		
	}
	
	public void ShowDefAssist( int nAtkIdent ,  int nDefIdent )
	{
		
	}

	// 防禦方選防守
	public bool IsCounterMode()
	{
		return ( eDefCmdID == _CMD_ID._COUNTER );
	}

	public bool IsDefMode()
	{
		return ( eDefCmdID == _CMD_ID._DEF );
	}

	public List< cUnitData > GetAffectPool( cUnitData Atker , int nDefer , int SkillID , int nTarX , int nTarY )
	{
		List< cUnitData > pool = new List< cUnitData > ();
		// check  if have affect



		return pool;
	}

	// cal result of castout hit
	public List<cHitResult> CalSkillHitResult( int nAtker, int  nGridX ,int  nGridY , int nSkillID  )
	{
		cUnitData pAtker = GameDataManager.Instance.GetUnitDateByIdent( nAtker ); 	//Panel_StageUI.Instance.GetUnitByIdent( nAtker ); 
		if ( (pAtker == null)  )
			return null;
		List<cHitResult> resPool = new List<cHitResult> ();
		resPool.Add ( new cHitResult( cHitResult._TYPE._HIT ,nAtker   ) );
		SKILL Skill = pAtker.FightAttr.SkillData.skill;

		if (Skill.n_TARGET == 0 ) { // self cast a buff
			// hit result

//		MyTool.DoSkillEffect( pAtker , pAtker.FightAttr.HitPool , Skill.s_CAST_TRIG ,  pAtker.FightAttr.HitEffPool , ref resPool  );
		pAtker.DoSkillHitEffect ( ref resPool );

		//	MyScript.Instance.RunSkillEffect ( pAtker , null , pAtker.FightAttr.Skill.s_HIT_EFFECT , ref resPool ); // bad frame work


		}
		return resPool;
	}

	public List<cHitResult> CalAttackResult( int nAtker , int nDefer , bool bCounter= false )
	{
		cUnitData pAtker = GameDataManager.Instance.GetUnitDateByIdent( nAtker ); 	//Panel_StageUI.Instance.GetUnitByIdent( nAtker ); 
		cUnitData pDefer = GameDataManager.Instance.GetUnitDateByIdent( nDefer );	//Panel_StageUI.Instance.GetUnitByIdent( nDefer ); 
		if ( (pAtker == null) || (pDefer == null) )
			return null;

		// create result pool

		List<cHitResult> resPool = new List<cHitResult> ();
		resPool.Add ( new cHitResult( cHitResult._TYPE._HIT ,nAtker , nDefer  ) );

		// buff effect
		float AtkMarPlus = 0.0f;
		float DefMarPlus = 0.0f;

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

		float AtkMar =  pAtker.GetMar() + AtkMarPlus;
		float DefMar =  pDefer.GetMar() + DefMarPlus ;

		// 1 mar = 0.5% hit rate
		float HitRate = ((AtkMar-DefMar) + Config.HIT) / 200.0f; // add base rate
		if( HitRate < 0.0f )
			HitRate = 0.0f;


		float AtkPow =  fAtkPowFactor*(pAtker.GetPow() + AtkPowPlus);
		float DefPow =  fDefPowFactor*(pDefer.GetPow() + DefPowPlus);

		int PowDmg = (int)(HitRate*(AtkPow-DefPow)); // 

		if( PowDmg > 0 ){
			nDefHp -= PowDmg;
	
		}
		else if( PowDmg < 0 ){
			nAtkHp = PowDmg; // it is neg value already
	

		}


		// buff effect
		float Atk = (pAtker.GetAtk() + AtkPlus)* fAtkFactor;
		float DefAC = 0.0f; // armor

		float fAtkDmg 	= (HitRate*Atk) - DefAC  ; 



		fAtkDmg = (fAtkDmg<0)? 0: fAtkDmg;
		if( IsDefMode() )
		{
			fAtkDmg = (fAtkDmg*Config.DefReduce /100.0f);
		}

		nDefHp -= (int)(fAtkDmg);

//		cHitResult res = new cHitResult (nAtker, nDefer );
//		res.AtkHp = nAtkHp;
//		res.DefHp = nDefHp;
		if( nAtkHp != 0 )
			resPool.Add ( new cHitResult( cHitResult._TYPE._HP ,nAtker , nAtkHp  ) );

		// 
		resPool.Add ( new cHitResult( cHitResult._TYPE._HP ,nDefer , nDefHp  ) );

		// Skill Hit spec Effect
//		MyScript.Instance.RunSkillEffect ( pAtker , pDefer, pAtker.FightAttr.Skill.s_HIT_EFFECT , ref resPool ); // bad frame work
		pAtker.DoSkillHitEffect ( ref resPool );
	//	MyTool.DoSkillEffect ( pAtker , pAtker.FightAttr.HitPool , pAtker.FightAttr.Skill.s_HIT_TRIG ,  pAtker.FightAttr.HitEffPool , ref resPool  );

		return resPool;
	}

	// Operation Token ID 
	//
	public bool CanPK( _CAMP  camp1 , _CAMP  camp2 ) 	
	{

		if (camp1 != camp2 ) {
			if( camp1 == _CAMP._ENEMY || camp2 == _CAMP._ENEMY )
			{
				return true;
			}
		}
		
		return false;
	}



};

