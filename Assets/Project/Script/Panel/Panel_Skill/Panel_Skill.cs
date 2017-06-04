using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyClassLibrary;			// for parser string
//using _SRW;

public class Panel_Skill : MonoBehaviour {
	public const string Name = "Panel_Skill";

	public GameObject OkBtn;
	public GameObject CloseBtn;
	public GameObject SkillGrid;
	public GameObject ScrollView;
	public GameObject SkillContent; // 說明
    //public GameObject SkillGridScrollBar;
    public GameObject SkillItemUnit;


    public GameObject SkillSprite;  //註解區
	public GameObject CastNote;


    public GameObject lblMP;  //玩家能量
    public GameObject lblCP;  // 玩家真氣
    public GameObject lblCondition;  // 技能禁止條件

    public _SKILL_TYPE eSkillType;

	public int 	nOpSkillID;			// current select skill ID
	int nOpIdent;
	int nOpCharID;

	cUnitData pData;
	Dictionary<GameObject  , SKILL > sklPool;


	//bool bLongPress;		// is long press?
	private float _longClickDuration = 1.0f;
	float _lastPress = -1f;

	void Awake()
	{
		sklPool = new Dictionary<GameObject  , SKILL > ();
		
		UIEventListener.Get(OkBtn).onClick += OnOkClick; // for trig next line
		UIEventListener.Get(CloseBtn).onClick += OnCloseClick; // for trig next line

        //	CastNote.SetActive( true );
    //    SkillSprite.SetActive(true);
        SkillItemUnit.SetActive(false);

        nOpSkillID = 0;
	}

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
	
