using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;
//using _SRW;

// 戰鬥零時摻數
public enum _FIGHTSTATE
{
	_NULL =0,
	_ATKER    ,
	_DAMAGE   ,		//執行過傷害技能
	_BEDAMAGE	,   //發生過{被} 攻擊行為
//	_DEFER    ,		// not atker is defer
	_DEFMODE  ,
	_DEAD	 ,		// 已死亡

	//status
	_DODGE	 ,		// 迴避
	_CIRIT	 ,		// 報擊
	_MERCY	 ,		// 留情
	_GUARD   ,		// 防衛	
	_THROUGH  ,		// 穿透
	_MISS		,	// 失誤
	_COMBO	,		// 攻擊後強制原地再次攻擊
	_BROKEN,		// 破防
	// 
	_KILL		,  //本次戰鬥有殺人



}
//
public enum _UNITTAG
{
	_NULL = 0,
	_UNDEAD = 1 ,   // 除非隊長死否則無限重生
	_CHARGE = 2 ,    // 突襲移動. no block
	_NODIE	= 3 ,		// 不死身... 劇情NPC
	_SILENCE = 4 ,    // can't  skill
}
//
public enum _ITEMSLOT
{
	_SLOT0 = 0,
	_SLOT1 ,
	_SLOTMAX ,
}
// AI 
public enum _AI_SEARCH{
	_NOCHANGE = -1,	// 

	_NORMAL=0,  	// 主動攻擊
	_PASSIVE=1,		// 被動攻擊
	_DEFENCE=2,		// 堅守原地 
	_TARGET=3,		// 前往指定 目標或地點
	_POSITION=4		// 不在目標地點前往目標
};

public enum _AI_COMBO{
	_NOCHANGE = -1,	// 

	_NORMAL=0,  	// Normal attack
	_DEFENCE=1,		// 
	
	
};

// base attr
public class cAttrData
{
	// attr index
	public static int _INTSCH = 0;
	public static int _EXTSCH = _INTSCH+1;
	public static int _CHARLV = _EXTSCH+1;
	public static int _ITEM   = _CHARLV+1;
	public static int _BUFF   = _ITEM+1;
	public static int _CONDBUFF   = _BUFF+1;
	public static int _FIGHT   = _CONDBUFF+1;
	public static int _MAX 	  = _FIGHT+1; // not use

	//========================

	public int n_ReCharID;		// 進階版的頭像與對話

	public int n_HP;
	public int n_MP;
	public int n_SP;
	public int n_CP;
	public int n_DEF;


	public float f_MAR;
	public int n_ATK;
	public int n_POW;


	public float fHpRate;
	public float fMpRate;
	public float fSpRate;
	public float fAtkRate;
	public float fDefRate;
	public float fPowRate;


	public float fBurstRate;	//	曾商
	public float fDamageRate; 	// 承受傷害

	public float fDrainHpRate;  // 
	public float fDrainMpRate;  //

	public float fDropRate;
	public float fMpCostRate;
	public int n_MOV;



	public void ClearBase(){
		f_MAR = 0.0f;
		n_ReCharID = 0;
		n_HP = 0;
		n_MP = 0;
		n_SP = 0;
		n_CP = 0;
		n_DEF = 0;
		n_ATK = 0;
		n_POW = 0;
		n_MOV = 0;

		fBurstRate = 0.0f;
		fDamageRate = 0.0f;

		fDrainHpRate =0.0f;  // 
		fDrainMpRate =0.0f;  //

		fDropRate = 0.0f;
		fMpCostRate = 0.0f;

		fHpRate	=0.0f;
		fMpRate	=0.0f;
		fSpRate	=0.0f;
		fAtkRate =0.0f;
		fDefRate =0.0f;
		fPowRate =0.0f;
	}

	virtual public void Reset()
	{
		ClearBase ();
		
	}
}
//fight attr
public class cFightAttr : cAttrData
{
//	public int Ident;
	public int TarIdent;

	public int TarGridX;
	public int TarGridY;
	public int SkillID;

	// this will move to skill data in the future
	//public SKILL Skill;
	public cSkillData SkillData;

	//extra attr
	public float fAtkAssist;
	public float fDefAssist;

	// effect pool
//	public List< cEffect > CastPool;
//	public string		   CastCond;
//	public List< cEffect > CastEffPool;

//	public List< cEffect > HitPool;
//	public string		   HitCond;
//	public List< cEffect > HitEffPool;



	//
	override public void Reset()
	{
		ClearBase ();

		TarIdent = 0;
		TarGridX = 0;
		TarGridY = 0;
		SkillID = 0;
		SkillData = null;

		fAtkAssist = 0;
		fDefAssist = 0;
//		Skill = null;
//		CastPool 	= null;
//		CastEffPool = null;
//		HitPool 	= null;
//		HitEffPool 	= null;
	}
};


// current stage runtime data
// All data store in this . buffcondition need this to check
public class cUnitData{
	public int nVersion=1;			// for save/load data


	public CHARS cCharData;
	//public int n_Rank;		// max school lv
	public bool bEnable;			// is join party
// save data start
	public int n_Ident;		// auto create by game system
	public _CAMP eCampID;
	public int n_CharID;
	public int n_UpgradeFromCharID;			// 進階前的腳色ID

	public int n_Lv;
	public int n_EXP;
	public int n_X;
	public int n_Y;

	public int n_HP;
	public int n_MP;
	public int n_SP;
	public int n_CP;
	public int n_DEF;

	public int [] nActSch;		// current use 

	public int n_LeaderIdent;	// follow leader
	public int n_BornX;			// born Pox
	public int n_BornY;
	public cBuffs						Buffs;				// all buffs of unit
	public int []						Items;			// all buffs of unit
	
