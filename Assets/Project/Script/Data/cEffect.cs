using UnityEngine;
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


	public virtual void _Cast( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list){ }		// cast a skill
	public virtual void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ }			// BUFF 專用.命中後　額外效果
	public virtual void _BeHit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ }			// BUFF 專用.受擊後　額外效果 

	// cost
	public virtual bool _IsImmune(  int nBuffType ){		return false;	}				// check user in one status		

	public virtual bool _IsStatus( _FIGHTSTATE st  ){ return false; }				// check user in one status	

	public virtual bool _IsTag(  _UNITTAG tag ){ return false; }								// check unit extra tag

	public virtual int _UpSkill( int nBaseSkillID  ){ return nBaseSkillID;	} 					// 判斷有無進階的武功招式
}

// Cast Effect
public class ADDBUFF_I: cEffect
{
	public ADDBUFF_I( int buffid ){		iValue = buffid;	}
	//public int iValue ;
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
//	public int iValue ;
	
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

public class ADDACTTIME_I: cEffect
{
	public ADDACTTIME_I( int time ){		iValue = time;	}
	//public int iValue ;
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){
		if (Atker != null) {
			
			int nDefId = 0;			if( Defer != null )nDefId = Defer.n_Ident;
			list.Add( new cHitResult( cHitResult._TYPE._ACTTIME , Atker.n_Ident , iValue , Atker.n_Ident, nSkillID ,nDefId  ) );
		}
	}
}
public class ADDACTTIME_E: cEffect
{
	public ADDACTTIME_E( int time ){		iValue = time;;	}
	//public int iValue ;
	
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		if (Defer != null) {
			if( Defer.IsStates( _FIGHTSTATE._DODGE ) )
				return ;
			
			//	list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF ,Defer.n_Ident , nBuffID ) );
			int nAtkId = 0;			if( Atker != null )nAtkId = Atker.n_Ident;
			list.Add( new cHitResult( cHitResult._TYPE._ACTTIME , Defer.n_Ident , iValue, nAtkId , nSkillID , Defer.n_Ident  ) );
		}
	}
}
//aura
public class AURABUFF_I: cEffect
{
	public int nRange ;
	public AURABUFF_I( int range , int buffid ){	nRange = range; 	iValue = buffid;	}
	//public int iValue ;
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){
		if (Atker != null) {
			if( GameDataManager.Instance != null ){
				foreach( KeyValuePair<int,cUnitData> pair in GameDataManager.Instance.UnitPool )
				{
					if( BattleManager.CanPK( Atker ,pair.Value ) == false ){
						if( Atker.Dist( pair.Value ) <= nRange ){
							list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF , pair.Key , iValue , Atker.n_Ident, nSkillID ,pair.Key  ) );
						}
					}
				}
			}
		}
	}
}

public class AURABUFF_E: cEffect
{
	public int nRange ;
	public AURABUFF_E( int range , int buffid ){	nRange = range; 	iValue = buffid;	}
	//public int iValue ;
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){
		if (Atker != null) {
			if( GameDataManager.Instance != null ){
				foreach( KeyValuePair<int,cUnitData> pair in GameDataManager.Instance.UnitPool )
				{
					if( BattleManager.CanPK( Atker ,pair.Value ) == true ){
						if( Atker.Dist( pair.Value ) <= nRange ){
							list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF , pair.Key , iValue , Atker.n_Ident, nSkillID ,pair.Key  ) );
						}
					}
				}
			}
		}
	}
}


// Hit effect
public class HITBUFF_I: cEffect
{
	public HITBUFF_I( int buffid ){		iValue = buffid;	}
	//public int iValue ;
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

public class HITHP_I: cEffect
{
	public HITHP_I( float f , int i ){	fValue = f;	iValue = i; }
	
	override public void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		float fHp = Atker.GetMaxHP() * fValue ;
		//	list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF ,Defer.n_Ident , nBuffID ) );
		list.Add( new cHitResult( cHitResult._TYPE._HP , Atker.n_Ident , (int)fHp + iValue , Atker.n_Ident, nSkillID , nBuffID   ) );
	}
}
public class HITMP_I: cEffect
{
	public HITMP_I( float f , int i ){	fValue = f;	iValue = i;	}
	
	override public void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 	
		float fMp = Atker.GetMaxMP() * fValue ;
		list.Add( new cHitResult( cHitResult._TYPE._MP , Atker.n_Ident , (int)fMp +iValue  , Atker.n_Ident, nSkillID , nBuffID   ) );
	}
}
public class HITSP_I: cEffect
{
	public HITSP_I( int i ){	iValue = i;	}
	
