using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
using MYGRIDS;
using MyClassLibrary;
using System.Linq;
//using _SRW;

// 
public class MyTool {

	// cache value
	public static float fScnRatio ;

	// All static func as tool
	public static float ScreenToLocX( float ScnX )
	{
		float x = (ScnX -(Screen.width/2)) * fScnRatio;	
		return x;
	}
	public static float ScreenToLocY( float ScnY )
	{
		float y = (ScnY -(Screen.height/2)) * fScnRatio;
		return y;
	}

	public static Vector3 LocToScreenX( GameObject obj )
	{
		return  UICamera.currentCamera.WorldToScreenPoint( obj.transform.position );
	}

	public static Vector3 SnyGridtoLocalPos( int x , int y ,  ref MyGrids grids  )
	{
		Vector3 v = new Vector3();
		v.x = grids.GetRealX( x );
		v.y = grids.GetRealY( y );
		return v;

	}

	public static float GetEffectValue( float fValue , float fRate , float fPlus )
	{
		if( fRate < 0.0f ) fRate = 0.0f;
		return (fValue*fRate) + fPlus;
	}

	public static int GetEffectValue( int nValue , float fRate , int nPlus )
	{
		if( fRate < 0.0f ) fRate = 0.0f;
		return (int )(nValue*fRate) + nPlus;
	}

	public static Panel_CMDUnitUI CMDUI()
	{
		return Panel_CMDUnitUI.JustGetCMDUI ();
		//return MyTool.GetPanel<Panel_CMDUnitUI> ( PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name) );
	}

	public static string GetCMDName( _CMD_ID ID )
	{
		return ID.ToString ();

	}

	public static string GetSkillName( int nID )
	{
		SKILL skl = ConstDataManager.Instance.GetRow< SKILL > ( nID );
		if (skl == null)
			return "null";
		return skl.s_NAME;
	}

	public static string GetBuffName( int nID )
	{
		//BUFF buff = ConstDataManager.Instance.GetRow< BUFF > ( nID );
		//if (buff == null)
		//	return "null";
		DataRow row = ConstDataManager.Instance.GetRow( (int)ConstDataTables.BUFF_TIP , nID );
		if( row != null )
		{				
			string content = row.Field<string>( "s_TITLE");
			return content;

		}
		return "null";
	}
	public static string GetItemName( int nID )
	{
		//BUFF buff = ConstDataManager.Instance.GetRow< BUFF > ( nID );
		//if (buff == null)
		//	return "null";
		ITEM_MISC item = ConstDataManager.Instance.GetRow< ITEM_MISC > ( nID );
		if (item == null)
			return "null";
		return item.s_NAME;
	}


	public static string GetUnitSchoolFullName( int nIdent , int nSchool )
	{
		cUnitData data = GameDataManager.Instance.GetUnitDateByIdent (nIdent);
		if (data == null)
			return "Error-No Unit";

		SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(nSchool);   //GameDataManager.Instance.GetConstSchoolData ( nSchool );
		if (sch == null)
			return "Error-No Skill";

		int nLv = 0;
		if (data.SchoolPool.TryGetValue (nSchool, out nLv) == true ) {
			return sch.s_NAME + "(" +nLv.ToString() + ")";
		}
		return "Error- No Leran School" + nSchool.ToString();
	}

	static bool CheckInPosPool( List < iVec2 > lst , iVec2 v )
	{
		if (lst == null || v == null)
			return false;
		foreach (iVec2 v2 in lst) {
			if( v2.Collision( v ) ){
				return true;
			}
		}
		return false;
	}


	// tool func to get aoe affect pool
	static public List < iVec2 > GetAOEPool( int nX , int nY , int nAoe , int nCastX , int nCastY )
	{
		List < iVec2 > pool = null;
		iVec2 st 	= new iVec2 ( nX , nY );
		List< iVec2 > tmp = new List< iVec2 >();
	//	tmp.Add (  st );

	//	int nRotate = GetGridDir(nX ,nY ,nCastX ,nCastY ); // normal
		_DIR dir = iVec2.GetDir ( nCastX , nCastY ,nX , nY );
		//			pool.Add ( st );
		
		AOE aoe = ConstDataManager.Instance.GetRow<AOE> ( nAoe ) ;
		if (aoe != null) {
			// add extra first
			cTextArray TA = new cTextArray();
			TA.SetText ( aoe.s_EXTRA );
			for( int i = 0 ; i < TA.GetMaxCol(); i++ )
			{
				CTextLine line  = TA.GetTextLine( i );
				for( int j = 0 ; j < line.GetRowNum() ; j++ )
				{
					string s = line.m_kTextPool[ j ];
					
					string [] arg = s.Split( ",".ToCharArray() );
					if( arg.Length < 2 )
						continue;

					if( arg[0] != null && arg[1] != null )
					{
						int x = int.Parse( arg[0] );
						int y = int.Parse( arg[1] );
						iVec2 t = new iVec2( x , y );
						t.Rotate( dir );
						iVec2 v = st + t; 

						if( GameScene.Instance.Grids.Contain( v ) == false )
							continue;
						tmp.Add(  v );
					}
				}
			}
			// get range pool	
			pool = GameScene.Instance.Grids.GetRangePool( st , aoe.n_MAX , aoe.n_MIN );
		}

		// merge two pool
		if (pool == null) {
			pool = new List< iVec2 >();
		}

		foreach( iVec2 v2 in tmp ){
			if( CheckInPosPool( pool , v2 ) == false ){ // final check
				pool.Add(  v2 );
			}
		}

		//List < iVec2 > pool = new List < iVec2 > ();
		return pool;
	}

