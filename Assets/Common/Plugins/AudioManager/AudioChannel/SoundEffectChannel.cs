using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 音效頻道，可同時播放多種聲音，但相同的聲音同一時間只會有一個，下一個會取代目前播放的
/// 播放的聲音預設只播一次，播完後不會主動清除快取
/// </summary>
public class SoundEffectChannel : AudioChannelBase
{
	private List<AudioSource> audioSourceList;

	protected override void OnStopAudioClip (AudioClip clip)
	{
		//停止並刪除聲音
		AudioSource existAudio = this.FindAudioSource(clip);
		
		if (existAudio != null)
		{
			audioSourceList.Remove(existAudio);

			existAudio.Stop();
			existAudio.clip = null;

			//回收
			NGUITools.Destroy(existAudio);
		}
	}
	
	protected override void OnPlayAudioClip (AudioClip clip)
	{
		if (audioSourceList == null)
			audioSourceList = new List<AudioSource>();
		
		AudioSource existAudio = FindAudioSource(clip);
		
		if (existAudio == null)
		{
			//播放聲音
			AudioSource newAudio =  gameObject.AddComponent<AudioSource>();
			
			newAudio.loop = false;
			newAudio.clip = clip;
			newAudio.playOnAwake = false;
			newAudio.volume = Volume;
			newAudio.mute = IsMute;
			
			audioSourceList.Add(newAudio);
			
			newAudio.Play();
		}
		else
		{
			//已存在的聲音，再播放
			existAudio.Play();
		}
	}

    protected override bool IsAudioClipPlaying(AudioClip clip)  // check audio is play done
    {
        AudioSource existAudio = FindAudioSource(clip);

        if (existAudio == null)
        {
            return false;
        }

        if (clip != existAudio.clip) // not current clip
            return false;


        if (existAudio.isPlaying && !existAudio.mute)
        {
            return true;
        }

        return false;
    }

    private AudioSource FindAudioSource(AudioClip clip)
	{
		if (audioSourceList == null)
			return null;

		foreach (var audio in audioSourceList)
		{
			if (audio.clip == clip)
				return audio;
		}
		return null;
	}
	
	protected override void OnVolumeChanged ()
	{
		if (audioSourceList == null)
			return;

		foreach (var audio in audioSourceList)
		{
			audio.volume = Volume;
		}
	}
	
	protected override void OnMuteChanged ()
	{
		if (audioSourceList == null)
			return;

		foreach (var audio in audioSourceList)
		{
			audio.mute = IsMute;
		}
	}
}

