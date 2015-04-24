using UnityEngine;
using System;

/// <summary>
/// 控制 Particle 的 Queue 值，以配合 NGUI 深度
/// （ 3D 模型也可用 Queue 值）
/// </summary>
public class ParticleQueue : MonoBehaviour
{
	public int RenderQStartNum = 0;

	ParticleSystem particle;

	void Awake ()
	{
		particle = gameObject.GetComponent<ParticleSystem>();
		if(particle != null)
			particle.GetComponent<Renderer>().sharedMaterial.renderQueue = RenderQStartNum;

		if( particle == null )
		{
			ParticleRenderer renderer = gameObject.GetComponent<ParticleRenderer>();
			if( renderer != null )
				renderer.sharedMaterial.renderQueue = RenderQStartNum;
		}
	}
}
