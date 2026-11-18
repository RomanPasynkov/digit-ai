
namespace PI32_Pasynkov_DigitAI
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.button2 = new System.Windows.Forms.Button();
            this.Button_Recogn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.numericDesiredValue = new System.Windows.Forms.NumericUpDown();
            this.buttonTrain = new System.Windows.Forms.Button();
            this.buttonSetTest = new System.Windows.Forms.Button();
            this.Label_Output = new System.Windows.Forms.Label();
            this.Label_Probability = new System.Windows.Forms.Label();
            this.buttonSaveTrain = new System.Windows.Forms.Button();
            this.buttonSaveTest = new System.Windows.Forms.Button();
            this.buttonCancelTraining = new System.Windows.Forms.Button();
            this.numericEpochs = new System.Windows.Forms.NumericUpDown();
            this.labelEpochs = new System.Windows.Forms.Label();
            this.labelDesiredValue = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.progressTraining = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.numericDesiredValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEpochs)).BeginInit();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(79, 97);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(35, 35);
            this.button2.TabIndex = 1;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // Button_Recogn
            // 
            this.Button_Recogn.Location = new System.Drawing.Point(249, 138);
            this.Button_Recogn.Name = "Button_Recogn";
            this.Button_Recogn.Size = new System.Drawing.Size(100, 60);
            this.Button_Recogn.TabIndex = 15;
            this.Button_Recogn.Text = "Распознать";
            this.Button_Recogn.UseVisualStyleBackColor = true;
            this.Button_Recogn.Click += new System.EventHandler(this.Button_Recogn_Click);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(38, 97);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(35, 35);
            this.button1.TabIndex = 0;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(120, 97);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(35, 35);
            this.button3.TabIndex = 2;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // button4
            // 
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(38, 138);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(35, 35);
            this.button4.TabIndex = 3;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // button5
            // 
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Location = new System.Drawing.Point(79, 138);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(35, 35);
            this.button5.TabIndex = 4;
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // button6
            // 
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Location = new System.Drawing.Point(120, 138);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(35, 35);
            this.button6.TabIndex = 5;
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // button7
            // 
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Location = new System.Drawing.Point(38, 179);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(35, 35);
            this.button7.TabIndex = 6;
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // button8
            // 
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Location = new System.Drawing.Point(79, 179);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(35, 35);
            this.button8.TabIndex = 7;
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // button9
            // 
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button9.Location = new System.Drawing.Point(120, 179);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(35, 35);
            this.button9.TabIndex = 8;
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // button10
            // 
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button10.Location = new System.Drawing.Point(38, 220);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(35, 35);
            this.button10.TabIndex = 9;
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // button11
            // 
            this.button11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button11.Location = new System.Drawing.Point(79, 220);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(35, 35);
            this.button11.TabIndex = 10;
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // button12
            // 
            this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button12.Location = new System.Drawing.Point(120, 220);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(35, 35);
            this.button12.TabIndex = 11;
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // button13
            // 
            this.button13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button13.Location = new System.Drawing.Point(38, 261);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(35, 35);
            this.button13.TabIndex = 12;
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // button14
            // 
            this.button14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button14.Location = new System.Drawing.Point(79, 261);
            this.button14.Name = "button14";
            this.button14.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.button14.Size = new System.Drawing.Size(35, 35);
            this.button14.TabIndex = 13;
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // button15
            // 
            this.button15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button15.Location = new System.Drawing.Point(120, 261);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(35, 35);
            this.button15.TabIndex = 14;
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.Change_Status_Button);
            // 
            // numericDesiredValue
            // 
            this.numericDesiredValue.Location = new System.Drawing.Point(38, 330);
            this.numericDesiredValue.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numericDesiredValue.Name = "numericDesiredValue";
            this.numericDesiredValue.Size = new System.Drawing.Size(46, 20);
            this.numericDesiredValue.TabIndex = 16;
            // 
            // buttonTrain
            // 
            this.buttonTrain.Location = new System.Drawing.Point(400, 138);
            this.buttonTrain.Name = "buttonTrain";
            this.buttonTrain.Size = new System.Drawing.Size(150, 30);
            this.buttonTrain.TabIndex = 17;
            this.buttonTrain.Text = "Обучить";
            this.buttonTrain.UseVisualStyleBackColor = true;
            this.buttonTrain.Click += new System.EventHandler(this.Train_Click);
            // 
            // buttonSetTest
            // 
            this.buttonSetTest.Location = new System.Drawing.Point(400, 178);
            this.buttonSetTest.Name = "buttonSetTest";
            this.buttonSetTest.Size = new System.Drawing.Size(150, 30);
            this.buttonSetTest.TabIndex = 18;
            this.buttonSetTest.Text = "Тест";
            this.buttonSetTest.UseVisualStyleBackColor = true;
            this.buttonSetTest.Click += new System.EventHandler(this.Test_Click);
            // 
            // Label_Output
            // 
            this.Label_Output.AutoSize = true;
            this.Label_Output.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label_Output.Location = new System.Drawing.Point(28, 20);
            this.Label_Output.Name = "Label_Output";
            this.Label_Output.Size = new System.Drawing.Size(101, 55);
            this.Label_Output.TabIndex = 19;
            this.Label_Output.Text = "Out";
            // 
            // Label_Probability
            // 
            this.Label_Probability.AutoSize = true;
            this.Label_Probability.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Label_Probability.Location = new System.Drawing.Point(151, 35);
            this.Label_Probability.Name = "Label_Probability";
            this.Label_Probability.Size = new System.Drawing.Size(198, 37);
            this.Label_Probability.TabIndex = 20;
            this.Label_Probability.Text = "Вероятность";
            // 
            // buttonSaveTrain
            //
            this.buttonSaveTrain.Location = new System.Drawing.Point(38, 378);
            this.buttonSaveTrain.Name = "buttonSaveTrain";
            this.buttonSaveTrain.Size = new System.Drawing.Size(200, 25);
            this.buttonSaveTrain.TabIndex = 21;
            this.buttonSaveTrain.Text = "Добавить в обучающую выборку";
            this.buttonSaveTrain.UseVisualStyleBackColor = true;
            this.buttonSaveTrain.Click += new System.EventHandler(this.SaveTrainSample_Click);
            //
            // buttonSaveTest
            //
            this.buttonSaveTest.Location = new System.Drawing.Point(38, 413);
            this.buttonSaveTest.Name = "buttonSaveTest";
            this.buttonSaveTest.Size = new System.Drawing.Size(200, 25);
            this.buttonSaveTest.TabIndex = 22;
            this.buttonSaveTest.Text = "Добавить в тестовую выборку";
            this.buttonSaveTest.UseVisualStyleBackColor = true;
            this.buttonSaveTest.Click += new System.EventHandler(this.SaveTestSample_Click);
            //
            // buttonCancelTraining
            //
            this.buttonCancelTraining.Location = new System.Drawing.Point(565, 138);
            this.buttonCancelTraining.Name = "buttonCancelTraining";
            this.buttonCancelTraining.Size = new System.Drawing.Size(150, 30);
            this.buttonCancelTraining.TabIndex = 23;
            this.buttonCancelTraining.Text = "Остановить";
            this.buttonCancelTraining.Enabled = false;
            this.buttonCancelTraining.UseVisualStyleBackColor = true;
            this.buttonCancelTraining.Click += new System.EventHandler(this.CancelTraining_Click);
            //
            // numericEpochs
            //
            this.numericEpochs.Location = new System.Drawing.Point(565, 105);
            this.numericEpochs.Name = "numericEpochs";
            this.numericEpochs.Minimum = 1;
            this.numericEpochs.Maximum = 100000;
            this.numericEpochs.Value = 1000;
            this.numericEpochs.Size = new System.Drawing.Size(100, 20);
            this.numericEpochs.TabIndex = 24;
            //
            // labelEpochs
            //
            this.labelEpochs.Location = new System.Drawing.Point(400, 107);
            this.labelEpochs.Name = "labelEpochs";
            this.labelEpochs.Size = new System.Drawing.Size(155, 20);
            this.labelEpochs.Text = "Число эпох обучения:";
            //
            // labelDesiredValue
            //
            this.labelDesiredValue.Location = new System.Drawing.Point(95, 332);
            this.labelDesiredValue.Name = "labelDesiredValue";
            this.labelDesiredValue.Size = new System.Drawing.Size(150, 20);
            this.labelDesiredValue.Text = "Правильная цифра";
            //
            // labelStatus
            //
            this.labelStatus.Location = new System.Drawing.Point(400, 255);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(360, 210);
            this.labelStatus.Text = "Нарисуйте цифру, укажите правильный ответ и добавьте пример в нужную выборку. Затем запустите обучение или тестирование.";
            //
            // progressTraining
            //
            this.progressTraining.Location = new System.Drawing.Point(400, 220);
            this.progressTraining.Name = "progressTraining";
            this.progressTraining.Size = new System.Drawing.Size(315, 20);
            //
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(799, 496);
            this.Controls.Add(this.buttonSaveTrain);
            this.Controls.Add(this.buttonSaveTest);
            this.Controls.Add(this.buttonCancelTraining);
            this.Controls.Add(this.numericEpochs);
            this.Controls.Add(this.labelEpochs);
            this.Controls.Add(this.labelDesiredValue);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.progressTraining);
            this.Controls.Add(this.Label_Probability);
            this.Controls.Add(this.Label_Output);
            this.Controls.Add(this.buttonSetTest);
            this.Controls.Add(this.buttonTrain);
            this.Controls.Add(this.numericDesiredValue);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Button_Recogn);
            this.Controls.Add(this.button2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form1";
            this.Text = "Распознавание цифр";
            ((System.ComponentModel.ISupportInitialize)(this.numericDesiredValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEpochs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button Button_Recogn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.NumericUpDown numericDesiredValue;
        private System.Windows.Forms.Button buttonTrain;
        private System.Windows.Forms.Button buttonSetTest;
        private System.Windows.Forms.Label Label_Output;
        private System.Windows.Forms.Label Label_Probability;
        private System.Windows.Forms.Button buttonSaveTrain;
        private System.Windows.Forms.Button buttonSaveTest;
        private System.Windows.Forms.Button buttonCancelTraining;
        private System.Windows.Forms.NumericUpDown numericEpochs;
        private System.Windows.Forms.Label labelEpochs;
        private System.Windows.Forms.Label labelDesiredValue;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ProgressBar progressTraining;
    }
}

