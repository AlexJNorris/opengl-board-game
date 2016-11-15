using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using BoardCGame.Entity;
using BoardCGame.OpenGL;


namespace BoardCGame
{
    public class BoardGame : GameWindow
    {
        private View view;
        private Texture texture;

        public BoardGame(int width, int height) 
            : base(width, height)
        {
            GL.Enable(EnableCap.Texture2D);
            view = new View(Vector2.Zero);
            Mouse.ButtonDown += Mouse_ButtonDown;
        }

        private void Mouse_ButtonDown(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            Vector2 pos = new Vector2(e.Position.X, e.Position.Y);
            pos -= new Vector2(this.Width, this.Height)/2f;
            pos = view.ToWorld(pos);
            view.SetPosition(pos, TweenType.QuadraticInOut, 30);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            texture = TextureLoader.LoadTexture("path.png");
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
            view.Update();
        }

        private void Draw()
        {
            Drawer.Begin(this.Width, this.Height);
            view.ApplyTransform();
            
            Drawer.Draw(texture, Vector2.Zero, new Vector2(0.3f,0.3f), Color.White, new Vector2(10, 50));
        }
    }
}
