using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ThirdMethod
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
                new List<double> {1, 1, 2, 1},
                new List<double> {1, 2, 4, 2},
                new List<double> {2, 3, 8, 4},
                new List<double> {3, 4, 10, 6},
            };
            var answer = new List<double> {0, 1, 2, 3};
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
                        CanSolveMatrix(LS, answer);
                        MakeTriangleMatrix(LS, answer);
                        answer.Reverse();
                        LS.Reverse();
                        foreach (var list in LS)
                            list.Reverse();

                        MakeTriangleMatrix(LS, answer);
                        answer.Reverse();
                        var answerString = new StringBuilder();
                        for (int i = 0; i < answer.Count; i++)
                            answerString.Append($"X{i + 1} = {answer[i]} ");
                        PrintCorrectText(answerString.ToString());
                        state = States.Finished;
                        break;
                    case States.Finished:
                        PrintChooseText("Что делать дальше:\n" +
                                        "(1) Решить новую матрицу\n" +
                                        "(2) Завершить программу");
                        if (!TryGetInt(out var nextCommand))
                            break;
                        state = nextCommand switch
                        {
                            1 => States.Start,
                            2 => States.Exit,
                            _ => States.Finished
                        };
                        break;
                    case States.Exit:
                        Environment.Exit(0);
                        return;
                }
            }
        }

        private static void MakeTriangleMatrix(List<List<double>> LS, List<double> answers)
        {

            for (int g = 0; g < LS.Count; g++)
            {
                for (int f = 0; f < LS.Count; f++)
                    Console.Write($"{LS[g][f]} ");
                Console.Write($"|{answers[g]}");
                Console.WriteLine();
            }

            Console.WriteLine("_______________________");
            for (int i = 0; i < LS.Count; i++)
            {
                var tempDivide = LS[i][i];
                answers[i] /= tempDivide;
                for (int j = i; j < LS.Count; j++)
                    LS[i][j] = LS[i][j] / tempDivide;
                for (int j = i + 1; j < LS.Count; j++)
                {
                    var temp = new List<double>(LS[i]);
                    var tempAnswers = answers[i];
                    tempAnswers *= -LS[j][i];
                    for (int k = i; k < LS.Count; k++)
                        temp[k] *= -LS[j][i];
                    answers[j] += tempAnswers;
                    for (int k = i; k < LS.Count; k++)
                        LS[j][k] += temp[k];

                    for (int g = 0; g < LS.Count; g++)
                    {
                        for (int f = 0; f < LS.Count; f++)
                            Console.Write($"{LS[g][f]} ");
                        Console.Write($"|{answers[g]}");
                        Console.WriteLine();
                    }

                    Console.WriteLine("_______________________");
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