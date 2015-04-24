using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>預設存在的 Channel Type</summary>
public enum AudioChannelType
{
	BGM,
	SoundFX,
}

[RequireComponent(typeof(AudioListener))]
public class AudioManager : Singleton<AudioManager>
{
	public delegate AudioClip GetAudioClipDelegate (string audioPath);

	private bool hadInit;
	public bool HadInit{ get{ return hadInit; } }

	public GetAudioClipDelegate GetAudioClipFunc;

	private Dictionary<int, AudioChannelBase> channels = new Dictionary<int, AudioChannelBase>();

	public void Initial(GetAudioClipDelegate getAudioClipFunc){
		hadInit = true;
		this.GetAudioClipFunc = getAudioClipFunc;
	}

	/// <summary>
	/// 整體音量
	/// </summary>
	/// <value>The volume.</value>
	public float Volume 
	{
		get {return AudioListener.volume;}
		set
		{
			AudioListener.volume = value;

			PlayerPrefs.SetFloat("volumeAudioManager", AudioListener.volume);
		}
	}

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		AudioListener.volume = PlayerPrefs.GetFloat("volumeAudioManager", 1f);
		CreateChannel<BGMChannel>(AudioChannelType.BGM);
		CreateChannel<SoundEffectChannel>(AudioChannelType.SoundFX);
	}

	/// <summary>
	/// 播放聲音到指定頻道
	/// </summary>
	/// <param name="channel">Channel.</param>
	/// <param name="path">Path.</param>
	public void Play(AudioChannelType channelType, string path) { Play((int) channelType, path); }
	public void Play(int channel, string path)
	{
		AudioChannelBase audioChannel = null;

		if (!channels.TryGetValue(channel, out audioChannel))
			return;

		audioChannel.Play(path);
	}
	
	public void Play(AudioChannelType channelType, AudioClip clip) { Play((int) channelType, clip); }
	public void Play(int channel, AudioClip clip)
	{
		AudioChannelBase audioChannel = null;
		
		if (!channels.TryGetValue(channel, out audioChannel))
			return;
		
		audioChannel.Play(clip);
	}



	/// <summary>
	/// 停止特定頻道的聲音
	/// </summary>
	/// <param name="channel">Channel.</param>
	/// <param name="path">Path.</param>
	public void Stop(AudioChannelType channelType, string path){ Stop((int) channelType, path); }
	public void Stop(int channel, string path)
	{
		AudioChannelBase audioChannel = null;
		
		if (!channels.TryGetValue(channel, out audioChannel))
			return;
		
		audioChannel.Stop(path);
	}

	/// <summary>
	/// 停止頻道內所有聲音
	/// </summary>
	/// <param name="channel">Channel.</param>
	public void Stop(AudioChannelType channelType, bool cleanCacne = true){ Stop((int) channelType, cleanCacne); }
	public void Stop(int channel, bool cleanCacne = true)
	{
		AudioChannelBase audioChannel = null;
		
		if (!channels.TryGetValue(channel, out audioChannel))
			return;

		audioChannel.StopAll(cleanCacne);
	}

	public void Stop(AudioChannelType channelType, AudioClip clip){ Stop((int) channelType, clip); }
	public void Stop(int channel, AudioClip clip)
	{
		AudioChannelBase audioChannel = null;
		
		if (!channels.TryGetValue(channel, out audioChannel))
			return;
		
		audioChannel.Stop(clip);
	}

	/// <summary>
	/// 停止所有頻道指定的聲音
	/// </summary>
	/// <param name="path">Path.</param>
	public void Stop(string path)
	{
		foreach (var pair in channels)
		{
			pair.Value.Stop(path);
		}
	}

	/// <summary>
	/// 停止所有聲音
	/// </summary>
	public void StopAll()
	{
		foreach (var pair in channels)
		{
			pair.Value.StopAll();
		}
	}

	/// <summary>
	/// 依channel id取得頻道
	/// </summary>
	/// <returns>The channel.</returns>
	/// <param name="channel">Identifier.</param>
	public AudioChannelBase FindChannel(AudioChannelType channelType) { return FindChannel((int)channelType); }
	public AudioChannelBase FindChannel(int channel)
	{
		AudioChannelBase audioChannel = null;

		channels.TryGetValue(channel, out audioChannel);

		return audioChannel;
	}

	public bool SetChannelMute(AudioChannelType channelType, bool isMute){ return SetChannelMute((int)channelType, isMute); }
	public bool SetChannelMute(int channel, bool isMute)
	{
		AudioChannelBase audioChannel = FindChannel(channel);
		if(audioChannel == null) return false;

		audioChannel.IsMute = isMute;
		return true;
	}


	/// <summary>
	/// 建立聲音頻道
	/// </summary>
	/// <returns><c>true</c>, if channel was created, <c>false</c> otherwise.</returns>
	/// <param name="channel">Channel.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public bool CreateChannel<T>(AudioChannelType channelType) where T : AudioChannelBase { return CreateChannel<T>((int)channelType); }
	public bool CreateChannel<T>(int channel) where T : AudioChannelBase
	{
		if (channels.ContainsKey(channel))
		{
			Debug.LogWarning("建立聲音頻道失敗，id已存在, id=" + channel);
			return false;
		}

		GameObject go = NGUITools.AddChild(gameObject);

		//名稱必須唯一，使用channel+類別名
		go.name = string.Format("{0}_{1}", channel, typeof(T).ToString());

		T audioChannel = go.AddComponent<T>();

		channels.Add(channel, audioChannel);

		return true;
	}
}

