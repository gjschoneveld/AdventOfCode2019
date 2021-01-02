using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day06
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");

            var orbits = new Dictionary<string, List<string>>();
            var neighbours = new Dictionary<string, List<string>>();

            foreach (var line in input)
            {
                var parts = line.Split(')');

                var main = parts[0];
                var satellite = parts[1];

                if (!orbits.ContainsKey(main))
                {
                    orbits.Add(main, new List<string>());
                }

                orbits[main].Add(satellite);

                if (!neighbours.ContainsKey(main))
                {
                    neighbours.Add(main, new List<string>());
                }

                neighbours[main].Add(satellite);

                if (!neighbours.ContainsKey(satellite))
                {
                    neighbours.Add(satellite, new List<string>());
                }

                neighbours[satellite].Add(main);
            }

            var current = new List<string> { "COM" };

            var distance = 0;
            var total = 0;

            while (current.Count > 0)
            {
                total += current.Count * distance;

                current = current.SelectMany(x => orbits.ContainsKey(x) ? orbits[x] : new List<string>()).ToList();

                distance++;
            }

            var answer1 = total;

            Console.WriteLine($"Answer 1: {answer1}");


            var visited = new List<string>();
            current = new List<string> { "YOU" };

            distance = 0;

            while (!current.Contains("SAN"))
            {
                visited.AddRange(current);

                current = current.SelectMany(x => neighbours.ContainsKey(x) ? neighbours[x].Where(n => !visited.Contains(n)) : new List<string>()).ToList();

                distance++;
            }

            var answer2 = distance - 2;

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
