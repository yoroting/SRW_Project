using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MYGRIDS;
using MyClassLibrary;
//using _SRW;
// maybe don't use this
//public class cAI_CMD{
//	public enum _AI_TYPE
//	{
//		_NONE,
//		_MOVE,
//		_ATK,
//		_DEF,
//		_ABILITY,
//		_WAIT,
//	};
//
//	public int nTarIdent { set; get; }
//	public int nTarX { set; get; }
//	public int nTarY { set; get; }
//	public int nSkillID { set; get; }
//	public int nAbilityID { set; get; }
//
//}


public class Panel_unit : MonoBehaviour {

	public GameObject FaceObj;
	public GameObject HpBarObj;
	public GameObject DefBarObj;
	public GameObject MaskObj;
	public GameObject BGObj;

	public _CAMP 	eCampID;
	public int  	CharID;			// not identift
	public iVec2 Loc;
	public cUnitData pUnitData;  
	uAction CurAction;
	int		nSubActFlow;			// index of action run

	public List< iVec2 > PathList;
	List< iVec2 > pathfind;				// if have findinf path

	iVec2	TarPos;					   //目標左標
	public int TarIdent { set; get ;}  //攻擊對象

//	public int  Identify;		avoid double 
	bool bOnSelected;

	//int nActionTime=1;			

	public bool bIsDead = false;
	public bool bIsAtking  = false;
	public bool bIsCasting = false;
	public bool bIsShaking = false;
	public bool bIsMoving 	= false;
	public bool bIsBorning = false;
	public bool bIsDeading = false;
	public bool bIsLeaving = false;

	public int  Ident() 
	{
		if( pUnitData != null  ){
			return pUnitData.n_Ident;
		}
		return 0;
	}
	public bool CanDoCmd()
	{
		if( pUnitData!= null )
			return pUnitData.nActionTime>0;
		return false;
	}

	// ensure Loc exist
	public Panel_unit()
	{
		Loc 	= new iVec2( 0 , 0 );
		TarPos  = new iVec2( 0 , 0 );

		CharID = 0;
	//	nActionTime = 1;
		bOnSelected = true;
		bIsDead = false;
		TarIdent = 0;

	}

	void OnEnable(){
		transform.localRotation = new Quaternion(); 
		transform.localScale = new Vector3( 1.0f, 1.0f ,1.0f);

		bOnSelected = false;
		bIsDead = false;
		TarIdent = 0;

//		UITweener[] twAry = this.GetComponents< UITweener >();
//		foreach (UITweener tw in twAry) {
//			tw.enabled = false;
//
//		}

//		TweenPosition twpos = this.GetComponent< TweenPosition > ();
//		if (twpos != null) {
//			twpos.enabled = false;
//		}

		// shake
//		TweenShake tws = TweenShake.Begin(gameObject, 1.0f , 15 );
//		if( tws )
//		{
//			tws.shakeX = true;
//			tws.shakeY = true;
//		}
//		
//		//TweenGrayLevel
//		//Vector2 vfrom = new Vector3( 1.0f , 1.0f , 1.0f );
//		//Vector2 vto   = new Vector3( 0.0f , 10.0f, 1.0f );
//		TweenGrayLevel tw = TweenGrayLevel.Begin <TweenGrayLevel >( FaceObj, 1.0f);
//		if (tw) {

		// clear all tween
		UITweener[] tws = GetComponents< UITweener > ();
		foreach (UITweener tw in tws) {
			Destroy( tw );
		}


		//UIEventListener.Get ( this.gameObject ).onClick
//		GameObject instance = ResourcesManager.CreatePrefabGameObj ( this.gameObject ,"FX/Cartoon FX/CFXM4 SmokePuff Ground B" );
//		
//		ParticleSystem ps =instance.GetComponent< ParticleSystem>();
//		if (ps!= null) {
//			
//		}
//		ParticleSystemRenderer psr =instance.GetComponent< ParticleSystemRenderer>();
//		if (psr != null) {
//			psr.sortingLayerName = "FX";
//		}


		SetBorn (); // start born animate
	}

	// Awake
	void Awake(){
		bOnSelected = false;
	//	nActionTime = 1;				// default is 1

		//ParticleSystemRenderer

//		GameObject instance = ResourcesManager.CreatePrefabGameObj ( this.gameObject ,"FX/Cartoon FX/CFXM4 Splash" );
//
//		ParticleSystem ps =instance.GetComponent< ParticleSystem>();
//		if (ps!= null) {
//
//		}
//		ParticleSystemRenderer psr =instance.GetComponent< ParticleSystemRenderer>();
//		if (psr != null) {
//			psr.sortingLayerName = "FX";
//		}
//
//		SetBorn (); // start born animate
		//GameObject instance = Resources.Load ( "/FX/Cartoon FX" );
		//GameObject instance = CFX_SpawnSystem.GetNextObject ( CFX_SpawnSystem.instance.objectsToPreload[3] );
		
	}
	// Use this for initialization
	void Start () {
		// change Texture
	
	}
	
	// Update is called once per frame
	void Update () {

		// STOP UPDATE OTHER ACTION
		if (IsAnimate () == true)
			return;

		// stop update when msg 
		if (BattleMsg.nMsgCount > 0)
			return ;

		// check if need to move
		//if (IsMoveing () == false ) {			// check have null point
		if ((PathList != null) && (PathList.Count > 0)) {
			MoveNextPoint ();			// auto move
		}
		//}

		RunAction ();

		// process mask
		if (this.eCampID == GameDataManager.Instance.nActiveCamp) {
			if (CanDoCmd ()) {
				MaskObj.SetActive (false);
			} else {
				MaskObj.SetActive (true);
			}
		} else {
			MaskObj.SetActive (false);
		}

		// If not null 
		if( pUnitData != null )
		{
			// Update HP
			float hp 	= pUnitData.n_HP;
			float nMaxhp  = pUnitData.GetMaxHP();
			float def 	= pUnitData.n_DEF;
			float nMaxdef  = pUnitData.GetMaxDef();
			float nTotal =  nMaxdef+nMaxhp;
			if( nTotal == 0) nTotal = 1;




			if( pUnitData.n_HP <= 0 ){				 
				pUnitData.n_HP =0;
			}
			if( pUnitData.n_MP <= 0 ){
				pUnitData.n_MP =0;
			}
			if( pUnitData.n_DEF <= 0 ){
				pUnitData.n_DEF =0;
			}
			if( pUnitData.n_SP <= 0 ){
				pUnitData.n_SP =0;
			}

			UISlider hpbar = HpBarObj.GetComponent<UISlider>();
			if( hpbar != null ){
				hpbar.value = hp / nTotal;
			}
			// Update Def
			UISlider defbar = DefBarObj.GetComponent<UISlider>();
			if( defbar != null ){
				defbar.value = (def+hp) / nTotal;
			}

		}	
	}

