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
        private static Texture Texture_background_menu;
        private static VertexBuffer vertices_img;
        private static SharpDX.Direct3D9.Font font;

        public static bool IsInMenu { get; set; }

        static public void Initialize()
        {
            // Image
            int nbvertex = 5;
            vertices_img = new VertexBuffer(Program.device, Utilities.SizeOf<Vertex>() * nbvertex, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            Console.WriteLine(Utilities.SizeOf<Vertex>());
            // ******* ICI ****** /
            Texture_background_menu = SharpDX.Direct3D9.Texture.FromFile(Program.device, @"Ressources\Game\Images\bg.jpg");

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
            int w = 1280, h = 1024;
            for (int y = 0; y < 1; y++)
            {
                for (int x = 0; x < 1; x++)
                {
                    vertices_img.Lock(0, 0, LockFlags.None).WriteRange(new Vertex[] {
                                                new Vertex() { Position = new Vector4(x * w, y * h, 0f, 1.0f), CoordTextures = new Vector2(0f, 0f) },
                                                new Vertex() { Position = new Vector4(x * w + w, y * h, 0f, 1.0f), CoordTextures = new Vector2(1f, 0f) },
                                                new Vertex() { Position = new Vector4(x * w, y * h + h, 0f, 1.0f), CoordTextures = new Vector2(0f, 1f) },
                                                new Vertex() { Position = new Vector4(x * w + w, y * h + h, 0f, 1.0f), CoordTextures = new Vector2(1f, 1f) },
                                                new Vertex() { Position = new Vector4(x * w + w, y * h, 0f, 1.0f), CoordTextures = new Vector2(1f, 0f) }});
                    vertices_img.Unlock();
                    VertexElement[] VertexElemsImg = new[] {
                                                new VertexElement(0,0,DeclarationType.Float4,DeclarationMethod.Default,DeclarationUsage.PositionTransformed,0),
                                                new VertexElement(0,16,DeclarationType.Float2,DeclarationMethod.Default,DeclarationUsage.TextureCoordinate,0),
                                                VertexElement.VertexDeclarationEnd
                                            };

                    VertexDeclaration VertexDec3 = new VertexDeclaration(Program.device, VertexElemsImg);
                    Program.device.SetStreamSource(0, vertices_img, 0, Utilities.SizeOf<Vertex>() * 5);
                    Program.device.VertexDeclaration = VertexDec3;
                    Program.device.SetTexture(0, Texture_background_menu);
                    Program.device.SetStreamSource(0, vertices_img, 0, Utilities.SizeOf<Vertex>());
                    Program.device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                }
            }

            string displayText = "PLAY !";
            SharpDX.Rectangle fontDimension = font.MeasureText(null, displayText, new SharpDX.Rectangle(0, 0, w, h), SharpDX.Direct3D9.FontDrawFlags.Center | SharpDX.Direct3D9.FontDrawFlags.VerticalCenter);

            // Texte
            font.DrawText(null, displayText, fontDimension, FontDrawFlags.Center | FontDrawFlags.VerticalCenter, Color.Gray);

            float xRes = 2.0f / (w - 5);
            float yRes = 2.0f / (h - 5);

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
                    Texture_background_menu.Dispose();
                    vertices_img.Dispose();
                    font.Dispose();

                    VertexElement[] vertexElems = new[] {
        		        new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				        new VertexElement(0,
                            Convert.ToInt16(Utilities.SizeOf<Vector4>()),
                            DeclarationType.Float2, DeclarationMethod.Default,DeclarationUsage.TextureCoordinate,0),
                        new VertexElement(0,
                            Convert.ToInt16(Utilities.SizeOf<Vector4>()+Utilities.SizeOf<Vector2>()),
                            DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),

                        // NORMAL
                        new VertexElement(0,
                            Convert.ToInt16(Utilities.SizeOf<Vector4>()+Utilities.SizeOf<Vector4>()+Utilities.SizeOf<Vector2>()),
                            DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                        VertexElement.VertexDeclarationEnd
        	        };
                    VertexDeclaration vertexDecl = new VertexDeclaration(Program.device, vertexElems);
                    Program.device.VertexDeclaration = vertexDecl;
                    IsInMenu = false;
                }
            }



        }
    }
}
