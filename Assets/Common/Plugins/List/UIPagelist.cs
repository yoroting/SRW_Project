using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIPagelist : UIList 
{
	private const float FAST_TAP_DURATION = 1f;

	private const int FAST_TAP_DISTANCE = 100;

	public event Action PageChangedEvent;

	public float nextPageThreshold;

	private int totalPage;

	private int currentPage;

	private float dragStartTime;

	/// <summary>
	/// 總頁數
	/// </summary>
	/// <value>The total page.</value>
	public int TotalPage
	{
		get {return totalPage;}
	}

	/// <summary>
	/// 目前頁數
	/// </summary>
	/// <value>The current page.</value>
	public int CurrentPage
	{
		get {return currentPage;}
	}

	public override bool IsTop 
	{
		get {return currentPage <= 1;}
	}

	public override bool IsEnd 
	{
		get {return currentPage == totalPage;}
	}

	private int pageFieldNum;

	protected override void Initialize ()
	{
		base.Initialize ();

		this.scrollView.onDragFinished += ScrollViewDragEndHandler;
		this.scrollView.onDragStarted += ScrollViewDragStartHandler;
		this.scrollView.restrictWithinPanel = false;
	}

	override public void MoveToData(object data, bool useTween = true)
	{
		if (dataProvider == null || data == null)
			return;

		int pos = dataProvider.IndexOf(data);

		if (pos < 0)
			return;

		MoveToPage(pos / pageFieldNum + 1, useTween);
	}

	/// <summary>
	/// 移到特定面頁
	/// </summary>
	/// <param name="page">Page.</param>
	/// <param name="useTween">If set to <c>true</c> use tween.</param>
	public bool MoveToPage(int page, bool useTween = true)
	{
		if (page < 1 || page > totalPage)
			return false;

		Vector2 clipOffset = scrollView.panel.clipOffset;

		this.currentPage = page;

		if (arrangement == Arrangement.Vertical)
		{
			float offsetY = clipOffset.y + (currentPage - 1) * cols * cellHeight;

			if (offsetY != 0)
				MoveToPosition(new Vector3(0, offsetY), useTween);
		}
		else
		{
			float offsetX = clipOffset.x - ((currentPage - 1) * rows * cellWidth);

			if (offsetX != 0)
				MoveToPosition(new Vector3(offsetX, 0), useTween);
		}

		if (PageChangedEvent != null)
			PageChangedEvent();

		return true;
	}

	protected override void UpdateListView ()
	{
		base.UpdateListView ();

		Vector3 pos = container.cachedTransform.localPosition;

		Vector4 clipping = GetClipping();

		totalPage = Mathf.CeilToInt((float)dataProvider.Count / (float)(cols * rows));
		if (arrangement == Arrangement.Vertical)
		{
			container.height = totalPage * cols * cellHeight;
			pos.y = -(container.height - clipping.w) / 2;
		}
		else
		{
			container.width = totalPage * rows * cellWidth;
			pos.x = (container.width - clipping.z) / 2;
		}
		container.cachedTransform.localPosition = pos;

		currentPage = 1;

		pageFieldNum = cols * rows;
	}

	/// <summary>
	/// 移到下一筆
	/// </summary>
	override public void MoveToNext(bool useTween = true)
	{
		MoveToPage(this.CurrentPage + 1, useTween);
	}
	
	/// <summary>
	/// 移到上一筆
	/// </summary>
	override public void MoveToPrevious(bool useTween = true)
	{
		MoveToPage(this.CurrentPage - 1, useTween);
	}
	
	/// <summary>
	/// 移到最後一筆
	/// </summary>
	override public void MoveToEnd(bool useTween = true)
	{
		MoveToPage(this.TotalPage, useTween);
	}
	
	/// <summary>
	/// 移到最前
	/// </summary>
	/// <param name="useTween">If set to <c>true</c> use tween.</param>
	override public void MoveToTop(bool useTween = true)
	{
		MoveToPage(1, useTween);
	}

	protected override int GetNextDataIndex (int index)
	{
		int nextIndex = position + index;

		if (arrangement == Arrangement.Horizontal)
		{
			//改變資料順序，水平排列
			int pageIndex = nextIndex / pageFieldNum;

			nextIndex %= pageFieldNum;

			nextIndex = nextIndex / cols + nextIndex % cols * rows;
			
			nextIndex += pageIndex * pageFieldNum;
		}
		return nextIndex;
	}

	private void ScrollViewDragStartHandler()
	{
		dragStartTime = Time.realtimeSinceStartup;
	}

	private void ScrollViewDragEndHandler ()
	{
		if (UICamera.currentTouch == null)
		{
			FitPage();
			return;
		}

		//依滑動距離切頁
		float delta = 0;
		int pageSize = 0;

		if (arrangement == Arrangement.Vertical)
		{
			pageSize = cols * cellHeight;
			delta = UICamera.currentTouch.totalDelta.y;
		}
		else
		{
			pageSize = rows * cellWidth;
			delta = -UICamera.currentTouch.totalDelta.x;
		}

		if (Math.Abs(delta) < pageSize)
		{
			if (Time.realtimeSinceStartup - dragStartTime <= FAST_TAP_DURATION &&  Mathf.Abs(delta) > FAST_TAP_DISTANCE)
			{
				//快撥
				if (delta > 0)
				{
					int page = currentPage + 1;
					page = Math.Min(page, totalPage);

					MoveToPage(page, true);
				}
				else
				{
					int page = currentPage - 1;
					page = Math.Max(page, 1);
					
					MoveToPage(page, true);
				}
			}
			else if (nextPageThreshold > 0)
			{
				if (delta > nextPageThreshold)
				{
					int page = currentPage + 1;
					page = Math.Min(page, totalPage);
					
					MoveToPage(page, true);
				}
				else if (delta < -nextPageThreshold)
				{
					int page = currentPage - 1;
					page = Math.Max(page, 1);
					
					MoveToPage(page, true);
				}
				else
				{
					FitPage();
				}
			}
		}
		else
		{
			FitPage();
		}
	}

	/// <summary>
	/// 調整列表座標以符合頁面
	/// </summary>
	private void FitPage()
	{
		//依列表位置切頁
		Vector2 clipOffset = scrollView.panel.clipOffset;
		
		if (arrangement == Arrangement.Vertical)
		{
			int pageHeight = cellHeight * cols;
			
			//取得頁面差異量
			float offsetY = clipOffset.y % pageHeight;
			
			if (Math.Abs(offsetY) > pageHeight / 2)
			{
				//超過一半時切到下頁
				if (offsetY < 0)
				{
					offsetY += pageHeight;
				}
				else
				{
					offsetY -= pageHeight;
				}
			}
			int nextPage = CalculateIndex(-(clipOffset.y - offsetY), pageHeight, totalPage - 1) + 1;

			MoveToPage(nextPage, true);
		}
		else
		{
			int pageWidth = cellWidth * rows;
			
			//取得頁面差異量
			float offsetX = clipOffset.x % pageWidth;
			if (Math.Abs(offsetX) > pageWidth / 2)
			{
				//超過一半時切到下頁
				if (offsetX < 0)
				{
					offsetX += pageWidth;
				}
				else
				{
					offsetX -= pageWidth;
				}
			}
			int nextPage = CalculateIndex(clipOffset.x - offsetX, pageWidth, totalPage - 1) + 1;

			MoveToPage(nextPage, true);
		}
	}
}

