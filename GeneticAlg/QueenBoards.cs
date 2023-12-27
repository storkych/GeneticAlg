using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlg
{
    internal class QueenBoards
    {
        /// <summary>
        /// Расстановка.
        /// </summary>
        public int[] Board { get; set; }

        /// <summary>
        /// Функция пригодности. Максимальное количество диагональных столкновений - 28.
        /// </summary>
        public int Fitness { get; set; }

        /// <summary>
        /// Размер доски.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Конструктор досок.
        /// </summary>
        /// <param name="Board">Расстановка фигур на доске.</param>
        public QueenBoards(int[] Board)
        {
            this.Board = Board;
            Fitness = CalculateFitness();
            Size = Board.Length;
        }

        /// <summary>
        /// Определяет пригодность доски.
        /// </summary>
        /// <returns>Пригодность доски.</returns>
        public int CalculateFitness()
        {
            var Fitness = 0;
            /* Проверяем каждого ферзя на доске,
             * и если он не атакует другого ферзя, 
             * увеличиваем пригодность доски на единицу.
            */
            for (var i = 0; i < Board.Length; i++)
            {
                for (var j = i + 1; j < Board.Length; j++)
                {
                    // Проверяем атаки диагоналей и строк. Если никто никого не бьёт, то увеличиваем функцию пригодности.
                    if ((Math.Abs(Board[i] - Board[j]) != Math.Abs(j - i)) && (Board[i] != Board[j]))
                    {
                        Fitness++;
                    }
                }
            }
            return Fitness;
        }

        /// <summary>
        /// Определяет , является ли доска решением.
        /// </summary>
        /// <returns>True - если решение, false - если не является решением (есть столкновения).</returns>
        public bool Solved()
        {
            for (var i = 0; i < Board.Length; i++)
            {
                for (var j = i + 1; j < Board.Length; j++)
                {
                    // Проверяем атаки диагоналей и строк. Если столкновения, то решение не подходит.
                    if ((Math.Abs(Board[i] - Board[j]) == Math.Abs(j - i)) || (Board[i] == Board[j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Выполняет мутацию на одной королеве.
        /// </summary>
        public void Mutate()
        {
            Random r = new();
            // Выбираем любую позицию (столбец).
            int randomPosition = r.Next(0, Size);
            // Выбираем любую королеву (расположение на строке).
            int randomQueenValue = r.Next(1, Size + 1);
            Board[randomPosition] = randomQueenValue;
        }

        /// <summary>
        /// Возвращает доску в виде массива.
        /// </summary>
        /// <returns>Копия расстановки доски в виде массива.</returns>
        public int[] GetBoard()
        {
            return (int[])Board.Clone();
        }

        /// <summary>
        /// Возвращает строковое представление доски.
        /// </summary>
        /// <returns>Строковое представление доски.</returns>
        public override string ToString()
        {
            string str = string.Empty;

            for (int i = 0; i < Board.Length; i++)
            {
                str += Board[i] + " ";
            }

            return str;
        }

        /// <summary>
        /// Печать решения.
        /// </summary>
        public void PrintBoard()
        {
            string str = string.Empty;
            for (int i = 0; i < Board.Length; i++)
            {
                for (int j = 0; j < Board.Length; j++)
                {
                    if (Board[j] == i + 1)
                    {
                        str += "Q ";
                    }
                    else
                    {
                        str += ". ";
                    }
                }
                str += "\n\n";
            }
            Console.Write(str);
        }
    }
}
