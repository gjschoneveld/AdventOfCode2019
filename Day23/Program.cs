using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Day23
{
    class Computer
    {
        private int programCounter;
        private int relativeBase;

        public Dictionary<int, BigInteger> Data { get; set; }
        public IEnumerator<BigInteger?> Input { get; set; }

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

        public BigInteger? NextOutput()
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
                        Input.MoveNext();
                        SetValue(programCounter + 1, mode1, Input.Current ?? -1);
                        programCounter += 2;

                        if (Input.Current == null)
                        {
                            return null;
                        }

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
        static IEnumerable<BigInteger?> InputGenerator(int address, Queue<(BigInteger x, BigInteger y)> queue)
        {
            yield return address;

            while (true)
            {
                if (queue.Count == 0)
                {
                    yield return null;
                    continue;
                }

                var (x, y) = queue.Dequeue();

                yield return x;
                yield return y;
            }
        }

        static BigInteger Simulate(List<BigInteger> program, bool useNat)
        {
            var addresses = 50;

            var queues = new List<Queue<(BigInteger x, BigInteger y)>>();
            var computers = new List<Computer>();

            for (int address = 0; address < addresses; address++)
            {
                var data = new Dictionary<int, BigInteger>();

                for (int i = 0; i < program.Count; i++)
                {
                    data[i] = program[i];
                }

                queues.Add(new Queue<(BigInteger x, BigInteger y)>());

                computers.Add(new Computer
                {
                    Data = new Dictionary<int, BigInteger>(data),
                    Input = InputGenerator(address, queues[address]).GetEnumerator()
                });
            }

            (BigInteger x, BigInteger y) nat = (0, 0);

            var seen = new HashSet<BigInteger>();

            while (true)
            {
                var idle = true;

                foreach (var computer in computers)
                {
                    var address = computer.NextOutput();

                    if (address == null)
                    {
                        // computer has no input, move to next
                        continue;
                    }

                    var x = computer.NextOutput().Value;
                    var y = computer.NextOutput().Value;

                    idle = false;

                    if (address != 255)
                    {
                        queues[(int)address].Enqueue((x, y));
                        continue;
                    }

                    if (!useNat)
                    {
                        return y;
                    }

                    nat = (x, y);
                }

                if (idle)
                {
                    if (seen.Contains(nat.y))
                    {
                        return nat.y;
                    }

                    seen.Add(nat.y);

                    queues[0].Enqueue(nat);
                }
            }
        }

        static void Main()
        {
            var input = File.ReadAllText("input.txt");
            var program = input.Split(',').Select(BigInteger.Parse).ToList();

            var answer1 = Simulate(program, false);

            Console.WriteLine($"Answer 1: {answer1}");

            var answer2 = Simulate(program, true);

            Console.WriteLine($"Answer 2: {answer2}");
        }
    }
}
