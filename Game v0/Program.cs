using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using Color = SharpDX.Color;

namespace MiniCube
{
    struct Vertex
    {
        public Vector4 Position;
        //public SharpDX.Color Color;
        public Vector2 Texture_Pos;
    }
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            int nbvertex = 12500;
            RenderForm form = new RenderForm("Titre");
            Direct3D direct3D = new Direct3D();
            Device device = new Device(direct3D, 0, DeviceType.Reference, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(form.ClientSize.Width, form.ClientSize.Height));
            VertexBuffer vertices = new VertexBuffer(device, Utilities.SizeOf<Vertex>() * nbvertex, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            Console.WriteLine(Utilities.SizeOf<Vertex>());
            /******* ICI ******/
            //FileStream fichier = File.Open("earth.bmp", FileMode.Open);
            Texture Texture_ressource = SharpDX.Direct3D9.Texture.FromFile(device, "earth.bmp");
            /*device.SetSamplerState(0, SamplerState.MinFilter, 2);
            device.SetSamplerState(0, SamplerState.MagFilter, 2);
            device.SetRenderState(RenderState.Lighting, false);*/
            /******* END **********/
            
            

            


            RenderLoop.Run(form, () =>
            {
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Blue, 1.0f, 0);
                device.BeginScene();
                int w = 100, h = 10;
                for (int y = 0; y < 50; y++)
                {
                    for (int x = 0; x < 50; x++)
                    {
                        vertices.Lock(0, 0, LockFlags.None).WriteRange(new Vertex[] {
                new Vertex() { Position = new Vector4(x * w, y * h, 0f, 1.0f), Texture_Pos = new Vector2(0f, 0f) },
                new Vertex() { Position = new Vector4(x * w + w, y * h, 0f, 1.0f), Texture_Pos = new Vector2(1f, 0f) },
                new Vertex() { Position = new Vector4(x * w, y * h + h, 0f, 1.0f), Texture_Pos = new Vector2(0f, 1f) },
                new Vertex() { Position = new Vector4(x * w + w, y * h + h, 0f, 1.0f), Texture_Pos = new Vector2(1f, 1f) },
                new Vertex() { Position = new Vector4(x * w + w, y * h, 0f, 1.0f), Texture_Pos = new Vector2(1f, 0f) }});
                        vertices.Unlock();
                        VertexElement[] VertexElems = new[] {
                new VertexElement(0,0,DeclarationType.Float4,DeclarationMethod.Default,DeclarationUsage.PositionTransformed,0),
                new VertexElement(0,16,DeclarationType.Float2,DeclarationMethod.Default,DeclarationUsage.TextureCoordinate,0),
                VertexElement.VertexDeclarationEnd
            };

                        VertexDeclaration VertexDec3 = new VertexDeclaration(device, VertexElems);
                        device.SetStreamSource(0, vertices, 0, Utilities.SizeOf<Vertex>() * nbvertex);
                        device.VertexDeclaration = VertexDec3;
                        device.SetTexture(0, Texture_ressource);
                        device.SetStreamSource(0, vertices, 0, Utilities.SizeOf<Vertex>());
                        //231231Matrix.RotationX(x + y * 10)*;
                        device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                    }
                }
            
                device.EndScene();
                device.Present();
            });

            Texture_ressource.Dispose();
            vertices.Dispose();
            device.Dispose();
            direct3D.Dispose();
        }
    }
}
