using UnityEngine;
using System.Collections;

public class Panel_Enhance : MonoBehaviour
{
    public const string Name = "Panel_Enhance";

    public GameObject lblName;
    public GameObject FaceObj;

    public GameObject BtnReset;
    public GameObject BtnClose;
    public GameObject BtnOk;

    public enhance_school_item school_ext_item;
    public enhance_school_item school_int_item;
    public item_param[] lbl_param_list;
    public enhance_param_item [] enhance_param_list;


    public GameObject lblRank;
    public GameObject lblLimit;
    //  public GameObject GridExt; // GRID obj
    //  public GameObject GridInt;

    //    public GameObject SchItemObj;  // school item

    //public GameObject lblOrgMar;
    //public GameObject lblOrgHp;
    //public GameObject lblOrgMp;
    //public GameObject lblOrgAtk;
    //public GameObject lblOrgDef;
    //public GameObject lblOrgPow;
    //public GameObject lblOrgMov;
    //public GameObject lblOrgBrust;
    //public GameObject lblOrgReduce;
    //public GameObject lblOrgArmor;

    //public GameObject lblAddMar;
    //public GameObject lblAddHp;
    //public GameObject lblAddMp;
    //public GameObject lblAddAtk;
    //public GameObject lblAddDef;
    //public GameObject lblAddPow;
    //public GameObject lblAddMov;
    //public GameObject lblAddBrust;
    //public GameObject lblAddReduce;
    //public GameObject lblAddArmor;


    public GameObject lblTotMoney;
    public GameObject lblCostMoney;

//    public int nOpSchType;          // 操作的武學

    // 裝備道具
  //  public GameObject[] ItemPool;


    public int nOpCharID=0;

    public cUnitData pOrgData;
    public cUnitData pTmpData;


  //  public int nCostExt=0;
  //  public int nCostInt=0;


    //public int nCostMoney=0;


    // 以下將放棄
    //UIScrollView ExtScrollView;
    //UIScrollView IntScrollView;
    //UIGrid       ExtGrid;
    //UIGrid       IntGrid;




    // Use this for initialization
    void Awake()
    {

        UIEventListener.Get(BtnReset).onClick = OnResetClick;
        UIEventListener.Get(BtnClose).onClick = OnCloseClick;
        UIEventListener.Get(BtnOk).onClick = OnOkClick;

        //ExtScrollView = ExtObj.GetComponent<UIScrollView>();
        //IntScrollView = IntObj.GetComponent<UIScrollView>();

        //ExtGrid = ExtObj.GetComponentInChildren<UIGrid>(); 
        //IntGrid = IntObj.GetComponentInChildren<UIGrid>();


     //   svSchList = sprSchList.GetComponent<UIScrollView>();
     //   gridSchList = sprSchList.GetComponentInChildren<UIGrid>();


     //   UIEventListener.Get(btnChangeExt).onClick = OnChangeExtClick;
     //   UIEventListener.Get(btnChangeInt).onClick = OnChangeIntClick;

     //   UIEventListener.Get(btnAddExt).onClick = OnExtSchAddClick;
     //   UIEventListener.Get(btnAddInt).onClick = OnIntSchAddClick;


    //    SchItemObj.CreatePool(4);
    //    SchItemObj.SetActive(false);

        pTmpData = new cUnitData();             // new for temp
      
        // 道具裝備相關
        //int idx = 0;
        //foreach (GameObject o in ItemPool)




    }
    // Use this for initialization
    void Start()
    {
        // 在 awake 後 設定 index
        //for (int idx = 0; idx < ItemPool.Length; idx++)
        //{
        //    GameObject o = ItemPool[idx];
        //    Item_Unit item = o.GetComponent<Item_Unit>();
        //    if (item != null)
        //    {
        //        item.SetItemSlot(idx);
        //        item.SetItemCallBack(OnSetItemEquipClick);
        //    }
        //}

    }

    // Update is called once per frame
    void Update()
    {
        UpdateAttr();

        MyTool.SetLabelInt(lblTotMoney, GameDataManager.Instance.nMoney);
        MyTool.SetLabelInt(lblCostMoney, ( GetSumCostPrice() ) );

    }

