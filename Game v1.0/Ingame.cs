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
    static class Ingame
    {
        public static PrimitiveType PrimType = PrimitiveType.TriangleList;
        public static bool Sepia = false;
        public static bool Following_light = true;
        public static bool maximum_disallowed = false;
        
        public static void recup_env(ref List<Model> Liste_Models, ref List<Byte[]> ModelFiles)
        {
            Liste_Models.Clear();

            /*for (int i = 0; i < ModelFiles.Count; i++)
            {
                ObjLoader.read_obj(ModelFiles[i], Matrix.Translation(i * 0, 0, i * 10) * Matrix.RotationY((float) Math.PI/2), ref Liste_Models);
            }*/
            ObjLoader.read_obj(ModelFiles[0], Matrix.RotationY(0 * (float)Math.PI / 2) * Matrix.Translation(0, 0, 0), ref Liste_Models);
            ObjLoader.read_obj(ModelFiles[1], Matrix.RotationY(3 * (float)Math.PI / 2) * Matrix.Translation(-2, 0, 34), ref Liste_Models);
            ObjLoader.read_obj(ModelFiles[2], Matrix.RotationY(1 * (float)Math.PI / 2) * Matrix.Translation(-34, 0, -2), ref Liste_Models);
            ObjLoader.read_obj(ModelFiles[3], Matrix.RotationY(2 * (float)Math.PI / 2) * Matrix.Translation(-36, 0, 32), ref Liste_Models);
            ObjLoader.read_obj(ModelFiles[4], Matrix.Scaling(0.5f) * Matrix.RotationZ(1 * (float)Math.PI / 12) * Matrix.Translation(-3f, -1.8f, 6), ref Liste_Models);
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
            List<Byte[]> ModelFiles = new List<Byte[]>();
            List<Model> Liste_Models = new List<Model>();
            Texture[] Texture_ressource = new Texture[5];
            Matrix view = Matrix.LookAtLH(new Vector3(0, 0, -0.00002f), new Vector3(0, 0, 0), Vector3.UnitY);
            Matrix proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, Program.form.ClientSize.Width / (float)Program.form.ClientSize.Height, 0.1f, 100.0f);
            Matrix viewProj = Matrix.Multiply(view, proj);
            Matrix worldViewProj = viewProj;
            Macro macro = new Macro("nblights", nblights.ToString());
            Effect effect = Effect.FromFile(Program.device, "MiniCube.fx", new Macro[] { macro }, null, "", ShaderFlags.None);
            //effect.SetValue("nblights", nblights);
            EffectHandle technique = effect.GetTechnique(0);
            EffectHandle pass = effect.GetPass(technique, 0);
            Camera macamera = new Camera();
            Vector3 position_Light = -macamera.position;
            Stopwatch clock = new Stopwatch();
            clock.Start();


            //string path = @"..\..\ct0_new.obj";
            //string path = @"Ressources\Game\Lighthouse.obj";
            //string path = @"Ressources\Game\cube.obj";
            //string path = @"C:\Users\b95093cf\Desktop\model.obj";
            string path = @"Ressources\Game\ct0.obj";
            string path2 = @"Ressources\Game\cabine.obj";

            Byte[] fichier = File.ReadAllBytes(path);
            Byte[] fichier2 = File.ReadAllBytes(path2);
            ModelFiles.Add(fichier);
            ModelFiles.Add(fichier);
            ModelFiles.Add(fichier);
            ModelFiles.Add(fichier);
            ModelFiles.Add(fichier2);
            //ModelFiles.Add(fichier);
            //ModelFiles.Add(fichier);
            Texture_ressource[0] = Texture.FromMemory(Program.device, File.ReadAllBytes("null.bmp"));
            for (int i = 1; i < 5; i++)
            {
                if (i == 1)
                    //Texture_ressource[i] = Texture.FromFile(device, @"Ressources\Game\Images\dev.png");
                    Texture_ressource[i] = Texture.FromMemory(Program.device, File.ReadAllBytes(@"Ressources\Game\Images\Beton20.jpg"));
                else if (i == 2)
                    Texture_ressource[i] = Texture.FromMemory(Program.device, File.ReadAllBytes(@"Ressources\Game\Images\porte-beton-texture-en-beton_19-136906.jpg"));
                else if (i == 3)
                    Texture_ressource[i] = Texture.FromMemory(Program.device, File.ReadAllBytes(@"Ressources\Game\Images\dev2.png"));
                else if (i == 4)
                    Texture_ressource[i] = Texture.FromMemory(Program.device, File.ReadAllBytes(@"Ressources\Game\Images\woodfloor.bmp"));
                else
                    Texture_ressource[i] = Texture.FromMemory(Program.device, File.ReadAllBytes(@"Ressources\Game\Images\Beton20.jpg"));
            }

            recup_env(ref Liste_Models, ref ModelFiles);

            for (int i = 0; i < Liste_Models.Count; i++)
            {
                Liste_Models[i].VertexBuffer.Lock(0, 0, LockFlags.DoNotWait).WriteRange(Liste_Models[i].Sommets);
                Liste_Models[i].VertexBuffer.Unlock();
            }

            // Lumière ambiante
            effect.SetValue("AmbientLightColor", new Vector4(0f, 0f, 0f, 1f));

            // Light1
            effect.SetValue("LightPosition[0]", new Vector4(position_Light, 1));
            effect.SetValue("LightDiffuseColor[0]", new Vector4(0.9f, 0.9f, 0.9f, 1));
            effect.SetValue("LightDistanceSquared[0]", 50f);

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

                    //worldViewProj = macamera.view * proj;
                    //effect.SetValue("worldViewProj", worldViewProj);

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
                    /*if (maximum_disallowed)
                    {
                        effect.SetValue("nblights", 1);
                        effect.SetValue("test", 100000f);
                    }
                    else
                    {
                        effect.SetValue("nblights", 3);
                        effect.SetValue("test", 1f);
                    }*/
                    for (int i = 0; i < Liste_Models.Count; i++)
                    {
                        Program.device.SetStreamSource(0, Liste_Models[i].VertexBuffer, 0, Utilities.SizeOf<Vertex>());
                        int j = 0;
                        while (Liste_Models[i].map_Kd != Program.Liste_textures[j].path)
                        {
                            j++;
                        }
                        Program.device.SetTexture(0, Program.Liste_textures[j].texture);
                        Program.device.DrawPrimitives(PrimType, 0, Liste_Models[i].nbfaces);
                    }

                    effect.EndPass();
                    effect.End();
                }
                Program.device.EndScene(); // Okay
                Program.device.Present();
            });

            for (int i = 0; i < Liste_Models.Count; i++)
            {
                Liste_Models[i].VertexBuffer.Dispose();
            }
            clock.Stop();
            effect.Dispose();
        }
    }
}
