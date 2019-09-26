namespace TinkerWorX.AccidentalNoiseLibrary
{
    public abstract class ImplicitModuleBase
    {
        public virtual int Seed { get; set; }

        public virtual double Get(double x, double y) => 0.00;

        public virtual double Get(double x, double y, double z) => 0.00;

        public virtual double Get(double x, double y, double z, double w) => 0.00;

        public virtual double Get(double x, double y, double z, double w, double u, double v) => 0.00;

        public static implicit operator ImplicitModuleBase(double value) => new ImplicitConstant(value);
    }
}
