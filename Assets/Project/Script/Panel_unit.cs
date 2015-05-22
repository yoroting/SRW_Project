using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MYGRIDS;
using MyClassLibrary;

public class Panel_unit : MonoBehaviour {

	public GameObject FaceObj;
	public GameObject HpBarObj;
	public GameObject DefBarObj;

	public int  CharID;
	public iVec2 Loc;
	public UNIT_DATA pUnitData;  // always need to check before use it
	public List< iVec2 > PathList;

	iVec2	TarPos;
//	public int  Identify;		avoid double 

	public int  ID() 
	{
		if( pUnitData != null  ){
			return pUnitData.n_Ident;
		}
		return 0;
	}

	// ensure Loc exist
	public Panel_unit()
	{
		Loc 	= new iVec2( 0 , 0 );
		TarPos  = new iVec2( 0 , 0 );
	}

	// Awake
	void Awake(){

		
	}
	// Use this for initialization
	void Start () {
		// change Texture

	}
	
	// Update is called once per frame
	void Update () {

		// check if need to move
		if( IsMoveing() == false )
		{
			MoveNextPoint();			// auto move
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

	public void OnDoCmd(  )
	{

	}

	// Cell utility Func 
	public int X(){ return Loc.X; } 
	public int Y(){ return Loc.Y; } 
	public void X( int x ){ Loc.X=x; } 
	public void Y( int y ){ Loc.Y=y; } 
	
	public iVec2 GetXY() { return Loc; }
	public void SetXY( int x , int y ) { X(x);Y(y); }

	public void CreateChar( int nCharID , int x , int y )
	{
		pUnitData = GameDataManager.Instance.CreateChar( nCharID );
		if( pUnitData == null )
			return;

		SetXY( x , y );
	}

	public void MoveTo( int x , int y )
	{
		PathList = Panel_StageUI.Grids.GetPathList( Loc , new iVec2( x ,y ) ) ;

		MoveNextPoint();
	}
	public bool IsMoveing()
	{
		if( nTweenMoveCount > 0 )
			return true;
		if( (PathList!=null) && PathList.Count > 0  )
			return true;

		return false; 
	}
	public void MoveNextPoint( )
	{
		if( (PathList== null) || (PathList.Count <= 0)  )
			return ;
		iVec2 v = PathList[0];
		PathList.RemoveAt( 0 );
		TarPos = v;

		int iDist = Loc.Dist( v );
		float during = iDist* (0.5f);

		// cal target location position
		Vector3 tar = this.gameObject.transform.localPosition;
		tar.x =  Panel_StageUI.Grids.GetRealX( v.X );
		tar.y =  Panel_StageUI.Grids.GetRealY( v.Y );
		// Tween move
		TweenPosition tw = TweenPosition.Begin( this.gameObject , during , tar );
		if( tw )
		{
			nTweenMoveCount++;	
			tw.SetOnFinished( OnTweenNotifyMoveEnd );
		}

	}

	// call back func
	int	   nTweenMoveCount	= 0;		// check move is done
	public  void OnTweenNotifyMoveEnd(  )
	{
		nTweenMoveCount--;
		// Update pos
		Loc = TarPos;		// update pos

		MoveNextPoint();
	}
}
