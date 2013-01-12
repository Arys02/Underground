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
    struct Vertex
    {
        public Vector4 Position;
        //public Color Color;
        public Vector2 CoordTextures;
        public Vector4 Color;
    }
    internal static class Program
    {
        [STAThread]

        public static void recup_env(ref List<Vertex[]> ListeVerticesFinal,ref List<int> ModelSizes, ref List<Byte[]> ModelFiles, int nbmodels)
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
                ListeVerticesFinal.Add(ObjLoader.read_obj(ModelFiles[i], new Vector4(i * 0, ((float)i) / 10, i * 3, 0), ref nbmodelfaces, ref nbsommets, ref nbnormales, ref nbtextures));
                ModelSizes.Add(nbmodelfaces);
            }
        }

        private static void Main()
        {
            Vector3 position = new Vector3(0, -3, 0);
            Vector3 angle = new Vector3(-0.30f, -0.60f, 0.14f);
            bool environnement_altere = true;
            bool camera_altere = true;

            RenderForm form = new RenderForm("Game - Soutenance 1");
            Direct3D direct3D = new Direct3D();
            PresentParameters parametres = new PresentParameters(form.ClientSize.Width, form.ClientSize.Height);
            parametres.BackBufferWidth = form.ClientSize.Width;
            parametres.BackBufferHeight = form.ClientSize.Height;
            parametres.Windowed = true;
            parametres.SwapEffect = SwapEffect.FlipEx;
            Device device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(1200, 1200));

            
            // Compiles the effect
            Effect effect = Effect.FromFile(device, "MiniCube.fx", ShaderFlags.None);


            /******************************** /!\ /!\ ********************************/
            // Allocate Vertex Elements
            VertexElement[] vertexElems = new[] {
        		new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
        		
				new VertexElement(0,
                    Convert.ToInt16(Utilities.SizeOf<Vector4>()),
                    DeclarationType.Float2, DeclarationMethod.Default,DeclarationUsage.TextureCoordinate,0),

                new VertexElement(0,
                    Convert.ToInt16(Utilities.SizeOf<Vector4>()+Utilities.SizeOf<Vector2>()),
                    DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),

                VertexElement.VertexDeclarationEnd
        	};
            /******************************** /!\ /!\ ********************************/
            // Creates and sets the Vertex Declaration
            VertexDeclaration vertexDecl = new VertexDeclaration(device, vertexElems);
            device.VertexDeclaration = vertexDecl;
            //device.SetRenderState(RenderState.Lighting, false);


            // Get the technique
            EffectHandle technique = effect.GetTechnique(0);
            EffectHandle pass = effect.GetPass(technique, 0);

            // Prepare matrices
            Matrix view = Matrix.LookAtLH(new Vector3(0, 0, -0.00002f), new Vector3(0, 0, 0), Vector3.UnitY);
            Matrix proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, form.ClientSize.Width / (float)form.ClientSize.Height, 0.1f, 500.0f);
            Matrix viewProj = Matrix.Multiply(view, proj);

            // Use clock
            Stopwatch clock = new Stopwatch();
            clock.Start();

            Matrix worldViewProj = viewProj;
            Input input = new Input(form);

            List<Byte[]> ModelFiles = new List<Byte[]>();

            int nbmodels = 2;
            //string path = "couloirtest.obj";
            //Byte[] fichier = File.ReadAllBytes(path);
            Texture[] Texture_ressource = new Texture[nbmodels];
            for (int i = 0; i < nbmodels; i++)
            {
                ModelFiles.Add(Resource1.Lighthouse1);
                if (i == 0)
                Texture_ressource[i] = Texture.FromFile(device, "dev.png");
                else
                    Texture_ressource[i] = Texture.FromFile(device, "woodfloor.jpg");
            }

            List<Vertex[]> ListeVerticesFinal = new List<Vertex[]>();
            List<int> SizeModels = new List<int>();
            ListeVerticesFinal.Clear();
            SizeModels.Clear();
            recup_env(ref ListeVerticesFinal, ref SizeModels, ref ModelFiles, nbmodels);
            Vertex[][] VerticesFinal = new Vertex[nbmodels][];
            for (int i = 0; i < nbmodels; i++)
            {
                VerticesFinal[i] = ListeVerticesFinal[i];
            }

            int VerticesCount = 0;
            RenderLoop.Run(form, () =>
            {
                //DrawingPoint Center; = form.ClientSize.Height
                //device.SetCursorPosition(form.ClientSize.Width / 2, form.ClientSize.Height / 2);
                //device.SetRenderState(RenderState.CullMode, Cull.Clockwise);
                if (environnement_altere)
                {
                    recup_env(ref ListeVerticesFinal, ref SizeModels, ref ModelFiles, nbmodels);
                    environnement_altere = false;
                    VerticesCount = ListeVerticesFinal.Count;
                    for (int i = 0; i < nbmodels; i++)
                    {
                        VerticesFinal[i] = ListeVerticesFinal[i];
                    }
                }


                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                for (int i = 0; i < VerticesCount; i++)
                {
                    device.BeginScene();
                    effect.Technique = technique;
                    effect.Begin();
                    effect.BeginPass(0);
                    Light abc = new Light();
                    abc.Ambient.Alpha = 0;
                    abc.Ambient.Blue = 0;
                    abc.Ambient.Green = 0.5f;
                    abc.Ambient.Red = 0;
                    abc.Range = 500;
                    device.SetLight(0, ref abc);
                    device.EnableLight(0, true);


                    effect.SetValue("worldViewProj", worldViewProj);
                    VertexBuffer vertices = new VertexBuffer(device,
                            (
                                Utilities.SizeOf<Vector4>()
                                + Utilities.SizeOf<Vector2>()
                                + Utilities.SizeOf<Vector4>()
                            ) * 3 * SizeModels[i], Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                    device.SetStreamSource(0, vertices, 0, Utilities.SizeOf<Vertex>());
                    device.SetTexture(0, Texture_ressource[i]);
                    vertices.Lock(0, 0, LockFlags.None).WriteRange(VerticesFinal[i]);
                    vertices.Unlock();
                    Camera.orient_camera(ref input, ref position, ref angle, ref camera_altere);
                    //var time = clock.ElapsedMilliseconds / 2000f;
                    if (camera_altere)
                    {
                        worldViewProj =
                            Matrix.Translation(position.X, position.Y, position.Z) *
                            Matrix.RotationYawPitchRoll(angle.Y, angle.X, angle.Z) *
                            viewProj;

                        camera_altere = false;
                    }
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, SizeModels[i]);
                    vertices.Dispose();
                    effect.EndPass();
                    effect.End();

                    device.EndScene();
                    device.Present();
                }
            });

            for (int i = 0; i < nbmodels; i++)
            {
                Texture_ressource[i].Dispose();
            }
            effect.Dispose();
            device.Dispose();
            direct3D.Dispose();
        }
    }
}
