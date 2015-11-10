using UnityEngine;
using System.Collections;

public class Mainten_Unit : MonoBehaviour {

    public GameObject EnhanceBtn;
    private cUnitData pUnitData;


    public GameObject FaceObj;
    public GameObject NameObj;

    public GameObject MarObj;

    public GameObject IntSchObj;
    public GameObject ExtSchObj;

    public GameObject LvObj;
    public GameObject ExpObj;



    // Use this for initialization
    void Start () {

        UIEventListener.Get(FaceObj).onClick += OnUnitInfoClick;
        UIEventListener.Get(EnhanceBtn).onClick += OnUnitEnhanceClick;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void onEnable()
    {
        
        ReSize();
        
    }

    public void ReSize()
    {
       
        transform.localScale = Vector3.one;

        transform.localRotation = Quaternion.identity;
    }

    public void SetData( cUnitData UnitData )
    {
        pUnitData = UnitData;
        ReloadData();
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
            tex.mainTexture = MyTool.GetCharTexture(nCharId);
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
        if (pUnitData == null) {
            Debug.LogError("OnUnitInfoClick with null data");
            return;
        }

        Panel_UnitInfo.OpenUI( pUnitData );
        

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
}
