using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Panel_ItemList : MonoBehaviour
{
    public const string Name = "Panel_ItemList";


    public GameObject GridObj;
    public GameObject ItemObj;
    public UISlider m_SB_Ver;

    public int nMode;
    public int nVar1;
    public int nVar2;
    public int nVar3;
    public int nVar4;

    public int m_nType;
  //  cUnitData m_pUnitData;
    // Use this for initialization
    void Awake()
    {
        ItemObj.CreatePool(8);
        ItemObj.SetActive(false);
        m_nType = 0;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    static public Panel_ItemList Open(int mode, int var1 = 0, int var2 = 0, int var3 = 0,int var4 = 0)
    {
        Panel_ItemList pList = MyTool.GetPanel<Panel_ItemList>(PanelManager.Instance.OpenUI(Name));
        if (pList != null)
        {
            pList.SetMode(mode, var1, var2, var3, var4);
            pList.ReloadList();
        }
        return pList;
    }

    public void SetMode(int mode, int var1 = 0, int var2 = 0, int var3 = 0, int var4 = 0)
    {
        nMode = mode;
        nVar1 = var1;
        nVar2 = var2;
        nVar3 = var3;
        nVar4 = var4;
    }


    public void ReloadList()
    {
        UIGrid grid = GridObj.GetComponent<UIGrid>();
        if (grid != null)
        {
            grid.repositionNow = true;
        }
        // create school data item
        ItemObj.RecycleAll();

        // clear
        MyTool.DestoryGridItem(grid );
        //while (GridObj.transform.childCount > 0)
        //{

        //    DestroyImmediate(GridObj.transform.GetChild(0).gameObject);
        //}

        // 先整理各 item pool
       // Dictionary < int , int > tmpItems = new Dictionary<int, int>();
        Dictionary<int, int> items = new Dictionary<int, int>();

        List<ITEM_MISC> itemsort = new List<ITEM_MISC>();       // 排序

      // 建立數量表與 const data 陣列
        foreach (int itemid in GameDataManager.Instance.ItemPool)
        {
            if (items.ContainsKey(itemid) == false)
            {
                // insert
                items.Add(itemid , 1 );

                ITEM_MISC item = ConstDataManager.Instance.GetRow<ITEM_MISC>(itemid);   //GameDataManager.Instance.GetConstSchoolData ( nSchool );
                if (item != null)
                {
                    itemsort.Add(item);
                }
            }
            else {
                // update
                items[itemid] += 1;
            }

        }

        // create list
        //create a empty null 
        GameObject nullobj = ItemObj.Spawn(GridObj.transform);
        if (nullobj != null)
        {
            nullobj.SetActive(true); // 由於 CharUnit 被 disable。所以copy 出來的會變成disable 。需手動 active
            ItemList_Item item = nullobj.GetComponent<ItemList_Item>();
            if (item != null)
            {
                item.ReSize();
                item.SetData(0, 0);
                UIEventListener.Get(nullobj).onClick = OnItemClick; //
            }
        }
        // how to sort it
        List<ITEM_MISC> itemlist = itemsort.OrderBy(o=> o.n_TAG_LOOT).ThenBy(o => o.n_QUALITY).ThenByDescending(o => o.n_ITEMLV).ToList(); ;

        //        List<Order> objListOrder =
        //source.OrderBy(order => order.OrderDate).ThenBy(order => order.OrderId).ToList();
        //itemsort.Sort(
        //    delegate (ITEM_MISC p1, ITEM_MISC p2)
        //    {



        //        return p1.OrderDate.CompareTo(p2.OrderDate);
        //    }
        //);


        // create normal item
        //foreach (KeyValuePair<int, int> pair in items)
        foreach (ITEM_MISC misc in itemlist)
        {
            GameObject obj = ItemObj.Spawn(GridObj.transform);
            if (obj != null)
            {
                obj.SetActive(true); // 由於 CharUnit 被 disable。所以copy 出來的會變成disable 。需手動 active
                ItemList_Item item = obj.GetComponent<ItemList_Item>();
                if (item != null)
                {
                    item.ReSize();
                    int itemid = misc.n_ID;
                    int itemcount = 0;

                    items.TryGetValue(itemid,out itemcount);

                    item.SetData(itemid, itemcount);
                    //item.SetData(pair.Key, pair.Value );

                    item.CheckEquip(nVar2); // 檢查有無裝備

                    UIEventListener.Get(obj).onClick = OnItemClick; //
                }
            }
        }


        // rescroll 
    //    m_SB_Ver.value = 0.0f;

        MyTool.ResetScrollView(grid);

    }

    public void OnCloseClick(GameObject go)
    {
        PanelManager.Instance.CloseUI(Name);
        GameSystem.BtnSound(1);
    }

    public void OnItemClick(GameObject go)
    {
        ItemList_Item item = go.GetComponent<ItemList_Item>();
        if (item == null)
            return;
        
        int itemid = item.m_nItemID;

        switch (nMode) {
            case 0:// list
                {
                     // list only
                }break;

            case 1: // equip
                {
                    if(PanelManager.Instance.CheckUIIsOpening(Panel_UnitInfo.Name) ) {

                        Panel_UnitInfo panel = MyTool.GetPanel<Panel_UnitInfo>( Panel_UnitInfo.Name ); //PanelManager.Instance.OpenUI( Panel_UnitInfo.Name );
                        if (panel != null) {
                            GameSystem.PlaySound(146);
                            panel.EquipItem(nVar1 , itemid  ); // syn bag
                        }
                    }
                }
                PanelManager.Instance.CloseUI(Name);
                break;
            case 2: // enhance - 本操作取消
                {
                    if (PanelManager.Instance.CheckUIIsOpening(Panel_Enhance.Name))
                    {

                        Panel_Enhance panel = MyTool.GetPanel<Panel_Enhance>(Panel_Enhance.Name); //PanelManager.Instance.OpenUI( Panel_UnitInfo.Name );
                        if (panel != null)
                        {
                            GameSystem.PlaySound(146);
                            // panel.EquipItem(nVar1, itemid); // syn bag
                        }

                    }
                    PanelManager.Instance.CloseUI(Name);
                };
                break;
            default:
                {
                    Panel_Tip.OpenItemTip(itemid);;
                 //   Panel_Tip.OpenUI( MyTool.GetSkillName(itemid)   ); 
                }

                break;
        }
    }

    public void OnTypeClick(GameObject go)
    {
       CMD_BTN cmd = go.GetComponent<CMD_BTN>();
        if (cmd != null)
        {
            int nType = cmd.m_nVar1;
            m_nType = nType;
            RefreshList(nType);
        }
        // ReloadList();
    }

    public void RefreshList(int nType)
    {
        m_nType = nType;
        // 判斷所有的 item 並決定是否顯示，再由 grid 排序
        UIGrid grid = GridObj.GetComponent<UIGrid>();
        if (grid != null)
        {
            ItemList_Item[] itemlist = grid.GetComponentsInChildren<ItemList_Item>(true); // includeInactive
            foreach (ItemList_Item item in itemlist)
            {
                item.CheckShow( m_nType );
            }
            // re
            grid.repositionNow = true;
            
        }

    }
}
