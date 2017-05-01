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


	public virtual void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1) { }
	
																											// SKill
	public virtual void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ }				//  casr/ hit event will run this


	public virtual void _Cast( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list){ }		// cast a skill
	public virtual void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ }			// BUFF 專用.命中後　額外效果
	public virtual void _BeHit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ }			// BUFF 專用.受擊後　額外效果 

	// cost
	public virtual bool _IsImmune(  int nBuffType ){		return false;	}				// check user in one status		
    public virtual bool _IsCharImmune(int nCharID ) { return false; }				// check user in one status		
    

    public virtual bool _IsStatus( _FIGHTSTATE st  ){ return false; }				// check user in one status	

	public virtual bool _IsTag(  _UNITTAG tag ){ return false; }								// check unit extra tag

	public virtual int _UpSkill( int nBaseSkillID  ){ return nBaseSkillID;	} 					// 判斷有無進階的武功招式
}

// Cast Effect
public class ADDBUFF_I: cEffect
{
	public ADDBUFF_I( int buffid, int num) {		iValue = buffid; nNum = num;     }
    public int nNum;
	//public int iValue ;
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){
		if (Atker != null) {
		
			int nDefId = 0;			if( Defer != null )nDefId = Defer.n_Ident;
			list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF , Atker.n_Ident , iValue , Atker.n_Ident, nSkillID ,nDefId , nNum ) );
		}
	}
}
public class ADDBUFF_E: cEffect
{
	public ADDBUFF_E( int buffid  , int num ){		iValue = buffid; nNum = num; 	}
    public int nNum;//	public int iValue ;
	
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		if (Defer != null) {
			if( Defer.IsStates( _FIGHTSTATE._DODGE ) )
				return ;

		//	list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF ,Defer.n_Ident , nBuffID ) );
			int nAtkId = 0;			if( Atker != null )nAtkId = Atker.n_Ident;
			list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF , Defer.n_Ident , iValue, nAtkId , nSkillID , Defer.n_Ident, nNum) );
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


public class ADD_DEF_I : cEffect
{
    public ADD_DEF_I(float f, int i) { fValue = f; iValue = i; }

    override public void _Do(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
        int def =  (int)(Atker.GetMaxDef() * fValue) + iValue;

        list.Add(new cHitResult(cHitResult._TYPE._DEF, Atker.n_Ident, def, Atker.n_Ident, nSkillID, nBuffID));
    }
}
public class ADD_DEF_E : cEffect
{
    public ADD_DEF_E(float f, int i) { fValue = f; iValue = i; }

    override public void _Do(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
        if (Defer != null)
        {
            int def = (int)(Defer.GetMaxDef() * fValue) + iValue;
            list.Add(new cHitResult(cHitResult._TYPE._DEF, Defer.n_Ident, def, Atker.n_Ident, nSkillID, nBuffID));
        }
    }
}

//public class SKILL_EFFECT : cEffect
//{
//    public SKILL_EFFECT(int nSkillID) { iValue = nSkillID; }
//    //public int iValue ;

//    override public void _Do(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
//    {
//        if (Defer != null)
//        {

//  //          uAction pAct = ActionManager.Instance.CreateHitAction(Atker.n_Ident , 0, 0, iValue);
////            if (pAct!= null )
//            {
////                BattleManager.Instance.GetAtkHitResult(Atker, Defer , iValue , 0 , 0 , ref pAct) , ;
//            }


//            // if (Defer.IsStates(_FIGHTSTATE._DODGE))
//            //    return;

//            //	list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF ,Defer.n_Ident , nBuffID ) );
//            //int nAtkId = 0; if (Atker != null) nAtkId = Atker.n_Ident;
//            //// cast eff
//            //cSkillData skilldata = MyTool.GetSkillData(iValue);
//            //if (skilldata != null)
//            //{
//            //    skilldata.DoCastEffect(Atker, Defer, ref list );
//            //}
           
//            //list.Add(new cHitResult(cHitResult._TYPE._ACTTIME, Defer.n_Ident, iValue, nAtkId, nSkillID, Defer.n_Ident));
//        }
//    }
//    override public void _Hit(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
//    {
//        if (Atker != null)
//        {
//            // hit effect
//  //          List<cHitResult> l = BattleManager.CalSkillHitResult(Atker, Defer, iValue);
////            foreach (cHitResult res in l)
//            {
//                //list.Add(res);
//            }
//        }
//    }
//}

