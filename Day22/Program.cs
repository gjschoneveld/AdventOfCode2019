using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Day22
{
    class Formula
    {
        public BigInteger A { get; set; }
        public BigInteger B { get; set; }

        public BigInteger Size { get; set; }

        public BigInteger Evaluate(BigInteger x)
        {
            return Modulo(A * x + B);
        }

        public Formula Simplify()
        {
            return new Formula
            {
                A = Modulo(A),
                B = Modulo(B),
                Size = Size
            };
        }

        public Formula Combine(Formula formula)
        {
            return new Formula
            {
                A = A * formula.A,
                B = A * formula.B + B,
                Size = Size
            }.Simplify();
        }

        public Formula Invert()
        {
            var inverseA = Inverse(A, Size);

            return new Formula
            {
                A = inverseA,
                B = inverseA * -B,
                Size = Size
            }.Simplify();
        }

        private BigInteger Modulo(BigInteger value)
        {
            return ((value % Size) + Size) % Size;
        }

        // from https://en.wikipedia.org/wiki/Extended_Euclidean_algorithm
        private BigInteger Inverse(BigInteger a, BigInteger n)
        {
            BigInteger t = 0;
            BigInteger newt = 1;

            var r = n;
            var newr = a;

            while (newr != 0)
            {
                var quotient = r / newr;
                (t, newt) = (newt, t - quotient * newt);
                (r, newr) = (newr, r - quotient * newr);
            }

            if (r > 1)
            {
                throw new Exception("a is not invertible");
            }

            if (t < 0)
            {
                t += n;
            }

            return t;
        }
    }

    abstract class Rule
    {
        public abstract Formula Apply(Formula formula);

        public static Rule Parse(string x)
        {
            var parts = x.Split(' ');

            if (parts[0] == "cut")
            {
                return new Cut { Value = int.Parse(parts[1]) };
            }

            if (parts[2] == "new")
            {
                return new New();
            }

            return new Increment { Value = int.Parse(parts[3]) };
        }
    }

    class New : Rule
    {
        public override Formula Apply(Formula formula)
        {
            return new Formula
            {
                A = -formula.A,
                B = -formula.B - 1,
                Size = formula.Size
            }.Simplify();
        }
    }

    class Cut : Rule
    {
        public int Value { get; set; }

        public override Formula Apply(Formula formula)
        {
            return new Formula
            {
                A = formula.A,
                B = formula.B - Value,
                Size = formula.Size
            }.Simplify();
        }
    }

    class Increment : Rule
    {
        public int Value { get; set; }

        public override Formula Apply(Formula formula)
        {
            return new Formula
            {
                A = Value * formula.A,
                B = Value * formula.B,
                Size = formula.Size
            }.Simplify();
        }
    }

    class Program
    {
        static void Main()
        {
            var input = File.ReadAllLines("input.txt");
            var rules = input.Select(Rule.Parse).ToList();

            var size = 10007L;

            var formula = new Formula { A = 1, Size = size };

            foreach (var rule in rules)
            {
                formula = rule.Apply(formula);
            }

            var answer1 = formula.Evaluate(2019);

            Console.WriteLine($"Answer1: {answer1}");


            size = 119315717514047L;

            formula = new Formula { A = 1, Size = size };

            foreach (var rule in rules)
            {
                formula = rule.Apply(formula);
            }

            var combined = new Formula { A = 1, Size = size };

            var rounds = 101741582076661L;

            while (rounds > 0)
            {
                if ((rounds & 1) == 1)
                {
                    combined = formula.Combine(combined);
                }

                formula = formula.Combine(formula);
                rounds >>= 1;
            }

            var answer2 = combined.Invert().Evaluate(2020);

            Console.WriteLine($"Answer2: {answer2}");
        }
    }
}
