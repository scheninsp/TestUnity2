using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public partial class REListViewCustom<TItemView, TItem> : REListViewCustomBase
                where TItemView : REListViewItem
{
    public class REListViewComponentPool
    {
		protected class Diff
		{
			/// <summary>
			/// Indices of the added items.
			/// </summary>
			protected List<int> Added = new List<int>();

			/// <summary>
			/// Indices of the removed items.
			/// </summary>
			protected List<int> Removed = new List<int>();

			/// <summary>
			/// Indices of the untouched items.
			/// </summary>
			protected List<int> Untouched = new List<int>();

			/// <summary>
			/// Indices of the displayed items.
			/// </summary>
			public List<int> Displayed = new List<int>();

			/// <summary>
			/// Calculate difference.
			/// </summary>
			/// <param name="current">Current indices.</param>
			/// <param name="required">Required indices.</param>
			public void Calculate(List<int> current, List<int> required)
			{
				Added.Clear();
				Removed.Clear();
				Untouched.Clear();
				Displayed.Clear();

				foreach (var index in required)
				{
					if (!current.Contains(index))
					{
						Added.Add(index);
					}
				}

				foreach (var index in current)
				{
					if (!required.Contains(index))
					{
						Removed.Add(index);
					}
					else
					{
						Untouched.Add(index);
					}
				}

				Displayed.AddRange(required);
			}

			/// <summary>
			/// Check if indices are same.
			/// </summary>
			/// <param name="current">Current indices.</param>
			/// <param name="required">Required indices.</param>
			/// <returns>true if indices are same; otherwise false.</returns>
			public bool Same(List<int> current, List<int> required)
			{
				if (current.Count != required.Count)
				{
					return false;
				}

				for (int i = 0; i < current.Count; i++)
				{
					if (current[i] != required[i])
					{
						return false;
					}
				}

				return true;
			}
		}


		public readonly REListViewCustom<TItemView, TItem> Owner;
		public readonly UIWidgets.InstanceID OwnerID;

		public Action<TItemView> SetInstanceData;
		public Func<int, TItemView, bool> InstanceCompare = 
			(int index, TItemView instance) => instance.Index == index;

		internal REListViewComponentPool(REListViewCustom<TItemView, TItem> owner, Action<TItemView> setInstanceData)
		{
			Owner = owner;
			OwnerID = new UIWidgets.InstanceID(owner);
			SetInstanceData = setInstanceData;
		}

		protected Diff IndicesDiff = new Diff();


		public void Init()
        {
			foreach (var template in Owner.Templates)
			{
				template.UpdateId();
			}
		}

        public void DisplayedIndicesSet(List<int> newIndices)
        {
			PrepareInstances(newIndices);

			RequestInstances(newIndices, allAsNew: true);

			UpdateOwnerData(newIndices);
		}

        public void DisplayedIndicesUpdate(List<int> newIndices)
        {
			if (IndicesDiff.Same(Owner.InstancesDisplayedIndices, newIndices))
			{
				return;
			}

			PrepareInstances(newIndices);

			RequestInstances(newIndices, allAsNew: false);

			UpdateOwnerData(newIndices);
		}

		protected void PrepareInstances(List<int> indices)
		{
			foreach (var template in Owner.Templates)
			{
				template.RequiredInstances = 0;
			}

			foreach (var index in indices)
			{
				GetTemplate(index).Require(
					Owner, OwnerID, index, InstanceCompare);
			}

			foreach (var template in Owner.Templates)
			{
				template.RequestInstances(Owner, OwnerID);
			}
		}

		protected void RequestInstances(List<int> indices, bool allAsNew = false)
		{
			Owner.Instances.Clear();

			bool is_new;
			foreach (var index in indices)
			{
				var instance = GetTemplate(index).RequestInstance(OwnerID, index, InstanceCompare, out is_new);
				Owner.Instances.Add(instance);

				if (is_new || allAsNew)
				{
					instance.Index = index;
					SetInstanceData(instance);
				}
			}
		}

		protected void UpdateOwnerData(List<int> indices)
		{
			SetOwnerItems();

			Owner.InstancesDisplayedIndices.Clear();
			Owner.InstancesDisplayedIndices.AddRange(indices);

			foreach (var c in Owner.Instances)
			{
				c.RectTransform.SetAsLastSibling();
			}
		}

		protected void SetOwnerItems()
		{
			Owner.UpdateComponents(Owner.Instances);
		}


		public virtual void Destroy(TItemView[] excludeTemplates)
		{
			for (int i = Owner.Templates.Count - 1; i >= 0; i--)
			{
				var template = Owner.Templates[i];
				if (Array.IndexOf(excludeTemplates, template.Template) != -1)
				{
					continue;
				}

				template.Destroy(OwnerID);
				Owner.Templates.RemoveAt(i);
			}
		}

		public Template GetTemplate(int index)
		{
			var template = Owner.Index2Template(index);

			return GetTemplate(template);
		}

		public Template GetTemplate(TItemView component)
		{
			var id = new UIWidgets.InstanceID(component);

			foreach (var template in Owner.Templates)
			{
				if (template.TemplateID == id)
				{
					return template;
				}
			}

			var created = CreateTemplate(component);
			Owner.Templates.Add(created);

			return created;
		}

		public Template CreateTemplate(TItemView component)
		{
			var created = UIWidgets.REListViewItemTemplate<TItemView>.Create<Template>(component);
			return created;
		}

		public virtual void DisableTemplates()
		{
			foreach (var template in Owner.Templates)
			{
				template.DisableTemplate();
			}
		}
	}
}


