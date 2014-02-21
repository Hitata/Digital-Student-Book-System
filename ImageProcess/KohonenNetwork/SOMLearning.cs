// AForge Neural Net Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2012
// contacts@aforgenet.com
//

namespace ImageProcess.KohonenNetwork
{
    using System;

    /// <summary>
    /// Kohonen Self Organizing Map (SOM) learning algorithm.
    /// </summary>
    /// 
    /// <remarks><para>This class implements Kohonen's SOM learning algorithm and
    /// is widely used in clusterization tasks. The class allows to train
    /// <see cref="DistanceNetwork">Distance Networks</see>.</para>
    /// 
    /// <para>Sample usage (clustering RGB colors):</para>
    /// <code>
    /// // set range for randomization neurons' weights
    /// Neuron.RandRange = new Range( 0, 255 );
    /// // create network
    /// DistanceNetwork	network = new DistanceNetwork(
    ///         3, // thress inputs in the network
    ///         100 * 100 ); // 10000 neurons
    /// // create learning algorithm
    /// SOMLearning	trainer = new SOMLearning( network );
    /// // network's input
    /// double[] input = new double[3];
    /// // loop
    /// while ( !needToStop )
    /// {
    ///     input[0] = rand.Next( 256 );
    ///     input[1] = rand.Next( 256 );
    ///     input[2] = rand.Next( 256 );
    /// 
    ///     trainer.Run( input );
    /// 
    ///     // ...
    ///     // update learning rate and radius continuously,
    ///     // so networks may come steady state
    /// }
    /// </code>
    /// </remarks>
    /// 
    public class SOMLearning : IUnsupervisedLearning
    {
        // neural network to train
        private Network network;
        // network's dimension
        private int width;
        private int height;

        // learning rate
        private double learningRate = 0.9;
        // learning radius
        private double learningRadius = 15;

        // squared learning radius multiplied by 2 (precalculated value to speed up computations)
        private double squaredRadius2 = 2 * 15 * 15;

        /// <summary>
        /// Learning rate, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>Determines speed of learning.</para>
        /// 
        /// <para>Default value equals to <b>0.1</b>.</para>
        /// </remarks>
        /// 
        public double LearningRate
        {
            get { return learningRate; }
            set
            {
                learningRate = Math.Max( 0.0, Math.Min( 1.0, value ) );
            }
        }