	public int nActionTime;							//	行動次數

	public _AI_SEARCH  eSearchAI;								// 找怪物
	public _AI_COMBO	 eComboAI;								// 選技能

	public int n_AITarget;						// AI目標
	public int n_AIX;							// AI目標 X
	public int n_AIY;							// AI目標 Y
// save end

	// calcul attr
	Dictionary< int , cAttrData > Attr; 		// 0-內功 , 1-外功  , 2-等級 , 3- buff 

	// school
	public Dictionary< int , int >		SchoolPool;			// all study school < school id , lv >
	public Dictionary< int , int >		AbilityPool;		// all ability school < ability id , lv can use >

	// current skill data pool
	public Dictionary< int , cSkillData >SkillPool;						// all skill from school < skill id  >

	// Buff list

//	public Dictionary< int , int >		CDPool;				// all study school < cd type , round >
	public cFightAttr					FightAttr;			// need update each calcul
	//public cAttrData					BuffCondAttr;		// buff cond trig attr

	// 企劃資料解析出來的特別旗標，支援buff 動態 新增

	List< _UNITTAG > Tags;		
	
	List< _UNITTAG > GetTags()
	{
		if (Tags == null) {
			Tags = new List< _UNITTAG >();
		}
		return Tags;
	}
	
	public void AddTag( _UNITTAG tag ){
		if ( GetTags ().IndexOf (tag) < 0 ) {
			GetTags ().Add( tag );
		}		
	}	
	public bool IsTag( _UNITTAG tag ){
		if( GetTags ().IndexOf(tag) >= 0 )
			return true;
		return Buffs.CheckTag( tag );
	}
	
	public void RemoveTag( _UNITTAG tag ){
		GetTags ().Remove (tag);
	}
	
	public void ClearTag(  ){
		GetTags().Clear ();
	}



	// no need save
	bool [] bUpdateFlag;

	public void ActionFinished(  ){
		nActionTime--;		
	}
	
	public void AddActionTime( int nTime ){
		nActionTime +=nTime ;		
	}

	public cUnitData()
	{
		bEnable = false;
		SchoolPool  = new Dictionary< int , int > ();
		AbilityPool = new Dictionary< int , int > ();
		SkillPool 	= new Dictionary< int , cSkillData > ();

		Buffs = new cBuffs( this ); 
	//	CDPool = new Dictionary< int , int > ();
		
		Attr = new Dictionary< int , cAttrData > (); 
		nActSch = new int []{0,0};
		bUpdateFlag = new bool[ cAttrData._MAX ] ;
		for (int i=0; i <bUpdateFlag.Length; i++) {
			bUpdateFlag[i] = true ;
		}

		Items = new int [ (int)_ITEMSLOT._SLOTMAX ];			// all buffs of unit

		// special insert
		FightAttr =   new cFightAttr ();
		Attr.Add ( cAttrData._FIGHT , FightAttr ); // special insert

		n_Lv = 1; // base lv
		nActionTime = 1;

		//===========
		eSearchAI = _AI_SEARCH._NORMAL;								// 找怪物
		eComboAI  = _AI_COMBO._NORMAL;								// 選技能
	}

	// setup update flag
	public void SetUpdate( int index  )
	{
		bUpdateFlag [index] = true;
	}
	//提升武功，不一定會馬上使用
	public void LearnSchool( int id , int nLv )						// 
	{
		if (nLv <= 0)
			nLv = 1;

		int lv = 0;
		if (SchoolPool.TryGetValue (id, out lv)) {
			if( nLv != lv )
			{
				SchoolPool[ id ] = nLv;
			}

		}
		else {
			SchoolPool.Add(id , nLv );
		}

		// check need upgrade skill
		SCHOOL school = ConstDataManager.Instance.GetRow<SCHOOL>( id );
		if( school == null )
			return;
		int nIdx = cAttrData._INTSCH;
		if ( school.n_TYPE==1 ){
			nIdx = cAttrData._EXTSCH;
		}
		int nOldSchId = nActSch[ nIdx ];
		if (nOldSchId == id) {
			RemoveSkillBySchool( nOldSchId );
			AddSkillBySchool( id );
			SetUpdate ( nIdx );					// update attr
		}

		// update both for save
//		SetUpdate ( cAttrData._INTSCH );
//		SetUpdate ( cAttrData._EXTSCH );

		// clean then re add skill 
		//RemoveSkillBySchool ( id );

		//
	

	}
	//
	public void ActiveSchool( int SchID )
	{
		SCHOOL school = ConstDataManager.Instance.GetRow<SCHOOL>( SchID );
		if( school == null )
			return;


		int nIdx = cAttrData._INTSCH;
		if ( school.n_TYPE==1 ){
			nIdx = cAttrData._EXTSCH;
		}

		//remove skill of old sch
		int nOldSchId = nActSch[ nIdx ];

		// change
		RemoveSkillBySchool( nOldSchId );

		// set new school
		//SetSchool(SchID , lv ); // for ability / Skill

		AddSkillBySchool ( SchID );

		//	
		nActSch [nIdx] = SchID;		// active

		// update
		SetUpdate( nIdx );

	}

	
	public void AddSkill( int nSkillID )
	{
		SKILL  skl = ConstDataManager.Instance.GetRow< SKILL>( nSkillID );
		if( skl != null )
		{
			if( skl.n_BUFF > 0 ){
				Buffs.AddBuff( skl.n_BUFF , n_Ident , nSkillID , 0 );
				SetUpdate( cAttrData._BUFF );
			}

			if( SkillPool.ContainsKey( nSkillID ) == false )
			{
				SkillPool.Add( nSkillID , GameDataManager.Instance.GetSkillData(nSkillID)  );
			}
		}
	}

