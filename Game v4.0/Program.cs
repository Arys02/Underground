using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.XAudio2;
using SharpDX.Windows;
using Color = SharpDX.Color;


namespace LOL_l
{
    class Program
    {
        static void Main(string[] args)
        {
            Importer.OBJLoader Statue = new Importer.OBJLoader("statue.obj", Matrix.Identity);
            RenderForm form = new RenderForm("Underground - POO version");
            form.Size = new Size(1280, 700);
            Matrix proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, form.ClientSize.Width / (float)form.ClientSize.Height, 0.1f, 100.0f);
            Player player1 = new Player(ref form);

            Light[] Lights = new Light[2];
            Lights[0].Type = LightType.Point;
            Lights[0].Position = new Vector3(0, 0, 0);
            Lights[0].Ambient = new Color4(0.5f, 0.5f, 0.5f, 1);
            Lights[0].Range = 30f * 70;
            Lights[1].Type = LightType.Point;
            Lights[1].Position = new Vector3(-100, -100, -100);
            Lights[1].Ambient = new Color4(0.5f, 0, 0, 1);
            Lights[1].Range = 5f * 70;

            Direct3D direct3D = new Direct3D();
            Device device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(form.ClientSize.Width, form.ClientSize.Height));
            ResManager.Initialize(ref device, ref form, ref Lights);
            
            Stopwatch clock = new Stopwatch();
            clock.Start();

            Statue.Prepare(ref device);
            float luminosity = 1;
            float percent_negatif = 0;
            long previous_time = clock.ElapsedMilliseconds;

            RenderLoop.Run(form, () =>
            {
                float time = clock.ElapsedMilliseconds / 1000.0f;
                player1.orient_camera(clock.ElapsedMilliseconds - previous_time, ref form);
                previous_time = clock.ElapsedMilliseconds;
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                device.BeginScene();
                //player1.view = Matrix.RotationY(time * 2) * Matrix.Translation(new Vector3(0, -1, 1));
                //worldViewProj *= Matrix.Translation(new Vector3(0, -1, 0)) * viewProj;

                Statue.Draw(ref device, player1.view, proj, ref Lights, luminosity, percent_negatif);
                
                device.EndScene();
                device.Present();
            });

            ResManager.Dispose();
            device.Dispose();
            direct3D.Dispose();
        }
    }
}
