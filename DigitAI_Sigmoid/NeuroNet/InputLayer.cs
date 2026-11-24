using System;
using System.IO;

namespace DigitAI_Sigmoid.NeuroNet
{
    class InputLayer
    {
        // Поля
        private double[,] trainset;
        private double[,] testset;
        private readonly Random random = new Random();

        // Свойства
        public double[,] Trainset { get => trainset; }
        public double[,] Testset { get => testset; }

        // Конструктор
        public InputLayer(NetworkMode nm)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string[] tmpArrStr;
            string[] tmpStr;

            switch (nm)
            {
                case NetworkMode.Train:
                    tmpArrStr = File.ReadAllLines(path + "train.txt");
                    trainset = new double[tmpArrStr.Length, 16];
                    for (int i = 0; i < tmpArrStr.Length; i++)
                    {
                        tmpStr = tmpArrStr[i].Split(' ');
                        for (int j = 0; j < 16; j++)
                        {
                            trainset[i, j] = double.Parse(tmpStr[j]);
                        }
                    }
                    break;

                case NetworkMode.Test:
                    tmpArrStr = File.ReadAllLines(path + "test.txt");
                    testset = new double[tmpArrStr.Length, 16];
                    for (int i = 0; i < tmpArrStr.Length; i++)
                    {
                        tmpStr = tmpArrStr[i].Split(' ');
                        for (int j = 0; j < 16; j++)
                        {
                            testset[i, j] = double.Parse(tmpStr[j]);
                        }
                    }
                    break;
            }
        }

        public void Shuffling_Array_Rows(double[,] arr)
        {
            int columns = arr.GetLength(1);

            // Алгоритм Фишера — Йетса. Меняем местами строки целиком.
            for (int row = arr.GetLength(0) - 1; row > 0; row--)
            {
                int otherRow = random.Next(row + 1);
                for (int column = 0; column < columns; column++)
                {
                    double value = arr[row, column];
                    arr[row, column] = arr[otherRow, column];
                    arr[otherRow, column] = value;
                }
            }
        }
    }
}
