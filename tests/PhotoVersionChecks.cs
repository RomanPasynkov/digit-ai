using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using PI32_Pasynkov_DigitAI;
using PI32_Pasynkov_DigitAI.NeuroNet;

// Runs against the photo-only source copy in a temporary output directory.
// The intentionally missing hidden gradient must remain explicit, not guessed.
class PhotoVersionChecks
{
    [STAThread]
    static int Main()
    {
        try
        {
            var train = new InputLayer(NetworkMode.Train).Trainset;
            var test = new InputLayer(NetworkMode.Test).Testset;
            Assert(train.GetLength(0) == 100 && train.GetLength(1) == 16, "training data shape");
            Assert(test.GetLength(0) == 10 && test.GetLength(1) == 16, "test data shape");
            for (int digit = 0; digit < 10; digit++)
            {
                Assert(test[digit, 0] == digit, "test labels");
                for (int sample = 0; sample < 10; sample++)
                    Assert(train[digit * 10 + sample, 0] == digit, "train labels");
            }
            Console.WriteLine("PASS: photo InputLayer reads train and test separately");

            var network = new Network();
            network.ForwardPass(new double[15]);
            Near(network.Fact.Sum(), 1d, "original forward normalization");
            Assert(network.Fact.All(p => !double.IsNaN(p) && p >= 0 && p <= 1), "finite probabilities");
            Assert(typeof(Network).GetMethod("Train") == null && typeof(Network).GetMethod("Test") == null,
                "no invented complete training/testing workflow");

            var output = new InspectPhotoOutput();
            double[,] weights = { { 0.2, 0.3, -0.1 }, { -0.1, 0.5, 0.4 } };
            output.SetWeights(weights);
            output.Data = new[] { 1d, 0d };
            Near(output.FirstOutput, Math.Exp(0.5), "output neuron returns exp(sum)");
            double[] errors = { 0.4, -0.4 };
            double[] propagated = output.BackwardPass(errors);
            Near(propagated[0], 0.3 * 0.4 + 0.5 * -0.4, "old weights propagate output error");
            Near(propagated[1], -0.1 * 0.4 + 0.4 * -0.4, "second propagated component");
            double[,] actual = output.Snapshot();
            for (int i = 0; i < 2; i++)
            {
                Near(actual[i, 0], weights[i, 0] + 0.087 * errors[i], "output bias correction");
                Near(actual[i, 1], weights[i, 1] + 0.087 * errors[i], "active input correction");
                Near(actual[i, 2], weights[i, 2], "zero input weight unchanged");
            }

            var hidden = new InspectPhotoHidden();
            hidden.Data = new[] { 1d, 0d };
            double before = hidden.FirstWeight;
            bool missingReported = false;
            try { hidden.BackwardPass(new[] { 1d, 1d }); }
            catch (NotImplementedException error) { missingReported = error.Message.Contains("фотографии"); }
            Assert(missingReported, "missing folded code is explicit");
            Near(hidden.FirstWeight, before, "missing hidden code does not silently train");
            Console.WriteLine("PASS: visible output backward code and explicit missing hidden fragment");

            Application.EnableVisualStyles();
            using (var form = new Form1())
            {
                Assert(form.Controls["numericEpochs"] == null && form.Controls["buttonSaveTest"] == null,
                    "original coursework interface");
                int oldCount = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "train.txt")).Length;
                ((NumericUpDown)form.Controls["numericDesiredValue"]).Value = 2;
                typeof(Button).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(form.Controls["buttonTrain"], new object[] { EventArgs.Empty });
                Assert(File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "train.txt")).Length == oldCount + 1,
                    "old train button only appends one example");
                form.ShowInTaskbar = false;
                form.Opacity = 0d;
                form.Show();
                Application.DoEvents();
                using (var bitmap = new Bitmap(form.Width, form.Height))
                {
                    form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, form.Size));
                    bitmap.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "form-preview.png"));
                }
            }
            Console.WriteLine("PASS: original form loads and its sample-save button works");
            Console.WriteLine("Photo version checks passed; hidden backward remains intentionally incomplete.");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception("FAIL: " + message);
    }

    static void Near(double actual, double expected, string message)
    {
        Assert(!double.IsNaN(actual) && Math.Abs(actual - expected) < 1e-12, message);
    }
}

class InspectPhotoOutput : OutputLayer
{
    public InspectPhotoOutput() : base(2, 2, NeuronType.Output, "check_output") { }
    public double FirstOutput { get => neurons[0].Output; }
    public void SetWeights(double[,] weights)
    {
        for (int i = 0; i < numofneurons; i++)
            for (int j = 0; j <= numofprevneurons; j++) neurons[i].Weights[j] = weights[i, j];
    }
    public double[,] Snapshot()
    {
        double[,] weights = new double[numofneurons, numofprevneurons + 1];
        for (int i = 0; i < numofneurons; i++)
            for (int j = 0; j <= numofprevneurons; j++) weights[i, j] = neurons[i].Weights[j];
        return weights;
    }
}

class InspectPhotoHidden : HiddenLayer
{
    public InspectPhotoHidden() : base(2, 2, NeuronType.Hidden, "check_hidden") { }
    public double FirstWeight { get => neurons[0].Weights[0]; }
}
