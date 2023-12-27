using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlg
{
    class Program
    {
        static int NumQueens;

        static void GenerationCallback(int generation, string bestIndividual, string target)
        {
            Console.WriteLine($"Generation {generation}: Best fit - {bestIndividual.Length}, Best individual - {bestIndividual}");
        }

        static void ExecuteMenuItem(int index)
        {
            Console.Clear();
            Console.WriteLine($"Вы выбрали: {index + 1}");
            if (index == 0)
            {
                
            }
            else if (index == 1)
            {
                while (true)
                {
                    Console.WriteLine("Введите число королев: ");
                    if (int.TryParse(Console.ReadLine(), out NumQueens) && (NumQueens > 0))
                    {
                        Console.WriteLine("Запуск алгоритма");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Ошибка. Некорректное значение");
                    }
                }
                //Время работы программы.
                //Stopwatch stopWatch = new();
                //stopWatch.Start();
                //Вызываем метод geneticAlg() для выполнения генетического алгоритма.
                QueenBoards solution = QueenGenAlg.QueenGeneticAlg(NumQueens);
                //stopWatch.Stop();
                //TimeSpan ts = stopWatch.Elapsed;
                //Console.WriteLine($"Время работы алгоритма : {ts}");
                //Выводит решение на экран.
                Console.WriteLine(solution.ToString() + "\n");
                solution.PrintBoard();
                Console.ReadLine();
            }
            else if (index == 2)
            {
                string target = "HelloWorld";
                int populationSize = 100;
                double mutationRate = 0.03;

                StringRebuider geneticAlgorithm = new StringRebuider(GenerationCallback);
                geneticAlgorithm.RunGeneticAlgorithm(target, populationSize, mutationRate);
            }
            else if (index == 3)
            {
                
            }

            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void Main()
        {
            Console.CursorVisible = false;

            string[] menuItems = { "Задача о рюкзаке", "Расстановка ферзей", "Воссоздание строки", "Задача коммивояжера" };
            int selectedItemIndex = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Выберите пункт меню:");

                for (int i = 0; i < menuItems.Length; i++)
                {
                    if (i == selectedItemIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write(">");
                    }
                    else
                    {
                        Console.Write(" ");
                    }

                    Console.WriteLine(" " + menuItems[i]);

                    Console.ResetColor();
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedItemIndex = (selectedItemIndex - 1 + menuItems.Length) % menuItems.Length;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedItemIndex = (selectedItemIndex + 1) % menuItems.Length;
                        break;

                    case ConsoleKey.Enter:
                        ExecuteMenuItem(selectedItemIndex);
                        break;
                }
            }

        }
    }
}





