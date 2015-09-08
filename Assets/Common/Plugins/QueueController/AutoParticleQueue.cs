using UnityEngine;
using System;

/// <summary>
/// 控制 Particle 的 Queue 值，以配合 NGUI 深度
/// （ 3D 模型也可用 Queue 值）
/// </summary>
public class AutoParticleQueue : MonoBehaviour
{
	[SerializeField] private int queueOffset = 0;
	private UIPanel targetPanel;
	private GameObject cacheGameObject;
	private Renderer cacheRenderer;
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

		if(cacheRenderer == null){
			ParticleSystem cacheParticle = cacheGameObject.GetComponent<ParticleSystem>();
			if(cacheParticle != null){
				cacheRenderer = cacheParticle.GetComponent<Renderer>();
			}else{
				cacheRenderer = cacheGameObject.GetComponent<ParticleRenderer>();
			}
			
			if(cacheRenderer == null){
				Debug.LogWarning("沒有 renderer 物件!");
				return false;
			}
		}
		return true;
	}

	void Update(){
		if(!checkAndInit()){
			enabled = false;
			return;
		}

		nowRnederQueue = targetPanel.startingRenderQueue + queueOffset;
		SetQueue(nowRnederQueue);
	}

	void OnDisable(){
		cacheGameObject = null;
		cacheRenderer = null;
	}

	public void SetQueueOffset( int nOffset ){
		queueOffset = nOffset;
	}

	private void SetQueue(int queue){
		if(cacheGameObject == null)
			return;
		if(cacheRenderer == null)
			return;

		cacheRenderer.sharedMaterial.renderQueue = queue;
	}
}
