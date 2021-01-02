using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day07
{
    class Computer
    {
        private int programCounter;
        private int inputCounter;

        public List<int> Data { get; set; }
        public List<int> Inputs { get; set; }

        int GetValue(int index, int mode)
        {
            var value = 0;

            if (0 <= index && index < Data.Count)
            {
                value = Data[index];
            }

            if (mode == 1)
            {
                return value;
            }

            if (0 <= value && value < Data.Count)
            {
                return Data[value];
            }

            return 0;
        }

        void SetValue(int index, int value)
        {
            Data[Data[index]] = value;
        }

        public int NextOutput()
        {
            const int add = 1;
            const int mult = 2;
            const int input = 3;
            const int output = 4;
            const int jumpIfTrue = 5;
            const int jumpIfFalse = 6;
            const int lessThan = 7;
            const int equals = 8;
            const int stop = 99;

            while (Data[programCounter] != stop)
            {
                var instruction = Data[programCounter];
                var opcode = instruction % 100;

                var mode1 = instruction / 100 % 10;
                var mode2 = instruction / 1000 % 10;

                var first = GetValue(programCounter + 1, mode1);
                var second = GetValue(programCounter + 2, mode2);

                switch (opcode)
                {
                    case add:
                        SetValue(programCounter + 3, first + second);
                        programCounter += 4;
                        break;
                    case mult:
                        SetValue(programCounter + 3, first * second);
                        programCounter += 4;
                        break;
                    case input:
                        SetValue(programCounter + 1, Inputs[inputCounter++]);
                        programCounter += 2;
                        break;
                    case output:
                        programCounter += 2;
                        return first;
                    case jumpIfTrue:
                        programCounter = first != 0 ? second : programCounter + 3;
                        break;
                    case jumpIfFalse:
                        programCounter = first == 0 ? second : programCounter + 3;
                        break;
                    case lessThan:
                        SetValue(programCounter + 3, first < second ? 1 : 0);
                        programCounter += 4;
                        break;
                    case equals:
                        SetValue(programCounter + 3, first == second ? 1 : 0);
                        programCounter += 4;
                        break;
                }
            }

            throw new Exception("No output");
        }
    }

    class Program
    {
        static IEnumerable<List<int>> Permutations(List<int> choices)
        {
            if (choices.Count == 1)
            {
                yield return choices;
                yield break;
            }

            foreach (var choice in choices)
            {
                var remaining = choices.Where(c => c != choice).ToList();

                var inner = Permutations(remaining);

                foreach (var i in inner)
                {
                    yield return new List<int>(i) { choice };
                }
            }
        }

        static void Main()
        {
            var input = File.ReadAllText("input.txt");
            var data = input.Split(',').Select(int.Parse).ToList();

            var max = 0;

            foreach (var combi in Permutations(new List<int> { 0, 1, 2, 3, 4 }))
            {
                var value = 0;

                foreach (var x in combi)
                {
                    var computer = new Computer
                    {
                        Data = new List<int>(data),
                        Inputs = new List<int> { x, value }
                    };

                    value = computer.NextOutput();
                }

                max = Math.Max(max, value);
            }

            var answer1 = max;

            Console.WriteLine($"Answer 1: {answer1}");


            max = 0;

            foreach (var combi in Permutations(new List<int> { 5, 6, 7, 8, 9 }))
            {
                var value = 0;

                var computers = new Dictionary<int, Computer>();

                foreach (var x in combi)
                {
                    computers[x] = new Computer
                    {
                        Data = new List<int>(data),
                        Inputs = new List<int> { x }
                    };
                }

                try
                {
                    while (true)
                    {
                        foreach (var x in combi)
                        {
                            computers[x].Inputs.Add(value);

                            value = computers[x].NextOutput();
                        }
                    }
                }
                catch
                {
                    max = Math.Max(max, value);
                }
            }

            var answer2 = max;

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
