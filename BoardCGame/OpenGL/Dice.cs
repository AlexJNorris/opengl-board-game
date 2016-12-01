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
using BoardCGame.Entity.Enumerations;
using BoardCGame.OpenGL;

namespace BoardCGame
{
    public class Dice
    {
        private IList<DiceSide> _sides;
        private IList<Vector2> _textCoords;
        private IList<DiceTexture> _redTextures;
        private IList<DiceTexture> _greenTextures;

        public IList<DiceTexture> StoppedSides { get; set; }

        public IList<DiceTexture> RedTextures
        {
            get { return _redTextures; }
        }

        public IList<DiceTexture> GreenTextures
        {
            get { return _greenTextures; }
        }

        public IList<DiceTexture> Sides { get; set; }

        public Dice()
        {
            InitializeFaces();
            InitializeTextures();
        }

        private void InitializeTextures()
        {
            _redTextures = new List<DiceTexture>()
            {
                TextureLoader.LoadDiceTexture("1.2.png", EnumDiceTextureType.RedDice,1),
                TextureLoader.LoadDiceTexture("2.2.png", EnumDiceTextureType.RedDice,2),
                TextureLoader.LoadDiceTexture("3.2.png", EnumDiceTextureType.RedDice,3),
                TextureLoader.LoadDiceTexture("4.2.png", EnumDiceTextureType.RedDice,4),
                TextureLoader.LoadDiceTexture("5.2.png", EnumDiceTextureType.RedDice,5),
                TextureLoader.LoadDiceTexture("6.2.png", EnumDiceTextureType.RedDice,6)
            };

            _greenTextures = new List<DiceTexture>()
            {
                TextureLoader.LoadDiceTexture("1.png", EnumDiceTextureType.GreenDice,1),
                TextureLoader.LoadDiceTexture("2.png", EnumDiceTextureType.GreenDice,2),
                TextureLoader.LoadDiceTexture("3.png", EnumDiceTextureType.GreenDice,3),
                TextureLoader.LoadDiceTexture("4.png", EnumDiceTextureType.GreenDice,4),
                TextureLoader.LoadDiceTexture("5.png", EnumDiceTextureType.GreenDice,5),
                TextureLoader.LoadDiceTexture("6.png", EnumDiceTextureType.GreenDice,6)
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

        
        public DiceTexture PickRandomSideTexture(IList<DiceTexture> textures)
        {
            Random rnd = new Random();
            DiceTexture chosenTexture;
            if (textures.Count > 0)
            {
                chosenTexture = textures.OrderBy(t => t.FaceNumber).Skip(rnd.Next(0, textures.Count - 1)).Take(1).First();
            }
            else
            {
                chosenTexture = textures.First();
            }

            DiceTexture clone = (DiceTexture)chosenTexture.Clone();
            textures.Remove(chosenTexture);
            return clone;
        }

        public void Roll(EnumPlayer player)
        {
            IList<DiceTexture> textureBackup = new List<DiceTexture>(player == EnumPlayer.Player1 ? _redTextures
                                                                                          : _greenTextures);        
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

        private int GetNextTexture(IList<DiceTexture> textures)
        {
            var next = textures.First();
            int id = next.Id;
            textures.Remove(next);
            return id;
        }

        public void Stop(int textId)
        {
            IList<DiceSide> diceBackup = new List<DiceSide>(_sides);
            IList<DiceTexture> textureBackup = new List<DiceTexture>(this.StoppedSides);

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
