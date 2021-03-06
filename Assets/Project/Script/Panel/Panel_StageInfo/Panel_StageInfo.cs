﻿using UnityEngine;
using System.Collections;

public class Panel_StageInfo : MonoBehaviour {
	public const string Name = "Panel_StageInfo";

    public GameObject lblWinObj;
    public GameObject lblLostObj;
    public GameObject lblMissionObj;

	public GameObject lblRoundValueObj;
	public GameObject lblStarValueObj;
	public GameObject lblMoneyValueObj;

	public GameObject lblStageNameObj;
	public GameObject btnCloseObj;

	// Use this for initialization
	void Start () {
	
		UIEventListener.Get(this.btnCloseObj).onClick += OnCloseClick; // for trig next line
	}


	void OnEnable () {

		SetData ();
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void SetData(){
        // set round , money, star
        string sround = GameDataManager.Instance.nRound.ToString();

        STAGE_DATA stage = ConstDataManager.Instance.GetRow<STAGE_DATA>(GameDataManager.Instance.nStageID);
        if (stage != null)
        {
            sround = string.Format(" {0}/{1}" , GameDataManager.Instance.nRound , stage.n_ROUND );            
        }

        //MyTool.SetLabelInt ( lblRoundValueObj , GameDataManager.Instance.nRound );
        MyTool.SetLabelText(lblRoundValueObj , sround );

		MyTool.SetLabelInt ( lblStarValueObj  , GameDataManager.Instance.nStars );
		MyTool.SetLabelInt ( lblMoneyValueObj , GameDataManager.Instance.nMoney );

        // stage name
        MyTool.SetLabelText(lblStageNameObj, MyTool.GetStoryName(GameDataManager.Instance.nStageID));
        //ConstDataManager.Instance.GetRow ("STORY_NAME" , nStageID );
        // 
       // int nStageID = GameDataManager.Instance.nStageID;
		//STAGE_DATA stage = ConstDataManager.Instance.GetRow< STAGE_DATA > (nStageID);
		//if (stage == null)
		//	return;
        // 改變設計， 透過 phase 取字串
        STAGE_PHASE phase = ConstDataManager.Instance.GetRow<STAGE_PHASE>( GameDataManager.Instance.n_StagePhase) ;
        if (phase == null) {
            return;
        }
        char[] split = { ';' };
        // 勝利
        string sWin = "";
        string[] strWins = phase.s_WIN_CONDITION.Split(split);
        for (int i = 0; i < strWins.Length; i++)
        {
            int nID = 0;
            if (int.TryParse(strWins[i], out nID))
            {
                //MISSION_TEXT mText = ConstDataManager.Instance.GetRow<MISSION_TEXT> ( nMissionID );
                DataRow row = ConstDataManager.Instance.GetRow("MISSION_TEXT", nID);
                //if( mText != null )
                if (row != null)
                {
                    string content = row.Field<string>("s_CONTENT");

                    if (i > 0)
                    {
                        sWin += "\r\n";

                    }
                    sWin += content;
                }
            }
        }
        if (sWin != "")
        {
            sWin = sWin.Replace("$F", Config.PlayerFirst); // replace player name
            sWin = sWin.Replace("$N", Config.PlayerName); // replace player name

            MyTool.SetLabelText(lblWinObj, sWin);
        }
        lblWinObj.SetActive(sWin != "");

        // 失敗
        string sLost = "";
        string[] strLosts = phase.s_LOST_CONDITION.Split(split);
        for (int i = 0; i < strLosts.Length; i++)
        {
            int nID = 0;
            if (int.TryParse(strLosts[i], out nID))
            {
                //MISSION_TEXT mText = ConstDataManager.Instance.GetRow<MISSION_TEXT> ( nMissionID );
                DataRow row = ConstDataManager.Instance.GetRow("MISSION_TEXT", nID);
                //if( mText != null )
                if (row != null)
                {
                    string content = row.Field<string>("s_CONTENT");

                    if (i > 0)
                    {
                        sLost += "\r\n";

                    }
                    sLost += content;
                }
            }
        }
        if (sLost != "")
        {
            sLost = sLost.Replace("$F", Config.PlayerFirst); // replace player name
            sLost = sLost.Replace("$N", Config.PlayerName); // replace player name
            MyTool.SetLabelText(lblLostObj, sLost);
        }
        lblLostObj.SetActive(sLost != "");

        // mission
        //        UILabel lbl = lblMissionObj.GetComponent< UILabel > ();
        //		if (lbl != null)
        //        {
        //			lbl.text = ""; // clear first




        // 任務
        //string[] strEvent = stage.s_MISSION.Split(split);
        string sMission = "";
            string[] strMissions = phase.s_MISSION.Split(split);
            for (int i = 0; i < strMissions.Length; i++)
			{
				int nMissionID = 0 ; 
				if( int.TryParse(strMissions[i] , out nMissionID )  )
				{
					//MISSION_TEXT mText = ConstDataManager.Instance.GetRow<MISSION_TEXT> ( nMissionID );
					DataRow row = ConstDataManager.Instance.GetRow( "MISSION_TEXT" , nMissionID);
					//if( mText != null )
					if( row != null )
					{				
						string content = row.Field<string>( "s_CONTENT");
						
                        if( i > 0 )
                        {
                            sMission += "\r\n";
                        
                        }
                        sMission += content;                        
                    }
				}
			}
            if (sMission != "")
            {
                sMission = sMission.Replace("$F", Config.PlayerFirst); // replace player name
                sMission = sMission.Replace("$N", Config.PlayerName); // replace player name
                MyTool.SetLabelText(lblMissionObj, sMission);
            }
            lblMissionObj.SetActive( sMission != "" );


   //     }
		//MyTool.SetLabelText (lblStageNameObj, MyTool.GetStoryName (nStageID));




		

	}
	//=================

	void OnCloseClick(GameObject go)
	{
        GameSystem.BtnSound(1);
        PanelManager.Instance.CloseUI (Panel_StageInfo.Name);
	}

}
