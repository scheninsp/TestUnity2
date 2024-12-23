using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BalatroSim
{
    public class BalatroDiscardSim : MonoBehaviour
    {
        public int discard_num;

        void Start()
        {
            TestValidate1();
        }

        //probability of drawing n cards ang get a flush
        void TestValidate1()
        {
            ulong draw_num = 8;

            //https://math.stackexchange.com/questions/2621581/probability-of-drawing-a-5-card-flush-given-n-cards
            ulong C13_5 = BMath.C(13, 5);
            ulong C52_n = BMath.C(52, draw_num);

            //number of flush
            ulong comb1 = 0;
            for(ulong i= draw_num; i>4; i--)
            {
                comb1 += BMath.C(13, i) * 4 * BMath.C(3*13, draw_num-i);
            }

            //number of all
            ulong comb2 = C52_n;
            //number of full house with flush (draw_num = 8)
            ulong comb3 = C13_5 * 5 * 3;
            //number of 4-of-a-kind with flush (draw_num = 8)
            ulong comb4 = C13_5 * 5 * BMath.C(3, 2) * 4 * 3;
            double prob = (double)(comb1) / (double)comb2;
            //draw_num=7, p = 0.305
            //draw_num=8, p = 0.696
            Debug.Log($"result : flush={comb1} all={comb2}  prob:{prob} == 0.069");
        }
    }

}