	void OnDestory () {
	//	GameDataManager.Instance.DelUnit( Ident() ); // no more destory
	}

	void OnDisable () 
	{
		// bad place.. move to on dead dead 
		// don't del unit during stage


	}
	public void SetUnitData( cUnitData data )
	{
		pUnitData = data;
	

//		if (pUnitData == null) {
//			pUnitData = GameDataManager.Instance.CreateChar (nCharID);
//		}
//		else{
//			// check game data to insert
//			GameDataManager.Instance.AddCharToPool( pUnitData );
//		}

		// return if unit data keep null
		if (pUnitData == null) {
			Debug.Log( "SetUnitData with null data" );
			return;	
		}
		
		CharID = data.n_CharID;
		
		SetXY( data.n_X , data.n_Y );

		CHARS charData = ConstDataManager.Instance.GetRow<CHARS>( CharID );
		if( charData == null)
			return;
		// change face
		string url = "Art/char/" + charData.s_FILENAME +"_S";
		UITexture face = FaceObj.GetComponent<UITexture>();
		if( face != null )
		{
			face.mainTexture = Resources.Load <Texture>( url  ) ;
			
		}
		
		// have assign data
		//if (data != null) {
		SetCamp( data.eCampID );
			//SetLevel( data.n_Lv );
		//}
	}

	public void FreeUnitData()
	{
		if (eCampID == _CAMP._PLAYER) {
			GameDataManager.Instance.BackUnitToStorage( Ident() );
			pUnitData = null;
			return ;
		}
		else {
			if( Panel_StageUI.Instance.bIsStageEnd == false )
			{
				if( pUnitData.CheckCanRePop() )	{
					pUnitData.SetUnDead();  // don't clear
					return;
				}
			}		
		}
		// default is clear 
		GameDataManager.Instance.DelUnit( Ident() );

		pUnitData = null;
	}

	public bool IsIdle()
	{
		if( IsMoving() )
		{
			return false;
		}
		if( IsAnimate() )
		{
			return false;
		}
		if( IsAction() )
		{
			return false;
		}

		return true; 
	}

	//click
//	public void OnClick( Panel_StageUI Stage )
//	{
		// 查情報
//	}



	// Cell utility Func 
	public int X(){ return Loc.X; } 
	public int Y(){ return Loc.Y; } 
//	public void X( int x ){ Loc.X=x; } 
//	public void Y( int y ){ Loc.Y=y; } 
	
	public iVec2 GetXY() { return Loc; }
	public void SetXY( int x , int y , bool bSyn=true ) {
		Loc.X = x;
		Loc.Y = y;

		if( pUnitData != null ){
			pUnitData.n_X = x;
			pUnitData.n_Y = y;
		}
		// syn  gameobj pos
		if( bSyn== true && GameScene.Instance != null ){
			gameObject.transform.localPosition =  MyTool.SnyGridtoLocalPos( x , y , ref GameScene.Instance.Grids ) ; 
		}

	}

	public void CreateChar( int nCharID , int x , int y , cUnitData data  )
	{
		CharID = nCharID;

		pUnitData = data;

		// no more create data here
//		if (pUnitData == null) {
//			pUnitData = GameDataManager.Instance.CreateChar (nCharID);
//		}
//		else{
//			// check game data to insert
//			GameDataManager.Instance.AddCharToPool( pUnitData );
//		}


		// return if unit data keep null
		if( pUnitData == null )
			return;	

		//record born pos
		//pUnitData.n_BornX = x; 
		//pUnitData.n_BornY = y; 

		SetXY( x , y );
		CHARS charData = ConstDataManager.Instance.GetRow<CHARS>( nCharID );
		if( charData == null)
			return;
		// change face
		string url = "Art/char/" + charData.s_FILENAME +"_S";
		UITexture face = FaceObj.GetComponent<UITexture>();
		if( face != null )
		{
			face.mainTexture = Resources.Load <Texture>( url  ) ;

		}

		// have assign data
		if (data != null) {
			SetCamp( data.eCampID );
			//SetLevel( data.n_Lv );
		}
	}

	public void MoveTo( int x , int y )
	{
		TarPos = new iVec2 (x, y);
		//TarPos.X = x;
		//TarPos.Y = y;
		if (TarPos.Collision (Loc)) {
			// this case won't trig move end event.

			return;
		}
		if (pathfind != null) {
			PathList = pathfind;
			pathfind = null;
		} else {
			PathList = Panel_StageUI.Instance.PathFinding( this  , Loc , TarPos  ); // ensure  we can reach 

			if( (PathList == null) || (PathList.Count<=0) ) // move direct if no path can find
			{
				PathList = GameScene.Instance.Grids.GetPathList (Loc, TarPos);
			}
		}
		MoveNextPoint();
	}

	// set path to move
	public void SetPath( List< iVec2 > path ){
		pathfind = path;
	}

	public bool IsAnimate()
	{
//		if( nTweenMoveCount != 0 )
//			return true;

		if( bIsAtking || bIsShaking || bIsCasting || bIsBorning || bIsDeading || bIsLeaving )
			return true;
		if (IsMoving ())
			return true;

		return false;
	}

