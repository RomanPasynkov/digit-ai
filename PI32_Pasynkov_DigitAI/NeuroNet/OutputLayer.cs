namespace PI32_Pasynkov_DigitAI.NeuroNet
{
    class OutputLayer : Layer
    {
        public OutputLayer(int non, int nonp, NeuronType nt, string type, string memoryDirectory = null) :
            base(non, nonp, nt, type, memoryDirectory)
        { }

        public override void Recognize(Network net, Layer nextLayer)
        {
            // Устойчивый softmax: вычитание максимума не меняет вероятности.
            double max = neurons[0].Output;
            for (int i = 1; i < neurons.Length; i++)
                max = System.Math.Max(max, neurons[i].Output);

            double e_sum = 0d;
            for (int i = 0; i < neurons.Length; i++)
            {
                net.Fact[i] = System.Math.Exp(neurons[i].Output - max);
                e_sum += net.Fact[i];
            }

            for (int i = 0; i < neurons.Length; i++)
                net.Fact[i] /= e_sum;
        }

        // Для softmax с перекрёстной энтропией errors = правильный ответ - вероятность.
        public override double[] BackwardBass(double[] errors)
        {
            if (errors == null || errors.Length != numofneurons)
                throw new System.ArgumentException("Неверный размер ошибки выходного слоя.", nameof(errors));

            double[] gr_sum = new double[numofprevneurons];
            for (int j = 0; j < numofprevneurons; j++)
            {
                for (int i = 0; i < numofneurons; i++)
                    gr_sum[j] += neurons[i].Weights[j + 1] * errors[i];
            }

            ApplyDeltas(errors);
            return gr_sum;
        }
    }
}
