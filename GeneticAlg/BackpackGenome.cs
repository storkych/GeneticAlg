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
    /// <typeparam name="T"></typeparam>
    internal class BackpackGenome<T>
    {
        /// <summary>
        /// Значение приспособленности генома и список выбранных предметов.
        /// </summary>
        public float Fitness;
        public List<BackpackItem> ItemsPicked = new List<BackpackItem>();

        /// <summary>
        /// Параметр генома.
        /// </summary>
        public T Parameter;

        /// <summary>
        /// Конструктор класса, инициализирующий параметры генома.
        /// </summary>
        /// <param name="parameter"></param>
        public BackpackGenome(T parameter)
        {
            Parameter = parameter;
            Fitness = 0;
        }
    }
}
