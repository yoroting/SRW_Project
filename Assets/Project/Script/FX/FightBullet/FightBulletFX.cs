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
	public int nMissileID { set; get ; }

	public static FightBulletFX CreatFX(int nMissileID, Transform parent, Transform start, Transform target, Action endFunc = null){
		Vector3 startPos = parent.InverseTransformPoint(start.position);
		Vector3 targetPos = parent.InverseTransformPoint(target.position);
		return CreatFX(nMissileID, parent, startPos, targetPos, endFunc );
	}

	public static FightBulletFX CreatFX(int nMissileID, Transform parent, Vector3 start, Vector3 target, Action endFunc = null){
		string sFile = "ACT_FLAME"; // defau;t missile
		Missile mData = ConstDataManager.Instance.GetRow < Missile > ( nMissileID );
		bool bRandom = false;

		if (mData != null) {
			sFile = mData.s_MISSILE;
			if( mData.n_TYPE == 1  ){
				bRandom = true;
			}
		}

		// Missile
	
		FightBulletFX pFX = CreatFX ( sFile  , parent, start, target, endFunc , bRandom );
		if( pFX != null ){
			pFX.nMissileID = nMissileID;
//			if( mData.n_SPEED ){
//
//			}
		}
		
		return pFX;
	}

	public static FightBulletFX CreatFX(string FXName, Transform parent, Transform start, Transform target, Action endFunc = null , bool bRandom = false ){
		Vector3 startPos = parent.InverseTransformPoint(start.position);
		Vector3 targetPos = parent.InverseTransformPoint(target.position);
		return CreatFX(FXName, parent, startPos, targetPos, endFunc , bRandom);
	}

	public static FightBulletFX CreatFX(string FXName, Transform parent, Vector3 start, Vector3 target, Action endFunc = null , bool bRandom = false ){
		GameObject prefab = ResourcesManager.LoadFightBulletFx(FXName);
		GameObject fxObj = NGUITools.AddChild(parent.gameObject, prefab);
		FightBulletFX fx = fxObj.GetComponent<FightBulletFX>();
		fx.transform.localPosition = start;
		fx.tween = FightBulletTween_Direction.Begin(fxObj, target, endFunc, bRandom);
		fx.tween.OnEndFunc += fx.onTweenEnd;
		return fx;
	}

}
