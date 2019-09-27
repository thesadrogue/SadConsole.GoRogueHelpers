using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitCos : ImplicitModuleBase
    {
        public ImplicitCos(ImplicitModuleBase source) => Source = source;

        public ImplicitModuleBase Source { get; set; }

        public override double Get(double x, double y) => Math.Cos(Source.Get(x, y));

        public override double Get(double x, double y, double z) => Math.Cos(Source.Get(x, y, z));

        public override double Get(double x, double y, double z, double w) => Math.Cos(Source.Get(x, y, z, w));

        public override double Get(double x, double y, double z, double w, double u, double v) => Math.Cos(Source.Get(x, y, z, w, u, v));
    }
}
