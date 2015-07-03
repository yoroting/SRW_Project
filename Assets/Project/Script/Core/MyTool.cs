using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enum = System.Enum;
using MYGRIDS;
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

	public static Vector3 SnyGridtoLocalPos( int x , int y ,  ref cMyGrids grids  )
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
		return MyTool.GetPanel<Panel_CMDUnitUI> ( PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name) );
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
		List<T> cut = new List<T> ();
		int c = 0;
		foreach( T t in lst)
		{
			//iVec2 pos = new iVec2( pt.X-hW , pt.Y -hH );
			cut.Add( t );
			if( ++c >= Len  )
			{
				break;
			}
		}
		return cut;
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

	public static void DoSkillEffect( cUnitData atker , List< cEffect > effPool , cEffectCondition EffCond, List< cEffect > CondEffPool , ref List<cHitResult>  pool  )
	{
		if (atker == null || effPool == null )
			return;

		cUnitData defer = GameDataManager.Instance.GetUnitDateByIdent ( atker.FightAttr.TarIdent );

		// normal eff
		foreach( cEffect eft  in effPool )
		{
			eft._Do( atker , defer , ref  pool );
		}
		if ( EffCond == null || CondEffPool == null)
			return;

		//cond eff
		//if (MyScript.Instance.CheckSkillCond (strCond, atker, defer) == true)
		if( EffCond.Check( atker , defer  ) == true )
		{

			foreach( cEffect eft  in CondEffPool )
			{
				eft._Do( atker , defer , ref pool );
			}
		}

	}
	public static void AttrSkillEffect( cUnitData atker , List< cEffect > effPool , cEffectCondition EffCond, List< cEffect > CondEffPool )
	{
		if (atker == null || effPool == null )
			return;
		cAttrData attr = atker.FightAttr;

		cUnitData defer = GameDataManager.Instance.GetUnitDateByIdent ( atker.FightAttr.TarIdent );
		
		// normal eff
		foreach( cEffect eft  in effPool )
		{
			eft._Attr(atker , defer , ref attr  )  ;
		}
		if ( EffCond == null || CondEffPool == null)
			return;
		
		//cond eff
		//if (MyScript.Instance.CheckSkillCond (strCond, atker, defer) == true)
		if( EffCond.Check( atker , defer  ) == true )
		{
			
			foreach( cEffect eft  in CondEffPool )
			{
				eft._Attr(atker , defer ,ref attr  )  ;
			}
		}
		
	}

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
