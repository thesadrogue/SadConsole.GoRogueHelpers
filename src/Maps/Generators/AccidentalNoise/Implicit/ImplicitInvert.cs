namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitInvert : ImplicitModuleBase
    {
        public ImplicitInvert(ImplicitModuleBase source) => Source = source;

        public ImplicitModuleBase Source { set; get; }

        public override double Get(double x, double y) => -Source.Get(x, y);

        public override double Get(double x, double y, double z) => -Source.Get(x, y, z);

        public override double Get(double x, double y, double z, double w) => -Source.Get(x, y, z, w);

        public override double Get(double x, double y, double z, double w, double u, double v) => -Source.Get(x, y, z, w, u, v);
    }
}
