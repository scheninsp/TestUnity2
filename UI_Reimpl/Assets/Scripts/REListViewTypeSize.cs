using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public partial class REListViewCustom<TItemView, TItem> : REListViewCustomBase
            where TItemView : REListViewItem
{

    /// <summary>
    /// ListView renderer with items of variable size.
    /// </summary>
    protected class REListViewTypeSize
    {
        #region Visibility
        protected Visibility Visible;

        protected struct Visibility : IEquatable<Visibility>
        {
            /// <summary>
            /// First visible index.
            /// </summary>
            public int FirstVisible;

            /// <summary>
            /// Count of the visible items.
            /// </summary>
            public int Items;

            /// <summary>
            /// Last visible index.
            /// </summary>
            public int LastVisible
            {
                get
                {
                    return FirstVisible + Items;
                }
            }

            /// <summary>
            /// Determines whether the specified object is equal to the current object.
            /// </summary>
            /// <param name="obj">The object to compare with the current object.</param>
            /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
            public override bool Equals(object obj)
            {
                if (obj is Visibility)
                {
                    return Equals((Visibility)obj);
                }

                return false;
            }

            /// <summary>
            /// Determines whether the specified object is equal to the current object.
            /// </summary>
            /// <param name="other">The object to compare with the current object.</param>
            /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
            public bool Equals(Visibility other)
            {
                return FirstVisible == other.FirstVisible && Items == other.Items;
            }

            /// <summary>
            /// Hash function.
            /// </summary>
            /// <returns>A hash code for the current object.</returns>
            public override int GetHashCode()
            {
                return FirstVisible ^ Items;
            }

            /// <summary>
            /// Compare specified visibility data.
            /// </summary>
            /// <param name="obj1">First data.</param>
            /// <param name="obj2">Second data.</param>
            /// <returns>true if the data are equal; otherwise, false.</returns>
            public static bool operator ==(Visibility obj1, Visibility obj2)
            {
                return obj1.Equals(obj2);
            }

            /// <summary>
            /// Compare specified visibility data.
            /// </summary>
            /// <param name="obj1">First data.</param>
            /// <param name="obj2">Second data.</param>
            /// <returns>true if the data not equal; otherwise, false.</returns>
            public static bool operator !=(Visibility obj1, Visibility obj2)
            {
                return !obj1.Equals(obj2);
            }
        }

        protected const int MinVisibleItems = 2;

        /// <summary>
        /// Maximal count of the visible items.
        /// </summary>
        public int MaxVisibleItems
        {
            get;
            protected set;
        }

        public virtual int VisibleIndex2ItemIndex(int index)
        {
            if (index < 0)
            {
                index += Owner.DataSource.Count * Mathf.CeilToInt((float)-index / (float)Owner.DataSource.Count);
            }

            return index % Owner.DataSource.Count;
        }
        #endregion

        //that's why called REListViewType "Size"
        public bool SupportVariableSize
        {
            get
            {
                return true;
            }
        }

        public virtual bool AllowSetContentSizeFitter
        {
            get
            {
                return true;
            }
        }

        #region InstanceSizes
        protected readonly UIWidgets.IInstanceSizes<TItem> InstanceSizes;

        protected bool HasInstanceSize(TItem item)
        {
            return InstanceSizes.Contains(item);
        }

        protected void SaveInstanceSize(TItem item, Vector2 size)
        {
            InstanceSizes[item] = size;
        }

        protected float GetInstanceSize(int index)
        {
            var size = GetInstanceFullSize(index);
            return size.y;
        }

        public Vector2 GetInstanceFullSize(int index)
        {
            return GetInstanceFullSize(Owner.DataSource[index]);
        }

        protected float GetInstanceSize(TItem item)
        {
            var size = GetInstanceFullSize(item);
            return Owner.IsHorizontal() ? size.x : size.y;
        }

        protected Vector2 GetInstanceFullSize(TItem item)
        {
            return InstanceSizes.Get(item, Owner.DefaultInstanceSize);
        }

        public void CalculateInstancesSizes(List<TItem> items, bool forceUpdate = true)
        {
            RemoveOldItems(items);

            for (int index = 0; index < items.Count; index++)
            {
                var item = items[index];
                if (!HasInstanceSize(item))
                {
                    var template = Owner.ComponentsPool.GetTemplate(index);
                    template.EnableTemplate();

                    SaveInstanceSize(item, CalculateInstanceSize(item, template));
                }
            }

            Owner.ComponentsPool.DisableTemplates();
        }

        protected virtual Vector2 CalculateInstanceSize(TItem item, Template template)
        {
            Owner.SetData(template.Template, item);

            LayoutRebuilder.ForceRebuildLayoutImmediate(Owner.Container);

            return template.Template.RectTransform.rect.size;
        }

        void UpdateInstanceSize(TItemView instance, Vector2 size)
        {
            var current_size = instance.RectTransform.rect.size;

            if (Owner.IsHorizontal())
            {
                size.x = Mathf.Round(size.x);
                if (!Mathf.Approximately(current_size.x, size.x))
                {
                    instance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
                }
            }
            else
            {
                size.y = Mathf.Round(size.y);
                if (!Mathf.Approximately(current_size.y, size.y))
                {
                    instance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
                }
            }
        }

        protected virtual void UpdateInstanceSize(TItemView instance)
        {
            var item = Owner.DataSource[instance.Index];
            if (!InstanceSizes.TryGetOverridden(item, out var size))
            {
                return;
            }

            UpdateInstanceSize(instance, size);
        }


        public virtual void UpdateInstancesSizes()
        {
            foreach (var instance in Owner.Instances)
            {
                UpdateInstanceSize(instance);
            }
        }

        #endregion

        public void CalculateMaxVisibleItems()
        {
            CalculateInstancesSizes(Owner.DataSource, false);

            MaxVisibleItems = CalculateMaxVisibleItems(Owner.DataSource);
        }

        protected int CalculateMaxVisibleItems(List<TItem> items)
        {
            if (!Owner.Virtualization)
            {
                return Owner.DataSource.Count;
            }

            if (items.Count == 0)
            {
                return 0;
            }

            var visible = MaxVisible(Owner.IsHorizontal(), 
                Owner.ScaledScrollRectSize(),
                Owner.LayoutBridge.GetSpacing());

            return MinVisibleItems + visible;
        }

        protected int MaxVisible(bool isHorizontal, float visibleAreaSize, float spacing)
        {
            return InstanceSizes.Visible(isHorizontal, visibleAreaSize, spacing);
        }

        #region variables
        protected REListViewCustom<TItemView, TItem> Owner;
        bool isEnabled;
        protected bool DefaultInertia;

        bool IsTileView = false;

        #endregion

        public REListViewTypeSize(REListViewCustom<TItemView, TItem> owner)
        {
            Owner = owner;
            if(InstanceSizes == null)
            {
                InstanceSizes = new UIWidgets.InstanceSizes<TItem>();
            }
        }

        public void Enable()
        {
            if (isEnabled)
            {
                return;
            }

            if (Owner.ScrollRect != null)
            {
                DefaultInertia = Owner.ScrollRect.inertia;
                Owner.ScrollRect.onValueChanged.AddListener(OnScroll);
                isEnabled = true;
            }
        }

        /// <inheritdoc/>
        public void Disable()
        {
            if (!isEnabled)
            {
                return;
            }

            if (Owner.ScrollRect != null)
            {
                Owner.ScrollRect.onValueChanged.RemoveListener(OnScroll);
                isEnabled = false;
            }
        }

        /// <summary>
        /// Process scroll event.
        /// </summary>
        /// <param name="unused">Scroll value.</param>
        protected void OnScroll(Vector2 unused)
        {
            UpdateView();
        }

        public virtual void UpdateView()
        {
            if (!UpdateDisplayedIndices())
            {
                return;
            }

            Owner.SetDisplayedIndices(IsTileView);
        }

        public virtual bool UpdateDisplayedIndices()
        {
            var new_visible = VisibilityData();
            if (new_visible == Visible)
            {
                return false;
            }

            Visible = new_visible;

            Owner.DisplayedIndices.Clear();

            for (int i = Visible.FirstVisible; i < Visible.LastVisible; i++)
            {
                Owner.DisplayedIndices.Add(VisibleIndex2ItemIndex(i));
            }

            return true;
        }

        public float GetPosition()
        {
            var pos = GetPositionVector();
            return pos.y;
        }

        public Vector2 GetPositionVector()
        {
            var result = Owner.ContainerAnchoredPosition;

            if (float.IsNaN(result.x))
            {
                result.x = 0f;
            }

            if (float.IsNaN(result.y))
            {
                result.y = 0f;
            }

            return result;
        }

        public void SetPosition(float value, bool updateView = true)
        {
            if ((Owner.ScrollRect == null) || Owner.ScrollRect.content == null)
            {
                return;
            }

            var current_position = Owner.ContainerAnchoredPosition;
            var new_position = Owner.IsHorizontal()
                ? new Vector2(-value, current_position.y)
                : new Vector2(current_position.x, value);

            SetPosition(new_position, updateView);
        }

        public void SetPosition(Vector2 newPosition, bool updateView = true)
        {
            var current_position = Owner.ContainerAnchoredPosition;
            var diff = (Owner.IsHorizontal() && !Mathf.Approximately(current_position.x, newPosition.x))
                    || (!Owner.IsHorizontal() && !Mathf.Approximately(current_position.y, newPosition.y));

            if (diff)
            {
                Owner.ContainerAnchoredPosition = newPosition;

                if (updateView)
                {
                    UpdateView();
                }
            }

            if (Owner.ScrollRect != null)
            {
                Owner.ScrollRect.StopMovement();
            }
        }

        public int GetFirstVisibleIndex(bool strict = false)
        {
            var first_visible_index = Mathf.Max(0, GetIndexAtPosition(GetPosition()));
            return Mathf.Min(first_visible_index, Mathf.Max(0, Owner.DataSource.Count - MinVisibleItems));
        }


        protected virtual Visibility VisibilityData()
        {
            var visible = default(Visibility);


            if (Owner.DataSource.Count > 0)
            {
                visible.FirstVisible = GetFirstVisibleIndex();
                visible.Items = Mathf.Min(MaxVisibleItems, Owner.DataSource.Count);
                if ((visible.FirstVisible + visible.Items) > Owner.DataSource.Count)
                {
                    visible.Items = Owner.DataSource.Count - visible.FirstVisible;
                    if (visible.Items < 1)
                    {
                        visible.Items = Mathf.Min(Owner.DataSource.Count,
                                        visible.Items + 1);
                        visible.FirstVisible = Owner.DataSource.Count - visible.Items;
                    }
                }
            }
            else
            {
                visible.FirstVisible = 0;
                visible.Items = Owner.DataSource.Count;
            }

            return visible;
        }

        int GetIndexAtPosition(float position)
        {
            var result = GetIndexAtPosition(position, NearestType.Before);
            if (result >= Owner.DataSource.Count)
            {
                result = Owner.DataSource.Count - 1;
            }

            return result;
        }

        int GetIndexAtPosition(float position, NearestType type)
        {
            var spacing = Owner.LayoutBridge.GetSpacing();
            var index = 0;
            for (int i = 0; i < Owner.DataSource.Count; i++)
            {
                index = i;

                var item_size = GetInstanceSize(i);
                if (i > 0)
                {
                    item_size += spacing;
                }

                if (position < item_size)
                {
                    break;
                }

                position -= item_size;
            }

            switch (type)
            {
                case NearestType.Auto:
                    var item_size = GetInstanceSize(index);
                    if (position >= (item_size / 2f))
                    {
                        index += 1;
                    }

                    break;
                case NearestType.Before:
                    break;
                case NearestType.After:
                    index += 1;
                    break;
                default:
                    throw new NotSupportedException(
                        string.Format("Unsupported NearestType: {0}",
                        UIWidgets.EnumHelper<NearestType>.ToString(type)));
            }

            return index;
        }

        public virtual void DirectionChanged()
        {
            if (Owner.Layout != null)
            {
                Owner.Layout.MainAxis = !Owner.IsHorizontal() ? EasyLayoutNS.Axis.Horizontal : EasyLayoutNS.Axis.Vertical;
            }
        }

        protected virtual void RemoveOldItems(List<TItem> items)
        {
            InstanceSizes.RemoveUnexisting(items);
        }

        public virtual bool IsVirtualizationSupported()
        {
            return IsVirtualizationPossible();
        }

        public virtual bool IsVirtualizationPossible()
        {
            var has_scrollrect = Owner.ScrollRect != null;
            var has_container = Owner.Container != null;
            var layout = has_container
                ? ((Owner.layout != null) ? Owner.layout : Owner.Container.GetComponent<LayoutGroup>())
                : null;
            var valid_layout = false;
            if (layout != null)
            {
                var is_easy_layout = layout is EasyLayoutNS.EasyLayout;
                valid_layout = Owner.RequireEasyLayout
                    ? is_easy_layout
                    : (is_easy_layout || (layout is HorizontalOrVerticalLayoutGroup));
            }

            return has_scrollrect && valid_layout;
        }

        public virtual Vector2 CalculateDefaultInstanceSize(Vector2 currentSize, bool reset = false)
        {
            Owner.DefaultItem.gameObject.SetActive(true);

            var rt = Owner.DefaultItem.transform as RectTransform;

            Owner.LayoutElements.Clear();
            RECompatibility.GetComponents<ILayoutElement>(Owner.DefaultItem.gameObject, Owner.LayoutElements);
            Owner.LayoutElements.Sort(LayoutElementsComparison);

            var size = currentSize;

            if ((size.x == 0f) || reset)
            {
                var preff_width = SupportVariableSize ? PreferredWidth(Owner.LayoutElements) : 0f;
                size.x = Mathf.Max(Mathf.Max(preff_width, rt.rect.width), 1f);
                if (float.IsNaN(size.x))
                {
                    size.x = 1f;
                }
            }

            if ((size.y == 0f) || reset)
            {
                var preff_height = SupportVariableSize ? PreferredHeight(Owner.LayoutElements) : 0f;
                size.y = Mathf.Max(Mathf.Max(preff_height, rt.rect.height), 1f);
                if (float.IsNaN(size.y))
                {
                    size.y = 1f;
                }
            }

            Owner.DefaultItem.gameObject.SetActive(false);

            return size;
        }

        protected static int LayoutElementsComparison(ILayoutElement x, ILayoutElement y)
        {
            return -x.layoutPriority.CompareTo(y.layoutPriority);
        }

        static float PreferredHeight(List<ILayoutElement> elems)
        {
            if (elems.Count == 0)
            {
                return 0f;
            }

            var priority = elems[0].layoutPriority;
            var result = -1f;
            for (int i = 0; i < elems.Count; i++)
            {
                if ((result > -1f) && (elems[i].layoutPriority < priority))
                {
                    break;
                }

                result = Mathf.Max(Mathf.Max(result, elems[i].minHeight), elems[i].preferredHeight);
                priority = elems[i].layoutPriority;
            }

            return result;
        }

        static float PreferredWidth(List<ILayoutElement> elems)
        {
            if (elems.Count == 0)
            {
                return 0f;
            }

            var priority = elems[0].layoutPriority;
            var result = -1f;
            for (int i = 0; i < elems.Count; i++)
            {
                if ((result > -1f) && (elems[i].layoutPriority < priority))
                {
                    break;
                }

                result = Mathf.Max(Mathf.Max(result, elems[i].minWidth), elems[i].preferredWidth);
                priority = elems[i].layoutPriority;
            }

            return result;
        }
        public void ResetPosition()
        {
            Owner.ContainerAnchoredPosition = Vector2.zero;

            if (Owner.ScrollRect != null)
            {
                Owner.ScrollRect.horizontal = Owner.IsHorizontal();
                Owner.ScrollRect.vertical = !Owner.IsHorizontal();
                Owner.ScrollRect.StopMovement();
            }
        }

        public void ToggleScrollToItemCenter(bool enable)
        {
            if (Owner.ScrollRect == null)
            {
                return;
            }

            if (enable)
            {
                DefaultInertia = Owner.ScrollRect.inertia;
                Owner.ScrollRect.inertia = false;
            }
            else
            {
                Owner.ScrollRect.inertia = DefaultInertia;
            }
        }

        public void UpdateLayout()
        {
            if (!Owner.Virtualization)
            {
                Owner.LayoutBridge.SetFiller(0f, 0f);
            }
            else
            {
                var top = TopFillerSize();
                var bottom = BottomFillerSize();

                Owner.LayoutBridge.SetFiller(top, bottom);
            }

            // Owner.LayoutBridge.UpdateLayout();
        }

        public float TopFillerSize()
        {
            return GetItemPosition(Visible.FirstVisible) - Owner.LayoutBridge.GetMargin();
        }

        public float GetItemPosition(int index)
        {
            var n = Mathf.Min(index, Owner.DataSource.Count);
            var size = 0f;
            for (int i = 0; i < n; i++)
            {
                size += GetInstanceSize(Owner.DataSource[i]);
            }

            return size + (Owner.LayoutBridge.GetSpacing() * index) + Owner.LayoutBridge.GetMargin();
        }

        public float BottomFillerSize()
        {
            var last = Owner.DisplayedIndexLast + 1;
            var size = last < 0 ? 0f : GetItemsSize(last, Owner.DataSource.Count - last);
            if (size > 0f)
            {
                size += Owner.LayoutBridge.GetSpacing();
            }

            return size;
        }

        protected float GetItemsSize(int start, int count)
        {
            if (count == 0)
            {
                return 0f;
            }

            var width = 0f;
            var n = Mathf.Min(start + count, Owner.DataSource.Count);
            for (int i = start; i < n; i++)
            {
                width += GetInstanceSize(Owner.DataSource[VisibleIndex2ItemIndex(i)]);
            }

            width += Owner.LayoutBridge.GetSpacing() * (count - 1);

            return Mathf.Max(0, width);
        }

    }//end REListViewTypeSize

}
