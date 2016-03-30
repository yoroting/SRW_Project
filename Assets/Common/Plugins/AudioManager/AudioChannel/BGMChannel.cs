using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 背景音樂頻道，一次只播放一首，切換時自動FadeOut、FadeIn
/// </summary>
public class BGMChannel : AudioChannelBase
{
	private float duration = .5f;
	
	private AudioSource currentAudio;
	
	private AudioClip nextClip;
	
	private TweenVolume fadeOutTween;
	
	protected override void OnStopAudioClip (AudioClip clip)
	{
		if (currentAudio != null)
		{
			currentAudio.Stop();
			currentAudio.clip = null;
		}
	}

	protected override void Awake ()
	{
//		useCache = false;
		base.Awake ();
	}

	void Update()
	{
		//手動檢查是否播放完成
		if (currentAudio != null && currentAudio.enabled && !currentAudio.isPlaying && currentAudio.clip != null && nextClip == null)
		{
			//播放完成且沒下一首，循環播放
			currentAudio.Play();
		}
	}
	
	protected override void OnPlayAudioClip (AudioClip clip)
	{
		if (currentAudio == null)
			currentAudio = gameObject.AddComponent<AudioSource>();

		if (clip == currentAudio.clip)
			return;

		//置換下一首
		this.nextClip = clip;
		
		if (currentAudio.isPlaying && !currentAudio.mute)
		{
			if (fadeOutTween == null)
			{
				//FadeOut，TweenVolume到0.01以下會自動關掉聲音，所以改為0.02f
				fadeOutTween = TweenVolume.Begin(gameObject, duration, 0.02f);
				EventDelegate.Set(fadeOutTween.onFinished, FadeOutEndHandler);
			}
		}
		else
		{
			FadeOutEndHandler();
		}
	}

    protected override bool IsAudioClipPlaying(AudioClip clip)  // check audio is play done
    {
        if (currentAudio == null)
            return false;

        if (clip != currentAudio.clip) // not current clip
            return false;
        

        if (currentAudio.isPlaying && !currentAudio.mute)
        {
            return true;
        }
        
        return false;
    }

    private void FadeOutEndHandler()
	{
		if (fadeOutTween != null)
		{
			EventDelegate.Remove(fadeOutTween.onFinished, FadeOutEndHandler);
			fadeOutTween = null;
		}

		if (nextClip != null)
		{
			currentAudio.loop = false; //預設的循環設定在串流播放時會回到最後串流的起始（bug），導致循環播放最後的串流區間
			currentAudio.clip = nextClip;
			currentAudio.volume = 0.02f;
			currentAudio.playOnAwake = false;
			currentAudio.mute = IsMute;

			nextClip = null;
			
			currentAudio.Play();
			
			//FadeIn
			TweenVolume.Begin(gameObject, duration, Volume);
		}
	}
	
	protected override void OnVolumeChanged ()
	{
		if (currentAudio != null)
		{
			currentAudio.volume = Volume;

			currentAudio.enabled = Volume > 0.01f;
		}
	}
	
	protected override void OnMuteChanged ()
	{
		if (currentAudio != null)
			currentAudio.mute = IsMute;
	}
}