	public bool IsMoving()
	{
		//if( nTweenMoveCount != 0  || bIsMoving )
		if( bIsMoving )
			return true;

		if( (PathList!=null) && PathList.Count > 0  )
			return true;

		return false; 
	}
	public void MoveNextPoint( )
	{
		if ((PathList == null))
			return ;
		if ((PathList.Count <= 0)) {
			// move end
			PathList = null;
		
			return  ;
		}


		iVec2 v = PathList [0];
		PathList.RemoveAt (0);

		// avoid the same point
		if (v.Collision (Loc)) {
			 MoveNextPoint(); // add this to avoid very ciritial bug. some time ismoving check will fail in this.
			return;
		}
		//TarPos = v;

		Vector3 tar = this.gameObject.transform.localPosition;
		tar.x = GameScene.Instance.Grids.GetRealX (v.X);
		tar.y = GameScene.Instance.Grids.GetRealY (v.Y);


		int iDist = Loc.Dist (v);
		float during = iDist * (0.2f);

		if (Config.GOD == true) {
			during = 0.2f; // always 0.2f
		}
		// cal target location position

		SetXY (v.X, v.Y, false);// record target pos as current pos , don't syn go pos. tween move will do it
//		Loc = v; 
		// update pos to unit data
//		pUnitData.n_X = Loc.X;
//		pUnitData.n_Y = Loc.Y;

		// Tween move
		TweenPosition tw = TweenPosition.Begin( this.gameObject , during , tar );
		if( tw!= null )
		{
			nTweenMoveCount++;	

//			tw.SetOnFinished( OnTweenNotifyMoveEnd );
			MyTool.TweenSetOneShotOnFinish( tw , OnTweenNotifyMoveEnd );

			bIsMoving = true;
		}

		// move camera to unit
		Panel_StageUI.Instance.MoveToGameObj (this.gameObject, false);

		//return bIsMoving;
	}

	public void MoveNextAtkPoint()
	{
		if ((PathList == null))
			return ;
		if ((PathList.Count <= 0)) {
			// flash attack  end
			PathList = null;
			OnTwAtkHit();

			return  ;
		}
		iVec2 v = PathList [0];
		PathList.RemoveAt (0);
		
		// avoid the same point
		if (v.Collision (Loc)) {
			MoveNextAtkPoint(); // add this to avoid very ciritial bug. some time ismoving check will fail in this.
			return;
		}
		
		Vector3 tar = this.gameObject.transform.localPosition;
		tar.x = GameScene.Instance.Grids.GetRealX (v.X);
		tar.y = GameScene.Instance.Grids.GetRealY (v.Y);	
		

		float during = 0.05f; // always 0.05f

		// cal target location position
		
		SetXY (v.X, v.Y, false);// record target pos as current pos , don't syn go pos. tween move will do it
		//		Loc = v; 
		// update pos to unit data
		//		pUnitData.n_X = Loc.X;
		//		pUnitData.n_Y = Loc.Y;
		
		// Tween move
		TweenPosition tw = TweenPosition.Begin( this.gameObject , during , tar );
		if( tw!= null )
		{
			nTweenMoveCount++;	
			
			//			tw.SetOnFinished( OnTweenNotifyMoveEnd );
			MyTool.TweenSetOneShotOnFinish( tw , OnTweenNotifyFlashMoveEnd );
			
			bIsMoving = true;
		}
	}

	public void OnSelected( bool OnOff )
	{
		if( bOnSelected == OnOff )
			return ;

		if (OnOff) {
			TweenScale tw = this.GetComponent<TweenScale>( );
			if( tw )
			{
				tw.ResetToBeginning();
				tw.enabled = true;
				tw.Play( true );
				//NGUITools.SetActive( tw , true );
			}
		
			bOnSelected = true;
		} else 
		{
			TweenScale tw = this.GetComponent<TweenScale>( );
			if( tw )
			{
				tw.ResetToBeginning();
				tw.enabled = false;

				Vector3 s = new Vector3( 1.0f , 1.0f ,1.0f );
				this.gameObject.transform.localScale = s; 
				//NGUITools.SetActive( tw , false );
			}

			// close 
			bOnSelected = false;
		}
	}

	public bool IsAction()
	{
		if (CurAction != null)
			return true;

		return false;
	}

