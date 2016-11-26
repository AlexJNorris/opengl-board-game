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
using OpenTK.Input;
using BoardCGame.OpenGL;
using BoardCGame.Util;


namespace BoardCGame
{
    public class Player
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Velocity { get; set; }

        private Texture _playerSprite;
        private bool _climbing, _facingRight, _onLadder, _grounded;

        //Retangulo de colisão
        public RectangleF ColRec
        {
            get
            {
                return new RectangleF(Position.X - Size.X/2f, Position.Y - Size.Y/2f, Size.X, Size.Y);
            }
        }

        public RectangleF DrawRec
        {
            get
            {
                RectangleF colRec = ColRec;
                colRec.X = colRec.X - 5;
                colRec.Width = colRec.Width + 10;
                return colRec;
            }
        }

        public Player(Vector2 startPos)
        {
            Position = startPos;
            Velocity = Vector2.Zero;
            _facingRight = true;
            Size = new Vector2(40, 32);
            _playerSprite = TextureLoader.LoadTexture("player1walkingRight2.png");
        }

        public void Update()
        {
            HandleInput();

            //Velocity += new Vector2(0, 0.5f);
            Position += Velocity;
            ResolveCollision();
        }

        public void HandleInput()
        {
            
        }

        public void ResolveCollision()
        {
            
        }

        public void Draw()
        {
            OpenGLDrawer.Draw(_playerSprite,
                new Vector2(Position.X*Constants.GRIDSIZE, -Position.Y*Constants.GRIDSIZE),
                new Vector2(DrawRec.Width/_playerSprite.Width, DrawRec.Height/_playerSprite.Height),
                Color.Transparent,
                Vector2.Zero,//new Vector2(_playerSprite.Width/4f, _playerSprite.Height/2f),
                new RectangleF(0, 0,_playerSprite.Width, _playerSprite.Height));
        }
    }
}
