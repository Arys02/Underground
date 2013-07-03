using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.XAudio2;
using SharpDX.Windows;

namespace LOL_l
{
    class Player
    {
        public Vector3 position = new Vector3(0f, -1, 5);
        public Vector3 angle = new Vector3(0, 0, 0);
        public Matrix view = new Matrix();
        public Matrix proj;

        private Vector3 speedTranslation = new Vector3(0.0005f, 0.0005f, 0.0005f);
        private Vector3 speedTranslationRun = new Vector3(0.001f, 0.001f, 0.001f);
        private Vector3 speedRotation = new Vector3(0.0000006f, 0.0000006f, 0.0000006f);
        private Vector3 speedTranslationPythagore;
        private Vector3 speedTranslationPythagoreRun;
        private Input input;
        private Debug debug = new Debug();
        private Point previous_mousepoint;
        private Thread ThOrientCamera; // Tickrate = 60
        private RenderForm form;
        private bool ThreadLaunched = false;

        public static Keys bindKeyRun = Keys.O;
        
        public Keys bindKeyAvance = Keys.Z;
        public Keys bindKeyRecule = Keys.S;
        public Keys bindKeyRight = Keys.Right;
        public Keys bindKeyLeft = Keys.Left;

        public void Disable()
        {
            if (ThreadLaunched)
            {
                ThOrientCamera.Abort();
                ThreadLaunched = false;
            }
            else
            {
                debug.WriteNicely("!", ConsoleColor.Magenta, "DANGER ! Tentative d'arret d'un Thread pas lancé", 0);
            }
        }
        public void Reenable()
        {
            if (!ThreadLaunched)
            {
                ThOrientCamera.Start();
                ThreadLaunched = true;
            }
            else
            {
                debug.WriteNicely("!", ConsoleColor.Magenta, "DANGER ! Multiplication des Thread", 0);
            }
        }
        public Player(ref RenderForm form, Matrix proj)
        {
            form.FormClosing += TurnOFFThread;
            this.form = form;
            this.proj = proj;
            if (!ThreadLaunched)
            {
                ThreadLaunched = true;
                ThOrientCamera = new Thread(orient_camera);
                ThOrientCamera.Start();
            }
            else
            {
                debug.WriteNicely("!", ConsoleColor.Magenta, "?? WTF ARE YOU DOING WITH MY SOURCE CODE ??", 0);
            }
            input = new Input(ref form);
            previous_mousepoint = Cursor.Position;
            view =  Matrix.Translation(position.X, position.Y, position.Z) *
                    Matrix.RotationAxis(new Vector3(0, 1, 0), angle.Y) *
                    Matrix.RotationAxis(new Vector3(1, 0, 0), angle.X) *
                    Matrix.RotationAxis(new Vector3(0, 0, 1), angle.Z);
            speedTranslationPythagore = new Vector3(
                Convert.ToSingle(speedTranslation.X * speedTranslation.X / Math.Sqrt(speedTranslation.X * speedTranslation.X * 2)),
                Convert.ToSingle(speedTranslation.Y * speedTranslation.Y / Math.Sqrt(speedTranslation.Y * speedTranslation.Y * 2)),
                Convert.ToSingle(speedTranslation.Z * speedTranslation.Z / Math.Sqrt(speedTranslation.Z * speedTranslation.Z * 2)));
            speedTranslationPythagoreRun = new Vector3(
                Convert.ToSingle(speedTranslationRun.X * speedTranslationRun.X / Math.Sqrt(speedTranslationRun.X * speedTranslationRun.X * 2)),
                Convert.ToSingle(speedTranslationRun.Y * speedTranslationRun.Y / Math.Sqrt(speedTranslationRun.Y * speedTranslationRun.Y * 2)),
                Convert.ToSingle(speedTranslationRun.Z * speedTranslationRun.Z / Math.Sqrt(speedTranslationRun.Z * speedTranslationRun.Z * 2)));

        }

        private void TurnOFFThread(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ThreadLaunched) Disable();
        }

