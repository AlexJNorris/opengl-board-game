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
        public Vector2 PositionToCut { get; set; }

        private Texture _playerSprite;
        public int CurrentPathIndex { get; set; }

        private Block _currentBlock;
        

        public Block CurrentBlock
        {
            get { return _currentBlock; }
        }

        private Texture _playerIconTexture;

        public Texture PlayerIconTexture
        {
            get { return _playerIconTexture; }
        }

        public EnumPlayer Name { get; set; }

        public bool Penalized { get; set; }
        public bool DoubleDice { get; set; }
        public bool NegativeDice { get; set; }
        public bool DoubleNegativeDice { get; set; }
        public bool Won { get; set; }
        

        //Retangulo de colisão
        public RectangleF ColRec
        {
            get
            {
                return new RectangleF(Position.X - Size.X / 2f, Position.Y - Size.Y / 2f, Size.X, Size.Y);
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

        public Player(Vector2 startPos, Vector2 positionToCut, string fileNameIconTexture, EnumPlayer name)
        {
            Position = startPos;
            PositionToCut = positionToCut;
            Size = new Vector2(40, 40);
            _playerSprite = TextureLoader.LoadTexture("Shockbolt_64x64_01.png");
            _playerIconTexture = TextureLoader.LoadTexture(fileNameIconTexture);
            Name = name;
        }

        public void Update(Board board, int qtdBlocksToWalk)
        {
            // Nada a fazer
            if (qtdBlocksToWalk == 0)
            {
                ResetFlags();
                return;
            }

            qtdBlocksToWalk = ApplyBoardRules(board, qtdBlocksToWalk);
           
            IList<Block> boardPaths = board.Paths;
            this.CurrentPathIndex += qtdBlocksToWalk;


            Block nextBlock = boardPaths.ElementAt(CurrentPathIndex - 1);
            _currentBlock = nextBlock;
            this.Position = new Vector2(nextBlock.X, nextBlock.Y);
            ResetFlags();
        }

        
        public void Draw()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit);

            OpenGLDrawer.Draw(_playerSprite,
                new Vector2(this.Position.X * Constants.GRIDSIZE, (this.Position.Y) * Constants.GRIDSIZE),
                new Vector2((float)Constants.GRIDSIZE / Constants.TILESETSIZE),
                Color.Transparent,
                Vector2.Zero,
                new RectangleF(PositionToCut.X * Constants.TILESETSIZE, PositionToCut.Y * Constants.TILESETSIZE,
                                  Constants.TILESETSIZE, Constants.TILESETSIZE));
        }

        public void ResetGame(Point startPos)
        {
            ResetFlags();
            this.Position = new Vector2(startPos.X,startPos.Y);
        }

        #region Private Methods
        private void ResetFlags()
        {
            this.DoubleDice = false;
            this.NegativeDice = false;
            this.DoubleNegativeDice = false;
            this.Won = false;
            this.Penalized = false;
        }

        private int ApplyBoardRules(Board board, int qtdBlocksToWalk)
        {
            if (board.Paths.Count < (qtdBlocksToWalk - 1))
            {
                throw new Exception("Problema ao carregar Board!");
            }            

            if (this.DoubleDice)
            {
                qtdBlocksToWalk *= 2;
            }
            if (this.NegativeDice)
            {
                qtdBlocksToWalk *= -1;
            }
            if (this.DoubleNegativeDice)
            {
                qtdBlocksToWalk *= -1;
                qtdBlocksToWalk *= 2;
            }

            // Fim de jogo
            if (board.Paths.Count <= (this.CurrentPathIndex + qtdBlocksToWalk))
            {
                this.Won = true;
            }


            return qtdBlocksToWalk;
        }


        #endregion
    }
}
