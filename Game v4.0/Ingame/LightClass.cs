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

namespace LOL_l.Ingame
{
    public class LightClass
    {
        public float Range;
        public LightType Type;
        public Vector3 Position;
        public Color Ambiant;
        public Color Diffuse;
        public Color Specular;
        
        public LightClass(LightType Type, Vector3 Position, Color Ambiant, Color Diffuse, Color Specular, float Range)
        {
            this.Type = Type;
            this.Position = Position;
            this.Ambiant = Ambiant;
            this.Diffuse = Diffuse;
            this.Specular = Specular;
            this.Range = Range;
        }
    }
}
