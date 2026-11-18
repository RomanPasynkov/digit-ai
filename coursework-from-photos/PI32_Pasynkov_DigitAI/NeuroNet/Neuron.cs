using static System.Math;

namespace PI32_Pasynkov_DigitAI.NeuroNet
{
    class Neuron
    {
        // Поля
        private NeuronType type;
        private double[] weights;
        private double[] inputs;
        private double output;
        private double derivative;

        // Константы для функции активации
        private double a = 0.01d;

        // Свойства
        public double[] Weights { get => weights; set => weights = value; }
        public double[] Inputs { get => inputs; set => inputs = value; }
        public double Output { get => output; }
        public double Derivative { get => derivative; }

        // Конструктор
        public Neuron(double[] memoryWeights, NeuronType typeNeuron)
        {
            type = typeNeuron;
            weights = memoryWeights;
        }

        // методы
        public void Activator(double[] i)
        {
            inputs = i;

            // Первый вес используется как смещение (bias).
            double sum = weights[0];

            // Остальные веса соответствуют входным сигналам нейрона.
            for (int j = 0; j < inputs.Length; j++)
            {
                sum += inputs[j] * weights[j + 1];
            }

            switch (type)
            {
                // Для нейронов скрытого слоя
                case NeuronType.Hidden:
                    output = LeakyReLU(sum);
                    derivative = LeakyReLU_Derivactivator(sum);
                    break;

                // Экспонента выходного нейрона — как в исходном учебном варианте.
                case NeuronType.Output:
                    output = Exp(sum);
                    break;
            }
        }

        private double LeakyReLU(double sum)
        {
            return sum < 0d ? a * sum : sum;
        }

        private double LeakyReLU_Derivactivator(double sum)
        {
            return sum < 0d ? a : 1d;
        }
    }
}
