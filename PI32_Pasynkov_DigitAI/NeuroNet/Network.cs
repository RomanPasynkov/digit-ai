using System;
using System.IO;
using System.Threading;

namespace PI32_Pasynkov_DigitAI.NeuroNet
{
    class Network
    {
        private InputLayer input_layer;
        private readonly HiddenLayer hidden_layer1;
        private readonly HiddenLayer hidden_layer2;
        private readonly OutputLayer output_layer;
        private readonly string dataDirectory;
        private readonly Random random = new Random();
        private readonly double[] fact = new double[10];
        private double[] e_error_avr = new double[0];

        public double[] Fact { get => fact; }
        public double[] E_error_var { get => e_error_avr; set => e_error_avr = value; }

        public Network(string dataDirectory = null)
        {
            this.dataDirectory = dataDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
            string memoryDirectory = Path.Combine(this.dataDirectory, "memory");
            hidden_layer1 = new HiddenLayer(70, 15, NeuronType.Hidden, nameof(hidden_layer1), memoryDirectory);
            hidden_layer2 = new HiddenLayer(37, 70, NeuronType.Hidden, nameof(hidden_layer2), memoryDirectory);
            output_layer = new OutputLayer(10, 37, NeuronType.Output, nameof(output_layer), memoryDirectory);
        }

        public void ForwardPass(double[] netInput)
        {
            hidden_layer1.Data = netInput;
            hidden_layer1.Recognize(null, hidden_layer2);
            hidden_layer2.Recognize(null, output_layer);
            output_layer.Recognize(this, null);
            for (int i = 0; i < fact.Length; i++)
            {
                if (double.IsNaN(fact[i]) || double.IsInfinity(fact[i]))
                    throw new InvalidOperationException("Сеть получила некорректные значения. Проверьте веса и данные.");
            }
        }

        public EvaluationResult Train(int epochs, IProgress<TrainingProgress> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (epochs <= 0) throw new ArgumentOutOfRangeException(nameof(epochs));
            input_layer = new InputLayer(NetworkMode.Train, dataDirectory);
            double[,] samples = input_layer.Trainset;
            int count = samples.GetLength(0);
            int[] order = new int[count];
            for (int i = 0; i < count; i++) order[i] = i;

            hidden_layer1.ResetMomentum();
            hidden_layer2.ResetMomentum();
            output_layer.ResetMomentum();
            e_error_avr = new double[epochs];
            double[] pixels = new double[15];
            double[] errors = new double[10];

            try
            {
                for (int epoch = 0; epoch < epochs; epoch++)
                {
                    // Перемешиваем порядок примеров на каждой эпохе.
                    for (int i = count - 1; i > 0; i--)
                    {
                        int j = random.Next(i + 1);
                        int temp = order[i];
                        order[i] = order[j];
                        order[j] = temp;
                    }

                    double loss = 0d;
                    for (int i = 0; i < count; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        int row = order[i];
                        int digit = (int)samples[row, 0];
                        CopyPixels(samples, row, pixels);
                        ForwardPass(pixels);
                        loss -= Math.Log(Math.Max(fact[digit], 1e-15));

                        // Знак согласован с прибавлением поправки к весу в ApplyDeltas.
                        for (int k = 0; k < errors.Length; k++)
                            errors[k] = (k == digit ? 1d : 0d) - fact[k];

                        double[] gr_sums = output_layer.BackwardBass(errors);
                        gr_sums = hidden_layer2.BackwardBass(gr_sums);
                        hidden_layer1.BackwardBass(gr_sums);
                    }

                    e_error_avr[epoch] = loss / count;
                    if (progress != null && ((epoch + 1) % Math.Max(1, epochs / 100) == 0 || epoch == epochs - 1))
                        progress.Report(new TrainingProgress(epoch + 1, epochs, e_error_avr[epoch]));
                }
            }
            catch (OperationCanceledException)
            {
                SaveWeights();
                throw;
            }

            EvaluationResult result = Evaluate(samples);
            SaveWeights();
            return result;
        }

        public EvaluationResult Test()
        {
            input_layer = new InputLayer(NetworkMode.Test, dataDirectory);
            // Только прямой проход: тестовые примеры не участвуют в обучении.
            return Evaluate(input_layer.Testset);
        }

        public void SaveWeights()
        {
            hidden_layer1.SaveWeights();
            hidden_layer2.SaveWeights();
            output_layer.SaveWeights();
        }

        private EvaluationResult Evaluate(double[,] samples)
        {
            int count = samples.GetLength(0);
            int correct = 0;
            double loss = 0d;
            double[] pixels = new double[15];
            for (int i = 0; i < count; i++)
            {
                CopyPixels(samples, i, pixels);
                ForwardPass(pixels);
                int prediction = 0;
                for (int k = 1; k < fact.Length; k++)
                {
                    if (fact[k] > fact[prediction]) prediction = k;
                }
                int digit = (int)samples[i, 0];
                if (prediction == digit) correct++;
                loss -= Math.Log(Math.Max(fact[digit], 1e-15));
            }
            return new EvaluationResult(count, correct, loss / count);
        }

        private static void CopyPixels(double[,] samples, int row, double[] pixels)
        {
            for (int j = 0; j < pixels.Length; j++)
                pixels[j] = samples[row, j + 1];
        }
    }

    class TrainingProgress
    {
        public int Epoch { get; private set; }
        public int TotalEpochs { get; private set; }
        public double AverageError { get; private set; }

        public TrainingProgress(int epoch, int totalEpochs, double averageError)
        {
            Epoch = epoch;
            TotalEpochs = totalEpochs;
            AverageError = averageError;
        }
    }

    class EvaluationResult
    {
        public int SampleCount { get; private set; }
        public int CorrectCount { get; private set; }
        public double AverageError { get; private set; }
        public double Accuracy { get => 100d * CorrectCount / SampleCount; }

        public EvaluationResult(int sampleCount, int correctCount, double averageError)
        {
            SampleCount = sampleCount;
            CorrectCount = correctCount;
            AverageError = averageError;
        }
    }
}
