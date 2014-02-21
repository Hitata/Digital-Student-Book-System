// AForge Neural Net Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright � AForge.NET, 2007-2012
// contacts@aforgenet.com
//

namespace ImageProcess.SOMImplementation
{
    using System;
    using AForge;

    /// <summary>
    /// Base neuron class.
    /// </summary>
    /// 
    /// <remarks>This is a base neuron class, which encapsulates such
    /// common properties, like neuron's input, output and weights.</remarks>
    /// 
    [Serializable]
    public class Neuron
    {
        /// <summary>
        /// Neuron's inputs count.
        /// </summary>
        protected int inputsCount = 0;

        /// <summary>
        /// Nouron's wieghts.
        /// </summary>
        protected double[] weights = null;

        /// <summary>
        /// Neuron's output value.
        /// </summary>
        protected double output = 0;
        protected int win = 0;

        public int Win
        {
            get { return win; }
            set { win = value; }
        }
        /// <summary>
        /// Random number generator.
        /// </summary>
        /// 
        /// <remarks>The generator is used for neuron's weights randomization.</remarks>
        /// 
        protected static ThreadSafeRandom rand = new ThreadSafeRandom( );

        /// <summary>
        /// Random generator range.
        /// </summary>
        /// 
        /// <remarks>Sets the range of random generator. Affects initial values of neuron's weight.
        /// Default value is [0, 1].</remarks>
        /// 
        protected static Range randRange = new Range( 0.0f, 1.0f );
        
        /// <summary>
        /// Random number generator.
        /// </summary>
        /// 
        /// <remarks>The property allows to initialize random generator with a custom seed. The generator is
        /// used for neuron's weights randomization.</remarks>
        /// 
        public static ThreadSafeRandom RandGenerator
        {
            get { return rand; }
            set
            {
                if ( value != null )
                {
                    rand = value;
                }
            }
        }

        /// <summary>
        /// Random generator range.
        /// </summary>
        /// 
        /// <remarks>Sets the range of random generator. Affects initial values of neuron's weight.
        /// Default value is [0, 1].</remarks>
        /// 
        public static Range RandRange
        {
            get { return randRange; }
            set { randRange = value; }
        }

        /// <summary>
        /// Neuron's inputs count.
        /// </summary>
        public int InputsCount
        {
            get { return inputsCount; }
        }

        /// <summary>
        /// Neuron's output value.
        /// </summary>
        /// 
        /// <remarks>The calculation way of neuron's output value is determined by inherited class.</remarks>
        /// 
        public double Output
        {
            get { return output; }
        }


        /// <summary>
        /// Neuron's weights.
        /// </summary>
        public double[] Weights
        {
            get { return weights; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Neuron"/> class.
        /// </summary>
        ///
        /// <param name="inputs">Neuron's inputs count.</param>
        /// 
        /// <remarks>The new neuron will be randomized (see <see cref="Randomize"/> method)
        /// after it is created.</remarks>
        ///
        public double Compute(double[] input)
        {
            // check for corrent input vector
            if (input.Length != inputsCount)
                throw new ArgumentException("Wrong length of the input vector.");

            // difference value
            double dif = 0.0;

            // compute distance between inputs and weights
            for (int i = 0; i < inputsCount; i++)
            {
                dif += (weights[i] - input[i]) * (weights[i] - input[i]);
            }
            dif = Math.Sqrt(dif);
            // assign output property as well (works correctly for single threaded usage)
            this.output = dif;

            return dif;
        }

        public Neuron( int inputs )
        {
            // allocate weights
            inputsCount = Math.Max( 1, inputs );
            weights = new double[inputsCount];
            // randomize the neuron
            Randomize( );
        }

        //public Neuron(int inputsCount_2)
        //{
        //    // TODO: Complete member initialization
        //    this.inputsCount_2 = inputsCount_2;
        //}

        /// <summary>
        /// Randomize neuron.
        /// </summary>
        /// 
        /// <remarks>Initialize neuron's weights with random values within the range specified
        /// by <see cref="RandRange"/>.</remarks>
        /// 
        public virtual void Randomize( )
        {
            double d = randRange.Length;

            // randomize weights
            for ( int i = 0; i < inputsCount; i++ )
                weights[i] = rand.NextDouble( ) * d + randRange.Min;
        }

        /// <summary>
        /// Computes output value of neuron.
        /// </summary>
        /// kmjm
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns neuron's output value.</returns>
        /// 
        /// <remarks>The actual neuron's output value is determined by inherited class.
        /// The output value is also stored in <see cref="Output"/> property.</remarks>
        /// 
    }
}
