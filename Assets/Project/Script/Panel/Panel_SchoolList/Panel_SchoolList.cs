using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Panel_SchoolList : MonoBehaviour {

    public const string Name = "Panel_SchoolList";


    public GameObject GridObj;

    public GameObject ItemObj;

    public GameObject ContentObj;

    public int nMode;
    public int nVar1;
    public int nVar2;
    public int nVar3;
    public int nVar4;

    public cUnitData pUnitData;

    public int nSelectID;

    // Use this for initialization
    void Awake()
    {
        ItemObj.CreatePool(8);
        ItemObj.SetActive(false);

        MyTool.SetLabelText(ContentObj, "");
    }

    void Start () {
	
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
        pUnitData = pData;
        nVar1 = var1;
        nVar2 = var2;
        nVar3 = var3;
        nVar4 = var4;
        nSelectID = 0;
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
        if (pUnitData == null)
            return;

        foreach ( KeyValuePair< int , int > pair in pUnitData.SchoolPool)
        {
            int schoolid = pair.Key;
            int lv = pair.Value;
            SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(schoolid);
            if (sch == null) {
                continue;
            }
            // int 
            if (nMode == 0)
            {
                if (sch.n_TYPE == 1) {
                    continue;
                }
            }
            // ext
            else if (nMode == 1) {
                if (sch.n_TYPE == 0)                {
                    continue;
                }
            }

            GameObject obj = ItemObj.Spawn(GridObj.transform);
            if (obj != null)
            {
                ItemList_School item = obj.GetComponent<ItemList_School>();
                if (item != null)
                {
                    item.ReSize();
                    item.SetData(schoolid , lv );
                    UIEventListener.Get(obj).onClick = OnItemClick; //
                }
            }
        }
        // rescroll 
        UIGrid grid = GridObj.GetComponent<UIGrid>();
        if (grid != null)
        {
            grid.repositionNow = true;
        }
    }

    public void OnCloseClick(GameObject go)
    {
        PanelManager.Instance.CloseUI(Name);
    }

    public void OnOKClick(GameObject go)
    {
        if (nSelectID > 0 && (pUnitData != null ) ) {

            pUnitData.ActiveSchool(nSelectID);
            GameSystem.PlaySound("Audios 00050");
        }

        PanelManager.Instance.CloseUI(Name); // change school
    }


    public void OnItemClick(GameObject go)
    {
        ItemList_School item = go.GetComponent<ItemList_School>();
        if (item == null)
            return;

        nSelectID = item.nSchoolID;

        //Panel_TipOpenItemTip(itemid);
        //Panel_Tip.OpenUI(MyTool.GetSkillName(itemid));

        ;
        MyTool.SetLabelText(ContentObj, MyTool.GetSchoolName(nSelectID) );
    }

}