	public void RemoveSkill( int nSkillID )
	{
		if( SkillPool.ContainsKey( nSkillID ) )
		{
			Buffs.DelBuffBySkillID( nSkillID );

			SkillPool.Remove( nSkillID );
		}
	}

	public void AddSkillBySchool ( int  SchID )
	{
		if (0 == SchID)
			return;

		SCHOOL school = ConstDataManager.Instance.GetRow<SCHOOL>( SchID );
		if( school == null )
			return;

		int nLv = 0;	// get school lv
		if (SchoolPool.TryGetValue (SchID, out nLv) == false ) {
			Debug.LogErrorFormat( "Unit can't change school type{0} to sch{1} to , charid={2},identid={3}  " , school.n_TYPE ,SchID, n_CharID, n_Ident );
			return ;
		}

		DataTable tbl = ConstDataManager.Instance.GetTable< SKILL > ();
		if (tbl != null) {
			foreach( SKILL skl in tbl )
			{
				if( skl.n_LEVEL_LEARN  < 0 )// 小於0的是進階技能。不能直接學
				{
					continue;
				}
				
				if( skl.n_LEVEL_LEARN > nLv && (!Config.GOD) )
				{
					continue;
				}
				if( skl.n_SCHOOL != SchID )
					continue;
				
				int nSklID = skl.n_ID;
				//				//判斷是否進階
				//				if( Buffs.HaveBuff( skl.n_UPGRADE_BUFF ) )
				//				{
				//					nSklID = skl.n_UPGRADE_SKILL;
				//				}
				//
				if( SkillPool.ContainsKey( nSklID ) == false )
				{
					AddSkill( nSklID );
				}
			}
		}
		
		// add school buff

		if (school != null) {
			if( school.n_BUFF != 0 ){
				Buffs.AddBuff(school.n_BUFF  , 0 , 0 , 0 );
			}
		}

	}
	public void RemoveSkillBySchool ( int  schid )
	{
		if( 0 == schid )
			return;

		List< int > tmp = new List< int > ();
		foreach (KeyValuePair< int , cSkillData >  pair in SkillPool) {
			SKILL skl = pair.Value.skill; //ConstDataManager.Instance.GetRow< SKILL >( pair.Key );
			if( skl != null && skl.n_SCHOOL == schid )
			{
				// remove skill'd buff
				tmp.Add( pair.Key );
			}
		}
		//
		foreach (int id  in tmp) {
			RemoveSkill( id );
			//SkillPool.Remove( id );
		}

		// remove school buff
		SCHOOL school = ConstDataManager.Instance.GetRow<SCHOOL> ( schid );
		if (school != null) {
			if( school.n_BUFF != 0 ){
				Buffs.DelBuff(school.n_BUFF  , true );
			}
		}

	}

	//紀錄一個角色的等級對應能力
	public void SetAbility( int id , int nLv ) // this is record all ability. but not all active soon
	{
		if (nLv <= 0)
			nLv = 1;
		
		int lv = 0;
		if (AbilityPool.TryGetValue (id, out lv)) {
			if( nLv < lv ) // replace if more less lv
			{
				AbilityPool[ id ] = nLv;
			}			
		}
		else {
			AbilityPool.Add(id , nLv );
		}
		// update both for save
//		AddSkill( id );

		SetUpdate ( cAttrData._CHARLV ); // update with char lv
	}

	public void RemoveAbility( int id  )
	{
		if (AbilityPool.ContainsKey (id)) {
//			SKILL skl = ConstDataManager.Instance.GetRow< SKILL> ( id );
//			if( skl != null ){
//				if( skl.n_BUFF > 0 ){
//					Buffs.DelBuff( skl.n_BUFF );
//					SetUpdate( cAttrData._BUFF );
//				}
//			}
			AbilityPool.Remove( id );

//			RemoveSkill( id );
		}
	}

	public void SetContData( CHARS cData )
	{
		//n_CharID = cData.n_ID;	
		cCharData = cData;
		if (n_CharID != cData.n_ID) {
			Debug.LogErrorFormat ("cUnitData{0} set wrong SetContData{1}", n_CharID, cData.n_ID);
		}
		//n_Rank = cData.n_RANK;
		// set school;
		SkillPool.Clear ();
		//被動能力先，因為會影響出生BUFF 與技能進階
		//Set born Buff
		if (cData.s_BUFF != "0" && cData.s_BUFF.ToUpper() != "NULL") {
			string [] strBorn = cData.s_BUFF.Split(";".ToCharArray());
			for( int i= 0 ; i <strBorn.Length; i++  )
			{
				int nBuffID = 0;
				if( int.TryParse( strBorn[i] , out nBuffID ) ){
					Buffs.AddBuff( nBuffID , n_Ident , 0 , 0 );
				}
			}
		}


		///==
		cTextArray TA = new cTextArray ();
		TA.SetText (cData.s_SCHOOL);
		for (int i = 0; i < TA.GetMaxCol(); i++) {
			CTextLine line = TA.GetTextLine (i);
			for (int j = 0; j < line.GetRowNum(); j++) {
				string s = line.m_kTextPool [j];

				string [] arg = s.Split (",".ToCharArray ());
				if (arg [0] != null) {
					int school = int.Parse (arg [0]);
					int lv = 1;
					if (arg [1] != null) {
						lv = int.Parse (arg [1]);
					}
					LearnSchool (school, lv);
				}
			}
		}
		// set Ability
		TA.SetText (cData.s_ABILITY);
		for (int i = 0; i < TA.GetMaxCol(); i++) {
			CTextLine line = TA.GetTextLine (i);
			for (int j = 0; j < line.GetRowNum(); j++) {
				string s = line.m_kTextPool [j];
				
				string [] arg = s.Split (",".ToCharArray ());
				if (arg [0] != null) {
					int ability = int.Parse (arg [0]);
					int lv = 1;
					if (arg [1] != null) {
						lv = int.Parse (arg [1]);
					}
					SetAbility (ability, lv);
					//SetSchool( school , lv  );
				}
			}
		}


		// set up item
		if (cData.n_ITEM1 > 0) {
			EquipItem( _ITEMSLOT._SLOT0 , cData.n_ITEM1 ) ;
		} 
		if (cData.n_ITEM2 > 0) {
			EquipItem( _ITEMSLOT._SLOT1 , cData.n_ITEM2 ) ;
		} 

		// active school
		ActiveSchool( cData.n_INT_SCHOOL );
		ActiveSchool( cData.n_EXT_SCHOOL );

		//AvtiveSchool (0, cData.n_INT_SCHOOL);
		//AvtiveSchool (1, cData.n_EXT_SCHOOL);
		// 取回Const 特定旗標
		
		// Set TAG 
		string[] tags = cCharData.s_EXT_TAG.Split ( ";".ToCharArray() );
		foreach (string s in tags) {
			int tag =0;
			if( int.TryParse( s , out tag ) ){ 
				AddTag( ( _UNITTAG)tag );
			}
			
		}
		// set up AI
		eSearchAI = (_AI_SEARCH)cCharData.n_MOBAI; // 

		// fill base data;
		Relive();
	}