	override public void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		
		list.Add( new cHitResult( cHitResult._TYPE._SP , Atker.n_Ident , iValue  , Atker.n_Ident, nSkillID , nBuffID   ) );
	}
}
public class HITCP_I: cEffect
{
	public HITCP_I( int i ){	iValue = i;	}
	
	override public void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		
		list.Add( new cHitResult( cHitResult._TYPE._CP , Atker.n_Ident , iValue  , Atker.n_Ident, nSkillID , nBuffID   ) );
	}
}
public class HITHP_E: cEffect
{
	public HITHP_E(float f , int i ){	fValue = f;	iValue = i;	}
	
	override public void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		if (Defer != null) {
			float fHp = Defer.GetMaxHP() * fValue ;
			list.Add( new cHitResult( cHitResult._TYPE._HP , Defer.n_Ident , (int)fHp +iValue , Atker.n_Ident, nSkillID , nBuffID   ) );
		}
	}
}
public class HITMP_E: cEffect
{
	public HITMP_E(float f , int i ){	fValue = f;	iValue = i;	}
	
	override public void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 	
		if (Defer != null) {
			float fMp = Defer.GetMaxMP() * fValue ;
			list.Add( new cHitResult( cHitResult._TYPE._MP , Defer.n_Ident , (int)fMp +iValue , Atker.n_Ident, nSkillID , nBuffID   ) );
		}
	}
}
public class HITSP_E: cEffect
{
	public HITSP_E( int i ){	iValue = i;	}
	
	override public void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		if (Defer != null) {			
			list.Add( new cHitResult( cHitResult._TYPE._SP , Defer.n_Ident , iValue , Atker.n_Ident, nSkillID , nBuffID   ) );
		}
		
	}
}
public class HITCP_E: cEffect
{
	public HITCP_E( int i ){	iValue = i;	}
	
	override public void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		
		list.Add( new cHitResult( cHitResult._TYPE._CP , Defer.n_Ident , iValue  , Atker.n_Ident, nSkillID , nBuffID   ) );
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
	public ADD_MAR_DIFF( float f , int i ){	 fValue = f; iValue = i;	}
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		if ((Atker != null) && ( Defer != null)) {
			float fDelt = Defer.GetMar( true ) - Atker.GetMar( true );
			fDelt *=fValue;
			// this is final
			attr.f_MAR += fDelt;
			attr.f_MAR += iValue;
		}
	}
}

public class ADD_ATTACK_DIFF: cEffect
{
	public ADD_ATTACK_DIFF( float f , int i ){	fValue = f;	iValue = i; }

	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		if ((Atker != null) && ( Defer != null)) {
			float fDelt = Defer.GetBaseAttack() - Atker.GetBaseAttack();
			fDelt *=fValue;
			// this is final
			attr.n_ATK += (int)fDelt;
			attr.n_ATK += iValue;
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
//	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fDropRate += fValue;
	}
}

public class MUL_BRUST: cEffect
{
	public MUL_BRUST( float v ){ fValue = v;}
//	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fBurstRate += fValue;
	}
}

public class MUL_DAMAGE: cEffect
{
	public MUL_DAMAGE( float v ){fValue = v;}
//	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fDamageRate += fValue;
	}
}

public class MUL_ATTACK: cEffect
{
	public MUL_ATTACK( float v ){fValue = v;}
//	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fAtkRate += fValue;
	}
}

public class MUL_DEF: cEffect
{
	public MUL_DEF( float v ){fValue = v;}
//	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fDefRate += fValue;
	}
}

public class MUL_POWER: cEffect
{
	public MUL_POWER( float v ){fValue = v;}
//	public float fValue ;	
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
//====  Drain ===
public class MUL_DRAINHP: cEffect
{
	public MUL_DRAINHP( float v ){ fValue = v;}
//	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fDrainHpRate += fValue;
	}
}

public class MUL_DRAINMP: cEffect
{
	public MUL_DRAINMP( float v ){ fValue = v;}
//	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fDrainMpRate += fValue;
	}
}


//==== cost ==
public class MUL_MPCOST: cEffect
{
	public MUL_MPCOST( float v ){ fValue = v;}
//	public float fValue ;
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr  ){ 
		attr.fMpCostRate += fValue;
	}
}


