using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day03
{
    class Program
    {
        static Dictionary<(int x, int y), int> GetVisitedLocations(string path)
        {
            var result = new Dictionary<(int x, int y), int>();

            (int x, int y) position = (0, 0);
            int steps = 0;

            foreach (var direction in path.Split(','))
            {
                Func<(int x, int y), (int x, int y)> next = null;

                switch (direction[0])
                {
                    case 'L':
                        next = p => (p.x - 1, p.y);
                        break;
                    case 'U':
                        next = p => (p.x, p.y - 1);
                        break;
                    case 'R':
                        next = p => (p.x + 1, p.y);
                        break;
                    case 'D':
                        next = p => (p.x, p.y + 1);
                        break;
                }

                var count = int.Parse(direction.Substring(1));

                for (int i = 0; i < count; i++)
                {
                    position = next(position);
                    steps++;

                    if (!result.ContainsKey(position))
                    {
                        result.Add(position, steps);
                    }
                }
            }

            return result;
        }

        static void Main()
        {
            var paths = File.ReadAllLines("input.txt");

            var locations = paths.Select(GetVisitedLocations).ToList();

            var intersections = locations[0].Keys.Intersect(locations[1].Keys).ToList();

            var nearestIntersection = intersections.Min(p => Math.Abs(p.x) + Math.Abs(p.y));

            var answer1 = nearestIntersection;

            Console.WriteLine($"Answer 1: {answer1}");


            var steps = intersections.Select(p => locations[0][p] + locations[1][p]).ToList();

            var answer2 = steps.Min();

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
