// AForge Framework
// Perceptron Classifier
//
// Copyright ?Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Threading;
using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using AForge.Controls;

namespace Classifier
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class MainForm : Form
    {
        #region controls

        private GroupBox groupBox1;
        private ListView dataList;
        private Button loadButton;
        private OpenFileDialog openFileDialog;
        private Chart chart;
        private GroupBox groupBox2;
        private Label label1;
        private TextBox learningRateBox;
        private Button startButton;
        private Label noVisualizationLabel;
        private Label label2;
        private Label label3;
        private ListView weightsList;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private Label label4;
        private TextBox iterationsBox;
        private Button stopButton;
        private Label label5;
        private Chart errorChart;
        private CheckBox saveFilesCheck;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        #endregion

        private int samples = 0;
        private int variables = 0;
        private double[,] data = null;
        private int[] classes = null;

        private double learningRate = 0.1;
        private bool saveStatisticsToFiles = false;

        private Thread workerThread = null;
        private bool needToStop = false;

        // Constructor
        public MainForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // initialize charts
            this.chart.AddDataSeries("class1", Color.Red, Chart.SeriesType.Dots, 5);
            this.chart.AddDataSeries("class2", Color.Blue, Chart.SeriesType.Dots, 5);
            this.chart.AddDataSeries("classifier", Color.Gray, Chart.SeriesType.Line, 1, false);

            this.errorChart.AddDataSeries("error", Color.Red, Chart.SeriesType.ConnectedDots, 3, false);

            // update some controls
            this.saveFilesCheck.Checked = this.saveStatisticsToFiles;
            UpdateSettings();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.components != null)
                {
                    this.components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chart = new AForge.Controls.Chart();
            this.loadButton = new System.Windows.Forms.Button();
            this.dataList = new System.Windows.Forms.ListView();
            this.noVisualizationLabel = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.saveFilesCheck = new System.Windows.Forms.CheckBox();
            this.errorChart = new AForge.Controls.Chart();
            this.label5 = new System.Windows.Forms.Label();
            this.stopButton = new System.Windows.Forms.Button();
            this.iterationsBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.weightsList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.learningRateBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chart);
            this.groupBox1.Controls.Add(this.loadButton);
            this.groupBox1.Controls.Add(this.dataList);
            this.groupBox1.Controls.Add(this.noVisualizationLabel);
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(190, 420);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data";
            // 
            // chart
            // 
            this.chart.Location = new System.Drawing.Point(10, 215);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(170, 170);
            this.chart.TabIndex = 2;
            this.chart.Text = "chart1";
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(10, 390);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 23);
            this.loadButton.TabIndex = 1;
            this.loadButton.Text = "&Load";
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // dataList
            // 
            this.dataList.FullRowSelect = true;
            this.dataList.GridLines = true;
            this.dataList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.dataList.Location = new System.Drawing.Point(10, 20);
            this.dataList.Name = "dataList";
            this.dataList.Size = new System.Drawing.Size(170, 190);
            this.dataList.TabIndex = 0;
            this.dataList.UseCompatibleStateImageBehavior = false;
            this.dataList.View = System.Windows.Forms.View.Details;
            // 
            // noVisualizationLabel
            // 
            this.noVisualizationLabel.Location = new System.Drawing.Point(10, 215);
            this.noVisualizationLabel.Name = "noVisualizationLabel";
            this.noVisualizationLabel.Size = new System.Drawing.Size(170, 170);
            this.noVisualizationLabel.TabIndex = 2;
            this.noVisualizationLabel.Text = "Visualization is not available.";
            this.noVisualizationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.noVisualizationLabel.Visible = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
            this.openFileDialog.Title = "Select data file";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.saveFilesCheck);
            this.groupBox2.Controls.Add(this.errorChart);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.stopButton);
            this.groupBox2.Controls.Add(this.iterationsBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.weightsList);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.startButton);
            this.groupBox2.Controls.Add(this.learningRateBox);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(210, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(240, 420);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Training";
            // 
            // saveFilesCheck
            // 
            this.saveFilesCheck.Location = new System.Drawing.Point(10, 80);
            this.saveFilesCheck.Name = "saveFilesCheck";
            this.saveFilesCheck.Size = new System.Drawing.Size(182, 16);
            this.saveFilesCheck.TabIndex = 11;
            this.saveFilesCheck.Text = "Save weights and errors to files";
            // 
            // errorChart
            // 
            this.errorChart.Location = new System.Drawing.Point(10, 270);
            this.errorChart.Name = "errorChart";
            this.errorChart.Size = new System.Drawing.Size(220, 140);
            this.errorChart.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(10, 250);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "Error\'s dynamics:";
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(155, 49);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 8;
            this.stopButton.Text = "S&top";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // iterationsBox
            // 
            this.iterationsBox.Location = new System.Drawing.Point(90, 50);
            this.iterationsBox.Name = "iterationsBox";
            this.iterationsBox.ReadOnly = true;
            this.iterationsBox.Size = new System.Drawing.Size(50, 20);
            this.iterationsBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(10, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Iterations:";
            // 
            // weightsList
            // 
            this.weightsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.weightsList.FullRowSelect = true;
            this.weightsList.GridLines = true;
            this.weightsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.weightsList.Location = new System.Drawing.Point(10, 130);
            this.weightsList.Name = "weightsList";
            this.weightsList.Size = new System.Drawing.Size(220, 110);
            this.weightsList.TabIndex = 5;
            this.weightsList.UseCompatibleStateImageBehavior = false;
            this.weightsList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Weight";
            this.columnHeader1.Width = 70;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 100;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(10, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Perceptron weights:";
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(10, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(220, 2);
            this.label2.TabIndex = 3;
            // 
            // startButton
            // 
            this.startButton.Enabled = false;
            this.startButton.Location = new System.Drawing.Point(155, 19);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 2;
            this.startButton.Text = "&Start";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // learningRateBox
            // 
            this.learningRateBox.Location = new System.Drawing.Point(90, 20);
            this.learningRateBox.Name = "learningRateBox";
            this.learningRateBox.Size = new System.Drawing.Size(50, 20);
            this.learningRateBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Learning rate:";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(459, 440);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Perceptron Classifier";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new MainForm());
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        // On main form closing
        private void MainForm_Closing(object sender, CancelEventArgs e)
        {
            // check if worker thread is running
            if ((this.workerThread != null) && (this.workerThread.IsAlive))
            {
                this.needToStop = true;
                this.workerThread.Join();
            }
        }

        // On "Load" button click - load data
        private void loadButton_Click(object sender, EventArgs e)
        {
            // data file format:
            // X1, X2, ... Xn, class (0|1)

            // show file selection dialog
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = null;

                // temp buffers (for 50 samples only)
                double[,] tempData = null;
                var tempClasses = new int[50];

                // min and max X values
                var minX = double.MaxValue;
                var maxX = double.MinValue;

                // samples count
                this.samples = 0;

                try
                {
                    string str = null;

                    // open selected file
                    reader = File.OpenText(this.openFileDialog.FileName);

                    // read the data
                    while ((this.samples < 50) && ((str = reader.ReadLine()) != null))
                    {
                        // split the string
                        var strs = str.Split(';');
                        if (strs.Length == 1)
                            strs = str.Split(',');

                        // allocate data array
                        if (this.samples == 0)
                        {
                            this.variables = strs.Length - 1;
                            tempData = new double[50, this.variables];
                        }

                        // parse data
                        for (var j = 0; j < this.variables; j++)
                        {
                            tempData[this.samples, j] = double.Parse(strs[j]);
                        }
                        tempClasses[this.samples] = int.Parse(strs[this.variables]);

                        // search for min value
                        if (tempData[this.samples, 0] < minX)
                            minX = tempData[this.samples, 0];
                        // search for max value
                        if (tempData[this.samples, 0] > maxX)
                            maxX = tempData[this.samples, 0];

                        this.samples++;
                    }

                    // allocate and set data
                    this.data = new double[this.samples, this.variables];
                    Array.Copy(tempData, 0, this.data, 0, this.samples*this.variables);
                    this.classes = new int[this.samples];
                    Array.Copy(tempClasses, 0, this.classes, 0, this.samples);

                    // clear current result
                    ClearCurrentSolution();
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed reading the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    // close file
                    if (reader != null)
                        reader.Close();
                }

                // update list and chart
                UpdateDataListView();

                // show chart or not
                var showChart = (this.variables == 2);

                if (showChart)
                {
                    this.chart.RangeX = new DoubleRange(minX, maxX);
                    ShowTrainingData();
                }

                this.chart.Visible = showChart;
                this.noVisualizationLabel.Visible = !showChart;

                // enable start button
                this.startButton.Enabled = true;
            }
        }

        // Update settings controls
        private void UpdateSettings()
        {
            this.learningRateBox.Text = this.learningRate.ToString();
        }

        // Update data in list view
        private void UpdateDataListView()
        {
            // remove all curent data and columns
            this.dataList.Items.Clear();
            this.dataList.Columns.Clear();

            // add columns
            for (int i = 0, n = this.variables; i < n; i++)
            {
                this.dataList.Columns.Add(string.Format("X{0}", i + 1),
                    50, HorizontalAlignment.Left);
            }
            this.dataList.Columns.Add("Class", 50, HorizontalAlignment.Left);

            // add items
            for (var i = 0; i < this.samples; i++)
            {
                this.dataList.Items.Add(this.data[i, 0].ToString());

                for (var j = 1; j < this.variables; j++)
                {
                    this.dataList.Items[i].SubItems.Add(this.data[i, j].ToString());
                }
                this.dataList.Items[i].SubItems.Add(this.classes[i].ToString());
            }
        }

        // Show training data on chart
        private void ShowTrainingData()
        {
            var class1Size = 0;
            var class2Size = 0;

            // calculate number of samples in each class
            for (int i = 0, n = this.samples; i < n; i++)
            {
                if (this.classes[i] == 0)
                    class1Size++;
                else
                    class2Size++;
            }

            // allocate classes arrays
            var class1 = new double[class1Size, 2];
            var class2 = new double[class2Size, 2];

            // fill classes arrays
            for (int i = 0, c1 = 0, c2 = 0; i < this.samples; i++)
            {
                if (this.classes[i] == 0)
                {
                    // class 1
                    class1[c1, 0] = this.data[i, 0];
                    class1[c1, 1] = this.data[i, 1];
                    c1++;
                }
                else
                {
                    // class 2
                    class2[c2, 0] = this.data[i, 0];
                    class2[c2, 1] = this.data[i, 1];
                    c2++;
                }
            }

            // updata chart control
            this.chart.UpdateDataSeries("class1", class1);
            this.chart.UpdateDataSeries("class2", class2);
        }

        // Enable/disale controls
        private void EnableControls(bool enable)
        {
            this.learningRateBox.Enabled = enable;
            this.loadButton.Enabled = enable;
            this.startButton.Enabled = enable;
            this.saveFilesCheck.Enabled = enable;
            this.stopButton.Enabled = !enable;
        }

        // Clear current solution
        private void ClearCurrentSolution()
        {
            this.chart.UpdateDataSeries("classifier", null);
            this.errorChart.UpdateDataSeries("error", null);
            this.weightsList.Items.Clear();
        }

        // On button "Start" - start learning procedure
        private void startButton_Click(object sender, EventArgs e)
        {
            // get learning rate
            try
            {
                this.learningRate = Math.Max(0.00001, Math.Min(1, double.Parse(this.learningRateBox.Text)));
            }
            catch
            {
                this.learningRate = 0.1;
            }
            this.saveStatisticsToFiles = this.saveFilesCheck.Checked;

            // update settings controls
            UpdateSettings();

            // disable all settings controls
            EnableControls(false);

            // run worker thread
            this.needToStop = false;
            this.workerThread = new Thread(new ThreadStart(SearchSolution));
            this.workerThread.Start();
        }

        // On button "Stop" - stop learning procedure
        private void stopButton_Click(object sender, EventArgs e)
        {
            // stop worker thread
            this.needToStop = true;
            this.workerThread.Join();
            this.workerThread = null;
        }

        // Worker thread
        void SearchSolution()
        {
            // prepare learning data
            var input = new double[this.samples][];
            var output = new double[this.samples][];

            for (var i = 0; i < this.samples; i++)
            {
                input[i] = new double[this.variables];
                output[i] = new double[1];

                // copy input
                for (var j = 0; j < this.variables; j++)
                    input[i][j] = this.data[i, j];
                // copy output
                output[i][0] = this.classes[i];
            }

            // create perceptron
            var network = new ActivationNetwork(new ThresholdFunction(), this.variables, 1);
            var neuron = network[0][0];
            // create teacher
            var teacher = new PerceptronLearning(network);
            // set learning rate
            teacher.LearningRate = this.learningRate;

            // iterations
            var iteration = 1;

            // statistic files
            StreamWriter errorsFile = null;
            StreamWriter weightsFile = null;

            try
            {
                // check if we need to save statistics to files
                if (this.saveStatisticsToFiles)
                {
                    // open files
                    errorsFile = File.CreateText("errors.csv");
                    weightsFile = File.CreateText("weights.csv");
                }

                // erros list
                var errorsList = new ArrayList();

                // loop
                while (!this.needToStop)
                {
                    // save current weights
                    if (weightsFile != null)
                    {
                        for (var i = 0; i < this.variables; i++)
                        {
                            weightsFile.Write(neuron[i] + ";");
                        }
                        weightsFile.WriteLine(neuron.Threshold);
                    }

                    // run epoch of learning procedure
                    var error = teacher.RunEpoch(input, output);
                    errorsList.Add(error);

                    // show current iteration
                    this.iterationsBox.Text = iteration.ToString();

                    // save current error
                    if (errorsFile != null)
                    {
                        errorsFile.WriteLine(error);
                    }

                    // show classifier in the case of 2 dimensional data
                    if ((neuron.InputsCount == 2) && (neuron[1] != 0))
                    {
                        var k = -neuron[0]/neuron[1];
                        var b = -neuron.Threshold/neuron[1];

                        var classifier = new double[2, 2]
                        {
                            {this.chart.RangeX.Min, this.chart.RangeX.Min*k + b},
                            {this.chart.RangeX.Max, this.chart.RangeX.Max*k + b}
                        };
                        // update chart
                        this.chart.UpdateDataSeries("classifier", classifier);
                    }

                    // stop if no error
                    if (error == 0)
                        break;

                    iteration++;
                }

                // show perceptron's weights
                this.weightsList.Items.Clear();
                for (var i = 0; i < this.variables; i++)
                {
                    this.weightsList.Items.Add(string.Format("Weight {0}", i + 1));
                    this.weightsList.Items[i].SubItems.Add(neuron[i].ToString("F6"));
                }
                this.weightsList.Items.Add("Threshold");
                this.weightsList.Items[this.variables].SubItems.Add(neuron.Threshold.ToString("F6"));

                // show error's dynamics
                var errors = new double[errorsList.Count, 2];

                for (int i = 0, n = errorsList.Count; i < n; i++)
                {
                    errors[i, 0] = i;
                    errors[i, 1] = (double) errorsList[i];
                }

                this.errorChart.RangeX = new DoubleRange(0, errorsList.Count - 1);
                this.errorChart.RangeY = new DoubleRange(0, this.samples);
                this.errorChart.UpdateDataSeries("error", errors);
            }
            catch (IOException)
            {
                MessageBox.Show("Failed writing file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // close files
                if (errorsFile != null)
                    errorsFile.Close();
                if (weightsFile != null)
                    weightsFile.Close();
            }

            // enable settings controls
            EnableControls(true);
        }
    }
}
