using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(UIScrollView))]
/// <summary>
/// NGUI列表組件
/// </summary>
public abstract class UIList : UIListBase
{
	private const float MOVE_SPRING_STRENGTH = 8f;

	public enum Arrangement
	{
		Vertical,
		Horizontal,
	}

	private bool isDragable = true;

	public int cellWidth;

	public int cellHeight;

	public Arrangement arrangement = Arrangement.Horizontal;

	public GameObject itemRendererPrefab;

	public bool isCenterChild = true;
	
	public event Action MoveStartEvent;
	
	public event Action MoveEndEvent;

	/// <summary>
	/// 索引位置改變
	/// </summary>
	public event Action PositionChangedEvent;

	protected UIWidget container;

	protected UIScrollView scrollView;

	/// <summary>
	/// 可見範圍的列數（直向）
	/// </summary>
	protected int cols;

	/// <summary>
	/// 可見範圍的欄數（橫向）
	/// </summary>
	protected int rows;

	/// <summary>
	/// 實際資料的最大列數
	/// </summary>
	protected int maxCols;

	/// <summary>
	/// 實際資料的最大欄數
	/// </summary>
	protected int maxRows;

	/// <summary>
	/// 是否要更新列表
	/// </summary>
	protected bool isUpdateListView;

	/// <summary>
	/// 列表是否有捲動
	/// </summary>
	private bool isUpdatePosition = false;

	/// <summary>
	///  是否為單頁
	/// </summary>
	protected bool isOnePage = true;

	private Vector2 prevPos;
	
	private Vector2 rendererOffset;
	
	private bool isUpdateRenderer;

	private bool isChanged = false;

	private SpringPanel tweenSpring;

	private Vector3 defaultPanelPos;

	private List<UIDragScrollView> dragScrollViews;

	/// <summary>
	/// 列表是否可拖曳
	/// </summary>
	/// <value><c>true</c> if this instance is dragable; otherwise, <c>false</c>.</value>
	public bool IsDragable
	{
		get {return isDragable;}
		set 
		{
			if (isDragable == value)
				return;

			isDragable = value;

			UpdateDragable();
		}
	}

	public UIPanel panel
	{
		get {return scrollView.panel;}
	}

	public UIScrollView ScrollView
	{
		get {return scrollView;}
	}

	public int ColIndex
	{
		//依目前位置計算所的列索引
		get 
		{
			return CalculateIndex(-scrollView.panel.clipOffset.y, cellHeight, maxCols);
		}
	}

	public int RowIndex
	{
		//依目前位置計算所的欄索引
		get 
		{
			return CalculateIndex(scrollView.panel.clipOffset.x, cellWidth, maxRows);
		}
	}

	public static int CalculateIndex(float val, int size, int max)
	{
		//修正小數點
		val = Mathf.Ceil(val);

		int result = Mathf.FloorToInt(val / size);
		
		//範圍檢查
		if (result < 0)
			result = 0;
		else if (result > max)
			result = max;

		return result;
	}

	/// <summary>
	/// 是否正在移動中
	/// </summary>
	/// <value><c>true</c> if this instance is moving; otherwise, <c>false</c>.</value>
	public bool IsMoving
	{
		get 
		{
			return tweenSpring != null && tweenSpring.enabled;
		}
	}

	/// <summary>
	/// 是否在最尾
	/// </summary>
	/// <value><c>true</c> if this instance is end; otherwise, <c>false</c>.</value>
	public virtual bool IsEnd
	{
		get 
		{
			if (arrangement == Arrangement.Vertical)
			{
				return this.Position >= (maxCols - cols) * rows;
			}
			else
			{
				return this.Position >= (maxRows - rows) * cols;
			}
		}
	}
	
	/// <summary>
	/// 是否在最前
	/// </summary>
	/// <value><c>true</c> if this instance is top; otherwise, <c>false</c>.</value>
	public virtual bool IsTop
	{
		get 
		{
			if (arrangement == Arrangement.Vertical)
			{
				return this.Position < rows;
			}
			else
			{
				return this.Position < cols;
			}
		}
	}

	public override IList DataProvider 
	{
		set
		{
			this.prevPos = Vector3.zero;
			this.isUpdateListView = dataProvider != value;

			base.DataProvider = value;
		}
	}

	public override AbstractItemRenderer FindItemRenderer (int index)
	{
		foreach (var renderer in itemRendererList)
		{
			if (renderer.DataIndex == index)
				return renderer;
		}
		return null;
	}

