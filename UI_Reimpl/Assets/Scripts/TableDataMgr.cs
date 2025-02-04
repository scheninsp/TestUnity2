using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIReimpl
{
    public class TableItemData
    {
        public string name;
        public float price;
        public float to_last_ratio; //与上一周期比上涨下跌百分比
        public float cost;  //成本
        public float hold_num;  //持有数量
        public float exchange_num;  //正在交易数量,+买入 -卖出
    }

    public class TableDataMgr : MonoBehaviour
    {
        int batch_data_num = 100;

        public List<TableItemData> item_data_list;

        private void Awake()
        {
            item_data_list = new List<TableItemData>();
        }

        void Start()
        {
            Init();
        }

        public void Init()
        {
            RequestData(batch_data_num);
        }

        public void RequestData(int batch_data_num)
        {
            //Simulate Data
            for (int i = 0; i < batch_data_num; i++)
            {
                item_data_list.Add(SimulateOneData());
            }
        }

        public static TableItemData SimulateOneData()
        {
            TableItemData data = new();

            int index = Random.Range(1, 10000);
            data.name = "item" + index.ToString();
            data.price = Random.Range(0.0f, 100.0f);
            data.to_last_ratio = 0;
            data.cost = 0;
            data.hold_num = 0;
            data.exchange_num = 0;

            return data;
        }

        public static void SimulateOneDataUpdate(TableItemData data)
        {
            float old_price = data.price;
            data.to_last_ratio = Random.Range(-10.0f, 10.0f);
            data.price = old_price * (1 + data.to_last_ratio);
        }

        void Update()
        {

        }
    }

}
