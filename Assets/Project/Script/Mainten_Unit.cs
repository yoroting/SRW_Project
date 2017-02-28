﻿using UnityEngine;
using System.Collections;

public class Mainten_Unit : MonoBehaviour
{

    public GameObject EnhanceBtn;
    private cUnitData pUnitData;


    public GameObject FaceObj;
    public GameObject NameObj;

    public GameObject MarObj;

    public GameObject IntSchObj;
    public GameObject ExtSchObj;

    public GameObject LvObj;
    public GameObject ExpObj;

    public GameObject FuncObj;  // 功能名稱

    public int m_nType = 0;

    // Use this for initialization
    void Start()
    {

        UIEventListener.Get(FaceObj).onClick = OnUnitInfoClick;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {

        ReSize();

    }
    private void OnDisable()
    {

        int A = 0;

    }

    public void ReSize()
    {

        transform.localScale = Vector3.one;

        transform.localRotation = Quaternion.identity;
    }

    public void SetData(cUnitData UnitData, int nType = 0)
    {
        //if (scrollview != null) {
        //    UIDragScrollView dragitem= this.GetComponent<UIDragScrollView>();
        //    if (dragitem != null) {
        //        dragitem.scrollView = scrollview;
        //    }
        //}
        pUnitData = UnitData;
        ReloadData();
        SetType(nType);
    }

    public void SetType(int nType)
    {
        if (nType == 0) // 強化
        {
            MyTool.SetLabelText(FuncObj, "修練");
            UIEventListener.Get(EnhanceBtn).onClick = OnUnitEnhanceClick;
        }
        else if (nType == 1) // 出擊
        {
            MyTool.SetLabelText(FuncObj, "出擊");
            UIEventListener.Get(EnhanceBtn).onClick = OnUnitWaiting; // 點擊後 待機
        }
        else if (nType == 2) // 待機
        {
            MyTool.SetLabelText(FuncObj, "待機");
            UIEventListener.Get(EnhanceBtn).onClick = OnUnitFight;// 點擊後  出擊

        }
    }

    public void ReloadData()
    {
        if (pUnitData == null)
            return;

        int nCharId = pUnitData.n_CharID;
        //CHARS chars = data.cCharData;
        // change face	
        UITexture tex = FaceObj.GetComponent<UITexture>();
        if (tex != null)
        {
            //          string url = "Art/char/" + pUnitData.cCharData.s_FILENAME + "_S";
            //        //Texture2D tex = Resources.LoadAssetAtPath(url, typeof(Texture2D)) as Texture2D;
            //      Texture t = Resources.Load(url, typeof(Texture)) as Texture;
            tex.mainTexture = MyTool.GetCharTexture(pUnitData.n_FaceID);
        }


        // name 
        //string name = pUnitData.cCharData.s_NAME;
        MyTool.SetLabelText(NameObj, MyTool.GetCharName(nCharId));


        pUnitData.UpdateAllAttr();
        pUnitData.UpdateAttr();
        pUnitData.UpdateBuffConditionAttr();
        // lv
        MyTool.SetLabelInt(LvObj, pUnitData.n_Lv);
        // exp 
        MyTool.SetLabelInt(ExpObj, pUnitData.n_EXP);
        // mar
        MyTool.SetLabelFloat(MarObj, pUnitData.GetMar());
        //// HP
        //int nMaxHp = pUnitData.GetMaxHP();
        //MyTool.SetLabelText(HpObj, string.Format("{0}/{1}", pUnitData.n_HP, nMaxHp));
        //// MP
        //int nMaxMp = pUnitData.GetMaxMP();
        //MyTool.SetLabelText(MpObj, string.Format("{0}/{1}", pUnitData.n_MP, nMaxMp));
        //// SP
        //int nMaxSp = pUnitData.GetMaxSP();
        //MyTool.SetLabelText(SpObj, string.Format("{0}/{1}", pUnitData.n_SP, nMaxSp));
        //// mov
        //MyTool.SetLabelInt(MovObj, pUnitData.GetMov());
        //// atk 
        //MyTool.SetLabelInt(AtkObj, pUnitData.GetAtk());
        //// def 
        //int nMaxDef = pUnitData.GetMaxDef();
        //MyTool.SetLabelText(DefObj, string.Format("{0}/{1}", pUnitData.n_DEF, nMaxDef));

        //// pow
        //MyTool.SetLabelInt(PowObj, pUnitData.GetPow());

        // school name

        //SCHOOL inSch = GameDataManager.Instance.GetConstSchoolData( data.nActSch[0] ); // int 
        MyTool.SetLabelText(IntSchObj, pUnitData.GetSchoolFullName(pUnitData.GetIntSchID()));

        //SCHOOL exSch = ConstDataManager.Instance.GetRow<SCHOOL>( pUnitData.nActSch[1] );//   GameDataManager.Instance.GetConstSchoolData( pUnitData.nActSch[1] ); // ext 
        MyTool.SetLabelText(ExtSchObj, pUnitData.GetSchoolFullName(pUnitData.GetExtSchID()));


        // Set ability
        //UpdateCharData();

    }


    void OnUnitInfoClick(GameObject go)
    {
        if (pUnitData == null)
        {
            Debug.LogError("OnUnitInfoClick with null data");
            return;
        }

        Panel_UnitInfo.OpenUI(pUnitData);


    }

    void OnUnitEnhanceClick(GameObject go)
    {
        if (pUnitData == null)
        {
            Debug.LogError("OnUnitInfoClick with null data");
            return;
        }

        //Panel_Cheat panel = MyTool.GetPanel<Panel_Cheat>(PanelManager.Instance.OpenUI(Panel_Cheat.Name));
        //if (pUnitData != null)
        //{            
        //        panel.SetData(pUnitData);           
        //}

        Panel_Enhance panel = MyTool.GetPanel<Panel_Enhance>(PanelManager.Instance.OpenUI(Panel_Enhance.Name));
        if (pUnitData != null)
        {
            panel.SetData(pUnitData);
        }

    }

    void OnUnitFight(GameObject go)
    {
        Panel_Dispatch panel = MyTool.GetPanel<Panel_Dispatch>(PanelManager.Instance.OpenUI(Panel_Dispatch.Name));
        if (panel != null) {
            if( panel.AddUnit(pUnitData.n_CharID ))
            {
                SetType(1);
            }
        }        
    }

    void OnUnitWaiting(GameObject go)
    {
        Panel_Dispatch panel = MyTool.GetPanel<Panel_Dispatch>(PanelManager.Instance.OpenUI(Panel_Dispatch.Name));
        if (panel != null)
        {
            if (panel.DelUnit(pUnitData.n_CharID))
            {
                SetType(2);
            }
        }        
    }
}