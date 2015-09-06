﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;
using MYGRIDS;

public class cEffect
{
//	public enum _EFF
//	{
//		_NULL = 0,		// No eff
//		ADDBUFF_I,
//		ADDBUFF_E,
//		ADD_MAR,
//		
//		ADD_MAR_DIFF,	//如果有敵對目標，提升自身
//		
//	};
//	
//	public _EFF eType;
	//	public cTextFunc func;		// func Data
	
	//string strFunc;		// origin al string
	//public int nID { set; get; }
	public int 	 iValue =0;	
	public float fValue=0.0f ;	
	public int nSkillID =0;
	public int nBuffID =0;

//	cEffect( int skilid , int buffid ){
//		nSkillID = skilid;	nBuffID = buffid;
//	}
	public void SetBaseParam( int sillid , int buffid ){
		nSkillID = sillid; nBuffID = buffid;
	}


	public virtual void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr ){ }
	
																											// SKill
	public virtual void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ }				//  casr/ hit event will run this


	public virtual void _Cast( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list){ }		// hit a target
	public virtual void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ }			// BUFF 專用.命中後　額外效果
	public virtual void _BeHit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ }			// BUFF 專用.受擊後　額外效果 

	// cost
	public virtual bool _IsStatus( _FIGHTSTATE st  ){ return false; }				// check user in one status	

	public virtual bool _IsTag(  _UNITTAG tag ){ return false; }								// check unit extra tag

	public virtual int _UpSkill( int nBaseSkillID  ){ return nBaseSkillID;	} 					// 判斷有無進階的武功招式
}

// Cast Effect
public class ADDBUFF_I: cEffect
{
	public ADDBUFF_I( int buffid ){		iValue = buffid;	}
	public int iValue ;
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){
		if (Atker != null) {
		
			int nDefId = 0;			if( Defer != null )nDefId = Defer.n_Ident;
			list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF , Atker.n_Ident , iValue , Atker.n_Ident, nSkillID ,nDefId  ) );
		}
	}
}
public class ADDBUFF_E: cEffect
{
	public ADDBUFF_E( int buffid ){		iValue = buffid;;	}
	public int iValue ;
	
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		if (Defer != null) {
			if( Defer.IsStates( _FIGHTSTATE._DODGE ) )
				return ;

		//	list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF ,Defer.n_Ident , nBuffID ) );
			int nAtkId = 0;			if( Atker != null )nAtkId = Atker.n_Ident;
			list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF , Defer.n_Ident , iValue, nAtkId , nSkillID , Defer.n_Ident  ) );
		}
	}
}

// Hit effect
public class HITBUFF_I: cEffect
{
	public HITBUFF_I( int buffid ){		iValue = buffid;	}
	public int iValue ;
	override public void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){
		if (Atker != null) {
			// ( int nBuffID , int nCastIdent , int nSkillID  , int nTargetId )
			//pData.Buffs.AddBuff( res.Value1 , res.Value2, res.SkillID, res.Value3 );
			int nDefId = 0;			if( Defer != null )nDefId = Defer.n_Ident;
			list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF , Atker.n_Ident , iValue , Atker.n_Ident, nSkillID ,nDefId  ) );
		}
	}
}

public class HITBUFF_E: cEffect
{
	public HITBUFF_E( int buffid ){		iValue = buffid;;	}

	
	override public void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		if (Defer != null) {
			if( Defer.IsStates( _FIGHTSTATE._DODGE ) )
				return ;
			//	list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF ,Defer.n_Ident , nBuffID ) );
			int nAtkId = 0;			if( Atker != null )nAtkId = Atker.n_Ident;
			list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF , Defer.n_Ident , iValue, nAtkId , nSkillID ,Defer.n_Ident  ) );
		}
	}
}
// upgrade skill
public class UP_SKILL: cEffect
{
	public UP_SKILL( int baseid, int upid ){iValue = baseid; iValue2=upid;}
	public int iValue2 ;
	override public int _UpSkill( int nBaseSkillID  )
	{
		if (nBaseSkillID == iValue)
			return  iValue2;
		return nBaseSkillID;
	}
}


