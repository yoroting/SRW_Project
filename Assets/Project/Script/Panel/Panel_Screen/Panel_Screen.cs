using UnityEngine;
using System.Collections;

public class Panel_Screen : MonoBehaviour {
    public const string Name = "Panel_Screen";

    public GameObject BackObj;              //場景名稱 label 物件
    public GameObject SceneNameObj;              //場景名稱 label 物件
                                                 // Use this for initialization

    public bool  m_bIsFadeIning;
    public bool  m_bIsFadeOuting;
  //  int m_SceneID;

    void Start () {
    //    m_SceneID = 0;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnEnable()
    {
        UIWidget widget = BackObj.GetComponent<UIWidget>();
        if (widget != null)
        {
            widget.width = Config.WIDTH;
            widget.height = Config.HEIGHT;
        }

           // auto hide background when reopen
           m_bIsFadeIning = false;
        m_bIsFadeOuting = false;

        FadeIn();
    }
    public void FadeIn()
    {
        // clear all
        //Clear();
        // GameDataManager.Instance.nTalkID = 1512;
        //int nTalkID = GameDataManager.Instance.nTalkID;
        //if (nTalkID > 0)
        //{
        //    SetScript(GameDataManager.Instance.nTalkID);
        //}
        // fade out bgm
        GameSystem.PlayBGM(0);

        TweenAlpha tw = TweenAlpha.Begin<TweenAlpha>(this.gameObject, 0.2f);
        if (tw != null)
        {
            //MyTool.SetAlpha (TilePlaneObj, 0.0f);
            tw.delay = 0.0f;
            tw.from = 0.0f;
            //tw.SetStartToCurrentValue();
            tw.to = 1.0f;
            tw.SetOnFinished(OnFadeInFinished);
        }

        m_bIsFadeIning = true;
        m_bIsFadeOuting = false;
        //bSkipMode = false;

        // no more fadein
        //OnFadeInFinished();
    }
    void OnFadeInFinished()
    {
        m_bIsFadeIning = false;
        // auto fade aftert 
        FadeOut();
    }


    public void FadeOut()
    {
        if (m_bIsFadeIning)
            return;
        // clear all
        //Clear();
        // GameDataManager.Instance.nTalkID = 1512;
        //int nTalkID = GameDataManager.Instance.nTalkID;
        //if (nTalkID > 0)
        //{
        //    SetScript(GameDataManager.Instance.nTalkID);
        //}

        TweenAlpha tw = TweenAlpha.Begin<TweenAlpha>(this.gameObject, 2.0f);
        if (tw != null)
        {
            //MyTool.SetAlpha (TilePlaneObj, 0.0f);
            tw.delay = 1.0f;
            tw.from = 1.0f;
            //tw.SetStartToCurrentValue();
            tw.to = 0.0f;
            tw.SetOnFinished(OnFadeOutFinished);
        }
        
        m_bIsFadeOuting = true;
        //bSkipMode = false;

        // no more fadein
        //OnFadeInFinished();
    }

    void OnFadeOutFinished()
    {
        m_bIsFadeOuting = false;
        // auto fade aftert 
        // auto click
        TalkClickEvent evt = new TalkClickEvent();
        GameEventManager.DispatchEvent(evt);

//        if (PanelManager.Instance.CheckUIIsOpening(Panel_Talk.Name) == true)
//        {
//        }
            


        PanelManager.Instance.CloseUI( Name );

    }

    public void SetSceneName(string sName )
    {
        //    if (Panel_StageUI.Instance.m_bIsSkipMode)
        //        return;
        MyTool.SetLabelText(SceneNameObj, sName);
    }


    static public void Open(int nID)
    {
        if (nID == 0 )
            return ;
        
        DataRow row = ConstDataManager.Instance.GetRow("SCENE_NAME_TIP", nID);
        if (row != null)
        {
            string sName = row.Field<string>("s_SCENE_NAME");
            GameObject go = PanelManager.Instance.OpenUI(Name);
            Panel_Screen panel = MyTool.GetPanel<Panel_Screen>(go);
            if (panel != null)
            {
                panel.SetSceneName(sName);
            }         
        }
       


       

    }
}
