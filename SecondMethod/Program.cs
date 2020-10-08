using System;
using System.Collections.Generic;
using System.Globalization;

namespace SecondMethod
{
    class Program
    {
        enum States
        {
            Start,
            EpsilonInput,
            AllPowerInputs,
            SegmentInputs,
            SelectMethod,
            NewtonMethod,
            SegmentMethod,
            Finished,
            Exit
        }

        static void Main()
        {
            // x^3+0.4*x-1.2 = 0
            var state = States.Start;
            var maxPower = 0;
            var epsilon = 0d;
            var polynomFactors = new List<double>();
            var startSegment = -1d;
            var endSegment = -1d;
            while (true)
            {
                switch (state)
                {
                    case States.Start:
                        Console.WriteLine("Введите ЦЕЛЫЙ порядок могочлена:");
                        polynomFactors = new List<double>();
                        state = TryGetInt(out maxPower) ? States.EpsilonInput : States.Start;
                        break;
                    case States.EpsilonInput:
                        Console.WriteLine("Введите точность рассчёта:");
                        state = TryGetDouble(out epsilon) ? States.AllPowerInputs : States.Start;
                        break;
                    case States.AllPowerInputs:
                        Console.WriteLine($"Введите коэффициент у x^{maxPower - polynomFactors.Count}");
                        if (!TryGetDouble(out var coefficient))
                            break;
                        polynomFactors.Add(coefficient);
                        if (polynomFactors.Count > maxPower)
                        {
                            state = States.SegmentInputs;
                            polynomFactors.Reverse();
                        }

                        break;
                    case States.SegmentInputs:
                        if (!TryGetSegmentValue("левую", out startSegment))
                            break;
                        if (!TryGetSegmentValue("правую", out endSegment))
                            break;
                        if (startSegment > endSegment)
                            break;
                        state = States.SelectMethod;
                        break;
                    case States.SelectMethod:
                        Console.WriteLine("Выберите метод для решения:\n" +
                                          "(1) Метод деления отрезка пополам\n" +
                                          "(2) Метод Ньютона");
                        if (!TryGetInt(out var method))
                            break;
                        state = method switch
                        {
                            1 => States.SegmentMethod,
                            2 => States.NewtonMethod,
                            _ => States.SelectMethod
                        };
                        break;
                    case States.SegmentMethod:
                        if (!TryGetSegmentMethod(polynomFactors, epsilon, startSegment, endSegment,
                            out var segmentResult))
                        {
                            Console.WriteLine("Нет решения в отрезке");
                            state = States.SegmentInputs;
                            break;
                        }

                        Console.WriteLine($"Результат {FormatResult(segmentResult, epsilon)}");
                        state = States.Finished;
                        break;
                    case States.NewtonMethod:
                        if (!TryGetNewtonMethod(polynomFactors, epsilon, startSegment, endSegment,
                            out var newtonResult))
                        {
                            Console.WriteLine("Нет решения в отрезке");
                            state = States.SegmentInputs;
                            break;
                        }

                        Console.WriteLine($"Результат {FormatResult(newtonResult, epsilon)}");
                        state = States.Finished;
                        break;
                    case States.Finished:
                        Console.WriteLine("Что делать дальше:\n" +
                                          "(1) Решить новую функцию\n" +
                                          "(2) Решить ещё одним методом\n" +
                                          "(3) Сменить отрезок\n" +
                                          "(4) Завершить программу");
                        if (!TryGetInt(out var nextCommand))
                            break;
                        state = nextCommand switch
                        {
                            1 => States.Start,
                            2 => States.SelectMethod,
                            3 => States.SegmentInputs,
                            4 => States.Exit,
                            _ => States.SelectMethod
                        };
                        break;
                    case States.Exit:
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static bool TryGetInt(out int value)
        {
            return int.TryParse(Console.ReadLine(), out value);
        }

        private static bool TryGetDouble(out double coefficient)
        {
            coefficient = 0;
            var input = Console.ReadLine();
            if (input == null)
                return false;
            input = input.Contains(',') ? input.Replace(',', '.') : input;
            return double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out coefficient);
        }

        private static bool TryGetSegmentValue(string align, out double coefficient)
        {
            Console.WriteLine($"Введите {align} границу");
            return TryGetDouble(out coefficient);
        }

        private static double CalculateFunc(List<double> coefficients, double xValue)
        {
            var sum = 0d;
            for (int i = 0; i < coefficients.Count; ++i)
                sum += Math.Pow(xValue, i) * coefficients[i];

            return sum;
        }

        private static bool TryGetSegmentMethod(List<double> coefficients, double epsilon, double startFunction,
            double endFunction, out double result)
        {
            result = 0;

            if (!InSegment(coefficients, startFunction, endFunction))
                return false;

            while (Math.Abs(endFunction - startFunction) > epsilon)
            {
                result = (startFunction + endFunction) / 2;
                if (InSegment(coefficients, startFunction, result))
                    endFunction = result;
                else
                    startFunction = result;
            }

            return true;
        }

        private static bool TryGetNewtonMethod(List<double> coefficients, double epsilon, double startFunction,
            double endFunction, out double result)
        {
            return TryCalculateNewton(coefficients, epsilon, startFunction, out result) ||
                   TryCalculateNewton(coefficients, epsilon, endFunction, out result);
        }

        private static bool TryCalculateNewton(List<double> coefficients, double epsilon, double value,
            out double result)
        {
            result = 0;
            if (CalculateFunc(coefficients, value) * Diff(coefficients, Diff(coefficients, value)) > 0)
            {
                result = value;
                while (Math.Abs(CalculateFunc(coefficients, result)) > epsilon)
                    result = result - CalculateFunc(coefficients, result) / Diff(coefficients, result);
                return true;
            }

            return false;
        }

        private static bool InSegment(List<double> coefficients, double startFunction, double endFunction)
        {
            return CalculateFunc(coefficients, startFunction) * CalculateFunc(coefficients, endFunction) <= 0;
        }

        private static double Diff(List<double> coefficients, double x)
        {
            const double h = 1e-10;
            return (CalculateFunc(coefficients, x + h) - CalculateFunc(coefficients, x - h)) / (2.0 * h);
        }

        private static string FormatResult(double result, double epsilon)
        {
            var epsilonString = epsilon.ToString(CultureInfo.InvariantCulture);
            if (epsilon < 1)
            {
                return Math.Round(result, epsilonString.Contains('.') ? epsilonString!.Split('.')[1].Length : 0)
                    .ToString(CultureInfo.InvariantCulture);
            }

            var epsilonLength =
                epsilonString.Contains('.') ? epsilonString!.Split('.')[0].Length : epsilonString.Length;
            return ((int) (Math.Round(result / Math.Pow(10, epsilonLength - 1)) * Math.Pow(10, epsilonLength - 1)))
                .ToString();
        }
    }
}