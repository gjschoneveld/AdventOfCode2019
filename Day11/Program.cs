using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Day11
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

    enum Direction
    {
        Left,
        Up,
        Right,
        Down
    }

    class Program
    {
        static void Paint(List<BigInteger> program, Dictionary<(int x, int y), int> painted)
        {
            var data = new Dictionary<int, BigInteger>();

            for (int i = 0; i < program.Count; i++)
            {
                data[i] = program[i];
            }

            var computer = new Computer
            {
                Data = new Dictionary<int, BigInteger>(data),
                Inputs = new List<BigInteger> { }
            };

            var position = (x: 0, y: 0);
            var direction = Direction.Up;

            try
            {
                while (true)
                {
                    if (painted.ContainsKey(position))
                    {
                        computer.Inputs.Add(painted[position]);
                    }
                    else
                    {
                        computer.Inputs.Add(0);
                    }

                    painted[position] = (int)computer.NextOutput();

                    var turnCommand = (int)computer.NextOutput();

                    if (turnCommand == 0)
                    {
                        // turn left
                        switch (direction)
                        {
                            case Direction.Left:
                                direction = Direction.Down;
                                break;
                            case Direction.Up:
                                direction = Direction.Left;
                                break;
                            case Direction.Right:
                                direction = Direction.Up;
                                break;
                            case Direction.Down:
                                direction = Direction.Right;
                                break;
                        }
                    }
                    else
                    {
                        // turn right
                        switch (direction)
                        {
                            case Direction.Left:
                                direction = Direction.Up;
                                break;
                            case Direction.Up:
                                direction = Direction.Right;
                                break;
                            case Direction.Right:
                                direction = Direction.Down;
                                break;
                            case Direction.Down:
                                direction = Direction.Left;
                                break;
                        }
                    }

                    // step
                    switch (direction)
                    {
                        case Direction.Left:
                            position = (position.x - 1, position.y);
                            break;
                        case Direction.Up:
                            position = (position.x, position.y - 1);
                            break;
                        case Direction.Right:
                            position = (position.x + 1, position.y);
                            break;
                        case Direction.Down:
                            position = (position.x, position.y + 1);
                            break;
                    }
                }
            }
            catch
            {
            }
        }

        static void Main()
        {
            var input = File.ReadAllText("input.txt");
            var program = input.Split(',').Select(BigInteger.Parse).ToList();

            var painted = new Dictionary<(int x, int y), int>();

            Paint(program, painted);

            var answer1 = painted.Count;

            Console.WriteLine($"Answer 1: {answer1}");


            painted = new Dictionary<(int x, int y), int> { [(0, 0)] = 1 };

            Paint(program, painted);

            Console.WriteLine("Answer 2:");

            var minX = painted.Keys.Min(k => k.x);
            var maxX = painted.Keys.Max(k => k.x);

            var minY = painted.Keys.Min(k => k.y);
            var maxY = painted.Keys.Max(k => k.y);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = 0; x <= maxX; x++)
                {
                    if (painted.ContainsKey((x, y)) && painted[(x, y)] == 1)
                    {
                        Console.Write("█");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }

                Console.WriteLine();
            }
             
            Console.ReadKey();
        }
    }
}
