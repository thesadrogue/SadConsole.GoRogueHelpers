using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitSin : ImplicitModuleBase
    {
        public ImplicitSin(ImplicitModuleBase source) => Source = source;

        public ImplicitModuleBase Source { get; set; }

        public override double Get(double x, double y) => Math.Sin(Source.Get(x, y));

        public override double Get(double x, double y, double z) => Math.Sin(Source.Get(x, y, z));

        public override double Get(double x, double y, double z, double w) => Math.Sin(Source.Get(x, y, z, w));

        public override double Get(double x, double y, double z, double w, double u, double v) => Math.Sin(Source.Get(x, y, z, w, u, v));
    }
}
