using System;
using System.Collections.Generic;
using System.Linq;

namespace FifthMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = true;
            var xValues = new List<double> {-2, 1, 2};
            var yValues = new List<double> {-3, 0, -2};
            if (!xValues.Any() && !yValues.Any() || !test)
            {
                Console.WriteLine("Введите знчения x через пробел в одну строку");
                xValues = Console.ReadLine().Split(' ').Select(x => double.Parse(x)).ToList();
                Console.WriteLine("Введите знчения y через пробел в одну строку");
                yValues = Console.ReadLine().Split(' ').Select(x => double.Parse(x)).ToList();
            }
            Lagrange(xValues, yValues);
            Newton(xValues, yValues);
        }

        private static void Lagrange(List<double> xValues, List<double> yValues)
        {
            var firstL = new List<double>
            {
                xValues[1] * xValues[2],
                -xValues[2] - xValues[1],
                1
            };
            firstL = firstL.Select(x => x / ((xValues[0] - xValues[2]) * (xValues[0] - xValues[1])) * yValues[0])
                .ToList();
            var secondL = new List<double>
            {
                xValues[0] * xValues[2],
                -xValues[2] - xValues[0],
                1
            };
            secondL = secondL.Select(x => x / ((xValues[1] - xValues[0]) * (xValues[1] - xValues[2])) * yValues[1])
                .ToList();
            var thirdL = new List<double>
            {
                xValues[0] * xValues[1],
                -xValues[1] - xValues[0],
                1
            };
            thirdL = thirdL.Select(x => x / ((xValues[2] - xValues[0]) * (xValues[2] - xValues[1])) * yValues[2])
                .ToList();
            var coefList = new List<double>();

            for (int i = 0; i < 3; i++)
            {
                coefList.Add(firstL[i] + secondL[i] + thirdL[i]);
            }

            Console.WriteLine($"Lagrange:\n{coefList[2]}X^2 + {coefList[1]}X + {coefList[0]}");
        }

        private static void Newton(List<double> xValues, List<double> yValues)
        {
            var dict = new Dictionary<double, double>();
            for (int i = 0; i < xValues.Count; i++)
                dict.Add(xValues[i], yValues[i]);
            Console.WriteLine(
                $"Newton:\n{DeltaTwo(xValues[0], xValues[1], xValues[2])}X^2 + " +
                $"{DeltaOne(xValues[0], xValues[1]) - DeltaTwo(xValues[0], xValues[1], xValues[2]) * (xValues[0] + xValues[1])}X + " +
                $"{yValues[0] + DeltaTwo(xValues[0], xValues[1], xValues[2])  * xValues[0] * xValues[1] - DeltaOne(xValues[0], xValues[1]) * xValues[0]}");

            double DeltaOne(double x1, double x2)
            {
                return (dict[x1] - dict[x2]) / (x1 - x2);
            }

            double DeltaTwo(double x1, double x2, double x3)
            {
                return (DeltaOne(x1, x2) - DeltaOne(x2, x3)) / (x1 - x3);
            }
        }
    }
}