	public int GetSchoolLv( int School )
	{
		int nLv = 0;
		if (SchoolPool.TryGetValue(School, out nLv ) == false) {
			Debug.LogErrorFormat( "Unit can't get school{1} lv , charid={1},identid={2} " ,School , n_CharID, n_Ident );
			return 0;
		}
		return nLv;
	}
	public int GetIntSchID(  )
	{
		return  nActSch[ 0 ];
	}
	public int GetExtSchID(  )
	{
		return  nActSch[ 1 ];
	}
	public int GetIntSchLv(  )
	{
		return GetSchoolLv( nActSch[ 0 ] );
	}
	public int GetExtSchLv(  )
	{
		return GetSchoolLv( nActSch[ 1 ] );
	}

	public int GetIntSchRank(  )
	{
		SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL> ( nActSch[ 0 ] );
		if( sch != null ){
			return sch.n_RANK;
		}
		return 0;
	}
	public int GetExtSchRank(  )
	{
		SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL> ( nActSch[ 1 ] );
		if( sch != null ){
			return sch.n_RANK;
		}
		return GetSchoolLv( nActSch[ 1 ] );
	}

//	 void AvtiveSchool( int index , int School )
//	{
//		if (SchoolPool.ContainsKey (School) == false) {
//			Debug.LogErrorFormat( "Unit can't active index{0} to sch{1} to , charid={2},identid={3}  " ,index,School, n_CharID, n_Ident );
//			return;
//		}
//		nActSch [index] = School;
//
//		SetUpdate (index);
//		//UpdateSchoolAttr (index , School ); 
//	}

	public void SetLevel( int lv )
	{
		if( lv > Config.MaxCharLevel ){			
			lv = Config.MaxCharLevel;
		}
		if (n_Lv == lv)
			return;
		int nOldLv = n_Lv;
		n_Lv = lv;
		//SetUpdate (2);
		// 檢查習得能力
		for( int i = nOldLv+1 ; i <=n_Lv ; i++ )
		{
			 // show learn ability

		}
		// update direct
		UpdateLevelAttr ( );
		// add sp 
		int nSp = (lv - nOldLv) * Config.CharSpLVUp;
		AddSp (nSp);

	}

	public bool CheckItemCanEquip( int nItemID )
	{
		ITEM_MISC newitem = ConstDataManager.Instance.GetRow< ITEM_MISC > ( nItemID );
		if (newitem == null) {
			return false;
		}
		// check gender

		//check lv


		return true;
	}

	public int EquipItem( _ITEMSLOT slot, int nItemID )
	{
		if (slot >= _ITEMSLOT._SLOTMAX)
			return 0; 
		int nOldItemID = Items [(int)slot];
		ITEM_MISC olditem = ConstDataManager.Instance.GetRow< ITEM_MISC > ( nOldItemID );
		if ( olditem != null) {
			Buffs.DelBuff( olditem.n_ID_BUFF );
		}


		// replace 
		Items [(int)slot] = nItemID;
		ITEM_MISC newitem = ConstDataManager.Instance.GetRow< ITEM_MISC > ( nItemID );
		if ( newitem != null) {
			Buffs.AddBuff( newitem.n_ID_BUFF , this.n_Ident , 0 , 0 );
		}

		SetUpdate( cAttrData._BUFF );

		return nOldItemID;
	}

	public void AddExp( int nExp )
	{
		n_EXP  += nExp;
		// check level up
		if( n_EXP > 100 )
		{

			int nUp = n_EXP / 100;
			n_EXP   = n_EXP -(nUp*100);

			ActionManager.Instance.CreateLevleUpAction( n_Ident , nUp );

			SetLevel( nUp + n_Lv );
		}
	}
	public void UpdateAllAttr( )
	{
		for (int i=0; i <bUpdateFlag.Length; i++) {
			bUpdateFlag[i] = true ;
		}
		//UpdateAttr ();
	}

