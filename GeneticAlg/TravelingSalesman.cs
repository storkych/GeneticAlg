using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlg
{
    internal class TravelingSalesman
    {
        /// <summary>
        /// Инициализация начальной популяции (добавляем в список рандомные маршруты).
        /// </summary>
        /// <param name="numOfCities">Количество городов.</param>
        /// <param name="populationSize">Размер популяции.</param>
        /// <returns>Начальная популяция.</returns>
        public static List<List<int>> InitializePopulation(int numOfCities, int populationSize)
        {
            List<List<int>> population = new();

            for (var i = 0; i < populationSize; i++)
            {
                // Начальная вершина - 0.
                List<int> chromosome = new() { 0 };
                // Генерируем остальные вершины пути
                List<int> chromosomeEnd = GenerateUniqueNumberList(numOfCities);
                for (var j = 1; j < numOfCities; j++)
                {
                    chromosome.Add(chromosomeEnd[j - 1]);
                }
                // Добавляем хромосому в популяцию
                population.Add(chromosome);
            }

            return population;
        }

        /// <summary>
        /// Рандомная генерация вершин от второй до предпоследней.
        /// </summary>
        /// <param name="n">Количество элементов в списке.</param>
        /// <returns>Список вершин от второй до предпоследней.</returns>
        public static List<int> GenerateUniqueNumberList(int n)
        {
            List<int> numbers = new();

            // Добавляем числа от 1 до n в список.
            for (var i = 1; i < n; i++)
            {
                numbers.Add(i);
            }

            Random random = new();

            // Перетасовка элементов списка.
            for (var i = 0; i < n - 1; i++)
            {
                // Берём рандомный элемент списка от i до n-1.
                int randomIndex = random.Next(i, n - 1);
                // Присваиваем i-ому значение рандомного. Рандомному присваиваем значение i-ого и больше i-ый не трогаем.
                (numbers[randomIndex], numbers[i]) = (numbers[i], numbers[randomIndex]);
            }

            return numbers;
        }

        /// <summary>
        /// Вычисление приспособленности особей.
        /// </summary>
        /// <param name="population">Популяция.</param>
        /// <param name="distances">Массив расстояний между городами.</param>
        /// <returns>Словарь, где ключ - это особь (маршрут), значение - дистанция для этой особи (маршрута).</returns>
        public static Dictionary<List<int>, double> CalculateFitness(List<List<int>> population, double[,] distances)
        {
            Dictionary<List<int>, double> fitness = new();

            //Бежим по каждой хромосоме (особи) в популяции и считаем дистанцию.
            foreach (List<int> chromosome in population)
            {
                double distance = CalculateTotalDistance(chromosome, distances);
                // Добавляем в словарь.
                fitness.Add(chromosome, distance);
            }
            return fitness;
        }

        /// <summary>
        /// Вычисление общей длины пути.
        /// </summary>
        /// <param name="path">Особь (маршрут).</param>
        /// <param name="distances">Массив расстояний между городами.</param>
        /// <returns>Дистанция для этой особи.</returns>
        public static double CalculateTotalDistance(List<int> path, double[,] distances)
        {
            double totalDistance = 0;

            for (var i = 0; i < path.Count - 1; i++)
            {
                totalDistance += distances[path[i], path[i + 1]];
            }
            // Расстояние между последней вершиной и первой.
            totalDistance += distances[path[^1], path[0]];

            return totalDistance;
        }

        /// <summary>
        /// Выбор родителей для скрещивания.
        /// </summary>
        /// <param name="population">Популяция.</param>
        /// <param name="fitness">Словарь, где ключ - это особь (маршрут), значение - дистанция для этой особи (маршрута).</param>
        /// <returns>Список родителей.</returns>
        public static List<List<int>> SelectParents(List<List<int>> population)
        {
            List<List<int>> selectedParents = new();
            Random random = new();

            int randomIndex = random.Next(0, population.Count);
            for (var j = 0; j < population.Count; j++)
                if (j == randomIndex)
                {
                    selectedParents.Add(population[j]);
                    break;
                }

            int randomIndex2 = random.Next(0, population.Count);
            while (randomIndex2 == randomIndex) randomIndex2 = random.Next(0, population.Count);
            for (var j = 0; j < population.Count; j++)
                if (j == randomIndex2)
                {
                    selectedParents.Add(population[j]);
                    break;
                }

            return selectedParents;
        }

        /// <summary>
        /// Скрещивание родителей (одноточечное порядковое скрещивание).
        /// </summary>
        /// <param name="parents">Список рандомных родителей.</param>
        /// <returns>Список двух потомков.</returns>
        public static List<List<int>> Crossover(List<List<int>> parents)
        {
            Random random = new();
            // В первый список помещаем первого родителя.
            List<int> parent1 = parents[0];
            // Во второй список помещаем второго родителя.
            List<int> parent2 = parents[1];

            // Точка разрыва (элемент), не может быть первым или последним элементом, иначе родители и потомки будут идентичны.
            int crossoverPoint = random.Next(2, parent1.Count - 1);

            // Потомок 1. Создает неполную копию диапазона элементов в исходном List<T>. 0 - начало копирования, crossoverPoint - кол-во копируемых элементов
            List<int> offspring1 = parent1.GetRange(0, crossoverPoint);
            // Потомк 2. Аналогично.
            List<int> offspring2 = parent2.GetRange(0, crossoverPoint);
            // Добавляем недостающие гены, которых ещё нет в потомке от второго родителя (по порядку).
            for (var i = 0; i < parent2.Count; i++)
            {
                if (!offspring1.Contains(parent2[i]))
                {
                    offspring1.Add(parent2[i]);
                }
            }
            // Добавляем недостающие гены, которых ещё нет в потомке от первого родителя (по порядку).
            for (var i = 0; i < parent1.Count; i++)
            {
                if (!offspring2.Contains(parent1[i]))
                {
                    offspring2.Add(parent1[i]);
                }
            }

            return new List<List<int>> { offspring1, offspring2 };
        }

        /// <summary>
        /// Мутация потомства.
        /// </summary>
        /// <param name="offspring">Список двух потомков.</param>
        public static void Mutate(List<List<int>> offspring)
        {
            Random random = new();
            for (var i = 0; i < offspring.Count; i++)
            {
                // Возвращает случайное число с плавающей запятой, которое больше или равно 0,0, но меньше 1,0.
                if (random.NextDouble() < 0.1)
                {
                    // От первого элемента, чтобы не менялась нулевая вершина.
                    int index1 = random.Next(1, offspring[i].Count);
                    int index2 = random.Next(1, offspring[i].Count);
                    while (index1 == index2)
                    {
                        index2 = random.Next(1, offspring[i].Count);
                    }
                    (offspring[i][index2], offspring[i][index1]) = (offspring[i][index1], offspring[i][index2]);
                }
            }
        }

        /// <summary>
        /// Замещение популяции новыми особями.
        /// </summary>
        /// <param name="oldPopulation">Текущая популяция.</param>
        /// <param name="offspring">Список потомков.</param>
        /// <param name="fitness">Словарь, где ключ - это особь (маршрут), значение - дистанция для этой особи (маршрута).</param>
        /// <param name="distances">Массив расстояний между городами.</param>
        /// <returns>Новая популяция.</returns>
        public static List<List<int>> ReplacePopulation(List<List<int>> oldPopulation, List<List<int>> offspring, Dictionary<List<int>, double> fitness, double[,] distances)
        {
            // Создание новой коллекции и добавление в неё старой коллекции.
            Dictionary<List<int>, double> newDictionary = new(fitness);

            // Добавление в новую коллекцию потомков.
            foreach (List<int> chromosome in offspring)
            {
                double distance = CalculateTotalDistance(chromosome, distances);
                // Добавляем в словарь.
                newDictionary.Add(chromosome, distance);
            }

            // Сортировка коллекции по значениям. 
            // Метод ToDictionary() преобразует отсортированную последовательность обратно в словарь, сохраняя порядок сортировки.
            var sortedDictionary = newDictionary.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            // Создание новой популяции.
            List<List<int>> newPopulation = new(); ;
            // Отбрасываем 2 самых слабых (последних) элемента.
            int count = 0;
            foreach (var item in sortedDictionary)
            {
                newPopulation.Add(item.Key);
                count++;

                if (count == oldPopulation.Count)
                    break;
            }

            return newPopulation;
        }
    }

}