//aura
public class AURABUFF_I: cEffect
{
	public int nRange;
    public int nCharId;
    public AURABUFF_I( int range , int buffid, int charid = 0) {	nRange = range; 	iValue = buffid; nCharId = charid; }
	//public int iValue ;
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){
		if (Atker != null) {
			if( GameDataManager.Instance != null ){
				foreach( KeyValuePair<int,cUnitData> pair in GameDataManager.Instance.UnitPool )
				{                    
					if( BattleManager.CanPK( Atker ,pair.Value ) == false ){
                        if (pair.Value.IsTriggr())
                            continue; // 機關不上 aura buff
                        // 有指定 char 則判斷 char
                        if (nCharId > 0 && pair.Value.n_CharID != nCharId)
                        {
                            continue;
                        }
                        if ( Atker.Dist( pair.Value ) <= nRange ){
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
    public int nCharId;
    public AURABUFF_E( int range , int buffid , int charid =0 ){	nRange = range; 	iValue = buffid; nCharId = charid; }
	//public int iValue ;
	override public void _Do( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){
		if (Atker != null) {
			if( GameDataManager.Instance != null ){
				foreach( KeyValuePair<int,cUnitData> pair in GameDataManager.Instance.UnitPool )
				{
					if( BattleManager.CanPK( Atker ,pair.Value ) == true ){
                        if (pair.Value.IsTriggr())
                            continue; // 機關不上 aura buff
                        // 有指定 char 則判斷 char
                        if (nCharId > 0 && pair.Value.n_CharID != nCharId ) {
                            continue;
                        }

                        if ( Atker.Dist( pair.Value ) <= nRange ){
							list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF , pair.Key , iValue , Atker.n_Ident, nSkillID ,pair.Key  ) );
						}
					}
				}
			}
		}
	}
}

public class AURA_DELBUFF_I : cEffect
{
    public int nRange;
    public int nCharId;
    public AURA_DELBUFF_I(int range, int buffid, int charid = 0) { nRange = range; iValue = buffid; nCharId = charid; }
    //public int iValue ;
    override public void _Do(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
        if (Atker != null)
        {
            if (GameDataManager.Instance != null)
            {
                foreach (KeyValuePair<int, cUnitData> pair in GameDataManager.Instance.UnitPool)
                {
                    if (BattleManager.CanPK(Atker, pair.Value) == false)
                    {
                        if (pair.Value.IsTriggr())
                            continue; // 機關不上 aura buff
                        // 有指定 char 則判斷 char
                        if (nCharId > 0 && pair.Value.n_CharID != nCharId)
                        {
                            continue;
                        }
                        if (Atker.Dist(pair.Value) <= nRange)
                        {
                            list.Add(new cHitResult(cHitResult._TYPE._DELBUFF, pair.Key, iValue, Atker.n_Ident, nSkillID, pair.Key));
                        }
                    }
                }
            }
        }
    }
}

public class AURA_DELBUFF_E : cEffect
{
    public int nRange;
    public int nCharId;
    public AURA_DELBUFF_E(int range, int buffid, int charid = 0) { nRange = range; iValue = buffid; nCharId = charid; }
    //public int iValue ;
    override public void _Do(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
        if (Atker != null)
        {
            if (GameDataManager.Instance != null)
            {
                foreach (KeyValuePair<int, cUnitData> pair in GameDataManager.Instance.UnitPool)
                {
                    if (BattleManager.CanPK(Atker, pair.Value) == true)
                    {
                        if (pair.Value.IsTriggr())
                            continue; // 機關不上 aura buff
                        // 有指定 char 則判斷 char
                        if (nCharId > 0 && pair.Value.n_CharID != nCharId)
                        {
                            continue;
                        }
                        if (Atker.Dist(pair.Value) <= nRange)
                        {
                            list.Add(new cHitResult(cHitResult._TYPE._DELBUFF, pair.Key, iValue, Atker.n_Ident, nSkillID, pair.Key));
                        }
                    }
                }
            }
        }
    }
}

public class AURA_DELSTACK_I : cEffect
{
    public int nRange;
    public int nCharId;
    public AURA_DELSTACK_I(int range, int stackid, int charid = 0) { nRange = range; iValue = stackid; nCharId = charid; }
    //public int iValue ;
    override public void _Do(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
        if (Atker != null)
        {
            if (GameDataManager.Instance != null)
            {
                foreach (KeyValuePair<int, cUnitData> pair in GameDataManager.Instance.UnitPool)
                {
                    if (BattleManager.CanPK(Atker, pair.Value) == false)
                    {
                        if (pair.Value.IsTriggr())
                            continue; // 機關不上 aura buff
                                      // 有指定 char 則判斷 char
                        if (nCharId > 0 && pair.Value.n_CharID != nCharId)
                        {
                            continue;
                        }
                        if (Atker.Dist(pair.Value) <= nRange)
                        {
                            list.Add(new cHitResult(cHitResult._TYPE._DELSTACK, pair.Key, iValue, Atker.n_Ident, nSkillID, pair.Key));
                        }
                    }
                }
            }
        }
    }
}

public class AURA_DELSTACK_E : cEffect
{
    public int nRange;
    public int nCharId;
    public AURA_DELSTACK_E(int range, int stackid, int charid = 0) { nRange = range; iValue = stackid; nCharId = charid; }
    //public int iValue ;
    override public void _Do(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
        if (Atker != null)
        {
            if (GameDataManager.Instance != null)
            {
                foreach (KeyValuePair<int, cUnitData> pair in GameDataManager.Instance.UnitPool)
                {
                    if (BattleManager.CanPK(Atker, pair.Value) == true)
                    {
                        if (pair.Value.IsTriggr())
                            continue; // 機關不上 aura buff
                                      // 有指定 char 則判斷 char
                        if (nCharId > 0 && pair.Value.n_CharID != nCharId)
                        {
                            continue;
                        }
                        if (Atker.Dist(pair.Value) <= nRange)
                        {
                            list.Add(new cHitResult(cHitResult._TYPE._DELSTACK, pair.Key, iValue, Atker.n_Ident, nSkillID, pair.Key));
                        }
                    }
                }
            }
        }
    }
}

public class AURA_CP_I : cEffect
{
    public int nRange;
    public AURA_CP_I(int range, int cp) { nRange = range; iValue = cp; }
    //public int iValue ;
    override public void _Do(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
        if (Atker != null)
        {
            if (GameDataManager.Instance != null)
            {
                foreach (KeyValuePair<int, cUnitData> pair in GameDataManager.Instance.UnitPool)
                {
                    if (BattleManager.CanPK(Atker, pair.Value) == false)
                    {
                        if (pair.Value.IsTriggr())
                            continue; // 機關不上 aura buff
                        if (Atker.Dist(pair.Value) <= nRange)
                        {
                            list.Add(new cHitResult(cHitResult._TYPE._CP, pair.Key, iValue, Atker.n_Ident, nSkillID, pair.Key));
                        }
                    }
                }
            }
        }
    }
}

public class AURA_CP_E : cEffect
{
    public int nRange;
    public AURA_CP_E(int range, int cp) { nRange = range; iValue = cp; }
    //public int iValue ;
    override public void _Do(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
        if (Atker != null)
        {
            if (GameDataManager.Instance != null)
            {
                foreach (KeyValuePair<int, cUnitData> pair in GameDataManager.Instance.UnitPool)
                {
                    if (BattleManager.CanPK(Atker, pair.Value) == true)
                    {
                        if (pair.Value.IsTriggr())
                            continue; // 機關不上 aura buff

                        if (Atker.Dist(pair.Value) <= nRange)
                        {
                            list.Add(new cHitResult(cHitResult._TYPE._CP, pair.Key, iValue, Atker.n_Ident, nSkillID, pair.Key));
                        }
                    }
                }
            }
        }
    }
}

// 陣型
public class BATTLE_ARRAY : cEffect
{
    public int nRange;
   // public int nCondBuff;
    public BATTLE_ARRAY(int range,  int buffid) { nRange = range; iValue = buffid; }
    //public int iValue ;
    override public void _Do(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
        if (Atker != null)
        {
            if (GameDataManager.Instance != null)
            {
                foreach (KeyValuePair<int, cUnitData> pair in GameDataManager.Instance.UnitPool)
                {
                    if (Atker == pair.Value) {
                        continue;
                    }
                    //只能與我方結陣
                    if (BattleManager.CanPK(Atker, pair.Value) == false)
                    {
                        if (Atker.Dist(pair.Value) <= nRange)
                        {
                            if (pair.Value.IsTriggr())
                                continue; // 機關不上 aura buff
                            if ( pair.Value.Buffs.HaveBuff( nBuffID)) // 確定對方是能跟自己結陣的
                            {
                                // 每有一個人，自己獲得一層 buff
                                list.Add(new cHitResult(cHitResult._TYPE._ADDBUFF, Atker.n_Ident, iValue, Atker.n_Ident, nSkillID, pair.Key)); // slef will in unit pool dist check
                               // list.Add(new cHitResult(cHitResult._TYPE._ADDBUFF, pair.Key, iValue, Atker.n_Ident, nSkillID, pair.Key));
                                //list.Add(new cHitResult(cHitResult._TYPE._ADDBUFF, pair.Key, iValue, Atker.n_Ident, nSkillID, pair.Key));
                            }
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

// Hit effect
public class HITDELBUFF_I : cEffect
{
    public HITDELBUFF_I(int buffid) { iValue = buffid; }
    //public int iValue ;
    override public void _Hit(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
        if (Atker != null)
        {
            // ( int nBuffID , int nCastIdent , int nSkillID  , int nTargetId )
            //pData.Buffs.AddBuff( res.Value1 , res.Value2, res.SkillID, res.Value3 );
            int nDefId = 0; if (Defer != null) nDefId = Defer.n_Ident;
            list.Add(new cHitResult(cHitResult._TYPE._DELBUFF, Atker.n_Ident, iValue, Atker.n_Ident, nSkillID, nDefId));
        }
    }
}

public class HITDELBUFF_E : cEffect
{
    public HITDELBUFF_E(int buffid) { iValue = buffid; ; }


    override public void _Hit(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
        if (Defer != null)
        {
            if (Defer.IsStates(_FIGHTSTATE._DODGE))
                return;
            //	list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF ,Defer.n_Ident , nBuffID ) );
            int nAtkId = 0; if (Atker != null) nAtkId = Atker.n_Ident;
            list.Add(new cHitResult(cHitResult._TYPE._DELBUFF, Defer.n_Ident, iValue, nAtkId, nSkillID, Defer.n_Ident));
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
	public HITSP_I(float f, int i ){ fValue = f; iValue = i; }
	
	override public void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){
        float fSp = Defer.GetMaxSP() * fValue;
        list.Add( new cHitResult( cHitResult._TYPE._SP , Atker.n_Ident , (int)fSp + iValue, Atker.n_Ident, nSkillID , nBuffID   ) );
	}
}
public class HITCP_I: cEffect
{
	public HITCP_I( int i ){	iValue = i;	}
	
	override public void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		
		list.Add( new cHitResult( cHitResult._TYPE._CP , Atker.n_Ident , iValue  , Atker.n_Ident, nSkillID , nBuffID   ) );
	}
}
public class HITTIRED_I : cEffect
{
    public HITTIRED_I(int i) { iValue = i; }

    override public void _Hit(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {

        list.Add(new cHitResult(cHitResult._TYPE._TIRED, Atker.n_Ident, iValue, Atker.n_Ident, nSkillID, nBuffID));
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
	public HITSP_E(float f, int i ){ fValue = f; iValue = i; }
	
	override public void _Hit( cUnitData Atker , cUnitData Defer , ref List<cHitResult> list ){ 
		if (Defer != null) {
            float fSp = Defer.GetMaxSP() * fValue;
            list.Add( new cHitResult( cHitResult._TYPE._SP , Defer.n_Ident , (int)fSp + iValue, Atker.n_Ident, nSkillID , nBuffID   ) );
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
public class HITTIRED_E : cEffect
{
    public HITTIRED_E(int i) { iValue = i; }

    override public void _Hit(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {

        list.Add(new cHitResult(cHitResult._TYPE._TIRED, Defer.n_Ident, iValue, Atker.n_Ident, nSkillID, nBuffID));
    }
}

// Be Hit atker is 打的人， defer 是被打人的人( behit)
public class BEHITBUFF_I : cEffect
{
    public BEHITBUFF_I(int buffid) { iValue = buffid; }
    //public int iValue ;
    override public void _BeHit(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
#if DEBUG 
        BUFF buff = ConstDataManager.Instance.GetRow<BUFF>(iValue);
        if (buff == null){
            Debug.LogErrorFormat("BEHITBUFF_I  with null buff {0}" , iValue );
        }
        else if(buff.n_DURATION == -1 ){
            Debug.LogErrorFormat("BEHITBUFF_I  with error fight buff {0}", iValue);
        }   
#endif 

        if (Defer != null)
        {
            if (Defer.IsStates(_FIGHTSTATE._DODGE))
                return;
           
            // ( int nBuffID , int nCastIdent , int nSkillID  , int nTargetId )
            //pData.Buffs.AddBuff( res.Value1 , res.Value2, res.SkillID, res.Value3 );
            int nDefId = 0; if (Defer != null) nDefId = Defer.n_Ident;
            list.Add(new cHitResult(cHitResult._TYPE._ADDBUFF, Defer.n_Ident, iValue, nDefId, nSkillID, nDefId));
        }
    }
}

public class BEHITBUFF_E : cEffect
{
    public BEHITBUFF_E(int buffid) { iValue = buffid; ; }


    override public void _BeHit(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
#if DEBUG 
        BUFF buff = ConstDataManager.Instance.GetRow<BUFF>(iValue);
        if (buff == null)
        {
            Debug.LogErrorFormat("BEHITBUFF_E  with null buff {0}", iValue);
        }
        else if (buff.n_DURATION == -1)
        {
            Debug.LogErrorFormat("BEHITBUFF_E  with error fight buff {0}", iValue);
        }
#endif
        if (Atker != null)
        {
         
            //	list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF ,Defer.n_Ident , nBuffID ) );
            int nDefId = 0; if (Defer != null) nDefId = Defer.n_Ident;
            list.Add(new cHitResult(cHitResult._TYPE._ADDBUFF, Atker.n_Ident, iValue, nDefId, nSkillID, nDefId));
        }
    }
}

public class BEHITCP_I : cEffect
{
    public BEHITCP_I(int i) { iValue = i; }

    override public void _BeHit(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
       
        list.Add(new cHitResult(cHitResult._TYPE._CP, Defer.n_Ident, iValue, Atker.n_Ident, nSkillID, nBuffID));
    }
}
public class BEHITCP_E : cEffect
{
    public BEHITCP_E(int i) { iValue = i; }

    override public void _BeHit(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
        if (Defer == Atker)
            return;
        list.Add(new cHitResult(cHitResult._TYPE._CP, Atker.n_Ident, iValue, Defer.n_Ident, nSkillID, nBuffID));
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

	override public void _Attr( cUnitData Atker , cUnitData Defer,  ref cAttrData attr  ,int nNum = 1 )
    { 
		attr.f_MAR += (fValue * nNum);
    }
}

public class ADD_MAR_DIFF: cEffect
{
	public ADD_MAR_DIFF( float f , int i ){	 fValue = f; iValue = i;	}
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		if ((Atker != null) && ( Defer != null)) {
			float fDelt = Defer.GetMar( true ) - Atker.GetMar( true );
			fDelt *=fValue;
			// this is final
			attr.f_MAR += (fDelt* nNum);
			attr.f_MAR += (iValue* nNum);
		}
	}
}

public class ADD_ATTACK_DIFF: cEffect
{
	public ADD_ATTACK_DIFF( float f , int i ){	fValue = f;	iValue = i; }

	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		if ((Atker != null) && ( Defer != null)) {
			float fDelt = Defer.GetBaseAttack() - Atker.GetBaseAttack();
			fDelt *=fValue;
			// this is final
			attr.n_ATK += (int)(fDelt* nNum);
			attr.n_ATK += (iValue* nNum);
		}
	}
}

public class ADD_ATTACK: cEffect
{
	public ADD_ATTACK( int i ){	iValue = i;	}
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		if ((Atker != null)) {
			attr.n_ATK += (iValue* nNum);
		}
	}
}

public class ADD_MAXDEF: cEffect
{
	public ADD_MAXDEF( int i ){	iValue = i;	}
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		if ((Atker != null)) {
			attr.n_DEF += (iValue * nNum);
        }
	}
}

public class ADD_POWER: cEffect
{
	public ADD_POWER( int i ){	iValue = i;	}
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		if ((Atker != null)) {
			attr.n_POW += (iValue* nNum);
		}
	}
}

public class ADD_MAXHP: cEffect
{
	public ADD_MAXHP( int i ){	iValue = i;	}
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		if ((Atker != null)) {
			attr.n_HP += (iValue* nNum);
		}
	}
}
public class ADD_MAXMP: cEffect
{
	public ADD_MAXMP( int i ){	iValue = i;	}
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		if ((Atker != null)) {
			attr.n_MP += (iValue* nNum);
		}
	}
}
public class ADD_MAXSP: cEffect
{
	public ADD_MAXSP( int i ){	iValue = i;	}
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		if ((Atker != null)) {
			attr.n_SP += (iValue* nNum);
		}
	}
}



public class ADD_MOVE: cEffect
{
	public ADD_MOVE( int n ){ iValue = n;	}
//	public int nValue ;
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		attr.n_MOV += (iValue * nNum);
	}
}

// armor
public class ADD_ARMOR : cEffect
{
    public ADD_ARMOR(float v) { fValue = v; }
    //	public float fValue ;	
    override public void _Attr(cUnitData Atker, cUnitData Defer, ref cAttrData attr, int nNum = 1)
    {
        attr.fArmor += (fValue * nNum);
    }
}

public class HEALHP : cEffect
{
    public HEALHP(float f, int i) { fValue = f; iValue = i; }

    override public void _Do(cUnitData Atker, cUnitData Defer, ref List<cHitResult> list)
    {
        float fHp = Atker.GetPow() * fValue ;
        //	list.Add( new cHitResult( cHitResult._TYPE._ADDBUFF ,Defer.n_Ident , nBuffID ) );
        list.Add(new cHitResult(cHitResult._TYPE._HP, Defer.n_Ident, (int)fHp + iValue, Atker.n_Ident, nSkillID, nBuffID));
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
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1 )
    { 
		attr.fDropRate += (fValue * nNum);
    }
}

public class MUL_BRUST: cEffect
{
	public MUL_BRUST( float v ){ fValue = v;}
//	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		attr.fBurstRate += (fValue * nNum);
    }
}

public class MUL_DAMAGE: cEffect
{
	public MUL_DAMAGE( float v ){fValue = v;}
//	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		attr.fDamageRate += (fValue* nNum);
	}
}


public class MUL_ATTACK: cEffect
{
	public MUL_ATTACK( float v ){fValue = v;}
//	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		attr.fAtkRate += (fValue* nNum);
	}
}

public class MUL_MAXDEF: cEffect
{
	public MUL_MAXDEF( float v ){fValue = v;}
//	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		attr.fDefRate += (fValue* nNum);
	}
}

public class MUL_POWER: cEffect
{
	public MUL_POWER( float v ){fValue = v;}
//	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		attr.fPowRate += (fValue * nNum);
    }
}

public class MUL_MAXHP: cEffect
{
	public MUL_MAXHP( float v ){ fValue = v; }
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		if ((Atker != null)) {
			attr.fHpRate += (fValue * nNum);
        }
	}
}
public class MUL_MAXMP: cEffect
{
	public MUL_MAXMP( float v ){ fValue = v; }
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		if ((Atker != null)) {
			attr.fMpRate += (fValue * nNum);
        }
	}
}
public class MUL_MAXSP: cEffect
{
	public MUL_MAXSP( float v ){ fValue = v; }
	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		if ((Atker != null)) {
			attr.fSpRate += (fValue * nNum);
        }
	}
}
//====  Drain ===
public class MUL_DRAINHP: cEffect
{
	public MUL_DRAINHP( float v ){ fValue = v;}
//	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		attr.fDrainHpRate += (fValue * nNum);
    }
}

