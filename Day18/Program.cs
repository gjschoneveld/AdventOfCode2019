using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day18
{
    class Program
    {
        static char CharAt((int x, int y) position)
        {
            return field[position.y][position.x];
        }

        static Dictionary<char, (int x, int y)> FindPositions()
        {
            var result = new Dictionary<char, (int x, int y)>();

            for (int y = 0; y < field.Length; y++)
            {
                for (int x = 0; x < field[y].Length; x++)
                {
                    var value = CharAt((x, y));

                    if (value != '#' && value != '.' && !char.IsUpper(value))
                    {
                        result.Add(value, (x, y));
                    }
                }
            }

            return result;
        }

        static List<(int x, int y)> NeighBours((int x, int y) position)
        {
            return new List<(int x, int y)>
            {
                (position.x, position.y - 1),
                (position.x, position.y + 1),
                (position.x - 1, position.y),
                (position.x + 1, position.y)
            }; 
        }

        static Dictionary<char, List<char>> DoorsNeeded(HashSet<(int x, int y)> visited, List<char> doors, (int x, int y) position)
        {
            var result = new Dictionary<char, List<char>>();

            var value = CharAt(position);

            if (value == '#' || visited.Contains(position))
            {
                return result;
            }

            visited.Add(position);

            if ('A' <= value && value <= 'Z')
            {
                // a door
                doors = new List<char>(doors);
                doors.Add(value);
            }
            else if ('a' <= value && value <= 'z')
            {
                // a key
                result.Add(value, doors);
            }

            // visit neighbours
            foreach (var nb in NeighBours(position))
            {
                var inner = DoorsNeeded(visited, doors, nb);

                foreach (var kv in inner)
                {
                    result.Add(kv.Key, kv.Value);
                }
            }

            return result;
        }

        static int? Steps(HashSet<(int x, int y)> visited, (int x, int y) from, (int x, int y) to)
        {
            if (from == to)
            {
                return 0;
            }

            var value = CharAt(from);

            if (value == '#' || visited.Contains(from))
            {
                return null;
            }

            visited.Add(from);

            var results = new List<int>();

            // visit neighbours
            foreach (var nb in NeighBours(from))
            {
                var inner = Steps(visited, nb, to);

                if (inner != null)
                {
                    results.Add(inner.Value + 1);
                }
            }

            visited.Remove(from);

            if (results.Count > 0)
            {
                return results.Min();
            }

            return null;
        }

        class CacheKey
        {
            public List<char> values;
            public List<char> keysToFind;

            public override int GetHashCode()
            {
                return values.Sum(c => c) + keysToFind.Sum(c => c);
            }

            public override bool Equals(object obj)
            {
                if (!(obj is CacheKey other))
                {
                    return false;
                }

                return values.SequenceEqual(other.values) && keysToFind.SequenceEqual(other.keysToFind);
            }
        }

        static Dictionary<CacheKey, int> cache = new Dictionary<CacheKey, int>();

        static void ClearClache()
        {
            cache.Clear();
        }

        static void PutInCache(List<char> values, List<char> keysToFind, int steps)
        {
            var key = new CacheKey
            {
                values = values,
                keysToFind = keysToFind
            };

            cache[key] = steps;
        }

        static int? GetFromCache(List<char> values, List<char> keysToFind)
        {
            var key = new CacheKey
            {
                values = values,
                keysToFind = keysToFind
            };

            if (cache.ContainsKey(key))
            {
                return cache[key];
            }

            return null;
        }

        static List<char> AllKeys()
        {
            return doorsNeeded.Keys.OrderBy(k => k).ToList();
        }

        static List<char> ReachableKeys(List<char> keysToFind)
        {
            var opened = AllKeys().Except(keysToFind).Select(c => char.ToUpper(c)).ToList();

            var reachableKeys = keysToFind.Where(k => doorsNeeded[k].All(d => opened.Contains(d))).ToList();

            return reachableKeys;
        }

        static int StepsNeeded(char value, List<char> keysToFind)
        {
            if (keysToFind.Count == 0)
            {
                return 0;
            }

            var cached = GetFromCache(new List<char> { value }, keysToFind);

            if (cached != null)
            {
                return cached.Value;
            }

            var results = new List<int>();

            var reachableKeys = ReachableKeys(keysToFind);

            foreach (var key in reachableKeys)
            {
                var innerKeys = new List<char>(keysToFind);
                innerKeys.Remove(key);

                var innerSteps = StepsNeeded(key, innerKeys);

                var total = steps[(value, key)] + innerSteps;

                results.Add(total);
            }

            var result = results.Min();

            PutInCache(new List<char> { value }, keysToFind, result);

            return result;
        }

        static int StepsNeeded2(List<char> values, List<char> keysToFind)
        {
            if (keysToFind.Count == 0)
            {
                return 0;
            }

            var cached = GetFromCache(values, keysToFind);

            if (cached != null)
            {
                return cached.Value;
            }

            var results = new List<int>();

            var reachableKeys = ReachableKeys(keysToFind);

            foreach (var key in reachableKeys)
            {
                var quadrant = quadrants[key];

                var innerValues = new List<char>(values);
                innerValues[quadrant] = key;

                var innerKeys = new List<char>(keysToFind);
                innerKeys.Remove(key);

                var innerSteps = StepsNeeded2(innerValues, innerKeys);

                var total = steps2[(values[quadrant], key)] + innerSteps;

                results.Add(total);
            }

            var result = results.Min();

            PutInCache(values, keysToFind, result);

            return result;
        }

        static int GetQuadrant((int x, int y) position)
        {
            var centerX = field[0].Length / 2;
            var centerY = field.Length / 2;

            var result = 0;

            if (position.x > centerX)
            {
                result += 1;
            }

            if (position.y > centerY)
            {
                result += 2;
            }

            return result;
        }

        static string[] field;

        static Dictionary<char, List<char>> doorsNeeded;

        static Dictionary<(char from, char to), int> steps;
        static Dictionary<(char from, char to), int> steps2;

        static Dictionary<char, int> quadrants;

        static void Main(string[] args)
        {
            field = File.ReadAllLines("input.txt");

            var positions = FindPositions();

            doorsNeeded = DoorsNeeded(new HashSet<(int x, int y)>(), new List<char>(), positions['@']);

            steps = new Dictionary<(char from, char to), int>();

            foreach (var from in positions.Keys)
            {
                foreach (var to in positions.Keys)
                {
                    steps[(from, to)] = Steps(new HashSet<(int x, int y)>(), positions[from], positions[to]).Value;
                }
            }

            var answer1 = StepsNeeded('@', AllKeys());

            Console.WriteLine($"Answer 1: {answer1}");


            // @ are two closer because of change in input; only from @ is being used
            steps2 = steps.ToDictionary(s => s.Key, s => s.Key.from == '@' ? s.Value - 2 : s.Value);

            quadrants = new Dictionary<char, int>();

            foreach (var key in doorsNeeded.Keys)
            {
                quadrants[key] = GetQuadrant(positions[key]);
            }

            ClearClache();

            var answer2 = StepsNeeded2(new List<char> { '@', '@', '@', '@' }, AllKeys());

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
