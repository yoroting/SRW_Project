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

	public static iVec2 SnyLocalPostoGrid( Vector3 v ,  ref MyGrids grids  )
	{
		iVec2 l = new iVec2( grids.GetGridX (v.x) , grids.GetGridY (v.y));
		return l;
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
	public static string GetCharName( int nID )
	{
		if (nID == 0) {
			return Config.PlayerFirst + Config.PlayerName;
		}
		CHARS chardata = ConstDataManager.Instance.GetRow<CHARS> (nID);
		if (chardata != null) {
			return chardata.s_NAME;
		}
		return "null";
	}


	public static string GetSkillName( int nID )
	{
		DataRow row = ConstDataManager.Instance.GetRow( (int)ConstDataTables.SKILL_TEXT , nID );
		if( row != null )
		{				
			string content = row.Field<string>( "s_TITLE");
			return content;
			
		}

		SKILL skl = ConstDataManager.Instance.GetRow< SKILL > ( nID );
		if (skl == null)
			return "null";
		return skl.s_NAME;
	}

	public static string GetBuffName( int nID )
	{
		DataRow row = ConstDataManager.Instance.GetRow( (int)ConstDataTables.BUFF_TIP , nID );
		if( row != null )
		{				
			string content = row.Field<string>( "s_TITLE");
			return content;

		}
		// try get in buff data
		BUFF buff = ConstDataManager.Instance.GetRow< BUFF > ( nID );
		if (buff != null) {
			return buff.s_NAME;
		}
		return "null";
	}
	public static string GetItemName( int nID )
	{
        if (nID == 0){
            return "- 無 -";
        }
		//BUFF buff = ConstDataManager.Instance.GetRow< BUFF > ( nID );
		//if (buff == null)
		//	return "null";
		ITEM_MISC item = ConstDataManager.Instance.GetRow< ITEM_MISC > ( nID );
		if (item == null)
			return "";
		return item.s_NAME;
	}

	public static string GetStoryName( int nID )
	{
		//	return "null";
		DataRow row = ConstDataManager.Instance.GetRow( "STORY_NAME" , nID );
		if( row != null )
		{				
			string content = row.Field<string>( "s_NAME");
            string sname = "";
          //  if (Config.GOD)
            {
                sname = string.Format("第{0}關 {1}", nID, content);
            }
            //else {
            //    sname = string.Format("下一關 {0}",  content);
            //}
			
			return sname;
		}
		return "null";

	}

    // 0-small , 1- Large
    public static Texture GetCharTexture( int nFaceID, int nSize=0)
    {
        //CHARS cdata = ConstDataManager.Instance.GetRow<CHARS>(nCharID);
        //if (cdata == null)
        //    return null ;
        //int nFaceID = nCharID;

        FACES data = ConstDataManager.Instance.GetRow<FACES>(nFaceID);
        if (data == null)
            return null ;

        string url = "Art/char/" + data.s_FILENAME ;
        switch (nSize)
        {
            case 1:   url += "_L"; break;
            default:  url += "_S"; break;
        }
      
        Texture t = Resources.Load(url, typeof(Texture)) as Texture;
        return t;
    }

    public static string GetSchoolName( int nSchool)
    {
        SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(nSchool);   //GameDataManager.Instance.GetConstSchoolData ( nSchool );
        if (sch == null)
            return "Error-No Skill";

        return sch.s_NAME;             
    }
    //public static string GetUnitSchoolFullName( int nIdent , int nSchool )
    //{
    //	cUnitData data = GameDataManager.Instance.GetUnitDateByIdent (nIdent);
    //	if (data == null)
    //		return "Error-No Unit";

    //	SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(nSchool);   //GameDataManager.Instance.GetConstSchoolData ( nSchool );
    //	if (sch == null)
    //		return "Error-No Skill";

    //	int nLv = 0;
    //	if (data.SchoolPool.TryGetValue (nSchool, out nLv) == true ) {
    //		return sch.s_NAME + "(" +nLv.ToString() + ")";
    //	}
    //	return "Error- No Leran School" + nSchool.ToString();
    //}


    public static string GetUIText( int nID )
	{
        DataRow row = ConstDataManager.Instance.GetRow("UI_MESSAGE", nID);
        if (row != null)
        {
            string msg = row.Field<string>("s_UI_WORDS");
            return msg;
        }
        return "null";
       
	}

	public static string GetSYSText( int nID )
	{
        DataRow row = ConstDataManager.Instance.GetRow("MESSAGE", nID);
        if (row != null)
        {
            string msg = row.Field<string>("s_MESSAGE");
            return msg;
        }
        return "null";
    }

    public static string GetMsgText(int nID)
    {
        DataRow row = ConstDataManager.Instance.GetRow("MESSAGE", nID);
        if (row != null)
        {
            string msg = row.Field<string>("s_MESSAGE");      
            return msg;
        }
        return "null";
    }

    public static bool CheckInPosPool( List < iVec2 > lst , iVec2 v )
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
	
	public static bool CheckInRect( int X , int Y , int rx1 , int ry1 , int w , int h )
	{
		w = Mathf.Abs (w);
		h = Mathf.Abs (h);
		if (rx1 <= X && X <= (rx1+w) ) {
			if (ry1 <= Y && Y <= (ry1+h) ){
				return true;
			}
		}
		return false;
	}

	// tool func to get aoe affect pool
	public static List < iVec2 > GetAOEPool( int nX , int nY , int nAoe , int nCastX , int nCastY )
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
						int x = 0;
						int.TryParse( arg[0] , out x);
						int y = 0;
						int.TryParse( arg[1] , out y );
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
			return true; // skill -0 is damage skill
		}
		cSkillData skldata = GameDataManager.Instance.GetSkillData (nSkillID);
		if( skldata != null  ){
			return skldata.IsTag( _SKILLTAG._DAMAGE );
		}
		return false;
	}

    public static bool IsFinishSkill(int nSkillID)
    {
        if (nSkillID == 0)
        {
            return true; // skill -0 is damage skill
        }

        SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nSkillID);
        if (skl != null)
        {
            if (skl.n_FINISH > 0)
                return true;
        }        
        return false;
    }

    public static bool IsSkillTag( int nSkillID , _SKILLTAG tag )
	{
		if (nSkillID == 0) {
			if( tag == _SKILLTAG._DAMAGE ){
				return true;
			}
			else {
				return false;  
			}
		}
		cSkillData skldata = GameDataManager.Instance.GetSkillData (nSkillID);
		if( skldata != null  ){
			return skldata.IsTag( tag );
		}
		return false;
	}


	public static int GetSkillRange( int nID  , out int nMax,out int nMin  )
	{
		// default value
		nMin = 0;
		nMax = 1;
		if (nID == 0){
			nMin = 1; // normal atk can't atk self
			return 1;
		}

		SKILL skl = ConstDataManager.Instance.GetRow< SKILL > ( nID );
        if (skl == null)
        {
            nMin = 1;
            return 0;
        }
		// get const value
		nMin = skl.n_MINRANGE;
		nMax = skl.n_RANGE;
		if( nMax == 0) { 
			nMax =1;
		}

		return skl.n_RANGE;
	}

    public static bool GetSkillCanPKmode(int nSkillID)
    {
        _PK_MODE mode = GetSkillPKmode(nSkillID);
        if( (mode == _PK_MODE._ENEMY) || (mode == _PK_MODE._ALL) )
        {
            return true;
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

		if ( skl.n_TARGET == 5 || skl.n_TARGET == 8 || skl.n_TARGET == 11 ) {// 5→MAPALL , 8→自我AOEALL , 11→ALL
			return _PK_MODE._ALL;
		}
		else if (skl.n_TARGET == 2 || skl.n_TARGET == 4 || skl.n_TARGET ==6 || skl.n_TARGET == 9 ) {			//2→需要友方目標 , 4→MAP我方 , 6→自我AOE我方 ,9→我ALL
			return _PK_MODE._PLAYER;
		}
		return _PK_MODE._ENEMY;
	}

//	public static int GetSkillTarget( SKILL skill )
//	{
//		if( skill == null )
//			return 1;
//		switch( skill.n_TARGET ){
//			case 0:	//0→對自己施展
//			case 6:	//6→自我AOE我方
//			case 7:	//7→自我AOE敵方
//			case 8:	//8→自我AOEALL
//			case 9:	//9→我ALL
//			case 10:	//10→敵ALL
//			case 11:	//11→ALL
//				return 0;
//				break;
//			case 1:	//→需要敵方目標
//			case 2:	//→需要友方目標
//				return 1;
//			break;
//			case 3:	//→MAP敵方
//			case 4: //→MAP我方
//			case 5:	//→MAPALL		
//				return -1;
//			break;
//		}
//
//		return -1;
//	}
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

		// 销毁现有元素
		//while (grid.transform.childCount > 0)
		//{
		//	NGUITools.DestroyImmediate(grid.transform.GetChild(0).gameObject); // this func don't works in PC mode and cause infin loop crash
		//}


		
		List< Transform > lst = grid.GetChildList ();
        //		//List< GameObject > CmdBtnList = MyTool.GetChildPool( NGuiGrids );
        //		
    if (lst != null) {
        foreach (Transform t in lst)
        {

            ///UIEventListener.Get(obj).onClick -= OnCMDButtonClick;;  // no need.. destory soon
            NGUITools.Destroy(t.gameObject);
        }
    }
       
        grid.repositionNow = true;		// need this for second pop to re pos
		grid.Reposition ();             // for re value
	}

	public static void DestoryTweens( GameObject go )
	{
		if ( go == null)
			return ;
		UITweener[] tws = go.GetComponents< UITweener > ();
		foreach (UITweener tw in tws) {
			NGUITools.Destroy (tw);
			//Destroy( tw );B
		}
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

	public static void SetLabelColor( GameObject go , Color color )
	{
		UILabel lbl = go.GetComponent< UILabel >();
		if( lbl != null )
		{
			lbl.color = color;
		}
	}

	public static void SetBuffIcon( GameObject go ,int nID , int nNum )
	{
		BuffIcon icon = go.GetComponent<BuffIcon>();
		if( icon != null ){
			icon.SetBuffData( nID ,nNum );

		}
	}

    public static void ResetScrollView(GameObject go)
    {
        UIScrollView sv = go.GetComponent<UIScrollView>();
        if ( sv != null ) {
            sv.ResetPosition();
        }
    }


    public static int RollWidgetPool( Dictionary<int , int > pool )
	{
		if( pool == null )
			return 0;
		//=====================================
		int totWidget = 0;
		foreach( KeyValuePair<int,int> p in pool ){
			totWidget += p.Value;
		}
		int roll = UnityEngine.Random.Range( 0 , totWidget );
		// find item
		int sumWidget = 0;
		foreach( KeyValuePair<int,int> p2 in pool ){
			sumWidget += p2.Value;
			if( sumWidget >= roll ){
				return p2.Key;
			}
		}
		return 0; 
	}

	public static void RotateGameObjToGridXY( GameObject go , int nX , int nY , int nToX , int nToY  , int nRotType = 0 )
	{
		_DIR dir =  iVec2.Get8Dir ( nX, nY ,nToX, nToY );
		//Vector3 rot ;
		switch (dir) {
		case _DIR._UP:
			if( 0 == nRotType ){ // Z-axis
				go.transform.localRotation = Quaternion.Euler (0, 0, 0);
			}
			else{				// y - axis
				go.transform.localRotation = Quaternion.Euler (-90, 0, 0);
			}
			break;
		case _DIR._RIGHT:
			if( 0 == nRotType ){
				go.transform.localRotation = Quaternion.Euler (0, 0, -90);
			}
			else{
				go.transform.localRotation = Quaternion.Euler (0, 90, 0);
			}
			break;
		case _DIR._DOWN:
			if( 0 == nRotType ){
				go.transform.localRotation = Quaternion.Euler (0, 0, 180);
			}
			else{
				go.transform.localRotation = Quaternion.Euler (90, 0, 0);
			}
			break;
		case _DIR._LEFT:
			if( 0 == nRotType ){
				go.transform.localRotation = Quaternion.Euler (0, 0, 90);
			}
			else{
				go.transform.localRotation = Quaternion.Euler (0, -90, 0);
			}
			break;
			// 8 way 
		case _DIR._RIGHT_UP:
			if( 0 == nRotType ){
				go.transform.localRotation = Quaternion.Euler (0, 0, -45);
			}
			else{
				go.transform.localRotation = Quaternion.Euler (-45, 90, 0);
			}
			break;
		case _DIR._LEFT_UP:
			if( 0 == nRotType ){
				go.transform.localRotation = Quaternion.Euler (0, 0, 45);
			}
			else{
				go.transform.localRotation = Quaternion.Euler (-45, -90, 0);
			}
			break;
		case _DIR._RIGHT_DOWN:
			if( 0 == nRotType ){
				go.transform.localRotation = Quaternion.Euler (0, 0, -135);
			}
			else{
				go.transform.localRotation = Quaternion.Euler (45, 90, 0);
			}
			break;
		case _DIR._LEFT_DOWN:
			if( 0 == nRotType ){
				go.transform.localRotation = Quaternion.Euler (0, 0, 135);	
			}
			else{
				go.transform.localRotation = Quaternion.Euler (45, -90, 0);
			}
			break;
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
		int nID = (int)eID;
        string s = GetUIText(nID);
        if( s != "null"){
            return s;
        }

	////	if (ConstDataManager.Instance != null ) {
	//		DataRow row = ConstDataManager.Instance.GetRow("UI_MESSAGE", nID); 	
	//		/// if(row != null)	
	//		///		Debug.Log(row.Field<string>("s_UI_WORDS"));
	//		/// 
	//	//	DataRow row = ConstDataManager.Instance.GetRow<DataRow> ("UI_MESSAGE", nID);
	//		if (row != null) {
	//			string s = row.Field< string > ("s_UI_WORDS");
	//			if (s != null) {
	//				return s;
	//			}
	//		}
	////	}
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
		if (obj != null)
			return obj.GetComponent< T > ();
		else {
			Debug.LogError( "err GetPanel<T>" );
			return default (T);
		}
	}
    public static T GetPanel<T>(string  name )
    {
        GameObject obj = PanelManager.Instance.OpenUI(name );
        if (obj != null)
            return obj.GetComponent<T>();
        else
        {
            Debug.LogError("err GetPanel<T>");
            return default(T);
        }
    }
    public static List<T> CutList<T>( List<T> lst , int Len )
	{
        if (lst == null)
            return null;

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

	public static Dictionary< string , int > ConvetToStringInt( Dictionary< int , int > org )
	{
		Dictionary<string,int>  tar = new Dictionary<string,int>();
		foreach( KeyValuePair< int , int > pair in org ){
			tar.Add( pair.Key.ToString() , pair.Value );
		}
		return tar;
	}

	public static Dictionary< int , int > ConvetToIntInt( Dictionary< string , int > org )
	{
		Dictionary<int,int>  tar = new Dictionary<int,int>();
		foreach( KeyValuePair< string , int > pair in org ){
			int id = 0;
			if( int.TryParse( pair.Key , out id ) == true ){
				tar.Add( id , pair.Value );
			}
		}
		return tar;
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

    static public void SetDepth(GameObject go, int n , GameObject from = null )
    {
        UIWidget widget = go.GetComponent<UIWidget>();
        if (from != null)
        {
            UIWidget widget2 = from.GetComponent<UIWidget>();
            if (widget2 != null)
            {
                widget.depth = widget2.depth + n;
                return;
            }
        }
        widget.depth = n;
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
