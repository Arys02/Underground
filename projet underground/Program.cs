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

        public static Input input;
        [STAThread]

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

            #region Menu, fonts

            #endregion

            Thread test = new Thread(Sound.main);
            test.Start();

            Menu.Initialize();

            Ingame.ingame();

            device.Dispose();
            direct3D.Dispose();

        }
    }
}
