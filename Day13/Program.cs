using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Day13
{
    class Computer
    {
        private int programCounter;
        private int relativeBase;

        public Dictionary<int, BigInteger> Data { get; set; }
        public Func<BigInteger> Input { get; set; }

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
                        SetValue(programCounter + 1, mode1, Input());
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
        const int empty = 0;
        const int wall = 1;
        const int block = 2;
        const int paddle = 3;
        const int ball = 4;

        static void Print(Dictionary<(int x, int y), int> tiles, int score)
        {
            var minX = tiles.Min(t => t.Key.x);
            var maxX = tiles.Max(t => t.Key.x);
            var minY = tiles.Min(t => t.Key.y);
            var maxY = tiles.Max(t => t.Key.y);

            Console.CursorVisible = false;

            Console.CursorTop = 1;
            Console.CursorLeft = 0;
            Console.WriteLine($"Score: {score}");

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    switch (tiles[(x, y)])
                    {
                        case empty:
                        default:
                            Console.Write(' ');
                            break;
                        case wall:
                            Console.Write("█");
                            break;
                        case block:
                            Console.Write('#');
                            break;
                        case paddle:
                            Console.Write('-');
                            break;
                        case ball:
                            Console.Write('O');
                            break;
                    }
                }

                Console.WriteLine();
            }

            Console.CursorVisible = true;
        }

        static int GetPaddleX(Dictionary<(int x, int y), int> tiles)
        {
            return tiles.First(t => t.Value == paddle).Key.x;
        }

        static int GetBallX(Dictionary<(int x, int y), int> tiles)
        {
            return tiles.First(t => t.Value == ball).Key.x;
        }

        static void Main()
        {
            var input = File.ReadAllText("input.txt");
            var program = input.Split(',').Select(BigInteger.Parse).ToList();

            var data = new Dictionary<int, BigInteger>();

            for (int i = 0; i < program.Count; i++)
            {
                data[i] = program[i];
            }

            var computer = new Computer
            {
                Data = new Dictionary<int, BigInteger>(data),
            };

            var tiles = new Dictionary<(int x, int y), int>();

            try
            {
                while (true)
                {
                    var x = (int)computer.NextOutput();
                    var y = (int)computer.NextOutput();
                    var tileId = (int)computer.NextOutput();

                    tiles[(x, y)] = tileId;
                }
            }
            catch
            {
            }

            var answer1 = tiles.Count(t => t.Value == block);

            Console.WriteLine($"Answer 1: {answer1}");


            var score = 0;

            computer = new Computer
            {
                Data = new Dictionary<int, BigInteger>(data),
                Input = () =>
                {
                    Print(tiles, score);
                    System.Threading.Thread.Sleep(10);

                    var ballX = GetBallX(tiles);
                    var paddleX = GetPaddleX(tiles);

                    return Math.Sign(ballX - paddleX);
                }
            };

            computer.Data[0] = 2;

            try
            {
                while (true)
                {
                    var x = (int)computer.NextOutput();
                    var y = (int)computer.NextOutput();
                    var value = (int)computer.NextOutput();

                    if (x == -1 && y == 0)
                    {
                        score = value;
                    }
                    else
                    {
                        tiles[(x, y)] = value;
                    }
                }
            }
            catch
            {
            }

            Print(tiles, score);
            var answer2 = score;

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
