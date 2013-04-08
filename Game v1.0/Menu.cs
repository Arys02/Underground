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
    static class Menu
    {
        private static SharpDX.Direct3D9.Font font;

        public static bool IsInMenu { get; set; }

        static public void Initialize()
        {
            Program.getTexture(@"Ressources\Game\Images\bg.jpg");

            FontDescription fontDescription = new FontDescription()
            {
                Height = 45,
                Italic = false,
                CharacterSet = FontCharacterSet.Ansi,
                FaceName = "Arial",
                MipLevels = 0,
                OutputPrecision = FontPrecision.TrueType,
                PitchAndFamily = FontPitchAndFamily.Default,
                Quality = FontQuality.ClearType,
                Weight = FontWeight.Bold
            };
            font = new SharpDX.Direct3D9.Font(Program.device, fontDescription);

            IsInMenu = true;
        }

        static public void InMenu()
        {
            int nbvertex = 5;
            VertexBuffer VertexBufferHUD = new VertexBuffer(Program.device, Utilities.SizeOf<structVertex>() * nbvertex, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            /************ LEAK MEMOIRE DANS LE MENU **************/
            VertexElement[] vertexElems2D = new[] {
                new VertexElement(0,0,DeclarationType.Float4,DeclarationMethod.Default,DeclarationUsage.PositionTransformed,0),
                new VertexElement(0,16,DeclarationType.Float2,DeclarationMethod.Default,DeclarationUsage.TextureCoordinate,0),
                VertexElement.VertexDeclarationEnd
            };
            VertexDeclaration VertexDeclaration = new VertexDeclaration(Program.device, vertexElems2D);
            Program.device.VertexDeclaration = VertexDeclaration;
            VertexDeclaration.Dispose();
            VertexBufferHUD.Lock(0, 0, LockFlags.None).WriteRange(new structVertex[] {
                new structVertex() { Position = new Vector4(0                    , 0                    , 0f, 1.0f), CoordTextures = new Vector2(0f, 0f) },
                new structVertex() { Position = new Vector4(Program.resolution[0], 0                    , 0f, 1.0f), CoordTextures = new Vector2(1f, 0f) },
                new structVertex() { Position = new Vector4(0                    , Program.resolution[1], 0f, 1.0f), CoordTextures = new Vector2(0f, 1f) },
                new structVertex() { Position = new Vector4(Program.resolution[0], Program.resolution[1], 0f, 1.0f), CoordTextures = new Vector2(1f, 1f) },
                new structVertex() { Position = new Vector4(Program.resolution[0], 0                    , 0f, 1.0f), CoordTextures = new Vector2(1f, 0f) }});
            VertexBufferHUD.Unlock();

            Program.device.SetTexture(0, Program.Liste_textures[Program.getTexture(@"Ressources\Game\Images\bg.jpg")].texture);
            Program.device.SetStreamSource(0, VertexBufferHUD, 0, Utilities.SizeOf<structVertex>());
            Program.device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

            string displayText = "PLAY !";
            SharpDX.Rectangle fontDimension = font.MeasureText(null, displayText, new SharpDX.Rectangle(0, 0, Program.resolution[0], Program.resolution[1]), SharpDX.Direct3D9.FontDrawFlags.Center | SharpDX.Direct3D9.FontDrawFlags.VerticalCenter);

            // Texte
            font.DrawText(null, displayText, fontDimension, FontDrawFlags.Center | FontDrawFlags.VerticalCenter, Color.Gray);

            float xRes = 2.0f / (Program.resolution[0] - 5);
            float yRes = 2.0f / (Program.resolution[1] - 5);

            if (Program.input.MouseLeft)
            {
                //MousePointer location
                Vector4 mouse = new Vector4(
                    xRes * (Cursor.Position.X - Program.form.Location.X - 5) - 1.0f,
                    1.0f - yRes * (Cursor.Position.Y - Program.form.Location.Y - 5),
                    0.0f, 0.0f);

                Program.WriteNicely("i", 3, "Mouse clicked ! x:" + mouse.X + " & y:" + mouse.Y);

                if (mouse.X > -0.16 && mouse.X < 0.15 && mouse.Y > -0.08 && mouse.Y < 0.04)
                {
                    Program.device.VertexDeclaration = new VertexDeclaration(Program.device, Program.vertexElems3D);
                    IsInMenu = false;
                }
            }
            VertexBufferHUD.Dispose();
        }
        static public void Dispose()
        {
            Program.Liste_textures[Program.getTexture(@"Ressources\Game\Images\bg.jpg")].texture.Dispose();
            font.Dispose();
        }
    }
}
