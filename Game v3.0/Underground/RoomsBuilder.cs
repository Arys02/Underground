using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Drawing;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using System.Windows.Forms;
using Color = SharpDX.Color;
using Device = SharpDX.Direct3D9.Device;
using Effect = SharpDX.Direct3D9.Effect;

namespace Underground
{
    class RoomsBuilder
    {
        static string obj = "mur7";

        public void buildLRoom(int rayon_salles, Case Case)
        {
            
            #region Pièce centrale            
            Program.getModel(@"Ressources\Game\solplafond2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));

            Program.getModel(@"Ressources\Game\"+obj+".obj", Matrix.RotationY(Convert.ToSingle(-Math.PI / 2))
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI))
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));

            Program.getModel(@"Ressources\Game\col2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x + rayon_salles / 3,
                                0,
                                -(2 * rayon_salles) * Case.y + rayon_salles / 3),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\col2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x + rayon_salles / 3,
                                0,
                                -(2 * rayon_salles) * Case.y - rayon_salles / 3),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\col2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x - rayon_salles / 3,
                                0,
                                -(2 * rayon_salles) * Case.y + rayon_salles / 3),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\col2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x - rayon_salles / 3,
                                0,
                                -(2 * rayon_salles) * Case.y - rayon_salles / 3),
                             new Point(Case.x, Case.y));
            #endregion
            #region Pièce 1
            Program.getModel(@"Ressources\Game\solplafond2.obj", Matrix.RotationY((float)Math.PI / 2 * -(Case.rot - 1))
                             * Matrix.Translation(0, 0, 2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));

            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI))
                             * Matrix.Translation(0, 0, 2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(0))
                             * Matrix.Translation(0, 0, 2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            #endregion
            #region Pièce 2
            Program.getModel(@"Ressources\Game\solplafond2.obj", Matrix.RotationY((float)Math.PI / 2 * -(Case.rot - 1))
                             * Matrix.Translation(-2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(3 * Math.PI / 2))
                             * Matrix.Translation(-2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI / 2))
                             * Matrix.Translation(-2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            #endregion
        }
        public void buildTRoom(int rayon_salles, Case Case)
        {
            #region Pièce centrale
            Program.getModel(@"Ressources\Game\solplafond2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(0))
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));

            Program.getModel(@"Ressources\Game\col2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x + rayon_salles / 3,
                                0,
                                -(2 * rayon_salles) * Case.y + rayon_salles / 3),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\col2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x + rayon_salles / 3,
                                0,
                                -(2 * rayon_salles) * Case.y - rayon_salles / 3),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\col2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x - rayon_salles / 3,
                                0,
                                -(2 * rayon_salles) * Case.y + rayon_salles / 3),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\col2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x - rayon_salles / 3,
                                0,
                                -(2 * rayon_salles) * Case.y - rayon_salles / 3),
                             new Point(Case.x, Case.y));
            #endregion
            #region Pièce 1
            Program.getModel(@"Ressources\Game\solplafond2.obj", Matrix.RotationY((float)Math.PI / 2 * -(Case.rot - 1))
                             * Matrix.Translation(0, 0, 2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(0))
                             * Matrix.Translation(0, 0, 2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI))
                             * Matrix.Translation(0, 0, 2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            #endregion
            #region Pièce 3
            Program.getModel(@"Ressources\Game\solplafond2.obj", Matrix.RotationY((float)Math.PI / 2 * -(Case.rot - 1))
                             * Matrix.Translation(0, 0, -2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(0))
                             * Matrix.Translation(0, 0, -2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI))
                             * Matrix.Translation(0, 0, -2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            #endregion
            #region Pièce 4
            Program.getModel(@"Ressources\Game\solplafond2.obj", Matrix.RotationY((float)Math.PI / 2 * -(Case.rot - 1))
                             * Matrix.Translation(2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(3 * Math.PI / 2))
                             * Matrix.Translation(2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI / 2))
                             * Matrix.Translation(2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            #endregion
        }
        public void buildXRoom(int rayon_salles, Case Case)
        {
            #region Pièce centrale
            Program.getModel(@"Ressources\Game\solplafond2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));

            Program.getModel(@"Ressources\Game\col2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x + rayon_salles / 3,
                                0,
                                -(2 * rayon_salles) * Case.y + rayon_salles / 3),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\col2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x + rayon_salles / 3,
                                0,
                                -(2 * rayon_salles) * Case.y - rayon_salles / 3),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\col2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x - rayon_salles / 3,
                                0,
                                -(2 * rayon_salles) * Case.y + rayon_salles / 3),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\col2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x - rayon_salles / 3,
                                0,
                                -(2 * rayon_salles) * Case.y - rayon_salles / 3),
                             new Point(Case.x, Case.y));
            #endregion
            #region Pièce 1
            Program.getModel(@"Ressources\Game\solplafond2.obj", Matrix.RotationY((float)Math.PI / 2 * -(Case.rot - 1))
                             * Matrix.Translation(0, 0, 2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\"+obj+".obj", Matrix.RotationY(Convert.ToSingle(0))
                             * Matrix.Translation(0, 0, 2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI))
                             * Matrix.Translation(0, 0, 2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            #endregion
            #region Pièce 2
            Program.getModel(@"Ressources\Game\solplafond2.obj", Matrix.RotationY((float)Math.PI / 2 * -(Case.rot - 1))
                             * Matrix.Translation(-2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(3 * Math.PI / 2))
                             * Matrix.Translation(-2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI / 2))
                             * Matrix.Translation(-2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            #endregion
            #region Pièce 3
            Program.getModel(@"Ressources\Game\solplafond2.obj", Matrix.RotationY((float)Math.PI / 2 * -(Case.rot - 1))
                             * Matrix.Translation(0, 0, -2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(0))
                             * Matrix.Translation(0, 0, -2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI))
                             * Matrix.Translation(0, 0, -2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            #endregion
            #region Pièce 4
            Program.getModel(@"Ressources\Game\solplafond2.obj", Matrix.RotationY((float)Math.PI / 2 * -(Case.rot - 1))
                             * Matrix.Translation(2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(3 * Math.PI / 2))
                             * Matrix.Translation(2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI / 2))
                             * Matrix.Translation(2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            #endregion
        }
        public void buildIoRoom(int rayon_salles, Case Case)
        {
            #region Pièce centrale
            Program.getModel(@"Ressources\Game\solplafond2.obj",
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(3 * Math.PI / 2))
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI / 2))
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));

            Program.getModel(@"Ressources\Game\col2.obj", Matrix.Translation(0, 0, -rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\col2.obj", Matrix.Translation(0, 0, rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            #endregion
            #region Pièce 2
            Program.getModel(@"Ressources\Game\solplafond2.obj", Matrix.RotationY((float)Math.PI / 2 * -(Case.rot - 1))
                             * Matrix.Translation(-2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(3 * Math.PI / 2))
                             * Matrix.Translation(-2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI / 2))
                             * Matrix.Translation(-2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            #endregion
            #region Pièce 4
            Program.getModel(@"Ressources\Game\solplafond2.obj", Matrix.RotationY((float)Math.PI / 2 * -(Case.rot - 1))
                             * Matrix.Translation(2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(3 * Math.PI / 2))
                             * Matrix.Translation(2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI / 2))
                             * Matrix.Translation(2 * rayon_salles / 3, 0, 0)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            #endregion
        }
        public void buildIfRoom(int rayon_salles, Case Case)
        {
            #region Pièce centrale
            Program.getModel(@"Ressources\Game\solplafond2.obj", 
                             Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(0))
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI))
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI / 2))
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));

            Program.getModel(@"Ressources\Game\col2.obj", Matrix.Translation(rayon_salles / 3, 0, rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\col2.obj", Matrix.Translation(-rayon_salles / 3, 0, rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            #endregion
            #region Pièce 3
            Program.getModel(@"Ressources\Game\solplafond2.obj", Matrix.RotationY((float)Math.PI / 2 * -(Case.rot - 1))
                             * Matrix.Translation(0, 0, -2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(0))
                             * Matrix.Translation(0, 0, -2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            Program.getModel(@"Ressources\Game\" + obj + ".obj", Matrix.RotationY(Convert.ToSingle(Math.PI))
                             * Matrix.Translation(0, 0, -2 * rayon_salles / 3)
                             * Matrix.RotationY((float)Math.PI / 2 * (Case.rot - 1))
                             * Matrix.Translation(
                                -(2 * rayon_salles) * Case.x,
                                0,
                                -(2 * rayon_salles) * Case.y),
                             new Point(Case.x, Case.y));
            #endregion
        }
    }
}
