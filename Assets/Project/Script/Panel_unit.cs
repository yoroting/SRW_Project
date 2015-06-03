using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MYGRIDS;
using MyClassLibrary;
using _SRW;
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
	public UNIT_DATA pUnitData;  // always need to check before use it
	public List< iVec2 > PathList;

	iVec2	TarPos;					   //目標左標
	public int TarIdent { set; get ;}  //攻擊對象

//	public int  Identify;		avoid double 
	bool bOnSelected;

	int nActionTime=1;			//

	public int  Ident() 
	{
		if( pUnitData != null  ){
			return pUnitData.n_Ident;
		}
		return 0;
	}
	public bool CanAction()
	{
		return nActionTime>0;
	}

	// ensure Loc exist
	public Panel_unit()
	{
		Loc 	= new iVec2( 0 , 0 );
		TarPos  = new iVec2( 0 , 0 );
	}

	// Awake
	void Awake(){
		bOnSelected = false;
		nActionTime = 1;				// default is 1

		//ParticleSystemRenderer

		GameObject instance = ResourcesManager.CreatePrefabGameObj ( this.gameObject ,"FX/Cartoon FX/CFXM4 Splash" );

		ParticleSystem ps =instance.GetComponent< ParticleSystem>();
		if (ps!= null) {

		}
		ParticleSystemRenderer psr =instance.GetComponent< ParticleSystemRenderer>();
		if (psr != null) {
			psr.sortingLayerName = "FX";
		}

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
		if (nActionTime <= 0) {
			nActionTime = 0;
			MaskObj.SetActive (true);
		} else {
			MaskObj.SetActive (false);
		}
	}

	void OnDestory () {
		GameDataManager.Instance.DelUnit( pUnitData );
	}

	//click
//	public void OnClick( Panel_StageUI Stage )
//	{
		// 查情報
//	}

	public void ActionFinished(  )
	{
		nActionTime--;

	}

	public void AddActionTime( int nTime )
	{
		nActionTime +=nTime ;		
	}

	// Cell utility Func 
	public int X(){ return Loc.X; } 
	public int Y(){ return Loc.Y; } 
//	public void X( int x ){ Loc.X=x; } 
//	public void Y( int y ){ Loc.Y=y; } 
	
	public iVec2 GetXY() { return Loc; }
	public void SetXY( int x , int y ) {
		Loc.X = x;
		Loc.Y = y;
		if( GameScene.Instance != null )
			gameObject.transform.localPosition =  MyTool.SnyGridtoLocalPos( x , y , ref GameScene.Instance.Grids ) ; 
	}

	public void CreateChar( int nCharID , int x , int y )
	{
		CharID = nCharID;
		SetXY( x , y );
		pUnitData = GameDataManager.Instance.CreateChar( nCharID );
		if( pUnitData == null )
			return;


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

		PathList =  GameScene.Instance.Grids.GetPathList( Loc , TarPos ) ;
		MoveNextPoint();
	}

	public bool IsAnimate()
	{
		if( nTweenMoveCount != 0 )
			return true;

		return false;
	}

	public bool IsMoving()
	{
		if( nTweenMoveCount != 0 )
			return true;

		if( (PathList!=null) && PathList.Count > 0  )
			return true;

		return false; 
	}
	public void MoveNextPoint( )
	{
		if( (PathList== null)   )
			return ;
		if ((PathList.Count <= 0)) {
			// move end
			PathList = null;
		
			return ;
		}


		iVec2 v = PathList[0];
		PathList.RemoveAt( 0 );

		// avoid the same point
		if ( v.Collision(Loc) )
			return;
		//TarPos = v;

		Vector3 tar = this.gameObject.transform.localPosition;
		tar.x =  GameScene.Instance.Grids.GetRealX( v.X );
		tar.y =  GameScene.Instance.Grids.GetRealY( v.Y );


		int iDist = Loc.Dist( v );
		float during = iDist* (0.2f);

		// cal target location position

		Loc = v; // record target pos as current pos
		// Tween move
		TweenPosition tw = TweenPosition.Begin( this.gameObject , during , tar );
		if( tw )
		{
			nTweenMoveCount++;	
			tw.SetOnFinished( OnTweenNotifyMoveEnd );
		}

	}

	// 待機中 可下命令
	bool IsIdle()
	{
		if (IsMoving ())
			return false;

		return true;
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



	public void SetDead()
	{
		TweenScale tw = TweenScale.Begin <TweenScale >(this.gameObject, 1.0f);
		if (tw) {
			Vector2 vfrom = new Vector3( 1.0f , 1.0f , 1.0f );
			Vector2 vto   = new Vector3( 0.0f , 10.0f, 1.0f );
			tw.from = vfrom;
			tw.to   = vto;
			tw.style = UITweener.Style.Once; // PLAY ONCE
			tw.SetOnFinished( OnDead );
		}
	}

	public void OnDead()
	{	// remove char
		StageDelUnitByIdentEvent evt = new StageDelUnitByIdentEvent ();
		evt.eCamp  = eCampID;
		evt.nIdent = this.Ident ();

		GameEventManager.DispatchEvent ( evt );
	}


	public void SetCamp( _CAMP camp )
	{
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


	}
	// enemy use
	public void RunAI( )
	{
		// select to wait
		BattleManager.Instance.ShowBattleMsg (this, "waiting");
		ActionFinished ();
	}

	// call back func
	int	   nTweenMoveCount	= 0;		// check move is done
	public  void OnTweenNotifyMoveEnd(  )
	{
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
}
