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
	public int nSkillID ;		// which skill cast this buff, for fast remove to ensure no bug


	public BUFF tableData = null ;		// const data reference	

	public cBuffData( BUFF buff , int ident , int skillid ){
		tableData = buff;
		if (tableData == null)
			return;

		nID = buff.n_ID;
		nTime = buff.n_DURATION;
		nNum = 1;

		nCastIdent = ident;
		nSkillID   = skillid;

		// create eff pool
		ConditionEffectPool = MyScript.Instance.CreateEffectPool ( buff.s_CONDITIONAL_BUFF );
		Condition 			 = MyScript.Instance.CreateEffectCondition ( buff.s_BUFF_CONDITON );
		EffectPool 		 = MyScript.Instance.CreateEffectPool ( buff.s_CONSTANT_BUFF );
		
//		CancelCondition	 = MyScript.Instance.CreateEffectCondition ( buff.s_BUFF_CANCEL );

	}


	// 
//	public List< cBuffCondition > ConditionPool;
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
		Owner = unit;
	}

	public Dictionary< int , cBuffData > Pool;

	//
	public cBuffData CreateData( BUFF buff , int Ident , int skillid  )
	{
		cBuffData data = new cBuffData( buff , Ident , skillid  );
	
		return data;
	}

	//
	public cBuffData AddBuff( int nBuffID , int nCastIdent=0 , int nSkillID = 0 ){
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
					data = CreateData(buff, nCastIdent , nSkillID  );

					Pool[ buff.n_STACK ] = data;		// replace
				}
				else{
					return null;
				}
			}
		}
		else {
			//add buff
			data = CreateData(buff, nCastIdent , nSkillID );
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
		List< int > lst = new List< int >();
		foreach( KeyValuePair<int , cBuffData>  pair in Pool )
		{
			if( pair.Value.nSkillID == skillid ){
				lst.Add( pair.Key );
			}
		}

		foreach( int id in lst ){
			Pool.Remove( id );
		}
	}

	public void DelBuffByCastID( int castid ){
		List< int > lst = new List< int >();
		foreach( KeyValuePair<int , cBuffData>  pair in Pool )
		{
			if( pair.Value.nCastIdent == castid ){
				lst.Add( pair.Key );
			}
		}
		
		foreach( int id in lst ){
			Pool.Remove( id );
		}
	}


	public bool DelBuff( int nBuffID ){
		BUFF buff = ConstDataManager.Instance.GetRow< BUFF > ( nBuffID );
		if (buff == null)
			return false;
		cBuffData data = null;
		if (Pool.TryGetValue( buff.n_STACK , out data ) == true ){

			if( data.nID ==  nBuffID ){
				Owner.SetUpdate ( cAttrData._BUFF );
				Pool.Remove( buff.n_STACK );
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
		List< int > lst = new List< int >();
		foreach( KeyValuePair<int , cBuffData>  pair in Pool )
		{
			//if( pair.Value.nCastIdent == castid )
			if( pair.Value.nTime > 0 )
			{
				if( --pair.Value.nTime == 0 ){
					lst.Add( pair.Key );
				}
			}
		}


		foreach( int id in lst ){
			Pool.Remove( id );
		}

		return (lst.Count>0);
	}

	// remove all buff that time = -1 
	public bool BuffFightEnd()
	{
		// del buff when fight end
		List< int > lst = new List< int >();
		foreach( KeyValuePair<int , cBuffData>  pair in Pool )
		{
			//if( pair.Value.nCastIdent == castid )
			if( pair.Value.nTime < 0 )
			{
				lst.Add( pair.Key );
			}
		}
		
		
		foreach( int id in lst ){
			Pool.Remove( id );
		}
		
		return (lst.Count>0);
	}

	public void UpdateAttr( ref cAttrData attr  )
	{
		attr.Reset ();
		if (Pool.Count == 0)
			return;
		cUnitData unit_e = GameDataManager.Instance.GetUnitDateByIdent ( Owner.FightAttr.TarIdent );
		foreach( KeyValuePair< int , cBuffData > pair in Pool )
		{
			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null )
				{
					eft._Attr( Owner , unit_e ,  ref attr );
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
			//if( MyScript.Instance.CheckSkillCond( pair.Value.tableData.s_BUFF_CONDITON , this.Owner , unit_e ) == true )
			if( pair.Value.Condition.Check( this.Owner , unit_e ) )
			{
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null )
					{
						eft._Attr( Owner , unit_e , ref attr );
					}
				}
			}
		}
	}

	// on hit event
	public void OnHit(  ref List< cHitResult > resPool )
	{
		if (Pool.Count == 0)
			return;
		// normal hit
		cUnitData unit_e = GameDataManager.Instance.GetUnitDateByIdent ( Owner.FightAttr.TarIdent );
		foreach( KeyValuePair< int , cBuffData > pair in Pool )
		{
			// normal 
			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null )
				{
					eft._OnHit( Owner , unit_e , ref resPool );
				}
			}
			// condition
			if( pair.Value.Condition.Check( Owner , unit_e ) )
			{
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null )
					{
						eft._OnHit( Owner , unit_e , ref resPool );
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
			//if( MyScript.Instance.CheckSkillCond( pair.Value.tableData.s_BUFF_CONDITON , this.Owner , unit_e ) == true )
			if( pair.Value.Condition.Check( this.Owner , unit_e ) )
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
