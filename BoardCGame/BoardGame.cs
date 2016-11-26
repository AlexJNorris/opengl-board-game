using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using BoardCGame.Entity;
using BoardCGame.Entity.Enumerations;
using BoardCGame.OpenGL;
using BoardCGame.Util;
using OpenTK.Graphics;
using RectangleF = System.Drawing.Rectangle;
using Projeto_CG_1_GQ.Configuration;

namespace BoardCGame
{
    public class BoardGame : GameWindow
    {
        private View view;
        private Texture _cubeTexture, _tileSet, _teste;
        private Board _board;
        private Player _player;
        private Dice _dice;
        private int currentDiceFaceId;
        private System.Diagnostics.Stopwatch _watch;
        private float _angle;

        //

        private IInputHandler inputHandler = new InputHandler();
        float rx = 0;
        float ry = 0;
        bool rotate = false;
        //

        public BoardGame(int width, int height)
            : base(width, height, GraphicsMode.Default, "Board CG Game", GameWindowFlags.Default, DisplayDevice.Default, 2, 1, GraphicsContextFlags.Debug)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            view = new View(Vector2.Zero);
            view.Camera = new OpenGL.Camera();
            Input.Initialize(this);
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(-40, 200, 100, 0.0f));
            GL.Light(LightName.Light0, LightParameter.Ambient, new Vector4(0.2f, 0.2f, 0.2f, 1.0f));
            GL.Light(LightName.Light0, LightParameter.Diffuse, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Lighting);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.ColorMaterial);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            _dice = new Dice();
            RandomizeCube();

            _tileSet = TextureLoader.LoadTexture("ICONS_COMPLETE_1024x9.png");
            _board = new OpenGL.Board("Content/MapaJogo.tmx");
            _teste = TextureLoader.LoadTexture("lowPolyTree.png");

            _player = new BoardCGame.Player(new Vector2(
                (_board.PlayerStartPos.X) * Constants.GRIDSIZE,
                (_board.PlayerStartPos.Y) * Constants.GRIDSIZE));

            //MouseMove += BoardGame_MouseMove;
            MouseWheel += BoardGame_MouseWheel;

            //Initialize();

            _player.Position = new Vector2(_board.PlayerStartPos.X / Constants.TILESETSIZE,
                                           _board.PlayerStartPos.Y / Constants.TILESETSIZE);

            

            _watch = System.Diagnostics.Stopwatch.StartNew();



            view.ResetPosition();
        }

        private void Initialize()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 lookAt = Matrix4.LookAt(0, 0, 0, 0, 0, 0, 0, 1, 0);
            Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, this.Width / (float)this.Height, 1, 100f);
            GL.LoadMatrix(ref perspective);
            GL.LoadMatrix(ref lookAt);
            GL.Enable(EnableCap.DepthTest);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        private void BoardGame_MouseWheel(object sender, OpenTK.Input.MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                view.ZoomOut();
            }
            else
            {
                view.ZoomIn();
            }
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            _watch.Stop();
            float deltaElapsedTime = (float)_watch.ElapsedTicks/System.Diagnostics.Stopwatch.Frequency;
            _watch.Restart();
            _angle += deltaElapsedTime * 4;

            GL.Viewport(0, 0, Width, Height);
            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
           
            // DICE
            GL.LoadIdentity();
            GL.PushMatrix();

            Matrix4 calculatedMatrix = Matrix4.Identity;
            if (rotate)
            {
                calculatedMatrix = Matrix4.Mult(calculatedMatrix, Matrix4.CreateRotationX(_angle / 2));
                calculatedMatrix = Matrix4.Mult(calculatedMatrix, Matrix4.CreateRotationY(_angle));
            }

            calculatedMatrix = Matrix4.Mult(calculatedMatrix, Matrix4.CreateTranslation(new Vector3(-1, 0, -5)));
            calculatedMatrix = Matrix4.Mult(calculatedMatrix, Matrix4.CreateScale(.1f, .1f, .5f));
            GL.MultMatrix(ref calculatedMatrix);

            if (rotate)
            {
                _dice.Roll();
            }
            else
            {
                _dice.Stop(currentDiceFaceId);
            }

            GL.PopMatrix();


            GL.LoadIdentity();
            GL.PushMatrix();
            Matrix4 calculatedMatrix2 = Matrix4.Identity; 
            calculatedMatrix2 = Matrix4.Mult(calculatedMatrix2, Matrix4.CreateTranslation(new Vector3(-20, -30, -80)));
            GL.MultMatrix(ref calculatedMatrix2);


            Draw();
            _player.Draw();

            GL.PopMatrix();

            this.SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            float aspect = (float)Width / (float)Height;
            Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1f, 1000f); // Ultimos parametros ->  quão perto e quão longe, respectivamente, os objetos deixara de ser renderizado
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        private void RandomizeCube()
        {
            currentDiceFaceId = _dice.PickRandomSideTexture(new List<Texture>(_dice._textures));
            _dice._stoppedSides = _dice._textures.Where(t => t.Id != currentDiceFaceId).ToList();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            _player.Update();

            //view.SetPosition(_player.Position, TweenType.QuarticOut, 60);

            //v.ViewProjectionMatrix = cam.GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(1.3f, ClientSize.Width / (float)ClientSize.Height, 1.0f, 40.0f);

            if (Input.MousePress(OpenTK.Input.MouseButton.Left))
            {
                rotate = !rotate;
                if (!rotate) // Pare!
                {
                    RandomizeCube();
                }
                //Vector2 pos = new Vector2(Mouse.X, Mouse.Y) - new Vector2(this.Width, this.Height)/2f;
                //pos = view.ToWorld(pos);
                //view.SetPosition(pos, TweenType.QuarticOut, 15);
            }

            //if (Input.MousePress(OpenTK.Input.MouseButton.Right))
            //{
            //    rotate = false;
            //    //Vector2 pos = new Vector2(Mouse.X, Mouse.Y) - new Vector2(this.Width, this.Height)/2f;
            //    //pos = view.ToWorld(pos);
            //    //view.SetPosition(pos, TweenType.QuarticOut, 15);
            //}

            if (Input.KeyDown(OpenTK.Input.Key.Right))
            {
                view.SetPosition(view.PositionGoto + new Vector2(-5, 0), TweenType.QuarticOut, 60);
                //_player.Position = _board.Paths.ElementAt(cont) != null ? new Vector2(_board.Paths.ElementAt(cont).X,
                //                                                                 _board.Paths.ElementAt(cont).Y)
                //                                                                 : _board.TestePos;
                //cont++;
                //_player.Update();
            }
            if (Input.KeyDown(OpenTK.Input.Key.Left))
            {
                view.SetPosition(view.PositionGoto + new Vector2(5, 0), TweenType.QuarticOut, 60);
            }

            if (Input.KeyDown(OpenTK.Input.Key.Down))
            {
                view.SetPosition(view.PositionGoto + new Vector2(0, 5), TweenType.QuarticOut, 60);
            }

            if (Input.KeyDown(OpenTK.Input.Key.Up))
            {
                view.SetPosition(view.PositionGoto + new Vector2(0, -5), TweenType.QuarticOut, 60);
            }

            view.Update();
            Input.Update();
            //view.Camera.AddRotation(rotx, roty);
        }

        private void Draw()
        {
            Matrix4 calculatedMatrix = view.GetTransformMatrix();
            GL.MultMatrix(ref calculatedMatrix);
                  
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    RectangleF source = new RectangleF(0, 0, 0, 0);

                    switch (_board[x, y].Type)
                    {
                        case BlockType.TerrainBoard:
                            source = new RectangleF(1 * Constants.TILESETSIZE, 47 * Constants.TILESETSIZE,
                                   Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case BlockType.Path:
                            source = new RectangleF(2 * Constants.TILESETSIZE, 30 * Constants.TILESETSIZE,
                                   Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case BlockType.StartPoint:
                            source = new RectangleF(4 * Constants.TILESETSIZE, 38 * Constants.TILESETSIZE,
                                  Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case BlockType.EndPoint:
                            source = new RectangleF(5 * Constants.TILESETSIZE, 5 * Constants.TILESETSIZE,
                                 Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        default:
                            break;
                    }

                    OpenGLDrawer.Draw(_tileSet,
                        new Vector2(x * Constants.GRIDSIZE, y * Constants.GRIDSIZE),
                        new Vector2((float)Constants.GRIDSIZE / Constants.TILESETSIZE),
                        Color.Transparent,
                        Vector2.Zero,
                        source);
                }
            }


            //RectangleF playerSource = new RectangleF(0, 0, 0, 0);

            //// COORDS IMAGEM CORTADA
            //playerSource = new RectangleF(1 * Constants.TILESETSIZE, 3 * Constants.TILESETSIZE,
            //                      Constants.TILESETSIZE, Constants.TILESETSIZE);


            //OpenGLDrawer.Draw(_tileSet, new Vector2(3 * Constants.GRIDSIZE, 35 * Constants.GRIDSIZE),
            //           new Vector2((float)Constants.GRIDSIZE / Constants.TILESETSIZE),
            //           Color.Transparent, Vector2.Zero, playerSource);


        }
    }
}
