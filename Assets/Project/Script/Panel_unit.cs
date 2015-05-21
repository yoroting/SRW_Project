using UnityEngine;
using System.Collections;
using MYGRIDS;
using MyClassLibrary;

public class Panel_unit : MonoBehaviour {

	public GameObject FaceObj;
	public GameObject HpBarObj;
	public GameObject DefBarObj;

	public int  CharID;
	public iVec2 Loc;
	public UNIT_DATA pUnitData;  // always need to check before use it


	// ensure Loc exist
	public Panel_unit()
	{
		Loc = new iVec2( 0 , 0 );
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
	
	}

	void OnDestory () {
		GameDataManager.Instance.DelUnit( pUnitData );
	}

	//click
	void OnUnitClick(GameObject go)
	{
		// 查情報
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

}
