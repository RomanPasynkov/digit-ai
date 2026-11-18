using System;
using System.IO;

namespace PI32_Pasynkov_DigitAI.NeuroNet
{
    class InputLayer
    {
        // Поля
        private double[,] trainset;
        private double[,] testset;

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

                // По подписи одногруппницы: тот же код для TEST,
                // с заменой trainset на testset и train.txt на test.txt.
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
    }
}
