using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PI32_Pasynkov_DigitAI.NeuroNet;
using System.Windows.Forms;
using System.IO;

namespace PI32_Pasynkov_DigitAI
{
    public partial class Form1 : Form
    {
        double[] inputPixels; // Массив входных данных
        private Network network;
        private CancellationTokenSource trainingCancellation;
        private bool closeAfterTraining;
        public Form1()
        {
            InitializeComponent();

            inputPixels = new double[15];
            network = new Network();
            foreach (Button button in Controls.OfType<Button>().Where(b => b.TabIndex < 15))
            {
                button.UseVisualStyleBackColor = false;
                button.BackColor = Color.White;
            }
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
            SaveSample("train.txt");
        }

        private void SaveTestSample_Click(object sender, EventArgs e)
        {
            SaveSample("test.txt");
        }

        private void SaveSample(string fileName)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                string row = numericDesiredValue.Value.ToString(CultureInfo.InvariantCulture) + " "
                    + string.Join(" ", inputPixels.Select(p => p.ToString("R", CultureInfo.InvariantCulture)));
                File.AppendAllText(path, row + Environment.NewLine);
                labelStatus.Text = "Пример цифры " + numericDesiredValue.Value + " сохранён:\n" + path;
            }
            catch (Exception error)
            {
                ShowError(error);
            }
        }

        private async void Train_Click(object sender, EventArgs e)
        {
            int epochs = (int)numericEpochs.Value;
            trainingCancellation = new CancellationTokenSource();
            SetTrainingState(true);
            labelStatus.Text = "Загрузка train.txt...";
            var progress = new Progress<TrainingProgress>(p =>
            {
                if (trainingCancellation == null) return;
                progressTraining.Value = (int)(100d * p.Epoch / p.TotalEpochs);
                labelStatus.Text = "Эпоха " + p.Epoch + " из " + p.TotalEpochs
                    + "\nСредняя ошибка: " + p.AverageError.ToString("0.0000");
            });

            try
            {
                CancellationToken token = trainingCancellation.Token;
                EvaluationResult result = await Task.Run(() => network.Train(epochs, progress, token));
                labelStatus.Text = "Обучение завершено. Веса сохранены.\n"
                    + FormatResult(result) + "\nЭто результат на обучающей выборке; для проверки нажмите «Тест».";
            }
            catch (OperationCanceledException)
            {
                labelStatus.Text = "Обучение остановлено. Текущие веса сохранены.";
            }
            catch (Exception error)
            {
                ShowError(error);
            }
            finally
            {
                trainingCancellation.Dispose();
                trainingCancellation = null;
                SetTrainingState(false);
                if (closeAfterTraining) Close();
            }
        }

        private void Test_Click(object sender, EventArgs e)
        {
            try
            {
                EvaluationResult result = network.Test();
                labelStatus.Text = "Проверка на test.txt:\n" + FormatResult(result) + "\nВеса не изменялись.";
            }
            catch (Exception error)
            {
                ShowError(error);
            }
        }

        private void CancelTraining_Click(object sender, EventArgs e)
        {
            if (trainingCancellation != null)
            {
                trainingCancellation.Cancel();
                buttonCancelTraining.Enabled = false;
            }
        }

        private void SetTrainingState(bool busy)
        {
            foreach (Button button in Controls.OfType<Button>())
                button.Enabled = !busy;
            buttonCancelTraining.Enabled = busy;
            numericDesiredValue.Enabled = !busy;
            numericEpochs.Enabled = !busy;
            if (busy) progressTraining.Value = 0;
        }

        private static string FormatResult(EvaluationResult result)
        {
            return "Верно: " + result.CorrectCount + " из " + result.SampleCount
                + " (" + result.Accuracy.ToString("0.00") + " %)\nСредняя ошибка: "
                + result.AverageError.ToString("0.0000");
        }

        private void ShowError(Exception error)
        {
            labelStatus.Text = error.Message;
            MessageBox.Show(this, error.Message, "Не удалось выполнить действие",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (trainingCancellation != null)
            {
                e.Cancel = true;
                closeAfterTraining = true;
                trainingCancellation.Cancel();
            }
            base.OnFormClosing(e);
        }

        private void Button_Recogn_Click(object sender, EventArgs e)
        {
            try
            {
                network.ForwardPass(inputPixels);
                Label_Output.Text = network.Fact.ToList().IndexOf(network.Fact.Max()).ToString();
                Label_Probability.Text = (100 * network.Fact.Max()).ToString("0.00") + " %";
            }
            catch (Exception error)
            {
                ShowError(error);
            }
        }
    }
}
