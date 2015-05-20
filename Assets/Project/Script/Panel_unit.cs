﻿using UnityEngine;
using System.Collections;
using MYGRIDS;
using MyClassLibrary;

public class Panel_unit : MonoBehaviour {

	public GameObject FaceObj;
	public GameObject HpBarObj;
	public GameObject DefBarObj;

	public int  CharID;
	public iVec2 Loc;

	// Awake
	void Awake(){
		Loc = new iVec2( 0 , 0 );
		
	}
	// Use this for initialization
	void Start () {
		// change Texture

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestory () {

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


}