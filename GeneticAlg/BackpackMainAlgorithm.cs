using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlg
{
    /// <summary>
    /// Класс, представляющий главный алгоритм генетического алгоритма.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class BackpackMainAlgorithm<T>
    {
        /// <summary>
        /// Делегат для скрещивания геномов.
        /// </summary>
        /// <param name="parent1"></param>
        /// <param name="parent2"></param>
        /// <returns></returns>
        public delegate T[] Crossover(BackpackGenome<T> parent1, BackpackGenome<T> parent2);

        /// <summary>
        /// Делегат для генерации первого поколения геномов.
        /// </summary>
        /// <param name="genom"></param>
        public delegate void FirstGeneration(BackpackGenome<T> genom);

        /// <summary>
        /// Делегат для вычисления приспособленности генома.
        /// </summary>
        /// <param name="genom"></param>
        public delegate void FitnessValue(BackpackGenome<T> genom);

        /// <summary>
        /// Делегат для мутации генома.
        /// </summary>
        public delegate void GenerateRandomItems();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="populationSize"></param>
        /// <returns></returns>
        public delegate List<BackpackGenome<T>> GenerateRandomSolutions(int populationSize);

        /// <summary>
        /// Делегат для мутации генома.
        /// </summary>
        /// <param name="genom"></param>
        /// <returns></returns>
        public delegate T Mutation(BackpackGenome<T> genom);

        /// <summary>
        /// Список геномов следующего поколения, список текущих геномов.
        /// </summary>
        private readonly List<BackpackGenome<T>> NextGeneration = new List<BackpackGenome<T>>();
        private readonly List<BackpackGenome<T>> Solutions = new List<BackpackGenome<T>>();
        private readonly Crossover _crossover;
        private readonly FirstGeneration _firstGeneration;
        private readonly FitnessValue _fitnessValue;
        private readonly Mutation _mutation;
        private readonly Random rnd = new Random();

        /// <summary>
        /// Вероятность скрещивания, количество поколений, вероятность мутации и размер популяции.
        /// </summary>
        public double CROSSOVERPROBABILITY;
        public int GENERATIONCOUNT;
        public double MUTATIONPROBABILITY;
        public int POPULATIONSIZE;

        /// <summary>
        /// Лучший геном среди всех поколений.
        /// </summary>
        private BackpackGenome<T> _bestFitness;

        /// <summary>
        /// Массив для хранения всех результатов скрещивания.
        /// </summary>
        private T[] _crossedBackpackGenomes = new T[2];
        /// <summary>
        /// Геном-партнер для скрещивания.
        /// </summary>
        private BackpackGenome<T> _crossoverPartner;

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="crossoverProbability"></param>
        /// <param name="mutationProbability"></param>
        /// <param name="populationSize"></param>
        /// <param name="generationCount"></param>
        /// <param name="firstGeneration"></param>
        /// <param name="fitnessValue"></param>
        /// <param name="crossover"></param>
        /// <param name="mutation"></param>
        public BackpackMainAlgorithm(double crossoverProbability, double mutationProbability, int populationSize,
                    int generationCount, FirstGeneration firstGeneration, FitnessValue fitnessValue, Crossover crossover,
                    Mutation mutation)
        {
            CROSSOVERPROBABILITY = crossoverProbability;
            MUTATIONPROBABILITY = mutationProbability;
            POPULATIONSIZE = populationSize;
            GENERATIONCOUNT = generationCount;
            _firstGeneration = firstGeneration;
            _fitnessValue = fitnessValue;
            _crossover = crossover;
            _mutation = mutation;
        }

        /// <summary>
        /// Метод для эволюции генетического алгоритма.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public BackpackGenome<T> Evolve(List<BackpackGenome<T>> items)
        {
            Solutions.AddRange(items);

            for (var i = 0; i < GENERATIONCOUNT; i++)
            {
                for (var k = 0; k < POPULATIONSIZE; k++)
                {
                    _firstGeneration.Invoke(Solutions[k]);
                    _fitnessValue.Invoke(Solutions[k]);
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
                            if (Solutions[m].Fitness >= minimalFitness && rnd.NextDouble() <= CROSSOVERPROBABILITY)
                            {
                                if (_crossoverPartner == null)
                                {
                                    _crossoverPartner = Solutions[m];
                                }
                                else
                                {
                                    _crossedBackpackGenomes = _crossover.Invoke(_crossoverPartner, Solutions[m]);

                                    NextGeneration.Add(new BackpackGenome<T>(_crossedBackpackGenomes[0]));
                                    NextGeneration.Add(new BackpackGenome<T>(_crossedBackpackGenomes[1]));
                                    _crossoverPartner = null;
                                }
                            }

                            if (rnd.NextDouble() <= MUTATIONPROBABILITY)
                            {
                                NextGeneration.Add(new BackpackGenome<T>(_mutation.Invoke(Solutions[m])));
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
    }
}

