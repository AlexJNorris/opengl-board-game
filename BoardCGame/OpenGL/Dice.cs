using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using BoardCGame.Entity;
using BoardCGame.OpenGL;

namespace BoardCGame
{
    public class Dice
    {
        public IList<DiceSide> _sides;
        private IList<Vector2> _textCoords;
        public IList<Texture> _stoppedSides;
        public IList<Texture> _textures;

        public Dice()
        {
            InitializeFaces();
            InitializeTextures();
        }

        private void InitializeTextures()
        {
            _textures = new List<Texture>()
            {
                TextureLoader.LoadTexture("1.2.png"),
                TextureLoader.LoadTexture("2.2.png"),
                TextureLoader.LoadTexture("3.2.png"),
                TextureLoader.LoadTexture("4.2.png"),
                TextureLoader.LoadTexture("5.2.png"),
                TextureLoader.LoadTexture("6.2.png")
            };


            _textCoords = new List<Vector2>()
        {
              new Vector2(0, 0),
              new Vector2(1, 0),
              new Vector2(1, 1),
              new Vector2(0, 1)
        };
        }

        private void InitializeFaces()
        {
            _sides = new List<DiceSide>()
            {
                new DiceSide(new List<Vector3>()
                {
                    new Vector3(-1, 1, 1),
                    new Vector3(1, 1, 1),
                    new Vector3(1, -1, 1),
                    new Vector3(-1, -1, 1)
                }),
                new DiceSide(new List<Vector3>()
                {
                    new Vector3(1, 1, -1),
                    new Vector3(-1, 1, -1),
                    new Vector3(-1, -1, -1),
                    new Vector3(1, -1, -1)
                }),
                new DiceSide(new List<Vector3>()
                {
                    new Vector3(1, 1, 1),
                    new Vector3(1, 1, -1),
                    new Vector3(1, -1, -1),
                    new Vector3(1, -1, 1)
                }),
                new DiceSide(new List<Vector3>()
                {
                    new Vector3(-1, 1, -1),
                    new Vector3(-1, -1, -1),
                    new Vector3(-1, -1, 1),
                    new Vector3(-1, 1, 1)
                }),
                new DiceSide(new List<Vector3>()
                {
                    new Vector3(-1, 1, -1),
                    new Vector3(1, 1, -1),
                    new Vector3(1, 1, 1),
                    new Vector3(-1, 1, 1)
                }),
                new DiceSide(new List<Vector3>()
                {
                    new Vector3(-1, -1, -1),
                    new Vector3(-1, -1, 1),
                    new Vector3(1, -1, 1),
                    new Vector3(1, -1, -1)
                }),
            };
        }

        private static IList<Vector3> faces = new List<Vector3>() {
                new Vector3(1, 1, -1), new Vector3(-1, 1, -1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1),
                new Vector3(1, -1, 1), new Vector3(-1, -1, 1), new Vector3(-1, -1, -1), new Vector3(1, -1, -1),
                new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(-1, -1, 1), new Vector3(1, -1, 1),
                new Vector3(1, -1, -1), new Vector3(-1, -1, -1), new Vector3(-1, 1, -1), new Vector3(1, 1, -1),
                new Vector3(-1, 1, 1), new Vector3(-1, 1, -1), new Vector3(-1, -1, -1), new Vector3(-1, -1, 1),
                new Vector3(1, 1, -1), new Vector3(1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, -1, -1) };


        public int PickRandomSideTexture(IList<Texture> textures)
        {
            Random rnd = new Random();
            Texture chosenTexture;
            if (textures.Count > 0)
            {
                chosenTexture = textures.OrderBy(t => t.Id).Skip(rnd.Next(0, textures.Count - 1)).Take(1).First();
            }
            else
            {
                chosenTexture = textures.First();
            }
            int chosenId = chosenTexture.Id;
            textures.Remove(chosenTexture);
            return chosenId;
        }

        public void Roll()
        {
            IList<Texture> textureBackup = new List<Texture>(_textures);
            foreach (var side in _sides.Select((value, i) => new { i, value }))
            {
                int textId = textureBackup.ElementAt(side.i).Id;
                GL.BindTexture(TextureTarget.Texture2D, textId);
                GL.Begin(BeginMode.Quads);
                for (int j = 0; j < 4; j++)
                {
                    GL.TexCoord2(_textCoords.ElementAt(j));
                    GL.Vertex3(side.value.VerticesCords.ElementAt(j));
                }

                GL.End();
            }
        }

        private int GetNextTexture(IList<Texture> textures)
        {
            var next = textures.First();
            int id = next.Id;
            textures.Remove(next);
            return id;
        }

        public void Stop(int textId)
        {
            IList<DiceSide> diceBackup = new List<DiceSide>(_sides);
            IList<Texture> textureBackup = new List<Texture>(_stoppedSides);

            var side1 = _sides.First();
            GL.BindTexture(TextureTarget.Texture2D, textId);
            GL.Begin(BeginMode.Quads);

            for (int j = 0; j < 4; j++)
            {
                GL.TexCoord2(_textCoords.ElementAt(j));
                GL.Vertex3(side1.VerticesCords.ElementAt(j));
            }

            diceBackup.Remove(_sides.First());

            foreach (var side in _sides.Skip(1).Select((value, i) => new { i, value }))
            {
                textId = GetNextTexture(textureBackup);
                GL.BindTexture(TextureTarget.Texture2D, textId);
                GL.Begin(BeginMode.Quads);

                for (int j = 0; j < 4; j++)
                {
                    GL.TexCoord2(_textCoords.ElementAt(j));
                    GL.Vertex3(side.value.VerticesCords.ElementAt(j));
                }

                GL.End();
            }
        }


    }
}
