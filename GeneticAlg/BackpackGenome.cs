using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlg
{
    /// <summary>
    /// Класс, представляющий геном в генетическом алгоритме.
    /// </summary>
    internal class BackpackGenome
    {
        /// <summary>
        /// Значение приспособленности генома и список выбранных предметов.
        /// </summary>
        public float Fitness;
        public List<BackpackItem> ItemsPicked = new();

        /// <summary>
        /// Параметр генома.
        /// </summary>
        public int Parameter;

        /// <summary>
        /// Конструктор класса, инициализирующий параметры генома.
        /// </summary>
        /// <param name="parameter"></param>
        public BackpackGenome(int parameter)
        {
            Parameter = parameter;
            Fitness = 0;
        }
    }
}