	public void UpdateAttr( )
	{
		// don't update when dead. change method to flag better
//		if (n_HP <= 0)
//			return;

		// update all attr
		if (bUpdateFlag [  cAttrData._INTSCH  ] == true) {
			bUpdateFlag [ cAttrData._INTSCH ] = false;		//馬上設定可以避免 後面的 update 引起遞迴
			UpdateSchoolAttr ( cAttrData._INTSCH , nActSch [0]);

		}

		if (bUpdateFlag [ cAttrData._EXTSCH ] == true) {
			bUpdateFlag [ cAttrData._EXTSCH ] = false;
			UpdateSchoolAttr ( cAttrData._EXTSCH, nActSch [1]);

		}

		if (bUpdateFlag [ cAttrData._CHARLV ] == true) {
			bUpdateFlag [ cAttrData._CHARLV ] = false;
			UpdateLevelAttr ();
		}

		// no more need this
//		if (bUpdateFlag [ cAttrData._ITEM ] == true) {
//			bUpdateFlag [ cAttrData._ITEM ] = false;
//			UpdateItemAttr();
//		}

		if (bUpdateFlag [ cAttrData._BUFF ] == true) {
			bUpdateFlag [ cAttrData._BUFF ] = false;
			UpdateBuffAttr ();		// maybe Recursive here
		}

		// don't update each frame for performance . call condition update for each calcul
//		if( bUpCond )
//			UpdateBuffConditionAttr ();		// always check condition and update

	}


	void UpdateLevelAttr(  )
	{
	
		cAttrData attr =GetAttrData( cAttrData._CHARLV ) ;
		attr.Reset();

		int nLV = this.n_Lv;
		if ( nLV > Config.MaxCharLevel ) {
			nLV = Config.MaxCharLevel;
		}
		attr.n_SP = Config.CharBaseSp + nLV * Config.CharSpLVUp;
		attr.f_MAR = Config.CharMarLVUp * nLV;

		// For Ability attr
		foreach( KeyValuePair< int , int > pair in  AbilityPool)
		{
			if(  pair.Key == 0 )
				continue;
			// refresh  skill
//			RemoveSkill( pair.Key );

			if( pair.Value > nLV )
				continue;


//			AddSkill( pair.Key );
		}

		// update passiv skill attr
		foreach( KeyValuePair< int , cSkillData >  pair in SkillPool )
		{
		//	if( pair.Value. )

		}

	}

	void UpdateSchoolAttr( int nIdx , int nSchool )
	{	
		int nLv = 0;
		if (SchoolPool.TryGetValue (nSchool, out nLv) == false ) {
			Debug.LogErrorFormat( "UpdateSchoolAttr err! Unit{0} don't have School{1} , " , n_CharID ,nSchool );
			return ;
		}	
		 
		cAttrData attr = GetAttrData (nIdx);
		attr.Reset();
		//===========================================================================
		SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>( nSchool ); //GameDataManager.Instance.GetConstSchoolData ( nSchool );
		if (sch == null) {
			Debug.LogErrorFormat( "UpdateSchoolAttr err! Unit{0} can't get School{1} , " , n_CharID ,nSchool );
			return;
		}
		if (nLv > sch.n_MAXLV)
			nLv = sch.n_MAXLV;
		int rank = sch.n_RANK;

		attr.f_MAR 	 = rank * ( sch.f_MAR+ (sch.f_MAR_LVUP * nLv) );

		float fGrow = Mathf.Pow( 1.2f , nLv );

		attr.n_HP    = rank * ( sch.n_HP + (int)(sch.n_HP_LVUP * fGrow ) );
		attr.n_MP 	 = rank * ( sch.n_MP + (int)(sch.n_MP_LVUP * fGrow ) );
		attr.n_ATK 	 = rank * ( sch.n_ATK+ (int)(sch.n_ATK_LVUP * fGrow ) );
		attr.n_DEF 	 = rank * ( sch.n_DEF+ (int)(sch.n_DEF_LVUP * fGrow ) );
		attr.n_POW 	 = rank * ( sch.n_POW+ (int)(sch.n_POW_LVUP * fGrow ) );


//		attr.n_HP 	 = rank * ( sch.n_HP + (sch.n_HP_LVUP * nLv) );
//		attr.n_MP 	 = rank * ( sch.n_MP + (sch.n_MP_LVUP * nLv) );
//		attr.n_ATK 	 = rank * ( sch.n_ATK+ (sch.n_ATK_LVUP * nLv) );
//		attr.n_DEF 	 = rank * ( sch.n_DEF+ (sch.n_DEF_LVUP * nLv) );
//		attr.n_POW 	 = rank * ( sch.n_POW+ (sch.n_POW_LVUP * nLv) );

		Mathf.Pow( 1.2f , 1 );//

		attr.n_SP = 0;
		attr.n_MOV = sch.n_MOV;
	}

	void UpdateItemAttr( )
	{
		cAttrData attr =GetAttrData( cAttrData._ITEM ) ;
	}
	void UpdateBuffAttr( )
	{
		cAttrData attr =GetAttrData( cAttrData._BUFF ) ;
	//	attr.ClearBase (); // update inside

		// update add value
		Buffs.UpdateAttr ( ref attr );


		// fix error range

	}

	public void UpdateBuffConditionAttr( )
	{
		cAttrData attr =GetAttrData( cAttrData._CONDBUFF ) ;
		//attr.Reset ();
		Buffs.UpdateCondAttr (ref attr ); // reset inside


	}

//	// this should work only each casting phase
//	void UpdateFightAttr( )
//	{
//		if (FightAttr == null )
//			return;
//
//		FightAttr.ClearBase (); // clear base attr only
//
//		// cast attr will add in  cast pahse
//
////		if (FightAttr.SkillData != null) {
////			MyTool.AttrSkillEffect( this , FightAttr.SkillData.CastPool , FightAttr.SkillData.CastCond , FightAttr.SkillData.CastCondEffectPool );
////		}
//	}




