using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_CG_1_GQ.Configuration.Camera
{
    public interface ICamera
    {
        double X { get; }
        double Y { get; }
        double Z { get; }
        double XFocus { get; }
        double YFocus { get; }
        double ZFocus { get; }

        void Apply(ICameraLookable lookable);

        void Up(int interval);
        void Down(int interval);
        void Left(int interval);
        void Right(int interval);

        void Forward(int interval);
        void Backward(int interval);
        void PadLeft(int interval);
        void PadRight(int interval);
    }
}