	public static bool IsDamageSkill( int nSkillID )
	{
		if (nSkillID == 0) {
			return true;
		}
		cSkillData skldata = GameDataManager.Instance.GetSkillData (nSkillID);
		if( skldata != null  ){
			return skldata.IsTag( _SKILLTAG._DAMAGE );
		}
		return false;
	}

	public static _PK_MODE GetSkillPKmode( int nSkillID )
	{
		return GetSkillPKmode ( ConstDataManager.Instance.GetRow< SKILL > ( nSkillID ) );
	}

	public static _PK_MODE GetSkillPKmode( SKILL skl )
	{
		if (skl == null )
			return _PK_MODE._ENEMY;

		if ( skl.n_TARGET == 5 || skl.n_TARGET == 8 ) {// 5→MAPALL , 8→自我AOEALL
			return _PK_MODE._ALL;
		}
		else if (skl.n_TARGET == 2 || skl.n_TARGET == 4 || skl.n_TARGET ==6 ) {			//2→需要友方目標 , 4→MAP我方 , 6→自我AOE我方
			return _PK_MODE._PLAYER;
		}

		return _PK_MODE._ENEMY;
	}
	//
	public static List <GameObject > GetChildPool( GameObject obj)
	{
		if (obj == null)
			return null;

		List <GameObject > pool = new List <GameObject >();

		foreach (Transform child in obj.transform)
		{
			//child is your child transform

			pool.Add( child.gameObject );

		}
		return pool;
	}

    public static void DestoryImmediateAllChildren(GameObject root)
    {
        var tempList = root.transform.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            NGUITools.DestroyImmediate(child.gameObject);
        }
    }

	public static void DestoryGridItem( GameObject go )
	{
		if ( go == null)
			return ;

		UIGrid grid = go.GetComponent<UIGrid>(); 
		if (grid == null) {
			return ;
		}
		
		List< Transform > lst = grid.GetChildList ();
		//List< GameObject > CmdBtnList = MyTool.GetChildPool( NGuiGrids );
		
		if (lst != null) {
			foreach (Transform t in lst) {
				
				///UIEventListener.Get(obj).onClick -= OnCMDButtonClick;;  // no need.. destory soon
				NGUITools.Destroy (t.gameObject);
			}
		}

		grid.repositionNow = true;		// need this for second pop to re pos
	}

	//
	public static void SetLabelText( GameObject go , string sText )
	{
		UILabel lbl = go.GetComponent< UILabel >();
		if( lbl != null )
		{
			lbl.text = sText;
		}
	}
	
	public static void SetLabelInt( GameObject go , int nValue )
	{
		UILabel lbl = go.GetComponent< UILabel >();
		if( lbl != null )
		{
			lbl.text =  nValue.ToString();
		}
	}
	
	public static void SetLabelFloat( GameObject go , float fValue )
	{
		UILabel lbl = go.GetComponent< UILabel >();
		if( lbl != null )
		{
			lbl.text =  fValue.ToString();
		}
	}

	public static void SetBuffIcon( GameObject go ,int nID , int nNum )
	{
		BuffIcon icon = go.GetComponent<BuffIcon>();
		if( icon != null ){
			icon.SetBuffData( nID ,nNum );

		}
	}


	public static void TweenSetOneShotOnFinish( UITweener Tween , EventDelegate.Callback call )
	{
		if (Tween != null) {
			Tween.style = UITweener.Style.Once;

			EventDelegate del = new EventDelegate( call );
			del.oneShot = true;
			Tween.SetOnFinished( del );
		}
	}
	// Get CmdID key name
	public static string GetCMDNameByID( _CMD_ID eID )
	{
		return eID.ToString ();
	}
	public static _CMD_ID GetCMDIDByName( string name )
	{
		string[] names = Enum.GetNames(typeof(_CMD_ID));
		_CMD_ID[] values = (_CMD_ID[])Enum.GetValues(typeof(_CMD_ID));
		
		for( int i = 0; i < names.Length; i++ )
		{
			if( name == names[i] ){
				return values[i];
			}

		}
		return _CMD_ID._NONE;

	}


	public static T GetPanel<T>( GameObject obj   )
	{	
		return obj.GetComponent< T > ();  
	}

	public static List<T> CutList<T>( List<T> lst , int Len )
	{
		int c = lst.Count - Len;
		if (c > 0) {
			lst.RemoveRange (Len, c);
		}
		return lst;
//
//		lst.RemoveRange(
//
//		List<T> cut = new List<T> ();
//		int c = 0;
//		foreach( T t in lst)
//		{
//			//iVec2 pos = new iVec2( pt.X-hW , pt.Y -hH );
//			cut.Add( t );
//			if( ++c >= Len  )
//			{
//				break;
//			}
//		}
//		return cut;
	}
	//取得指定CDTYPE 有多少回合
	public static int GetRoundByCD( int nCD )
	{
		CD_TIMER cd = ConstDataManager.Instance.GetRow<CD_TIMER> (nCD);
		if (cd != null ) {
			return cd.n_TIME;
		}
		return 0;
	}

	public static cSkillData GetSkillData( int nSkillID ){
		return GameDataManager.Instance.GetSkillData( nSkillID );
	}

