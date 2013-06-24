using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using System.Drawing;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using SharpDX.Diagnostics;

namespace Underground
{
    class MediaPlayer
    {
        public static bool intro_finished = false;

        private static bool intro_started = false;

        public static void Intro()
        {
            if (!intro_started)
            {
                Program.m_play.Start();
                intro_started = true;
            }
        }

        public static void Fin_intro(Object sender)
        {
            intro_finished = true;

            //Program.panel1.Invoke((MethodInvoker) (() => Program.panel1.Dispose()));
            Program.form.Invoke((MethodInvoker)(() => Program.form.Controls.Remove(Program.panel1)));

        }
    }
}
