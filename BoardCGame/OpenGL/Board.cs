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

namespace BoardCGame.OpenGL
{
    public class Board
    {
        private Block[,] _grid;
        private string _fileName;
        public Point PlayerStartPos { get; set; }

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

        public Board(string filePath)
        {
            try
            {
                TmxMap map = new TmxMap(filePath);

                int width = map.Width;
                int height = map.Height;

                _grid = new Block[width, height];
                _fileName = filePath;
                PlayerStartPos = new Point(1, 1);

                //XmlNode tileLayer = document.DocumentElement.SelectSingleNode("layer[@name='Tile Layer 1']");
                //XmlNodeList tiles = tileLayer.SelectSingleNode("data").SelectNodes("tile");

                TmxLayer tileLayer2 = map.Layers.FirstOrDefault();
                if (tileLayer2 == null)
                {
                    return;
                }

                IList<TmxLayerTile> tiles2 = tileLayer2.Tiles;

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
                        case 2817:
                            _grid[x, y] = new Block(BlockType.TerrainBoard, x, y);
                            break;
                        case 3567:
                            _grid[x, y] = new Block(BlockType.Path, x, y);
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
                    if (x >= width)
                    {
                        x = 0;
                        y++;
                    }
                }

                var objectGroup = map.ObjectGroups.FirstOrDefault(t => t.Name.Contains("Player1"));
                if (objectGroup != null)
                {
                    var objects = objectGroup.Objects;

                    for (int i = 0; i < objects.Count; i++)
                    {
                        double xPos = objects[i].X;
                        double yPos = objects[i].Y;

                        switch (objects[i].Name)
                        {
                            case "PlayerStartPos":
                                //Sabendo a posicao do jogador
                                this.PlayerStartPos = new Point((int)(xPos / 128), (int)(yPos / 128));
                                break;
                            default:
                                break;
                        }
                    }
                }




            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                int width = 20, height = 20;
                _grid = new Block[width, height];
                _fileName = "none";
                PlayerStartPos = new Point(1, 1);

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
        }

        public Board(int width, int height)
        {
            _grid = new Block[width, height];
            _fileName = "none";
            PlayerStartPos = new Point(1, 1);

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
    }
}