//	public static void DoSkillEffect( cUnitData atker , cUnitData defer , List< cEffect > effPool , cEffectCondition EffCond, List< cEffect > CondEffPool , ref List<cHitResult>  pool  )
//	{
//		if (atker == null || effPool == null )
//			return;
//
//		//cUnitData defer = GameDataManager.Instance.GetUnitDateByIdent ( atker.FightAttr.TarIdent );
//
//		// normal eff
//		foreach( cEffect eft  in effPool )
//		{
//			eft._Do( atker , defer , ref  pool );
//		}
//		if ( EffCond == null || CondEffPool == null)
//			return;
//
//		//cond eff
//		//if (MyScript.Instance.CheckSkillCond (strCond, atker, defer) == true)
//		if( EffCond.Check( atker , defer , atker.FightAttr.SkillID , 0  ) == true )
//		{
//
//			foreach( cEffect eft  in CondEffPool )
//			{
//				eft._Do( atker , defer , ref pool );
//			}
//		}
//	}
//
//	public static void HitSkillEffect( cUnitData atker , cUnitData defer , List< cEffect > effPool , cEffectCondition EffCond, List< cEffect > CondEffPool , ref List<cHitResult>  pool  )
//	{
//		if (atker == null || effPool == null )
//			return;
//		
//		//cUnitData defer = GameDataManager.Instance.GetUnitDateByIdent ( atker.FightAttr.TarIdent );
//		
//		// normal eff
//		foreach( cEffect eft  in effPool )
//		{
//			eft._Do( atker , defer , ref  pool );
//		}
//		if ( EffCond == null || CondEffPool == null)
//			return;
//		
//		//cond eff
//		//if (MyScript.Instance.CheckSkillCond (strCond, atker, defer) == true)
//		if( EffCond.Check( atker , defer , atker.FightAttr.SkillID , 0  ) == true )
//		{
//			
//			foreach( cEffect eft  in CondEffPool )
//			{
//				eft._Do( atker , defer , ref pool );
//			}
//		}
//	}
//	public static void AttrSkillEffect( cUnitData atker , List< cEffect > effPool , cEffectCondition EffCond, List< cEffect > CondEffPool )
//	{
//		if (atker == null || effPool == null )
//			return;
//		cAttrData attr = atker.FightAttr;
//
//		cUnitData defer = GameDataManager.Instance.GetUnitDateByIdent ( atker.FightAttr.TarIdent );
//		
//		// normal eff
//		foreach( cEffect eft  in effPool )
//		{
//			eft._Attr(atker , defer , ref attr  )  ;
//		}
//		if ( EffCond == null || CondEffPool == null)
//			return;
//		
//		//cond eff
//		//if (MyScript.Instance.CheckSkillCond (strCond, atker, defer) == true)
//		if( EffCond.Check( atker , defer , atker.FightAttr.SkillID, 0 ) == true )
//		{
//			
//			foreach( cEffect eft  in CondEffPool )
//			{
//				eft._Attr(atker , defer ,ref attr  )  ;
//			}
//		}
//		
//	}

	static public bool CanPK( _CAMP camp1 ,  _CAMP camp2 ) 	
	{
		if (camp1 != camp2) {
			if( camp1 == _CAMP._ENEMY || camp2 == _CAMP._ENEMY )
			{
				return true;
			}
		}
		
		return false;
	}

	static public void SetAlpha( GameObject go  ,  float f ) 	
	{
		if (go == null)
			return;

		UIWidget [] mWidgets = go.GetComponentsInChildren<UIWidget>();
		
		for (int i = 0, imax = mWidgets.Length; i < imax; ++i)
		{
			UIWidget w = mWidgets[i];
			Color c = w.color;
			c.a = f;
			w.color = c;
		}
	}
	static public int ClampInt( int v  , int min  ,  int max ) 	
	{
		if (v < min)
			v = min;
		else if( v > max )
			v = max;
		return v;
	}

	// 动态的计算出现在manualHeight的高度。
	static private void AdaptiveUI()
	{
		int ManualWidth = 960;
		int ManualHeight = 640;
		UIRoot uiRoot = GameObject.FindObjectOfType<UIRoot>();
		if (uiRoot != null)
		{
			if (System.Convert.ToSingle(Screen.height) / Screen.width > System.Convert.ToSingle(ManualHeight) / ManualWidth)
				uiRoot.manualHeight = Mathf.RoundToInt(System.Convert.ToSingle(ManualWidth) / Screen.width * Screen.height);
			else
				uiRoot.manualHeight = ManualHeight;
		}
	}



}
