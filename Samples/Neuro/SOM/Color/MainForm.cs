// AForge Framework
// Color Clustering using Kohonen SOM
//
// Copyright ?Andrew Kirillov, 2006
// andrew.kirillov@gmail.com
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;

namespace Color
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : Form
	{
        private GroupBox groupBox1;
        private BufferedPanel mapPanel;
		private GroupBox groupBox2;
		private Label label1;
		private TextBox iterationsBox;
		private Label label2;
		private TextBox rateBox;
		private Label label3;
		private TextBox radiusBox;
		private Label label4;
		private Button startButton;
		private Button stopButton;
		private Button randomizeButton;
		private Label label5;
		private TextBox currentIterationBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private DistanceNetwork	network;
		private Bitmap			mapBitmap;
		private Random			rand = new Random();

		private int				iterations = 5000;
		private double			learningRate = 0.1;
		private double			radius = 15;

		private Thread	workerThread = null;
		private bool	needToStop = false;

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent( );

			// Create network
		    this.network = new DistanceNetwork( 3, 100 * 100 );

			// Create map bitmap
		    this.mapBitmap = new Bitmap( 200, 200, PixelFormat.Format24bppRgb );

			//
			RandomizeNetwork( );
			UpdateSettings( );
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (this.components != null ) 
				{
				    this.components.Dispose( );
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
            this.randomizeButton = new System.Windows.Forms.Button();
            this.mapPanel = new Color.BufferedPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.currentIterationBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.radiusBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rateBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.iterationsBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.randomizeButton);
            this.groupBox1.Controls.Add(this.mapPanel);
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(222, 265);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Map";
            // 
            // randomizeButton
            // 
            this.randomizeButton.Location = new System.Drawing.Point(10, 230);
            this.randomizeButton.Name = "randomizeButton";
            this.randomizeButton.Size = new System.Drawing.Size(75, 23);
            this.randomizeButton.TabIndex = 1;
            this.randomizeButton.Text = "&Randomize";
            this.randomizeButton.Click += new System.EventHandler(this.randomizeButton_Click);
            // 
            // mapPanel
            // 
            this.mapPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapPanel.Location = new System.Drawing.Point(10, 20);
            this.mapPanel.Name = "mapPanel";
            this.mapPanel.Size = new System.Drawing.Size(202, 202);
            this.mapPanel.TabIndex = 0;
            this.mapPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mapPanel_Paint);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.currentIterationBox);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.stopButton);
            this.groupBox2.Controls.Add(this.startButton);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.radiusBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.rateBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.iterationsBox);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(240, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(190, 265);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Neural Network";
            // 
            // currentIterationBox
            // 
            this.currentIterationBox.Location = new System.Drawing.Point(110, 120);
            this.currentIterationBox.Name = "currentIterationBox";
            this.currentIterationBox.ReadOnly = true;
            this.currentIterationBox.Size = new System.Drawing.Size(70, 20);
            this.currentIterationBox.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(10, 122);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 16);
            this.label5.TabIndex = 9;
            this.label5.Text = "Curren iteration:";
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(105, 230);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 8;
            this.stopButton.Text = "S&top";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(20, 230);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 7;
            this.startButton.Text = "&Start";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(10, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(170, 2);
            this.label4.TabIndex = 6;
            // 
            // radiusBox
            // 
            this.radiusBox.Location = new System.Drawing.Point(110, 70);
            this.radiusBox.Name = "radiusBox";
            this.radiusBox.Size = new System.Drawing.Size(70, 20);
            this.radiusBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(10, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Initial radius:";
            // 
            // rateBox
            // 
            this.rateBox.Location = new System.Drawing.Point(110, 45);
            this.rateBox.Name = "rateBox";
            this.rateBox.Size = new System.Drawing.Size(70, 20);
            this.rateBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Initial learning rate:";
            // 
            // iterationsBox
            // 
            this.iterationsBox.Location = new System.Drawing.Point(110, 20);
            this.iterationsBox.Name = "iterationsBox";
            this.iterationsBox.Size = new System.Drawing.Size(70, 20);
            this.iterationsBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Iteraions:";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(439, 285);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Color Clustering using Kohonen SOM";
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
		    this.iterationsBox.Text	= this.iterations.ToString( );
		    this.rateBox.Text		= this.learningRate.ToString( );
		    this.radiusBox.Text		= this.radius.ToString( );
		}

		// On "Rundomize" button clicked
		private void randomizeButton_Click(object sender, EventArgs e)
		{
			RandomizeNetwork( );
		}

		// Radnomize weights of network
		private void RandomizeNetwork( )
		{
			Neuron.RandRange = new DoubleRange( 0, 255 );

			// randomize net
		    this.network.Randomize( );

			// update map
			UpdateMap( );
		}

		// Update map from network weights
		private void UpdateMap( )
		{
			// lock
			Monitor.Enter( this );

			// lock bitmap
			var mapData = this.mapBitmap.LockBits( new Rectangle( 0, 0 , 200, 200 ),
				ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb );

			var stride = mapData.Stride;
			var offset = stride - 200 * 3;
			Layer layer = this.network[0];

			unsafe
			{
				var ptr = (byte*) mapData.Scan0;

				// for all rows
				for ( int y = 0, i = 0; y < 100; y++ )
				{
					// for all pixels
					for ( var x = 0; x < 100; x++, i++, ptr += 6 )
					{
						var neuron = layer[i];

						// red
						ptr[2] = ptr[2 + 3] = ptr[2 + stride] = ptr[2 + 3 + stride]	=
							(byte) Math.Max( 0, Math.Min( 255, neuron[0] ) );
						// green
						ptr[1] = ptr[1 + 3] = ptr[1 + stride] = ptr[1 + 3 + stride]	=
							(byte) Math.Max( 0, Math.Min( 255, neuron[1] ) );
						// blue
						ptr[0] = ptr[0 + 3] = ptr[0 + stride] = ptr[0 + 3 + stride]	=
							(byte) Math.Max( 0, Math.Min( 255, neuron[2] ) );
					}

					ptr += offset;
					ptr += stride;
				}
			}

			// unlock image
		    this.mapBitmap.UnlockBits( mapData );

			// unlock
			Monitor.Exit( this );

			// invalidate maps panel
		    this.mapPanel.Invalidate( );
		}

		// Paint map
		private void mapPanel_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;

			// lock
			Monitor.Enter( this );

			// drat image
			g.DrawImage(this.mapBitmap, 0, 0, 200, 200 );

			// unlock
			Monitor.Exit( this );
		}

		// Enable/disale controls
		private void EnableControls( bool enable )
		{
		    this.iterationsBox.Enabled	= enable;
		    this.rateBox.Enabled			= enable;
		    this.radiusBox.Enabled		= enable;

		    this.startButton.Enabled		= enable;
		    this.randomizeButton.Enabled	= enable;
		    this.stopButton.Enabled		= !enable;
		}

		// On "Start" button click
		private void startButton_Click(object sender, EventArgs e)
		{
			// get iterations count
			try
			{
			    this.iterations = Math.Max( 10, Math.Min( 1000000, int.Parse(this.iterationsBox.Text ) ) );
			}
			catch
			{
			    this.iterations = 5000;
			}
			// get learning rate
			try
			{
			    this.learningRate = Math.Max( 0.00001, Math.Min( 1.0, double.Parse(this.rateBox.Text ) ) );
			}
			catch
			{
			    this.learningRate = 0.1;
			}
			// get radius
			try
			{
			    this.radius = Math.Max( 5, Math.Min( 75, int.Parse(this.radiusBox.Text ) ) );
			}
			catch
			{
			    this.radius = 15;
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

		// On "Stop" button click
		private void stopButton_Click(object sender, EventArgs e)
		{
			// stop worker thread
		    this.needToStop = true;
		    this.workerThread.Join( );
		    this.workerThread = null;
		}

		// Worker thread
		void SearchSolution( )
		{
			// create learning algorithm
			var	trainer = new SOMLearning(this.network );

			// input
			var input = new double[3];

			var	fixedLearningRate = this.learningRate / 10;
			var	driftingLearningRate = fixedLearningRate * 9;

			// iterations
			var i = 0;

			// loop
			while ( !this.needToStop )
			{
				trainer.LearningRate = driftingLearningRate * (this.iterations - i ) /this.iterations + fixedLearningRate;
				trainer.LearningRadius = (double) this.radius * (this.iterations - i ) /this.iterations;

				input[0] = this.rand.Next( 256 );
				input[1] = this.rand.Next( 256 );
				input[2] = this.rand.Next( 256 );

				trainer.Run( input );

				// update map once per 50 iterations
				if ( ( i % 10 ) == 9 )
				{
					UpdateMap( );
				}

				// increase current iteration
				i++;

				// set current iteration's info
			    this.currentIterationBox.Text = i.ToString( );

				// stop ?
				if ( i >= this.iterations )
					break;
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
