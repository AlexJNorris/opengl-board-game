using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projeto_CG_1_GQ.Configuration.Camera;
using System.Diagnostics;
using OpenTK.Input;

namespace Projeto_CG_1_GQ.Configuration
{
    public class InputHandler : IInputHandler
    {
        public ICamera Camera { get; set; }

        public ICameraLookable Lookable { get; set; }

        public InputHandler()
        {
            Camera = new Camera.OpenGLImpl.Camera();
            Lookable = new Camera.OpenGLImpl.OpenGLCameraLookable();
        }

        public void Execute()
        {
            Camera.Apply(Lookable);
        }

        public void Handle(Key keys)
        {
            if (keys == Key.W)
                Camera.Forward(53);

            if (keys == Key.S)
                Camera.Backward(53);

            if (keys == Key.A)
                Camera.PadLeft(53);

            if (keys == Key.D)
                Camera.PadRight(53);

            if (keys == Key.Up)
                Camera.Up(53);
            if (keys == Key.Left)
                Camera.Left(53);
            if (keys == Key.Right)
                Camera.Right(53);
            if (keys == Key.Down)
                Camera.Down(53);

            Debug.WriteLine(string.Format("X: {0}; Y: {1}; Z: {2}; XFocus: {3}; YFocus: {4}, ZFocus: {5}", Camera.X, Camera.Y, Camera.Z, Camera.X + Camera.XFocus, Camera.Y + Camera.YFocus, Camera.Z + Camera.ZFocus));
        }
    }
}
