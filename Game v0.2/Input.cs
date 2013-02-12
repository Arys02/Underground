using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Windows;

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

        public Input(Form form)
        {
            
            this.form = form;
            form.KeyDown += new KeyEventHandler(form_KeyDown);
            form.KeyUp += new KeyEventHandler(form_KeyUp);
            form.MouseMove += new MouseEventHandler(form_MouseMove);
            KeysDown = new List<Keys>();
            KeysPressed = new List<Keys>();
            KeysReleased = new List<Keys>();
        }

        void form_MouseMove(object sender, MouseEventArgs e)
        {
            MousePoint = e.Location;
            Console.WriteLine(MousePoint.X);
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
            //Console.WriteLine(key);
        }

        public void ClearKeyCache()
        {
            KeysPressed.Clear();
            KeysReleased.Clear();
        }
    }
}
