using UnityEngine;
using System.Collections;

public class AudioPlayer : MonoBehaviour
{
	public AudioChannelType audioChannelType = AudioChannelType.SoundFX;

	public string audioPath;

	public float delay;

	public bool autoPlay = true;

	void Start ()
	{
		if (autoPlay)
			Play();
	}

	void OnDisable()
	{
		CancelInvoke("PlaySound");
	}

	private void PlaySound()
	{
		AudioManager.Instance.Play((int)audioChannelType, audioPath);
	}

	public void Play()
	{
		if (!string.IsNullOrEmpty(audioPath))	
		{
			if (delay > 0)
				Invoke("PlaySound", delay);
			else
				PlaySound();
		}
	}
}
