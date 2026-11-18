namespace PI32_Pasynkov_DigitAI.NeuroNet
{
    class HiddenLayer : Layer
    {
        public HiddenLayer(int non, int nonp, NeuronType nt, string type) :
            base(non, nonp, nt, type)
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

        // Обратный проход: видимая часть фотографии.
        public override double[] BackwardPass(double[] gr_sums)
        {
            double[] gr_sum = new double[numofprevneurons];

            // На фото тело этого цикла свёрнуто. Следующая заглушка —
            // НЕ код преподавателя: она явно отмечает недостающий фрагмент.
            for (int j = 0; j < numofprevneurons; j++)
            {
                MissingPhotoGradient();
            }

            // Ниже — показанный на фотографии цикл коррекции весов.
            for (int i = 0; i < numofneurons; i++)
                for (int n = 0; n < numofprevneurons + 1; n++)
                {
                    double deltaw;
                    if (n == 0)
                        deltaw = momentum * lastdeltaweights[i, 0] +
                            learningrate * neurons[i].Derivative * gr_sums[i];
                    else
                        deltaw = momentum * lastdeltaweights[i, n] +
                            learningrate * neurons[i].Inputs[n - 1] * neurons[i].Derivative * gr_sums[i];

                    lastdeltaweights[i, n] = deltaw;
                    neurons[i].Weights[n] += deltaw;
                }
            return gr_sum;
        }

        // Только техническая заглушка для отсутствующего на фото фрагмента.
        private static void MissingPhotoGradient()
        {
            throw new System.NotImplementedException(
                "На фотографии свёрнут расчёт градиентной суммы скрытого слоя. Нужен исходный фрагмент с занятия.");
        }
    }
}
