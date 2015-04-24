using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public interface IItemList
{
	IList DataProvider
	{
		get;
		set;
	}
	
	int SelectedIndex
	{
		get;
		set;
	}
		
	object SelectedItem
	{
		get;
		set;
	}
	
	int Position
	{
		get;
		set;
	}

	void DispatchItemClick(object data);
}

public class UIListBase : UIComponent, IItemList 
{
	public event Action OnSelectionChanged;
	
	public event Action OnDataProviderChaged;

	public event Action<object> ItemClickEvent;
	
	public event Action OnDataRedrawFinished;
	
	protected IList dataProvider;
	
	protected List<AbstractItemRenderer> itemRendererList;
	
	protected int position;
	
	protected int selectedIndex = -1;
	
	private bool isAutoHide = true;

	/// <summary>
	/// Renderer的起始位置
	/// </summary>
	protected int rendererStartFrom;

	public virtual bool IsAutoHide
	{
		get {return isAutoHide;}
		set
		{
			if (isAutoHide == value)
				return;
			
			isAutoHide = value;
			
			Invalidate();
		}
	}
	
	public virtual int SelectedIndex
	{
		get {return this.selectedIndex;}
		set 
		{
			if (dataProvider == null)
				return;
			
			if (value >= dataProvider.Count)
				return;
			
			if (value < 0)
				value = -1;
			
			if (selectedIndex != value)
			{
				selectedIndex = value;
				
				if (OnSelectionChanged != null)
					OnSelectionChanged();
				
				Invalidate();
			}
		}
	}	

	public virtual object SelectedItem 
	{
		get 
		{
			if (selectedIndex >= 0 && selectedIndex < dataProvider.Count)
				return dataProvider[selectedIndex];
			else
				return null;
		}
		set 
		{
			if (dataProvider != null && value != null)
				SelectedIndex = dataProvider.IndexOf(value);
		}
	}

	public virtual IList DataProvider 
	{
		get {return this.dataProvider;}
		
		set 
		{
			if (dataProvider == value)
				return;
			
			rendererStartFrom = 0;
			selectedIndex = -1;
			position = 0;
			dataProvider = value;
			
			if (OnDataProviderChaged != null)
				OnDataProviderChaged();
			
			Invalidate();
		}
	}

	public int Position
	{
		get {return this.position;}
		
		set
		{
			if (position == value)
				return;
			
			if (dataProvider == null || value < 0 || value >= dataProvider.Count)
				return;

			position = value;

			Invalidate();
		}
	}	
	
	public int ItemRendererNum
	{
		get {return (itemRendererList != null) ? itemRendererList.Count : 0;}	
	}
	
	public virtual void Clean()
	{
		this.itemRendererList = null;
		this.DataProvider = null;
		this.selectedIndex = -1;
	}
	
	public AbstractItemRenderer[] ListItemRenderers()
	{
		return itemRendererList != null ? itemRendererList.ToArray() : null; 
	}
	
	public void Refresh(bool isImmediately = false)
	{
		if (itemRendererList == null)
			return;
		
		foreach (AbstractItemRenderer itemRenderer in itemRendererList)
		{
			itemRenderer.Invalidate(isImmediately);
		}
	}
	
	public void RefreshByData(object data, bool isImmediately = false)
	{
		AbstractItemRenderer renderer = FindItemRendererByData(data);
		
		if (renderer != null)
			renderer.Invalidate(isImmediately);
	}
	
	public virtual AbstractItemRenderer[] FindItemRenderersByData(object data)
	{
		if (itemRendererList == null)
			return null;
		
		List<AbstractItemRenderer> result = new List<AbstractItemRenderer>();
		
		foreach (AbstractItemRenderer renderer in itemRendererList)
		{
			if (renderer.Data == data)
				result.Add(renderer);
		}
		return result.ToArray();
	}

	public virtual AbstractItemRenderer FindItemRendererByData(object data)
	{
		if (itemRendererList == null)
			return null;
		
		foreach (AbstractItemRenderer renderer in itemRendererList)
		{
			if (renderer.Data == data)
				return renderer;
		}
		return null;
	}

	public virtual AbstractItemRenderer FindItemRenderer(int index)
	{
		if (itemRendererList == null || index < 0 || index >= itemRendererList.Count)
			return null;

		return itemRendererList[index];
	}
	
	protected override void Redraw()
	{
		if (itemRendererList == null)
			return;
		
		int rendererNum = itemRendererList.Count;
		
		if (dataProvider != null)
		{
			int dataNum = dataProvider.Count;
			
			if (selectedIndex >= dataNum)
				selectedIndex = -1;
			
			for (int i = 0; i < rendererNum; ++i)
			{
				int rendererIndex = rendererStartFrom + i;
				
				if (rendererIndex >= rendererNum)
					rendererIndex %= rendererNum;
				
				AbstractItemRenderer renderer = itemRendererList[rendererIndex];
				
				int currentIndex = GetNextDataIndex(i);
				
				if (currentIndex < dataNum)
				{
					renderer.DataIndex = currentIndex;
					renderer.Data = dataProvider[currentIndex];
				}
				else
				{
					renderer.DataIndex = -1;
					renderer.Data = null;
				}
				
				renderer.Selected = currentIndex == selectedIndex;
				
				if (isAutoHide)
				{
					bool isActive = renderer.Data != null;
					
					if (renderer.gameObject.activeSelf != isActive)
						NGUITools.SetActive(renderer.gameObject, isActive);
				}
			}
			if(OnDataRedrawFinished != null)
				OnDataRedrawFinished();
		}
		else
		{
			for (int i = 0; i < rendererNum; ++i)
			{
				AbstractItemRenderer renderer = itemRendererList[i];
				renderer.Data = null;
				renderer.Selected = false;
				
				NGUITools.SetActive(renderer.gameObject, !isAutoHide);
			}
		}
	}

	protected virtual int GetNextDataIndex(int index)
	{
		return position + index;
	}

	public virtual void DispatchItemClick(object data)
	{
		if (ItemClickEvent != null)
			ItemClickEvent(data);
	}

	protected override void Initialize()
	{
	}

	protected override void Destroy ()
	{
	}
}