        private void orient_camera()
        {
            Stopwatch clock = new Stopwatch();
            clock.Start();
            long previous_time = clock.ElapsedMilliseconds;
            bool a_progresse;
            Vector3 VitesseTranslation;
            int[] progression = new int[3];

            double produit_scalaire;
            while (true)
            {
                a_progresse = false;
                progression[0] = 0;
                progression[1] = 0;
                progression[2] = 0;

                /************ Utile pour faire des tests ************/

                /*if (input.KeysDown.Contains(Keys.Escape))
                {
                    Menu.IsInMenu = true;
                }

                if (input.KeysDown.Contains(Keys.A))
                {
                    if (Ingame.stateinflash == 0) Ingame.stateinflash = -1;
                }*/
                if (input.KeysDown.Contains(Keys.R)) // Reset
                {
                    angle = new Vector3(0, 0, 0);
                }
                /************ END ************/

                #region Translation
                if (input.KeysDown.Contains(Keys.Space))
                {
                    progression[1]--;
                    a_progresse = true;
                }

                if (input.KeysDown.Contains(Keys.ShiftKey))
                {
                    progression[1]++;
                    a_progresse = true;
                }
                if (input.KeysDown.Contains(Keys.Up) || input.KeysDown.Contains(bindKeyAvance))
                {
                    progression[2]++;
                    a_progresse = true;
                    //Console.WriteLine("En avant !");
                }
                if (input.KeysDown.Contains(Keys.Down) || input.KeysDown.Contains(bindKeyRecule))
                {
                    progression[2]--;
                    a_progresse = true;
                    // //Console.WriteLine("En arrière !");
                }
                if (input.KeysDown.Contains(Keys.Q))
                {
                    progression[0]--;
                    a_progresse = true;
                }
                if (input.KeysDown.Contains(Keys.D))
                {
                    progression[0]++;
                    a_progresse = true;
                }
                #endregion

                long timer = clock.ElapsedMilliseconds - previous_time;
                previous_time = clock.ElapsedMilliseconds;

                #region Rotation
                if (input.KeysDown.Contains(bindKeyRight) || input.KeysDown.Contains(Keys.Right))
                {
                    angle.Y -= speedRotation.Y * timer;
                }
                if (input.KeysDown.Contains(bindKeyLeft) || input.KeysDown.Contains(Keys.Left))
                {
                    angle.Y += speedRotation.Y * timer;
                }
                if (input.KeysDown.Contains(Keys.PageUp))
                {
                    angle.X += speedRotation.X * timer;
                }
                if (input.KeysDown.Contains(Keys.PageDown))
                {
                    angle.X -= speedRotation.X * timer;
                }
                if (Cursor.Position != previous_mousepoint)
                {
                    angle.Y -= (Cursor.Position.X - previous_mousepoint.X) * speedRotation.X * 5000;
                    angle.X -= (Cursor.Position.Y - previous_mousepoint.Y) * speedRotation.Y * 5000;
                    Cursor.Position = new Point(form.DesktopBounds.Width / 2 + form.DesktopBounds.X, form.DesktopBounds.Height / 2 + form.DesktopBounds.Y);
                    previous_mousepoint = new Point(Cursor.Position.X, Cursor.Position.Y);
                }
                #endregion

                #region Blocage de l'angle X dans [pi / 4 ; 7pi / 4] et de tous les angles dans l'intervalle [0;2pi[
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
                # endregion

                VitesseTranslation = input.KeysDown.Contains(bindKeyRun) ?
                    progression[0] != 0 && progression[2] != 0 ? speedTranslationPythagoreRun : speedTranslationRun :
                    progression[0] != 0 && progression[2] != 0 ? speedTranslationPythagore : speedTranslation;

                if (a_progresse)
                {
                    produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation.Z * timer);
                    position.Z -= Convert.ToSingle(produit_scalaire * progression[2]);
                    produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation.Z * timer);
                    position.X += Convert.ToSingle(produit_scalaire * progression[2]);

                    produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation.X * timer);
                    position.Z -= Convert.ToSingle(produit_scalaire * progression[0]);
                    produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation.X * timer);
                    position.X -= Convert.ToSingle(produit_scalaire * progression[0]);

                    position.Y += speedTranslation.Y * timer * progression[1];
                }

                view = Matrix.Translation(position.X, position.Y /*+ (float)Math.Sin(Ingame.sinusoide) / 45f * 70*/, position.Z) *
                                Matrix.RotationAxis(new Vector3(0, 1, 0), angle.Y /*+ Ingame.parasitesCamera[0]*/) *
                                Matrix.RotationAxis(new Vector3(1, 0, 0), angle.X /*+ Ingame.parasitesCamera[1]*/) *
                                Matrix.RotationAxis(new Vector3(0, 0, 1), angle.Z);
                Thread.Sleep(Convert.ToInt32(16 - timer < 0 ? 0 : 16 - timer));
                //* Matrix.LookAtLH(-position, new Vector3(0, 0, 0), Vector3.UnitY);
            }
        }
    }
}
