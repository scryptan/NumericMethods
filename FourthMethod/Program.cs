using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FourthMethod
{
    class Program
    {
        enum States
        {
            Start,
            AnswerInput,
            Calculation,
            Finished,
            Exit
        }

        static void Main()
        {
            var state = States.Start;
            var LS = new List<List<double>>
            {
                new List<double> {2.5, 1.5, 0, 0},
                new List<double> {-2, 4, -1, 0},
                new List<double> {0, 1, 6, -1},
                new List<double> {0, 0, 2, 5},
            };
            // var LS = new List<List<double>>
            // {
            // new List<double> {10, 1, 0, 0},
            // new List<double> {-2, 9, 1, 0},
            // new List<double> {0, 0.1, 4, -1},
            // new List<double> {0, 0, -1, 8},
            // };
            var answer = new List<double> {8.4, 4, 5.6, 7};
            // var answer = new List<double> {5, -1, -5, 40};
            while (true)
            {
                switch (state)
                {
                    case States.Start:
                        if (LS.Count < 1)
                        {
                            PrintDefaultText($"Введите коэффициенты {LS.Count + 1}го уравнения:");
                            var tempRowRow = Console.ReadLine()!.Split(' ').Select(x => double.Parse(x)).ToList();
                            if (LS.Count > 0 && tempRowRow.Count != LS.First().Count)
                            {
                                PrintErrorText($"Элементов в строке должно быть ровно {LS.First().Count}");
                                break;
                            }

                            LS.Add(tempRowRow);
                        }

                        if (LS.Count >= LS.First().Count)
                        {
                            state = States.AnswerInput;
                            break;
                        }

                        state = States.Start;
                        break;
                    case States.AnswerInput:
                        if (answer.Count < 1)
                        {
                            PrintDefaultText("Введите строку ответов к уравнениям: ");
                            answer = Console.ReadLine()!.Split(' ').Select(x => double.Parse(x)).ToList();

                            if (answer.Count != LS.Count)
                            {
                                PrintErrorText(
                                    $"Количество ответов не совпадает с количеством уравнений, их должно быть {LS.Count} штук");
                                break;
                            }
                        }

                        if (!CanSolveMatrix(LS, answer))
                        {
                            PrintErrorText(
                                $"Ранг матрицы не совпадает с рангом расширенной амтрицы, значит она несовместна и решений не имеет");
                            state = States.Finished;
                            break;
                        }

                        state = States.Calculation;
                        break;
                    case States.Calculation:
                        var coefs = new List<List<double>>
                        {
                            new List<double> {0, LS[0][0], LS[0][1], answer[0]},
                            new List<double> {LS[1][0], LS[1][1], LS[1][2], answer[1]},
                            new List<double> {LS[2][1], LS[2][2], LS[2][3], answer[2]},
                            new List<double> {LS[3][2], LS[3][3], 0, answer[3]},
                        };
                        var uList = new List<double>();
                        var vList = new List<double>();
                        uList.Add(-coefs[0][2] / coefs[0][1]);
                        vList.Add(coefs[0][3] / coefs[0][1]);
                        for (int i = 1; i < coefs.Count; i++)
                        {
                            uList.Add(-coefs[i][2] / (coefs[i][0] * uList[i - 1] + coefs[i][1]));
                            vList.Add((coefs[i][3] - coefs[i][0] * vList[i - 1]) /
                                      (coefs[i][0] * uList[i - 1] + coefs[i][1]));
                        }
                        //Обратный ход
                        var xList = new double[coefs.Count];
                        xList[xList.Length - 1] = uList.Last() + vList.Last();
                        for (int i = coefs.Count - 2; i >= 0; i--)
                        {
                            xList[i] = uList[i] * xList[i + 1] + vList[i];
                        }
                        var discrepancy = new List<double>();
                        for (int i = 0; i < coefs.Count; i++)
                        {
                            discrepancy.Add(answer[i]);
                            for (int j = 0; j < coefs.Count; j++)
                            {
                                if(discrepancy.Count > i)
                                    discrepancy[i] -= LS[i][j] * xList[j];
                            }
                        }
                        for (int i = 0; i < coefs.Count; i++)
                        {
                            PrintCorrectText($"X{i+1}: {xList[i]}   невязка: {Math.Round(discrepancy[i], 2)}");
                        }
                        state = States.Finished;
                        break;
                    case States.Finished:
                        PrintChooseText("Что делать дальше:\n" +
                                        "(1) Решить новую матрицу\n" +
                                        "(2) Завершить программу");
                        if (!TryGetInt(out var nextCommand))
                            break;
                        switch (nextCommand)
                        {
                            case 1:
                                answer = new List<double>();
                                LS = new List<List<double>>();
                                state = States.Start;
                                break;
                            case 2:
                                state = States.Exit;
                                break;
                        }

                        break;
                    case States.Exit:
                        Environment.Exit(0);
                        return;
                }
            }
        }

        private static bool CanSolveMatrix(List<List<double>> LS, List<double> answers)
        {
            for (int i = 0; i < LS.Count; ++i)
                if (LS[i].Sum() < 1e-3 && answers[i] != 0)
                {
                    PrintErrorText("Система не имеет решений");
                    return false;
                }

            return true;
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

        private static void PrintCorrectText(string text)
        {
            PrintColorizedText($"{text}\n", ConsoleColor.Yellow);
        }

        private static void PrintChooseText(string text)
        {
            PrintColorizedText($"{text}\n", ConsoleColor.DarkGreen);
        }

        private static void PrintErrorText(string text)
        {
            PrintColorizedText($"{text}\n", ConsoleColor.Red);
        }

        private static void PrintDefaultText(string text)
        {
            PrintColorizedText($"{text}\n", ConsoleColor.Blue);
        }

        private static void PrintIterationText(string text)
        {
            PrintColorizedText(text, ConsoleColor.Cyan);
        }

        private static void PrintColorizedText(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}