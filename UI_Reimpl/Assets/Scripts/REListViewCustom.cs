using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public partial class REListViewCustom<TItemView, TItem> : REListViewCustomBase
            where TItemView : REListViewItem
    //declare TItemView type for IListViewTemplateSelector convert TItemView -> REListViewItem
{
    [Serializable]
    public class Template : UIWidgets.REListViewItemTemplate<TItemView>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Template"/> class.
        /// </summary>
        public Template()
            : base()
        {
        }
    }

    protected float LastScrollingTime;
    protected bool Scrolling;

    [SerializeField]
    [Tooltip("Custom scroll inertia until reach item center.")]
    bool scrollInertiaUntilItemCenter;
    public bool ScrollInertiaUntilItemCenter
    {
        get
        {
            return scrollInertiaUntilItemCenter;
        }

        set
        {
            if (scrollInertiaUntilItemCenter != value)
            {
                scrollInertiaUntilItemCenter = value;

                ListRenderer.ToggleScrollToItemCenter(scrollInertiaUntilItemCenter);
            }
        }
    }

    [SerializeField]
    [HideInInspector]
    REListViewTypeSize listRenderer;
    protected REListViewTypeSize ListRenderer
    {
        get
        {
            if (listRenderer == null)
            {
                listRenderer = GetRendererTypeSize();
            }

            return listRenderer;
        }

        set
        {
            listRenderer = value;
        }
    }

    protected virtual REListViewTypeSize GetRendererTypeSize()
    {
        REListViewTypeSize renderer = new REListViewTypeSize(this); ;
        renderer.Enable();
        renderer.ToggleScrollToItemCenter(ScrollInertiaUntilItemCenter);
        return renderer;
    }

    protected bool DataSourceSetted;
    protected bool IsDataSourceChanged;

    protected bool NeedUpdateView;

    [SerializeField]
    [HideInInspector]
    protected List<TItemView> Instances = new List<TItemView>();

    protected void UpdateComponents(List<TItemView> newItems)
    {
        //Useless. No ListViewCustomBase.instances,
        //only ListViewCustom.Instances
    }

    [SerializeField]
    [HideInInspector]
    protected List<Template> SharedTemplates;

    [SerializeField]
    [HideInInspector]
    protected List<Template> OwnTemplates = new List<Template>();

    protected List<Template> Templates
    {
        get
        {
            if (SharedTemplates != null)
            {
                return SharedTemplates;
            }

            return OwnTemplates;
        }
    }

    protected virtual TItemView Index2Template(int index)
    {
        return TemplateSelector.Select(index, DataSource[index]);
    }


    protected Thread MainThread;

    protected bool IsMainThread
    {
        get
        {
            return MainThread != null && MainThread.Equals(Thread.CurrentThread);
        }
    }

    [SerializeField]
    protected bool virtualization = true;
    public bool Virtualization
    {
        get
        {
            return virtualization;
        }

        set
        {
            if (virtualization != value)
            {
                virtualization = value;
                UpdateView();
            }
        }
    }

    [SerializeField]
    [HideInInspector]
    protected List<int> InstancesDisplayedIndices = new List<int>();


    public bool IsHorizontal()
    {
        return false;  //Not Implemented
    }

    protected EasyLayoutNS.EasyLayout layout;

    /// <summary>
    /// Gets the layout.
    /// </summary>
    /// <value>The layout.</value>
    public EasyLayoutNS.EasyLayout Layout
    {
        get
        {
            if (layout == null)
            {
                layout = Container.GetComponent<EasyLayoutNS.EasyLayout>();
            }

            return layout;
        }
    }

    protected UIWidgets.ILayoutBridge LayoutBridge
    {
        get
        {
            if (layoutBridge == null)
            {
                if (Layout != null)
                {
                    layoutBridge = new UIWidgets.EasyLayoutBridge(
                        Layout, DefaultItem.transform as RectTransform,
                        true, true)
                    {
                        IsHorizontal = IsHorizontal(),
                    };
                    ListRenderer.DirectionChanged();
                }
                else
                {
                    var hv_layout = Container.GetComponent<HorizontalOrVerticalLayoutGroup>();
                    if (hv_layout != null)
                    {
                        layoutBridge = new UIWidgets.StandardLayoutBridge(
                            hv_layout, DefaultItem.transform as RectTransform, 
                            true);
                    }
                }
            }

            return layoutBridge;
        }
    }

    protected UIWidgets.ILayoutBridge layoutBridge;

    [SerializeField]
    [HideInInspector]
    protected List<ILayoutElement> LayoutElements = new List<ILayoutElement>();

    protected Vector2 ContainerAnchoredPosition
    {
        get
        {
            var pos = Container.anchoredPosition;
            var scale = Container.localScale;
            return new Vector2(pos.x / scale.x, pos.y / scale.y);
        }

        set
        {
            var scale = Container.localScale;
            Container.anchoredPosition = new Vector2(value.x * scale.x, value.y * scale.y);
        }
    }

    protected override void SetScrollRect(ScrollRect newScrollRect)
    {
        //Remove old listeners
        if (scrollRect != null)
        {
            ListRenderer.Disable();
            scrollRect.onValueChanged.RemoveListener(OnScrollRectUpdate);
        }

        scrollRect = newScrollRect;

        //Add new listeners
        if (scrollRect != null)
        {
            ListRenderer.Enable();
            scrollRect.onValueChanged.AddListener(OnScrollRectUpdate);

            UpdateScrollRectSize();
        }
    }

    /// <summary>
    /// Process the ScrollRect update event.
    /// </summary>
    /// <param name="position">Position.</param>
    protected virtual void OnScrollRectUpdate(Vector2 position)
    {
        StartScrolling();
    }

    /// <summary>
    /// Start to track scrolling event.
    /// </summary>
    protected virtual void StartScrolling()
    {
        LastScrollingTime = UIWidgets.UtilitiesTime.GetTime(true);
        if (Scrolling)
        {
            return;
        }

        Scrolling = true;
    }

    REListViewComponentPool componentsPool;

    /// <summary>
    /// The components pool.
    /// Constructor with lists needed to avoid lost connections when instantiated copy of the inited ListView.
    /// </summary>
    protected virtual REListViewComponentPool ComponentsPool
    {
        get
        {
            if (componentsPool == null)
            {
                componentsPool = new REListViewComponentPool(this, SetInstanceData);
                componentsPool.Init();
            }

            return componentsPool;
        }
    }

    DefaultSelector defaultTemplateSelector;

    /// <summary>
    /// Default template selector.
    /// </summary>
    protected DefaultSelector DefaultTemplateSelector
    {
        get
        {
            if (defaultTemplateSelector == null)
            {
                if (DefaultItem == null)
                {
                    Debug.LogError("ListView.DefaultItem is not specified.", this);
                }

                defaultTemplateSelector = new DefaultSelector(DefaultItem);
            }

            return defaultTemplateSelector;
        }
    }

    [SerializeField]
    TItemView defaultItem;

    /// <summary>
    /// The default item template.
    /// </summary>
    public TItemView DefaultItem
    {
        get
        {
            return defaultItem;
        }

        set
        {
            SetDefaultItem(value);
        }
    }

    protected virtual void SetDefaultItem(TItemView newDefaultItem)
    {
        if (newDefaultItem == null)
        {
            throw new ArgumentNullException("newDefaultItem");
        }

        defaultItem = newDefaultItem;
        DefaultTemplateSelector.Replace(newDefaultItem);
    }

    IListViewTemplateSelector<TItemView, TItem> templateSelector;

    /// <summary>
    /// Template selector.
    /// </summary>
    public IListViewTemplateSelector<TItemView, TItem> TemplateSelector
    {
        get
        {
            if (templateSelector == null)
            {
                templateSelector = DefaultTemplateSelector;
            }

            return templateSelector;
        }

        set
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (ReferenceEquals(templateSelector, value))
            {
                return;
            }

            templateSelector = value;

            if (componentsPool != null)
            {
                componentsPool.Destroy(templateSelector.AllTemplates());
            }

            //if (isListViewCustomInited)
            //{
            //    TemplatesChanged();
            //}
        }
    }

    [SerializeField]
    protected List<TItem> customItems = new List<TItem>();

    [NonSerialized] //overwritten in RESimpleTable
    private List<TItem> dataSource;
    
    public List<TItem> DataSource
    {
        get
        {
            if (dataSource == null)
            {
                dataSource = new List<TItem>(customItems);
            }

            if (!isListViewCustomInited)
            {
                Init();
            }

            return dataSource;
        }

        set
        {
            if (!isListViewCustomInited)
            {
                Init();
            }

            SetNewItems(value, IsMainThread);

            if (IsMainThread)
            {
                ListRenderer.SetPosition(0f);
            }
            else
            {
                DataSourceSetted = true;
            }
        }
    }

    public void FillDataSource(List<TItem> items)
    {
        dataSource = items;
    }

    protected Vector2 DefaultInstanceSize;
    [NonSerialized]
    protected bool isListViewCustomInited = false;

    public bool IsValid(int index)
    {
        return (index >= 0) && (index < DataSource.Count);
    }

    protected virtual void SetInstanceData(TItemView instance)
    {
        if (IsValid(instance.Index))
        {
            SetData(instance, DataSource[instance.Index]);
        }
    }

    protected virtual void SetData(TItemView component, TItem item)
    {
         (component as IREViewData<TItem>).SetData(item);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Init();
    }

    public override void Init()
    {
        if (isListViewCustomInited)
        {
            return;
        }

        isListViewCustomInited = true;

        MainThread = Thread.CurrentThread;

        var go = new GameObject("DisableContainer");
        go.SetActive(false);
        DisabledContainer = go.AddComponent<RectTransform>();
        DisabledContainer.SetParent(transform, false);

        foreach (var template in OwnTemplates)
        {
            template.SetOwner(this);
        }

        foreach (var template in Templates)
        {
            template.UpdateId();
        }

        base.Init();

        Items = new List<REListViewItem>();

        DestroyGameObjects = false;

        InitTemplates();

        if (ListRenderer.IsVirtualizationSupported())
        {
            ScrollRect = scrollRect;
            CalculateItemSize();
        }

        SetContentSizeFitter = setContentSizeFitter;

        SetDirection(direction, false);

        UpdateItems();
    }

    protected void InitTemplates()
    {
        CanSetData = true;

        foreach (var template in TemplateSelector.AllTemplates())
        {
            if (template.gameObject == null)
            {
                Debug.LogError("ListView.TemplateSelector.AllTemplates() has template without gameobject.", this);
                continue;
            }

            if (!(template is IREViewData<TItem>))
            {
                CanSetData = false;
            }

            ComponentsPool.GetTemplate(template);
        }
    }

    protected virtual void CalculateItemSize(bool reset = false)
    {
        DefaultInstanceSize = ListRenderer.CalculateDefaultInstanceSize(DefaultInstanceSize, reset);
    }

    /// <summary>
    /// Sets the displayed indices.
    /// </summary>
    /// <param name="isNewData">Is new data?</param>
    protected virtual void SetDisplayedIndices(bool isNewData = true)
    {
        if (isNewData)
        {
            ComponentsPool.DisplayedIndicesSet(DisplayedIndices);
        }
        else
        {
            ComponentsPool.DisplayedIndicesUpdate(DisplayedIndices);
        }

        ListRenderer.UpdateInstancesSizes();
        ListRenderer.UpdateLayout();
    }

    public void UpdateItems()
    {
        SetNewItems(DataSource, IsMainThread);
        IsDataSourceChanged = !IsMainThread;
    }

    protected virtual void SetNewItems(List<TItem> newItems, bool updateView = true)
    {
        lock (DataSource)
        {
            dataSource = newItems;

            ListRenderer.CalculateInstancesSizes(DataSource, false);

            CalculateMaxVisibleItems();

            if (updateView)
            {
                UpdateView();
            }
        }
    }

    protected virtual void CalculateMaxVisibleItems()
    {
        if (!isListViewCustomInited)
        {
            return;
        }

        ListRenderer.CalculateMaxVisibleItems();
    }

    protected float ScaledScrollRectSize()
    {
        var scale = Container.localScale;
        return IsHorizontal() ? ScrollRectSize.x / scale.x : ScrollRectSize.y / scale.y;
    }

    public void UpdateView()
    {
        NeedUpdateView = false;

        if (!isListViewCustomInited)
        {
            return;
        }

        ListRenderer.UpdateDisplayedIndices();

        SetDisplayedIndices();
    }

    protected override void UpdateLayoutBridgeContentSizeFitter()
    {
        if (LayoutBridge != null)
        {
            LayoutBridge.UpdateContentSizeFitter = SetContentSizeFitter && ListRenderer.AllowSetContentSizeFitter;
        }
    }

    protected void SetDirection(ListViewDirection newDirection, bool updateView = true)
    {
        direction = newDirection;

        ListRenderer.ResetPosition();

        if (ListRenderer.IsVirtualizationSupported())
        {
            LayoutBridge.IsHorizontal = IsHorizontal();
            ListRenderer.DirectionChanged();

            CalculateMaxVisibleItems();
        }

        if (updateView)
        {
            UpdateView();
        }
    }

#if UNITY_EDITOR
    /// <inheritdoc/>
    protected override void OnValidate()
    {
        if ((scrollRect == null) && (transform.childCount > 0))
        {
            scrollRect = transform.GetChild(0).GetComponent<ScrollRect>();
        }

        if ((Container == null) && (scrollRect != null))
        {
            Container = scrollRect.content;
        }

        if ((defaultItem == null) && (Container != null))
        {
            for (int i = 0; i < Container.childCount; i++)
            {
                defaultItem = Container.GetChild(i).GetComponent<TItemView>();
                if (defaultItem != null)
                {
                    break;
                }
            }
        }

        base.OnValidate();
    }
#endif

}
