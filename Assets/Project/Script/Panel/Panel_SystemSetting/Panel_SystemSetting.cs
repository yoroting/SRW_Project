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

     string skeySound = "muteSound";
     string skeyMusic = "muteMusic";

    // Use this for initialization
    void Start () {
        Initializ();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Initializ()
    {

        if (GetPlayerPrefsKey(skeyMusic))
        {
            NGUITools.SetActive(Btn_MusicOn, true);
            NGUITools.SetActive(Btn_MusicOff, false);
        }
        else
        {
            NGUITools.SetActive(Btn_MusicOn, false);
            NGUITools.SetActive(Btn_MusicOff, true);
        }


        if (GetPlayerPrefsKey( skeySound ))
        {
            NGUITools.SetActive(Btn_SoundOn, true);
            NGUITools.SetActive(Btn_SoundOff, false);
        }
        else
        {
            NGUITools.SetActive(Btn_SoundOn, false);
            NGUITools.SetActive(Btn_SoundOff, true);
        }


        UIEventListener.Get(Btn_SoundOn).onClick = OnSoundOnClicked; // for trig next line
        UIEventListener.Get(Btn_SoundOff).onClick = OnSoundOffClicked; // for trig next line
        UIEventListener.Get(Btn_MusicOn).onClick = OnMusicOnClicked; // for trig next line
        UIEventListener.Get(Btn_MusicOff).onClick = OnMusicOffClicked; // for trig next line

        UIEventListener.Get(Btn_Close).onClick = OnCloseClicked; // for trig next line
        UIEventListener.Get(Btn_Return).onClick = OnReturnClicked; // for trig next line
    }

    void SetPlayerPrefsKey(string sKey, bool bOn = true)
    {
        PlayerPrefs.SetInt( sKey , (bOn ? 1:0) );
    }

    bool GetPlayerPrefsKey(string sKey)
    {
        if (PlayerPrefs.HasKey(sKey)) {
            if (PlayerPrefs.GetInt(sKey) > 0) {
                return true;
            }

        }
        return false;
    }

    void MuteSound( bool bMute = false )
    {
        if (bMute == true)
        {
            NGUITools.SetActive(Btn_SoundOn, true);
            NGUITools.SetActive(Btn_SoundOff, false);
        }
        else {

            NGUITools.SetActive(Btn_SoundOn, false);
            NGUITools.SetActive(Btn_SoundOff, true);
        }
        SetPlayerPrefsKey( skeyMusic , bMute );
        AudioManager.Instance.SetChannelMute(AudioChannelType.SoundFX , bMute );
    }

    void MuteMusic(bool bMute = false)
    {
        if (bMute == true)
        {
            //Btn_MusicOn.SetActive( true );
            //Btn_MusicOff.SetActive( false );
            NGUITools.SetActive(Btn_MusicOn, true);
            NGUITools.SetActive(Btn_MusicOff, false);
        }
        else
        {            
            //Btn_MusicOn.SetActive(false);
            //Btn_MusicOff.SetActive(true);
            NGUITools.SetActive(Btn_MusicOn, false);
            NGUITools.SetActive(Btn_MusicOff, true);
        }
        SetPlayerPrefsKey(skeyMusic, bMute);
        AudioManager.Instance.SetChannelMute(AudioChannelType.BGM, bMute);
    }

    void OnMusicOnClicked(GameObject btn)
    {
        MuteMusic(false);
    }
    void OnMusicOffClicked(GameObject btn)
    {
        MuteMusic(true);
    }
    void OnSoundOnClicked(GameObject btn)
    {
        MuteSound(false);
    }
    void OnSoundOffClicked(GameObject btn)
    {
        MuteSound(true);
    }
    void OnCloseClicked(GameObject btn)
    {
        PanelManager.Instance.CloseUI(Name);
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
        
        // 一定會關閉
         PanelManager.Instance.CloseUI(Name);
           
    }
    


}
