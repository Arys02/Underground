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
        private Vector3 speedTranslation = new Vector3(0.0005f, 0.0005f, 0.0005f);
        private Vector3 speedTranslationRun = new Vector3(0.001f, 0.001f, 0.001f);
        private Vector3 speedRotation = new Vector3();
        private Vector3 speedTranslationPythagore;
        private Vector3 speedTranslationPythagoreRun;
        private Input input;
        private Debug debug;
        private Point previous_mousepoint;
        public Matrix view = new Matrix();

        public static Keys bindKeyRun = Keys.O;
        
        public Keys bindKeyAvance = Keys.Z;
        public Keys bindKeyRecule = Keys.S;
        public Keys bindKeyRight = Keys.Right;
        public Keys bindKeyLeft = Keys.Left;

        public Player(ref RenderForm form)
        {
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

        public bool orient_camera(long timer, ref RenderForm form)
        {
            //input = new Input(ref form);
            bool a_progresse = false;
            float[] VitesseRotation = new float[3] { 0.0000006f, 0.0000006f, 0.0000006f };
            Vector3 VitesseTranslation;
        
            double produit_scalaire;
            int[] progression = new int[3] { 0, 0, 0 };

            #region clavier
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

            if (input.KeysDown.Contains(bindKeyRight) || input.KeysDown.Contains(Keys.Right))
            {
                angle.Y -= VitesseRotation[1] * timer;
            }
            if (input.KeysDown.Contains(bindKeyLeft) || input.KeysDown.Contains(Keys.Left))
            {
                angle.Y += VitesseRotation[1] * timer;
            }
            /************ END ************/

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
            if (input.KeysDown.Contains(Keys.PageUp))
            {
                angle.X += VitesseRotation[0] * timer;
            }
            if (input.KeysDown.Contains(Keys.PageDown))
            {
                angle.X -= VitesseRotation[0] * timer;
            }
            #endregion
            #region souris
            if (Cursor.Position != previous_mousepoint)
            {
                angle.Y -= (Cursor.Position.X - previous_mousepoint.X) * VitesseRotation[0] * 5000;
                angle.X -= (Cursor.Position.Y - previous_mousepoint.Y) * VitesseRotation[1] * 5000;
                Cursor.Position = new Point(form.DesktopBounds.Width / 2 + form.DesktopBounds.X, form.DesktopBounds.Height / 2 + form.DesktopBounds.Y);
                previous_mousepoint = new Point(Cursor.Position.X, Cursor.Position.Y);
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
            return a_progresse;
        }
    }
}
