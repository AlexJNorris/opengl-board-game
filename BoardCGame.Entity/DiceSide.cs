using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace BoardCGame.Entity
{
    public class DiceSide
    {
        public IList<Vector3> VerticesCords { get; set; }

        public DiceSide(IList<Vector3> coords)
        {
            this.VerticesCords = coords;
        }
    }
}
