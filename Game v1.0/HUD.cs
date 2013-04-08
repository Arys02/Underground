using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D9;
using SharpDX.DirectInput;
using SharpDX.Windows;

using Color = SharpDX.Color;
using Device = SharpDX.Direct3D9.Device;
using DeviceType = SharpDX.Direct3D9.DeviceType;
using Effect = SharpDX.Direct3D9.Effect;

namespace Underground
{
    enum EltType
    {
        Default = 0,
        EyeBar = 1,
    }
    struct EltInfo
    {
        public EltInfo(Vector2 Position, Vector2 Taille, string PathTexture, EltType EltType)
        {
            this.Position = Position;
            this.Taille = Taille;
            this.PathTexture = PathTexture;
            this.EltType = EltType;
        }
        public Vector2 Position;
        public Vector2 Taille;
        public string PathTexture;
        public EltType EltType;
    }
    class HUD
    {
        //private static Texture Texture_Progressbar_Eye;
        private List<EltInfo> Liste_Elt;
        public HUD()
        {
            Liste_Elt = new List<EltInfo>();
            Liste_Elt.Add(new EltInfo(Vector2.Zero, new Vector2(Program.resolution[0],Program.resolution[1]), @"Ressources\HUD\Torch.png", EltType.Default));
            Liste_Elt.Add(new EltInfo(new Vector2(0, Program.resolution[1] - 200), new Vector2(400, 200), @"Ressources\HUD\Texture.png", EltType.Default));
            Liste_Elt.Add(new EltInfo(new Vector2(42, Program.resolution[1] - 102), new Vector2(300, 30), @"Ressources\HUD\ProgressBar1.png", EltType.EyeBar));
        }

        public void Display_HUD()
        {
            VertexBuffer VertexBufferHUD;
            Program.device.VertexDeclaration = Program.VertexDeclaration2D;

            foreach (EltInfo Elt in Liste_Elt)
            {
                VertexBufferHUD = new VertexBuffer(Program.device,
                    (
                        Utilities.SizeOf<structVertex>()
                    ) * 3 * 2, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                structVertex[] Sommets;
                if (Elt.EltType == EltType.EyeBar) // Barre de progression des yeux
                {
                    Sommets = new structVertex[] {
                    new structVertex() { Position = new Vector4(Elt.Position.X                                , Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(0f, 0f) },
                    new structVertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X * Ingame.percent, Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(1f, 0f) },
                    new structVertex() { Position = new Vector4(Elt.Position.X                                , Elt.Position.Y+Elt.Taille.Y, 0f, 1.0f), CoordTextures = new Vector2(0f, 1f) },
                    new structVertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X * Ingame.percent, Elt.Position.Y+Elt.Taille.Y, 0f, 1.0f), CoordTextures = new Vector2(1f, 1f) },
                    new structVertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X * Ingame.percent, Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(1f, 0f) }};
                }
                else // Par défaut
                {
                    Sommets = new structVertex[] {
                    new structVertex() { Position = new Vector4(Elt.Position.X               , Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(0f, 0f) },
                    new structVertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X, Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(1f, 0f) },
                    new structVertex() { Position = new Vector4(Elt.Position.X               , Elt.Position.Y+Elt.Taille.Y, 0f, 1.0f), CoordTextures = new Vector2(0f, 1f) },
                    new structVertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X, Elt.Position.Y+Elt.Taille.Y, 0f, 1.0f), CoordTextures = new Vector2(1f, 1f) },
                    new structVertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X, Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(1f, 0f) }};
                }
                VertexBufferHUD.Lock(0, 0, LockFlags.DoNotWait).WriteRange(Sommets);
                VertexBufferHUD.Unlock();
                Program.device.SetStreamSource(0, VertexBufferHUD, 0, Utilities.SizeOf<structVertex>());
                Program.device.SetTexture(0, Program.Liste_textures[Program.getTexture(Elt.PathTexture)].texture);
                Program.device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                VertexBufferHUD.Dispose();
            }
            Program.device.VertexDeclaration = Program.VertexDeclaration3D;
        }

        public void Dispose()
        {
        }
    }
}
