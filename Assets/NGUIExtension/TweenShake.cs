using UnityEngine;
using System.Collections;

public class TweenShake : UITweener {

	Transform mTrans;
	
	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }
	public Vector3 position { get { return cachedTransform.localPosition; } set { cachedTransform.localPosition = value; } }
	public Vector3 v3PosOld { 
		get { 
			if(!OriginPosSet)
			{
				mPos = position;
				OriginPosSet = true;
			}
			return mPos; 
		} 
		set { 
			if(!OriginPosSet)
			{
				mPos = value;
				OriginPosSet = true;
			}
		} 
	}

	public float fPosRadius = 10f;
	public int playPeriod = 1;	// play once every ? frame.
	public float stopAfterSec = 999;	//after ? second will stop and hold.
	public bool shakeX = true;
	public bool shakeY = true;

	private int counter = 1;
	private float totalTime = 0;
	private Vector3 mPos;
	public bool OriginPosSet = false;

	override protected void OnUpdate (float factor, bool isFinished) {

		float delta = ignoreTimeScale ? RealTime.deltaTime : Time.deltaTime;
		totalTime += delta;

		if(!isFinished && (totalTime < stopAfterSec) && delta > 0f){
			if(counter >= playPeriod)
			{
				Vector3 v3Pos = v3PosOld ;
				if(shakeX)
				{
					if(v3Pos.x > v3PosOld.x){
						v3Pos.x += Random.Range(-fPosRadius,0f);
					}else{
						v3Pos.x += Random.Range(0f,fPosRadius);
					}
				}
				if(shakeY)
				{
					if(v3Pos.y > v3PosOld.y){
						v3Pos.y += Random.Range(-fPosRadius,0);
					}else{
						v3Pos.y += Random.Range(0f,fPosRadius);
					}
				}
				position = v3Pos;
				counter = 1;
			}else{
				counter++;
			}
		}else{
			//恢复原来位置
			position = v3PosOld;
		}
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>
	
	static public TweenShake Begin (GameObject go, float duration, float radius, int period = 1)
	{
		TweenShake comp = UITweener.Begin<TweenShake>(go, duration);
		comp.fPosRadius = radius;
		comp.playPeriod = period;
		comp.stopAfterSec = duration;
		comp.totalTime = 0f;

		comp.shakeX = true;
		comp.shakeY = true;

		comp.OriginPosSet = false;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}

}