    public void SetData(cUnitData pData)
    {
        pOrgData = pData;
        pTmpData.Copy(pOrgData); // 複製資料
                                 //  pTmpData.eCampID = _CAMP._FRIEND; // 變更成 友方陣營 以避免 道具穿戴會影響到 背包

        UITexture tex = FaceObj.GetComponent<UITexture>();
        if (tex != null)
        {
            tex.mainTexture = MyTool.GetCharTexture(pOrgData.n_FaceID);
        }

        // school
        school_ext_item.LinkUnit( pTmpData );
        school_int_item.LinkUnit( pTmpData );

        foreach ( enhance_param_item item in enhance_param_list )
        {
            item.LinkUnit( pTmpData ); // 連接 操作資料
        }



        ResetAll();
        Reload();
    }
    public void ResetAll()
    {
        if (pOrgData != null)
        {
            int nTmpExtId = pTmpData.GetExtSchID();
            int nTmpIntId = pTmpData.GetIntSchID();

            //改為等級重置
            //  pTmpData.Copy(pOrgData); // 道具怎麼辦？
            school_int_item.ReSet();
            school_ext_item.ReSet();

            //if (nTmpExtId > 0)
            //{
            //    int OrgExtLv = pOrgData.GetSchoolLv(nTmpExtId);
            //    pTmpData.LearnSchool(nTmpExtId, OrgExtLv);
            //    // pTmpData.ActiveSchool(nTmpExtId);
            //}

            //if (nTmpIntId > 0)
            //{
            //    int OrgIntLv = pOrgData.GetSchoolLv(nTmpIntId);
            //    // pTmpData.ActiveSchool(nTmpIntId);
            //    pTmpData.LearnSchool(nTmpIntId, OrgIntLv);
            //}
            // 重置 強化

            //foreach (  ) {

            //}
            //pOrgData.EnhancePool;
            foreach (enhance_param_item item in enhance_param_list)
            {
                item.ReSet(); // 更新介面
            }

        }

        ZeroCostPrice();

    }
   
    // Uidate UI
    public void Reload()
    {
        // 確認關卡修練上限
        STAGE_DATA stage = ConstDataManager.Instance.GetRow<STAGE_DATA>(GameDataManager.Instance.nStageID);
        if (stage != null)
        {            
            MyTool.SetLabelInt(lblLimit, stage.n_ENHANCE_LIMIT);
        }
       
        //    sprSchList.SetActive(false);
        if (pTmpData != null)
        {
            // remove item?
            pTmpData.UpdateAllAttr();
            pTmpData.UpdateAttr();
            pTmpData.UpdateBuffConditionAttr();
            pTmpData.UpdateBuffConditionEffect();

            MyTool.SetLabelInt(lblRank, pTmpData.cCharData.n_RANK);

        }

        // create school data item
        // SchItemObj.RecycleAll();

        // Set School Data
        //EnhanceItem exItem = ExtSchItemObj.GetComponent<EnhanceItem>();// 外功school item
        //EnhanceItem inItem = IntSchItemObj.GetComponent<EnhanceItem>();// 外功school item
        //if (exItem != null)
        //{
        //    exItem.SetData(pTmpData.GetExtSchID(), pTmpData.GetExtSchLv());
        //}
        //if (inItem != null)
        //{
        //    inItem.SetData(pTmpData.GetIntSchID(), pTmpData.GetIntSchLv());
        //}
        school_int_item.ReLoad();
        school_ext_item.ReLoad();


        foreach (enhance_param_item item in enhance_param_list)
        {
            item.ReLoad(); // 更新介面
        }

        //
        //for (int idx = 0; idx < ItemPool.Length; idx++)
        //{
        //    GameObject o = ItemPool[idx];
        //    Item_Unit item = o.GetComponent<Item_Unit>();
        //    if (item != null && idx < pTmpData.Items.Length)
        //    {
        //        item.SetItemID(pTmpData.Items[idx]);

        //    }
        //}

        // clear
        //while (ExtGrid.transform.childCount > 0)
        //{

        //    DestroyImmediate(ExtGrid.transform.GetChild(0).gameObject);
        //}
        //while (IntGrid.transform.childCount > 0)
        //{
        //    DestroyImmediate(IntGrid.transform.GetChild(0).gameObject);
        //}

        //foreach ( var pair in pTmpData.SchoolPool )
        //{
        //    SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(pair.Key);
        //    if (sch == null)                continue;

        //    GameObject obj = null;
        //    if (sch.n_TYPE==1)            {// ext
        //        obj = SchItemObj.Spawn(ExtGrid.transform);
        //        UIDragScrollView drag = obj.GetComponent<UIDragScrollView>();
        //        if (drag != null)
        //        {
        //            drag.scrollView = ExtScrollView;
        //        }
        //    } 
        //    else {
        //        obj = SchItemObj.Spawn(IntGrid.transform);
        //        UIDragScrollView drag = obj.GetComponent<UIDragScrollView>();
        //        if (drag != null)
        //        {
        //            drag.scrollView = IntScrollView;
        //        }

        //    }                // int
        //    if (obj == null)
        //        continue;

        //    EnhanceItem enItem = obj.GetComponent<EnhanceItem>();
        //    if (enItem != null) {
        //        enItem.ReSize();
        //        //UIEventListener.Get(enItem.btnAdd).onClick = null;
        //        UIEventListener.Get(enItem.btnAdd).onClick = OnSchLvAddClick;
        //        if (sch.n_TYPE == 1)
        //        {
        //            enItem.SetData(pair.Key , pair.Value );
        //        }
        //        else {
        //            enItem.SetData(pair.Key, pair.Value);
        //        }
        //    }
        //}

        //// re position
        //ExtGrid.repositionNow = true ;
        //IntGrid.repositionNow = true;
        //// show active school
        //if(ExtScrollView != null)
        //    ExtScrollView.ResetPosition();
        //if(IntScrollView != null)
        //    IntScrollView.ResetPosition();
        //MyTool.ResetScrollView(ExtScrollView);
        //MyTool.ResetScrollView(IntScrollView);
    }

