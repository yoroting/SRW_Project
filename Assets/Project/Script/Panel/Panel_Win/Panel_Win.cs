using UnityEngine;
using System.Collections;

public class Panel_Win : MonoBehaviour {
	public const string Name = "Panel_Win";

	public GameObject   SpritObj;
    public UILabel      m_lbl_Money;
    public UIGrid        m_gridReward;
    
    public item_reward m_cRewardObj;      // 獎勵物件

    public UITexture   m_texFace;
    public UISprite     m_SpritOver;

    public GameObject m_lblNext;

    bool m_bIsAllReady = false;
    int m_nTotalMoney=0;
    void OnEnable()
    {
        Panel_StageUI.Instance.EndStage(1); // 馬上呼叫關閉旗標，是為了防止其他事件觸發，卻觸發了 通用 事件Lost() .. 因為全物件刪除( 改用旗標方法)
                                            //Panel_StageUI.Instance.bIsStageEnd = true;

        // GameDataManager.Instance.EndStage ();   // 處理戰場結束的資料回存

        // hide stage 

        m_SpritOver.alpha = 0.0f;

        m_lbl_Money.text = "0";

        m_cRewardObj.gameObject.CreatePool(4);
        m_cRewardObj.gameObject.SetActive( false );
        m_lblNext.SetActive( false );
        GameSystem.PlayBGM(5); // 勝利 BGM

        MyTool.DestoryGridItem(m_gridReward);

        // change face
        
        int nFaceID = 1; // 預設
        STAGE_DATA stage = ConstDataManager.Instance.GetRow<STAGE_DATA>(GameDataManager.Instance.nStageID);
        if (stage != null) {
            if (stage.n_MVP > 0) {
                nFaceID = stage.n_MVP;
            }
        }

        // show active school
        MyTool.ResetScrollView(m_gridReward);

        m_texFace.mainTexture = MyTool.GetCharTexture(nFaceID , 1  ) ;

        m_bIsAllReady = false;
        m_nTotalMoney = 0;

        // 陸續給獎勵
        StartCoroutine( GiveReward() );
    }
	// Use this for initialization
	void Start () {
		UIEventListener.Get(SpritObj).onClick += OnCloseBtnClick; // for trig next lineev



	}
	
	// Update is called once per frame
	void Update () {
        if (m_bIsAllReady == true)
        {

        }
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
        //GameDataManager.Instance.nEarnMoney += money;
        //GameDataManager.Instance.nMoney += money;

        //m_lbl_Money.text = money.ToString();

     //   m_bIsAllReady = true;
    }
	
	void OnCloseBtnClick(GameObject go)
	{
        if (!m_bIsAllReady)
        {
            return;
        }

		// check mission complete
	//	Panel_StageUI.Instance.CheckMissionComplete (); // last check for mission complete ， 補星星

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

    public IEnumerator GiveReward()
    {
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(2.0f);


        int nStageID = GameDataManager.Instance.nStageID;
        STAGE_DATA stage = ConstDataManager.Instance.GetRow<STAGE_DATA>(nStageID);
        if (stage != null)
        {   
            item_reward reward1 = m_cRewardObj.Spawn(m_gridReward.transform);
            if (reward1 != null)
            {
                reward1.gameObject.SetActive(true); // 由於 CharUnit 被 disable。所以copy 出來的會變成disable 。需手動 active
                reward1.SetMoney("通關獎勵", stage.n_MONEY); // 獎勵金
                m_gridReward.repositionNow = true;      // need this for second pop to re pos
            }
            yield return new WaitForSeconds(1.0f);
        }


        // 撤退人數
        if (GameDataManager.Instance.n_DeadCount == 0)
        {

            item_reward reward2 = m_cRewardObj.Spawn(m_gridReward.transform);
            if (reward2 != null)
            {
                int nReward = 1000;
                if (stage != null) {
                    nReward = stage.n_MONEY / 2;
                }
                reward2.gameObject.SetActive(true); // 由於 CharUnit 被 disable。所以copy 出來的會變成disable 。需手動 active
                reward2.SetMoney("無人撤退", nReward);
                m_gridReward.repositionNow = true;      // need this for second pop to re pos
            }
            yield return new WaitForSeconds(1.0f);
        }

        // 星星
        int nStar = Panel_StageUI.Instance.CheckMissionComplete();
        if (nStar > 0)
        {
            item_reward reward3 = m_cRewardObj.Spawn(m_gridReward.transform);
            if (reward3 != null)
            {
                reward3.gameObject.SetActive(true); // 由於 CharUnit 被 disable。所以copy 出來的會變成disable 。需手動 active
                reward3.SetMoney("星星任務", 3000* nStar);
                m_gridReward.repositionNow = true;      // need this for second pop to re pos
            }

            yield return new WaitForSeconds(1.0f);
        }

        // 回合
        if (stage != null)
        {
            if ( GameDataManager.Instance.nRound <= stage.n_ROUND )
            {
                item_reward reward4 = m_cRewardObj.Spawn(m_gridReward.transform);
                if (reward4 != null)
                {
                    reward4.gameObject.SetActive(true); // 由於 CharUnit 被 disable。所以copy 出來的會變成disable 。需手動 active
                    reward4.SetMoney("回合限制", 1000);
                    m_gridReward.repositionNow = true;      // need this for second pop to re pos
                }
                yield return new WaitForSeconds(1.0f);
            }
        }
        m_lblNext.SetActive(true);
        m_bIsAllReady = true;               // 完成
        yield break;
    }
}
