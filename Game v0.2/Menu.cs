using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D9;
using SharpDX.DirectInput;
using SharpDX.Windows;
using Underground;
using Color = SharpDX.Color;
using Device = SharpDX.Direct3D9.Device;
using DeviceType = SharpDX.Direct3D9.DeviceType;
using Effect = SharpDX.Direct3D9.Effect;

namespace Underground
{
    static class Menu
    {
        static public void menu()
        {
            // 
            #region Souris
            MouseHandler mouseHandler = new Underground.MouseHandler();
            Program.form.MouseDown += mouseHandler.MouseDown;
            Program.form.MouseUp += mouseHandler.MouseUp;
            Size orgSize = Program.form.Size;
            Point orgLocation = Program.form.Location;
            float xRes = 2.0f / (Program.form.Width - 5);
            float yRes = 2.0f / (Program.form.Height - 5);
            Program.form.KeyDown += (sender, arg) =>
            {
                switch (arg.KeyCode)
                {
                    case Keys.Escape:   //Exit
                        ((RenderForm)sender).Close();
                        break;
                    case Keys.Space:    //Orginal size
                        ((RenderForm)sender).Size = orgSize;
                        ((RenderForm)sender).Location = orgLocation;
                        break;
                }
            };
            #endregion
        }
    }
}
