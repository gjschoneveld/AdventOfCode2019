using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day20
{
    class Program
    {
        static char GetValue((int x, int y) position)
        {
            if (position.y < 0 || position.x < 0 || position.y >= input.Length || position.x >= input[position.y].Length)
            {
                return ' ';
            }

            return input[position.y][position.x];
        }

        static string GetLabel((int x, int y) position)
        {
            var value = GetValue(position);

            if (value != '.')
            {
                return null;
            }

            var north = GetValue((position.x, position.y - 1));
            var south = GetValue((position.x, position.y + 1));
            var west = GetValue((position.x - 1, position.y));
            var east = GetValue((position.x + 1, position.y));

            if (char.IsLetter(north))
            {
                var other = GetValue((position.x, position.y - 2));

                return $"{other}{north}";
            }
            else if (char.IsLetter(south))
            {
                var other = GetValue((position.x, position.y + 2));

                return $"{south}{other}";
            }
            else if (char.IsLetter(west))
            {
                var other = GetValue((position.x - 2, position.y));

                return $"{other}{west}";
            }
            else if (char.IsLetter(east))
            {
                var other = GetValue((position.x + 2, position.y));

                return $"{east}{other}";
            }

            return null;
        }

        static (int x, int y) Find(string label)
        {
            for (int y = 0; y < input.Length; y++)
            {
                for (int x = 0; x < input[y].Length; x++)
                {
                    if (GetLabel((x, y)) == label)
                    {
                        return (x, y);
                    }
                }
            }

            throw new Exception("Not found");
        }

        static Dictionary<(int x, int y), (int x, int y)> FindLinks()
        {
            var result = new Dictionary<(int x, int y), (int x, int y)>();

            var locations = new Dictionary<string, (int x, int y)>();

            for (int y = 0; y < input.Length; y++)
            {
                for (int x = 0; x < input[y].Length; x++)
                {
                    var label = GetLabel((x, y));

                    if (label == null)
                    {
                        continue;
                    }

                    if (locations.ContainsKey(label))
                    {
                        result.Add((x, y), locations[label]);
                        result.Add(locations[label], (x, y));
                    }
                    else
                    {
                        locations.Add(label, (x, y));
                    }
                }
            }

            return result;
        }

        static List<(int x, int y)> Neighbours((int x, int y) position)
        {
            return new List<(int x, int y)>
            {
                (x: position.x, y: position.y - 1),
                (x: position.x, y: position.y + 1),
                (x: position.x - 1, y: position.y),
                (x: position.x + 1, y: position.y)
            };
        }
 
        static int? Steps(HashSet<(int x, int y)> visited, (int x, int y) start, (int x, int y) end)
        {
            if (start == end)
            {
                return 0;
            }

            if (visited.Contains(start))
            {
                return null;
            }

            visited.Add(start);

            var neighbours = Neighbours(start).Where(nb => GetValue(nb) == '.').ToList();

            if (links.ContainsKey(start))
            {
                neighbours.Add(links[start]);
            }

            var results = new List<int>();

            foreach (var nb in neighbours)
            {
                var inner = Steps(visited, nb, end);

                if (inner != null)
                {
                    results.Add(inner.Value + 1);
                }
            }

            visited.Remove(start);

            if (results.Count == 0)
            {
                return null;
            }

            return results.Min();
        }

        static string[] input;

        static Dictionary<(int x, int y), (int x, int y)> links;

        static void Main(string[] args)
        {
            input = File.ReadAllLines("input.txt");

            var start = Find("AA");
            var end = Find("ZZ");

            links = FindLinks();

            var answer1 = Steps(new HashSet<(int x, int y)>(), start, end);

            Console.WriteLine($"Answer 1: {answer1}");


            var minX = links.Min(kv => kv.Key.x);
            var maxX = links.Max(kv => kv.Key.x);
            var minY = links.Min(kv => kv.Key.y);
            var maxY = links.Max(kv => kv.Key.y);

            var outerBorder = new HashSet<(int x, int y)>();

            foreach (var kv in links)
            {
                if (kv.Key.x == minX || kv.Key.x == maxX)
                {
                    outerBorder.Add(kv.Key);
                }

                if (kv.Key.y == minY || kv.Key.y == maxY)
                {
                    outerBorder.Add(kv.Key);
                }
            }

            var visited = new HashSet<(int level, (int x, int y) position)>();

            var current = new List<(int level, (int x, int y) position)> { (0, start) };

            var steps = 0;

            while (!current.Contains((0, end)))
            {
                foreach (var c in current)
                {
                    visited.Add(c);
                }

                var next = new List<(int level, (int x, int y) position)>();

                foreach (var c in current)
                {
                    var neighbours = Neighbours(c.position).Where(nb => GetValue(nb) == '.').ToList();
                    var neighboursWithLevel = neighbours.Select(nb => (c.level, nb)).ToList();

                    if (links.ContainsKey(c.position))
                    {
                        if (outerBorder.Contains(c.position))
                        {
                            // go up one level
                            if (c.level > 0)
                            {
                                neighboursWithLevel.Add((c.level - 1, links[c.position]));
                            }
                        }
                        else
                        {
                            // go down one level
                            neighboursWithLevel.Add((c.level + 1, links[c.position]));
                        }
                    }

                    next.AddRange(neighboursWithLevel.Where(n => !visited.Contains(n)));
                }

                current = next.Distinct().ToList();

                steps++;
            }

            var answer2 = steps;

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