        /// <summary>
        /// Learning radius.
        /// </summary>
        /// 
        /// <remarks><para>Determines the amount of neurons to be updated around
        /// winner neuron. Neurons, which are in the circle of specified radius,
        /// are updated during the learning procedure. Neurons, which are closer
        /// to the winner neuron, get more update.</para>
        /// 
        /// <para><note>In the case if learning rate is set to 0, then only winner
        /// neuron's weights are updated.</note></para>
        /// 
        /// <para>Default value equals to <b>7</b>.</para>
        /// </remarks>
        /// 
        public double LearningRadius
        {
            get { return learningRadius; }
            set
            {
                learningRadius = Math.Max( 0, value );
                squaredRadius2 = 2 * learningRadius * learningRadius;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SOMLearning"/> class.
        /// </summary>
        /// 
        /// <param name="network">Neural network to train.</param>
        /// 
        /// <remarks><para>This constructor supposes that a square network will be passed for training -
        /// it should be possible to get square root of network's neurons amount.</para></remarks>
        /// 
        /// <exception cref="ArgumentException">Invalid network size - square network is expected.</exception>
        /// 
        public SOMLearning( Network network )
        {
            // network's dimension was not specified, let's try to guess
            int neuronsCount = network.Layers[0].Neurons.Length;
            int width = (int) Math.Sqrt( neuronsCount );

            if ( width * width != neuronsCount )
            {
                throw new ArgumentException( "Invalid network size." );
            }

            // ok, we got it
            this.network = network;
            this.width = width;
            this.height = width;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SOMLearning"/> class.
        /// </summary>
        /// 
        /// <param name="network">Neural network to train.</param>
        /// <param name="width">Neural network's width.</param>
        /// <param name="height">Neural network's height.</param>
        ///
        /// <remarks>The constructor allows to pass network of arbitrary rectangular shape.
        /// The amount of neurons in the network should be equal to <b>width</b> * <b>height</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">Invalid network size - network size does not correspond
        /// to specified width and height.</exception>
        /// 
        public SOMLearning( Network network, int width, int height )
        {
            // check network size
            if ( network.Layers[0].Neurons.Length != width * height )
            {
                throw new ArgumentException( "Invalid network size." );
            }

            this.network = network;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Runs learning iteration.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns learning error - summary absolute difference between neurons' weights
        /// and appropriate inputs. The difference is measured according to the neurons
        /// distance to the winner neuron.</returns>
        /// 
        /// <remarks><para>The method runs one learning iterations - finds winner neuron (the neuron
        /// which has weights with values closest to the specified input vector) and updates its weight
        /// (as well as weights of neighbor neurons) in the way to decrease difference with the specified
        /// input vector.</para></remarks>
        /// 
        //public double Run( double[] input )
        //{
        //    double error = 0.0;

        //    // compute the network
        //    network.Compute( input );
        //    int winner = network.GetWinner( );

        //    // get layer of the network
        //    Layer layer = network.Layers[0];
        //    layer.Neurons[winner].Win += 1;
        //    // check learning radius
        //    int lr = Convert.ToInt16(learningRadius);
        //    if (lr == 0)
        //    {
        //        Neuron neuron = layer.Neurons[winner];
        //        //neuron.Win += 1;                // update weight of the winner only
        //        for (int i = 0; i < neuron.Weights.Length; i++)
        //        {
        //            // calculate the error
        //            double e = input[i] - neuron.Weights[i];
        //            error += Math.Abs(e);
        //            // update weights
        //            neuron.Weights[i] += e * learningRate;
        //        }
        //    }
        //    else
        //    {
        //        // winner's X and Y
        //        int wx = winner % width;
        //        int wy = winner / width;

        //        // walk through all neurons of the layer


        //        //for (int i = wx - lr; i < wx + lr + 1; i++)
        //        //{
        //        //    for (int j = wy - lr; j < wy + lr + 1; j++)
        //        //    {
        //        //        if ((i >= 0) && (i < width) && (j >= 0) && (j < height))
        //        //        {
                            
        //        for(int j = 0; j < layer.Neurons.Length; j ++)
        //        {

        //            int dx = (j % width) - wx;
        //            int dy = (j / width) - wy;
        //            int d = (dx - wx) * (dx - wx) + (dy - wy) * (dy - wy);
        //            double sqrLR = learningRadius*learningRadius;
        //            if (d < sqrLR)
        //            {
                        
        //                Neuron neuron = layer.Neurons[j];
        //                double factor = Math.Exp(-(double)(d) / squaredRadius2);
        //                //double factor = (-(double)Math.Sqrt(d / 2) / learningRadius) + 1;
        //                for (int w = 0; w < neuron.Weights.Length; w++)
        //                {
        //                    double e = (input[w] - neuron.Weights[w]) * factor;
        //                    error += Math.Abs(e);
        //                    neuron.Weights[w] += e * learningRate;
        //                }
        //            }
        //        }
        //            //        }
        //            //    }
        //            //}
                
        //    }
            
        //    return error;
        //}
        public double RunM(double[] input)
        {
            double error = 0.0;

            // compute the network
            network.Compute(input);
            int winner = network.GetWinner();

            // get layer of the network
            Layer layer = network.Layers[0];
            layer.Neurons[winner].Win += 1;
            // check learning radius
            int lr = Convert.ToInt32(learningRadius);
            if (lr == 0)
            {
                Neuron neuron = layer.Neurons[winner];
                
                // update weight of the winner only
                for (int i = 0; i < neuron.Weights.Length; i++)
                {
                    // calculate the error
                    double e = input[i] - neuron.Weights[i];
                    error += Math.Abs(e);
                    // update weights
                    neuron.Weights[i] += e * learningRate;
                }
            }
            else
            {
                // winner's X and Y
                int wx = winner % width;
                int wy = winner / width;
                
                for (int i = wx - lr; i < wx + lr + 1; i++)
                    {
                        for (int j = wy - lr; j < wy + lr + 1; j++)
                        {
                            if ((i >= 0) && (i < width) && (j >= 0) && (j < height))
                            {
                                // walk through all neurons of the layer
                                //for (int j = 0; j < layer.Neurons.Length; j++)
                                //{
                                Neuron neuron = layer.Neurons[i * width + j];

                                //int dx = (j % width) - wx;
                                //int dy = (j / width) - wy;
                                int dx = i - wx;
                                int dy = j - wy;

                                // update factor ( Gaussian based )
                                //double factor = Math.Exp(-(double)(dx * dx + dy * dy) / squaredRadius2);
                                double factor = (-(double)Math.Sqrt((dx * dx + dy * dy) / 2) / lr) + 1;
                                // update weight of the neuron
                                for (int w = 0; w < neuron.Weights.Length; w++)
                                {
                                    // calculate the error
                                    double e = (input[w] - neuron.Weights[w]) * factor;
                                    error += Math.Abs(e);
                                    // update weight
                                    neuron.Weights[w] += e * learningRate;
                                }
                            }
                        }
                    }
                
                }   
            return error;
        }

        public double Run(double[] input)
        {
            Neuron neuron;
            int num3;
            double num4;
            double num = 0.0;
            this.network.Compute(input);
            int winner = this.network.GetWinner();
            Layer layer = this.network.Layers[0];
            if (this.learningRadius == 0.0)
            {
                neuron = layer.Neurons[winner];
                num3 = 0;
                while (num3 < neuron.Weights.Length)
                {
                    num4 = input[num3] - neuron.Weights[num3];
                    num += Math.Abs(num4);
                    neuron.Weights[num3] += num4 * this.learningRate;
                    num3++;
                }
                return num;
            }
            int num5 = winner % this.width;
            int num6 = winner / this.width;
            for (int i = 0; i < layer.Neurons.Length; i++)
            {
                neuron = layer.Neurons[i];
                int num8 = (i % this.width) - num5;
                int num9 = (i / this.width) - num6;
                double num10 = Math.Exp(-((double)((num8 * num8) + (num9 * num9))) / this.squaredRadius2);
                for (num3 = 0; num3 < neuron.Weights.Length; num3++)
                {
                    num4 = (input[num3] - neuron.Weights[num3]) * num10;
                    num += Math.Abs(num4);
                    neuron.Weights[num3] += num4 * this.learningRate;
                }
            }
            return num;
        }
        /// <summary>
        /// Runs learning epoch.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        /// 
        /// <returns>Returns summary learning error for the epoch. See <see cref="Run"/>
        /// method for details about learning error calculation.</returns>
        /// 
        /// <remarks><para>The method runs one learning epoch, by calling <see cref="Run"/> method
        /// for each vector provided in the <paramref name="input"/> array.</para></remarks>
        /// 
        public double RunEpoch( double[][] input )
        {
            double error = 0.0;

            // walk through all training samples
            foreach ( double[] sample in input )
            {
                error += Run( sample );
            }

            // return summary error
            return error;
        }
    }
}
