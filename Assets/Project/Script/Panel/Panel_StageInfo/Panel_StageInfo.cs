using UnityEngine;
using System.Collections;

public class Panel_StageInfo : MonoBehaviour {
	public const string Name = "Panel_StageInfo";

	public GameObject lblContentObj;

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
		MyTool.SetLabelInt ( lblRoundValueObj , GameDataManager.Instance.nRound );
		MyTool.SetLabelInt ( lblStarValueObj  , GameDataManager.Instance.nStars );
		MyTool.SetLabelInt ( lblMoneyValueObj , GameDataManager.Instance.nMoney );

		// 
		int nStageID = GameDataManager.Instance.nStageID;
		STAGE_DATA stage = ConstDataManager.Instance.GetRow< STAGE_DATA > (nStageID);
		if (stage == null)
			return;
		// mission
		UILabel lbl = lblContentObj.GetComponent< UILabel > ();
		if (lbl != null) {
			lbl.text = ""; // clear first

			char[] split = { ';' };
			string[] strEvent = stage.s_MISSION.Split(split);
			for (int i = 0; i < strEvent.Length; i++)
			{
				int nMissionID = 0 ; 
				if( int.TryParse( strEvent[i] , out nMissionID )  )
				{
					//MISSION_TEXT mText = ConstDataManager.Instance.GetRow<MISSION_TEXT> ( nMissionID );
					DataRow row = ConstDataManager.Instance.GetRow( "MISSION_TEXT" , nMissionID);
					//if( mText != null )
					if( row != null )
					{				
						string content = row.Field<string>( "s_CONTENT");
						//string sname = string.Format( " {1}" , nID ,content );
						if( lbl.text.Length > 0  ){
							lbl.text += "\r\n";
						}
						lbl.text += content ;
					}
				}
			}

		}
		//MyTool.SetLabelText (lblStageNameObj, MyTool.GetStoryName (nStageID));




		// stage name
		MyTool.SetLabelText (lblStageNameObj, MyTool.GetStoryName (nStageID));
		//ConstDataManager.Instance.GetRow ("STORY_NAME" , nStageID );

	}
	//=================

	void OnCloseClick(GameObject go)
	{
		PanelManager.Instance.CloseUI (Panel_StageInfo.Name);
	}

}
