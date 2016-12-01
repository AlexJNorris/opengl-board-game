using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace BoardCGame.OpenGL
{
    public class Camera
    {
        private Vector3 _orientation = new Vector3((float)Math.PI, 0f, 0f);

        public int XRotate { get; set; }
        public int YRotate { get; set; }

        public void Reset()
        {
            this.XRotate = 0;
            this.YRotate = 0;
        }

        public Matrix4 GetLookAtMatrix()
        {
            var lookAt = new Vector3();
            lookAt.X = (float)(Math.Sin((float)_orientation.X) * Math.Cos((float)_orientation.Y));
            lookAt.Y = (float)Math.Sin((float)_orientation.Y);
            lookAt.Z = (float)(Math.Cos((float)_orientation.X) * Math.Cos((float)_orientation.Y));
            return Matrix4.LookAt(Vector3.Zero, Vector3.Zero + lookAt, Vector3.UnitY);
        }
    }
}
