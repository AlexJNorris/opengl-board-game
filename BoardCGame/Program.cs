﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardCGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (BoardGame game = new BoardGame(1920, 1080))
            {
                game.Run();
            }
        }
    }
}
