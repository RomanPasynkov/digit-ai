namespace PI32_Pasynkov_DigitAI.NeuroNet
{
    class Network
    {
        private InputLayer input_layer = null;
        private HiddenLayer hidden_layer1 = new HiddenLayer(70, 15, NeuronType.Hidden, nameof(hidden_layer1));
        private HiddenLayer hidden_layer2 = new HiddenLayer(37, 70, NeuronType.Hidden, nameof(hidden_layer2));
        private OutputLayer output_layer = new OutputLayer(10, 37, NeuronType.Output, nameof(output_layer));

        private double[] fact = new double[10];
        private double[] e_error_avr;

        public double[] Fact { get => fact; }
        public double[] E_error_var { get => e_error_avr; set => e_error_avr = value; }

        public Network() { }

        // Существовавший прямой проход. Цикла обучения на фотографиях нет.
        public void ForwardPass(double[] netInput)
        {
            hidden_layer1.Data = netInput;
            hidden_layer1.Recognize(null, hidden_layer2);
            hidden_layer2.Recognize(null, output_layer);
            output_layer.Recognize(this, null);
        }
    }
}
