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
        /// Переменные для хранения точек мутации, скрещивания и потомков.
        /// </summary>
        private static int _crossPoint;
        private static int _mutatePoint;
        private static int _child1;
        private static int _child2;

        // Список геномов следующего поколения, список текущих геномов.
        private List<BackpackGenome> NextGeneration = new();
        private readonly List<BackpackGenome> Solutions = new();
        // Список предметов.
        public static List<BackpackItem> Selection = new();

        // Лучшая особь.
        private BackpackGenome? _bestFitness;
        // Массив, который используется для хранения двух геномов, результатов кроссовера.
        private int[] _crossedGenomes = new int[2];
        // Хранит ссылку на геном, который будет использован в качестве партнера для кроссовера.
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
            // Добавление всех предметов в список предметов.
            Solutions.AddRange(items);
            for (var i = 0; i < GENERATIONCOUNT; i++)
            {
                for (var k = 0; k < POPULATIONSIZE; k++)
                {
                    // Определение списка предметов для каждого генома.
                    Sort(Solutions[k]);
                    // Подсчет функции приспособленности для каждого генома.
                    CalculateFitness(Solutions[k]);
                }

                // Вычисление минимального значения приспособленности для установления порога, выше которого геномы будут участвовать в операции кроссовера.
                double minimalFitness = Solutions.Sum(t => t.Fitness) / POPULATIONSIZE;

                // Генерация следующего поколения.
                while (NextGeneration.Count < POPULATIONSIZE)
                {
                    // Цикл для обработки каждого генома в текущей популяции.
                    for (int m = 0; m < Solutions.Count; m++)
                    {
                        if (NextGeneration.Count < POPULATIONSIZE)
                        {
                            // Проверка, соответствует ли приспособленность генома требованиям для участия в кроссовере.
                            if (Solutions[m].Fitness >= minimalFitness && Rnd.NextDouble() <= CROSSOVERPROBABILITY)
                            {
                                // Если партнер для кроссовера не определен, текущий геном становится партнером.
                                if (_crossoverPartner == null)
                                {
                                    _crossoverPartner = Solutions[m];
                                }
                                // Если партнер уже определен, происходит кроссовер между текущим геномом и партнером.
                                else
                                {
                                    _crossedGenomes = Crossover(_crossoverPartner, Solutions[m]);

                                    // Создаем новые геномы на основе значений из массива _crossedGenomes.
                                    // Проходим по каждому новому геному.
                                    foreach (var genome in _crossedGenomes.Select(child => new BackpackGenome(child)))
                                    {
                                        // Если вес нового генома не превышает максимально допустимый вес, то этот геном добавляется в следующее поколение.
                                        if (GetWeight(genome.Parameter) <= MaxValue)
                                        {
                                            NextGeneration.Add(genome);
                                        }
                                    }
                                }
                            }
                            // Вероятность мутации генома.
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
                // Сортировка следующего поколения по убыванию приспособленности.
                NextGeneration = NextGeneration.OrderByDescending(t => t.Fitness).ToList();
                // Усечение следующего поколения до установленного размера.
                while (NextGeneration.Count > POPULATIONSIZE)
                {
                    NextGeneration.RemoveAt(NextGeneration.Count - 1);
                }
                // Обновление лучшего результата, если текущий лучше.
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
                // Удаление геномов, которые участвовали в операции кроссовера или мутации.
                Solutions.RemoveAll(t => t.Parameter != 0);
                // Добавление следующего поколения к текущему.
                Solutions.AddRange(NextGeneration);
                // Очистка следующего поколения от геномов, которые могли остаться.
                NextGeneration.RemoveAll(t => t.Parameter != 0);
            }
            // Возвращение лучшего генома, полученного в результате эволюции.
            return _bestFitness!;
        }

        /// <summary>
        /// Метод для вычисления приспособленности генома.
        /// </summary>
        /// <param name="genom">Геном.</param>
        public static void CalculateFitness(BackpackGenome genom)
        {
            // Суммирование ценности выбранных предметов.
            genom.Fitness += genom.ItemsPicked.Sum(t => t.Worth);
        }

        /// <summary>
        /// Генетическое скрещивание.
        /// </summary>
        /// <param name="parent1">Родитель 1.</param>
        /// <param name="parent2">Родитель 2.</param>
        /// <returns></returns>
        public static int[] Crossover(BackpackGenome parent1, BackpackGenome parent2)
        {
            _crossPoint = Rnd.Next(1, Selection.Count - 1);
            // Создание маски, которая содержит 1 во всех битах до точки разрыва (включительно) и нули после.
            int mask = (1 << _crossPoint) - 1;
            // Берутся биты до точки разрыва из одного родителя и биты после точки разрыва из другого.
            _child1 = (parent1.Parameter & ~mask) | (parent2.Parameter & mask);
            _child2 = (parent2.Parameter & ~mask) | (parent1.Parameter & mask);
            // Повторяем кроссовер, если оба потомка равны 0
            if (_child1 == 0 && _child2 == 0)
            {
                return Crossover(parent1, parent2);
            }
            // Массив возвращается как результат операции кроссовера.
            var crossedGenomes = new int[2];
            crossedGenomes[0] = _child1;
            crossedGenomes[1] = _child2;
            return crossedGenomes;
        }

        /// <summary>
        /// Генетическая мутация.
        /// </summary>
        /// <param name="genom">Текущий геном для мутации</param>
        /// <returns></returns>
        public static int Mutation(BackpackGenome genom)
        {
            // Индекс случайного предмета в списке Selection.
            _mutatePoint = Rnd.Next(0, Selection.Count + 1);
            // Cоздаем маску, установив бит в соответствии с индексом. 
            int temp = (1 << _mutatePoint);
            // Проверяем, не превышает ли вес генома после мутации максимально допустимый вес.
            if (GetWeight(genom.Parameter ^ temp) <= MaxValue)
            {
                // Вовзвращаем мутированный геном.
                return genom.Parameter ^ temp;
            }
            else
            {
                // Возвращаем исходный геном.
                return genom.Parameter;
            }
        }

        /// <summary>
        /// Вычисление веса решения, представленного геномом.
        /// </summary>
        /// <param name="parameter">Число, характеризующее параметр генома.</param>
        /// <returns></returns>
        public static int GetWeight(int parameter)
        {
            int weight = 0;
            // Цикл проходится по каждому биту.
            for (int i = 0; i < Selection.Count; i++)
            {
                // Если бит в параметре для соответствующего предмета установлен в 1, то добавляем его вес к общему весу.
                if ((parameter & (1 << i)) != 0)
                {
                    weight += Selection[i].Weight;
                }
            }
            return weight;
        }

        /// <summary>
        /// Преобразует бинарное представление генома в конкретные предметы, выбранные в рюкзак.
        /// </summary>
        /// <param name="genom">Геном с информацией о выбранных предметах.</param>
        public static void Sort(BackpackGenome genom)
        {
            //  Временная переменная temp, бинарное представление генома.
            int temp = genom.Parameter;
            //  Цикл через все предметы в списке предметов. 
            for (var i = 0; i < Selection.Count; i++)
            {
                // Проверяется бит на позиции i в переменной temp. Если бит равен 1, то check будет равен 1 << i, иначе будет равен 0.
                int check = temp & 1 << i;
                // Если предмет был выбран (бит на позиции i равен 1), то предмет добавляется в список ItemsPicked объекта genom.
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
            // Временный список для хранения сгенерированных геномов.
            var temp = new List<BackpackGenome>();
            // Rаждый предмет в рюкзаке представлен одним битом, максимальное значение параметра - число, у которого все биты установлены в 1.
            int maxParameter = (1 << Selection.Count) - 1;
            // Максимальное количество попыток.
            int maxAttempts = 1000;

            for (var i = 0; i < populationSize; i++)
            {
                // Инициализация, параметр генома = 0.
                int randomParameter = 0;
                // Оставшийся вес, изначально = 0.
                int remainingWeight = MaxValue;
                // Номер попытки.
                int attempts = 0;

                // Цикл продолжается до выбора всех предметов или достижения максимального числа попыток.
                while (randomParameter != maxParameter && attempts < maxAttempts)
                {
                    // Генерируется случайный индекс, представляющий случайный выбор предмета из списка доступных предметов.
                    int randomIndex = Rnd.Next(0, Selection.Count);
                    // Создается битовая маска, устанавливающая бит в положение, соответствующее выбранному индексу.
                    int bit = 1 << randomIndex;
                    // Проверяется, не был ли уже выбран предмет с данным индексом. 
                    if ((randomParameter & bit) == 0)
                    {
                        // Получаем вес выбранного предмета.
                        int itemWeight = Selection[randomIndex].Weight;
                        // Проверяем, чтобы не добавить предмет, если не хватает места.
                        if (remainingWeight >= itemWeight)
                        {
                            // Устанавливаем бит в параметре генома, отвечающий за выбранный предмет.
                            randomParameter |= bit;
                            // Уменьшается оставшийся вес на вес выбранного предмета.
                            remainingWeight -= itemWeight;
                        }
                    }
                    // Увеличивается счетчик попыток.
                    attempts++;
                }
                // Созданный геном добавляется в список временных геномов.
                temp.Add(new BackpackGenome(randomParameter));
            }
            // Возвращается список созданных геномов.
            return temp;
        }
    }
}
