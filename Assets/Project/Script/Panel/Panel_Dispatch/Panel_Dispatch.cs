using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MYGRIDS;
using MyClassLibrary;			// for parser string

public class Panel_Dispatch : MonoBehaviour
{
    public const string Name = "Panel_Dispatch";

    public UIButton btnDispatch;
    public GameObject lblNum;
    public GameObject GridUnitList;
    public GameObject CharUnit;

    int nStartX=0;
    int nStartY=0;
    int nEndX=0;
    int nEndY=0;

    int nDispatchNum=5;

    List<int> DispatchList ;


    // Use this for initialization
    void Start()
    {
       // DispatchList = new List<int>();
        UIEventListener.Get(btnDispatch.gameObject).onClick = OnDispatch; // cheat
        CharUnit.CreatePool(5);

        if (CharUnit != null)
        {
            CharUnit.SetActive(false);
        }
        
    }

    public void OnEnable()
    {
        if (DispatchList == null) {
            DispatchList = new List<int>();
        }
        DispatchList.Clear();
        // calcul current unit
        nDispatchNum = Config.MaxStageUnit -  Panel_StageUI.Instance.GetCampNum(_CAMP._PLAYER);

        if(nDispatchNum <= 0)
        {
            PanelManager.Instance.CloseUI(Panel_Dispatch.Name); // close ui
            return;
        }

        ReloadUnitList();
        // scale
        //transform.localScale = Vector3.zero;


        //TweenScale tws = TweenScale.Begin<TweenScale>( this.gameObject , 0.1f );
        //tws.from = Vector3.zero;
        //tws.to = Vector3.one;
        //tws.PlayForward();
        //MyTool.TweenSetOneShotOnFinish(tws, OnTwAtkRotateEnd); // for once only       
    }

    public void OnTwAtkRotateEnd()
    {
        ReloadUnitList();
    }

    // Update is called once per frame
    void Update()
    {

        if (btnDispatch != null) {
            btnDispatch.isEnabled = (DispatchList.Count >= nDispatchNum );
        }

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
        if (CheckSlot() == false)
        {
           
            return false;
        }

        if (DispatchList.Contains(nCharID) == false)
        {
            DispatchList.Add(nCharID );
            UpdateUnitNum();
           
            return true;
        }
       
        return false;
    }


    public bool DelUnit( int nCharID )
    {
        if (DispatchList.Contains(nCharID) == true)
        {
            DispatchList.Remove(nCharID);
            UpdateUnitNum();
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


        GameSystem.PlaySound(201);

        PanelManager.Instance.CloseUI(Panel_Dispatch.Name);
    }

    void OnUnitClick(GameObject go)
    {

    }

    public void ReloadUnitList(int nCharID = 0) // 0- all
    {
        
        //if (nCharID != 0) {
        List<int> avoidList = new List<int>();
        STAGE_DATA StageData = ConstDataManager.Instance.GetRow<STAGE_DATA>(GameDataManager.Instance.nStageID);
        if (StageData != null)
        {
            // create avoid list
            char[] split = { ';', ' ', ',' };
            string[] strAvoid = StageData.s_AVOID_CHAR.Split(split);
            for (int i = 0; i < strAvoid.Length; i++)
            {
                int nAvoid = 0;
                if (int.TryParse(strAvoid[i], out nAvoid))
                {
                    if (0 == nAvoid)
                        continue;                  
                    if (avoidList.Contains(nAvoid) == false)
                    {
                        avoidList.Add(nAvoid);
                    }
                }
            }
        }

        //    // sort grid resort

        //    return;
        //}

        // clear all
        CharUnit.RecycleAll();
        
        UIGrid grid = GridUnitList.GetComponent<UIGrid>();     
        MyTool.DestoryGridItem(grid );

        //grid.repositionNow = true;  // for re pos
        //grid.Reposition();
        // sort by mar

        var items = from pair in GameDataManager.Instance.StoragePool
                    orderby pair.Value.GetMar() descending
                    select pair;


        // release all unit
        //foreach (var pair in GameDataManager.Instance.StoragePool)
        int nCount = 0;
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
            // skip avoid char
            if (avoidList.Contains(pair.Value.n_CharID) ) {
                continue;
            }

            GameObject obj = CharUnit.Spawn(GridUnitList.transform);
            if (obj != null)
            {
                obj.SetActive(true); // 由於 CharUnit 被 disable。所以copy 出來的會變成disable 。需手動 active
                Mainten_Unit unit = obj.GetComponent<Mainten_Unit>();
                if (unit != null)
                {
                    unit.ReSize();
                    unit.SetData(pair.Value,2);  //設定為 待機
                }
                nCount++;
            }
        }
        // 倉庫人數太少，以倉庫人數為最大值
        if (nCount < nDispatchNum ) {
            nDispatchNum = nCount;
        }


        grid.repositionNow = true;  // for re pos
        grid.Reposition();

        MyTool.ResetScrollView( grid );

        UpdateUnitNum();
    }

    void UpdateUnitNum()
    {
        if (lblNum != null)
        {  
            string snum = string.Format("{0}/{1}" , DispatchList.Count, nDispatchNum );
            MyTool.SetLabelText(lblNum , snum );
        }
    }
}
