using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using REStyles;


[RequireComponent(typeof(Image))]
public class REListViewItem : UIBehaviour
{
	bool isInited;

	Vector2 oldSize = new Vector2(-1f, -1f);

	[HideInInspector]
    public int Index = -1;

    [HideInInspector]
    public REListViewCustomBase Owner;

	[HideInInspector]
	public bool DisableRecycling;

	Image background;
	public Image Background
	{
		get
		{
			Init();

			return background;
		}
	}

	RectTransform rectTransform;
	public RectTransform RectTransform
	{
		get
		{
			Init();

			return rectTransform;
		}
	}

	public virtual void Init()
	{
		if (isInited)
		{
			return;
		}

		isInited = true;

		background = GetComponent<Image>();
		rectTransform = transform as RectTransform;
	}

	public virtual void MovedToCache()
	{
		UIWidgets.Updater.RemoveRunOnceNextFrame(OnRectTransformDimensionsChange);
	}

	//UIBehaviour function override
	protected override void OnRectTransformDimensionsChange()
	{
		var current = RectTransform.rect.size;
		if (oldSize.Equals(current))
		{
			return;
		}

		oldSize = current;
	}


}
