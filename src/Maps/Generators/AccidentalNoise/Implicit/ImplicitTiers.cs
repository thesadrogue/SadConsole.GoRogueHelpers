using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitTiers : ImplicitModuleBase
    {
        public ImplicitTiers(ImplicitModuleBase source, int tiers = 0, bool smooth = true)
        {
            Source = source;
            Tiers = tiers;
            Smooth = smooth;
        }

        public ImplicitModuleBase Source { get; set; }

        public int Tiers { get; set; }

        public bool Smooth { get; set; }

        public override double Get(double x, double y)
        {
            int numsteps = Tiers;
            if (Smooth)
            {
                --numsteps;
            }

            double val = Source.Get(x, y);
            double tb = Math.Floor(val * numsteps);
            double tt = tb + 1.0;
            double t = val * numsteps - tb;
            tb /= numsteps;
            tt /= numsteps;
            double u = (Smooth ? Utilities.QuinticBlend(t) : 0.0);
            return tb + u * (tt - tb);
        }

        public override double Get(double x, double y, double z)
        {
            int numsteps = Tiers;
            if (Smooth)
            {
                --numsteps;
            }

            double val = Source.Get(x, y, z);
            double tb = Math.Floor(val * numsteps);
            double tt = tb + 1.0;
            double t = val * numsteps - tb;
            tb /= numsteps;
            tt /= numsteps;
            double u = (Smooth ? Utilities.QuinticBlend(t) : 0.0);
            return tb + u * (tt - tb);
        }

        public override double Get(double x, double y, double z, double w)
        {
            int numsteps = Tiers;
            if (Smooth)
            {
                --numsteps;
            }

            double val = Source.Get(x, y, z, w);
            double tb = Math.Floor(val * numsteps);
            double tt = tb + 1.0;
            double t = val * numsteps - tb;
            tb /= numsteps;
            tt /= numsteps;
            double u = (Smooth ? Utilities.QuinticBlend(t) : 0.0);
            return tb + u * (tt - tb);
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            int numsteps = Tiers;
            if (Smooth)
            {
                --numsteps;
            }

            double val = Source.Get(x, y, z, w, u, v);
            double tb = Math.Floor(val * numsteps);
            double tt = tb + 1.0;
            double t = val * numsteps - tb;
            tb /= numsteps;
            tt /= numsteps;
            double s = (Smooth ? Utilities.QuinticBlend(t) : 0.0);
            return tb + s * (tt - tb);
        }
    }
}
