using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace BoardCGame.Entity
{
    public class View
    {
        /// <summary>
        /// Posicao na tela
        /// </summary>
        private Vector2 _position;

        public Vector2 Position
        {
            get { return _position; }
        }

        public Vector2 PositionGoto
        {
            get { return _positionGoto; }
        }

        private Vector2 _positionGoto, _positionFrom;
        private TweenType _tweenType;
        private int _currentStep, _tweenSteps;

        /// <summary>
        /// 1 = Sem zoom
        /// 2 = 2x Zoom
        /// </summary>
        public double Zoom { get; set; }

        /// <summary>
        /// Em radianos, clockwise
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        /// Fazendo caminho reverso da funcao de transformacao
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Vector2 ToWorld(Vector2 input)
        {
            input /= (float) Zoom;
            Vector2 dx = new Vector2((float) Math.Cos(Rotation), (float) Math.Sin(Rotation));
            Vector2 dy = new Vector2((float)Math.Cos(Rotation + MathHelper.PiOver2), (float)Math.Sin(Rotation + MathHelper.PiOver2));
            return (this.Position + dx*input.X + dy*input.Y);
        }

        public View(Vector2 startPosition, double startZoom = 1.0, double startRotation = 0.0)
        {
            _position = startPosition;
            Zoom = startZoom;
            Rotation = startRotation;
        }

        public void Update()
        {
            if (_currentStep < _tweenSteps)
            {
                _currentStep++;
                switch (_tweenType)
                {
                    case TweenType.Linear:
                        _position = _positionFrom +
                                    (_positionGoto - _positionFrom)*GetLinear((float) _currentStep/_tweenSteps);
                        break;
                    case TweenType.QuadraticInOut:
                        _position = _positionFrom +
                                    (_positionGoto - _positionFrom) * GetQuadraticInOut((float)_currentStep / _tweenSteps);
                        break;
                    case TweenType.CubicInOut:
                        _position = _positionFrom +
                                    (_positionGoto - _positionFrom) * GetCubicInOut((float)_currentStep / _tweenSteps);
                        break;
                    case TweenType.QuarticOut:
                        _position = _positionFrom +
                                    (_positionGoto - _positionFrom) * GetQuarticOut((float)_currentStep / _tweenSteps);
                        break;
                }
            }
            else
            {
                _position = _positionGoto;
            }
        }


        public void SetPosition(Vector2 newPosition)
        {
            _position = newPosition;
            _positionFrom = newPosition;
            _positionGoto = newPosition;
            _tweenType = TweenType.Instant;
            _currentStep = 0;
            _tweenSteps = 0;
        }

        public void SetPosition(Vector2 newPosition, TweenType type, int numSteps)
        {
            _positionFrom = _position;
            _position = newPosition;
            _positionGoto = newPosition;
            _tweenType = type;
            _currentStep = 0;
            _tweenSteps = numSteps;
        }

        public void ApplyTransform()
        {
            Matrix4 transform = Matrix4.Identity;
            transform = Matrix4.Mult(transform, Matrix4.CreateTranslation(-Position.X, -Position.Y, 0));
            transform = Matrix4.Mult(transform, Matrix4.CreateRotationZ(-(float)Rotation));
            transform = Matrix4.Mult(transform, Matrix4.CreateScale((float) Zoom, (float) Zoom, 1.0f));

            GL.MultMatrix(ref transform);
        }

        public float GetLinear(float t)
        {
            return t;
        }

        public float GetQuadraticInOut(float t)
        {
            return (t * t) / ((2 * t * t) - (2 * t) + 1);
        }

        public float GetCubicInOut(float t)
        {
            return (t * t * t) / ((3 * t * t) - (3 * t) + 1);
        }

        public float GetQuarticOut(float t)
        {
            return -((t - 1) * (t - 1) * (t - 1) * (t - 1)) + 1;
        }

    }
}
