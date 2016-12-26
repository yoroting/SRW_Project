using UnityEngine;
using System.Collections;

public class Panel_Enhance : MonoBehaviour {
    public const string Name = "Panel_Enhance";

    


    public GameObject lblName;
    public GameObject FaceObj;

    public GameObject BtnReset;
    public GameObject BtnClose;
    public GameObject BtnOk;
    public GameObject ExtObj; 
    public GameObject IntObj;
  //  public GameObject GridExt; // GRID obj
  //  public GameObject GridInt;

    public GameObject SchItemObj;  // school item

    public GameObject lblOrgMar;
    public GameObject lblOrgHp;
    public GameObject lblOrgMp;
    public GameObject lblOrgAtk;
    public GameObject lblOrgDef;
    public GameObject lblOrgPow;
    public GameObject lblOrgMov;


    public GameObject lblAddMar;
    public GameObject lblAddHp;
    public GameObject lblAddMp;
    public GameObject lblAddAtk;
    public GameObject lblAddDef;
    public GameObject lblAddPow;
    public GameObject lblAddMov;

    public GameObject lblTotMoney;
    public GameObject lblCostMoney;



    UIScrollView ExtScrollView;
    UIScrollView IntScrollView;
    UIGrid       ExtGrid;
    UIGrid       IntGrid;

    public int nOpCharID;

    public cUnitData pOrgData;
    public cUnitData pTmpData;

    public int nCostMoney;


    // Use this for initialization
    void Awake()
    {

        UIEventListener.Get(BtnReset).onClick += OnResetClick;
        UIEventListener.Get(BtnClose).onClick += OnCloseClick;
        UIEventListener.Get(BtnOk).onClick += OnOkClick;

        ExtScrollView = ExtObj.GetComponent<UIScrollView>();
        IntScrollView = IntObj.GetComponent<UIScrollView>();

        ExtGrid = ExtObj.GetComponentInChildren<UIGrid>(); 
        IntGrid = IntObj.GetComponentInChildren<UIGrid>(); 

        SchItemObj.CreatePool(4);
        SchItemObj.SetActive(false);

        pTmpData = new cUnitData();             // new for temp
        nCostMoney = 0;
    }
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        UpdateAttr();

