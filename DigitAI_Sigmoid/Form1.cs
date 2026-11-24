using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitAI_Sigmoid.NeuroNet;
using System.Windows.Forms;
using System.IO;

namespace DigitAI_Sigmoid
{
    public partial class Form1 : Form
    {
        double[] inputPixels; // Массив входных данных
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

        private void SaveTrainSample_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "train.txt";
            string tmpStr = numericDesiredValue.Value.ToString();

            for(int i = 0; i< inputPixels.Length; i++)
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
