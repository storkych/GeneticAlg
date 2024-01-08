using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlg
{
    /// <summary>
    /// Генетический алгоритм для решения задачи о рюкзаке.
    /// </summary>
    internal class BackpackSolver: IGeneticAlgorithm
    {
        /// <summary>
        /// Делегат для обратной связи в процессе генетического алгоритма.
        /// </summary>
        private readonly Action<string> generationCallback;

        private const double CROSSOVERPROBABILITY = 0.8;
        private const double MUTATIONPROBABILITY = 0.03;
        private const int POPULATIONSIZE = 100;
        private const int GENERATIONCOUNT = 500;
        public static int MaxValue;

        private static readonly Random Rnd = new();



        /// <summary>
        /// Переменные для хранения точек скрещивания и потомков.
        /// </summary>
        private static int _crossPoint;
        private static int _mutatePoint;
        private static int _child1;
        private static int _child2;

        /// <summary>
        /// Список геномов следующего поколения, список текущих геномов.
        /// </summary>
        private readonly List<BackpackGenome> NextGeneration = new();
        private readonly List<BackpackGenome> Solutions = new();

        public static List<BackpackItem> Selection = new();

        private BackpackGenome? _bestFitness;
        private int[] _crossedGenomes = new int[2];
        private BackpackGenome? _crossoverPartner;

        public BackpackSolver(Action<string> generationCallback, List<BackpackItem> selection, int maxValue)
        {
            this.generationCallback = generationCallback;
            Selection = selection;
            MaxValue = maxValue;
        }

        /// <summary>
        /// Запускает генетический алгоритм для решения задачи о рюкзаке.
        /// </summary>
        public void RunGeneticAlgorithm()
        {
            // Эволюция популяции.
            BackpackGenome Result = Evolve(GenerateRandomSolutions(POPULATIONSIZE));

            // Вывод результатов в консоль.
            generationCallback?.Invoke("Выбранные предметы:");
            generationCallback?.Invoke("");

            foreach (BackpackItem t in Result.ItemsPicked)
            {
                generationCallback?.Invoke(t.Name + "  " + "\t" + " (Ценность:" + t.Worth + ") " +
                                  "\t" + "(Вес:" + t.Weight + ")");
            }

            generationCallback?.Invoke("");
            generationCallback?.Invoke("Максимальный вес: " + MaxValue);
            generationCallback?.Invoke("Текущий вес: " + Result.ItemsPicked.Sum(t => t.Weight));
            generationCallback?.Invoke("Текущая ценность: " + Result.ItemsPicked.Sum(t => t.Worth));
            generationCallback?.Invoke("");
            generationCallback?.Invoke("Вес всех предметов: " + Selection.Sum(t => t.Weight));
            generationCallback?.Invoke("Ценность всех предметов: " + Selection.Sum(t => t.Worth));
        }

        /// <summary>
        /// Метод для эволюции генетического алгоритма.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public BackpackGenome Evolve(List<BackpackGenome> items)
        {
            Solutions.AddRange(items);
            for (var i = 0; i < GENERATIONCOUNT; i++)
            {
                for (var k = 0; k < POPULATIONSIZE; k++)
                {
                    Sort(Solutions[k]);
                    CalculateFitness(Solutions[k]);
                }

                _ = Solutions.OrderByDescending(t => t.Fitness);

                // Вычисление минимального значения приспособленности.
                double minimalFitness = Solutions.Where(x => !x.Fitness.Equals(0)).Sum(t => t.Fitness) / POPULATIONSIZE -
                                        Solutions.Count(x => !x.Fitness.Equals(0));

                // Генерация следующего поколения.
                while (NextGeneration.Count < POPULATIONSIZE)
                {
                    for (int m = 0; m < Solutions.Count; m++)
                    {
                        if (NextGeneration.Count < POPULATIONSIZE)
                        {
                            if (Solutions[m].Fitness >= minimalFitness && Rnd.NextDouble() <= CROSSOVERPROBABILITY)
                            {
                                if (_crossoverPartner == null)
                                {
                                    _crossoverPartner = Solutions[m];
                                }
                                else
                                {
                                    _crossedGenomes = Crossover(_crossoverPartner, Solutions[m]);

                                    foreach (var genome in _crossedGenomes.Select(child => new BackpackGenome(child)))
                                    {
                                        if (GetWeight(genome.Parameter) <= MaxValue)
                                        {
                                            NextGeneration.Add(genome);
                                        }
                                    }
                                }
                            }

                            if (Rnd.NextDouble() <= MUTATIONPROBABILITY)
                            {
                                NextGeneration.Add(new BackpackGenome(Mutation(Solutions[m])));
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                while (NextGeneration.Count > POPULATIONSIZE)
                {
                    NextGeneration.RemoveAt(NextGeneration.Count - 1);
                }

                if (_bestFitness != null)
                {
                    if (_bestFitness.Fitness <= Solutions[0].Fitness)
                    {
                        _bestFitness = Solutions[0];
                    }
                }
                else
                {
                    _bestFitness = Solutions[0];
                }

                Solutions.RemoveAll(t => t.Parameter != null);
                Solutions.AddRange(NextGeneration);
                NextGeneration.RemoveAll(t => t.Parameter != null);
            }
            return _bestFitness;
        }

        /// <summary>
        /// Метод для вычисления приспособленности генома.
        /// </summary>
        /// <param name="genom"></param>
        public static void CalculateFitness(BackpackGenome genom)
        {


            // Суммирование ценности выбранных предметов.
            genom.Fitness += genom.ItemsPicked.Sum(t => t.Worth);

            // Рассчет штрафа за превышение максимального веса с использованием квадратичной функции.
            float x = genom.ItemsPicked.Sum(t => t.Weight) - MaxValue;
            genom.Fitness += (-((x * x) + (10 * x))); // Изменяем формулу для более сильного штрафа


            // Если приспособленность меньше 1, устанавливаем ее в 1.
            if (genom.Fitness < 1)
            {
                genom.Fitness = 1;
            }
        }

        /// <summary>
        /// Генетическое скрещивание.
        /// </summary>
        /// <param name="parent1"></param>
        /// <param name="parent2"></param>
        /// <returns></returns>
        public static int[] Crossover(BackpackGenome parent1, BackpackGenome parent2)
        {
            _crossPoint = Rnd.Next(1, Selection.Count);

            int temp = (1 << _crossPoint) - 1;
            _child1 = parent1.Parameter & temp;
            _child2 = parent2.Parameter & temp;

            temp = Int32.MaxValue - temp;
            _child1 |= (parent2.Parameter & temp);
            _child2 |= (parent1.Parameter & temp);

            // Повторяем кроссовер, если оба потомка равны 0
            if (_child1 == 0 && _child2 == 0)
            {
                return Crossover(parent1, parent2);
            }

            var crossedGenomes = new int[2];
            crossedGenomes[0] = _child1;
            crossedGenomes[1] = _child2;
            return crossedGenomes;
        }


        /// <summary>
        /// Генетическая мутация.
        /// </summary>
        /// <param name="genom"></param>
        /// <returns></returns>
        public static int Mutation(BackpackGenome genom)
        {
            _mutatePoint = Rnd.Next(0, 31);
            int temp = (1 << _mutatePoint);
            if (GetWeight(genom.Parameter ^ temp) <= MaxValue)
            {
                return genom.Parameter ^ temp;
            }
            else
            {
                return genom.Parameter;
            }
        }

        public static int GetWeight(int parameter)
        {
            int bitCount = sizeof(int) * 8; // Получаем количество битов в int
            return Enumerable.Range(0, bitCount)
                .Where(i => (parameter & (1 << i)) != 0 && i < Selection.Count) // Добавляем проверку на допустимый диапазон
                .Sum(i => Selection[i].Weight);
        }


        /// <summary>
        /// Сортирует предметы в рюкзаке по порядку выбора.
        /// </summary>
        /// <param name="genom">Геном с информацией о выбранных предметах.</param>
        public static void Sort(BackpackGenome genom)
        {
            int temp = genom.Parameter;

            for (var i = 0; i < Selection.Count; i++)
            {
                int check = temp & 1 << i;
                if (check == (1 << i))
                {
                    genom.ItemsPicked.Add(Selection[i]);
                }
            }
        }

        /// <summary>
        /// Генерирует случайные геномы для начальной популяции.
        /// </summary>
        /// <param name="populationSize">Размер популяции.</param>
        /// <returns>Список сгенерированных геномов.</returns>
        public static List<BackpackGenome> GenerateRandomSolutions(int populationSize)
        {
            var temp = new List<BackpackGenome>();
            int maxParameter = (1 << Selection.Count) - 1;
            int maxAttempts = 1000; // Максимальное количество попыток

            for (var i = 0; i < populationSize; i++)
            {
                int randomParameter = 0;
                int remainingWeight = MaxValue;
                int attempts = 0;

                // Цикл продолжается до выбора всех предметов или достижения максимального числа попыток
                while (randomParameter != maxParameter && attempts < maxAttempts)
                {
                    int randomIndex = Rnd.Next(0, Selection.Count);
                    int bit = 1 << randomIndex;

                    if ((randomParameter & bit) == 0)
                    {
                        int itemWeight = Selection[randomIndex].Weight;

                        // Проверяем, чтобы не добавить предмет, если не хватает места
                        if (remainingWeight >= itemWeight)
                        {
                            randomParameter |= bit;
                            remainingWeight -= itemWeight;
                        }
                    }

                    attempts++;
                }

                temp.Add(new BackpackGenome(randomParameter));
            }

            return temp;
        }



        /// <summary>
        /// Очистка списка.
        /// </summary>
        /// <param name="result">Результат работы алгоритма.</param>
        private static void ClearLists(BackpackGenome result)
        {
            result.ItemsPicked.Clear();
            Selection.Clear();

        }
    }
}
