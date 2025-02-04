

using UnityEngine;
using System.Collections.Generic;

public class RESimpleTable : REListViewCustom<RESimpleTableComponent, RESimpleTableItem>
{
    [SerializeField]
    private List<RESimpleTableItem> dataSourceExt;

    protected override void OnEnable()
    {
        //dataSource in base class cannot serialize
        base.FillDataSource(dataSourceExt);
        base.OnEnable();
    }

}