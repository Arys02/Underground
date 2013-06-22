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
    static class Extension
    {
        public static void Rapproche(this float ToGo, ref float Origin)
        {
            float diff = ToGo - Origin;

            if (diff < 8 && diff > -8)
                Origin = ToGo;
            else if (diff > 0)
                Origin += 8;
            else if (diff < 0)
                Origin -= 8;
        }
    }

    static class Menu
    {
        private static EltInfo[] _menuTabMain,
                                 _menuTabSettings,
                                 _menuTabExtra,
                                 _pauseTab,
                                 _tabTmp;

        private static EltInfo[] _switchTo;

        private static void Switcher()
        {
            if (_switchTo != _tabTmp)
            {
                for (int index = 0; index < _tabTmp.Length; index++)
                {
                    if (_tabTmp[index].EltType == EltType.Button || _tabTmp[index].EltType == EltType.Info)
                    {
                        _switchTo[index].Position.X.Rapproche(ref _tabTmp[index].Position.X);
                        _switchTo[index].Position.Y.Rapproche(ref _tabTmp[index].Position.Y);
                    }

                    _tabTmp[11].Text = Get_Volume();
                }
            }
        }

        enum EltType
        {
            Default = 0,
            Button = 1,
            Info = 2
        }

        struct EltInfo
        {
            public EltInfo(Vector2 Position, Vector2 Taille, string PathTexture, EltType EltType, string Text)
            {
                this.Position = Position;
                this.Taille = Taille;
                this.PathTexture = PathTexture;
                this.EltType = EltType;
                this.Text = Text;
            }

            public Vector2 Position;
            public Vector2 Taille;
            public string PathTexture;
            public EltType EltType;
            public String Text;
        }

        private static SharpDX.Direct3D9.Font font;

        public static bool IsInMenu { get; set; }
        public static bool IsInGame { get; set; }

        static public void Initialize_Menu()
        {
            var fontDescription = new FontDescription()
            {
                Height = 40,
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

            _menuTabMain = new EltInfo[]
                {
                    new EltInfo(Vector2.Zero, new Vector2(Program.resolution[0], Program.resolution[1]),
                                @"Ressources\Game\Images\bg.jpg", EltType.Default, String.Empty),
                    new EltInfo(new Vector2(0, 3*Program.resolution[1]/5), 
                                new Vector2(Program.resolution[0], 2*Program.resolution[1]/5),
                                @"Ressources\Menu\smoke2.png", EltType.Default, String.Empty),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1]/2 - 100),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Play"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1]/2),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Settings"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1]/2 + 100),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Exit"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1] + 200),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Resolution"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1] + 200),
                                new Vector2(50, 50), @"Ressources\HUD\button.png", EltType.Button, "-"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1] + 200),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Info, Program.resolution[0] + " x " + Program.resolution[1]),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1] + 200),
                                new Vector2(50, 50), @"Ressources\HUD\button.png", EltType.Button, "+"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1] + 300),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Volume"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1] + 300),
                                new Vector2(50, 50), @"Ressources\HUD\button.png", EltType.Button, "- "),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1] + 300),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Info, Get_Volume()),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1] + 300),
                                new Vector2(50, 50), @"Ressources\HUD\button.png", EltType.Button, "+ "),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 140, Program.resolution[1] + 400),
                                new Vector2(120, 50), @"Ressources\HUD\button.png", EltType.Button, "Back")
                };

            _menuTabSettings = new EltInfo[]
                {
                    new EltInfo(Vector2.Zero, new Vector2(Program.resolution[0], Program.resolution[1]),
                                @"Ressources\Game\Images\bg.jpg", EltType.Default, String.Empty),
                    new EltInfo(new Vector2(0, 3*Program.resolution[1]/5), 
                                new Vector2(Program.resolution[0], 2*Program.resolution[1]/5),
                                @"Ressources\Menu\smoke1.png", EltType.Default, String.Empty),
                    new EltInfo(new Vector2(Program.resolution[0] + 200, Program.resolution[1]/2 - 100),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Play"),         
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1]/2 - 100),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Settings"),
                    new EltInfo(new Vector2(Program.resolution[0] + 200, Program.resolution[1]/2 + 100),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Exit"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1]/2 ),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Info, "Resolution"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 + 150, Program.resolution[1]/2),
                                new Vector2(50, 50), @"Ressources\HUD\button.png", EltType.Button, "-"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 + 210, Program.resolution[1] /2),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Info, Program.resolution[0] + " x " + Program.resolution[1]),
                    new EltInfo(new Vector2(Program.resolution[0]/2 + 460, Program.resolution[1] /2),
                                new Vector2(50, 50), @"Ressources\HUD\button.png", EltType.Button, "+"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1]/2 + 100),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Volume"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 + 150, Program.resolution[1]/2 + 100),
                                new Vector2(50, 50), @"Ressources\HUD\button.png", EltType.Button, "- "),
                    new EltInfo(new Vector2(Program.resolution[0]/2 + 210, Program.resolution[1]/2 + 100),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Info, Get_Volume()),
                    new EltInfo(new Vector2(Program.resolution[0]/2 +460, Program.resolution[1]/2 + 100),
                                new Vector2(50, 50), @"Ressources\HUD\button.png", EltType.Button, "+ "),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 140, Program.resolution[1]/2 + 200),
                                new Vector2(120, 50), @"Ressources\HUD\button.png", EltType.Button, "Back")
                };

            _menuTabExtra = new EltInfo[]
                {
                    new EltInfo(Vector2.Zero, new Vector2(Program.resolution[0], Program.resolution[1]),
                                @"Ressources\Game\Images\bg.jpg", EltType.Default, String.Empty),
                    new EltInfo(new Vector2(0, 3*Program.resolution[1]/5), 
                                new Vector2(Program.resolution[0], 2*Program.resolution[1]/5),
                                @"Ressources\Menu\smoke1.png", EltType.Default, String.Empty),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1]/2 - 100),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Play"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1]/2),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Settings"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1]/2 + 100),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Exit"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1] + 200),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Resolution"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1] + 200),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "-"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1] + 200),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, Program.resolution[0] + " x " + Program.resolution[1]),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1] + 200),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "+"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 140, Program.resolution[1] + 200),
                                new Vector2(120, 50), @"Ressources\HUD\button.png", EltType.Button, "Back")
                };


            _pauseTab = new EltInfo[]
                {
                    new EltInfo(new Vector2(0, 3*Program.resolution[1]/5), 
                                new Vector2(Program.resolution[0], 2*Program.resolution[1]/5),
                                @"Ressources\Menu\smoke2.png", EltType.Default, String.Empty),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1]/2 - 100),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Resume"),
                    new EltInfo(new Vector2(Program.resolution[0]/2 - 120, Program.resolution[1]/2 + 100),
                                new Vector2(240, 50), @"Ressources\HUD\button.png", EltType.Button, "Exit")
                };

            IsInMenu = true;

            _switchTo = _tabTmp = new EltInfo[_menuTabMain.Length];

            Array.Copy(_menuTabMain, _switchTo, _menuTabMain.Length);
            Array.Copy(_menuTabMain, _tabTmp, _menuTabMain.Length);
        }


        private static string Get_Volume()
        {
            string r = String.Empty;
            for (int i = 0; i < Sound.mastervoice.Volume; i++)
            {
                r += "| ";
            }
            return r;
        }

        static public void InMenu()
        {
            VertexBuffer VertexBufferMenu;
            Program.device.VertexDeclaration = Program.VertexDeclaration2D;

            if (IsInGame)
            {
                _switchTo = _pauseTab;
                _tabTmp = _pauseTab;
            }

            Switcher();

            string hoveringButton = string.Empty;

            foreach (EltInfo Elt in _tabTmp)
            {
                VertexBufferMenu = new VertexBuffer(Program.device,
                    (
                        Utilities.SizeOf<structVertex>()
                    ) * 3 * 2, Usage.WriteOnly, VertexFormat.None, Pool.Managed);

                var Sommets = new structVertex[] {
                    new structVertex() { Position = new Vector4(Elt.Position.X               , Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(0f, 0f) },
                    new structVertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X, Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(1f, 0f) },
                    new structVertex() { Position = new Vector4(Elt.Position.X               , Elt.Position.Y+Elt.Taille.Y, 0f, 1.0f), CoordTextures = new Vector2(0f, 1f) },
                    new structVertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X, Elt.Position.Y+Elt.Taille.Y, 0f, 1.0f), CoordTextures = new Vector2(1f, 1f) },
                    new structVertex() { Position = new Vector4(Elt.Position.X + Elt.Taille.X, Elt.Position.Y             , 0f, 1.0f), CoordTextures = new Vector2(1f, 0f) }};
                VertexBufferMenu.Lock(0, 0, LockFlags.None).WriteRange(Sommets);
                VertexBufferMenu.Unlock();
                Program.device.SetStreamSource(0, VertexBufferMenu, 0, Utilities.SizeOf<structVertex>());

                if (Elt.EltType == EltType.Button &&
                       Program.input.MousePoint.X > Elt.Position.X && Program.input.MousePoint.X < Elt.Position.X + Elt.Taille.X &&
                       Program.input.MousePoint.Y > Elt.Position.Y && Program.input.MousePoint.Y < Elt.Position.Y + Elt.Taille.Y)
                {
                    Program.device.SetTexture(0, Program.Liste_textures[Program.getTexture(@"Ressources\HUD\button_hover.png")].texture);
                    hoveringButton = Elt.Text;
                }
                else
                {
                    Program.device.SetTexture(0, Program.Liste_textures[Program.getTexture(Elt.PathTexture)].texture);
                }
                Program.device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                VertexBufferMenu.Dispose();

                if (Elt.Text != String.Empty)
                {
                    font.DrawText(null, Elt.Text, (int)Elt.Position.X + 15, (int)Elt.Position.Y + 5, Color.Gray);
                }
            }

            if (Program.input.MouseLeft && hoveringButton != String.Empty)
            {
                switch (hoveringButton)
                {
                    case "Play":
                        Program.device.VertexDeclaration = new VertexDeclaration(Program.device, Program.vertexElems3D);
                        IsInMenu = false;
                        IsInGame = true;
                        break;
                    case "Settings":
                        _switchTo = _menuTabSettings;
                        break;
                    case "Back":
                        _switchTo = _menuTabMain;
                        break;
                    case "-":
                        Process.Start(Application.ExecutablePath, "-r 800 600");

                        Program.device.Dispose();
                        Program.form.Dispose();
                        Menu.Dispose();

                        Process.GetCurrentProcess().Kill();
                        break;
                    case "+":
                        Process.Start(Application.ExecutablePath, "-r 1366 768");

                        Program.device.Dispose();
                        Program.form.Dispose();
                        Menu.Dispose();                        

                        Process.GetCurrentProcess().Kill();
                        break;
                    case "+ ":
                        Sound.mastervoice.SetVolume((Sound.mastervoice.Volume < 12)
                                                        ? Sound.mastervoice.Volume + 1
                                                        : Sound.mastervoice.Volume);
                        _tabTmp[11].Text = Get_Volume();
                        break;
                    case "- ":
                        Sound.mastervoice.SetVolume((Sound.mastervoice.Volume > 0)
                                                        ? Sound.mastervoice.Volume - 1
                                                        : Sound.mastervoice.Volume);
                        _tabTmp[11].Text = Get_Volume();
                        break;
                    case "Resume":
                        Program.device.VertexDeclaration = new VertexDeclaration(Program.device, Program.vertexElems3D);
                        IsInMenu = false;
                        IsInGame = true;
                        break;
                    case "Exit":
                        Program.device.Dispose();
                        Program.form.Dispose();
                        Menu.Dispose();
                        Process.GetCurrentProcess().Kill();
                        break;
                }
            }
            Program.device.VertexDeclaration = Program.VertexDeclaration3D;
        }
        static public void Dispose()
        {
            Program.Liste_textures[Program.getTexture(@"Ressources\Game\Images\bg.jpg")].texture.Dispose();
            font.Dispose();
        }
    }
}
