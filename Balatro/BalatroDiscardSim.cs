using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

namespace BalatroSim
{
    public class BalatroDiscardSim : MonoBehaviour
    {
        public int discard_num;

        void Start()
        {
            TestValidate5();
        }

        //probability of drawing n cards ang get a flush
        void TestValidate1()
        {
            int draw_num = 8;

            //https://math.stackexchange.com/questions/2621581/probability-of-drawing-a-5-card-flush-given-n-cards
            BigInteger C13_5 = BMath.C(13, 5);
            BigInteger C52_n = BMath.C(52, draw_num);

            //number of flush
            BigInteger comb1 = 0;
            for (int i = draw_num; i > 4; i--)
            {
                comb1 += BMath.C(13, i) * 4 * BMath.C(3 * 13, draw_num - i);
            }

            //number of all
            BigInteger comb2 = C52_n;
            //number of full house with flush (draw_num = 8)
            BigInteger comb3 = C13_5 * 5 * 3;
            //number of 4-of-a-kind with flush (draw_num = 8)
            BigInteger comb4 = C13_5 * 5 * BMath.C(3, 2) * 4 * 3;
            double prob = (double)(comb1) / (double)comb2;
            //draw_num=7, p = 0.305
            //draw_num=8, p = 0.696
            Debug.Log($"result : flush={comb1} all={comb2}  prob:{prob} == 0.069");
        }

        void TestValidate3()
        {
            BigInteger a = BMath.C(52, 8);
            BigInteger b = BMath.C(44, 5);
            BigInteger f = BMath.C(8, 5);

            BigInteger c = a * b * f;
            Debug.Log($"result : a={a} b={b}  f:{f} c:{c}");
        }

        void TestValidate4()
        {
            int total = 145;
            int m1 = 59;
            //int n = 28;
            //int k = 5;

            int n = 15;
            int k = 5;

            float p = LeastSelectProb(total, m1, n, k);

            Debug.Log($"total {total} blue ball m1 {m1}, " +
                $"select {n} times, at least {k}, prob = {p}");

        }

        //blue ball prob:pb
        //select n times to get at least k blue ball
        float LeastSelectProb(int total, int m1, int n, int k)
        {
            //TODO: BigInteger may overflow
            BigInteger sum = 0;
            for(int i=(k-1); i>=0; i--)
            {
                sum += BMath.C(m1, i) * BMath.C(total - m1, n - i);
            }
            BigInteger ss = BMath.C(total, n);

            //UnityEngine.Mathf.Exp can only accept float input
            float p = BMath.BigIntegerDivide(sum, ss);
            return 1-p;
        }

        //additive mult : a, multiplicative mult b, economy c, replica e
        //copy 3
        void TestValidate5()
        {
            BigInteger sum = (BMath.C(9, 2) * BMath.C(20, 2) * BMath.C(18, 1) +
BMath.C(9, 2) * BMath.C(3, 1) * BMath.C(20, 1) * BMath.C(18, 1) +
BMath.C(9, 1) * BMath.C(3, 1) * BMath.C(20, 2) * BMath.C(18, 1));
            BigInteger ss = BMath.C(53, 5);
            float p = BMath.BigIntegerDivide(sum, ss);

            BigInteger sum2 = BMath.C(6, 2) * BMath.C(9, 1) * BMath.C(20, 1) * BMath.C(18, 1) +
BMath.C(6, 1) * BMath.C(9, 2) * BMath.C(20, 1) * BMath.C(18, 1);
            float p2 = BMath.BigIntegerDivide(sum2, ss);

            float psum = p + p2;

            Debug.Log($"TestValidate5 : sum: {sum} sum2: {sum2} ss: {ss}," +
                $" p: {p} p2: {p2} psum: {psum}");
        }

    }

}
