using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;
using _SRW;


public enum _UNITSTATE
{
	_NULL =0,
	_ATKER    ,
//	_DEFER    ,		// not atker is defer
	_DEFMODE  ,
	_DEAD	 ,
}


// base attr
public class cAttrData
{
	// attr index
	public static int _INTSCH = 0;
	public static int _EXTSCH = _INTSCH+1;
	public static int _CHARLV = _EXTSCH+1;
	public static int _BUFF   = _CHARLV+1;
	public static int _CONDBUFF   = _BUFF+1;
	public static int _FIGHT   = _CONDBUFF+1;
	public static int _MAX 	  = _FIGHT+1; // not use

	//========================
	public float f_MAR;

	// load data
	public int n_HP;
	public int n_MP;
	public int n_SP;
	public int n_DEF;
	public int n_ATK;
	public int n_POW;


	public float fHpRate;
	public float fMpRate;
	public float fSpRate;
	public float fAtkRate;
	public float fDefRate;
	public float fPowRate;



	public int n_MOV;

	public void ClearBase(){
		f_MAR = 0.0f;
		n_HP = 0;
		n_MP = 0;
		n_SP = 0;
		n_DEF = 0;
		n_ATK = 0;
		n_POW = 0;
		n_MOV = 0;


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
	public int n_Ident;		// auto create by game system
	public _CAMP eCampID;
	public CHARS cCharData;
	public int n_CharID;
	public int n_Rank;		// max school lv

	// save data
	public int n_Lv;
	public int n_EXP;
	public int n_X;
	public int n_Y;

	public int n_HP;
	public int n_MP;
	public int n_SP;
	public int n_DEF;

	public int [] nActSch;		// current use 
	bool [] bUpdateFlag;


	// calcul attr
	Dictionary< int , cAttrData > Attr; 		// 0-內功 , 1-外功  , 2-等級 , 3- buff 

	// school
	public Dictionary< int , int >		SchoolPool;			// all study school < school id , lv >
	public Dictionary< int , int >		AbilityPool;		// all ability school < ability id , lv can use >
	// Buff list

	public Dictionary< int , int >		CDPool;				// all study school < cd type , round >
	public cFightAttr					FightAttr;			// need update each calcul
	//public cAttrData					BuffCondAttr;		// buff cond trig attr

	public cBuffs						Buff;				// all buffs of unit

	public cUnitData()
	{
		SchoolPool  = new Dictionary< int , int > ();
		AbilityPool = new Dictionary< int , int > ();
		Buff = new cBuffs( this ); 
		CDPool = new Dictionary< int , int > ();
		
		Attr = new Dictionary< int , cAttrData > (); 
		nActSch = new int []{0,0};
		bUpdateFlag = new bool[]{ true,true,true,true } ;

		// special insert
		FightAttr =   new cFightAttr ();
		Attr.Add ( cAttrData._FIGHT , FightAttr ); // special insert

		n_Lv = 1; // base lv
	}

	// setup update flag
	public void SetUpdate( int index  )
	{
		bUpdateFlag [index] = true;
	}

	public void SetSchool( int id , int nLv )
	{
		if (nLv <= 0)
			nLv = 1;

		int lv = 0;
		if (SchoolPool.TryGetValue (id, out lv)) {
			if( nLv > lv )
			{
				SchoolPool[ id ] = nLv;
			}

		}
		else {
			SchoolPool.Add(id , nLv );
		}
		// update both for save
		SetUpdate (0);
		SetUpdate (1);
	}



	public void SetContData( CHARS cData )
	{
		//n_CharID = cData.n_ID;	
		cCharData = cData;
		if (n_CharID != cData.n_ID) {
			Debug.LogErrorFormat( "cUnitData{0} set wrong SetContData{1}" ,n_CharID ,cData.n_ID );
		}
		n_Rank = cData.n_RANK;

		cTextArray TA = new cTextArray (  );
		TA.SetText (cData.s_SCHOOL);
		for( int i = 0 ; i < TA.GetMaxCol(); i++ )
		{
			CTextLine line  = TA.GetTextLine( i );
			for( int j = 0 ; j < line.GetRowNum() ; j++ )
			{
				string s = line.m_kTextPool[ j ];

				string [] arg = s.Split( ",".ToCharArray() );
				if( arg[0] != null )
				{
					int school= int.Parse( arg[0] );
					int lv = 1;
					if( arg[1] != null )
					{
						lv = int.Parse( arg[1] );
					}
					SetSchool( school , lv  );
				}
			}
		}
		// set Ability

		//Set Buff

		// active school
		AvtiveSchool (0, cData.n_INT_SCHOOL);
		AvtiveSchool (1, cData.n_EXT_SCHOOL);


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

	public void AvtiveSchool( int index , int School )
	{
		if (SchoolPool.ContainsKey (School) == false) {
			Debug.LogErrorFormat( "Unit can't active index{0} to sch{1} to , charid={2},identid={3}  " ,index,School, n_CharID, n_Ident );
			return;
		}
		nActSch [index] = School;

		SetUpdate (index);
		//UpdateSchoolAttr (index , School ); 
	}

	public void SetLevel( int lv )
	{
		if( lv > Config.MaxCharLevel ){			
			lv = Config.MaxCharLevel;
		}
		if (n_Lv == lv)
			return;

		n_Lv = lv;
		SetUpdate (2);
	}

	public void AddExp( int nExp )
	{

	}

	public void UpdateAttr( )
	{
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
			UpdateLevelAttr (n_Lv);

		}
		if (bUpdateFlag [ cAttrData._BUFF ] == true) {
			bUpdateFlag [ cAttrData._BUFF ] = false;
			UpdateBuffAttr ();		// maybe Recursive here
		}

		UpdateBuffConditionAttr ();		// always check condition and update

	}


	void UpdateLevelAttr( int nLV )
	{
		cAttrData attr =GetAttrData( cAttrData._CHARLV ) ;
		if ( nLV > Config.MaxCharLevel ) {
			nLV = Config.MaxCharLevel;
		}
		attr.n_SP = Config.CharBaseSp + nLV * Config.CharSpLVUp;

		attr.f_MAR = Config.CharMarLVUp * nLV;
	}

	void UpdateSchoolAttr( int nIdx , int nSchool )
	{	
		int nLv = 0;
		if (SchoolPool.TryGetValue (nSchool, out nLv) == false ) {
			Debug.LogErrorFormat( "UpdateSchoolAttr err! Unit{0} don't have School{1} , " , n_CharID ,nSchool );
			return ;
		}	
		 
		cAttrData attr =GetAttrData( nIdx ) ;
		//===========================================================================
		SCHOOL sch = GameDataManager.Instance.GetConstSchoolData ( nSchool );
		if (sch == null) {
			Debug.LogErrorFormat( "UpdateSchoolAttr err! Unit{0} can't get School{1} , " , n_CharID ,nSchool );
			return;
		}
		if (nLv > sch.n_MAXLV)
			nLv = sch.n_MAXLV;
		int rank = sch.n_RANK;

		attr.f_MAR 	 = rank * ( sch.f_MAR+ (sch.f_MAR_LVUP * nLv) );

		attr.n_HP 	 = rank * ( sch.n_HP+ (sch.n_HP_LVUP * nLv) );
		attr.n_MP 	 = rank * ( sch.n_MP+ (sch.n_MP_LVUP * nLv) );
		attr.n_ATK 	 = rank * ( sch.n_ATK+ (sch.n_ATK_LVUP * nLv) );
		attr.n_DEF 	 = rank * ( sch.n_DEF+ (sch.n_DEF_LVUP * nLv) );
		attr.n_POW 	 = rank * ( sch.n_POW+ (sch.n_POW_LVUP * nLv) );

		attr.n_SP = 0;
		attr.n_MOV = sch.n_MOV;
	}

	void UpdateBuffAttr( )
	{
		cAttrData attr =GetAttrData( cAttrData._BUFF ) ;


		// update add value
		Buff.UpdateAttr ( ref attr );


		// fix error range

//		if( fHpRate < 0.0f )
//			fHpRate	=0.0f;
//		if( fMpRate < 0.0f )
//			fMpRate	=0.0f;
//		if( fSpRate < 0.0f )
//			fSpRate	=0.0f;
//		if( fAtkRate < 0.0f )
//			fAtkRate =0.0f;
//		if( fDefRate < 0.0f )
//			fDefRate =0.0f;
//		if( fPowRate < 0.0f )
//			fPowRate =0.0f;

	}

	void UpdateBuffConditionAttr( )
	{
		cAttrData attr =GetAttrData( cAttrData._CONDBUFF ) ;

		Buff.UpdateCondAttr (ref attr );


	}

	// this should work only each casting phase
	void UpdateFightAttr( )
	{
		if (FightAttr == null )
			return;

		FightAttr.ClearBase (); // clear base attr only


		if (FightAttr.SkillData != null) {
			MyTool.AttrSkillEffect( this , FightAttr.SkillData.CastPool , FightAttr.SkillData.CastCond , FightAttr.SkillData.CastCondEffectPool );
		}
	}

	//fight end to clear data
	public void FightEnd()
	{
		ClearState();


	}


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
		n_HP = GetMaxHP();
		n_MP = GetMaxMP();
		n_SP = GetMaxSP();
		n_DEF = GetMaxDef();
	}

