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
    struct Vertex
    {
        public Vector4 Position;
        //public Color Color;
        public Vector2 CoordTextures;
        public Vector4 Color;
        //public Vector4 Normal;
    }
    internal static class Program
    {
        const int nbmodels = 2;
        [STAThread]

        public static void recup_env(ref Device device,ref List<VertexBuffer> vertices ,ref List<Vertex[]> ListeVerticesFinal,ref List<int> ModelSizes, ref List<Byte[]> ModelFiles, int nbmodels)
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
                            ) * 3 * nbmodelfaces, Usage.WriteOnly, VertexFormat.None, Pool.Managed)
                );
            }
        }

        private static void Main()
        {
            #region Variables, Fenêtre
            int[] resolution = new int[2];
            resolution[0] = 1280; resolution[1] = 1024;
            List<Byte[]> ModelFiles = new List<Byte[]>();
            List<Vertex[]> ListeVerticesFinal = new List<Vertex[]>();
            List<VertexBuffer> Vertices = new List<VertexBuffer>();
            List<int> SizeModels = new List<int>();
            Vertex[][] VerticesFinal = new Vertex[nbmodels][];
            Texture[] Texture_ressource = new Texture[nbmodels];
            Direct3D direct3D = new Direct3D();
            Vector3 position = new Vector3(0, -10, 20), angle = new Vector3(0f, 0f, 0f);
            RenderForm form = new RenderForm("Game - Soutenance 1");
            form.Width = resolution[0];
            form.Height = resolution[1];
            PresentParameters Parametres = new PresentParameters(
                form.Width,
                form.Height,
                Format.X8R8G8B8,
                1,
                MultisampleType.None,
                0,
                SwapEffect.Discard,
                IntPtr.Zero,
                true,
                true,
                Format.D24X8,
                PresentFlags.None,
                0,
                PresentInterval.Immediate);
            VertexElement[] vertexElems = new[] {
        		new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0,
                    Convert.ToInt16(Utilities.SizeOf<Vector4>()),
                    DeclarationType.Float2, DeclarationMethod.Default,DeclarationUsage.TextureCoordinate,0),
                new VertexElement(0,
                    Convert.ToInt16(Utilities.SizeOf<Vector4>()+Utilities.SizeOf<Vector2>()),
                    DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                /*new VertexElement(0,
                    Convert.ToInt16(Utilities.SizeOf<Vector4>()+Utilities.SizeOf<Vector4>()+Utilities.SizeOf<Vector2>()),
                    DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Normal, 0),*/
                VertexElement.VertexDeclarationEnd
        	};
            MouseHandler mouseHandler = new Underground.MouseHandler();
            Input input = new Input(form);
            Device device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, Parametres);
            VertexDeclaration vertexDecl = new VertexDeclaration(device, vertexElems);
            device.VertexDeclaration = vertexDecl;
            Effect effect = Effect.FromFile(device, "MiniCube.fx", ShaderFlags.None);
            EffectHandle technique = effect.GetTechnique(0);
            EffectHandle pass = effect.GetPass(technique, 0);
            Matrix view = Matrix.LookAtLH(new Vector3(0, 0, -0.00002f), new Vector3(0, 0, 0), Vector3.UnitY);
            Matrix proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, form.ClientSize.Width / (float)form.ClientSize.Height, 0.1f, 9999.0f);
            Matrix viewProj = Matrix.Multiply(view, proj);
            Matrix worldViewProj = viewProj;
            //CameraFirstPerson FreeLook = new CameraFirstPerson(input, 0, 0, Vector3.Zero, form.Width, form.Height);
            int VerticesCount = 0;
            //int menu_running = 1;
            // 0 -> jeu en cours
            // 1 -> menu de demarrage
            // 2 -> menu de jeu
            #endregion
            #region Souris
            form.MouseDown += mouseHandler.MouseDown;
            form.MouseUp += mouseHandler.MouseUp;
            Size orgSize = form.Size;
            Point orgLocation = form.Location;
            float xRes = 2.0f / (form.Width - 5);
            float yRes = 2.0f / (form.Height - 5);
            form.KeyDown += (sender, arg) =>
            {
                switch (arg.KeyCode)
                {
                    case Keys.Escape:   //Exit
                        ((RenderForm)sender).Close();
                        break;
                    case Keys.Space:    //Orginal size
                        ((RenderForm)sender).Size = orgSize;
                        ((RenderForm)sender).Location = orgLocation;
                        break;
                }
            };
            #endregion
            
            // Use clock
            Stopwatch clock = new Stopwatch();
            clock.Start();


            string path = @"Ressources\Game\Firstlevel.obj";
            //string path = @"E:\TEST SOUTENANCE\Couloir_Maya2.obj";
            //string path = @"C:\Users\b95093cf\Desktop\untitled.obj";
            //string path = @"Ressources\Game\Lighthouse.obj";
            Byte[] fichier = File.ReadAllBytes(path);
            for (int i = 0; i < nbmodels; i++)
            {
                ModelFiles.Add(fichier);
                //ModelFiles.Add(fichier);
                if (i == 0)
                    //Texture_ressource[i] = Texture.FromFile(device, @"Ressources\Game\Images\dev.png");
                    Texture_ressource[i] = Texture.FromFile(device, @"Ressources\Game\Images\Beton20.jpg");
                else if (i == 1)
                {
                    Texture_ressource[i] = Texture.FromFile(device, @"Ressources\Game\Images\porte-beton-texture-en-beton_19-136906.jpg");
                }
                else if (i == 2)
                {
                    Texture_ressource[i] = Texture.FromFile(device, @"Ressources\Game\Images\dev2.png");
                }
                else if (i == 3)
                {
                    Texture_ressource[i] = Texture.FromFile(device, @"Ressources\Game\Images\woodfloor.bmp");
                }
                else
                {
                    Texture_ressource[i] = Texture.FromFile(device, @"Ressources\Game\Images\Beton20.jpg");
                }
                    //Texture_ressource[i] = Texture.FromFile(device, @"Ressources\Game\Images\dev.png");
                    
                    //Texture_ressource[i] = Texture.FromFile(device, @"Ressources\Game\Images\porte-beton-texture-en-beton_19-136906.png");
            }

            Camera macamera = new Camera();

            recup_env(ref device, ref Vertices, ref ListeVerticesFinal, ref SizeModels, ref ModelFiles, nbmodels);
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

      
            

            #region Menu, fonts
            FontDescription fontDescription = new FontDescription()
            {
                Height = 45,
                Italic = false,
                CharacterSet = FontCharacterSet.Ansi,
                FaceName = "Arial",
                MipLevels = 0,
                OutputPrecision = FontPrecision.TrueType,
                PitchAndFamily = FontPitchAndFamily.Default,
                Quality = FontQuality.ClearType,
                Weight = FontWeight.Bold
            };
            SharpDX.Direct3D9.Font font = new SharpDX.Direct3D9.Font(device, fontDescription);
            string displayText = "PLAY !";
            SharpDX.Rectangle fontDimension = font.MeasureText(null, displayText, new SharpDX.Rectangle(0, 0, form.ClientSize.Width, form.ClientSize.Height), SharpDX.Direct3D9.FontDrawFlags.Center | SharpDX.Direct3D9.FontDrawFlags.VerticalCenter);
            SharpDX.Rectangle fontPauseDimension = font.MeasureText(null, "PAUSE", new SharpDX.Rectangle(0, 0, form.ClientSize.Width, form.ClientSize.Height), SharpDX.Direct3D9.FontDrawFlags.Top | SharpDX.Direct3D9.FontDrawFlags.Right);

            // Image

            int nbvertex = 5;
            VertexBuffer vertices_img = new VertexBuffer(device, Utilities.SizeOf<Vertex>() * nbvertex, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            Console.WriteLine(Utilities.SizeOf<Vertex>());
            // ******* ICI ****** /
            Texture Texture_background_menu = SharpDX.Direct3D9.Texture.FromFile(device, @"Ressources\Menu\bg.jpg");
            
            #endregion
            RenderLoop.Run(form, () =>
            {
                //DrawingPoint Center; = form.ClientSize.Height
                //device.SetCursorPosition(form.ClientSize.Width / 2, form.ClientSize.Height / 2);

                //device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new ColorBGRA(0, 0, 0, 1f), 1.0f, 0);
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                device.BeginScene();
                effect.Technique = technique;
                effect.Begin();
                effect.BeginPass(0);
                effect.SetValue("AmbientColor", new Vector4(0.5f, 0.5f, 0.5f, 1));
                device.SetCursorPosition(new DrawingPoint(1, 1), true);


                macamera.orient_camera(input);

                for (int i = 0; i < VerticesCount; i++)
                {
                    device.SetStreamSource(0, Vertices[i], 0, Utilities.SizeOf<Vertex>());
                    device.SetTexture(0, Texture_ressource[i]);
                    //var time = clock.ElapsedMilliseconds / 2000f;
                    worldViewProj = macamera.view * proj;
                    device.SetRenderState(RenderState.CullMode, true);
                    //device.SetRenderState(RenderState.FillMode, FillMode.Point);
                    //device.SetRenderState(RenderState.FillMode, true);
                    //device.DrawPrimitives(PrimitiveType.TriangleList, 300000, SizeModels[i]-300000);
                    //device.DrawPrimitives(PrimitiveType.TriangleList, 0, 300000);
                    effect.SetValue("worldViewProj", worldViewProj);
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, SizeModels[i]);
                    //Vertices[i].Dispose();
                }
                effect.EndPass();
                effect.End();

                device.EndScene();
                device.Present();
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
