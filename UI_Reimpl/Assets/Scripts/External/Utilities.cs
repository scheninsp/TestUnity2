namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Utilities.
	/// </summary>
	public static class Utilities
	{
		/// <summary>
		/// Get path to gameobject.
		/// </summary>
		/// <param name="go">GameObject.</param>
		/// <returns>Path.</returns>
		public static string GameObjectPath(GameObject go)
		{
			var parent = go.transform.parent;
			return parent == null ? go.name : string.Format("{0}/{1}", GameObjectPath(parent.gameObject), go.name);
		}

		/// <summary>
		/// Get hierarchy depth of transform.
		/// </summary>
		/// <param name="transform">Transform.</param>
		/// <returns>Depth.</returns>
		public static int GetDepth(Transform transform)
		{
			int depth = 0;

			while (transform.parent != null)
			{
				transform = transform.parent;
				depth += 1;
			}

			return depth;
		}

		/// <summary>
		/// Get or add component.
		/// </summary>
		/// <returns>Component.</returns>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">Component type.</typeparam>
		public static T GetOrAddComponent<T>(Component obj)
			where T : Component
		{
			var component = obj.GetComponent<T>();
			if (component == null)
			{
				component = obj.gameObject.AddComponent<T>();
			}

			return component;
		}

		/// <summary>
		/// Get or add component.
		/// </summary>
		/// <returns>Component.</returns>
		/// <param name="obj">Object.</param>
		/// <typeparam name="T">Component type.</typeparam>
		public static T GetOrAddComponent<T>(GameObject obj)
			where T : Component
		{
			var component = obj.GetComponent<T>();
			if (component == null)
			{
				component = obj.AddComponent<T>();
			}

			return component;
		}

		/// <summary>
		/// Get or add component.
		/// </summary>
		/// <param name="source">Source component.</param>
		/// <param name="target">Target component.</param>
		/// <typeparam name="T">Component type.</typeparam>
		public static void GetOrAddComponent<T>(Component source, ref T target)
			where T : Component
		{
			if ((target != null) || (source == null))
			{
				return;
			}

			target = GetOrAddComponent<T>(source);
		}

		/// <summary>
		/// Fix the instantiated object (in some cases object have wrong position, rotation and scale).
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="instance">Instance.</param>
		public static void FixInstantiated(Component source, Component instance)
		{
			FixInstantiated(source.gameObject, instance.gameObject);
		}

		/// <summary>
		/// Fix the instantiated object (in some cases object have wrong position, rotation and scale).
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="instance">Instance.</param>
		public static void FixInstantiated(GameObject source, GameObject instance)
		{
			var defaultRectTransform = source.transform as RectTransform;
			if (defaultRectTransform == null)
			{
				return;
			}

			var rectTransform = instance.transform as RectTransform;

			rectTransform.localPosition = defaultRectTransform.localPosition;
			rectTransform.localRotation = defaultRectTransform.localRotation;
			rectTransform.localScale = defaultRectTransform.localScale;
			rectTransform.anchoredPosition = defaultRectTransform.anchoredPosition;
			rectTransform.sizeDelta = defaultRectTransform.sizeDelta;
		}

		/// <summary>
		/// Default handle for the property changed event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event arguments.</param>
		public static void DefaultPropertyHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
		}

		/// <summary>
		/// Default handle for the changed event.
		/// </summary>
		public static void DefaultHandler()
		{
		}

	}
}