	cAttrData GetAttrData( int idx )
	{
		cAttrData attr;
		if (Attr.TryGetValue( idx , out attr ) == false ) {
			attr = new cAttrData();
			Attr.Add( idx , attr );
		}
		if (attr == null) {
			Debug.LogErrorFormat( "GetAttrData err! Unit{0} can't get attr{1} , " , n_CharID ,idx );
		}
		return attr;
	}

	public void Relive()
	{
		Buffs.BuffRelive ();

		for (int i=0; i <bUpdateFlag.Length; i++) {
			bUpdateFlag[i] = true ;
		}
		UpdateAttr(); // update soon

		n_HP = GetMaxHP();
		n_MP = GetMaxMP();
		n_SP = GetMaxSP();
		n_DEF = GetMaxDef();

		nActionTime = 1;

		//
		UpdateAllAttr ();
		UpdateAttr ();

	}

	public void AddHp( int nhp )
	{
		// this cheat always hard to balance battle value. mark it
//		if ( (Config.GOD==true) && nhp < 0 ) {
//			Panel_unit p = Panel_StageUI.Instance.GetUnitByIdent( this.n_Ident );
//			if( p != null && p.eCampID == _CAMP._ENEMY )
//			{
//				nhp *= 10;
//			}
//		}

		if( nhp > 0 )
		{
			 // heal 
			n_HP += nhp;
			int maxhp = GetMaxHP();
			if( n_HP > maxhp )
			{
				n_HP = maxhp;
			}

		}
		else 
		{
			// damg
			n_DEF += nhp;
			if(	n_DEF < 0)
			{
				n_HP +=n_DEF;
				n_DEF = 0;
			}

			if( n_HP <= 0 )
			{
				n_HP =0;
				// set to dead.
			}
		}

//		Panel_unit p = Panel_StageUI.Instance.GetUnitByIdent(  n_Ident );
//		if( p != null )
//		{
//			p.OnDamage( nhp );
//		}

	}

	public void AddMp( int mp , bool bShow= false )
	{

		n_SP =  MyTool.ClampInt(n_MP+mp , 0 , GetMaxMP () ); 
	}

	public void AddSp( int sp , bool bShow= false )
	{

		n_SP =  MyTool.ClampInt(n_SP+sp , 0 , GetMaxSP () ); 
	}

	public void AddCp( int cp , bool bShow= false )
	{
		
		n_CP =  MyTool.ClampInt(n_CP+cp , 0 , GetMaxCP() ); 
	}

	public void AddDef( int def , bool bShow= false  )
	{
		n_DEF =  MyTool.ClampInt(n_DEF+def , 0 , GetMaxDef () );  //   NGUIMath.ClampIndex (n_DEF, GetMaxDef ());	
	}

	public int GetMaxCP ()
	{
		return  Config.MaxCP;
	}

	// Get Data func
	public int GetMaxHP()
	{
		UpdateAttr(  ); // update first to get newest data

		int nHp = cCharData.n_HP ;		// base HP
		float fHpRate = 1.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			nHp +=pair.Value.n_HP;

			fHpRate += pair.Value.fHpRate;
		}

		if (fHpRate < 0.0f)
			fHpRate = 0.0f;
		nHp = (int)(fHpRate*nHp);

		if (nHp < 1)	
			nHp = 1;

