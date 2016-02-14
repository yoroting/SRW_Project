using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MYGRIDS;
using MyClassLibrary;			// for parser string

public class Panel_Dispatch : MonoBehaviour
{
    public const string Name = "Panel_Dispatch";

    public GameObject btnDispatch;

    public GameObject GridUnitList;
    public GameObject CharUnit;

    int nStartX=0;
    int nStartY=0;
    int nEndX=0;
    int nEndY=0;

    int nDispatchNum=5;

    List<int> DispatchList;


    // Use this for initialization
    void Start()
    {
        DispatchList = new List<int>();
        UIEventListener.Get(btnDispatch).onClick = OnDispatch; // cheat
        CharUnit.CreatePool(5);

        if (CharUnit != null)
        {
            CharUnit.SetActive(false);
        }
        
    }

    public void OnEnable()
    {
        // calcul current unit
        nDispatchNum = Config.MaxStageUnit -  GameDataManager.Instance.GetCampNum(_CAMP._PLAYER);

        if(nDispatchNum <= 0)
        {
            PanelManager.Instance.CloseUI(Panel_Dispatch.Name); // close ui
            return;
        }    

        ReloadUnitList();
    }


    // Update is called once per frame
    void Update()
    {

    }

   public  void BornBlock(int nStX, int nStY , int nEdX , int nEdY)
    {
        nStartX = nStX < nEdX ? nStX : nEdX;
        nStartY = nStY < nEdY ? nStY : nEdY ;
        nEndX = nStX > nEdX ? nStX : nEdX;
        nEndY =  nStY > nEdY ? nStY : nEdY;



    }


    public bool AddUnit( int nCharID )
    {
        if ( CheckSlot() == false)
            return false;

        if (DispatchList.Contains(nCharID) == false)
        {
            DispatchList.Add(nCharID );
            return true;
        }

        return false;
    }


    public bool DelUnit( int nCharID )
    {
        if (DispatchList.Contains(nCharID) == true)
        {
            DispatchList.Remove(nCharID);
            return true;
        }
        return true;
    }

    // 判斷還有沒有派兵空間
    public bool CheckSlot()
    {
        return (DispatchList.Count < nDispatchNum);
    }

    void OnDispatch(GameObject go)
    {
        for( int i = nStartX; i <= nEndX; i++)
        {
            if (DispatchList.Count <= 0)
            {
                break;
            }

            for (int j = nStartY; j <= nEndY && (DispatchList.Count > 0) ; j++)
            {
                if (Panel_StageUI.Instance.CheckIsEmptyPos( new iVec2( i , j ) )== false ) {
                    continue;
                }


                int charid = DispatchList[0];
                DispatchList.RemoveAt(0);
                StagePopUnitEvent evt = new StagePopUnitEvent();
                evt.eCamp = _CAMP._PLAYER;
                evt.nCharID = charid;
                evt.nX = i;
                evt.nY = j;
                Panel_StageUI.Instance.OnStagePopUnitEvent(evt);
            }
        }
        // 強制 pop  剩下的
        while (DispatchList.Count > 0)
        {
            int charid = DispatchList[0];
            DispatchList.RemoveAt(0);
            StagePopUnitEvent evt = new StagePopUnitEvent();
            evt.eCamp = _CAMP._PLAYER;
            evt.nCharID = charid;
            evt.nX = nEndX;
            evt.nY = nEndY;
            Panel_StageUI.Instance.OnStagePopUnitEvent(evt);
        }  ;



        PanelManager.Instance.CloseUI(Panel_Dispatch.Name);
    }

    void OnUnitClick(GameObject go)
    {

    }

    public void ReloadUnitList(int nCharID = 0) // 0- all
    {
        //if (nCharID != 0) {


        //    // sort grid resort

        //    return;
        //}

        // clear all
        CharUnit.RecycleAll();
        UIGrid grid = GridUnitList.GetComponent<UIGrid>();
        if (grid != null)
        {
            while (grid.transform.childCount > 0)
            {
                DestroyImmediate(grid.transform.GetChild(0).gameObject);
            }
        }

        // sort by mar

        var items = from pair in GameDataManager.Instance.StoragePool
                    orderby pair.Value.GetMar() descending
                    select pair;


        // release all unit
        //foreach (var pair in GameDataManager.Instance.StoragePool)
        foreach (var pair in items)
        {
            if (pair.Value == null)
                continue;


            if (pair.Value.bEnable == false)
            {
                if (Config.SHOW_LEAVE == false)
                {
                    continue;
                }
            }

            GameObject obj = CharUnit.Spawn(GridUnitList.transform);
            if (obj != null)
            {
                Mainten_Unit unit = obj.GetComponent<Mainten_Unit>();
                if (unit != null)
                {
                    unit.ReSize();
                    unit.SetData(pair.Value,2);  //設定為 待機
                }
            }
        }

        grid.repositionNow = true;  // for re pos
    }
}
