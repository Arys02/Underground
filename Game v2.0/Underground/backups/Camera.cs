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
        public Vector3 position = new Vector3(0, -2.5f, 0);
        private Vector3 old_position;
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

        public bool orient_camera(Input input, long previous_time)
        {
            old_position = position;
            int[] deplacement = new int[3]; // 0 = avant/arrière | 1 = lateral | 2 = haut/bas
            float[] VitesseRotation = new float[3] { 0.0000006f * previous_time, 0.0000006f * previous_time, 0.0000006f * previous_time };
            float[] VitesseTranslation = new float[3] { 
                0.0000006f * previous_time,
                0.0000006f * previous_time,
                0.0000006f * previous_time,
            };

            /************ Utile pour faire des tests ************/
            /*if (input.KeysDown.Contains(Keys.A))
            {
                Console.WriteLine("Y: {0} \t X: {1} \t Z: {2}", angle.Y, angle.X, angle.Z);
            }*/
            if (input.KeysDown.Contains(Keys.R)) // Reset
            {
                angle = new Vector3(0, 0, 0);
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.Y)) Ingame.PrimType = PrimitiveType.TriangleList;
            if (input.KeysDown.Contains(Keys.U)) Ingame.PrimType = PrimitiveType.LineList;
            if (input.KeysDown.Contains(Keys.I)) Ingame.PrimType = PrimitiveType.PointList;
            if (input.KeysDown.Contains(Keys.G)) Ingame.Sepia = false;
            if (input.KeysDown.Contains(Keys.H)) Ingame.Sepia = true;
            if (input.KeysDown.Contains(Keys.A)) Ingame.Following_light = true;
            if (input.KeysDown.Contains(Keys.E)) Ingame.Following_light = false;
            /************ END ************/

            if (input.KeysDown.Contains(Keys.Space))
            {
                deplacement[2]--;
                //position.Y -= VitesseTranslation[1];
              //  Console.WriteLine("Vers le haut !");
                camera_altere = true;
            }

            if (input.KeysDown.Contains(Keys.ShiftKey))
            {
                deplacement[2]++;
                //position.Y += VitesseTranslation[1];
               // Console.WriteLine("Vers le bas !");
                camera_altere = true;
            }
            
            view = Matrix.LookAtLH(position,new Vector3(0, 0, 0), Vector3.UnitY);

            if (input.KeysDown.Contains(Keys.Up) || input.KeysDown.Contains(Keys.Z))
            {
                deplacement[0]++;
                /*produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[2]);
                position.Z -= Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[2]);
                position.X += Convert.ToSingle(produit_scalaire);*/
               // Console.WriteLine("En avant !");
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.Down) || input.KeysDown.Contains(Keys.S))
            {
                deplacement[0]--;
                /*produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[2]);
                position.Z += Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[2]);
                position.X -= Convert.ToSingle(produit_scalaire);*/
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
                deplacement[1]--;
                /*produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[0]);
                position.Z += Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[0]);
                position.X += Convert.ToSingle(produit_scalaire);*/
               // Console.WriteLine("Left");
                camera_altere = true;
            }
            if (input.KeysDown.Contains(Keys.D))
            {
                deplacement[1]++;
                /*produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[0]);
                position.Z -= Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[0]);
                position.X -= Convert.ToSingle(produit_scalaire);*/
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

            position.Z = position.Z
                - Convert.ToSingle(deplacement[1] * Math.Sin(angle.Y) * VitesseTranslation[0])
                - Convert.ToSingle(deplacement[0] * Math.Cos(angle.Y) * VitesseTranslation[2]);
            position.X = position.X
                - Convert.ToSingle(deplacement[1] * Math.Cos(angle.Y) * VitesseTranslation[0])
                + Convert.ToSingle(deplacement[0] * Math.Sin(angle.Y) * VitesseTranslation[2]);
            position.Y += VitesseTranslation[1] * deplacement[2];

            angle.X = Convert.ToSingle(angle.X % (Math.PI * 2));
            if (angle.X < 0) angle.X += Convert.ToSingle(Math.PI * 2);
            angle.Y = Convert.ToSingle(angle.Y % (Math.PI * 2));
            if (angle.Y < 0) angle.Y += Convert.ToSingle(Math.PI * 2);
            angle.Z = Convert.ToSingle(angle.Z % (Math.PI * 2));
            if (angle.Z < 0) angle.Z += Convert.ToSingle(Math.PI * 2);

            return camera_altere;
        }
    }
}
