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
        //[STAThread]

        private static void Main()
        {
            Vector3 position = new Vector3(0, 0, 0);
            Vector3 angle = new Vector3(0, 0, 0);
            //string path = "untitled.obj";
            //string path = "FilmNoirTriangl.obj";
            //string path = "cube.obj";
            string path = "Tuture2.obj";
            //string path = "ressource.obj";
            //string path = "FilmNoirTriangl.obj";
            //string path = "couloir2.obj";

            
            RenderForm form = new RenderForm("Game - Soutenance 1");
            Direct3D direct3D = new Direct3D();
            PresentParameters parametres = new PresentParameters(form.ClientSize.Width,form.ClientSize.Height);
            parametres.BackBufferWidth = form.ClientSize.Width;
            parametres.BackBufferHeight = form.ClientSize.Height;
            parametres.Windowed = true;
            parametres.SwapEffect = SwapEffect.FlipEx;
            Device device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(form.ClientSize.Width, form.ClientSize.Height));
            //device.SetRenderState(RenderState.CullMode, Cull.Clockwise);

            int nbfaces = 0;
            int nbsommets = 0;
            int nbnormales = 0;
            int nbtextures = 0;
            Texture Texture_ressource = Texture.FromFile(device, "woodfloor.bmp");
            Vertex[] ModelVertices = ObjLoader.read_obj(path, new Vector4(0, 0, 0, 0), ref nbfaces, ref nbsommets, ref nbnormales, ref nbtextures);


            // Creates the VertexBuffer
            VertexBuffer vertices = new VertexBuffer(device,
                (
                    Utilities.SizeOf<Vector4>()
                    + Utilities.SizeOf<Vector2>()
                    + Utilities.SizeOf<Vector4>()
                ) * 3 * nbfaces, Usage.WriteOnly, VertexFormat.None, Pool.Managed);

            
            vertices.Lock(0, 0, LockFlags.None).WriteRange(ModelVertices);
            vertices.Unlock();

            // Compiles the effect
            Effect effect = Effect.FromFile(device, "MiniCube.fx", ShaderFlags.None);


            /******************************** /!\ /!\ ********************************/
            // Allocate Vertex Elements
            VertexElement[] vertexElems = new[] {
        		new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
        		
                /*new VertexElement(0,
                    Convert.ToInt16(Utilities.SizeOf<Vector4>()),
                    DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),*/

				new VertexElement(0,
                    Convert.ToInt16(Utilities.SizeOf<Vector4>()),//+Utilities.SizeOf<Color>()),
                    DeclarationType.Float2, DeclarationMethod.Default,DeclarationUsage.TextureCoordinate,0),

                new VertexElement(0,
                    Convert.ToInt16(Utilities.SizeOf<Vector4>()+Utilities.SizeOf<Vector2>()),
                    DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),

                VertexElement.VertexDeclarationEnd
        	};
            /******************************** /!\ /!\ ********************************/

            // Creates and sets the Vertex Declaration
            VertexDeclaration vertexDecl = new VertexDeclaration(device, vertexElems);
            device.SetStreamSource(0, vertices, 0, Utilities.SizeOf<Vertex>());
            device.SetTexture(0, Texture_ressource);
            device.VertexDeclaration = vertexDecl;
            device.SetRenderState(RenderState.Lighting, false);


            // Get the technique
            EffectHandle technique = effect.GetTechnique(0);
            EffectHandle pass = effect.GetPass(technique, 0);

            // Prepare matrices
            Matrix view = Matrix.LookAtLH(new Vector3(0, 0, -1), new Vector3(0, 0, 0), Vector3.UnitY);
            Matrix proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, form.ClientSize.Width / (float)form.ClientSize.Height, 0.1f, 500.0f);
            Matrix viewProj = Matrix.Multiply(view, proj);

            // Use clock
            Stopwatch clock = new Stopwatch();
            clock.Start();

            Matrix worldViewProj = viewProj;

            Input input = new Input(form);
            
            RenderLoop.Run(form, () =>
            {
                //var time = clock.ElapsedMilliseconds / 1000.0f;

                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Yellow, 1.0f, 0);
                device.BeginScene();

                effect.Technique = technique;
                effect.Begin();
                effect.BeginPass(0);
                
                
                /*//Matrix worldViewProj = Matrix.Translation(new Vector3(10+time, -20, 0)) * Matrix.RotationX(-(float)Math.PI / 6)  * viewProj;
                //Matrix worldViewProj = Matrix.Translation(new Vector3(position[0],position[1],position[2])) * Matrix.RotationX(time) * Matrix.RotationY(time * 2) * Matrix.RotationZ(time * .7f) * viewProj;
                Matrix worldViewProj = Matrix.Translation(new Vector3(-50, 0, 0)) * Matrix.RotationX(time) * Matrix.RotationY(time) * viewProj;
                */

                Camera.orient_camera(ref input, ref position, ref angle);

                worldViewProj =
                    Matrix.Translation(position.X, position.Y, position.Z) *
                    Matrix.RotationX(angle.X) *
                    Matrix.RotationY(angle.Y) *
                    Matrix.RotationZ(angle.Z) *
                    viewProj;
                
                effect.SetValue("worldViewProj", worldViewProj);
                device.SetTexture(0, Texture_ressource);
                
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, nbfaces);

                effect.EndPass();
                effect.End();

                device.EndScene();
                device.Present();
            });

            Texture_ressource.Dispose();
            effect.Dispose();
            vertices.Dispose();
            device.Dispose();
            direct3D.Dispose();
             
        }
    }
}

