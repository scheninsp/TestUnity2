using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RESimpleTableComponent : REListViewItem, IREViewData<RESimpleTableItem>
{
	[SerializeField]
	public Text Field1;

	[SerializeField]
	public Text Field2;

	[SerializeField]
	public Text Field3;

	public virtual void SetData(RESimpleTableItem item)
	{
		Field1.text = item.Field1;
		Field2.text = item.Field2;
		Field3.text = item.Field3;
	}

}
