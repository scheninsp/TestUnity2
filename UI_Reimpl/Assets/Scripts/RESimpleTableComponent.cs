using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RESimpleTableComponent : REListViewItem, IREViewData<RESimpleTableItem>
{
	[SerializeField]
	public Image Field1;

	[SerializeField]
	public Text Field2;

	[SerializeField]
	public Text Field3;

	[SerializeField]
	public Text Field4;

	[SerializeField]
	public Text Field5;

	[SerializeField]
	public Text Field6;
	public virtual void SetData(RESimpleTableItem item)
	{
		Field1.sprite = item.Field1;
		Field2.text = item.Field2;
		Field3.text = item.Field3;
		Field4.text = item.Field4;
		Field5.text = item.Field5;
		Field6.text = item.Field6;
	}

}
