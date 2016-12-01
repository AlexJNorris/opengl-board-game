using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace BoardCGame.Util
{
    public class Constants
    {
        public static int GRIDSIZE = 32;
        public static int TILESETSIZE = 64;
        public static float LOOK_CAMERA_ANGLE_ROOT = 0.025f;
        public static IList<Vector2> TEXTCOORDS = new List<Vector2>() {new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };

    }
}
