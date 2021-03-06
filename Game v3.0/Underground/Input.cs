﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Windows;
using System.Windows.Forms.Layout;

namespace Underground
{
    public class Input
    {
        public List<Keys> KeysPressed { get; private set; }
        public List<Keys> KeysReleased { get; private set; }
        public List<Keys> KeysDown { get; private set; }

        public MouseButtons MousePressed { get; private set; }
        public MouseButtons MouseReleased { get; private set; }
        public MouseButtons MouseDown { get; private set; }
        public Point MousePoint { get; private set; }
        public int MouseWheelDelta { get; private set; }
        
        Form form;
       
        private bool mouseLeft;
        public bool MouseLeft
        {
            get
            {
                //Only one left is set if the user holds..
                if (mouseLeft)
                {
                    mouseLeft = false;
                    return true;
                }
                return mouseLeft;
            }
        }

        public Input(Form form)
        {

            this.form = form;
            form.KeyDown += form_KeyDown;
            form.KeyUp += form_KeyUp;
            form.MouseMove += form_MouseMove;
            form.MouseDown += form_MouseDown;
            KeysDown = new List<Keys>();
            KeysPressed = new List<Keys>();
            KeysReleased = new List<Keys>();
        }

        void form_MouseMove(object sender, MouseEventArgs e)
        {
            //Program.device.ShowCursor = false;
            //Cursor.Hide();
            //Cursor.Current = new Cursor("Curseur.cur");
            //Cursor.Position = new Point(form.DesktopBounds.X., form.DesktopBounds.Y);
            MousePoint = e.Location;

            //Program.WriteNicely("i", 5, "x mousePos = " + MousePoint.X);
            //Program.WriteNicely("i", 5, "y mousePos = " + MousePoint.Y);
        }


        void form_MouseDown(object sender, MouseEventArgs e)
        {
            MousePoint = e.Location;
            Program.WriteNicely("i", 6, "Mouse clicked ! x:" + MousePoint.X + " & y:" + MousePoint.Y);
            if (e.Button == MouseButtons.Left)
            {
                mouseLeft = true;
            }
        }

        void form_KeyDown(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyData & ~Keys.Shift;

            if (KeysDown.Contains(key) == false)
            {
                KeysPressed.Add(key);
                //Matrix worldViewProj = Matrix.RotationY(time) * viewProj;
                KeysDown.Add(key);
            }
        }

        void form_KeyUp(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyData & ~Keys.Shift;
            ClearKeyCache();
            KeysReleased.Add(key);
            KeysDown.Remove(key);
            ////Console.WriteLine(key);
        }

        public void ClearKeyCache()
        {
            KeysPressed.Clear();
            KeysReleased.Clear();
        }



        
    }
}
