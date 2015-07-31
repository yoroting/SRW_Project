using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MYGRIDS;
using MyClassLibrary;
//using _SRW;
// maybe don't use this
public class cAI_CMD{
	public enum _AI_TYPE
	{
		_NONE,
		_MOVE,
		_ATK,
		_DEF,
		_ABILITY,
		_WAIT,
	};

	public int nTarIdent { set; get; }
	public int nTarX { set; get; }
	public int nTarY { set; get; }
	public int nSkillID { set; get; }
	public int nAbilityID { set; get; }

}


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
	}

	// Awake
	void Awake(){
		bOnSelected = false;
	//	nActionTime = 1;				// default is 1

		//ParticleSystemRenderer

		GameObject instance = ResourcesManager.CreatePrefabGameObj ( this.gameObject ,"FX/Cartoon FX/CFXM4 Splash" );

		ParticleSystem ps =instance.GetComponent< ParticleSystem>();
		if (ps!= null) {

		}
		ParticleSystemRenderer psr =instance.GetComponent< ParticleSystemRenderer>();
		if (psr != null) {
			psr.sortingLayerName = "FX";
		}

		SetBorn (); // start born animate
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
			UISlider hpbar = HpBarObj.GetComponent<UISlider>();
			if( hpbar != null ){
				float hp 	= pUnitData.n_HP;
				float nMaxhp  = pUnitData.GetMaxHP();
				hpbar.value = hp / nMaxhp;
			}
			// Update Def
			UISlider defbar = DefBarObj.GetComponent<UISlider>();
			if( defbar != null ){
				float def 	= pUnitData.n_DEF;
				float nMaxdef  = pUnitData.GetMaxDef();
				hpbar.value = def / nMaxdef;
			}

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

			UISlider hpSlider = HpBarObj.GetComponent<UISlider>();
			if( hpSlider != null ){
				hpSlider.value =  (float) pUnitData.n_HP /  (float) pUnitData.GetMaxHP() ;
			}
			UISlider defSlider = DefBarObj.GetComponent<UISlider>();
			if( defSlider != null ){
				defSlider.value =  (float) pUnitData.n_DEF /  (float) pUnitData.GetMaxDef() ;
			}

		}	
	}

	void OnDestory () {
	//	GameDataManager.Instance.DelUnit( Ident() ); // no more destory
	}

	void OnDisable () 
	{
		// don't del unit during stage
		if (eCampID == _CAMP._PLAYER) {
			GameDataManager.Instance.BackUnitToStorage( Ident() );
		}
		else {
			GameDataManager.Instance.DelUnit( Ident() );
		}
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

	public void CreateChar( int nCharID , int x , int y , cUnitData data = null )
	{
		CharID = nCharID;

		pUnitData = data;

		if (pUnitData == null) {
			pUnitData = GameDataManager.Instance.CreateChar (nCharID);
		}
		else{
			// check game data to insert
			GameDataManager.Instance.AddCharToPool( pUnitData );
		}
		// return if unit data keep null
		if( pUnitData == null )
			return;

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

			if( PathList == null ) // move direct if no path can find
			{
				PathList = GameScene.Instance.Grids.GetPathList (Loc, TarPos);
			}
		}
		MoveNextPoint();
	}

	public bool IsAnimate()
	{
//		if( nTweenMoveCount != 0 )
//			return true;

		if( bIsAtking || bIsShaking || bIsCasting || bIsBorning || bIsDeading)
			return true;
		if (IsMoving ())
			return true;

		return false;
	}

	public bool IsMoving()
	{
		if( nTweenMoveCount != 0  || bIsMoving )
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
		//RunAction (); // don't first run. it will broke unit update flow
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
			//	ActionFinished ();
				CurAction = null; // clear act
				nSubActFlow++;
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
				ActionAttack( CurAction.nTarIdent );
				nSubActFlow++;
				break;
			case 2:
				// wait all hit result preform
				if( IsAnimate() == false ){
					nSubActFlow++;
				}
				break;

			case 3:
			//	ActionFinished ();
				CurAction = null; // clear act
				nSubActFlow++;
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
				nSubActFlow++;
				CurAction = null; // clear act
				break;
			}
			break;
		case _ACTION._CAST:			// casting
			switch( nSubActFlow )
			{
			case 0:
				nSubActFlow++;
				ActionCasting( CurAction.nSkillID );
				//ActionMove( CurAction.nTarGridX , CurAction.nTarGridY  );
				break;
			case 1:
				nSubActFlow++;
				CurAction = null; // clear act
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
				nSubActFlow++;
				CurAction = null; // clear act
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
				nSubActFlow++;
				CurAction = null; // clear act
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
				nSubActFlow++;
				CurAction = null; // clear act
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
				nSubActFlow++;
				CurAction = null; // clear act
				break;
			}
			break;


		default:
			CurAction = null; // set null to avoid char block in infinite action
			break;

		}
	}



	public void ActionAttack( int tarident )
	{
		TarIdent = tarident;
		Panel_unit defer = Panel_StageUI.Instance.GetUnitByIdent( TarIdent );
		if (defer == null) {
			Debug.LogErrorFormat( "unit {0} attack null target{1}  " , Ident() , TarIdent );
			return;
		}

		//Vector3 vOrg = this.transform.localPosition;
		//Vector3 vTar = defer.transform.localPosition;

		TweenPosition tw = TweenPosition.Begin< TweenPosition >( this.gameObject , 0.2f );
		if( tw != null )
		{
			tw.SetStartToCurrentValue();
			tw.to	= defer.transform.localPosition;

			Debug.LogFormat("ActionAttack from {0} , {1} , locPos {2} , {3} ", tw.from.x, tw.from.y , transform.localPosition.x ,  transform.localPosition.y );
//			EventDelegate del = new EventDelegate( OnTwAtkHit );
//			del.oneShot = true;
//			tw.SetOnFinished(  del ) ; // this will add 

			MyTool.TweenSetOneShotOnFinish( tw , OnTwAtkHit ); // for once only
//			tw.SetOnFinished( OnTwAtkHit );

			bIsAtking = true;
		}
		// add 


	}
	public void ActionHit( int nSkill , int GridX , int GridY )
	{
		// add preform in the future

		// do hitresult direct
		ActionManager.Instance.ExecActionHitResult ( CurAction );			// this is very import . all preform and data modify here!!
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

	}

	public void ActionCasting( int nSkillID )
	{
		if( CurAction.nSkillID == 0 ){
			BattleManager.Instance.ShowBattleMsg( this  , MyTool.GetUnitSchoolFullName(Ident(), pUnitData.nActSch[1] ) );  // Get school name
		}
		else{
			// Get Skill
			SKILL skl = ConstDataManager.Instance.GetRow<SKILL>( CurAction.nSkillID );
			if( skl != null ){
				BattleManager.Instance.ShowBattleMsg( this, skl.s_NAME );
				
				// play fx
				GameSystem.PlayFX( this.gameObject , "CFXM4 Magic Drain Fast" );
			}
			
		}
		ActionManager.Instance.ExecActionHitResult ( CurAction  );


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


	//==============Tween CAll back
	public void OnTwAtkHit( )
	{
		// move back 
		Debug.LogFormat (this.ToString () + " hit Target{0}", TarIdent);

		Vector3  vTar = MyTool.SnyGridtoLocalPos(Loc.X , Loc.Y , ref GameScene.Instance.Grids );
		//Vector3 vTar = defer.transform.localPosition;

		//TweenPosition tw = new TweenPosition (); 

	
		// play effect , get by ext school 
	
		BattleManager.Instance.ShowBattleFX( TarIdent , "CFXM4 Hit B (Orange, CFX Blend)"  );

		//List< cHitResult>  resPool = BattleManager.Instance.CalAttackResult( Ident() , TarIdent );


//		bIsAtking = false;
//		TarIdent = 0;
		TweenPosition tw = TweenPosition.Begin< TweenPosition >( this.gameObject , 0.3f ); // always move back to start pos
		if (tw != null) {
			//tw.from = vOrg;
			tw.SetStartToCurrentValue ();
			Debug.LogFormat ("ActionHit from {0} , {1} ,loc Pos {2} , {3}  ", tw.from.x, tw.from.y, transform.localPosition.x, transform.localPosition.y);
			tw.to = vTar;
			Debug.LogFormat ("ActionHit to {0} , {1} ,loc  {2} , {3}  ", tw.to.x, tw.to.y, Loc.X, Loc.Y);
			//tw.onFinished.Clear();
			MyTool.TweenSetOneShotOnFinish (tw, OnTwAtkEnd); // for once only
			//tw.SetOnFinished(  OnTwAtkEnd ) ;
			//tw.Play();
		} else {
			//===========================================================
			ActionManager.Instance.ExecActionHitResult ( CurAction );			// this is very import . all preform and data modify here!!
			//===========================================================

		}
	}
	public void OnTwAtkEnd( )
	{
		// move to loc pos
		//===========================================================
		ActionManager.Instance.ExecActionHitResult ( CurAction );			// this is very import . all preform and data modify here!!
		//===========================================================

		transform.localPosition =  MyTool.SnyGridtoLocalPos(Loc.X , Loc.Y , ref GameScene.Instance.Grids );

		bIsAtking = false;
		TarIdent = 0;
	}


	public void ShowValueEffect( int nValue , int nMode )
	{
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
		BGObj.SetActive (false);
		bIsDead = true; // set dead
		bIsDeading = true;
		// shake
		TweenShake tws = TweenShake.Begin(gameObject, 1.0f , 15 );
		if( tws )
		{
			tws.shakeX = true;
			tws.shakeY = true;
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
	}

	public void OnDead()
	{	
		bIsDeading = false;
		// remove char
		StageDelUnitByIdentEvent evt = new StageDelUnitByIdentEvent ();
		//evt.eCamp  = eCampID; // no need
		evt.nIdent = this.Ident ();

		GameEventManager.DispatchEvent ( evt );
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
			tw.SetEndToCurrentValue();
			MyTool.TweenSetOneShotOnFinish( tw , OnBornFinish );
		}

	}
	public void OnBornFinish()
	{
		bIsBorning = false;
	}

	public void SetCamp( _CAMP camp )
	{
		GameDataManager.Instance.DelCampMember( eCampID , Ident() ); // global game data

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
			sp.color = new Color( 1.0f , 1.0f , 0.0f );
			break;
		}

		sp.alpha= 0.5f;

		GameDataManager.Instance.AddCampMember( camp , Ident() ); // global game data

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
	
		// find a pos 
		Dictionary< Panel_unit , int > pool = Panel_StageUI.Instance.GetUnitDistPool ( this , true );
		var items = from pair in pool orderby pair.Value ascending select pair;
		//Dictionary< Panel_unit , int > items = from pair in pool orderby pair.Value ascending select pair;
		foreach (KeyValuePair<Panel_unit , int> pair in items)
		{
			Debug.LogFormat("{0}: {1}", pair.Key, pair.Value);
			int nDist = pair.Value;
			cUnitData data = GameDataManager.Instance.GetUnitDateByIdent( pair.Key.Ident() ); 

			int nMove  = data.GetMov() ; // mob movement;
			// path find when dist > 1
			if( nDist > 1 )
			{
				// throw event
			//	GameScene.Instance.Grids.ClearIgnorePool();

				//GameScene.Instance.Grids.AddIgnorePool(  GetPKPosPool(  true )  ); // need check camp
				List< iVec2 > nearList = pair.Key.Loc.AdjacentList(); // the 4 pos can't stand ally
				Dictionary< iVec2 , int > distpool = new Dictionary< iVec2 , int >();
				foreach( iVec2 v in nearList ){
					if( Panel_StageUI.Instance.CheckIsEmptyPos( v ) == true ){// 目標 周圍 不可以站人
						distpool.Add(v , v.Dist( Loc ) );
					}
				}
				// start try each vaild pos
				nDist = pair.Value;
				iVec2 last = null;

				var itemsdist = from pair2 in distpool orderby pair2.Value ascending select pair2;
				foreach (KeyValuePair<iVec2 , int> pair2 in itemsdist)
				{
					nDist = pair.Value; // try other
					last = null;

					List< iVec2> path = Panel_StageUI.Instance.PathFinding( this  , this.Loc , pair2.Key , 999  ); // get a vaild path to run
					
					// limit out side
					path = MyTool.CutList<iVec2>( path ,nMove  );
					
					// avoid stand on invalid pos
					while( path.Count > 0 )
					{
						last = path[path.Count-1];
						if ( Panel_StageUI.Instance.CheckIsEmptyPos( last ) == false ) 
						{
							path.RemoveAt( path.Count-1 ); // then go again
							
						}
						else
						{
							// success
							pathfind = path; 
							// check if last pos is attack able pos
							ActionManager.Instance.CreateMoveAction( Ident() , last.X , last.Y );	

							
							if (Config.GOD == true) {
								Panel_StageUI.Instance.CreatePathOverEffect (pathfind); // draw path
							}


							break;
						}
					}
					if( last != null ){
						nDist = last.Dist ( pair.Key.Loc);  // final dist
						break;
					}
					else {
						nDist = -1 ; // can't find
					}




				}



			//	iVec2 last = null;
				// start pathfind
//				List< iVec2> path = Panel_StageUI.Instance.PathFinding( this  , this.Loc , pair.Key.Loc , 999  ); // get a vaild path to run
//
//				// limit out side
//				path = MyTool.CutList<iVec2>( path ,nMove  );
//
//				// avoid stand on invalid pos
//				while( path.Count > 0 )
//				{
//					last = path[path.Count-1];
//					if ( Panel_StageUI.Instance.CheckIsEmptyPos( last ) == false ) 
//					{
//						path.RemoveAt( path.Count-1 ); // then go again
//
//					}
//					else
//					{
//						// success
//						pathfind = path; 
//						// check if last pos is attack able pos
//						ActionManager.Instance.CreateMoveAction( Ident() , last.X , last.Y );	
//						break;
//					}
//				}
//				if( last != null ){
//					nDist = last.Dist ( pair.Key.Loc);  // final dist
//				}
//				else {
//					nDist = -1 ; // can't find
//				}


			}
//			int nDist = Loc.Dist (pair.Key.Loc);
//			if (nDist > 4 ) {
//				// send move act
//				iVec2 tarPos =   Panel_StageUI.Instance.FindEmptyPos( pair.Key.Loc );
//				ActionManager.Instance.CreateMoveAction( Ident() , tarPos.X , tarPos.Y );				
//				
//			}
			
			// send attack

			if ( nDist >=0 && nDist<= 1) {
				ActionManager.Instance.CreateAttackCMD( Ident (), pair.Key.Ident (), 0); // create Attack CMD . need battle manage to run
			}
			else{
				//  can't attavk . waiting only 
				ActionManager.Instance.CreateWaitingAction( Ident() );
			}


			Panel_StageUI.Instance.MoveToGameObj ( this.gameObject , false );
	//		ActionManager.Instance.CreateWaitingAction( Ident() );

			return; // one unit once time
		}
	}


	// call back func
	int	   nTweenMoveCount	= 0;		// check move is done
	public  void OnTweenNotifyMoveEnd(  )
	{
		bIsMoving = false; 
		nTweenMoveCount--;
		if (nTweenMoveCount < 0)
			nTweenMoveCount = 0;
		// Update pos
	//	Vector3 v = transform.localPosition;
	//	Loc.X = Panel_StageUI.Grids.GetGridX( v.x );
	//	Loc.Y = Panel_StageUI.Grids.GetGridY( v.y );
		//Loc = TarPos;		// update pos

		MoveNextPoint();
	}


	//====== Fight 
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