//public class RECOVER_SP: cEffect
//{
//	public RECOVER_SP( float v ){ fValue = v;}
////	public float fValue ;
////	override public void _Mul_SpCost( ref float value ){ 
////		value += F;
////	}
//}

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
public class TAG_SILENCE: cEffect
{
	public TAG_SILENCE(  ){ }	
	
	override public bool _IsTag(  _UNITTAG tag ){
		return (_UNITTAG._SILENCE == tag );
	}				// check user in one status		
}
//==== immune buff==
public class IMMUNE: cEffect
{
	public IMMUNE( int v ){  iValue = v; }	
	
	override public bool _IsImmune(  int nBuffType ){
		return (iValue == nBuffType );
	}				// check user in one status		
}

//==== is status ==
public class IS_HIT: cEffect
{
	public IS_HIT(  ){ }	
	
	override public bool _IsStatus(  _FIGHTSTATE st ){
		return (_FIGHTSTATE._HIT == st );
	}				// check user in one status		
}



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
public class IS_MISS: cEffect
{
	public IS_MISS( ){	}	
	
	override public bool _IsStatus(  _FIGHTSTATE st ){
		return (st == _FIGHTSTATE._MISS);
	}				// check user in one status		
}
public class IS_COMBO: cEffect
{
	public IS_COMBO( ){	}	
	
	override public bool _IsStatus(  _FIGHTSTATE st ){
		return (st == _FIGHTSTATE._COMBO);
	}				// check user in one status		
}

public class IS_BROKEN: cEffect
{
	public IS_BROKEN( ){	}	
	
	override public bool _IsStatus(  _FIGHTSTATE st ){
		return (st == _FIGHTSTATE._BROKEN);
	}				// check user in one status		
}
public class IS_RETURN: cEffect
{
	public IS_RETURN( ){	}	
	
	override public bool _IsStatus(  _FIGHTSTATE st ){
		return (st == _FIGHTSTATE._RETURN);
	}				// check user in one status		
}
public class IS_COPY: cEffect
{
	public IS_COPY( ){	}	
	
	override public bool _IsStatus(  _FIGHTSTATE st ){
		return (st == _FIGHTSTATE._COPY);
	}				// check user in one status		
}

public class IS_TWICE: cEffect
{
	public IS_TWICE( ){	}	
	
