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


namespace BoardCGame
{
    public class BoardGame : GameWindow
    {
        private View view;
        private Texture _texture, _tileSet;
        private Board _board;
        private Player _player;


        private int cont = 0;

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

            _texture = TextureLoader.LoadTexture("path.png");
            _tileSet = TextureLoader.LoadTexture("Shockbolt_64x64_01.png");
            _board = new OpenGL.Board("Content/spritetestev3.tmx");
            _player = new BoardCGame.Player(new Vector2(
                (_board.PlayerStartPos.X + 0.5f) * Constants.GRIDSIZE,
                (_board.PlayerStartPos.Y + 0.5f) * Constants.GRIDSIZE));

            MouseMove += BoardGame_MouseMove;
            MouseWheel += BoardGame_MouseWheel;

            //Initialize();

            _player.Position = new Vector2(_board.PlayerStartPos.X / Constants.TILESETSIZE,
                                           _board.PlayerStartPos.Y / Constants.TILESETSIZE);
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

        private void BoardGame_MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            //rotx += e.XDelta;
            //roty += e.YDelta;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.White);
    
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(0, this.Width, this.Height, 0, -1, 4);

            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadIdentity();

            Draw();
            _player.Draw();
            //view.Camera.AddRotation(rotx, roty);           

            this.SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            float aspect = (float)Width / (float)Height;
            OpenTK.Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 1, 100f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            _player.Update();
            //view.SetPosition(_player.Position, TweenType.QuarticOut, 60);
     
            //v.ViewProjectionMatrix = cam.GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(1.3f, ClientSize.Width / (float)ClientSize.Height, 1.0f, 40.0f);

            if (Input.MousePress(OpenTK.Input.MouseButton.Left))
            {
                //Vector2 pos = new Vector2(Mouse.X, Mouse.Y) - new Vector2(this.Width, this.Height)/2f;
                //pos = view.ToWorld(pos);
                //view.SetPosition(pos, TweenType.QuarticOut, 15);
            }

            if (Input.KeyDown(OpenTK.Input.Key.Right))
            {
                view.SetPosition(view.PositionGoto + new Vector2(-5, 0), TweenType.QuarticOut, 15);
                _player.Position = _board.Paths.ElementAt(cont) != null ? new Vector2(_board.Paths.ElementAt(cont).X,
                                                                                 _board.Paths.ElementAt(cont).Y)
                                                                                 : _board.TestePos;
                cont++;
                _player.Update();
            }
            if (Input.KeyDown(OpenTK.Input.Key.Left))
            {
                view.SetPosition(view.PositionGoto + new Vector2(5, 0), TweenType.QuarticOut, 15);
            }

            if (Input.KeyDown(OpenTK.Input.Key.Up))
            {
                view.SetPosition(view.PositionGoto + new Vector2(0, 5), TweenType.QuarticOut, 15);
            }

            if (Input.KeyDown(OpenTK.Input.Key.Down))
            {
                view.SetPosition(view.PositionGoto + new Vector2(0, -5), TweenType.QuarticOut, 15);
            }

            view.Update();
            Input.Update();
            //view.Camera.AddRotation(rotx, roty);
        }

        private void Draw()
        {
            OpenGLDrawer.Begin(this.Width, this.Height);
            Matrix4 calculatedMatrix = view.GetTransformMatrix();
            //Matrix4 projectionMatrix = view.Camera.GetViewMatrix() * Matrix4.CreatePerspectiveFieldOfView(1.3f, ClientSize.Width / (float)ClientSize.Height, 1.0f, 40.0f);
            //Matrix4 modelViewProjection = calculatedMatrix * projectionMatrix;
            GL.MultMatrix(ref calculatedMatrix);

            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    RectangleF source = new RectangleF(0, 0, 0, 0);

                    switch (_board[x, y].Type)
                    {
                        case BlockType.Terrain:
                            source = new RectangleF(7 * Constants.TILESETSIZE, 23 * Constants.TILESETSIZE,
                                   Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case BlockType.TerrainBoard:
                            source = new RectangleF(0 * Constants.TILESETSIZE, 22 * Constants.TILESETSIZE,
                                   Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case BlockType.Path:
                            source = new RectangleF(110 * Constants.TILESETSIZE, 27 * Constants.TILESETSIZE,
                                   Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case BlockType.Teste:
                            source = new RectangleF(3 * Constants.TILESETSIZE, 23 * Constants.TILESETSIZE,
                                   Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case BlockType.StartPoint:
                            source = new RectangleF(74 * Constants.TILESETSIZE, 27 * Constants.TILESETSIZE,
                                  Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;

                        //case BlockType.LadderPlatform:
                        //    source = new RectangleF(3 * Constants.TILESETSIZE, 0 * Constants.TILESETSIZE,
                        //        Constants.TILESETSIZE, Constants.TILESETSIZE);
                        //    break;
                        //case BlockType.Solid:
                        //    source = new RectangleF(1 * Constants.TILESETSIZE, 0 * Constants.TILESETSIZE,
                        //        Constants.TILESETSIZE, Constants.TILESETSIZE);
                        //    break;
                        //case BlockType.Platform:
                        //    source = new RectangleF(0 * Constants.TILESETSIZE, 1 * Constants.TILESETSIZE,
                        //        Constants.TILESETSIZE, Constants.TILESETSIZE);
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
