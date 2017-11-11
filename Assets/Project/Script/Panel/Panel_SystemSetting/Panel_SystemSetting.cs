using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_SystemSetting : MonoBehaviour {

    public const string Name = "Panel_SystemSetting";

    public GameObject Btn_Close;
    public GameObject Btn_Return;

    public GameObject Btn_SoundOn;
    public GameObject Btn_SoundOff;
    public GameObject Btn_MusicOn;
    public GameObject Btn_MusicOff;

    public GameObject Btn_TextFast;
    public GameObject Btn_TextSlow;

    public GameObject Btn_MoveFast;
    public GameObject Btn_MoveSlow;

//    static string skeySound = "muteSound";
//    static string skeyMusic = "muteMusic";
    static string skeyTextSpeed = "TextSpeed";
    static string skeyMoveSpeed = "MoveSpeed";

    // Use this for initialization
    void Start () {
        Initializ();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Initializ()
    {
        
        //if (GetPlayerPrefsKey(skeyMusic))
        if(AudioManager.Instance.IsChannelMute(AudioChannelType.BGM) )
        {
            NGUITools.SetActive(Btn_MusicOn, false);
            NGUITools.SetActive(Btn_MusicOff, true);
        }
        else
        {
            NGUITools.SetActive(Btn_MusicOn, true);
            NGUITools.SetActive(Btn_MusicOff, false);

            
        }
        if (AudioManager.Instance.IsChannelMute(AudioChannelType.SoundFX) )
        {
            NGUITools.SetActive(Btn_SoundOn, false);
            NGUITools.SetActive(Btn_SoundOff, true);
        }
        else
        {
            NGUITools.SetActive(Btn_SoundOn, true);
            NGUITools.SetActive(Btn_SoundOff, false);

            
        }

        //SetPlayerPrefsKey(skeyTextSpeed, false);
        if (GetPlayerPrefsKey(skeyTextSpeed))
        {
            NGUITools.SetActive(Btn_TextFast, true);
            NGUITools.SetActive(Btn_TextSlow, false);
            Config.TextFast = true;
        }
        else
        {
            NGUITools.SetActive(Btn_TextFast, false);
            NGUITools.SetActive(Btn_TextSlow, true);
            Config.TextFast = true;
        }

       // SetPlayerPrefsKey(skeyMoveSpeed , false );
        if (GetPlayerPrefsKey(skeyMoveSpeed))
        {
            NGUITools.SetActive(Btn_MoveFast, true);
            NGUITools.SetActive(Btn_MoveSlow, false);
            Config.MoveFast = true;
        }
        else
        {
            NGUITools.SetActive(Btn_MoveFast, false);
            NGUITools.SetActive(Btn_MoveSlow, true);
            Config.MoveFast = false;
        }



        UIEventListener.Get(Btn_SoundOn).onClick = OnSoundOnClicked; // for trig next line
        UIEventListener.Get(Btn_SoundOff).onClick = OnSoundOffClicked; // for trig next line
        UIEventListener.Get(Btn_MusicOn).onClick = OnMusicOnClicked; // for trig next line
        UIEventListener.Get(Btn_MusicOff).onClick = OnMusicOffClicked; // for trig next line

        UIEventListener.Get(Btn_TextFast).onClick = OnTextSlowClicked; // for trig next line
        UIEventListener.Get(Btn_TextSlow).onClick = OnTextFastClicked; // for trig next line

        UIEventListener.Get(Btn_MoveFast).onClick = OnMoveSlowClicked; // for trig next line
        UIEventListener.Get(Btn_MoveSlow).onClick = OnMoveFastClicked; // for trig next line


        UIEventListener.Get(Btn_Close).onClick = OnCloseClicked; // for trig next line
        UIEventListener.Get(Btn_Return).onClick = OnReturnClicked; // for trig next line
    }
    // 

    void SetPlayerPrefsKey(string sKey, bool bOn = true)
    {
        PlayerPrefs.SetInt( sKey , (bOn ? 1:0) );
    }

    static bool GetPlayerPrefsKey(string sKey)
    {
        if (PlayerPrefs.HasKey(sKey)) {
            if (PlayerPrefs.GetInt(sKey) > 0) {
                return true;
            }

        }
        return false;
    }

    static public void LoadConfig()
    {
        Config.TextFast = GetPlayerPrefsKey(skeyTextSpeed);
        Config.MoveFast = GetPlayerPrefsKey(skeyMoveSpeed);
    }

    static public void OpenUI()
    {
        PanelManager.Instance.OpenUI(Panel_SystemSetting.Name);
        
    }


    void MuteSound( bool bMute = false )
    {
        if (bMute == true)
        {
            NGUITools.SetActive(Btn_SoundOn, false);
            NGUITools.SetActive(Btn_SoundOff, true);
        }
        else {

            NGUITools.SetActive(Btn_SoundOn, true);
            NGUITools.SetActive(Btn_SoundOff, false);
           
        }
    //    SetPlayerPrefsKey( skeyMusic , bMute );
        AudioManager.Instance.SetChannelMute(AudioChannelType.SoundFX , bMute );
    }

    void MuteMusic(bool bMute = false)
    {
        if (bMute == true)
        {
            //Btn_MusicOn.SetActive( true );
            //Btn_MusicOff.SetActive( false );
            NGUITools.SetActive(Btn_MusicOn, false);
            NGUITools.SetActive(Btn_MusicOff, true);
        }
        else
        {
            NGUITools.SetActive(Btn_MusicOn, true);
            NGUITools.SetActive(Btn_MusicOff, false);
            //Btn_MusicOn.SetActive(false);
            //Btn_MusicOff.SetActive(true);
           
        }
    //    SetPlayerPrefsKey(skeyMusic, bMute);
        AudioManager.Instance.SetChannelMute(AudioChannelType.BGM, bMute);
    }


    void TextSpeed(bool bFast = false)
    {
        if (bFast == true)
        {
            //Btn_MusicOn.SetActive( true );
            //Btn_MusicOff.SetActive( false );
            NGUITools.SetActive(Btn_TextFast, true);
            NGUITools.SetActive(Btn_TextSlow, false);
        }
        else
        {
            NGUITools.SetActive(Btn_TextFast, false);
            NGUITools.SetActive(Btn_TextSlow, true);
            //Btn_MusicOn.SetActive(false);
            //Btn_MusicOff.SetActive(true);

        }
        SetPlayerPrefsKey(skeyTextSpeed, bFast);
        Config.TextFast = bFast;
        //AudioManager.Instance.SetChannelMute(AudioChannelType.BGM, bMute);
    }

    void MoveSpeed(bool bFast = false)
    {
        if (bFast == true)
        {
            //Btn_MusicOn.SetActive( true );
            //Btn_MusicOff.SetActive( false );
            NGUITools.SetActive(Btn_MoveFast, true);
            NGUITools.SetActive(Btn_MoveSlow, false);
        }
        else
        {
            NGUITools.SetActive(Btn_MoveFast, false);
            NGUITools.SetActive(Btn_MoveSlow, true);
            //Btn_MusicOn.SetActive(false);
            //Btn_MusicOff.SetActive(true);

        }
        SetPlayerPrefsKey(skeyMoveSpeed, bFast);
        Config.MoveFast = bFast;
        //AudioManager.Instance.SetChannelMute(AudioChannelType.BGM, bMute);
    }


    void OnMusicOnClicked(GameObject btn)
    {
        MuteMusic(true);
        GameSystem.BtnSound();
    }
    void OnMusicOffClicked(GameObject btn)
    {
        MuteMusic(false);
        GameSystem.BtnSound();
    }
    void OnSoundOnClicked(GameObject btn)
    {
        MuteSound(true);
        GameSystem.BtnSound();
    }
    void OnSoundOffClicked(GameObject btn)
    {
        MuteSound(false);
        GameSystem.BtnSound();
    }

    void OnTextFastClicked(GameObject btn)
    {
        TextSpeed(true);
        GameSystem.BtnSound();
    }
    void OnTextSlowClicked(GameObject btn)
    {
        TextSpeed(false);
        GameSystem.BtnSound();
    }

    void OnMoveFastClicked(GameObject btn)
    {
        MoveSpeed(true);
        GameSystem.BtnSound();
    }
    void OnMoveSlowClicked(GameObject btn)
    {
        MoveSpeed(false);
        GameSystem.BtnSound();
    }

    void OnCloseClicked(GameObject btn)
    {
        PanelManager.Instance.CloseUI(Name);
        GameSystem.BtnSound( 1 );
    }
    void OnReturnClicked(GameObject btn)
    {
        //  PanelManager.Instance.CloseUI(Name);
        if (PanelManager.Instance.CheckUIIsOpening(Panel_StageUI.Name))
        {
            // close stage ui            
            // entry endstage
            Panel_StageUI.Instance.EndStage();

            Panel_StageUI.Instance.ShowStage(false);
            // free here waill cause some  StartCoroutine of stageUI break 
            PanelManager.Instance.DestoryUI(Panel_StageUI.Name);

            // reopen main UI
            PanelManager.Instance.OpenUI(MainUIPanel.Name);


        }
        else if (PanelManager.Instance.CheckUIIsOpening(Panel_Mainten.Name)) {

            PanelManager.Instance.OpenUI(MainUIPanel.Name);

            PanelManager.Instance.CloseUI(Panel_Mainten.Name);

        }
        // 一定會關閉
         PanelManager.Instance.CloseUI(Name);
         GameSystem.BtnSound(1);
    }
    


}
