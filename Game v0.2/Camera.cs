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

namespace MiniCube
{
    static class Camera
    {
        static public void orient_camera(ref Input input, ref Vector3 position, ref Vector3 angle)
        {
            float[] VitesseRotation = new float[3] { .005f, 0.005f, 0.005f };
            float[] VitesseTranslation = new float[3] { 
                Convert.ToSingle(Math.Sqrt(.001f)),
                Convert.ToSingle(Math.Sqrt(.001f)),
                Convert.ToSingle(Math.Sqrt(.001f)),
            };
            double produit_scalaire;

            if (input.KeysDown.Contains(Keys.Space))
            {
                position.Y -= VitesseTranslation[1];
                Console.WriteLine("Vers le haut !");
            }

            if (input.KeysDown.Contains(Keys.ShiftKey))
            {
                position.Y += VitesseTranslation[1];
                Console.WriteLine("Vers le bas !");
            }

            if (input.KeysDown.Contains(Keys.Up))
            {
                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[2]);
                position.Z -= Convert.ToSingle(produit_scalaire);


                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[2]);
                position.X += Convert.ToSingle(produit_scalaire);


                Console.WriteLine("En avant !");
            }
            if (input.KeysDown.Contains(Keys.Down))
            {

                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[2]);
                position.Z += Convert.ToSingle(produit_scalaire);


                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[2]);
                position.X -= Convert.ToSingle(produit_scalaire);
                Console.WriteLine("En arrière !");
            }

            if (input.KeysDown.Contains(Keys.D))
            {
                angle.Y -= VitesseRotation[1];
                Console.WriteLine("A tribord !");
            }

            if (input.KeysDown.Contains(Keys.A) || input.KeysDown.Contains(Keys.Q))
            {
                angle.Y += VitesseRotation[1];
                Console.WriteLine("A babord !");
            }
            if (input.KeysDown.Contains(Keys.W) || input.KeysDown.Contains(Keys.Z))
            {
                angle.X += VitesseRotation[1];
                Console.WriteLine("en haut!");
            }
            if (input.KeysDown.Contains(Keys.S))
            {
                angle.X -= VitesseRotation[1];
                Console.WriteLine("en bas!");
            }
            if (input.KeysDown.Contains(Keys.Left))
            {
                //position.X += VitesseTranslation[0];

                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[0]);
                position.Z += Convert.ToSingle(produit_scalaire);


                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[0]);
                position.X += Convert.ToSingle(produit_scalaire);
                Console.WriteLine("Left");
            }
            if (input.KeysDown.Contains(Keys.Right))
            {
                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[0]);
                position.Z -= Convert.ToSingle(produit_scalaire);


                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[0]);
                position.X -= Convert.ToSingle(produit_scalaire);
                Console.WriteLine("Right");
            }
            angle.X = Convert.ToSingle(angle.X % (Math.PI * 2));
            angle.Y = Convert.ToSingle(angle.Y % (Math.PI * 2));
            angle.Z = Convert.ToSingle(angle.Z % (Math.PI * 2));
        }
    }
}
