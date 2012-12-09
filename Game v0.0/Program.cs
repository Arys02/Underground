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
        public Vector4 Color;
        //public Vector4 Normale;
        //public Vector2 Texture_Pos;
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
            //string path = "Tuture.obj";
            //string path = "ressource.obj";
            //string path = "FilmNoirTriangl.obj";
            string path = "couloir2.obj";

            
            RenderForm form = new RenderForm("Game - Soutenance 1");
            Direct3D direct3D = new Direct3D();
            PresentParameters parametres;
            parametres.BackBufferWidth = form.ClientSize.Width;
            parametres.BackBufferHeight = form.ClientSize.Height;
            parametres.Windowed = true;
            parametres.SwapEffect = SwapEffect.FlipEx;
            Device device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(form.ClientSize.Width, form.ClientSize.Height));
            device.SetRenderState(RenderState.CullMode, Cull.Clockwise);

            Vector4[] ModelVertices = ObjLoader.read_obj(path, new Vector4(0,0,0,0));
            int nbsommets = ModelVertices.Length;


            // Creates the VertexBuffer
            VertexBuffer vertices = new VertexBuffer(device, (Utilities.SizeOf<Vector4>() + Utilities.SizeOf<Vector4>()) * nbsommets, Usage.WriteOnly, VertexFormat.None, Pool.Managed);

            
            vertices.Lock(0, 0, LockFlags.None).WriteRange(ModelVertices);
            vertices.Unlock();

            // Compiles the effect
            Effect effect = Effect.FromFile(device, "MiniCube.fx", ShaderFlags.None);

            // Allocate Vertex Elements
            VertexElement[] vertexElems = new[] {
        		new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
        		new VertexElement(0, Convert.ToInt16(Utilities.SizeOf<Vector4>()), DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd
        	};

            // Creates and sets the Vertex Declaration
            VertexDeclaration vertexDecl = new VertexDeclaration(device, vertexElems);
            device.SetStreamSource(0, vertices, 0, Utilities.SizeOf<Vector4>() + Utilities.SizeOf<Vector4>());
            device.VertexDeclaration = vertexDecl;

            // Get the technique
            EffectHandle technique = effect.GetTechnique(0);
            EffectHandle pass = effect.GetPass(technique, 0);

            // Prepare matrices
            Matrix view = Matrix.LookAtLH(new Vector3(0, 0, -1), new Vector3(0, 0, 0), Vector3.UnitY);
            Matrix proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, form.ClientSize.Width / (float)form.ClientSize.Height, 0.1f, 100.0f);
            Matrix viewProj = Matrix.Multiply(view, proj);

            // Use clock
            Stopwatch clock = new Stopwatch();
            clock.Start();

            // Load Texture
            Texture Texture_ressource = Texture.FromFile(device, "woodfloor.bmp");
            Matrix worldViewProj = viewProj;

            Input input = new Input(form);
            
            RenderLoop.Run(form, () =>
            {
                //var time = clock.ElapsedMilliseconds / 1000.0f;

                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                device.BeginScene();

                effect.Technique = technique;
                effect.Begin();
                effect.BeginPass(0);
                
                
                /*//Matrix worldViewProj = Matrix.Translation(new Vector3(10+time, -20, 0)) * Matrix.RotationX(-(float)Math.PI / 6)  * viewProj;
                //Matrix worldViewProj = Matrix.Translation(new Vector3(position[0],position[1],position[2])) * Matrix.RotationX(time) * Matrix.RotationY(time * 2) * Matrix.RotationZ(time * .7f) * viewProj;
                Matrix worldViewProj = Matrix.Translation(new Vector3(-50, 0, 0)) * Matrix.RotationX(time) * Matrix.RotationY(time) * viewProj;
                */
                if (input.KeysDown.Contains(Keys.W))
                {
                    position.Y -= 0.01f;
                    Console.WriteLine("Vers le haut !");
                }

                if (input.KeysDown.Contains(Keys.A))
                {
                    position.Y += 0.01f;
                    Console.WriteLine("Vers le bas !");
                }

                if (input.KeysDown.Contains(Keys.Up) || input.KeysDown.Contains(Keys.Z))
                {
                    position.Z -= 0.01f;
                    Console.WriteLine("En avant !");
                }
                if (input.KeysDown.Contains(Keys.Down) || input.KeysDown.Contains(Keys.S))
                {
                    position.Z += 0.01f;
                    Console.WriteLine("En arrière !");
                }

                if (input.KeysDown.Contains(Keys.Right))
                {
                    angle.Y -= 0.001f;
                    Console.WriteLine("A tribord !");
                }
                if (input.KeysDown.Contains(Keys.Left))
                {
                    angle.Y += 0.001f;
                    Console.WriteLine("A babord !");
                }
                if (input.KeysDown.Contains(Keys.Q))
                {
                    position.X += 0.01f;
                    Console.WriteLine("Left");
                }
                if (input.KeysDown.Contains(Keys.D))
                {
                    position.X -= 0.01f;
                    Console.WriteLine("Right");
                }

                worldViewProj =
                    Matrix.Translation(position.X, position.Y, position.Z) *
                    Matrix.RotationX(angle.X) *
                    Matrix.RotationY(angle.Y) *
                    Matrix.RotationZ(angle.Z) *
                    viewProj;
                
                effect.SetValue("worldViewProj", worldViewProj);
                device.SetTexture(0, Texture_ressource);
                
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, nbsommets/3);

                effect.EndPass();
                effect.End();

                device.EndScene();
                device.Present();
            });

            effect.Dispose();
            vertices.Dispose();
            device.Dispose();
            direct3D.Dispose();
             
        }
    }
}

            