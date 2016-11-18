using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoardCGame.Entity.Enumerations;
using OpenTK;

namespace BoardCGame.Entity
{
    public class BlockPath : Block
    {
        public Vector2 BoardPosition { get; set; }

        public BlockPath(BlockType type, int x, int y) 
            : base(type,x,y)
        {
            
        }
    }
}
