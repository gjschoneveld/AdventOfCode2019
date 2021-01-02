using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Day24
{
    class Program
    {
        class FieldComparer : IEqualityComparer<bool[,]>
        {
            public bool Equals([AllowNull] bool[,] x, [AllowNull] bool[,] y)
            {
                return x.Cast<bool>().SequenceEqual(y.Cast<bool>());
            }

            public int GetHashCode([DisallowNull] bool[,] obj)
            {
                return BiodiversityRating(obj);
            }
        }

        static int BiodiversityRating(bool[,] field)
        {
            var result = 0;
            var power = 1;

            for (int y = 0; y < field.GetLength(1); y++)
            {
                for (int x = 0; x < field.GetLength(0); x++)
                {
                    if (field[x, y])
                    {
                        result += power;
                    }

                    power *= 2;
                }
            }

            return result;
        }

        static bool SafeGet(bool[,] field, int x, int y)
        {
            if (0 <= x && x < field.GetLength(0) && 0 <= y && y < field.GetLength(1))
            {
                return field[x, y];
            }

            return false;
        }

        static int AliveNeighbours(bool[,] field, int x, int y)
        {
            var neighbours = new List<(int x, int y)>
            {
                (x, y - 1),
                (x, y + 1),
                (x - 1, y),
                (x + 1, y)
            };

            return neighbours.Count(nb => SafeGet(field, nb.x, nb.y));
        }

        static int Part1(bool[,] field)
        {
            var seen = new HashSet<bool[,]>(new FieldComparer());

            while (!seen.Contains(field))
            {
                seen.Add(field);

                var next = (bool[,])field.Clone();

                for (int x = 0; x < field.GetLength(0); x++)
                {
                    for (int y = 0; y < field.GetLength(1); y++)
                    {
                        var isAlive = field[x, y];
                        var aliveNeighbours = AliveNeighbours(field, x, y);

                        if (isAlive && aliveNeighbours != 1)
                        {
                            next[x, y] = false;
                        }
                        else if (!isAlive && (aliveNeighbours == 1 || aliveNeighbours == 2))
                        {
                            next[x, y] = true;
                        }
                    }
                }

                field = next;
            }

            return BiodiversityRating(field);
        }

        static List<(int depth, int x, int y)> GetNeighbours((int depth, int x, int y) position)
        {
            var result = new List<(int depth, int x, int y)>();

            // top
            if (position.y == 0)
            {
                result.Add((position.depth - 1, 2, 1));
            }
            else if (position.x == 2 && position.y == 3)
            {
                for (int x = 0; x < 5; x++)
                {
                    result.Add((position.depth + 1, x, 4));
                }
            }
            else
            {
                result.Add((position.depth, position.x, position.y - 1));
            }

            // bottom
            if (position.y == 4)
            {
                result.Add((position.depth - 1, 2, 3));
            }
            else if (position.x == 2 && position.y == 1)
            {
                for (int x = 0; x < 5; x++)
                {
                    result.Add((position.depth + 1, x, 0));
                }
            }
            else
            {
                result.Add((position.depth, position.x, position.y + 1));
            }

            // left
            if (position.x == 0)
            {
                result.Add((position.depth - 1, 1, 2));
            }
            else if (position.x == 3 && position.y == 2)
            {
                for (int y = 0; y < 5; y++)
                {
                    result.Add((position.depth + 1, 4, y));
                }
            }
            else
            {
                result.Add((position.depth, position.x - 1, position.y));
            }

            // right
            if (position.x == 4)
            {
                result.Add((position.depth - 1, 3, 2));
            }
            else if (position.x == 1 && position.y == 2)
            {
                for (int y = 0; y < 5; y++)
                {
                    result.Add((position.depth + 1, 0, y));
                }
            }
            else
            {
                result.Add((position.depth, position.x + 1, position.y));
            }

            return result;
        }

        static int Part2(bool[,] start)
        {
            var field = new HashSet<(int depth, int x, int y)>();

            for (int x = 0; x < start.GetLength(0); x++)
            {
                for (int y = 0; y < start.GetLength(1); y++)
                {
                    if (start[x, y])
                    {
                        field.Add((0, x, y));
                    }
                }
            }

            var size = start.GetLength(0);

            var minutes = 200;

            for (int minute = 1; minute <= minutes; minute++)
            {
                var flipped = new List<(int depth, int x, int y)>();

                var minDepth = field.Min(x => x.depth) - 1;
                var maxDepth = field.Max(x => x.depth) + 1;

                for (int depth = minDepth; depth <= maxDepth; depth++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        for (int y = 0; y < size; y++)
                        {
                            if (x == 2 && y == 2)
                            {
                                continue;
                            }

                            var position = (depth, x, y);
                            var neighbours = GetNeighbours(position);

                            var isAlive = field.Contains(position);
                            var aliveNeighbours = neighbours.Count(nb => field.Contains(nb));

                            if (isAlive && aliveNeighbours != 1)
                            {
                                flipped.Add(position);
                            }
                            else if (!isAlive && (aliveNeighbours == 1 || aliveNeighbours == 2))
                            {
                                flipped.Add(position);
                            }
                        }
                    }
                }

                foreach (var position in flipped)
                {
                    if (field.Contains(position))
                    {
                        field.Remove(position);
                    }
                    else
                    {
                        field.Add(position);
                    }
                }
            }

            return field.Count;
        }

        static void Main()
        {
            var input = File.ReadAllLines("input.txt");

            var field = new bool[input[0].Length, input.Length];

            for (int x = 0; x < field.GetLength(0); x++)
            {
                for (int y = 0; y < field.GetLength(1); y++)
                {
                    field[x, y] = input[y][x] == '#'; 
                }
            }

            var answer1 = Part1(field);

            Console.WriteLine($"Answer 1: {answer1}");


            var answer2 = Part2(field);

            Console.WriteLine($"Answer 2: {answer2}");
        }
    }
}
