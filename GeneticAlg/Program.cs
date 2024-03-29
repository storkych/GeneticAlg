﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GeneticAlg
{
    class Program
    {
        /// <summary>
        /// Метод для ввода целочисленного значения с нижним ограничением
        /// </summary>
        /// <param name="prompt"> Выводящийся промт </param>
        /// <param name="minimumValue"> Минимальный порог для получаемого значения </param>
        /// <returns></returns>
        static int GetMinimumInteger(string prompt, int minimumValue)
        {
            int result;

            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (input == null)
                {
                    // Обработка ситуации, когда ReadLine возвращает null (например, при завершении работы программы)
                    Console.WriteLine("Произошла ошибка при чтении ввода.");
                    continue;
                }

                try
                {
                    result = int.Parse(input);

                    if (result >= minimumValue)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Пожалуйста, введите целое число больше или равное {minimumValue}.");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Пожалуйста, введите целое число.");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Введенное число слишком большое или слишком маленькое.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }
            return result;
        }
        /// <summary>
        /// Метод ввода строки
        /// </summary>
        /// <param name="prompt"> Промт выводимый пользователю</param>
        /// <param name="validation"> Функция валидации (требуемые условия для получаемой строки) </param>
        /// <returns></returns>
        static string GetString(string prompt, Func<string, bool> validation)
        {
            string? result;

            while (true)
            {
                Console.Write(prompt);

                try
                {
                    result = Console.ReadLine();

                    if (validation(result!))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Пожалуйста, введите корректное значение.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }
            return result!;
        }

        /// <summary>
        /// Метод для вывода строки в консоль, используем как делегат
        /// </summary>
        /// <param name="target"> Целевая строка </param>
        static void GenerationCallback(string target)
        {
            Console.WriteLine(target);
        }

        /// <summary>
        /// Метод для обработки действий в меню.
        /// </summary>
        /// <param name="index"> Индекс выбранного пункта меню </param>
        static void ExecuteMenuItem(int index)
        {
            Stopwatch stopWatch = new();
            Console.Clear();

            switch ((MenuOption)index)
            {
                case MenuOption.BackpackProblem:
                    // Ввод максимального веса в рюкзаке.
                    int maxValue = GetMinimumInteger("Введите максимальный вес в рюкзаке (минимум 1): ", 1);

                    // Ввод количества предметов (не менее 5).
                    int itemCount = GetMinimumInteger("Введите количество предметов (минимум 5): ", 5);

                    // Создание списка для предметов.
                    List<BackpackItem> itemList = new();

                    // Ввод данных для каждого предмета.
                    for (var i = 0; i < itemCount; i++)
                    {
                        Console.WriteLine($"\nВведите данные для предмета {i + 1}:");

                        // Ввод веса (больше 0).
                        int weight = GetMinimumInteger("Вес предмета (больше 0): ", 1);

                        // Ввод стоимости (больше 0).
                        int worth = GetMinimumInteger("Стоимость предмета (больше 0): ", 1);

                        // Задаём имя предмета.
                        string name = "Item" + (i + 1).ToString();

                        // Создание объекта BackpackItem и добавление его в список.
                        BackpackItem newItem = new(weight, worth, name);
                        itemList.Add(newItem);
                    }
               
                    IGeneticAlgorithm backpackSolver = new BackpackSolver(GenerationCallback, itemList, maxValue);
                    stopWatch.Start();
                    backpackSolver.RunGeneticAlgorithm();
                    break;

                case MenuOption.QueenProblem:
                    int quantity = GetMinimumInteger("Введите число королев(не менее 4): ", 4);

                    stopWatch.Start();
                    QueenGenAlg queenSolver = new(GenerationCallback);

                    QueenBoards solution = queenSolver.QueenGeneticAlg(quantity);

                    Console.WriteLine(solution.ToString() + "\n");
                    Console.WriteLine(solution.PrintBoard());
                    break;

                case MenuOption.StringRebuilder:
                    // Ввод целевой строки (не пустой)
                    string target = GetString("Введите целевую строку (не может быть пустой + только латиница): ", 
                        s => !string.IsNullOrWhiteSpace(s) && 
                        s.All(c => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".Contains(c)));

                    IGeneticAlgorithm stringRebuider = new StringRebuider(GenerationCallback, target);
                    stopWatch.Start();
                    stringRebuider.RunGeneticAlgorithm();
                    break;

                case MenuOption.TravellingSalesman:
                    // Входные данные.
                    // Количество городов. Минимум 3 городa.
                    int numOfCities = GetMinimumInteger("Введите число городов (не менее 3): ", 3);

                    double[,] distances = new double[numOfCities, numOfCities];

                    Console.WriteLine("Заполните матрицу расстояний: ");
                    for (var i = 0; i < numOfCities; i++)
                    {
                        for (var j = 0; j < numOfCities; j++)
                        {
                            while (!double.TryParse(Console.ReadLine(), out distances[i, j]) || distances[i, j] <= 0)
                            {
                                Console.Write("Неправильный ввод данных. Попробуйте ещё раз: ");
                            }
                        }
                    }

                    Console.WriteLine("Матрица расстояний: ");
                    for (var i = 0; i < numOfCities; i++)
                    {
                        for (var j = 0; j < numOfCities; j++)
                            Console.Write(string.Format("{0,3}", distances[i, j]));
                        Console.WriteLine();
                    }

                    // Параметры генетического алгоритма.
                    // Размер одной популяции = 100.
                    int populationSize = 100;
                    // Сколько сменится поколений.

                    int maxGenerations = 100;

                    // Создание начальной популяции (популяция - это множество особей, то есть маршрутов).
                    List<List<int>> population = TravelingSalesman.InitializePopulation(numOfCities, populationSize);

                    Console.WriteLine("Начальная популяция: ");
                    foreach (List<int> list in population)
                    {
                        foreach (int number in list)
                        {
                            Console.Write(number);
                        }
                        Console.WriteLine(" ");
                    }
                    stopWatch.Start();
                    // Основной алгоритм.
                    for (var generation = 0; generation < maxGenerations; generation++)
                    {
                        // Вычисление приспособленности особей в текущей популяции.
                        Dictionary<List<int>, double> fitness = TravelingSalesman.CalculateFitness(population, distances);

                        // Выбор рандомных особей для скрещивания.
                        List<List<int>> selectedParents = TravelingSalesman.SelectParents(population);

                        Console.WriteLine("Выбранные родители: ");
                        foreach (List<int> list in selectedParents)
                        {
                            foreach (int number in list)
                            {
                                Console.Write(number);
                            }
                            Console.WriteLine(" ");
                        }

                        // Скрещивание выбранных особей.
                        List<List<int>> offspring = TravelingSalesman.Crossover(selectedParents);

                        // Мутация потомства.
                        TravelingSalesman.Mutate(offspring);

                        Console.WriteLine("Потомки: ");
                        foreach (List<int> list in offspring)
                        {
                            foreach (int number in list)
                            {
                                Console.Write(number);
                            }
                            Console.WriteLine(" ");
                        }

                        // Замещение старой популяции новой.
                        population = TravelingSalesman.ReplacePopulation(population, offspring, fitness, distances);

                        Console.WriteLine("Отсортированная новая популяция: ");
                        Console.WriteLine("Особь:    Расстояние:");
                        foreach (List<int> chromosome in population)
                        {
                            foreach (int number in chromosome)
                            {
                                Console.Write(number);
                            }
                            Console.Write("     ");
                            Console.WriteLine(TravelingSalesman.CalculateTotalDistance(chromosome, distances));
                        }

                        // Если наблюдается постоянство сильных особей, значит скорее всего нашли оптимальный маршрут, выходим из цикла.
                        if ((TravelingSalesman.CalculateTotalDistance(population[0], distances) == TravelingSalesman.CalculateTotalDistance(population[1], distances))
                            && (TravelingSalesman.CalculateTotalDistance(population[1], distances)) == TravelingSalesman.CalculateTotalDistance(population[2], distances))
                        {
                            Console.WriteLine("Поколение: ");
                            Console.WriteLine(generation);
                            break;
                        }
                    }

                    Console.Write("Путь: ");
                    foreach (int number in population[0])
                    {
                        Console.Write(number);
                    }
                    Console.WriteLine(" ");
                    Console.Write("Расстояние: ");
                    Console.WriteLine(TravelingSalesman.CalculateTotalDistance(population[0], distances));
                    break;

                case MenuOption.Exit:
                    Environment.Exit(0);
                    break;
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            Console.WriteLine($"Время работы алгоритма : {ts}");
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        /// <summary>
        /// Запуск программы.
        /// </summary>
        static void Main()
        {
            Console.CursorVisible = false;

            string[] menuItems = { "Задача о рюкзаке", "Расстановка ферзей", "Воссоздание строки", "Задача коммивояжера", "Выход" };
            int selectedItemIndex = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Выберите пункт меню:");

                for (var i = 0; i < menuItems.Length; i++)
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





