using UnityEngine;
using System.Collections;

public class Panel_ItemList : MonoBehaviour
{
    public const string Name = "Panel_ItemList";


    public GameObject GridObj;

    public GameObject ItemObj;

    public int nMode;
    public int nVar1;
    public int nVar2;
    public int nVar3;
    public int nVar4;
    // Use this for initialization
    void Awake()
    {
        ItemObj.CreatePool(8);
        ItemObj.SetActive(false);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    static public Panel_ItemList Open(int mode, int var1 = 0, int var2 = 0, int var3 = 0, int var4 = 0)
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
        // create school data item
        ItemObj.RecycleAll();

        // clear
        while (GridObj.transform.childCount > 0)
        {

            DestroyImmediate(GridObj.transform.GetChild(0).gameObject);
        }
        // create list
        foreach (int itemid in GameDataManager.Instance.ItemPool)
        {
            GameObject obj = ItemObj.Spawn(GridObj.transform);
            if (obj != null)
            {
                ItemList_Item item = obj.GetComponent<ItemList_Item>();
                if (item != null)
                {
                    item.ReSize();
                    item.SetData(itemid, 1);

                    UIEventListener.Get(obj).onClick = OnItemClick; //
                }
            }
        }
        //create a empty null 
        GameObject nullobj = ItemObj.Spawn(GridObj.transform);
        if (nullobj != null)
        {
            ItemList_Item item = nullobj.GetComponent<ItemList_Item>();
            if (item != null)
            {
                item.ReSize();
                item.SetData(0, 0);
                UIEventListener.Get(nullobj).onClick = OnItemClick; //
            }
        }

        // rescroll 
        UIGrid grid = GridObj.GetComponent<UIGrid>();
        if ( grid!= null ) {
            grid.repositionNow = true;
        }



    }

    public void OnCloseClick(GameObject go)
    {
        PanelManager.Instance.CloseUI(Name);
    }

    public void OnItemClick(GameObject go)
    {
        ItemList_Item item = go.GetComponent<ItemList_Item>();
        if (item == null)
            return;
        
        int itemid = item.nItemID;

        switch (nMode) {
            case 1: // equip
                {
                    if(PanelManager.Instance.CheckUIIsOpening(Panel_UnitInfo.Name) ) {

                        Panel_UnitInfo panel = MyTool.GetPanel<Panel_UnitInfo>( Panel_UnitInfo.Name ); //PanelManager.Instance.OpenUI( Panel_UnitInfo.Name );
                        if (panel != null) {
                            panel.EquipItem(nVar1 , itemid  ); // syn bag
                        }

                    }
                }
                PanelManager.Instance.CloseUI(Name);
                break;
            default:
                {
                    Panel_Tip.OpenItemTip(itemid);
                    Panel_Tip.OpenUI( MyTool.GetSkillName(itemid)   ); 
                }

                break;
        }
    }
}