	public void AddHp( int nhp )
	{
		if ( (Config.GOD==true) && nhp < 0 ) {
			Panel_unit p = Panel_StageUI.Instance.GetUnitByIdent( this.n_Ident );
			if( p != null && p.eCampID == _SRW._CAMP._ENEMY )
			{
				nhp *= 10;
			}
		}

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

	public void AddDef( int ndef )
	{
		
		
	}

	// Get Data func
	public int GetMaxHP()
	{
//		UpdateAttr(); // update first to get newest data
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
//		UpdateAttr(); // update first to get newest data
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
//		UpdateAttr(); // update first to get newest data
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

	// only get school + char lv
	public float GetBaseMar()
	{
		float f = 0;
		for (int i = cAttrData._INTSCH; i <=  cAttrData._CHARLV; i++) {
			cAttrData att = GetAttrData( i );
			f += att.f_MAR;
		}
		if (f < 0.0f )			f = 0.0f;
		return f;
	}

	public float GetMar()
	{
//		UpdateAttr(); // update first to get newest data
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
//		UpdateAttr(); // update first to get newest data
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
//		UpdateAttr(); // update first to get newest data
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
//		UpdateAttr(); // update first to get newest data
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
//		UpdateAttr(); // update first to get newest data
		int n = 0;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			n +=pair.Value.n_MOV;
		}
		if (n < 0)
			n = 0;
		return n;
	}

	// Fight Attr
	public void SetFightAttr( int nTarId , int SkillID )
	{
		FightAttr.Reset ();
		FightAttr.TarIdent = nTarId;
		FightAttr.SkillID = SkillID;
		//SKILL skill = ConstDataManager.Instance.GetRow< SKILL> ( SkillID ); 
		FightAttr.SkillData = new cSkillData ( ConstDataManager.Instance.GetRow< SKILL> ( SkillID ) );

		UpdateFightAttr();

//		FightAttr.Skill = skill;
//
//		if (FightAttr.Skill != null) {
//			FightAttr.CastPool = MyScript.Instance.CreateEffectPool (skill.s_CAST);
//			FightAttr.CastEffPool = MyScript.Instance.CreateEffectPool (skill.s_CAST_EFFECT);
//
//
//			FightAttr.HitPool = MyScript.Instance.CreateEffectPool (skill.s_HIT);
//			FightAttr.HitEffPool = MyScript.Instance.CreateEffectPool (skill.s_HIT_EFFECT);
//
//			UpdateFightAttr();
//		}
	}

	public void SetFightEnd(  )
	{

	}

	// need to updte each frame for each 
	public void UpdateBuffConditionEffect(  )
	{
		//cUnitData tar = GameDataManager.Instance.GetUnitDateByIdent( FightAttr.TarIdent ) ;
		cAttrData attr = this.GetAttrData (cAttrData._CONDBUFF);

		Buff.UpdateCondAttr (ref  attr );
	}

	public void DoSkillCastEffect( ref List< cHitResult > resPool )
	{
		if (FightAttr.SkillData == null)
			return;

		MyTool.DoSkillEffect ( this , FightAttr.SkillData.CastPool , FightAttr.SkillData.CastCond ,  FightAttr.SkillData.CastCondEffectPool , ref resPool  );

	}
	public void DoSkillHitEffect( ref List< cHitResult > resPool )
	{
		if (FightAttr.SkillData == null)
			return;
		
		MyTool.DoSkillEffect ( this , FightAttr.SkillData.HitPool , FightAttr.SkillData.HitCond ,  FightAttr.SkillData.CastCondEffectPool , ref resPool  );

//		if (FightAttr.Skill == null)
//			return;
//		
//		MyTool.DoSkillEffect ( this , FightAttr.HitPool , FightAttr.Skill.s_HIT_TRIG ,  FightAttr.HitEffPool , ref resPool  );


	}
	// state battle flag

	List< _UNITSTATE > States;

	List< _UNITSTATE > GetStates()
	{
		if (States == null) {
			States = new List< _UNITSTATE >();
		}
		return States;
	}

	public void AddStates( _UNITSTATE st ){
		if ( GetStates ().IndexOf (st) < 0 ) {
			GetStates ().Add( st );
		}
	
	}

	public bool IsStates( _UNITSTATE st ){ return  (GetStates ().IndexOf(st)>=0) ; }

	public void RemoveStates( _UNITSTATE st ){
		GetStates ().Remove (st);
	}

	public void ClearState(  ){
		States.Clear ();
	}
		
	//============================================================
	//


}

//
public class cMobGroup
{
	public int nGroupID{ set; get; }
	public cMobGroup()
	{
		memList = new List< int >{};

	}


	public List< int > memList ;
}