// Attr
public class ADD_MAR: cEffect
{
	public ADD_MAR( float f ){		fValue = f;	}

	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.f_MAR += fValue;
	}
}

public class ADD_MAR_DIFF: cEffect
{
	public ADD_MAR_DIFF( float f ){	 fValue = f;	}
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		if ((Atker != null) && ( Defer != null)) {
			float fDelt = Defer.GetBaseMar() - Atker.GetBaseMar();
			fDelt *=fValue;
			// this is final
			attr.f_MAR += fDelt;
		}
	}
}

public class ADD_ATTACK_DIFF: cEffect
{
	public ADD_ATTACK_DIFF( float f ){	fValue = f;	}

	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		if ((Atker != null) && ( Defer != null)) {
			float fDelt = Defer.GetBaseAttack() - Atker.GetBaseAttack();
			fDelt *=fValue;
			// this is final
			attr.n_ATK += (int)fDelt;
		}
	}
}

public class ADD_ATTACK: cEffect
{
	public ADD_ATTACK( int i ){	iValue = i;	}
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		if ((Atker != null)) {
			attr.n_ATK += iValue;
		}
	}
}

public class ADD_MAXDEF: cEffect
{
	public ADD_MAXDEF( int i ){	iValue = i;	}
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		if ((Atker != null)) {
			attr.n_DEF += iValue;
		}
	}
}

public class ADD_DEF_I: cEffect
{
	public ADD_DEF_I( int i ){	iValue = i;	}
	
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
			list.Add( new cHitResult( cHitResult._TYPE._DEF , Atker.n_Ident , iValue , Atker.n_Ident, nSkillID , nBuffID   ) );
	}
}
public class ADD_DEF_E: cEffect
{
	public ADD_DEF_E( int i ){	iValue = i;	}
	
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		if (Defer != null) {
			list.Add( new cHitResult( cHitResult._TYPE._DEF , Defer.n_Ident , iValue , Atker.n_Ident, nSkillID , nBuffID   ) );
		}
	}
}
public class ADD_POWER: cEffect
{
	public ADD_POWER( int i ){	iValue = i;	}
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		if ((Atker != null)) {
			attr.n_POW += iValue;
		}
	}
}

public class ADD_MAXHP: cEffect
{
	public ADD_MAXHP( int i ){	iValue = i;	}
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		if ((Atker != null)) {
			attr.n_HP += iValue;
		}
	}
}
public class ADD_MAXMP: cEffect
{
	public ADD_MAXMP( int i ){	iValue = i;	}
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		if ((Atker != null)) {
			attr.n_MP += iValue;
		}
	}
}
public class ADD_MAXSP: cEffect
{
	public ADD_MAXSP( int i ){	iValue = i;	}
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		if ((Atker != null)) {
			attr.n_SP += iValue;
		}
	}
}

public class ADD_MOVE: cEffect
{
	public ADD_MOVE( int n ){		nValue = n;	}
	public int nValue ;
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.n_MOV += nValue;
	}
}

public class ADDHP_I: cEffect
{
	public ADDHP_I( float f , int i ){	fValue = f;	iValue = i; }

	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
			float fHp = Atker.GetMaxHP() * fValue ;
			//	list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF ,Defer.n_Ident , nBuffID ) );
		list.Add( new cHitResult( cHitResult._TYPE._HP , Atker.n_Ident , (int)fHp + iValue , Atker.n_Ident, nSkillID , nBuffID   ) );
	}
}
public class ADDMP_I: cEffect
{
	public ADDMP_I( float f , int i ){	fValue = f;	iValue = i;	}
	
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 	
		float fMp = Atker.GetMaxMP() * fValue ;
		list.Add( new cHitResult( cHitResult._TYPE._MP , Atker.n_Ident , (int)fMp +iValue  , Atker.n_Ident, nSkillID , nBuffID   ) );
	}
}
public class ADDSP_I: cEffect
{
	public ADDSP_I( int i ){	iValue = i;	}
	
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 

