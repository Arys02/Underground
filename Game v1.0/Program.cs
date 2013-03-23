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
    public struct Vertex
    {
        public Vector4 Position;
        //public Color Color;
        public Vector2 CoordTextures;
        public Vector4 Color;
        public Vector4 Normal;
    }
    public struct Texturestruct
    {
        public string path;
        public Texture texture;
        public Texturestruct(string path, Texture texture)
        {
            this.path = path;
            this.texture = texture;
        }
    }
    public struct mtlstruct
    {
        public string name;
        public Color Ka;
        public Color Kd;
        public Color Ks;
        public string map_Kd;
        public mtlstruct(string name, Color Ka, Color Kd, Color Ks, string map_Kd)
        {
            this.Ka = Ka;
            this.Kd = Kd;
            this.name = name;
            this.Ks = Ks;
            this.map_Kd = map_Kd;
        }
    }
    public struct Model
    {
        public VertexBuffer VertexBuffer;
        public Vertex[] Sommets;
        public int nbfaces;
        public string map_Kd;
        public Model(VertexBuffer VertexBuffer, Vertex[] Sommets, int nbfaces, string map_Kd)
        {
            this.VertexBuffer = VertexBuffer;
            this.Sommets = Sommets;
            this.nbfaces = nbfaces;
            this.map_Kd = map_Kd;
        }
    }
    internal static class Program
    {
        public static Device device;
        public static RenderForm form;
        public static List<Texturestruct> Liste_textures;

        public static Input input;
        [STAThread]
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
            int[] resolution = new int[2] { 1280, 1024 };
            int menu_running = 1;
            // 0 -> jeu en cours
            // 1 -> menu de demarrage
            // 2 -> menu de jeu
            #endregion

            #region Fenêtre
            form = new RenderForm("Game - Soutenance 3");
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
            input = new Input(form);
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

            Liste_textures = new List<Texturestruct>();
            Liste_textures.Add(new Texturestruct("null.bmp", Texture.FromFile(device, "null.bmp")));

            Thread test = new Thread(Sound.main);
            test.Start();

            Menu.Initialize();


            Ingame.ingame();

            device.Dispose();
            direct3D.Dispose();

        }
    }
}
