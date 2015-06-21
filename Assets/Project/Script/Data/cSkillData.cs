using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;
//using _SRW;

// Tag state
public enum _SKILLTAG
{
	_DAMAGE 	= 1,
	_AOE		= 2,
	_BANATK 	= 3,	//攻擊 禁用
	_BANDEF 	= 4,	//防禦 禁用
	_MOVE 		= 5, 	// can move atk
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
		// fill data
		UseCond = MyScript.Instance.CreateEffectCondition ( skill.s_CONDITION );

		CastPool = MyScript.Instance.CreateEffectPool ( skill.s_CAST );
		CastCond = MyScript.Instance.CreateEffectCondition ( skill.s_CAST_TRIG);
		CastCondEffectPool  = MyScript.Instance.CreateEffectPool ( skill.s_CAST_EFFECT );

		HitPool = MyScript.Instance.CreateEffectPool ( skill.s_HIT );
		HitCond = MyScript.Instance.CreateEffectCondition ( skill.s_HIT_TRIG );
		HitCondEffectPool = MyScript.Instance.CreateEffectPool ( skill.s_HIT_EFFECT );

		// Set TAG 
		string[] tags = skill.s_TAG.Split ( ";".ToCharArray() );
		foreach (string s in tags) {
			int tag =0;
			if( int.TryParse( s , out tag ) ){ 
				AddTag( ( _SKILLTAG)tag );
			}

		}


	}
//	public List< cBuffCondition > ConditionPool;

	public cEffectCondition	  UseCond;				//  use condition

	// cast 
	public List< cEffect > 	  CastPool;				// normal
	public cEffectCondition	  CastCond;				// condition
	public List< cEffect > 	  CastCondEffectPool;	// Trig Effect

	// 
	public List< cEffect > 	  HitPool;				// normal
	public cEffectCondition	  HitCond;				// condition
	public List< cEffect > 	  HitCondEffectPool;	// Trig Effect


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

}