		list.Add( new cHitResult( cHitResult._TYPE._SP , Atker.n_Ident , iValue  , Atker.n_Ident, nSkillID , nBuffID   ) );
	}
}
public class ADDCP_I: cEffect
{
	public ADDCP_I( int i ){	iValue = i;	}
	
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		
		list.Add( new cHitResult( cHitResult._TYPE._CP , Atker.n_Ident , iValue  , Atker.n_Ident, nSkillID , nBuffID   ) );
	}
}
public class ADDHP_E: cEffect
{
	public ADDHP_E(float f , int i ){	fValue = f;	iValue = i;	}
	
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		if (Defer != null) {
			float fHp = Defer.GetMaxHP() * fValue ;
			list.Add( new cHitResult( cHitResult._TYPE._HP , Defer.n_Ident , (int)fHp +iValue , Atker.n_Ident, nSkillID , nBuffID   ) );
		}
	}
}
public class ADDMP_E: cEffect
{
	public ADDMP_E(float f , int i ){	fValue = f;	iValue = i;	}
	
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 	
		if (Defer != null) {
			float fMp = Defer.GetMaxMP() * fValue ;
			list.Add( new cHitResult( cHitResult._TYPE._MP , Defer.n_Ident , (int)fMp +iValue , Atker.n_Ident, nSkillID , nBuffID   ) );
		}
	}
}
public class ADDSP_E: cEffect
{
	public ADDSP_E( int i ){	iValue = i;	}
	
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		if (Defer != null) {			
			list.Add( new cHitResult( cHitResult._TYPE._SP , Defer.n_Ident , iValue , Atker.n_Ident, nSkillID , nBuffID   ) );
		}

	}
}
public class ADDCP_E: cEffect
{
	public ADDCP_E( int i ){	iValue = i;	}
	
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		
		list.Add( new cHitResult( cHitResult._TYPE._CP , Defer.n_Ident , iValue  , Atker.n_Ident, nSkillID , nBuffID   ) );
	}
}
// ==== MUL_BRUST
public class MUL_DROP: cEffect
{
	public MUL_DROP( float v ){ fValue = v;}
	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fDropRate += fValue;
	}
}

public class MUL_BRUST: cEffect
{
	public MUL_BRUST( float v ){ fValue = v;}
	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fBurstRate += fValue;
	}
}

public class MUL_DAMAGE: cEffect
{
	public MUL_DAMAGE( float v ){fValue = v;}
	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fDamageRate += fValue;
	}
}

public class MUL_ATTACK: cEffect
{
	public MUL_ATTACK( float v ){fValue = v;}
	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fAtkRate += fValue;
	}
}

public class MUL_DEF: cEffect
{
	public MUL_DEF( float v ){fValue = v;}
	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fDefRate += fValue;
	}
}

public class MUL_POWER: cEffect
{
	public MUL_POWER( float v ){fValue = v;}
	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fPowRate += fValue;
	}
}

public class MUL_MAXHP: cEffect
{
	public MUL_MAXHP( float v ){ fValue = v; }
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		if ((Atker != null)) {
			attr.fHpRate += fValue;
		}
	}
}
public class MUL_MAXMP: cEffect
{
	public MUL_MAXMP( float v ){ fValue = v; }
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		if ((Atker != null)) {
			attr.fMpRate += fValue;
		}
	}
}
public class MUL_MAXSP: cEffect
{
	public MUL_MAXSP( float v ){ fValue = v; }
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		if ((Atker != null)) {
			attr.fSpRate += fValue;
		}
	}
}


