using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public static class RECompatibility
{

	public static void GetComponents<T>(GameObject source, List<T> components)
	where T : class
	{
		source.GetComponents<T>(components);
	}

	public static T GetComponent<T>(Component source)
	where T : class
	{
		return GetComponent<T>(source.gameObject);
	}

	public static T GetComponent<T>(GameObject source)
	where T : class
	{
		return source.GetComponent<T>();
	}

	public static T Instantiate<T>(T original)
	where T : UnityEngine.Object
	{
		var result = UnityEngine.Object.Instantiate(original);
#if UNITY_4_6 || UNITY_4_7
			return result as T;
#else
		return result;
#endif
	}

	public static T Instantiate<T>(T original, Transform parent)
	where T : Component
	{
		var result = UnityEngine.Object.Instantiate<T>(original, parent);
#if UNITY_4_6 || UNITY_4_7
			return result as T;
#else
		return result;
#endif
	}

	public static bool GetLayoutChildControlWidth(HorizontalOrVerticalLayoutGroup lg)
	{
#if UNITY_2017_1_OR_NEWER
		return lg.childControlWidth;
#else
			return true;
#endif
	}

	public static bool GetLayoutChildControlHeight(HorizontalOrVerticalLayoutGroup lg)
	{
#if UNITY_2017_1_OR_NEWER
		return lg.childControlHeight;
#else
			return true;
#endif
	}
}