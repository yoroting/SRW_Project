using UnityEngine;
using System;
using System.Collections;
/// <summary>
/// 子彈的路徑Tween
/// 方向指向方式
/// </summary>
public class FightBulletTween_Direction : MonoBehaviour {

	private Transform cachedTransForm;

	private Vector3 targetPos;
	public event Action OnEndFunc;

	private bool isTweenEnd;
	public bool IsTweenEnd{ get{ return isTweenEnd; } }

	/// <summary>移動速度值</summary>
	private float moveSpeed = 3f; // 30.0f
	/// <summary>目標移動速度值</summary>
	private float targetMoveSpeed = 20f;
	/// <summary>移動加速度</summary>
	private float addSpeed = 0.1f;
	
	/// <summary>當前移動方向（角度）</summary>
	private float nowAngle;
	/// <summary>轉向速度值（變更角度的速度）</summary>
	private float angleSpeed = 8.0f; // 8.0f
	/// <summary>轉向加速度值（變更角度速度的加速度）</summary>
	private float angleAddSpeed = 0.1f; // 0.1f


	void Start(){
		cachedTransForm = transform;
	}

	void Update(){
		Vector3 nowPos = cachedTransForm.localPosition;

		//旋轉移動角度
		float targetAngle = FormatAngle360(GetAngle(nowPos, targetPos));
		if(targetAngle != nowAngle){
			float includedAngle = GetIncludedAngle(nowAngle, targetAngle);
			if(includedAngle == 0){
				//Do Nothing
			}else if(includedAngle > 0){
				if(angleSpeed > includedAngle){
					nowAngle += includedAngle;
				}else{
					nowAngle += angleSpeed;
				}
			}else{
				if(-angleSpeed < includedAngle){
					nowAngle += includedAngle;
				}else{
					nowAngle += -angleSpeed;
				}
			}
			angleSpeed+=angleAddSpeed;
		}

		//加減速度
		if(moveSpeed != targetMoveSpeed){
			if(moveSpeed < targetMoveSpeed){
				moveSpeed += addSpeed;
				if(moveSpeed > targetMoveSpeed)
					moveSpeed = targetMoveSpeed;
			}else{
				moveSpeed -= addSpeed;
				if(moveSpeed < targetMoveSpeed)
					moveSpeed = targetMoveSpeed;
			}
		}
		
		//計算移動分量
		float dx = (float)Math.Cos(AngleToRadians(nowAngle)) * moveSpeed;
		float dy = (float)Math.Sin(AngleToRadians(nowAngle)) * moveSpeed;
		nowPos.x += dx;
		nowPos.y += dy;
		
		bool atTargetPos = (Math.Abs(nowPos.x-targetPos.x) < moveSpeed && Math.Abs(nowPos.y-targetPos.y) < moveSpeed );
		
		if(atTargetPos)
			nowPos = targetPos;
		
		cachedTransForm.localPosition = nowPos;

		if(atTargetPos){
			isTweenEnd = true;
			if(OnEndFunc != null){
				OnEndFunc();
				OnEndFunc = null;
			}
//			if(endTimer != null){
//				endTimer.doEndFunc();
//				endTimer = null;
//			}
			enabled = false;
		}
	}

	/// <summary>角度轉徑度</summary>
	private static float AngleToRadians(float angle){
		return angle * (float)Math.PI / 180f;
	}

	private static float GetAngle(Vector3 positionA, Vector3 positionB){
		return (float)Mathf.Atan2(positionB.y-positionA.y, positionB.x-positionA.x) * 180f / (float)Math.PI;
	}

	/// <summary>限制角度在 0 ~ 360</summary>
	private static float FormatAngle360(float angle){
		angle = angle - (((int)(angle / 360f)) * 360f);
		if(angle < 0) angle += 360f;
		return angle;
	}

	/// <summary>取得 A 到 B 最小夾角（順時針為負數， -180 ~ 180）</summary>
	private static float GetIncludedAngle(float angleA, float angleB){
		angleA = FormatAngle360(angleA);
		angleB = FormatAngle360(angleB);

		if(angleA > angleB){
			//順時針夾角（正值）
			float includedAngleA = angleA - angleB;
			//逆時針夾角
			float includedAngleB = angleB + (360f - angleA);
			if(includedAngleA > includedAngleB){
				return includedAngleB;
			}else{
				return -includedAngleA;
			}
		}else{
			//順時針夾角（正值）
			float includedAngleA = angleB - angleA;
			//逆時針夾角
			float includedAngleB = angleA + (360f - angleB);
			if(includedAngleA > includedAngleB){
				return -includedAngleB;
			}else{
				return includedAngleA;
			}
		}
	}

	public static FightBulletTween_Direction Begin(GameObject bullet, Vector3 targetPos, Action endFunc = null){
		float toTargetAngle = GetAngle(bullet.transform.localPosition, targetPos);
		float randomAngle = ((float)UnityEngine.Random.Range(0, 2) -0.5f) * 50f;
		randomAngle += (randomAngle > 0) ? UnityEngine.Random.Range(0, 25f)*2f : UnityEngine.Random.Range(-25f, 0)*2f;
		toTargetAngle+= randomAngle;

		FightBulletTween_Direction tween = bullet.AddComponent<FightBulletTween_Direction>();
		tween.targetPos = targetPos;
		tween.nowAngle = toTargetAngle;

		tween.OnEndFunc+=endFunc;

		return tween;
	}
}