//==== cost ==
public class MUL_MPCOST: cEffect
{
	public MUL_MPCOST( float v ){ fValue = v;}
	public float fValue ;
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fMpCostRate += fValue;
	}
}


public class RECOVER_SP: cEffect
{
	public RECOVER_SP( float v ){ fValue = v;}
	public float fValue ;
//	override public void _Mul_SpCost( ref float value ){ 
//		value += F;
//	}
}

//==== tag status ==
public class TAG_CHARGE: cEffect
{
	public TAG_CHARGE(  ){ }	
	
	override public bool _IsTag(  _UNITTAG tag ){
		return (_UNITTAG._CHARGE == tag );
	}				// check user in one status		
}
public class TAG_NODIE: cEffect
{
	public TAG_NODIE(  ){ }	
	
	override public bool _IsTag(  _UNITTAG tag ){
		return (_UNITTAG._NODIE == tag );
	}				// check user in one status		
}
//==== is status ==
public class IS_DODGE: cEffect
{
	public IS_DODGE(  ){ }	

	override public bool _IsStatus(  _FIGHTSTATE st ){
		return (_FIGHTSTATE._DODGE == st );
	}				// check user in one status		
}
public class IS_CIRIT: cEffect
{
	public IS_CIRIT( ){	}	
	
	override public bool _IsStatus(  _FIGHTSTATE st ){
		return (st == _FIGHTSTATE._CIRIT);
	}				// check user in one status		
}
public class IS_MERCY: cEffect
{
	public IS_MERCY( ){	}	
	
	override public  bool _IsStatus(  _FIGHTSTATE st ){
		return (st == _FIGHTSTATE._MERCY);
	}				// check user in one status		
}
public class IS_GUARD: cEffect
{
	public IS_GUARD( ){	}	
	
	override public  bool _IsStatus(  _FIGHTSTATE st ){
		return (st == _FIGHTSTATE._GUARD);
	}				// check user in one status		
}
public class IS_THROUGH: cEffect
{
	public IS_THROUGH( ){	}	
	
