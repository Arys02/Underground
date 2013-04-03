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
        public EltInfo(Vector2 Position, Vector2 Taille, Texture Texture, VertexBuffer VertexBuffer, EltType EltType)
        {
            this.Position = Position;
            this.Taille = Taille;
            this.Texture = Texture;
            this.VertexBuffer = VertexBuffer;
            this.EltType = EltType;
        }
        public Vector2 Position;
        public Vector2 Taille;
        public Texture Texture;
        public VertexBuffer VertexBuffer;
        public EltType EltType;
    }
    class HUD
    {
        //private static Texture Texture_Progressbar_Eye;
        private static VertexBuffer VertexBufferHUD;
        private List<EltInfo> Liste_Elt;
        public HUD()
        {
            VertexBufferHUD = new VertexBuffer(Program.device, (Utilities.SizeOf<Vector4>() + Utilities.SizeOf<Vector2>()) * 2, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            Liste_Elt = new List<EltInfo>();
            Liste_Elt.Add(new EltInfo(Vector2.Zero, new Vector2(Program.resolution[0],Program.resolution[1]), Texture.FromFile(Program.device, @"Ressources\HUD\Torch.png"), VertexBufferHUD, EltType.Default));
            Liste_Elt.Add(new EltInfo(new Vector2(0, Program.resolution[1] - 200), new Vector2(400, 200), Texture.FromFile(Program.device, @"Ressources\HUD\Texture.png"), VertexBufferHUD, EltType.Default));
            Liste_Elt.Add(new EltInfo(new Vector2(42, Program.resolution[1] - 102), new Vector2(300, 30), Texture.FromFile(Program.device, @"Ressources\HUD\ProgressBar1.png"), VertexBufferHUD, EltType.EyeBar));
        }

        public void Display_HUD()
        {
            VertexElement[] vertexElems2D = new[] {
                new VertexElement(0,0,DeclarationType.Float4,DeclarationMethod.Default,DeclarationUsage.PositionTransformed,0),
                new VertexElement(0,16,DeclarationType.Float2,DeclarationMethod.Default,DeclarationUsage.TextureCoordinate,0),
                VertexElement.VertexDeclarationEnd
            };
            Program.device.VertexDeclaration = new VertexDeclaration(Program.device, vertexElems2D);

            foreach (EltInfo Elt in Liste_Elt)
            {
                if (Elt.EltType == EltType.EyeBar) // Barre de progression des yeux
                {
                    Elt.VertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(new Vertex[] {
                    new Vertex() { Position = new Vector4(Elt.Position.X                                , Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(0f, 0f) },
                    new Vertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X * Ingame.percent, Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(1f, 0f) },
                    new Vertex() { Position = new Vector4(Elt.Position.X                                , Elt.Position.Y+Elt.Taille.Y, 0f, 1.0f), CoordTextures = new Vector2(0f, 1f) },
                    new Vertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X * Ingame.percent, Elt.Position.Y+Elt.Taille.Y, 0f, 1.0f), CoordTextures = new Vector2(1f, 1f) },
                    new Vertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X * Ingame.percent, Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(1f, 0f) }});
                    Elt.VertexBuffer.Unlock();
                }
                else // Par défaut
                {
                    Elt.VertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(new Vertex[] {
                    new Vertex() { Position = new Vector4(Elt.Position.X               , Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(0f, 0f) },
                    new Vertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X, Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(1f, 0f) },
                    new Vertex() { Position = new Vector4(Elt.Position.X               , Elt.Position.Y+Elt.Taille.Y, 0f, 1.0f), CoordTextures = new Vector2(0f, 1f) },
                    new Vertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X, Elt.Position.Y+Elt.Taille.Y, 0f, 1.0f), CoordTextures = new Vector2(1f, 1f) },
                    new Vertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X, Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(1f, 0f) }});
                    Elt.VertexBuffer.Unlock();
                }
                Program.device.SetStreamSource(0, Elt.VertexBuffer, 0, Utilities.SizeOf<Vertex>());
                Program.device.SetTexture(0, Elt.Texture);
                Program.device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            }
            Program.device.VertexDeclaration = new VertexDeclaration(Program.device, Program.vertexElems3D);
        }

        public void Dispose()
        {
            foreach (EltInfo Elt in Liste_Elt)
            {
                Elt.Texture.Dispose();
                Elt.VertexBuffer.Dispose();
            }
        }
    }
}
