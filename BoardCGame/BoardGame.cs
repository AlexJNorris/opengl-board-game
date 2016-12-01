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
using OpenTK.Input;
using BoardCGame.Config;
using RectangleF = System.Drawing.Rectangle;

namespace BoardCGame
{
    public class BoardGame : GameWindow
    {
        private View _view;
        private Texture _tileSet, _backgroundTexture;
        private Board _board;
        private Player _player1, _player2;
        private Dice _dice;
        private int _currentDiceFaceId;
        private System.Diagnostics.Stopwatch _watch;
        private float _angle;
        private Point _mousePos;
        private Camera _camera;
        private Audio _audio;
        private EnumPlayer _currentPlayer;
        bool _diceRotate;
        bool _boardRotate;
        
        public BoardGame(int width, int height)
            : base(width, height, GraphicsMode.Default, "Board CG Game", GameWindowFlags.Default, DisplayDevice.Default, 2, 1, GraphicsContextFlags.Debug)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            _view = new View(Vector3.Zero);
            Input.Initialize(this);
        }
    
        private void BoardGame_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            // Exit
            if (e.Key == OpenTK.Input.Key.F4 && e.Key.HasFlag(OpenTK.Input.Key.AltLeft))
            {
                Environment.Exit(0);
            }
        }

        private void BoardGame_MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            if (_boardRotate)
            {
                _camera.XRotate += e.XDelta;
                _camera.YRotate += e.YDelta;
            }
        }

        private void BoardGame_MouseWheel(object sender, OpenTK.Input.MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                _view.ZoomOut();
            }
            else
            {
                _view.ZoomIn();
            }
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

            LoadTextures();

            _camera = new Camera();
            _audio = new Audio();
            _dice = new Dice();
            _board = new Board("Content/BoardMap.tmx");
            InitializePlayers();


            MouseMove += BoardGame_MouseMove;
            MouseWheel += BoardGame_MouseWheel;
            KeyDown += BoardGame_KeyDown;

            _currentPlayer = EnumPlayer.Player1;
            StartGamePosition();

            _watch = System.Diagnostics.Stopwatch.StartNew();
            _audio.PlayIntro();
            RandomizeCube();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            DrawBackground();
            
            GL.Viewport(0, 0, Width, Height);
       
            LoadPerspective();
            DrawDice();

            PrepareDrawing();

            CameraRotate();
        
            GL.PushMatrix();
            DrawBoard();
            _player1.Draw();
            _player2.Draw();
            GL.PopMatrix();

            DrawIcons();

            
            this.SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            float aspect = (float)Width / (float)Height;
            Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1f, 100f); // Ultimos parametros ->  quão perto e quão longe, respectivamente, os objetos deixara de ser renderizado
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);        

            if (Input.MousePress(OpenTK.Input.MouseButton.Left))
            {
                _boardRotate = true;
            }

            if (Input.MouseRelease(MouseButton.Left))
            {
                _boardRotate = false;
            }

            if (Input.MousePress(OpenTK.Input.MouseButton.Right))
            {
                _diceRotate = !_diceRotate;
                if (!_diceRotate) // Pare!
                {
                    _audio.PlayStopDice();
                    int result = RandomizeCube();
                    Play(result);
                }
                else
                {
                    _audio.PlayRollDice();
                }
            }

            if (Input.KeyDown(OpenTK.Input.Key.Right) || Input.KeyDown(OpenTK.Input.Key.A))
            {
                _view.SetPosition(_view.PositionGoto + new Vector3(10, 0, 0), EnumAnimationType.QuarticOut, 30);
            }
            if (Input.KeyDown(OpenTK.Input.Key.Left) || Input.KeyDown(OpenTK.Input.Key.D))
            {
                _view.SetPosition(_view.PositionGoto + new Vector3(-10, 0, 0), EnumAnimationType.QuarticOut, 30);
            }

            if (Input.KeyDown(OpenTK.Input.Key.Down) || Input.KeyDown(OpenTK.Input.Key.S))
            {
                _view.SetPosition(_view.PositionGoto + new Vector3(0, 10, 0), EnumAnimationType.QuarticOut, 30);
            }

            if (Input.KeyDown(OpenTK.Input.Key.Up) || Input.KeyDown(OpenTK.Input.Key.W))
            {
                _view.SetPosition(_view.PositionGoto + new Vector3(0, -10, 0), EnumAnimationType.QuarticOut, 30);
            }

            if (Input.KeyDown(OpenTK.Input.Key.G)) // Reset Camera
            {
                _camera.Reset();
                _view.ResetPosition();
            }
      
            Input.Update();
        }

        private void LoadTextures()
        {
            _tileSet = TextureLoader.LoadTexture("ICONS_COMPLETE_1024x9.png");
            _backgroundTexture = TextureLoader.LoadTexture("backgroundgame.jpg");
        }

        private void CalculateDiceRotation()
        {
            _watch.Stop();
            float deltaElapsedTime = (float)_watch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency;
            _watch.Restart();
            _angle += deltaElapsedTime * 4;
        }

        private void StartGamePosition()
        {
            _view.ResetPosition();
            LoadPerspective();
        }

        private void LoadPerspective()
        {
            GL.LoadIdentity();
            GL.PushMatrix();
            Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(1.04f, 4 / 3f, 1, 10000f);
            Matrix4 lookAtMatrix = _camera.GetLookAtMatrix(); 
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookAtMatrix);
        }
      
        private void CameraRotate()
        {
            GL.Rotate(_camera.XRotate, 0, 1, 0);
            GL.Rotate(_camera.YRotate, 1, 0, 0);
        }

        private void PrepareDrawing()
        {
            GL.LoadIdentity();
            GL.PushMatrix();
            Matrix4 calculatedMatrix2 = Matrix4.Identity;
            calculatedMatrix2 = Matrix4.Mult(calculatedMatrix2, Matrix4.CreateTranslation(new Vector3(0, 0, -5f)));
            calculatedMatrix2 = Matrix4.Mult(calculatedMatrix2, Matrix4.CreateScale(0.1f, -0.1f, .5f));
            GL.MultMatrix(ref calculatedMatrix2);
        }

        private void InitializePlayers()
        {
            _player1 = new Player(new Vector2(_board.PlayerStartPos.X, _board.PlayerStartPos.Y),
                      new Vector2(7, 3),
                      "redplayericon.png",
                      EnumPlayer.Player1);
            _player2 = new Player(new Vector2(_board.PlayerStartPos.X, _board.PlayerStartPos.Y),
                                  new Vector2(18, 3),
                                  "blueplayericon.png",
                                  EnumPlayer.Player2);
        }

        private int RandomizeCube()
        {
            DiceTexture chosenSideTexture;
            if (_currentPlayer == EnumPlayer.Player1)
            {
                chosenSideTexture = _dice.PickRandomSideTexture(new List<DiceTexture>(_dice.RedTextures));
                _dice.StoppedSides = _dice.RedTextures.Where(t => t.FaceNumber != chosenSideTexture.FaceNumber).ToList();
            }
            else
            {
                chosenSideTexture = _dice.PickRandomSideTexture(new List<DiceTexture>(_dice.BlueTextures));
                _dice.StoppedSides = _dice.BlueTextures.Where(t => t.FaceNumber != chosenSideTexture.FaceNumber).ToList();
            }

            _currentDiceFaceId = chosenSideTexture.Id;
            return chosenSideTexture.FaceNumber;
        }

        private void SwitchPlayer()
        {
            if (_currentPlayer == EnumPlayer.Player1)
            {
                _currentPlayer = EnumPlayer.Player2;
            }
            else
            {
                _currentPlayer = EnumPlayer.Player1;
            }
        }

        private void CheckGameRules(Player currentPlayer)
        {
            if (currentPlayer.Won)
            {
                EndGame(currentPlayer);
                return;
            }

            Block ruleBlock = _board.GetCorrespondingRuleBlock(currentPlayer.CurrentBlock);
            if (ruleBlock != null)
            {
                EnumBlockType currentBlockType = ruleBlock.Type;
                switch (currentBlockType)
                {
                    case EnumBlockType.Dice:
                        // Nada a fazer, jogue de novo!
                        break;
                    case EnumBlockType.GreenSkull:
                        currentPlayer.Penalized = true;
                        SwitchPlayer();
                        break;
                    case EnumBlockType.SpeedBoots:
                        currentPlayer.DoubleDice = true;
                        SwitchPlayer();
                        break;
                    case EnumBlockType.Hole:
                        currentPlayer.NegativeDice = true;
                        SwitchPlayer();
                        break;
                    case EnumBlockType.BadluckRedSpot:
                        currentPlayer.Position = new Vector2(_board.PlayerStartPos.X, _board.PlayerStartPos.Y);
                        SwitchPlayer();
                        break;
                    case EnumBlockType.Dynamite:
                        currentPlayer.DoubleNegativeDice = true;
                        SwitchPlayer();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                SwitchPlayer();
            }

        }

        private void Play(int diceResult)
        {
            if (_currentPlayer == EnumPlayer.Player1)
            {
                if (!_player1.Penalized)
                {
                    _player1.Update(_board, diceResult);
                    CheckGameRules(_player1);
                }
                else
                {
                    _player1.Penalized = false;
                    SwitchPlayer();
                }
            }
            else
            {
                if (!_player2.Penalized)
                {
                    _player2.Update(_board, diceResult);
                    CheckGameRules(_player2);
                }
                else
                {
                    _player2.Penalized = false;
                    SwitchPlayer();
                }
            }
        }

        private void DrawIcons()
        {
            GL.LoadIdentity();
            GL.PushMatrix();
            GL.Translate(-2.5, 2, -5f);
            GL.Scale(.5f, .5f, 1f);
            GL.Enable(EnableCap.Texture2D);

            if (_currentPlayer == EnumPlayer.Player1)
            {
                GL.BindTexture(TextureTarget.Texture2D, _player1.PlayerIconTexture.Id);
            }
            else
            {
                GL.BindTexture(TextureTarget.Texture2D, _player2.PlayerIconTexture.Id);
            }

            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < 4; i++)
            {
                GL.TexCoord2(Constants.TEXTCOORDS.ElementAt(i));
                GL.Vertex2(Constants.TEXTCOORDS.ElementAt(i));
            }

            GL.End();
            GL.PopMatrix();
        }

        private void DrawDice()
        {
            GL.LoadIdentity();
            GL.PushMatrix();

            CalculateDiceRotation();

            Matrix4 calculatedMatrix = Matrix4.Identity;
            if (_diceRotate)
            {
                calculatedMatrix = Matrix4.Mult(calculatedMatrix, Matrix4.CreateRotationX(_angle / 2));
                calculatedMatrix = Matrix4.Mult(calculatedMatrix, Matrix4.CreateRotationY(_angle));
            }

            calculatedMatrix = Matrix4.Mult(calculatedMatrix, Matrix4.CreateTranslation(new Vector3(0, 0, -5)));
            calculatedMatrix = Matrix4.Mult(calculatedMatrix, Matrix4.CreateScale(.1f, .1f, .5f));
            GL.MultMatrix(ref calculatedMatrix);

            if (_diceRotate)
            {

                _dice.Roll(_currentPlayer);
            }
            else
            {
                _dice.Stop(_currentDiceFaceId);


            }

            GL.PopMatrix();
        }

        private void DrawBackground()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, _backgroundTexture.Id);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, -1.0, 1.0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(-1.0f, -1.0f, -0.0001);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(1.0f, -1.0f, -0.0001);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(1.0f, 1.0f, -0.0001);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(-1.0f, 1.0f, -0.0001);
            GL.End();
            GL.PushMatrix();
            GL.Enable(EnableCap.DepthTest);
        }


        private void DrawBoard()
        {
            _view.GetTransformMatrix();
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    RectangleF source = new RectangleF(0, 0, 0, 0);

                    switch (_board[x, y].Type)
                    {
                        case EnumBlockType.TerrainBoard:
                            source = new RectangleF(1 * Constants.TILESETSIZE, 47 * Constants.TILESETSIZE,
                                   Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case EnumBlockType.Path:
                            source = new RectangleF(2 * Constants.TILESETSIZE, 30 * Constants.TILESETSIZE,
                                   Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case EnumBlockType.StartPoint:
                            source = new RectangleF(4 * Constants.TILESETSIZE, 38 * Constants.TILESETSIZE,
                                  Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case EnumBlockType.EndPoint:
                            source = new RectangleF(5 * Constants.TILESETSIZE, 5 * Constants.TILESETSIZE,
                                 Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case EnumBlockType.Dice:
                            source = new RectangleF(47 * Constants.TILESETSIZE, 47 * Constants.TILESETSIZE,
                                  Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case EnumBlockType.Twister:
                            source = new RectangleF(2 * Constants.TILESETSIZE, 47 * Constants.TILESETSIZE,
                                  Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case EnumBlockType.SpeedBoots:
                            source = new RectangleF(0 * Constants.TILESETSIZE, 40 * Constants.TILESETSIZE,
                                  Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case EnumBlockType.GreenSkull:
                            source = new RectangleF(9 * Constants.TILESETSIZE, 38 * Constants.TILESETSIZE,
                                  Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case EnumBlockType.Hole:
                            source = new RectangleF(21 * Constants.TILESETSIZE, 46 * Constants.TILESETSIZE,
                                  Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case EnumBlockType.BadluckRedSpot:
                            source = new RectangleF(43 * Constants.TILESETSIZE, 43 * Constants.TILESETSIZE,
                                  Constants.TILESETSIZE, Constants.TILESETSIZE);
                            break;
                        case EnumBlockType.Dynamite:
                            source = new RectangleF(33 * Constants.TILESETSIZE, 47 * Constants.TILESETSIZE,
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
        }

        private void EndGame(Player currentPlayer)
        {
            string playerWhoWon = currentPlayer.Name == EnumPlayer.Player1 ? "Vermelho" : "Azul";
            var result = Window.ShowDialog("Jogador " + playerWhoWon + " venceu!", "Deseja jogar novamente?");
            if (result)
            {
                _player1.ResetGame(_board.PlayerStartPos);
                _player2.ResetGame(_board.PlayerStartPos);
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}
