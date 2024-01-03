using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlg
{
    /// <summary>
    /// Класс, содержащий функционал для вычисления приспособленности генома (Fitness Function).
    /// </summary>
    internal class BackpackFitnessFunction
    {
        /// <summary>
        /// Максимальное значение для ограничения веса рюкзака.
        /// </summary>
        public static int MaxValue;

        /// <summary>
        /// Метод для вычисления приспособленности генома.
        /// </summary>
        /// <param name="genom"></param>
        public static void CalculateFitness(BackpackGenome<int> genom)
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
    }
}
