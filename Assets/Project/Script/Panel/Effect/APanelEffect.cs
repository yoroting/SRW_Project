//using UnityEngine;
//using System;
//using System.Collections;
//
//public abstract class APanelEffect {
//
//	private Action oneTimeEndFunc;
//
//	protected bool isRuning;
//	/// <summary>是否正在跑效果中</summary>
//	public bool IsRuning{ get{ return isRuning; } }
//
//	private Nullable<bool> nowShowing = null;
//	/// <summary>是否在跑顯示中效果</summary>
//	protected Nullable<bool> NowShowing{ get{ return nowShowing; } }
//
//	private void callEndFunc(){
//		if(oneTimeEndFunc == null)
//			return;
//		Action tempFunc = oneTimeEndFunc;
//		oneTimeEndFunc = null;
//		tempFunc();
//	}
//
//	public void BeginShow(Action endFunc=null){
//		this.oneTimeEndFunc = endFunc;
//		if(nowShowing != null && nowShowing.Value == false){
//			isRuning = true;
//			nowShowing = true;
//			callEndFunc();
//		}else{
//			OnBeginShow(callEndFunc);
//		}
//	}
//	public void BeginHide(Action endFunc=null){
//		this.oneTimeEndFunc = endFunc;
//		if(nowShowing != null && nowShowing.Value == true){
//			isRuning = true;
//			nowShowing = false;
//			callEndFunc();
//		}else{
//			OnBeginShow(callEndFunc);
//		}
//	}
//
//	/// <summary>當執行顯示效果，子類別繼承但不執行</summary>
//	protected virtual void OnBeginShow(Action endFunc){ endFunc(); }
//	/// <summary>當執行隱藏效果，子類別繼承但不執行</summary>
//	protected virtual void OnBeginHide(Action endFunc){ endFunc(); }
//}
