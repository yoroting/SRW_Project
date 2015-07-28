using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;
//using _SRW;

public class cBuffData
{
	public int nID ;
	public int nTime ;			//還有幾回合 
	public int nNum ;			//疊幾層了


	public int nCastIdent ;		// record castident
	public int nTargetIdent ;		// record targetident
	public int nSkillID ;		// which skill cast this buff, for fast remove to ensure no bug


	public BUFF tableData = null ;		// const data reference	

	public cBuffData( BUFF buff , int ident , int skillid , int tarident ){
		tableData = buff;
		if (tableData == null)
			return;

		nID = buff.n_ID;
		nTime = buff.n_DURATION;
		nNum = 1;

		nCastIdent = ident;
		nSkillID   = skillid;
		nTargetIdent = tarident;

		// create eff pool
		CancelCondition  = MyScript.Instance.CreateEffectCondition ( buff.s_BUFF_CANCEL );

		ConditionEffectPool = MyScript.Instance.CreateEffectPool ( buff.s_CONDITIONAL_BUFF );
		Condition 			 = MyScript.Instance.CreateEffectCondition ( buff.s_BUFF_CONDITON );
		EffectPool 		 = MyScript.Instance.CreateEffectPool ( buff.s_CONSTANT_BUFF );

		foreach (cEffect eft in ConditionEffectPool) {
			eft.SetBaseParam( skillid ,  nID);
		}

		foreach (cEffect eft in EffectPool) {
			eft.SetBaseParam( skillid ,  nID);
		}

//		CancelCondition	 = MyScript.Instance.CreateEffectCondition ( buff.s_BUFF_CANCEL );

	}


	// 
//	public List< cBuffCondition > ConditionPool;
	public cEffectCondition   CancelCondition;			// check buff cancel condition

	public List< cEffect > 	  EffectPool;				// normal effect
	public cEffectCondition   Condition;				// check condition

	public List< cEffect > 	  ConditionEffectPool;		// condition effect

//	public cEffectCondition   CancelCondition;			// some condition to cancel buff

}

// for unit to manage buff
public class cBuffs
{
	cUnitData Owner;		// owner
	public cBuffs( cUnitData unit ){
		Pool = new Dictionary< int , cBuffData > ();
		RemoveList = new List< int >();
		Owner = unit;
	}

	public Dictionary< int , cBuffData > Pool;
	List< int > RemoveList ;

	//
	public cBuffData CreateData( BUFF buff , int Ident , int skillid , int targetident  )
	{
		cBuffData data = new cBuffData( buff , Ident , skillid , targetident );
	
		return data;
	}

	//
	public cBuffData AddBuff( int nBuffID , int nCastIdent , int nSkillID  , int nTargetId ){
		BUFF buff = ConstDataManager.Instance.GetRow< BUFF > ( nBuffID );
		if (buff == null)
			return null;


		// always re cal next update
		Owner.SetUpdate ( cAttrData._BUFF );


		cBuffData data = null;
		cBuffData olddata = null;
		if (Pool.TryGetValue (buff.n_STACK, out olddata) == true) {
			// check if need replace
			if( nBuffID == olddata.nID ) // the same buff
			{
				olddata.nNum ++;
				STACK stack = ConstDataManager.Instance.GetRow< STACK >( buff.n_STACK );
				if( stack != null ){
					if( olddata.nNum > stack.n_MAX_STACK  ){
						olddata.nNum= stack.n_MAX_STACK ;
					}
				}
				// refresh time
				olddata.nTime =buff.n_DURATION;
				return olddata;
			}
			else{
				// diff buff. check which id high lv
				if( olddata.tableData.n_LV <= buff.n_LV )
				{
					data = CreateData(buff, nCastIdent , nSkillID,nTargetId  );

					Pool[ buff.n_STACK ] = data;		// replace
				}
				else{
					return null;
				}
			}
		}
		else {
			//add buff
			data = CreateData(buff, nCastIdent , nSkillID ,nTargetId  );
			Pool.Add( buff.n_STACK ,  data );
		}
		return data;
	}

	public bool DelBuffByStack( int nStack ){

		if (Pool.ContainsKey( nStack ) == true ){

			Owner.SetUpdate ( cAttrData._BUFF );

			Pool.Remove( nStack );
			return true;
		}

		return false;
	}

	public void DelBuffBySkillID( int skillid ){
		foreach( KeyValuePair<int , cBuffData>  pair in Pool )
		{
			if( pair.Value.nSkillID == skillid ){
				RemoveList.Add( pair.Key );
			}
		}

		foreach( int id in RemoveList ){
			Pool.Remove( id );
		}
		RemoveList.Clear ();
	}

