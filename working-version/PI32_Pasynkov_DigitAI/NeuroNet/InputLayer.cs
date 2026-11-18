using System;
using System.Globalization;
using System.IO;

namespace PI32_Pasynkov_DigitAI.NeuroNet
{
    class InputLayer
    {
        private double[,] trainset = new double[0, 16];
        private double[,] testset = new double[0, 16];

        public double[,] Trainset { get => trainset; }
        public double[,] Testset { get => testset; }

        public InputLayer(NetworkMode mode, string dataDirectory = null)
        {
            string directory = dataDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
            switch (mode)
            {
                case NetworkMode.Train:
                    trainset = ReadSamples(Path.Combine(directory, "train.txt"));
                    break;
                case NetworkMode.Test:
                    testset = ReadSamples(Path.Combine(directory, "test.txt"));
                    break;
                case NetworkMode.Demo:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        private static double[,] ReadSamples(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Сначала сохраните примеры в " + path, path);

            string[] rows = File.ReadAllLines(path);
            if (rows.Length == 0)
                throw new InvalidDataException("Выборка пуста: " + path);

            // В строке: правильная цифра, затем 15 пикселей (0 или 1). Смещения здесь нет.
            double[,] samples = new double[rows.Length, 16];
            for (int i = 0; i < rows.Length; i++)
            {
                string[] cells = rows[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (cells.Length != 16)
                    throw new InvalidDataException("В строке " + (i + 1) + " должны быть цифра и 15 пикселей: " + path);

                int digit;
                if (!int.TryParse(cells[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out digit)
                    || digit < 0 || digit > 9)
                    throw new InvalidDataException("В строке " + (i + 1) + " ожидается цифра от 0 до 9: " + path);
                samples[i, 0] = digit;

                for (int j = 1; j < 16; j++)
                {
                    double pixel;
                    if (!double.TryParse(cells[j], NumberStyles.Float, CultureInfo.InvariantCulture, out pixel)
                        || (pixel != 0d && pixel != 1d))
                        throw new InvalidDataException("Пиксели в строке " + (i + 1) + " должны быть 0 или 1: " + path);
                    samples[i, j] = pixel;
                }
            }
            return samples;
        }
    }
}
