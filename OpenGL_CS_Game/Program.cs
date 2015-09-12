using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace OpenGL_CS_Game
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                game.Run(60.0, 60.0);
            }
        }
    }
}