	public void DelBuffByCastID( int castid ){

		foreach( KeyValuePair<int , cBuffData>  pair in Pool )
		{
			if( pair.Value.nCastIdent == castid ){
				RemoveList.Add( pair.Key );
			}
		}
		
		foreach( int id in RemoveList ){
			Pool.Remove( id );
		}
		RemoveList.Clear ();
	}


	public bool DelBuff( int nBuffID , bool DelAll = false ){
		BUFF buff = ConstDataManager.Instance.GetRow< BUFF > ( nBuffID );
		if (buff == null)
			return false;
		cBuffData data = null;
		if (Pool.TryGetValue( buff.n_STACK , out data ) == true ){

			if( data.nID ==  nBuffID ){
				Owner.SetUpdate ( cAttrData._BUFF );
				if( (DelAll==true) ||  --data.nNum <= 0 )
				{
					Pool.Remove( buff.n_STACK );
				}
				return true;
			}
		}
		
		return false;
	}

	public bool HaveBuff( int nBuffID ){
		foreach (KeyValuePair< int , cBuffData>  pair in Pool) {
			if( pair.Value.nID ==  nBuffID )
			{
				return true;
			}

		}

		return false;
	}
	public cBuffData GetBuff( int nBuffID ){
		foreach (KeyValuePair< int , cBuffData>  pair in Pool) {
			if( pair.Value.nID ==  nBuffID )
			{
				return pair.Value;
			}
		}

		return null;
	}


	// run 1 round .  buff time-1 with all >= 1 . remove buff if time become 0
	public bool BuffRoundEnd()
	{

		foreach( KeyValuePair<int , cBuffData>  pair in Pool )
		{
			//if( pair.Value.nCastIdent == castid )
			if( pair.Value.nTime > 0 )
			{
				if( --pair.Value.nTime == 0 ){
					RemoveList.Add( pair.Key );
				}
			}
		}


		foreach( int id in RemoveList ){
			Pool.Remove( id );
		}
		bool bUpdate = RemoveList.Count > 0;
		RemoveList.Clear ();
		return bUpdate;
	}

	// remove all buff that time = -1 
	public bool BuffFightEnd(  )
	{
		// del buff when fight end
		List< int > lst = new List< int >();
		cUnitData unit_e = GameDataManager.Instance.GetUnitDateByIdent ( Owner.FightAttr.TarIdent );
		foreach( KeyValuePair<int , cBuffData>  pair in Pool )
		{
			//if( pair.Value.nCastIdent == castid )
			if( pair.Value.nTime == 0 )
			{
				cUnitData unit = null ;
				if( pair.Value.nTargetIdent > 0 ){
					unit = GameDataManager.Instance.GetUnitDateByIdent ( pair.Value.nTargetIdent );
				}
				else {
					unit = unit_e;
				}
				// check buff end
				if( pair.Value.CancelCondition.Check( this.Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
				{				
					RemoveList.Add( pair.Key );
				}
			}
			else if( pair.Value.nTime == -1 ){ // 本次戰鬥有造成傷害
				if( Owner.IsStates( _UNITSTATE._DAMAGE ) ){
					RemoveList.Add( pair.Key );
				}
			}
			else if( pair.Value.nTime == -2 ){ //本次戰鬥為防守方
				if( Owner.IsStates( _UNITSTATE._ATKER ) == false ){
					RemoveList.Add( pair.Key );
				}
			}
		}		
		
		foreach( int id in RemoveList ){
			Pool.Remove( id );
		}
		bool bUpdate = RemoveList.Count > 0;
		RemoveList.Clear ();
		return bUpdate;
	}

	public void UpdateAttr( ref cAttrData attr  )
	{
		attr.Reset ();
		if (Pool.Count == 0)
			return;
		cUnitData unit_e = GameDataManager.Instance.GetUnitDateByIdent ( Owner.FightAttr.TarIdent );
		foreach( KeyValuePair< int , cBuffData > pair in Pool )
		{
			cUnitData unit = null ;
			if( pair.Value.nTargetIdent > 0 ){
				unit = GameDataManager.Instance.GetUnitDateByIdent ( pair.Value.nTargetIdent );
			}
			else {
				unit = unit_e;
			}

			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null )
				{
					eft._Attr( Owner , unit ,  ref attr );
				}
			}
		}
	}
	public void UpdateCondAttr( ref cAttrData attr  )
	{
		attr.Reset ();
		if (Pool.Count == 0)
			return;

		cUnitData unit_e = GameDataManager.Instance.GetUnitDateByIdent ( Owner.FightAttr.TarIdent );
		foreach( KeyValuePair< int , cBuffData > pair in Pool )
		{
			cUnitData unit = null ;
			if( pair.Value.nTargetIdent > 0 ){
				unit = GameDataManager.Instance.GetUnitDateByIdent ( pair.Value.nTargetIdent );
			}
			else {
				unit = unit_e;
			}
			//if( MyScript.Instance.CheckSkillCond( pair.Value.tableData.s_BUFF_CONDITON , this.Owner , unit_e ) == true )
			if( pair.Value.Condition.Check( this.Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
			{
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null )
					{
						eft._Attr( Owner , unit , ref attr );
					}
				}
			}
		}
	}

