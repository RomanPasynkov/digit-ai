using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using PI32_Pasynkov_DigitAI;
using PI32_Pasynkov_DigitAI.NeuroNet;

// Checks run in an isolated temporary directory, never against the user's weights.
class NetworkChecks
{
    [STAThread]
    static int Main(string[] args)
    {
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            string root = args[0];
            CheckSamples(Path.Combine(root, "samples"));
            CheckHiddenGradient(Path.Combine(root, "hidden"));
            CheckOutputGradient(Path.Combine(root, "output"));
            CheckLearning(Path.Combine(root, "learning"));
            CheckForm(root);
            Console.WriteLine("All checks passed.");
            return 0;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(error);
            return 1;
        }
    }

    static void CheckSamples(string root)
    {
        Directory.CreateDirectory(root);
        string valid = "2 1 1 1 0 0 1 1 1 1 1 0 0 1 1 1\n";
        File.WriteAllText(Path.Combine(root, "train.txt"), valid.Replace(" ", "  \t"));
        double[,] samples = new InputLayer(NetworkMode.Train, root).Trainset;
        Assert(samples.GetLength(0) == 1 && samples.GetLength(1) == 16, "sample dimensions");
        Assert(samples[0, 0] == 2 && samples[0, 1] == 1, "label and pixels are separate");
        Expect<FileNotFoundException>(() => new InputLayer(NetworkMode.Test, root));
        foreach (string invalid in new[] { "", "2 1 0\n", valid.Replace("2 ", "10 "), valid.Replace("1 ", "NaN ") })
        {
            File.WriteAllText(Path.Combine(root, "test.txt"), invalid);
            Expect<InvalidDataException>(() => new InputLayer(NetworkMode.Test, root));
        }
        File.WriteAllText(Path.Combine(root, "test.txt"), valid.Replace("2 ", "7 "));
        Assert(new InputLayer(NetworkMode.Test, root).Testset[0, 0] == 7, "test reads its own file");
        Console.WriteLine("PASS: sample parsing and invalid/missing files");
    }

    static void CheckHiddenGradient(string root)
    {
        var layer = new InspectHidden(root);
        double[,] weights = { { 0.3, 0.2, -0.1 }, { -0.2, 0.1, 0.2 } };
        double[] input = { 0.4, 0.7 };
        double[] incoming = { 0.6, -0.8 };
        double[,] lastChange = new double[2, 3];
        for (int step = 0; step < 2; step++)
        {
            double[,] gradient = NumericalGradient(weights, w =>
            {
                layer.SetWeights(w);
                return layer.Loss(input, incoming);
            });
            layer.SetWeights(weights);
            double[] expectedSignal = new double[2];
            for (int j = 0; j < 2; j++)
            {
                double old = input[j];
                input[j] = old + 1e-6;
                double plus = layer.Loss(input, incoming);
                input[j] = old - 1e-6;
                double minus = layer.Loss(input, incoming);
                input[j] = old;
                expectedSignal[j] = -(plus - minus) / 2e-6;
            }
            layer.Data = input;
            double[] signal = layer.BackwardBass(incoming);
            for (int j = 0; j < 2; j++) Near(signal[j], expectedSignal[j], 1e-8, "hidden input gradient");
            double[,] actual = layer.Snapshot();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double change = -0.087 * gradient[i, j] + 0.070 * lastChange[i, j];
                    Near(actual[i, j], weights[i, j] + change, 1e-8, "hidden weight/bias and momentum");
                    lastChange[i, j] = change;
                }
            }
            weights = actual;
        }
        Console.WriteLine("PASS: numerical hidden gradients, bias, old-weight propagation and momentum");
    }

    static void CheckOutputGradient(string root)
    {
        var net = new Network(Path.Combine(root, "network"));
        var layer = new InspectOutput(root);
        double[,] weights = new double[10, 3];
        for (int i = 0; i < 10; i++)
        {
            weights[i, 0] = 0.1 * i;
            weights[i, 1] = 0.02 * i;
            weights[i, 2] = -0.01 * i;
        }
        double[] input = { 0.4, 0.7 };
        double[,] gradient = NumericalGradient(weights, w =>
        {
            layer.SetWeights(w);
            layer.Data = input;
            layer.Recognize(net, null);
            return -Math.Log(net.Fact[3]);
        });
        layer.SetWeights(weights);
        layer.Data = input;
        layer.Recognize(net, null);
        double[] errors = net.Fact.Select((p, k) => (k == 3 ? 1d : 0d) - p).ToArray();
        double[] signal = layer.BackwardBass(errors);
        double[,] actual = layer.Snapshot();
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 3; j++)
                Near(actual[i, j], weights[i, j] - 0.087 * gradient[i, j], 1e-8, "output cross-entropy gradient");
        for (int j = 0; j < 2; j++)
            Near(signal[j], Enumerable.Range(0, 10).Sum(i => weights[i, j + 1] * errors[i]), 1e-12, "output old-weight signal");

        layer.SetWeights(weights);
        layer.Data = input;
        layer.Recognize(net, null);
        double[] before = (double[])net.Fact.Clone();
        for (int i = 0; i < 10; i++) weights[i, 0] += 1000;
        layer.SetWeights(weights);
        layer.Data = input;
        layer.Recognize(net, null);
        for (int i = 0; i < 10; i++) Near(net.Fact[i], before[i], 1e-12, "softmax handles large logits");
        Near(net.Fact.Sum(), 1d, 1e-12, "probabilities sum to one");
        Console.WriteLine("PASS: numerical output gradients and stable softmax");
    }

    static void CheckLearning(string root)
    {
        Directory.CreateDirectory(root);
        string[] glyphs = { "111101101101111", "010110010010111", "111001111100111", "111001111001111",
            "101101111001001", "111100111001111", "111100111101111", "111001010100100", "111101111101111", "111101111001111" };
        string[] rows = glyphs.Select((glyph, digit) => digit + " " + string.Join(" ", glyph.ToCharArray())).ToArray();
        File.WriteAllLines(Path.Combine(root, "train.txt"), rows);
        File.WriteAllLines(Path.Combine(root, "test.txt"), rows.Reverse());
        var network = new Network(root);
        double before = network.Test().AverageError;
        EvaluationResult trained = network.Train(2000);
        Console.WriteLine("Learning: loss {0:F6} -> {1:F6}; fitted {2}/{3} glyphs", before,
            trained.AverageError, trained.CorrectCount, trained.SampleCount);
        Assert(trained.AverageError < before / 10, "training decreases loss");
        Assert(trained.CorrectCount == 10, "training fits ten distinct known glyphs");
        double[] pixels = glyphs[4].Select(c => (double)(c - '0')).ToArray();
        network.ForwardPass(pixels);
        double[] predictions = (double[])network.Fact.Clone();
        string[] files = Directory.GetFiles(Path.Combine(root, "memory"));
        string[] content = files.Select(File.ReadAllText).ToArray();
        DateTime[] times = files.Select(File.GetLastWriteTimeUtc).ToArray();
        EvaluationResult tested = network.Test();
        Assert(tested.CorrectCount == 10, "test evaluation");
        network.ForwardPass(pixels);
        for (int i = 0; i < 10; i++) Near(network.Fact[i], predictions[i], 0, "test leaves in-memory weights unchanged");
        for (int i = 0; i < files.Length; i++)
            Assert(File.ReadAllText(files[i]) == content[i] && File.GetLastWriteTimeUtc(files[i]) == times[i], "test never writes weights");
        var restored = new Network(root);
        restored.ForwardPass(pixels);
        for (int i = 0; i < 10; i++) Near(restored.Fact[i], predictions[i], 0, "save/reload round trip");

        File.WriteAllLines(Path.Combine(root, "test.txt"), glyphs.Select((glyph, digit) =>
            ((digit + 1) % 10) + " " + string.Join(" ", glyph.ToCharArray())));
        Assert(network.Test().CorrectCount == 0, "test uses test labels, not train labels");
        using (var cancellation = new CancellationTokenSource())
        {
            cancellation.Cancel();
            Expect<OperationCanceledException>(() => network.Train(100, null, cancellation.Token));
        }
        Console.WriteLine("PASS: learning, test isolation, exact persistence and cancellation");
    }

    static void CheckForm(string root)
    {
        Application.EnableVisualStyles();
        using (var form = new Form1())
        {
            // Create visible child controls for DrawToBitmap without displaying a test window.
            form.ShowInTaskbar = false;
            form.Opacity = 0d;
            form.Show();
            Application.DoEvents();
            // This console harness pumps messages manually instead of using Application.Run.
            SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
            Control.CheckForIllegalCrossThreadCalls = true;
            var label = (NumericUpDown)form.Controls["numericDesiredValue"];
            label.Value = 3;
            Click(form, "buttonSaveTrain");
            Click(form, "buttonSaveTest");
            Assert(File.ReadAllLines(Path.Combine(root, "train.txt")).Length == 1, "train sample button");
            Assert(File.ReadAllLines(Path.Combine(root, "test.txt")).Length == 1, "test sample button");
            ((NumericUpDown)form.Controls["numericEpochs"]).Value = 2;
            Click(form, "buttonTrain");
            FieldInfo running = typeof(Form1).GetField("trainingCancellation", BindingFlags.Instance | BindingFlags.NonPublic);
            var timer = Stopwatch.StartNew();
            while (running.GetValue(form) != null && timer.Elapsed < TimeSpan.FromSeconds(20))
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
            Assert(running.GetValue(form) == null, "background training completes");
            Assert(form.Controls["labelStatus"].Text.Contains("Обучение завершено"), "training button is connected");
            Assert(form.Controls["buttonTrain"].Enabled && form.Controls["buttonSaveTrain"].Enabled
                && form.Controls["numericEpochs"].Enabled && !form.Controls["buttonCancelTraining"].Enabled,
                "controls are restored after training");
            Assert(((ProgressBar)form.Controls["progressTraining"]).Value == 100, "training progress completes");
            Click(form, "buttonSetTest");
            Assert(form.Controls["labelStatus"].Text.Contains("Проверка на test.txt"), "test button is connected");
            form.Refresh();
            Thread.Sleep(300);
            Application.DoEvents();
            using (var bitmap = new Bitmap(form.Width, form.Height))
            {
                form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, form.Size));
                bitmap.Save(Path.Combine(root, "form-preview.png"));
            }
        }
        Console.WriteLine("PASS: sample buttons, asynchronous training, test button and form rendering");
    }

    static void Click(Form form, string name)
    {
        typeof(Button).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(form.Controls[name], new object[] { EventArgs.Empty });
    }

    static double[,] NumericalGradient(double[,] weights, Func<double[,], double> loss)
    {
        double[,] gradient = new double[weights.GetLength(0), weights.GetLength(1)];
        for (int i = 0; i < weights.GetLength(0); i++)
        {
            for (int j = 0; j < weights.GetLength(1); j++)
            {
                double value = weights[i, j];
                weights[i, j] = value + 1e-6;
                double plus = loss(weights);
                weights[i, j] = value - 1e-6;
                double minus = loss(weights);
                weights[i, j] = value;
                gradient[i, j] = (plus - minus) / 2e-6;
            }
        }
        return gradient;
    }

    static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception("FAIL: " + message);
    }

    static void Near(double actual, double expected, double tolerance, string message)
    {
        Assert(!double.IsNaN(actual) && Math.Abs(actual - expected) <= tolerance,
            message + ": " + actual + " vs " + expected);
    }

    static void Expect<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new Exception("Expected " + typeof(T).Name);
    }
}

class InspectHidden : HiddenLayer
{
    public InspectHidden(string directory) : base(2, 2, NeuronType.Hidden, "hidden", directory) { }
    public void SetWeights(double[,] weights)
    {
        for (int i = 0; i < numofneurons; i++)
            for (int j = 0; j <= numofprevneurons; j++) neurons[i].Weights[j] = weights[i, j];
    }
    public double[,] Snapshot()
    {
        return WeightInitialize(MemoryMode.SET, pathFileWeights);
    }
    public double Loss(double[] input, double[] incoming)
    {
        Data = input;
        return -neurons.Select((neuron, i) => neuron.Output * incoming[i]).Sum();
    }
}

class InspectOutput : OutputLayer
{
    public InspectOutput(string directory) : base(10, 2, NeuronType.Output, "output", directory) { }
    public void SetWeights(double[,] weights)
    {
        for (int i = 0; i < numofneurons; i++)
            for (int j = 0; j <= numofprevneurons; j++) neurons[i].Weights[j] = weights[i, j];
    }
    public double[,] Snapshot()
    {
        return WeightInitialize(MemoryMode.SET, pathFileWeights);
    }
}
