using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day04
{
    class Program
    {
        static bool IsValid(string candidate, bool partOne)
        {
            var hasPair = false;
            var isDecreasing = false;

            for (int i = 0; i < candidate.Length - 1; i++)
            {
                var pair = candidate[i] == candidate[i + 1];
                var leftPair = i > 0 && candidate[i - 1] == candidate[i];
                var rightPair = i < candidate.Length - 2 && candidate[i + 1] == candidate[i + 2];

                if (partOne && pair || !partOne && pair && !leftPair && !rightPair)
                {
                    hasPair = true;
                }

                if (candidate[i] > candidate[i + 1])
                {
                    isDecreasing = true;
                }
            }

            return hasPair && !isDecreasing;
        }

        static void Main(string[] args)
        {
            var from = 234208;
            var to = 765869;

            var validPasswords1 = 0;
            var validPasswords2 = 0;

            for (int candidate = from; candidate <= to; candidate++)
            {
                if (IsValid(candidate.ToString(), true))
                {
                    validPasswords1++;
                }

                if (IsValid(candidate.ToString(), false))
                {
                    validPasswords2++;
                }
            }

            var answer1 = validPasswords1;

            Console.WriteLine($"Answer 1: {answer1}");

            var answer2 = validPasswords2;

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();

        }
    }
}
