using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day14
{
    class Rule
    {
        public List<(string name, long quantity)> Inputs { get; set; }
        public (string name, long quantity) Output { get; set; }

        public bool Processed { get; set; }

        public static Rule Parse(string x)
        {
            var parts = x.Split(new[] { ' ', ',', '=', '>' }, StringSplitOptions.RemoveEmptyEntries );

            var inputs = new List<(string name, long quantity)>();

            for (long i = 0; i < parts.Length - 2; i += 2)
            {
                inputs.Add((parts[i + 1], long.Parse(parts[i])));
            }

            var output = (parts[parts.Length - 1], long.Parse(parts[parts.Length - 2]));

            return new Rule
            {
                Inputs = inputs,
                Output = output
            };
        }
    }

    class Program
    {
        static long OreNeeded(List<Rule> rules, long fuel)
        {
            var needed = new Dictionary<string, long> { ["FUEL"] = fuel };

            while (needed.Count > 1 || !needed.ContainsKey("ORE"))
            {
                var rule = rules.First(r => needed.ContainsKey(r.Output.name) && !rules.Any(other => !other.Processed && other.Inputs.Any(i => i.name == r.Output.name)));

                var quantity = needed[rule.Output.name];

                var multiplier = quantity / rule.Output.quantity;

                if (quantity % rule.Output.quantity > 0)
                {
                    multiplier++;
                }

                foreach (var i in rule.Inputs)
                {
                    if (!needed.ContainsKey(i.name))
                    {
                        needed[i.name] = 0;
                    }

                    needed[i.name] += multiplier * i.quantity;
                }

                needed.Remove(rule.Output.name);

                rule.Processed = true;
            }

            foreach (var rule in rules)
            {
                rule.Processed = false;
            }

            return needed["ORE"];
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var rules = input.Select(Rule.Parse).ToList();
           
            var answer1 = OreNeeded(rules, 1);

            Console.WriteLine($"Answer 1: {answer1}");


            var maxOre = 1000000000000L;

            var minFuel = 1;
            var maxFuel = 1;

            while (OreNeeded(rules, maxFuel) < maxOre)
            {
                maxFuel *= 2;
            }

            while (maxFuel > minFuel)
            {
                // + 1 to round up
                var center = (maxFuel - minFuel + 1) / 2 + minFuel;

                var needed = OreNeeded(rules, center);

                if (needed == maxOre)
                {
                    minFuel = center;
                    maxFuel = center;
                }
                else if (needed > maxOre)
                {
                    maxFuel = center - 1;
                }
                else
                {
                    minFuel = center;
                }
            }

            var answer2 = maxFuel;

            Console.WriteLine($"Answer 2: {answer2}");

            Console.ReadKey();
        }
    }
}
