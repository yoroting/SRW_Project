/*
 *	GameEventManager
 *
 *	Event system, also works with javascript
 *
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void EventCallbackFunc (GameEvent evt);

public static class GameEventManager
{
	private static SortedDictionary<string, ArrayList> mEventPool = new SortedDictionary<string, ArrayList> ();
	
	static GameEventManager ()
	{
		if (mEventPool == null) {
			mEventPool = new SortedDictionary<string, ArrayList> ();
		}
	}
	
	// clear all event listeners
	public static void ResetCallbackPool ()
	{
		if (mEventPool != null) {
			ArrayList al;
			
			foreach (KeyValuePair<string, ArrayList> item in mEventPool) {
				al = item.Value as ArrayList;
				al.Clear ();
			}
		}
		
		mEventPool = new SortedDictionary<string, ArrayList> ();
	}
	
	// add event listener
	public static void AddEventListener (string eventName, EventCallbackFunc func)
	{
		if (mEventPool.ContainsKey (eventName) == false) {
			ArrayList callbackArray = new ArrayList ();
			mEventPool.Add (eventName, callbackArray);
			callbackArray.Add (func);
			
			return;
		} 
		
		ArrayList callbacksArray = mEventPool[eventName] as ArrayList;
		if (!callbacksArray.Contains(func)) {
			callbacksArray.Add (func);
		}
	}
	
	// remove event listeneer
	public static void RemoveEventListener (string eventName, EventCallbackFunc func)
	{
		if (mEventPool.ContainsKey (eventName) == false) {
			return;
		}
		
		ArrayList callbacksArray = mEventPool[eventName] as ArrayList;
		if (callbacksArray.Contains(func)) {
			callbacksArray.Remove (func);
		}
	}
	
	// dispatch event, call the functions listening to the event
	public static void DispatchEvent (GameEvent evt)
	{
		string eventName = evt.name;
		if (mEventPool.ContainsKey (eventName)) {
			ArrayList callbacksArray = mEventPool[eventName];
			if (callbacksArray != null) {
				int startIndex = callbacksArray.Count - 1;
				for (int i = startIndex; i >= 0; i--) {
					if (callbacksArray[i] != null) {
						EventCallbackFunc func = callbacksArray[i] as EventCallbackFunc;
						if (func != null) {
							func (evt);
						}
					}
				}
			}
		}
	}
	
	// dispatch event by name
	public static void DispatchEventByName (string evtName, object param = null)
	{
		GameEvent evt = new GameEvent ();
		evt.name = evtName;
		evt.param = param;

		DispatchEvent (evt);
	}
	
	//Report All Event
	public static void ReportAllEvents()
	{
		foreach(string key in mEventPool.Keys)
		{
			ArrayList callbacksArray = mEventPool[key];
			if(callbacksArray.Count > 0)
				Debug.Log("Event:" + key);
		}
	}
	
	//檢查指定事件是否已存在
	public static bool IsEventExist(string eventName)
	{
		if ( string.IsNullOrEmpty(eventName) )
			return false;
		
		if ( mEventPool == null )
			return false;
		
		if ( mEventPool.ContainsKey(eventName) )
		{
			return true;
		}
		
		return false;
	}
}

