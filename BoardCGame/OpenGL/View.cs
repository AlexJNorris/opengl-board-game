using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardCGame.Entity;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace BoardCGame.OpenGL
{
    public class View
    {
        private Vector3 _position;
        public Vector3 Position
        {
            get { return _position; }
        }

        private Vector3 _positionGoto;
        public Vector3 PositionGoto
        {
            get { return _positionGoto; }
        }

        public Camera Camera { get; set; }

        private EnumAnimationType _enumAnimationType;

        public double Zoom { get; set; }

        /// <summary>
        /// Em radianos, clockwise
        /// </summary>
        public double Rotation { get; set; }

        public View(Vector3 startPosition, double startZoom = .1f, double startRotation = 0.0)
        {
            _position = startPosition;
            Zoom = startZoom;
            Rotation = startRotation;
        }


        public void SetPosition(Vector3 newPosition)
        {
            _position = newPosition;
            _positionGoto = newPosition;
            _enumAnimationType = EnumAnimationType.Instant;
        }

        public void SetPosition(Vector3 newPosition, EnumAnimationType type, int numSteps)
        {
            _position = newPosition;
            _positionGoto = newPosition;
            _enumAnimationType = type;
        }

        public Matrix4 GetTransformMatrix()
        {
            Matrix4 transform = Matrix4.Identity;
            transform = Matrix4.Mult(transform, Matrix4.CreateTranslation(Position.X, Position.Y, Position.Z));
            transform = Matrix4.Mult(transform, Matrix4.CreateRotationZ(-(float)Rotation));
            transform = Matrix4.Mult(transform, Matrix4.CreateScale((float) Zoom, (float) Zoom, 1f));

            GL.MultMatrix(ref transform);
            return transform;
        }

        public void ResetPosition()
        {
            _position.X = -1015;
            _position.Y = -870;
            _position.Z = -5f;
            _positionGoto = Position;
            Zoom = .03f;
        }

        #region Camera

        public void ZoomIn()
        {
            Zoom += .005;
        }

        public void ZoomOut()
        {
            if (Zoom >= 0.01) // Limitando zoom
            {
                Zoom -= .005;
            }
        }

        #endregion    
    }
}
