using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.IO;
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
        private Vector3 position = new Vector3(0,0,0);
        private Vector3 angle = new Vector3(0, 0, 0);
        private bool camera_altere = true;
        public Matrix view = new Matrix();

        public Camera()
        {
            view = Matrix.Translation(position.X, position.Y, position.Z) *
                                Matrix.RotationAxis(new Vector3(0, 1, 0), angle.Y) *
                                Matrix.RotationAxis(new Vector3(1, 0, 0), angle.X) *
                                Matrix.RotationAxis(new Vector3(0, 0, 1), angle.Z);
        }

        public void orient_camera(Input input)
        {
            float[] VitesseRotation = new float[3] { 0.1f, 0.1f, 0.1f };
            float[] VitesseTranslation = new float[3] { 
                Convert.ToSingle((0.5f)),
                Convert.ToSingle((0.5f)),
                Convert.ToSingle((0.5f)),
            };
            double produit_scalaire;

            /************ Utile pour faire des tests ************/
            if (input.KeysDown.Contains(Keys.A))
            {
                Console.WriteLine("Y: {0} \t X: {1} \t Z: {2}", angle.Y, angle.X, angle.Z);
            }
            if (input.KeysDown.Contains(Keys.T)) // Reset
            {
                angle = new Vector3(Convert.ToSingle(Math.PI / 10), Convert.ToSingle(2 * Math.PI / 2), 0);
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.R)) // Reset
            {
                angle = new Vector3(0, 0, 0);
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.E)) // Reset
            {
                angle = new Vector3(0, Convert.ToSingle(Math.PI/4), 0);
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.NumPad9))
            {
                angle.Z += VitesseRotation[0];
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.NumPad3))
            {
                angle.Z -= VitesseRotation[0];
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.NumPad8))
            {
                angle.Y += VitesseRotation[0];
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.NumPad2))
            {
                angle.Y -= VitesseRotation[0];
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.NumPad7))
            {
                angle.X += VitesseRotation[0];
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.NumPad1))
            {
                angle.X -= VitesseRotation[0];
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.Enter))
            {
               // Console.WriteLine("Equal");
                angle.X = angle.Z;
                camera_altere = true;
            }
            /************ END ************/

            if (input.KeysDown.Contains(Keys.Space))
            {
                position.Y -= VitesseTranslation[1];
              //  Console.WriteLine("Vers le haut !");
                camera_altere = true;
            }

            if (input.KeysDown.Contains(Keys.ShiftKey))
            {
                position.Y += VitesseTranslation[1];
               // Console.WriteLine("Vers le bas !");
                camera_altere = true;
            }

            view = Matrix.LookAtLH(position,new Vector3(0, 0, 0), Vector3.UnitY);

            if (input.KeysDown.Contains(Keys.Up) || input.KeysDown.Contains(Keys.Z))
            {
                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[2]);
                position.Z -= Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[2]);
                position.X += Convert.ToSingle(produit_scalaire);
               // Console.WriteLine("En avant !");
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.Down) || input.KeysDown.Contains(Keys.S))
            {
                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[2]);
                position.Z += Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[2]);
                position.X -= Convert.ToSingle(produit_scalaire);
               // Console.WriteLine("En arrière !");
                camera_altere = true;
            }

            /************ EFFECTUER LES PROJECTIONS ************/
            if (input.KeysDown.Contains(Keys.Right))
            {
                angle.Y -= VitesseRotation[1];
               // Console.WriteLine("A tribord !");
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.Left))
            {
                angle.Y += VitesseRotation[1];
              //  Console.WriteLine("A babord !");
                camera_altere = true;
            }
            /************ END ************/

            if (input.KeysDown.Contains(Keys.Q))
            {
                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[0]);
                position.Z += Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[0]);
                position.X += Convert.ToSingle(produit_scalaire);
               // Console.WriteLine("Left");
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.D))
            {
                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[0]);
                position.Z -= Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[0]);
                position.X -= Convert.ToSingle(produit_scalaire);
               // Console.WriteLine("Right");
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.PageUp))
            {
                produit_scalaire = (Math.Cos(angle.Y)) * VitesseRotation[0];
                angle.X += Convert.ToSingle(produit_scalaire);

                produit_scalaire = (Math.Sin(angle.Y)) * VitesseRotation[0];
                angle.Z += Convert.ToSingle(produit_scalaire);
               // Console.WriteLine("Montez !");
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.PageDown))
            {
                produit_scalaire = (Math.Cos(angle.Y)) * VitesseRotation[0];
                angle.X -= Convert.ToSingle(produit_scalaire);


                produit_scalaire = (Math.Sin(angle.Y)) * VitesseRotation[0];
                angle.Z -= Convert.ToSingle(produit_scalaire);
               // Console.WriteLine("Coulez !");
                camera_altere = true;
            }

            angle.X = Convert.ToSingle(angle.X % (Math.PI * 2));
            if (angle.X < 0) angle.X += Convert.ToSingle(Math.PI * 2);
            angle.Y = Convert.ToSingle(angle.Y % (Math.PI * 2));
            if (angle.Y < 0) angle.Y += Convert.ToSingle(Math.PI * 2);
            angle.Z = Convert.ToSingle(angle.Z % (Math.PI * 2));
            if (angle.Z < 0) angle.Z += Convert.ToSingle(Math.PI * 2);

            if (camera_altere)
            {
                view = Matrix.Translation(position.X, position.Y, position.Z) *
                                Matrix.RotationAxis(new Vector3(0, 1, 0), angle.Y) *
                                Matrix.RotationAxis(new Vector3(1, 0, 0), angle.X) *
                                Matrix.RotationAxis(new Vector3(0, 0, 1), angle.Z);
               // camera_altere = false;
            }
        }
    }
}