        MyTool.SetLabelInt(lblTotMoney, GameDataManager.Instance.nMoney );
        MyTool.SetLabelInt(lblCostMoney, nCostMoney);
        
    }

    public void SetData(cUnitData pData )
    {
        pOrgData = pData;

        UITexture tex = FaceObj.GetComponent<UITexture>();
        if (tex != null)
        {
            tex.mainTexture = MyTool.GetCharTexture(pOrgData.n_FaceID);
        }


        Reset();
    }

    public void Reset()
    {
        if(pOrgData != null) {
            // copy Buff data

            pTmpData.Copy(pOrgData );

            //pTmpData.Buffs.Reset();
            //foreach (var p in pOrgData.Buffs.Pool)
            //{
            //    cBuffData data = p.Value;
            //    pTmpData.Buffs.AddBuff(data.nID, data.nID, data.nSkillID, data.nTargetIdent);
            //}
            //// set data
            //pTmpData.n_CharID = pOrgData.cCharData.n_ID;
            //pTmpData.SetContData( pOrgData.cCharData );

            //// copy school data
            //pTmpData.SchoolPool.Clear();
            //foreach (var pair in pOrgData.SchoolPool)
            //{
            //    pTmpData.LearnSchool(pair.Key, pair.Value);
            //    //pTmpData.SchoolPool.Add(pair.Key, pair.Value);
            //}
           
            // remove item?


            pTmpData.UpdateAllAttr();
            pTmpData.UpdateAttr();
            pTmpData.UpdateBuffConditionAttr();
            pTmpData.UpdateBuffConditionEffect();
        }

        nCostMoney = 0;

        // create school data item
        SchItemObj.RecycleAll();
      
        // clear
        while (ExtGrid.transform.childCount > 0)
        {

            DestroyImmediate(ExtGrid.transform.GetChild(0).gameObject);
        }
        while (IntGrid.transform.childCount > 0)
        {
            DestroyImmediate(IntGrid.transform.GetChild(0).gameObject);
        }

        foreach ( var pair in pTmpData.SchoolPool )
        {
            SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(pair.Key);
            if (sch == null)                continue;
            
            GameObject obj = null;
            if (sch.n_TYPE==1)            {// ext
                obj = SchItemObj.Spawn(ExtGrid.transform);
                UIDragScrollView drag = obj.GetComponent<UIDragScrollView>();
                if (drag != null)
                {
                    drag.scrollView = ExtScrollView;
                }
            } 
            else {
                obj = SchItemObj.Spawn(IntGrid.transform);
                UIDragScrollView drag = obj.GetComponent<UIDragScrollView>();
                if (drag != null)
                {
                    drag.scrollView = IntScrollView;
                }

            }                // int
            if (obj == null)
                continue;

            EnhanceItem enItem = obj.GetComponent<EnhanceItem>();
            if (enItem != null) {
                enItem.ReSize();
                //UIEventListener.Get(enItem.btnAdd).onClick = null;
                UIEventListener.Get(enItem.btnAdd).onClick = OnSchLvAddClick;
                if (sch.n_TYPE == 1)
                {
                    enItem.SetData(pair.Key , pair.Value );
                }
                else {
                    enItem.SetData(pair.Key, pair.Value);
                }
            }
        }

        // re position
        ExtGrid.repositionNow = true ;
        IntGrid.repositionNow = true;
        // show active school


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

        MyTool.SetLabelInt( lblOrgMar, (int)pTmpData.GetMar() );
        MyTool.SetLabelInt( lblOrgHp , pTmpData.GetMaxHP());
        MyTool.SetLabelInt( lblOrgMp , pTmpData.GetMaxMP() );
        MyTool.SetLabelInt( lblOrgAtk, pTmpData.GetAtk() );
        MyTool.SetLabelInt( lblOrgDef, pTmpData.GetMaxDef() );
        MyTool.SetLabelInt( lblOrgPow, pTmpData.GetPow() );
        MyTool.SetLabelInt( lblOrgMov, pTmpData.GetMov() );


    }

    void UpdateAdd()
    {

    }


    public void OnResetClick(GameObject go)
    {
        Reset();
    }
    public void OnOkClick(GameObject go)
    {
        if (GameDataManager.Instance.nMoney < nCostMoney) {
            // message Money not enough
            //Panel_CheckBox.            
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

        // store to char

        UpdateAttr();

        pOrgData.Relive();


        GameDataManager.Instance.nMoney -= nCostMoney;
         nCostMoney = 0;

        //main ten UI  要reload unit list        
        Panel_Mainten panel = MyTool.GetPanel<Panel_Mainten>(PanelManager.Instance.OpenUI(Panel_Mainten.Name));
        if (panel != null)
        {
            panel.ReloadUnitList();
        }

    }
    public void OnCloseClick(GameObject go)
    {
        // update mainten ui
        if (PanelManager.Instance.CheckUIIsOpening( Panel_Mainten.Name ))
        {
            Panel_Mainten p = MyTool.GetPanel<Panel_Mainten>( Panel_Mainten.Name );
            if (p != null) {
                p.ReloadUnitList( );
            }
        }



        PanelManager.Instance.CloseUI(Name);
    }

    // On Add School
    public void OnSchLvAddClick(GameObject go)
    {
        EnhanceItem enItem = go.GetComponentInParent <EnhanceItem> ();
        if (enItem != null)
        {
            int nSchID = enItem.nSchoolID;
            int nLv = enItem.nLv  ;

            SCHOOL sch = ConstDataManager.Instance.GetRow<SCHOOL>(nSchID);
            if (sch == null)
                return;
            // check max lv
            if (nLv >= sch.n_MAXLV)
                return;

            float fRate = Mathf.Pow(1.5f, nLv);
            int nCost = (int)(Config.LevelUPMoney * sch.f_RANK * fRate); // 用當前等級算才不會太高
            nCostMoney += nCost;


            nLv++;

            pTmpData.LearnSchool(nSchID , nLv );
            enItem.SetData( nSchID, nLv);            
                        
           

            // update lab value
            pTmpData.ActiveSchool(nSchID );

            UpdateAttr();
        }
    }

  
}
