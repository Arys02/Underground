using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.DirectInput;
using SharpDX.Windows;
//using Underground;
using Color = SharpDX.Color;
using Device = SharpDX.Direct3D9.Device;
using DeviceType = SharpDX.Direct3D9.DeviceType;
using Effect = SharpDX.Direct3D9.Effect;

namespace Underground
{
    class Camera
    {
      //  public Vector3 position = new Vector3(2.0f, 0, -2.0f);
      //  private static Vector3 old_pos = new Vector3(2.0f, 0, -2.0f);

        public Vector3 position = new Vector3(0f, -2f, 0);
        public Vector3 angle = new Vector3(0, 0, 0);
        private bool camera_altere = true;
        private Point previous_mousepoint = Program.input.MousePoint;
        public Matrix view = new Matrix();
        
        public static Keys keyrun = Keys.O;
        
        public Keys keyavence = Keys.Z;
        public Keys keyrecule = Keys.S;
        public Keys keyright = Keys.D;
        public Keys keyleft = Keys.Q;

        public Camera()
        {
            view =  Matrix.Translation(position.X, position.Y, position.Z) *
                    Matrix.RotationAxis(new Vector3(0, 1, 0), angle.Y) *
                    Matrix.RotationAxis(new Vector3(1, 0, 0), angle.X) *
                    Matrix.RotationAxis(new Vector3(0, 0, 1), angle.Z);
        }

        public void orient_camera(long timer)
        {
            Ingame.a_progresse = false;
            float[] VitesseRotation = new float[3] { 0.0000006f * timer, 0.0000006f * timer, 0.0000006f * timer };
            float[] VitesseTranslation = !Program.input.KeysDown.Contains(keyrun)
                                             ? new float[]
                                                 {
                                                     0.000001f*timer,
                                                     0.000001f*timer,
                                                     0.000001f*timer,
                                                 }
                                             : new float[] {0.000003f*timer, 0.000003f*timer, 0.000003f*timer};
        
            double produit_scalaire;

            #region clavier
            /************ Utile pour faire des tests ************/
            if (Program.input.KeysDown.Contains(Keys.Escape))
            {
                Menu.IsInMenu = true;
            }
            if (Program.input.KeysDown.Contains(Keys.A))
            {
                if (Ingame.stateinflash == 0) Ingame.stateinflash = -1;
            }
            if (Program.input.KeysDown.Contains(Keys.R)) // Reset
            {
                angle = new Vector3(0, 0, 0);
                camera_altere = true;
            }
            if (Program.input.KeysDown.Contains(Keys.F)) Ingame.Slender.seraaffiche = true;
            if (Program.input.KeysDown.Contains(Keys.G)) Ingame.Slender.seraaffiche = false;
            if (Program.input.KeysDown.Contains(Keys.F1)) Ingame.PrimType = PrimitiveType.TriangleList;
            if (Program.input.KeysDown.Contains(Keys.F2)) Ingame.PrimType = PrimitiveType.LineList;
            if (Program.input.KeysDown.Contains(Keys.F3)) Ingame.PrimType = PrimitiveType.PointList;
            if (Program.input.KeysDown.Contains(Keys.F4)) Ingame.Sepia = false;
            if (Program.input.KeysDown.Contains(Keys.F5)) Ingame.Sepia = true;
            if (Program.input.KeysDown.Contains(Keys.F6)) Ingame.Following_light = true;
            if (Program.input.KeysDown.Contains(Keys.F7)) Ingame.Following_light = false;
            if (Program.input.KeysDown.Contains(Keys.F8)) Ingame.maximum_disallowed = true;
            if (Program.input.KeysDown.Contains(Keys.F9)) Ingame.maximum_disallowed = false;
            /************ END ************/

            if (Program.input.KeysDown.Contains(Keys.Space))
            {
                position.Y -= VitesseTranslation[1];
                //  Console.WriteLine("Vers le haut !");
                camera_altere = true;
            }

            if (Program.input.KeysDown.Contains(Keys.ShiftKey))
            {
                position.Y += VitesseTranslation[1];
                // Console.WriteLine("Vers le bas !");
                camera_altere = true;
            }

            //view = Matrix.LookAtLH(position, new Vector3(0, 0, 0), Vector3.UnitY);

            if (Program.input.KeysDown.Contains(Keys.Up) || Program.input.KeysDown.Contains(keyavence))
            {
                Ingame.a_progresse = true;
                produit_scalaire = (Math.Cos(angle.Y)*VitesseTranslation[2]);
                position.Z -= Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Sin(angle.Y)*VitesseTranslation[2]);
                position.X += Convert.ToSingle(produit_scalaire);

                if (!Sound.pas.IsAlive)
                {
                    if (Sound.pas.ThreadState == System.Threading.ThreadState.Stopped)
                    {
                        Sound.soundContinue = true;
                        Sound.pas = new Thread(Sound.bruitpas);
                    }
                    Sound.pas.Start();
                }
                // Console.WriteLine("En avant !");
                camera_altere = true;
            }
            else
            {
                Sound.soundContinue = false;
               // Console.WriteLine("prout");
            }
                
            
                //Sound.pas = new Thread(Sound.bruitpas);
                //Sound.pas.Interrupt();
            
