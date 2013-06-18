using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D9;
using SharpDX.DirectInput;
using SharpDX.Windows;
using Underground;
using Color = SharpDX.Color;
using Device = SharpDX.Direct3D9.Device;
using DeviceType = SharpDX.Direct3D9.DeviceType;
using Effect = SharpDX.Direct3D9.Effect;

namespace Underground
{
    struct Vertex
    {
        public Vector4 Position;
        //public Color Color;
        public Vector2 CoordTextures;
        public Vector4 Color;
        public Vector4 Normal;
    }
    internal static class Program
    {
        public static Device device;
        public static RenderForm form;
        public static Stopwatch clock = new Stopwatch();

        [STAThread]
        //> Color codes for WriteNicely : 
        // 0: Black.
        // 1: DarkBlue.
        // 2: DarkGreen.
        // 3: DarkCyan.
        // 4: DarkRed.
        // 5: DarkMagenta.
        // 6: DarkYellow.
        // 7: Gray.
        // 8: DarkGray.
        // 9: Blue.
        //10: Green.
        //11: Cyan.
        //12: Red.
        //13: Magenta.
        //14: Yellow.
        //15: White.

        public static void WriteNicely(string op, int c, string msg)
        {
            Console.ForegroundColor = (ConsoleColor)c;
            Console.Write("[" + op + "] ");
            Console.ResetColor();
            Console.WriteLine(msg);
        }

        private static void Main()
        {
            #region Variables
            Program.clock.Start();
            int[] resolution = new int[2] { 1280, 1024 };
            int menu_running = 1;
            // 0 -> jeu en cours
            // 1 -> menu de demarrage
            // 2 -> menu de jeu
            #endregion

            #region Fenêtre
            form = new RenderForm("Game - Soutenance 1");
            form.Width = resolution[0];
            form.Height = resolution[1];
            Direct3D direct3D = new Direct3D();
            PresentParameters Parametres = new PresentParameters(
                form.Width,
                form.Height,
                Format.X8R8G8B8,
                1,
                MultisampleType.None,
                0,
                SwapEffect.Discard,
                IntPtr.Zero,
                true,
                true,
                Format.D24X8,
                PresentFlags.None,
                0,
                PresentInterval.Immediate);
            Input input = new Input(form);
            device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, Parametres);
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
            VertexDeclaration vertexDecl = new VertexDeclaration(device, vertexElems);
            device.VertexDeclaration = vertexDecl;
            #endregion
            

            #region Menu, fonts
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
            SharpDX.Direct3D9.Font font = new SharpDX.Direct3D9.Font(device, fontDescription);
            string displayText = "PLAY !";
            SharpDX.Rectangle fontDimension = font.MeasureText(null, displayText, new SharpDX.Rectangle(0, 0, form.ClientSize.Width, form.ClientSize.Height), SharpDX.Direct3D9.FontDrawFlags.Center | SharpDX.Direct3D9.FontDrawFlags.VerticalCenter);
            SharpDX.Rectangle fontPauseDimension = font.MeasureText(null, "PAUSE", new SharpDX.Rectangle(0, 0, form.ClientSize.Width, form.ClientSize.Height), SharpDX.Direct3D9.FontDrawFlags.Top | SharpDX.Direct3D9.FontDrawFlags.Right);

            // Image

            int nbvertex = 5;
            VertexBuffer vertices_img = new VertexBuffer(device, Utilities.SizeOf<Vertex>() * nbvertex, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            Console.WriteLine(Utilities.SizeOf<Vertex>());
            // ******* ICI ****** /
            Texture Texture_background_menu = SharpDX.Direct3D9.Texture.FromFile(device, @"Ressources\Menu\bg.jpg");

            #endregion

            //Menu.menu();
            Thread test = new Thread(Sound.main);
            test.Start();
            Ingame.ingame();
            clock.Stop();

            device.Dispose();
            direct3D.Dispose();
        }
    }
}
