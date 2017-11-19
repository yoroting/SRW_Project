using UnityEngine;
using System.Collections;

public class Panel_Win : MonoBehaviour {
	public const string Name = "Panel_Win";

	public GameObject   SpritObj;
    public UILabel      m_lbl_Money;
    public UIGrid        m_gridReward;
    
    public UITexture   m_texFace;
    public UISprite     m_SpritOver;
    bool m_bIsAllReady = false;
    void OnEnable()
	{
		Panel_StageUI.Instance.EndStage ( 1 ); // 馬上呼叫關閉旗標，是為了防止其他事件觸發，卻觸發了 通用 事件Lost() .. 因為全物件刪除( 改用旗標方法)
                                               //Panel_StageUI.Instance.bIsStageEnd = true;

        // GameDataManager.Instance.EndStage ();   // 處理戰場結束的資料回存

        // hide stage 

        m_SpritOver.alpha = 0.0f;

        m_lbl_Money.text = "0";

        GameSystem.PlayBGM ( 5 ); // 勝利 BGM

        MyTool.DestoryGridItem(m_gridReward);

        m_bIsAllReady = false;
    }
	// Use this for initialization
	void Start () {
		UIEventListener.Get(SpritObj).onClick += OnCloseBtnClick; // for trig next lineev



	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDisable () {
	//	UIEventListener.Get(SpritObj).onClick -= OnCloseBtnClick; // for trig next lineev
	}

    public void OnAllReady()
    {

        // 計算 獎勵
        int money = 0;
        int nStageID = GameDataManager.Instance.nStageID;
        STAGE_DATA stage = ConstDataManager.Instance.GetRow<STAGE_DATA>(nStageID);
        if (stage != null)
        {
            money += stage.n_MONEY; // 獎勵金
        }

        // 界算金錢
        // Debug 計算
        GameDataManager.Instance.nEarnMoney += money;
        GameDataManager.Instance.nMoney += money;

        m_lbl_Money.text = money.ToString();

        m_bIsAllReady = true;
    }
	
	void OnCloseBtnClick(GameObject go)
	{
        if (!m_bIsAllReady)
        {
            return;
        }

		// check mission complete
		Panel_StageUI.Instance.CheckMissionComplete (); // last check for mission complete ， 補星星


        

            // if it have talk event. play it
        Panel_StageUI.Instance.ShowStage (false);

        // 進入 結束 事件
        Panel_StageUI.Instance.EnterAfterPhase();
        // 決定 下一個story

        GameDataManager.Instance.NextStoryFromWin();

        // 清空 backJson 已用不著了
//        GameDataManager.Instance.sBackJson = "";

        //STAGE_DATA stage = ConstDataManager.Instance.GetRow<STAGE_DATA>(GameDataManager.Instance.nStageID);
        //if (stage != null)
        //{
        //    GameDataManager.Instance.nStoryID = stage.n_NEXT_STORY;
        //}


        // Go to Mainten Ui 
        PanelManager.Instance.DestoryUI ( Name );

	}
}
