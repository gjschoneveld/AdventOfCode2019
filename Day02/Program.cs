using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day02
{
    class Program
    {
        static int RunProgram(List<int> data, int noun, int verb)
        {
            data = new List<int>(data)
            {
                [1] = noun,
                [2] = verb
            };

            var programCounter = 0;

            const int add = 1;
            const int mult = 2;
            const int stop = 99;

            while (data[programCounter] != stop)
            {
                switch (data[programCounter])
                {
                    case add:
                        data[data[programCounter + 3]] = data[data[programCounter + 1]] + data[data[programCounter + 2]];
                        break;
                    case mult:
                        data[data[programCounter + 3]] = data[data[programCounter + 1]] * data[data[programCounter + 2]];
                        break;
                }

                programCounter += 4;
            }

            return data[0];
        }

        static int Find(List<int> data, int target)
        {
            for (int noun = 0; noun <= 99; noun++)
            {
                for (int verb = 0; verb <= 99; verb++)
                {
                    var result = RunProgram(data, noun, verb);

                    if (result == target)
                    {
                        return 100 * noun + verb;
                    }
                }
            }

            return -1;
        }

        static void Main()
        {
            var input = File.ReadAllText("input.txt");
            var data = input.Split(',').Select(int.Parse).ToList();

            var answer1 = RunProgram(data, 12, 2);

            Console.WriteLine($"Answer 1: {answer1}");

            var answer2 = Find(data, 19690720);

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
