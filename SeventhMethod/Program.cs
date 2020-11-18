using System;
using System.Linq;

namespace SeventhMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите знчения x через пробел в одну строку");
            var xValues = Console.ReadLine().Split(' ').Select(x => double.Parse(x)).ToList();
            Console.WriteLine("Введите знчения y через пробел в одну строку");
            var yValues = Console.ReadLine().Split(' ').Select(x => double.Parse(x)).ToList();

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