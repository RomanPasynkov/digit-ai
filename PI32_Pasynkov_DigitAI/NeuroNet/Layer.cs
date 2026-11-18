using System;
using System.IO;
using System.Globalization;

namespace PI32_Pasynkov_DigitAI.NeuroNet
{
    abstract class Layer
    {
        // Поля
        protected string name_Layer;
        private readonly string pathDirWeights;
        protected readonly string pathFileWeights;
        protected int numofneurons;
        protected int numofprevneurons;
        protected const double learningrate = 0.087d;
        protected const double momentum = 0.070d;
        protected double[,] lastdeltaweights;
        protected Neuron[] neurons;

        // Свойства
        public double[] Data
        {
            set
            {
                if (value == null || value.Length != numofprevneurons)
                    throw new ArgumentException("Неверное количество входных сигналов слоя.", nameof(value));

                for (int i = 0; i < numofneurons; i++)
                {
                    neurons[i].Activator(value);
                }
            }
        }

        // Конструктор
        protected Layer(int non, int nonp, NeuronType nt, string nm_Layer, string memoryDirectory = null)
        {
            if (non <= 0) throw new ArgumentOutOfRangeException(nameof(non));
            if (nonp <= 0) throw new ArgumentOutOfRangeException(nameof(nonp));
            if (string.IsNullOrWhiteSpace(nm_Layer)) throw new ArgumentException("Имя слоя не задано.", nameof(nm_Layer));

            numofneurons = non;
            numofprevneurons = nonp;
            neurons = new Neuron[non];
            name_Layer = nm_Layer;
            pathDirWeights = memoryDirectory ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "memory");
            pathFileWeights = Path.Combine(pathDirWeights, name_Layer + "_memory.csv");
            lastdeltaweights = new double[non, nonp + 1];

            double[,] weights;
            if (File.Exists(pathFileWeights))
            {
                weights = WeightInitialize(MemoryMode.GET, pathFileWeights);
            }
            else
            {
                Directory.CreateDirectory(pathDirWeights);
                weights = WeightInitialize(MemoryMode.INIT, pathFileWeights);
            }

            for (int i = 0; i < non; i++)
            {
                double[] neuronWeights = new double[nonp + 1];
                for (int j = 0; j < nonp + 1; j++)
                    neuronWeights[j] = weights[i, j];

                neurons[i] = new Neuron(neuronWeights, nt);
            }
        }

        // GET загружает веса, INIT создаёт и сохраняет их, SET сохраняет текущие веса нейронов.
        public double[,] WeightInitialize(MemoryMode mode, string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("Путь к весам не задан.", nameof(path));

            switch (mode)
            {
                case MemoryMode.GET:
                    return ReadWeights(path);

                case MemoryMode.INIT:
                    double[,] initialWeights = new double[numofneurons, numofprevneurons + 1];
                    lock (random)
                    {
                        for (int i = 0; i < numofneurons; i++)
                        {
                            for (int j = 0; j <= numofprevneurons; j++)
                                initialWeights[i, j] = 0.02 * random.NextDouble() - 0.01;
                        }
                    }
                    WriteWeights(path, initialWeights);
                    return initialWeights;

                case MemoryMode.SET:
                    double[,] currentWeights = new double[numofneurons, numofprevneurons + 1];
                    for (int i = 0; i < numofneurons; i++)
                    {
                        for (int j = 0; j <= numofprevneurons; j++)
                            currentWeights[i, j] = neurons[i].Weights[j];
                    }
                    WriteWeights(path, currentWeights);
                    return currentWeights;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        private static readonly Random random = new Random();

        private double[,] ReadWeights(string path)
        {
            string[] rows = File.ReadAllLines(path);
            if (rows.Length != numofneurons)
                throw new InvalidDataException("Число строк в файле весов не совпадает с числом нейронов слоя: " + path);

            double[,] weights = new double[numofneurons, numofprevneurons + 1];
            for (int i = 0; i < numofneurons; i++)
            {
                // Основной разделитель — ';'. Запятую поддерживаем для ранее созданных файлов.
                bool semicolonSeparated = rows[i].Contains(";");
                string[] cells = rows[i].Split(semicolonSeparated ? ';' : ',');
                if (cells.Length != numofprevneurons + 1)
                    throw new InvalidDataException("Число весов в строке " + (i + 1) + " неверно: " + path);

                for (int j = 0; j <= numofprevneurons; j++)
                {
                    double weight;
                    string cell = semicolonSeparated ? cells[j].Replace(',', '.') : cells[j];
                    if (!double.TryParse(cell, NumberStyles.Float, CultureInfo.InvariantCulture, out weight)
                        || double.IsNaN(weight) || double.IsInfinity(weight))
                        throw new InvalidDataException("Некорректный вес в строке " + (i + 1) + ": " + path);
                    weights[i, j] = weight;
                }
            }
            return weights;
        }

        private void WriteWeights(string path, double[,] weights)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

            using (StreamWriter writer = new StreamWriter(path, false))
            {
                for (int i = 0; i < numofneurons; i++)
                {
                    string[] cells = new string[numofprevneurons + 1];
                    for (int j = 0; j <= numofprevneurons; j++)
                        cells[j] = weights[i, j].ToString("R", CultureInfo.InvariantCulture);
                    writer.WriteLine(string.Join(";", cells));
                }
            }
        }

        public void SaveWeights()
        {
            WeightInitialize(MemoryMode.SET, pathFileWeights);
        }

        public void ResetMomentum()
        {
            Array.Clear(lastdeltaweights, 0, lastdeltaweights.Length);
        }

        // Сначала вычисляем сигнал для предыдущего слоя по старым весам,
        // затем применяем поправки. Нулевой вес — смещение, его вход равен 1.
        protected void ApplyDeltas(double[] deltas)
        {
            for (int i = 0; i < numofneurons; i++)
            {
                for (int n = 0; n <= numofprevneurons; n++)
                {
                    double input = n == 0 ? 1d : neurons[i].Inputs[n - 1];
                    double deltaWeight = momentum * lastdeltaweights[i, n]
                        + learningrate * input * deltas[i];
                    lastdeltaweights[i, n] = deltaWeight;
                    neurons[i].Weights[n] += deltaWeight;
                }
            }
        }

        abstract public void Recognize(Network net, Layer nextLayer);
        abstract public double[] BackwardBass(double[] stuff);
    }
}
