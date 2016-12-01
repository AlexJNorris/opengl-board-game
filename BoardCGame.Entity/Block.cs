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
        public EnumBlockType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Solid { get; set; }

        public Block(EnumBlockType type, int x, int y)
        {
            Type = type;
            X = x;
            Y = y;
        }
    }
}
