using UnityEngine;
using System.Collections;

public class AudioManagerSample : MonoBehaviour {
#if UNITY_EDITOR

	public bool TestAudioBGM;

	void Awake () {
		AudioManager.Instance.Initial(LoadClipFunc);
	}

	void Update(){
		if(TestAudioBGM){
			AudioManager.Instance.Play(AudioChannelType.BGM, "Assets/Common/Plugins/AudioManager/Sample/Editor/AudioManagerSampleAudio.wav");
		}else{
			AudioManager.Instance.Stop(AudioChannelType.BGM);
		}
	}
	
	private static AudioClip LoadClipFunc(string audioPath)
	{
		AudioClip clip = Resources.Load<AudioClip>(audioPath);
		if(clip == null){
			Debug.LogError("音效資源讀取失敗:" + audioPath);
		}
		return clip; 
	}

#endif
}