            if (Program.input.KeysDown.Contains(Keys.Down) || Program.input.KeysDown.Contains(keyrecule))
            {
                Ingame.a_progresse = true;
                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[2]);
                position.Z += Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[2]);
                position.X -= Convert.ToSingle(produit_scalaire);
                if (!Sound.pas.IsAlive)
                {
                    if (Sound.pas.ThreadState == System.Threading.ThreadState.Stopped)
                        Sound.pas = new Thread(Sound.bruitpas);

                    Sound.pas.Start();

                }
                // Console.WriteLine("En arrière !");
                camera_altere = true;
            }

            if (Program.input.KeysDown.Contains(keyright) || Program.input.KeysDown.Contains(Keys.Right))
            {
                angle.Y -= VitesseRotation[1];
                // Console.WriteLine("A tribord !");
                camera_altere = true;
            }
            if (Program.input.KeysDown.Contains(keyleft) ||Program.input.KeysDown.Contains(Keys.Left))
            {
                angle.Y += VitesseRotation[1];
                //  Console.WriteLine("A babord !");
                camera_altere = true;
            }
            /************ END ************/

            if (Program.input.KeysDown.Contains(Keys.Q))
            {
                Ingame.a_progresse = true;
                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[0]);
                position.Z += Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[0]);
                position.X += Convert.ToSingle(produit_scalaire);
                if (!Sound.pas.IsAlive)
                {
                    if (Sound.pas.ThreadState == System.Threading.ThreadState.Stopped)
                        Sound.pas = new Thread(Sound.bruitpas);

                    Sound.pas.Start();

                }
                // Console.WriteLine("Left");
                camera_altere = true;
            }
            if (Program.input.KeysDown.Contains(Keys.D))
            {
                Ingame.a_progresse = true;
                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[0]);
                position.Z -= Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[0]);
                position.X -= Convert.ToSingle(produit_scalaire);
                if (!Sound.pas.IsAlive)
                {
                    if (Sound.pas.ThreadState == System.Threading.ThreadState.Stopped)
                        Sound.pas = new Thread(Sound.bruitpas);

                    Sound.pas.Start();

                }
                // Console.WriteLine("Right");
                camera_altere = true;
            }
            if (Program.input.KeysDown.Contains(Keys.PageUp))
            {
                angle.X += VitesseRotation[0];
                // Console.WriteLine("Montez !");
                camera_altere = true;
            }
            if (Program.input.KeysDown.Contains(Keys.PageDown))
            {
                angle.X -= VitesseRotation[0];
                // Console.WriteLine("Coulez !");
                camera_altere = true;
            }
            #endregion
            #region souris
            if (Program.input.MousePoint != previous_mousepoint)
            {
                angle.Y -= (Program.input.MousePoint.X - previous_mousepoint.X) * VitesseRotation[0];
                angle.X -= (Program.input.MousePoint.Y - previous_mousepoint.Y) * VitesseRotation[1];
                //Console.WriteLine(Program.input.MousePoint.X + " " + Program.input.MousePoint.Y);
                //Cursor.Position = new Point(Program.form.DesktopBounds.Width / 2 + Program.form.DesktopBounds.X, Program.form.DesktopBounds.Height / 2 + Program.form.DesktopBounds.Y);
                previous_mousepoint = new Point(Program.input.MousePoint.X, Program.input.MousePoint.Y);
                //Cursor.Hide();
            }
            #endregion

            if (angle.X >= Math.PI / 4 && angle.X <= Math.PI)
            {
                angle.X = Convert.ToSingle(Math.PI / 4);
            }
            if (angle.X >= Math.PI && angle.X <= 7 * Math.PI / 4)
            {
                angle.X = Convert.ToSingle(7 * Math.PI / 4);
            }
            angle.X = Convert.ToSingle(angle.X % (Math.PI * 2));
            if (angle.X < 0) angle.X += Convert.ToSingle(Math.PI * 2);
            angle.Y = Convert.ToSingle(angle.Y % (Math.PI * 2));
            if (angle.Y < 0) angle.Y += Convert.ToSingle(Math.PI * 2);
            angle.Z = Convert.ToSingle(angle.Z % (Math.PI * 2));
            if (angle.Z < 0) angle.Z += Convert.ToSingle(Math.PI * 2);

            if (camera_altere)
            {
                view = Matrix.Translation(position.X, position.Y + (float)Math.Sin(Ingame.sinusoide)/55f, position.Z) *
                                Matrix.RotationAxis(new Vector3(0, 1, 0), angle.Y) *
                                Matrix.RotationAxis(new Vector3(1, 0, 0), angle.X) *
                                Matrix.RotationAxis(new Vector3(0, 0, 1), angle.Z);
                //camera_altere = false;
            }
        }
    }
}