public class MUL_DRAINMP: cEffect
{
	public MUL_DRAINMP( float v ){ fValue = v;}
//	public float fValue ;	
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		attr.fDrainMpRate += (fValue * nNum);
    }
}


//==== cost ==
public class MUL_MPCOST: cEffect
{
	public MUL_MPCOST( float v ){ fValue = v;}
//	public float fValue ;
	override public void _Attr( cUnitData Atker , cUnitData Defer, ref cAttrData attr, int nNum = 1)
    { 
		attr.fMpCostRate += (fValue * nNum);
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
public class TAG_UNDEAD : cEffect
{
    public TAG_UNDEAD() { }

    override public bool _IsTag(_UNITTAG tag)
    {
        return (_UNITTAG._UNDEAD == tag);
    }				// check user in one status		
}

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
public class TAG_PEACE : cEffect
{
    public TAG_PEACE() { }

    override public bool _IsTag(_UNITTAG tag)
    {
        return (_UNITTAG._PEACE == tag);
    }				// check user in one status		
}
public class TAG_TRIGGER : cEffect
{
    public TAG_TRIGGER() { }

    override public bool _IsTag(_UNITTAG tag)
    {
        return (_UNITTAG._TRIGGER == tag);
    }				// check user in one status		
}
public class TAG_BLOCKITEM : cEffect
{
    public TAG_BLOCKITEM() { }

    override public bool _IsTag(_UNITTAG tag)
    {
        return (_UNITTAG._BLOCKITEM == tag);
    }				// check user in one status		
}
public class TAG_STUN : cEffect
{
    public TAG_STUN() { }

    override public bool _IsTag(_UNITTAG tag)
    {
        return (_UNITTAG._STUN == tag);
    }				// check user in one status		
}

public class TAG_HIDE : cEffect
{
    public TAG_HIDE() { }

    override public bool _IsTag(_UNITTAG tag)
    {
        return (_UNITTAG._HIDE == tag);
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

public class IS_NODMG : cEffect
{
    public IS_NODMG( ) { }

    override public bool _IsStatus(_FIGHTSTATE st)
    {
        return (_FIGHTSTATE._NODMG == st);
    }				// check user in one status		
}

public class IS_ANTIFLY : cEffect
{
    public IS_ANTIFLY() { }

    override public bool _IsStatus(_FIGHTSTATE st)
    {
        return (_FIGHTSTATE._ANTIFLY == st);
    }				// check user in one status		
}

public class IS_SHIELD : cEffect
{
    public IS_SHIELD() { }

    override public bool _IsStatus(_FIGHTSTATE st)
    {
        return (_FIGHTSTATE._SHIELD == st);
    }				// check user in one status		
}


//==========================================================================
// use to cache condition sctipr parser result
public class cEffectCondition
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

    List<List<cTextFunc>> CondLst;      // multi array.
    public void Clear()
    {
        if (CondLst != null)
        {
            CondLst.Clear();
        }
    }

    public void Add(CTextLine line)
    {
        if (CondLst == null)
            CondLst = new List<List<cTextFunc>>();
        if (line == null)
            return;
        CondLst.Add(line.GetFuncList());
    }

    private bool CheckCond(cBuffData buff, cUnitData data_I, cUnitData data_E, List<cTextFunc> funcList, int nSkillID, int nBuffID)
    {
        if (funcList == null)
            return false;

        foreach (cTextFunc func in funcList)
        {
            // 成功的都是 continue， 不要return true 。會破壞後面的判斷

            if (func.sFunc == "GO")
            {
               // return true;        // always true
            }
            else if (func.sFunc == "NULL" || func.sFunc == "0")
            {
                return false;       // always fail
            }
            else if (func.sFunc == "RATE")
            {
                int Rate = func.I(0);
                int nRoll = Random.Range(0, 100);
                if (Rate < nRoll)
                {
                    return false;
                }
                
            }
            else if (func.sFunc == "MRATE")  // 必須比兩者武功差值
            {
                if (data_E == null || data_I == null)
                    return false;
                int Rate = func.I(0);
                Rate += (int)((data_I.GetMar() - data_E.GetMar()) * 0.5f);
                int nRoll = Random.Range(0, 100);
                if(Rate < nRoll)
                {
                    return false;
                }

            }
            else if (func.sFunc == "HP_I")
            {
                float f1 = data_I.n_HP / data_I.GetMaxHP();
                float f2 = func.F(1);
                if (MyScript.Instance.ConditionFloat(f1, func.S(0), f2) == false)
                {
                    return false;       //  fail
                }
            }
            else if (func.sFunc == "HP_E")
            {
                float f1 = data_E.n_HP / data_E.GetMaxHP();
                float f2 = func.F(1);
                if (MyScript.Instance.ConditionFloat(f1, func.S(0), f2) == false)
                {
                    return false;       //  fail
                }
            }
            else if (func.sFunc == "MP_I")
            {
                float f1 = data_I.n_MP / data_I.GetMaxMP();
                float f2 = func.F(1);
                if (MyScript.Instance.ConditionFloat(f1, func.S(0), f2) == false)
                {
                    return false;       //  fail
                }
            }
            else if (func.sFunc == "MP_E")
            {
                float f1 = data_E.n_MP / data_E.GetMaxMP();
                float f2 = func.F(1);
                if (MyScript.Instance.ConditionFloat(f1, func.S(0), f2) == false)
                {
                    return false;       //  fail
                }
            }
            else if (func.sFunc == "POW_I")
            {
                int i1 = data_I.GetPow();
                int i2 = 0;


                string s1 = func.S(1);  // s1 
                if (s1 == "E")
                {
                    if (data_E != null)
                    {
                        i2 = data_E.GetPow();
                    }
                    else
                    {
                        return false; // no enemy is false
                    }
                }
                else if (s1 == "I")
                {
                    if (data_I != null)
                    {
                        i2 = data_I.GetPow();
                    }
                    else
                    {
                        return false; // no enemy is false
                    }
                }
                else
                {
                    i2 = func.I(1);
                }
                //
                if (MyScript.Instance.ConditionInt(i1, func.S(0), i2) == false)
                {
                    return false;       // always fail
                }

            }
            else if (func.sFunc == "POW_E")
            {
                int i1 = data_E.GetPow();
                int i2 = 0;


                string s1 = func.S(1);  // s1 
                if (s1 == "E")
                {
                    if (data_E != null)
                    {
                        i2 = data_E.GetPow();
                    }
                    else
                    {
                        return false; // no enemy is false
                    }
                }
                else if (s1 == "I")
                {
                    if (data_I != null)
                    {
                        i2 = data_I.GetPow();
                    }
                    else
                    {
                        return false; // no enemy is false
                    }
                }
                else
                {
                    i2 = func.I(1);
                }
                //
                if (MyScript.Instance.ConditionInt(i1, func.S(0), i2) == false)
                {
                    return false;       // always fail
                }

            }
            else if (func.sFunc == "MAR_I")
            {
                float f1 = data_I.GetMar();
                float f2 = 0.0f;
                string s1 = func.S(1);  // s1 
                if (s1 == "E")
                {
                    if (data_E != null)
                    {
                        f2 = data_E.GetMar();
                    }
                    else
                    {
                        return false; // no enemy is false
                    }
                }
                else if (s1 == "I")
                {
                    if (data_I != null)
                    {
                        f2 = data_I.GetMar();
                    }
                    else
                    {
                        return false; // no enemy is false
                    }
                }
                else
                {
                    f2 = func.F(1);
                }
                //
                if (MyScript.Instance.ConditionFloat(f1, func.S(0), f2) == false)
                {
                    return false;       // always fail
                }
            }
            else if (func.sFunc == "MAR_E")
            {
                float f1 = data_E.GetMar();
                float f2 = 0.0f;
                string s1 = func.S(1);  // s1 
                if (s1 == "E")
                {
                    if (data_E != null)
                    {
                        f2 = data_E.GetMar();
                    }
                    else
                    {
                        return false; // no enemy is false
                    }
                }
                else if (s1 == "I")
                {
                    if (data_I != null)
                    {
                        f2 = data_I.GetMar();
                    }
                    else
                    {
                        return false; // no enemy is false
                    }
                }
                else
                {
                    f2 = func.F(1);
                }
                //
                if (MyScript.Instance.ConditionFloat(f1, func.S(0), f2) == false)
                {
                    return false;       // always fail
                }
            }
            else if (func.sFunc == "BUFF_I")
            {
                int buffid = func.I(0);
                if (data_I != null)
                {
                    if (!data_I.Buffs.HaveBuff(buffid))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;       // always fail
                }
            }
            else if (func.sFunc == "BUFF_E")
            {
                int buffid = func.I(0);
                if (data_E != null)
                {
                    if (!data_E.Buffs.HaveBuff(buffid))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;       // always fail
                }
            }
            else if (func.sFunc == "SCHOOL_I")
            {
                int schoolid = func.I(0);
                // if (data_I.nActSch[0] != schoolid && data_I.nActSch[1] != schoolid)
                if (data_I == null)
                    return false;

                if (!data_I.IsActiveSchool(schoolid))
                {
                    return false;
                }

            }
            else if (func.sFunc == "SCHOOL_E")
            {
                int schoolid = func.I(0);
                if (data_E == null)
                    return false;

                if (!data_E.IsActiveSchool(schoolid))
                {
                    return false;
                }
            }
            else if (func.sFunc == "NOSCHOOL_E")
            {
                int schoolid = func.I(0);
                if (data_E == null)
                    return false;

                if (data_E.IsActiveSchool(schoolid))
                {
                    return false;
                }
            }
            else if (func.sFunc == "SLV_I")
            {
                int schoolid = func.I(0);
                if (data_I == null)
                    return false;
                int nLv = data_I.GetSchoolLv(schoolid);
                if (MyScript.Instance.ConditionInt(nLv, func.S(1), func.I(2)) == false)
                {
                    return false;       // always fail
                }
            }
            else if (func.sFunc == "SLV_E")
            {
                int schoolid = func.I(0);
                if (data_E == null)
                    return false;
                int nLv = data_E.GetSchoolLv(schoolid);
                if (MyScript.Instance.ConditionInt(nLv, func.S(1), func.I(2)) == false)
                {
                    return false;       // always fail
                }
            }
            else if (func.sFunc == "CHAR_CHECK") // combo check
            {
                bool bFind = false;
                int charid = func.I(0);
                int dist = func.I(1);
                int schoolid = func.I(2);
                int buffid = func.I(3);
                foreach (KeyValuePair<int, cUnitData> pair in GameDataManager.Instance.UnitPool)
                {
                    cUnitData unit = pair.Value;
                    if (unit == null) continue;

                    if (unit.n_CharID != charid)
                        continue;

                    if (dist > 0 && (dist < iVec2.Dist(data_I.n_X, data_I.n_Y, unit.n_X, unit.n_Y)))
                    {
                        continue;
                    }

                    if (schoolid > 0 && !unit.IsActiveSchool(schoolid))
                    {
                        continue;
                    }

                    if (buffid > 0 && !data_I.Buffs.HaveBuff(buffid))
                    {
                        continue;
                    }

                    // break when find
                    bFind = true;
                    break;

                }
                // check find
                if (bFind == false)
                {
                    return false;
                }

            }


            else if (func.sFunc == "RANGE" || func.sFunc == "DIST")
            {
                if ((data_I != null) && (data_E != null))
                {
                    int Range = func.I(1);
                    int nDist = iVec2.Dist(data_I.n_X, data_I.n_Y, data_E.n_X, data_E.n_Y);
                    if (MyScript.Instance.ConditionInt(nDist, func.S(0), Range) == false)
                    {
                        return false;       // always fail
                    }

                }
                else
                {
                    return false;
                }
            }
            else if (func.sFunc == "DIST_C")
            {
                cUnitData pCast = buff.GetCastUnit();
                if (pCast != null)
                {
                    int Range = func.I(1);
                    int nDist = iVec2.Dist(data_I.n_X, data_I.n_Y, pCast.n_X, pCast.n_Y);
                    if (MyScript.Instance.ConditionInt(nDist, func.S(0), Range) == false)
                    {
                        return false;       // always fail
                    }
                }
                else
                {
                    return false;
                }
            }

            else if (func.sFunc == "GENDER_I")
            {
                int gender = func.I(0);
                if (data_I != null)
                {
                    CHARS charData = ConstDataManager.Instance.GetRow<CHARS>(data_I.n_CharID);
                    if (gender != charData.n_GENDER)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (func.sFunc == "GENDER_E")
            {
                int gender = func.I(0);
                if (data_E != null)
                {
                    CHARS charData = ConstDataManager.Instance.GetRow<CHARS>(data_E.n_CharID);
                    if (gender != charData.n_GENDER)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            //雙方內外功星等比較
            else if (func.sFunc == "INTRANK_I")
            {
                float i1 = data_I.GetIntSchRank();
                float i2 = 0;
                string s1 = func.S(1);  // s1 
                if (s1 == "E")
                {
                    if (data_E != null)
                    {
                        i2 = data_E.GetIntSchRank();
                    }
                    else
                    {
                        return false; // no enemy is false
                    }
                }
                else if (s1 == "I")
                {
                    if (data_I != null)
                    {
                        i2 = data_I.GetIntSchRank();
                    }
                    else
                    {
                        return false; // no enemy is false
                    }
                }
                else
                {
                    i2 = func.I(1);
                }
                //
                if (MyScript.Instance.ConditionInt(i1, func.S(0), i2) == false)
                {
                    return false;       // always fail
                }

            }
            else if (func.sFunc == "EXTRANK_I")
            {
                float i1 = data_I.GetExtSchRank();
                float i2 = 0;
                string s1 = func.S(1);  // s1 
                if (s1 == "E")
                {
                    if (data_E != null)
                    {
                        i2 = data_E.GetExtSchRank();
                    }
                    else
                    {
                        return false; // no enemy is false
                    }
                }
                else if (s1 == "I")
                {
                    if (data_I != null)
                    {
                        i2 = data_I.GetExtSchRank();
                    }
                    else
                    {
                        return false; // no enemy is false
                    }
                }
                else
                {
                    i2 = func.I(1);
                }
                //
                if (MyScript.Instance.ConditionInt(i1, func.S(0), i2) == false)
                {
                    return false;       // always fail
                }
            }
            else if (func.sFunc == "ITEM_I")
            {
                int nItemID = func.I(0);
                if (data_I.CheckItemEquiped(nItemID) == false)
                {
                    return false;
                }
            }
            else if (func.sFunc == "COUNT")
            {
                if (MyScript.Instance.ConditionCount(func.I(0), func.S(1), func.I(2), func.I(3)) == false)
                {
                    return false;
                }
            }
            // Fight stat check . need entry battle first
            else if (func.sFunc == "FST_ATKER")
            {
                if (!BattleManager.Instance.IsBattlePhase())
                    return false;

                if ( data_I.FightStates(_FIGHTSTATE._ATKER) == false)
                    return false;
            }
            else if (func.sFunc == "FST_DEFER")
            {
                if (!BattleManager.Instance.IsBattlePhase())
                    return false;
                if (data_I.FightStates(_FIGHTSTATE._ATKER))
                    return false;
            }
            else if (func.sFunc == "FST_DAMAGE")
            {
                if (!BattleManager.Instance.IsBattlePhase())
                    return false;
                bool bIsDamage = data_I.FightStates(_FIGHTSTATE._DAMAGE);
                if (bIsDamage == false)
                {
                    return false;
                }

            }
            else if (func.sFunc == "FST_HELP")
            {
                if (!BattleManager.Instance.IsBattlePhase())
                    return false;
                bool bIsDamage = data_I.FightStates(_FIGHTSTATE._DAMAGE);
                if (bIsDamage)
                {
                    return false;
                }
            }
            else if (func.sFunc == "FST_KILL")
            {
                if (!BattleManager.Instance.IsBattlePhase())
                    return false;
                if (data_I.FightStates(_FIGHTSTATE._KILL) == false)
                    return false;
            }
            else if (func.sFunc == "FST_DEAD")
            {
                if (!BattleManager.Instance.IsBattlePhase())
                    return false;
                if (data_I.FightStates(_FIGHTSTATE._DEAD) == false)
                    return false;
            }
            else if (func.sFunc == "FST_DODGE")
            {
                if (!BattleManager.Instance.IsBattlePhase())
                    return false;
                if (data_I.FightStates(_FIGHTSTATE._DODGE) == false)
                    return false;
            }
            else 
            {
                Debug.LogError(string.Format("Error-Can't find script cond func '{0}' ", func.sFunc));
              
            }
            
        }

        // 都過了，傳成功
        return true;
    }


    public bool Check(cBuffData buff, cUnitData data_I, cUnitData data_E, int nSkillID, int nBuffID)
    {

        if (CondLst == null)
            return false;

        foreach (List<cTextFunc> funcList in CondLst)
        {
            if (CheckCond(buff, data_I, data_E, funcList, nSkillID, nBuffID) == true)
            {
                return true;            // any one passed. all passed
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
