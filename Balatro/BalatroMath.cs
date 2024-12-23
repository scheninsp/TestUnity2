using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//use ulong
//https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types

namespace BalatroSim
{

    public static class BMath
    {
        
        public static ulong Factorial(ulong n)
        {
            ulong result = 1;
            for (ulong i = n; i > 1; i--){
                result *= i;
            }
            return result;
        }

        public static ulong C(ulong n, ulong k)
        {
            ulong numerator = 1;
            for(ulong i= n; i>n-k; i--)
            {
                numerator *= i;
            }
            ulong denominator = Factorial(k);
            return numerator / denominator;
        }

    }
}
