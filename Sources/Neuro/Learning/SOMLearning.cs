// AForge Neural Net Library
//
// Copyright ?Andrew Kirillov, 2005-2006
// andrew.kirillov@gmail.com
//

namespace AForge.Neuro.Learning
{
	using System;

	/// <summary>
	/// Kohonen Self Organizing Map (SOM) learning algorithm
	/// </summary>
	/// 
	/// <remarks>This class implements Kohonen's SOM learning algorithm and
	/// is widel?used in clusterization tasks. The class allows to train
	/// <see cref="DistanceNetwork">Distance Networks</see>.</remarks>
	/// 
	public class SOMLearning : IUnsupervisedLearning
	{
		// neural network to train
		private DistanceNetwork	network;
		// network's dimension
		private int		width;
		private int		height;

		// learning rate
		private double	learningRate = 0.1;
		// learning radius
		private double	learningRadius = 7;
		
		// squared learning radius multiplied by 2 (precalculated value to speed up computations)
		private double	squaredRadius2 = 2 * 7 * 7;

		/// <summary>
		/// Learning rate
		/// </summary>
		/// 
		/// <remarks>Determines speed of learning. Value range is [0, 1].
		/// Default value equals to 0.1.</remarks>
		/// 
		public double LearningRate
		{
			get { return this.learningRate; }
			set
			{
			    this.learningRate = Math.Max( 0.0, Math.Min( 1.0, value ) );
			}
		}

		/// <summary>
		/// Learning radius
		/// </summary>
		/// 
		/// <remarks>Determines the amount of neurons to be updated around
		/// winner neuron. Neurons, which are in the circle of specified radius,
		/// are updated during the learning procedure. Neurons, which are closer
		/// to the winner neuron, get more update.<br /><br />
		/// Default value equals to 7.</remarks>
		/// 
		public double LearningRadius
		{
			get { return this.learningRadius; }
			set
			{
			    this.learningRadius = Math.Max( 0, value );
			    this.squaredRadius2 = 2 *this.learningRadius *this.learningRadius;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SOMLearning"/> class
		/// </summary>
		/// 
		/// <param name="network">Neural network to train</param>
		/// 
		/// <remarks>This constructor supposes that a square network will be passed for training -
		/// it should be possible to get square root of network's neurons amount.</remarks>
		/// 
		public SOMLearning( DistanceNetwork network )
		{
			// network's dimension was not specified, let's try to guess
			var neuronsCount = network[0].NeuronsCount;
		    this.width = (int) Math.Sqrt( neuronsCount );

			if (this.width *this.width != neuronsCount )
			{
				throw new ArgumentException( "Invalid network size" );
			}

			// ok, we got it
			this.network	= network;
			this.width		= this.width;
			this.height		= this.height;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="SOMLearning"/> class
		/// </summary>
		/// 
		/// <param name="network">Neural network to train</param>
		/// <param name="width">Neural network's width</param>
		/// <param name="height">Neural network's height</param>
		///
		/// <remarks>The constructor allows to pass network of arbitrary rectangular shape.
		/// The amount of neurons in the network should be equal to <b>width</b> * <b>height</b>.
		/// </remarks> 
		///
		public SOMLearning( DistanceNetwork network, int width, int height )
		{
			// check network size
			if ( network[0].NeuronsCount != width * height )
			{
				throw new ArgumentException( "Invalid network size" );
			}

			this.network	= network;
			this.width		= width;
			this.height		= height;
		}

		/// <summary>
		/// Runs learning iteration
		/// </summary>
		/// 
		/// <param name="input">input vector</param>
		/// 
		/// <returns>Returns learning error - summary absolute difference between updated
		/// weights and according inputs. The difference is measured according to the neurons
		/// distance to the  winner neuron.</returns>
		/// 
		public double Run( double[] input )
		{
			var error = 0.0;

			// compute the network
		    this.network.Compute( input );
			var winner = this.network.GetWinner( );

			// get layer of the network
			Layer layer = this.network[0];

			// check learning radius
			if (this.learningRadius == 0 )
			{
				var neuron = layer[winner];

				// update weight of the winner only
				for ( int i = 0, n = neuron.InputsCount; i < n; i++ )
				{
					neuron[i] += ( input[i] - neuron[i] ) *this.learningRate;
				}
			}
			else
			{
				// winner's X and Y
				var wx = winner %this.width;
				var wy = winner /this.width;

				// walk through all neurons of the layer
				for ( int j = 0, m = layer.NeuronsCount; j < m; j++ )
				{
					var neuron = layer[j];

					var dx = ( j %this.width ) - wx;
					var dy = ( j /this.width ) - wy;

					// update factor ( Gaussian based )
					var factor = Math.Exp( - (double) ( dx * dx + dy * dy ) /this.squaredRadius2 );

					// update weight of the neuron
					for ( int i = 0, n = neuron.InputsCount; i < n; i++ )
					{
						// calculate the error
						var e = ( input[i] - neuron[i] ) * factor;
						error += Math.Abs( e );
						// update weight
						neuron[i] += e *this.learningRate;
					}
				}
			}
			return error;
		}

		/// <summary>
		/// Runs learning epoch
		/// </summary>
		/// 
		/// <param name="input">array of input vectors</param>
		/// 
		/// <returns>Returns summary learning error for the epoch. See <see cref="Run"/>
		/// method for details about learning error calculation.</returns>
		/// 
		public double RunEpoch( double[][] input )
		{
			var error = 0.0;

			// walk through all training samples
			foreach ( var sample in input )
			{
				error += Run( sample );
			}

			// return summary error
			return error;
		}
	}
}
