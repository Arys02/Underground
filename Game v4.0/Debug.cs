using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOL_l
{
    class Debug
    {
        bool debug = true;
        int priorite = 2;
        public void WriteNicely(string op, ConsoleColor c, string msg, int priorite)
        {
            if (debug && (this.priorite >= priorite || this.priorite == -1))
            {
                Console.ForegroundColor = c;
                Console.Write("[" + op + "] ");
                Console.ResetColor();
                Console.WriteLine(msg);
            }
        }
    }
}
