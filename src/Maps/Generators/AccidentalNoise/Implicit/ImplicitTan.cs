using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitTan : ImplicitModuleBase
    {
        public ImplicitTan(ImplicitModuleBase source) => Source = source;

        public ImplicitModuleBase Source { get; set; }

        public override double Get(double x, double y) => Math.Tan(Source.Get(x, y));

        public override double Get(double x, double y, double z) => Math.Tan(Source.Get(x, y, z));

        public override double Get(double x, double y, double z, double w) => Math.Tan(Source.Get(x, y, z, w));

        public override double Get(double x, double y, double z, double w, double u, double v) => Math.Tan(Source.Get(x, y, z, w, u, v));
    }
}
