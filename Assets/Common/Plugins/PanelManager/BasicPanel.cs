using UnityEngine;
using System;
using System.Collections;

public class BasicPanel : MonoBehaviour {

	/// <summary>播放顯示特效，結束時一定要呼叫 endFunc</summary>
	public virtual void PlayOpenEffect(Action endFunc = null)
	{
		if(endFunc != null)
			endFunc();
	}

	
	/// <summary>播放關閉特效，結束時一定要呼叫 endFunc</summary>
	public virtual void PlayCloseEffect(Action endFunc = null)
	{
		if(endFunc != null)
			endFunc();
	}
}
