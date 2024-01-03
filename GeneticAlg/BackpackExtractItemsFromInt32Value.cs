using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlg
{
    /// <summary>
    /// Класс, содержащий функциональность для извлечения предметов из числового параметра.
    /// </summary>
    internal class BackpackExtractItemsFromInt32Value
    {
        /// <summary>
        /// Коллекция предметов, выбранных из числового параметра.
        /// </summary>
        public static List<BackpackItem> Selection = new List<BackpackItem>();

        /// <summary>
        ///  Сортировка и извлечение предметов из числового параметра.
        /// </summary>
        /// <param name="genom"></param>
        public static void Sort(BackpackGenome<int> genom)
        {
            int temp = genom.Parameter;

            for (var i = 0; i < 31; i++)
            {
                int check = temp & 1 << i;
                if (check == (1 << i))
                {
                    genom.ItemsPicked.Add(Selection[i]);
                }
            }
        }
    }
}
