using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardCGame.Entity
{
    public class Texture
    {
        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }


        public Texture(int id, int width, int height)
        {
            Id = id;
            Width = width;
            Height = height;
        }
    }
}
