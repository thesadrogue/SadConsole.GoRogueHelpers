using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitFractal : ImplicitModuleBase
    {
        private readonly ImplicitBasisFunction[] basisFunctions = new ImplicitBasisFunction[Noise.MAX_SOURCES];

        private readonly ImplicitModuleBase[] sources = new ImplicitModuleBase[Noise.MAX_SOURCES];

        private readonly double[] expArray = new double[Noise.MAX_SOURCES];

        private readonly double[,] correct = new double[Noise.MAX_SOURCES, 2];

        private int seed;

        private FractalType type;

        private int octaves;

        public ImplicitFractal(FractalType fractalType, BasisType basisType, InterpolationType interpolationType)
        {
            Octaves = 8;
            Frequency = 1.00;
            Lacunarity = 2.00;
            Type = fractalType;
            SetAllSourceTypes(basisType, interpolationType);
            ResetAllSources();
        }

        public ImplicitFractal(FractalType fractalType, BasisType basisType, InterpolationType interpolationType, int octaves, double frequency, int seed)
        {
            this.seed = seed;
            Octaves = octaves;
            Frequency = frequency;
            Octaves = 8;
            Frequency = 1.00;
            Lacunarity = 2.00;
            Type = fractalType;
            SetAllSourceTypes(basisType, interpolationType);
            ResetAllSources();
        }

        public override int Seed
        {
            get => seed;
            set
            {
                seed = value;
                for (int source = 0; source < Noise.MAX_SOURCES; source += 1)
                    sources[source].Seed = ((seed + source * 300));
            }
        }

        public FractalType Type
        {
            get => type;
            set
            {
                type = value;
                switch (type)
                {
                    case FractalType.FractionalBrownianMotion:
                        H = 1.00;
                        Gain = 0.00;
                        Offset = 0.00;
                        FractionalBrownianMotion_CalculateWeights();
                        break;
                    case FractalType.RidgedMulti:
                        H = 0.90;
                        Gain = 2.00;
                        Offset = 1.00;
                        RidgedMulti_CalculateWeights();
                        break;
                    case FractalType.Billow:
                        H = 1.00;
                        Gain = 0.00;
                        Offset = 0.00;
                        Billow_CalculateWeights();
                        break;
                    case FractalType.Multi:
                        H = 1.00;
                        Gain = 0.00;
                        Offset = 0.00;
                        Multi_CalculateWeights();
                        break;
                    case FractalType.HybridMulti:
                        H = 0.25;
                        Gain = 1.00;
                        Offset = 0.70;
                        HybridMulti_CalculateWeights();
                        break;
                    default:
                        H = 1.00;
                        Gain = 0.00;
                        Offset = 0.00;
                        FractionalBrownianMotion_CalculateWeights();
                        break;
                }
            }
        }

        public int Octaves
        {
            get => octaves;
            set
            {
                if (value >= Noise.MAX_SOURCES)
                    value = Noise.MAX_SOURCES - 1;

                octaves = value;
            }
        }

        public double Frequency { get; set; }

        public double Lacunarity { get; set; }

        public double Gain { get; set; }

        public double Offset { get; set; }

        public double H { get; set; }

        public void SetAllSourceTypes(BasisType newBasisType, InterpolationType newInterpolationType)
        {
            for (int i = 0; i < Noise.MAX_SOURCES; ++i)
                basisFunctions[i] = new ImplicitBasisFunction(newBasisType, newInterpolationType);
        }

        public void SetSourceType(int which, BasisType newBasisType, InterpolationType newInterpolationType)
        {
            if (which >= Noise.MAX_SOURCES || which < 0) return;

            basisFunctions[which].BasisType = newBasisType;
            basisFunctions[which].InterpolationType = newInterpolationType;
        }

        public void SetSourceOverride(int which, ImplicitModuleBase newSource)
        {
            if (which < 0 || which >= Noise.MAX_SOURCES) return;

            sources[which] = newSource;
        }

        public void ResetSource(int which)
        {
            if (which < 0 || which >= Noise.MAX_SOURCES) return;


            sources[which] = basisFunctions[which];
        }

        public void ResetAllSources()
        {
            for (int c = 0; c < Noise.MAX_SOURCES; ++c)
                sources[c] = basisFunctions[c];
        }

        public ImplicitBasisFunction GetBasis(int which)
        {
            if (which < 0 || which >= Noise.MAX_SOURCES) return null;

            return basisFunctions[which];
        }

        public override double Get(double x, double y)
        {
            double v;
            switch (type)
            {
                case FractalType.FractionalBrownianMotion:
                    v = FractionalBrownianMotion_Get(x, y);
                    break;
                case FractalType.RidgedMulti:
                    v = RidgedMulti_Get(x, y);
                    break;
                case FractalType.Billow:
                    v = Billow_Get(x, y);
                    break;
                case FractalType.Multi:
                    v = Multi_Get(x, y);
                    break;
                case FractalType.HybridMulti:
                    v = HybridMulti_Get(x, y);
                    break;
                default:
                    v = FractionalBrownianMotion_Get(x, y);
                    break;
            }
            return Utilities.Clamp(v, -1.0, 1.0);
        }

        public override double Get(double x, double y, double z)
        {
            double val;
            switch (type)
            {
                case FractalType.FractionalBrownianMotion:
                    val = FractionalBrownianMotion_Get(x, y, z);
                    break;
                case FractalType.RidgedMulti:
                    val = RidgedMulti_Get(x, y, z);
                    break;
                case FractalType.Billow:
                    val = Billow_Get(x, y, z);
                    break;
                case FractalType.Multi:
                    val = Multi_Get(x, y, z);
                    break;
                case FractalType.HybridMulti:
                    val = HybridMulti_Get(x, y, z);
                    break;
                default:
                    val = FractionalBrownianMotion_Get(x, y, z);
                    break;
            }
            return Utilities.Clamp(val, -1.0, 1.0);
        }

        public override double Get(double x, double y, double z, double w)
        {
            double val;
            switch (type)
            {
                case FractalType.FractionalBrownianMotion:
                    val = FractionalBrownianMotion_Get(x, y, z, w);
                    break;
                case FractalType.RidgedMulti:
                    val = RidgedMulti_Get(x, y, z, w);
                    break;
                case FractalType.Billow:
                    val = Billow_Get(x, y, z, w);
                    break;
                case FractalType.Multi:
                    val = Multi_Get(x, y, z, w);
                    break;
                case FractalType.HybridMulti:
                    val = HybridMulti_Get(x, y, z, w);
                    break;
                default:
                    val = FractionalBrownianMotion_Get(x, y, z, w);
                    break;
            }
            return Utilities.Clamp(val, -1.0, 1.0);
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            double val;
            switch (type)
            {
                case FractalType.FractionalBrownianMotion:
                    val = FractionalBrownianMotion_Get(x, y, z, w, u, v);
                    break;
                case FractalType.RidgedMulti:
                    val = RidgedMulti_Get(x, y, z, w, u, v);
                    break;
                case FractalType.Billow:
                    val = Billow_Get(x, y, z, w, u, v);
                    break;
                case FractalType.Multi:
                    val = Multi_Get(x, y, z, w, u, v);
                    break;
                case FractalType.HybridMulti:
                    val = HybridMulti_Get(x, y, z, w, u, v);
                    break;
                default:
                    val = FractionalBrownianMotion_Get(x, y, z, w, u, v);
                    break;
            }

            return Utilities.Clamp(val, -1.0, 1.0);
        }


        private void FractionalBrownianMotion_CalculateWeights()
        {
            for (int i = 0; i < Noise.MAX_SOURCES; ++i)
                expArray[i] = Math.Pow(Lacunarity, -i * H);

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            double minvalue = 0.00;
            double maxvalue = 0.00;
            for (int i = 0; i < Noise.MAX_SOURCES; ++i)
            {
                minvalue += -1.0 * expArray[i];
                maxvalue += 1.0 * expArray[i];

                const double a = -1.0;
                const double b = 1.0;
                double scale = (b - a) / (maxvalue - minvalue);
                double bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }
        }

        private void RidgedMulti_CalculateWeights()
        {
            for (int i = 0; i < Noise.MAX_SOURCES; ++i)
                expArray[i] = Math.Pow(Lacunarity, -i * H);

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            double minvalue = 0.00;
            double maxvalue = 0.00;
            for (int i = 0; i < Noise.MAX_SOURCES; ++i)
            {
                minvalue += (Offset - 1.0) * (Offset - 1.0) * expArray[i];
                maxvalue += (Offset) * (Offset) * expArray[i];

                const double a = -1.0;
                const double b = 1.0;
                double scale = (b - a) / (maxvalue - minvalue);
                double bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }

        }

        private void Billow_CalculateWeights()
        {
            for (int i = 0; i < Noise.MAX_SOURCES; ++i)
                expArray[i] = Math.Pow(Lacunarity, -i * H);

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            double minvalue = 0.0;
            double maxvalue = 0.0;
            for (int i = 0; i < Noise.MAX_SOURCES; ++i)
            {
                minvalue += -1.0 * expArray[i];
                maxvalue += 1.0 * expArray[i];

                const double a = -1.0;
                const double b = 1.0;
                double scale = (b - a) / (maxvalue - minvalue);
                double bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }

        }

        private void Multi_CalculateWeights()
        {
            for (int i = 0; i < Noise.MAX_SOURCES; ++i)
                expArray[i] = Math.Pow(Lacunarity, -i * H);

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            double minvalue = 1.0;
            double maxvalue = 1.0;
            for (int i = 0; i < Noise.MAX_SOURCES; ++i)
            {
                minvalue *= -1.0 * expArray[i] + 1.0;
                maxvalue *= 1.0 * expArray[i] + 1.0;

                const double a = -1.0;
                const double b = 1.0;
                double scale = (b - a) / (maxvalue - minvalue);
                double bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }

        }

        private void HybridMulti_CalculateWeights()
        {
            for (int i = 0; i < Noise.MAX_SOURCES; ++i)
                expArray[i] = Math.Pow(Lacunarity, -i * H);

            // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
            const double a = -1.0;
            const double b = 1.0;

            double minvalue = Offset - 1.0;
            double maxvalue = Offset + 1.0;
            double weightmin = Gain * minvalue;
            double weightmax = Gain * maxvalue;

            double scale = (b - a) / (maxvalue - minvalue);
            double bias = a - minvalue * scale;
            correct[0, 0] = scale;
            correct[0, 1] = bias;


            for (int i = 1; i < Noise.MAX_SOURCES; ++i)
            {
                if (weightmin > 1.00)
                    weightmin = 1.00;

                if (weightmax > 1.00)
                    weightmax = 1.00;

                double signal = (Offset - 1.0) * expArray[i];
                minvalue += signal * weightmin;
                weightmin *= Gain * signal;

                signal = (Offset + 1.0) * expArray[i];
                maxvalue += signal * weightmax;
                weightmax *= Gain * signal;


                scale = (b - a) / (maxvalue - minvalue);
                bias = a - minvalue * scale;
                correct[i, 0] = scale;
                correct[i, 1] = bias;
            }

        }


        private double FractionalBrownianMotion_Get(double x, double y)
        {
            double value = 0.00;
            x *= Frequency;
            y *= Frequency;


            for (int i = 0; i < octaves; ++i)
            {
                double signal = sources[i].Get(x, y) * expArray[i];
                value += signal;
                x *= Lacunarity;
                y *= Lacunarity;
            }

            return value;
        }

        private double FractionalBrownianMotion_Get(double x, double y, double z)
        {
            double value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                double signal = sources[i].Get(x, y, z) * expArray[i];
                value += signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            return value;
        }

        private double FractionalBrownianMotion_Get(double x, double y, double z, double w)
        {
            double value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                double signal = sources[i].Get(x, y, z, w) * expArray[i];
                value += signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double FractionalBrownianMotion_Get(double x, double y, double z, double w, double u, double v)
        {
            double value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                double signal = sources[i].Get(x, y, z, w, u, v) * expArray[i];
                value += signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }


        private double Multi_Get(double x, double y)
        {
            double value = 1.00;
            x *= Frequency;
            y *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                value *= sources[i].Get(x, y) * expArray[i] + 1.0;
                x *= Lacunarity;
                y *= Lacunarity;

            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double Multi_Get(double x, double y, double z, double w)
        {
            double value = 1.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                value *= sources[i].Get(x, y, z, w) * expArray[i] + 1.0;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double Multi_Get(double x, double y, double z)
        {
            double value = 1.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                value *= sources[i].Get(x, y, z) * expArray[i] + 1.0;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double Multi_Get(double x, double y, double z, double w, double u, double v)
        {
            double value = 1.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                value *= sources[i].Get(x, y, z, w, u, v) * expArray[i] + 1.00;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }


        private double Billow_Get(double x, double y)
        {
            double value = 0.00;
            x *= Frequency;
            y *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                double signal = sources[i].Get(x, y);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;

            }

            value += 0.5;
            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double Billow_Get(double x, double y, double z, double w)
        {
            double value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                double signal = sources[i].Get(x, y, z, w);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            value += 0.5;
            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double Billow_Get(double x, double y, double z)
        {
            double value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                double signal = sources[i].Get(x, y, z);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            value += 0.5;
            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double Billow_Get(double x, double y, double z, double w, double u, double v)
        {
            double value = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                double signal = sources[i].Get(x, y, z, w, u, v);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            value += 0.5;
            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }


        private double RidgedMulti_Get(double x, double y)
        {
            double result = 0.00;
            x *= Frequency;
            y *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                double signal = sources[i].Get(x, y);
                signal = Offset - Math.Abs(signal);
                signal *= signal;
                result += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;

            }

            return result * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double RidgedMulti_Get(double x, double y, double z, double w)
        {
            double result = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                double signal = sources[i].Get(x, y, z, w);
                signal = Offset - Math.Abs(signal);
                signal *= signal;
                result += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            return result * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double RidgedMulti_Get(double x, double y, double z)
        {
            double result = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                double signal = sources[i].Get(x, y, z);
                signal = Offset - Math.Abs(signal);
                signal *= signal;
                result += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            return result * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double RidgedMulti_Get(double x, double y, double z, double w, double u, double v)
        {
            double result = 0.00;
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            for (int i = 0; i < octaves; ++i)
            {
                double signal = sources[i].Get(x, y, z, w, u, v);
                signal = Offset - Math.Abs(signal);
                signal *= signal;
                result += signal * expArray[i];

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            return result * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }


        private double HybridMulti_Get(double x, double y)
        {
            x *= Frequency;
            y *= Frequency;

            double value = sources[0].Get(x, y) + Offset;
            double weight = Gain * value;
            x *= Lacunarity;
            y *= Lacunarity;

            for (int i = 1; i < octaves; ++i)
            {
                if (weight > 1.0)
                    weight = 1.0;

                double signal = (sources[i].Get(x, y) + Offset) * expArray[i];
                value += weight * signal;
                weight *= Gain * signal;
                x *= Lacunarity;
                y *= Lacunarity;

            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double HybridMulti_Get(double x, double y, double z)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            double value = sources[0].Get(x, y, z) + Offset;
            double weight = Gain * value;
            x *= Lacunarity;
            y *= Lacunarity;
            z *= Lacunarity;

            for (int i = 1; i < octaves; ++i)
            {
                if (weight > 1.0)
                    weight = 1.0;

                double signal = (sources[i].Get(x, y, z) + Offset) * expArray[i];
                value += weight * signal;
                weight *= Gain * signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double HybridMulti_Get(double x, double y, double z, double w)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;

            double value = sources[0].Get(x, y, z, w) + Offset;
            double weight = Gain * value;
            x *= Lacunarity;
            y *= Lacunarity;
            z *= Lacunarity;
            w *= Lacunarity;

            for (int i = 1; i < octaves; ++i)
            {
                if (weight > 1.0)
                    weight = 1.0;

                double signal = (sources[i].Get(x, y, z, w) + Offset) * expArray[i];
                value += weight * signal;
                weight *= Gain * signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }

        private double HybridMulti_Get(double x, double y, double z, double w, double u, double v)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;
            w *= Frequency;
            u *= Frequency;
            v *= Frequency;

            double value = sources[0].Get(x, y, z, w, u, v) + Offset;
            double weight = Gain * value;
            x *= Lacunarity;
            y *= Lacunarity;
            z *= Lacunarity;
            w *= Lacunarity;
            u *= Lacunarity;
            v *= Lacunarity;

            for (int i = 1; i < octaves; ++i)
            {
                if (weight > 1.0)
                    weight = 1.0;

                double signal = (sources[i].Get(x, y, z, w, u, v) + Offset) * expArray[i];
                value += weight * signal;
                weight *= Gain * signal;
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                w *= Lacunarity;
                u *= Lacunarity;
                v *= Lacunarity;
            }

            return value * correct[octaves - 1, 0] + correct[octaves - 1, 1];
        }
    }
}
