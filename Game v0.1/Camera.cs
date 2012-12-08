using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpDX;
using SharpDX.DirectInput;
using SharpDX.Windows;
using SharpExamples;
using Color = SharpDX.Color;
using Device = SharpDX.Direct3D9.Device;
using DeviceType = SharpDX.Direct3D9.DeviceType;
using Effect = SharpDX.Direct3D9.Effect;


namespace Underground
{
    class Camera 
    {
       

        public Camera(ref float time, ref float time2, Matrix worldViewProj, Matrix viewProj, RenderForm form)
        {
            Input input = new Input(form);

            if (input.KeysDown.Contains(Keys.Up) || input.KeysDown.Contains(Keys.W))
            {
                Console.WriteLine("up");
                worldViewProj = Matrix.Translation(0, 0, time2)*viewProj;
                time2 += -0.1f;
            }
            if (input.KeysDown.Contains(Keys.Down) || input.KeysDown.Contains(Keys.W))
            {
                Console.WriteLine("Down");
                worldViewProj = Matrix.Translation(0, 0, time2)*viewProj;
                time2 += 0.1f;
            }

            if (input.KeysDown.Contains(Keys.Right) || input.KeysDown.Contains(Keys.W))
            {
                Console.WriteLine("Right");
                worldViewProj = Matrix.RotationY(time)*viewProj;
                time += -0.01f;
            }
            if (input.KeysDown.Contains(Keys.Left) || input.KeysDown.Contains(Keys.W))
            {
                Console.WriteLine("Left");
                worldViewProj = Matrix.RotationY(time)*viewProj;
                time += 0.01f;
            }
        }

    }
}
