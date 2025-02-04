using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class REListViewCustomBase : UIBehaviour
{
	[NonSerialized]
	bool isListViewBaseInited;

	[SerializeField]
	[HideInInspector]
	protected List<REListViewItem> instances = new List<REListViewItem>();

	protected List<int> DisplayedIndices = new List<int>();

	public int DisplayedIndexLast
	{
		get
		{
			return DisplayedIndices.Count > 0 ? DisplayedIndices[DisplayedIndices.Count - 1] : -1;
		}
	}


	[SerializeField]
	[HideInInspector]
	public bool DestroyGameObjects = true;

	protected bool CanSetData;

	[SerializeField]
	protected ListViewDirection direction = ListViewDirection.Vertical;

	[SerializeField]
	protected bool setContentSizeFitter = true;

	public bool SetContentSizeFitter
	{
		get
		{
			return setContentSizeFitter;
		}

		set
		{
			setContentSizeFitter = value;

			UpdateLayoutBridgeContentSizeFitter();
		}
	}

	protected virtual bool RequireEasyLayout
	{
		get
		{
			return false;
		}
	}

	protected List<REListViewItem> Items
	{
		get
		{
			return new List<REListViewItem>(instances);
		}

		set
		{
			UpdateComponents(value);
		}
	}

	[SerializeField]
    public RectTransform Container;
    public RectTransform DisabledContainer
    {
        get;
        protected set;
    }

	[SerializeField]
	protected ScrollRect scrollRect;
	public ScrollRect ScrollRect
	{
		get
		{
			return scrollRect;
		}

		set
		{
			SetScrollRect(value);
		}
	}

	protected abstract void SetScrollRect(ScrollRect newScrollRect);


	protected override void OnEnable()
    {
        base.OnEnable();
    }


    protected override void OnValidate()
    {
        base.OnValidate();
    }

	public virtual void Init()
	{
		if (isListViewBaseInited)
		{
			return;
		}

		isListViewBaseInited = true;
		OnCanvasGroupChanged();
	}

	protected virtual void UpdateComponents<TItem>(List<TItem> newItems)
	where TItem : REListViewItem
	{
		for (int i = 0; i < instances.Count; i++)
		{
			if ((instances[i] != null) && (!newItems.Contains(instances[i] as TItem)))
			{
				Free(instances[i]);
			}
		}

		instances.Clear();
		for (int i = 0; i < newItems.Count; i++)
		{
			newItems[i].transform.SetParent(Container, false);
			newItems[i].Owner = this;
			instances.Add(newItems[i]);
		}
	}

	void Free(REListViewItem item)
	{
		if (item == null)
		{
			return;
		}

		item.Owner = null;
	}

	protected abstract void UpdateLayoutBridgeContentSizeFitter();



}
