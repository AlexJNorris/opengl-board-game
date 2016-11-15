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
using RectangleF = System.Drawing.Rectangle;

namespace BoardCGame
{
    public class BoardGame : GameWindow
    {
        private View view;
        private Texture _texture, _tileSet;
        private Board _board;

        public BoardGame(int width, int height)
            : base(width, height)
        {
            GL.Enable(EnableCap.Texture2D);
            view = new View(Vector2.Zero);
            Input.Initialize(this);
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _texture = TextureLoader.LoadTexture("path.png");
            _tileSet = TextureLoader.LoadTexture("tileset.png");
            //_board = new Board(20, 20);
            _board = new OpenGL.Board("Content/BoardGame.tmx");

            
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.AntiqueWhite);

            Draw();

            this.SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (Input.MousePress(OpenTK.Input.MouseButton.Left))
            {
                Vector2 pos = new Vector2(Mouse.X, Mouse.Y) - new Vector2(this.Width, this.Height)/2f;
                pos = view.ToWorld(pos);
                view.SetPosition(pos, TweenType.QuarticOut, 15);
            }

            if (Input.KeyDown(OpenTK.Input.Key.Right))
            {
                view.SetPosition(view.PositionGoto + new Vector2(-5, 0), TweenType.QuarticOut, 15);
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
        }

        private void Draw()
        {
            Drawer.Begin(this.Width, this.Height);
            view.ApplyTransform();

            
            

            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    RectangleF source = new RectangleF(0, 0, 0, 0);

                    switch (_board[x, y].Type)
                    {
                        case BlockType.Ladder:
                            source = new RectangleF(2 * Constants.TILESETSIZE, 0 * Constants.TILESETSIZE,
                                Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case BlockType.LadderPlatform:
                            source = new RectangleF(3 * Constants.TILESETSIZE, 0 * Constants.TILESETSIZE,
                                Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case BlockType.Solid:
                            source = new RectangleF(1 * Constants.TILESETSIZE, 0 * Constants.TILESETSIZE,
                                Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case BlockType.Platform:
                            source = new RectangleF(0 * Constants.TILESETSIZE, 1 * Constants.TILESETSIZE,
                                Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;

                    }
                    //Matrix4 matrixProjection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1f, 100f);

                    //GL.Viewport(0, 0, this.Width, this.Height);
                    //GL.MatrixMode(MatrixMode.Projection);
                    //GL.LoadMatrix(ref matrixProjection);
                    //GL.MatrixMode(MatrixMode.Modelview);
                    //GL.LoadIdentity();

                    //Vector3 m_eye = new Vector3(0f, 0, 100);
                    //Vector3 target = new Vector3(0f, 20f, 0f);
                    //Vector3 up = new Vector3(0f, 1f, 0f);

                    //matrixProjection = Matrix4.LookAt(m_eye, target, up);
                    //GL.LoadMatrix(ref matrixProjection);

                    Drawer.Draw(_tileSet, new Vector2(x*Constants.GRIDSIZE, y*Constants.GRIDSIZE),
                        new Vector2((float) Constants.GRIDSIZE/Constants.TILESETSIZE),
                        Color.White, Vector2.Zero, source);

                    

                }
            }
        }
    }
}
