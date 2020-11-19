using System;
using System.Collections.Generic;
using System.Linq;

namespace SeventhMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = true;
            var xValues = new List<double> {0, 2, 4, 6};
            var yValues = new List<double> {2, -4, -10, -16};
            if (!xValues.Any() && !yValues.Any() || !test)
            {
                Console.WriteLine("Введите знчения x через пробел в одну строку");
                xValues = Console.ReadLine().Split(' ').Select(x => double.Parse(x)).ToList();
                Console.WriteLine("Введите знчения y через пробел в одну строку");
                yValues = Console.ReadLine().Split(' ').Select(x => double.Parse(x)).ToList();
            }

            var sumOfMultiplying = xValues.Select((t, i) => t * yValues[i]).Sum();
            var sumOfXSqr = xValues.Select(x => x * x).Sum();
            var sumOfX = xValues.Sum();
            var sumOfY = yValues.Sum();

            var b = (sumOfMultiplying - (sumOfXSqr * sumOfY / sumOfX)) / (sumOfX - xValues.Count * sumOfXSqr / sumOfX);
            var a = (sumOfY - b * xValues.Count) / sumOfX;
            Console.WriteLine($"Ответ:\ny = {a}x + {b:F3}");
        }
    }
}