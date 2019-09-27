using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public class ImplicitCellular : ImplicitModuleBase
    {
        private CellularGenerator generator;

        public readonly double[] Coefficients = new double[4];

        public ImplicitCellular(CellularGenerator generator)
        {
            if (generator == null)
            {
                throw new ArgumentNullException("generator");
            }

            this.generator = generator;
        }

        public CellularGenerator Generator
        {
            get => generator;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                generator = value;
            }
        }

        public override double Get(double x, double y)
        {
            CellularCache c = generator.Get(x, y);

            return
                c.F[0] * Coefficients[0] +
                c.F[1] * Coefficients[1] +
                c.F[2] * Coefficients[2] +
                c.F[3] * Coefficients[3];
        }

        public override double Get(double x, double y, double z)
        {
            CellularCache c = generator.Get(x, y, z);

            return
                c.F[0] * Coefficients[0] +
                c.F[1] * Coefficients[1] +
                c.F[2] * Coefficients[2] +
                c.F[3] * Coefficients[3];
        }

        public override double Get(double x, double y, double z, double w)
        {
            CellularCache c = generator.Get(x, y, z, w);

            return
                c.F[0] * Coefficients[0] +
                c.F[1] * Coefficients[1] +
                c.F[2] * Coefficients[2] +
                c.F[3] * Coefficients[3];
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            CellularCache c = generator.Get(x, y, z, w, u, v);

            return
                c.F[0] * Coefficients[0] +
                c.F[1] * Coefficients[1] +
                c.F[2] * Coefficients[2] +
                c.F[3] * Coefficients[3];
        }
    }
}
