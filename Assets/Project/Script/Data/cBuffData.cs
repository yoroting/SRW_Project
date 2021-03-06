using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;
using System.Linq;

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

	public cBuffData( BUFF buff , int ident , int skillid , int tarident , int num= 1)
    {
		tableData = buff;
		if (tableData == null)
			return;

		nID = buff.n_ID;
		nTime = buff.n_DURATION;
		nNum = num;
        if (nNum <= 0)
        {
            nNum = 1;
        }

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

	public cUnitData GetTargetUnit(){
		if (nTargetIdent != 0) {
			cUnitData p = GameDataManager.Instance.GetUnitDateByIdent( nTargetIdent );
			if( p == null ){
				Debug.LogFormat( "buff{0}'s target{1} is null" ,nID , nTargetIdent);
				nTargetIdent = 0; 
			}
			return p;
		}
		return null;
	}
	public cUnitData GetCastUnit(){
		if (nCastIdent != 0) {
			cUnitData p = GameDataManager.Instance.GetUnitDateByIdent( nCastIdent );
			if( p == null ){
				Debug.LogFormat( "buff{0}'s cast{1} is null" ,nID , nCastIdent);
				nCastIdent = 0; 
			}
			return p;
		}
		return null;
	}
    public int GetUITime()
    {
        int n = nTime;
        if (nTime > 0)
        {
            n = nTime / 3;
            if ( (nTime%3) !=0 )  {
                n++;
            }
        }

        return n;
    }
}

// for unit to manage buff
public class cBuffs
{
	cUnitData Owner;		// owner
	public cBuffs( cUnitData unit ){
		Pool = new Dictionary< int , cBuffData > ();
		//RemoveList = new List< int >();
		Owner = unit;
	}

	public Dictionary< int , cBuffData > Pool;
	//List< int > RemoveList ;
    
    // clear all buff
    public void Reset()
    {
        Pool.Clear();
    }

    //
    public cBuffData CreateData( BUFF buff , int Ident , int nSkillID , int nTargetId , int nNum=1 )
	{
		cBuffData data = new cBuffData( buff , Ident , nSkillID , nTargetId , nNum);
	
		return data;
	}