	protected Vector4 GetClipping()
	{
		if (scrollView.panel.clipping == UIDrawCall.Clipping.None)
			return Vector4.zero;

		Vector4 clipping = scrollView.panel.baseClipRegion;
		
		if (scrollView.panel.clipping == UIDrawCall.Clipping.SoftClip)
		{
			//大小要加算softness
			clipping.z -= scrollView.panel.clipSoftness.x * 2f;
			clipping.w -= scrollView.panel.clipSoftness.y * 2f;
		}
		return clipping;
	}

	private void CleanAllRenderers()
	{
		if (itemRendererList != null)
		{
			foreach (AbstractItemRenderer renderer in itemRendererList)
			{
				renderer.Data = null;
				NGUITools.SetActive(renderer.CachedGameObject, !IsAutoHide, false);
			}
		}
	}

	protected override void Initialize()
	{
		dragScrollViews = new List<UIDragScrollView>();

		scrollView = GetComponent<UIScrollView>();

		//建立列表容器
		container = NGUITools.AddChild<UIWidget>(CachedGameObject);
		container.name = "Container";

		container.autoResizeBoxCollider = true;
		BoxCollider collider = container.cachedGameObject.AddComponent<BoxCollider>();
		collider.enabled = isDragable;
		container.depth = -10;

		UIDragScrollView containerScrollView = container.cachedGameObject.AddComponent<UIDragScrollView>();

		dragScrollViews.Add(containerScrollView);

		//更新panel上的數值
		UIPanel panel = GetComponent<UIPanel>();

		defaultPanelPos = panel.cachedTransform.localPosition;
		UpdateDragable();
	}

	private void UpdateDragable()
	{
		if (container != null)
		{
			Collider collider = container.GetComponent<Collider>();
			
			if (collider != null)
				collider.enabled = isDragable;
		}

		if (dragScrollViews == null)
			return;

		int num = dragScrollViews.Count;

		for (int i = 0; i < num; ++i)
		{
			dragScrollViews[i].enabled = isDragable;
		}
	}

	protected virtual void UpdateListView()
	{
		Vector4 clipping = GetClipping();

		//計算顯示的欄數、列數
		rows = Mathf.FloorToInt(clipping.z / (float)cellWidth + .8f);
		cols = Mathf.FloorToInt(clipping.w / (float)cellHeight + .8f);

		int num = dataProvider != null ? dataProvider.Count : 0;
		
		Vector3 pos = container.cachedTransform.localPosition;

		//更新大小
		if (arrangement == Arrangement.Vertical)
		{
			maxRows = rows;
			maxCols = Mathf.CeilToInt((float)num / (float)rows);

			isOnePage = dataProvider.Count <= cols * rows;

			//單頁時避免出現scrollBar，再減1像素
			container.height = isOnePage ? (int)clipping.w - 1 : maxCols * cellHeight;
			container.width = rows * cellWidth;

			pos.x = 0;
			pos.y = -(container.height - clipping.w) / 2;

			scrollView.movement = UIScrollView.Movement.Vertical;
		}
		else
		{
			maxRows = Mathf.CeilToInt((float)num / (float)cols);
			maxCols = cols;

			isOnePage = dataProvider.Count <= cols * rows;

			//單頁時避免出現scrollBar，再減1像素
			container.width = isOnePage ? (int)clipping.z - 1 : maxRows * cellWidth;
			container.height = cols * cellHeight;
			
			pos.x = (container.width - clipping.z) / 2;
			pos.y = 0;

			scrollView.movement = UIScrollView.Movement.Horizontal;
		}

		pos.x += clipping.x;
		pos.y += clipping.y;

		//只建一次
		if (itemRendererList == null)
		{
			itemRendererList = CreateItemRenderers();

			if (itemRendererList.Count > 0)
			{
				rendererOffset = new Vector2(cellWidth, -cellHeight) / 2;
				
				if (isCenterChild)
				{
					//取得ItemRenderer bounds
					Bounds rendererBonuds;
					
					AbstractItemRenderer renderer = itemRendererList[0];

					renderer.CachedTransform.localPosition = Vector3.zero;

					if (!renderer.gameObject.activeSelf)
					{
						//ItemRenderer若為Deactive，無法取得大小，先啟用再關閉
						NGUITools.SetActive(renderer.gameObject, true, false);

						rendererBonuds = NGUIMath.CalculateRelativeWidgetBounds(renderer.CachedTransform, renderer.CachedTransform);

						NGUITools.SetActive(renderer.gameObject, false, false);
					}
					else
					{
						rendererBonuds = NGUIMath.CalculateRelativeWidgetBounds(renderer.CachedTransform, renderer.CachedTransform);
					}
					
					rendererOffset -= (Vector2)rendererBonuds.center;
				}
				else
				{
					rendererOffset = Vector3.zero;
				}
			}
		}
		
		//修正座標
		container.cachedTransform.localPosition = pos;

		scrollView.ResetPosition();

		UpdateDragable();
	}
	