		return nHp;
	}
	public int GetMaxMP()
	{
		UpdateAttr(  ); // update first to get newest data
		int nMp = 0;
		float fMpRate = 1.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			nMp +=pair.Value.n_MP;
			fMpRate +=pair.Value.fMpRate;
		}
		if (fMpRate < 0.0f)
			fMpRate = 0.0f;

		nMp = (int)(fMpRate*nMp);
		if (nMp < 1)			nMp = 1;


		return nMp;
	}
	public int GetMaxSP()
	{
		UpdateAttr(  ); // update first to get newest data
		int nSp = 0;
		float fSpRate = 1.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			nSp +=pair.Value.n_SP;
			fSpRate += pair.Value.fSpRate;
		}
		if (fSpRate < 0.0f)
			fSpRate = 0.0f;

		nSp = (int)(fSpRate*nSp);
		if (nSp < 1)			nSp = 1;
		return nSp;
	}
	public float GetHpPercent()
	{
		UpdateAttr(  );
		return (float)n_HP / GetMaxHP();
	}


	// only get school + char lv
	public float GetBaseMar()
	{
		UpdateAttr(  );
		float f = 0;
		for (int i = cAttrData._INTSCH; i <=  cAttrData._CHARLV; i++) {
			cAttrData att = GetAttrData( i );
			f += att.f_MAR;
		}
		if (f < 0.0f )			f = 0.0f;
		return f;
	}

	public int GetBaseAttack()
	{
		UpdateAttr(  );
		int  n = 0;
		for (int i = cAttrData._INTSCH; i <=  cAttrData._CHARLV; i++) {
			cAttrData att = GetAttrData( i );
			n += att.n_ATK;
		}
		if ( n < 0 )			n = 0;
		return n;
	}

	public float GetMar()
	{
		UpdateAttr(  ); // update first to get newest data
		float f = 0;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			f +=pair.Value.f_MAR;
		}
		if (f < 0.0f )			f = 0.0f;
		return f;
	}

	public int GetAtk()
	{
		UpdateAttr(  ); // update first to get newest data
		int n = 0;
		float fAtkRate = 1.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			n +=pair.Value.n_ATK;
			fAtkRate += pair.Value.fAtkRate;

		}
		if (fAtkRate < 0.0f)
			fAtkRate = 0.0f;
		n = (int)(fAtkRate* n);

		if (n < 0)			n = 0;
		return n;
	}

	public int GetMaxDef()
	{
		UpdateAttr(  ); // update first to get newest data
		int n = 0;
		float fDefRate = 1.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			n +=pair.Value.n_DEF;
			fDefRate += pair.Value.fDefRate;
		}
		if (fDefRate < 0.0f)
			fDefRate = 0.0f;
		n = (int)(fDefRate* n);
		if (n < 0)
			n = 0;
		return n;
	}
	public int GetPow()
	{
		UpdateAttr(  ); // update first to get newest data
		int n = 0;
		float fPowRate = 1.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			n +=pair.Value.n_POW;
			fPowRate += pair.Value.fPowRate;
		}
		if (fPowRate < 0.0f)
			fPowRate = 0.0f;
		n = (int)(fPowRate * n);
		if (n < 0)
			n = 0;
		return n;
	}

	public int GetMov()
	{
		UpdateAttr(  ); // update first to get newest data
		int n = 0;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			n +=pair.Value.n_MOV;
		}
		if (n < 0)
			n = 0;
		return n;
	}

	public float GetMulDrop()
	{
		UpdateAttr(  ); // update first to get newest data
		float f = 1.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr ){
			f +=pair.Value.fDropRate;
		}
		if (f < 0.0f )			f = 0.0f;
		return f;
	}

	public float GetMulDamage()
	{
		UpdateAttr(  ); // update first to get newest data
		float f = 1.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr ){
			f +=pair.Value.fDamageRate;
		}
		 if (f < 0.0f )			f = 0.0f; 
		return f;
	}
	public float GetMulBurst()
	{
		UpdateAttr(  ); // update first to get newest data
		float f = 1.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr ){
			f +=pair.Value.fBurstRate;
		}
		if (f < 0.0f )			f = 0.0f;
		return f;
	}

	public float GetMulAttack()
	{
		UpdateAttr(  ); // update first to get newest data
		float f = 1.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr ){
			f +=pair.Value.fAtkRate;
		}
		if (f < 0.0f )			f = 0.0f;
		return f;
	}

	public float GetMulDef()
	{
		UpdateAttr(  ); // update first to get newest data
		float f = 1.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr ){
			f +=pair.Value.fDefRate;
		}
		if (f < 0.0f )			f = 0.0f;
		return f;
	}
	public float GetMulPower()
	{
		UpdateAttr(  ); // update first to get newest data
		float f = 1.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr ){
			f +=pair.Value.fPowRate;
		}
		if (f < 0.0f )			f = 0.0f;
		return f;
	}

	public float GetMulMpCost()
	{
		UpdateAttr(  ); // update first to get newest data
		float f = 1.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr ){
			f +=pair.Value.fMpCostRate;
		}
		if (f < 0.0f )			f = 0.0f;
		return f;
	}

	public float GetDrainHP()
	{
		UpdateAttr(  ); // update first to get newest data
		
		float fDrainHpRate = 0.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			fDrainHpRate +=pair.Value.fDrainHpRate;
		}
		if (fDrainHpRate < 0.0f)
			fDrainHpRate = 0.0f;		
		return fDrainHpRate;
	}


	public float GetDrainMP()
	{
		UpdateAttr(  ); // update first to get newest data

		float fDrainMpRate = 0.0f;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			fDrainMpRate +=pair.Value.fDrainMpRate;
		}
		if (fDrainMpRate < 0.0f)
			fDrainMpRate = 0.0f;

		return fDrainMpRate;
	}

	// Fight Attr
	public void SetFightAttr( int nTarId , int SkillID )
	{
		FightAttr.Reset ();
		FightAttr.TarIdent = nTarId;
		FightAttr.SkillID = SkillID;
		//SKILL skill = ConstDataManager.Instance.GetRow< SKILL> ( SkillID ); 
		FightAttr.ClearBase ();			// important

		FightAttr.SkillData = GameDataManager.Instance.GetSkillData(SkillID) ;   //new cSkillData ( ConstDataManager.Instance.GetRow< SKILL> ( SkillID ) );


		//// GET buff status 
		//GetBuffStatus ();

		// get skill data

		UpdateAttr (  );
		UpdateBuffConditionAttr ();
	}

	//fight end to clear data
	public void FightEnd( bool bIsAtker = false )
	{
		if (bIsAtker) {
			if( FightAttr.SkillID == 0 || (FightAttr.SkillData.skill.n_FINISH !=0) ){
				this.ActionFinished();
			}
		}
		
		//FightAttr.ClearBase (); // clear base attr only
		FightAttr.Reset();			// clear all fight attr

		// Fight end to remove buff
		if (Buffs.BuffFightEnd ( )) {
			SetUpdate( cAttrData._BUFF );
		}

		// state 最後清
		ClearState(); // clear fight state

		UpdateAllAttr ();
		UpdateAttr (  );
		UpdateBuffConditionAttr ();

	}


	// need to updte each frame for each 
	public void UpdateBuffConditionEffect(  )
	{
		//cUnitData tar = GameDataManager.Instance.GetUnitDateByIdent( FightAttr.TarIdent ) ;
		cAttrData attr = this.GetAttrData (cAttrData._CONDBUFF);

		Buffs.UpdateCondAttr (ref  attr );
	}

	public void DoCastEffect( int nSkillID  , cUnitData tarunit , ref List< cHitResult > resPool )
	{
		//cUnitData Defer = GameDataManager.Instance.GetUnitDateByIdent (FightAttr.TarIdent);

		cSkillData skilldata = MyTool.GetSkillData ( nSkillID ) ;
		if (skilldata != null) {
			skilldata.DoCastEffect (this, tarunit, ref resPool);  
		}

		Buffs.OnCast (tarunit, ref resPool);


		//MyTool.DoSkillEffect ( this , Defer ,  FightAttr.SkillData.CastPool , FightAttr.SkillData.CastCond ,  FightAttr.SkillData.CastCondEffectPool , ref resPool  );
	}


	public void DoHitEffect( int nSkillID  , cUnitData tarunit , ref List< cHitResult > resPool )
	{

		cSkillData skilldata = MyTool.GetSkillData ( nSkillID ) ;
		if (skilldata != null) {
			skilldata.DoHitEffect (this, tarunit, ref resPool);  
		}

		Buffs.OnHit (tarunit, ref resPool);
//		MyTool.DoSkillEffect ( this , tarunit , FightAttr.SkillData.HitPool , FightAttr.SkillData.HitCond ,  FightAttr.SkillData.CastCondEffectPool , ref resPool  );

//		if (FightAttr.Skill == null)
//			return;
//		
//		MyTool.DoSkillEffect ( this , FightAttr.HitPool , FightAttr.Skill.s_HIT_TRIG ,  FightAttr.HitEffPool , ref resPool  );

	}

	public void DoBeHitEffect( cUnitData tarunit , ref List< cHitResult > resPool )
	{		
		Buffs.OnBeHit (tarunit, ref resPool);

	}

	public void Waiting( )
	{
		ActionFinished ();
		//AddActionTime (1);
		
		// restore full def
		AddDef ( GetMaxDef()/2 , true );

		// 
		AddCp (1);			
//		uAction act =  ActionManager.Instance.CreateWeakUpAction ( this.n_Ident );
//		if (act != null) {
//			
//			
//			if( Buffs.BuffRoundEnd( ) ){
//				SetUpdate( cAttrData._BUFF );
//				
//			}
//		}
		//Buffs.OnDo ( ref resPool );
	}


	// pass 1 round
	public void WeakUp( )
	{

		//AddActionTime (1);
//		if (nActionTime > 0) {
//			AddCp( 1 );		// 上回合，有殘留行動力的話獲的1 CP
//		}

		nActionTime = 1;   // setup to default 

		// restore def
		AddDef ( GetMaxDef() / 2 , true );

		uAction act =  ActionManager.Instance.CreateWeakUpAction ( this.n_Ident );
		if (act != null) {

			if( Buffs.BuffRoundEnd( ref act ) ){
				SetUpdate( cAttrData._BUFF );
			}
		}
		//Buffs.OnDo ( ref resPool );
	}

	// mark as undead to wait relive
	public bool CheckCanRePop( )
	{
		if (this.IsTag (  _UNITTAG._UNDEAD ) ) {
			if( n_LeaderIdent > 0){
				cUnitData leader = GameDataManager.Instance.GetUnitDateByIdent( n_LeaderIdent );
				if( leader != null ){
					if( leader.n_HP > 0 && !MyTool.CanPK( eCampID , leader.eCampID ) ){
						return true;
					}
				}
			}
		}
		return false;
	}

	public void SetUnDead()
	{
		n_HP = 0;

		nActionTime = 0; // no more action
		n_X = n_BornX;
		n_Y = n_BornY;
	}

