// AForge Framework
// Time Series Prediction using Multi-Layer Neural Network
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
using System.Threading;
using System.IO;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;
using AForge.Controls;

namespace TimeSeries
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : Form
	{
		private GroupBox groupBox1;
		private ListView dataList;
		private ColumnHeader yColumnHeader;
		private ColumnHeader estimatedYColumnHeader;
		private Button loadDataButton;
		private GroupBox groupBox2;
		private Chart chart;
		private OpenFileDialog openFileDialog;
		private GroupBox groupBox3;
		private TextBox momentumBox;
		private Label label6;
		private TextBox alphaBox;
		private Label label2;
		private TextBox learningRateBox;
		private Label label1;
		private Label label10;
		private TextBox iterationsBox;
		private Label label9;
		private Label label8;
		private TextBox predictionSizeBox;
		private Label label7;
		private TextBox windowSizeBox;
		private Label label3;
		private Label label5;
		private Button stopButton;
		private Button startButton;
		private GroupBox groupBox4;
		private TextBox currentPredictionErrorBox;
		private Label label13;
		private TextBox currentLearningErrorBox;
		private Label label12;
		private TextBox currentIterationBox;
		private Label label11;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private double[]	data = null;
		private double[,]	dataToShow = null;

		private double		learningRate = 0.1;
		private double		momentum = 0.0;
		private double		sigmoidAlphaValue = 2.0;
		private int			windowSize = 5;
		private int			predictionSize = 1;
		private int			iterations = 1000;

		private Thread		workerThread = null;
		private bool		needToStop = false;

		private double[,]	windowDelimiter = new double[2, 2] { { 0, 0 }, { 0, 0 } };
		private double[,]	predictionDelimiter = new double[2, 2] { { 0, 0 }, { 0, 0 } };

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// initializa chart control
		    this.chart.AddDataSeries( "data", Color.Red, Chart.SeriesType.Dots, 5 );
		    this.chart.AddDataSeries( "solution", Color.Blue, Chart.SeriesType.Line, 1 );
		    this.chart.AddDataSeries( "window", Color.LightGray, Chart.SeriesType.Line, 1, false );
		    this.chart.AddDataSeries( "prediction", Color.Gray, Chart.SeriesType.Line, 1, false );

			// update controls
			UpdateSettings( );
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (this.components != null) 
				{
				    this.components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataList = new System.Windows.Forms.ListView();
            this.yColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.estimatedYColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loadDataButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chart = new AForge.Controls.Chart();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.momentumBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.alphaBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.learningRateBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.iterationsBox = new System.Windows.Forms.TextBox();
            this.predictionSizeBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.windowSizeBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.currentPredictionErrorBox = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.currentLearningErrorBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.currentIterationBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataList);
            this.groupBox1.Controls.Add(this.loadDataButton);
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(180, 380);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data";
            // 
            // dataList
            // 
            this.dataList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.yColumnHeader,
            this.estimatedYColumnHeader});
            this.dataList.FullRowSelect = true;
            this.dataList.GridLines = true;
            this.dataList.Location = new System.Drawing.Point(10, 20);
            this.dataList.Name = "dataList";
            this.dataList.Size = new System.Drawing.Size(160, 315);
            this.dataList.TabIndex = 3;
            this.dataList.UseCompatibleStateImageBehavior = false;
            this.dataList.View = System.Windows.Forms.View.Details;
            // 
            // yColumnHeader
            // 
            this.yColumnHeader.Text = "Y:Real";
            this.yColumnHeader.Width = 70;
            // 
            // estimatedYColumnHeader
            // 
            this.estimatedYColumnHeader.Text = "Y:Estimated";
            this.estimatedYColumnHeader.Width = 70;
            // 
            // loadDataButton
            // 
            this.loadDataButton.Location = new System.Drawing.Point(10, 345);
            this.loadDataButton.Name = "loadDataButton";
            this.loadDataButton.Size = new System.Drawing.Size(75, 23);
            this.loadDataButton.TabIndex = 2;
            this.loadDataButton.Text = "&Load";
            this.loadDataButton.Click += new System.EventHandler(this.loadDataButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chart);
            this.groupBox2.Location = new System.Drawing.Point(200, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(300, 380);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Function";
            // 
            // chart
            // 
            this.chart.Location = new System.Drawing.Point(10, 20);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(280, 350);
            this.chart.TabIndex = 0;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
            this.openFileDialog.Title = "Select data file";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.momentumBox);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.alphaBox);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.learningRateBox);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.iterationsBox);
            this.groupBox3.Controls.Add(this.predictionSizeBox);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.windowSizeBox);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(510, 10);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(195, 205);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Settings";
            // 
            // momentumBox
            // 
            this.momentumBox.Location = new System.Drawing.Point(125, 45);
            this.momentumBox.Name = "momentumBox";
            this.momentumBox.Size = new System.Drawing.Size(60, 20);
            this.momentumBox.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(10, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 17);
            this.label6.TabIndex = 8;
            this.label6.Text = "Momentum:";
            // 
            // alphaBox
            // 
            this.alphaBox.Location = new System.Drawing.Point(125, 70);
            this.alphaBox.Name = "alphaBox";
            this.alphaBox.Size = new System.Drawing.Size(60, 20);
            this.alphaBox.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "Sigmoid\'s alpha value:";
            // 
            // learningRateBox
            // 
            this.learningRateBox.Location = new System.Drawing.Point(125, 20);
            this.learningRateBox.Name = "learningRateBox";
            this.learningRateBox.Size = new System.Drawing.Size(60, 20);
            this.learningRateBox.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 14);
            this.label1.TabIndex = 6;
            this.label1.Text = "Learning rate:";
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label8.Location = new System.Drawing.Point(10, 157);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(175, 2);
            this.label8.TabIndex = 22;
            // 
            // iterationsBox
            // 
            this.iterationsBox.Location = new System.Drawing.Point(125, 165);
            this.iterationsBox.Name = "iterationsBox";
            this.iterationsBox.Size = new System.Drawing.Size(60, 20);
            this.iterationsBox.TabIndex = 24;
            // 
            // predictionSizeBox
            // 
            this.predictionSizeBox.Location = new System.Drawing.Point(125, 130);
            this.predictionSizeBox.Name = "predictionSizeBox";
            this.predictionSizeBox.Size = new System.Drawing.Size(60, 20);
            this.predictionSizeBox.TabIndex = 21;
            this.predictionSizeBox.TextChanged += new System.EventHandler(this.predictionSizeBox_TextChanged);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(10, 132);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 16);
            this.label7.TabIndex = 20;
            this.label7.Text = "Prediction size:";
            // 
            // windowSizeBox
            // 
            this.windowSizeBox.Location = new System.Drawing.Point(125, 105);
            this.windowSizeBox.Name = "windowSizeBox";
            this.windowSizeBox.Size = new System.Drawing.Size(60, 20);
            this.windowSizeBox.TabIndex = 19;
            this.windowSizeBox.TextChanged += new System.EventHandler(this.windowSizeBox_TextChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(10, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 18;
            this.label3.Text = "Window size:";
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(126, 185);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(58, 14);
            this.label10.TabIndex = 25;
            this.label10.Text = "( 0 - inifinity )";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(10, 167);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 16);
            this.label9.TabIndex = 23;
            this.label9.Text = "Iterations:";
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(10, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(175, 2);
            this.label5.TabIndex = 17;
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(630, 360);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 6;
            this.stopButton.Text = "S&top";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startButton
            // 
            this.startButton.Enabled = false;
            this.startButton.Location = new System.Drawing.Point(540, 360);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 5;
            this.startButton.Text = "&Start";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.currentPredictionErrorBox);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.currentLearningErrorBox);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.currentIterationBox);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Location = new System.Drawing.Point(510, 225);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(195, 100);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Current iteration:";
            // 
            // currentPredictionErrorBox
            // 
            this.currentPredictionErrorBox.Location = new System.Drawing.Point(125, 70);
            this.currentPredictionErrorBox.Name = "currentPredictionErrorBox";
            this.currentPredictionErrorBox.ReadOnly = true;
            this.currentPredictionErrorBox.Size = new System.Drawing.Size(60, 20);
            this.currentPredictionErrorBox.TabIndex = 5;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(10, 72);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(100, 16);
            this.label13.TabIndex = 4;
            this.label13.Text = "Prediction error:";
            // 
            // currentLearningErrorBox
            // 
            this.currentLearningErrorBox.Location = new System.Drawing.Point(125, 45);
            this.currentLearningErrorBox.Name = "currentLearningErrorBox";
            this.currentLearningErrorBox.ReadOnly = true;
            this.currentLearningErrorBox.Size = new System.Drawing.Size(60, 20);
            this.currentLearningErrorBox.TabIndex = 3;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(10, 47);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 16);
            this.label12.TabIndex = 2;
            this.label12.Text = "Learning error:";
            // 
            // currentIterationBox
            // 
            this.currentIterationBox.Location = new System.Drawing.Point(125, 20);
            this.currentIterationBox.Name = "currentIterationBox";
            this.currentIterationBox.ReadOnly = true;
            this.currentIterationBox.Size = new System.Drawing.Size(60, 20);
            this.currentIterationBox.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(10, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(70, 16);
            this.label11.TabIndex = 0;
            this.label11.Text = "Iteration:";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(715, 398);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Time Series Prediction using Multi-Layer Neural Network";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main( ) 
		{
			Application.Run( new MainForm( ) );
		}

		// On main form closing
		private void MainForm_Closing(object sender, CancelEventArgs e)
		{
			// check if worker thread is running
			if ( (this.workerThread != null ) && (this.workerThread.IsAlive ) )
			{
			    this.needToStop = true;
			    this.workerThread.Join( );
			}
		}

		// Update settings controls
		private void UpdateSettings( )
		{
		    this.learningRateBox.Text	= this.learningRate.ToString( );
		    this.momentumBox.Text		= this.momentum.ToString( );
		    this.alphaBox.Text			= this.sigmoidAlphaValue.ToString( );
		    this.windowSizeBox.Text		= this.windowSize.ToString( );
		    this.predictionSizeBox.Text	= this.predictionSize.ToString( );
		    this.iterationsBox.Text		= this.iterations.ToString( );
		}

		// Load data
		private void loadDataButton_Click(object sender, EventArgs e)
		{
			// show file selection dialog
			if (this.openFileDialog.ShowDialog( ) == DialogResult.OK )
			{
				StreamReader reader = null;
				// read maximum 50 points
				var tempData = new double[50];

				try
				{
					// open selected file
					reader = File.OpenText(this.openFileDialog.FileName );
					string	str = null;
					var		i = 0;

					// read the data
					while ( ( i < 50 ) && ( ( str = reader.ReadLine( ) ) != null ) )
					{
						// parse the value
						tempData[i] = double.Parse( str );

						i++;
					}

					// allocate and set data
				    this.data = new double[i];
				    this.dataToShow = new double[i, 2];
					Array.Copy( tempData, 0, this.data, 0, i );
					for ( var j = 0; j < i; j++ )
					{
					    this.dataToShow[j, 0] = j;
					    this.dataToShow[j, 1] = this.data[j];
					}
				}
				catch (Exception)
				{
					MessageBox.Show( "Failed reading the file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}
				finally
				{
					// close file
					if ( reader != null )
						reader.Close( );
				}

				// update list and chart
				UpdateDataListView( );
			    this.chart.RangeX = new DoubleRange( 0, this.data.Length - 1 );
			    this.chart.UpdateDataSeries( "data", this.dataToShow );
			    this.chart.UpdateDataSeries( "solution", null );
				// set delimiters
				UpdateDelimiters( );
				// enable "Start" button
			    this.startButton.Enabled = true;
			}
		}

		// Update delimiters on the chart
		private void UpdateDelimiters( )
		{
			// window delimiter
		    this.windowDelimiter[0, 0] = this.windowDelimiter[1, 0] = this.windowSize;
		    this.windowDelimiter[0, 1] = this.chart.RangeY.Min;
		    this.windowDelimiter[1, 1] = this.chart.RangeY.Max;
		    this.chart.UpdateDataSeries( "window", this.windowDelimiter );
			// prediction delimiter
		    this.predictionDelimiter[0, 0] = this.predictionDelimiter[1, 0] = this.data.Length - 1 - this.predictionSize;
		    this.predictionDelimiter[0, 1] = this.chart.RangeY.Min;
		    this.predictionDelimiter[1, 1] = this.chart.RangeY.Max;
		    this.chart.UpdateDataSeries( "prediction", this.predictionDelimiter );
		}

		// Update data in list view
		private void UpdateDataListView( )
		{
			// remove all current records
		    this.dataList.Items.Clear( );
			// add new records
			for ( int i = 0, n = this.data.GetLength( 0 ); i < n; i++ )
			{
			    this.dataList.Items.Add(this.data[i].ToString( ) );
			}
		}

		// Enable/disable controls
		private void EnableControls( bool enable )
		{
		    this.loadDataButton.Enabled		= enable;
		    this.learningRateBox.Enabled		= enable;
		    this.momentumBox.Enabled			= enable;
		    this.alphaBox.Enabled			= enable;
		    this.windowSizeBox.Enabled		= enable;
		    this.predictionSizeBox.Enabled	= enable;
		    this.iterationsBox.Enabled		= enable;

		    this.startButton.Enabled			= enable;
		    this.stopButton.Enabled			= !enable;
		}

		// On window size changed
		private void windowSizeBox_TextChanged(object sender, EventArgs e)
		{
			UpdateWindowSize( );
		}

		// On prediction changed
		private void predictionSizeBox_TextChanged(object sender, EventArgs e)
		{
			UpdatePredictionSize( );		
		}

		// Update window size
		private void UpdateWindowSize( )
		{
			if (this.data != null )
			{
				// get new window size value
				try
				{
				    this.windowSize = Math.Max( 1, Math.Min( 15, int.Parse(this.windowSizeBox.Text ) ) );
				}
				catch
				{
				    this.windowSize = 5;
				}
				// check if we have too few data
				if (this.windowSize >= this.data.Length )
				    this.windowSize = 1;
				// update delimiters
				UpdateDelimiters( );
			}
		}

		// Update prediction size
		private void UpdatePredictionSize( )
		{
			if (this.data != null )
			{
				// get new prediction size value
				try
				{
				    this.predictionSize = Math.Max( 1, Math.Min( 10, int.Parse(this.predictionSizeBox.Text ) ) );
				}
				catch
				{
				    this.predictionSize = 1;
				}
				// check if we have too few data
				if (this.data.Length - this.predictionSize - 1 < this.windowSize )
				    this.predictionSize = 1;
				// update delimiters
				UpdateDelimiters( );
			}
		}

		// On button "Start"
		private void startButton_Click( object sender, EventArgs e )
		{
			// clear previous solution
			for ( int j = 0, n = this.data.Length; j < n; j++ )
			{
				if (this.dataList.Items[j].SubItems.Count > 1 )
				    this.dataList.Items[j].SubItems.RemoveAt( 1 );
			}

			// get learning rate
			try
			{
			    this.learningRate = Math.Max( 0.00001, Math.Min( 1, double.Parse(this.learningRateBox.Text ) ) );
			}
			catch
			{
			    this.learningRate = 0.1;
			}
			// get momentum
			try
			{
			    this.momentum = Math.Max( 0, Math.Min( 0.5, double.Parse(this.momentumBox.Text ) ) );
			}
			catch
			{
			    this.momentum = 0;
			}
			// get sigmoid's alpha value
			try
			{
			    this.sigmoidAlphaValue = Math.Max( 0.001, Math.Min( 50, double.Parse(this.alphaBox.Text ) ) );
			}
			catch
			{
			    this.sigmoidAlphaValue = 2;
			}
			// iterations
			try
			{
			    this.iterations = Math.Max( 0, int.Parse(this.iterationsBox.Text ) );
			}
			catch
			{
			    this.iterations = 1000;
			}
			// update settings controls
			UpdateSettings( );
		
			// disable all settings controls except "Stop" button
			EnableControls( false );

			// run worker thread
		    this.needToStop = false;
		    this.workerThread = new Thread( new ThreadStart( SearchSolution ) );
		    this.workerThread.Start( );
		}

		// On button "Stop"
		private void stopButton_Click( object sender, EventArgs e )
		{
			// stop worker thread
		    this.needToStop = true;
		    this.workerThread.Join( );
		    this.workerThread = null;
		}

		// Worker thread
		void SearchSolution( )
		{
			// number of learning samples
			var samples = this.data.Length - this.predictionSize - this.windowSize;
			// data transformation factor
			var factor = 1.7 /this.chart.RangeY.Length;
			var yMin = this.chart.RangeY.Min;
			// prepare learning data
			var input = new double[samples][];
			var output = new double[samples][];

			for ( var i = 0; i < samples; i++ )
			{
				input[i] = new double[this.windowSize];
				output[i] = new double[1];
			
				// set input
				for ( var j = 0; j < this.windowSize; j++ )
				{
					input[i][j] = (this.data[i + j] - yMin ) * factor - 0.85;
				}
				// set output
				output[i][0] = (this.data[i + this.windowSize] - yMin ) * factor - 0.85;
			}

			// create multi-layer neural network
			var	network = new ActivationNetwork(
				new BipolarSigmoidFunction(this.sigmoidAlphaValue ), this.windowSize, this.windowSize * 2, 1 );
			// create teacher
			var teacher = new BackPropagationLearning( network );
			// set learning rate and momentum
			teacher.LearningRate	= this.learningRate;
			teacher.Momentum		= this.momentum;

			// iterations
			var iteration = 1;

			// solution array
			var			solutionSize = this.data.Length - this.windowSize;
			var	solution = new double[solutionSize, 2];
			var	networkInput = new double[this.windowSize];

			// calculate X values to be used with solution function
			for ( var j = 0; j < solutionSize; j++ )
			{
				solution[j, 0] = j + this.windowSize;
			}
			
			// loop
			while ( !this.needToStop )
			{
				// run epoch of learning procedure
				var error = teacher.RunEpoch( input, output ) / samples;
			
				// calculate solution and learning and prediction errors
				var learningError = 0.0;
				var predictionError = 0.0;
				// go through all the data
				for ( int i = 0, n = this.data.Length - this.windowSize; i < n; i++ )
				{
					// put values from current window as network's input
					for ( var j = 0; j < this.windowSize; j++ )
					{
						networkInput[j] = (this.data[i + j] - yMin ) * factor - 0.85;
					}

					// evalue the function
					solution[i, 1] = ( network.Compute( networkInput)[0] + 0.85 ) / factor + yMin;

					// calculate prediction error
					if ( i >= n - this.predictionSize )
					{
						predictionError += Math.Abs( solution[i, 1] - this.data[this.windowSize + i] );
					}
					else
					{
						learningError += Math.Abs( solution[i, 1] - this.data[this.windowSize + i] );
					}
				}
				// update solution on the chart
			    this.chart.UpdateDataSeries( "solution", solution );

				// set current iteration's info
			    this.currentIterationBox.Text = iteration.ToString( );
			    this.currentLearningErrorBox.Text = learningError.ToString( "F3" );
			    this.currentPredictionErrorBox.Text = predictionError.ToString( "F3" );

				// increase current iteration
				iteration++;

				// check if we need to stop
				if ( (this.iterations != 0 ) && ( iteration > this.iterations ) )
					break;
			}
			
			// show new solution
			for ( int j = this.windowSize, k = 0, n = this.data.Length; j < n; j++, k++ )
			{
			    this.dataList.Items[j].SubItems.Add( solution[k, 1].ToString( ) );
			}

			// enable settings controls
			EnableControls( true );
		}

        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

	}
}
