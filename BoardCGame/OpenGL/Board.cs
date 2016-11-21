using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using BoardCGame.Entity;
using BoardCGame.OpenGL;
using System.IO;
using BoardCGame.Entity.Enumerations;
using System.Xml;
using TiledSharp;
using BoardCGame.Util;

namespace BoardCGame.OpenGL
{
    public class Board
    {
        private Block[,] _grid;
        private string _fileName;
        private Point _playerStartPos, _playerEndPos;

        public Point PlayerStartPos
        {
            get { return _playerStartPos; }
        }

        public Point PlayerEndPos
        {
            get { return _playerEndPos; }
        }

        public Block this[int x, int y]
        {
            get { return _grid[x, y]; }
            set { _grid[x, y] = value; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public int Width
        {
            get { return _grid.GetLength(0); }
        }

        public int Height
        {
            get { return _grid.GetLength(1); }
        }

        public Vector2 TestePos { get; set; }

        private IList<Block> _paths;
        private IList<Block> _boardPaths;

        public IList<Block> Paths
        {
            get { return _paths; }
        }


        public Board(string filePath)
        {
            try
            {
                TmxMap map = new TmxMap(filePath);
                _grid = new Block[map.Width, map.Height];
                _fileName = filePath;

                BuildBoard(map);
                BuildObjects(map);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                DrawBaseCase();
            }
        }

        public Board(int width, int height)
        {
            _grid = new Block[width, height];
            _fileName = "none";
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    // Evitando pintar o meio
                    if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                    {
                        _grid[x, y] = new Block(BlockType.Terrain, x, y);
                    }
                    else
                    {
                        _grid[x, y] = new Block(BlockType.Empty, x, y);
                    }
                }
            }
        }

        #region Private Methods

        private void MapStartPositions(IList<TmxObjectGroup> objectGroup)
        {
            //TmxObject playerStartPos = objectGroup.SelectMany(z => z.Objects).FirstOrDefault(t => t.Name == "PlayerStart");
            //TmxObject playerEndPos = objectGroup.SelectMany(z => z.Objects).FirstOrDefault(t => t.Name == "PlayerEnd");
            //if (playerStartPos == null || playerEndPos == null)
            //{
            //    throw new Exception("Coordenadas de inicio e fim não encontradas");
            //}


            //_playerStartPos = new Point((int)playerStartPos.X, (int)playerStartPos.Y);
            //_playerEndPos = new Point((int)playerEndPos.X, (int)playerEndPos.Y);
        }

        private void MapBoardPath(IList<TmxObjectGroup> objectGroup)
        {
            int tValue;
            _paths = new List<Block>();
            IList<TmxObject> objects =
                objectGroup.Where(t => t.Name.Contains("Path")).SelectMany(z => z.Objects).ToList();
            objects = objects.Where(t => int.TryParse(t.Name, out tValue)).OrderBy(z => int.Parse(z.Name)).ToList();
            foreach (var obj in objects)
            {
                var xPos = (int)obj.X / Constants.TILESETSIZE;
                var yPos = (int)obj.Y / Constants.TILESETSIZE;
                _paths.Add(new Block(BlockType.Path, xPos, yPos));
            }
        }


        private void BuildBoard(TmxMap map)
        {
            TmxLayer tileLayer = map.Layers.FirstOrDefault();
            if (tileLayer == null)
            {
                throw new Exception("Mapa não encontrado.");
            }


            IList<TmxLayerTile> tiles2 = tileLayer.Tiles;
            _boardPaths = new List<Block>();

            int x = 0;
            int y = 0;
            for (int i = 0; i < tiles2.Count; i++)
            {
                int gid = tiles2[i].Gid;
                switch (gid)
                {
                    case 2952:
                        _grid[x, y] = new Block(BlockType.Terrain, x, y);
                        break;
                    //case 2817:
                    case 2258:
                        _grid[x, y] = new Block(BlockType.TerrainBoard, x, y);
                        break;
                    //case 3567:
                    case 1443:
                        _grid[x, y] = new Block(BlockType.Path, x, y);
                        _boardPaths.Add(new Block(BlockType.Path, x, y));
                        break;
                    //case 3569:
                    case 1829:
                        _grid[x, y] = new Block(BlockType.StartPoint, x, y);
                        break;

                    //case 3:
                    //    _grid[x, y] = new Block(BlockType.Ladder, x, y);
                    //    break;
                    //case 4:
                    //    _grid[x, y] = new Block(BlockType.LadderPlatform, x, y);
                    //    break;
                    //case 5:
                    //    _grid[x, y] = new Block(BlockType.Platform, x, y);
                    //    break;
                    default:
                        _grid[x, y] = new Block(BlockType.Empty, x, y);
                        break;
                }

                x++;
                if (x >= map.Width)
                {
                    x = 0;
                    y++;
                }
            }
        }

        private void BuildObjects(TmxMap map)
        {
            IList<TmxObjectGroup> objectGroup = map.ObjectGroups.ToList();
            if (objectGroup.Any())
            {
                MapStartPositions(objectGroup);
                MapBoardPath(objectGroup);
            }
        }

        private void DrawBaseCase()
        {
            int width = 20, height = 20;
            _grid = new Block[width, height];
            _fileName = "none";

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    // Evitando pintar o meio
                    if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                    {
                        _grid[x, y] = new Block(BlockType.Terrain, x, y);
                    }
                    else
                    {
                        _grid[x, y] = new Block(BlockType.Empty, x, y);
                    }
                }
            }
        }

        #endregion
    }
}