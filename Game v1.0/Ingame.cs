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
        public static void recup_env(ref Device device, ref List<VertexBuffer> vertices, ref List<Vertex[]> ListeVerticesFinal, ref List<int> ModelSizes, ref List<Byte[]> ModelFiles, int nbmodels)
        {
            int nbmodelfaces = 0;
            int nbsommets = 0;
            int nbnormales = 0;
            int nbtextures = 0;
            ListeVerticesFinal.Clear();
            ModelSizes.Clear();
            for (int i = 0; i < nbmodels; i++)
            {
                nbmodelfaces = 0;
                nbsommets = 0;
                nbnormales = 0;
                nbtextures = 0;
                ListeVerticesFinal.Add(ObjLoader.read_obj(ModelFiles[i], new Vector4(i * 0, ((float)i) / 10, i * 10, 0), ref nbmodelfaces, ref nbsommets, ref nbnormales, ref nbtextures));
                ModelSizes.Add(nbmodelfaces);
                vertices.Add(new VertexBuffer(device,
                            (
                                Utilities.SizeOf<Vector4>()
                                + Utilities.SizeOf<Vector2>()
                                + Utilities.SizeOf<Vector4>()
                                + Utilities.SizeOf<Vector4>() // NORMAL
                            ) * 3 * nbmodelfaces, Usage.WriteOnly, VertexFormat.None, Pool.Managed)
                );
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
            const int nbmodels = 1;
            int VerticesCount = 0;
            List<Byte[]> ModelFiles = new List<Byte[]>();
            List<Vertex[]> ListeVerticesFinal = new List<Vertex[]>();
            List<VertexBuffer> Vertices = new List<VertexBuffer>();
            List<int> SizeModels = new List<int>();
            Vertex[][] VerticesFinal = new Vertex[nbmodels][];
            Texture[] Texture_ressource = new Texture[nbmodels];
            //Vector3 position = new Vector3(0, -10, 20), angle = new Vector3(0f, 0f, 0f);
            Matrix view = Matrix.LookAtLH(new Vector3(0, 0, -0.00002f), new Vector3(0, 0, 0), Vector3.UnitY);
            Matrix proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, Program.form.ClientSize.Width / (float)Program.form.ClientSize.Height, 0.1f, 100.0f);
            Matrix viewProj = Matrix.Multiply(view, proj);
            Matrix worldViewProj = viewProj;
            Effect effect = Effect.FromFile(Program.device, "MiniCube.fx", ShaderFlags.None);
            EffectHandle technique = effect.GetTechnique(0);
            EffectHandle pass = effect.GetPass(technique, 0);
            Camera macamera = new Camera();
            Vector3 position_Light = -macamera.position;
            Stopwatch clock = new Stopwatch();
            clock.Start();


            //string path = @"..\..\ct0_new.obj";
            string path = @"Ressources\Game\Lighthouse.obj";

            //string path = @"Ressources\Game\ct0.obj";

            Byte[] fichier = File.ReadAllBytes(path);
            for (int i = 0; i < nbmodels; i++)
            {
                ModelFiles.Add(fichier);
                //ModelFiles.Add(fichier);
                if (i == 0)
                    //Texture_ressource[i] = Texture.FromFile(device, @"Ressources\Game\Images\dev.png");
                    Texture_ressource[i] = Texture.FromFile(Program.device, @"Ressources\Game\Images\Beton20.jpg");
                else if (i == 1)
                {
                    Texture_ressource[i] = Texture.FromFile(Program.device, @"porte-beton-texture-en-beton_19-136906.jpg");
                }
                else if (i == 2)
                {
                    Texture_ressource[i] = Texture.FromFile(Program.device, @"dev2.png");
                }
                else if (i == 3)
                {
                    Texture_ressource[i] = Texture.FromFile(Program.device, @"woodfloor.bmp");
                }
                else
                {
                    Texture_ressource[i] = Texture.FromFile(Program.device, @"Beton20.jpg");
                }
            }

            recup_env(ref Program.device, ref Vertices, ref ListeVerticesFinal, ref SizeModels, ref ModelFiles, nbmodels);
            VerticesCount = ListeVerticesFinal.Count;
            for (int i = 0; i < nbmodels; i++)
            {
                VerticesFinal[i] = ListeVerticesFinal[i];
            }

            for (int i = 0; i < VerticesCount; i++)
            {
                Vertices[i].Lock(0, 0, LockFlags.DoNotWait).WriteRange(VerticesFinal[i]);
                Vertices[i].Unlock();
            }

            // Lumière ambiante
            effect.SetValue("EmissiveColor", new Vector4(0f, 0f, 0f, 1f));
            effect.SetValue("AmbientLightColor", new Vector4(0f, 0f, 0f, 1f));

            // Light1
            effect.SetValue("LightPosition0", new Vector4(position_Light, 1));
            effect.SetValue("LightDiffuseColor0", new Vector4(0.1f, 0.1f, 0f, 1));
            effect.SetValue("LightSpecularColor0", new Vector4(0.1f, 0.1f, 0.1f, 1));
            effect.SetValue("LightDistanceSquared0", 15f);
            effect.SetValue("DiffuseColor0", new Vector4(0.5f, 0.5f, 0.5f, 1));
            effect.SetValue("SpecularColor0", new Vector4(0.5f, 0.5f, 0.5f, 1f));
            effect.SetValue("SpecularPower0", 1);

            // Light2
            effect.SetValue("LightPosition1", new Vector4(0, 2, 2, 1));
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
            effect.SetValue("SpecularPower2", 1);

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

            List<BoundingBox> ListeBoundingBoxes = Collision.Initialize(VerticesFinal);

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
                    macamera.orient_camera(Program.input, clock.ElapsedTicks - previous_time);


                    if (Following_light)
                        position_Light = -macamera.position;

                    previous_time = clock.ElapsedTicks;

                    //worldViewProj = macamera.view * proj;
                    //effect.SetValue("worldViewProj", worldViewProj);

                    bool collide = Collision.CheckCollisions(macamera.position);

                    if (collide)
                        macamera.position = oldPos;
                    else
                        oldPos = macamera.position;

                    effect.SetValue("CameraPos", new Vector4(macamera.position, 1));
                    effect.SetValue("LightPosition0", new Vector4(position_Light, 1));

                    if (collide)
                    {
                        oldView *=
                            Matrix.RotationAxis(new Vector3(0, 1, 0), macamera.angle.Y - oldAngle.Y) *
                            Matrix.RotationAxis(new Vector3(1, 0, 0), macamera.angle.X - oldAngle.X) *
                            Matrix.RotationAxis(new Vector3(0, 0, 1), macamera.angle.Z - oldAngle.Z);
                        macamera.view = oldView;
                    }
                    else
                        oldView = macamera.view;

                    effect.SetValue("View", macamera.view);
                    effect.SetValue("Sepia", false);

                    for (int i = 0; i < VerticesCount; i++)
                    {
                        Program.device.SetStreamSource(0, Vertices[i], 0, Utilities.SizeOf<Vertex>());
                        Program.device.SetTexture(0, Texture_ressource[i]);
                        Program.device.DrawPrimitives(PrimType, 0, SizeModels[i]);
                    }

                    effect.EndPass();
                    effect.End();
                }
                Program.device.EndScene();
                Program.device.Present();
            });

            for (int i = 0; i < nbmodels; i++)
            {
                Texture_ressource[i].Dispose();
            }
            clock.Stop();
            effect.Dispose();
        }
    }
}
