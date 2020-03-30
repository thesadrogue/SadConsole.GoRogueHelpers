using System;

namespace TinkerWorX.AccidentalNoiseLibrary
{
    public sealed class ImplicitSphere : ImplicitModuleBase
    {
        public ImplicitSphere(
            double xCenter = 0.00, double yCenter = 0.00, double zCenter = 0.00,
            double wCenter = 0.00, double uCenter = 0.00, double vCenter = 0.00,
            double radius = 1.00)
        {
            XCenter = new ImplicitConstant(xCenter);
            YCenter = new ImplicitConstant(yCenter);
            ZCenter = new ImplicitConstant(zCenter);
            WCenter = new ImplicitConstant(wCenter);
            UCenter = new ImplicitConstant(uCenter);
            VCenter = new ImplicitConstant(vCenter);
            Radius = new ImplicitConstant(radius);
        }

        public ImplicitModuleBase XCenter { get; set; }

        public ImplicitModuleBase YCenter { get; set; }

        public ImplicitModuleBase ZCenter { get; set; }

        public ImplicitModuleBase WCenter { get; set; }

        public ImplicitModuleBase UCenter { get; set; }

        public ImplicitModuleBase VCenter { get; set; }

        public ImplicitModuleBase Radius { get; set; }

        public override double Get(double x, double y)
        {
            double dx = x - XCenter.Get(x, y);
            double dy = y - YCenter.Get(x, y);
            double len = Math.Sqrt(dx * dx + dy * dy);
            double rad = Radius.Get(x, y);
            double i = (rad - len) / rad;
            if (i < 0)
                i = 0;

            if (i > 1)
                i = 1;

            return i;
        }

        public override double Get(double x, double y, double z)
        {
            double dx = x - XCenter.Get(x, y, z);
            double dy = y - YCenter.Get(x, y, z);
            double dz = z - ZCenter.Get(x, y, z);
            double len = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            double rad = Radius.Get(x, y, z);
            double i = (rad - len) / rad;
            if (i < 0)
                i = 0;

            if (i > 1)
                i = 1;

            return i;
        }

        public override double Get(double x, double y, double z, double w)
        {
            double dx = x - XCenter.Get(x, y, z, w);
            double dy = y - YCenter.Get(x, y, z, w);
            double dz = z - ZCenter.Get(x, y, z, w);
            double dw = w - WCenter.Get(x, y, z, w);
            double len = Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw);
            double rad = Radius.Get(x, y, z, w);
            double i = (rad - len) / rad;
            if (i < 0)
                i = 0;

            if (i > 1)
                i = 1;

            return i;
        }

        public override double Get(double x, double y, double z, double w, double u, double v)
        {
            double dx = x - XCenter.Get(x, y, z, w, u, v);
            double dy = y - YCenter.Get(x, y, z, w, u, v);
            double dz = z - ZCenter.Get(x, y, z, w, u, v);
            double dw = w - WCenter.Get(x, y, z, w, u, v);
            double du = u - UCenter.Get(x, y, z, w, u, v);
            double dv = v - VCenter.Get(x, y, z, w, u, v);
            double len = Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw + du * du + dv * dv);
            double rad = Radius.Get(x, y, z, w, u, v);
            double i = (rad - len) / rad;
            if (i < 0)
                i = 0;

            if (i > 1)
                i = 1;

            return i;
        }
    }
}
