using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.XAudio2;
using SharpDX.Windows;


namespace LOL_l
{
    class Program
    {
        static void Main(string[] args)
        {
            Importer.OBJLoader Statue = new Importer.OBJLoader("statue.obj", Matrix.Identity);
            RenderForm form = new RenderForm("Underground - POO version");
            Matrix view = Matrix.LookAtLH(new Vector3(0, 0, -3), new Vector3(0, 0, 0), Vector3.UnitY);
            Matrix proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, form.ClientSize.Width / (float)form.ClientSize.Height, 0.1f, 100.0f);

            // Creates the Device
            Direct3D direct3D = new Direct3D();
            Device device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(form.ClientSize.Width, form.ClientSize.Height));
            ResManager.Initialize(ref device);

            //Statue.Prepare(ref device);

            // Use clock
            var clock = new Stopwatch();
            clock.Start();

            Statue.Prepare(ref device);
            
            RenderLoop.Run(form, () =>
            {

                var time = clock.ElapsedMilliseconds / 1000.0f;

                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                device.BeginScene();
                view = Matrix.RotationY(time * 2) * Matrix.Translation(new Vector3(0, -1, 5));
                //worldViewProj *= Matrix.Translation(new Vector3(0, -1, 0)) * viewProj;

                Statue.Draw(ref device, view, proj);
                
                device.EndScene();
                device.Present();
            });

            ResManager.Dispose();
            device.Dispose();
            direct3D.Dispose();
        }
    }
}
