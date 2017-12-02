using UnityEngine;
using System.Collections;



public class Panel_Loading : MonoBehaviour {
	public const string Name =	"Panel_Loading";

    public enum _LOAD_TYPE
    {
        _START,
        _SAVE_DATA,
        _STORY,
        _STAGE
    }

    public GameObject lblName;


    public cSaveData save;
    public _LOAD_TYPE m_Type;
    bool m_LoadComplete;
  
    void OnEnable()
	{
		if (lblName != null) {
			MyTool.SetLabelText( lblName , ""  );
		}
        m_LoadComplete = false;
        GameSystem.bFXPlayMode = false;
    //    m_fLoadingTime = 0.0f;


    }

    void OnDisable()
    {   
        GameSystem.bFXPlayMode = true ; // play fx
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        
        //    StartCoroutine (  SaveLoading( save  ) );

        if (m_LoadComplete)
        {
            // close this
            PanelManager.Instance.CloseUI(Name);
        }
        else {
          
        }

    }

    public void Start_Coroutine()
    {
        if (m_Type == _LOAD_TYPE._SAVE_DATA)
        {
            StartCoroutine(SaveLoading(save));
        }
        else if (m_Type == _LOAD_TYPE._STORY)
        {
            StartCoroutine(EnterStory(GameDataManager.Instance.nStoryID));
        }
        else if (m_Type == _LOAD_TYPE._STAGE)
        {
            StartCoroutine(EnterStage(GameDataManager.Instance.nStageID));
        }
    }

    public void ShowStoryName()
	{
		if (lblName != null) {
			MyTool.SetLabelText( lblName , MyTool.GetStoryName( GameDataManager.Instance.nStageID )  );
		}

	}

    static public void  StartLoad(cSaveData save  , _LOAD_TYPE type )
    {

        Panel_Loading Loading = MyTool.GetPanel<Panel_Loading>(PanelManager.Instance.OpenUI("Panel_Loading"));
        if (Loading != null)
        {
            Loading.save = save;
            Loading.m_Type = type;

            Loading.Start_Coroutine(); // 宣告在 這裡才不會因別的plane 關閉而影響到
        }
    }

    // func
    IEnumerator EnterStory(int nStoryID)
    {
        //   PanelManager.Instance.OpenUI("Panel_Loading");
     
        // back up char pool
        GameDataManager.Instance.PrepareEnterStage();

        yield return new WaitForEndOfFrame();
     
        GameDataManager.Instance.nStoryID = nStoryID;
        yield return new WaitForEndOfFrame();
     
        PanelManager.Instance.OpenUI(StoryUIPanel.Name);        
     
        yield return new WaitForSeconds(3);
        //   PanelManager.Instance.DestoryUI(Name);              // close main 
        m_LoadComplete = true;
        yield break;

    }

    IEnumerator EnterStage(int nStageID)
    {        
        GameDataManager.Instance.nStageID = nStageID;

        GameObject obj = PanelManager.Instance.OpenUI("Panel_Loading");
        if (obj != null)
        {
            Panel_Loading ploading = MyTool.GetPanel<Panel_Loading>(obj);
            if (ploading != null)
            {
                ploading.ShowStoryName();
            }
        }


        yield return false;
        
        PanelManager.Instance.OpenUI(Panel_StageUI.Name);//"Panel_StageUI"
        yield return false;
        
        //   PanelManager.Instance.DestoryUI(Name); // destory this ui will broken this Coroutine soon
        //   yield return true;
        yield return new WaitForSeconds(3);
        m_LoadComplete = true;
        yield break;
    }

    public IEnumerator SaveLoading(cSaveData save)
    {
        if (save == null) {
            cSaveData.SetLoading(false);
            m_LoadComplete = true;
            yield break;
        }        
        //GameDataManager.Instance.nStoryID = nStoryID;
        //GameDataManager.Instance.nStageID = save.n_StageID;

        //PanelManager.Instance.OpenUI( "Panel_Loading");
     //   System.Threading.Thread.Sleep(100);
        yield return new WaitForEndOfFrame();        
        if (save.ePhase == _SAVE_PHASE._MAINTEN)
        {
            
            Panel_Mainten panel = MyTool.GetPanel<Panel_Mainten>(PanelManager.Instance.OpenUI(Panel_Mainten.Name));
            yield return new WaitForEndOfFrame();            
            panel.RestoreBySaveData(save);
          
        }
        else if (save.ePhase == _SAVE_PHASE._STAGE)
        {            
            if (PanelManager.Instance.CheckUIIsOpening(Panel_StageUI.Name) == false)
            {
                PanelManager.Instance.OpenUI(Panel_StageUI.Name);  // don't run start() during open
            }
                                                               //			Panel_StageUI.Instance.bIsRestoreData = true;
            yield return new WaitForEndOfFrame();        
            Panel_StageUI.Instance.RestoreBySaveData(save); // will start stage update

        }
        //   System.Threading.Thread.Sleep(100);
        //   yield return new WaitForEndOfFrame();

        //  PanelManager.Instance.CloseUI(Name);
        cSaveData.SetLoading(false);
        // yield return new WaitForSeconds(3); // no need wait second
        m_LoadComplete = true;
        yield break;

    }

}
