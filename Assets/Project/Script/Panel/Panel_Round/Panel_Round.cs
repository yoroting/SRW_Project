using UnityEngine;
using System.Collections;
//using _SRW;

public class Panel_Round : MonoBehaviour {

	public const string Name = "Panel_Round";

	public GameObject ContentObj; // back ground
	public GameObject RoundObj; // plane of all tiles sprite


	void OnEnable()
	{
		// if it call when open event done
		if( GameDataManager.Instance.nRound == 0 )
			GameDataManager.Instance.nRound = 1;

        UILabel lblcont = ContentObj.GetComponent< UILabel >();
		if( lblcont != null )
		{

			switch( GameDataManager.Instance.nActiveCamp )
			{
				case _CAMP._PLAYER:{
					lblcont.text = "我方行動";
				}break;
				case _CAMP._ENEMY:{
					lblcont.text = "敵方行動";
				}break;
				case _CAMP._FRIEND:{
					lblcont.text = "友方行動";
				}break;
			}
		}

		UILabel lblround = RoundObj.GetComponent< UILabel >();
		if( lblround != null )
		{
			string strbase = "回合 $V1";
			string param = GameDataManager.Instance.nRound.ToString();
			string round = strbase.Replace( "$V1" , param );

			lblround.text = round;
		}
	}

	// Use this for initialization
	void Start () {
        //// 如果沒 友方，直接關閉
        //if (GameDataManager.Instance.nActiveCamp == _CAMP._FRIEND)
        //{
        //    int nCount = GameDataManager.Instance.GetCampNum(_CAMP._FRIEND);
        //    if (nCount <= 0)
        //    {
        //        PanelManager.Instance.DestoryUI(Name); // can't destory in enable
        //        return;
        //    }
        //}
    }

	// Update is called once per frame
	void Update () {	


	}


	public void OnReady()
	{


	}

	public void OnFadeOut()
	{
        // reopen roundcheck
        Panel_StageUI.Instance.bIsAutoPopRoundCheck = false;

        System.GC.Collect();			// Free memory each round end
       
        // close UI
        //PanelManager.Instance.CloseUI( Name );
        PanelManager.Instance.DestoryUI( Name );

		
	}

}
