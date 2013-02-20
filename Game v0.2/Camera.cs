﻿using System;
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
        public Vector3 position = new Vector3(0,0,0);
        public Vector3 angle = new Vector3(0, 0, 0);
        private bool camera_altere = true;
        public Matrix view = new Matrix();

        public Camera()
        {
            view = Matrix.Translation(position.X, position.Y, position.Z) *
                                Matrix.RotationAxis(new Vector3(0, 1, 0), angle.Y) *
                                Matrix.RotationAxis(new Vector3(1, 0, 0), angle.X) *
                                Matrix.RotationAxis(new Vector3(0, 0, 1), angle.Z);
        }

        public void orient_camera(Input input, long timer)
        {
            float[] VitesseRotation = new float[3] { 0.0000006f * timer, 0.0000006f * timer, 0.0000006f * timer };
            float[] VitesseTranslation = new float[3] { 
                0.0000006f * timer,
                0.0000006f * timer,
                0.0000006f * timer,
            };
            double produit_scalaire;

            /************ Utile pour faire des tests ************/
            if (input.KeysDown.Contains(Keys.A))
            {
                Console.WriteLine("Y: {0} \t X: {1} \t Z: {2}", angle.Y, angle.X, angle.Z);
            }
            if (input.KeysDown.Contains(Keys.R)) // Reset
            {
                angle = new Vector3(0, 0, 0);
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.F1)) Ingame.PrimType = PrimitiveType.TriangleList;
            if (input.KeysDown.Contains(Keys.F2)) Ingame.PrimType = PrimitiveType.LineList;
            if (input.KeysDown.Contains(Keys.F3)) Ingame.PrimType = PrimitiveType.PointList;
            if (input.KeysDown.Contains(Keys.F4)) Ingame.Sepia = false;
            if (input.KeysDown.Contains(Keys.F5)) Ingame.Sepia = true;
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
                angle.X += VitesseRotation[0];
               // Console.WriteLine("Montez !");
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.PageDown))
            {
                angle.X -= VitesseRotation[0];
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
               //camera_altere = false;
            }
        }
    }
}
