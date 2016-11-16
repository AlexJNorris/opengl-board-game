using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardCGame.Entity.Enumerations;

namespace BoardCGame.Entity
{
    public class Block
    {
        public BlockType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Solid { get; set; }
        public bool Platform { get; set; }
        public bool Ladder { get; set; }

        public Block(BlockType type, int x, int y)
        {
            Type = type;
            X = x;
            Y = y;

            //switch (Type)
            //{
            //    case BlockType.Ladder:
            //        Ladder = true;
            //        break;
            //    case BlockType.LadderPlatform:
            //        Ladder = true;
            //        Platform = true;
            //        break;
            //    case BlockType.Solid:
            //        Solid = true;
            //        break;
            //    case BlockType.Platform:
            //        Platform = true;
            //        break;
            //    default:
            //        break;
            //}
        }

    }
}
