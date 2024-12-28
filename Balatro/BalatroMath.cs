using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

//use BigInteger
//https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types

namespace BalatroSim
{

    public static class BMath
    {
        
        public static BigInteger Factorial(BigInteger n)
        {
            BigInteger result = 1;
            for (BigInteger i = n; i > 1; i--){
                result *= i;
            }
            return result;
        }

        public static BigInteger C(int n, int k)
        {
            BigInteger numerator = 1;
            for(BigInteger i= n; i>n-k; i--)
            {
                numerator *= i;
            }
            BigInteger denominator = Factorial(k);
            return numerator / denominator;
        }

        public static float BigIntegerDivide(BigInteger x, BigInteger y)
        {
            return (float)System.Math.Exp(BigInteger.Log(x) - BigInteger.Log(y));
        }
    }
}
