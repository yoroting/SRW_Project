using UnityEngine;
using System.Collections;

/// <summary>
/// 項目列表
/// </summary>
public class UIItemList : UIList
{
	/// <summary>
	/// 自動貼齊項目
	/// </summary>
	public bool autoFit = false;

	private bool isFitted;

	/// <summary>
	/// 移到資料索引
	/// </summary>
	/// <param name="index">Index.</param>
	/// <param name="useTween">If set to <c>true</c> use tween.</param>
	override public void MoveToData(object data, bool useTween = true)
	{
		if (data == null || isOnePage)
			return;

		int index = dataProvider.IndexOf(data);

		if (index < 0)
			return;

		Vector3 clipOffset = scrollView.panel.clipOffset;

		if (arrangement == Arrangement.Vertical)
		{
			int colIdx = Mathf.CeilToInt(index / rows);

			colIdx = Mathf.Min(maxCols - cols, colIdx);

			float offsetY = colIdx * cellHeight + clipOffset.y;

			MoveToPosition(new Vector3(0, offsetY), useTween);
		}
		else
		{
			int rowIdx = Mathf.CeilToInt(index / cols);

			rowIdx = Mathf.Min(maxRows - rows, rowIdx);

			float offsetX = -(rowIdx * cellWidth - clipOffset.x);

			MoveToPosition(new Vector3(offsetX, 0), useTween);
		}
	}

	/// <summary>
	/// 移到下一筆
	/// </summary>
	override public void MoveToNext(bool useTween = true)
	{
		Vector3 clipOffset = scrollView.panel.clipOffset;

		if (arrangement == Arrangement.Vertical)
		{
			int nextCol = this.ColIndex + 1;

			if (nextCol <= maxCols - cols)
			{
				float offsetY = nextCol * cellHeight + clipOffset.y;
				MoveToPosition(new Vector3(0, offsetY), useTween);
			}
		}
		else
		{
			int nextRow = this.RowIndex + 1;
			
			if (nextRow <= maxRows - rows)
			{
				float offsetX = -(nextRow * cellWidth - clipOffset.x);
				MoveToPosition(new Vector3(offsetX, 0), useTween);
			}
		}
	}

	/// <summary>
	/// 移到上一筆
	/// </summary>
	override public void MoveToPrevious(bool useTween = true)
	{
		Vector3 clipOffset = scrollView.panel.clipOffset;

		if (arrangement == Arrangement.Vertical)
		{
			int nextCol = this.ColIndex - 1;
			
			if (nextCol >= 0)
			{
				float offsetY = nextCol * cellHeight + clipOffset.y;
				MoveToPosition(new Vector3(0, offsetY), useTween);
			}
		}
		else
		{
			int nextRow = this.RowIndex - 1;
			
			if (nextRow >= 0)
			{
				float offsetX = -(nextRow * cellWidth - clipOffset.x);
				MoveToPosition(new Vector3(offsetX, 0), useTween);
			}
		}
	}

	/// <summary>
	/// 移到最後一筆
	/// </summary>
	override public void MoveToEnd(bool useTween = true)
	{
		Vector3 clipOffset = scrollView.panel.clipOffset;
		
		if (arrangement == Arrangement.Vertical)
		{
			int nextCol = maxCols - cols;
			float offsetY = nextCol * cellHeight + clipOffset.y;
			MoveToPosition(new Vector3(0, offsetY), useTween);
		}
		else
		{
			int nextRow = maxRows - rows;
			float offsetX = -(nextRow * cellWidth - clipOffset.x);
			MoveToPosition(new Vector3(offsetX, 0), useTween);
		}
	}

	/// <summary>
	/// 移到最前
	/// </summary>
	/// <param name="useTween">If set to <c>true</c> use tween.</param>
	override public void MoveToTop(bool useTween = true)
	{
		Vector3 clipOffset = scrollView.panel.clipOffset;
		
		if (arrangement == Arrangement.Vertical)
		{
			MoveToPosition(new Vector3(0, clipOffset.y), useTween);
		}
		else
		{
			MoveToPosition(new Vector3(clipOffset.x, 0), useTween);
		}
	}

	protected override void Initialize ()
	{
		base.Initialize ();

		scrollView.onDragFinished += ScrollViewDragEndHandler;
	}
	
	protected override void Update ()
	{
		base.Update ();

		if (!isFitted)
		{
			isFitted = true;

			if (arrangement == Arrangement.Vertical)
				FitScrollY();
			else
				FitScrollX();
		}
	}

	private void FitScrollX()
	{
		Vector3 pos = scrollView.panel.CalculateConstrainOffset(scrollView.bounds.min, scrollView.bounds.max);
		
		if (pos.x == 0)
		{
			pos.x = scrollView.panel.clipOffset.x % cellWidth;;
			
			if (Mathf.Abs(pos.x) > cellWidth / 2)
			{
				if (pos.x < 0)
					pos.x += cellWidth;
				else
					pos.x -= cellWidth;
			}
		}
		MoveToPosition(pos, true);
	}
	
	private void FitScrollY()
	{
		Vector3 pos = scrollView.panel.CalculateConstrainOffset(scrollView.bounds.min, scrollView.bounds.max);

		if (pos.y == 0)
		{
			pos.y = scrollView.panel.clipOffset.y % cellHeight;
			
			if (Mathf.Abs(pos.y) > cellHeight / 2)
			{
				if (pos.y < 0)
					pos.y += cellHeight;
				else
					pos.y -= cellHeight;
			}
		}
		MoveToPosition(pos, true);
	}

	private void ScrollViewDragEndHandler ()
	{
		isFitted = !autoFit;
	}
}

