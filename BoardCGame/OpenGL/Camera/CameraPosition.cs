using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_CG_1_GQ.Configuration.Camera
{
    public class CameraPosition
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double XFocus { get; set; }
        public double YFocus { get; set; }
        public double ZFocus { get; set; }

        public CameraPosition(ICamera camera)
        {
            X = camera.X;
            Y = camera.Y;
            Z = camera.Z;
            ZFocus = camera.ZFocus;
            YFocus = camera.YFocus;
            XFocus = camera.XFocus;
        }

    }
}
