using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;
//using _SRW;

// Tag state
public enum _SKILLTAG
{
	_DAMAGE 	= 1,	// 造成傷害
	_FLY		= 2,	// 飛行道具
	_BANATK 	= 3,	//攻擊 禁用
	_BANDEF 	= 4,	//防禦 禁用
	_NOMOVE 	= 5, 	//不可移動後攻擊
	_ROTATE		= 6,	// 原地旋轉
	_JUMP		= 7,	// 跳躍攻擊
	_FLASH		= 8,	// AOE閃擊
	_BOW		= 9,	// 拉弓攻擊
	_NOACT		= 10,	// 無表演直接算傷害
	_CROSS		= 11,	// 穿過
	// TAG for mob ai check
	_TIEUP 		= 100,	//牽制
};


// use for cache skill table data
public class cSkillData
{
	public int nID ;
	public SKILL skill;
	// 
	public cSkillData( SKILL skl )
	{
		skill = skl;
		if (skill == null)
			return; 
		nID = skill.n_ID;
		// fill data
		UseCond = MyScript.Instance.CreateEffectCondition ( skill.s_CONDITION );

		EffPool = MyScript.Instance.CreateEffectPool ( skill.s_CAST );
		EffCond = MyScript.Instance.CreateEffectCondition ( skill.s_CAST_COND);
		CondEffPool  = MyScript.Instance.CreateEffectPool ( skill.s_CAST_EFFECT);


//		CastPool = MyScript.Instance.CreateEffectPool ( skill.s_CAST );
//		CastCond = MyScript.Instance.CreateEffectCondition ( skill.s_CAST_TRIG);
//		CastCondEffectPool  = MyScript.Instance.CreateEffectPool ( skill.s_CAST_EFFECT );

//		HitPool = MyScript.Instance.CreateEffectPool ( skill.s_HIT );
//		HitCond = MyScript.Instance.CreateEffectCondition ( skill.s_HIT_TRIG );
//		HitCondEffectPool = MyScript.Instance.CreateEffectPool ( skill.s_HIT_EFFECT );


		// ???
		foreach (cEffect eft in EffPool) {
			eft.SetBaseParam( nID ,  0 ); // skill id 
		}
		foreach (cEffect eft in CondEffPool) {
			eft.SetBaseParam( nID ,  0 ); // skill id 
		}


//		foreach (cEffect eft in CastPool) {
//			eft.SetBaseParam( nID ,  0 ); // skill id 
//		}
//		foreach (cEffect eft in CastCondEffectPool) {
//			eft.SetBaseParam( nID ,  0 ); // skill id 
//		}
//		foreach (cEffect eft in HitPool) {
//			eft.SetBaseParam( nID ,  0 ); // skill id 
//		}
//		foreach (cEffect eft in HitCondEffectPool) {
//			eft.SetBaseParam( nID ,  0 ); // skill id 
//		}

		// Set TAG 
		string[] tags = skill.s_TAG.Split ( ";".ToCharArray() );
		foreach (string s in tags) {
			int tag =0;
			if( int.TryParse( s , out tag ) ){ 
				AddTag( ( _SKILLTAG)tag );
			}
			else{
				Debug.LogErrorFormat( "paser skill tag fail with {0} , maybe have ':'  not ';' " , nID );

			}

		}


	}
//	public List< cBuffCondition > ConditionPool;

	public cEffectCondition	  UseCond;				//  use condition

	// cast 
	public List< cEffect > 	  EffPool;				// normal
	public cEffectCondition	  EffCond;				// condition
	public List< cEffect > 	  CondEffPool;	// Trig Effect


//	public List< cEffect > 	  CastPool;				// normal
//	public cEffectCondition	  CastCond;				// condition
//	public List< cEffect > 	  CastCondEffectPool;	// Trig Effect
//
//	// 
//	public List< cEffect > 	  HitPool;				// normal
//	public cEffectCondition	  HitCond;				// condition
//	public List< cEffect > 	  HitCondEffectPool;	// Trig Effect


	// Tag arrag
	List< _SKILLTAG > TAGS;
	
	List< _SKILLTAG > GetTags()
	{
		if (TAGS == null) {
			TAGS = new List< _SKILLTAG >();
		}
		return TAGS;
	}
	
	public void AddTag( _SKILLTAG st ){
		if ( GetTags ().IndexOf (st) < 0 ) {
			GetTags ().Add( st );
		}
		
	}
	
	public bool IsTag( _SKILLTAG st ){ return  (GetTags ().IndexOf(st)>=0) ; }


	public void DoCastEffect( cUnitData atker , cUnitData defer , ref List<cHitResult>  pool  ){
		AttrEffect ( atker , defer  );
		DoEffect ( atker , defer , ref  pool );

	}

	public void DoHitEffect( cUnitData atker , cUnitData defer , ref List<cHitResult>  pool  ){
	//	AttrEffect ( atker , defer , HitPool ,HitCond , HitCondEffectPool );
		HitEffect ( atker , defer ,ref  pool );
	}

	public void DoBeHitEffect( cUnitData atker , cUnitData defer , ref List<cHitResult>  pool  ){
		BeHitEffect( atker , defer ,ref  pool );
	}

