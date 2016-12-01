using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK.Input;

namespace BoardCGame.Entity
{
    public class Input
    {
        private static IList<Key> _keysDown;
        private static IList<Key> _keysDownLast;
        private static IList<MouseButton> _buttonsDown;
        private static IList<MouseButton> _buttonsDownLast;

        public static void Initialize(GameWindow window)
        {
            _keysDown = new List<Key>();
            _keysDownLast = new List<Key>();
            _buttonsDown = new List<MouseButton>();
            _buttonsDownLast = new List<MouseButton>();

            window.KeyUp += GameKeyUp;
            window.KeyDown += GameKeyDown;
            window.MouseDown += GameMouseDown;
            window.MouseUp += GameMouseUp;
        }

        private static void GameMouseUp(object sender, MouseButtonEventArgs e)
        {
            while (_buttonsDown.Contains(e.Button))
            {
                _buttonsDown.Remove(e.Button);
            }
        }

        private static void GameMouseDown(object sender, MouseButtonEventArgs e)
        {
            _buttonsDown.Add(e.Button);
        }

        private static void GameKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            _keysDown.Add(e.Key);
        }

        private static void GameKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            while (_keysDown.Contains(e.Key))
            {
                _keysDown.Remove(e.Key);
            }
        }

        public static void Update()
        {
            _keysDownLast = new List<Key>(_keysDown);
            _buttonsDownLast = new List<MouseButton>(_buttonsDown);
        }

        public static bool KeyPress(Key key)
        {
            //Pressionada no frame atual e não no anterior
            return (_keysDown.Contains(key) && !_keysDownLast.Contains(key));
        }

        public static bool KeyRelease(Key key)
        {
            return (!_keysDown.Contains(key) && _keysDownLast.Contains(key));
        }

        public static bool KeyDown(Key key)
        {
            return (_keysDown.Contains(key));
        }

        public static bool MousePress(MouseButton button)
        {
            return (_buttonsDown.Contains(button) && !_buttonsDownLast.Contains(button));
        }

        public static bool MouseRelease(MouseButton button)
        {
            return (!_buttonsDown.Contains(button) && _buttonsDownLast.Contains(button));
        }

        public static bool MouseDown(MouseButton button)
        {
            return (_buttonsDown.Contains(button));
        }
    }
}