	public bool SetAction( uAction act )
	{
		if (CurAction != null)
			return false;
		//need a pool for multi act
		CurAction = act;
		nSubActFlow = 0;

		// plane 
		UIPanel p = this.GetComponent<UIPanel> ();
		p.depth += 10;
		//RunAction (); // don't first run. it will broke unit update flow
		return true;
	}
	public bool ActionFinished(  )
	{
		// plane 
		UIPanel p = this.GetComponent<UIPanel> ();
		p.depth -= 10;

		//need a pool for multi act
		CurAction = null;
		nSubActFlow ++;

		return true;
	}
	public void RunAction()
	{
		if (CurAction == null)
			return;

		switch( CurAction.eAct )
		{
		case _ACTION._WAIT:
			switch( nSubActFlow )
			{
			case 0:
				ActionWait();
				nSubActFlow++;
				break;
			case 1:
				ActionFinished ();			
				break;
			}

			break;
		case _ACTION._ATK:
			switch( nSubActFlow )
			{
			case 0:
				nSubActFlow++;
				break;
			case 1:
				ActionAttack( CurAction.nTarIdent , CurAction.nSkillID );
				nSubActFlow++;
				break;
			case 2:
				// wait all hit result preform
				if( IsAnimate() == false ){
					ActionManager.Instance.ExecActionEndResult ( CurAction  );
					nSubActFlow++;
				}
				break;

			case 3:
				ActionFinished();
				break;
			}



			break;
		case _ACTION._MOVE:
			switch( nSubActFlow )
			{
			case 0:
				nSubActFlow++;
				ActionMove( CurAction.nTarGridX , CurAction.nTarGridY  );
				break;
			case 1:
				if( IsAnimate() == false ){ // wait movint finish
					nSubActFlow++;
				}
				break;
			case 2:
				ActionFinished();
				break;
			}
			break;
		case _ACTION._CAST:			// casting
			switch( nSubActFlow )
			{
			case 0:
				nSubActFlow++;
				ActionCasting( CurAction.nSkillID  , CurAction.nTarGridX , CurAction.nTarGridY );
				//ActionMove( CurAction.nTarGridX , CurAction.nTarGridY  );
				break;
			case 1:
				if( IsAnimate() == false ){
					// clear effect cell
					Panel_StageUI.Instance.ClearAOECellEffect();
					// play effect
					ActionManager.Instance.ExecActionEndResult ( CurAction  );
					nSubActFlow++;
				}

//				if( BattleMsg.nMsgCount == 0 && !bIsAtking ){// wait all msg complete
//					// clear effect cell
//					Panel_StageUI.Instance.ClearAOECellEffect();
//					// play effect
//					ActionManager.Instance.ExecActionEndResult ( CurAction  );
//					nSubActFlow++;
//				}
				break;
			case 2:
				ActionFinished();
				break;
			}
			break;
		case _ACTION._HIT:			// castout
			switch( nSubActFlow )
			{
			case 0:
				nSubActFlow++;
				ActionHit( CurAction.nSkillID ,CurAction.nTarGridX , CurAction.nTarGridY );
				//ActionMove( CurAction.nTarGridX , CurAction.nTarGridY  );
				break;
			case 1:
				if( BattleMsg.nMsgCount == 0 ){// wait all msg complete
					// clear effect cell
					Panel_StageUI.Instance.ClearAOECellEffect();
					// play effect
					ActionManager.Instance.ExecActionEndResult ( CurAction  );
					nSubActFlow++;
				}
				//ActionMove( CurAction.nTarGridX , CurAction.nTarGridY  );
				break;

			case 2:
				ActionFinished();
				break;
			}
			break;
		case _ACTION._DROP:			// Drop
			switch( nSubActFlow )
			{
			case 0:
				nSubActFlow++;
				// show
				ActionDrop( CurAction.nActVar1 , CurAction.nActVar2  ); // exp / money
				//ActionHit( CurAction.nSkillID ,CurAction.nTarGridX , CurAction.nTarGridY );
				//ActionMove( CurAction.nTarGridX , CurAction.nTarGridY  );
				break;
			case 1:
				ActionFinished();
				break;
			}
			break;
		case _ACTION._LVUP:			// Drop
			switch( nSubActFlow )
			{
			case 0:
				nSubActFlow++;
				// show
				ActionLvUp( CurAction.nActVar1 , CurAction.nActVar2  ); // exp / money
				//ActionHit( CurAction.nSkillID ,CurAction.nTarGridX , CurAction.nTarGridY );
				//ActionMove( CurAction.nTarGridX , CurAction.nTarGridY  );
				break;
			case 1:
				ActionFinished();
				break;
			}
			break;
		case _ACTION._WEAKUP:
			switch( nSubActFlow )
			{
			case 0:
				nSubActFlow++;
				ActionWeakup(   ); // exp / money
				//ActionHit( CurAction.nSkillID ,CurAction.nTarGridX , CurAction.nTarGridY );
				//ActionMove( CurAction.nTarGridX , CurAction.nTarGridY  );
				break;
			case 1:
				ActionFinished();
				break;
			}
			break;


		default:
		//CurAction = null; // set null to avoid char block in infinite action
			ActionFinished();
			break;

		}
	}



	public void ActionAttack( int tarident  , int skillid )
	{
		TarIdent = tarident;
		Panel_unit defer = Panel_StageUI.Instance.GetUnitByIdent( TarIdent );
		if (defer == null) {
			Debug.LogErrorFormat( "unit {0} attack null target{1}  " , Ident() , TarIdent );
			return;
		}

		bIsAtking = true;
		// fly item
		if (MyTool.IsSkillTag (skillid, _SKILLTAG._FLY)) {
			string missile= "ACT_FLAME";
			Missile missdata = null;
			if( skillid > 0 ){
				SKILL skl = ConstDataManager.Instance.GetRow<SKILL>( skillid ); 
				if( skl!= null ){
					if( skl.n_MISSILE_ID > 0  ){
						missdata = ConstDataManager.Instance.GetRow<Missile>( skl.n_MISSILE_ID ); 
						if( missdata != null ){
							missile = missdata.s_MISSILE;
						}
					}
				}
			}
			FightBulletFX fbFx = FightBulletFX.CreatFX (missile, this.transform, this.transform.localPosition, defer.transform.localPosition, OnTwAtkFlyHit);

			// create a fly item
			return;
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._ROTATE)) {

			RotateAttack();
			return ;
		}
		else if (MyTool.IsSkillTag (skillid, _SKILLTAG._FLASH)) {
			SKILL skl = ConstDataManager.Instance.GetRow<SKILL>( skillid ); 
			if( skl!= null && skl.n_AREA > 0 ){
				// get aoe pool
				int nTarX = defer.Loc.X;
				int nTarY = defer.Loc.Y;
				List < iVec2 > lst = MyTool.GetAOEPool ( nTarX ,nTarY,skl.n_AREA ,Loc.X , Loc.Y  );
				FlashAttack( lst );
				return ;
			}

		}
		else if (MyTool.IsSkillTag (skillid, _SKILLTAG._JUMP )) {
			int nTarX = defer.Loc.X;
			int nTarY = defer.Loc.Y;
			JumpAttack( nTarX , nTarY );
			return ;
		}
		else if (MyTool.IsSkillTag (skillid, _SKILLTAG._BOW)) {
		
			int nTarX = defer.Loc.X;
			int nTarY = defer.Loc.Y;
			BowAttack( nTarX ,nTarY );
			return ;		
		}
	

		//Vector3 vOrg = this.transform.localPosition;
		//Vector3 vTar = defer.transform.localPosition;

		// melee attack

