using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlg
{
    internal class QueenGenAlg
    {

        private Action<string> generationCallback;

        /// <summary>
        /// Количество ферзей на шахматной доске.
        /// </summary>
        static int NumQueens;

        /// <summary>
        /// Размер популяции.
        /// </summary>
        const int popSize = 500;

        /// <summary>
        /// Вероятность мутации (в данном случае, 10%).
        /// </summary>
        const int mutation = 10;

        /// <summary>
        /// Массив, содержащий особи (доски) в популяции.
        /// </summary>
        static QueenBoards[] population = new QueenBoards[popSize];


        /// <summary>
        /// Реализует одноточечное скрещивание двух родителей (parentX и parentY), чтобы создать потомка.
        /// </summary>
        /// <param name="parentX">Первый родитель.</param>
        /// <param name="parentY">Второй родитель.</param>
        /// <returns>Ребенок в результате скрещивания.</returns>
        public QueenBoards Crossover(QueenBoards parentX, QueenBoards parentY)
        {
            QueenBoards child;
            Random r = new();
            //Выбираем случайную точку пересечения и создаём потомка, комбинируя части родительских досок.
            int crossoverPoint = r.Next(1, NumQueens - 1);
            //Получаем обе части массива от родителей, затем создаём один дочерний элемент.
            //LINQ: Take() извлекает определенное число элементов(crossoverPoint). 
            //ToArray() берет входную последовательность с элементами типа T и возвращает массив элементов типа Т.
            int[] firstHalf = parentX.GetBoard().Take(crossoverPoint).ToArray();
            //LINQ: Skip() пропускает определенное количество элементов.
            int[] secondHalf = parentY.GetBoard().Skip(crossoverPoint).Take(NumQueens - crossoverPoint).ToArray();
            //LINQ: Concat() соединяет две входные последовательности и выдает одну выходную последовательность
            int[] childArray = firstHalf.Concat(secondHalf).ToArray();
            child = new QueenBoards(childArray);
            return child;
        }

        /// <summary>
        /// Создает начальную популяцию случайных досок размером popSize. 
        /// Случайная популяция состоит из расстановки ферзей, которые не бьют друг друга по столбцам и строкам.
        /// </summary>
        public void CreatePopulation()
        {
            int[] initParent = new int[NumQueens];
            Random r = new();
            // Заполнение популяции случайными первоначальными родителями.
            for (var i = 0; i < popSize; i++)
            {
                for (var j = 0; j < initParent.Length; j++)
                {
                    initParent[j] = r.Next(1, NumQueens + 1);
                    while (true)
                    {
                        initParent[j] = r.Next(1, NumQueens + 1);
                        int k;
                        // Проверка уникальности(без столкновений для строк ).
                        for (k = 0; k <= j; k++)
                        {
                            if (initParent[k] == initParent[j])
                            {
                                //Если столкновение => выходим из цикла.
                                break;
                            }
                        }
                        // Если столкновений нет => выходим из цикла.
                        if (k == j)
                        {
                            break;
                        }
                    }
                }
                population[i] = new QueenBoards(initParent);
            }
        }

        /// <summary>
        ///  Выбирает случайным образом родителя из совокупности по уровню пригодности.
        /// </summary>
        /// <returns>Родитель</returns>
        public QueenBoards ChooseParent()
        {
            Random r = new();
            // Общая пригодность.
            var total = 0;
            // Получаем текущую общую пригодность.
            for (var i = 0; i < population.Length; i++)
            {
                total += population[i].Fitness;
            }
            int random = r.Next(0, total);
            // Выбираем случайного родителя с более высоким уровнем пригодности.
            for (var i = 0; i < population.Length; i++)
            {
                if (random < population[i].Fitness)
                {
                    return population[i];
                }
                random -= population[i].Fitness;
            }
            return null;
        }

        public QueenGenAlg(Action<string> generationCallback)
        {
            this.generationCallback = generationCallback;
        }



        /// <summary>
        /// Выполняет генетический алгоритм.
        /// </summary>
        /// <returns>Дочерний элемент, содержащий допустимое решение.</returns>
        public QueenBoards QueenGeneticAlg(int tarNumQeens)
        {
            NumQueens = tarNumQeens;
            //Дочерний элемент, содержащий допустимое решение.
            QueenBoards child;
            Random r = new();
            // Временная популяция размерностью popSize.
            QueenBoards[] tempPopulation = new QueenBoards[popSize];
            // Лучшее значение функции пригодности. Нужно для проверки пригодности потомка.
            var highestFitness = 0;
            // Подсчёт поколений.
            var generation = 0;
            // Создаём начальную популяцию.
            CreatePopulation();
            while (true)
            {
                //Счётчик поколений .
                generation++;
                // Создаём новые поколения особей, пока не будет найдено решение.
                for (var i = 0; i < popSize; i++)
                {
                    // CВыбираем двух родителей и создаём ребенка. Пока выбор любой.
                    child = Crossover(ChooseParent(), ChooseParent());
                    // Проверяем, является ли child решением.
                    if (child.Solved())
                    {
                        generationCallback?.Invoke($"Функция пригодности: {child.Fitness} Поколение: {generation}");
                        return child;
                    }
                    // Изменяем мутацию.
                    if (mutation > r.Next(0, 100))
                    {
                        child.Mutate();
                    }
                    // Проверяем пригодность ребенка.
                    if (child.Fitness > highestFitness)
                    {
                        highestFitness = child.Fitness;
                    }
                    tempPopulation[i] = child;
                }
                population = tempPopulation;
                generationCallback?.Invoke($"Функция пригодности: {highestFitness} Поколение: {generation}");
            }
        }
    }
}

