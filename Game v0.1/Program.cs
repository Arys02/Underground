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
using SharpExamples;
using Underground;
using Color = SharpDX.Color;
using Device = SharpDX.Direct3D9.Device;
using DeviceType = SharpDX.Direct3D9.DeviceType;
using Effect = SharpDX.Direct3D9.Effect;

namespace MiniCube
{
    internal static class Program
    {
        //[STAThread]


        private static void Main()
        {
            int[] position = new int[3];
            position[0] = 0;
            position[1] = 0;
            position[2] = 0;
            //string path = "untitled.obj";
            string path = @"C:\Users\Arys\Documents\informatique\Projet\test\underground\Underground\Underground\FilmNoirTriangl.obj";
            //string path = "Tuture.obj";
            //string path = "ressource.obj";
            //string path = "FilmNoirTriangl.obj";


            RenderForm form = new RenderForm("Game - Soutenance 1");
            Direct3D direct3D = new Direct3D();
            PresentParameters parametres;
            parametres.BackBufferWidth = form.ClientSize.Width;
            parametres.BackBufferHeight = form.ClientSize.Height;
            parametres.Windowed = true;
            parametres.SwapEffect = SwapEffect.FlipEx;
            Device device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(form.ClientSize.Width, form.ClientSize.Height));


            int nbfaces = ObjLoader.compte_faces(path);


            // Creates the VertexBuffer
            VertexBuffer vertices = new VertexBuffer(device, (Utilities.SizeOf<Vector4>() + Utilities.SizeOf<Vector4>()) * 3 * nbfaces, Usage.WriteOnly, VertexFormat.None, Pool.Managed);

            
            vertices.Lock(0, 0, LockFlags.None).WriteRange(ObjLoader.read_obj(path, nbfaces,0,0,0));
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
            Matrix view = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
            Matrix proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, form.ClientSize.Width / (float)form.ClientSize.Height, 0.1f, 100.0f);
            Matrix viewProj = Matrix.Multiply(view, proj);

            // Use clock
            Stopwatch clock = new Stopwatch();
            clock.Start();

            //Input
            Input input = new Input(form);

            Matrix worldViewProj = viewProj;
            var time = 0.01f;
            var time2 = 0.01f;
            //var Camera = new Camera(ref time, ref time2, worldViewProj, viewProj, form);
            RenderLoop.Run(form, () =>
                {
                   
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                device.BeginScene();

                effect.Technique = technique;
                effect.Begin();
                effect.BeginPass(0);

                    // essai d'implementation de la camera de maniere moduler //
                //Camera (ref time, ref time2, worldViewProj, viewProj, form);

                

                /////////camera/////////////
                
                if (input.KeysDown.Contains(Keys.Up) || input.KeysDown.Contains(Keys.W))      
                {
                    Console.WriteLine("up");
                    worldViewProj = Matrix.Translation(0, 0, time2) * viewProj;
                    time2 += -0.1f;
                }
                if (input.KeysDown.Contains(Keys.Down) || input.KeysDown.Contains(Keys.W))
                {
                    Console.WriteLine("Down");
                    worldViewProj = Matrix.Translation(0, 0, time2) * viewProj;
                    time2 += 0.1f;
                }

                if (input.KeysDown.Contains(Keys.Right) || input.KeysDown.Contains(Keys.W))
                {
                    Console.WriteLine("Right");
                    worldViewProj = Matrix.RotationY(time) * viewProj;
                    time += -0.01f;
                }
                if (input.KeysDown.Contains(Keys.Left) || input.KeysDown.Contains(Keys.W))
                {
                    Console.WriteLine("Left");
                    worldViewProj = Matrix.RotationY(time)*viewProj;
                    time += 0.01f;
                }
               
                 //fin camera//  


                
                
                effect.SetValue("worldViewProj", worldViewProj);
                
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, nbfaces);

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