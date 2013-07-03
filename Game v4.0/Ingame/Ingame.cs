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
using OBJLoader = LOL_l.Importer.OBJLoader;

namespace LOL_l.Ingame
{
    class Ingame
    {
        private Player player1;
        private List<OBJLoader> ListeOBJ;
        private List<LightClass> Lights;
        private float luminosity = 1;
        private float percent_negatif = 0;
        private long previous_time;
        private bool first_loop = true;
        public Ingame(ref Device device, ref RenderForm form)
        {
            Matrix proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, form.ClientSize.Width / (float)form.ClientSize.Height, 0.1f, 100.0f);

            this.player1 = new Player(ref form, proj);

            ListeOBJ = new List<OBJLoader>();
            ListeOBJ.Add(new OBJLoader("statue.obj", Matrix.Identity));

            Lights = new List<LightClass>();
            Lights.Add(new LightClass(LightType.Point, player1.position, Color.White, Color.White, Color.White, 2100));
            Lights.Add(new LightClass(LightType.Point, player1.position, Color.Red, Color.Red, Color.Red, 0));

            foreach (OBJLoader obj in ListeOBJ)
            {
                obj.Prepare(ref device);
            }
        }
        public void Draw(ref Device device, ref RenderForm form, ref Stopwatch clock)
        {
            if (first_loop)
            {
                previous_time = clock.ElapsedMilliseconds;
                first_loop = false;
            }
            Lights[0].Position = -player1.position;
            long timer = clock.ElapsedMilliseconds - previous_time;
            previous_time = clock.ElapsedMilliseconds;
            //player1.orient_camera(timer, ref form);
            foreach (OBJLoader obj in ListeOBJ)
            {
                obj.Draw(ref device, player1.view, player1.proj, ref Lights, luminosity, percent_negatif);
            }
        }
    }
}
