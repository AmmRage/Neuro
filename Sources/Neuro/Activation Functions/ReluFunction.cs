using System;

namespace AForge.Neuro
{
    public class ReluFunction : IActivationFunction
    {
        public double Function(double x)
        {
            return Math.Max(0, x);
        }

        public double Derivative(double x)
        {
            if (x <= 0)
                return 0;
            else
                return 1;
        }

        public double Derivative2(double y)
        {
            if (y <= 0)
                return 0;
            else
                return 1;
        }
    }
}