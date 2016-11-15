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

namespace BoardCGame.OpenGL
{
    public class Drawer
    {
        public static void Draw(Texture texture, Vector2 position, Vector2 scale, Color color, Vector2 origin, Rectangle? sourceRec = null)
        {
            Vector2[] vertices = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };

            GL.BindTexture(TextureTarget.Texture2D, texture.Id);
            GL.Begin(PrimitiveType.Quads);

            GL.Color3(color);
            for (int i = 0; i < 4; i++)
            {
                if (!sourceRec.HasValue)
                {
                    GL.TexCoord2(vertices[i]);
                }
                else
                {
                    //Mantendo entre 0 e 1
                    GL.TexCoord2((sourceRec.Value.Left + vertices[i].X * sourceRec.Value.Width) / texture.Width,
                                 (sourceRec.Value.Top + vertices[i].Y * sourceRec.Value.Height) / texture.Height);
                }
                vertices[i].X *= (sourceRec == null) ? texture.Width : sourceRec.Value.Width;
                vertices[i].Y *= (sourceRec == null) ? texture.Height : sourceRec.Value.Height;
                vertices[i] -= origin;
                vertices[i] *= scale;
                vertices[i] += position;

                GL.Vertex2(vertices[i]);
            }

            GL.End();
        }

        public static void Begin(int screenWidth, int screenHeight)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-screenWidth/2f, screenWidth/2f, screenHeight/2f, -screenHeight/2 , 0f, 1f);
        }
    }
}