	// utility func
	public void DoEffect( cUnitData atker , cUnitData defer , ref List<cHitResult>  pool  )
	{
		if (atker == null || EffPool == null )
			return;
		
		//cUnitData defer = GameDataManager.Instance.GetUnitDateByIdent ( atker.FightAttr.TarIdent );
		
		// normal eff
		foreach( cEffect eft  in EffPool )
		{
			eft._Do( atker , defer , ref  pool );
		}
		if ( EffCond == null || CondEffPool == null)
			return;
		
		//cond eff
		//if (MyScript.Instance.CheckSkillCond (strCond, atker, defer) == true)
		if( EffCond.Check( atker , defer ,nID , 0  ) == true )
		{
			
			foreach( cEffect eft  in CondEffPool )
			{
				eft._Do( atker , defer , ref pool );
			}
		}
	}
	public void HitEffect( cUnitData atker , cUnitData defer  , ref List<cHitResult>  pool  )
	{
		if (atker == null || EffPool == null )
			return;
		
		//cUnitData defer = GameDataManager.Instance.GetUnitDateByIdent ( atker.FightAttr.TarIdent );
		
		// normal eff
		foreach( cEffect eft  in EffPool )
		{
			eft._Hit ( atker , defer , ref  pool );
		}
		if ( EffCond == null || CondEffPool == null)
			return;
		
		//cond eff
		//if (MyScript.Instance.CheckSkillCond (strCond, atker, defer) == true)
		if( EffCond.Check( atker , defer ,nID , 0  ) == true )
		{
			
			foreach( cEffect eft  in CondEffPool )
			{
				eft._Hit( atker , defer , ref pool );
			}
		}
	}

	public void BeHitEffect( cUnitData atker , cUnitData defer , ref List<cHitResult>  pool  )
	{
		if (atker == null || EffPool == null )
			return;
		
		//cUnitData defer = GameDataManager.Instance.GetUnitDateByIdent ( atker.FightAttr.TarIdent );
		
		// normal eff
		foreach( cEffect eft  in EffPool )
		{
			eft._BeHit( atker , defer , ref  pool );
		}
		if ( EffCond == null || CondEffPool == null)
			return;
		
		//cond eff
		//if (MyScript.Instance.CheckSkillCond (strCond, atker, defer) == true)
		if( EffCond.Check( atker , defer ,nID , 0  ) == true )
		{
			
			foreach( cEffect eft  in CondEffPool )
			{
				eft._BeHit( atker , defer , ref pool );
			}
		}
	}

	public void AttrEffect( cUnitData atker , cUnitData defer  )
	{
		if (atker == null || EffPool == null )
			return;
		cAttrData attr = atker.FightAttr;
		
		//cUnitData defer = GameDataManager.Instance.GetUnitDateByIdent ( atker.FightAttr.TarIdent );
		
		// normal eff
		foreach( cEffect eft  in EffPool )
		{
			eft._Attr(atker , defer , ref attr  )  ;
			//================================================
//			if( eft._IsStatus( _FIGHTSTATE._DODGE ) == true ){
//				atker.AddStates( _FIGHTSTATE._DODGE );
//			}
//			if( eft._IsStatus( _FIGHTSTATE._CIRIT ) == true ){
//				atker.AddStates( _FIGHTSTATE._CIRIT );
//			}
//			if( eft._IsStatus( _FIGHTSTATE._MERCY ) == true ){
//				atker.AddStates( _FIGHTSTATE._MERCY );
//			}
//			if( eft._IsStatus( _FIGHTSTATE._GUARD ) == true ){
//				atker.AddStates( _FIGHTSTATE._GUARD );
//			}
		}
		if ( EffCond == null || CondEffPool == null)
			return;
		
		//cond eff
		//if (MyScript.Instance.CheckSkillCond (strCond, atker, defer) == true)
		if( EffCond.Check( atker , defer , nID, 0 ) == true )
		{			
			foreach( cEffect eft  in CondEffPool )
			{
				eft._Attr(atker , defer ,ref attr  );

//				//================================================
//				if( eft._IsStatus( _FIGHTSTATE._DODGE ) == true ){
//					atker.AddStates( _FIGHTSTATE._DODGE );
//				}
//				if( eft._IsStatus( _FIGHTSTATE._CIRIT ) == true ){
//					atker.AddStates( _FIGHTSTATE._CIRIT );
//				}
//				if( eft._IsStatus( _FIGHTSTATE._MERCY ) == true ){
//					atker.AddStates( _FIGHTSTATE._MERCY );
//				}
//				if( eft._IsStatus( _FIGHTSTATE._GUARD ) == true ){
//					atker.AddStates( _FIGHTSTATE._GUARD );
//				}			
			}
		}
		
	}

	// check status
	public bool CheckStatus( cUnitData atker , cUnitData defer ,_FIGHTSTATE status ){
		if (atker == null || EffPool == null )
			return false;

		cUnitData unit_e = defer ;

		
		// normal effect
		foreach( cEffect eft in EffPool )
		{
			if( eft != null && eft._IsStatus( status ) )
			{
				return true	;
			}
		}
		// condition
		if ( EffCond == null || CondEffPool == null)
			return false;
		
		//if( MyScript.Instance.CheckSkillCond( pair.Value.tableData.s_BUFF_CONDITON , this.Owner , unit_e ) == true )
		if( EffCond.Check( atker , defer , nID ,0 ) )
		{
			foreach( cEffect eft in CondEffPool )
			{
				if( eft != null && eft._IsStatus( status ) )
				{
					return true	;
				}
			}
		}
		return false;
	}

}

