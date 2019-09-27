namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitConstant : ImplicitModuleBase
    {
        public ImplicitConstant() => Value = 0.00;

        public ImplicitConstant(double value) => Value = value;

        public double Value { get; set; }

        public override double Get(double x, double y) => Value;

        public override double Get(double x, double y, double z) => Value;

        public override double Get(double x, double y, double z, double w) => Value;

        public override double Get(double x, double y, double z, double w, double u, double v) => Value;
    }
}