	protected override void Redraw()
	{
		if (isUpdateListView)
		{
			isUpdateListView = false;

			rendererStartFrom = 0;
			position = 0;

			CleanAllRenderers();

			if (dataProvider != null)
			{
				UpdateListView();

				isUpdatePosition = true;
			}
			scrollView.panel.cachedTransform.localPosition = defaultPanelPos;
			scrollView.panel.clipOffset = Vector2.zero;

			isChanged = true;
		}

		base.Redraw();

		if (isUpdatePosition)
		{
			isUpdatePosition = false;
			Reposition();

			isChanged = true;
		}
	}

	protected virtual void Update()
	{
		if (isChanged)
		{
			isChanged = false;

			//Panel為static時強制重繪
			if (scrollView.panel.widgetsAreStatic)
				scrollView.panel.SetDirty();
		}

		if (!isOnePage && itemRendererList != null && itemRendererList.Count > 0)
		{
			Vector2 nowPos = scrollView.panel.clipOffset;

			if (!nowPos.Equals(prevPos))
			{
				int nextDataPos = 0;

				//座標改變，檢查顯示位置
				if (arrangement == Arrangement.Vertical)
				{
					nextDataPos = this.ColIndex * rows;
					nextDataPos = Math.Min(nextDataPos, (maxCols - cols) * rows);
				}
				else
				{
					nextDataPos = this.RowIndex * cols;
					nextDataPos = Math.Min(nextDataPos, (maxRows - rows) * cols);
				}

				if (this.Position != nextDataPos)
				{
					this.rendererStartFrom = nextDataPos % itemRendererList.Count;
					this.Position = nextDataPos;
					
					this.isUpdatePosition = true;

					if (PositionChangedEvent != null)
						PositionChangedEvent();
				}
				this.prevPos = nowPos;
			}
		}
	}

	protected void MoveToPosition(Vector3 relative, bool useTween)
	{
		if (relative == Vector3.zero)
		{
			if (useTween)
			{
				if (MoveStartEvent != null)
					MoveStartEvent();

				if (MoveEndEvent != null)
					MoveEndEvent();
			}
			return;
		}

		//清除目前的移動量
		scrollView.currentMomentum = Vector3.zero;

		scrollView.DisableSpring();

		if (useTween)
		{
			if (MoveStartEvent != null)
				MoveStartEvent();

			Vector3 panelPos = scrollView.panel.cachedTransform.localPosition + relative; 
			
			tweenSpring = SpringPanel.Begin(scrollView.gameObject, panelPos, MOVE_SPRING_STRENGTH);

			tweenSpring.onFinished += FinishHandler;
		}
		else
		{
			scrollView.MoveRelative(relative);

			this.Update();
		}
	}

	private void FinishHandler()
	{
		tweenSpring.onFinished -= FinishHandler;

		if (MoveEndEvent != null)
			MoveEndEvent();
	}

	private List<AbstractItemRenderer> CreateItemRenderers()
	{
		List<AbstractItemRenderer> list = new List<AbstractItemRenderer>();

		if (itemRendererPrefab.GetComponent<AbstractItemRenderer>() == null)
		{
			Debug.LogWarning("建立列表失敗，ItemRenderer prefab 未實作AbstractItemRenderer");
			return list;
		}

		int num = cols * rows;

		//加上緩衝的ItemRenderer
		if (arrangement == Arrangement.Vertical)
		{
			num += rows;
		}
		else
		{
			num += cols;
		}		

		for (int i = 0; i < num; ++i)
		{
			GameObject child = NGUITools.AddChild(container.cachedGameObject, itemRendererPrefab);

			AbstractItemRenderer renderer = child.GetComponent<AbstractItemRenderer>();

			renderer.ListView = this;
			
			list.Add(renderer);

			UIDragScrollView rendererScrollView = renderer.GetComponentInChildren<UIDragScrollView>();

			if (rendererScrollView != null)
				dragScrollViews.Add(rendererScrollView);
		}
		return list;
	}
	
