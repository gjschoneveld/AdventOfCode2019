using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Day15
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
        North = 1,
        South = 2,
        West = 3,
        East = 4
    }

    class Program
    {
        const int Wall = 0;
        const int Regular = 1;
        const int Target = 2;

        static Computer computer;

        static int Move(Direction direction)
        {
            computer.Inputs.Add((int)direction);
            return (int)computer.NextOutput();
        }

        static HashSet<(int x, int y)> visited = new HashSet<(int x, int y)>();
        static (int x, int y) targetPosition;

        static List<(Direction direction, (int x, int y) position)> Neighbours((int x, int y) position)
        {
            return new List<(Direction direction, (int x, int y) position)>
            {
                (Direction.North, (position.x, position.y - 1)),
                (Direction.South, (position.x, position.y + 1)),
                (Direction.West, (position.x - 1, position.y)),
                (Direction.East, (position.x + 1, position.y))
            };
        }

        static int? StepsNeeded((int x, int y) position)
        {
            if (visited.Contains(position))
            {
                return null;
            }

            visited.Add(position);

            var possibleSteps = Neighbours(position);

            var results = new List<int>();

            foreach ((var direction, var nextPosition) in possibleSteps)
            {
                // step forward
                var result = Move(direction);

                if (result == Wall)
                {
                    continue;
                }

                if (result == Target)
                {
                    results.Add(1);
                }
                else if (result == Regular)
                {
                    var steps = StepsNeeded(nextPosition);

                    if (steps != null)
                    {
                        results.Add(steps.Value + 1);
                    }
                }

                // step back
                switch (direction)
                {
                    case Direction.North:
                        Move(Direction.South);
                        break;
                    case Direction.South:
                        Move(Direction.North);
                        break;
                    case Direction.West:
                        Move(Direction.East);
                        break;
                    case Direction.East:
                        Move(Direction.West);
                        break;
                }
            }

            if (results.Count == 0)
            {
                return null;
            }

            return results.Min();
        }

        static void Visit((int x, int y) position)
        {
            if (visited.Contains(position))
            {
                return;
            }

            visited.Add(position);

            var possibleSteps = Neighbours(position);

            foreach ((var direction, var nextPosition) in possibleSteps)
            {
                // step forward
                var result = Move(direction);

                if (result == Wall)
                {
                    continue;
                }

                if (result == Target)
                {
                    targetPosition = nextPosition;
                }

                Visit(nextPosition);

                // step back
                switch (direction)
                {
                    case Direction.North:
                        Move(Direction.South);
                        break;
                    case Direction.South:
                        Move(Direction.North);
                        break;
                    case Direction.West:
                        Move(Direction.East);
                        break;
                    case Direction.East:
                        Move(Direction.West);
                        break;
                }
            }
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

            computer = new Computer
            {
                Data = new Dictionary<int, BigInteger>(data),
                Inputs = new List<BigInteger> { }
            };

            var answer1 = StepsNeeded((0, 0));

            Console.WriteLine($"Answer 1: {answer1}");


            computer = new Computer
            {
                Data = new Dictionary<int, BigInteger>(data),
                Inputs = new List<BigInteger> { }
            };

            visited = new HashSet<(int x, int y)>();
            Visit((0, 0));

            var minutes = 0;

            var current = new List<(int x, int y)> { targetPosition };
            var processed = new HashSet<(int x, int y)>();

            while (current.Count > 0)
            {
                foreach (var pos in current)
                {
                    processed.Add(pos);
                }

                var neighbours = current.SelectMany(c => Neighbours(c)).Select(n => n.position).Distinct().ToList();

                current = neighbours.Where(n => visited.Contains(n) && !processed.Contains(n)).ToList();

                minutes++;
            }

            // minus 1 because in last step nothing is done
            var answer2 = minutes - 1;

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