/// <summary>
/// 基礎的聲音頻道，實作資源加載及快取管理
/// </summary>
public abstract class AudioChannelBase : MonoBehaviour
{
	/// <summary>
	/// 是否使用快取
	/// </summary>
	protected bool useCache = true;

	private string channelName;

	private float volume;

	private bool isMute;

	private Dictionary<string, AudioClip> cache;

//	private List<IAssetToken> tokenList;

	public float Volume
	{
		get {return volume;}
		set 
		{
			if (volume == value)
				return;

			volume = value;

			PlayerPrefs.SetFloat(channelName, volume);

			OnVolumeChanged();
		}
	}

	public bool IsMute 
	{
		get {return isMute;}
		set 
		{
			if (isMute == value)
				return;
			
			isMute = value;
			
			PlayerPrefs.SetInt(channelName, isMute ? 1 : 0);
			
			OnMuteChanged();
		}
	}

	protected virtual void Awake()
	{
		this.channelName = gameObject.name;

		this.volume = PlayerPrefs.GetFloat(channelName, .5f);
		this.isMute = PlayerPrefs.GetInt(channelName, 0) == 1;

//		this.tokenList = new List<IAssetToken>();
		this.cache = new Dictionary<string, AudioClip>();
	}

	public void Play(string path)
	{
		if(!AudioManager.Instance.HadInit){
			Debug.LogError("AudioManager 尚未出始化");
			return;
		}
		//檢查是否正在讀取
//		if (FindToken(path) != null)
//			return;

		AudioClip clip = null;

		cache.TryGetValue(path, out clip);
		
		if (clip == null)
		{
			//讀取
			if(AudioManager.Instance.GetAudioClipFunc == null){
				Debug.LogWarning("沒有指定取得 AudioClip 的方法");
			}else{
				clip = AudioManager.Instance.GetAudioClipFunc(path);
			}

			if(clip == null){
				Debug.LogError("播放音效失敗，無法取得 AudioClip 資源：" + path);

			}else{
				//加到快取
				if (useCache)
				{
					if (cache.ContainsKey(path))
						cache[path] = clip;
					else
						cache.Add(path, clip);
				}

				OnPlayAudioClip(clip);
			}



			//接 AssetManager
//			IAssetToken token = AssetManager.Instance.LoadAsset(path);
//			
//			tokenList.Add(token);
//			
//			token.OnError += LoadAudioErrorHandler;
//			token.OnComplete += LoadAudioCompleteHandler;
		}
		else
		{
			//使用快取資料
			OnPlayAudioClip(clip);
		}
	}

	public void Play(AudioClip clip)
	{
		if(!AudioManager.Instance.HadInit){
			Debug.LogError("AudioManager 尚未出始化");
			return;
		}

		if (clip == null)
			return;

		//直接播放不存快取
		OnPlayAudioClip(clip);
	}

	private string FindPathByClip(AudioClip clip)
	{
		foreach (var pair in cache)
		{
			if (pair.Value == clip)
				return pair.Key;
		}
		return null;
	}

//	private IAssetToken FindToken(string path)
//	{
//		foreach (var token in tokenList)
//		{
//			if (token.Path == path)
//				return token;
//		}
//		return null;
//	}

//	private void LoadAudioCompleteHandler(IAssetToken token)
//	{
//		tokenList.Remove(token);
//		
//		AudioClip clip = token.Content as AudioClip;
//		
//		if (clip != null)
//		{
//			//加到快取
//			if (useCache)
//			{
//				if (cache.ContainsKey(token.Path))
//					cache[token.Path] = clip;
//				else
//					cache.Add(token.Path, clip);
//			}
//
//			OnPlayAudioClip(clip);
//		}
//		else
//		{
//			Debug.LogWarning("播放音效失敗, 不正確的檔案格式, path=" + token.Path);
//		}
//	}
	
//	private void LoadAudioErrorHandler(IAssetToken token, string message)
//	{
//		tokenList.Remove(token);
//	}

	public void Stop(AudioClip clip)
	{
		if (clip == null)
			return;

		OnStopAudioClip(clip);
	}

	public void Stop(string path)
	{
//		int num = tokenList.Count;
//		for (int i = 0; i < num; ++i)
//		{
//			if (tokenList[i].Path == path)
//			{
//				//正在讀取，停止載入
//				tokenList.RemoveAt(i);
//				AssetManager.Instance.Stop(path);
//				break;
//			}
//		}

		if (cache.ContainsKey(path))
		{
			//停止播放
			OnStopAudioClip(cache[path]);

			//清除快取及參照
			cache.Remove(path);
//			AssetManager.Instance.DecreaseReferenceCount(path);
		}
	}

	public void StopAll(bool cleanCacne = true)
	{
//		//停止所有的下載
//		foreach (var token in tokenList)
//		{
//			AssetManager.Instance.Stop(token);
//		}
//		tokenList = new List<IAssetToken>();

		//停止所有聲音
		foreach (var pair in cache)
		{
			OnStopAudioClip(pair.Value);
//			AssetManager.Instance.DecreaseReferenceCount(pair.Key);
		}
		if (cleanCacne)
			cache = new Dictionary<string, AudioClip>();
	}

	abstract protected void OnStopAudioClip(AudioClip clip);

	abstract protected void OnPlayAudioClip(AudioClip clip);

	abstract protected void OnVolumeChanged();

	abstract protected void OnMuteChanged();
}