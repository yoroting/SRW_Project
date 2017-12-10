using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel_UnitInfo : MonoBehaviour {

	public const string Name = "Panel_UnitInfo";

    // char face image
	public GameObject FaceObj;
	public GameObject NameObj;
    public GameObject btnStory;

    //data
    public GameObject MarObj;
	public GameObject HpObj;
	public GameObject MpObj;
	public GameObject SpObj;
	public GameObject MovObj;

	public GameObject AtkObj;
	public GameObject DefObj;
	public GameObject PowObj;

    public GameObject BrustObj;
    public GameObject ReduceDamageObj;
    public GameObject ArmorObj;

    public GameObject IntSchObj;
	public GameObject ExtSchObj;

	public GameObject LvObj;
	public GameObject ExpObj;
    public GameObject TiredObj;

    public GameObject AbilityGrid;
	public GameObject SkillGrid;
	public GameObject ItemGrid;
	public GameObject FateGrid;
	public GameObject BuffGrid;
	public GameObject PassGrid; // 被動

    public GameObject ExtListGrid; //外功列表
    public GameObject IntListGrid;

    // item obj
    public GameObject[] ItemPool;
    //public GameObject Item1Obj;
    //public GameObject Item2Obj;

    // open switch
    public GameObject btnBase;
    public GameObject btnSchool;

 //   public GameObject btnAbility;    
 //   public GameObject btnItem;


    // open base
    public GameObject sprBase;
    public GameObject sprSchool;


    public GameObject sprAbility;    
    public GameObject sprItem;

    Dictionary<GameObject, GameObject> SwitchPairPool;

    // scroll view
    public GameObject ScrollViewPass;
    public GameObject ScrollViewInt;
    public GameObject ScrollViewExt;

    // close
    public GameObject CloseBtnObj;

    public int m_nAbilityWidth = 130;
    public int m_nPassAbilityWidth = 260 ;

    private cUnitData pUnitData;

	// open info by identify
	//static public int nCharIdent;
	static public GameObject OpenUI( int nIdent )
	{
        return OpenUI(GameDataManager.Instance.GetUnitDateByIdent(nIdent) ) ;
	}

    static public GameObject OpenUI(cUnitData pData)
    {
        GameObject go = PanelManager.Instance.OpenUI(Panel_UnitInfo.Name); // set data in onenable

        Panel_UnitInfo panel = MyTool.GetPanel<Panel_UnitInfo>(go);
        if (panel != null)
        {
            panel.SetData(pData);
        }

        return go;
    }
  


	void Awake()
	{
        SwitchPairPool = new Dictionary<GameObject, GameObject>();
        SwitchPairPool.Add(btnBase, sprBase);
        SwitchPairPool.Add(btnSchool, sprSchool);
        //SwitchPairPool.Add(btnBase, sprBase );
        //SwitchPairPool.Add(btnAbility, sprAbility);
        //SwitchPairPool.Add(btnSkill, sprSkill);
        //SwitchPairPool.Add(btnItem, sprItem);

        foreach (var pair in SwitchPairPool)
        {
            UIEventListener.Get(pair.Key).onClick = OnSwitchSpriteClick; // for trig next line
        }


        UIEventListener.Get(CloseBtnObj).onClick = OnCloseClick; // for trig next line


        UIEventListener.Get(FaceObj).onClick = OnCharImgClick; // for trig next line

        UIEventListener.Get(IntSchObj).onClick = OnSchoolClick;
        UIEventListener.Get(ExtSchObj).onClick = OnSchoolClick;
    }


    public void SetData(cUnitData pData)
    {
        pUnitData = pData;
        if (pUnitData == null) {
            OnCloseClick(this.gameObject);
            return;
        }
        // set data
        int nCharId = pUnitData.n_CharID;
        //CHARS chars = data.cCharData;
        // change face	
        UITexture tex = FaceObj.GetComponent<UITexture>();
        if (tex != null)
        {
            // string url = "Art/char/" + pUnitData.cCharData.s_FILENAME + "_L";
            //Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
            //Texture t = Resources.Load(url, typeof(Texture)) as Texture;
            tex.mainTexture = MyTool.GetCharTexture(pUnitData.n_FaceID , 1);
        }


        // name 
        //string name = pUnitData.cCharData.s_NAME;
        string sName = MyTool.GetCharName(pUnitData.n_CharID);
        if (Config.GOD)
        {
            sName += ("(" + pUnitData.n_CharID + ")");
        }

        MyTool.SetLabelText(NameObj, sName );

        ReloadData();

        // base as default 
        OnSwitchSpriteClick(btnBase);

    }
	void OnEnable()
	{
        //ReloadData();
        foreach (GameObject o in ItemPool)
        {         
            Item_Unit item = o.GetComponent<Item_Unit>();
            if (item == null)
                continue;
            // 非神模式或 戰場中 不能換裝備
            item.btnEquip.SetActive( (Config.GOD == true ) || (GameDataManager.Instance.ePhase == _SAVE_PHASE._MAINTEN) ); 
        }

    }

	void OnDisable()
	{
		Panel_Tip.CloseUI (); // auto close tip

	}

	public void ReloadData()
	{
		if( pUnitData == null )
			return;

        // 更新資料
		pUnitData.UpdateAllAttr ();
		pUnitData.UpdateAttr ();
		pUnitData.UpdateBuffConditionAttr ();
        pUnitData.FixOverData();

        // 更新介面
        UpdateBase();
        // school name
        UpdateSchool();
		// Set ability
		UpdateAbility ();
		// set skill
	//	UpdateSkill ();
		
		// set item 
		UpdateItem ();
		// set fate
		//UpdateFate ();
		// set buff
		UpdateBuff ();
		//
		UpdatePass ();

        // Updateschool
        UpdateSchoolList();

    }

	// Use this for initialization
	void Start () {

        // set item obj index on all ready .aet in awake()  will be  replace index
        int idx = 0;
        foreach (GameObject o in ItemPool)
        {
        //    UIEventListener.Get(o).onClick = OnItemClick;
            Item_Unit item = o.GetComponent<Item_Unit>();
            if (item == null)
                continue;
            item.SetItemSlot(idx++ );
            //item.nIndex = idx++;
            UIEventListener.Get(item.btnEquip).onClick = OnEquipItemClick; // 
            // 非神模式 不能換裝備
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public Item_Unit GetItemObj (int nIdx) {
        if (nIdx < 0 || nIdx >= ItemPool.Length )
        {
            return null;
        }
        GameObject obj = ItemPool[nIdx];
    
        if (obj == null)
            return null;

        return obj.GetComponent<Item_Unit>();
    }
     
    public void EquipItem(int nIdx, int nItemID)
    {
        //檢查是否可以裝備
        if(pUnitData.IsTag(_UNITTAG._BLOCKITEM) )
        {
            ITEM_MISC itemData = ConstDataManager.Instance.GetRow<ITEM_MISC>(nItemID);
            if (itemData == null || (itemData.n_ITEMLV < 5))
            {
                string smsg = MyTool.GetMsgText(11);
                smsg = smsg.Replace("$V1", MyTool.GetCharName(pUnitData.n_CharID));
                Panel_CheckBox chkBox = GameSystem.OpenCheckBox();
                if (chkBox != null)
                {
                    chkBox.SetMessageCheck(smsg);
                }
                return;
            }
        }

        pUnitData.EquipItem( (_ITEMSLOT) nIdx, nItemID , true );

        // update UI
      //  GameObject obj = null;
        Item_Unit item = GetItemObj(nIdx );
        if (item != null) {
            item.SetItemID(nItemID);
        }

        pUnitData.UpdateAllAttr();
        UpdateBase();

    }

	void OnCloseClick( GameObject go )
	{
        GameSystem.BtnSound(1);
        PanelManager.Instance.CloseUI( Name );
	}

    void OnSwitchSpriteClick(GameObject go)
    {
        foreach (var pair in SwitchPairPool)
        {
            pair.Value.SetActive( (pair.Key == go) );
        }

    }
    void UpdateBase()
    {
        // lv
        MyTool.SetLabelInt(LvObj, pUnitData.n_Lv);
        // exp 
        MyTool.SetLabelInt(ExpObj, pUnitData.n_EXP);
        //tired
        MyTool.SetLabelInt(TiredObj, pUnitData.nTired);
        // mar
        MyTool.SetLabelInt(MarObj, (int)( pUnitData.GetMar() + pUnitData.nTired ));
        // HP
        int nMaxHp = pUnitData.GetMaxHP();
        MyTool.SetLabelText(HpObj, string.Format("{0}/{1}", pUnitData.n_HP, nMaxHp));
        // MP
        int nMaxMp = pUnitData.GetMaxMP();
        MyTool.SetLabelText(MpObj, string.Format("{0}/{1}", pUnitData.n_MP, nMaxMp));
        // SP
        int nMaxSp = pUnitData.GetMaxSP();
        MyTool.SetLabelText(SpObj, string.Format("{0}/{1}", pUnitData.n_SP, nMaxSp));
        // mov
        MyTool.SetLabelInt(MovObj, pUnitData.GetMov());
        // atk 
        MyTool.SetLabelInt(AtkObj, pUnitData.GetAtk());
        // def 
        int nMaxDef = pUnitData.GetMaxDef();
        // MyTool.SetLabelText(DefObj, string.Format("{0}/{1}", pUnitData.n_DEF, nMaxDef));
        MyTool.SetLabelInt(DefObj, nMaxDef);
        // pow
        MyTool.SetLabelInt(PowObj, pUnitData.GetPow());

        // 
        MyTool.SetLabelText(BrustObj, string.Format("{0}％", (pUnitData.GetMulBurst()-1.0f)*100.0f  ));
        MyTool.SetLabelText( ReduceDamageObj, string.Format("{0}％", 100.0f*(1.0f- pUnitData.GetMulDamage() ) ));
        MyTool.SetLabelFloat( ArmorObj, pUnitData.GetArmor());
        //



    }
    void UpdateSchool()
    {
        //SCHOOL inSch = GameDataManager.Instance.GetConstSchoolData( data.nActSch[0] ); // int 

        Item_School exSch = ExtSchObj.GetComponent< Item_School >();
        if (exSch != null) {
            exSch.SetData(pUnitData , pUnitData.GetExtSchID() , pUnitData.GetExtSchLv()  );
            // 如果是整備 則切換 模式
            if (GameDataManager.Instance.ePhase == _SAVE_PHASE._MAINTEN)
            {
                exSch.SetMode(1);
            }
            else { // 一般為 察看模式
                exSch.SetMode(0);
            }

        }

        Item_School InSch = IntSchObj.GetComponent<Item_School>();
        if (InSch != null)
        {
            InSch.SetData(pUnitData, pUnitData.GetIntSchID(), pUnitData.GetIntSchLv());
            // 如果是整備 則切換 模式
            if (GameDataManager.Instance.ePhase == _SAVE_PHASE._MAINTEN)
            {
                InSch.SetMode(1);
            }
            else
            { // 一般為 察看模式
                InSch.SetMode(0);
            }
        }

        //    MyTool.SetLabelText(IntSchObj, pUnitData.GetSchoolFullName(pUnitData.GetIntSchID()));

        //SCHOOL exSch = ConstDataManager.Instance.GetRow<SCHOOL>( pUnitData.nActSch[1] );//   GameDataManager.Instance.GetConstSchoolData( pUnitData.nActSch[1] ); // ext 
        //        MyTool.SetLabelText(ExtSchObj, pUnitData.GetSchoolFullName(pUnitData.GetExtSchID()));

    }
    void UpdateAbility()
	{
		int nCharlv = pUnitData.n_Lv;
        UIGrid grid = AbilityGrid.GetComponent<UIGrid>();
        MyTool.DestoryGridItem ( AbilityGrid );

		foreach (KeyValuePair< int , int > pair in pUnitData.AbilityPool ) {
			if( pair.Value > nCharlv )
				continue;
			GameObject go = ResourcesManager.CreatePrefabGameObj( AbilityGrid , "Prefab/item_ability"); 
			if( go == null )
				continue;
            //	UIEventListener.Get(go).onClick = OnAbilityClick; // 

            //UIWidget wiget = go.GetComponent<UIWidget>();
            //if (wiget != null)
            //{
            //    wiget.width = m_nAbilityWidth;
            //}

            item_ability obj = go.GetComponent<item_ability>();
			if( obj != null ){
                obj.SetSkillD(pair.Key, 0);
            }
			
		}
        //	UIGrid grid = AbilityGrid.GetComponent<UIGrid>(); 
        //	grid.repositionNow = true;		// need this for second pop to re pos
        MyTool.ResetScrollView( grid);
	}

 
    // 將來只有 update school
    //	void UpdateSkill()
    //	{
    //		//int nExtSchool = pUnitData.nActSch [cAttrData._EXTSCH];

    //		MyTool.DestoryGridItem ( SkillGrid );


    //		foreach ( int nID in pUnitData.SkillPool ) {
    //			int nSkillID = nID;
    //			nSkillID = pUnitData.Buffs.GetUpgradeSkill( nSkillID ); // Get upgrade skill

    //			SKILL skl = ConstDataManager.Instance.GetRow< SKILL > ( nSkillID );

    //			if(skl.n_SCHOOL == 0 )
    //				continue;

    //			GameObject go = ResourcesManager.CreatePrefabGameObj( SkillGrid , "Prefab/Skill_simple" ); 
    //			if( go == null )
    //				continue;

    //			UIEventListener.Get(go).onClick += OnSkillClick; // 		


    //			Skill_Simple obj = go.GetComponent<Skill_Simple >();
    //			if( obj != null ){
    //				obj.nID = nSkillID;
    //				obj.nType = 1; // 0 is ability
    //				MyTool.SetLabelText( obj.lblName , MyTool.GetSkillName( nSkillID) );
    //			}

    //		}
    ////		UIGrid grid = SkillGrid.GetComponent<UIGrid>(); 
    ////		grid.repositionNow = true;		// need this for second pop to re pos


    //	}
    void UpdateItem()
	{

        // item obj
        int nIdx = 0;
        foreach ( GameObject obj in ItemPool ) {
            Item_Unit item = obj.GetComponent<Item_Unit>();
            if (item == null)
                continue;

            item.SetItemID(pUnitData.Items[nIdx++]);

        }

      

                                                                        //item1.nType = 2; // 2 is item



        //      int nIdx = 0;
        //MyTool.DestoryGridItem ( ItemGrid );
        //foreach (int itemid in pUnitData.Items) {
        //          // create contain for null item too
        //	//if( itemid <= 0 )
        //	//	continue;

        //	GameObject go = ResourcesManager.CreatePrefabGameObj( ItemGrid , "Prefab/Skill_simple" ); 
        //	if( go == null )
        //		continue;

        //	UIEventListener.Get(go).onClick = OnItemClick; // 


        //	Skill_Simple obj = go.GetComponent<Skill_Simple >();
        //	if( obj != null ){
        //              obj.nIndex = nIdx++;
        //              obj.nID = itemid;
        //		obj.nType = 2; // 2 is item
        //		MyTool.SetLabelText( obj.lblName , MyTool.GetItemName( itemid ) );
        //	}
        //	//ITEM_MISC item = ConstDataManager.
        //}

        //		int item0 = pUnitData.Items[ (int)_ITEMSLOT._SLOT0  ]; 
        //		int item1 = pUnitData.Items[ (int)_ITEMSLOT._SLOT1  ]; 



    }

    //沒有buff
	void UpdateBuff()
	{
        if (BuffGrid == null)
            return;
        UIGrid grid = BuffGrid.GetComponent<UIGrid>();
        MyTool.DestoryGridItem (grid);

		foreach ( KeyValuePair< int , cBuffData > pair in pUnitData.Buffs.Pool ) {
			if( pair.Value.nTime == 0 ) // never 0  
				continue;
			if( pair.Value.tableData.n_HIDE > 0 )
				continue;

			GameObject go = ResourcesManager.CreatePrefabGameObj( BuffGrid , "Prefab/Bufficon" ); 
			if( go == null )
				continue;

            MyTool.SetBuffIcon(go, pair.Value.nID, pair.Value.nTime, pair.Value.nNum);
   //         BuffIcon icon = go.GetComponent< BuffIcon >();
			//if( icon != null )
			//	icon.SetBuffData( pair.Value.nID , pair.Value.nTime ,  pair.Value.nNum  );
		//	UIEventListener.Get(go).onClick += OnBuffClick; // 

		}
        MyTool.ResetScrollView( grid );

	}

	void UpdatePass ()
	{
        UIGrid grid = PassGrid.GetComponent<UIGrid>();

        MyTool.DestoryGridItem ( PassGrid );

		foreach ( KeyValuePair< int , cBuffData > pair in pUnitData.Buffs.Pool ) {
			if( pair.Value.nTime != 0 ) // only 0  
				continue;
			if( pair.Value.tableData.n_HIDE > 0 )
				continue;

			GameObject go = ResourcesManager.CreatePrefabGameObj( PassGrid , "Prefab/Skill_simple" ); 
			if( go == null )
				continue;

            UIWidget wiget = go.GetComponent<UIWidget>();
            if (wiget != null)
            {
                wiget.width = m_nPassAbilityWidth; // set width
            }


            UIDragScrollView dsv = this.GetComponent<UIDragScrollView>();
            if (dsv != null)
            {
                dsv.scrollView =  ScrollViewPass.GetComponent<UIScrollView>();
            }

          //  UIEventListener.Get(go).onClick = OnBuffClick; // this is a buff

			Skill_Simple obj = go.GetComponent<Skill_Simple >();
			if( obj != null ){
                obj.SetSkillD(pair.Value.nID , 1 ); // 1 is buff
                //obj.nID = pair.Value.nID;
                //obj.nType = 0; // 0 is ability
                //MyTool.SetLabelText( obj.lblName , MyTool.GetBuffName (obj.nID ) ); // this is buff name
            }			
		}

        MyTool.ResetScrollView(grid);

    }

    // School List
    void UpdateSchoolList()
    {
        MyTool.DestoryGridItem(ExtListGrid);
        MyTool.DestoryGridItem(IntListGrid);

       // UIScrollView esv = ScrollViewExt.GetComponent<UIScrollView>();
       // UIScrollView isv = ScrollViewInt.GetComponent<UIScrollView>();


        foreach (KeyValuePair<int, int> pair in pUnitData.SchoolPool )
        {
            
            int schid = pair.Key;
            int schlv = pair.Value;
            SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(schid);   //GameDataManager.Instance.GetConstSchoolData ( nSchool );
            if (sch.n_TYPE == 0) // 內功
            {
                GameObject go = ResourcesManager.CreatePrefabGameObj(IntListGrid, "Prefab/Item_School");
                if (go == null)
                    continue;
                Item_School obj = go.GetComponent<Item_School>();
                if (obj != null)
                {
                    obj.SetData(pUnitData,schid, schlv);
                }
                obj.SetScrollView(ScrollViewInt );

                UIEventListener.Get(go).onClick += OnSchoolClick; 
            }
            else
            {          // 外功
                GameObject go = ResourcesManager.CreatePrefabGameObj(ExtListGrid, "Prefab/Item_School");
                if (go == null)
                    continue;
                Item_School obj = go.GetComponent<Item_School>();
                if (obj != null)
                {
                    obj.SetData(pUnitData,schid, schlv);
                }
                obj.SetScrollView(ScrollViewExt);

                UIEventListener.Get(go).onClick += OnSchoolClick;
            }
        }

        MyTool.ResetScrollView(ScrollViewExt );
        MyTool.ResetScrollView(ScrollViewInt );
    }

    void OnSchoolClick(GameObject go)
    {
        Item_School obj = go.GetComponent<Item_School>();
        if (obj != null)
        {
            GameSystem.BtnSound();
            Panel_Skill.OpenSchoolUI( pUnitData, _SKILL_TYPE._SCHOOL, obj.nSchID  );
        }
    }

    // onclick event
    void OnAbilityClick( GameObject go )
	{
		Skill_Simple obj = go.GetComponent<Skill_Simple >();
		if (obj != null) {           
            Panel_Tip.OpenSkillTip( obj.nID );
			//Panel_Tip.OpenUI( MyTool.GetSkillName( obj.nID )   ); 
		}
		
	}
	void OnSkillClick( GameObject go )
	{		
		Skill_Simple obj = go.GetComponent<Skill_Simple >();
		if (obj != null) {
			Panel_Tip.OpenSkillTip( obj.nID );
			//Panel_Tip.OpenUI( MyTool.GetSkillName( obj.nID )   ); 
		}
	}

	void OnItemClick( GameObject go )
	{
        Item_Unit obj = go.GetComponent<Item_Unit>();
		if (obj != null) {
            Panel_Tip.OpenItemTip( obj.nID );
         //   Panel_Tip.OpenUI( MyTool.GetSkillName( obj.nID )   ); 
            // open item list
           //Panel_ItemList.Open(1 , obj.nIndex );           
		}
	}

    void OnEquipItemClick(GameObject go)
    {
        Item_Unit obj = go.GetComponentInParent<Item_Unit>();
        if (obj != null) {
            GameSystem.BtnSound();
            Panel_ItemList.Open(1, obj.nIndex);
        }
    }

    void OnBuffClick( GameObject go )
	{
		Skill_Simple obj = go.GetComponent<Skill_Simple >();
		if (obj != null) {
			Panel_Tip.OpenBuffTip( obj.nID );
			//Panel_Tip.OpenUI( MyTool.GetSkillName( obj.nID )   ); 
		}
	}

    void OnCharImgClick(GameObject go)
    {
        Panel_FullCharImage panel = MyTool.GetPanel< Panel_FullCharImage  >(PanelManager.Instance.OpenUI(Panel_FullCharImage.Name) );
        if (panel != null)
        {
            panel.SetFace( pUnitData.n_FaceID );
        }
    }
    

}


