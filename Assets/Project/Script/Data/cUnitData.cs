using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;

public class cBuffData
{
	public int nID;
}


public class cAttrData
{
	public float f_MAR;

	// load data
	public int n_HP;
	public int n_MP;
	public int n_SP;
	public int n_DEF;

	public int n_ATK;
	public int n_POW;	

	public int n_MOV;
}

// current stage runtime data
public class cUnitData{
	public int n_Ident;		// auto create by game system

	public CHARS cCharData;
	public int n_CharID;
	public int n_Rank;		// max school lv

	// save data
	public int n_EXP;
	public int n_Lv;
	public int n_X;
	public int n_Y;

	public int n_HP;
	public int n_MP;
	public int n_SP;
	public int n_DEF;

	public int [] nActSch;		// current use 
	bool [] bUpdateFlag;

	// temp scale
	public float fScaleHp	=1.0f;
	public float fScaleMp	=1.0f;
	public float fScaleSp	=1.0f;
	public float fScaleAtk	=1.0f;
	public float fScaleDef	=1.0f;
	public float fScalePow	=1.0f;


	// calcul attr
	Dictionary< int , cAttrData > Attr; 		// 0-內功 , 1-外功  , 2-等級 , 3- buff 

	// school
	Dictionary< int , int >		SchoolPool;			// all study school < school id , lv >
	Dictionary< int , int >		AbilityPool;		// all ability school < ability id , lv can use >

	List<cBuffData>				BuffPool;			// all buff pool , need save /load
	Dictionary< int , int >		CDPool;				// all study school < cd type , round >
	// Buff list

	public cUnitData()
	{
		SchoolPool  = new Dictionary< int , int > ();
		AbilityPool = new Dictionary< int , int > ();
		BuffPool = new List<cBuffData> ();
		CDPool = new Dictionary< int , int > ();
		
		Attr = new Dictionary< int , cAttrData > (); 
		nActSch = new int []{0,0};
		bUpdateFlag = new bool[]{ true,true,true,true } ;
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

	void SetLevel( int lv )
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
		if (bUpdateFlag [0] == true) {
			UpdateSchoolAttr (0, nActSch [0]);
			bUpdateFlag [0] = false;
		}

		if (bUpdateFlag [1] == true) {
			UpdateSchoolAttr (1, nActSch [1]);
			bUpdateFlag [1] = false;
		}

		if (bUpdateFlag [2] == true) {
			UpdateLevelAttr (n_Lv);
			bUpdateFlag [2] = false;
		}
		if (bUpdateFlag [3] == true) {
			UpdateBuffAttr ();
			bUpdateFlag [3] = false;
		}


	}


	void UpdateLevelAttr( int nLV )
	{
		cAttrData attr =GetAttrData( 2 ) ;
		if ( nLV > Config.MaxCharLevel ) {
			nLV = Config.MaxCharLevel;
		}
		attr.n_SP = Config.CharBaseSp + nLV * Config.CharSpLVUp;

		attr.f_MAR = Config.CharMarLVUp * nLV;
	}

	void UpdateSchoolAttr( int nIdx , int nSchool )
	{	
		int nLv = SchoolPool[ nSchool ];
		 
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
		cAttrData attr =GetAttrData( 3 ) ;

		// update add value

		// update scale value

		// fix error range
		if( fScaleHp < 0.0f )
			fScaleHp	=0.0f;
		if( fScaleMp < 0.0f )
			fScaleMp	=0.0f;
		if( fScaleSp < 0.0f )
			fScaleSp	=0.0f;
		if( fScaleAtk < 0.0f )
			fScaleAtk	=0.0f;
		if( fScaleDef < 0.0f )
			fScaleDef	=0.0f;
		if( fScalePow < 0.0f )
			fScalePow	=0.0f;

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

	// Get Data func
	public int GetMaxHP()
	{
		UpdateAttr(); // update first to get newest data
		int nHp = 0;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			nHp +=pair.Value.n_HP;
		}
		if (nHp < 1)			nHp = 1;

		return (int)(fScaleHp*nHp);
	}
	public int GetMaxMP()
	{
		UpdateAttr(); // update first to get newest data
		int nMp = 0;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			nMp +=pair.Value.n_MP;
		}
		if (nMp < 1)			nMp = 1;
		return nMp;
	}
	public int GetMaxSP()
	{
		UpdateAttr(); // update first to get newest data
		int nSp = 0;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			nSp +=pair.Value.n_SP;
		}
		if (nSp < 1)			nSp = 1;
		return nSp;
	}

	public float GetMar()
	{
		UpdateAttr(); // update first to get newest data
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
		UpdateAttr(); // update first to get newest data
		int n = 0;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			n +=pair.Value.n_ATK;
		}
		if (n < 0)
			n = 0;
		return (int)(fScaleAtk* n);
	}

	public int GetDef()
	{
		UpdateAttr(); // update first to get newest data
		int n = 0;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			n +=pair.Value.n_DEF;
		}
		if (n < 0)
			n = 0;
		return (int)(fScaleDef* n);
	}
	public int GetPow()
	{
		UpdateAttr(); // update first to get newest data
		int n = 0;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			n +=pair.Value.n_POW;
		}
		if (n < 0)
			n = 0;
		return (int)(fScalePow * n);
	}

	public int GetMov()
	{
		UpdateAttr(); // update first to get newest data
		int n = 0;
		foreach( KeyValuePair< int ,cAttrData > pair  in Attr )
		{
			n +=pair.Value.n_MOV;
		}
		if (n < 0)
			n = 0;
		return n;
	}
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