	override public bool _IsStatus(  _FIGHTSTATE st ){
		return (st == _FIGHTSTATE._TWICE);
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
	public void Clear()
	{
		if( CondLst != null ){
			CondLst.Clear();
		}
	}

	public void Add( CTextLine line )
	{
		if (CondLst == null)
			CondLst = new List<  List<cTextFunc> > ();
		if (line == null)
			return;
		CondLst.Add (line.GetFuncList ());
	}
	
	private bool CheckCond( cBuffData buff , cUnitData data_I , cUnitData data_E , List<cTextFunc> funcList, int nSkillID, int  nBuffID   )
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
			else if( func.sFunc == "HP_I"  )
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
			else if( func.sFunc == "POW_I"  )
			{
				int i1 = data_I.GetPow();
				int i2 = 0;


				string s1 = func.S( 1 );  // s1 
				if( s1 == "E" ){
					if( data_E != null ){
						i2 = data_E.GetPow();
					}else{
						return false; // no enemy is false
					}
				}
				else if( s1 == "I" ){
					if( data_I != null ){
						i2 = data_I.GetPow();
					}else{
						return false; // no enemy is false
					}
				}
				else{
					i2 = func.I( 1 );
				}
				//
				if( MyScript.Instance.ConditionInt( i1 , func.S(0) ,i2  ) == false  ){
					return   false;		// always fail
				}

			}
			else if( func.sFunc == "POW_E"  )
			{
				int i1 = data_E.GetPow();
				int i2 = 0;
				
				
				string s1 = func.S( 1 );  // s1 
				if( s1 == "E" ){
					if( data_E != null ){
						i2 = data_E.GetPow();
					}else{
						return false; // no enemy is false
					}
				}
				else if( s1 == "I" ){
					if( data_I != null ){
						i2 = data_I.GetPow();
					}else{
						return false; // no enemy is false
					}
				}
				else{
					i2 = func.I( 1 );
				}
				//
				if( MyScript.Instance.ConditionInt( i1 , func.S(0) ,i2  ) == false  ){
					return   false;		// always fail
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
				else if( s1 == "I" ){
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
				else if( s1 == "I" ){
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
			else if( func.sFunc == "RANGE" || func.sFunc == "DIST" )
			{
				if( (data_I!= null) && (data_E != null) )	
				{
					int Range =  func.I(1);
					int nDist = iVec2.Dist( data_I.n_X , data_I.n_Y ,  data_E.n_X , data_E.n_Y );
					if( MyScript.Instance.ConditionInt( nDist , func.S(0) ,Range  ) == false  ){
						return   false;		// always fail
					}		

				}
				else{
					return false;
				}
			}
			else if( func.sFunc == "DIST_C" )
			{
				cUnitData pCast = buff.GetCastUnit();
				if( pCast != null )
				{
					int Range =  func.I(1);
					int nDist = iVec2.Dist( data_I.n_X , data_I.n_Y ,  pCast.n_X , pCast.n_Y );
					if( MyScript.Instance.ConditionInt( nDist , func.S(0) ,Range  ) == false  ){
						return   false;		// always fail
					}		
				}
				else{
					return false;
				}
			}

			else if( func.sFunc == "GENDER_I"  )
			{
				int gender = func.I(0);
				if( data_I!= null ){
					CHARS charData = ConstDataManager.Instance.GetRow<CHARS> ( data_I.n_CharID );
					if( gender != charData.n_GENDER   )
					{
						return false;
					}
				}
				else {
					return false;
				}
			}
			else if( func.sFunc == "GENDER_E"  )
			{
				int gender = func.I(0);
				if( data_E!= null ){
					CHARS charData = ConstDataManager.Instance.GetRow<CHARS> ( data_E.n_CharID );
					if( gender != charData.n_GENDER   )
					{
						return false;
					}
				}
				else {
					return false;
				}
			}

			//雙方內外功星等比較
			else if( func.sFunc == "INTRANK_I"  )
			{
				int i1 = data_I.GetIntSchRank();
				int i2 = 0;
				string s1 = func.S( 1 );  // s1 
				if( s1 == "E" ){
					if( data_E != null ){
						i2 = data_E.GetIntSchRank();
					}else{
						return false; // no enemy is false
					}
				}
				else if( s1 == "I" ){
					if( data_I != null ){
						i2 = data_I.GetIntSchRank();
					}else{
						return false; // no enemy is false
					}
				}
				else{
					i2 = func.I( 1 );
				}
				//
				if( MyScript.Instance.ConditionInt( i1 , func.S(0) ,i2  ) == false  ){
					return   false;		// always fail
				}

			}
			else if( func.sFunc == "EXTRANK_I"  )
			{
				int i1 = data_I.GetExtSchRank();
				int i2 = 0;
				string s1 = func.S( 1 );  // s1 
				if( s1 == "E" ){
					if( data_E != null ){
						i2 = data_E.GetExtSchRank();
					}else{
						return false; // no enemy is false
					}
				}
				else if( s1 == "I" ){
					if( data_I != null ){
						i2 = data_I.GetExtSchRank();
					}else{
						return false; // no enemy is false
					}
				}
				else{
					i2 = func.I( 1 );
				}
				//
				if( MyScript.Instance.ConditionInt( i1 , func.S(0) ,i2  ) == false  ){
					return   false;		// always fail
				}				
			}
			else if( func.sFunc == "ITEM_I"  )
			{
				int nItemID = func.I(0);
				if( data_I.CheckItemEquiped( nItemID ) == false ){
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
			else if( func.sFunc == "FST_THROUGH"  )
			{
				return data_I.IsStates( _FIGHTSTATE._THROUGH );
			}
			else if( func.sFunc == "FST_MISS"  )
			{
				return data_I.IsStates( _FIGHTSTATE._MISS );
			}
			else if( func.sFunc == "FST_COMBO"  )
			{
				return data_I.IsStates( _FIGHTSTATE._COMBO );
			}
			else if( func.sFunc == "FST_BROKEN"  )
			{
				return data_I.IsStates( _FIGHTSTATE._BROKEN );
			}

			else{
				Debug.LogError( string.Format( "Error-Can't find script cond func '{0}'" , func.sFunc ) );
			}
		}
		return true;
	}
	
	
	public bool Check( cBuffData buff , cUnitData data_I , cUnitData data_E , int nSkillID  , int nBuffID  ){
		
		if (CondLst == null)
			return false;
		
		foreach( List<cTextFunc> funcList in CondLst )
		{
			if(CheckCond( buff , data_I ,data_E ,funcList ,nSkillID,nBuffID ) == true ){
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
