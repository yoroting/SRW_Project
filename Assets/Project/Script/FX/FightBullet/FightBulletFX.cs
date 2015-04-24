using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FightBulletFX : MonoBehaviour {

	private FightBulletTween_Direction tween;

	private void onTweenEnd(){
		tween = null;
		Destroy(gameObject);
	}

	public static FightBulletFX CreatFX(string FXName, Transform parent, Transform start, Transform target, Action endFunc = null){
		Vector3 startPos = parent.InverseTransformPoint(start.position);
		Vector3 targetPos = parent.InverseTransformPoint(target.position);
		return CreatFX(FXName, parent, startPos, targetPos, endFunc);
	}

	public static FightBulletFX CreatFX(string FXName, Transform parent, Vector3 start, Vector3 target, Action endFunc = null){
		GameObject prefab = ResourcesManager.LoadFightBulletFx(FXName);
		GameObject fxObj = NGUITools.AddChild(parent.gameObject, prefab);
		FightBulletFX fx = fxObj.GetComponent<FightBulletFX>();
		fx.transform.localPosition = start;
		fx.tween = FightBulletTween_Direction.Begin(fxObj, target, endFunc);
		fx.tween.OnEndFunc += fx.onTweenEnd;
		return fx;
	}

}
