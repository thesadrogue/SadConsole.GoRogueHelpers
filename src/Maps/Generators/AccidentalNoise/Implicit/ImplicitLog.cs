using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitLog : ImplicitModuleBase
    {
        public ImplicitLog(ImplicitModuleBase source) => Source = source;

        public ImplicitModuleBase Source { set; get; }

        public override double Get(double x, double y) => Math.Log(Source.Get(x, y));

        public override double Get(double x, double y, double z) => Math.Log(Source.Get(x, y, z));

        public override double Get(double x, double y, double z, double w) => Math.Log(Source.Get(x, y, z, w));

        public override double Get(double x, double y, double z, double w, double u, double v) => Math.Log(Source.Get(x, y, z, w, u, v));
    }
}
