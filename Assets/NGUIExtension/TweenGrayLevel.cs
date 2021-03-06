﻿using UnityEngine;
using System.Collections;

[AddComponentMenu("NGUI/Tween/Tween Graylevel")]
public class TweenGrayLevel : UITweener {
	public float from;
	public float to;
    public float fPoint; 

	UITexture mUITexture;
	public UITexture CachedUITexture { get { if (mUITexture == null) mUITexture = GetComponent<UITexture>(); return mUITexture; } }
	public Material material { get { return CachedUITexture.drawCall.dynamicMaterial; } }
	override protected void OnUpdate (float factor, bool isFinished) {		
		float newPoint = from * (1f - factor) + to * factor;
        fPoint = newPoint;

        if (CachedUITexture == null) return;
		if(CachedUITexture.drawCall == null) return;
		if(material == null) return;

       
    	material.SetFloat("_GrayLevelScale", newPoint);
    }
	
	/// <summary>
	/// Start the tweening operation.
	/// </summary>
	
	public static TweenGrayLevel Begin (GameObject go, float duration, float newPoint)
	{
		TweenGrayLevel comp = UITweener.Begin<TweenGrayLevel>(go, duration); 
		// Change shader at run time.
//		comp.material.shader = Shader.Find("Custom/GrayLevel");

		comp.from = comp.material.GetFloat("_GrayLevelScale");
		comp.to = newPoint;
		
		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}
}
