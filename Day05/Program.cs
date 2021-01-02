using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day05
{
    class Program
    {
        static int GetValue(List<int> data, int index, int mode)
        {
            var value = 0;

            if (0 <= index && index < data.Count)
            {
                value = data[index];
            }

            if (mode == 1)
            {
                return value;
            }

            if (0 <= value && value < data.Count)
            {
                return data[value];
            }

            return 0;
        }

        static void SetValue(List<int> data, int index, int value)
        {
            data[data[index]] = value;
        }

        static int RunProgram(List<int> data, int id)
        {
            data = new List<int>(data);

            var programCounter = 0;

            const int add = 1;
            const int mult = 2;
            const int input = 3;
            const int output = 4;
            const int jumpIfTrue = 5;
            const int jumpIfFalse = 6;
            const int lessThan = 7;
            const int equals = 8;
            const int stop = 99;

            var result = 0;

            while (data[programCounter] != stop)
            {
                var instruction = data[programCounter];
                var opcode = instruction % 100;

                var mode1 = instruction / 100 % 10;
                var mode2 = instruction / 1000 % 10;

                var first = GetValue(data, programCounter + 1, mode1);
                var second = GetValue(data, programCounter + 2, mode2);

                switch (opcode)
                {
                    case add:
                        SetValue(data, programCounter + 3, first + second);
                        programCounter += 4;
                        break;
                    case mult:
                        SetValue(data, programCounter + 3, first * second);
                        programCounter += 4;
                        break;
                    case input:
                        SetValue(data, programCounter + 1, id);
                        programCounter += 2;
                        break;
                    case output:
                        result = first;
                        programCounter += 2;
                        break;
                    case jumpIfTrue:
                        programCounter = first != 0 ? second : programCounter + 3;
                        break;
                    case jumpIfFalse:
                        programCounter = first == 0 ? second : programCounter + 3;
                        break;
                    case lessThan:
                        SetValue(data, programCounter + 3, first < second ? 1 : 0);
                        programCounter += 4;
                        break;
                    case equals:
                        SetValue(data, programCounter + 3, first == second ? 1 : 0);
                        programCounter += 4;
                        break;
                }
            }

            return result;
        }

        static void Main()
        {
            var input = File.ReadAllText("input.txt");
            var data = input.Split(',').Select(int.Parse).ToList();

            var answer1 = RunProgram(data, 1);

            Console.WriteLine($"Answer 1: {answer1}");

            var answer2 = RunProgram(data, 5);

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
