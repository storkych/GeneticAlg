using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlg
{
    /// <summary>
    /// Класс, содержащий функциональность для генетического скрещивания и мутации.
    /// </summary>
    internal class BackpackBreeding
    {
        /// <summary>
        /// Генератор случайных чисел для использования внутри класса.
        /// </summary>
        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Переменные для хранения точек скрещивания и потомков.
        /// </summary>
        private static int _crossPoint;
        private static int _mutatePoint;
        private static int _child1;
        private static int _child2;

        /// <summary>
        /// Генетическое скрещивание.
        /// </summary>
        /// <param name="parent1"></param>
        /// <param name="parent2"></param>
        /// <returns></returns>
        public static int[] Crossover(BackpackGenome<int> parent1, BackpackGenome<int> parent2)
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
        public static int Mutation(BackpackGenome<int> genom)
        {
            _mutatePoint = Rnd.Next(0, 31);
            int temp = (1 << _mutatePoint);
            _child1 = genom.Parameter ^ temp;
            return _child1;
        }
    }
}
