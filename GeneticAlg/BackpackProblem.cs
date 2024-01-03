using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlg
{
    /// <summary>
    /// Класс, содержащий функциональность для демонстрации задачи о рюкзаке.
    /// </summary>
    internal class BackpackProblem
    {
        /// <summary>
        /// Список имен предметов и количество имен предметов.
        /// </summary>
        public static List<string> ListOfNames = new List<string>();
        public static int NumberOfNames = 1000;

        /// <summary>
        /// Генератор случайных чисел для использования внутри класса.
        /// </summary>
        static readonly Random Rnd = new Random();

        /// <summary>
        /// Генерация случайных решений для начальной популяции.
        /// </summary>
        /// <param name="populationSize"></param>
        /// <returns></returns>
        public static List<BackpackGenome<int>> GenerateRandomSolutions(int populationSize)
        {
            var temp = new List<BackpackGenome<int>>();
            for (var i = 0; i < populationSize; i++)
            {
                // Создание генома с случайным параметром.
                temp.Add(new BackpackGenome<int>(Rnd.Next(1, Int32.MaxValue))); //TODO
            }
            return temp;
        }

        /// <summary>
        /// Генерация случайных предметов.
        /// </summary>
        public static void GenerateRandomItems()
        {
            // Генерация имен предметов.
            for (var i = 0; i < NumberOfNames; i++)
            {
                ListOfNames.Add("Item_" + Rnd.Next(1, NumberOfNames));
            }

            // Генерация случайных параметров предметов и добавление в коллекцию.
            for (var i = 0; i < 32; i++)
            {
                BackpackExtractItemsFromInt32Value.Selection.Add(new BackpackItem(Rnd.Next(1, 51), Rnd.Next(0, 101), ListOfNames[Rnd.Next(0, NumberOfNames - 1)]));
            }
            // Установка максимального веса для ограничения рюкзака.
            BackpackFitnessFunction.MaxValue = (BackpackExtractItemsFromInt32Value.Selection.Sum(t => t.Weight) / 3);
        }
    }
}
