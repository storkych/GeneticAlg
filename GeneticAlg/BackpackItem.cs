using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlg
{
    /// <summary>
    /// Класс, представляющий предмет в задаче о рюкзаке.
    /// </summary>
    internal class BackpackItem
    {
        /// <summary>
        /// Наименование, вес и ценность.
        /// </summary>
        public string Name;
        public int Weight;
        public int Worth;

        /// <summary>
        /// Конструктор класса, инициализирующий параметры предмета.
        /// </summary>
        /// <param name="weigth"></param>
        /// <param name="worth"></param>
        /// <param name="name"></param>
        public BackpackItem(int weigth, int worth, string name)
        {
            Weight = weigth;
            Worth = worth;
            Name = name;
        }
    }
}
