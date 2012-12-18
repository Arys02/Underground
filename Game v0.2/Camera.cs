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
            float[] VitesseRotation = new float[3] { 0.005f, 0.005f, 0.005f };
            float[] VitesseTranslation = new float[3] { 
                Convert.ToSingle(Math.Sqrt(0.00005f)),
                Convert.ToSingle(Math.Sqrt(0.00005f)),
                Convert.ToSingle(Math.Sqrt(0.00005f)),
            };
            double produit_scalaire,produit_scalaire2;

            /************ Utile pour faire des tests ************/
            if (input.KeysDown.Contains(Keys.R)) // Reset
            {
                angle = new Vector3(0, 0, 0);
            }
            if (input.KeysDown.Contains(Keys.NumPad8))
            {
                angle.X += VitesseRotation[0];
            }
            if (input.KeysDown.Contains(Keys.NumPad2))
            {
                angle.X -= VitesseRotation[0];
            }
            /************ END ************/

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

            if (input.KeysDown.Contains(Keys.Up) || input.KeysDown.Contains(Keys.Z))
            {
                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[2]);
                position.Z -= Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[2]);
                position.X += Convert.ToSingle(produit_scalaire);
                Console.WriteLine("En avant !");
            }
            if (input.KeysDown.Contains(Keys.Down) || input.KeysDown.Contains(Keys.S))
            {
                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[2]);
                position.Z += Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[2]);
                position.X -= Convert.ToSingle(produit_scalaire);
                Console.WriteLine("En arrière !");
            }

            /************ EFFECTUER LES PROJECTIONS ************/
            if (input.KeysDown.Contains(Keys.Right))
            {
                angle.Y -= VitesseRotation[1];
                Console.WriteLine("A tribord !");
            }
            if (input.KeysDown.Contains(Keys.Left))
            {
                angle.Y += VitesseRotation[1];
                Console.WriteLine("A babord !");
            }
            /************ END ************/

            if (input.KeysDown.Contains(Keys.Q))
            {
                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[0]);
                position.Z += Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[0]);
                position.X += Convert.ToSingle(produit_scalaire);
                Console.WriteLine("Left");
            }
            if (input.KeysDown.Contains(Keys.D))
            {
                produit_scalaire = (Math.Sin(angle.Y) * VitesseTranslation[0]);
                position.Z -= Convert.ToSingle(produit_scalaire);
                produit_scalaire = (Math.Cos(angle.Y) * VitesseTranslation[0]);
                position.X -= Convert.ToSingle(produit_scalaire);
                Console.WriteLine("Right");
            }
            if (input.KeysDown.Contains(Keys.PageUp))
            {
                produit_scalaire = (Math.Cos(angle.Y)) * VitesseRotation[0];
                angle.X += Convert.ToSingle(produit_scalaire);

                produit_scalaire = (Math.Sin(angle.Y)) * VitesseRotation[0];
                angle.Z += Convert.ToSingle(produit_scalaire);
                Console.WriteLine("Montez !");
            }
            if (input.KeysDown.Contains(Keys.PageDown))
            {
                produit_scalaire = (Math.Cos(angle.Y)) * VitesseRotation[0];
                angle.X -= Convert.ToSingle(produit_scalaire);

                produit_scalaire = (Math.Sin(angle.Y)) * VitesseRotation[0];
                angle.Z -= Convert.ToSingle(produit_scalaire);
                Console.WriteLine("Coulez !");
            }

            angle.X = Convert.ToSingle(angle.X % (Math.PI * 2));
            angle.Y = Convert.ToSingle(angle.Y % (Math.PI * 2));
            angle.Z = Convert.ToSingle(angle.Z % (Math.PI * 2));
        }
    }
}
