using UnityEngine;
using System;

/// <summary>
/// 控制 Particle 的 Queue 值，以配合 NGUI 深度
/// （ 3D 模型也可用 Queue 值）
/// </summary>
using System.Collections.Generic;


public class AutoParticleQueue : MonoBehaviour
{
	[SerializeField] private int queueOffset = 0;
	private UIPanel targetPanel;
	private GameObject cacheGameObject;
	private List<Renderer> cacheRendererList = new List<Renderer>();
	private int nowRnederQueue = -1;

	/// <summary>做初始設定，並傳回是否成功初始化</summary>
	private bool checkAndInit(){
		
		if(cacheGameObject == null){
			cacheGameObject = gameObject;
			if(cacheGameObject == null){
				return false;
			}
		}
		
		if(targetPanel == null){
			targetPanel = NGUITools.FindInParents<UIPanel>(transform);
			if(targetPanel == null){
				Debug.LogWarning("找不到 Panel", cacheGameObject);
				return false;
			}
		}

		if(cacheRendererList.Count == 0){
			ParticleSystem[] cacheParticleList = cacheGameObject.GetComponentsInChildren<ParticleSystem>();
			if(cacheParticleList != null){
				foreach(ParticleSystem ps in cacheParticleList)
					cacheRendererList.Add(ps.GetComponent<Renderer>());
			}else{
				foreach(ParticleSystem ps in cacheParticleList)
					cacheRendererList.Add(ps.GetComponent<ParticleRenderer>());
			}
			
			if(cacheRendererList.Count == 0){
				Debug.LogWarning("沒有 renderer 物件!");
				return false;
			}
		}
		return true;
	}

	void Start(){
	//void Update(){
		if(!checkAndInit()){
			enabled = false;
			return;
		}

		nowRnederQueue = targetPanel.startingRenderQueue + queueOffset;
		SetQueue(nowRnederQueue);
	}

	void OnDisable(){
		cacheGameObject = null;
		cacheRendererList.Clear();
	}

	public void SetQueueOffset( int nOffset ){
		queueOffset = nOffset;
	}

	private void SetQueue(int queue){
		if(cacheGameObject == null)
			return;
		if(cacheRendererList == null)
			return;

		foreach(Renderer renderer in cacheRendererList)
			renderer.sharedMaterial.renderQueue = queue;
	}
}