		TweenPosition tw = TweenPosition.Begin< TweenPosition >( this.gameObject , 0.2f );
		if( tw != null )
		{
			tw.SetStartToCurrentValue();
			tw.to	= defer.transform.localPosition;
//			Debug.LogFormat("ActionAttack from {0} , {1} , locPos {2} , {3} ", tw.from.x, tw.from.y , transform.localPosition.x ,  transform.localPosition.y );
			MyTool.TweenSetOneShotOnFinish( tw , OnTwAtkHit ); // for once only



		}
		// add 


	}
	public void ActionHit( int skillid , int GridX , int GridY )
	{
		bIsAtking = true;
		if (MyTool.IsSkillTag (skillid, _SKILLTAG._FLY)) {
			string missile = "ACT_FLAME";
			Missile missdata = null;
			if (skillid > 0) {
				SKILL skl = ConstDataManager.Instance.GetRow<SKILL> (skillid); 
				if (skl != null) {
					if (skl.n_MISSILE_ID > 0) {
						missdata = ConstDataManager.Instance.GetRow<Missile> (skl.n_MISSILE_ID); 
						if (missdata != null) {
							missile = missdata.s_MISSILE;
						}
					}
				}
			}
			Vector3 vTar = new Vector3 (MyTool.ScreenToLocX (GridX), MyTool.ScreenToLocX (GridY));

			FightBulletFX fbFx = FightBulletFX.CreatFX (missile, this.transform, this.transform.localPosition, vTar, OnTwAtkFlyHit);
			
			// create a fly item
			return;
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._ROTATE)) {
			RotateAttack();
			return;
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._FLASH)) {
			SKILL skl = ConstDataManager.Instance.GetRow<SKILL> (skillid); 
			if (skl != null && skl.n_AREA > 0) {
				// get aoe pool
				int nTarX = GridX;
				int nTarY = GridY;
				List < iVec2 > lst = MyTool.GetAOEPool (nTarX, nTarY, skl.n_AREA, Loc.X, Loc.Y);
				FlashAttack (lst);
				return;
			}
			
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._JUMP )) {
			JumpAttack( GridX , GridY );
			return ;
		}
		else if (MyTool.IsSkillTag (skillid, _SKILLTAG._BOW)) {
			BowAttack( GridX ,GridY );
			return ;
		}
		// exec result directly
		ActionManager.Instance.ExecActionHitResult ( CurAction );
		bIsAtking = false;
		// do hitresult direct
	//	ActionManager.Instance.ExecActionEndResult ( CurAction );			// this is very import . all preform and data modify here!!
	}
	

	public void ActionMove( int GridX , int GridY )
	{
		// maybe need some other process in the future
		// Panel_StageUI.Instance.TraceUnit (this); // no good here


		MoveTo ( GridX , GridY );
	}

	public void ActionWait( )
	{
		// select to wait
		BattleManager.Instance.ShowBattleMsg (this, "waiting");
	//	ActionFinished ();
		if ( pUnitData!= null ) {
			pUnitData.Waiting();
		}

	}

	public void ActionWeakup( )
	{
		// 如果有數字 要表演// 
		if( CurAction.HitResult.Count > 0  )
			ActionManager.Instance.ExecActionEndResult ( CurAction  );

	}

	public void ActionCasting( int nSkillID , int nTarGridX ,int nTarGridY )
	{
		Panel_StageUI.Instance.AddAVGObj ( Ident() );


		if( CurAction.nSkillID == 0 ){

			BattleManager.Instance.ShowBattleMsg( this  , MyTool.GetUnitSchoolFullName(Ident(), pUnitData.nActSch[1] ) );  // Get school name
		}
		else{
			// Get Skill
			SKILL skl = ConstDataManager.Instance.GetRow<SKILL>( CurAction.nSkillID );
			if( skl != null ){


				BattleManager.Instance.ShowBattleMsg( this, skl.s_NAME );
				
				// play fx
				if( skl.n_CAST_FX > 0 ){
					GameSystem.PlayFX( this.gameObject , skl.n_CAST_FX );
				}
				// show effect area
				if( skl.n_AREA > 0 )
				{
					Panel_StageUI.Instance.CreateAOEOverEffect( this , nTarGridX , nTarGridY ,skl.n_AREA );
				}
			}
		}
	//	ActionManager.Instance.ExecActionEndResult ( CurAction  );
	}

	public void ActionDrop( int nExp , int nMoney )
	{
		string sMsg = string.Format( "Exp + {0} , \n Money + {1}" , nExp , nMoney );

		BattleManager.Instance.ShowBattleMsg( null , sMsg );  // show 

		pUnitData.AddExp( nExp );

		GameDataManager.Instance.nMoney += nMoney;


	}

	public void ActionLvUp( int nLvUP , int nExp )
	{
		string sMsg = string.Format( "Lv + {0} " , nLvUP  );
		
		BattleManager.Instance.ShowBattleMsg( null , sMsg );  // show 
		//pUnitData.AddExp( nExp );

		// show new ability

	}
	public void HitBackTo( int GridX , int GridY )
	{
		// maybe need some other process in the future
		// Panel_StageUI.Instance.TraceUnit (this); // no good here
		
		
		//MoveTo ( GridX , GridY );
		TarPos = new iVec2 (GridX, GridY);
		if (TarPos.Collision (Loc)) {
			// this case won't trig move end event.
			return;
		}
		if (PathList == null)
			PathList = new List< iVec2 > ();
		else // clear pathfild
			PathList.Clear ();

		PathList.Add ( TarPos );

		MoveNextPoint();	// set to target pos

	}

	public void FlashAttack( List< iVec2> lst  )
	{
		bIsAtking = true;
		lst.Add( new iVec2( Loc.X , Loc.Y  ) );
		PathList = lst;

		MoveNextAtkPoint();
	}
	public void JumpAttack( int GridX , int  GridY )
	{
		bIsAtking = true;
		//iVec2 v = new iVec2 (GridX, GridY);
		Vector3 tar = this.gameObject.transform.localPosition;
		tar.x = GameScene.Instance.Grids.GetRealX (GridX);
		tar.y = GameScene.Instance.Grids.GetRealY (GridY);
		
		
		int iDist = Loc.Dist ( GridX , GridY );
		float during = iDist * (0.2f);
		if (during <1.0f)
			during = 1.0f;

		// cal target location position

		// find final pos to stand
		//iVec2 tar =  Panel_StageUI.Instance.FindEmptyPos ( v );

		//SetXY (GridX, GridY , false);// record target pos as current pos , don't syn go pos. tween move will do it

		//		Loc = v; 
		// update pos to unit data
		//		pUnitData.n_X = Loc.X;
		//		pUnitData.n_Y = Loc.Y;
		
		// Tween move
		TweenPosition tw = TweenPosition.Begin( this.gameObject , during , tar );
		if( tw!= null )
		{
			nTweenMoveCount++;	
			
			//			tw.SetOnFinished( OnTweenNotifyMoveEnd );
			MyTool.TweenSetOneShotOnFinish( tw , OnTweenNotifyJumpEnd );
			
			bIsMoving = true;

			// scale
			during*= 0.8f;
			float time2 = during / 2;
			Vector3 scale = new Vector3( 1.5f , 1.5f , 1.0f );
			TweenScale twj = TweenScale.Begin<TweenScale>( this.gameObject , time2 );
			if( twj != null ){
				twj.delay = 0.0f;
				twj.SetStartToCurrentValue();
				twj.to = scale;
				twj.style = UITweener.Style.Once;
				//MyTool.TweenSetOneShotOnFinish (twj, OnTwJumpHighest); // for once only
			}
			TweenScale twj2 =  this.gameObject.AddComponent<TweenScale>();
			if( twj2 != null ){
				twj2.duration = time2;
				twj2.delay = time2;
				twj2.from = scale;
				twj2.SetEndToCurrentValue();
				twj2.style = UITweener.Style.Once;
				twj2.Play();
			}

			UIPanel p = this.gameObject.GetComponent<UIPanel> ();
			if( p != null ){
				p.depth ++ ; // move to hight then other
			}

		}
	}
	public void RotateAttack( )
	{
		TweenRotation twr = TweenRotation.Begin< TweenRotation >( gameObject , 0.5f );
		if( twr != null )
		{
			twr.SetStartToCurrentValue();
			twr.to	= new Vector3( 0.0f , 0.0f , -360.0f );//Math.PI
			MyTool.TweenSetOneShotOnFinish( twr , OnTwAtkRotateEnd ); // for once only
		}
	}
	public void OnTwAtkRotateEnd( )
	{		
		// clear all move tw
		TweenRotation[] tws = gameObject.GetComponents<TweenRotation> (); 
		foreach (TweenRotation tw in tws) {
			Destroy( tw );
		}
		// reset pos
		gameObject.transform.localRotation = Quaternion.identity;
		
		ActionManager.Instance.ExecActionHitResult ( CurAction );	 // perform sm hit action
		
		bIsAtking = false;
	}

	public void BowAttack( int GridX , int  GridY )
	{
		bIsAtking = true;
		Vector3 vTar = MyTool.SnyGridtoLocalPos (GridX, GridY ,ref GameScene.Instance.Grids  );

		Vector3 diff =  vTar - this.transform.localPosition; 
		float mag = diff.magnitude;
		if( mag > 100.0f ){
			diff.x *= ( 100.0f / mag );  
			diff.y *= ( 100.0f / mag );  
		}
		Vector3 v = this.transform.localPosition - diff; // final pos

		TweenPosition tw = TweenPosition.Begin< TweenPosition >( this.gameObject , 0.35f ); // always move back to start pos
		if (tw != null) {
			tw.SetStartToCurrentValue();
			tw.to = v;
		}

		TweenPosition tw2 = gameObject.AddComponent <TweenPosition>();
		if (tw2 != null) {
			tw2.duration = 0.15f;
			tw2.delay = 0.35f;
			tw2.from = v;
			tw2.SetEndToCurrentValue();
			MyTool.TweenSetOneShotOnFinish(tw2, OnTwAtkBowEnd );
			tw2.Play();
		}

	}
	public void OnTwAtkBowEnd( )
	{
	
		// clear all move tw
		TweenPosition[] tws = gameObject.GetComponents<TweenPosition> (); 
		foreach (TweenPosition tw in tws) {
			Destroy( tw );
		}
		// reset pos
		this.gameObject.transform.localPosition = MyTool.SnyGridtoLocalPos(Loc.X , Loc.Y , ref GameScene.Instance.Grids );

		ActionManager.Instance.ExecActionHitResult ( CurAction );	 // perform sm hit action

		bIsAtking = false;
	}

	//==============Tween CAll back
	public void OnTwAtkHit( )
	{
		// move back 
//		Debug.LogFormat (this.ToString () + " hit Target{0}", TarIdent);

		Vector3  vTar = MyTool.SnyGridtoLocalPos(Loc.X , Loc.Y , ref GameScene.Instance.Grids );
		ActionManager.Instance.ExecActionHitResult ( CurAction );	 // perform sm hit action

		TweenPosition tw = TweenPosition.Begin< TweenPosition >( this.gameObject , 0.3f ); // always move back to start pos
		if (tw != null) {
			tw.SetStartToCurrentValue ();
			//Debug.LogFormat ("ActionHit from {0} , {1} ,loc Pos {2} , {3}  ", tw.from.x, tw.from.y, transform.localPosition.x, transform.localPosition.y);
			tw.to = vTar;
			//Debug.LogFormat ("ActionHit to {0} , {1} ,loc  {2} , {3}  ", tw.to.x, tw.to.y, Loc.X, Loc.Y);
			MyTool.TweenSetOneShotOnFinish (tw, OnTwAtkEnd); // for once only

		} else {
			//===========================================================
			//ActionManager.Instance.ExecActionEndResult ( CurAction );			// this is very import . all preform and data modify here!!
			//===========================================================

		}
	}
	public void OnTwAtkFlyHit( )
	{
		ActionManager.Instance.ExecActionHitResult ( CurAction );	 // perform sm hit action

		OnTwAtkEnd(); // Hit is end
	}

	public void OnTwAtkEnd( )
	{
		// move to loc pos
		//===========================================================
	//	ActionManager.Instance.ExecActionEndResult ( CurAction );			// this is very import . all preform and data modify here!!
		//===========================================================

		transform.localPosition =  MyTool.SnyGridtoLocalPos(Loc.X , Loc.Y , ref GameScene.Instance.Grids );

		bIsAtking = false;
		TarIdent = 0;
	}


	public void ShowValueEffect( int nValue , int nMode )
	{
		//nMode : 0 - hp , 1- def , 2 - mp , 3 -sp
		if (nValue < 0) {
			// shake
			TweenShake tw = TweenShake.Begin(gameObject, 0.2f , 10 );
			if (tw) {
				//tw.OriginPosSet = false; // Important!
				//tw.style = UITweener.Style.Once;
				tw.shakeY = false;
				MyTool.TweenSetOneShotOnFinish( tw , OnTwShakeEnd );

				//tw.SetOnFinished (OnTwShakeEnd);
				bIsShaking = true;
			}

			BattleManager.Instance.ShowBattleResValue( this.gameObject , nValue , nMode );
		} else if (nValue > 0) {
			// heal

			BattleManager.Instance.ShowBattleResValue( this.gameObject , nValue , nMode );
		}
		// show dmg effect

	}
	public void OnTwShakeEnd( )
	{
		bIsShaking = false;
	}


	public void SetDead()
	{
		// avoid double run
		if (bIsDead == true)
			return;

		// check if ant event need to run?
		//
		MaskObj.SetActive (false); // 
	//	BGObj.SetActive (false); // why this?
		bIsDead = true; // set dead
		bIsDeading = true;
		// shake
		TweenShake tws = TweenShake.Begin(gameObject, 1.0f , 15 );
		if( tws )
		{
			tws.shakeX = true;
			tws.shakeY = true;

			Destroy( tws , 1.0f ); // 
		}

		//TweenGrayLevel
		//Vector2 vfrom = new Vector3( 1.0f , 1.0f , 1.0f );
		//Vector2 vto   = new Vector3( 0.0f , 10.0f, 1.0f );
		TweenGrayLevel tw = TweenGrayLevel.Begin <TweenGrayLevel >( FaceObj, 1.0f);
		if (tw) {

			tw.from = 0.0f;
			tw.to   = 1.0f;
			MyTool.TweenSetOneShotOnFinish( tw , OnDead );
//			tw.style = UITweener.Style.Once; // PLAY ONCE
//			tw.SetOnFinished( OnDead );

		}

		// free data here
		//FreeUnitData ();

	}

	public void OnDead()
	{	
		bIsDeading = false;
		// remove char
		StageDelUnitByIdentEvent evt = new StageDelUnitByIdentEvent ();
		//evt.eCamp  = eCampID; // no need
		evt.nIdent = this.Ident ();

		GameEventManager.DispatchEvent ( evt );

		// avoid shake tween


	}

	public void SetBorn()
	{
		// avoid double run
		if (bIsBorning == true)
			return;
		bIsBorning = true;

		TweenHeight tw = TweenHeight.Begin<TweenHeight>(  FaceObj , 0.5f );
		if (tw != null) {
			tw.from = 0;
			tw.to = Config.TileH;
			MyTool.TweenSetOneShotOnFinish( tw , OnBornFinish );
		}

		GameSystem.PlayFX (gameObject, 1);
	}
	public void OnBornFinish()
	{
		bIsBorning = false;
	}

	public void SetLeave()
	{
		// avoid double run
		if (bIsLeaving == true)
			return;
		// play leave fx
		GameObject fx =  GameSystem.PlayFX ( this.gameObject , 5  ); // need rot x  to -75

		bIsLeaving = true;
		
		TweenHeight tw = TweenHeight.Begin<TweenHeight>(  FaceObj , 1.0f );
		if (tw != null) {
			tw.SetStartToCurrentValue();
			tw.to = 0;
			MyTool.TweenSetOneShotOnFinish( tw , OnLeaveFinish );
		}
		
	}
	public void OnLeaveFinish()
	{
		bIsLeaving = false;
		StageDelUnitByIdentEvent evt = new StageDelUnitByIdentEvent (); // del by ident
		//evt.eCamp  = eCampID; // no need
		evt.nIdent = this.Ident ();
		
		GameEventManager.DispatchEvent ( evt );

	}


	public void SetCamp( _CAMP camp )
	{
//		GameDataManager.Instance.DelCampMember( eCampID , Ident() ); // global game data

		eCampID = camp;
		UISprite sp = BGObj.GetComponent<UISprite>();
		if (sp == null)			return;

		switch (camp) {
		case _CAMP._PLAYER:
			sp.color = new Color( 0.0f , 0.0f , 1.0f );
			break;

		case _CAMP._ENEMY:
			sp.color = new Color( 1.0f , 0.0f , 0.0f );
			break;

		default:
			sp.color = new Color( 0.0f , 1.0f , 0.0f );// green
			//sp.color = new Color( 1.0f , 1.0f , 0.0f );// yellow
			break;
		}

		sp.alpha= 1.0f;

		//=======================
		if(pUnitData != null) {
		   pUnitData.eCampID = eCampID;
		}

//		GameDataManager.Instance.AddCampMember( camp , Ident() ); // global game data

	}

	public void SetLevel( int nLV )
	{
		if (nLV < 1)
			nLV = 1;

		if (nLV > Config.MaxCharLevel)
			nLV = Config.MaxCharLevel;

		//cUnitData data = GameDataManager.Instance.GetUnitDateByIdent ( Ident() );
		if (pUnitData != null) {
			pUnitData.SetLevel ( nLV );

			// re sp 
			pUnitData.UpdateAttr();
			pUnitData.n_SP = pUnitData.GetMaxSP();
		}
	}

	public void SetLeader( int nLeader )
	{
		if (pUnitData != null) {
			pUnitData.n_LeaderIdent =  nLeader;
		}
	}

	public bool CanPK( Panel_unit  unit ) 	
	{
		if (unit == null)
			return false;

		return MyTool.CanPK ( this.eCampID , unit.eCampID );
//		if (unit.eCampID != this.eCampID) {
//			if( unit.eCampID == _CAMP._ENEMY || eCampID == _CAMP._ENEMY )
//			{
//				return true;
//			}
//		}
//
//		return false;
	}


