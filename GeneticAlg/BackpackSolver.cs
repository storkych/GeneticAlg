using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlg
{
    internal class BackpackSolver
    {
        private Action<string> generationCallback;

        private const double CROSSOVERPROBABILITY = 0.8;
        private const double MUTATIONPROBABILITY = 0.1;
        private const int POPULATIONSIZE = 100;
        private const int GENERATIONCOUNT = 500;
        public static int MaxValue;

        private static readonly Random Rnd = new Random();

        public static List<string> ListOfNames = new List<string>();
        public static int NumberOfNames = 1000;

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
        private readonly List<BackpackGenome> NextGeneration = new List<BackpackGenome>();
        private readonly List<BackpackGenome> Solutions = new List<BackpackGenome>();

        public static List<BackpackItem> Selection = new List<BackpackItem>();

        private BackpackGenome _bestFitness;
        private int[] _crossedGenomes = new int[2];
        private BackpackGenome _crossoverPartner;

        public BackpackSolver(Action<string> generationCallback, List<BackpackItem> selection, int maxValue)
        {
            this.generationCallback = generationCallback;
            Selection = selection;
            MaxValue = maxValue;
        }

        public void RunApplication()
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
            Console.ReadKey();

            ClearLists(Result);
            Console.Clear();
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

                Solutions.OrderByDescending(t => t.Fitness);

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

                                    NextGeneration.Add(new BackpackGenome(_crossedGenomes[0]));
                                    NextGeneration.Add(new BackpackGenome(_crossedGenomes[1]));
                                    _crossoverPartner = null;
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
            // Если вес выбранных предметов превышает 10% от максимального значения, приспособленность устанавливается в 0.
            if (genom.ItemsPicked.Sum(t => t.Weight) >= MaxValue * 1.1f)
            {
                genom.Fitness = 0;
                return;
            }

            // Суммирование ценности выбранных предметов.
            genom.Fitness += genom.ItemsPicked.Sum(t => t.Worth);

            // Рассчет штрафа за превышение максимального веса с использованием квадратичной функции.
            float x = genom.ItemsPicked.Sum(t => t.Weight) - MaxValue;
            genom.Fitness += (-((0.5f * x * x) + (5 * x)));

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
            _crossPoint = Rnd.Next(1, 32);

            int temp = (1 << _crossPoint) - 1;
            _child1 = parent1.Parameter & temp;
            _child2 = parent2.Parameter & temp;

            temp = Int32.MaxValue - temp;
            _child1 = _child1 | (parent2.Parameter & temp);
            _child2 = _child2 | (parent1.Parameter & temp);

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
            _child1 = genom.Parameter ^ temp;
            return _child1;
        }

        public static void Sort(BackpackGenome genom)
        {
            int temp = genom.Parameter;

            int topRange = 31;
            if (Selection.Count < 31) { topRange = Selection.Count; }


            for (var i = 0; i < topRange; i++)
            {
                int check = temp & 1 << i;
                if (check == (1 << i))
                {
                    genom.ItemsPicked.Add(Selection[i]);
                }
            }
        }

        public static List<BackpackGenome> GenerateRandomSolutions(int populationSize)
        {
            var temp = new List<BackpackGenome>();
            for (var i = 0; i < populationSize; i++)
            {
                // Создание генома с случайным параметром.
                temp.Add(new BackpackGenome(Rnd.Next(1, Int32.MaxValue))); //TODO
            }
            return temp;
        }

        private static void ClearLists(BackpackGenome result)
        {
            result.ItemsPicked.Clear();
            Selection.Clear();
            ListOfNames.Clear();
        }
    }
}
