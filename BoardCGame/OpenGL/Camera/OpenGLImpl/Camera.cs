using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_CG_1_GQ.Configuration.Camera.OpenGLImpl
{
    public class Camera : ICamera
    {
        const double LOOK_CONST = 0.01;
        const double MOVE_CONST = 0.0025;
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public double XFocus { get; set; }

        public double YFocus { get; set; }

        public double ZFocus { get; set; }

        private double horizontalAngle;

        public Camera()
        {
            horizontalAngle = 0;
            X = 0;
            Y = 0;
            Z = 5;
            XFocus = Math.Sin(horizontalAngle);
            YFocus = 0;
            ZFocus = -Math.Cos(horizontalAngle);
        }

        public void Apply(ICameraLookable lookable)
        {
            lookable.Look(new CameraPosition(this));
        }

        public void Forward(int interval)
        {
            X += XFocus * LOOK_CONST * interval;
            Z += ZFocus * LOOK_CONST * interval;
            Y += YFocus * LOOK_CONST * interval;
        }

        public void Backward(int interval)
        {
            X -= XFocus * LOOK_CONST * interval;
            Z -= ZFocus * LOOK_CONST * interval;
            Y -= YFocus * LOOK_CONST * interval;
        }

        public void PadLeft(int interval)
        {
            X += ZFocus * LOOK_CONST * interval;
            Z -= XFocus * LOOK_CONST * interval;
        }

        public void PadRight(int interval)
        {
            X -= ZFocus * LOOK_CONST * interval;
            Z += XFocus * LOOK_CONST * interval;
        }

        // Visão
        public void Down(int interval)
        {
            if (YFocus > -1)
            {
                YFocus -= MOVE_CONST * interval;
            }
        }

        public void Left(int interval)
        {
            horizontalAngle -= MOVE_CONST * interval;
            XFocus = Math.Sin(horizontalAngle);
            ZFocus = -Math.Cos(horizontalAngle);
        }

        public void Right(int interval)
        {
            horizontalAngle += MOVE_CONST * interval;
            XFocus = Math.Sin(horizontalAngle);
            ZFocus = -Math.Cos(horizontalAngle);
        }

        public void Up(int interval)
        {
            if (YFocus < 1)
            {
                YFocus += MOVE_CONST * interval;
            }
        }
    }
}