    // 
    void UpdateAttr()
    {
        if (pTmpData == null)
            return;

        //        pTmpData.UpdateAllAttr();
        //        pTmpData.UpdateAttr();
        //        pTmpData.UpdateBuffConditionAttr();
        //        pTmpData.UpdateBuffConditionEffect();

        // school buff affect attr too
        //cAttrData intAttr = pTmpData.GetAttrData( cAttrData._INTSCH );
        //cAttrData extAttr = pTmpData.GetAttrData( cAttrData._EXTSCH );

        //if (intAttr == null || extAttr == null )
        //    return;
        pTmpData.Relive();
        
        foreach (item_param item in lbl_param_list )
        {
            item.SetUnit(pTmpData, 1); // 更新資訊
        }

        //MyTool.SetLabelInt(lblOrgMar, (int)pTmpData.GetMar());
        //MyTool.SetLabelInt(lblOrgHp, pTmpData.GetMaxHP());
        //MyTool.SetLabelInt(lblOrgMp, pTmpData.GetMaxMP());
        //MyTool.SetLabelInt(lblOrgAtk, pTmpData.GetAtk());
        //MyTool.SetLabelInt(lblOrgDef, pTmpData.GetMaxDef());
        //MyTool.SetLabelInt(lblOrgPow, pTmpData.GetPow());
        //MyTool.SetLabelInt(lblOrgMov, pTmpData.GetMov());

        //MyTool.SetLabelText(lblOrgBrust, string.Format("{0}％", (pTmpData.GetMulBurst() - 1.0f) * 100.0f));
        //MyTool.SetLabelText(lblOrgReduce, string.Format("{0}％", 100.0f * (1.0f - pTmpData.GetMulDamage())));
        //MyTool.SetLabelFloat(lblOrgArmor, pTmpData.GetArmor());

    }

    void UpdateAdd()
    {

    }
    public void OnChangeIntClick(GameObject go)
    {
        ResetAll();
        Reload();
        Panel_SchoolList.Open(2, pTmpData, 0);
        // PopSchList(0);
    }
    public void OnChangeExtClick(GameObject go)
    {
        // 放棄 當前設定
        ResetAll();
        Reload();
        Panel_SchoolList.Open(2, pTmpData ,1);
        //PopSchList(1);
    }
  

    //public void PopSchList(int nType)
    //{
    //    nOpSchType = nType;
    //    sprSchList.SetActive(true); // 顯示

    //    SchItemObj.RecycleAll();

    //    // clear
    //    while (gridSchList.transform.childCount > 0)
    //    {
    //        DestroyImmediate(gridSchList.transform.GetChild(0).gameObject);
    //    }
    //    //
    //    foreach (var pair in pTmpData.SchoolPool)
    //    {
    //        SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(pair.Key);
    //        if (sch == null) continue;

    //        GameObject obj = null;
    //        if (sch.n_TYPE == nType)
    //        {// ext
    //            obj = SchItemObj.Spawn(gridSchList.transform);
    //            UIDragScrollView drag = obj.GetComponent<UIDragScrollView>();
    //            if (drag != null)
    //            {
    //                drag.scrollView = svSchList;
    //            }
    //        }
    //        if (obj == null)
    //            continue;

