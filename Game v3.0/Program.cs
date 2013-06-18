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
    #region structures
    public struct structModelFiles
    {
        public byte[] data;
        public Matrix Transformation;
        public structModelFiles(byte[] data, Matrix Transformation)
        {
            this.data = data;
            this.Transformation = Transformation;
        }
    }
    public struct structVertex
    {
        public Vector4 Position;
        //public Color Color;
        public Vector2 CoordTextures;
        public Vector4 Color;
        public Vector4 Normal;
    }
    public struct structTexture
    {
        public string path;
        public Texture texture;
        public structTexture(string path, Texture texture)
        {
            this.path = path;
            this.texture = texture;
        }
    }
    public struct structBinary
    {
        public string path;
        public Byte[] data;
        public structBinary(string path, Byte[] data)
        {
            this.path = path;
            this.data = data;
        }
    }
    public struct structMTL
    {
        public string name;
        public Color Ka;
        public Color Kd;
        public Color Ks;
        public string map_Kd;
        public string map_Ns;
        public structMTL(string name, Color Ka, Color Kd, Color Ks, string map_Kd, string map_Ns)
        {
            this.Ka = Ka;
            this.Kd = Kd;
            this.name = name;
            this.Ks = Ks;
            this.map_Kd = map_Kd;
            this.map_Ns = map_Ns;
        }
    }
    public struct structModel
    {
        public VertexBuffer VertexBuffer;
        public structVertex[] Sommets;
        public int nbfaces;
        public string map_Kd;
        public string map_Ns;
        public bool isready;

        public structModel(VertexBuffer VertexBuffer, structVertex[] Sommets, int nbfaces, string map_Kd, string map_Ns)
        {
            this.VertexBuffer = VertexBuffer;
            this.Sommets = Sommets;
            this.nbfaces = nbfaces;
            this.map_Kd = map_Kd;
            this.map_Ns = map_Ns;
            this.isready = true;
        }
    }
    public struct structOBJ
    {
        public string path;
        public Matrix Transformation;
        public List<structModel> data;
        public Point IDTile;
        public structOBJ(string path, Matrix Transformation, List<structModel> data, Point IDTile)
        {
            this.IDTile = IDTile;
            this.data = data;
            this.path = path;
            this.Transformation = Transformation;
        }
    }
    #endregion


    internal static class Program
    {
        public static Device device;
        public static RenderForm form;
        public static List<structTexture> Liste_textures;
        public static List<structBinary> Liste_binaires;
        public static List<structOBJ> Liste_OBJ;
        public static int[] resolution = new int[2] { 1280, 700 };
        public static VertexElement[] vertexElems3D;
        public static VertexElement[] vertexElems2D;
        public static VertexDeclaration VertexDeclaration3D;
        public static VertexDeclaration VertexDeclaration2D;
        public static VertexBuffer VertexBufferZero;
        public static Maze newmaze;

        public static Input input;
        [STAThread]
        public static void WriteNicely(string op, int c, string msg)
        {
            bool debug = false;
            if (debug)
            {
                Console.ForegroundColor = (ConsoleColor)c;
                Console.Write("[" + op + "] ");
                Console.ResetColor();
                Console.WriteLine(msg);
            }
        }

        ///////////////////////////////////////////
        // Ajoute une texture dans Liste_textures en gardant en mémoire bah, la texture et le chemin de la texture.
        // Si la texture est déjà présente dans la liste, inutile de la réajouter.
        // Retourne la position de la texture
        public static int getTexture(string filename)
        {
            for (int i = 0; i < Liste_textures.Count; i++)
            {
                if (Liste_textures[i].path == filename)
                {
                    return i;
                }
            }
            Liste_textures.Add(new structTexture(filename, Texture.FromFile(device, filename)));
            return Liste_textures.Count - 1;
        }

        public static int getBinary(string filename)
        {
            for (int i = 0; i < Liste_binaires.Count; i++)
            {
                if (Liste_binaires[i].path == filename)
                {
                    return i;
                }
            }
            Liste_binaires.Add(new structBinary(filename, File.ReadAllBytes(filename)));
            return Liste_binaires.Count - 1;
        }

        public static int getModel(string filename, Matrix Transformation, Point IDTile)
        {
            for (int i = 0; i < Liste_OBJ.Count; i++)
            {
                if (Liste_OBJ[i].path == filename && Liste_OBJ[i].Transformation == Transformation)
                {
                    return i;
                }
            }
            Ingame.last_unready = true;
            Liste_OBJ.Add(new structOBJ(filename, Transformation, ObjLoader.read_obj(Liste_binaires[getBinary(filename)].data, Transformation), IDTile));
            for (int i = 0; i < Liste_OBJ[Liste_OBJ.Count - 1].data.Count; i++)
            {
                Liste_OBJ[Liste_OBJ.Count - 1].data[i].VertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(Liste_OBJ[Liste_OBJ.Count - 1].data[i].Sommets);
                Liste_OBJ[Liste_OBJ.Count - 1].data[i].VertexBuffer.Unlock();

            }
            Ingame.last_unready = false;
            Collision.Initialize();
            return Liste_OBJ.Count - 1;
        }

        public static void freeModel(Point IDTile, bool supprimer_toutes_les_occurences)
        {
            for (int i = 0; i < Liste_OBJ.Count; i++)
            {
                if (Liste_OBJ[i].IDTile == IDTile)
                {
                    for (int j = 0; j < Liste_OBJ[i].data.Count; j++)
                    {
                        Liste_OBJ[i].data[j].VertexBuffer.Dispose();
                    }
                    Liste_OBJ.RemoveAt(i);
                    i--;
                    if (!supprimer_toutes_les_occurences) return;
                }
            }
        }

        private static void Main(string[] args)
        {
            #region Variables
            if (args.Length == 3 && args[0] == "-r" && int.TryParse(args[1], out resolution[0]) &&
                int.TryParse(args[2], out resolution[1])) { }
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
            vertexElems3D = new[] {
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
            vertexElems2D = new[] {
                new VertexElement(0,0,DeclarationType.Float4,DeclarationMethod.Default,DeclarationUsage.PositionTransformed,0),
                new VertexElement(0,16,DeclarationType.Float2,DeclarationMethod.Default,DeclarationUsage.TextureCoordinate,0),
                VertexElement.VertexDeclarationEnd
            };

            VertexDeclaration3D = new VertexDeclaration(Program.device, Program.vertexElems3D);
            VertexDeclaration2D = new VertexDeclaration(Program.device, Program.vertexElems2D);
            VertexBufferZero = new VertexBuffer(IntPtr.Zero);

            device.VertexDeclaration = VertexDeclaration3D;

            Program.device.SetRenderState(RenderState.AlphaBlendEnable, true); // graphics.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            Program.device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha); // graphics.GraphicsDevice.RenderState.DestinationBlend = Blend.One;
            Program.device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha); // graphics.GraphicsDevice.RenderState.SourceBlend = Blend.One;
            Program.device.SetRenderState(RenderState.AlphaFunc, BlendOperation.Maximum); // graphics.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add;
            Program.device.SetRenderState(RenderState.AlphaTestEnable, true);
            Program.device.SetRenderState(RenderState.MinTessellationLevel, 8);
            #endregion

            Liste_textures = new List<structTexture>();
            Liste_binaires = new List<structBinary>();
            Liste_OBJ = new List<structOBJ>();
            Liste_textures.Add(new structTexture("null.bmp", Texture.FromFile(device, "null.bmp")));
            Ingame.Slender.doit_etre_recharge = true;
            Thread ThSound = new Thread(Sound.main);
            Thread ThEvents = new Thread(Ingame.fevents);
            ThSound.Start();
            ThEvents.Start();

            Menu.Initialize_Menu();
            ObjLoader.Initialize();
            newmaze = new Maze(10, 10);
            newmaze.Initecelles();
            newmaze.Generate(newmaze.maze[0, 0]);
            newmaze.Setaffichage();

            Ingame.ingame();

            Menu.Dispose();
            ThSound.Abort();
            ThEvents.Abort();
            VertexBufferZero.Dispose();
            VertexDeclaration3D.Dispose();
            VertexDeclaration2D.Dispose();
            device.Dispose();
            direct3D.Dispose();

        }
    }
}
