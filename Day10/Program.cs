using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day10
{
    class Program
    {
        static int Gcd(int a, int b)
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

        static List<(int x, int y)> CoordinatesInBetween((int x, int y) a, (int x, int y) b)
        {
            var result = new List<(int x, int y)>();

            var deltaX = b.x - a.x;
            var deltaY = b.y - a.y;

            int steps;

            if (deltaX == 0)
            {
                steps = Math.Abs(deltaY);
            }
            else if (deltaY == 0)
            {
                steps = Math.Abs(deltaX);
            }
            else
            {
                steps = Gcd(Math.Abs(deltaX), Math.Abs(deltaY));
            }

            var stepX = deltaX / steps;
            var stepY = deltaY / steps;

            for (int i = 1; i < steps; i++)
            {
                var x = a.x + i * stepX;
                var y = a.y + i * stepY;

                result.Add((x, y));
            }

            return result;
        }

        static double CartesianToAngle(double x, double y)
        {
            double result;

            if (x == 0)
            {
                result = y < 0 ? 0 : Math.PI;
            }
            else if (y == 0)
            {
                result = x > 0 ? Math.PI / 2 : 3 * Math.PI / 2;
            }
            else if (x > 0 && y < 0)
            {
                result = Math.Atan(-x / y);
            }
            else if (x > 0 && y > 0)
            {
                result = Math.PI / 2 + Math.Atan(y / x);
            }
            else if (y > 0)
            {
                result = Math.PI + Math.Atan(-x / y);
            }
            else
            {
                result = 3 * Math.PI / 2 + Math.Atan(y / x);
            }

            return result;
        }

        static void Main()
        {
            var input = File.ReadAllLines("input.txt");

            var asteroids = new HashSet<(int x, int y)>();

            for (int y = 0; y < input.Length; y++)
            {
                for (int x = 0; x < input[y].Length; x++)
                {
                    if (input[y][x] == '#')
                    {
                        asteroids.Add((x, y));
                    }
                }
            }

            var maxVisible = 0;
            var maxLocation = (x: 0, y: 0);

            foreach (var asteroid in asteroids)
            {
                var visible = 0;

                foreach (var other in asteroids)
                {
                    if (other == asteroid)
                    {
                        continue;
                    }

                    var inBetween = CoordinatesInBetween(asteroid, other);

                    if (!inBetween.Any(c => asteroids.Contains(c)))
                    {
                        visible++;
                    }
                }

                if (visible > maxVisible)
                {
                    maxVisible = visible;
                    maxLocation = asteroid;
                }
                maxVisible = Math.Max(maxVisible, visible);
            }

            var answer1 = maxVisible;

            Console.WriteLine($"Answer 1: {answer1}");

            var station = maxLocation;

            var shotInRotation = new Dictionary<int, List<(int x, int y)>>();

            foreach (var other in asteroids)
            {
                if (other == station)
                {
                    continue;
                }

                var inBetween = CoordinatesInBetween(station, other);

                var rotationsNeeded = inBetween.Count(c => asteroids.Contains(c));

                if (!shotInRotation.ContainsKey(rotationsNeeded))
                {
                    shotInRotation[rotationsNeeded] = new List<(int x, int y)>();
                }

                shotInRotation[rotationsNeeded].Add(other);
            }

            var rotation = 0;
            var vaporized = 0;

            var target = 200;

            while (vaporized + shotInRotation[rotation].Count < target)
            {
                vaporized += shotInRotation[rotation].Count;
                rotation++;
            }

            var shotInTargetRotation = shotInRotation[rotation];

            var targetCoordinates = shotInTargetRotation.OrderBy(c => CartesianToAngle(c.x - station.x, c.y - station.y)).Skip(target - vaporized - 1).First();

            var answer2 = targetCoordinates.x * 100 + targetCoordinates.y;

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
