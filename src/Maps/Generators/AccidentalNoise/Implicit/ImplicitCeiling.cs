using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitCeiling : ImplicitModuleBase
    {
        public ImplicitCeiling(ImplicitModuleBase source) => Source = source;

        public ImplicitModuleBase Source { get; set; }

        public override double Get(double x, double y) => Math.Ceiling(Source.Get(x, y));

        public override double Get(double x, double y, double z) => Math.Ceiling(Source.Get(x, y, z));

        public override double Get(double x, double y, double z, double w) => Math.Ceiling(Source.Get(x, y, z, w));

        public override double Get(double x, double y, double z, double w, double u, double v) => Math.Ceiling(Source.Get(x, y, z, w, u, v));
    }
}
