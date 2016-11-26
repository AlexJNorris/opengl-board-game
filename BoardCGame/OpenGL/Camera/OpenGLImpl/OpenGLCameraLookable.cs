using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Projeto_CG_1_GQ.Configuration.Camera.OpenGLImpl
{
    public class OpenGLCameraLookable : ICameraLookable
    {
        public void Look(CameraPosition position)
        {
            Vector3 eye = new Vector3((float) position.X, (float) position.Y, (float) position.Z);
            Vector3 target = new Vector3((float) (position.X + position.XFocus), (float) (position.Y + position.YFocus), (float)( position.Z + position.ZFocus));
            Vector3 up = new Vector3(0, 1, 0);
            Matrix4 look = Matrix4.LookAt(eye, target, up);
            //Matrix4 lookat = Matrix4.LookAt(0, 5, 5, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref look);
        }
    }
}