	public void OnDo(  cUnitData unit_e , ref List< cHitResult > resPool )
	{
		if (Pool.Count == 0)
			return;
		foreach( KeyValuePair< int , cBuffData > pair in Pool )
		{
			// normal 
			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null )
				{
					eft._Do( Owner , unit_e , ref resPool );
				}
			}
			cUnitData unit = null ;
			if( pair.Value.nTargetIdent > 0 ){
				unit = GameDataManager.Instance.GetUnitDateByIdent ( pair.Value.nTargetIdent );
			}
			else {
				unit = unit_e;
			}

			// condition
			if( pair.Value.Condition.Check( Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
			{
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null )
					{
						eft._Do( Owner , unit , ref resPool );
					}
				}
			}
		}
	}
	// on hit event

	public void OnCast(  cUnitData unit_e , ref List< cHitResult > resPool )
	{
		if (Pool.Count == 0)
			return;
		// normal hit
		foreach( KeyValuePair< int , cBuffData > pair in Pool )
		{
			// normal 
			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null )
				{
					eft._Hit( Owner , unit_e , ref resPool );
				}
			}
			// condition
			cUnitData unit = null ;
			if( pair.Value.nTargetIdent > 0 ){
				unit = GameDataManager.Instance.GetUnitDateByIdent ( pair.Value.nTargetIdent );
			}
			else {
				unit = unit_e;
			}

			if( pair.Value.Condition.Check( Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
			{
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null )
					{
						eft._Hit( Owner , unit , ref resPool );
					}
				}
			}
		}
	}

	public void OnHit(  cUnitData unit_e , ref List< cHitResult > resPool )
	{
		if (Pool.Count == 0)
			return;
		// normal hit
		foreach( KeyValuePair< int , cBuffData > pair in Pool )
		{
			// normal 
			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null )
				{
					eft._Hit( Owner , unit_e , ref resPool );
				}
			}

			// condition
			cUnitData unit = null ;
			if( pair.Value.nTargetIdent > 0 ){
				unit = GameDataManager.Instance.GetUnitDateByIdent ( pair.Value.nTargetIdent );
			}
			else {
				unit = unit_e;
			}

			if( pair.Value.Condition.Check( Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
			{
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null )
					{
						eft._Hit( Owner , unit , ref resPool );
					}
				}
			}
		}
	}

	public void OnBeHit(  cUnitData unit_e , ref List< cHitResult > resPool )
	{
		if (Pool.Count == 0)
			return;
		// normal hit
		foreach( KeyValuePair< int , cBuffData > pair in Pool )
		{
			// normal 
			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null )
				{
					eft._BeHit( Owner , unit_e , ref resPool );
				}
			}

			// condition
			cUnitData unit = null ;
			if( pair.Value.nTargetIdent > 0 ){
				unit = GameDataManager.Instance.GetUnitDateByIdent ( pair.Value.nTargetIdent );
			}
			else {
				unit = unit_e;
			}

			if( pair.Value.Condition.Check( Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
			{
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null )
					{
						eft._BeHit( Owner , unit , ref resPool );
					}
				}
			}
		}
		
	}

	public bool CheckStatus( int nStatus ,ref int iValue , ref float fValue){


		cUnitData unit_e = null ;
		if( Owner.FightAttr.TarIdent > 0 ){
			GameDataManager.Instance.GetUnitDateByIdent ( Owner.FightAttr.TarIdent );
		}
		foreach( KeyValuePair< int , cBuffData > pair in Pool )
		{
			// normal effect
			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null && eft._IsStatus( ref iValue , ref fValue ) )
				{
					return true	;
				}
			}

			// condition
			// condition
			cUnitData unit = null ;
			if( pair.Value.nTargetIdent > 0 ){
				unit = GameDataManager.Instance.GetUnitDateByIdent ( pair.Value.nTargetIdent );
			}
			else {
				unit = unit_e;
			}

			//if( MyScript.Instance.CheckSkillCond( pair.Value.tableData.s_BUFF_CONDITON , this.Owner , unit_e ) == true )
			if( pair.Value.Condition.Check( this.Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
			{
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null && eft._IsStatus( ref iValue , ref fValue ) )
					{
						return true	;
					}
				}
			}
		}

		return false;
	}

}
