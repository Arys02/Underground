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
using IngameClass = LOL_l.Ingame.Ingame;


namespace LOL_l
{
    
    class Program
    {
        static void Main(string[] args)
        {
            RenderForm form = new RenderForm("Underground - POO version");
            form.Size = new Size(1280, 700);

            Direct3D direct3D = new Direct3D();
            PresentParameters parameters = new PresentParameters(form.ClientSize.Width, form.ClientSize.Height);
            Device device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, parameters);
            ResManager.Initialize(ref device, ref form);
            IngameClass ingame = new IngameClass(ref device, ref form);

            Stopwatch clock = new Stopwatch();
            clock.Start();

            RenderLoop.Run(form, () =>
            {
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                device.BeginScene();

                ingame.Draw(ref device, ref form, ref clock);

                device.EndScene();
                device.Present();
            });

            ResManager.Dispose();
            device.Dispose();
            direct3D.Dispose();
        }
    }
}
