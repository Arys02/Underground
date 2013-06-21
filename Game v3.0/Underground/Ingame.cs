﻿using System;
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
    struct structTile
    {
        public string modelpath;
        public Matrix Transformation;
        public int IDTile;
        public structTile(string modelpath, Matrix Transformation, int IDTile)
        {
            this.modelpath = modelpath;
            this.Transformation = Transformation;
            this.IDTile = IDTile;
        }
    }
    struct structMobSlender
    {
        public bool doit_etre_recharge;
        public bool seraaffiche;
        public Vector3 position;
        public structMobSlender(bool doit_etre_recharge, bool seraaffiche)
        {
            this.doit_etre_recharge = doit_etre_recharge;
            this.seraaffiche = seraaffiche;
            this.position = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        }
    }

    internal static class Ingame
    {
        //public static List<structTile> Tuiles_a_charger = new List<structTile>();
        //public static bool effect_is_loaded = false;
        public static PrimitiveType PrimType = PrimitiveType.TriangleList;
        public static bool Sepia = false;
        public static bool Following_light = true;
        public static bool maximum_disallowed = false;
        public static float angle_walking = 0f;
        public static float luminosity = 1f;
        public static int stateinflash = 0; // 0 = non débuté // -1 = en décroissance // 1 = en croissance
        public static float percent = 1;
        public static Camera macamera = new Camera();
        public static float sinusoide = 0f;
        public static bool a_progresse = false;
        public static structMobSlender Slender = new structMobSlender(true, true);
        public static int compteur_slender = 1;
        public static int stateInRun = 0;
        public static float percentRun;
        public static bool isTired = false;
        public static Thread tired;
       


        /*public static void getTilesToLoad(Vector3 position)
        {
            Tuiles_a_charger
            position.X/32
        }*/

        public static void istiredfct()
        {
            isTired = true;
            Thread.Sleep(5000);
            isTired = false;
        }

        public static double distenceSlender()
        {
            return
                Math.Sqrt(Math.Pow(-macamera.position.X - Slender.position.X, 2) +
                          Math.Pow(-macamera.position.Y - Slender.position.Y, 2));
        }

    public static void fevents()
        {
            Random rand = new Random();
            Stopwatch clock = new Stopwatch();
            clock.Start();
            Int64 previous_flash = clock.ElapsedTicks;
            Int64 previous_time = clock.ElapsedTicks;
            double Ticks_Clignement = 200000;
            int Seconde_A_Attendre = 6;
            int SecondeCapacityRun = 1;
            int repos = 7;
            
                   // - macamera.position     Ingame.Slender.position
           
            while (true)
            {
              
                    Console.WriteLine(distenceSlender());

                #region flash
                if (stateinflash == -1)
                {
                    luminosity -= Convert.ToSingle((clock.ElapsedTicks - previous_time) / Ticks_Clignement);
                    if (luminosity < 0)
                    {
                        luminosity = 0;
                        stateinflash = 1;
                        if (rand.Next(5) == 0)
                            Slender.doit_etre_recharge = true;
                    }
                }
                else if (stateinflash == 1)
                {
                    luminosity += Convert.ToSingle((clock.ElapsedTicks - previous_time) / Ticks_Clignement);
                    if (luminosity > 1)
                    {
                        luminosity = 1;
                        stateinflash = 0;
                        previous_flash = clock.ElapsedTicks;
                    }
                }
                else
                {
                    percent = 1f - ((float)(clock.ElapsedTicks - previous_flash) / (float)(Seconde_A_Attendre * 3500000));
                    if (percent < 0) percent = 0;
                    if (clock.ElapsedTicks > previous_flash + Seconde_A_Attendre * 3500000)
                    {
                        Console.WriteLine("Flash !");
                        stateinflash = -1;
                    }
                }
                #endregion
                #region run

                if (Program.input.KeysDown.Contains(Camera.keyrun) && !isTired && distenceSlender()<3)
                {
                   
                        percentRun +=
                            ((float) (clock.ElapsedTicks - previous_time)/(float) (SecondeCapacityRun*3500000));
                    if (percentRun > 1)
                    {                  
                        percentRun = 1;
                        if (tired == null || !tired.IsAlive)
                        {
                            tired = new Thread(istiredfct);
                            tired.Start();
                        }
                    }

                }
            
                else
                {
                    percentRun -= ((float)(clock.ElapsedTicks - previous_time) / (float)(repos * 3500000));
                    if (percentRun < 0)
                        percentRun = 0;

                }
    

                #endregion
                #region walking
                if (a_progresse)
                {
                    sinusoide += (0.0000025f * (clock.ElapsedTicks - previous_time));
                    sinusoide %= (float)(Math.PI * 2);
                }
                #endregion
                previous_time = clock.ElapsedTicks;
            }
        }

        private static List<Case> previous_cases = new List<Case>();
        public static void recup_env()
        {
            int rayon_salles = 16;
            //string pathL = @"Ressources\Game\C(L).obj";
            //string pathX = @"Ressources\Game\C(X).obj";
            //string pathIf = @"Ressources\Game\C(If).obj";
            //string pathT = @"Ressources\Game\C(T).obj";
            //string path2 = @"Ressources\Game\cabine.obj";
            string pathStatue = @"Ressources\Game\statue.obj";
            int slender_distancenbunite = 10;
            float slender_distanceapprocheunite = 2.1f;
            if (Slender.doit_etre_recharge)
            {
                Slender.doit_etre_recharge = false;
                Program.freeModel(new Point(-137, -137), true, false);
                if (Slender.seraaffiche)
                {
                    compteur_slender = compteur_slender % 4 + 1;
                    Slender.position = new Vector3(
                                             -(float)
                                              (macamera.position.X +
                                               (slender_distancenbunite - slender_distanceapprocheunite * compteur_slender) *
                                               Math.Sin(macamera.angle.Y)),
                                             0,
                                             -(float)
                                              (macamera.position.Z -
                                               (slender_distancenbunite - slender_distanceapprocheunite * compteur_slender) *
                                               (Math.Cos(macamera.angle.Y)))
                                             );
                    Program.Liste_Lights[1].Position = new Vector3(Slender.position.X,Slender.position.Y,Slender.position.Z);
                    Program.getModel(pathStatue,
                                     Matrix.Scaling(2f) * Matrix.RotationY(-macamera.angle.Y + (float)Math.PI) *
                                     Matrix.Translation(Slender.position), new Point(-137, -137));
                }
                else
                {
                    compteur_slender = 1;
                }
            }

            /// RECHERCHE S'IL Y A DES SALLES QUI DOIVENT ETRE CHARGEES PUIS RECHARGEES DE SUITE APRES ///
            //try
            //{
            // Recupere la liste des cases vues
            int a = ((int)macamera.position.X + rayon_salles) / (2 * rayon_salles);
            int b = ((int)macamera.position.Z + rayon_salles) / (2 * rayon_salles);
            //Console.WriteLine("Zone [{0},{1}] [{2},{3}]", a, b, macamera.position.X, macamera.position.Z);
            List<Case> cases = new List<Case>(Program.newmaze.maze[a, b].see);

            // Init
            bool[] idadecharger = new bool[previous_cases.Count];
            for (int i = 0; i < previous_cases.Count; i++)
            {
                idadecharger[i] = true;
            }

            // Recupere la liste des cases à décharger
            for (int i = 0; i < cases.Count; i++)
            {
                for (int j = 0; j < previous_cases.Count && cases.Count != 0; j++)
                {
                    if (previous_cases[j].x == cases[i].x && previous_cases[j].y == cases[i].y && idadecharger[j] == true)
                    {
                        idadecharger[j] = false;
                        cases.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            // Decharge les cases à être déchargées
            for (int i = 0; i < previous_cases.Count; i++)
            {
                if (idadecharger[i])
                {
                    Program.freeModel(new Point(previous_cases[i].x, previous_cases[i].y), true, false);
                    previous_cases.RemoveAt(i);
                    i--;
                }
            }

            // sauvegarde des cases
            previous_cases.AddRange(cases);

            // Charges les nouvelles cases
            for (int i = 0; i < cases.Count; i++)
            {
                if (cases[i].type == 3)
                    Program.getModel(@"Ressources\Game\C(L).obj", Matrix.RotationY((float)Math.PI / 2 * (cases[i].rot - 1))
                                     * Matrix.Translation(
                                        -(2 * rayon_salles) * cases[i].x,
                                        0,
                                        -(2 * rayon_salles) * cases[i].y),
                                     new Point(cases[i].x, cases[i].y));
                else if (cases[i].type == 2)
                    Program.getModel(@"Ressources\Game\C(T).obj", Matrix.RotationY((float)Math.PI / 2 * (cases[i].rot - 1 + 1))
                                     * Matrix.Translation(
                                        -(2 * rayon_salles) * cases[i].x,
                                        0,
                                        -(2 * rayon_salles) * cases[i].y),
                                     new Point(cases[i].x, cases[i].y));
                else if (cases[i].type == 4)
                    Program.getModel(@"Ressources\Game\C(Io).obj", Matrix.RotationY((float)Math.PI / 2 * (cases[i].rot - 1 + 1))
                                     * Matrix.Translation(
                                        -(2 * rayon_salles) * cases[i].x,
                                        0,
                                        -(2 * rayon_salles) * cases[i].y),
                                     new Point(cases[i].x, cases[i].y));
                else if (cases[i].type == 5)
                    Program.getModel(@"Ressources\Game\C(If).obj", Matrix.RotationY((float)Math.PI / 2 * (cases[i].rot - 1 + 2))
                                     * Matrix.Translation(
                                        -(2 * rayon_salles) * cases[i].x,
                                        0,
                                        -(2 * rayon_salles) * cases[i].y),
                                     new Point(cases[i].x, cases[i].y));
                else if (cases[i].type == 1)
                    Program.getModel(@"Ressources\Game\C(X).obj", Matrix.RotationY((float)Math.PI / 2 * (cases[i].rot - 1))
                                     * Matrix.Translation(
                                        -(2 * rayon_salles) * cases[i].x,
                                        0,
                                        -(2 * rayon_salles) * cases[i].y),
                                     new Point(cases[i].x, cases[i].y));
            }
        }

        //> Color codes for WriteNicely : 
        // 0: Black.
        // 1: DarkBlue.
        // 2: DarkGreen.
        // 3: DarkCyan.
        // 4: DarkRed.
        // 5: DarkMagenta.
        // 6: DarkYellow.
        // 7: Gray.
        // 8: DarkGray.
        // 9: Blue.
        //10: Green.
        //11: Cyan.
        //12: Red.
        //13: Magenta.
        //14: Yellow.
        //15: White.
        static public void ingame()
        {
            //Matrix view = Matrix.LookAtLH(new Vector3(0, 0, -0.00002f), new Vector3(0, 0, 0), Vector3.UnitY);
            List<structModelFiles> ModelFiles = new List<structModelFiles>();
            
            macamera = new Camera();
            HUD hud = new HUD();
            Stopwatch clock = new Stopwatch();
            clock.Start();

            Int64 previous_time = clock.ElapsedTicks;
            Int64 previous_clignement = clock.ElapsedTicks;
            Collision.Initialize();

            Matrix oldView = macamera.view;
            Vector3 oldPos = macamera.position;
            Vector3 oldAngle = macamera.angle;
            RenderLoop.Run(Program.form, () =>
            {
                Program.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                Program.device.BeginScene();

                if (Menu.IsInMenu)
                {
                    Menu.InMenu();
                }
                else
                {
                    recup_env();
                    //effect.SetValue("LightPosition", new Vector4(-macamera.position.X, macamera.position.Y, -macamera.position.Z, 1));
                    /*if (clock.ElapsedTicks - previous_clignement > 100000000) // 1 tick != 100ns (cf diff entre Date et Stopwatch) utiliser Stopwatch.Frequency
                    {
                        Console.WriteLine(Stopwatch.Frequency);
                        previous_clignement = clock.ElapsedTicks;
                    }*/
                    oldAngle = macamera.angle;
                    macamera.orient_camera(clock.ElapsedTicks - previous_time);

                    previous_time = clock.ElapsedTicks;

                    bool collide = Collision.CheckCollisions(macamera.position);
                    collide = false;
                    if (collide)
                    {
                        macamera.position = oldPos;
                        oldView =
                            Matrix.Translation(oldPos) *
                            Matrix.RotationAxis(new Vector3(0, 1, 0), macamera.angle.Y) *
                            Matrix.RotationAxis(new Vector3(1, 0, 0), macamera.angle.X) *
                            Matrix.RotationAxis(new Vector3(0, 0, 1), macamera.angle.Z);
                        macamera.view = oldView;
                    }
                    else
                    {
                        oldPos = macamera.position;
                        oldView = macamera.view;
                    }


                    //////////////////// ETABLISSEMENT DU FILTRE NEGATIF ////////////////////
                    float percent = Convert.ToSingle(Math.Max(0, 1 - Math.Pow(Vector3.Dot(macamera.position + Slender.position, macamera.position + Slender.position), 2) / 35000));
                    //////////////////// ETABLISSEMENT DU FILTRE NEGATIF ////////////////////

                    Program.Liste_Lights[0].Position = -Ingame.macamera.position;
                    for (int k = 0; k < Program.Liste_OBJ.Count; k++)
                    {
                        Program.Liste_OBJ[k].effect.SetValue("LightPosition[0]", new Vector4(Program.Liste_Lights[0].Position, 1));
                        Program.Liste_OBJ[k].effect.SetValue("LightPosition[1]", new Vector4(Program.Liste_Lights[1].Position.X, Program.Liste_Lights[1].Position.Y + 2f, Program.Liste_Lights[1].Position.Z, 1));
                        Program.Liste_OBJ[k].effect.SetValue("LightDistanceSquared[0]", Convert.ToSingle(Math.Min(Program.Liste_Lights[0].Range, Math.Pow(Vector3.Dot(macamera.position + Slender.position, macamera.position + Slender.position), 0.7) / Program.Liste_Lights[0].Range * 40)));
                        Program.Liste_OBJ[k].effect.SetValue("LightDistanceSquared[1]", Program.Liste_Lights[1].Range * 100);
                        Program.Liste_OBJ[k].effect.SetValue("CameraPos", new Vector4(macamera.position, 1));
                        Program.Liste_OBJ[k].effect.SetValue("percent_Negatif", percent);
                        Program.Liste_OBJ[k].effect.SetValue("View", macamera.view);
                        Program.Liste_OBJ[k].effect.SetValue("Sepia", Sepia);
                        Program.Liste_OBJ[k].effect.SetValue("luminosity", luminosity);
                        Program.Liste_OBJ[k].effect.Begin();
                        Program.Liste_OBJ[k].effect.BeginPass(0);
                        /*structOBJ a = Program.Liste_OBJ[k];
                        a.Transformation *= Matrix.Translation(new Vector3(0,0,0.0001f));
                        Program.Liste_OBJ[k] = a;*/
                        Program.Liste_OBJ[k].effect.SetValue("World", Program.Liste_OBJ[k].Transformation);
                        Program.Liste_OBJ[k].effect.SetValue("WorldInverseTranspose", Matrix.Transpose(Matrix.Invert(Program.Liste_OBJ[k].Transformation)));
                        for (int i = 0; i < Program.Liste_OBJ[k].data.Count; i++)
                        {
                            if (Program.Liste_OBJ[k].sera_affiche)
                            {
                                Program.device.SetStreamSource(0, Program.Liste_OBJ[k].data[i].VertexBuffer, 0, Utilities.SizeOf<structVertex>());
                                Program.device.SetTexture(0, Program.Liste_textures[Program.getTexture(Program.Liste_OBJ[k].data[i].map_Kd)].texture);
                                Program.device.SetTexture(1, Program.Liste_textures[Program.getTexture(Program.Liste_OBJ[k].data[i].map_Ns)].texture);
                                Program.device.DrawPrimitives(PrimType, 0, Program.Liste_OBJ[k].data[i].nbfaces);
                            }
                        }
                        Program.Liste_OBJ[k].effect.EndPass();
                        Program.Liste_OBJ[k].effect.End();
                    }
                    hud.Display_HUD();
                }
                Program.device.EndScene(); // Okay
                Program.device.Present();
            });
            Menu.Dispose();
            hud.Dispose();
            for (int k = 0; k < Program.Liste_OBJ.Count; k++)
            {
                for (int i = 0; i < Program.Liste_OBJ[k].data.Count; i++)
                {
                    Program.Liste_OBJ[k].data[i].VertexBuffer.Dispose();
                }
            }
            foreach (structTexture TextureElt in Program.Liste_textures)
            {
                TextureElt.texture.Dispose();
            }
            clock.Stop();
        }
    }
}