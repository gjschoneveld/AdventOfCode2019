using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Day25
{
    class Computer
    {
        private int programCounter;
        private int inputCounter;
        private int relativeBase;

        public Dictionary<int, BigInteger> Data { get; set; }
        public List<BigInteger> Inputs { get; set; }

        public bool Interactive { get; set; }

        public IEnumerator<BigInteger> InteractiveInput { get; set; }


        public const int PositionMode = 0;
        public const int ImmidiateMode = 1;
        public const int RelativeMode = 2;

        BigInteger GetValue(int index, int mode)
        {
            BigInteger value = 0;

            if (Data.ContainsKey(index))
            {
                value = Data[index];
            }

            if (mode == ImmidiateMode)
            {
                return value;
            }

            var address = (int)value;

            if (mode == RelativeMode)
            {
                address += relativeBase;
            }

            if (Data.ContainsKey(address))
            {
                return Data[address];
            }

            return 0;
        }

        void SetValue(int index, int mode, BigInteger value)
        {
            var address = (int)Data[index];

            if (mode == RelativeMode)
            {
                address += relativeBase;
            }

            Data[address] = value;
        }

        IEnumerable<BigInteger> InteractiveInputGenerator()
        {
            while (true)
            {
                var command = Console.ReadLine();

                foreach (var c in command)
                {
                    yield return c;
                }

                yield return 10;
            }
        }

        public BigInteger NextOutput()
        {
            const int add = 1;
            const int mult = 2;
            const int input = 3;
            const int output = 4;
            const int jumpIfTrue = 5;
            const int jumpIfFalse = 6;
            const int lessThan = 7;
            const int equals = 8;
            const int adjustsRelativeBase = 9;
            const int stop = 99;

            while (Data[programCounter] != stop)
            {
                var instruction = (int)Data[programCounter];
                var opcode = instruction % 100;

                var mode1 = instruction / 100 % 10;
                var mode2 = instruction / 1000 % 10;
                var mode3 = instruction / 10000 % 10;

                var first = GetValue(programCounter + 1, mode1);
                var second = GetValue(programCounter + 2, mode2);

                switch (opcode)
                {
                    case add:
                        SetValue(programCounter + 3, mode3, first + second);
                        programCounter += 4;
                        break;
                    case mult:
                        SetValue(programCounter + 3, mode3, first * second);
                        programCounter += 4;
                        break;
                    case input:
                        if (Interactive)
                        {
                            if (InteractiveInput == null)
                            {
                                InteractiveInput = InteractiveInputGenerator().GetEnumerator();
                            }

                            InteractiveInput.MoveNext();
                            SetValue(programCounter + 1, mode1, InteractiveInput.Current);
                            programCounter += 2;
                            break;
                        }

                        Console.Write((char)Inputs[inputCounter]);
                        SetValue(programCounter + 1, mode1, Inputs[inputCounter++]);
                        programCounter += 2;
                        break;
                    case output:
                        programCounter += 2;
                        return first;
                    case jumpIfTrue:
                        programCounter = first != 0 ? (int)second : programCounter + 3;
                        break;
                    case jumpIfFalse:
                        programCounter = first == 0 ? (int)second : programCounter + 3;
                        break;
                    case lessThan:
                        SetValue(programCounter + 3, mode3, first < second ? 1 : 0);
                        programCounter += 4;
                        break;
                    case equals:
                        SetValue(programCounter + 3, mode3, first == second ? 1 : 0);
                        programCounter += 4;
                        break;
                    case adjustsRelativeBase:
                        relativeBase += (int)first;
                        programCounter += 2;
                        break;
                }
            }

            throw new Exception("No output");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            var program = input.Split(',').Select(BigInteger.Parse).ToList();

            var data = new Dictionary<int, BigInteger>();

            for (int i = 0; i < program.Count; i++)
            {
                data[i] = program[i];
            }

            var commands = File.ReadAllLines("commands.txt").Where(c => !c.StartsWith(";"));

            var computer = new Computer
            {
                //Interactive = true,
                Data = new Dictionary<int, BigInteger>(data),
                Inputs = $"{string.Join('\n', commands)}\n".Select(c => (BigInteger)c).ToList(),
            };

            try
            {
                while (true)
                {
                    var character = (char)computer.NextOutput();
                    Console.Write(character);
                }
            }
            catch
            {
            }
        }
    }
}