		if (_lastPress > 0) {
			if( Time.realtimeSinceStartup - _lastPress > _longClickDuration )
			{
				//CastSkill();
				if( SkillSprite.activeSelf == false  ){
					//SKILL skl = ConstDataManager.Instance.GetRow< SKILL>( nOpSkillID );
					//SetSkill( skl );
                    SetSkill(nOpSkillID );

                }
			}
		}
	}
    // 
    public void ShowMpCost( int nSkillID )
    {
        int nCost = 0;
        int nCP = 0;
        
        if (nSkillID != 0)
        {
            SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nSkillID);
            if (skl != null)
            {
                if (eSkillType == _SKILL_TYPE._ABILITY)
                {
                    nCost = skl.n_SP;
                }
                else {
                    nCost = skl.n_MP;
                }

                nCP = skl.n_CP;
            }
        } 

        string sCP = string.Format("{0}/{1}", nCP, pData.n_CP);
        string sCond = "";

        if (eSkillType == _SKILL_TYPE._ABILITY)
        {
            lblCP.SetActive( false );
            lblCondition.SetActive(false);

            string sSP = string.Format("{0}/{1}", nCost, pData.n_SP );
            MyTool.SetLabelText(lblMP, sSP);
        }
        else if (eSkillType == _SKILL_TYPE._SKILL)
        {
            lblCP.SetActive(true);
            //lblCondition.SetActive(false);

            string sMP = string.Format("{0}/{1}", nCost, pData.n_MP);
            MyTool.SetLabelText(lblMP, sMP);
        }
        else {
            lblCP.SetActive(true);
            //lblCP.SetActive(false);
            string sMP = string.Format("{0}/{1}", nCost, pData.n_MP);
            MyTool.SetLabelText(lblMP, sMP);
        }
        MyTool.SetLabelText(lblCP, sCP); 
        MyTool.SetLabelText(lblCondition, sCond); 
    }

	public void SetData( cUnitData data , _SKILL_TYPE eType  , cUnitData target , _CMD_TYPE cmdType  , int schoolid=0 )
	{
        // 移動後攻擊，不用換（距離不同了不能更新）
		if ( eType == _SKILL_TYPE._LAST  ) {
			return ; // don't change data
		}

		ClearData ();
		eSkillType = eType;
        //
        nOpIdent = 0;
        nOpCharID = 0;

        pData = data;
        if (pData != null)
        {
            nOpIdent = data.n_Ident;
            nOpCharID = data.n_CharID;
        }

		List< SKILL > sklLst = new List< SKILL > ();

        //神模式的主角 全招式都能放
        if (Config.GOD == true && data.n_CharID == 1 )
        {
            DataTable pTable = ConstDataManager.Instance.GetTable<SKILL>();
            if (pTable == null)
                return;

            foreach (SKILL skl in pTable)
            {
                if (skl.n_PASSIVE == 1)
                    continue;
                if (eType == _SKILL_TYPE._SKILL)
                {
                    if (skl.n_SCHOOL == 0)  // == 0 is ability
                        continue;
                }
                else
                {
                    if (skl.n_SCHOOL != 0)  // == 0 is ability
                        continue;
                }

                sklLst.Add(skl);
            }
        }
        //else
        { // normal

            if (eType == _SKILL_TYPE._SKILL)
            {

                foreach (int nID in pData.SkillPool)
                {
                    int nSkillID = nID;
                    nSkillID = pData.Buffs.GetUpgradeSkill(nSkillID); // Get upgrade skill
                    SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nSkillID);
                    if (skl.n_SCHOOL == 0)  // == 0 is ability
                        continue;
                    if (skl.n_PASSIVE == 1)
                        continue;

                    sklLst.Add(ConstDataManager.Instance.GetRow<SKILL>(nSkillID));
                }
            }
            else if (eType == _SKILL_TYPE._ABILITY)
            {
                int CLv = pData.n_Lv;

                foreach (KeyValuePair<int, int> pair in pData.AbilityPool)
                {
                    SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(pair.Key);
                    if (skl == null)
                        continue;
                    if (skl.n_SCHOOL > 0) // > 0 is school
                        continue;
                    if (skl.n_PASSIVE == 1)
                        continue;
                    if (Config.GOD)
                    {
                        sklLst.Add(skl);
                        continue;
                    }
                    if (CLv < pair.Value)
                        continue;
                    sklLst.Add(skl);
                }
            }
            else if (eType == _SKILL_TYPE._SCHOOL) // 察看指定 school
            {
                DataTable pTable = ConstDataManager.Instance.GetTable<SKILL>();
                if (pTable == null)
                    return;

                foreach (SKILL sklrow in pTable)
                {
                    if (sklrow.n_PASSIVE == 1)
                        continue;

                    if (sklrow.n_SCHOOL != schoolid)  // == 0 is ability
                        continue;
                    if(sklrow.n_LEVEL_LEARN < 0) // 進階技能不處理
                    {
                        continue;
                    }
                    // 取得進階技能
                    int nSkillID = sklrow.n_ID;
                    nSkillID = pData.Buffs.GetUpgradeSkill(nSkillID); // Get upgrade skill
                    SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(nSkillID);
                    if (skl.n_SCHOOL == 0)  // == 0 is ability
                        continue;
                    if (skl.n_PASSIVE == 1)
                        continue;
                    sklLst.Add(ConstDataManager.Instance.GetRow<SKILL>(nSkillID));
                }
            }
            // UI SLIDER 到1.0


        }
		// Create UI item

		foreach( SKILL skl in sklLst )
		{
				// add this skill
			GameObject go = ResourcesManager.CreatePrefabGameObj( SkillGrid , "Prefab/Item_Skill" ); 
			if( go == null )
				continue;

			Item_Skill item = go.GetComponent<Item_Skill> ();
            int nCost = (skl.n_SCHOOL > 0) ? skl.n_MP : skl.n_SP;

            item.SetItemData( MyTool.GetSkillName( skl.n_ID )  , skl.n_MINRANGE , skl.n_RANGE , nCost);
            if (skl.n_SCHOOL != 0)  // == 0 is ability
            {
                item.SetItemDmgData( skl.f_ATK , skl.f_POW );
            }
            item.SetItemCD( data.CDs.GetCD( skl.n_ID ) , skl.n_CD );

            item.SetScrollView( ScrollView );
            //check can use
            
            // school mode 都是可用的           

			item.SetEnable( eType == _SKILL_TYPE._SCHOOL || 
                CheckSkillCanUse( pData , skl , target , cmdType ) );
		//	UIEventListener.Get(go).onClick += OnSkillClick; // for trig next line
			UIEventListener.Get(go).onPress += OnSkillPress; // 

			sklPool.Add(  go , skl );
		}

		// default to 1 st skill
		SetSkill (null);
		//foreach (KeyValuePair<GameObject  , SKILL> pair in sklPool) {
		//	SetSkill( pair.Value );			// set to first 
		//	break;
		//}

		// for grid re pos
		UIGrid grid = SkillGrid.GetComponent<UIGrid>(); 
		grid.repositionNow = true;		// need this for second pop to re pos
		
		//CastNote.SetActive( false ); 

		UIScrollView uiScrollView = ScrollView.GetComponent<UIScrollView> ();
		if (uiScrollView != null) {
            grid.Reposition();             // need this for reset grid pos            v
            uiScrollView.ResetPosition();
          //  uiScrollView.Scroll(1.0f);


        }

        // 無技能
        ShowMpCost( 0 );

        
        //特定模式才能施展
        OkBtn.SetActive(eType != _SKILL_TYPE._SCHOOL );
        
    }



    void ClearData()
	{
		sklPool.Clear ();

		MyTool.DestoryGridItem ( SkillGrid );
//		UIGrid grid = SkillGrid.GetComponent<UIGrid>(); 
//		if (grid == null) {
//			return ;
//		}
//
//		List< Transform > lst = grid.GetChildList ();
//		//List< GameObject > CmdBtnList = MyTool.GetChildPool( NGuiGrids );
//		
//		if (lst != null) {
//			foreach (Transform t in lst) {
//			
//				///UIEventListener.Get(obj).onClick -= OnCMDButtonClick;;  // no need.. destory soon
//				NGUITools.Destroy (t.gameObject);
//			}
//		}
	}

	public static Panel_Skill OpenUI( int nIdent , _SKILL_TYPE eType , int nTarIdent , _CMD_TYPE cmdType  )
	{
		cUnitData data = GameDataManager.Instance.GetUnitDateByIdent ( nIdent );
		if (data == null)
			return null;
		GameObject go = PanelManager.Instance.OpenUI (Name );
		if (go == null) 
			return null;	

		Panel_Skill pUI = MyTool.GetPanel<Panel_Skill>( go );
		pUI.SetData( data , eType  ,GameDataManager.Instance.GetUnitDateByIdent ( nTarIdent ) , cmdType  );
		return pUI;
	}

    public static Panel_Skill OpenSchoolUI(cUnitData data, _SKILL_TYPE eType, int schoolID )
    {
        GameObject go = PanelManager.Instance.OpenUI(Name);
        if (go == null)
            return null;
        Panel_Skill pUI = MyTool.GetPanel<Panel_Skill>(go);
        pUI.SetData(data, _SKILL_TYPE._SCHOOL , null, _CMD_TYPE._SYS , schoolID);
        return pUI;
    }

    // show skill deteail
    void SetSkill(int SkillID )
    {
        SKILL skl = ConstDataManager.Instance.GetRow<SKILL>(SkillID);
        SetSkill(skl );
    }


    void SetSkill( SKILL skl )
	{
		SkillSprite.SetActive ( (skl != null) );
		if (skl == null) {

			return;
		}
        string sName = skl.s_NAME;

#if DEBUG
        sName += "-"+ skl.n_ID;
#endif//DEBUG

        MyTool.SetLabelText (SkillContent, sName );
		nOpSkillID = skl.n_ID;

        //	CastNote.SetActive (true);
        // change cost
        ShowMpCost(skl.n_ID);

	}

	void CastSkill( GameObject go  )
	{
        // 察看模式，不能施展
        if ( eSkillType == _SKILL_TYPE._SCHOOL ) {
            SetSkill(nOpSkillID);
            return;
        }

		Item_Skill  item = go.GetComponent<Item_Skill>();
		if(item != null) {
			if( item.bEnable == false ){

                // 不能施展，則視同 查註解
                SetSkill( nOpSkillID );
                return ;
			}
		}


		Panel_CMDUnitUI panel = MyTool.GetPanel<Panel_CMDUnitUI> ( PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name) );
		if (panel != null) {
			panel.SetSkillID( nOpSkillID );
		}
		
		PanelManager.Instance.CloseUI( Name );
	}

	void OnOkClick(GameObject go)
	{

		// use skill to atk
		//GameDataManager.Instance
		// send skill ok command
		CastSkill ( go );


		//PanelManager.Instance.CloseUI ( Name ); // close SKILL UI
	}

	void OnCloseClick(GameObject go)
	{
        if (eSkillType == _SKILL_TYPE._SCHOOL)
        {
            PanelManager.Instance.CloseUI(Name);
            return; // 避免施展技能
        }


		Panel_CMDUnitUI panel = MyTool.GetPanel<Panel_CMDUnitUI> ( PanelManager.Instance.OpenUI (Panel_CMDUnitUI.Name) );
		if (panel != null) {
			panel.SetSkillID( 0  ); // 0 is cancel
		}

		// need check original cmd bwfore skill cmd .. maybe is char cmd
		Panel_CMDUnitUI.RollBackCMDUIWaitTargetMode();
		//Panel_CMDUnitUI.OpenCMDUI ( _CMD_TYPE._WAITATK ,  nOpIdent   );

		// rebuilf wait atk mode
		//Panel_CMDUnitUI.CancelCmd ();


		//PanelManager.Instance.CloseUI ( Name );

	}

	void OnSkillClick(GameObject go)
	{
		SKILL skl = null;
		if (sklPool.TryGetValue (go, out skl) == false) {
			return ;
		}
		// show skill detail
		SetSkill ( skl );

		// 
		OnOkClick (go);
	}
	void OnSkillPress(GameObject go , bool pressed )
	{

		if (pressed) {
			_lastPress = Time.realtimeSinceStartup; 

			SKILL skl = null;
			if (sklPool.TryGetValue (go, out skl) == false) {
				return ;
			}
			// show skill detail
			// SetSkill ( skl );

			nOpSkillID = skl.n_ID;
		}
		else 
		{ 
			if (Time.realtimeSinceStartup - _lastPress > _longClickDuration) 
			{
				// don't send ok cmd

//				SKILL skl = null;
//				if (sklPool.TryGetValue (go, out skl) == false) {
//					return ;
//				}
//				// show skill detail
//				SetSkill ( skl );
				SetSkill (null);
				nOpSkillID = 0;
			}
			else{ // normal click . if cast note is not disappear
				//if( CastNote.activeSelf ){
				//	OnOkClick (go);
				//}
				//_lastPress = -1.0f;		// reset key
				CastSkill( go );
			}

			_lastPress = -1.0f;		// reset key
		} 
	}


	static public bool CheckSkillCanUse( cUnitData pCast , SKILL skl , cUnitData tarunit , _CMD_TYPE cmdType )
	{
		if (skl == null) {
			return false;
		}
		if (pCast == null)
			return false;

		if (pCast.CheckSkillCanUse (skl) == false)
			return false;

		//cCMD.Instance.
		cSkillData sklData = GameDataManager.Instance.GetSkillData ( skl.n_ID );
		if (sklData == null) {
			return false;
		}

        // 攻擊時不能使用 反擊技能

		//====反擊時不能使用 MAP AOE 技能===============
		if (cmdType == _CMD_TYPE._COUNTER) {
			//if( skl.n_TARGET >= 3 )
			//if( MyTool.GetSkillTarget( skl ) != 1 )
//			{
			// AOE skill can't use
//				return false;
//			}

			if( (skl.n_TARGET==3) || (skl.n_TARGET==4) || (skl.n_TARGET==5) ){
				//			case 6:	//6→自我AOE我方
				//			case 7:	//7→自我AOE敵方
				//			case 8:	//8→自我AOEALL
				//	//		case 3:	//→MAP敵方
				//	//		case 4: //→MAP我方
				//	//		case 5:	//→MAPALL		

				return false;
			}


			if (sklData.IsTag (_SKILLTAG._BANDEF )) {
				return false;
			}

			// self cast can cast every time
			if( (skl.n_TARGET!=0) ){
				int nDist = 0;
				if (tarunit != null) {
					nDist = MYGRIDS.iVec2.Dist (tarunit.n_X, tarunit.n_Y, pCast.n_X, pCast.n_Y);
				}

				int nMinRange ;
				int nRange ;
				MyTool.GetSkillRange( skl.n_ID ,out nRange,out nMinRange);
				if( (nDist > nRange) || (nDist < nMinRange) ) {
					return false;
				}
			}


		} else if (cmdType == _CMD_TYPE._WAITATK) { // move and atk mode 
			if (sklData.IsTag (_SKILLTAG._NOMOVE)) {
				return false;
			}
            if (sklData.IsTag(_SKILLTAG._BANATK)){
                return false;
            }

        } else { // normal atk
			if (sklData.IsTag (_SKILLTAG._BANATK)) {
				return false;
			}

		}


		// check condition
		return true;
	}
}