    //        EnhanceItem enItem = obj.GetComponent<EnhanceItem>();
    //        if (enItem != null)
    //        {
    //            enItem.ReSize();
    //            //UIEventListener.Get(enItem.btnAdd).onClick = null;
    //            UIEventListener.Get(enItem.gameObject).onClick = OnChangeItemClick;
    //            enItem.SetData(pair.Key, pair.Value);
    //            //if (sch.n_TYPE == 1)
    //            //{
    //            //    enItem.SetData(pair.Key, pair.Value);
    //            //}
    //            //else
    //            //{
    //            //    enItem.SetData(pair.Key, pair.Value);
    //            //}
    //        }
    //    }

    //    //
    //    gridSchList.repositionNow = true;
    //    // show active school
    //    if (svSchList != null)
    //    {
    //        svSchList.ResetPosition();
    //    }
    //}

    // 確定更換
    public void OnChangeItemClick(GameObject go)
    {
        //EnhanceItem enItem = go.GetComponent<EnhanceItem>();
        //if (enItem != null)
        //{
        //    // 異動武學
        //    if (enItem.nSchoolID > 0)
        //    {
        //        pTmpData.ActiveSchool(enItem.nSchoolID);
        //        Reload();       // 參數重讀      
        //        // 原始資料的使用技能也更動
        //        pOrgData.ActiveSchool(enItem.nSchoolID);
        //    }
        //}
        //sprSchList.SetActive(false); // 關閉
    }
    public void OnResetClick(GameObject go)
    {
        GameSystem.BtnSound(2);
        ResetAll();        
        Reload();       // 參數重讀                
    }
    public void OnOkClick(GameObject go)
    {
        int nsum = GetSumCostPrice();

        if (nsum == 0) {
            GameSystem.BtnSound(2);
            return; // 表示沒有強化
        }


        if (GameDataManager.Instance.nMoney < nsum )
        {
            // message Money not enough
            //Panel_CheckBox.           
            GameSystem.BtnSound(2);
            return;
        }

        //比較兩邊武學，並把較低的設定進去
        //pTmpData.SchoolPool.Clear();
        foreach (var pair in pTmpData.SchoolPool)
        {
            int nSchID = pair.Key;
            int nSchLV = pair.Value;

            int nOldLV = pOrgData.GetSchoolLv(nSchID);
            if (nOldLV != nSchLV)
            {
                pOrgData.LearnSchool(nSchID, nSchLV);
            }
            //pTmpData.SchoolPool.Add(pair.Key, pair.Value);
        }

        // 設定 強化 參數
        foreach (enhance_param_item item in enhance_param_list)
        {
            pOrgData.SetEnhanceLv( item.m_eParamIdx , item.m_nCurLv );
            // 重設 link
            item.LinkUnit( pTmpData );
        }


        // 道具怎麼辦？

        // store to char

        UpdateAttr();

        pOrgData.Relive();
       

        GameDataManager.Instance.nSpendMoney += nsum; // 消費紀錄起來

        GameDataManager.Instance.nMoney -= nsum;
        
        ZeroCostPrice();
        //nCostMoney = 0;

        //main ten UI  要reload unit list        
        //Panel_Mainten panel = MyTool.GetPanel<Panel_Mainten>(PanelManager.Instance.OpenUI(Panel_Mainten.Name));
        //if (panel != null)
        //{
        //    panel.ReloadUnitList();
        //}
        Reload(); // refresh UI
        // 播放音效
        GameSystem.PlaySound("Audios 00050");
    }
    public void OnCloseClick(GameObject go)
    {
        // update mainten ui
        if (PanelManager.Instance.CheckUIIsOpening(Panel_Mainten.Name))
        {
            Panel_Mainten p = MyTool.GetPanel<Panel_Mainten>(Panel_Mainten.Name);
            if (p != null)
            {
                p.ReloadUnitList();
            }
        }

        GameSystem.BtnSound(1);

        PanelManager.Instance.CloseUI(Name);
    }

    // On Add School
    //public void OnIntSchAddClick(GameObject go)
    //{
    //    int nSchID = pTmpData.GetIntSchID();
    //    int nLv = pTmpData.GetIntSchLv();
    //    OnSchLvAddClick(nSchID, nLv, 0);
    //    Reload();
    //}

