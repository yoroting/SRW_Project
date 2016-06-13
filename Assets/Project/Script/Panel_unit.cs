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

	public GameObject TailObj;		// 尾巴特效
	public GameObject FxObj;		// fx

	public _CAMP 	eCampID;
	public int  	CharID;			// not identift
	public iVec2 Loc;
	public cUnitData pUnitData;  
	uAction CurAction;
	int		nSubActFlow;			// index of action run

	public List< iVec2 > PathList;
	List< iVec2 > pathfind;				// if have assign finding path

	iVec2	TarPos;					   //目標左標
	public int TarIdent { set; get ;}  //攻擊對象

//	public int  Identify;		avoid double 
	bool bOnSelected;

	//int nActionTime=1;	
	public int  	MissileCount = 0 ;          //Missile Count


    List<cHitResult> WaitMsgPool;
  //  List<int>        WaitFxPool;

    float m_fNextMsgTime;    

    public bool bIsDead = false;
	public bool bIsAtking  = false;
	public bool bIsCasting = false;
	public bool bIsShaking = false;
	public bool bIsMoving 	= false;
	public bool bIsBorning = false;
	public bool bIsDeading = false;
	public bool bIsLeaving = false;
	public bool bIsDodgeing = false;
	public bool bIsMissing = false;
	public bool bIsGuarding = false;

	public int  Ident() 
	{
		if( pUnitData != null  ){
			return pUnitData.n_Ident;
		}
		return 0;
	}
	public bool CanDoCmd()
	{
		if (pUnitData != null) {
//			if( Config.GOD ){
//				if( eCampID == _CAMP._PLAYER ){
//					return true;
//				}
//			}
			return pUnitData.nActionTime > 0;
		}
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
    void Init()
    {
        if (FaceObj != null)
        {
            FaceObj.transform.localPosition = Vector3.zero;
            UITexture tex = FaceObj.GetComponent<UITexture>();
            if (tex != null)
            {
                tex.width = Config.UnitW;
                tex.height = Config.UnitH;
            }
        }

        transform.localRotation = new Quaternion();
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        bOnSelected = false;
        bIsDead = false;
        TarIdent = 0;
        UITweener[] tws = GetComponents<UITweener>();
        foreach (UITweener tw in tws)
        {
            Destroy(tw);
        }
        PathList = null; // stop move
        WaitMsgPool.Clear();
        m_fNextMsgTime = 0.0f;

        bIsDead = false;
        bIsAtking = false;
        bIsCasting = false;
        bIsShaking = false;
        bIsMoving = false;
        bIsBorning = false;
        bIsDeading = false;
        bIsLeaving = false;
        bIsDodgeing = false;
        bIsMissing = false;
        bIsGuarding = false;
    }

    void OnEnable(){
        Init();
        
    }

	// Awake
	void Awake(){
		bOnSelected = false;
        //	nActionTime = 1;				// default is 1
        WaitMsgPool = new List<cHitResult>();
   //     WaitFxPool = new List<int>(); 
        m_fNextMsgTime = 0.0f;
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


        // process show value 
        if (ProcessHitResult())         // this wlll cause playfx
            return;

        // STOP UPDATE OTHER ACTION
        if (IsAnimate () == true) // this line will wait all FX played
			return;

        //if (WaitMsgPool.Count > 0 && (Time.time> m_fNextMsgTime) )
        //{
        //    cHitResult hitres = WaitMsgPool[0];
        //    if (hitres != null)
        //    {
        //        m_fNextMsgTime = Time.time + 0.5f;
        //        //m_fNextMsgTime = 0.0f;
        //        WaitMsgPool.RemoveAt(0);
        //    }
        //}

        // stop update when msg 
        if (BattleMsg.nMsgCount > 0)
			return ;

//        // wait all fx played
//        if ( WaitFxPool.Count > 0 ) {
//            if (FxObj != null)
//            { // detect obj is end
//                // it will be null when fx playend and auto delete
////                ParticleSystem ps = FxObj.GetComponent<ParticleSystem>();
////                if (ps.IsAlive() == true)
////                {
//                    //Debug.Log(" ps end ");
////                }
//                return; // block
//            }

//            PlayFX( WaitFxPool[0] );
//            WaitFxPool.RemoveAt(0);
//            return;
//        }
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
        // del unit may have fx with it       
//        ParticleSystem[] psAry = this.gameObject.GetComponentsInChildren<ParticleSystem>();
//        foreach (ParticleSystem ps in psAry)
//        {
//            if( ps.transform.parent == this )
//            {
//                //NGUITools.AddChild( )
//                ps.transform.parent = this.transform.parent; // change parent to unit's parent

//            }

////            Destroy(ps);
//        }
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

        //CHARS charData = ConstDataManager.Instance.GetRow<CHARS>( CharID );
        //if( charData == null)
        //	return;
        //// change face
        //string url = "Art/char/" + charData.s_FILENAME +"_S";

        SetFace(data.n_FaceID );
  //      UITexture face = FaceObj.GetComponent<UITexture>();
		//if( face != null )
		//{
		//	face.mainTexture = MyTool.GetCharTexture(data.n_FaceID);
  //      }
		
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

        // free ps
        ParticleSystem[] psAry = this.gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in psAry)
        {
            if (ps.transform.parent == this.transform )
            {              //move to parent to keep play
                ps.transform.parent = this.transform.parent; // change parent to unit's parent
            };
        }

    }

    public void SetFace( int nFaceID )
    {
        UITexture face = FaceObj.GetComponent<UITexture>();
        if (face != null)
        {
            face.mainTexture = MyTool.GetCharTexture( nFaceID );
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

        SetFace(data.n_FaceID );

		//string url = "Art/char/" + charData.s_FILENAME +"_S";
		//UITexture face = FaceObj.GetComponent<UITexture>();
		//if( face != null )
		//{
		//	face.mainTexture = Resources.Load <Texture>( url  ) ;

		//}

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

		if( bIsAtking || bIsShaking || bIsCasting || bIsBorning || bIsDeading || bIsLeaving || bIsDodgeing || bIsMissing || bIsGuarding )
			return true;

		if (IsMoving ())
			return true;

		if( MissileCount > 0 )
			return true; 

		if( BattleMsg.nMsgCount > 0 )
			return true;

//		if( CFX_AutoDestructShuriken.nFXCount>0 )
//			return true;

		if( FxObj != null )
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
            
            Panel_StageUI.Instance.OnMoveEnd(this);
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
			during = 0.1f; // always 0.2f
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
	public void MoveToTop( bool bTop = true){

        UISprite p = this.GetComponent<UISprite>();
        if (p != null) {
            if (bTop)
            {
                p.depth += 10;
            }
            else
            {
                p.depth -= 10;
            }
        }


        //UIPanel p = this.GetComponent<UIPanel> ();
        //if (p != null) {
        //		if( bTop ){
        //			p.depth += 10;
        //		}
        //		else{
        //			p.depth -= 10;
        //		}

        //}

    }

	public bool SetAction( uAction act )
	{
		if (CurAction != null)
			return false;
		//need a pool for multi act
		CurAction = act;
		nSubActFlow = 0;

		// plane 
		MoveToTop (true);
		
		//RunAction (); // don't first run. it will broke unit update flow
		return true;
	}
	public bool ActionFinished(  )
	{
		//Debug.Log ( "ActionFinished" );
		// plane 
		MoveToTop (false );

		//need a pool for multi act
		CurAction = null;
		nSubActFlow ++;

//		// check auto pop round end
//		if( false == CanDoCmd() )
//		{
//			// check need round end or not 
//			if( eCampID == _CAMP._PLAYER ){				
//				Panel_StageUI.Instance.CheckPlayerRoundEnd();
//			}
//		}


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
				if( IsAnimate() == false ){
					nSubActFlow++;
				}
				break;
			case 2:
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
				if( !IsAnimate() && BattleMsg.nMsgCount == 0 ){// wait all msg complete
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

				// check auto pop round end
				if( eCampID == _CAMP._PLAYER )
				{
					// check need round end or not 
					if(  false == CanDoCmd() ){				
						Panel_StageUI.Instance.CheckPlayerRoundEnd();
					}
				}

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
				if( IsAnimate() == false ){
					nSubActFlow++;
				}
				break;
			case 2:
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

		// swing fx
		ShowSwingFX( skillid , TarIdent , 0 , 0 );

        // trace to target unit 
        if (defer != null)
        {
            Panel_StageUI.Instance.TraceUnit(defer);
        }

        // fly item
        if (MyTool.IsSkillTag (skillid, _SKILLTAG._FLY)) {
			int nMissileID = 0;
			//string missile = "ACT_FLAME";
//			Missile missdata = null;
			if (skillid > 0) {
				SKILL skl = ConstDataManager.Instance.GetRow<SKILL> (skillid); 
				if (skl != null) {
					if (skl.n_MISSILE_ID > 0) {
						nMissileID = skl.n_MISSILE_ID ;
//						missdata = ConstDataManager.Instance.GetRow<Missile> (skl.n_MISSILE_ID); 
//						if (missdata != null) {
//							missile = missdata.s_MISSILE;
//						}
					}
				}
			}
			// attach on parent
			FightBulletFX fbFx = FightBulletFX.CreatFX (nMissileID, transform.parent, this.transform.localPosition, defer.transform.localPosition, OnTwAtkFlyHit);
			if( fbFx != null ){
				MissileCount ++;
			}
            // trace atk target
            
			// create a fly item
			return;
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._ROTATE)) {

			RotateAttack ();
			return;
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._FLASH)) {
			SKILL skl = ConstDataManager.Instance.GetRow<SKILL> (skillid); 
			if (skl != null && skl.n_AREA > 0) {
				// get aoe pool
				int nTarX = defer.Loc.X;
				int nTarY = defer.Loc.Y;
				List < iVec2 > lst = MyTool.GetAOEPool (nTarX, nTarY, skl.n_AREA, Loc.X, Loc.Y);
				FlashAttack (lst);
				return;
			}

		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._JUMP)) {
			int nTarX = defer.Loc.X;
			int nTarY = defer.Loc.Y;
			JumpAttack (nTarX, nTarY);
			return;
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._BOW)) {
		
			int nTarX = defer.Loc.X;
			int nTarY = defer.Loc.Y;
			BowAttack (nTarX, nTarY);
			return;		
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._NOACT)) { //無動作
			
			int nTarX = defer.Loc.X;
			int nTarY = defer.Loc.Y;
			NoActAttack (nTarX, nTarY);
			return;		
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._CROSS)) {
			//_CROSS
			int nTarX = defer.Loc.X;
			int nTarY = defer.Loc.Y;
			CrossAttack (nTarX, nTarY);
			return;
		}
		else if (MyTool.IsSkillTag (skillid, _SKILLTAG._AOEMISSILE)) { // 
			int nTarX = defer.Loc.X;
			int nTarY = defer.Loc.Y;
			AOEMissileAttack(  skillid , nTarX , nTarY );


            // attach on parent
            //RotateAttack(); 
            //			TweenRotation twr = TweenRotation.Begin< TweenRotation >( gameObject , 0.5f );
            //			if( twr != null )
            //			{
            //				twr.SetStartToCurrentValue();
            //				twr.to	= new Vector3( 0.0f , 0.0f , 360.0f );//Math.PI
            //				//MyTool.TweenSetOneShotOnFinish( twr , OnTwAtkRotateEnd ); // for once only
            //			}
            return;

		}
		else if (MyTool.IsSkillTag (skillid, _SKILLTAG._DANCEKILL) ) { // 
			int nTarX = defer.Loc.X;
			int nTarY = defer.Loc.Y;
			//List < iVec2 > lst = MyTool.GetAOEPool (nTarX, nTarY, skl.n_AREA, Loc.X, Loc.Y);
			// need some code


		}
		//  非攻擊型技能，跳過攻擊動作
		else if (MyTool.IsSkillTag (skillid, _SKILLTAG._DAMAGE)==false) {
			OnTwAtkHit ();
			return;
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
       


	}
	public void ActionHit( int skillid , int GridX , int GridY )
	{
		bIsAtking = true;
		SKILL skl = ConstDataManager.Instance.GetRow<SKILL> (skillid); 


		// swing fx
		ShowSwingFX( skillid , TarIdent ,GridX ,GridY );


		// attack perform
		if (MyTool.IsSkillTag (skillid, _SKILLTAG._FLY)) {
			int nMissileID = 0 ;
			//string missile = "ACT_FLAME";
			//Missile missdata = null;
			if (skl != null) {
				if (skl.n_MISSILE_ID > 0) {
					nMissileID = skl.n_MISSILE_ID;
			//		missdata = ConstDataManager.Instance.GetRow<Missile> (skl.n_MISSILE_ID); 
			//		if (missdata != null) {
			//			missile = missdata.s_MISSILE;
			//		}
				}
			}
			Vector3 vTar = MyTool.SnyGridtoLocalPos( GridX , GridY  , ref GameScene.Instance.Grids );

			//FightBulletFX fbFx = 
			FightBulletFX.CreatFX (nMissileID, transform.parent , this.transform.localPosition, vTar, OnTwAtkFlyHit);
			
			// create a fly item
			return;
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._ROTATE)) {
			RotateAttack ();
			return;
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._FLASH)) {		
			if (skl != null && skl.n_AREA > 0) {
				// get aoe pool
				int nTarX = GridX;
				int nTarY = GridY;
				List < iVec2 > lst = MyTool.GetAOEPool (nTarX, nTarY, skl.n_AREA, Loc.X, Loc.Y);
				FlashAttack (lst);
				return;
			}
			
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._JUMP)) {
			JumpAttack (GridX, GridY);
			return;
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._BOW)) {
			BowAttack (GridX, GridY);
			return;
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._NOACT)) {
			NoActAttack (GridX, GridY);
			return;		
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._CROSS)) {
			//_CROSS	
			CrossAttack (GridX, GridY);
			return;
		} else if (MyTool.IsSkillTag (skillid, _SKILLTAG._AOEMISSILE))  { // 
			AOEMissileAttack (skillid, GridX, GridY);
            return;
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
			// too long.. skip this
			//BattleManager.Instance.ShowBattleMsg( this  , MyTool.GetUnitSchoolFullName(Ident(), pUnitData.nActSch[1] ) );  // Get school name
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
				if( skl.n_TAIL_FX > 0 ){

					FX fx = ConstDataManager.Instance.GetRow< FX >( skl.n_TAIL_FX ); 
					if( fx != null ){
						ShowTailFX( fx.s_FILENAME );
					}
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
	// cirit
	public void SetCirit(  )
	{
		BattleManager.Instance.ShowBattleResValue( this.gameObject , "爆擊" , 0 );
	}

	// dodge
	public void SetDodge(  )
	{

		TweenRotation twr = TweenRotation.Begin< TweenRotation >( FaceObj, 0.5f );
		if( twr != null )
		{
			twr.SetStartToCurrentValue();
			twr.to	= new Vector3( 0.0f , 360.0f , 0.0f );//Math.PI
			MyTool.TweenSetOneShotOnFinish( twr , OnTwDodgeRotateEnd ); // for once only
			bIsDodgeing = true;
		}
		BattleManager.Instance.ShowBattleResValue( this.gameObject , "迴避" , 1 );
        GameSystem.PlaySound("Se07");

	}
	public void OnTwDodgeRotateEnd( )
	{		
		// clear all move tw
		TweenRotation[] tws = FaceObj.GetComponents<TweenRotation> (); 
		foreach (TweenRotation tw in tws) {
			Destroy( tw );
		}
        // reset pos
        FaceObj.transform.localRotation = Quaternion.identity;			
		bIsDodgeing = false;
	}
	// miss
	public void SetMiss(  )
	{
		TweenRotation twr = TweenRotation.Begin< TweenRotation >( gameObject , 0.25f );
		if( twr != null )
		{
			twr.SetStartToCurrentValue();
			twr.to	= new Vector3( 0.0f , 0.0f , -90.0f );//Math.PI
			MyTool.TweenSetOneShotOnFinish( twr , OnTwMissRotateEnd ); // for once only
			bIsMissing = true;
		}
		BattleManager.Instance.ShowBattleResValue( this.gameObject , "失誤" , 0 );
        GameSystem.PlaySound("Se38");

    }
	public void OnTwMissRotateEnd( )
	{		
		// clear all move tw
		TweenRotation[] tws = gameObject.GetComponents<TweenRotation> (); 
		foreach (TweenRotation tw in tws) {
			Destroy( tw );
		}
		// reset pos
		gameObject.transform.localRotation = Quaternion.identity;			
		bIsMissing = false;
	}

	// guard a target
	public void SetGuardTo( int TarId )
	{
		Panel_unit Tar = Panel_StageUI.Instance.GetUnitByIdent (TarId);
		if (Tar != null) {
		
			Vector3 vTar = Tar.gameObject.transform.localPosition;
			Vector3 v = MyTool.SnyGridtoLocalPos ( Loc.X , Loc.Y ,ref GameScene.Instance.Grids  );

			this.gameObject.transform.localPosition = vTar; // guard
			// move back later
			TweenPosition tw = TweenPosition.Begin< TweenPosition >( this.gameObject , 0.35f ); // always move back to start pos
			if (tw != null) {
				tw.delay = 0.2f;
				tw.from = vTar;
				tw.to = v;
				MyTool.TweenSetOneShotOnFinish( tw , OnTwGuardEnd ); // for once only

				MoveToTop();
			}
			bIsGuarding = true ;
		}
	}
	public void OnTwGuardEnd( )
	{		
		// clear all move tw
		TweenRotation[] tws = gameObject.GetComponents<TweenRotation> (); 
		foreach (TweenRotation tw in tws) {
			Destroy( tw );
		}
		// reset pos
		gameObject.transform.localRotation = Quaternion.identity;		

		MoveToTop( false );
		bIsGuarding = false;
	}

    public void SetImmune()
    {
        BattleManager.Instance.ShowBattleResValue(this.gameObject, "免疫", 1);
    }
    // Attack action animate
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
				twj2.PlayForward();
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
			twr.to	= new Vector3( 0.0f , 0.0f , 360.0f );//Math.PI
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
			tw2.PlayForward();
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

	public void NoActAttack( int GridX , int  GridY )
	{
		bIsAtking = true;
		ActionManager.Instance.ExecActionHitResult ( CurAction );	 // perform sm hit action
		bIsAtking = false;
	}

	public void CrossAttack( int GridX , int GridY )
	{
		bIsAtking = true;
		Vector3 vTar = MyTool.SnyGridtoLocalPos (GridX, GridY ,ref GameScene.Instance.Grids  );
		
		Vector3 diff =  vTar - this.transform.localPosition; 
		float mag = diff.magnitude;
		if( mag > 100.0f ){
			diff.x *= ( 100.0f / mag );  
			diff.y *= ( 100.0f / mag );  
		}
		Vector3 v = this.transform.localPosition - diff; // back

		
		TweenPosition tw = TweenPosition.Begin< TweenPosition >( this.gameObject , 0.35f ); // always move back to start pos
		if (tw != null) {
			tw.SetStartToCurrentValue();
			tw.to = v;
		}

		Vector3 v2 = vTar + diff; // corss atk pos
		TweenPosition tw2 = gameObject.AddComponent <TweenPosition>();
		if (tw2 != null) {
			tw2.duration = 0.15f;
			tw2.delay = 0.35f;
			tw2.from = v;
			tw2.to = v2;
			MyTool.TweenSetOneShotOnFinish(tw2, OnTwAtkHit );
			tw2.PlayForward();
		}


	}

	public void AOEMissileAttack( int nSkillID ,   int GridX , int GridY )
	{
		bIsAtking = true;
//		string missile = "ACT_FLAME";
//		Missile missdata = null;
		if (nSkillID > 0) {
			int nMissileID = 0;
			SKILL skl = ConstDataManager.Instance.GetRow<SKILL> (nSkillID); 
			if (skl != null) {
				if (skl.n_MISSILE_ID > 0) {
					nMissileID = skl.n_MISSILE_ID;
//					missdata = ConstDataManager.Instance.GetRow<Missile> (skl.n_MISSILE_ID); 
//					if (missdata != null) {
//						missile = missdata.s_MISSILE;
//					}
				}
				List < iVec2 > lst = MyTool.GetAOEPool (GridX, GridY, skl.n_AREA, Loc.X, Loc.Y);
				
				foreach( iVec2 v in lst ){
					FightBulletFX fbFx = FightBulletFX.CreatFX (nMissileID, transform.parent, this.transform.localPosition, MyTool.SnyGridtoLocalPos(v.X , v.Y ,  ref GameScene.Instance.Grids  ), OnTwAtkFlyHit  );
					if( fbFx != null ){
						MissileCount ++;

					}
				}
			}
		}
		if( MissileCount <= 0  ){
			ActionManager.Instance.ExecActionHitResult ( CurAction );	 // perform sm hit action
			bIsAtking = false;
		}

		TweenRotation twr = TweenRotation.Begin< TweenRotation >( gameObject , 0.5f );
		if( twr != null )
		{
			twr.SetStartToCurrentValue();
			twr.to	= new Vector3( 0.0f , 0.0f , 360.0f );//Math.PI
//			MyTool.TweenSetOneShotOnFinish( twr , OnTwAtkRotateEnd ); // for once only
		}

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
		--MissileCount;
		// wait all missile fly end
		if (MissileCount > 0) {
			return ;
		}

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


			SetShake();

			BattleManager.Instance.ShowBattleResValue( this.gameObject , nValue , nMode );
		} else if (nValue > 0) {
			// heal

			BattleManager.Instance.ShowBattleResValue( this.gameObject , nValue , nMode );
		}
        // show dmg effect
        m_fNextMsgTime = Time.time + 0.5f; // next can play time
    }

	public void SetShake()
	{
		// guard will not shake , 
		if (bIsGuarding)
			return;

		// 由於 shake結束 會重置 座標點。 造成 移動結果被還原。所以與到移動中的shake 應該要delay 一下
		float fDelay = 0;
		if( IsMoving() ){
			fDelay = (PathList.Count+1) *0.2f+0.05f ;
		}



		TweenShake tw = TweenShake.Begin(FaceObj, 0.2f , 10 ); // shake face only to avoid loc modify
		if (tw) {
			//tw.OriginPosSet = false; // Important!
			//tw.style = UITweener.Style.Once;
			tw.delay = fDelay;
			tw.shakeY = false;


			MyTool.TweenSetOneShotOnFinish( tw , OnTwShakeEnd );
			
			//tw.SetOnFinished (OnTwShakeEnd);
			bIsShaking = true;
		}
	}
	public void OnTwShakeEnd( )
	{
		FaceObj.transform.localPosition = Vector3.zero;
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
		TweenShake tws = TweenShake.Begin( FaceObj , 1.0f , 15 );
		if( tws )
		{
			tws.shakeX = true;
			tws.shakeY = true;

			Destroy( tws , 1.0f ); // 
		}

		//TweenGrayLevel
		//Vector2 vfrom = new Vector3( 1.0f , 1.0f , 1.0f );
		//Vector2 vto   = new Vector3( 0.0f , 10.0f, 1.0f );
		TweenGrayLevel tw = GrayLevelHelper.StartTweenGrayLevel(FaceObj, 1.0f);
		if (tw) {

			tw.from = 0.0f;
			tw.to   = 1.0f;
			MyTool.TweenSetOneShotOnFinish( tw , OnDead );
//			tw.style = UITweener.Style.Once; // PLAY ONCE
//			tw.SetOnFinished( OnDead );

		}

		// 死亡音效
		GameSystem.PlaySound ( "Se16");

		// free data here
		//FreeUnitData ();

	}

	public void OnDead()
	{			
        // delete ps        

        // remove char
        StageDelUnitByIdentEvent evt = new StageDelUnitByIdentEvent ();
		//evt.eCamp  = eCampID; // no need
		evt.nIdent = this.Ident ();

		GameEventManager.DispatchEvent ( evt );

        // avoid shake tween
        // auto close talk window
        TalkSayEndEvent sayevt = new TalkSayEndEvent();
        sayevt.nChar = this.CharID;
        GameEventManager.DispatchEvent(sayevt);

        bIsDeading = false;
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

		CHARS data = ConstDataManager.Instance.GetRow <CHARS>(CharID);
		if (data != null) {
			GameSystem.PlayFX (gameObject, data.n_BORN_FX );
		}
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
		//GameObject fx = 
			GameSystem.PlayFX ( this.gameObject , 205  ); // need rot x  to -75

		bIsLeaving = true;
		
		TweenWidth tw = TweenHeight.Begin<TweenWidth>(  FaceObj , 1.0f );
		if (tw != null) {
			tw.SetStartToCurrentValue();
			tw.to = 0;
			MyTool.TweenSetOneShotOnFinish( tw , OnLeaveFinish );
		}
		
	}
	public void OnLeaveFinish()
	{		
		StageDelUnitByIdentEvent evt = new StageDelUnitByIdentEvent (); // del by ident
		//evt.eCamp  = eCampID; // no need
		evt.nIdent = this.Ident ();		
		GameEventManager.DispatchEvent ( evt ); // will check bIsLeaving inside

        // importane here to avoid double leave in delunit event 
        bIsLeaving = false;
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
	public void ShowSwingFX( int nSkillID , int nTarIdent , int nX , int nY )
	{
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

		//====================== cast skill ================================
		SKILL skl = ConstDataManager.Instance.GetRow< SKILL > ( nSkillID ); 
		if (skl == null)
			return;
		FX fxData = ConstDataManager.Instance.GetRow< FX > ( skl.n_SWING_FX ); 
		if (fxData == null)
			return;

		if (fxData.n_TAG == 5) {			// tail effect
			ShowTailFX( fxData.s_FILENAME );
			return;
		}

		GameObject go = GameSystem.PlayFX ( this.gameObject , skl.n_SWING_FX );
		if (go == null) {
			return;
		}

		// rotate have 2 type to rotate
		if (fxData.n_TAG == 1) {			// 處理旋轉
			MyTool.RotateGameObjToGridXY( go , Loc.X , Loc.Y , nX, nY , fxData.n_ROT_TYPE );
		}
	}

	public void ShowSkillCastOutFX( int nSkillID , int nTarIdent , int nX , int nY )
	{
		//Debug.Log ("show skillfx");
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
		//================ cast skill =================
		SKILL skl = ConstDataManager.Instance.GetRow< SKILL > ( nSkillID ); 
		if (skl == null)
			return;
		FX fxData = ConstDataManager.Instance.GetRow< FX > ( skl.n_CASTOUT_FX ); 
		if (fxData == null)
			return;
		switch ( skl.n_CASTOUT_TYPE ) {
		case 1:// 處理旋轉
			GameObject go = GameSystem.PlayFX ( this.gameObject , skl.n_CASTOUT_FX );
			if (go != null) {

				MyTool.RotateGameObjToGridXY( go , Loc.X , Loc.Y , nX, nY , fxData.n_ROT_TYPE  );
			}
			break;
		case 2:// AOE fx 
			Panel_StageUI.Instance.PlayAOEFX (this, skl.n_CASTOUT_FX, nX, nY, skl.n_AREA);
			break;
		case 4:// play in terrain place
			Panel_StageUI.Instance.PlayFX (skl.n_CASTOUT_FX, nX, nY);
			break;
//		case 5: // change to skilldata
//			ShowTailFX( fxData.s_FILENAME );
		default:
			GameSystem.PlayFX ( this.gameObject , skl.n_CASTOUT_FX );
			break;
		}


//		FX fxData = ConstDataManager.Instance.GetRow< FX > ( skl.n_CASTOUT_FX ); 
//		if (fxData == null)
//			return;
//
//		// AOE fx 
//		if (fxData.n_TAG == 2) {
//			Panel_StageUI.Instance.PlayAOEFX (this, skl.n_CASTOUT_FX, nX, nY, skl.n_AREA);
//
//		} else if (fxData.n_TAG == 4) { // play in place
//			Panel_StageUI.Instance.PlayFX (skl.n_CASTOUT_FX, nX, nY);
//		} else if (fxData.n_TAG == 5) { // tail fx
//			ShowTailFX( fxData.s_FILENAME );
//		}
//		else {
//			GameObject go = GameSystem.PlayFX ( this.gameObject , skl.n_CASTOUT_FX );
//			if (go == null) {
//				return;
//			}
//
//			if (fxData.n_TAG == 1) {	// 處理旋轉
//				MyTool.RotateGameObjToGridXY( go , Loc.X , Loc.Y , nX, nY , fxData.n_ROT_TYPE  );
//			}
//		}
	}

	public void ShowTailFX( string sFileName , bool bClose = false)
	{
		if( TailObj != null ){
			NGUITools.Destroy( TailObj );
			TailObj = null;
		}
		if( bClose == true ){
			return;
		}

		// add tail
//		string sFileName = "";
//		switch( nType ){
//			case 0: sFileName = "TAIL_BLACK"  ; break;
//			case 1: sFileName = "TAIL_CHAOS"  ; break;
//			case 2: sFileName = "TAIL_FLAME"  ; break;
//			case 3: sFileName = "TAIL_ICE"  ; break;
//			case 4: sFileName = "TAIL_LIFE"  ; break;
//		default:
//			return;
//		}


		string path = "FX/Tail/" + sFileName;		
		GameObject instance = ResourcesManager.CreatePrefabGameObj ( this.gameObject , path );
		if( instance != null ){
			TailObj = instance;
		}
	}

	public void PlayFX( int nFXID  )
	{
        if (nFXID == 0)
            return;

        // skip mode no FX


        //本機制維護成本過高，暫時移除
        //if(bForce == true)
        //{
        //    GameSystem.PlayFX(this.gameObject, nFXID);
        //    return;
        //}

        //// normal play fx will auto queue
        //if (FxObj != null) {

        //    WaitFxPool.Add(nFXID);
        //    // push to 

        //    return;
        //}

        FxObj = GameSystem.PlayFX( this.gameObject , nFXID );
	}

    public bool ProcessHitResult()
    {
        if (WaitMsgPool == null )
            return false;
        // show value 
        if (WaitMsgPool.Count > 0 && (Time.time > m_fNextMsgTime) )
        {
            cHitResult hitres = WaitMsgPool[0];
            if (hitres != null)
            {
                PlayHitResult(hitres);
                //m_fNextMsgTime = 0.0f;
                WaitMsgPool.RemoveAt(0);
            }
        }

        return (WaitMsgPool.Count>0);
    }

    public void PlayHitResult(cHitResult res , bool bForce = false)
    {
        if (res == null)
        {          
            return;
        }
        // if some action playing .. queue it
        if ( (bForce== false) && (m_fNextMsgTime > Time.time) )
        {
            WaitMsgPool.Add(res);
            return;
        }//

        // next time can play
        //m_fNextMsgTime = Time.time + 0.2f; // add in showvalueeff

        // normal play
        switch (res.eHitType)
        {
            case cHitResult._TYPE._HP:
                {
                    ShowValueEffect(res.Value1, 0); // HP
                    if (res.Value1 != 0) // maybe change data in  battle manage
                    {
                        pUnitData.AddHp(res.Value1);
                    }

                }
                break;
            case cHitResult._TYPE._DEF:
                {

                    ShowValueEffect(res.Value1, 1); // DEF
                    if (res.Value1 != 0) // maybe change data in  battle manage
                    {
                        pUnitData.AddDef(res.Value1);
                    }

                }
                break;
            case cHitResult._TYPE._MP:
                {
                    ShowValueEffect(res.Value1, 2); // MP
                    if (res.Value1 != 0) // maybe change data in  battle manage
                    {
                        pUnitData.AddMp(res.Value1);
                    }
                }
                break;
            case cHitResult._TYPE._SP:
                {
                    ShowValueEffect(res.Value1, 3); // SP
                    if (res.Value1 != 0) // maybe change data in  battle manage
                    {
                        pUnitData.AddSp(res.Value1);
                    }
                }
                break;
            case cHitResult._TYPE._CP:
                {
                    //pUnit.ShowValueEffect( res.Value1 , 0 ); // CP
                    if (res.Value1 != 0) // maybe change data in  battle manage
                    {
                        pUnitData.AddCp(res.Value1);
                    }
                }
                break;
            case cHitResult._TYPE._ACTTIME:
                {
                    //pUnit.ShowValueEffect( res.Value1 , 0 ); // SP
                    if (res.Value1 != 0) // maybe change data in  battle manage
                    {
                        pUnitData.AddActionTime(res.Value1);
                    }
                }
                break;

            case cHitResult._TYPE._ADDBUFF: // add buff
                {
                   
                    if (res.Value1 != 0) // maybe change data in  battle manage
                    {
                        // cUnitData pData = GameDataManager.Instance.GetUnitDateByIdent(res.Ident);
                        // if (pData != null)
                        // {
                        pUnitData.Buffs.AddBuff(res.Value1, res.Value2, res.Value3, res.Value4);
                        // }

                        m_fNextMsgTime = Time.time + 0.5f;
                    }

                }
                break;
            case cHitResult._TYPE._DELBUFF: // remove buff
                {
                    if (res.Value1 != 0) // maybe change data in  battle manage
                    {
                        //  cUnitData pData = GameDataManager.Instance.GetUnitDateByIdent(res.Ident);
                        //  if (pData != null)
                        {
                            pUnitData.Buffs.DelBuff(res.Value1);
                        }
                    }
                }
                break;

            case cHitResult._TYPE._DELSTACK: // remove buff by stack
                {
                    if (res.Value1 != 0) // maybe change data in  battle manage
                    {
                        //  cUnitData pData = GameDataManager.Instance.GetUnitDateByIdent(res.Ident);
                        //  if (pData != null)
                        {
                            if (pUnitData.Buffs.DelBuffByStack(res.Value1)) {
                                // check need play fx
                                PlayFX( 206 );
                            }
                        }
                    }
                }
                break;
            case cHitResult._TYPE._BEHIT: // be Hit fX
                {
                    int nhitFX = 203;// default  
                    int nHitRes = res.Value2;

                    if (nHitRes == (int)_FIGHTSTATE._BLOCK)// 格檔
                    {
                        nhitFX = 412;
                    }
                    else if (nHitRes == (int)_FIGHTSTATE._PARRY)// 招架
                    {
                        nhitFX = 411;
                    }
                    else // normal hit
                    {
                        SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(res.Value1);
                        if (skl != null)
                        {
                            nhitFX = skl.n_HIT_FX;  // skill data may cancel hit fx to 0
                        }
                    }
                    //if( nhitFX == 0)
                    //	nhitFX = 203;
                    PlayFX( nhitFX ); // hit effect is direct play

                }
                break;
            case cHitResult._TYPE._SHIELD: // shield fX
                {
                    int nShieldFX = 400;
                    PlayFX(nShieldFX); // shield  effect is direct play
                }
                break;

            default:
                break;

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