	override public bool _IsStatus(  _FIGHTSTATE st ){
		return (st == _FIGHTSTATE._THROUGH);
	}				// check user in one status		
}
//==========================================================================
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
	
	private bool CheckCond( cUnitData data_I , cUnitData data_E , List<cTextFunc> funcList, int nSkillID, int  nBuffID   )
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
			else if( func.sFunc == "RATE"  )
			{
				int Rate = func.I (0);
				int nRoll = Random.Range (0, 100);				
				return ( Rate > nRoll );
				//return data_I.IsStates( _FIGHTSTATE._DODGE );
			}
			else if( func.sFunc == "MRATE"  )  // 必須比兩者武功差值
			{
				if( data_E == null || data_I == null )
					return false;
				int Rate = func.I (0);
				Rate += (int)(( data_I.GetMar() - data_E.GetMar() )* 0.5f);
				int nRoll = Random.Range (0, 100);				
				return ( Rate > nRoll );

			}
			if( func.sFunc == "HP_I"  )
			{
				float f1 = data_I.n_HP /data_I.GetMaxHP() ;
				float f2 =  func.F( 1 ) ;
				if( MyScript.Instance.ConditionFloat( f1 , func.S(0) ,f2  ) == false  ){
					return   false;		//  fail
				}
			}
			else if( func.sFunc == "HP_E"  )
			{
				float f1 = data_E.n_HP /data_E.GetMaxHP() ;
				float f2 =  func.F( 1 ) ;
				if( MyScript.Instance.ConditionFloat( f1 , func.S(0) ,f2  ) == false  ){
					return   false;		//  fail
				}
			}
			else if( func.sFunc == "MAR_I"  )
			{
				float f1 = data_I.GetMar();
				float f2 = 0.0f;
				string s1 = func.S( 1 );  // s1 
				if( s1 == "E" ){
					if( data_E != null ){
						f2 = data_E.GetMar();
					}else{
						return false; // no enemy is false
					}
				}
				if( s1 == "I" ){
					if( data_I != null ){
						f2 = data_I.GetMar();
					}else{
						return false; // no enemy is false
					}
				}
				else{
					f2 = func.F( 1 );
				}
				//
				if( MyScript.Instance.ConditionFloat( f1 , func.S(0) ,f2  ) == false  ){
					return   false;		// always fail
				}
			}
			else if( func.sFunc == "MAR_E"  )
			{		
				float f1 = data_E.GetMar();
				float f2 = 0.0f;
				string s1 = func.S( 1 );  // s1 
				if( s1 == "E" ){
					if( data_E != null ){
						f2 = data_E.GetMar();
					}else{
						return false; // no enemy is false
					}
				}
				if( s1 == "I" ){
					if( data_I != null ){
						f2 = data_I.GetMar();
					}else{
						return false; // no enemy is false
					}
				}
				else{
					f2 = func.F( 1 );
				}
				//
				if( MyScript.Instance.ConditionFloat( f1 , func.S(0) ,f2  ) == false  ){
					return   false;		// always fail
				}			
			}
			else if( func.sFunc == "BUFF_I"  )
			{
				int buffid = func.I(0);
				if( data_I!= null ){
					if( !data_I.Buffs.HaveBuff( buffid ) )
						return false;
				}
				return   false;		// always fail
			}
			else if( func.sFunc == "BUFF_E"  )
			{
				int buffid = func.I(0);
				if( data_E != null ){
					if( !data_E.Buffs.HaveBuff( buffid ) )
						return false;
				}
				return   false;		// always fail
			}
			else if( func.sFunc == "SCHOOL_I"  )
			{
				int schoolid = func.I(0);
				if( data_I.nActSch[0] != schoolid && data_I.nActSch[1] != schoolid   ){
					return false;	
				}
			}
			else if( func.sFunc == "SCHOOL_E"  )
			{
				int schoolid = func.I(0);
				if( data_E.nActSch[0] != schoolid && data_E.nActSch[1] != schoolid   ){
					return false;	
				}
			}
			else if( func.sFunc == "RANGE"  )
			{
				if( (data_I!= null) && (data_E != null) )	
				{
					int Range =  func.I(1);
					int nDist = iVec2.Dist( data_I.n_X , data_I.n_Y ,  data_E.n_X , data_E.n_Y );
					if( MyScript.Instance.ConditionInt( nDist , func.S(0) ,Range  ) == false  ){
						return   false;		// always fail
					}		

				}else{
					return false;
				}
			}

			// Fight stat check
			else if( func.sFunc == "FST_ATKER"  )
			{				
				return data_I.IsStates( _FIGHTSTATE._ATKER );
			}
			else if( func.sFunc == "FST_DEFER"  )
			{				
				return data_I.IsStates( _FIGHTSTATE._ATKER )==false;
			}
			else if( func.sFunc == "FST_DAMAGE"  )
			{
				return data_I.IsStates( _FIGHTSTATE._DAMAGE );
			}
			else if( func.sFunc == "FST_KILL"  )
			{
				return data_I.IsStates( _FIGHTSTATE._KILL );
			}
			else if( func.sFunc == "FST_DEAD"  )
			{
				return data_I.IsStates( _FIGHTSTATE._DEAD );
			}
			else if( func.sFunc == "FST_DODGE"  )
			{
				return data_I.IsStates( _FIGHTSTATE._DODGE );
			}

			else{
				Debug.LogError( string.Format( "Error-Can't find script cond func '{0}'" , func.sFunc ) );
			}
		}
		return true;
	}
	
	
	public bool Check( cUnitData data_I , cUnitData data_E , int nSkillID  , int nBuffID  ){
		
		if (CondLst == null)
			return false;
		
		foreach( List<cTextFunc> funcList in CondLst )
		{
			if(CheckCond( data_I ,data_E ,funcList ,nSkillID,nBuffID ) == true ){
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
