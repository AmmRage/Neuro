// AForge Framework
// Kohonen SOM 2D Organizing
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

using AForge;
using AForge.Neuro;
using AForge.Neuro.Learning;

namespace SOMOrganizing
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : Form
	{
		private GroupBox groupBox1;
        private Button generateButton;
        private BufferedPanel pointsPanel;
        private GroupBox groupBox2;
        private BufferedPanel mapPanel;
		private CheckBox showConnectionsCheck;
		private CheckBox showInactiveCheck;
		private GroupBox groupBox3;
		private Label label1;
		private TextBox sizeBox;
		private Label label2;
		private Label label3;
		private TextBox radiusBox;
		private Label label4;
		private TextBox rateBox;
		private Label label5;
		private TextBox iterationsBox;
		private Label label6;
		private Label label7;
		private TextBox currentIterationBox;
		private Label label8;
		private Button stopButton;
		private Button startButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;


		private const int	groupRadius = 20;
		private const int	pointsCount = 100;
		private int[,]		points = new int[pointsCount, 2];	// x, y
		private double[][]	trainingSet = new double[pointsCount][];
		private int[,,]		map;

		private int			networkSize		= 15;
		private int			iterations		= 500;
		private double		learningRate	= 0.3;
		private int			learningRadius	= 3;

		private Random		rand = new Random( );
		private Thread		workerThread = null;
		private bool		needToStop = false;

		// Constructor
		public MainForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent( );

			//
			GeneratePoints( );
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
            this.generateButton = new System.Windows.Forms.Button();
            this.pointsPanel = new SOMOrganizing.BufferedPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.showInactiveCheck = new System.Windows.Forms.CheckBox();
            this.showConnectionsCheck = new System.Windows.Forms.CheckBox();
            this.mapPanel = new SOMOrganizing.BufferedPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.currentIterationBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.radiusBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rateBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.iterationsBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sizeBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.generateButton);
            this.groupBox1.Controls.Add(this.pointsPanel);
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(220, 295);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Points";
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(10, 260);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(75, 23);
            this.generateButton.TabIndex = 1;
            this.generateButton.Text = "&Generate";
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // pointsPanel
            // 
            this.pointsPanel.BackColor = System.Drawing.Color.White;
            this.pointsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pointsPanel.Location = new System.Drawing.Point(10, 20);
            this.pointsPanel.Name = "pointsPanel";
            this.pointsPanel.Size = new System.Drawing.Size(200, 200);
            this.pointsPanel.TabIndex = 0;
            this.pointsPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.pointsPanel_Paint);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.showInactiveCheck);
            this.groupBox2.Controls.Add(this.showConnectionsCheck);
            this.groupBox2.Controls.Add(this.mapPanel);
            this.groupBox2.Location = new System.Drawing.Point(240, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(220, 295);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Map";
            // 
            // showInactiveCheck
            // 
            this.showInactiveCheck.Checked = true;
            this.showInactiveCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showInactiveCheck.Location = new System.Drawing.Point(10, 265);
            this.showInactiveCheck.Name = "showInactiveCheck";
            this.showInactiveCheck.Size = new System.Drawing.Size(160, 16);
            this.showInactiveCheck.TabIndex = 2;
            this.showInactiveCheck.Text = "Show Inactive Neurons";
            this.showInactiveCheck.CheckedChanged += new System.EventHandler(this.showInactiveCheck_CheckedChanged);
            // 
            // showConnectionsCheck
            // 
            this.showConnectionsCheck.Checked = true;
            this.showConnectionsCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showConnectionsCheck.Location = new System.Drawing.Point(10, 240);
            this.showConnectionsCheck.Name = "showConnectionsCheck";
            this.showConnectionsCheck.Size = new System.Drawing.Size(150, 16);
            this.showConnectionsCheck.TabIndex = 1;
            this.showConnectionsCheck.Text = "Show Connections";
            this.showConnectionsCheck.CheckedChanged += new System.EventHandler(this.showConnectionsCheck_CheckedChanged);
            // 
            // mapPanel
            // 
            this.mapPanel.BackColor = System.Drawing.Color.White;
            this.mapPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mapPanel.Location = new System.Drawing.Point(10, 20);
            this.mapPanel.Name = "mapPanel";
            this.mapPanel.Size = new System.Drawing.Size(200, 200);
            this.mapPanel.TabIndex = 0;
            this.mapPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mapPanel_Paint);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.stopButton);
            this.groupBox3.Controls.Add(this.startButton);
            this.groupBox3.Controls.Add(this.currentIterationBox);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.radiusBox);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.rateBox);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.iterationsBox);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.sizeBox);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(470, 10);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(180, 295);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Neural Network";
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(95, 260);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 16;
            this.stopButton.Text = "S&top";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(10, 260);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 15;
            this.startButton.Text = "&Start";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // currentIterationBox
            // 
            this.currentIterationBox.Location = new System.Drawing.Point(110, 160);
            this.currentIterationBox.Name = "currentIterationBox";
            this.currentIterationBox.ReadOnly = true;
            this.currentIterationBox.Size = new System.Drawing.Size(60, 20);
            this.currentIterationBox.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(10, 162);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 16);
            this.label8.TabIndex = 13;
            this.label8.Text = "Curren iteration:";
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point(10, 148);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(160, 2);
            this.label7.TabIndex = 12;
            // 
            // radiusBox
            // 
            this.radiusBox.Location = new System.Drawing.Point(110, 120);
            this.radiusBox.Name = "radiusBox";
            this.radiusBox.Size = new System.Drawing.Size(60, 20);
            this.radiusBox.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(10, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 16);
            this.label4.TabIndex = 10;
            this.label4.Text = "Initial radius:";
            // 
            // rateBox
            // 
            this.rateBox.Location = new System.Drawing.Point(110, 95);
            this.rateBox.Name = "rateBox";
            this.rateBox.Size = new System.Drawing.Size(60, 20);
            this.rateBox.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(10, 97);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Initial learning rate:";
            // 
            // iterationsBox
            // 
            this.iterationsBox.Location = new System.Drawing.Point(110, 70);
            this.iterationsBox.Name = "iterationsBox";
            this.iterationsBox.Size = new System.Drawing.Size(60, 20);
            this.iterationsBox.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(10, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 16);
            this.label6.TabIndex = 6;
            this.label6.Text = "Iteraions:";
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(10, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(160, 2);
            this.label3.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "(neurons count = size * size)";
            // 
            // sizeBox
            // 
            this.sizeBox.Location = new System.Drawing.Point(110, 20);
            this.sizeBox.Name = "sizeBox";
            this.sizeBox.Size = new System.Drawing.Size(60, 20);
            this.sizeBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Size:";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(659, 315);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Kohonen SOM 2D Organizing";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
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
		    this.sizeBox.Text		= this.networkSize.ToString( );
		    this.iterationsBox.Text	= this.iterations.ToString( );
		    this.rateBox.Text		= this.learningRate.ToString( );
		    this.radiusBox.Text		= this.learningRadius.ToString( );
		}

		// On "Generate" button click
		private void generateButton_Click(object sender, EventArgs e)
		{
			GeneratePoints( );
		}

		// Generate point
		private void GeneratePoints( )
		{
			var width	= this.pointsPanel.ClientRectangle.Width;
			var height	= this.pointsPanel.ClientRectangle.Height;
			var diameter = groupRadius * 2;

			// generate groups of ten points
			for ( var i = 0; i < pointsCount; )
			{
				var cx = this.rand.Next( width );
				var cy = this.rand.Next( height );

				// generate group
				for ( var j = 0 ; ( i < pointsCount ) && ( j < 10 ); )
				{
					var x = cx + this.rand.Next( diameter ) - groupRadius;
					var y = cy + this.rand.Next( diameter ) - groupRadius;

					// check if wee are not out
					if ( ( x < 0 ) || ( y < 0 ) || ( x >= width ) || ( y >= height ) )
					{
						continue;
					}

					// add point
				    this.points[i, 0] = x;
				    this.points[i, 1] = y;

					j++;
					i++;
				}
			}

		    this.map = null;
		    this.pointsPanel.Invalidate( );
		    this.mapPanel.Invalidate( );
		}

		// Paint points
		private void pointsPanel_Paint( object sender, PaintEventArgs e )
		{
			var g = e.Graphics;

			using ( Brush brush = new SolidBrush( Color.Blue ) )
			{
				// draw all points
				for ( int i = 0, n = this.points.GetLength( 0 ); i < n; i++ )
				{
					g.FillEllipse( brush, this.points[i, 0] - 2, this.points[i, 1] - 2, 5, 5 );
				}
			}
		}

		// Paint map
		private void mapPanel_Paint( object sender, PaintEventArgs e )
		{
			var g = e.Graphics;

			if (this.map != null )
			{
				//
				var showConnections = this.showConnectionsCheck.Checked;
				var showInactive = this.showInactiveCheck.Checked;

				// pens and brushes
				Brush brush = new SolidBrush( Color.Blue );
				Brush brushGray = new SolidBrush( Color.FromArgb( 192, 192, 192 ) );
				var pen = new Pen( Color.Blue, 1 );
				var penGray = new Pen( Color.FromArgb( 192, 192, 192 ), 1 );

				// lock
				Monitor.Enter( this );

				if ( showConnections )
				{
					// draw connections
					for ( int i = 0, n = this.map.GetLength( 0 ); i < n; i++ )
					{
						for ( int j = 0, k = this.map.GetLength( 1 ); j < k; j++ )
						{
							if ( ( !showInactive ) && (this.map[i, j, 2] == 0 ) )
								continue;

							// left
							if ( ( i > 0 ) && ( ( showInactive ) || (this.map[i - 1, j, 2] == 1 ) ) )
							{
								g.DrawLine( ( (this.map[i, j, 2] == 0 ) || (this.map[i - 1, j, 2] == 0 ) ) ? penGray : pen, this.map[i, j, 0], this.map[i, j, 1], this.map[i - 1, j, 0], this.map[i - 1, j, 1] );
							}

							// right
							if ( ( i < n - 1 ) && ( ( showInactive ) || (this.map[i + 1, j, 2] == 1 ) ) )
							{
								g.DrawLine( ( (this.map[i, j, 2] == 0 ) || (this.map[i + 1, j, 2] == 0 ) ) ? penGray : pen, this.map[i, j, 0], this.map[i, j, 1], this.map[i + 1, j, 0], this.map[i + 1, j, 1] );
							}

							// top
							if ( ( j > 0 ) && ( ( showInactive ) || (this.map[i, j - 1, 2] == 1 ) ) )
							{
								g.DrawLine( ( (this.map[i, j, 2] == 0 ) || (this.map[i, j - 1, 2] == 0 ) ) ? penGray : pen, this.map[i, j, 0], this.map[i, j, 1], this.map[i, j - 1, 0], this.map[i, j - 1, 1] );
							}

							// bottom
							if ( ( j < k - 1 ) && ( ( showInactive ) || (this.map[i, j + 1, 2] == 1 ) ) )
							{
								g.DrawLine( ( (this.map[i, j, 2] == 0 ) || (this.map[i, j + 1, 2] == 0 ) ) ? penGray : pen, this.map[i, j, 0], this.map[i, j, 1], this.map[i, j + 1, 0], this.map[i, j + 1, 1] );
							}
						}
					}
				}

				// draw the map
				for ( int i = 0, n = this.map.GetLength( 0 ); i < n; i++ )
				{
					for ( int j = 0, k = this.map.GetLength( 1 ); j < k; j++ )
					{
						if ( ( !showInactive ) && (this.map[i, j, 2] == 0 ) )
							continue;

						// draw the point
						g.FillEllipse( (this.map[i, j, 2] == 0 ) ? brushGray : brush, this.map[i, j, 0] - 2, this.map[i, j, 1] - 2, 5, 5 );
					}
				}

				// unlock
				Monitor.Exit( this );

				brush.Dispose( );
				brushGray.Dispose( );
				pen.Dispose( );
				penGray.Dispose( );
			}
		}

		// Enable/disale controls
		private void EnableControls( bool enable )
		{
		    this.sizeBox.Enabled			= enable;
		    this.iterationsBox.Enabled	= enable;
		    this.rateBox.Enabled			= enable;
		    this.radiusBox.Enabled		= enable;

		    this.startButton.Enabled		= enable;
		    this.generateButton.Enabled	= enable;
		    this.stopButton.Enabled		= !enable;
		}

		// Show/hide connections on map
		private void showConnectionsCheck_CheckedChanged(object sender, EventArgs e)
		{
		    this.mapPanel.Invalidate( );
		}

		// Show/hide inactive neurons on map
		private void showInactiveCheck_CheckedChanged(object sender, EventArgs e)
		{
		    this.mapPanel.Invalidate( );
		}

		// On "Start" button click
		private void startButton_Click(object sender, EventArgs e)
		{
			// get network size
			try
			{
			    this.networkSize = Math.Max( 5, Math.Min( 50, int.Parse(this.sizeBox.Text ) ) );
			}
			catch
			{
			    this.networkSize = 15;
			}
			// get iterations count
			try
			{
			    this.iterations = Math.Max( 10, Math.Min( 1000000, int.Parse(this.iterationsBox.Text ) ) );
			}
			catch
			{
			    this.iterations = 500;
			}
			// get learning rate
			try
			{
			    this.learningRate = Math.Max( 0.00001, Math.Min( 1.0, double.Parse(this.rateBox.Text ) ) );
			}
			catch
			{
			    this.learningRate = 0.3;
			}
			// get radius
			try
			{
			    this.learningRadius = Math.Max( 1, Math.Min( 30, int.Parse(this.radiusBox.Text ) ) );
			}
			catch
			{
			    this.learningRadius = 3;
			}
			// update settings controls
			UpdateSettings( );

			// disable all settings controls except "Stop" button
			EnableControls( false );

			// generate training set
			for ( var i = 0; i < pointsCount; i++ )
			{
				// create new training sample
			    this.trainingSet[i] = new double[2] {this.points[i, 0], this.points[i, 1] };
			}
		
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
			// set random generators range
			Neuron.RandRange = new DoubleRange( 0, Math.Max(this.pointsPanel.ClientRectangle.Width, this.pointsPanel.ClientRectangle.Height ) );

			// create network
			var network = new DistanceNetwork( 2, this.networkSize *this.networkSize );

			// create learning algorithm
			var	trainer = new SOMLearning( network, this.networkSize, this.networkSize );

			// create map
		    this.map = new int[this.networkSize, this.networkSize, 3];

			var	fixedLearningRate = this.learningRate / 10;
			var	driftingLearningRate = fixedLearningRate * 9;

			// iterations
			var i = 0;

			// loop
			while ( !this.needToStop )
			{
				trainer.LearningRate = driftingLearningRate * (this.iterations - i ) /this.iterations + fixedLearningRate;
				trainer.LearningRadius = (double) this.learningRadius * (this.iterations - i ) /this.iterations;

				// run training epoch
				trainer.RunEpoch(this.trainingSet );

				// update map
				UpdateMap( network );

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

		// Update map
		private void UpdateMap( DistanceNetwork network )
		{
			// get first layer
			Layer layer = network[0];

			// lock
			Monitor.Enter( this );

			// run through all neurons
			for ( int i = 0, n = layer.NeuronsCount; i < n; i++ )
			{
				var neuron = layer[i];

				var x = i %this.networkSize;
				var y = i /this.networkSize;

			    this.map[y, x, 0] = (int) neuron[0];
			    this.map[y, x, 1] = (int) neuron[1];
			    this.map[y, x, 2] = 0;
			}

			// collect active neurons
			for ( var i = 0; i < pointsCount; i++ )
			{
				network.Compute(this.trainingSet[i] );
				var w = network.GetWinner( );

			    this.map[w /this.networkSize, w %this.networkSize, 2] = 1;
			}

			// unlock
			Monitor.Exit( this );

			//
		    this.mapPanel.Invalidate( );
		}

        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }
	}
}