	//
	public cBuffData AddBuff( int nBuffID , int nCastIdent , int nSkillID  , int nTargetId , int nNum=0 ){
		BUFF buff = ConstDataManager.Instance.GetRow< BUFF > ( nBuffID );
		if (buff == null)
			return null;

		// check if immune
		if (CheckImmune( buff.n_BUFF_TYPE ) == true ) {
			return null;
		}

		// always re cal next update
		Owner.SetUpdate ( cAttrData._BUFF );

        
        // 預設為增加 1 stack
        if (nNum == 0)
            nNum = 1;

        cBuffData data = null;
		cBuffData olddata = null;
		if (Pool.TryGetValue (buff.n_STACK, out olddata) == true) {
			// check if need replace
			if( nBuffID == olddata.nID ) // the same buff
			{	
				STACK stack = ConstDataManager.Instance.GetRow< STACK >( buff.n_STACK );
                if (stack != null)
                {
                    olddata.nNum += nNum ;
                    if (olddata.nNum > stack.n_MAX_STACK)
                    {
                        olddata.nNum = stack.n_MAX_STACK;
                    }
                }
                else {
                    olddata.nNum = 1 ; // no stack data always = 1
                }
				// refresh time
				olddata.nCastIdent = nCastIdent;
				olddata.nSkillID = nSkillID;
				olddata.nTargetIdent = nTargetId;
				olddata.nTime = buff.n_DURATION;
				data = olddata;
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
			data = CreateData(buff, nCastIdent , nSkillID ,nTargetId  , nNum);
			Pool.Add( buff.n_STACK ,  data );
		}
        // 特別機制， 讓時間* 陣營數量，然後 每次 換陣營 都全部運算
        if (data != null) {
            if (data.nTime > 0)
            {
                data.nTime *= Config.CampNum;
            }
        }

		// play buff fx
		if (buff.n_BUFF_FXS > 0) {
			if( Panel_StageUI.Instance && (Panel_StageUI.Instance.m_bIsSkipMode==false) )
            {
				Panel_unit unit = Panel_StageUI.Instance.GetUnitByIdent( Owner.n_Ident );
				if( unit != null ){
                    //unit.PlayFX( buff.n_BUFF_FXS  , false ) ;
                    unit.PlayFX(buff.n_BUFF_FXS);
                }
			}
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

	public void DelBuffBySkillID( int skillid , bool bSelfOnly= false ){
        for (int i = 0; i < Pool.Count; i++)
        {
            KeyValuePair<int, cBuffData> pair = Pool.ElementAt(i);

            // 只能刪除自己施展的
            if (bSelfOnly == true)
            {
                if (pair.Value.nCastIdent > 0)
                {
                    continue; // 不是自己施展的不能刪除
                }
            }

            if ( pair.Value.nSkillID == skillid ){
                Pool.Remove(pair.Key);
                i--;
            }
		}
	}

	public void DelBuffByCastID( int castid ){

        for (int i = 0; i < Pool.Count; i++)
        {
            KeyValuePair<int, cBuffData> pair = Pool.ElementAt(i);
			if( pair.Value.nCastIdent == castid ){
                Pool.Remove(pair.Key);
                i--;
                //RemoveList.Add( pair.Key );
            }
		}
		

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

    // 檢查是否有指定 BUFF 並層數
	public bool HaveBuff( int nBuffID , int nNum = 0 ){
		foreach (KeyValuePair< int , cBuffData>  pair in Pool) {
			if( pair.Value.nID ==  nBuffID )
			{
                if (pair.Value.nNum >= nNum)
                {
                    return true;
                }
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

    public bool BuffCheckCancel() {

        bool bUpdate = false;

        cUnitData unit_e = GameDataManager.Instance.GetUnitDateByIdent(Owner.FightAttr.TarIdent);        
        for (int i = 0; i < Pool.Count; i++)
        {
            KeyValuePair<int, cBuffData> pair = Pool.ElementAt(i);
            // condition
            cUnitData unit = null;
            if (pair.Value.nTargetIdent > 0)
            {
                unit = pair.Value.GetTargetUnit();
                if (unit == null)
                {
                    Debug.LogFormat("Buff BuffCheckCancel CharID{0}-Buff{1} with null TargetIdent{2} ", Owner.n_CharID, pair.Value.nID, pair.Value.nTargetIdent);
                }
            }
            else
            {
                unit = unit_e;
            }

            //if( MyScript.Instance.CheckSkillCond( pair.Value.tableData.s_BUFF_CONDITON , this.Owner , unit_e ) == true )
            if (pair.Value.CancelCondition.Check(pair.Value, this.Owner, unit, pair.Value.nSkillID, pair.Value.nID))
            {               
                Pool.Remove(pair.Key);
                i--;  // 重新判斷 當前 index 
                bUpdate = true;
            }
        }
        // have buff need clear

        return bUpdate;
    }

	// 取得因BUFF而進階的技能
	public int GetUpgradeSkill( int nSkillID )
	{
		foreach( KeyValuePair<int , cBuffData>  pair in Pool )
		{
			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null )
				{
					int nTemp = eft._UpSkill( nSkillID ); 
					if( nTemp != nSkillID ){
						return nTemp;
					}
				}
			}
		}
		return nSkillID;
	}

	//
	public int GetGuarder(){
//		int nGuarder = 0;
		foreach( KeyValuePair<int , cBuffData>  pair in Pool )
		{
			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null )
				{
					if( eft._IsStatus( _FIGHTSTATE._GUARD ) ){
						if( pair.Value.nCastIdent != 0 && (pair.Value.nCastIdent!= Owner.n_Ident) )
                        {
							return pair.Value.nCastIdent;
						}
						
					}
				}
			}
			//condition
			if( pair.Value.Condition.Check( pair.Value , this.Owner , null , pair.Value.nSkillID , pair.Value.nID ) )
			{				
				//RemoveList.Add( pair.Key );
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null )
					{
						if( eft._IsStatus( _FIGHTSTATE._GUARD ) ){
							if( pair.Value.nCastIdent != 0 && (pair.Value.nCastIdent != Owner.n_Ident) )
                            {
								return pair.Value.nCastIdent;
							}							
						}
					}
				}
			}

		}
		return 0;
	}

    public bool BuffNextCamp()
    {
        bool bUpdate = false;

        for (int i = 0; i < Pool.Count; i++)
        {
            KeyValuePair<int, cBuffData> pair = Pool.ElementAt(i);
            if (pair.Value.nTime > 0) // 新作法， 一回合 3 tick
            {
                // 以下判斷 是否為 作用回合

                //// 如果不是自己施展的
                //if (pair.Value.nCastIdent > 0 && (pair.Value.nCastIdent != Owner.n_Ident))
                //{
                //    cUnitData unit = pair.Value.GetCastUnit();
                //    if (unit != null)
                //    {
                //        if (unit.eCampID != ecamp)
                //        {
                //            continue; // 不是 計數 回合， 不扣除次數
                //        }
                //    }
                //}

                //  以下判斷 是否要移除
                if (--pair.Value.nTime == 0) //移除結束的BUFF
                {
                    // 只能移除一層

                    Pool.Remove(pair.Key);
                    i--;  // 重新判斷 當前 index 
                    bUpdate = true;
                }
            }
        }
        return bUpdate;
    }
    // run 1 round .  buff time-1 with all >= 1 . remove buff if time become 0
    public bool BuffTimeToDel( _CAMP ecamp )
	{
        // 作用一次 buff
        //OnDo( null , ref act.HitResult);
        bool bUpdate = false;

        for (int i = 0; i < Pool.Count; i++)
        {
            KeyValuePair<int, cBuffData> pair = Pool.ElementAt(i);
            if (pair.Value.nTime > 0)
            {
                // 以下判斷 是否為 作用回合

                // 如果不是自己施展的
                if (pair.Value.nCastIdent > 0 && (pair.Value.nCastIdent != Owner.n_Ident )) {
                    cUnitData unit =   pair.Value.GetCastUnit();
                    if (unit != null)
                    {
                        if (unit.eCampID != ecamp) {
                            continue; // 不是 計數 回合， 不扣除次數
                        }
                    }
                }

                //  以下判斷 是否要移除
                if (--pair.Value.nTime == 0) //移除結束的BUFF
                {
                    Pool.Remove(pair.Key);
                    i--;  // 重新判斷 當前 index 
                    bUpdate = true;
                }
            }
        }


  //      //移除結束的BUFF
  //      foreach ( KeyValuePair<int , cBuffData>  pair in Pool )
		//{

		//	//if( pair.Value.nCastIdent == castid )
		//	if( pair.Value.nTime > 0 )
		//	{
		//		if( --pair.Value.nTime == 0 ){
		//			RemoveList.Add( pair.Key );
		//		}
		//	}
		//}

		//foreach( int id in RemoveList ){
		//	Pool.Remove( id );
		//}

		//// bool bUpdate = RemoveList.Count > 0;
		//RemoveList.Clear ();
		return bUpdate;
	}

	// remove all buff that time = -1 
	public bool BuffFightEnd(  )
	{
        // del buff when fight end
        //List< int > lst = new List< int >();
        bool bUpdate = false;

        cUnitData unit_e = GameDataManager.Instance.GetUnitDateByIdent ( Owner.FightAttr.TarIdent );
        for (int i = 0; i < Pool.Count; i++)
        {
            KeyValuePair<int, cBuffData> pair = Pool.ElementAt(i);
            //if( pair.Value.nCastIdent == castid )
            if ( pair.Value.nTime == 0 )
			{
				cUnitData unit = null ;
				if( pair.Value.nTargetIdent > 0 ){
					unit =  pair.Value.GetTargetUnit();
					if( unit == null ){
						Debug.LogErrorFormat( "Buff BuffFightEnd CharID{0}-Buff{1} with null TargetIdent{2} " , Owner.n_CharID , pair.Value.nID , pair.Value.nTargetIdent  );
					}
				}
				else {
					unit = unit_e;
				}
				// check buff end
				if( pair.Value.CancelCondition.Check( pair.Value , this.Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
				{
                    Pool.Remove(pair.Key);// 到零了要刪除
                    i--;
                    bUpdate = true;
                }
			}
			else if( pair.Value.nTime == -1 ){ 
				if( Owner.IsStates( _FIGHTSTATE._DAMAGE ) ){// 本次戰鬥有實際造成傷害
                    //數量減少一次
                    if ( -- pair.Value.nNum <= 0)
                    {
                        Pool.Remove(pair.Key);// 到零了要刪除
                        i--;
                        bUpdate = true;
                    }


                }
			}
			else if( pair.Value.nTime == -2 ){ //本次戰鬥為防守方
				if( Owner.IsStates( _FIGHTSTATE._BEDAMAGE ) == true ){
                    if ( -- pair.Value.nNum <= 0)
                    {
                        Pool.Remove(pair.Key);// 到零了要刪除
                        i--;
                        bUpdate = true;                       
                    }
                }
			}
		}		
		
		
		return bUpdate;
	}

	// clear when relive / stage end
	public bool BuffRelive()
	{
        // del buff when fight end
        //		List< int > lst = new List< int >();
        bool bUpdate = false;
        for (int i = 0; i < Pool.Count; i++)
        {
            KeyValuePair<int, cBuffData> pair = Pool.ElementAt(i);
            //if( pair.Value.nCastIdent == castid )
            if ( pair.Value.nTime != 0 )
			{
                Pool.Remove(pair.Key);// 到零了要刪除
                i--;
                bUpdate = true;
                
			}
		}				
		
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
				unit =  pair.Value.GetTargetUnit();
				if( unit == null ){
					Debug.LogFormat( "UpdateAttr CharID{0}-Buff{1} with null TargetIdent{2} " , Owner.n_CharID , pair.Value.nID , pair.Value.nTargetIdent  );
				}
			}
			else {
				unit = unit_e;
			}

			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null )
				{
					eft._Attr( Owner , unit ,  ref attr , pair.Value.nNum  );
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
				unit =  pair.Value.GetTargetUnit();
				if( unit == null ){
					Debug.LogFormat( "UpdateCondAttr CharID{0}-Buff{1} with null TargetIdent{2} " , Owner.n_CharID , pair.Value.nID , pair.Value.nTargetIdent  );
				}
			}
			else {
				unit = unit_e;
			}
			//if( MyScript.Instance.CheckSkillCond( pair.Value.tableData.s_BUFF_CONDITON , this.Owner , unit_e ) == true )
			if( pair.Value.Condition.Check( pair.Value , this.Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
			{
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null )
					{
						eft._Attr( Owner , unit , ref attr, pair.Value.nNum );
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
            cUnitData unit_cast = GameDataManager.Instance.GetUnitDateByIdent(pair.Value.nCastIdent);
          
            // normal 
            foreach ( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null )
				{
					eft._Do( Owner , unit_cast  , ref resPool );
				}
			}
			cUnitData unit = null ;
			if( pair.Value.nTargetIdent > 0 ){
				unit = pair.Value.GetTargetUnit();
				if( unit == null ){
					Debug.LogFormat( "Buff OnDo CharID{0}-Buff{1} with null TargetIdent{2} " , Owner.n_CharID , pair.Value.nID , pair.Value.nTargetIdent  );
				}
			}
			else {
				unit = unit_e;
			}

			// condition
			if( pair.Value.Condition.Check( pair.Value , Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
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
					eft._Cast( Owner , unit_e , ref resPool );
				}
			}
			// condition
			cUnitData unit = null ;
			if( pair.Value.nTargetIdent > 0 ){
				unit =  pair.Value.GetTargetUnit();
				if( unit == null ){
					Debug.LogFormat( "Buff OnCast CharID{0}-Buff{1} with null TargetIdent{2} " , Owner.n_CharID , pair.Value.nID , pair.Value.nTargetIdent  );
				}
			}
			else {
				unit = unit_e;
			}

			if( pair.Value.Condition.Check( pair.Value , Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
			{
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null )
					{
						eft._Cast( Owner , unit , ref resPool );
					}
				}
			}
		}
	}

	// hit
	public void OnHit(  cUnitData unit_e , ref List< cHitResult > resPool )
	{
		if (Pool.Count == 0)
			return;
        if (unit_e == Owner)
            return;
        //		// dodge 
        //		if( unit_e != null ){
        //			if( unit_e.IsStates(_FIGHTSTATE._DODGE)  ){
        //				return ;
        //			}
        //		}
        //		// miss
        //		if( Owner.IsStates( _FIGHTSTATE._MISS ) ){
        //			return ;
        //		}

        // normal hit
        foreach ( KeyValuePair< int , cBuffData > pair in Pool )
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
				unit =  pair.Value.GetTargetUnit();
				if( unit == null ){
					Debug.LogFormat( "Buff OnHit CharID{0}-Buff{1} with null TargetIdent{2} " , Owner.n_CharID , pair.Value.nID , pair.Value.nTargetIdent  );
				}
			}
			else {
				unit = unit_e;
			}

			if( pair.Value.Condition.Check( pair.Value, Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
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
        if (unit_e == Owner)
            return;
        // normal hit
        foreach ( KeyValuePair< int , cBuffData > pair in Pool )
		{
			// normal 
			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null )
				{
					eft._BeHit( unit_e , Owner , ref resPool );
				}
			}

			// condition
			cUnitData unit = null ;
			if( pair.Value.nTargetIdent > 0 ){
				unit =  pair.Value.GetTargetUnit();
				if( unit == null ){
					Debug.LogFormat( "Buff OnBeHit CharID{0}-Buff{1} with null TargetIdent{2} " , Owner.n_CharID , pair.Value.nID , pair.Value.nTargetIdent  );
				}
			}
			else {
				unit = unit_e;
			}

			if( pair.Value.Condition.Check( pair.Value , Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
			{
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null )
					{
						eft._BeHit( unit , Owner , ref resPool );
					}
				}
			}
		}
		
	}
	public bool CheckImmune( int nBuffType ){
		if (nBuffType == 0)
			return false;

		cUnitData unit_e = null ;
		if( Owner.FightAttr.TarIdent > 0 ){
			GameDataManager.Instance.GetUnitDateByIdent ( Owner.FightAttr.TarIdent );
		}
		foreach( KeyValuePair< int , cBuffData > pair in Pool )
		{
			// normal effect
			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null && eft._IsImmune( nBuffType ) )
				{
					return true	;
				}
			}
			// condition
			cUnitData unit = null ;
			if( pair.Value.nTargetIdent > 0 ){
				unit =  pair.Value.GetTargetUnit();
				if( unit == null ){
					Debug.LogFormat( "Buff CheckStatus CharID{0}-Buff{1} with null TargetIdent{2} " , Owner.n_CharID , pair.Value.nID , pair.Value.nTargetIdent  );
				}
			}
			else {
				unit = unit_e;
			}
			
			//if( MyScript.Instance.CheckSkillCond( pair.Value.tableData.s_BUFF_CONDITON , this.Owner , unit_e ) == true )
			if( pair.Value.Condition.Check( pair.Value , this.Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
			{
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null &&  eft._IsImmune( nBuffType ) )
					{
						return true	;
					}
				}
			}
		}
		