	private void Reposition()
	{
		if (itemRendererList == null || itemRendererList.Count == 0)
			return;

		//計算都以左上為原點
		Vector2 startPos = new Vector2((float)container.width / -2f, (float)container.height / 2f);

		if (arrangement == Arrangement.Vertical)
		{
			startPos.y -= this.ColIndex * cellHeight;
		}
		else
		{
			startPos.x += this.RowIndex * cellWidth;
		}

		//修正ItemRenderer定位點
		startPos.x += rendererOffset.x;
		startPos.y += rendererOffset.y;

		int num = itemRendererList.Count;
		for (int i = 0; i < num; ++i)
		{
			int rendererIndex = rendererStartFrom + i;
			
			if (rendererIndex >= num)
				rendererIndex %= num;

			AbstractItemRenderer renderer = itemRendererList[rendererIndex];

			//不調整隱藏的元件
			if (IsAutoHide && renderer.Data == null)
				continue;

			Transform trans = renderer.CachedTransform;

			Vector3 pos = CalculateNextPos(i, startPos);

			trans.localPosition = pos;
		}
	}

	private Vector3 CalculateNextPos(int index, Vector3 pos)
	{
		if (arrangement == Arrangement.Vertical)
		{
			//由左至右
			pos.x += index % rows * cellWidth;
			pos.y -= index / rows * cellHeight;
		}
		else
		{
			//由上至下
			pos.x += index / cols * cellWidth;
			pos.y -= index % cols * cellHeight;
		}
		return pos;
	}

	abstract public void MoveToNext(bool useTween = true);

	abstract public void MoveToPrevious(bool useTween = true);

	abstract public void MoveToData(object data, bool useTween = true);

	abstract public void MoveToTop(bool useTween = true);

	abstract public void MoveToEnd(bool useTween = true);
	
	[ContextMenu("調整遮罩範圍以符合項目")]
	public void ModifyPanelClipping()
	{
		if (scrollView.panel.clipping == UIDrawCall.Clipping.None)
			return;

		if (scrollView == null)
			scrollView = GetComponent<UIScrollView>();

		Vector4 clipping = GetClipping();
		
		int rows = Mathf.CeilToInt(clipping.z / (float)cellWidth);
		int cols = Mathf.CeilToInt(clipping.w / (float)cellHeight);

		clipping.x = 0;
		clipping.y = 0;
		clipping.z = rows * cellWidth;
		clipping.w = cols * cellHeight;

		if (scrollView.panel.clipping == UIDrawCall.Clipping.SoftClip)
		{
			clipping.z += scrollView.panel.clipSoftness.x * 2f;
			clipping.w += scrollView.panel.clipSoftness.y * 2f;
		}

		scrollView.panel.baseClipRegion = clipping;
	}

#if UNITY_EDITOR
	void OnDrawGizmos ()
	{
		if (scrollView == null)
			scrollView = GetComponent<UIScrollView>();

		//繪制列表的大小及排列方式
		if (scrollView != null && !Application.isPlaying)
		{
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = new Color(1f, 0.4f, 0f);

			Vector4 clipping = GetClipping();

			int rows = Mathf.FloorToInt(clipping.z / (float)cellWidth + .8f);
			int cols = Mathf.FloorToInt(clipping.w / (float)cellHeight + .8f);

			int num = cols * rows;
			int width = rows * cellWidth;
			int height = cols * cellHeight;

			Vector2 startPos = new Vector2((float)width / -2f, (float)height / 2f);

			startPos.x += (width - clipping.z) / 2;
			startPos.y += -(height - clipping.w) / 2;

			startPos.x += clipping.x;
			startPos.y += clipping.y;

			Vector3 size = new Vector3(cellWidth, cellHeight, 0f);

			for (int i = 0; i < num; ++i)
			{
				Vector3 pos = startPos;

				pos.x += i % rows * cellWidth + cellWidth / 2;
				pos.y -= i / rows * cellHeight + cellHeight / 2;

				Gizmos.DrawWireCube(pos, size);
			}
		}
	}
#endif
}
