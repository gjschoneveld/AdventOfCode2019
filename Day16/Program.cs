using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day16
{
    class Program
    {
        static List<int> GenerateSums(List<int> input)
        {
            var result = new List<int>(input.Count + 1);

            result.Add(0);

            for (int to = 0; to < input.Count; to++)
            {
                result.Add(result[to] + input[to]);
            }

            return result;
        }

        static List<int> ApplyPhase(List<int> input)
        {
            var sums = GenerateSums(input);

            var result = new List<int>(input.Count);

            for (int i = 0; i < input.Count; i++)
            {
                var sum = 0;

                var add = true;
                var current = i;

                while (current < input.Count)
                {
                    var start = current;
                    var end = current + i + 1;

                    if (end > input.Count)
                    {
                        end = input.Count;
                    }

                    var delta = sums[end] - sums[start];

                    if (add)
                    {
                        sum += delta;
                    }
                    else
                    {
                        sum -= delta;
                    }

                    add = !add;
                    current += 2 * i + 2;
                }

                var digit = Math.Abs(sum) % 10;

                result.Add(digit);
            }

            return result;
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            var list = input.Select(c => c - '0').ToList();

            var list1 = list;

            var phases = 100;

            for (int i = 0; i < phases; i++)
            {
                list1 = ApplyPhase(list1);
            }

            var answer1 = string.Join("", list1.Take(8));

            Console.WriteLine($"Answer 1: {answer1}");


            var list2 = new List<int>();

            for (int i = 0; i < 10000; i++)
            {
                list2.AddRange(list);
            }

            for (int i = 0; i < phases; i++)
            {
                list2 = ApplyPhase(list2);
            }

            var offset = int.Parse(string.Join("", list.Take(7)));

            var answer2 = string.Join("", list2.Skip(offset).Take(8));

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
