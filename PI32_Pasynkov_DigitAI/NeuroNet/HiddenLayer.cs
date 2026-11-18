namespace PI32_Pasynkov_DigitAI.NeuroNet
{
    class HiddenLayer : Layer
    {
        public HiddenLayer(int non, int nonp, NeuronType nt, string type, string memoryDirectory = null) :
            base(non, nonp, nt, type, memoryDirectory)
        { }

        public override void Recognize(Network net, Layer nextLayer)
        {
            double[] hidden_out = new double[numofneurons];
            for (int i = 0; i < numofneurons; i++)
            {
                hidden_out[i] = neurons[i].Output;
            }
            nextLayer.Data = hidden_out;
        }

        // gr_sums — сигнал от следующего слоя, до производной активации этого слоя.
        public override double[] BackwardBass(double[] gr_sums)
        {
            if (gr_sums == null || gr_sums.Length != numofneurons)
                throw new System.ArgumentException("Неверный размер сигнала ошибки скрытого слоя.", nameof(gr_sums));

            double[] deltas = new double[numofneurons];
            for (int i = 0; i < numofneurons; i++)
                deltas[i] = neurons[i].Derivative * gr_sums[i];

            double[] gr_sum = new double[numofprevneurons];
            for (int j = 0; j < numofprevneurons; j++)
            {
                for (int i = 0; i < numofneurons; i++)
                    gr_sum[j] += neurons[i].Weights[j + 1] * deltas[i];
            }

            ApplyDeltas(deltas);
            return gr_sum;
        }
    }
}