//	public List< Panel_unit  > GetPKUnitPool( bool bCanPK )
//	{
//		List< Panel_unit  > pool = new List< Panel_unit  >();
//
//
//		return pool;
//	}
//
//	public List<iVec2 > GetPKPosPool( bool bCanPK )
//	{
//		List< iVec2  > pool = new List< iVec2  >();
//		List< Panel_unit  > unitpool = GetPKUnitPool (bCanPK);
//
//		
//		return pool;
//	}

	public int Dist( Panel_unit  unit ) 	
	{
		return this.Loc.Dist (unit.Loc);
	}

	// enemy use
	public void RunAI( )
	{
		if (Config.MOBAI == false) {

			ActionWait();
			return;
		}
		MobAI.Run (this);

		Panel_StageUI.Instance.MoveToGameObj ( this.gameObject , false );
		return;



	}


	// call back func
	// normal move
	int	   nTweenMoveCount	= 0;		// check move is done
	public  void OnTweenNotifyMoveEnd(  )
	{
		bIsMoving = false; 
		nTweenMoveCount--;
		if (nTweenMoveCount < 0)
			nTweenMoveCount = 0;
	

		MoveNextPoint();
	}
	// flash attack
	public  void OnTweenNotifyFlashMoveEnd(  )
	{
		bIsMoving = false; 
		nTweenMoveCount--;
		if (nTweenMoveCount < 0)
			nTweenMoveCount = 0;
	
		
		MoveNextAtkPoint();
	}

	public  void OnTweenNotifyJumpEnd(  )
	{
		bIsMoving = false; 
		nTweenMoveCount--;
		if (nTweenMoveCount < 0)
			nTweenMoveCount = 0;

		iVec2 v = MyTool.SnyLocalPostoGrid ( transform.localPosition , ref GameScene.Instance.Grids );

		iVec2 final =  Panel_StageUI.Instance.FindEmptyPos ( v );

		// clear all path 

		SetXY (final.X, final.Y);

		PathList = null; // move end
		//MoveNextPoint();
		UIPanel p = this.gameObject.GetComponent<UIPanel> ();
		if( p != null ){
			p.depth -- ; // move to hight then other
		}
		// remove all scale
		TweenScale[] tws = gameObject.GetComponents<TweenScale> (); 
		foreach (TweenScale tw in tws) {
			Destroy( tw );
		}
		//
		this.gameObject.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);

		ActionManager.Instance.ExecActionHitResult ( CurAction );	 // perform sm hit action

		bIsAtking = false;
	}


	//====== Fight 

	public void ShowSkillFX( int nSkillID , int nTarIdent , int nX , int nY )
	{
		Debug.Log ("show skillfx");
		if (nSkillID == 0) {
			return ;
		}

		if (nTarIdent != 0) {
			cUnitData pdata = GameDataManager.Instance.GetUnitDateByIdent( nTarIdent  );
			if( pdata == null ){
				return;
			}
			nX = pdata.n_X; nY = pdata.n_Y;
		}
		//=================cast skill
		SKILL skl = ConstDataManager.Instance.GetRow< SKILL > ( nSkillID ); 
		if (skl == null)
			return;

		FX fxData = ConstDataManager.Instance.GetRow< FX > ( skl.n_CASTOUT_FX ); 
		if (fxData == null)
			return;

		// AOE fx 
		if (fxData.n_TAG == 2) {
			Panel_StageUI.Instance.PlayAOEFX( this, skl.n_CASTOUT_FX ,  nX, nY , skl.n_AREA  );

		}
		else if (fxData.n_TAG == 4) { // play in place
			Panel_StageUI.Instance.PlayFX(  skl.n_CASTOUT_FX ,  nX, nY  );
		}
		else {
			GameObject go = GameSystem.PlayFX ( this.gameObject , skl.n_CASTOUT_FX );
			if (go == null) {
				return;
			}

			if (fxData.n_TAG == 1) {			// 處理旋轉
				_DIR dir = Loc.Get8Dir (nX, nY);
				//Vector3 rot ;
				switch (dir) {
				case _DIR._UP:
				go.transform.localRotation = Quaternion.Euler (-90, 0, 0);
				break;
				case _DIR._RIGHT:
					go.transform.localRotation = Quaternion.Euler (0, 90, 0);
					break;
				case _DIR._DOWN:
					go.transform.localRotation = Quaternion.Euler (90, 0, 0);
					break;
				case _DIR._LEFT:
					go.transform.localRotation = Quaternion.Euler (0, -90, 0);
					break;
				// 8 way 
				case _DIR._RIGHT_UP:
					go.transform.localRotation = Quaternion.Euler (-45, 90, 0);
					break;
				case _DIR._LEFT_UP:
					go.transform.localRotation = Quaternion.Euler (-45, -90, 0);
					break;
				case _DIR._RIGHT_DOWN:
					go.transform.localRotation = Quaternion.Euler (45, 90, 0);
					break;
				case _DIR._LEFT_DOWN:
					go.transform.localRotation = Quaternion.Euler (45, -90, 0);
					break;
				}
			}
		}
	}
//	public void AddBuff( int nBuffID )
//	{
//		pUnitData.Buffs.AddBuff (nBuffID);
//
//	}

//	public void DelBuff( int nBuffID )
//	{
//		pUnitData.Buffs.DelBuff(nBuffID);
//	}

}
