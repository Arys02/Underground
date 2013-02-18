using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D9;
using SharpDX.DirectInput;
using SharpDX.Windows;
using Underground;
using Color = SharpDX.Color;
using Device = SharpDX.Direct3D9.Device;
using DeviceType = SharpDX.Direct3D9.DeviceType;
using Effect = SharpDX.Direct3D9.Effect;

namespace Underground
{
    static class Ingame
    {
        public static PrimitiveType PrimType = PrimitiveType.TriangleList;
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

        static public void ingame()
        {
            Input input = new Input(Program.form);
            const int nbmodels = 1;
            int VerticesCount = 0;
            List<Byte[]> ModelFiles = new List<Byte[]>();
            List<Vertex[]> ListeVerticesFinal = new List<Vertex[]>();
            List<VertexBuffer> Vertices = new List<VertexBuffer>();
            List<int> SizeModels = new List<int>();
            Vertex[][] VerticesFinal = new Vertex[nbmodels][];
            Texture[] Texture_ressource = new Texture[nbmodels];
            Vector3 position = new Vector3(0, -10, 20), angle = new Vector3(0f, 0f, 0f);
            Matrix view = Matrix.LookAtLH(new Vector3(0, 0, -0.00002f), new Vector3(0, 0, 0), Vector3.UnitY);
            Matrix proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, Program.form.ClientSize.Width / (float)Program.form.ClientSize.Height, 0.1f, 9999.0f);
            Matrix viewProj = Matrix.Multiply(view, proj);
            Matrix worldViewProj = viewProj;
            Effect effect = Effect.FromFile(Program.device, "MiniCube.fx", ShaderFlags.None);
            EffectHandle technique = effect.GetTechnique(0);
            EffectHandle pass = effect.GetPass(technique, 0);
            Stopwatch clock = new Stopwatch();
            clock.Start();

            //string path = @"Ressources\Game\Firstlevel.obj";
            //string path = @"E:\TEST SOUTENANCE\Couloir_Maya2.obj";
            //string path = @"C:\Users\b95093cf\Desktop\untitled.obj";
            //string path = @"Ressources\Game\Lighthouse.obj";
            //string path = @"Ressources\Game\boxman.obj";
            string path = @"Ressources\Game\ct0.obj";
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
                    Texture_ressource[i] = Texture.FromFile(Program.device, @"Ressources\Game\Images\porte-beton-texture-en-beton_19-136906.jpg");
                }
                else if (i == 2)
                {
                    Texture_ressource[i] = Texture.FromFile(Program.device, @"Ressources\Game\Images\dev2.png");
                }
                else if (i == 3)
                {
                    Texture_ressource[i] = Texture.FromFile(Program.device, @"Ressources\Game\Images\woodfloor.bmp");
                }
                else
                {
                    Texture_ressource[i] = Texture.FromFile(Program.device, @"Ressources\Game\Images\Beton20.jpg");
                }
                //Texture_ressource[i] = Texture.FromFile(device, @"Ressources\Game\Images\dev.png");

                //Texture_ressource[i] = Texture.FromFile(device, @"Ressources\Game\Images\porte-beton-texture-en-beton_19-136906.png");
            }

            Camera macamera = new Camera();

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
                //SizeModels[i] = 150;
            }

            effect.SetValue("AmbientColor", new Vector4(1f, 1f, 1f, 1f));
            effect.SetValue("DiffuseColor", new Vector4(1f, 1f, 1f, 1f));
            effect.SetValue("DiffuseLightDirection", new Vector4(1f, 1f, 3f, 1f));
            effect.SetValue("Lumiere", true);
            /*
            effect.SetValue("vecLightPos",position);
            effect.SetValue("LightRange",30.0f);
            effect.SetValue("LightColor",new Vector4(255,255,255,255));*/

            effect.Technique = technique;
            //Program.device.SetRenderState(RenderState.CullMode, true);

            Int64 previous_time = clock.ElapsedTicks;

            RenderLoop.Run(Program.form, () =>
            {
                Program.device.BeginScene();
                //DrawingPoint Center; = form.ClientSize.Height
                //device.SetCursorPosition(form.ClientSize.Width / 2, form.ClientSize.Height / 2);

                Program.device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                effect.Begin();
                effect.BeginPass(0);

                macamera.orient_camera(input, clock.ElapsedTicks - previous_time);
                previous_time = clock.ElapsedTicks;

                worldViewProj = macamera.view * proj;
                effect.SetValue("worldViewProj", worldViewProj);
                for (int i = 0; i < VerticesCount; i++)
                {
                    Program.device.SetStreamSource(0, Vertices[i], 0, Utilities.SizeOf<Vertex>());
                    Program.device.SetTexture(0, Texture_ressource[i]);
                    Program.device.DrawPrimitives(PrimType, 0, SizeModels[i]);
                }
                effect.EndPass();
                effect.End();
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
