//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright 穢 2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's position.
/// </summary>

[AddComponentMenu("NGUI/Tween/TweenX")]
public class TweenX : UITweener
{
	public float from;
	public float to;
	public bool isRandom = false;
	bool mHasChanged = false;
	
	Transform mTrans;
	
	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }
	public Vector3 position { get { return cachedTransform.localPosition; } set { cachedTransform.localPosition = value; } }

	override protected void OnUpdate (float factor, bool isFinished) {
		if (isRandom && !mHasChanged) {
			to = UnityEngine.Random.Range(-to, to);
			mHasChanged = true;
		}
		
		float newPoint = from * (1f - factor) + to * factor;
		
		cachedTransform.localPosition = new Vector3(newPoint, cachedTransform.localPosition.y, cachedTransform.localPosition.z);
		
		if (isFinished) mHasChanged = false;
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenX Begin (GameObject go, float duration, float newPoint)
	{
		TweenX comp = UITweener.Begin<TweenX>(go, duration);

		comp.from = comp.position.x;
		comp.to = newPoint;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}
}
