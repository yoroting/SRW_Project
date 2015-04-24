using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 每一短時間執行一次的機制
/// </summary>
partial class PanelManager {

	private float repeatTimer = 0.05f;

	private List<Action> repeatFuncs = new List<Action>();
	private Action[] repeatFuncArr = null;
	
	public void addRepeatFunc(Action repeatFunc){
		repeatFuncs.Add(repeatFunc);
		if(repeatFuncs.Count == 1){
			InvokeRepeating("onRepeatTimeOut", repeatTimer, repeatTimer);
		}
		repeatFuncArr = null;
	}
	
	public void removeRepeatFunc(Action repeatFunc){
		repeatFuncs.Remove(repeatFunc);
		repeatFuncArr = null;
	}
	
	private void onRepeatTimeOut(){
		if(repeatFuncArr == null)
			repeatFuncArr = repeatFuncs.ToArray();
		Action[] workFuncArr = repeatFuncArr;
		
		for(int i=workFuncArr.Length-1; i>=0; i--){
			Action func = workFuncArr[i];
			func();
		}
	}

//
//	private List<WeakReference> repeatFuncs = new List<WeakReference>();
//	private WeakReference[] repeatFuncArr = null;
//
//	public void addRepeatFunc(Action repeatFunc){
//		Debug.LogError("加入方法");
//		repeatFuncs.Add(new WeakReference(repeatFunc));
//		if(repeatFuncs.Count == 1){
//			InvokeRepeating("onRepeatTimeOut", repeatTimer, repeatTimer);
//		}
//		repeatFuncArr = null;
//	}
//
//	public void removeRepeatFunc(Action repeatFunc){
//		Debug.LogError("移除方法");
//		for(int i=repeatFuncs.Count-1; i>=0; i--){
//			WeakReference reference = repeatFuncs[i];
//			Action func = reference.Target as Action;
//			if(func == repeatFunc){
//				repeatFuncs.RemoveAt(i);
//				repeatFuncArr = null;
//				if(repeatFuncs.Count == 0){
//					CancelInvoke("onRepeatTimeOut");
//				}
//				break;
//			}
//		}
//	}
//
//	private void onRepeatTimeOut(){
//		if(repeatFuncArr == null)
//			repeatFuncArr = repeatFuncs.ToArray();
//		WeakReference[] workFuncArr = repeatFuncArr;
//
//		for(int i=workFuncArr.Length-1; i>=0; i--){
//			WeakReference reference = workFuncArr[i];
//			Action func = reference.Target as Action;
//			if(func == null){
//				repeatFuncs.RemoveAt(i);
//				continue;
//			}
//			func();
//		}
//		
//		if(repeatFuncs.Count == 0){
//			CancelInvoke("onRepeatTimeOut");
//		}
//	}



}
