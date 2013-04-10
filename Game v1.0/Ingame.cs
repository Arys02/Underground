using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
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
    static class Ingame
    {
        //public static List<structTile> Tuiles_a_charger = new List<structTile>();
        public static PrimitiveType PrimType = PrimitiveType.TriangleList;
        public static bool Sepia = false;
        public static bool Following_light = true;
        public static bool maximum_disallowed = false;
        public static float angle_walking = 0f;
        public static float luminosity = 1f;
        public static int stateinflash = 0; // 0 = non débuté // -1 = en décroissance // 1 = en croissance
        public static float percent = 1;
        public static Camera macamera;
        public static float sinusoide = 0f;
        public static bool a_progresse = false;


        /*public static void getTilesToLoad(Vector3 position)
        {
            Tuiles_a_charger
            position.X/32
        }*/

        public static void fevents()
        {
            Stopwatch clock = new Stopwatch();
            clock.Start();
            Int64 previous_flash = clock.ElapsedTicks;
            Int64 previous_time = clock.ElapsedTicks;
            double Ticks_Clignement = 500000;
            int Seconde_A_Attendre = 10;
            while (true)
            {
                #region flash
                if (stateinflash == -1)
                {
                    luminosity -= Convert.ToSingle((clock.ElapsedTicks - previous_time) / Ticks_Clignement);
                    if (luminosity < 0)
                    {
                        luminosity = 0;
                        stateinflash = 1;
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

        public static void recup_env()
        {
            int rayon_salles = 16;
            //string path = @"..\..\ct0_new.obj";
            //string path = @"Ressources\Game\Lighthouse.obj";
            //string path = @"Ressources\Game\cube.obj";
            //string path = @"C:\Users\b95093cf\Desktop\model.obj";
            string path = @"Ressources\Game\ct0bis.obj";
            string path2 = @"Ressources\Game\cabine.obj";
            if ((-macamera.position.X <= rayon_salles && -macamera.position.X >= -rayon_salles) && (-macamera.position.Z >= -rayon_salles && -macamera.position.Z <= rayon_salles))
            {
                //Console.WriteLine("Zone 1");
                Program.getModel(path2, Matrix.Scaling(0.5f) * Matrix.RotationY((float)Math.PI) * Matrix.RotationZ(1 * (float)Math.PI / 12) * Matrix.Translation(-1f, -1.8f, 28), 0);
                Program.getModel(path, Matrix.RotationY(0 * (float)Math.PI / 2) * Matrix.Translation(0, 0, 0), 1);
                Program.getModel(path, Matrix.RotationY(1 * (float)Math.PI / 2) * Matrix.Translation(-32, 0, 0), 2);
                Program.getModel(path, Matrix.RotationY(3 * (float)Math.PI / 2) * Matrix.Translation(0, 0, 32), 3);
                Program.freeModel(4, true);
            }
            else if ((-macamera.position.X <= -rayon_salles && -macamera.position.X >= -rayon_salles * 3) && (-macamera.position.Z >= -rayon_salles && -macamera.position.Z <= rayon_salles))
            {
                //Console.WriteLine("Zone 2");
                Program.freeModel(0, true);
                Program.getModel(path, Matrix.RotationY(0 * (float)Math.PI / 2) * Matrix.Translation(0, 0, 0), 1);
                Program.getModel(path, Matrix.RotationY(1 * (float)Math.PI / 2) * Matrix.Translation(-32, 0, 0), 2);
                Program.freeModel(3, true);
                Program.getModel(path, Matrix.RotationY(2 * (float)Math.PI / 2) * Matrix.Translation(-32, 0, 32), 4);
            }
            else if ((-macamera.position.X <= rayon_salles && -macamera.position.X >= -rayon_salles) && (-macamera.position.Z >= rayon_salles && -macamera.position.Z <= rayon_salles * 3))
            {
                //Console.WriteLine("Zone 3");
                Program.getModel(path2, Matrix.Scaling(0.5f) * Matrix.RotationY((float)Math.PI) * Matrix.RotationZ(1 * (float)Math.PI / 12) * Matrix.Translation(-1f, -1.8f, 28), 0);
                Program.getModel(path, Matrix.RotationY(0 * (float)Math.PI / 2) * Matrix.Translation(0, 0, 0), 1);
                Program.freeModel(2, true);
                Program.getModel(path, Matrix.RotationY(3 * (float)Math.PI / 2) * Matrix.Translation(0, 0, 32), 3);
                Program.getModel(path, Matrix.RotationY(2 * (float)Math.PI / 2) * Matrix.Translation(-32, 0, 32), 4);
            }
            else if ((-macamera.position.X <= -rayon_salles && -macamera.position.X >= -rayon_salles * 3) && (-macamera.position.Z >= rayon_salles && -macamera.position.Z <= rayon_salles * 3))
            {
                //Console.WriteLine("Zone 4");
                Program.getModel(path2, Matrix.Scaling(0.5f) * Matrix.RotationY((float)Math.PI) * Matrix.RotationZ(1 * (float)Math.PI / 12) * Matrix.Translation(-1f, -1.8f, 28), 0);
                Program.freeModel(1, true);
                Program.getModel(path, Matrix.RotationY(1 * (float)Math.PI / 2) * Matrix.Translation(-32, 0, 0), 2);
                Program.getModel(path, Matrix.RotationY(3 * (float)Math.PI / 2) * Matrix.Translation(0, 0, 32), 3);
                Program.getModel(path, Matrix.RotationY(2 * (float)Math.PI / 2) * Matrix.Translation(-32, 0, 32), 4);
            }
            else Console.WriteLine("Out of bounds");
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
            int nblights = 3;
            List<structModelFiles> ModelFiles = new List<structModelFiles>();
            Matrix view = Matrix.LookAtLH(new Vector3(0, 0, -0.00002f), new Vector3(0, 0, 0), Vector3.UnitY);
            Matrix proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, Program.form.ClientSize.Width / (float)Program.form.ClientSize.Height, 0.1f, 100.0f);
            Matrix viewProj = Matrix.Multiply(view, proj);
            Matrix worldViewProj = viewProj;
            Macro macro = new Macro("nblights", nblights.ToString());
            Effect effect = Effect.FromFile(Program.device, "MiniCube.fx", new Macro[] { macro }, null, "", ShaderFlags.None);
            EffectHandle technique = effect.GetTechnique(0);
            EffectHandle pass = effect.GetPass(technique, 0);
            macamera = new Camera();
            HUD hud = new HUD();
            Vector3 position_Light = -macamera.position;
            Stopwatch clock = new Stopwatch();
            clock.Start();

            // Lumière ambiante
            effect.SetValue("AmbientLightColor", new Vector4(0f, 0f, 0f, 0f));

            // Light1
            effect.SetValue("LightPosition[0]", new Vector4(position_Light, 1));
            effect.SetValue("LightDiffuseColor[0]", new Vector4(0.9f, 0.9f, 0.9f, 1));
            effect.SetValue("LightDistanceSquared[0]", 280f);

            // Light2
            if (nblights > 1)
            {
                effect.SetValue("LightPosition[1]", new Vector4(0, 2, 2, 1));
                effect.SetValue("LightDiffuseColor[1]", new Vector4(0.1f, 0.1f, 0.4f, 1));
                effect.SetValue("LightDistanceSquared[1]", 10f);

                // Light3
                if (nblights > 2)
                {
                    effect.SetValue("LightPosition[2]", new Vector4(2, 2, 2, 1));
                    effect.SetValue("LightDiffuseColor[2]", new Vector4(0.1f, 0.4f, 0.1f, 1));
                    effect.SetValue("LightDistanceSquared[2]", 10f);
                }
            }

            // Light2
            /*effect.SetValue("LightPosition1", new Vector4(0, 2, 2, 1));
            effect.SetValue("LightDiffuseColor1", new Vector4(0.1f, 0.1f, 0.4f, 1));
            effect.SetValue("LightSpecularColor1", new Vector4(0.1f, 0.1f, 0.1f, 1));
            effect.SetValue("LightDistanceSquared1", 50f);
            effect.SetValue("DiffuseColor1", new Vector4(0.5f, 0.5f, 0.5f, 1));
            effect.SetValue("SpecularColor1", new Vector4(0.5f, 0.5f, 0.5f, 1f));
            effect.SetValue("SpecularPower1", 1);

            // Light3
            effect.SetValue("LightPosition2", new Vector4(2, 2, 2, 1));
            effect.SetValue("LightDiffuseColor2", new Vector4(0.1f, 0.4f, 0.1f, 1));
            effect.SetValue("LightSpecularColor2", new Vector4(0.1f, 0.1f, 0.1f, 1));
            effect.SetValue("LightDistanceSquared2", 50f);
            effect.SetValue("DiffuseColor2", new Vector4(0.5f, 0.5f, 0.5f, 1));
            effect.SetValue("SpecularColor2", new Vector4(0.5f, 0.5f, 0.5f, 1f));
            effect.SetValue("SpecularPower2", 1);*/

            effect.SetValue("World", Matrix.Identity);
            effect.SetValue("Projection", proj);

            /*
            effect.SetValue("vecLightPos",position);
            effect.SetValue("LightRange",30.0f);
            effect.SetValue("LightColor",new Vector4(255,255,255,255));*/

            effect.Technique = technique;
            //Program.device.SetRenderState(RenderState.CullMode, false);
            Int64 previous_time = clock.ElapsedTicks;
            Int64 previous_clignement = clock.ElapsedTicks;

            //List<BoundingBox> ListeBoundingBoxes = Collision.Initialize(new Vertex[1][] { Liste_structure_models[0].Sommets });

            Matrix oldView = macamera.view;
            Vector3 oldPos = macamera.position;
            Vector3 oldAngle = macamera.angle;
            RenderLoop.Run(Program.form, () =>
            {
                Program.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                Program.device.BeginScene();

                if (Menu.IsInMenu)
                    Menu.InMenu();
                else
                {
                    recup_env();
                    //DrawingPoint Center; = form.ClientSize.Height
                    //device.SetCursorPosition(form.ClientSize.Width / 2, form.ClientSize.Height / 2);
                    //effect.SetValue("LightPosition", new Vector4(-macamera.position.X, macamera.position.Y, -macamera.position.Z, 1));
                    effect.Begin();
                    effect.BeginPass(0);
                    /*if (clock.ElapsedTicks - previous_clignement > 100000000) // 1 tick != 100ns (cf diff entre Date et Stopwatch) utiliser Stopwatch.Frequency
                    {
                        Console.WriteLine(Stopwatch.Frequency);
                        previous_clignement = clock.ElapsedTicks;
                    }*/
                    oldAngle = macamera.angle;
                    macamera.orient_camera(clock.ElapsedTicks - previous_time);
                    previous_time = clock.ElapsedTicks;

                    bool collide = false; //Collision.CheckCollisions(macamera.position);

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

                    effect.SetValue("CameraPos", new Vector4(macamera.position, 1));
                    if (Following_light)
                        position_Light = -macamera.position;
                    effect.SetValue("LightPosition[0]", new Vector4(position_Light, 1));

                    effect.SetValue("View", macamera.view);
                    effect.SetValue("Sepia", Sepia);
                    effect.SetValue("luminosity", luminosity);
                    for (int k = 0; k < Program.Liste_OBJ.Count; k++)
                    {
                        for (int i = 0; i < Program.Liste_OBJ[k].data.Count; i++)
                        {
                            Program.device.SetStreamSource(0, Program.Liste_OBJ[k].data[i].VertexBuffer, 0, Utilities.SizeOf<structVertex>());
                            int j = 0;
                            while (Program.Liste_OBJ[k].data[i].map_Kd != Program.Liste_textures[j].path)
                            {
                                j++;
                            }
                            Program.device.SetTexture(0, Program.Liste_textures[j].texture);
                            Program.device.DrawPrimitives(PrimType, 0, Program.Liste_OBJ[k].data[i].nbfaces);
                        }
                    }
                    effect.EndPass();
                    effect.End();
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
            effect.Dispose();
        }
    }
}
