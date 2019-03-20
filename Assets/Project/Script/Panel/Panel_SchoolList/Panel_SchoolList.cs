using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel_SchoolList : MonoBehaviour {

    public const string Name = "Panel_SchoolList";


    public GameObject GridObj;
    //public GameObject sprSchList;
    public UIScrollView svSchList;
    public GameObject ItemObj;
    public GameObject SelectSchObj;
    public GameObject ContentObj;

    public int nMode; // 0 - 察看, 1- 換武功 , 2- 強化
    public int nVar1;  // id
    public int nVar2;  // lv
    public int nVar3;  // sch type
    public int nVar4;

    public cUnitData m_pUnitData;

  //  public int nSelectID;

    // Use this for initialization
    void Awake()
    {
        ItemObj.CreatePool(3);
        ItemObj.SetActive(false);

        MyTool.SetLabelText(ContentObj, "");
    }

    void Start () {
       // svSchList = sprSchList.GetComponent<UIScrollView>();
        SelectSchObj.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    // mode =0 切換 內功， 1- 切換外功
    static public Panel_SchoolList Open(int mode, cUnitData pData , int var1 = 0, int var2 = 0, int var3 = 0, int var4 = 0)
    {
        Panel_SchoolList pList = MyTool.GetPanel<Panel_SchoolList>(PanelManager.Instance.OpenUI(Name));
        if (pList != null)
        {
            pList.SetMode(mode, pData ,  var1, var2, var3, var4);
            pList.ReloadList();
        }
        return pList;
    }

    public void SetMode(int mode, cUnitData pData, int var1 = 0, int var2 = 0, int var3 = 0, int var4 = 0)
    {
        nMode = mode;
        m_pUnitData = pData;
        nVar1 = var1; // schoool type
        nVar2 = var2;
        nVar3 = var3;
        nVar4 = var4;
     //   nSelectID = 0;
    }


    public void ReloadList()
    {
        //reopen 後會需要重抓
      //  svSchList = sprSchList.GetComponent<UIScrollView>();
        // create school data item
        ItemObj.RecycleAll();
        // clear
        //while (GridObj.transform.childCount > 0)
        //{
        //    DestroyImmediate(GridObj.transform.GetChild(0).gameObject);
        //}
        UIGrid grid = GridObj.GetComponent<UIGrid>();
        if (grid != null)
        {
            grid.repositionNow = true;
        }
        MyTool.DestoryGridItem( grid );
        // create list
        if (m_pUnitData == null)
            return;

        int nCount=0;
        foreach ( KeyValuePair< int , int > pair in m_pUnitData.SchoolPool)
        {
            int schoolid = pair.Key;
            int lv = pair.Value;
            SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(schoolid);
            if (sch == null) {
                continue;
            }
            // 不是指定的 sch id
            //if (nVar1 > 0 && (nVar1!= schoolid) )
            //{
            //    continue;
            //}
            // 不用管等級

            // 不是指定的 sch type
            if ( (nVar3>=0) &&  (nVar3!=sch.n_TYPE) )
            {
                continue;
            }
            //// int 
            //if (nMode == 0)
            //{
            //    if (sch.n_TYPE == 1) {
            //        continue;
            //    }
            //}
            //// ext
            //else if (nMode == 1) {
            //    if (sch.n_TYPE == 0)                {
            //        continue;
            //    }
            //}

            GameObject obj = ItemObj.Spawn(GridObj.transform);
            if (obj != null)
            {
                obj.SetActive(true); // 由於 CharUnit 被 disable。所以copy 出來的會變成disable 。需手動 active
                UIDragScrollView drag = obj.GetComponent<UIDragScrollView>();
                if (drag != null)
                {
                    drag.scrollView = svSchList;
                }

                Item_school_detail item = obj.GetComponent<Item_school_detail>();
                if (item != null)
                {
                    item.ReSize();
                    item.SetData(schoolid , lv , m_pUnitData);

                   // item.ShowDiff();
                    //// 判斷是否為 裝備中武學
                    //if (pUnitData != null) {
                    //    if (schoolid == pUnitData.GetSchIDbyType(sch.n_TYPE)) // 本次的內外功類型
                    //    {
                    //        item.SetChecked(true);
                    //    }
                    //    else {
                    //        item.SetChecked(false);
                    //    }
                    //}

                    //  UIEventListener.Get(obj).onClick = OnItemClick; //
                    UIEventListener.Get(obj).onClick = OnOKClick;
                }
                nCount++;
            }
        }

        // show active school
        MyTool.ResetScrollView(grid);
      

        MyTool.SetLabelText(ContentObj, "");

        if (nCount <= 0) {
            // close
        }

    }

    public void OnCloseClick(GameObject go)
    {
        PanelManager.Instance.CloseUI(Name);
        GameSystem.BtnSound(1);
    }

    public void OnOKClick(GameObject go)
    {
        Item_school_detail item = go.GetComponent<Item_school_detail>();
        int schid = item.m_nSchId;
        switch (nMode)
        {
            case 0:// sheck skill
                {
                    if (MyTool.GetSkillNumBySchool(schid) > 0)
                    {
                        GameSystem.BtnSound(0);
                        Panel_Skill.OpenSchoolUI(m_pUnitData, _SKILL_TYPE._SCHOOL, schid);
                        PanelManager.Instance.CloseUI(Name); // change school
                    }
                    else {
                        GameSystem.BtnSound(2);
                        Panel_Tip.OpenUI("本武學無特殊招式");
                    }
                    
                }

                break;
            case 1: // equip
                {
                    if (schid > 0 && (m_pUnitData != null))
                    {

                        m_pUnitData.ActiveSchool(schid);
                        m_pUnitData.n_CP = 0 ; // CP 歸零
                        GameSystem.PlaySound(201);
                    }
                    // 有命令UI 要 關閉
                    if (PanelManager.Instance.CheckUIIsOpening(Panel_CMDUnitUI.Name))
                    {
                        Panel_CMDUnitUI.CancelCmd(); // 要關命令
                        //Panel_CMDUnitUI panel = MyTool.GetPanel<Panel_CMDUnitUI>(Panel_CMDUnitUI.Name); //PanelManager.Instance.OpenUI( Panel_UnitInfo.Name );
                        //if (panel != null)
                        //{
                           
                        //}
                    }
                    // 有資訊UI 要 更新
                    if (PanelManager.Instance.CheckUIIsOpening(Panel_UnitInfo.Name))
                    {

                        Panel_UnitInfo panel = MyTool.GetPanel<Panel_UnitInfo>(Panel_UnitInfo.Name); //PanelManager.Instance.OpenUI( Panel_UnitInfo.Name );
                        if (panel != null)
                        {
                            panel.ReloadData();
                            //panel.EquipItem(nVar1, itemid); // syn bag
                        }
                    }
                    // 整備時 需reload list
                    if (PanelManager.Instance.CheckUIIsOpening(Panel_Mainten.Name))
                    {

                        Panel_Mainten panel = MyTool.GetPanel<Panel_Mainten>(Panel_Mainten.Name); 
                        if (panel != null)
                        {
                            panel.ReloadUnitList();
                            //panel.EquipItem(nVar1, itemid); // syn bag
                        }
                    }



                }
                PanelManager.Instance.CloseUI(Name); // close school
                break;
            case 2: // enhance
                {
                    if (PanelManager.Instance.CheckUIIsOpening(Panel_Enhance.Name))
                    {
                        GameSystem.BtnSound();
                        Panel_Enhance panel = MyTool.GetPanel<Panel_Enhance>(Panel_Enhance.Name); //PanelManager.Instance.OpenUI( Panel_UnitInfo.Name );
                        if (panel != null)
                        {
                             panel.EquipSchool(schid); // syn bag
                        }

                    }
                    PanelManager.Instance.CloseUI(Name); // change school
                };
                break;
            default:
                {
                    GameSystem.BtnSound();
                    PanelManager.Instance.CloseUI(Name); // change school
                                                         // Panel_Tip.OpenItemTip(itemid);
                                                         //    Panel_Tip.OpenUI(MyTool.GetSkillName(itemid));
                }

                break;
        }

      
    }


    public void OnItemClick(GameObject go)
    {
        //ItemList_School item = go.GetComponent<ItemList_School>();
        Item_School item = go.GetComponent<Item_School>();
        if (item == null)
            return;

    //    nSelectID = item.nSchID;

        //Panel_TipOpenItemTip(itemid);
        //Panel_Tip.OpenUI(MyTool.GetSkillName(itemid));

        ;
     //   MyTool.SetLabelText(ContentObj, MyTool.GetSchoolName(nSelectID) );
    }

}
