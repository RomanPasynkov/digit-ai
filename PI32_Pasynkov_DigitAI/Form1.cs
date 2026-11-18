using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PI32_Pasynkov_DigitAI.NeuroNet;

namespace PI32_Pasynkov_DigitAI
{
    public partial class Form1 : Form
    {
        double[] inputPixels;
        private Network network;

        public Form1()
        {
            InitializeComponent();
            inputPixels = new double[15];
            network = new Network();
        }

        private void Change_Status_Button(object sender, EventArgs e)
        {
            Button pixelButton = (Button)sender;
            if (pixelButton.BackColor != Color.Black)
            {
                inputPixels[pixelButton.TabIndex] = 1d;
                pixelButton.BackColor = Color.Black;
            }
            else
            {
                inputPixels[pixelButton.TabIndex] = 0d;
                pixelButton.BackColor = Color.White;
            }
        }

        // Старый обработчик: кнопка «Обучить» только сохраняет пример.
        // Полноценного запуска обучения на предоставленных фотографиях нет.
        private void SaveTrainSample_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "train.txt";
            string tmpStr = numericDesiredValue.Value.ToString();
            for (int i = 0; i < inputPixels.Length; i++)
            {
                tmpStr += " " + inputPixels[i].ToString();
            }
            tmpStr += '\n';
            File.AppendAllText(path, tmpStr);
        }

        private void Button_Recogn_Click(object sender, EventArgs e)
        {
            network.ForwardPass(inputPixels);
            Label_Output.Text = network.Fact.ToList().IndexOf(network.Fact.Max()).ToString();
            Label_Probability.Text = (100 * network.Fact.Max()).ToString("0.00") + " %";
        }
    }
}
