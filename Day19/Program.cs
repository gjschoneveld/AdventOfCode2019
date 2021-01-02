using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Day19
{
    class Computer
    {
        private int programCounter;
        private int inputCounter;
        private int relativeBase;

        public Dictionary<int, BigInteger> Data { get; set; }
        public List<BigInteger> Inputs { get; set; }

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
        static int GetValue((int x, int y) position)
        {
            if (position.x < 0 || position.y < 0)
            {
                return 0;
            }

            var data = new Dictionary<int, BigInteger>();

            for (int i = 0; i < program.Count; i++)
            {
                data[i] = program[i];
            }

            var computer = new Computer
            {
                Data = new Dictionary<int, BigInteger>(data),
                Inputs = new List<BigInteger> { position.x, position.y }
            };

            return (int)computer.NextOutput();
        }

        static List<BigInteger> program;

        static void Main()
        {
            var input = File.ReadAllText("input.txt");
            program = input.Split(',').Select(BigInteger.Parse).ToList();


            var affected = 0;

            for (int y = 0; y < 50; y++)
            {
                for (int x = 0; x < 50; x++)
                {
                    var result = GetValue((x, y));

                    if (result == 1)
                    {
                        affected++;
                    }
                }
            }

            var answer1 = affected;

            Console.WriteLine($"Answer 1: {answer1}");

            var size = 100;

            var upperRight = (x: size - 1, y: 0);

            while (true)
            {
                // move down till top of the beam
                while (GetValue(upperRight) == 0)
                {
                    upperRight = (upperRight.x, upperRight.y + 1);
                }

                // if upperRight and bottomLeft are in the beam, the whole square is
                var bottomLeft = (x: upperRight.x - size + 1, y: upperRight.y + size - 1);

                if (GetValue(bottomLeft) == 1)
                {
                    break;
                }

                // move one to the right
                upperRight = (upperRight.x + 1, upperRight.y);
            }

            var upperLeft = (x: upperRight.x - size + 1, y: upperRight.y);

            var answer2 = upperLeft.x * 10000 + upperLeft.y;

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }

}
