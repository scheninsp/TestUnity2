using System;
using UnityEngine;

[Serializable]
public class RESimpleTableItem
{
	[SerializeField]
	public Sprite Field1;

	[SerializeField]
	public string Field2;

	[SerializeField]
	public string Field3;

	[SerializeField]
	public string Field4;

	[SerializeField]
	public string Field5;

	[SerializeField]
	public string Field6;

	public override string ToString()
	{
		return string.Format("{0} | {1} | {2}", Field2, Field3, Field4);
	}
}
