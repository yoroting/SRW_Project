using UnityEngine;
using System;
using System.Collections;
/// <summary>
/// 子彈的路徑Tween
/// 引力演算方式（要再調整）
/// </summary>
public class FightBulletTween_Attraction : MonoBehaviour {
	
	private Vector3 targetPos;
	/// <summary>速度值</summary>
	private float speed;
	private Action endFunc;
	/// <summary>當前移動速度</summary>
	private Vector3 nowSpeed;

	void Start(){
	}

	void Update(){
//		nowSpeed*=0.9f;	//初速度干擾衰減
		Vector3 nowPos = transform.localPosition;
		float angle = Mathf.Atan2(targetPos.y-nowPos.y, targetPos.x-nowPos.x);
		float speedVal = speed*GetDistanceValue(nowPos, targetPos);
		float speedVx = speedVal * (float)Math.Cos((double)angle);
		float speedVy = speedVal * (float)Math.Sin((double)angle);
		nowSpeed.x += speedVx;
		nowSpeed.y += speedVy;
		nowPos += nowSpeed;

		bool atTargetPos = (Math.Abs(nowPos.x-targetPos.x) < speed && Math.Abs(nowPos.y-targetPos.y) < speed );
		if(atTargetPos)
			nowPos = targetPos;
		
		transform.localPosition = nowPos;
		if(atTargetPos){
			if(endFunc != null){
				endFunc();
				endFunc = null;
			}
			enabled = false;
		}
	}

	/// <summary>取得距離影響速度的參數，距離越近參數越大 0.01~1</summary>
	private static float GetDistanceValue(Vector3 a, Vector3 b){
		float dx = b.x-a.x;
		float dy = b.y-a.y;
		float distance = dx*dx+dy*dy;
		if(distance == 0)
			return 1;
		float val = 1f/(distance*0.00001f);
		if(val > 1)
			return 1;
		if(val <= 0.01f)
			return 0.01f;
		return val;
	}

	public static FightBulletTween_Attraction Begin(GameObject bullet, Vector3 defaultSpeed, Vector3 targetPos, float speed, Action endFunc = null){
		FightBulletTween_Attraction tween = bullet.AddComponent<FightBulletTween_Attraction>();
		tween.nowSpeed = defaultSpeed;
		tween.targetPos = targetPos;
		tween.speed = speed;
		tween.endFunc = endFunc;
		return tween;
	}
}


