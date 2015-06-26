﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;


public class cEffect
{
	public enum _EFF
	{
		_NULL = 0,		// No eff
		ADDBUFF_I,
		ADDBUFF_E,
		ADD_MAR,
		
		ADD_MAR_DIFF,	//如果有敵對目標，提升自身
		
	};
	
	public _EFF eType;
	//	public cTextFunc func;		// func Data
	
	//string strFunc;		// origin al string
	//public int nID { set; get; }
	
	public virtual void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr ){ }
	
	
	public virtual void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ }				//
	public virtual void _OnCasting( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list){ }		// hit a target
	public virtual void _OnHit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ }			// 
	public virtual void _OnBeHit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ }		// 
	
}

// Cast Effect
public class ADDBUFF_I: cEffect
{
	public ADDBUFF_I( int buffid ){		eType = _EFF.ADDBUFF_I; nBuffID = buffid;	}
	public int nBuffID ;
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){
		if (Atker != null) {
			list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF ,Atker.n_Ident , nBuffID ) );
		}
	}
}
public class ADDBUFF_E: cEffect
{
	public ADDBUFF_E( int buffid ){		eType = _EFF.ADDBUFF_E; nBuffID = buffid;	}
	public int nBuffID ;
	
	
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		if (Defer != null) {
			list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF ,Defer.n_Ident , nBuffID ) );
		}
	}
}



// Attr
public class ADD_MAR: cEffect
{
	public ADD_MAR( float f ){		eType = _EFF.ADD_MAR; fValue = f;	}
	
	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.f_MAR += fValue;
	}
}

public class ADD_MAR_DIFF: cEffect
{
	public ADD_MAR_DIFF( float f ){		eType = _EFF.ADD_MAR_DIFF; fValue = f;	}
	
	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		if ((Atker != null) && ( Defer != null)) {
			float fDelt = Defer.GetBaseMar() - Atker.GetBaseMar();
			fDelt *=fValue;
			// this is final
			attr.f_MAR += fDelt;
		}
	}
}

// use to cache condition sctipr parser result
public class  cEffectCondition
{
	//	public enum _CON
	//	{
	//		_NULL = 0,		// No cond
	//		BUFF_I,
	//		BUFF_E,
	//		MAR_I,
	//		MAR_E,
	//	};
	//	public _CON eType;
	
	List<  List<cTextFunc> > CondLst;		// multi array.
	
	public void Add( CTextLine line )
	{
		if (CondLst == null)
			CondLst = new List<  List<cTextFunc> > ();
		if (line == null)
			return;
		CondLst.Add (line.GetFuncList ());
	}
	
	private bool CheckCond( cUnitData data_I , cUnitData data_E , List<cTextFunc> funcList  )
	{
		if (funcList == null)
			return false;
		
		foreach( cTextFunc func in funcList )
		{
			if( func.sFunc == "GO" )
			{
				
				return true;		// always true
			}
			else if( func.sFunc == "NULL" || func.sFunc == "0" )
			{
				return   false;		// always fail
			}
			if( func.sFunc == "HP_I"  )
			{
				return   false;		// always fail
			}
			else if( func.sFunc == "HP_E"  )
			{
				return   false;		// always fail
			}
			else if( func.sFunc == "MAR_I"  )
			{
				float f1 = data_I.GetMar();
				float f2 = 0.0f;
				if( func.S( 1 ) == "E" )
				{
					if( data_E != null ){
						f2 = data_E.GetMar();
					}else{
						return false; // no enemy is false
					}
				}
				else{
					f2 = func.F( 1 );
				}
				
				if( MyScript.Instance.ConditionFloat( f1 , func.S(0) ,f2  ) == false  ){
					return   false;		// always fail
				}
			}
			else if( func.sFunc == "MAR_E"  )
			{					
				return   false;		// always fail
			}
			else if( func.sFunc == "BUFF_I"  )
			{
				return   false;		// always fail
			}
			else if( func.sFunc == "BUFF_E"  )
			{
				return   false;		// always fail
			}
			else if( func.sFunc == "SCHOOL_I"  )
			{
				
				return false;
			}
			else if( func.sFunc == "SCHOOL_E"  )
			{
				
				return false;
			}
			else if( func.sFunc == "SKILL_I"  )
			{
				
				return false;
			}
			else if( func.sFunc == "SKILL_E"  )
			{
				
				return false;
			}
			else if( func.sFunc == "RANGE_E"  )
			{
				
				return false;
			}
			
			else{
				Debug.LogError( string.Format( "Error-Can't find script cond func '{0}'" , func.sFunc ) );
			}
		}
		return true;
	}
	
	
	public bool Check( cUnitData data_I , cUnitData data_E ){
		
		if (CondLst == null)
			return false;
		
		foreach( List<cTextFunc> funcList in CondLst )
		{
			if(CheckCond( data_I ,data_E ,funcList  ) == true ){
				return true;			// any one passed. all passed
			}
		}
		return false;
	}
}

//public class BUFF_I: cEffectCondition
//{
//	public BUFF_I( int nBuffid ){
//	}
//	int nBuffID ;
//
//	override public bool Check( cUnitData I , cUnitData E ){
//		if (I != null) {
//			return I.Buff.HaveBuff(nBuffID  );
//		}
//		return false;
//	}
//}

//
//public class cBuffCondition
//{
//	public int nID { set; get; }
//}