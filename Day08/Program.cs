using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day08
{
    class Program
    {
        static int CountDigits(int[,] layer, int digit)
        {
            int result = 0;

            for (int i = 0; i < layer.GetLength(0); i++)
            {
                for (int j = 0; j < layer.GetLength(1); j++)
                {
                    if (layer[i,j] == digit)
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        static void Main()
        {
            var input = File.ReadAllText("input.txt");

            const int width = 25;
            const int height = 6;

            var layers = new List<int[,]>();
            int[,] currentLayer = null;

            var x = 0;
            var y = 0;

            foreach (var digit in input)
            {
                if (x == 0 && y == 0)
                {
                    layers.Add(new int[height, width]);
                    currentLayer = layers.Last();
                }

                currentLayer[y, x] = digit - '0';

                x++;

                if (x == width)
                {
                    x = 0;
                    y++;
                }

                if (y == height)
                {
                    y = 0;
                }
            }

            var minimumZeros = layers.Min(l => CountDigits(l, 0));
            var minimumLayer = layers.First(l => CountDigits(l, 0) == minimumZeros);

            var answer1 = CountDigits(minimumLayer, 1) * CountDigits(minimumLayer, 2);

            Console.WriteLine($"Answer 1: {answer1}");

            var image = new int[height, width];

            const int transparent = 2;

            for (y = 0; y < height; y++)
            {
                for (x = 0; x < width; x++)
                {
                    foreach (var layer in layers)
                    {
                        if (layer[y, x] == transparent)
                        {
                            continue;
                        }

                        image[y, x] = layer[y, x];
                        break;
                    }
                }
            }

            Console.WriteLine("Answer 2:");

            const int white = 1;

            for (y = 0; y < height; y++)
            {
                for (x = 0; x < width; x++)
                {
                    if (image[y, x] == white)
                    {
                        Console.Write("█");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }

                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
