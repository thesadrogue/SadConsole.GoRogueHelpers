using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitFloor : ImplicitModuleBase
    {
        public ImplicitFloor() => Source = new ImplicitConstant(0.00);

        public ImplicitFloor(ImplicitModuleBase source) => Source = source;

        public ImplicitModuleBase Source { get; set; }

        public override double Get(double x, double y) => Math.Floor(Source.Get(x, y));

        public override double Get(double x, double y, double z) => Math.Floor(Source.Get(x, y, z));

        public override double Get(double x, double y, double z, double w) => Math.Floor(Source.Get(x, y, z, w));

        public override double Get(double x, double y, double z, double w, double u, double v) => Math.Floor(Source.Get(x, y, z, w, u, v));
    }
}