//	public void GetBuffStatus( ){
//		if (Buffs.CheckStatus ( _FIGHTSTATE._DODGE )) {
//			AddStates( _FIGHTSTATE._DODGE );
//		}
//		if (Buffs.CheckStatus ( _FIGHTSTATE._CIRIT )) {
//			AddStates( _FIGHTSTATE._CIRIT );
//		}
//
//		if (Buffs.CheckStatus ( _FIGHTSTATE._MERCY )) {
//			AddStates( _FIGHTSTATE._MERCY );
//		}
//		if (Buffs.CheckStatus ( _FIGHTSTATE._GUARD )) {
//			AddStates( _FIGHTSTATE._GUARD );
//		}
//	}

	public bool IsDead(){ return (n_HP <= 0); }

	// state battle flag
    // 戰鬥臨時旗標
	List< _FIGHTSTATE > States;

	List< _FIGHTSTATE > GetStates()
	{
		if (States == null) {
			States = new List< _FIGHTSTATE >();
		}
		return States;
	}

	public void AddStates( _FIGHTSTATE st ){
		if ( GetStates ().IndexOf (st) < 0 ) {
			GetStates ().Add( st );
		}
	
	}

	public bool IsStates( _FIGHTSTATE st ){ 
		if (GetStates ().IndexOf (st) >= 0) {
			return true;
		}	
		return Buffs.CheckStatus (st);
	}
//	public bool GetStateValue( _FIGHTSTATE st , out float f , out int i )
//	{
//		bool bFind = false;
//		f = 0.0f;
//		i = 0;
//
//
//
//		return bFind;
//	}
		 
	public void RemoveStates( _FIGHTSTATE st ){
		GetStates ().Remove (st);
	}

	public void ClearState(  ){
		GetStates().Clear ();
	}
		
	//============================================================
	public bool CheckSkillCanUse( SKILL skill )
	{
		if (skill == null)
			return false;

		if (Config.GOD == true)
			return true;

		if ( skill.n_MP > 0 && (skill.n_MP > this.n_MP) )
			return false;
		if ( skill.n_SP > 0 && (skill.n_SP > this.n_SP) )
			return false;
		if ( skill.n_CP > 0 && (skill.n_CP > this.n_CP) )
			return false;
		
		return true;
	}


}

//
//public class cMobGroup
//{
//	public int nGroupID{ set; get; }
//	public cMobGroup()
//	{
//		memList = new List< int >{};
//
//	}
//
//
//	public List< int > memList ;
//}