﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using BoardCGame.Entity;
using BoardCGame.Entity.Enumerations;

namespace BoardCGame.OpenGL
{
    public class TextureLoader
    {
        public static Texture LoadTexture(string fileName)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap("Content/" + fileName);
            BitmapData data = bmp.LockBits(new Rectangle(0,0,bmp.Width,bmp.Height),
                                           ImageLockMode.ReadOnly,
                                           System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
                          data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, 
                          PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            return new Texture(id, bmp.Width, bmp.Height);
        }

        public static DiceTexture LoadDiceTexture(string fileName, EnumDiceTextureType diceTextureType, int faceNumber)
        {
            Texture texture = LoadTexture(fileName);
            return new DiceTexture(texture.Id, texture.Width, texture.Height, diceTextureType, faceNumber);
        }
    }
}