		return false;
	}

	public bool CheckTag( _UNITTAG tag ){
		cUnitData unit_e = null ;
		//if( Owner.FightAttr.TarIdent > 0 ){
  //          unit_e = GameDataManager.Instance.GetUnitDateByIdent ( Owner.FightAttr.TarIdent );
		//}
		foreach( KeyValuePair< int , cBuffData > pair in Pool )
		{
			// normal effect
			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null && eft._IsTag( tag ) )
				{
					return true	;
				}
			}
			// condition
			cUnitData unit = null ;
			if( pair.Value.nTargetIdent > 0 ){
				unit = pair.Value.GetTargetUnit();
				if( unit == null ){
					Debug.LogFormat( "Buff CheckTag CharID{0}-Buff{1} with null TargetIdent{2} " , Owner.n_CharID , pair.Value.nID , pair.Value.nTargetIdent  );
				}
			}
			else {
				unit = unit_e;
			}
			
			//if( MyScript.Instance.CheckSkillCond( pair.Value.tableData.s_BUFF_CONDITON , this.Owner , unit_e ) == true )
			if( pair.Value.Condition.Check( pair.Value , this.Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
			{
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null && eft._IsTag( tag ) )
					{
						return true	;
					}
				}
			}
		}
		
		return false;
	}

	//  ==
	public bool CheckStatus( _FIGHTSTATE status ){
		cUnitData unit_e = null ;
		if( Owner.FightAttr.TarIdent > 0 ){
			unit_e = GameDataManager.Instance.GetUnitDateByIdent ( Owner.FightAttr.TarIdent );
		}
		foreach( KeyValuePair< int , cBuffData > pair in Pool )
		{
			// normal effect
			foreach( cEffect eft in pair.Value.EffectPool )
			{
				if( eft != null && eft._IsStatus( status ) )
				{
					return true	;
				}
			}
					// condition
			cUnitData unit = null ;
			if( pair.Value.nTargetIdent > 0 ){
				unit = pair.Value.GetTargetUnit();
				if( unit == null ){
					Debug.LogFormat( "Buff CheckStatus CharID{0}-Buff{1} with null TargetIdent{2} " , Owner.n_CharID , pair.Value.nID , pair.Value.nTargetIdent  );
				}
			}
			else {
				unit = unit_e;
			}

			//if( MyScript.Instance.CheckSkillCond( pair.Value.tableData.s_BUFF_CONDITON , this.Owner , unit_e ) == true )
			if( pair.Value.Condition.Check( pair.Value , this.Owner , unit , pair.Value.nSkillID , pair.Value.nID ) )
			{
				foreach( cEffect eft in pair.Value.ConditionEffectPool )
				{
					if( eft != null && eft._IsStatus( status ) )
					{
						return true	;
					}
				}
			}
		}

		return false;
	}

	public List< cBuffSaveData > ExportSavePool(){
		List< cBuffSaveData > pool = new List< cBuffSaveData > ();
		foreach( KeyValuePair< int , cBuffData > pair in Pool  )
		{
			//// ignore  
			//if( pair.Value.nTime == 0 )
			//	continue;
			//
			pool.Add( new cBuffSaveData( pair.Value )  );

		}
		return pool;
	}

	public void ImportSavePool( List< cBuffSaveData > pool )
	{
		Pool.Clear();
		foreach( cBuffSaveData data in pool  )
		{
			cBuffData buff = AddBuff( data.nID , data.nCastIdent , data.nSkillID  , data.nTargetIdent );
			if( buff != null ){
				buff.nNum = data.nNum;
				buff.nTime = data.nTime;
			}
		}
	}
}
