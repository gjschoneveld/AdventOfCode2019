using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day12
{
    class State : IEquatable<State>
    {
        public List<(int p, int v)> Data { get; set; }

        public override int GetHashCode()
        {
            return Data.Sum(d => d.p * d.v);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is State other))
            {
                return false;
            }

            return Equals(other);
        }

        public bool Equals(State other)
        {
            return Data.SequenceEqual(other.Data);
        }

        public static State GetState(List<Moon> moons, int dimension)
        {
            var data = moons.Select(m => (p: m.Position[dimension], v: m.Velocity[dimension])).ToList();

            return new State { Data = data };
        }
    }

    class Moon
    {
        public List<int> Position { get; set; }
        public List<int> Velocity { get; set; } = new List<int> { 0, 0, 0 };

        public void UpdateVelocity(List<Moon> moons, int dimension)
        {
            foreach (var moon in moons)
            {
                if (moon == this)
                {
                    continue;
                }

                if (moon.Position[dimension] > Position[dimension])
                {
                    Velocity[dimension]++;
                }
                else if (moon.Position[dimension] < Position[dimension])
                {
                    Velocity[dimension]--;
                }
            }
        }

        public void UpdatePosition(int dimension)
        {

            Position[dimension] += Velocity[dimension];
        }

        public int Energy()
        {
            return Position.Sum(p => Math.Abs(p)) * Velocity.Sum(v => Math.Abs(v)); 
        }


        public static Moon Parse(string x)
        {
            var parts = x.Split(new[] { '<', 'x', 'y', 'z', '=', ',', ' ', '>' }, StringSplitOptions.RemoveEmptyEntries);

            var position = parts.Select(int.Parse).ToList();

            return new Moon
            {
                Position = position
            };
        }
    }

    class Program
    {
        static long Gcd(long a, long b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                {
                    a %= b;
                }
                else
                {
                    b %= a;
                }
            }

            return a == 0 ? b : a;
        }

        static long Lcm(long a, long b)
        {
            return a * b / Gcd(a, b);
        }

        static void Step(List<Moon> moons, int dimension)
        {
            foreach (var moon in moons)
            {
                moon.UpdateVelocity(moons, dimension);
            }

            foreach (var moon in moons)
            {
                moon.UpdatePosition(dimension);
            }
        }

        static void Step(List<Moon> moons)
        {
            for (int dimension = 0; dimension < 3; dimension++)
            {
                Step(moons, dimension);
            }
        }

        static void Main()
        {
            var input = File.ReadAllLines("input.txt");
            var start = input.Select(Moon.Parse).ToList();

            var moons = new List<Moon>(start);

            long steps = 1000;

            for (int i = 0; i < steps; i++)
            {
                Step(moons);
            }

            var answer1 = moons.Sum(m => m.Energy());

            Console.WriteLine($"Answer 1: {answer1}");


            moons = new List<Moon>(start);

            var periods = new List<long> { 0, 0, 0 };

            for (int dimension = 0; dimension < periods.Count; dimension++)
            {
                var seen = new HashSet<State>();

                steps = 0;

                var state = State.GetState(moons, dimension);

                while (!seen.Contains(state))
                {
                    seen.Add(state);

                    Step(moons, dimension);
                    steps++;

                    state = State.GetState(moons, dimension);
                }

                periods[dimension] = steps;
            }

            var answer2 = periods.Aggregate((a, b) => Lcm(a, b));

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
