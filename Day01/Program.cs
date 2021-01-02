using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day01
{
    class Program
    {
        static int CalculateFuel1(int mass)
        {
            var fuel = (mass / 3) - 2;

            if (fuel <= 0)
            {
                return 0;
            }

            return fuel;
        }

        static int CalculateFuel2(int mass)
        {
            var fuel = (mass / 3) - 2;

            if (fuel <= 0)
            {
                return 0;
            }

            var extra = CalculateFuel2(fuel);

            return fuel + extra;
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var masses = input.Select(int.Parse).ToList();

            var fuels1 = masses.Select(CalculateFuel1).ToList();
            var answer1 = fuels1.Sum();

            Console.WriteLine($"Answer 1: {answer1}");

            var fuels2 = masses.Select(CalculateFuel2).ToList();
            var answer2 = fuels2.Sum();

            Console.WriteLine($"Answer 2: {answer2}");
            Console.ReadKey();
        }
    }
}