    //public void OnExtSchAddClick(GameObject go)
    //{
    //    int nSchID = pTmpData.GetExtSchID();
    //    int nLv = pTmpData.GetExtSchLv();        // 計算 金錢


    //    OnSchLvAddClick(nSchID, nLv , 1);
    //    Reload();

    //}
    


    //public void OnSchLvAddClick(GameObject go)
    //public void OnSchLvAddClick(int nSchID, int nLv , int nSchType )
    //{
    //    //EnhanceItem enItem = go.GetComponentInParent <EnhanceItem> ();
    //    //if (enItem != null)
    //    //{
    //    //  int nSchID = enItem.nSchoolID;
    //    //  int nLv = enItem.nLv  ;

    //    SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(nSchID);
    //    if (sch == null)
    //        return;
    //    // check max lv
    //    if (nLv >= sch.n_MAXLV)
    //        return;

    //    //float fRate = Mathf.Pow(1.5f, nLv);
    //    float fRate = (nLv-1)* 2.0f ;  // 一顆星 2000

    //    int nCost = (int)(Config.LevelUPMoney * sch.f_RANK * fRate); // 用當前等級算才不會太高
    //     //nCostMoney += nCost;


    //    nLv++;

    //    pTmpData.LearnSchool(nSchID, nLv);
    //    //   enItem.SetData( nSchID, nLv);
    //    // update lab value
    //    pTmpData.ActiveSchool(nSchID);

    //    UpdateAttr();

    //    GameSystem.PlaySound( 170 );
    //    // }
    //}

    //public Item_Unit GetItemObj(int nIdx)
    //{
    //    if (nIdx < 0 || nIdx >= ItemPool.Length)
    //    {
    //        return null;
    //    }
    //    GameObject obj = ItemPool[nIdx];

    //    if (obj == null)
    //        return null;

    //    return obj.GetComponent<Item_Unit>();
    //}

    //// 變更裝備
    //public void OnSetItemEquipClick(GameObject go)
    //{
    //    Item_Unit item = go.transform.parent.GetComponent<Item_Unit>();
    //    if (item != null)
    //    {
    //        // open item list
    //        Panel_ItemList.Open(2, item.nIndex);

    //    }
    //}

    //public void EquipItem(int nIdx, int nItemID)
    //{
    //    //檢查是否可以裝備
    //    if (pTmpData.IsTag(_UNITTAG._BLOCKITEM))
    //    {
    //        ITEM_MISC itemData = ConstDataManager.Instance.GetRow<ITEM_MISC>(nItemID);
    //        if (itemData == null || (itemData.n_ITEMLV < 5))
    //        {
    //            string smsg = MyTool.GetMsgText(11);
    //            smsg = smsg.Replace("$V1", MyTool.GetCharName(pTmpData.n_CharID));
    //            Panel_CheckBox chkBox = GameSystem.OpenCheckBox();
    //            if (chkBox != null)
    //            {
    //                chkBox.SetMessageCheck(smsg);
    //            }
    //            return;
    //        }
    //    }

    //    // 同步 穿戴裝備
    //    pTmpData.EquipItem((_ITEMSLOT)nIdx, nItemID, false); // 不要同步背包
    //    pOrgData.EquipItem((_ITEMSLOT)nIdx, nItemID, true);
    //    // update UI
    //    //  GameObject obj = null;
    //    Item_Unit item = GetItemObj(nIdx); // 變更裝備顯示
    //    if (item != null)
    //    {
    //        item.SetItemID(nItemID);
    //    }
    //    pTmpData.UpdateAllAttr();
    //    //pTmpData();
    //    Reload();
    //}

    public void EquipSchool(int nSchoolID)
    {
        if (nSchoolID <= 0)
            return;

        pTmpData.ActiveSchool(nSchoolID);
        pTmpData.UpdateAllAttr();
        Reload();
    }

    public int  GetSumCostPrice()
    {
        int sum = school_int_item.m_nCost + school_ext_item.m_nCost;        

        foreach (enhance_param_item item in enhance_param_list)
        {
            sum += item.m_nCost;
        }          

        //MyTool.SetLabelInt(lblCostMoney , nTotal);
        return sum;
    }
    public void ZeroCostPrice()
    {   
        school_int_item.m_nCost = school_ext_item.m_nCost=0;
        foreach (enhance_param_item item in enhance_param_list)
        {
            item.m_nCost=0;
        }

    }
    
}