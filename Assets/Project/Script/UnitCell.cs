using UnityEngine;
using System.Collections;
using MYGRIDS;


public enum _CellType{
	_TILE = 0,
	_THING ,
	_MOB ,
	_NPC ,
	
};


public class UnitCell : MonoBehaviour {



	// unit data
//	public int  X { set; get; } // 
//	public int  Y { set; get; } // 
	public int  Z { set; get; } // 
	public iVec2 Loc;

	// cell type
	public _CellType  Type { set; get; } // 

	void Awake(){
		Loc = new iVec2( 0 , 0 );

	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Cell utility Func 
	public int X(){ return Loc.X; } 
	public int Y(){ return Loc.Y; } 
	public void X( int x ){ Loc.X=x; } 
	public void Y( int y ){ Loc.Y=y; } 

	public iVec2 GetXY() { return Loc; }
	public void SetXY( int x , int y ) { X(x);Y(y); }



}
