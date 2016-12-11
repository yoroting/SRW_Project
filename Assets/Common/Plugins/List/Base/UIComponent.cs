using UnityEngine;
using System.Collections;
using System;

public abstract class UIComponent : MonoBehaviour 
{
	protected bool isRedraw = false;
	
	protected bool isInitizlized = false;
	
	private bool isEnabled = true;
	
	private GameObject cachedGameObject;
	
	private Transform cachedTransform;
	
	public bool IsEnabled
	{
		get {return isEnabled;}
		
		set 
		{
			if (isEnabled == value)
				return;
			
			isEnabled = value;
			
			UpdateEnabled();
		}
	}
	
	public bool IsDestroyed
	{
		get {return cachedGameObject == null;}
	}
	
	public GameObject CachedGameObject
	{
		get {
			if(cachedGameObject == null)
				cachedGameObject = this.gameObject;
			return cachedGameObject;
		}
	}
	
	public Transform CachedTransform
	{
		get 
		{
			if (cachedTransform == null)
				cachedTransform = this.transform;
			return cachedTransform;
		}
	}	
	
	public void Invalidate(bool isImmediately = false)
	{
		isRedraw = true;
		
		if (isImmediately && cachedGameObject != null && cachedGameObject.activeInHierarchy)
			UpdateProperties();
	}
	
	protected virtual void LateUpdate()
	{
		UpdateProperties();
	}
	
	protected virtual void Awake()
	{
		Initialize();
		
		isInitizlized = true;
	}
	
	protected virtual void Start()
	{
		if (isRedraw)
			Invalidate(true);
	}
	
	protected virtual void OnDestroy()
	{
		Destroy();
	}
	
	private void UpdateProperties()
	{
		if (isRedraw)
		{
			#if DEBUG
			UnityEngine.Profiling.Profiler.BeginSample("UIComponent Redraw");
			#endif
			Redraw();
			#if DEBUG
			UnityEngine.Profiling.Profiler.EndSample();
			#endif
			isRedraw = false;
		}
	}
	
	protected virtual void UpdateEnabled()
	{
	}
	
	abstract protected void Initialize();
	
	abstract protected void Redraw();
	
	abstract protected void Destroy();
}

