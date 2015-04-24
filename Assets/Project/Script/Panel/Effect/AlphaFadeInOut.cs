//using UnityEngine;
//using System;
//using System.Collections;
//
//public class AlphaFadeInOut : APanelEffect {
//
//	private float tweenTime;
//	private float alpha;
//
//	public AlphaFadeInOut(float tweenTime = 0, float alpha = 1){
//		this.tweenTime = tweenTime;
//		this.alpha = alpha;
//	}
//
//
////	public void Begin(float tweenTime = 0, float alpha = 1, Action endFunc = null){
////		EventDelegate.Callback tweenEndFunc = () => {
////			if(endFunc != null)
////			endFunc();
////		};
////		
////		if(!IsShowing){
////			transform.localPosition = showPos;
////			
////			if(tweenTime > 0){
////				TweenAlpha.Begin(gameObject, 0, 0);
////				TweenAlpha.Begin(gameObject, tweenTime, 1f).AddOnFinished(tweenEndFunc);
////				
////			}else{
////				NGUITools.SetActive(gameObject, true);
////				tweenEndFunc();
////			}
////		}else{
////			tweenEndFunc();
////		}
////	}
//
//}
