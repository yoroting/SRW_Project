using UnityEngine;
using System;
using System.Collections;

public interface IItemRenderer
{
    object Data
    {
        get;
        set;
    }

    bool Selected
    {
        get;
        set;
    }

    int DataIndex
    {
        get;
        set;
    }

    IItemList ListView
    {
        get;
        set;
    }
}

public abstract class AbstractItemRenderer : UIComponent, IItemRenderer
{
    protected object data;

    protected bool selected;

    protected IItemList listView;

    private int dataIndex;

    public IItemList ListView
    {
        get { return this.listView; }
        set { listView = value; }
    }

    public int DataIndex
    {
        get { return this.dataIndex; }
        set { dataIndex = value; }
    }

    public virtual object Data
    {
        get { return data; }

        set
        {
			if (data == value)
				return;

			if (data != null && data.Equals(value))
                return;

            data = value;

            Invalidate();
        }
    }

    public bool Selected
    {
        get { return selected; }

        set
        {
            if (selected == value)
                return;

            selected = value;

            Invalidate();
        }
    }

    protected override void Initialize()
    {
    }

    protected override void Destroy()
    {
    }
}

