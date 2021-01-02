using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Day17
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
        static List<(int x, int y)> Neighbours((int x, int y) position)
        {
            return new List<(int x, int y)>
            {
                (position.x, position.y - 1),
                (position.x, position.y + 1),
                (position.x - 1, position.y),
                (position.x + 1, position.y)
            };
        }

        static List<string> GeneratePatterns(int length)
        {
            var result = new List<string>();

            var values = new int[length - 1];

            while (true)
            {
                // increment
                var index = values.Length - 1;

                while (true)
                {
                    if (index < 0)
                    {
                        return result;
                    }

                    values[index]++;

                    if (values[index] < 3)
                    {
                        break;
                    }

                    values[index] = 0;
                    index--;
                }

                // must contain a B and a C
                if (!values.Any(v => v == 1) || !values.Any(v => v == 2))
                {
                    continue;
                }

                // must start with zero or more As and then a B
                index = 0;
                while (values[index] == 0)
                {
                    index++;
                }

                if (values[index] != 1)
                {
                    continue;
                }

                // construct result
                var pattern = "A" + string.Join("", values.Select(v => (char)(v + 'A')));
                result.Add(pattern);
            }
        }

        static string RouteToString(List<(char turn, int steps)> route)
        {
            return string.Join(",", route.Select(x => $"{x.turn},{x.steps}"));
        }

        static Dictionary<char, List<(char turn, int steps)>> Split(List<(char turn, int steps)> route, string pattern)
        {
            var countA = pattern.Count(p => p == 'A');
            var countB = pattern.Count(p => p == 'B');
            var countC = pattern.Count(p => p == 'C');

            var lengths = new Dictionary<char, int>();

            var maxA = (route.Count - countB - countC) / countA;

            for (lengths['A'] = 1; lengths['A'] <= maxA; lengths['A']++)
            {
                var maxB = (route.Count - countA * lengths['A'] - countC) / countB;

                for (lengths['B'] = 1; lengths['B'] <= maxB; lengths['B']++)
                {
                    lengths['C'] = (route.Count - countA * lengths['A'] - countB * lengths['B']) / countC;

                    if (countA * lengths['A'] + countB * lengths['B'] + countC * lengths['C'] != route.Count)
                    {
                        continue;
                    }

                    var patterns = new Dictionary<char, List<(char turn, int steps)>>();

                    var index = 0;
                    var error = false;

                    foreach (var p in pattern)
                    {
                        var part = route.Skip(index).Take(lengths[p]).ToList();

                        if (!patterns.ContainsKey(p))
                        {
                            patterns[p] = new List<(char, int)>(part);
                        }
                        else if (!patterns[p].SequenceEqual(part))
                        {
                            error = true;
                            break;
                        }

                        index += lengths[p];
                    }

                    if (error)
                    {
                        continue;
                    }

                    var patternStrings = patterns.Select(kv => RouteToString(kv.Value));

                    if (patternStrings.Any(p => p.Length > 20))
                    {
                        continue;
                    }

                    return patterns;
                }
            }

            return null;
        }

        enum Direction
        {
            Left,
            Up,
            Right,
            Down
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
                Inputs = new List<BigInteger> { }
            };

            var field = new Dictionary<(int x, int y), char>();

            try
            {
                int x = 0;
                int y = 0;

                while (true)
                {
                    var character = (char)computer.NextOutput();

                    if (character != '\n')
                    {
                        //Console.Write(character);

                        field[(x, y)] = character;

                        x++;
                    }
                    else
                    {
                        //Console.WriteLine();
                        x = 0;
                        y++;
                    }
                }
            }
            catch
            {
            }

            var intersections = field.Where(c => Neighbours(c.Key).All(n => field.ContainsKey(n) && field[n] != '.' )).ToList();

            var answer1 = intersections.Sum(i => i.Key.x * i.Key.y);

            Console.WriteLine($"Answer 1: {answer1}");


            // find route
            var startDirections = new Dictionary<char, Direction>
            {
                ['^'] = Direction.Up,
                ['v'] = Direction.Down,
                ['<'] = Direction.Left,
                ['>'] = Direction.Right
            };

            var start = field.Single(f => startDirections.ContainsKey(f.Value));

            var route = new List<(char, int)>();

            var direction = startDirections[start.Value];
            var position = start.Key;

            char turn = '?'; 
            var steps = 0;

            while (true)
            {
                var northPosition = (position.x, position.y - 1);
                var southPosition = (position.x, position.y + 1);
                var westPosition = (position.x - 1, position.y);
                var eastPosition = (position.x + 1, position.y);

                var forwardPosition = (0, 0);
                var leftDirection = Direction.Up;
                var leftPosition = (0, 0);
                var rightDirection = Direction.Up;
                var rightPosition = (0, 0);

                switch (direction)
                {
                    case Direction.Left:
                        forwardPosition = westPosition;
                        leftDirection = Direction.Down;
                        leftPosition = southPosition;
                        rightDirection = Direction.Up;
                        rightPosition = northPosition;
                        break;
                    case Direction.Up:
                        forwardPosition = northPosition;
                        leftDirection = Direction.Left;
                        leftPosition = westPosition;
                        rightDirection = Direction.Right;
                        rightPosition = eastPosition;
                        break;
                    case Direction.Right:
                        forwardPosition = eastPosition;
                        leftDirection = Direction.Up;
                        leftPosition = northPosition;
                        rightDirection = Direction.Down;
                        rightPosition = southPosition;
                        break;
                    case Direction.Down:
                        forwardPosition = southPosition;
                        leftDirection = Direction.Right;
                        leftPosition = eastPosition;
                        rightDirection = Direction.Left;
                        rightPosition = westPosition;
                        break;
                }

                if (field.ContainsKey(forwardPosition) && field[forwardPosition] != '.')
                {
                    position = forwardPosition;
                    steps++;
                }
                else
                {
                    if (steps > 0)
                    {
                        route.Add((turn, steps));
                        steps = 0;
                    }

                    if (field.ContainsKey(leftPosition) && field[leftPosition] != '.')
                    {
                        turn = 'L';
                        direction = leftDirection;
                    }
                    else if (field.ContainsKey(rightPosition) && field[rightPosition] != '.')
                    {
                        turn = 'R';
                        direction = rightDirection;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // split into A, B and C (3 is first with A, B and C in it; 10 is max because of max of 20 with included commas)
            string matchedPattern = null;
            Dictionary<char, List<(char turn, int steps)>> parts = null;

            for (int patternWidth = 3; patternWidth <= 10; patternWidth++)
            {
                var patterns = GeneratePatterns(patternWidth);

                foreach (var pattern in patterns)
                {
                    parts = Split(route, pattern);

                    if (parts != null)
                    {
                        matchedPattern = pattern;
                        break;
                    }
                }

                if (parts != null)
                {
                    break;
                }
            }

            // run program
            var inputs = new List<BigInteger>();

            inputs.AddRange(string.Join(",", matchedPattern.ToList()).Select(c => (BigInteger)c));
            inputs.Add(10);

            foreach (var kv in parts)
            {
                inputs.AddRange(RouteToString(kv.Value).Select(c => (BigInteger)c));
                inputs.Add(10);
            }

            inputs.Add('n');
            inputs.Add(10);

            //foreach (var x in inputs)
            //{
            //    Console.Write((char)x);
            //}

            computer = new Computer
            {
                Data = new Dictionary<int, BigInteger>(data),
                Inputs = inputs
            };

            computer.Data[0] = 2;

            BigInteger dust = 0;

            try
            {
                while (true)
                {
                    var next = computer.NextOutput();

                    if (next >= 128)
                    {
                        dust = next;
                        break;
                    }

                    //Console.Write((char)next);
                }
            }
            catch
            {
            }

            var answer2 = dust;

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
